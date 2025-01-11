using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit
{
    /// <summary>
    /// Execute stage's Functional Unit: Memory Unit (Load/Store instructions)
    /// <br></br>Possible stages:
    /// <br></br>o Address
    /// <br></br>o Mem-1, ..., Mem-N
    /// <br></br>o Writeback
    /// </summary>
    public class MemUnit : ExecuteUnit
    {
        private readonly MemoryManagmentUnit MMU;

        /// <summary>Invoked when effective address of Store <see cref="Instruction"/> is calculated.</summary>
        public GUIEventHandler<StageDataArgs> StoreEffectiveAddressCalculated;

        public MemUnit(ReservationStationCollection stations, MemoryManagmentUnit mmu) 
            : base(stations, nameof(MemUnit))
        {
            MMU = mmu;
        }
        private Int32 LoadFromMemory(in Instruction i32, uint address)
        {
            switch (i32.funct3)
            {
                case 0b000: // LB  
                    return unchecked((SByte)(MMU.ReadByte(address)));
                case 0b001: // LH 
                    return unchecked((Int16)(MMU.ReadHWord(address)));
                case 0b010: // LW 
                    return unchecked((Int32)MMU.ReadWord(address));
                case 0b100: // LBU 
                    return unchecked((Byte)(MMU.ReadByte(address)));
                case 0b101: // LHU 
                    return unchecked((UInt16)(MMU.ReadHWord(address)));
                default:
                    throw new NotImplementedInstructionException(i32, cause: nameof(Instruction.funct3));
            }
        }

        private Int32 CalculateStoreEffectiveAddress(in ReservationStation station)
        {
            if (station.A is null || station.OpVal2 is null)
                throw new InvalidPipelineState($"{station.IR32} from station {station.Tag} does not have avaliable A or Vj values!");

            station.A = (station.OpVal2.Value + station.A.Value); // update effective address value
            station.EffectiveAddressCalulated = true; 
            return station.A.Value; // return store address as placeholder
        }

        public override void Cycle()
        {
            base.Cycle();
            if (UsedReservationStation is null || ProcessedInstruction is null)
                return;

            if (UsedReservationStation.A is null)
                throw new InvalidPipelineState($"Effective address from station {UsedReservationStation.Tag} is null ({ProcessedInstruction}).");
           
            if (Opcodes.IsLoad(ProcessedInstruction))
            {
                uint address = unchecked((uint)(UsedReservationStation.A.Value));
                EffectiveValue = LoadFromMemory(ProcessedInstruction, address);
            } 
            else if (Opcodes.IsStore(ProcessedInstruction))
            {
                EffectiveValue = CalculateStoreEffectiveAddress(UsedReservationStation);
                StoreEffectiveAddressCalculated?.Invoke(this, new StageDataArgs(ProcessedInstruction, UsedReservationStation.A.Value));
            } 
            else
            {
                throw new NotImplementedInstructionException(ProcessedInstruction, cause: nameof(Instruction.Type));
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
