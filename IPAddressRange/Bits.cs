using System;
using System.ComponentModel;
using System.Linq;

namespace NetTools
{
    public static class Bits
    {
        public static byte[] Not(byte[] bytes)
        {
            var result = (byte[])bytes.Clone();
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (byte)~result[i];
            }
            return result;
            //return bytes.Select(b => (byte)~b).ToArray();
        }

        public static byte[] And(byte[] A, byte[] B)
        {
            var result = (byte[])A.Clone();
            for (var i = 0; i < A.Length; i++)
            {
                result[i] &= B[i];
            }
            return result;
            //return A.Zip(B, (a, b) => (byte)(a & b)).ToArray();
        }

        public static byte[] Or(byte[] A, byte[] B)
        {
            var result = (byte[])A.Clone();
            for (var i = 0; i < A.Length; i++)
            {
                result[i] |= B[i];
            }
            return result;
            //return A.Zip(B, (a, b) => (byte)(a | b)).ToArray();
        }

        // DON'T FIX this non-intuitive behavior that returns true when A <= B, 
        // even if the method name means "A is Greater than or Equals B", for keeping backward compatibility.
        // Fixed verison is in "NetTools.Internal" namespace "Bits" class.
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("This method returns true when A<=B, not A is greater than or equal (>=) B. use LtE method to check A<=B or not.")]
        public static bool GE(byte[] A, byte[] B) => LtE(A, B);

        // DON'T FIX this non-intuitive behavior that returns true when A >= B, 
        // even if the method name means "A is Less than or Equals B", for keeping backward compatibility.
        // Fixed verison is in "NetTools.Internal" namespace "Bits" class.
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("This method returns true when A>=B, not A is less than or equal (<=) B. use GtE method to check A>=B or not.")]
        public static bool LE(byte[] A, byte[] B) => GtE(A, B);

        public static bool LtE(byte[] A, byte[] B, int offset = 0)
        {
            if (A == null) throw new ArgumentNullException(nameof(A));
            if (B == null) throw new ArgumentNullException(nameof(B));
            if (offset < 0) throw new ArgumentException("offset must be greater than or equal 0.", nameof(offset));
            if (A.Length <= offset || B.Length <= offset) throw new ArgumentException("offset must be less than length of A and B.", nameof(offset));

            return LtECore(A, B, offset);
        }

        internal static bool LtECore(byte[] A, byte[] B, int offset = 0)
        {
            var length = A.Length;
            if (length > B.Length) length = B.Length;
            for (var i = offset; i < length; i++)
            {
                if (A[i] != B[i]) return A[i] <= B[i];
            }
            return true;
        }

        public static bool GtE(byte[] A, byte[] B, int offset = 0)
        {
            if (A == null) throw new ArgumentNullException(nameof(A));
            if (B == null) throw new ArgumentNullException(nameof(B));
            if (offset < 0) throw new ArgumentException("offset must be greater than or equal 0.", nameof(offset));
            if (A.Length <= offset || B.Length <= offset) throw new ArgumentException("offset must be less than length of A and B.", nameof(offset));

            return GtECore(A, B, offset);
        }

        internal static bool GtECore(byte[] A, byte[] B, int offset = 0)
        {
            var length = A.Length;
            if (length > B.Length) length = B.Length;
            for (var i = offset; i < length; i++)
            {
                if (A[i] != B[i]) return A[i] >= B[i];
            }
            return true;
        }

        public static bool IsEqual(byte[] A, byte[] B)
        {
            if (A == null || B == null) { return false; }
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
            for (; idx < bytes.Length && bytes[idx] == 0xff; idx++) ;
            bitLength = 8 * idx;

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