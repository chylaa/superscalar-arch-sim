using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using I32_TYPE = superscalar_arch_sim.RV32.ISA.ISAProperties.InstType;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage
{
    /// <summary>
    /// [ <i>WB</i> ] Fith stage of TYP pipeline. 
    /// Data of instructions arrived from <see cref="Pipeline.Stage.BufferPrev"/> are written to <see cref="Register32File"/>.
    /// (Request to <see cref="Memory.Memory"/> takes place in <see cref="MemoryReach"/>) stage.
    /// </summary>
    public class RegWriteBack : TYPStage
    {
        private readonly Register32File IntRegFile;
        private readonly Register32File FpRegFile;
        private readonly ControlStatusRegFile CSRs;
        private Register32 BP_LMD => BufferPrev.LoadMemoryData;
        private Register32 BP_Asource => BufferPrev.A;
        private Register32 BP_Imm => BufferPrev.Imm;
        private Register32 BP_ALUOut => BufferPrev.ALUOutput;

        private Int32? _writebackValue;
        private Int32? _csrWritebackValue;
        private Int32 _writebackRD;
        private Int32 _csrWritebackRD;

        public event DataWriteEventHandler RegisterWritten;

        public RegWriteBack(PipeRegisters prev, Register32File regfile, ControlStatusRegFile csrs, PipeRegisters next) 
            : base(HardwareProperties.TYPPipelineStage.Writeback, prev: prev, next: next)
        {
            IntRegFile = regfile;
            FpRegFile = null; // Not implemented
            CSRs = csrs;
            _writebackValue = null;
            _csrWritebackValue = null;
        }

        /// <returns><see cref="Tuple"/> of integer values: 
        /// <br></br>- CSR value to be written to Integer Register
        /// <br></br>- New CSR value to be written into CSR Register.</returns>
        private Tuple<int?, int?> ReadSetNewCSRValue(int oldcsr)
        {
            int operand = BP_Asource.Read();
            int rd = ProcessedInstruction.rd;

            int? intRegValue = null; // REG[rd] = CSR[imm] (oldcsr)
            int? csrRegValue = null; // CSR[imm] = REG[rs1] (BP_Asource)

            switch (ProcessedInstruction.funct3)
            {
                case 0b001: //CSRRW
                case 0b101: //CSRRWI
                    if (rd != 0) intRegValue = oldcsr;
                    csrRegValue = operand;
                    break;
                case 0b010: //CSRRS
                case 0b110: //CSRRSI
                    intRegValue = oldcsr;
                    if (operand != 0) csrRegValue |= operand;
                    break;
                case 0b011: //CSRRC
                case 0b111: //CSRRCI
                    intRegValue = oldcsr;
                    if(operand != 0) csrRegValue &= (~operand);
                    break;

                default:
                    throw new NotImplementedInstructionException(ProcessedInstruction);
            }
            return new Tuple<int?, int?>(intRegValue, csrRegValue);
        }

        public override void Cycle()
        {
            base.Cycle();
            
            I32_TYPE type = ProcessedInstruction.Type;
            _writebackRD = ProcessedInstruction.rd;

            if (Opcodes.IsLoad(ProcessedInstruction))
            {
                _writebackValue = BP_LMD.Read();
            }
            else if (type != I32_TYPE.B && type != I32_TYPE.S)
            {
                _writebackValue = BP_ALUOut.Read();
            }
            else if (Opcodes.IsCSR(ProcessedInstruction))
            {
                _csrWritebackRD = ProcessedInstruction.imm;
                ReadSetNewCSRValue(BP_LMD.Read()).Deconstruct(out _writebackValue, out _csrWritebackValue); 
            }
        }

        public override void Latch()
        {
            base.Latch();

            if (_writebackValue.HasValue && _writebackRD != 0)
            {
                int value = _writebackValue.Value;
                int rd = _writebackRD;
                IntRegFile[rd] = value;

                uint i32address = LocalPC.ReadUnsigned();
                RegisterWritten?.Invoke(this, new DataWriteEventArgs(unchecked((uint)value), unchecked((uint)rd), DataWriteEventArgs.WriteDestination.Register, ProcessedInstruction, i32address));
            }
            if (_csrWritebackValue.HasValue)
            {
                if (CSRs.IsValidCSR((uint)_csrWritebackRD)) {
                    CSRs[_csrWritebackRD] = _csrWritebackValue.Value;
                } else {
                    throw new Memory.InvalidMemoryAccess(HardwareProperties.MemoryAccess.Write, (uint)_csrWritebackRD,
                        "Cannot write CSR - address out of range or CSR is read-only");
                }
            }
            _writebackValue = null;
            _csrWritebackValue = null;
        }
    }
}
