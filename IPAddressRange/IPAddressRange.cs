using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace NetTools
{
    [Serializable]
    public class IPAddressRange : ISerializable
    {
        public IPAddress Begin { get; set; }

        public IPAddress End { get; set; }

        public IPAddressRange()
        {
            this.Begin = new IPAddress(0L);
            this.End = new IPAddress(0L);
        }

        [Obsolete("Use IPAddressRange.Parse static method instead.")]
        public IPAddressRange(string ipRangeString)
        {
            // remove all spaces.
            ipRangeString = ipRangeString.Replace(" ", "");

            // Pattern 1. CIDR range: "192.168.0.0/24", "fe80::/10"
            var m1 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)/(?<maskLen>\d+)$", RegexOptions.IgnoreCase);
            if (m1.Success)
            {
                var baseAdrBytes = IPAddress.Parse(m1.Groups["adr"].Value).GetAddressBytes();
                var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, int.Parse(m1.Groups["maskLen"].Value));
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                this.Begin = new IPAddress(baseAdrBytes);
                this.End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
                return;
            }

            // Pattern 2. Uni address: "127.0.0.1", ":;1"
            var m2 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                this.Begin = this.End = IPAddress.Parse(ipRangeString);
                return;
            }

            // Pattern 3. Begin end range: "169.258.0.0-169.258.0.255"
            var m3 = Regex.Match(ipRangeString, @"^(?<begin>[\da-f\.:]+)-(?<end>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m3.Success)
            {
                this.Begin = IPAddress.Parse(m3.Groups["begin"].Value);
                this.End = IPAddress.Parse(m3.Groups["end"].Value);
                return;
            }

            // Pattern 4. Bit mask range: "192.168.0.0/255.255.255.0"
            var m4 = Regex.Match(ipRangeString, @"^(?<adr>[\da-f\.:]+)/(?<bitmask>[\da-f\.:]+)$", RegexOptions.IgnoreCase);
            if (m4.Success)
            {
                var baseAdrBytes = IPAddress.Parse(m4.Groups["adr"].Value).GetAddressBytes();
                var maskBytes = IPAddress.Parse(m4.Groups["bitmask"].Value).GetAddressBytes();
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                this.Begin = new IPAddress(baseAdrBytes);
                this.End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
                return;
            }

            throw new FormatException("Unknown IP range string.");
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
#pragma warning disable 618
            return new IPAddressRange(ipRangeString);
#pragma warning restore
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
    }
}
