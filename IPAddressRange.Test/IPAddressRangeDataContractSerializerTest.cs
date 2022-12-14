using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTools;

#if NET452_OR_GREATER

namespace IPRange.Test
{
    [TestClass]
    public class IPAddressRangeDataContractSerializerTest
    {
        [TestMethod]
        public void SerializeTest()
        {
            var ms1 = new MemoryStream();
            new DataContractJsonSerializer(typeof(IPAddressRange))
                .WriteObject(ms1, IPAddressRange.Parse("192.168.0.0/24"));
            Encoding.ASCII.GetString(ms1.ToArray())
                .Is(@"{""Begin"":""192.168.0.0"",""End"":""192.168.0.255""}");

            var ms2 = new MemoryStream();
            new DataContractJsonSerializer(typeof(IPAddressRange))
                .WriteObject(ms2, IPAddressRange.Parse("::7 - ::9"));
            Encoding.ASCII.GetString(ms2.ToArray())
                .Is(@"{""Begin"":""::7"",""End"":""::9""}");
        }

        [TestMethod]
        public void DeserializeTest()
        {
            MemoryStream toMemStrm(string str) => new MemoryStream(Encoding.ASCII.GetBytes(str));

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
}
#endif