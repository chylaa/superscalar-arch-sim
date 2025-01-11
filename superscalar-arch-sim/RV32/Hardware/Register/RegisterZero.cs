using System;
namespace superscalar_arch_sim.RV32.Hardware.Register
{
    /// <summary>
    /// Special register that on read always returns zero.
    /// </summary>
    public sealed class RegisterZero : Register32
    {
        /// <summary>
        /// Creates "zero" register. Invoke of <see cref="Write(int)"/>/<see cref="WriteUnsigned(uint)"/> method has no effect.
        /// Marked as <see cref="Register32.Preserved"/> = true.
        /// </summary>
        public RegisterZero() : base("zero", name: "x0", meaning:"Hardwired zero") 
        { 
            Value = 0;
        }
        /// <summary>Does not perform any operation whatever <paramref name="value"/> is passed.</summary>
        public override void Write(Int32 value) { /*Basically no-op*/ }
        /// <summary>Does not perform any operation whatever <paramref name="value"/> is passed.</summary>
        public override void WriteUnsigned(UInt32 value) { /*Basically no-op*/ }
        /// <summary>Does not perform any operation, <see cref="Register32.Value"/> remains 0.</summary>
        public override void Reset() { /*Basically no-op*/  }
    }
}
