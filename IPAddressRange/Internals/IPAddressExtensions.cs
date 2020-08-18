using System;
using System.Net;

namespace NetTools.Internals
{
    internal static class IPAddressExtensions
    {
        public static UInt32 ToUInt32(this IPAddress ipAddress)
        {
            var addressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(addressBytes);
            return BitConverter.ToUInt32(addressBytes, 0);
        }

        public static IPAddress ToIPv4Address(this UInt32 value)
        {
            var addressBytes = BitConverter.GetBytes(value);
            Array.Reverse(addressBytes);
            return new IPAddress(addressBytes);
        }
    }
}
