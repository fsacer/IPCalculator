using System.Collections;

namespace IPKalkulator
{
    public static class BitArrayExtensions
    {
        //Metoda, ki ugotovi ali se bitno polje začne z določenimi biti
        public static bool StartsWith(this BitArray bits, BitArray bitsToCheck)
        {
            //Obrnemo bite, ker je vrstni red bitov v BitArray od LSB -> MSB (Most Significant Bit)
            bits.Reverse();

            //Gremo čez bitno polje, dokler je potrebno preverjati enakost obeh polj na začetku
            for (int i = 0; i < bitsToCheck.Length; i++)
            {
                if (bits[i] != bitsToCheck[i])
                {
                    return false;
                }
            }
            return true;
        }

        //Učuinkovita metoda za obračanje polja
        public static void Reverse(this BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        //Metoda, ki združi dve bitni polji
        public static BitArray Append(this BitArray current, BitArray after)
        {
            bool[] bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }
    }
}