IPAddressRange Class Library
=============

This library allows you to parse range of IP address string such as "192.168.0.0/24" and "192.168.0.0/255.255.255.0" and "192.168.0.0-192.168.0.255", and can conatins check.
This library supports both IPv4 and IPv6.

Example
-------

```C#
using NetTools;
...
// rangeA.Begin is "192.168.0.0", and rangeA.End is "192.168.0.255".
var rangeA = new IPAddressRange("192.168.0.0/255.255.255.0");
rangeA.Contains(IPAddress.Parse("192.168.0.34")) // is True.
rangeA.Contains(IPAddress.Parse("192.168.10.1")) // is False.

// rangeB.Begin is "192.168.0.10", and rangeB.End is "192.168.10.20".
var rangeB = new IPAddressRange("192.168.0.10 - 192.168.10.20");
rangeB.Contains(IPAddress.Parse("192.168.3.45")) // is True.
rangeB.Contains(IPAddress.Parse("192.168.0.9")) // is False.

var rangeC = new IPAddressRange("fe80::/10"); // Support CIDR expression and IPv6.
rangeC.Contains(IPAddress.Parse("fe80::d503:4ee:3882:c586%3")) // is True.
rangeC.Contains(IPAddress.Parse("::1")) // is False.

// "Contains()" method also support IPAddressRange argument.
var rangeD1 = new IPAddressRange("192.168.0.0/16");
var rangeD2 = new IPAddressRange("192.168.10.0/24");
rangeD1.Contains(rangeD2) // is True.
```
