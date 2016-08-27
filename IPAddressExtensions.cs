using System;
using System.Net;
using System.Collections;
using System.Linq;

namespace IPKalkulator
{
    public static class IPAddressExtensions
    {
        //Naštevanje, ki predstavlja IP razred
        public enum IPClass
        {
            Undefined = -1,
            A,
            B,
            C,
            D,
            E, 
            Invalid,
            Loopback,
            Special
        }

        //začetki prvega okteta za preverjanje razreda IP naslova: http://www.vlsm-calc.net/ipclasses.php
        static readonly BitArray HighOrderA = new BitArray(new[] { false });
        static readonly BitArray HighOrderB = new BitArray(new[] { true, false });
        static readonly BitArray HighOrderC = new BitArray(new[] { true, true, false });
        static readonly BitArray HighOrderD = new BitArray(new[] { true, true, true, false });
        static readonly BitArray HighOrderE = new BitArray(new[] { true, true, true, true });

        //Metoda, ki pridobi Broadcast naslov na podlagi podanega IP naslova in maske podmrežja.
        //To izračunamo z OR operacijo bajta IP naslova in eniškega komplementa bajta maske podmrežja
        public static IPAddress GetBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | ~subnetMaskBytes[i]);
            }
            return new IPAddress(broadcastAddress);
        }

        //Metoda, ki pridobi omrežni naslov na podlagi IP naslova in maske podmrežja
        //Uporabimo lastnost maske, da lahko omrežni del pridobimo z AND operacijo
        public static IPAddress GetNetworkAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] networkAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < networkAddress.Length; i++)
            {
                networkAddress[i] = (byte)(ipAdressBytes[i] & subnetMaskBytes[i]);
            }
            return new IPAddress(networkAddress);
        }

        //Metoda, ki na podlagi dveh naslovov in maske ugotovi ali sta IP naslova v istem podmrežju.
        public static bool IsInSameSubnet(this IPAddress address2, IPAddress address, IPAddress subnetMask)
        {
            var network1 = address.GetNetworkAddress(subnetMask);
            var network2 = address2.GetNetworkAddress(subnetMask);

            return network1.Equals(network2);
        }

        //Metoda namenjena pretvorbi IP naslova v binarno obliko.
        public static string ToBinary(this IPAddress address)
        {
            return String.Join(".", address.GetAddressBytes().Select( 

                //vsak bajt pretvorimo v binarno obliko, in dodamo mankajoče ničle
                x => Convert.ToString((int)x, 2).PadLeft(8, '0')

                //naštevanje pretvorimo v polje
                ).ToArray());
            //združimo niz po bajtih
        }


        // Metoda namenjena pridobitvi razreda IP naslova.
        public static IPClass GetClass(this IPAddress address)
        {
            //pridobimo samo prvi bajt IP naslova
            byte[] addressBytes = address.GetAddressBytes();
            byte firstByte = addressBytes[0];
            var addressBits = new BitArray(new[] { firstByte });
            
            IPClass ipClass;

            //Vrnemo razred IP naslova glede na prve bite prvega bajta in poskrbimo za posebne naslove
            if (firstByte == 127)
                ipClass = IPClass.Loopback;
            else if(addressBytes.SequenceEqual(new byte[]{ 255,255,255,255 }))
                ipClass = IPClass.Special;
            else if (addressBytes.SequenceEqual(new byte[] { 0,0,0,0 }))
                ipClass = IPClass.Invalid;
            else if (addressBits.StartsWith(HighOrderA))
                ipClass = IPClass.A;
            else if (addressBits.StartsWith(HighOrderB))
                ipClass = IPClass.B;
            else if (addressBits.StartsWith(HighOrderC))
                ipClass = IPClass.C;
            else if (addressBits.StartsWith(HighOrderD))
                ipClass = IPClass.D;
            else if (addressBits.StartsWith(HighOrderE))
                ipClass = IPClass.E;
            else
                ipClass = IPClass.Undefined;

            return ipClass;
        }

        //Povečanje IP naslova za 1
        public static IPAddress Increment(this IPAddress address)
        {
            byte[] addressBytes = address.GetAddressBytes();

            if (++addressBytes[3] == 0 && ++addressBytes[2] == 0 && ++addressBytes[1] == 0)
                ++addressBytes[0];

            return new IPAddress(addressBytes);
        }

        //Pomanjšanje IP naslova za 1
        public static IPAddress Decrement(this IPAddress address)
        {
            byte[] addressBytes = address.GetAddressBytes();

            if (--addressBytes[3] == 255 && --addressBytes[2] == 255 && --addressBytes[1] == 255)
                --addressBytes[0];

            return new IPAddress(addressBytes);
        }

        //Metoda, ki ugotovi, ali je podani IP naslov eden izmed naslovov, ki niso vidni internetu
        //Vir: http://www.vlsm-calc.net/ipclasses.php
        public static bool IsPrivate(this IPAddress address) => 
            address.IsInSameSubnet(IPAddress.Parse("10.0.0.0"), SubnetMask.ClassA)
            || address.IsInSameSubnet(IPAddress.Parse("127.16.0.0"), IPAddress.Parse("255.240.0.0"))
            || address.IsInSameSubnet(IPAddress.Parse("192.168.0.0"), SubnetMask.ClassB);
    }
}