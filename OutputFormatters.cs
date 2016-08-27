using System;
using System.Net;

using static System.Console;
using static System.ConsoleColor;
using static IPKalkulator.IPAddressExtensions;

namespace IPKalkulator
{
    static class OutputFormatters
    {
        const int RightPadding = 15, ThirdRow = 40;
        
        //Metoda izpiše tekst prvega stolpca, IP naslov v decimalni in v binarni obliki.
        public static void WriteStandardFormat(string firstCol, IPAddress address, ConsoleColor binaryColor = DarkGray)
        {
            Write(firstCol.PadRight(RightPadding));
            ForegroundColor = Blue;
            Write(address.ToString().PadRight(RightPadding));
            ForegroundColor = binaryColor;
            SetCursorPosition(ThirdRow, CursorTop);
            Write(address.ToBinary());
            ResetColor();
            WriteLine();
        }

        //Metoda za izpis obarvane bitne maske omrežja za razredni del.
        public static void WriteWithColoredStartChars(string line, int firstColoredChars)
        {
            int firstBitPoz = line.IndexOfAny(new[] { '0', '1' });
            ForegroundColor = DarkGray;
            Write(line.Substring(0, firstBitPoz));
            ForegroundColor = Green;
            Write(line.Substring(firstBitPoz, firstColoredChars));
            ForegroundColor = DarkGray;
            Write(line.Substring(firstBitPoz + firstColoredChars));
        }

        //Oblikujemo izpis za omrežje, kjer so dodatno obrvani prvi biti glede na razred IP naslova.
        public static void WriteNetwork(IPAddress ipAddress, IPAddress subnetMask)
        {
            var networkAddress = ipAddress.GetNetworkAddress(subnetMask);
            var ipClass = networkAddress.GetClass();

            Write("Network: ".PadRight(RightPadding));
            ForegroundColor = Blue;
            string network = $"{networkAddress}/{SubnetMask.GetNetBitLength(subnetMask)}";
            Write(network.PadRight(RightPadding));
            
            ForegroundColor = DarkGray;
            int numOfColoredBits = (int)ipClass + 1;
            if (numOfColoredBits == 5) numOfColoredBits--;
            SetCursorPosition(ThirdRow, CursorTop);
            if (ipClass > IPClass.Undefined && ipClass < IPClass.Invalid)
                WriteWithColoredStartChars(networkAddress.ToBinary(), numOfColoredBits);
            else
                Write(networkAddress.ToBinary());

            Write(" (");
            ForegroundColor = Green;
            if(ipClass > IPClass.Undefined && ipClass < IPClass.Invalid)
                Write($"Class {ipClass}");
            else
                Write(ipClass);
            ForegroundColor = DarkGray;
            Write(")");
            ResetColor();
            WriteLine();
        }

        //Izpišemo masko podmrežja v obeh oblikah.
        public static void WriteNetmask(IPAddress subnetMask)
        {
            Write("Netmask: ".PadRight(RightPadding));
            ForegroundColor = Blue;
            string netMask = $"{subnetMask} = {SubnetMask.GetNetBitLength(subnetMask)}";
            Write(netMask.PadRight(RightPadding));
            ForegroundColor = Red;
            SetCursorPosition(ThirdRow, CursorTop);
            Write(subnetMask.ToBinary());
            ResetColor();
            WriteLine();
        }

        //Izpišemo koliko uporabnih naslovov je na voljo in še ali omrežje spada med nejavne naslove.
        public static void WriteAvailableHosts(IPAddress subnetMask, IPAddress ipAddress)
        {
            Write("Hosts/Net: ".PadRight(RightPadding));
            ForegroundColor = Blue;
            Write($"{SubnetMask.NumberOfUsableHosts(subnetMask)}".PadRight(RightPadding));
            ForegroundColor = DarkBlue;
            string specialOutput = "";

            if (ipAddress.IsPrivate())
                specialOutput = "(Private Internet)";
            else if (ipAddress.Equals(IPAddress.Loopback))
                specialOutput = "(LOCALHOST)";

            SetCursorPosition(ThirdRow, CursorTop);
            Write(specialOutput);
            
            ResetColor();
            WriteLine();
        }
    }
}