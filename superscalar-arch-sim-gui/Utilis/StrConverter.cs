using System;
using System.Globalization;
using System.Text;

namespace superscalar_arch_sim_gui.Utilis
{
    /// <summary>Used to convert between values and strings using styles defined in <see cref="StringStyle"/> enum.</summary>
    public static class StrConverter
    {
        public enum StringStyle { None, Hex, SignedInt, Float, ASCII, Instruction };

        private static StringStyle _default = StringStyle.Hex;

        /// <summary>Style used when <see cref="StringStyle.None"/> is provided to any of <see cref="StrConverter"/> method.
        /// Value <see cref="StringStyle.None"/> will not be accepted.</summary>
        public static StringStyle DefaultStyle { get => _default; set { if (value != StringStyle.None) _default = value; } }

        private static StringStyle GetStyle(StringStyle style)
        {
            if (style == StringStyle.None) return DefaultStyle;
            return style;
        }

        public static bool TryParse(StringStyle style, string text, out uint value) 
        {
            bool result;
            switch (GetStyle(style))
            {
                case StringStyle.Hex:
                AsHex:
                    return uint.TryParse(text, NumberStyles.HexNumber, null, out value);
                case StringStyle.SignedInt:
                    result = int.TryParse(text, out int ivalue);
                    value = unchecked((uint)ivalue);
                    return result;
                case StringStyle.Float:
                    result = float.TryParse(text, out float fvalue);
                    value = BitConverter.ToUInt32(BitConverter.GetBytes(fvalue), 0);
                    return result;
                case StringStyle.Instruction:
                    value = uint.MaxValue;
                    return false;
                case StringStyle.ASCII:
                    try {
                        byte[] ascii = Encoding.ASCII.GetBytes(text);
                        value = BitConverter.ToUInt32(ascii, 0);
                        return true;
                    } catch { value = 0xBAD; return false; }
                default:
                    goto AsHex;
            }
        }

        public static string FormatValue(StringStyle style, uint value, bool reverseBytes = false, int hexsize = 8)
        {
            if (reverseBytes)
                value = superscalar_arch_sim.Utilis.Utilis.ReverseBytes(value);

            switch (GetStyle(style))
            {
                case StringStyle.Hex:
                    return hexsize > 0 ? value.ToString($"X{hexsize}") : value.ToString("X");
                case StringStyle.SignedInt:
                    return unchecked((int)value).ToString();
                case StringStyle.ASCII:
                    return Encoding.ASCII.GetString(BitConverter.GetBytes(value));
                case StringStyle.Float:
                    return BitConverter.ToSingle(BitConverter.GetBytes(value), 0).ToString();
                case StringStyle.Instruction:
                    var asI32 = new superscalar_arch_sim.RV32.ISA.Instructions.Instruction(value);
                    superscalar_arch_sim.RV32.ISA.Decoder.DecodeInstruction(asI32);
                    return superscalar_arch_sim.RV32.ISA.Disassembler.DecodeToHumanReadable(asI32);
                default:
                    return value.ToString("X8");
            }

        }
    }
}
