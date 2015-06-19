using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTools;

namespace IPRange.Test
{
    [TestClass]
    public class IPAddressRangeParseTest
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        [TestCase("192.168.60.13", "192.168.60.13", "192.168.60.13")]
        [TestCase("  192.168.60.13  ", "192.168.60.13", "192.168.60.13")]   
        [TestCase("fe80::d503:4ee:3882:c586", "fe80::d503:4ee:3882:c586", "fe80::d503:4ee:3882:c586")]
        [TestCase("  fe80::d503:4ee:3882:c586  ", "fe80::d503:4ee:3882:c586", "fe80::d503:4ee:3882:c586")]
        [TestCase("3232252004", "192.168.64.100", "192.168.64.100")] // decimal - new 
        [TestCase("  3232252004  ", "192.168.64.100", "192.168.64.100")] // decimal - new 

        [TestCase("219.165.64.0/19", "219.165.64.0", "219.165.95.255")]
        [TestCase("  219.165.64.0  /  19  ", "219.165.64.0", "219.165.95.255")]   
        [TestCase("192.168.1.0/255.255.255.0", "192.168.1.0", "192.168.1.255")]
        [TestCase("  192.168.1.0  /  255.255.255.0  ", "192.168.1.0", "192.168.1.255")]
        [TestCase("3232252004/24", "192.168.64.0", "192.168.64.255")] // decimal - new 
        [TestCase("  3232252004  /  24  ", "192.168.64.0", "192.168.64.255")] // decimal - new 

        [TestCase("192.168.60.26–192.168.60.37", "192.168.60.26", "192.168.60.37")]
        [TestCase("  192.168.60.26  –  192.168.60.37  ", "192.168.60.26", "192.168.60.37")]
        [TestCase("fe80::c586-fe80::c600", "fe80::c586", "fe80::c600")]
        [TestCase("  fe80::c586  -  fe80::c600  ", "fe80::c586", "fe80::c600")]
        [TestCase("3232252004-3232252504", "192.168.64.100", "192.168.66.88")]
        [TestCase("  3232252004  -  3232252504  ", "192.168.64.100", "192.168.66.88")]
        
        // with "dash (–)" (0x2013) is also support.
        [TestCase("192.168.61.26–192.168.61.37", "192.168.61.26", "192.168.61.37")]
        [TestCase("  192.168.61.26  –  192.168.61.37  ", "192.168.61.26", "192.168.61.37")]
        [TestCase("fe80::c586–fe80::c600", "fe80::c586", "fe80::c600")]
        [TestCase("  fe80::c586  –  fe80::c600  ", "fe80::c586", "fe80::c600")]
        [TestCase("3232252004–3232252504", "192.168.64.100", "192.168.66.88")]
        [TestCase("  3232252004  –  3232252504  ", "192.168.64.100", "192.168.66.88")]
        public void ParseSucceeds()
        {
            TestContext.Run((string input, string expectedBegin, string expectedEnd) =>
            {
                Console.WriteLine("TestCase: \"{0}\", Expected Begin: {1}, End: {2}", input, expectedBegin, expectedEnd);
                var range = IPAddressRange.Parse(input);
                range.IsNotNull();
                Console.WriteLine("  Result: Begin: {0}, End: {1}", range.Begin, range.End);
                range.Begin.ToString().Is(expectedBegin);
                range.End.ToString().Is(expectedEnd);
            });
        }

        [TestMethod]
        [TestCase(null, typeof(ArgumentNullException))] 
        [TestCase("", typeof(FormatException))]
        [TestCase(" ", typeof(FormatException))]
        [TestCase("gvvdv", typeof(FormatException))]
        [TestCase("192.168.0.10/48", typeof(FormatException))] // out of CIDR range 
        [TestCase("192.168.0.10-192.168.0.5", typeof(ArgumentException))] // bigger to lower
        [TestCase("10.256.1.1", typeof(FormatException))] // invalid ip
        public void ParseFails()
        {
            TestContext.Run((string input, Type expectedException) =>
            {
                Console.WriteLine("TestCase: \"{0}\", Expected Exception: {1}", input, expectedException.Name);
                try
                {
                    IPAddressRange.Parse(input);
                    Assert.Fail("Expected exception of type {0} to be thrown for input \"{1}\"", expectedException.Name,input);
                }
                catch (AssertFailedException)
                {
                    throw; // allow Assert.Fail to pass through 
                }
                catch (Exception ex)
                {
                    ex.GetType().Is(expectedException);
                }
            });
        }

        [TestMethod]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("fdfv", false)]
        [TestCase("192.168.0.10/48", false)] // CIDR out of range

        [TestCase("192.168.60.13", true)]
        [TestCase("fe80::d503:4ee:3882:c586", true)]
        [TestCase("219.165.64.0/19", true)]
        [TestCase("219.165.64.73/32", true)]
        [TestCase("192.168.1.0/255.255.255.0", true)]
        [TestCase("192.168.60.26-192.168.60.37", true)]
        public void TryParse()
        {
            TestContext.Run((string input, bool expectedReturn) =>
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
            });
        }
    }
}
