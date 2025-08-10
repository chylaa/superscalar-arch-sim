using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Units;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Forms
{
    public partial class IOTerminal : Form
    {
        /// <summary>'0' we fetched character from IO_RX_BUFFER, set to '1' by CPU upon sending us a character.</summary>
        private const int IO_CONTROL_RX_BIT = 1;
        /// <summary>'1' we put character in IO_TX_BUFFER, set to '0' by CPU as ackowledge of receiving (and ready for more).</summary>
        private const int IO_CONTROL_TX_BIT = 0;

        private readonly MemoryManagmentUnit _mmu;
        private readonly BackgroundWorker _inputOutputObserver;
        private readonly CancellationTokenSource _workerCancellationTokenSource;
        private readonly ConcurrentQueue<byte> _inputQueue;

        /// <summary>Address of byte received from CPU -> IOTerminal.</summary>
        private UInt32 RxBufferAddress => (uint)rxByteAddressNumericUpDown.Value;
        /// <summary>Address of byte sent from IOTerminal -> CPU.</summary>
        private UInt32 TxBufferAddress => (uint)txByteAddressNumericUpDown.Value;
        private UInt32 ControlAddress => (uint)controlByteAddressNumericUpDown.Value;

        public IOTerminal(ICPU cpu)
        {
            InitializeComponent();
            
            _mmu = cpu.MMU;
            _inputQueue = new ConcurrentQueue<byte>();

            controlByteAddressNumericUpDown.Value = 0x008F_0000;
            txByteAddressNumericUpDown.Value = 0x008F_0001;
            rxByteAddressNumericUpDown.Value = 0x008F_0002;

            KeyPress += IOTerminal_KeyPress;

            _workerCancellationTokenSource = new CancellationTokenSource();
            _inputOutputObserver = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            _inputOutputObserver.DoWork += TermainalDoWork;
            FormClosing += IOTerminal_FormClosing;
            Shown += IOTerminal_Shown;
        }

        private void IOTerminal_Shown(object sender, EventArgs e)
        {
            _inputOutputObserver.RunWorkerAsync();
        }

        private void IOTerminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            _workerCancellationTokenSource.Cancel();
            SpinWait.SpinUntil(() => _inputOutputObserver.IsBusy, 100);
            _inputOutputObserver.Dispose();
            _workerCancellationTokenSource.Dispose();
        }

        private void IOTerminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            const char CTRL_V = '\x16';  
            if (e.KeyChar == CTRL_V && Clipboard.ContainsText())
            {
                string text = Clipboard.GetText().Replace(Environment.NewLine, "\n");
                foreach (char c in text) _inputQueue.Enqueue((byte)c);
            } 
            else
            {
                char c = e.KeyChar;
                if ((Keys)c == Keys.Enter) c = '\n';
                SetTextToKeyString(txKeyViewLabel, c);
                _inputQueue.Enqueue((byte)c);
            }
            e.Handled = true;
        }

        private void TermainalDoWork(object sender, DoWorkEventArgs e)
        {
            while (false == _workerCancellationTokenSource.IsCancellationRequested)
            {
                bool byteToSentAvaliable = (false == _inputQueue.IsEmpty);
                bool cpuReady = IsCpuReadyToReceiveByte();
                if (byteToSentAvaliable && cpuReady && _inputQueue.TryDequeue(out byte toSend))
                {
                    _mmu.WriteByte(TxBufferAddress, toSend);
                    SignalByteSent();
                }
                if (IsByteToReceiveAvaliable())
                {
                    char receivedChar = (char)_mmu.ReadByte(RxBufferAddress);
                    if (IsAscii(receivedChar))
                    {
                        Invoke((KeyPressEventHandler)OnCharacterReceived, terminalTextBox, new KeyPressEventArgs(receivedChar));
                    }
                    SignalByteReceived();
                }
            }
        }

        private void OnCharacterReceived(object sender, KeyPressEventArgs e)
        {
            if (false == (sender is TextBox textBox))
                return;

            SetTextToKeyString(rxKeyViewLabel, e.KeyChar);
            switch (e.KeyChar)
            {
                case '\n': // NEWLINE
                    textBox.AppendText(Environment.NewLine);
                    break;

                case '\r':
                    SetCursorAtTheBegining(textBox);
                    break;

                case  '\b': // BACKSPACE
                    if (textBox.TextLength > 0)
                    {
                        textBox.Text = textBox.Text.Remove(textBox.TextLength - 1);
                        SetCursorAtTheEnd(textBox);
                    }
                    break;

                default:
                    textBox.AppendText(e.KeyChar.ToString());
                    break;
            }
        }

        private bool IsByteToReceiveAvaliable()
        {
            byte control = _mmu.ReadByte(ControlAddress);
            return IsBitSet(control, IO_CONTROL_RX_BIT);
        }

        private void SignalByteReceived()
        {
            byte controlByte = _mmu.ReadByte(ControlAddress);
            ResetBit(ref controlByte, IO_CONTROL_RX_BIT);
            _mmu.WriteByte(ControlAddress, controlByte);
        }  

        private bool IsCpuReadyToReceiveByte() 
        {
            byte control = _mmu.ReadByte(ControlAddress);
            return false == IsBitSet(control, IO_CONTROL_TX_BIT);
        }

        private void SignalByteSent()
        {
            byte controlByte = _mmu.ReadByte(ControlAddress);
            SetBit(ref controlByte, IO_CONTROL_TX_BIT);
            _mmu.WriteByte(ControlAddress, controlByte);
        }

        private static bool IsBitSet(byte value, byte position)
        {
            return ((value & (1 << position)) >> position) == 1;
        }

        private static void SetBit(ref byte value, byte position)
        {
            value |= (byte)(1 << position);
        }

        private static void ResetBit(ref byte value, byte position)
        {
            value &= (byte)(~(1 << position));
        }

        private static bool IsAscii(char c)
        {
            return c < 0x80;
        }

        private static void SetCursorAtTheEnd(TextBox textBox)
        {
            textBox.SelectionStart = textBox.TextLength;
            textBox.SelectionLength = 0;
        }

        private static void SetCursorAtTheBegining(TextBox textBox)
        {
            textBox.SelectionStart = textBox.SelectionLength = 0;
        }

        private static void SetTextToKeyString(Label label, char keyChar)
        {
            label.Text = char.IsControl(keyChar) ? $"{{{(Keys)keyChar}}}" : keyChar.ToString();
        }
    }
}
