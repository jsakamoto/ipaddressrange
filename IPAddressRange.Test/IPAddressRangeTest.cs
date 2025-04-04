﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NetTools;

namespace IPRange.Test
{
    [TestClass]
    public class IPAddressRangeTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CtorTest_Empty()
        {
            var range = new IPAddressRange();
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("0.0.0.0");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("0.0.0.0");
        }

        [TestMethod]
        public void CtorTest_Single()
        {
            var range = new IPAddressRange(IPAddress.Parse("192.168.0.88"));
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("192.168.0.88");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("192.168.0.88");
            range.Contains(range.Begin).Is(true);
        }

        [TestMethod]
        public void CtorTest_MaskLength()
        {
            var range = new IPAddressRange(IPAddress.Parse("192.168.0.80"), 24);
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("192.168.0.0");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("192.168.0.255");
            range.Contains(range.Begin).Is(true);
        }

        [TestMethod]
        public void CtorTest_IPv6_BeginEndAddresses()
        {
            var range = new IPAddressRange(
                begin: IPAddress.Parse("ff80::1"),
                end: IPAddress.Parse("ff80::34"));
            range.Begin.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.Begin.ToString().Is("ff80::1");
            range.End.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.End.ToString().Is("ff80::34");
            range.Contains(range.Begin).Is(true);
        }

        [TestMethod]
        public void CtorTest_IPv6_BeginEndAddresses_with_ScopeId()
        {
            var range = new IPAddressRange(
                begin: IPAddress.Parse("ff80::56%23"),
                end: IPAddress.Parse("ff80::789%23"));
            range.Begin.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.Begin.ToString().Is("ff80::56");
            range.End.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.End.ToString().Is("ff80::789");
            range.Contains(range.Begin).Is(true);
        }

        [TestMethod]
        public void ParseTest_IPv4_Uniaddress()
        {
            var range = IPAddressRange.Parse("192.168.60.13");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("192.168.60.13");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("192.168.60.13");
        }

        [TestMethod]
        public void ParseTest_IPv4_CIDR()
        {
            var range = IPAddressRange.Parse("219.165.64.0/19");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("219.165.64.0");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("219.165.95.255");
        }

        [TestMethod]
        public void ParseTest_IPv4_CIDR_Max()
        {
            var range = IPAddressRange.Parse("219.165.64.73/32");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("219.165.64.73");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("219.165.64.73");
        }

        [TestMethod]
        public void ParseTest_IPv4_Bitmask()
        {
            var range = IPAddressRange.Parse("192.168.1.0/255.255.255.0");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("192.168.1.0");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("192.168.1.255");
        }

        [TestMethod]
        public void ParseTest_IPv4_Begin_to_End()
        {
            var range = IPAddressRange.Parse("192.168.60.26-192.168.60.37");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetwork);
            range.Begin.ToString().Is("192.168.60.26");
            range.End.AddressFamily.Is(AddressFamily.InterNetwork);
            range.End.ToString().Is("192.168.60.37");
        }

        [DataTestMethod]
        [DataRow("192.168.1.1", 1u)]
        [DataRow("192.168.1.1/32", 1u)]
        [DataRow("192.168.1.0/24", 256u)]
        [DataRow("173.0.0.0/8", 16777216u)]
        [DataRow("128.0.0.0/1", 2147483648u)]
        public void AddressCountTest_IPv4(string rangeStr, uint expectedAddressCount)
        {
            var range = IPAddressRange.Parse(rangeStr);

            range.AddressCount.Is(expectedAddressCount);
        }

        [TestMethod]
        public void AddressCountTest_IPv4_AllAddresses()
        {
            var fullIPv4Range = IPAddressRange.Parse("0.0.0.0/0");

            fullIPv4Range.AddressCount.Is(BigInteger.Pow(2, 32));
        }

        [DataTestMethod]
        [DataRow("2001:0db8:85a3:0000:0000:8a2e:0370:7334", "1")]
        [DataRow("2001:0db8:85a3:0000:0000:8a2e:0370:7334/128", "1")]
        [DataRow("2001:0db8:85a3:0000:0000:8a2e::/96", "4294967296")]
        [DataRow("2001:0db8:85a3::/64", "18446744073709551616")]
        public void AddressCountTest_IPv6(string rangeStr, string expectedAddressCountStr)
        {
            var range = IPAddressRange.Parse(rangeStr);

            range.AddressCount.Is(BigInteger.Parse(expectedAddressCountStr));
        }

        [TestMethod]
        public void AddressCountTest_IPv6_AllAddresses()
        {
            var fullIPv6Range = IPAddressRange.Parse("::/0");

            fullIPv6Range.AddressCount.Is(BigInteger.Pow(2, 128));
        }

        [TestMethod]
        public void ContainsTest_IPv4()
        {
            var range = IPAddressRange.Parse("192.168.60.26-192.168.60.37");

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
            var range = IPAddressRange.Parse("192.168.60.26-192.168.60.37");

            range.Contains(IPAddress.Parse("c0a8:3c1a::")).Is(false);
        }

        [TestMethod]
        public void ContainsTest_with_IPV4andv6_is_False_ever()
        {
            var fullRangeIPv6 = IPAddressRange.Parse("::-fff:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
            fullRangeIPv6.Contains(IPAddressRange.Parse("192.168.0.0/24")).Is(false);

            var fullRangeIPv4 = IPAddressRange.Parse("0.0.0.0-255.255.255.255");
            fullRangeIPv4.Contains(IPAddressRange.Parse("::1-::2")).Is(false);
        }

        [TestMethod]
        public void ContainsTest_Range_is_True_IPv4()
        {
            var range = IPAddressRange.Parse("192.168.60.26-192.168.60.37");
            var range1_same = IPAddressRange.Parse("192.168.60.26-192.168.60.37");
            var range2_samestart = IPAddressRange.Parse("192.168.60.26-192.168.60.30");
            var range3_sameend = IPAddressRange.Parse("192.168.60.36-192.168.60.37");
            var range4_subset = IPAddressRange.Parse("192.168.60.29-192.168.60.32");

            range.Contains(range1_same).Is(true);
            range.Contains(range2_samestart).Is(true);
            range.Contains(range3_sameend).Is(true);
            range.Contains(range4_subset).Is(true);
        }

        [TestMethod]
        public void ContainsTest_Range_is_False_IPv4()
        {
            var range = IPAddressRange.Parse("192.168.60.29-192.168.60.32");
            var range1_overLeft = IPAddressRange.Parse("192.168.60.26-192.168.70.1");
            var range2_overRight = IPAddressRange.Parse("192.168.50.1-192.168.60.37");
            var range3_outOfLeft = IPAddressRange.Parse("192.168.50.30-192.168.50.31");
            var range4_outOfRight = IPAddressRange.Parse("192.168.70.30-192.168.70.31");

            range.Contains(range1_overLeft).Is(false);
            range.Contains(range2_overRight).Is(false);
            range.Contains(range3_outOfLeft).Is(false);
            range.Contains(range4_outOfRight).Is(false);
        }

        [TestMethod]
        public void ParseTest_IPv6_CIDR()
        {
            var range = IPAddressRange.Parse("fe80::/10");
            range.Begin.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.Begin.ToString().Is("fe80::");
            range.End.AddressFamily.Is(AddressFamily.InterNetworkV6);
            range.End.ToString().Is("febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
        }

        [TestMethod]
        public void ContainsTest_IPv6()
        {
            var range = IPAddressRange.Parse("FE80::/10");

            range.Contains(IPAddress.Parse("::1")).Is(false);
            range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586")).Is(true);
            range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586%3")).Is(true);

            range = IPAddressRange.Parse("::/0");
            range.Contains(IPAddress.Parse("::1")).Is(true);
        }

        [TestMethod]
        public void ContainsTest_IPv6_with_ScopeId()
        {
            var range = IPAddressRange.Parse("FE80::%eth0/10");

            range.Contains(IPAddress.Parse("::1")).Is(false);
            range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586")).Is(true);
            range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586%4")).Is(true);
        }

        [TestMethod]
        public void ContainsTest_RewriteProperties_IPv4tov6()
        {
            var range = new IPAddressRange();
            ContainsTest_RewriteProperties_IPv4(range);
            ContainsTest_RewriteProperties_IPv6(range);
        }

        [TestMethod]
        public void ContainsTest_RewriteProperties_IPv6tov4()
        {
            var range = new IPAddressRange();
            ContainsTest_RewriteProperties_IPv4(range);
            ContainsTest_RewriteProperties_IPv6(range);
        }

        private static void ContainsTest_RewriteProperties_IPv4(IPAddressRange range)
        {
            range.Begin = IPAddress.Parse("192.168.60.26");
            range.End = IPAddress.Parse("192.168.60.37");

            range.Contains(IPAddress.Parse("192.168.60.25")).Is(false);
            range.Contains(IPAddress.Parse("192.168.60.26")).Is(true);
            range.Contains(IPAddress.Parse("192.168.60.27")).Is(true);

            range.Contains(IPAddress.Parse("192.168.60.36")).Is(true);
            range.Contains(IPAddress.Parse("192.168.60.37")).Is(true);
            range.Contains(IPAddress.Parse("192.168.60.38")).Is(false);
        }

        private static void ContainsTest_RewriteProperties_IPv6(IPAddressRange range)
        {
            range.Begin = IPAddress.Parse("fe80::");
            range.End = IPAddress.Parse("fe80::d503:4ee:3882:c586");

            range.Contains(IPAddress.Parse("::1")).Is(false);
            range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586")).Is(true);
            range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586%3")).Is(true);

            range = IPAddressRange.Parse("::/0");
            range.Contains(IPAddress.Parse("::1")).Is(true);
        }

        [TestMethod]
        public void ContainsTest_RewriteProperties_Different_AddressFamily()
        {
            var ipv4 = IPAddress.Parse("192.168.60.26");
            var ipv6 = IPAddress.Parse("fe80::d503:4ee:3882:c586");

            AssertEx.Throws<InvalidOperationException>(() =>
            {
                new IPAddressRange { Begin = ipv4, End = ipv6 }
                    .Contains(IPAddress.Parse("192.168.60.25"));
            }).Message.Is("Both Begin and End properties must be of the same address family");

            AssertEx.Throws<InvalidOperationException>(() =>
            {
                new IPAddressRange { Begin = ipv6, End = ipv4 }
                    .Contains(IPAddress.Parse("192.168.60.25"));
            }).Message.Is("Both Begin and End properties must be of the same address family");
        }

        [TestMethod]
        public void ContainsTest_RewriteProperties_BeginIsEnd()
        {
            var beginIPv4 = IPAddress.Parse("192.168.60.26");
            var endIPv4 = IPAddress.Parse("192.168.60.37");
            AssertEx.Throws<InvalidOperationException>(() =>
            {
                new IPAddressRange { Begin = endIPv4, End = beginIPv4 }
                    .Contains(IPAddress.Parse("192.168.60.25"));
            }).Message.Is("Begin must be smaller than the End");

            var beginIPv6 = IPAddress.Parse("fe80::");
            var endIPv6 = IPAddress.Parse("fe80::d503:4ee:3882:c586");
            AssertEx.Throws<InvalidOperationException>(() =>
            {
                new IPAddressRange { Begin = endIPv6, End = beginIPv6 }
                    .Contains(IPAddress.Parse("192.168.60.25"));
            }).Message.Is("Begin must be smaller than the End");
        }

        [TestMethod]
        public void ContainsTest_Range_is_True_IPv6()
        {
            var range = IPAddressRange.Parse("fe80::/10");
            var range1_same = IPAddressRange.Parse("fe80::/10");
            var range2_samestart = IPAddressRange.Parse("fe80::-fe80::d503:4ee:3882:c586");
            var range3_sameend = IPAddressRange.Parse("fe80::d503:4ee:3882:c586-febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
            var range4_subset = IPAddressRange.Parse("fe80::d503:4ee:3882:c586-fe80::d504:4ee:3882:c586");

            range.Contains(range1_same).Is(true);
            range.Contains(range2_samestart).Is(true);
            range.Contains(range3_sameend).Is(true);
            range.Contains(range4_subset).Is(true);
        }

        [TestMethod]
        public void ContainsTest_Range_is_False_IPv6()
        {
            var range = IPAddressRange.Parse("fe80::d503:4ee:3882:c586-fe80::d504:4ee:3882:c586");
            var range1_overLeft = IPAddressRange.Parse("fe80::d502:4ee:3882:c586-fe80::d503:4ee:3882:c586");
            var range2_overRight = IPAddressRange.Parse("fe80::d503:4ef:3882:c586-fe80::d505:4ee:3882:c586");
            var range3_outOfLeft = IPAddressRange.Parse("fe80::d501:4ee:3882:c586-fe80::d502:4ee:3882:c586");
            var range4_outOfRight = IPAddressRange.Parse("fe80::d505:4ee:3882:c586-fe80::d506:4ee:3882:c586");

            range.Contains(range1_overLeft).Is(false);
            range.Contains(range2_overRight).Is(false);
            range.Contains(range3_outOfLeft).Is(false);
            range.Contains(range4_outOfRight).Is(false);
        }

        [TestMethod]
        public void ContainsTest_IPv4_mappedTo_IPv6()
        {
            var range = IPAddressRange.Parse("::ffff:192.168.10.20-::ffff:192.168.11.20");

            range.Contains(IPAddress.Parse("::ffff:192.168.10.19")).Is(false);
            range.Contains(IPAddress.Parse("::ffff:192.168.10.20")).Is(true);
            range.Contains(IPAddress.Parse("::ffff:192.168.11.20")).Is(true);
            range.Contains(IPAddress.Parse("::ffff:192.168.11.21")).Is(false);

            range.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586")).Is(false);
            range.Contains(IPAddress.Parse("192.168.10.20")).Is(false);

            var range1_overLeft = IPAddressRange.Parse("::ffff:192.168.10.19-::ffff:192.168.10.21");
            var range2_overRight = IPAddressRange.Parse("::ffff:192.168.11.19-::ffff:192.168.11.21");
            var range3_outOfLeft = IPAddressRange.Parse("::ffff:192.168.10.18-::ffff:192.168.10.19");
            var range4_outOfRight = IPAddressRange.Parse("::ffff:192.168.11.21-::ffff:192.168.11.22");
            var range5_justInside = IPAddressRange.Parse("::ffff:192.168.10.20-::ffff:192.168.11.20");
            range.Contains(range1_overLeft).Is(false);
            range.Contains(range2_overRight).Is(false);
            range.Contains(range3_outOfLeft).Is(false);
            range.Contains(range4_outOfRight).Is(false);
            range.Contains(range5_justInside).Is(true);
        }

        [TestMethod]
        public void SubnetMaskLengthTest_Valid()
        {
            var range = new IPAddressRange(IPAddress.Parse("192.168.75.23"), IPAddressRange.SubnetMaskLength(IPAddress.Parse("255.255.254.0")));
            range.Begin.ToString().Is("192.168.74.0");
            range.End.ToString().Is("192.168.75.255");
        }

        [TestMethod]
        public void SubnetMaskLengthTest_Invalid()
        {
            AssertEx.Throws<ArgumentException>(() =>
                new IPAddressRange(IPAddress.Parse("192.168.75.23"), IPAddressRange.SubnetMaskLength(IPAddress.Parse("255.255.54.0"))));
        }

        [TestMethod]
        public void Enumerate_IPv4()
        {
            var ips = IPAddressRange.Parse("192.168.60.253-192.168.61.2").AsEnumerable().ToArray();
            ips.Is(new IPAddress[]
            {
                IPAddress.Parse("192.168.60.253"),
                IPAddress.Parse("192.168.60.254"),
                IPAddress.Parse("192.168.60.255"),
                IPAddress.Parse("192.168.61.0"),
                IPAddress.Parse("192.168.61.1"),
                IPAddress.Parse("192.168.61.2"),
            });
        }

        [TestMethod]
        public void Enumerate_IPv6()
        {
            var ips = IPAddressRange.Parse("fe80::d503:4ee:3882:c586/120").AsEnumerable().ToArray();
            ips.Length.Is(256);
            ips.First().Is(IPAddress.Parse("fe80::d503:4ee:3882:c500"));
            ips.Last().Is(IPAddress.Parse("fe80::d503:4ee:3882:c5ff"));
        }

        [TestMethod]
        public void EnumerateTest_With_Foreach()
        {
            foreach (var ip in IPAddressRange.Parse("192.168.60.2"))
            {
                ip.Is(IPAddress.Parse("192.168.60.2"));
            }

        }


        [TestMethod]
        [TestCase("192.168.60.2", "192.168.60.2")]
        [TestCase("192.168.60.2/24", "192.168.60.0-192.168.60.255")]
        [TestCase("fe80::d503:4ee:3882:c586", "fe80::d503:4ee:3882:c586")]
        [TestCase("fe80::d503:4ee:3882:c586/120", "fe80::d503:4ee:3882:c500-fe80::d503:4ee:3882:c5ff")]
        public void ToString_Output()
        {
            this.TestContext.Run((string input, string expected) =>
            {
                Console.WriteLine("TestCase: \"{0}\", Expected: \"{1}\"", input, expected);
                var output = IPAddressRange.Parse(input).ToString();
                Console.WriteLine("  Result: \"{0}\"", output);
                output.Is(expected);

                var parsed = IPAddressRange.Parse(output).ToString();
                parsed.Is(expected, "Output of ToString() should be usable by Parse() and result in the same output");
            });
        }

        [TestMethod]
        [TestCase("fe80::/10", 10)]
        [TestCase("192.168.0.0/24", 24)]
        [TestCase("192.168.0.0", 32)]
        [TestCase("192.168.0.0-192.168.0.0", 32)]
        [TestCase("fe80::", 128)]
        [TestCase("192.168.0.0-192.168.0.255", 24)]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 16)]
        public void GetPrefixLength_Success()
        {
            this.TestContext.Run((string input, int expected) =>
            {
                Console.WriteLine("TestCase: \"{0}\", Expected: \"{1}\"", input, expected);
                var output = IPAddressRange.Parse(input).GetPrefixLength();
                Console.WriteLine("  Result: \"{0}\"", output);
                output.Is(expected);
            });
        }

        [TestMethod]
        [TestCase("fe80::", "febf:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 10)]
        [TestCase("192.168.0.0", "192.168.0.255", 24)]
        [TestCase("192.168.0.0", "192.168.0.0", 32)]
        [TestCase("192.168.0.0", "192.168.0.0", 32)]
        [TestCase("fe80::", "fe80::", 128)]
        [TestCase("192.168.0.0", "192.168.0.255", 24)]
        [TestCase("fe80::", "fe80:ffff:ffff:ffff:ffff:ffff:ffff:ffff", 16)]
        public void GetPrefixLength_with_RewriteBeginEndProperties_Success()
        {
            var range = new IPAddressRange();
            this.TestContext.Run((string begin, string end, int expected) =>
            {
                range.Begin = IPAddress.Parse(begin);
                range.End = IPAddress.Parse(end);
                Console.WriteLine($"TestCase: \"{begin}\"~\"{end}\", Expected: \"{expected}\"");
                var output = range.GetPrefixLength();
                Console.WriteLine($"  Result: \"{output}\"");
                output.Is(expected);
            });
        }

        [TestMethod]
        [TestCase("192.168.0.0-192.168.0.254", typeof(FormatException))]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffe", typeof(FormatException))]
        public void GetPrefixLength_Failures()
        {
            this.TestContext.Run((string input, Type expectedException) =>
            {
                Console.WriteLine("TestCase: \"{0}\", Expected Exception: {1}", input, expectedException.Name);
                try
                {
                    IPAddressRange.Parse(input).GetPrefixLength();
                    Assert.Fail("Expected exception of type {0} to be thrown for input \"{1}\"", expectedException.Name, input);
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
        [TestCase("192.168.0.0", "192.168.0.254", typeof(FormatException), "fe80::", "fe80::", 128)]
        [TestCase("fe80::", "fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffe", typeof(FormatException), "192.168.0.0", "192.168.0.255", 24)]
        public void GetPrefixLength_with_RewriteBeginEndProperties_Failures()
        {
            var range = new IPAddressRange();
            this.TestContext.Run((string begin, string end, Type expectedException, string begin2, string end2, int expected) =>
            {
                Console.WriteLine($"TestCase: \"{begin}\"~\"{end}\", Expected Exception: \"{expectedException.Name}\"");
                try
                {
                    range.Begin = IPAddress.Parse(begin);
                    range.End = IPAddress.Parse(end);
                    range.GetPrefixLength();
                    Assert.Fail($"Expected exception of type {expectedException.Name} to be thrown for input \"{begin}\"~\"{end}\"");
                }
                catch (AssertFailedException)
                {
                    throw; // allow Assert.Fail to pass through 
                }
                catch (Exception ex)
                {
                    ex.GetType().Is(expectedException);
                }

                // Once it was failed, but it will be recovered with valid begin/end property.
                Console.WriteLine($"TestCase: \"{begin2}\"~\"{end2}\", Expected: \"{expected}\"");
                range.Begin = IPAddress.Parse(begin2);
                range.End = IPAddress.Parse(end2);
                var output = range.GetPrefixLength();
                Console.WriteLine($"  Result: \"{output}\"");
                output.Is(expected);
            });
        }

        [TestMethod]
        [TestCase("fe80::/10", "fe80::/10")]
        [TestCase("192.168.0.0/24", "192.168.0.0/24")]
        [TestCase("192.168.0.0", "192.168.0.0/32")]
        [TestCase("192.168.0.0-192.168.0.0", "192.168.0.0/32")]
        [TestCase("fe80::", "fe80::/128")]
        [TestCase("192.168.0.0-192.168.0.255", "192.168.0.0/24")]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:ffff", "fe80::/16")]
        public void ToCidrString_Output()
        {
            this.TestContext.Run((string input, string expected) =>
            {
                Console.WriteLine("TestCase: \"{0}\", Expected: \"{1}\"", input, expected);
                var output = IPAddressRange.Parse(input).ToCidrString();
                Console.WriteLine("  Result: \"{0}\"", output);
                output.Is(expected);
            });
        }

        [TestMethod]
        [TestCase("192.168.0.0-192.168.0.254", typeof(FormatException))]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffe", typeof(FormatException))]
        public void ToCidrString_ThrowsOnNonCidr()
        {
            this.TestContext.Run((string input, Type expectedException) =>
            {
                Console.WriteLine("TestCase: \"{0}\", Expected Exception: {1}", input, expectedException.Name);
                try
                {
                    IPAddressRange.Parse(input).ToCidrString();
                    Assert.Fail("Expected exception of type {0} to be thrown for input \"{1}\"", expectedException.Name, input);
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
        [TestCase("192.168.0.0-192.168.0.254")]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffe")]
        public void GetHashCode_SameRange_HashCodesAreSame()
        {
            this.TestContext.Run((string input) =>
            {
                Console.WriteLine("TestCase: \"{0}\"", input);
                var range1 = IPAddressRange.Parse(input);
                var range2 = IPAddressRange.Parse(input);
                range1.GetHashCode().Is(range2.GetHashCode());
            });
        }

        [TestMethod]
        [TestCase("192.168.0.0-192.168.0.254", "192.168.0.1-192.168.0.254")]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffe", "fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffd")]
        public void GetHashCode_DifferentRanges_HashCodesAreDifferent()
        {
            this.TestContext.Run((string input1, string input2) =>
            {
                Console.WriteLine("TestCase: \"{0}\" and \"{1}\"", input1, input2);
                var range1 = IPAddressRange.Parse(input1);
                var range2 = IPAddressRange.Parse(input2);
                range1.GetHashCode().IsNot(range2.GetHashCode());
            });
        }

        [TestMethod]
        [TestCase("192.168.0.0-192.168.0.254")]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffe")]
        public void Equals_SameRange_ReturnsTrue()
        {
            this.TestContext.Run((string input) =>
            {
                Console.WriteLine("TestCase: \"{0}\"", input);
                var range1 = IPAddressRange.Parse(input);
                var range2 = IPAddressRange.Parse(input);
                range1.Equals(range2).IsTrue();
            });
        }

        [TestMethod]
        [TestCase("192.168.0.0-192.168.0.254", "192.168.0.1-192.168.0.254")]
        [TestCase("fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffe", "fe80::-fe80:ffff:ffff:ffff:ffff:ffff:ffff:fffd")]
        public void Equals_SameRange_ReturnsFalse()
        {
            this.TestContext.Run((string input1, string input2) =>
            {
                Console.WriteLine("TestCase: \"{0}\" and \"{1}\"", input1, input2);
                var range1 = IPAddressRange.Parse(input1);
                var range2 = IPAddressRange.Parse(input2);
                range1.Equals(range2).IsFalse();
            });
        }

        [TestMethod]
        public void Equals_WithNull_ReturnsFalse()
        {
            var range1 = IPAddressRange.Parse("192.168.0.0/24");
            var range2 = default(IPAddressRange);
            range1.Equals(range2).IsFalse();
        }

        [TestMethod]
        public void Count_IPv4_Test()
        {
            var ipAddressRange = IPAddressRange.Parse("10.0.0.0/8");
            ipAddressRange.AsEnumerable().Count().Is(16777216);
        }

        [TestMethod]
        public void Count_IPv6_Test()
        {
            var ipAddressRange = IPAddressRange.Parse("fe80::0000:0000-fe80::0100:0001");
            ipAddressRange.AsEnumerable().Count().Is(16777218);
        }

        [TestMethod]
        public void CopyTo_IPv4_Test()
        {
            // Given
            var ipAddressRange = IPAddressRange
                .Parse("255.255.255.254 - 255.255.255.255")
                .AsEnumerable() as ICollection<IPAddress>;

            // When
            var ipaddresses = new IPAddress[2];
            ipAddressRange.CopyTo(ipaddresses, 0);

            // Then
            ipaddresses.Select(ipadr => ipadr.ToString())
                .Is("255.255.255.254",
                    "255.255.255.255");
        }

        [TestMethod]
        public void CopyTo_IPv6_Test()
        {
            // Given
            var ipAddressRange = IPAddressRange
                .Parse("ffff:ffff:ffff:ffff:ffff:ffff:ffff:fffe - ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")
                .AsEnumerable() as ICollection<IPAddress>;

            // When
            var ipaddresses = new IPAddress[2];
            ipAddressRange.CopyTo(ipaddresses, 0);

            // Then
            ipaddresses.Select(ipadr => ipadr.ToString())
                .Is("ffff:ffff:ffff:ffff:ffff:ffff:ffff:fffe",
                    "ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff");
        }
    }
}
