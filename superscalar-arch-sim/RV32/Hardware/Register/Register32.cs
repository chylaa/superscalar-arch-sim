using System;

namespace superscalar_arch_sim.RV32.Hardware.Register
{
    /// <summary>
    /// Represents simple 32-bit register that can be Read or Written.
    /// (Could be Register<T> ?)
    /// </summary>
    public class Register32
    {
        /// <summary>Passed to <see cref="ToString(string)"/> as format <see langword="string"/>, causes it to return <see cref="ShortFormat"/> <see langword="string"/>.</summary>
        public const string FORMAT_SHORT = "short";
        /// <summary>Value that is stored in <see cref="Register32"/> after initialization or <see cref="Reset"/>.</summary>
        public const int DefaultValue = Int32.MaxValue;
        /// <summary>Register internal value size in bits</summary>
        public const int Size = ISA.ISAProperties.ILEN;
        
        /// <summary>Architectural name of Register if provided in constructor, <see cref="string.Empty"/> otherwise.</summary>
        public string ABIMnemonic { get; }
        /// <summary>Short name of register if provided in constructor, <see cref="string.Empty"/> otherwise. For displaying and debugging purposes.</summary>
        public string Name { get; }
        /// <summary>Short description of register if provided in constructor, <see cref="string.Empty"/> otherwise. For displaying and debugging purposes.</summary>
        public string Meaning { get; }

        /// <summary>User Access Level to register. Always read/write except for CSR registers.</summary>
        public HardwareProperties.MemoryAccess UserAccess = HardwareProperties.MemoryAccess.RW;
        
        /// <summary>Value stored inside register. Initialized with <see cref="DefaultValue"/>.</summary>
        public Int32 Value = DefaultValue;

        /// <summary>
        /// Creates new 32-bit <see cref="Register32"/> object with <paramref name="abiMnem"/> and <paramref name="access"/>.
        /// </summary>
        /// <param name="abiMnem">Name of register specified in used ABI.</param>
        public Register32(string abiMnem = null, string name = null, string meaning = null) 
        {
            ABIMnemonic = abiMnem??string.Empty;
            Name = name ?? string.Empty;
            Meaning = meaning ?? string.Empty;
        }

        virtual public Int32 Read() => Value; 
        virtual public UInt32 ReadUnsigned() => unchecked((uint)Value);
        virtual public void Write(Int32 value) => Value = value; 
        virtual public void WriteUnsigned(UInt32 value) => Value = unchecked((int)value);
        virtual public void Reset() => Value = DefaultValue;
        public bool Compare(Register32 b) => (Value == b.Value);

        public string ToHexString(uint size) => (unchecked((UInt32)Value)).ToString($"X{size}"); 
        public string ToHexString() => (unchecked((UInt32)Value)).ToString("X"); 
        public string ToString(string format)
        {
            if (format == FORMAT_SHORT) return ShortFormat;
            return $"[{Name}] {ABIMnemonic} = {Value:X8} (" + string.Format(format, Value) + ')';
        } 
        public override string ToString()
            => $"[{Name}] {ABIMnemonic} = {Value:X8} ({Value})";

        public string ShortFormat { get => $"{ABIMnemonic} = {Value:X8}"; } 
        public string HexString { get => ToHexString(); } 
    }
}
