using superscalar_arch_sim.RV32.Hardware.Register;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System.Reflection.Emit;
using static superscalar_arch_sim.RV32.Hardware.Units.BranchPredictor;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units
{
    public class FetchAddressSelector
    {
        readonly BranchPredictor BranchPredictor;
        public FetchAddressSelector(BranchPredictor predictor) 
        {
            BranchPredictor = predictor;
        }

        public int GetNextFetchAddress(Register32 localPC, Instruction i32, out bool isControlTransferInstruction)
        {
            int? targetAddress = null;
            int iopcode = ((int)(i32.Value & 0b1111111));
            isControlTransferInstruction = false;
            if (iopcode == Opcodes.OP_B_TYPE_BRANCH)
            {
                isControlTransferInstruction = true;
                if (Prediction.Taken == BranchPredictor.SetCurrentPrediction(localPC.ReadUnsigned()))
                {
                    // Can return null if BTB does not contain corresponding entry - so predicting not-taken
                    targetAddress = BranchPredictor.GetPredictedTargetAddress(localPC);
                }
                
            } 
            else if (iopcode == Opcodes.OP_U_TYPE_JUMP || iopcode == Opcodes.OP_I_TYPE_JUMP)
            {
                isControlTransferInstruction = true;
                targetAddress = BranchPredictor.GetPredictedTargetAddress(localPC);
            }
            //---------------------------------------------------------------------------------------   
            if (targetAddress.HasValue)
            {
                return targetAddress.Value;
            }
            else
            {
                return (localPC.Read() + ISA.ISAProperties.WORD_BYTESIZE);         
            } 
        }



    }
}
