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
