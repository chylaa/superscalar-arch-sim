using System;
using System.Text;

namespace superscalar_arch_sim_gui.UserControls.CustomControls
{
    // TODO: Logic of terminal should be handled in separete class in smth like '.Devices' project
    //          the representation as text box should use the interface of this device to display its state
    public partial class TerminalTextBox
    {
        private static class EscapeSequences
        {
            public const char EscapeChar = '\x1b';
            public const char EscapeSequenceStartChar = '[';
            
            // \x1b[#X where # is a numeric argument and X is a CursorControls char
            public static class CursorControls
            {
                public const string MoveCursorHome = "[H"; // moves cursor (0,0)
                public static class Numeric
                {
                    public const char MoveCursorUp_Lines = 'A';      // moves cursor up # lines
                    public const char MoveCursorDown_Lines = 'B';    // moves cursor down # lines
                    public const char MoveCursorRight_Columns = 'C'; //	moves cursor right # columns
                    public const char MoveCursorLeft_Columns = 'D';  // moves cursor left # columns
                    public const char MoveCursorToColumn = 'G';      // moves cursor to column #
                }
            }

            public static class EraseFunctions
            {
                public const string EraseFromCursorToEndOfScreen = "[J";
                public const string EraseFromCursorToEndOfScreenAlt = "[0J";
                public const string EraseFromBeginingOfScreenToCursor = "[1J"; 
                public const string EraseEntireScreen = "[2J";
                public const string EraseFromCursorToEndOfLine = "[K";
                public const string EraseFromCursorToEndOfLineAlt = "[0K";
                public const string EraseFromStartOfLineToTheCursor = "[1K";
                public const string EraseEntireLine = "[2K";
            }

            public static class PrivateModes
            {
                public const string RestoreScreen = "[?47l";  
                public const string SaveScreen = "[?47h";  
                public const string EnablesTheAlternativeBuffer = "[?1049h"; // enables buffering
                public const string DisableTheAlternativeBuffer = "[?1049l"; // disables buffering and flushes buffer
            }
        }

        private string _altScreenBuffer = null;
        private readonly StringBuilder _escapeBuffer = new StringBuilder();

        private void HandleEnscapeSequenceCharacter(char c)
        {
            _escapeBuffer.Append(c);

            // Detect end of escape sequence (by spec, final byte is in range 0x40–0x7E)
            if (c >= 0x40 && c <= 0x7E)
            {
                string seq = _escapeBuffer.ToString();
                _escapeBuffer.Clear();
                _inEscapeSequence = false;

                if (seq.Length > 1 && seq[0] == EscapeSequences.EscapeSequenceStartChar)
                {
                    HandleFullEscapeSequence(seq);
                }
            }
        }

        private void HandleFullEscapeSequence(string seq)
        {
            TryHandleAsCursorControl(seq);
            TryHandleAsEraseFunction(seq);
            TryHandleAsPrivateMode(seq);
        }

        private void TryHandleAsCursorControl(string seq)
        {
            if (seq == EscapeSequences.CursorControls.MoveCursorHome)
            {
                SetCursorPosition(0);
                return;
            }
            int arg = ExtractNumericArgument(seq, 1);
            char type = seq[seq.Length - 1];
            switch (type)
            {
                case EscapeSequences.CursorControls.Numeric.MoveCursorUp_Lines:
                    MoveCursorVertical(-arg);
                    return;

                case EscapeSequences.CursorControls.Numeric.MoveCursorDown_Lines:
                    MoveCursorVertical(arg);
                    return;

                case EscapeSequences.CursorControls.Numeric.MoveCursorRight_Columns:
                    MoveCursorHorizontal(arg);
                    return;

                case EscapeSequences.CursorControls.Numeric.MoveCursorLeft_Columns:
                    MoveCursorHorizontal(-arg);
                    return;

                case EscapeSequences.CursorControls.Numeric.MoveCursorToColumn:
                    SetCursorPosition(arg);
                    return;

                default:
                    return;
            }
        }

        private void TryHandleAsEraseFunction(string seq)
        {
            if (seq == EscapeSequences.EraseFunctions.EraseEntireLine ||
                seq == EscapeSequences.EraseFunctions.EraseFromCursorToEndOfLine ||
                seq == EscapeSequences.EraseFunctions.EraseFromCursorToEndOfLineAlt)
            {
                EraseToEndOfLine();
                return;
            }

            if (seq == EscapeSequences.EraseFunctions.EraseFromStartOfLineToTheCursor)
            {
                EraseFromStartToCursor();
                return;
            }

            if (seq == EscapeSequences.EraseFunctions.EraseEntireScreen)
            {
                Clear();
                SetCursorPosition(0);
                return;
            }
        }

        private void TryHandleAsPrivateMode(string seq)
        {
            if (seq == EscapeSequences.PrivateModes.EnablesTheAlternativeBuffer)
            {
                EnableBuffering = true;
                return;
            }
            if (seq == EscapeSequences.PrivateModes.DisableTheAlternativeBuffer)
            {
                EnableBuffering = false;
                Flush();
                return;
            }
            if (seq == EscapeSequences.PrivateModes.SaveScreen)
            {
                _altScreenBuffer = Text;
                return;
            }
            if (seq == EscapeSequences.PrivateModes.RestoreScreen)
            {
                Text = _altScreenBuffer;
                return;
            }
        }

        private int ExtractNumericArgument(string seq, int defaultValue)
        {
            int start = seq.IndexOf('[') + 1;
            int end = seq.Length - 1;
            string number = seq.Substring(start, end - start);
            if (int.TryParse(number, out int n))
                return n;

            return defaultValue;
        }

        private void MoveCursorHorizontal(int offset)
        {
            _lastLineCursorPosition = Math.Max(0, _lastLineCursorPosition + offset);
        }

        private void MoveCursorVertical(int offset)
        {
            int currentLine = GetLineFromCharIndex(TextLength);
            int newLine = Math.Max(0, currentLine + offset);
            int newLineStart = GetFirstCharIndexFromLine(newLine);
            _lastLineCursorPosition = 0;
            SelectionStart = newLineStart;
        }

        private void EraseToEndOfLine()
        {
            int lastLineIndex = GetLineFromCharIndex(TextLength);
            int lastLineStart = GetFirstCharIndexFromLine(lastLineIndex);
            int lineLen = Lines[lastLineIndex].Length;
            int cursorPos = lastLineStart + _lastLineCursorPosition;

            if (cursorPos < TextLength)
            {
                Text = Text.Remove(cursorPos, lineLen - _lastLineCursorPosition);
            }
        }

        private void EraseFromStartToCursor()
        {
            int lastLineIndex = GetLineFromCharIndex(TextLength);
            int lastLineStart = GetFirstCharIndexFromLine(lastLineIndex);
            int cursorPos = lastLineStart + _lastLineCursorPosition;

            if (cursorPos > lastLineStart)
            {
                Text = Text.Remove(lastLineStart, _lastLineCursorPosition);
                _lastLineCursorPosition = 0;
            }
        }
    }
}
