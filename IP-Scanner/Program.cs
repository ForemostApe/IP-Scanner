using System.Net;
using System.Net.NetworkInformation;

Console.Write("Ange start-IP för scan: ");
string startIP= Console.ReadLine();
Console.Write("Ange slut-IP för scan: ");
string endIP = Console.ReadLine();

byte[] startRange = IPAddress.Parse(startIP).GetAddressBytes();
byte[] endRange = IPAddress.Parse(endIP).GetAddressBytes();

List<byte[]> ipRange = new List<byte[]>();

for (int i = startRange[3]; i <= endRange[3]; i++)
{
	byte[] currentIP = (byte[])startRange.Clone();
	currentIP[3] = (byte)i;
	ipRange.Add(currentIP);
}

foreach (byte[] ip in ipRange)
{
	string ipString = new IPAddress(ip).ToString();
	bool pinged = IsIPPingable(ipString);
	Console.WriteLine($"Reply from: {ipString} {(pinged ? "Answered" : "Didn't answer")}");
}

static bool IsIPPingable(string z)
{
	using Ping pingSender = new Ping();
	PingReply reply = pingSender.Send(z);
	return reply.Status == IPStatus.Success;
}


//192.168.50.1