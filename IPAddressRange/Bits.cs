using System;
using System.Linq;

namespace NetTools
{
	public static class Bits
	{
		public static byte[] Not(byte[] bytes)
		{
			byte[] res = new byte[bytes.Length];

			for (var i = 0; i < bytes.Length; i++)
				res[i] = (byte)~bytes[i];

			return res;
			//   return bytes.Select(b => (byte)~b).ToArray();
		}

		public static byte[] And(byte[] A, byte[] B)
		{
			int resLength = Math.Min(A.Length, B.Length);

			byte[] res = new byte[resLength];

			for (var i = 0; i < resLength; i++)
				res[i] = (byte)(A[i] & B[i]);

			return res;
			//   return A.Zip(B, (a, b) => (byte)(a & b)).ToArray();
		}

		public static byte[] Or(byte[] A, byte[] B)
		{
			int resLength = Math.Min(A.Length, B.Length);

			byte[] res = new byte[resLength];

			for (var i = 0; i < resLength; i++)
				res[i] = (byte)(A[i] | B[i]);

			return res;
			//		return A.Zip(B, (a, b) => (byte)(a | b)).ToArray();
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

		public static byte[] GetBitMask(int sizeOfBuff, int bitLen)
		{
			var maskBytes = new byte[sizeOfBuff];
			var bytesLen = bitLen / 8;
			var bitsLen = bitLen % 8;
			for (int i = 0; i < bytesLen; i++)
			{
				maskBytes[i] = 0xff;
			}
			if (bitsLen > 0)
			{
				int aggregate = 0;

				for (int i = 1; i <= 8 - bitsLen; i++)
					aggregate = (1 << i - 1) | aggregate;

				maskBytes[bytesLen] = (byte)~aggregate;
				//		maskBytes[bytesLen] = (byte)~Enumerable.Range(1, 8 - bitsLen).Select(n => 1 << n - 1).Aggregate((a, b) => a | b);
			}

			return maskBytes;
		}

		public static byte[] Increment(byte[] bytes)
		{
			var incrementIndex = Array.FindLastIndex(bytes, x => x < byte.MaxValue);
			if (incrementIndex < 0) throw new OverflowException();
			return bytes
				.Take(incrementIndex)
				.Concat(new byte[] { (byte)(bytes[incrementIndex] + 1) })
				.Concat(new byte[bytes.Length - incrementIndex - 1])
				.ToArray();
		}

	}
}
