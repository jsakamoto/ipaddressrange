using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTools;
using Newtonsoft.Json;

namespace IPRange.Test
{
    [TestClass]
    public class IPAddressRangeJsonTest
    {
        [TestMethod]
        public void ConvertToJson_IPv4()
        {
            var range = IPAddressRange.Parse("192.168.0.0/24");
            JsonConvert.SerializeObject(range).Is(@"{""Begin"":""192.168.0.0"",""End"":""192.168.0.255""}");
        }

        [TestMethod]
        public void ConvertToJson_IPv6()
        {
            var range = IPAddressRange.Parse("fe80::/10");
            JsonConvert.SerializeObject(range).Is(@"{""Begin"":""fe80::"",""End"":""febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff""}");
        }

        [TestMethod]
        public void ConvertFromJson_IPv4()
        {
            var range = JsonConvert.DeserializeObject<IPAddressRange>(@"{""Begin"":""219.165.64.0"",""End"":""219.165.95.255""}");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("219.165.64.0");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("219.165.95.255");
        }

        [TestMethod]
        public void ConvertFromJson_IPv6()
        {
            var range = JsonConvert.DeserializeObject<IPAddressRange>(@"{""Begin"":""fe80::"",""End"":""febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff""}");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.Begin.ToString().Is("fe80::");
            range.End.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.End.ToString().Is("febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
        }
    }
}
