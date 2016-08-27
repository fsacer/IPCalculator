using System.Linq;
using System.Net;
using System.Net.Sockets;

using static System.Console;

namespace IPKalkulator
{
    static class Parsers
    {
        /// <summary>
        /// Metoda namenjena preverjanju vnosa XXX.XXX.XXX.XXX/YY
        /// </summary>
        /// <param name="parts">deli vhodnega niza</param>
        /// <param name="ipAddress">izhodni ip naslov</param>
        /// <param name="subnetMask">izhodna maska</param>
        /// <returns>rezultat preverjanja veljavnosti</returns>
        public static bool ParseWithCIDR(string[] parts, out IPAddress ipAddress, out IPAddress subnetMask)
        {
            ipAddress = null;
            subnetMask = null;

            //Preverimo, če je smo dobili 2 dela
            if (parts.Length != 2) return false;

            //Preverimo veljavnost IP naslova
            if (!IPAddress.TryParse(parts[0], out ipAddress)
                || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                WriteLine("Input is invalid IPv4 address.");
                ipAddress = null;
                return false;
            }

            //Preverimo veljavnost CIDR pripone
            int cidr;
            if (!int.TryParse(parts[1], out cidr)
                || !Enumerable.Range(0, 32 + 1).Contains(cidr))
            {
                WriteLine("Input is invalid CIDR extension.");
                return false;
            }

            subnetMask = SubnetMask.CreateByNetBitLength(cidr);
            return true;
        }

        //Metoda, ki namenjena preverjanju vnosa XXX.XXX.XXX.XXX in YYY.YYY.YYY.YYY
        public static bool ParseWithIPv4Subnet(string ipInput, out IPAddress ipAddress, out IPAddress subnetMask)
        {
            //Preverjanje, če je naslov neveljaven
            if (!IPAddress.TryParse(ipInput, out ipAddress)
                || ipAddress.AddressFamily != AddressFamily.InterNetwork)
            {
                WriteLine("Input is invalid IPv4 address.");
                ipAddress = subnetMask = null;
                return false;
            }

            WriteLine("Enter valid IPv4 subnet mask:");
            string input = ReadLine();

            //Preverjanje, če je maska veljavna
            if (input != null
                && IPAddress.TryParse(input, out subnetMask)
                && ipAddress.AddressFamily == AddressFamily.InterNetwork
                && SubnetMask.IsValid(subnetMask))
                return true;

            WriteLine("Input is invalid IPv4 subnet mask.");
            ipAddress = subnetMask = null;
            return false;
        }
    }
}