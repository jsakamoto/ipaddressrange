using System;
using NetTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class BitsTest
{
    [TestMethod]
    public void NotTest()
    {
        Bits.Not(new byte[] { 0xD6, 0x5E })
            .Is(new byte[] { 0x29, 0xA1 });
    }

    [TestMethod]
    public void AndTest()
    {
        Bits.And(
            new byte[] { 0xD6, 0x5E, 0xD6 },
            new byte[] { 0x00, 0xFF, 0x72 })
            .Is(new byte[] { 0x00, 0x5E, 0x52 });
    }

    [TestMethod]
    public void OrTest()
    {
        Bits.Or(
            new byte[] { 0xD6, 0x5E, 0xD6 },
            new byte[] { 0x00, 0xFF, 0x72 })
            .Is(new byte[] { 0xD6, 0xFF, 0xF6 });
    }

    [TestMethod]
    public void LtETest()
    {
        Bits.LtE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(true);

        Bits.LtE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x4c, 0x00 }).Is(true);
        Bits.LtE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x13, 0x00, 0xA5 }).Is(true);

        Bits.LtE(
            new byte[] { 0x12, 0x3d, 0xFF },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(false);
        Bits.LtE(
            new byte[] { 0x11, 0xFF, 0xA5 },
            new byte[] { 0x10, 0x3c, 0xA5 }).Is(false);
    }

    [TestMethod]
    public void GtETest()
    {
        Bits.GtE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(true);

        Bits.GtE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x4c, 0x00 }).Is(false);
        Bits.GtE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x13, 0x00, 0xA5 }).Is(false);

        Bits.GtE(
            new byte[] { 0x12, 0x3d, 0xFF },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(true);
        Bits.GtE(
            new byte[] { 0x11, 0xFF, 0xA5 },
            new byte[] { 0x10, 0x3c, 0xA5 }).Is(true);
    }

    [TestMethod, Obsolete]
    public void KeepingBackwardCompatibility_GETest()
    {
        Bits.GE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(true);

        Bits.GE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x4c, 0x00 }).Is(true);
        Bits.GE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x13, 0x00, 0xA5 }).Is(true);

        Bits.GE(
            new byte[] { 0x12, 0x3d, 0xFF },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(false);
        Bits.GE(
            new byte[] { 0x11, 0xFF, 0xA5 },
            new byte[] { 0x10, 0x3c, 0xA5 }).Is(false);
    }

    [TestMethod, Obsolete]
    public void KeepingBackwardCompatibility_LETest()
    {
        Bits.LE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(true);

        Bits.LE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x4c, 0x00 }).Is(false);
        Bits.LE(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x13, 0x00, 0xA5 }).Is(false);

        Bits.LE(
            new byte[] { 0x12, 0x3d, 0xFF },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(true);
        Bits.LE(
            new byte[] { 0x11, 0xFF, 0xA5 },
            new byte[] { 0x10, 0x3c, 0xA5 }).Is(true);
    }

    [TestMethod]
    public void IsEqualTest()
    {
        Bits.IsEqual(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(true);

        Bits.IsEqual(
            new byte[] { 0x12, 0x3c, 0xA5, 0x00 },
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(false);

        Bits.IsEqual(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x11, 0x3c, 0xA5 }).Is(false);

        Bits.IsEqual(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x31, 0xA5 }).Is(false);

        Bits.IsEqual(
            new byte[] { 0x12, 0x3c, 0xA5 },
            new byte[] { 0x12, 0x3c, 0xA1 }).Is(false);
    }

    [TestMethod]
    public void IsEqualWithNullTest()
    {
        Bits.IsEqual(
            new byte[] { 0x12, 0x3c, 0xA5 },
            null).Is(false);

        Bits.IsEqual(
            null,
            new byte[] { 0x12, 0x3c, 0xA5 }).Is(false);

        Bits.IsEqual(
            null,
            null).Is(false);
    }

    [TestMethod]
    public void GetBitMaskTest()
    {
        Bits.GetBitMask(4, 15).Is(new byte[] { 0xff, 0xfe, 0x00, 0x00 });
        Bits.GetBitMask(4, 16).Is(new byte[] { 0xff, 0xff, 0x00, 0x00 });
        Bits.GetBitMask(4, 19).Is(new byte[] { 0xff, 0xff, 0xe0, 0x00 });
        Bits.GetBitMask(4, 24).Is(new byte[] { 0xff, 0xff, 0xff, 0x00 });
        Bits.GetBitMask(4, 32).Is(new byte[] { 0xff, 0xff, 0xff, 0xff });
    }

    [TestMethod]
    public void GetBitMaskLengthTest()
    {
        Bits.GetBitMaskLength(new byte[] { 128, 0, 0, 0 }).Is(1);
        Bits.GetBitMaskLength(new byte[] { 255, 0, 0, 0 }).Is(8);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 0, 0 }).Is(16);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 255, 0 }).Is(24);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 254, 0 }).Is(23);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 252, 0 }).Is(22);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 248, 0 }).Is(21);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 240, 0 }).Is(20);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 224, 0 }).Is(19);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 192, 0 }).Is(18);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 128, 0 }).Is(17);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 255, 254 }).Is(31);
        Bits.GetBitMaskLength(new byte[] { 255, 255, 255, 255 }).Is(32);

        Bits.GetBitMaskLength(new byte[] { 255, 1, 0, 0 }).Is((int?)null);
        Bits.GetBitMaskLength(new byte[] { 255, 127, 0, 0 }).Is((int?)null);

        Bits.GetBitMaskLength(new byte[] { 255, 0, 0, 128 }).Is((int?)null);
        Bits.GetBitMaskLength(new byte[] { 255, 192, 0, 255 }).Is((int?)null);
    }

    [TestMethod]
    public void IncrementTest()
    {
        Bits.Increment(new byte[] { 0x00, 0x00, 0x00, 0x00 }).Is(new byte[] { 0x00, 0x00, 0x00, 0x01 });
        Bits.Increment(new byte[] { 0x00, 0xff, 0xff, 0xff }).Is(new byte[] { 0x01, 0x00, 0x00, 0x00 });
        Bits.Increment(new byte[] { 0x0a, 0x00, 0x00, 0x01 }).Is(new byte[] { 0x0a, 0x00, 0x00, 0x02 });
        Bits.Increment(new byte[] { 0x0a, 0x00, 0x00, 0xff }).Is(new byte[] { 0x0a, 0x00, 0x01, 0x00 });
        Bits.Increment(new byte[] { 0x0a, 0x00, 0xf4, 0xff }).Is(new byte[] { 0x0a, 0x00, 0xf5, 0x00 });
        Bits.Increment(new byte[] { 0x0a, 0xff, 0xff, 0xff }).Is(new byte[] { 0x0b, 0x00, 0x00, 0x00 });
        Bits.Increment(new byte[] { 0xff, 0xff, 0xff, 0xfe }).Is(new byte[] { 0xff, 0xff, 0xff, 0xff });

        AssertEx.Throws<OverflowException>(() => Bits.Increment(new byte[] { 0xff, 0xff, 0xff, 0xff }));
    }

    [TestMethod]
    public void DecrementTest()
    {
        Bits.Decrement(new byte[] { 0x00, 0x00, 0x00, 0x01 }).Is(new byte[] { 0x00, 0x00, 0x00, 0x00 });
        Bits.Decrement(new byte[] { 0x01, 0x00, 0x00, 0x00 }).Is(new byte[] { 0x00, 0xff, 0xff, 0xff });
        Bits.Decrement(new byte[] { 0x0a, 0x00, 0x00, 0x02 }).Is(new byte[] { 0x0a, 0x00, 0x00, 0x01 });
        Bits.Decrement(new byte[] { 0x0a, 0x00, 0x01, 0x00 }).Is(new byte[] { 0x0a, 0x00, 0x00, 0xff });
        Bits.Decrement(new byte[] { 0x0a, 0x00, 0xf5, 0x00 }).Is(new byte[] { 0x0a, 0x00, 0xf4, 0xff });
        Bits.Decrement(new byte[] { 0x0b, 0x00, 0x00, 0x00 }).Is(new byte[] { 0x0a, 0xff, 0xff, 0xff });
        Bits.Decrement(new byte[] { 0xff, 0xff, 0xff, 0xff }).Is(new byte[] { 0xff, 0xff, 0xff, 0xfe });

        AssertEx.Throws<OverflowException>(() => Bits.Decrement(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
    }
}
