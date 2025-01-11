using superscalar_arch_sim.RV32.ISA;

using WORD = System.UInt32;
using SWORD = System.Int32;
using HWORD = System.UInt16;
using SHWORD = System.Int16;
using BYTE = System.Byte;
using SBYTE = System.Byte;

namespace superscalar_arch_sim.RV32.Hardware.Memory
{
    /// <summary>Represents alligment of values within memory in bytes. Each enum value is encoded as consecutive powers of 2.</summary>
    public enum Allign { 
        BYTE = 1, 
        HALF_WORD = ISAProperties.WORD_BYTESIZE / 2, 
        WORD = ISAProperties.WORD_BYTESIZE, 
        DWORD = 2 * ISAProperties.WORD_BYTESIZE 
    }

    /// <summary>
    /// Interface for memory components that can be represented as subsections of CPU-visible memory map.
    /// </summary>
    public interface IMemoryComponent
    {
        /// <summary>Name alias of <see cref="IMemoryComponent"/>.</summary>
        string Name { get; set; }
        /// <summary>Fixed memory component access.</summary>
        HardwareProperties.MemoryAccess Access { get; }
        /// <summary>Begining of <see cref="IMemoryComponent"/> region in memory map as from address 0x(0). Should be alligned to <see cref="ISA.ISAProperties.WORD_BYTESIZE"/>.</summary>
        WORD Origin { get; set; }
        /// <summary>Size in bytes of <see cref="IMemoryComponent"/> region. Should be alliged to <see cref="ISA.ISAProperties.WORD_BYTESIZE"/>.</summary>
        WORD ByteSize { get; set; }

        /// <summary>Checks if <paramref name="address"/> is in addressable range of <see cref="IMemoryComponent"/> base on its <see cref="Origin"/> and <see cref="ByteSize"/>.</summary>
        /// <param name="address"></param>
        /// <returns><see langword="true"></see> if memory contains <paramref name="address"/> within its addressable range, <see langword="false"></see> otherwise.</returns>
        bool Contains(WORD address);

        /// <summary>Allows to read WORD value from memory component.</summary>
        /// <param name="address">Unsigned-WORD byte-address of value.</param>
        /// <returns>WORD value under address <paramref name="address"/>.</returns>
        WORD ReadWord(WORD address);

        /// <summary>Allows to write WORD value into memory component under unsigned-WORD byte-address.</summary>
        /// <param name="address">Unsigned-WORD byte-address of value.</param>
        /// <param name="value">WORD value to write.</param>
        void WriteWord(WORD address, WORD value);

        /// <summary>Allows to read Half-WORD value from memory component.</summary>
        /// <param name="address">Unsigned-WORD byte-address of value.</param>
        /// <returns>Half-WORD value under address <paramref name="address"/>.</returns>
        HWORD ReadHWord(WORD address);

        /// <summary>Allows to write Half-WORD value into memory component under unsigned-WORD byte-address.</summary>
        /// <param name="address">Unsigned-WORD byte-address of value.</param>
        /// <param name="value">Half-WORD value to write.</param>
        void WriteHWord(WORD address, HWORD value);

        /// <summary>Allows to read single byte value from memory component.</summary>
        /// <param name="address">Unsigned-WORD byte-address of value.</param>
        /// <returns>Byte value under address <paramref name="address"/>.</returns>
        BYTE ReadByte(WORD address);

        /// <summary>Allows to write single byte value into memory component under unsigned-WORD byte-address.</summary>
        /// <param name="address">Unsigned-WORD byte-address of value.</param>
        /// <param name="value">Byte value to write.</param>
        void WriteByte(WORD address, BYTE value);

        /// <summary>Resets memory component to it's default state.</summary>
        void Reset();

    }
}
