using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace superscalar_arch_sim.RV32.Hardware.Register
{
    /*
    | Name    | ABI Mnemonic | Meaning                 | Preserved across calls? 
    -----------------------------------------------------------------------------
    | x0      | zero         | Zero                    | — (Immutable)
    | x1      | ra           | Return address          | No
    | x2      | sp           | Stack pointer           | Yes
    | x3      | gp           | Global pointer          | — (Unallocatable)
    | x4      | tp           | Thread pointer          | — (Unallocatable)
    | x5-x7   | t0-t2        | Temporary registers     | No
    | x8-x9   | s0-s1        | Callee-saved registers  | Yes
    | x10-x17 | a0-a7        | Argument registers      | No
    | x18-x27 | s2-s11       | Callee-saved registers  | Yes
    | x28-x31 | t3-t6        | Temporary registers     | No
     */
    public class Register32File : IEnumerable<Register32>, IReadOnlyCollection<Register32>
    {
        /// <summary>Architectural registers, accessible by user.</summary>
        private readonly Register32[] ArchRegisters = null;

        /// <summary>
        /// Each array's element holds a tag (<see cref="Pipeline.IUniqueInstructionEntry.Tag"/>) 
        /// that identifies producer of <see cref="Pipeline.IUniqueInstructionEntry.IR32"/> result. 
        /// This result should be stored into corresponding <see cref="Register32"/>.
        /// <br></br>
        /// Value 0 indicates that no instruction is currently computing a result for <see cref="Register32"/> with corresponding index (always true for <see cref="RegisterZero"/>).
        /// </summary>
        private readonly int[] RegisterStatus;

        /// <summary> Number of architectural registers </summary>
        public int Count => ArchRegisters.Length;

        /// <summary>Reads or writes signed value from architectre register at index <paramref name="index"/>.</summary>
        /// <param name="index">Index from 0 to <see cref="HardwareProperties.NumberOfArchitecturalRegisters"/>-1.</param>
        /// <returns>Value stored in <see cref="Register32.Value"/> at <paramref name="index"/>.</returns>
        public int this[int index] { get => Read(index); set => Write(index, value); }
        
        /// <summary>Reads or writes unsigned value from architectre register at index <paramref name="index"/>. </summary>
        /// <param name="index">Index from 0 to <see cref="HardwareProperties.NumberOfArchitecturalRegisters"/>-1.</param>
        /// <returns>Value stored in <see cref="Register32.Value"/> at <paramref name="index"/>.</returns>
        public uint this[uint index] { get => ReadUnsigned((int)index); set => WriteUnsigned((int)index, value); }

        /// <summary>Allows to access specific <see cref="Register32"/> instance that is a part of this <see cref="Register32File"/>.</summary>
        /// <param name="idx">Index of register (0-31).</param>
        /// <returns><see cref="Register32"/> instance.</returns>
        public virtual Register32 GetRegister(int idx) => ArchRegisters[idx];

        /// <summary>Base constructor</summary>
        protected Register32File() { }

        /// <summary>
        /// Creates new <see cref="Register32File"/> filled with architectural registers passed as <paramref name="archregs"/> list. 
        /// </summary>
        /// <param name="archregs">Collection of <see cref="Register32"/> objects to contain within <see cref="Register32File"/>.</param>
        public Register32File(IEnumerable<Register32> archregs)
        {
            ArchRegisters = archregs.ToArray();
            RegisterStatus = Enumerable.Repeat(0, archregs.Count()).ToArray();
        }


        public Int32 Read(int reg)
        {
            return GetRegister(reg).Read();
        }
        public UInt32 ReadUnsigned(int reg)
        {
            return GetRegister(reg).ReadUnsigned();
        }

        public void Write(int reg, Int32 value) 
        { 
            GetRegister(reg).Write(value); 
        }
        public void WriteUnsigned(int reg, UInt32 value)
        {
            GetRegister(reg).WriteUnsigned(value);
        }

        /// <returns>
        /// Index indicating which <see cref="Pipeline.IUniqueInstructionEntry.Tag"/> 
        /// will produce result for <see cref="Register32"/> at <paramref name="registerIdx"/>.
        /// Value 0 indicates that <see cref="Register32"/> at <paramref name="registerIdx"/>
        /// already contains valid data (always returned for <see cref="RegisterZero"/>).
        /// </returns>
        public int GetTagFromRegisterStatus(int registerIdx)
            => registerIdx == 0 ? 0 : RegisterStatus[registerIdx];

        /// <summary>
        /// Saves tag indicating which <see cref="Pipeline.IUniqueInstructionEntry.Tag"/> 
        /// identifies producer of result for <see cref="Register32"/> at <paramref name="registerIdx"/>.
        /// </summary>
        public void SetProducerTag(int registerIdx, int tag)
            => RegisterStatus[registerIdx] = tag;

        /// <summary>
        /// Resets back to 0 the tag of producer (<see cref="Pipeline.IUniqueInstructionEntry.Tag"/>) 
        /// associated with <see cref="Register32"/> at <paramref name="registerIdx"/>.
        /// </summary>
        /// <param name="registerIdx">Index of <see cref="Register32"/> for which assoctiated tag should be set to 0.</param>
        public void ClearRegisterStatusProducerTag(int registerIdx)
            => RegisterStatus[registerIdx] = 0;

        /// <summary>
        /// Performs <see cref="Register32.Reset"/> on all <see cref="ArchRegisters"/>.
        /// </summary>
        public virtual void Reset()
        {
            int regidx = 0;
            foreach (var register in ArchRegisters)
            {
                ClearRegisterStatusProducerTag(regidx++);
                register.Reset();
            }
        }

        public int[] GetRegisterStatusCopy()
        {
            int[] rs = new int[Count];
            RegisterStatus.CopyTo(rs, 0);
            return rs;
        }

        public IEnumerator<Register32> GetEnumerator()
            => ((IEnumerable<Register32>)ArchRegisters).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ArchRegisters.GetEnumerator();
    }
}
