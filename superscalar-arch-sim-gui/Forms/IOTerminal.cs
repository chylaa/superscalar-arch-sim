using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Units;
using superscalar_arch_sim_gui.UserControls.CustomControls;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Forms
{
    public partial class IOTerminal : Form
    {
        /// <summary>
        /// IO_RX_BUFFER: '0' we fetched character from IO_RX_BUFFER, set to '1' by CPU upon sending us a character.<br></br>
        /// IO_TX_BUFFER: '1' we put character in IO_TX_BUFFER, set to '0' by CPU as ackowledge of receiving (and ready for more)
        /// .</summary>
        private const int IO_CONTROL_BIT_POSITION = 7;
        /// <summary>Masks the 7 bits of a character in RX/TX buffer.</summary>
        private const int IO_CHARACTER_MASK = 0b0111_1111;

        private readonly MemoryManagmentUnit _mmu;
        private readonly BackgroundWorker _inputOutputObserver;
        private readonly CancellationTokenSource _workerCancellationTokenSource;
        private readonly ConcurrentQueue<byte> _inputQueue;

        /// <summary>Address of byte received from CPU -> IOTerminal.</summary>
        private UInt32 RxBufferAddress => (uint)rxByteAddressNumericUpDown.Value;
        /// <summary>Address of byte sent from IOTerminal -> CPU.</summary>
        private UInt32 TxBufferAddress => (uint)txByteAddressNumericUpDown.Value;

        public IOTerminal(ICPU cpu)
        {
            InitializeComponent();
            
            _mmu = cpu.MMU;
            _inputQueue = new ConcurrentQueue<byte>();

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
                terminalTextBox.EnableBuffering = enableBufferingCheckBox.Checked;
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
            if (terminalTextBox.EnableBuffering && _inputQueue.Count == 0)
            {
                terminalTextBox.Flush();
                terminalTextBox.EnableBuffering = false;
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
                    PutCharacterAndSignalSent(toSend);
                }
                if (IsByteToReceiveAvaliable())
                {
                    char receivedChar = GetCharacter();
                    SignalByteReceived();
                    Invoke((KeyPressEventHandler)OnCharacterReceived, terminalTextBox, new KeyPressEventArgs(receivedChar));
                }
            }
        }

        private void OnCharacterReceived(object sender, KeyPressEventArgs e)
        {
            if (sender is TerminalTextBox textBox)
            {
                char c = e.KeyChar;
                textBox.WriteAtCursorPosition(c);
            }
        }

        private bool IsByteToReceiveAvaliable()
        {
            byte control = _mmu.ReadByte(RxBufferAddress);
            return IsBitSet(control, IO_CONTROL_BIT_POSITION);
        }

        private char GetCharacter()
        {
            return (char)(_mmu.ReadByte(RxBufferAddress) & IO_CHARACTER_MASK);
        }

        private void SignalByteReceived()
        {
            byte rxByte = _mmu.ReadByte(RxBufferAddress);
            ResetBit(ref rxByte, IO_CONTROL_BIT_POSITION);
            _mmu.WriteByte(RxBufferAddress, rxByte);
        }  

        private bool IsCpuReadyToReceiveByte() 
        {
            byte control = _mmu.ReadByte(TxBufferAddress);
            return false == IsBitSet(control, IO_CONTROL_BIT_POSITION);
        }

        private void PutCharacterAndSignalSent(byte c)
        {
            byte txByte = 0;
            SetBit(ref txByte, IO_CONTROL_BIT_POSITION);
            txByte |= (byte)(c & IO_CHARACTER_MASK);
            _mmu.WriteByte(TxBufferAddress, txByte);
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

        private static void SetTextToKeyString(Label label, char keyChar)
        {
            label.Text = char.IsControl(keyChar) ? $"{{{(Keys)keyChar}}}" : keyChar.ToString();
        }
    }
}
