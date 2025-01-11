using superscalar_arch_sim.RV32.Hardware.Memory;
using superscalar_arch_sim.RV32.ISA.Instructions;
using System;

namespace superscalar_arch_sim.RV32.Hardware.Pipeline
{
    public class EnvironmentBreak : Exception { public EnvironmentBreak(string msg) : base(msg) { } }
    public class InvalidPipelineState : Exception { public InvalidPipelineState(string msg) : base(msg) { } }
    public class PipelineProcessingException : Exception 
    {
        /// <summary><see cref="Instruction"/> instance that caused exception to be thrown.</summary>
        public Instruction Cause { get; protected set; }
        public PipelineProcessingException(string msg) : base(msg) { } 
        public PipelineProcessingException(string msg, Instruction cause) : base(msg) 
        { Cause = cause; Source = $"{cause.ASM} ({cause.Value:X8})"; } 
    }

    public class NotImplementedInstructionException : PipelineProcessingException 
    { 
        public NotImplementedInstructionException(string msg) : base(msg) { } 
        public NotImplementedInstructionException(string msg, Instruction cause) : base(msg, cause) { } 
        public NotImplementedInstructionException(Instruction cause) : base($"Instruction {cause} not implemented!", cause) { }

        /// <summary>
        /// Sets <see cref="Instruction.Illegal"/> property in <paramref name="i32"/> and creates new <see cref="NotImplementedInstructionException"/>
        /// with message containing instruction details and optional <paramref name="cause"/>.
        /// </summary>
        /// <param name="i32"><see cref="Instruction"/> that cannot be executed.</param>
        /// <param name="cause">Optional string indicating what operand of <paramref name="i32"/> cannot be recognized.</param>
        /// <returns>New <see cref="NotImplementedInstructionException"/> instance that can be thrown.</returns>
        public NotImplementedInstructionException(Instruction i32, string cause = null)
        : base($"{(cause is null ? string.Empty : ('[' + cause + ']' + ' '))}Instruction Type-{i32.Type}: {i32.Value:X8} EXecution not implemented", i32)
        {
            i32.MarkIllegal();
        }
    }

    public class InvalidOperationException : PipelineProcessingException { public InvalidOperationException(string msg) : base(msg) { } }

    /// <summary>
    /// The conditional branch instructions will generate an <see cref="InstructionAddressMisaligned"/> <see cref="Exception"/> if the
    /// target address is not aligned to a four-byte boundary and the branch condition evaluates to <see langword="true"/>.
    /// If the branch condition evaluates to <see langword="false"/>, <see cref="InstructionAddressMisaligned"/> <see cref="Exception"/> 
    /// should not be raised.
    /// </summary>
    public class InstructionAddressMisaligned: PipelineProcessingException 
    {
        /// <summary>Fetch Address of <see cref="Cause"/> <see cref="Instruction"/>.</summary>
        public UInt32 Address { get; private set; }
        public InstructionAddressMisaligned(uint addr, Instruction cause, Allign allign, string msg="") 
        : base($"Destination address {addr:X8} not alligned to {allign} boundary. Caused by {cause}. {msg}", cause) 
        { Address = addr; } 
    }

}
