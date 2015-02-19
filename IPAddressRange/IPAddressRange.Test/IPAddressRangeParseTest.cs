using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTools;

namespace IPRange.Test
{
    [TestClass]
    public class IPAddressRangeParseTest
    {
        [TestMethod]
        public void ParseSucceeds_IPV4()
        {
            var range = IPAddressRange.Parse("192.168.60.13");
            range.IsNotNull();
        }

        [TestMethod]
        public void ParseSucceeds_IPV6()
        {
            var range = IPAddressRange.Parse("fe80::d503:4ee:3882:c586");
            range.IsNotNull();
        }

        [TestMethod]
        public void ParseSucceeds_IPV4_Cipdr()
        {
            var range = IPAddressRange.Parse("219.165.64.0/19");
            range.IsNotNull();
        }

        [TestMethod]
        public void ParseSucceeds_IPV4_Cipdr_Max()
        {
            var range = IPAddressRange.Parse("219.165.64.73/32");
            range.IsNotNull();
        }

        [TestMethod]
        public void ParseSucceeds_IPV4_Cipdr_BitMask()
        {
            var range = IPAddressRange.Parse("192.168.1.0/255.255.255.0");
            range.IsNotNull();
        }

        [TestMethod]
        public void ParseSucceeds_IPV4_Cipdr_Begin_To_End()
        {
            var range = IPAddressRange.Parse("192.168.60.26-192.168.60.37");
            range.IsNotNull();
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_EmptyString_Fails()
        {
            IPAddressRange.Parse("");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_InValidString_Fails()
        {
            IPAddressRange.Parse("gvvdv");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_CIDR_OutOfRange()
        {
            IPAddressRange.Parse("192.168.0.10/48");
        }

        [TestMethod]
        public void TryParse_Empty_String()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("", out temp);
            result.Is(false);
            temp.IsNull();
        }

        [TestMethod]
        public void TryParse_InValid_String()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("fdfv", out temp);
            result.Is(false);
            temp.IsNull();
        }

        [TestMethod]
        public void TryParse_CIDR_OutOfRange()
        {
            var ipadr = default(IPAddressRange);
            IPAddressRange.TryParse("192.168.0.10/48", out ipadr).Is(false);
            ipadr.IsNull();
        }

        [TestMethod]
        public void TryParse_IPV4()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("192.168.60.13", out temp);
            result.Is(true);
            temp.IsNotNull();
        }

        [TestMethod]
        public void TryParse_IPV6()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("fe80::d503:4ee:3882:c586", out temp);
            result.Is(true);
            temp.IsNotNull();
        }

        [TestMethod]
        public void TryParse_IPV4_Cipdr()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("219.165.64.0/19", out temp);
            result.Is(true);
            temp.IsNotNull();
        }

        [TestMethod]
        public void TryParse_IPV4_Cipdr_Max()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("219.165.64.73/32", out temp);
            result.Is(true);
            temp.IsNotNull();
        }

        [TestMethod]
        public void TryParse_IPV4_Cipdr_BitMask()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("192.168.1.0/255.255.255.0", out temp);
            result.Is(true);
            temp.IsNotNull();
        }

        [TestMethod]
        public void TryParse_IPV4_Cipdr_Begin_To_End()
        {
            IPAddressRange temp;
            var result = IPAddressRange.TryParse("192.168.60.26-192.168.60.37", out temp);
            result.Is(true);
            temp.IsNotNull();
        }
    }
}
