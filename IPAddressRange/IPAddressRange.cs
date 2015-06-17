using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace NetTools
{
    [Serializable]
    public class IPAddressRange : ISerializable, IEnumerable<IPAddress>
    {
        public IPAddress Begin { get; set; }

        public IPAddress End { get; set; }

        public IPAddressRange() : this(new IPAddress(0L)) { }

        /// <summary>
        /// Creates an empty range object, equivalent to "0.0.0.0/0".
        /// </summary>
        /// <param name="singleAddress"></param>
        public IPAddressRange(IPAddress singleAddress)
        {
            Begin = End = singleAddress;
        }

        /// <summary>
        /// Create a new range based on the first and last IPs in an array.
        /// Throws an exception if the array is empty or null. All but the first and last values 
        /// are ignored. Passing a single value is equivalent to a range of one address.
        /// </summary>
        /// <param name="beginEnd"></param>
        public IPAddressRange(ICollection<IPAddress> beginEnd)
        {
            if (beginEnd == null) throw new ArgumentNullException("beginEnd");
            if (beginEnd.Count == 0) throw new ArgumentOutOfRangeException("beginEnd", "Array contains no elements");
            
            Begin = beginEnd.First();
            End = beginEnd.Last();

            if (Begin.AddressFamily != End.AddressFamily) throw new ArgumentException("Elements must be of the same address family", "beginEnd");

            var beginBytes = Begin.GetAddressBytes();
            var endBytes = End.GetAddressBytes();
            if (!Bits.LE(endBytes, beginBytes)) throw new ArgumentException("Begin must be smaller than the End", "beginEnd");
        }

        /// <summary>
        /// Creates a range from a base address and mask bits 
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="maskLength"></param>
        public IPAddressRange(IPAddress baseAddress, int maskLength)
        {
            var baseAdrBytes = baseAddress.GetAddressBytes();
            if (baseAdrBytes.Length * 8 < maskLength) throw new FormatException();
            var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, maskLength);
            baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
            
            Begin = new IPAddress(baseAdrBytes);
            End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
        }
        
        /// <summary>
        /// Creates a range from a base address and subnet mask.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="subnetMask"></param>
        public IPAddressRange(IPAddress baseAddress, IPAddress subnetMask)
        {
            var baseAdrBytes = baseAddress.GetAddressBytes();
            var maskBytes = subnetMask.GetAddressBytes();
            baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
            
            Begin = new IPAddress(baseAdrBytes);
            End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
        }

        [Obsolete("Use IPAddressRange.Parse static method instead.")]
        public IPAddressRange(string ipRangeString)
        {
            var parsed = Parse(ipRangeString);
            Begin = parsed.Begin;
            End = parsed.End;
        }

        protected IPAddressRange(SerializationInfo info, StreamingContext context)
        {
            var names = new List<string>();
            foreach (var item in info) names.Add(item.Name);
            
            Func<string,IPAddress> deserialize = (name) => names.Contains(name) ? 
                IPAddress.Parse(info.GetValue(name, typeof(object)).ToString()) : 
                new IPAddress(0L);

            this.Begin = deserialize("Begin");
            this.End = deserialize("End");
        }

        public bool Contains(IPAddress ipaddress)
        {
            if (ipaddress.AddressFamily != this.Begin.AddressFamily) return false;
            var adrBytes = ipaddress.GetAddressBytes();
            return Bits.GE(this.Begin.GetAddressBytes(), adrBytes) && Bits.LE(this.End.GetAddressBytes(), adrBytes);
        }

        public bool Contains(IPAddressRange range)
        {
            if (this.Begin.AddressFamily != range.Begin.AddressFamily) return false;

            return 
                Bits.GE(this.Begin.GetAddressBytes(), range.Begin.GetAddressBytes()) &&
                Bits.LE(this.End.GetAddressBytes(), range.End.GetAddressBytes());

            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Begin", this.Begin != null ? this.Begin.ToString() : "");
            info.AddValue("End", this.End != null ? this.End.ToString() : "");
        }

        public static IPAddressRange Parse(string ipRangeString)
        {
            // remove all spaces.
            ipRangeString = ipRangeString.Replace(" ", "");

            // Pattern 1. CIDR range: "192.168.0.0/24", "fe80::/10"
            var m1 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)/(?<maskLen>\d+)$", RegexOptions.IgnoreCase);
            if (m1.Success)
            {
                return new IPAddressRange(IPAddress.Parse(m1.Groups["adr"].Value), int.Parse(m1.Groups["maskLen"].Value));
            }

            // Pattern 2. Uni address: "127.0.0.1", ":;1"
            var m2 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                return new IPAddressRange(IPAddress.Parse(ipRangeString));
            }

            // Pattern 3. Begin end range: "169.258.0.0-169.258.0.255"
            var m3 = Regex.Match(ipRangeString, @"^(?<begin>[\da-f\.:]+)[\-–](?<end>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m3.Success)
            {
                return new IPAddressRange(new[] { IPAddress.Parse(m3.Groups["begin"].Value), IPAddress.Parse(m3.Groups["end"].Value) });
            }

            // Pattern 4. Bit mask range: "192.168.0.0/255.255.255.0"
            var m4 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)/(?<bitmask>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m4.Success)
            {
                return new IPAddressRange(
                    IPAddress.Parse(m4.Groups["adr"].Value),
                    IPAddress.Parse(m4.Groups["bitmask"].Value));
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
            catch (FormatException)
            {
                ipRange = null;
                return false;
            }
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            var first = Begin.GetAddressBytes();
            var last = End.GetAddressBytes();
            for (var ip = first; Bits.GE(ip, last); ip = Bits.Increment(ip)) 
                yield return new IPAddress(ip);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
