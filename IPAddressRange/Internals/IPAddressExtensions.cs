using System;
using System.Net;
using System.Numerics;

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

        public static BigInteger ToBigInteger(this IPAddress ipAddress)
        {
            var addressBytes = ipAddress.GetAddressBytes();
            Array.Reverse(addressBytes);
            Array.Resize(ref addressBytes, addressBytes.Length + 1);
            return new BigInteger(addressBytes);
        }

        public static IPAddress ToIPv4Address(this UInt32 value)
        {
            var addressBytes = BitConverter.GetBytes(value);
            Array.Reverse(addressBytes);
            return new IPAddress(addressBytes);
        }

        public static IPAddress ToIPv6Address(ref this BigInteger value)
        {
            var addressBytes = value.ToByteArray();
            Array.Resize(ref addressBytes, 16);
            Array.Reverse(addressBytes);
            return new IPAddress(addressBytes);
        }
    }
}
