using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NetTools
{
    public class IPAddressRange
    {
        public IPAddress Begin { get; set; }

        public IPAddress End { get; set; }

        public IPAddressRange()
        {
            this.Begin = new IPAddress(0L);
            this.End = new IPAddress(0L);
        }

        public IPAddressRange(string ipRangeString)
        {
            // remove all spaces.
            ipRangeString = ipRangeString.Replace(" ", "");

            var m1 = Regex.Match(ipRangeString, @"^(?<adr>[\d\.:]+)/(?<maskLen>\d+)$");
            if (m1.Success)
            {
                var baseAdrBytes = IPAddress.Parse(m1.Groups["adr"].Value).GetAddressBytes();
                var maskBytes = Bits.GetBitMask(baseAdrBytes.Length, int.Parse(m1.Groups["maskLen"].Value));
                baseAdrBytes = Bits.And(baseAdrBytes, maskBytes);
                this.Begin = new IPAddress(baseAdrBytes);
                this.End = new IPAddress(Bits.Or(baseAdrBytes, Bits.Not(maskBytes)));
                return;
            }
            
            var m2 = Regex.Match(ipRangeString, @"^(?<adr>[\d\.:]+)$");
            if (m2.Success)
            {
                this.Begin = this.End = IPAddress.Parse(ipRangeString);
                return;
            }

            var m3 = Regex.Match(ipRangeString, @"^(?<begin>[\d\.:]+)-(?<end>[\d\.:]+)$");
            if (m3.Success)
            {
                this.Begin = IPAddress.Parse(m3.Groups["begin"].Value);
                this.End = IPAddress.Parse(m3.Groups["end"].Value);
                return;
            }

            throw new FormatException("Unknown IP range string.");
        }

        public bool Contains(IPAddress ipaddress)
        {
            if (ipaddress.AddressFamily != this.Begin.AddressFamily) return false;
            var adrBytes = ipaddress.GetAddressBytes();
            return Bits.GE(this.Begin.GetAddressBytes(), adrBytes) && Bits.LE(this.End.GetAddressBytes(), adrBytes);
        }
    }
}
