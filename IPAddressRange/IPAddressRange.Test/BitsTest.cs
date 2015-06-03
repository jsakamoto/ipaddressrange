using System;
using NetTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class BitsTest
{
    [TestMethod]
    public void NotTest()
    {
        Bits.Not(new byte[] { 0xD6,  0x5E})
            .Is(new byte[] { 0x29,  0xA1});
    }

    [TestMethod]
    public void AndTest()
    {
        Bits.And(
            new byte[] { 0xD6, 0x5E, 0xD6 }, 
            new byte[] { 0x00, 0xFF, 0x72 })
            .Is(new byte[] {0x00, 0x5E, 0x52 });
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
    public void GETest()
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

    [TestMethod]
    public void LETest()
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
    public void GetBitMaskTest()
    {
        Bits.GetBitMask(4, 15).Is(new byte[] { 0xff, 0xfe, 0x00, 0x00 });
        Bits.GetBitMask(4, 16).Is(new byte[] { 0xff, 0xff, 0x00, 0x00 });
        Bits.GetBitMask(4, 19).Is(new byte[] { 0xff, 0xff, 0xe0, 0x00 });
        Bits.GetBitMask(4, 24).Is(new byte[] { 0xff, 0xff, 0xff, 0x00 });
        Bits.GetBitMask(4, 32).Is(new byte[] { 0xff, 0xff, 0xff, 0xff });
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

        AssertEx.Throws<OverflowException>(() => Bits.Increment(new byte[] {0xff, 0xff, 0xff, 0xff}));
    }
}
