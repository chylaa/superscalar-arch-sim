using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.UserControls.CustomControls
{
    public partial class TerminalTextBox : TextBox
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        private readonly StringBuilder _buffer = new StringBuilder();
        private int _lastLineCursorPosition = 0;
        private bool _inEscapeSequence = false;

        public bool EnableBuffering { get; set; } = false;

        public TerminalTextBox() : base()
        {
            Multiline = true;
            WordWrap = false;
            ScrollBars = ScrollBars.Both;

            GotFocus += delegate { HideCaret(Handle); };
        }

        /// <summary>
        /// Writes a character at the current virtual cursor position
        /// </summary>
        public void WriteAtCursorPosition(char c)
        {
            if (_inEscapeSequence)
            {
                HandleEnscapeSequenceCharacter(c);
                return;
            }
            switch (c)
            {
                case '\n':
                    AppendBuffered(Environment.NewLine);
                    SetCursorPosition(0);
                    if (EnableBuffering) Flush();
                    break;

                case '\r':
                    SetCursorAtTheBeginingOfLastLine();
                    break;

                case '\b':
                    RemoveLastCharacter();
                    break;

                case '\x1b':
                    _inEscapeSequence = true;
                    break;

                default:
                    SetCharAtCursorPosition(c);
                    break;
            }
        }

        /// <summary>
        /// Forces to flush if anything was buffered when <see cref="EnableBuffering"/> was set to true.
        /// </summary>
        public void Flush()
        {
            if (_buffer.Length > 0)
            {
                AppendText(_buffer.ToString());
                _buffer.Clear();
            }
        }

        private void AppendBuffered(string text)
        {
            if (EnableBuffering)
            {
                _buffer.Append(text);
            }
            else
            {
                AppendText(text);
            }
        }

        private void SetCursorPosition(int pos)
        {
            _lastLineCursorPosition = Math.Max(0, pos);
        }

        private void SetCursorAtTheBeginingOfLastLine()
        {
            int lastLineIndex = GetLineFromCharIndex(TextLength);
            int lastLineStart = GetFirstCharIndexFromLine(lastLineIndex);
            _lastLineCursorPosition = 0;
            SelectionStart = lastLineStart;
        }

        private void SetCharAtCursorPosition(char c)
        {
            if (EnableBuffering)
            {
                _buffer.Append(c);
                _lastLineCursorPosition++;
                return;
            }

            int lastLineIndex = GetLineFromCharIndex(TextLength);
            int lastLineStart = GetFirstCharIndexFromLine(lastLineIndex);

            int insertPos = lastLineStart + _lastLineCursorPosition;

            if (insertPos < TextLength)
            {
                Text = Text.Remove(insertPos, 1).Insert(insertPos, c.ToString());
                SelectionStart = TextLength;
                ScrollToCaret();
            } 
            else
            {
                AppendText(c.ToString());
            }

            _lastLineCursorPosition++;
        }

        private void RemoveLastCharacter()
        {
            if (EnableBuffering)
            {
                if (_buffer.Length > 0)
                {
                    _buffer.Remove(_buffer.Length - 1, 1);
                    _lastLineCursorPosition = Math.Max(0, _lastLineCursorPosition - 1);
                }
            }
            else if (_lastLineCursorPosition > 0)
            {
                int lastLineIndex = GetLineFromCharIndex(TextLength);
                int lastLineStart = GetFirstCharIndexFromLine(lastLineIndex);

                int removePos = lastLineStart + _lastLineCursorPosition - 1;

                if (removePos >= 0 && removePos < TextLength)
                {
                    Text = Text.Remove(removePos, 1);
                    _lastLineCursorPosition--;
                }
            }
        }
    }
}
