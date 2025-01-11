using superscalar_arch_sim.RV32.Hardware.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace superscalar_arch_sim.Utilis
{
    public static class Utilis
    {
        public static bool IsInAddressSpace(UInt32 addr, UInt32 minaddr, UInt32 maxaddr)
            => (addr >= minaddr && addr < maxaddr);
        public static bool IsAlligned(UInt32 addr, Allign check)
            => (addr % (uint)check == 0);
        public static bool IsAlligned(Int32 addr, Allign check)
            => (addr % (uint)check == 0);

        public static bool IsZeroOrPowerOfTwo(Int32 x)
            => (x & (x - 1)) == 0;

        /// <summary>
        /// Calculates address closest to <paramref name="addr"/> that is alligned to specific <paramref name="boundary"/>.
        /// Returned value is always smaller (or equal) than initial <paramref name="addr"/>.
        /// <br></br> Note: Method assumes that <paramref name="boundary"/> integer value is power of 2!
        /// </summary>
        /// <param name="addr">Base address</param>
        /// <param name="boundary">Requested allignment of <paramref name="addr"/></param>
        /// <returns>Unsigned 32-bit integer representing first address, that is less or equal to <paramref name="addr"/>, 
        /// and is alligned to specific <paramref name="boundary"/> value.</returns>
        public static UInt32 NearestAlligned(UInt32 addr, uint boundary)
            => ((uint)((addr) & ~((int)boundary - 1)));

        /// <summary><inheritdoc cref="NearestAlligned(uint, uint)"/></summary>
        /// <param name="addr"><inheritdoc cref="NearestAlligned(uint, uint)"/></param>
        /// <param name="boundary"><inheritdoc cref="NearestAlligned(uint, uint)"/></param>
        /// <returns><inheritdoc cref="NearestAlligned(uint, uint)"/></returns>
        public static UInt32 NearestAlligned(UInt32 addr, Allign boundary)
            => NearestAlligned(addr, (uint)boundary);

        /// <summary>
        /// Creates <see cref="Int32"/> number from given <paramref name="value"/> assumming that bit at <paramref name="signBitPos"/> is sign bit.
        /// <br></br>Note: method sets to '0' all less significant bits 'up' from <paramref name="signBitPos"/>. 
        /// </summary>
        /// <param name="value">Value to convert to <see cref="Int32"/> sign-extending it using given <paramref name="signBitPos"/>.</param>
        /// <param name="signBitPos">zero-based index of sign bit in <paramref name="value"/>, counting from MSB.</param>
        /// <returns>32bit integer, sign-extended, using given <paramref name="signBitPos"/> as singn bit.</returns>
        public static int SignExtendToInt32(UInt32 value, int signBitPos)
        {
            if (((value & 1 << signBitPos) >> signBitPos) == 1)
                return unchecked((int)(value | (uint)(~((1 << signBitPos) - 1)))); // Create mask -> extend sign bit ('1') up to 31st bit
            return unchecked((int)value);
        }
        /// <summary>
        /// Creates <see cref="UInt32"/> number from given <paramref name="value"/> assumming that bit at <paramref name="signBitPos"/> is sign bit.
        /// <br></br>Note: method sets to '0' all less significant bits 'up' from <paramref name="signBitPos"/>. 
        /// </summary>
        /// /// <param name="value">Value to convert to <see cref="UInt32"/> sign-extending it using given <paramref name="signBitPos"/>.</param>
        /// <param name="signBitPos">zero-based index of sign bit in <paramref name="value"/>, counting from MSB.</param>
        /// <returns>32bit unsigned integer, sign-extended, using given <paramref name="signBitPos"/> as singn bit.</returns>
        public static uint SignExtendToUInt32(UInt32 value, int signBitPos)
        {
            if (((value & 1 << signBitPos) >> signBitPos) == 1)
                return (value | (uint)(~((1 << signBitPos) - 1))); // Create mask -> extend sign bit ('1') up to 31st bit
            return value;
        }

        public static int NearestMultiple(double value, double factor)
            => (int)(((int)Math.Round((value / factor), MidpointRounding.AwayFromZero)) * factor);

        /// <summary>Check if actual is between the boundaries without being equal to either.</summary>
        public static bool InBetweenNotEqual<T>(T boundary1, T actual, T boundary2) where T : IComparable<T>
            => (actual.CompareTo(boundary1) > 0 && actual.CompareTo(boundary2) < 0) ||
           (actual.CompareTo(boundary1) < 0 && actual.CompareTo(boundary2) > 0);

        public static HashSet<int> GetUniqueInts(int start, int count)
        {
            var set = new HashSet<int>(count+1);
            for (int i = start; i < start + count; i++) set.Add(i);
            return set;
        }
        public static HashSet<byte> GetUniqueBytes(byte start, byte count)
        {
            var set = new HashSet<byte>(count+1);
            for (byte i = start; i < start + count; i++) set.Add(i);
            return set;
        }
        public static string BinStr(Int32 val, int bitsize=0)
            => Convert.ToString(val, 2).PadLeft(bitsize, '0');
        public static string BinStr(UInt32 val, int bitsize=0)
            => Convert.ToString(val, 2).PadLeft(bitsize, '0');
        public static string BitsStringFromInteger(uint value, int size = sizeof(UInt32) * 8)
            => Convert.ToString(value, 2).PadLeft(size, '0');

        public static UInt32 ReverseBytes(UInt32 value)
           => (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
               (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;

        public static byte[] GetBytesFromFile(string filepath, out long filesize, uint maxsize = (1024 * 128))
        {
            byte[] data;
            byte[] buffer = new byte[maxsize];
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                filesize = fs.Length;
                data = new byte[fs.Read(buffer, 0, buffer.Length)];
            }

            Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
            return data;
        }
        public static UInt32[] GetUInt32sFromFile(byte[] buffer, bool input_little_endian = false)
        {
            byte[] uints = new byte[4];
            UInt32[] data = new UInt32[buffer.Length / 4];
            for (int i = 0; i < data.Length; i++)
            {
                Array.Copy(buffer, i * 4, uints, 0, 4);
                if (input_little_endian != BitConverter.IsLittleEndian)
                    Array.Reverse(uints);
                data[i] = BitConverter.ToUInt32(uints, 0);
            }
            return data;
        }
        public static UInt32[] GetUInt32sFromFile(string filepath, out long filesize, uint maxsize = (1024 * 128), bool input_little_endian = false)
        {
            byte[] buffer = GetBytesFromFile(filepath, out filesize, maxsize);
            return GetUInt32sFromFile(buffer, input_little_endian);
        }

    }
}
