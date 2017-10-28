using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetTools
{
    public static class Bits
    {
        public static byte[] Not(byte[] bytes)
        {
            return bytes.Select(b => (byte)~b).ToArray();
        }

        public static byte[] And(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => (byte)(a & b)).ToArray();
        }

        public static byte[] Or(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => (byte)(a | b)).ToArray();
        }

        public static bool GE(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => a == b ? 0 : a < b ? 1 : -1)
                .SkipWhile(c => c == 0)
                .FirstOrDefault() >= 0;
        }

        public static bool LE(byte[] A, byte[] B)
        {
            return A.Zip(B, (a, b) => a == b ? 0 : a < b ? 1 : -1)
                .SkipWhile(c => c == 0)
                .FirstOrDefault() <= 0;
        }

        public static bool IsEqual(byte[] A, byte[] B)
        {
            if (A.Length != B.Length) { return false; }
            return A.Zip(B, (a, b) => a == b).All(x => x == true);
        }

        public static byte[] GetBitMask(int sizeOfBuff, int bitLen)
        {
            var maskBytes = new byte[sizeOfBuff];
            var bytesLen = bitLen / 8;
            var bitsLen = bitLen % 8;
            for (var i = 0; i < bytesLen; i++)
            {
                maskBytes[i] = 0xff;
            }
            if (bitsLen > 0) maskBytes[bytesLen] = (byte)~Enumerable.Range(1, 8 - bitsLen).Select(n => 1 << n - 1).Aggregate((a, b) => a | b);
            return maskBytes;
        }

        /// <summary>
        /// Counts the number of leading 1's in a bitmask.
        /// Returns null if value is invalid as a bitmask.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int? GetBitMaskLength(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var bitLength = 0;
            var idx = 0;

            // find beginning 0xFF
            for (; idx < bytes.Length && bytes[idx] == 0xff; idx++);
            bitLength = 8*idx;

            if (idx < bytes.Length)
            {
                switch (bytes[idx])
                {
                    case 0xFE: bitLength += 7; break;
                    case 0xFC: bitLength += 6; break;
                    case 0xF8: bitLength += 5; break;
                    case 0xF0: bitLength += 4; break;
                    case 0xE0: bitLength += 3; break;
                    case 0xC0: bitLength += 2; break;
                    case 0x80: bitLength += 1; break;
                    case 0x00: break;
                    default: // invalid bitmask
                        return null;
                }
                // remainder must be 0x00
                if (bytes.Skip(idx + 1).Any(x => x != 0x00)) return null;
            }
            return bitLength;
        }


        public static byte[] Increment(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            var incrementIndex = Array.FindLastIndex(bytes, x => x < byte.MaxValue);
            if (incrementIndex < 0) throw new OverflowException();
            return bytes
                .Take(incrementIndex)
                .Concat(new byte[] { (byte)(bytes[incrementIndex] + 1) })
                .Concat(new byte[bytes.Length - incrementIndex - 1])
                .ToArray();
        }

        public static byte[] Decrement(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.All(x => x == byte.MinValue)) throw new OverflowException();

            byte[] result = new byte[bytes.Length];
            Array.Copy(bytes, result, bytes.Length);

            for (int i = result.Length - 1; i >= 0; i--)
            {
                if (result[i] > byte.MinValue)
                {
                    result[i]--;
                    break;
                }
                else
                {
                    result[i] = byte.MaxValue;
                }
            }

            return result;
        }
    }
}