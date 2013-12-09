using System;
using NetTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class BitsTest
{
    [TestMethod]
    public void NotTest()
    {
        Bits.Not(new[] { (byte)0xD6, (byte)0x5E })
            .Is((byte)0x29, (byte)0xA1);
    }

    [TestMethod]
    public void AndTest()
    {
        Bits.And(
            new[] { (byte)0xD6, (byte)0x5E, (byte)0xD6 }, 
            new[] { (byte)0x00, (byte)0xFF, (byte)0x72 })
            .Is(new[] {(byte)0x00, (byte)0x5E, (byte)0x52 });
    }

    [TestMethod]
    public void OrTest()
    {
        Bits.Or(
            new[] { (byte)0xD6, (byte)0x5E, (byte)0xD6 },
            new[] { (byte)0x00, (byte)0xFF, (byte)0x72 })
            .Is(new[] { (byte)0xD6, (byte)0xFF, (byte)0xF6 });
    }

    [TestMethod]
    public void GETest()
    {
        Bits.GE(
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 },
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 }).Is(true);

        Bits.GE(
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 },
            new[] { (byte)0x12, (byte)0x4c, (byte)0x00 }).Is(true);
        Bits.GE(
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 },
            new[] { (byte)0x13, (byte)0x00, (byte)0xA5 }).Is(true);

        Bits.GE(
            new[] { (byte)0x12, (byte)0x3d, (byte)0xFF },
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 }).Is(false);
        Bits.GE(
            new[] { (byte)0x11, (byte)0xFF, (byte)0xA5 },
            new[] { (byte)0x10, (byte)0x3c, (byte)0xA5 }).Is(false);
    }

    [TestMethod]
    public void LETest()
    {
        Bits.LE(
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 },
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 }).Is(true);

        Bits.LE(
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 },
            new[] { (byte)0x12, (byte)0x4c, (byte)0x00 }).Is(false);
        Bits.LE(
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 },
            new[] { (byte)0x13, (byte)0x00, (byte)0xA5 }).Is(false);

        Bits.LE(
            new[] { (byte)0x12, (byte)0x3d, (byte)0xFF },
            new[] { (byte)0x12, (byte)0x3c, (byte)0xA5 }).Is(true);
        Bits.LE(
            new[] { (byte)0x11, (byte)0xFF, (byte)0xA5 },
            new[] { (byte)0x10, (byte)0x3c, (byte)0xA5 }).Is(true);
    }

    [TestMethod]
    public void GetBitMaskTest()
    {
        Bits.GetBitMask(4, 15).Is((byte)0xff, (byte)0xfe, (byte)0x00, (byte)0x00);
        Bits.GetBitMask(4, 16).Is((byte)0xff, (byte)0xff, (byte)0x00, (byte)0x00);
        Bits.GetBitMask(4, 19).Is((byte)0xff, (byte)0xff, (byte)0xe0, (byte)0x00);
        Bits.GetBitMask(4, 24).Is((byte)0xff, (byte)0xff, (byte)0xff, (byte)0x00);
        Bits.GetBitMask(4, 32).Is((byte)0xff, (byte)0xff, (byte)0xff, (byte)0xff);
    }
}
