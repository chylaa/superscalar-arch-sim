using superscalar_arch_sim.RV32.ISA;
using System;
using System.Collections.Generic;

using static superscalar_arch_sim.Utilis.Utilis;

namespace superscalar_arch_sim.RV32.Hardware.Memory
{
    public class MemoryAccessMisallign : Exception { public MemoryAccessMisallign(string msg):base(msg) { } }
    public class MemoryAddressOutOfRange : Exception { public MemoryAddressOutOfRange(uint addr, string msg=""):base($"Tries to access: {addr:X8} " + msg) { } }
    public class InvalidMemoryAccess : Exception { public InvalidMemoryAccess(HardwareProperties.MemoryAccess access, uint addr, string msg=""):base($"Tries to access ({access}): {addr:X8} " + msg) { } }
    
    
    /// <summary>
    /// Memory class - writes/reads performed in big-endiann format.
    /// </summary>
    public class Memory : IMemoryComponent
    {
        public const UInt32 DEFAULT_WORD = UInt32.MaxValue;
        public const UInt16 DEFAULT_HWORD = UInt16.MaxValue;
        public const Byte DEFAULT_BYTE = Byte.MaxValue;

        public bool IsLittleEndiann => false;

        public string Name { get; set; } = nameof(Memory);
        /// <summary><inheritdoc/></summary>
        public HardwareProperties.MemoryAccess Access { get; set; } = HardwareProperties.MemoryAccess.Read | HardwareProperties.MemoryAccess.Execute;
        /// <summary><inheritdoc/></summary>
        public UInt32 ByteSize { get; set; } = 0;
        /// <summary><inheritdoc/></summary>
        public UInt32 Origin { get; set; } = 0x00_00_00_00;
        /// <summary> Last valid adress within memory component</summary>
        public UInt32 LastAddress { get; private set; }

        /// <summary>Memory capacity in WORDs (as in <see cref="ISAProperties.WORD"/>).</summary>
        public UInt32 WordCapacity => ((UInt32)(ByteSize / ISAProperties.WORD_BYTESIZE));
        /// <summary>
        /// Dictionary acting as memory (to not store not-neccessary data, used instead of array).
        /// Values of keys (addresses) are always alligned to <see cref="Allign.WORD"/> boundary,
        /// so each key element should be multiple of 4.
        /// </summary>
        private Dictionary<UInt32, UInt32> _memory { get; }

        /// <summary>
        /// Access to <see cref="Memory"/> 32bit WORD value via specific <paramref name="address"/>.
        /// Wrap over <see cref="ReadWord(uint)"/> and <see cref="WriteWord(uint, uint)"/> methods.
        /// </summary>
        /// <param name="address">Address to read/write</param>
        /// <returns><inheritdoc cref="ReadWord(uint)"/></returns>
        public UInt32 this[uint address] { get => ReadWord(address); set => WriteWord(address, value); }

        /// <summary>
        /// Creates new Random-Access-Memory representation with maximum capacity of <paramref name="bytesize"/> bytes.
        /// Values in this memory representation are stored in running enviroment endianness (<see cref="BitConverter.IsLittleEndian"/>).
        /// </summary>
        /// <param name="bytesize"></param>
        /// <param name="origin"></param>
        public Memory(uint origin, uint bytesize, string name=null)
        {
            _memory = new Dictionary<UInt32, UInt32>();
            ResizeMemory(origin, bytesize);
            Name = name ?? Name;
        }

        public bool Contains(UInt32 addr) => addr >= Origin && addr <= LastAddress;

        public void ResizeMemory(uint origin, uint bytesize, bool reset = true)
        {
            ByteSize = bytesize;
            Origin = origin;
            LastAddress = (origin + bytesize) - ISAProperties.WORD_BYTESIZE;
            if(reset) Reset();
        }

        public UInt32 ReadWord(UInt32 address)
        {
            //if (false == Contains(address))
            //    throw new MemoryAddressOutOfRange(address, "(READ WORD)");
            if (false == IsAlligned(address, Allign.WORD))
                throw new MemoryAccessMisallign($"Read address value {address} is not alligned to {Allign.WORD} boundary.");

            if (_memory.TryGetValue(address, out UInt32 value))
                return value;
            return DEFAULT_WORD;
        }
        /// <summary>Allows to read specific HALF-WORD value from memory component.
        /// Throws <see cref="MemoryAccessMisallign"/> if <paramref name="address"/> is not alligned to <see cref="Allign.HALF_WORD"/> 
        /// </summary>
        /// <param name="address">Unsigned-WORD byte-address of value.</param>
        /// <returns>HWORD value under address <paramref name="address"/>.</returns>
        /// <exception cref="MemoryAccessMisallign"></exception>
        public UInt16 ReadHWord(UInt32 address)
        {
            //if (false == Contains(address))
            //    throw new MemoryAddressOutOfRange(address, "(READ HALF-WORD)");
            if (false == IsAlligned(address, Allign.HALF_WORD))
                throw new MemoryAccessMisallign($"Read address value {address} is not alligned to {Allign.HALF_WORD} boundary.");

            uint baseaddr = NearestAlligned(address, Allign.WORD);
            int bitoffset = (int)(8 * (address - baseaddr)); // 8 * (0 or 2) - NearestAlligned return always less or eq than its argument
            if (_memory.TryGetValue(baseaddr, out UInt32 value))
                return (UInt16)((value & (0x0000FFFF << bitoffset)) >> bitoffset);  
            return DEFAULT_HWORD;       
        }
        public Byte ReadByte(UInt32 address)
        {
            //if (false == Contains(address))
            //    throw new MemoryAddressOutOfRange(address, "(READ BYTE)");

            uint baseaddr = NearestAlligned(address, Allign.WORD);
            int bitoffset = (int)(8 * (address - baseaddr)); // 8 * (0/1/2/3) NearestAlligned return always less or eq than its argument
            if (_memory.TryGetValue(baseaddr, out UInt32 value))
                return (byte)((value & (0x000000FF << bitoffset)) >> bitoffset); 
            return DEFAULT_BYTE;
        }

        public void WriteWord(UInt32 address, UInt32 value)
        {
            //if (false == Contains(address))
            //    throw new MemoryAddressOutOfRange(address, "(WRITE WORD)");
            if (false == IsAlligned(address, Allign.WORD))
                throw new MemoryAccessMisallign($"Write address value {address} is not alligned to {Allign.WORD} boundary.");
            _memory[address] = value; // adds new address-value pair if not in dictionary
        }
        public void WriteHWord(UInt32 address, UInt16 value)
        {
            //if (false == Contains(address))
            //    throw new MemoryAddressOutOfRange(address, "(WRITE HALF-WORD)");
            if (false == IsAlligned(address, Allign.HALF_WORD))
                throw new MemoryAccessMisallign($"Write address value {address} is not alligned to {Allign.HALF_WORD} boundary.");

            uint baseaddr = NearestAlligned(address, Allign.WORD);
            int bitoffset = (int)(8 * (address - baseaddr));
            if (false == _memory.TryGetValue(baseaddr, out UInt32 existing))
                existing = DEFAULT_WORD;
            _memory[baseaddr] = (uint)(existing & (0xFFFFFFFF_0000FFFFUL >> (16-bitoffset)) | ((uint)(value) << bitoffset));
        }

        public void WriteByte(UInt32 address, Byte value)
        {
            //if (false == Contains(address))
            //    throw new MemoryAddressOutOfRange(address, "(WRITE BYTE)");

            uint baseaddr = NearestAlligned(address, Allign.WORD);
            int bitoffset = (int)(8 * (address - baseaddr));
            if (false == _memory.TryGetValue(baseaddr, out UInt32 existing))
                existing = DEFAULT_WORD;    
            _memory[baseaddr] = (uint)(existing & (0xFFFFFFFF_00FFFFFFUL >> (24-bitoffset)) | ((uint)(value) << bitoffset));
            
        }

        public uint Write(UInt32 address, UInt32[] values)
        {
            uint addr;
            for (addr = address; addr < (address + (values.Length * ISAProperties.WORD_BYTESIZE)); addr+=ISAProperties.WORD_BYTESIZE)
                WriteWord(addr, values[(addr - address)/ISAProperties.WORD_BYTESIZE]);
            return (addr - address);
        }
        public uint Write(UInt32 address, byte[] values)
        {
            uint addr;
            for (addr = address; addr < (address + values.Length); addr++) { 
                WriteByte(addr, values[addr - address]);
            }
            return (addr - address);
        }

        /// <summary><inheritdoc/></summary>
        public void Reset()
        {
            _memory.Clear();
        }

        /// <summary> Allows for "start:end" formatting.</summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            string[] addrrange = format.Split(':');
            if (false == (uint.TryParse(addrrange[0], out uint st) && uint.TryParse(addrrange[1], out uint end)))
                throw new Exception("Invalid format: " + format + ". Must by numeric range []:[]");

            string s = "";
            uint wordsize = (ISAProperties.WORD_BYTESIZE); uint inline = (8 * wordsize); // byte word size ; bytes in line 
            for (uint addr = st; addr < end; addr += wordsize)
            {
                if (addr % inline == 0) s += ($"\n[{addr:X4} : {(addr + inline - 1):X4}]");
                if (_memory.TryGetValue(addr, out UInt32 value)) s += value.ToString("X8");
                else s += DEFAULT_WORD.ToString("X8");
            }
            return s;
        }

        
    }
}
