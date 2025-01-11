using superscalar_arch_sim.RV32.ISA;
using System.Collections.Generic;

namespace superscalar_arch_sim.RV32.Hardware.Register
{
    internal static class Reg32FileFactory
    {
        internal static Register32File InitArchitecturalIntegerRegisters()
        {
            List<Register32> regs = new List<Register32> {
                new RegisterZero(),
                new Register32("ra", name:"x1", meaning:"Return address"),
                new Register32("sp", name:"x2", meaning:"Stack pointer"),
                new Register32("gp", name:"x3", meaning:"Global pointer"),
                new Register32("tp", name:"x4", meaning:"Thread pointer"),
                new Register32("t0", name:"x5", meaning:"Temporary register 0"),
                new Register32("t1", name:"x6", meaning:"Temporary register 1"),
                new Register32("t2", name:"x7", meaning:"Temporary register 2"),
                new Register32("s0", name:"x8", meaning:"Callee-saved register 0"),
                new Register32("s1", name:"x9", meaning:"Callee-saved register 1"),
                new Register32("a0", name:"x10", meaning:"Function argument / return value 0"),
                new Register32("a1", name:"x11", meaning:"Function argument / return value 1")
            };
            for (int i = 2; i < 8; i++)
                regs.Add(new Register32($"a{i}", name: $"x{10 + i}", meaning: $"Function argument {i}"));
            for (int i = 2; i < 12; i++)
                regs.Add(new Register32($"s{i}", name: $"x{18 + i - 2}", meaning: $"Callee-saved register {i}"));
            for (int i = 3; i < 7; i++)
                regs.Add(new Register32($"t{i}", name: $"x{28 + i - 3}", meaning: $"Temporary register {i}"));

            return new Register32File(regs);
        }

        internal static Register32File InitArchitecturalFloatRegisters()
        {
            List<Register32> fpregs = new List<Register32>();
            for (int i = 0; i < ISAProperties.NO_FP_REGISTERS; i++)
                fpregs.Add(new Register32($"f{i}", name: $"f{i}", meaning: $"FP Register {i}"));
            return new Register32File(fpregs);
        }

        internal static ControlStatusRegFile InitControlStatusRegisterFile()
        {
            return new ControlStatusRegFile(new Dictionary<uint, Register32>()
            {
                {ControlStatusRegFile.CSRs.FFLAGS, new Register32(name:"fflags", meaning:"Floating-Point Accrued Exceptions.") },
                {ControlStatusRegFile.CSRs.FRM, new Register32(name:"frm", meaning:"Floating-Point Dynamic Rounding Mode.") },
                {ControlStatusRegFile.CSRs.FCSR, new Register32(name:"fcsr", meaning:"Floating-Point Control and Status Register (frm+fflags).") },
                
                {ControlStatusRegFile.CSRs.CYCLE, new Register32(name:"cycle", meaning:"Cycle counter for RDCYCLE instruction.") 
                {UserAccess = HardwareProperties.MemoryAccess.Read} },
                {ControlStatusRegFile.CSRs.TIME, new Register32(name:"time", meaning:"Timer for RDTIME instruction.") 
                {UserAccess = HardwareProperties.MemoryAccess.Read} },
                {ControlStatusRegFile.CSRs.INSTRET, new Register32(name:"instret", meaning:"Instructions-retired counter for RDINSTRET instruction.") 
                {UserAccess = HardwareProperties.MemoryAccess.Read} },
                
                {ControlStatusRegFile.CSRs.CYCLEH, new Register32(name:"cycleh", meaning:"Upper 32 bits of cycle, RV32I only.") 
                {UserAccess = HardwareProperties.MemoryAccess.Read} },
                {ControlStatusRegFile.CSRs.TIMEH, new Register32(name:"timeh", meaning:"Upper 32 bits of time, RV32I only.") 
                {UserAccess = HardwareProperties.MemoryAccess.Read} },
                {ControlStatusRegFile.CSRs.INSTRETH, new Register32(name:"instreth", meaning:"Upper 32 bits of instret, RV32I only.") 
                {UserAccess = HardwareProperties.MemoryAccess.Read} },
            });
        }
    }
}
