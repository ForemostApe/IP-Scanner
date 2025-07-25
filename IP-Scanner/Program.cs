using System.Net;
using System.Net.NetworkInformation;

IPAddress startOfIpRange, endOfIpRange;

while (true)
{
	Console.Write("Ange start-IP för scan: ");
	string? input = Console.ReadLine();
	if (input != null && IPAddress.TryParse(input, out startOfIpRange))
		break;
	Console.WriteLine("Ange en korrekt IP-address.");
}

while (true)
{
    Console.Write("Ange slut-IP för scan: ");
    string? input = Console.ReadLine();
    if (input != null && IPAddress.TryParse(input, out endOfIpRange))
        break;
    Console.WriteLine("Ange en korrekt IP-address.");
}

//Converts the IP from string-format to a byte array, required to create the full range to check.
byte[] startRange = startOfIpRange.GetAddressBytes();
byte[] endRange = endOfIpRange.GetAddressBytes();

List<byte[]> ipRange = new List<byte[]>();
List<Task> pingTasks = new List<Task>();

//Fills in the range between start- end endIP's provided. Only creates a range from the last octet.
for (int i = startRange[3]; i <= endRange[3]; i++)
{
	byte[] currentIP = (byte[])startRange.Clone();
	currentIP[3] = (byte)i;
	ipRange.Add(currentIP);
}

//Converts IP in byte-array-format to strings. Calls method that performs ping-operation.
foreach (byte[] ip in ipRange)
{
	string ipString = new IPAddress(ip).ToString();
	pingTasks.Add(PingResponseAsync(ipString));
}

//Awaits for the whole range of IPs to be pinged, otherwise will continue after the first ping
//thereby ending the operation of the app.
await Task.WhenAll(pingTasks);

//Checks and reports the answer from the pinged IP.
static async Task PingResponseAsync(string ipString)
{
	bool pinged = await IsIPPingableAsync(ipString);
	Console.WriteLine($"Reply from: {ipString} {(pinged ? "Answered" : "Didn't answer")}");
}

//Pings the provided IP and returns either true or false depending on if the IP provided an answer or not.
static async Task<bool> IsIPPingableAsync(string ip)
{
	using Ping pingSender = new Ping();
	PingReply reply = await pingSender.SendPingAsync(ip, 5000);
	return reply.Status == IPStatus.Success;
}

//192.168.50.1