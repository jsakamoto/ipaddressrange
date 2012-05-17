IPAddressRange Class Library
=============

This library allows you to parse range of IP address string such as "192.168.0.0/24", and can conatins check.
This library supports both IPv4 and IPv6.

Example
-------

	using NetTools;
	...
	var rangeA = new IPAddressRange("192.168.0.0/24"); // rangeA.Begin is "192.168.0.0", and rangeA.End is "192.168.0.255".
	rangeA.Conatins(IPAddress.Parse("192.168.0.34")) // is True.
	rangeA.Conatins(IPAddress.Parse("192.168.10.1")) // is False.

	var rangeB = new IPAddressRange("192.168.0.10 - 192.168.10.20"); // rangeB.Begin is "192.168.0.10", and rangeB.End is "192.168.10.20".
	rangeB.Conatins(IPAddress.Parse("192.168.3.45")) // is True.
	rangeB.Conatins(IPAddress.Parse("192.168.0.9")) // is False.

