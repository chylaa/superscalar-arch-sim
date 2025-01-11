using superscalar_arch_sim.RV32.Hardware.Pipeline.TYP.Units;
using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.ISA.Instructions;
using superscalar_arch_sim.Simulis;
using superscalar_arch_sim.Simulis.Reports;
using System;


namespace superscalar_arch_sim.RV32.Hardware.Pipeline.Units
{
    public class DataDependencyController
    {
        private readonly PipeRegisters ID_EX;
        private readonly PipeRegisters EX_MEM;
        private readonly PipeRegisters MEM_WB;

        private readonly SimReporter SimReporter;

        /// <summary><see cref="Settings.Static_UseForwarding"/></summary>
        private bool ForwardingEnabled => Settings.Static_UseForwarding;

        /// <summary>Called when <see cref="DataDependencyController"/> detects data dependency and feeds result of one stage to another.</summary>
        public GUIEventHandler<DatapathBufferEventArgs<PipeRegisters>> DataForwarded { get; set; }
        /// <summary>Called when <see cref="DataDependencyController"/> detects data dependency and <see cref="Settings.Static_UseForwarding"/> is <see langword="false"/>.</summary>
        public GUIEventHandler<DatapathBufferEventArgs<PipeRegisters>> NoForwardingDataDependencyDetected { get; set; }

        
        public DataDependencyController(PipeRegisters ALUInputBuffer,
                                       PipeRegisters ALUOutputBuffer, 
                                       PipeRegisters MEMOutputBuffer,
                                       SimReporter simreport) 
        {
            ID_EX = ALUInputBuffer;
            EX_MEM = ALUOutputBuffer;
            MEM_WB = MEMOutputBuffer;
            SimReporter = simreport;
        }

        private void WriteALUInputBufferAndInvokeEvent(Register32 dstALUIn, PipeRegisters dstBuffer, Register32 srcReg, PipeRegisters srcBuffer, ref ulong fwcounter)
        {
            if (ForwardingEnabled) 
            {
                Int32 forwardingValue = srcReg.Read();
                dstALUIn.Write(forwardingValue);
                ++fwcounter;
                DataForwarded?.Invoke(this, new DatapathBufferEventArgs<PipeRegisters>(srcBuffer, dstBuffer, srcReg, dstALUIn, forwardingValue));
            } 
            else
            {
                NoForwardingDataDependencyDetected?.Invoke(this, new DatapathBufferEventArgs<PipeRegisters>(srcBuffer, dstBuffer, srcReg, dstALUIn, null));
            }
            ++(SimReporter.DataDependencies);
        }

        #region Feed back Execute stage ALU inputs, from EX/MEM and MEM/WB ALUOutput

        /// <summary>
        /// Destination register is always <see cref="ID_EX"/>. To return <see langword="true"/>, <paramref name="srcReg"/> 
        /// instruction must be valid for <paramref name="srcOpcodeComparer"/> and and <see cref="ID_EX"/> 
        /// <see cref="Instruction.rs1"/> must equal <paramref name="srcReg"/> <see cref="Instruction.rd"/>
        /// </summary>
        private bool IsValidInstructionToForwardToA(PipeRegisters srcReg, Func<Instruction, bool> srcOpcodeComparer)
        {
            Int32 idex_rs1 = ID_EX.IR32.rs1;
            bool validopcode = srcOpcodeComparer(srcReg.IR32);
            bool needsForwarding = (idex_rs1 != 0 && idex_rs1 == srcReg.IR32.rd);
            return (validopcode && needsForwarding);
        }

        /// <summary>Source: ALUOutput/LoadMemoryData | Destination: ALUInput 1.</summary>
        private void ForwardToALUInput_A(ref bool LMDForwarded)
        {
            Int32 idex_rs1 = ID_EX.IR32.rs1;

            if (IsValidInstructionToForwardToA(EX_MEM, srcOpcodeComparer:Opcodes.IsRegRegOrImmALU))
            {
                ulong fwcounter = SimReporter.FeedbackALUInput;
                WriteALUInputBufferAndInvokeEvent(ID_EX.A, ID_EX, EX_MEM.ALUOutput, EX_MEM, ref fwcounter);
                SimReporter.FeedbackALUInput = fwcounter;     // EX_MEM ALU Output to ALU Input 1
            } 
            else if (IsValidInstructionToForwardToA(MEM_WB, srcOpcodeComparer: Opcodes.IsRegRegOrImmALU))
            {
                ulong fwcounter = SimReporter.FeedbackALUInput;
                WriteALUInputBufferAndInvokeEvent(ID_EX.A, ID_EX, MEM_WB.ALUOutput, MEM_WB, ref fwcounter);
                SimReporter.FeedbackALUInput = fwcounter;     // MEM_WB ALU Output to ALU Input 1
            } 
            else if (IsValidInstructionToForwardToA(MEM_WB, srcOpcodeComparer: Opcodes.IsLoad))
            {
                ulong fwcounter = SimReporter.ForwardMEMLoadToALU;
                WriteALUInputBufferAndInvokeEvent(ID_EX.A, ID_EX, MEM_WB.LoadMemoryData, MEM_WB, ref fwcounter);
                SimReporter.ForwardMEMLoadToALU = fwcounter;  // MEM_WB LMD to ALU Input 1
                LMDForwarded = true;
            }
        }
        /// <summary>
        /// Destination register is always <see cref="ID_EX"/>. To return <see langword="true"/>, 
        /// <see cref="ID_EX"/> <see cref="PipeRegisters.IR32"/> must be Register-Register ALU or Store operation,
        /// <paramref name="srcReg"/> instruction must be valid for <paramref name="srcOpcodeComparer"/> and <see cref="ID_EX"/> 
        /// <see cref="Instruction.rs2"/> must equal <paramref name="srcReg"/> <see cref="Instruction.rd"/>
        /// </summary>
        private bool IsValidInstructionToForwardToB(PipeRegisters srcReg, Func<Instruction, bool> srcOpcodeComparer)
        {
            Int32 idex_rs2 = ID_EX.IR32.rs2;
            bool validopcode = srcOpcodeComparer(srcReg.IR32) && ( Opcodes.IsRegRegALUOrBranch(ID_EX.IR32) || Opcodes.IsStore(ID_EX.IR32) ) ;
            bool needsForwarding = (idex_rs2 != 0 && idex_rs2 == srcReg.IR32.rd);
            return (validopcode && needsForwarding);
        }

        /// <summary>Source: ALUOutput/LoadMemoryData | Destination: ALUInput 2.</summary>
        private void ForwardToALUInput_B(ref bool LMDForwarded)
        {
            Int32 idex_rs2 = ID_EX.IR32.rs2;

            if (IsValidInstructionToForwardToB(EX_MEM, srcOpcodeComparer:Opcodes.IsRegRegOrImmALU))
            {
                ulong fwcounter = SimReporter.FeedbackALUInput;
                WriteALUInputBufferAndInvokeEvent(ID_EX.B, ID_EX, EX_MEM.ALUOutput, EX_MEM, ref fwcounter);
                SimReporter.FeedbackALUInput = fwcounter;     // EX_MEM ALU Output to ALU Input 2

            } 
            else if (IsValidInstructionToForwardToB(MEM_WB, srcOpcodeComparer:Opcodes.IsRegRegOrImmALU))
            {
                ulong fwcounter = SimReporter.FeedbackALUInput;
                WriteALUInputBufferAndInvokeEvent(ID_EX.B, ID_EX, MEM_WB.ALUOutput, MEM_WB, ref fwcounter);
                SimReporter.FeedbackALUInput = fwcounter;     // MEM_WB ALU Output to ALU Input 2
            } 
            else if (IsValidInstructionToForwardToB(MEM_WB, srcOpcodeComparer:Opcodes.IsLoad))
            {
                ulong fwcounter = SimReporter.ForwardMEMLoadToALU;
                WriteALUInputBufferAndInvokeEvent(ID_EX.B, ID_EX, MEM_WB.LoadMemoryData, MEM_WB, ref fwcounter);
                SimReporter.ForwardMEMLoadToALU = fwcounter;  // MEM_WB LMD to ALU Input 2
                LMDForwarded = true;
            } 
            else if (false == ForwardingEnabled && IsValidInstructionToForwardToB(EX_MEM, srcOpcodeComparer: Opcodes.IsLoad))
            {   // only when forwarding disabled, invoke 'NoForwardingDataDependencyDetected' event, and wait for writeback
                ulong dummy = 0;
                WriteALUInputBufferAndInvokeEvent(ID_EX.B, ID_EX, null, EX_MEM, ref dummy);
            }
        }
        #endregion

        #region Feed back Memory Stage B Input (value), with MEM/WB LMData (for Store instructions)
        private void ForwardToMEMInput_B(bool LMDWasForwardedToALU)
        {
            Int32 exmem_rs1 = EX_MEM.IR32.rs1;
            Int32 emmem_rs2 = EX_MEM.IR32.rs2;
            bool dstIsStore = (EX_MEM.IR32.opcode == Opcodes.OP_S_TYPE_STORE); // Store instruction about to enter MEM stage
            bool srcIsLoad = (MEM_WB.IR32.opcode == Opcodes.OP_I_TYPE_LOADS); // Load instruction finished MEM stage

            if (dstIsStore && srcIsLoad && MEM_WB.IR32.rd != 0 && ((exmem_rs1 == MEM_WB.IR32.rd) || emmem_rs2 == MEM_WB.IR32.rd))
            {
                ulong fwcounter = SimReporter.FeedbackMEMInput;
                WriteALUInputBufferAndInvokeEvent(EX_MEM.B, EX_MEM, MEM_WB.LoadMemoryData, MEM_WB, ref fwcounter);
                SimReporter.FeedbackMEMInput = fwcounter; // MEM LMD Output to MEM Addr Input
            } 
            else if (ForwardingEnabled)
            {   // Clear selection on datapaths and registers (.LoadMemoryData only if it was not forwarded to ALU Input)
                Register32 RegToClear = (LMDWasForwardedToALU ? null : MEM_WB.LoadMemoryData); // clear 
                DataForwarded?.Invoke(this, new DatapathBufferEventArgs<PipeRegisters>(MEM_WB, EX_MEM, RegToClear, EX_MEM.B, null));
            }
        }
        #endregion

        /// <summary>
        /// Detects data dependencies between instruction in flight and eliminates 
        /// them by forwarding results back to neccessary <see cref="PipeRegisters"/>.
        /// </summary>
        public void ForwardingControl()
        {
            // Clear Previous GUI selections
            if (ForwardingEnabled)
            {
                DataForwarded?.Invoke(this, new DatapathBufferEventArgs<PipeRegisters>(EX_MEM, ID_EX, EX_MEM.ALUOutput, ID_EX.A, null));
                DataForwarded?.Invoke(this, new DatapathBufferEventArgs<PipeRegisters>(MEM_WB, ID_EX, MEM_WB.ALUOutput, ID_EX.B, null));
            }
            // 'true' if memory data was forwarded to ALU Input
            bool LMDForwarded = false;

            // EX_MEM/MEM_WB (ALUOut/[ALUOut/LMD]) to ID_EX (ALUIn.A/B)
            ForwardToALUInput_A(ref LMDForwarded);
            ForwardToALUInput_B(ref LMDForwarded);
            
            // MEM_WB (LMD) to EX_MEM (ALUIn.B)[load value]
            ForwardToMEMInput_B(LMDForwarded);
        }
        /// <summary>
        /// Introduces stalls to pipeline when ineviteble <b>data hazard</b> situation is deteded
        /// (<see cref="MEM"/> stage produces value needed by <see cref="EX"/> stage 
        /// <u>in the same [current] clock cycle</u>).  
        /// <br></br><br></br><b>Note to myself:</b> 
        /// Currently only <see cref="Opcodes.OP_I_TYPE_LOADS"/> case, potentially can change when more extensions are added.
        /// </summary>
        /// <returns><see langword="true"/> if load data hazard detected, <see langword="false"/> otherwise.</returns>
        public bool LoadInterlockHazardDetected() // in next cycle...
        {
            Instruction iprev = EX_MEM.IR32; // instruction about to enter Stage.Memory
            if (iprev.opcode == Opcodes.OP_I_TYPE_LOADS) // [Only LOADs produce, in MEM, value needed by EX]
            {
                Instruction inext = ID_EX.IR32; //  instruction after iprev, about to enter Stage.Execute
                return (iprev.rd == inext.rs1) || ( (iprev.rd == inext.rs2) && Opcodes.IsRegRegALUOrBranch(inext) ); // data hazard requiring stall
            }
            return false;
        }
    }
}
