using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Stage;
using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.Units
{
    public class FetchAddressSelector
    {
        private bool BranchCondition => (TargetAddressSource.Condition.HasValue && TargetAddressSource.Condition.Value);
        private bool BranchConditionEvaluated => TargetAddressSource.Condition.HasValue;
        private readonly DatapathBufferEventArgs<PipeRegisters> ClearNextPCEventArg;
        private readonly DatapathBufferEventArgs<PipeRegisters> ClearALUOutEventArg;

        /// <summary>Set of pipeline registers containing calculated branch/jump target address and evaluated branch condition.</summary>
        public PipeRegisters TargetAddressSource { get; set; }
        /// <summary>
        /// Called when new PC is selected using value from <see cref="TargetAddressSource"/>. <see cref="DatapathBufferEventArgs.DataDest"/> is always <see langword="null"/>. 
        /// When datapath is cleared apart from <see cref="DatapathBufferEventArgs.Value"/> being <see langword="null"/>, 
        /// <see cref="DatapathBufferEventArgs.RegDest"/> is also <see langword="null"/>. 
        /// </summary>
        public GUIEventHandler<DatapathBufferEventArgs<PipeRegisters>> PCSelectedFromPipelineRegister;

        public FetchAddressSelector(PipeRegisters targetAddrSourceBuffer)
        {
            TargetAddressSource = targetAddrSourceBuffer;
            ClearNextPCEventArg = new DatapathBufferEventArgs<PipeRegisters>(TargetAddressSource, null, TargetAddressSource.NextPC, null, null);
            ClearALUOutEventArg = new DatapathBufferEventArgs<PipeRegisters>(TargetAddressSource, null, TargetAddressSource.ALUOutput, null, null);
        }

        private bool IsNextWordJump()
            => ((TargetAddressSource.LocalPC.Read() + ISA.ISAProperties.WORD_BYTESIZE) == TargetAddressSource.NextPC.Read());
        
        public int SelectPCValue(Register32 PCReg, TYPStage fetch, BranchPredictor predictor)
        {
            int pc;
            int iopcode = TargetAddressSource.IR32.opcode;

            PCSelectedFromPipelineRegister?.Invoke(this, ClearALUOutEventArg);
            PCSelectedFromPipelineRegister?.Invoke(this, ClearNextPCEventArg);

            if (false == fetch.Stalling && false == predictor.Enabled)
            {
                pc = PCReg.Read();
            }
            else if (iopcode == Opcodes.OP_I_TYPE_JUMP || (iopcode == Opcodes.OP_U_TYPE_JUMP && false == IsNextWordJump()))
            {
                pc = TargetAddressSource.NextPC.Read();
                var eventargs = new DatapathBufferEventArgs<PipeRegisters>(TargetAddressSource, null, TargetAddressSource.NextPC, PCReg, pc);
                PCSelectedFromPipelineRegister?.Invoke(this, eventargs);
            } 
            else if (false == predictor.Enabled && iopcode == Opcodes.OP_B_TYPE_BRANCH && BranchCondition)
            {
                pc = TargetAddressSource.ALUOutput.Read();
                var eventargs = new DatapathBufferEventArgs<PipeRegisters>(TargetAddressSource, null, TargetAddressSource.ALUOutput, PCReg, pc);
                PCSelectedFromPipelineRegister?.Invoke(this, eventargs);
            }
            else if (iopcode == Opcodes.OP_B_TYPE_BRANCH && (BranchCondition != predictor.PredictShouldBranch()))
            {
                Register32 newPCsrc;
                if (BranchConditionEvaluated && predictor.MispredictedTaken())
                {
                    newPCsrc = TargetAddressSource.NextPC;
                    predictor.CallMispredictionEvent(new StageDataArgs(TargetAddressSource.IR32, newPCsrc, lpc:TargetAddressSource.LocalPC));
                } 
                else if (BranchConditionEvaluated && predictor.MispredictedNotTaken())
                {
                    newPCsrc = TargetAddressSource.ALUOutput;
                    predictor.CallMispredictionEvent(new StageDataArgs(TargetAddressSource.IR32, newPCsrc, lpc: TargetAddressSource.LocalPC));

                } else { newPCsrc = TargetAddressSource.ALUOutput; }

                pc = newPCsrc.Read();
                var eventargs = new DatapathBufferEventArgs<PipeRegisters>(TargetAddressSource, null, newPCsrc, PCReg, pc);
                PCSelectedFromPipelineRegister?.Invoke(this, eventargs);
            } 
            else
            {
                pc = PCReg.Read();
            }
            return pc;
        }

        public int SelectPCValueFromPipeRegister(PipeRegisters targetAddrSrc, Register32 pcsrc, TYPStage fetch, BranchPredictor predictor )
        {
            var oldsrc = TargetAddressSource;
            TargetAddressSource = targetAddrSrc;
            int pc = SelectPCValue(pcsrc, fetch, predictor);
            TargetAddressSource = oldsrc;
            return pc;
        }
    }
}
