using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Net;


namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage
{
    /// <summary>
    /// [ <i>MEM</i> ] Fourth stage of TYP pipeline. 
    /// In this stage, instructions that operate on memory (mainly Load/Store) access it (send request) to read/write values from/to memory.
    /// Also branch completion cycle - Next PC is written into Global PC.</summary>
    /// <remarks>Not "MemoryAccess" to avoid confusion with <see cref="HardwareProperties.MemoryAccess"/> enumeration.</remarks>
    public class MemoryReach : TYPStage
    {
        readonly Register32 GlobalPC;
        readonly MemoryManagmentUnit MMU;
        readonly ControlStatusRegFile CSRs;

        Register32 BN_LMD => BufferNext.LoadMemoryData;
        Register32 BP_ALUOutput => BufferPrev.ALUOutput;

        private UInt32 _storeValue;
        private Int32 _memoryData;
        private Int32 _globalPC;

        /// <summary>
        /// Invoked after replacing global PC with new address, if <see cref="Pipeline.Stage.ProcessedInstruction"/> was <b>Jump</b> instruction
        /// or <b>Branch</b> instruction and <see cref="Pipeline.Stage.BufferPrev"/> <see cref="PipeRegisters.Condition"/> was <see langword="true"/>.
        /// <see cref="StageDataArgs.DataA"/> contains branch destination address from <see cref="Register32"/> <see cref="PipeRegisters.ALUOutput"/>.
        /// </summary>
        public event EventHandler<StageDataArgs> ControlTransfered;

        public event DataWriteEventHandler MemoryWritten;

        public MemoryReach(PipeRegisters prev, Register32 gpc, MemoryManagmentUnit mmu, ControlStatusRegFile csrs, PipeRegisters next) 
            : base(HardwareProperties.TYPPipelineStage.Memory, prev: prev, next: next) 
        {
            GlobalPC = gpc;
            MMU = mmu;
            CSRs = csrs;
        }

        private Int32 LoadFromMemory(in Instruction i32)
        {
            switch (i32.funct3) // returns effective address for LOAD operation (addr in register source 1 + immeditate storeValue)
            {
                case 0b000: // LB  
                    return unchecked((SByte)(MMU.ReadByte(BP_ALUOutput.ReadUnsigned())));
                case 0b001: // LH 
                    return unchecked((Int16)(MMU.ReadHWord(BP_ALUOutput.ReadUnsigned())));
                case 0b010: // LW 
                    return unchecked((Int32)MMU.ReadWord(BP_ALUOutput.ReadUnsigned()));
                case 0b100: // LBU 
                    return unchecked((Byte)(MMU.ReadByte(BP_ALUOutput.ReadUnsigned())));
                case 0b101: // LHU 
                    return unchecked((UInt16)(MMU.ReadHWord(BP_ALUOutput.ReadUnsigned())));
                default:
                    throw new NotImplementedInstructionException(i32);
            }
        }

        private void StoreToMemory(in Instruction i32)
        {
            uint address = BP_ALUOutput.ReadUnsigned();
            _storeValue = BufferPrev.B.ReadUnsigned();
            switch (i32.funct3)
            {
                case 0b000: // SB  
                    MMU.WriteByte(address, (byte)(_storeValue &= 0x00_00_00_FF));
                    break;
                case 0b001: // SH 
                    MMU.WriteHWord(address, (UInt16)(_storeValue &= 0x00_00_FF_FF));
                    break;
                case 0b010: // SW 
                    MMU.WriteWord(address, _storeValue);
                    break;
                default:
                    throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
            }
      
        }

        private Int32 ReadCSRRegister(in Instruction i32)
        {
            switch (i32.funct3)
            {
                case 0b001: //CSRRW
                case 0b101: //CSRRWI
                    return (i32.rd != 0) ? CSRs[i32.imm] : _memoryData;
                case 0b010: //CSRRS
                case 0b011: //CSRRC
                case 0b110: //CSRRSI
                case 0b111: //CSRRCI
                    return CSRs[i32.imm];
                default:
                    throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
            }
        }

        private void OnMemoryStageCycle(in Instruction i32)
        {
            if (i32.opcode == Opcodes.OP_I_TYPE_LOADS)
                _memoryData = LoadFromMemory(i32);

            else if (i32.opcode == Opcodes.OP_S_TYPE_STORE)
                StoreToMemory(i32);

            else if (Opcodes.IsCSR(i32))
                _memoryData = ReadCSRRegister(i32);
            
            // else: passes through MEM stage
        }

        //private void OnMemoryStageCycle(in Instruction i32) //Old when PC updated in MEM stage 
        //{
        //    switch (i32.Type)
        //    {
        //        case (InstType.I):
        //            if (i32.opcode == Opcodes.OP_I_TYPE_LOADS) // Load instructions
        //                _memoryData = (LoadFromMemory(i32));
        //            else if (i32.opcode == Opcodes.OP_I_TYPE_JUMP) // JALR
        //                goto READ_PC;
        //            break;

        //        case (InstType.S): // Store instructions
        //            StoreToMemory(i32);
        //            break;

        //        case (InstType.B): // Branch instructions
        //            if (false == BufferPrev.Condition.HasValue)
        //                throw new PipelineProcessingException($"[{LocalPC}] B-Type instruction Expected Branch Condition but got null instead", i32);
        //            if (BufferPrev.Condition.Value)
        //                _globalPC = BP_ALUOutput.Read();
        //            break;

        //        case (InstType.J): READ_PC:  // Jump instructions
        //            _globalPC = LocalPC.Read();
        //            break;

        //        default:
        //            break; // passes through MEM stage
        //    }
        //}

        public override void Cycle()
        {
            base.Cycle();

            _globalPC = GlobalPC.Read();
            _memoryData = BufferPrev.LoadMemoryData.Read(); // save in case of non-load instruction
            OnMemoryStageCycle(ProcessedInstruction);
    
        }

        public override void Latch()
        {
            if (Opcodes.IsStore(ProcessedInstruction))
            {
                uint address = BP_ALUOutput.ReadUnsigned();
                uint i32address = LocalPC.ReadUnsigned();
                MemoryWritten?.Invoke(this, new DataWriteEventArgs(_storeValue, address, DataWriteEventArgs.WriteDestination.Memory, ProcessedInstruction, i32address));
            }
            base.Latch();
            BN_LMD.Write(_memoryData);
            //GlobalPC.Write(_globalPC);
        }

        public override void Reset()
        {
            base.Reset();
            _storeValue = Int32.MaxValue;
            _memoryData = Int32.MaxValue;
            _globalPC = Int32.MinValue;
        }
    }
}
