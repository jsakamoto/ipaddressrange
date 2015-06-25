IPAddressRange Class Library
=============

This library allows you to parse range of IP address string such as "192.168.0.0/24" and "192.168.0.0/255.255.255.0" and "192.168.0.0-192.168.0.255", and can conatins check.
This library supports both IPv4 and IPv6.

[![Build status](https://ci.appveyor.com/api/projects/status/9xp7ahar7afcjk3l?svg=true)](https://ci.appveyor.com/project/jsakamoto/ipaddressrange) [![NuGet Package](https://img.shields.io/nuget/v/IPAddressRange.svg)](https://www.nuget.org/packages/IPAddressRange/)

Example
-------

```csharp
using NetTools;
...
// rangeA.Begin is "192.168.0.0", and rangeA.End is "192.168.0.255".
var rangeA = IPAddressRange.Parse("192.168.0.0/255.255.255.0");
rangeA.Contains(IPAddress.Parse("192.168.0.34")); // is True.
rangeA.Contains(IPAddress.Parse("192.168.10.1")); // is False.

// rangeB.Begin is "192.168.0.10", and rangeB.End is "192.168.10.20".
var rangeB = IPAddressRange.Parse("192.168.0.10 - 192.168.10.20");
rangeB.Contains(IPAddress.Parse("192.168.3.45")); // is True.
rangeB.Contains(IPAddress.Parse("192.168.0.9")); // is False.

var rangeC = IPAddressRange.Parse("fe80::/10"); // Support CIDR expression and IPv6.
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
```

License
-------
[LGPL v.3](LICENSE)
