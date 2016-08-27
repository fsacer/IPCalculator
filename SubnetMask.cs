using System;
using System.Collections;
using System.Net;

namespace IPKalkulator
{
    public static class SubnetMask
    {
        public static readonly IPAddress ClassA = IPAddress.Parse("255.0.0.0");
        public static readonly IPAddress ClassB = IPAddress.Parse("255.255.0.0");
        public static readonly IPAddress ClassC = IPAddress.Parse("255.255.255.0");

        //Ustvarjanje maske na podlagi števila bitov gostiteljevega dela IP naslova.
        public static IPAddress CreateByHostBitLength(int hostpartLength)
        {
            int hostPartLength = hostpartLength;
            int netPartLength = 32 - hostPartLength;

            byte[] binaryMask = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                if (i * 8 + 8 <= netPartLength)
                    binaryMask[i] = 255;
                else if (i * 8 > netPartLength)
                    binaryMask[i] = 0;
                else
                {
                    int oneLength = netPartLength - i * 8;
                    string binaryDigit =
                        String.Empty.PadLeft(oneLength, '1').PadRight(8, '0');
                    binaryMask[i] = Convert.ToByte(binaryDigit, 2);
                }
            }
            return new IPAddress(binaryMask);
        }

        //Ustavrjanje maske na podlagi omrežnega dela.
        public static IPAddress CreateByNetBitLength(int netpartLength)
        {
            int hostPartLength = 32 - netpartLength;
            return CreateByHostBitLength(hostPartLength);
        }

        //Ustvarjanje maske na podlagi števila naprav.
        public static IPAddress CreateByHostNumber(int numberOfHosts)
        {
            int maxNumber = numberOfHosts + 1;

            string b = Convert.ToString(maxNumber, 2);

            return CreateByHostBitLength(b.Length);
        }

        //Izračuna negirano masko.
        public static IPAddress GetWildCard(IPAddress subnetMask)
        {
            var negated = new BitArray(subnetMask.GetAddressBytes()).Not();
            byte[] bytes = new byte[negated.Length / 8];
            negated.CopyTo(bytes, 0);
            return new IPAddress(bytes);
        }

        //Pridobi dolžino omrežnega dela zaslona tako, da preštejemo enice.
        public static int GetNetBitLength(IPAddress subnetMask)
        {
            byte[] parts = subnetMask.GetAddressBytes();
            var bits = new BitArray(parts);
            int bitCount = 0;

            foreach (bool bit in bits)
            {
                if (bit) bitCount++;
                else break;
            }
            return bitCount;
        }

        //Preverjanje veljavnosti maske torej, da so enice ločene od ničel.
        public static bool IsValid(IPAddress subnetMask)
        {
            //pridobimo vse dele
            byte[] parts = subnetMask.GetAddressBytes();

            var bitArrayOfFirst = new BitArray(new[] { parts[0] });
            var bitArrayOfSecond = new BitArray(new[] { parts[1] });
            var bitArrayOfThird = new BitArray(new[] { parts[2] });
            var bitArrayOfFourth = new BitArray(new[] { parts[3] });

            //obrnemo bite, ker je vrstni red bitov v BitArray od LSB -> MSB (Most Significant Bit)
            bitArrayOfFirst.Reverse();
            bitArrayOfSecond.Reverse();
            bitArrayOfThird.Reverse();
            bitArrayOfFourth.Reverse();

            //spnemo bitna polja
            var bits = bitArrayOfFirst.Append(bitArrayOfSecond).Append(bitArrayOfThird).Append(bitArrayOfFourth);

            //preverjamo, da se po prvi ničli ne pojavi nobena enica
            bool foundZero = false;
            foreach (bool bit in bits)
            {
                if (!bit)
                {
                    foundZero = true;
                }

                if (foundZero && bit)
                {
                    return false;
                }
            }

            return true;
        }

        //Izračunamo število uporabnih naslovov tako, da pridobi eksponent na podlagi bajtov mask in jih seštejemo.
        //Nato potenciromo 2^eksponent.
        //Število uporobnih naslovov je za 2 manj (omrežni in broadcast naslov).
        public static int NumberOfUsableHosts(IPAddress subnetMask)
        {
            byte[] maskBytes = subnetMask.GetAddressBytes();
            int exponent = 0;

            byte[] possibleSubnets = { 255, 254, 252, 248, 240, 224, 192, 128, 0 };
            foreach (byte maskByte in maskBytes)
            {
                int incrementBy = Array.IndexOf(possibleSubnets, maskByte);
                if (incrementBy < 0) incrementBy = 0;
                exponent += incrementBy;
            }

            //potenciranje
            int count = 1;
            for (; exponent > 0; exponent--) count *= 2;
            count -= 2;

            return count > 0 ? count : 0;
        }
    }
}