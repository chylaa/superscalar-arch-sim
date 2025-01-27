using superscalar_arch_sim.RV32.Hardware.Memory;
using System;
using System.Linq;

namespace superscalar_arch_sim.RV32.Hardware.Units
{
    /// <summary>
    /// Memory Managment Unit. In simulator, implements logic to control&handle dataflow between memory components
    /// (as loading data from RAM to Caches ect...) as well as mapping certain addresses to related 
    /// </summary>
    public class MemoryManagmentUnit : IMemoryComponent
    {
        public string Name { get; set; } = "MMU";
        public HardwareProperties.MemoryAccess Access => HardwareProperties.MemoryAccess.RW;
        public uint Origin { get; set; } = 0;
        public uint ByteSize { get; set; } = uint.MaxValue;

        private readonly Memory.Memory RAM;
        private readonly Memory.Memory ROM;
        private readonly IMemoryComponent[] MemoryComponents;

        public MemoryManagmentUnit(Memory.Memory ram, Memory.Memory rom) { 
            RAM = ram;
            ROM = rom;
            
            MemoryComponents = new IMemoryComponent[] { RAM, ROM }; // TODO:  ICache, DCache
            Reset();
        }


        private IMemoryComponent GetMemoryComponent(uint address)
            => MemoryComponents.SingleOrDefault(com => com.Contains(address));

        public bool Contains(uint addr) 
            => addr >= Origin && (Origin + ByteSize) > addr;

        public void Reset()
        { 
            Array.ForEach(MemoryComponents, com => com.Reset());
            ThrowIfMemoryComponentsOverlaps(); // sanity check
        }
        
        /// <exception cref="Exception"> On overlapping addresses</exception>
        public void ThrowIfMemoryComponentsOverlaps()
        {
            if (MemoryOverlaps(MemoryComponents))
            {
                string msg = "Memory components have at least one common address in their ranges: ";
                msg += '[' + string.Join(",", MemoryComponents.Select(x => x.Origin)) + ']';
                throw new Exception(msg);
            }
        }

        public uint ReadWord(uint address)
        {
            IMemoryComponent mem = GetMemoryComponent(address) ?? throw new MemoryAddressOutOfRange(address, "(READ WORD)");
            uint localaddr = (address - mem.Origin);
            return mem.ReadWord(localaddr);
        }

        public void WriteWord(uint address, uint value)
        {
            IMemoryComponent mem = GetMemoryComponent(address) ?? throw new MemoryAddressOutOfRange(address, "(WRITE WORD)");
            uint localaddr = (address - mem.Origin);
            if (mem.Access != HardwareProperties.MemoryAccess.Read)
                mem.WriteWord(localaddr, value);
            else throw new InvalidMemoryAccess(HardwareProperties.MemoryAccess.Write, address, 
                       "Cannot WRITE WORD: " + mem.Name + " is read-only memory");
        }

        public ushort ReadHWord(uint address)
        {
            IMemoryComponent mem = GetMemoryComponent(address) ?? throw new MemoryAddressOutOfRange(address, "(READ HALF-WORD)");
            uint localaddr = (address - mem.Origin);
            return mem.ReadHWord(localaddr);
        }

        public void WriteHWord(uint address, ushort value)
        {
            IMemoryComponent mem = GetMemoryComponent(address) ?? throw new MemoryAddressOutOfRange(address, "(WRITE HALF-WORD)");
            uint localaddr = (address - mem.Origin);
            if (mem.Access != HardwareProperties.MemoryAccess.Read)
                mem.WriteHWord(localaddr, value);
            else throw new InvalidMemoryAccess(HardwareProperties.MemoryAccess.Write, address,
                       "Cannot WRITE HALF-WORD: " + mem.Name + " is read-only memory");
        }

        public byte ReadByte(uint address)
        {
            IMemoryComponent mem = GetMemoryComponent(address) ?? throw new MemoryAddressOutOfRange(address, "(READ BYTE)");
            uint localaddr = (address - mem.Origin);
            return mem.ReadByte(localaddr);
        }

        public void WriteByte(uint address, byte value)
        {
            IMemoryComponent mem = GetMemoryComponent(address) ?? throw new MemoryAddressOutOfRange(address, "(WRITE BYTE)");
            uint localaddr = (address - mem.Origin);
            if (mem.Access != HardwareProperties.MemoryAccess.Read)
                mem.WriteByte(localaddr, value);
            else throw new InvalidMemoryAccess(HardwareProperties.MemoryAccess.Write, address, 
                        "Cannot WRITE BYTE: " + mem.Name + " is read-only memory");
        }

        public override string ToString()
        {
            string mm = "";
            foreach (var mem in MemoryComponents)
                mm += $"{mem.Name}: Origin 0x{mem.Origin:X8} | Bytesize: 0x{mem.ByteSize:X8}\n";
            return mm;
        }

        public static bool MemoryOverlaps(params IMemoryComponent[] ms)
        {
            long lastOffset = -1;
            foreach (var mem in ms.OrderBy(x => x.Origin))
            {
                if (mem.Origin < lastOffset)
                    return true;
                lastOffset = mem.Origin;
            }
            return false;
        }

        ///// <summary>
        ///// Reads value of type <typeparamref name="V"/> from <paramref name="address"/> of type <typeparamref name="A"/>
        ///// </summary>
        ///// <typeparam name="A"><see cref="Type.IsValueType"/> Type of <paramref name="address"/> parameter.</typeparam>
        ///// <typeparam name="V"><see cref="Type.IsValueType"/> that would be type of returned, read value.</typeparam>
        ///// <param name="address">Unsigned/Signed value source address in memory.</param>
        ///// <returns> value of type <typeparamref name="V"/> from <paramref name="address"/> of type <typeparamref name="A"/></returns>
        //public V Read<A, V>(A address)
        //{

        //}

        ///// <summary>
        ///// Write <paramref name="value"/> of any type <typeparamref name="V"/> under <paramref name="address"/> 
        ///// specified by variable of type <typeparamref name="A"/>.
        ///// </summary>
        ///// <typeparam name="A"><see cref="Type.IsValueType"/> Type of <paramref name="address"/> parameter.</typeparam>
        ///// <typeparam name="V"><see cref="Type.IsValueType"/> Type of <paramref name="value"/> parameter.</typeparam>
        ///// <param name="address">Unsigned/Signed destination address of <paramref name="value"/>.</param>
        ///// <param name="value">Unsigned/Signed value to put under <paramref name="address"/>.</param>
        //public void Write<A, V>(A address, V value)
        //{

        //}
    }
}
