using superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.Units;
using superscalar_arch_sim.Utilis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline.TEM.FuncUnit
{
    /// <summary>
    /// Execute stage's Functional Unit: Floating-Point Unit
    /// <br></br>Possible stages:
    /// <br></br>o Float-1, ..., Float-N (typically more than in <see cref="IntUnit"/>)
    /// <br></br>o Writeback
    /// </summary>
    public class FPUnit : ExecuteUnit
    {
        public FPUnit(ReservationStationCollection reservationTags) : base(reservationTags, nameof(FPUnit))
        {
        }

        public override void Cycle()
        {
            base.Cycle();
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
