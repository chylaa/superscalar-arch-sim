using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit
{
    /// <summary>
    /// Execute stage's Functional Unit: Integer Unit
    /// <br></br>Possible stages:
    /// <br></br>o Int-1, ..., Int-N (typically less than in <see cref="FPUnit"/>)
    /// <br></br>o Writeback
    /// </summary>
    public class IntUnit : ExecuteUnit
    {
        /// <summary>Mask for upper 32 bits of 64bit number.</summary>
        private const Int64 MASK_UPPER_XLEN = (0xFF_FF_FF_FFL << 32);
        /// <summary>Bitmask used to extract shift amount (<i>shamt</i>) operand from instruction.</summary>
        private const int SHIFT_MASK = 0b0001_1111;

        internal EventHandler<StageDataArgs> EnvironmentBreakIssuedToExecutionUnit;

        public IntUnit(ReservationStationCollection stations) : base(stations, nameof(IntUnit))
        {
        }
   
        private Int32 ExecRType(in ReservationStation station)
        {
            Instruction i32 = station.IR32;
            Register32 LocalPC = station.FetchLocalPC;

            // Overflows are ignored and the low XLEN bits of results are written to the destination rd.
            if (i32.opcode == Opcodes.OP_R_TYPE_ARITHMETIC) // arithmetic operations
            {
                int VA = station.OpVal1.GetValueOrDefault();
                int VB = UsedReservationStation.OpVal2.GetValueOrDefault();
                uint VAU = unchecked((uint)(VA)); 
                uint VBU = unchecked((uint)(VB));

                // RV32M Standard Extension
                if (i32.funct7 == 0b0000001)
                {
                    switch (i32.funct3) // DIV [rs1=dividend rs2=divisor] | MUL [rs1=multiplier rs2=multiplicand]
                    {
                        case 0b000: // MUL      [return XLEN bits of product for signed * signed]
                            return (Int32)((VA * VB) & ISA.ISAProperties.UNSIGNED_MAX);
                        case 0b001: // MULH     [return upper XLEN bits of product for signed * signed]
                            return unchecked((Int32)(((VA * (Int64)VB) & MASK_UPPER_XLEN) >> 32));
                        case 0b010: // MULHSU   [return upper XLEN bits of product for signed * unsigned]
                            return unchecked((Int32)((VA * VBU) & MASK_UPPER_XLEN) >> 32);
                        case 0b011: // MULHU    [return upper XLEN bits of product for unsigned * unsigned]
                            return ((Int32)((VAU * VBU) & MASK_UPPER_XLEN) >> 32);

                        case 0b100: // DIV      [return XLEN bits signed * signed, rounding towards zero]                  
                            if (VB == 0)
                            {
                                DivisionByZero?.Invoke(this, new StageDataArgs(i32, VB, null, LocalPC));
                                return -1;
                            } else if (VA == Int32.MinValue && VB == -1)
                            {
                                SignedOverflow?.Invoke(this, new StageDataArgs(i32, VA, VB, LocalPC));
                                return Int32.MinValue;
                            } else
                                return (VA / VB);

                        case 0b101: // DIVU     [return XLEN bits unsigned * unsigned, rounding towards zero]
                            if (VBU == 0)
                            {
                                DivisionByZero?.Invoke(this, new StageDataArgs(i32, VB, null, LocalPC));
                                return -1;
                            } else
                                return (Int32)(VAU / VBU);

                        case 0b110: // REM      [return signed remiainder of div, sign equals sign of rs1]
                            if (VA == Int32.MinValue && VB == -1)
                            {
                                SignedOverflow?.Invoke(this, new StageDataArgs(i32, VA, VB, LocalPC));
                                return 0;
                            } else
                                return (VA % VB);

                        case 0b111: // REMU     [return unsigned remiainder of div]
                            return unchecked((Int32)(VAU % VBU));
                        default:
                            throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
                    }
                }
                // else RV32I base 
                switch (i32.funct3)
                {
                    case 0b000 when i32.funct7 == 0b0000000: // ADD
                        return (Int32)((VA + VB) & ISA.ISAProperties.UNSIGNED_MAX);
                    case 0b000 when i32.funct7 == 0b0100000: // SUB
                        return (Int32)((VA - VB) & ISA.ISAProperties.UNSIGNED_MAX);
                    case 0b001: // SLL
                        return (VA << VB);
                    case 0b010: // SLT Set Less Than [ set '1' in rd (if rs1 < rs2) else '0' in rd ]
                        return (VA < VB) ? 1 : 0;
                    case 0b011: // SLTU              [ same as SLT but treat numbers as unsigned ]
                        return (VAU < VBU) ? 1 : 0;
                    case 0b100: // XOR
                        return (VA ^ VB);
                    case 0b101 when i32.funct7 == 0b0000000: // SRL
                        return unchecked((Int32)(VAU >> VB));  // shift right logic
                    case 0b101 when i32.funct7 == 0b0100000: // SRA
                        return (VA >> VB);  // shift right arithmetic
                    case 0b110: // OR
                        return (VA | VB);
                    case 0b111: // AND
                        return (VA & VB);
                    default:
                        throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
                }
            } else
            {
                throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.opcode));
            }
        }

        private Int32 ExecIType(in ReservationStation station)
        {
            Instruction i32 = station.IR32;

            if (i32.Value == Opcodes.INSTR_EBREAK) {
                EnvironmentBreakIssuedToExecutionUnit?.Invoke(this, new StageDataArgs(i32, lpc: station.FetchLocalPC));
                return EffectiveValue; // Pass EBREAK if continued
            }

            if (i32.opcode == Opcodes.OP_I_TYPE_ARITHMETIC) // arithmetic operations
            {
                int RA = station.OpVal1.GetValueOrDefault(); // Reg[rs1]
                int RB = station.OpVal2.GetValueOrDefault(); // Reg[rs2] / Imm
                uint RAU = unchecked((uint)(RA));
                uint RBU = unchecked((uint)(RB));

                switch (i32.funct3)
                {
                    case 0b000: // ADDI
                        return unchecked((Int32)(((Int64)RA + (Int64)RB) & ISA.ISAProperties.UNSIGNED_MAX)); // overflow ignored (result is simply the low XLEN bits of the result)
                    case 0b010: // SLTI - Set < Immediate [ set '1' in rd (if rs1 < Immediate) else '0' in rd ]
                        return (RA < RB) ? 1 : 0;
                    case 0b011: // SLTIU - Set < Immediate Unsigned [ same as SLTI but treat numbers as unsigned ]
                        return (RAU) < RBU ? 1 : 0;
                    case 0b100: // XORI
                        return (RA ^ RB);
                    case 0b110: // ORI
                        return (RA | RB);
                    case 0b111: // ANDI
                        return (RA & RB);
                    case 0b001: // SLLI [shift left logical imm]
                        return (RA << (RB & SHIFT_MASK)); // shift amout encoded in lower 5 bits of value
                    case 0b101: // SRLI, SRAI
                        if (((RB & (1 << 10)) >> 10) == 0) // SRLI [shift right logical imm] - ignore sign-bit: fill with zeroes
                            return unchecked((int)((RAU >> (RB & SHIFT_MASK)))); // C#: If the left-hand operand is of type uint or ulong, the right-shift operator performs a LOGICAL shift.
                        else // SRAI [shift right arithmetic imm] - preserve sign-bit: fill with ones
                            return (RA >> (RB & SHIFT_MASK)); // C#: If the left-hand operand is int or long, the right-shift operator performs a ARITMETIC shift:
                    default:
                        throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
                }
            } 
            else if (i32.opcode == Opcodes.OP_I_TYPE_LOADS) // Load instructions
            {
                throw new InvalidPipelineState("Load instruction tries to execute in Integer Execution Unit!");
            } 
            else if (i32.opcode == Opcodes.OP_I_TYPE_JUMP) // JALR
            {
                throw new InvalidPipelineState("JARL jump instruction tries to execute in Integer Execution Unit!");
            } 
            else
            {
                throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.opcode));
            }
        }
        private Int32 ExecUType(in ReservationStation station)
        {
            Instruction i32 = station.IR32;
            Register32 LocalPC = station.FetchLocalPC;
            int RB = station.OpVal2.GetValueOrDefault(); // Reg[rs2] / Imm

            if (i32.opcode == 0b0110111) // LUI
                return (RB << 12);
            else if (i32.opcode == 0b0010111) // AUIPC 
                // Add address of 'AUIPC' instruction to specified upper imm
                return unchecked((Int32)(UInt32.MaxValue & ((RB << 12) + LocalPC.ReadUnsigned()))); // (& UInt32.MaxValue to not overflow but 'cut' to only 32bit's [following RISC-V ISA])
            else
                throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.opcode));
        }
        public override void Cycle()
        {
            base.Cycle();
            if (UsedReservationStation is null || ProcessedInstruction is null)
                return;

            switch (ProcessedInstruction.Type)
            {
                case ISA.ISAProperties.InstType.R:
                    EffectiveValue = ExecRType(UsedReservationStation);
                    break;
                case ISA.ISAProperties.InstType.I:
                    EffectiveValue = ExecIType(UsedReservationStation);
                    break;
                case ISA.ISAProperties.InstType.U:
                    EffectiveValue = ExecUType(UsedReservationStation);
                    break;
                default:
                    if (false == Opcodes.IsSystem(ProcessedInstruction))
                        throw new NotImplementedInstructionException(ProcessedInstruction, cause: nameof(Instruction.Type));
                    break; // do smth with system instructions in future?
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
