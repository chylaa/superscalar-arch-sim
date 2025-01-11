using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit
{
    /// <summary>
    /// Execute stage's Functional Unit: Branch Unit
    /// <br></br>Possible stages:
    /// <br></br>o Test
    /// <br></br>o Branch
    /// </summary>
    public class BranchUnit : ExecuteUnit
    {
        public BranchUnit(ReservationStationCollection stations) : base(stations, nameof(BranchUnit))
        {
        }

        private void ThrowOnTargetAddressMisaligned(int targetAddress, Instruction i32, Register32 LocalPC)
        {
            if (false == Utilis.Utilis.IsAlligned(targetAddress, Memory.Allign.WORD))
                throw new InstructionAddressMisaligned((uint)targetAddress, i32, Memory.Allign.WORD, LocalPC.ToString());
        }

        private Int32 ExecBTypeBranch(in ReservationStation station, out bool condition)
        {
            Instruction i32 = station.IR32;
            Register32 lpc = station.FetchLocalPC;
            int RA = station.OpVal1.Value; // Reg[rs1]
            int RB = station.OpVal2.Value; // Reg[rs2] / Imm

            switch (i32.funct3)
            {
                case 0b000: // BEQ 
                    condition = (RA.Equals(RB));
                    break;
                case 0b001:// BNE
                    condition = !(RA.Equals(RB));
                    break;
                case 0b100:// BLT (Branch if rs1 less than rs2 [signed])
                    condition = (RA < RB);
                    break;
                case 0b101:// BGE (Branch if rs1 greater or Equal rs2 [signed])
                    condition = (RA >= RB);
                    break;
                case 0b110:// BLTU -||- [unsigned]
                    condition = (unchecked((uint)RA) < unchecked((uint)RB));
                    break;
                case 0b111:// BGEU -||- [unsigned]
                    condition = (unchecked((uint)RA) >= unchecked((uint)RB));
                    break;
                default:
                    throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
            }
            
            if (condition) {
                Int32 targetaddr = station.A.Value; // Instruction.Imm of Branch instruction from ID stage
                targetaddr = unchecked((Int32)((targetaddr << ISAProperties.JMP_BRANCH_IMM_SHAMT) + lpc.ReadUnsigned()));
                ThrowOnTargetAddressMisaligned(targetaddr, station.IR32, lpc);
                return targetaddr;
            }
            return unchecked((int)(lpc.ReadUnsigned() + ISAProperties.WORD_BYTESIZE));
        }

        private Int32 ExecITypeJALR(in ReservationStation station, out Int32 targetAddress) 
        {
            Instruction i32 = station.IR32;
            Register32 lpc = station.FetchLocalPC;
            int RA = station.OpVal1.Value; // Reg[rs1]
            int RB = station.OpVal2.Value; // Reg[rs2] / Imm
            
            // apply rs1 to Imm to get final target address of Jump instruction
            targetAddress = ((RA + RB) & ~1);
            ThrowOnTargetAddressMisaligned(targetAddress, i32, lpc);

            Int32 nexti32addr = (lpc.Read() + ISAProperties.WORD_BYTESIZE);
            return nexti32addr; // return: address of the next instruction (written to ALUOutput and then to RegFile[rd] on WB)
            
        }

        private Int32 ExecUTypeJAL(in ReservationStation station, out Int32 targetAddress) 
        {
            // JAL target address already calculeted after Decode stage
            Register32 lpc = station.FetchLocalPC;
            int RB = station.OpVal2.Value; // Reg[rs2] / Imm

            targetAddress = unchecked((Int32)((RB << ISAProperties.JMP_BRANCH_IMM_SHAMT) + lpc.ReadUnsigned()));
            ThrowOnTargetAddressMisaligned(targetAddress, station.IR32, lpc);

            Int32 nexti32addr = (lpc.Read() + ISAProperties.WORD_BYTESIZE);
            return nexti32addr; // return: address of the next instruction (written to ALUOutput and then to RegFile[rd] on WB)
        }

        public override void Cycle()
        {
            base.Cycle();
            if (UsedReservationStation is null || ProcessedInstruction is null)
                return;

            if (ProcessedInstruction.opcode == Opcodes.OP_B_TYPE_BRANCH)
            {
                UsedReservationStation.A = ExecBTypeBranch(UsedReservationStation, out bool condition);
                EffectiveValue = condition ? 1 : 0;
            }
            else if (ProcessedInstruction.opcode == Opcodes.OP_I_TYPE_JUMP) // JALR
            {
                EffectiveValue = ExecITypeJALR(UsedReservationStation, out int targetAddress);
                UsedReservationStation.A = targetAddress;
            }
            else if (ProcessedInstruction.opcode == Opcodes.OP_U_TYPE_JUMP)
            {
                EffectiveValue = ExecUTypeJAL(UsedReservationStation, out int targetAddress);
                UsedReservationStation.A = targetAddress;
            }
            else
            {
                throw new NotImplementedInstructionException(ProcessedInstruction, cause: nameof(Instruction.opcode));
            }
        }

        public override void Latch()
        {
            base.Latch();
        }
        public override bool IsReady()
        {
            return base.IsReady();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
