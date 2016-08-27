using System;
using System.Net;

using static System.Console;
using static IPKalkulator.OutputFormatters;
using static IPKalkulator.Parsers;

namespace IPKalkulator
{
    static class Program
    {
        //Oblikujemo izhod programa
        static void WriteMainOutput(IPAddress ipAddress, IPAddress subnetMask)
        {
            WriteLine("Input interpreted as:");
            WriteStandardFormat("Address: ", ipAddress);
            WriteNetmask(subnetMask);
            WriteStandardFormat("Wildcard: ", SubnetMask.GetWildCard(subnetMask));
            WriteLine();
            WriteLine("=>");
            WriteLine();
            WriteNetwork(ipAddress, subnetMask);
            WriteStandardFormat("Broadcast: ", ipAddress.GetBroadcastAddress(subnetMask));
            IPAddress hostMin = ipAddress.GetNetworkAddress(subnetMask).Increment(),
                hostMax = ipAddress.GetBroadcastAddress(subnetMask).Decrement();
            WriteStandardFormat("HostMin: ", hostMin);
            WriteStandardFormat("HostMax: ", hostMax);
            WriteAvailableHosts(subnetMask, ipAddress);
        }

        static void Main()
        {
            Title = "IP Calculator by Franci Šacer";
            WindowWidth = BufferWidth = 90;

            IPAddress ipAddress, subnetMask;

            WriteLine("Enter valid IPv4 address with or without CIDR extension:");
            string input = ReadLine();

            if (input == null)
            {
                goto finish;
            }

            //Ločimo dele vhoda
            string[] addressParts = input.Split(new[] { ' ', '/'}, StringSplitOptions.RemoveEmptyEntries);

            //Preverimo, katero obliko bi naj vnesel uporabnik.
            switch (addressParts.Length)
            {
                case 1:
                    if (!ParseWithIPv4Subnet(addressParts[0], out ipAddress, out subnetMask)) goto finish;
                    break;
                case 2:
                    if (!ParseWithCIDR(addressParts, out ipAddress, out subnetMask)) goto finish;
                    break;
                default:
                    WriteLine("Invalid number of spaces or slashes.");
                    goto finish;
            }

            WriteLine();
            WriteMainOutput(ipAddress, subnetMask);

            finish:
            ReadLine();
        }
    }
}