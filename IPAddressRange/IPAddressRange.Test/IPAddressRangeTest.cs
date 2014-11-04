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
    public void ParseTest_IPv4_CIDR()
    {
        var range = new IPAddressRange("219.165.64.0/19");
        range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
        range.Begin.ToString().Is("219.165.64.0");
        range.End.AddressFamily.Is(AddressFamily.InterNetwork);
        range.End.ToString().Is("219.165.95.255");
    }

    [TestMethod]
    public void ParseTest_IPv4_CIDR_Max()
    {
        var range = new IPAddressRange("219.165.64.73/32");
        range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
        range.Begin.ToString().Is("219.165.64.73");
        range.End.AddressFamily.Is(AddressFamily.InterNetwork);
        range.End.ToString().Is("219.165.64.73");
    }

    [TestMethod]
    public void ParseTest_IPv4_Bitmask()
    {
        var range = new IPAddressRange("192.168.1.0/255.255.255.0");
        range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
        range.Begin.ToString().Is("192.168.1.0");
        range.End.AddressFamily.Is(AddressFamily.InterNetwork);
        range.End.ToString().Is("192.168.1.255");
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
    public void ContainsTest_with_IPV4andv6_is_False_ever()
    {
        var fullRangeIPv6 = new IPAddressRange("::-fff:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
        fullRangeIPv6.Contains(new IPAddressRange("192.168.0.0/24")).Is(false);

        var fullRangeIPv4 = new IPAddressRange("0.0.0.0-255.255.255.255");
        fullRangeIPv4.Contains(new IPAddressRange("::1-::2")).Is(false);
    }

    [TestMethod]
    public void ContainsTest_Range_is_True_IPv4()
    {
        var range = new IPAddressRange("192.168.60.26-192.168.60.37");
        var range1_same = new IPAddressRange("192.168.60.26-192.168.60.37");
        var range2_samestart = new IPAddressRange("192.168.60.26-192.168.60.30");
        var range3_sameend = new IPAddressRange("192.168.60.36-192.168.60.37");
        var range4_subset = new IPAddressRange("192.168.60.29-192.168.60.32");

        range.Contains(range1_same).Is(true);
        range.Contains(range2_samestart).Is(true);
        range.Contains(range3_sameend).Is(true);
        range.Contains(range4_subset).Is(true);
    }

    [TestMethod]
    public void ContainsTest_Range_is_False_IPv4()
    {
        var range = new IPAddressRange("192.168.60.29-192.168.60.32");
        var range1_overLeft = new IPAddressRange("192.168.60.26-192.168.70.1");
        var range2_overRight = new IPAddressRange("192.168.50.1-192.168.60.37");
        var range3_outOfLeft = new IPAddressRange("192.168.50.30-192.168.50.31");
        var range4_outOfRight = new IPAddressRange("192.168.70.30-192.168.70.31");

        range.Contains(range1_overLeft).Is(false);
        range.Contains(range2_overRight).Is(false);
        range.Contains(range3_outOfLeft).Is(false);
        range.Contains(range4_outOfRight).Is(false);
    }

    [TestMethod]
    public void ParseTest_IPv6_CIDR()
    {
        var range = new IPAddressRange("fe80::/10");
        range.Begin.AddressFamily.Is(AddressFamily.InterNetworkV6);
        range.Begin.ToString().Is("fe80::");
        range.End.AddressFamily.Is(AddressFamily.InterNetworkV6);
        range.End.ToString().Is("febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
    }

    [TestMethod]
    public void ContainsTest_IPv6()
    {
        var range = new IPAddressRange("FE80::/10");

        range.Contains(IPAddress.Parse("::1")).Is(false);
        range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586")).Is(true);
        range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586%3")).Is(true);
    }

    [TestMethod]
    public void ContainsTest_Range_is_True_IPv6()
    {
        var range = new IPAddressRange("fe80::/10");
        var range1_same = new IPAddressRange("fe80::/10");
        var range2_samestart = new IPAddressRange("fe80::-fe80::d503:4ee:3882:c586");
        var range3_sameend = new IPAddressRange("fe80::d503:4ee:3882:c586-febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
        var range4_subset = new IPAddressRange("fe80::d503:4ee:3882:c586-fe80::d504:4ee:3882:c586");

        range.Contains(range1_same).Is(true);
        range.Contains(range2_samestart).Is(true);
        range.Contains(range3_sameend).Is(true);
        range.Contains(range4_subset).Is(true);
    }

    [TestMethod]
    public void ContainsTest_Range_is_False_IPv6()
    {
        var range = new IPAddressRange("fe80::d503:4ee:3882:c586-fe80::d504:4ee:3882:c586");
        var range1_overLeft = new IPAddressRange("fe80::d502:4ee:3882:c586-fe80::d503:4ee:3882:c586");
        var range2_overRight = new IPAddressRange("fe80::d503:4ef:3882:c586-fe80::d505:4ee:3882:c586");
        var range3_outOfLeft = new IPAddressRange("fe80::d501:4ee:3882:c586-fe80::d502:4ee:3882:c586");
        var range4_outOfRight = new IPAddressRange("fe80::d505:4ee:3882:c586-fe80::d506:4ee:3882:c586");

        range.Contains(range1_overLeft).Is(false);
        range.Contains(range2_overRight).Is(false);
        range.Contains(range3_outOfLeft).Is(false);
        range.Contains(range4_outOfRight).Is(false);
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
