using System.Collections.Generic;
using System.Text;

namespace superscalar_arch_sim.RV32.Hardware.Register
{
    public class ControlStatusRegFile : Register32File
    {
        public  static class CSRs
        {
            public const uint FFLAGS  = 0x001;
            public const uint FRM     = 0x002;
            public const uint FCSR    = 0x003;
            public const uint CYCLE   = 0xC00;
            public const uint TIME    = 0xC01;
            public const uint INSTRET = 0xC02;
            public const uint CYCLEH  = 0xC80;
            public const uint TIMEH   = 0xC81;
            public const uint INSTRETH = 0xC82;
        }

        /// <summary>Architectural registers, accessible by user using CSR instructions.</summary>
        readonly IDictionary<uint, Register32> CSRRegisters;
        /// <summary>Origin address of memory where CRSs will be mapped.</summary>
        public uint Origin = 0x00;
        
        public ControlStatusRegFile(IDictionary<uint, Register32> archregs) : base()
        {
            CSRRegisters = archregs;
        }

        public override Register32 GetRegister(int idx) => CSRRegisters[unchecked((uint)idx)];

        public override void Reset()
        {
            foreach (var register in CSRRegisters.Values)
                register.Reset();
        }
        /// <summary>
        /// Checks if <paramref name="csr"/> address is known CSR and if <paramref name="hasAccess"/> is equal to 
        /// <see cref="Register32.UserAccess"/> of that CSR. If <paramref name="hasAccess"/> equals 0, value is not checked.
        /// </summary>
        /// <param name="csr">Address of CSR <see cref="Register32"/>.</param>
        /// <param name="hasAccess"><see cref="HardwareProperties.MemoryAccess"/> that should equal <see cref="Register32.UserAccess"/> of specified <paramref name="csr"/>.</param>
        /// <returns><see langword="true"/> if conditions are met, <see langword="false"/> otherwise.</returns>
        public bool IsValidCSR(uint csr, HardwareProperties.MemoryAccess hasAccess = 0)
        {
            bool result = CSRRegisters.ContainsKey(csr);
            if (result && hasAccess != 0) { result = result && (CSRRegisters[csr].UserAccess == hasAccess); }
            return result;
        }
    }
}
