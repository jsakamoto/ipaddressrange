﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net.Sockets;

#if NET45
using System.Runtime.Serialization;
#endif

namespace NetTools
{
    // NOTE: Why implement IReadOnlyDictionary<TKey,TVal> interface? 
    // =============================================================
    // Problem
    // ----------
    // An IPAddressRange after v.1.4 object cann't serialize to/deserialize from JSON text by using JSON.NET.
    //
    // Details
    // ----------
    // JSON.NET detect IEnumerable<IPAddress> interface prior to ISerializable. 
    // At a result, JSON.NET try to serialize IPAddressRange as array, such as "["192.168.0.1", "192.168.0.2"]".
    // This is unexpected behavior. (We expect "{"Begin":"192.168.0.1", "End:"192.168.0.2"}" style JSON text that is same with DataContractJsonSerializer.)
    // In addition, JSON serialization with JSON.NET crash due to IPAddress cann't serialize by JSON.NET.
    //
    // Work around
    // -----------
    // To avoid this JSON.NET behavior, IPAddressRange should implement more high priority interface than IEnumerable<T> in JSON.NET.
    // Such interfaces include the following.
    // - IDictionary
    // - IDictionary<TKey,TVal>
    // - IReadOnlyDictionary<TKey,TVal>
    // But, when IPAddressRange implement IDictionay or IDictionary<TKey,TVal>, serialization by DataContractJsonSerializer was broken.
    // (Implementation of DataContractJsonSerializer is special for IDictionay and IDictionary<TKey,TVal>)
    // 
    // So there is no way without implement IReadOnlyDictionary<TKey,TVal>.
    //
    // Trade off
    // -------------
    // IReadOnlyDictionary<TKey,TVal> interface doesn't exist in .NET Framework v.4.0 or before.
    // In order to give priority to supporting serialization by JSON.NET, I had to truncate the support for .NET Framework 4.0.
    // (.NET Standard 1.4 support IReadOnlyDictionary<TKey,TVal>, therefore there is no problem on .NET Core appliction.)
    // 
    // Binary level compatiblity
    // -------------------------
    // There is no problem even if IPAddressRange.dll is replaced with the latest version.
    // 
    // Source code level compatiblity
    // -------------------------
    // You cann't apply LINQ extension methods directory to IPAddressRange object.
    // Because IPAddressRange implement two types of IEnumerable<T> (IEnumerable<IPaddress> and IEnumerable<KeyValuePair<K,V>>).
    // It cause ambiguous syntax error.
    // To avoid this error, you should use "AsEnumerable()" method before IEnumerable<IPAddressRange> access.

#if NET45
    [Serializable]
    public class IPAddressRange : ISerializable, IEnumerable<IPAddress>, IReadOnlyDictionary<string, string>
#else
    public class IPAddressRange : IEnumerable<IPAddress>, IReadOnlyDictionary<string, string>
#endif
    {
        // Pattern 1. CIDR range: "192.168.0.0/24", "fe80::%lo0/10"
        private static Regex m1_regex = new Regex(@"^(?<adr>([\d.]+)|([\da-f:]+(:[\d.]+)?(%\w+)?))[ \t]*/[ \t]*(?<maskLen>\d+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Pattern 2. Uni address: "127.0.0.1", "::1%eth0"
        private static Regex m2_regex = new Regex(@"^(?<adr>([\d.]+)|([\da-f:]+(:[\d.]+)?(%\w+)?))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Pattern 3. Begin end range: "169.254.0.0-169.254.0.255", "fe80::1%23-fe80::ff%23"
        //            also shortcut notation: "192.168.1.1-7" (IPv4 only)
        private static Regex m3_regex = new Regex(@"^(?<begin>([\d.]+)|([\da-f:]+(:[\d.]+)?(%\w+)?))[ \t]*[\-–][ \t]*(?<end>([\d.]+)|([\da-f:]+(:[\d.]+)?(%\w+)?))$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Pattern 4. Bit mask range: "192.168.0.0/255.255.255.0"
        private static Regex m4_regex = new Regex(@"^(?<adr>([\d.]+)|([\da-f:]+(:[\d.]+)?(%\w+)?))[ \t]*/[ \t]*(?<bitmask>[\da-f\.:]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public IPAddress Begin { get; set; }

        public IPAddress End { get; set; }

        /// <summary>
        /// Creates an empty range object, equivalent to "0.0.0.0/0".
        /// </summary>
        public IPAddressRange() : this(new IPAddress(0L)) { }

        /// <summary>
        /// Creates a new range with the same start/end address (range of one)
        /// </summary>
        /// <param name="singleAddress"></param>
        public IPAddressRange(IPAddress singleAddress)
        {
            if (singleAddress == null)
                throw new ArgumentNullException(nameof(singleAddress));

            Begin = End = singleAddress;
        }

        /// <summary>
        /// Create a new range from a begin and end address.
        /// Throws an exception if Begin comes after End, or the
        /// addresses are not in the same family.
        /// </summary>
        public IPAddressRange(IPAddress begin, IPAddress end)
        {
            if (begin == null)
                throw new ArgumentNullException(nameof(begin));

            if (end == null)
                throw new ArgumentNullException(nameof(end));

            var beginBytes = begin.GetAddressBytes();
            var endBytes = end.GetAddressBytes();
            Begin = new IPAddress(beginBytes);
            End = new IPAddress(endBytes);

            if (Begin.AddressFamily != End.AddressFamily) throw new ArgumentException("Elements must be of the same address family", nameof(end));

            if (!Bits.GtECore(endBytes, beginBytes)) throw new ArgumentException("Begin must be smaller than the End", nameof(begin));
        }

        /// <summary>
        /// Creates a range from a base address and mask bits.
        /// This can also be used with <see cref="SubnetMaskLength"/> to create a
        /// range based on a subnet mask.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="maskLength"></param>
        public IPAddressRange(IPAddress baseAddress, int maskLength)
        {
            if (baseAddress == null)
                throw new ArgumentNullException(nameof(baseAddress));

            var baseAdrBytes = baseAddress.GetAddressBytes();
            if (baseAdrBytes.Length * 8 < maskLength) throw new FormatException();
            var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, maskLength);
            baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);

            Begin = new IPAddress(baseAdrBytes);
            End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use IPAddressRange.Parse static method instead.")]
        public IPAddressRange(string ipRangeString)
        {
            var parsed = Parse(ipRangeString);
            Begin = parsed.Begin;
            End = parsed.End;
        }

#if NET45
        protected IPAddressRange(SerializationInfo info, StreamingContext context)
        {
            var names = new List<string>();
            foreach (var item in info) names.Add(item.Name);

            Func<string, IPAddress> deserialize = (name) => names.Contains(name) ?
                 IPAddress.Parse(info.GetValue(name, typeof(object)).ToString()) :
                 new IPAddress(0L);

            this.Begin = deserialize("Begin");
            this.End = deserialize("End");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue("Begin", this.Begin != null ? this.Begin.ToString() : "");
            info.AddValue("End", this.End != null ? this.End.ToString() : "");
        }
#endif

        public bool Contains(IPAddress ipaddress)
        {
            if (ipaddress == null)
                throw new ArgumentNullException(nameof(ipaddress));

            if (ipaddress.AddressFamily != this.Begin.AddressFamily) return false;

            var offset = 0;
            if (Begin.IsIPv4MappedToIPv6 && ipaddress.IsIPv4MappedToIPv6)
            {
                offset = 12; //ipv4 has prefix of 10 zero bytes and two 255 bytes. 
            }

            var adrBytes = ipaddress.GetAddressBytes();
            return Bits.LtECore(this.Begin.GetAddressBytes(), adrBytes, offset) && Bits.GtECore(this.End.GetAddressBytes(), adrBytes, offset);
        }

        public bool Contains(IPAddressRange range)
        {
            if (range == null)
                throw new ArgumentNullException(nameof(range));

            if (this.Begin.AddressFamily != range.Begin.AddressFamily) return false;

            var offset = 0;
            if (Begin.IsIPv4MappedToIPv6 && range.Begin.IsIPv4MappedToIPv6)
            {
                offset = 12; //ipv4 has prefix of 10 zero bytes and two 255 bytes. 
            }

            return
                Bits.LtECore(this.Begin.GetAddressBytes(), range.Begin.GetAddressBytes(), offset) &&
                Bits.GtECore(this.End.GetAddressBytes(), range.End.GetAddressBytes(), offset);
        }

        public static IPAddressRange Parse(string ipRangeString)
        {
            if (ipRangeString == null) throw new ArgumentNullException(nameof(ipRangeString));

            // trim white spaces.
            ipRangeString = ipRangeString.Trim();

            // define local funtion to strip scope id in ip address string.
            string stripScopeId(string ipaddressString) => ipaddressString.Split('%')[0];

            // Pattern 1. CIDR range: "192.168.0.0/24", "fe80::/10%eth0"
            var m1 = m1_regex.Match(ipRangeString);
            if (m1.Success)
            {
                var baseAdrBytes = IPAddress.Parse(stripScopeId(m1.Groups["adr"].Value)).GetAddressBytes();
                var maskLen = int.Parse(m1.Groups["maskLen"].Value);
                if (baseAdrBytes.Length * 8 < maskLen) throw new FormatException();
                var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, maskLen);
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                return new IPAddressRange(new IPAddress(baseAdrBytes), new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes))));
            }

            // Pattern 2. Uni address: "127.0.0.1", ":;1"
            var m2 = m2_regex.Match(ipRangeString);
            if (m2.Success)
            {
                return new IPAddressRange(IPAddress.Parse(stripScopeId(ipRangeString)));
            }

            // Pattern 3. Begin end range: "169.254.0.0-169.254.0.255"
            var m3 = m3_regex.Match(ipRangeString);
            if (m3.Success)
            {
                // if the left part contains dot, but the right one does not, we treat it as a shortuct notation
                // and simply copy the part before last dot from the left part as the prefix to the right one
                var begin = m3.Groups["begin"].Value;
                var end = m3.Groups["end"].Value;
                if (begin.Contains('.') && !end.Contains('.'))
                {
                    if (end.Contains('%')) throw new FormatException("The end of IPv4 range shortcut notation contains scope id.");
                    var lastDotAt = begin.LastIndexOf('.');
                    end = begin.Substring(0, lastDotAt + 1) + end;
                }

                return new IPAddressRange(
                    begin: IPAddress.Parse(stripScopeId(begin)),
                    end: IPAddress.Parse(stripScopeId(end)));
            }

            // Pattern 4. Bit mask range: "192.168.0.0/255.255.255.0"
            var m4 = m4_regex.Match(ipRangeString);
            if (m4.Success)
            {
                var baseAdrBytes = IPAddress.Parse(stripScopeId(m4.Groups["adr"].Value)).GetAddressBytes();
                var maskBytes = IPAddress.Parse(m4.Groups["bitmask"].Value).GetAddressBytes();
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                return new IPAddressRange(new IPAddress(baseAdrBytes), new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes))));
            }

            throw new FormatException("Unknown IP range string.");
        }

        public static bool TryParse(string ipRangeString, out IPAddressRange ipRange)
        {
            try
            {
                ipRange = IPAddressRange.Parse(ipRangeString);
                return true;
            }
            catch (Exception)
            {
                ipRange = null;
                return false;
            }
        }

        /// <summary>
        /// Takes a subnetmask (eg, "255.255.254.0") and returns the CIDR bit length of that
        /// address. Throws an exception if the passed address is not valid as a subnet mask.
        /// </summary>
        /// <param name="subnetMask">The subnet mask to use</param>
        /// <returns></returns>
        public static int SubnetMaskLength(IPAddress subnetMask)
        {
            if (subnetMask == null)
                throw new ArgumentNullException(nameof(subnetMask));

            var length = Bits.GetBitMaskLength(subnetMask.GetAddressBytes());
            if (length == null) throw new ArgumentException("Not a valid subnet mask", "subnetMask");
            return length.Value;
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            if(Begin.Equals(End))
            {
                yield return Begin;
                yield break;
            }

            var first = Begin.GetAddressBytes();
            var last = End.GetAddressBytes();

            for (var ip = first; Bits.LtECore(ip, last); ip = Bits.Increment(ip))
                yield return new IPAddress(ip);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the range in the format "begin-end", or 
        /// as a single address if End is the same as Begin.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Equals(Begin, End) ? Begin.ToString() : string.Format("{0}-{1}", Begin, End);
        }

        public int GetPrefixLength()
        {
            byte[] byteBegin = Begin.GetAddressBytes();
            byte[] byteEnd = End.GetAddressBytes();

            // Handle single IP
            if (Begin.Equals(End))
            {
                return byteBegin.Length * 8;
            }

            int length = byteBegin.Length * 8;

            for (int i = 0; i < length; i++)
            {
                byte[] mask = Bits.GetBitMask(byteBegin.Length, i);
                if (new IPAddress(Bits.And(byteBegin, mask)).Equals(Begin))
                {
                    if (new IPAddress(Bits.Or(byteBegin, Bits.Not(mask))).Equals(End))
                    {
                        return i;
                    }
                }
            }
            throw new FormatException(string.Format("{0} is not a CIDR Subnet", ToString()));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + End.GetHashCode();
                hash = hash * 31 + Begin.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object comparand)
        {
            return comparand is IPAddressRange range
                ? range.GetHashCode() == this.GetHashCode()
                : false;
        }

        /// <summary>
        /// Returns a Cidr String if this matches exactly a Cidr subnet
        /// </summary>
        public string ToCidrString()
        {
            return string.Format("{0}/{1}", Begin, GetPrefixLength());
        }

        #region JSON.NET Support by implement IReadOnlyDictionary<string, string>

        [EditorBrowsable(EditorBrowsableState.Never)]
        public IPAddressRange(IEnumerable<KeyValuePair<string, string>> items)
        {
            this.Begin = IPAddress.Parse(TryGetValue(items, nameof(Begin), out var value1) ? value1 : throw new KeyNotFoundException());
            this.End = IPAddress.Parse(TryGetValue(items, nameof(End), out var value2) ? value2 : throw new KeyNotFoundException());
        }

        /// <summary>
        /// Returns the input typed as IEnumerable&lt;IPAddress&gt;
        /// </summary>
        public IEnumerable<IPAddress> AsEnumerable() => (this as IEnumerable<IPAddress>);

        private IEnumerable<KeyValuePair<string, string>> GetDictionaryItems()
        {
            return new[] {
                new KeyValuePair<string,string>(nameof(Begin), Begin.ToString()),
                new KeyValuePair<string,string>(nameof(End), End.ToString()),
            };
        }

        private bool TryGetValue(string key, out string value) => TryGetValue(GetDictionaryItems(), key, out value);

        private bool TryGetValue(IEnumerable<KeyValuePair<string, string>> items, string key, out string value)
        {
            items = items ?? GetDictionaryItems();
            var foundItem = items.FirstOrDefault(item => item.Key == key);
            value = foundItem.Value;
            return foundItem.Key != null;
        }

        IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => GetDictionaryItems().Select(item => item.Key);

        IEnumerable<string> IReadOnlyDictionary<string, string>.Values => GetDictionaryItems().Select(item => item.Value);

        int IReadOnlyCollection<KeyValuePair<string, string>>.Count => GetDictionaryItems().Count();

        string IReadOnlyDictionary<string, string>.this[string key] => TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();

        bool IReadOnlyDictionary<string, string>.ContainsKey(string key) => GetDictionaryItems().Any(item => item.Key == key);

        bool IReadOnlyDictionary<string, string>.TryGetValue(string key, out string value) => TryGetValue(key, out value);

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator() => GetDictionaryItems().GetEnumerator();

        #endregion
    }
}
