IPAddressRange Class Library [![Build status](https://ci.appveyor.com/api/projects/status/9xp7ahar7afcjk3l?svg=true)](https://ci.appveyor.com/project/jsakamoto/ipaddressrange) [![NuGet Package](https://img.shields.io/nuget/v/IPAddressRange.svg)](https://www.nuget.org/packages/IPAddressRange/)
=============

This library allows you to parse range of IP address string such as "192.168.0.0/24" and "192.168.0.0/255.255.255.0" and "192.168.0.0-192.168.0.255", and can contains check.
This library supports both IPv4 and IPv6.

Example
-------

```csharp
using NetTools;
...
// rangeA.Begin is "192.168.0.0", and rangeA.End is "192.168.0.255".
var rangeA = IPAddressRange.Parse("192.168.0.0/255.255.255.0");
rangeA.Contains(IPAddress.Parse("192.168.0.34")); // is True.
rangeA.Contains(IPAddress.Parse("192.168.10.1")); // is False.
rangeA.ToCidrString(); // is 192.168.0.0/24

// rangeB.Begin is "192.168.0.10", and rangeB.End is "192.168.10.20".
var rangeB1 = IPAddressRange.Parse("192.168.0.10 - 192.168.10.20");
rangeB1.Contains(IPAddress.Parse("192.168.3.45")); // is True.
rangeB1.Contains(IPAddress.Parse("192.168.0.9")); // is False.

// Support shortcut range description. 
// ("192.168.10.10-20" means range of begin:192.168.10.10 to end:192.168.10.20.)
var rangeB2 = IPAddressRange.Parse("192.168.10.10-20");

// Support CIDR expression and IPv6.
var rangeC = IPAddressRange.Parse("fe80::/10"); 
rangeC.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586%3")); // is True.
rangeC.Contains(IPAddress.Parse("::1")); // is False.

// "Contains()" method also support IPAddressRange argument.
var rangeD1 = IPAddressRange.Parse("192.168.0.0/16");
var rangeD2 = IPAddressRange.Parse("192.168.10.0/24");
rangeD1.Contains(rangeD2); // is True.

// IEnumerable<IPAddress> support, it's lazy evaluation.
foreach (var ip in IPAddressRange.Parse("192.168.0.1/23"))
{
    Console.WriteLine(ip);
}

// You can use LINQ via "AsEnumerable()" method.
var longValues = IPAddressRange.Parse("192.168.0.1/23")
  .AsEnumerable()
  .Select(ip => BitConvert.ToInt32(ip.GetAddressBytes(), 0))
  .Select(adr => adr.ToString("X8"));
Console.WriteLine(string.Join(",", longValues);

// Constructors from IPAddress objects.
var ipBegin = IPAddress.Parse("192.168.0.1");
var ipEnd = IPAddress.Parse("192.168.0.128");
var ipSubnet = IPAddress.Parse("255.255.255.0");

var rangeE = new IPAddressRange(); // This means "0.0.0.0/0".
var rangeF = new IPAddressRange(ipBegin, ipEnd);
var rangeG = new IPAddressRange(ipBegin, maskLength: 24);
var rangeH = new IPAddressRange(ipBegin, IPAddressRange.SubnetMaskLength(ipSubnet));

// Calculates Cidr subnets
var rangeI = IPAddressRange.Parse("192.168.0.0-192.168.0.254");
rangeI.ToCidrString();  // is 192.168.0.0/24
```

Release Note
------------

- **v.4.0.0** - Implement "IEquatable&lt;T&gt;" interface and "GetHashCode()".
- **v.3.2.2** - Fix: Parsing non-linear subnet mask should be failed.
- **v.3.2.1** - Fix implementation problem (Remove useless GetAddressBytes)
- **v.3.2.0**
  - Fix: Bits.GE/LE operations are reversed.(use GtE/LtE instead.)
  - Improve: Bts.GtE/LtE methods are more faster now.
- **v.3.1.1** - Fix: Error if parsing IP address includes some spaces.
- **v.3.1.0** - Support parsing for IPv4 mapped to IPv6 address range.
- **v.3.0.0** - Sign the assembly. (make it to strong-named assembly.)
- **v.2.2.0** - Enhance: IPv4 shortcut notation support (like "192.168.0.0-255").
- **v.2.1.1** - Fix: Error if parsing IP address with scope id in the end.
- **v.2.1.0** - Enhance: Add "IsEqual" and "Decrement" methods to "Bits" utility class.
- **v.2.0.0**
  - [BREAKING CHANGE] Truncate .NET 4.0 support.
  - Enhance: can serialize/deserialize json text by JSON.NET
- **v.1.6.2** - Support: .NET Standard 1.4 (.NET Core) and UWP
- **v.1.6.1** - Fix: Add some parameter checks for throwing ArgumentNullException
- **v.1.6.0** - Enhance: Add "ToCidrString()" and "GetprefixLength()" method.
- **v.1.5.0** - Enhance: Add constructors variation / Save memory usage.
- **v.1.4.0** - Enhance: Add IEnumerable<IPAddress> support.
- **v.1.3.0** - Enhance: support both "hyphen (-)" (0x002D) and "dash (–)" (0x2013) at begin-- end format.
- **v.1.2.1** - Fix: "Parse()" and "TryParse()" methods throw IndexOutOfRangeException when  bit- mask length is invalid at CIDR format.
- **v.1.2.0** - Enhance: Add "Parse()" and "TryParse()" methods. (Instead, the constructor - which has one string argument is obsoleted.)
- **v.1.1.0** - Enhance: Add "Contains()" method overload version of IPAddressRange argument.
- **v.1.0.6** - Fix Package: Repackage with Release build. (1.0.5 was Debug build)
- **v.1.0.5** - Fix: IPv4 CIDR range ".../32" causes crush.
- **v.1.0.4** - Enhance: support bit mask range not only CIDR range.
- **v.1.0.3** - Fix: Can not parse the address which contains hex string.

License
-------
[Mozilla Public License 2.0](https://github.com/jsakamoto/ipaddressrange/blob/master/LICENSE)
