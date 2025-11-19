using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTools;

namespace IPRange.Test
{
    [TestClass]
    public class IPAddressRangeParseTest
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        [DataRow("192.168.60.13", "192.168.60.13", "192.168.60.13")]
        [DataRow("  192.168.60.13  ", "192.168.60.13", "192.168.60.13")]
        [DataRow("fe80::d503:4ee:3882:c586", "fe80::d503:4ee:3882:c586", "fe80::d503:4ee:3882:c586")]
        [DataRow("  fe80::d503:4ee:3882:c586  ", "fe80::d503:4ee:3882:c586", "fe80::d503:4ee:3882:c586")]
        [DataRow("::/0", "::", "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")]
        [DataRow("3232252004", "192.168.64.100", "192.168.64.100")] // decimal - new 
        [DataRow("  3232252004  ", "192.168.64.100", "192.168.64.100")] // decimal - new 

        [DataRow("219.165.64.0/19", "219.165.64.0", "219.165.95.255")]
        [DataRow("  219.165.64.0  /  19  ", "219.165.64.0", "219.165.95.255")]
        [DataRow("192.168.1.0/255.255.255.0", "192.168.1.0", "192.168.1.255")]
        [DataRow("  192.168.1.0  /  255.255.255.0  ", "192.168.1.0", "192.168.1.255")]
        [DataRow("3232252004/24", "192.168.64.0", "192.168.64.255")] // decimal - new 
        [DataRow("  3232252004  /  24  ", "192.168.64.0", "192.168.64.255")] // decimal - new 

        [DataRow("192.168.60.26–192.168.60.37", "192.168.60.26", "192.168.60.37")]
        [DataRow("  192.168.60.26  –  192.168.60.37  ", "192.168.60.26", "192.168.60.37")]
        [DataRow("fe80::c586-fe80::c600", "fe80::c586", "fe80::c600")]
        [DataRow("  fe80::c586  -  fe80::c600  ", "fe80::c586", "fe80::c600")]
        [DataRow("3232252004-3232252504", "192.168.64.100", "192.168.66.88")]
        [DataRow("  3232252004  -  3232252504  ", "192.168.64.100", "192.168.66.88")]
        [DataRow("192.168.1- 192.168.1111", "192.168.0.1", "192.168.4.87")] // 3 part IPv4
        [DataRow("173.1 -173.1111", "173.0.0.1", "173.0.4.87")] // 2 part IPv4

        // with "dash (–)" (0x2013) is also support.
        [DataRow("192.168.61.26–192.168.61.37", "192.168.61.26", "192.168.61.37")]
        [DataRow("  192.168.61.26  –  192.168.61.37  ", "192.168.61.26", "192.168.61.37")]
        [DataRow("fe80::c586–fe80::c600", "fe80::c586", "fe80::c600")]
        [DataRow("  fe80::c586  –  fe80::c600  ", "fe80::c586", "fe80::c600")]
        [DataRow("3232252004–3232252504", "192.168.64.100", "192.168.66.88")]
        [DataRow("  3232252004  –  3232252504  ", "192.168.64.100", "192.168.66.88")]
        [DataRow("192.168.1.1-7", "192.168.1.1", "192.168.1.7")]

        // IPv6 with scope id (scope id should be stripped in begin/end properties.)
        [DataRow("fe80::0%eth0/112", "fe80::", "fe80::ffff")]
        [DataRow("fe80::8000%12-fe80::80ff%12", "fe80::8000", "fe80::80ff")]
        [DataRow("fe80::1%lo1", "fe80::1", "fe80::1")]

        // IPv4 mapped to IPv6
        [DataRow("::ffff:10.0.0.0/120", "::ffff:10.0.0.0", "::ffff:10.0.0.255")]
        [DataRow("::ffff:192.168.10.20-::ffff:192.168.11.20", "::ffff:192.168.10.20", "::ffff:192.168.11.20")]
        [DataRow("::ffff:10.0.0.203", "::ffff:10.0.0.203", "::ffff:10.0.0.203")]
        [DataRow("::ffff:10.0.2.0/ffff:ffff:ffff:ffff:ffff:ffff:ffff:ff00", "::ffff:10.0.2.0", "::ffff:10.0.2.255")]
        public void ParseSucceeds(string input, string expectedBegin, string expectedEnd)
        {
            Console.WriteLine("TestCase: \"{0}\", Expected Begin: {1}, End: {2}", input, expectedBegin, expectedEnd);
            var range = IPAddressRange.Parse(input);
            range.IsNotNull();
            Console.WriteLine("  Result: Begin: {0}, End: {1}", range.Begin, range.End);
            range.Begin.ToString().Is(expectedBegin);
            range.End.ToString().Is(expectedEnd);
        }

        [TestMethod]
        [DataRow(null, typeof(ArgumentNullException))]
        [DataRow("", typeof(FormatException))]
        [DataRow(" ", typeof(FormatException))]
        [DataRow("gvvdv", typeof(FormatException))]
        [DataRow("192.168.0.10/48", typeof(FormatException))] // out of CIDR range 
        [DataRow("192.168.0.10-192.168.0.5", typeof(ArgumentException))] // bigger to lower
        [DataRow("fe80::2%eth1-fe80::1%eth1", typeof(ArgumentException))] // bigger to lower
        [DataRow("10.256.1.1", typeof(FormatException))] // invalid ip
        [DataRow("127.0.0.1%1", typeof(FormatException))] // ipv4, but with scope id
        [DataRow("192.168.0.10/2.3.4", typeof(FormatException))] // ipv4, but subnet mask isn't linear 
        [DataRow("192.168.0.0-192.168.0.1%1", typeof(FormatException))] // ipv4, but with scope id at end of range
        [DataRow("192.168.0.0%1-192.168.0.1", typeof(FormatException))] // ipv4, but with scope id at begin of range
        [DataRow("192.168.0.0%1-192.168.0.1%1", typeof(FormatException))] // ipv4, but with scope id at both of begin and end
        [DataRow("192.168.0.0%1/24", typeof(FormatException))] // CIDR ipv4, but with scope id
        [DataRow("192.168.0.0%1/255.255.255.0", typeof(FormatException))] // ipv4 and subnet mask, but with scope id
        [DataRow("192.168-::1", typeof(FormatException))] // Invalid comibination of IPv4 and IPv6
        [DataRow("192.168.0.0-256", typeof(FormatException))] // shortcut notation, but out of range
        [DataRow("192.168.0.0-1%1", typeof(FormatException))] // ipv4 shortcut, but with scope id at end of range
        [DataRow("192.168.0.0%1-1", typeof(FormatException))] // ipv4 shortcut, but with scope id at begin of range
        [DataRow("172. 13.0.0/24", typeof(FormatException))] // ipv4, but include spaces
        [DataRow("fe80::0-fe80: :ffff", typeof(FormatException))] // ipv4, but include spaces
        public void ParseFails(string input, Type expectedException)
        {
            Console.WriteLine("TestCase: \"{0}\", Expected Exception: {1}", input, expectedException.Name);
            try
            {
                IPAddressRange.Parse(input);
                Assert.Fail($"Expected exception of type {expectedException.Name} to be thrown for input \"{input}\"");
            }
            catch (AssertFailedException)
            {
                throw; // allow Assert.Fail to pass through 
            }
            catch (Exception ex)
            {
                ex.GetType().Is(expectedException);
            }
        }

        [TestMethod]
        [DataRow(null, false)] // bug3
        [DataRow("", false)]
        [DataRow(" ", false)]
        [DataRow("fdfv", false)]
        [DataRow("192.168.0.10/48", false)] // CIDR out of range
        [DataRow("192.168.60.26-192.168.60.22", false)] // big to lower

        [DataRow("192.168.60.13", true)]
        [DataRow("fe80::d503:4ee:3882:c586", true)]
        [DataRow("fe80:db8::dead:beaf%eth2", true)]
        [DataRow("219.165.64.0/19", true)]
        [DataRow("219.165.64.73/32", true)]
        [DataRow("192.168.1.0/255.255.255.0", true)]
        [DataRow("192.168.60.26-192.168.60.37", true)]
        [DataRow("fe80:dead::beaf:a%eth2-fe80:dead::beaf:f%eth2", true)]
        [DataRow("fe80:dead::beaf:f%eth2-fe80:dead::beaf:a%eth2", false)]
        public void TryParse(string input, bool expectedReturn)
        {
            Console.WriteLine("TestCase: \"{0}\", Expected: {1}", input, expectedReturn);
            IPAddressRange temp;
            var result = IPAddressRange.TryParse(input, out temp);
            result.Is(expectedReturn);
            if (expectedReturn)
            {
                temp.IsNotNull();
            }
            else
            {
                temp.IsNull();
            }
        }
    }
}
