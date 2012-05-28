using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTools;

[TestClass]
public class IPAddressRangeTest
{
    [TestMethod]
    public void CtorTest()
    {
        var range = new IPAddressRange();
        range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
        range.Begin.ToString().Is("0.0.0.0");
        range.End.AddressFamily.Is(AddressFamily.InterNetwork);
        range.End.ToString().Is("0.0.0.0");
    }

    [TestMethod]
    public void ParseTest_IPv4_Uniaddress()
    {
        var range = new IPAddressRange("192.168.60.13");
        range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
        range.Begin.ToString().Is("192.168.60.13");
        range.End.AddressFamily.Is(AddressFamily.InterNetwork);
        range.End.ToString().Is("192.168.60.13");
    }

    [TestMethod]
    public void ParseTest_IPv4_Bitmask()
    {
        var range = new IPAddressRange("219.165.64.0/19");
        range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
        range.Begin.ToString().Is("219.165.64.0");
        range.End.AddressFamily.Is(AddressFamily.InterNetwork);
        range.End.ToString().Is("219.165.95.255");
    }

    [TestMethod]
    public void ParseTest_IPv4_Begin_to_End()
    {
        var range = new IPAddressRange("192.168.60.26-192.168.60.37");
        range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
        range.Begin.ToString().Is("192.168.60.26");
        range.End.AddressFamily.Is(AddressFamily.InterNetwork);
        range.End.ToString().Is("192.168.60.37");
    }

    [TestMethod]
    public void ContainsTest_IPv4()
    {
        var range = new IPAddressRange("192.168.60.26-192.168.60.37");
        
        range.Contains(IPAddress.Parse("192.168.60.25")).Is(false);
        range.Contains(IPAddress.Parse("192.168.60.26")).Is(true);
        range.Contains(IPAddress.Parse("192.168.60.27")).Is(true);

        range.Contains(IPAddress.Parse("192.168.60.36")).Is(true);
        range.Contains(IPAddress.Parse("192.168.60.37")).Is(true);
        range.Contains(IPAddress.Parse("192.168.60.38")).Is(false);
    }

    [TestMethod]
    public void ContainsTest_TestIPv6_to_IPv4Range()
    {
        var range = new IPAddressRange("192.168.60.26-192.168.60.37");
        
        range.Contains(IPAddress.Parse("c0a8:3c1a::")).Is(false);
    }

    [TestMethod]
    public void SerializeTest()
    {
        var ms1 = new MemoryStream();
        new DataContractJsonSerializer(typeof(IPAddressRange))
            .WriteObject(ms1, new IPAddressRange("192.168.0.0/24"));
        Encoding.ASCII.GetString(ms1.GetBuffer())
            .TrimEnd('\0')
            .Is(@"{""Begin"":""192.168.0.0"",""End"":""192.168.0.255""}");

        var ms2 = new MemoryStream();
        new DataContractJsonSerializer(typeof(IPAddressRange))
            .WriteObject(ms2, new IPAddressRange("::7 - ::9"));
        Encoding.ASCII.GetString(ms2.GetBuffer())
            .TrimEnd('\0')
            .Is(@"{""Begin"":""::7"",""End"":""::9""}");
    }

    [TestMethod]
    public void DeserializeTest()
    {
        Func<string, MemoryStream> toMemStrm = (str) => new MemoryStream(Encoding.ASCII.GetBytes(str));

        var range1 = new DataContractJsonSerializer(typeof(IPAddressRange))
            .ReadObject(toMemStrm(@"{""Begin"":""::2"",""End"":""::5""}")) as IPAddressRange;
        range1.Begin.ToString().Is("::2");
        range1.End.ToString().Is("::5");

        var range2 = new DataContractJsonSerializer(typeof(IPAddressRange))
            .ReadObject(toMemStrm(@"{""Begin"":""::3""}")) as IPAddressRange;
        range2.Begin.ToString().Is("::3");
        range2.End.ToString().Is("0.0.0.0");

        var range3 = new DataContractJsonSerializer(typeof(IPAddressRange))
            .ReadObject(toMemStrm(@"{""End"":""::6""}")) as IPAddressRange;
        range3.Begin.ToString().Is("0.0.0.0");
        range3.End.ToString().Is("::6");
    }


}
