using superscalar_arch_sim.RV32.Hardware.CPU;
using superscalar_arch_sim.RV32.Hardware.Memory;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Forms
{
    public partial class IOTerminal : Form
    {
        private const int BYTE_TO_RECEIVE_READY_FLAG_BIT = 1;
        private const int BYTE_TO_SEND_READY_FLAG_BIT = 0;

        private const byte BYTE_FROM_CPU_AVALIABLE = 1;
        private const byte CPU_READY_TO_RECEIVE = 0;

        private readonly Memory _ram;
        private readonly BackgroundWorker _inputOutputObserver;
        private readonly CancellationTokenSource _workerCancellationTokenSource;
        private readonly ConcurrentQueue<byte> _inputQueue;

        /// <summary>Address of byte received from CPU -> IOTerminal.</summary>
        private UInt32 OutAddress => (uint)outputByteAddressNumericUpDown.Value;
        /// <summary>Address of byte sent from IOTerminal -> CPU.</summary>
        private UInt32 InAddress => (uint)inputByteAddressNumericUpDown.Value;
        private UInt32 ControlAddress => (uint)controlByteAddressNumericUpDown.Value;

        public IOTerminal(ICPU cpu)
        {
            InitializeComponent();
            
            _ram = cpu.RAM;
            _inputQueue = new ConcurrentQueue<byte>();

            outputByteAddressNumericUpDown.Value = 0x008F_0000;
            inputByteAddressNumericUpDown.Value = 0x008F_0001;
            controlByteAddressNumericUpDown.Value = 0x008F_0002;

            KeyPress += IOTerminal_KeyPress;

            _workerCancellationTokenSource = new CancellationTokenSource();
            _inputOutputObserver = new BackgroundWorker()
            {
                WorkerSupportsCancellation = true
            };
            _inputOutputObserver.DoWork += TermainalDoWork;
            FormClosing += IOTerminal_FormClosing;
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
            _inputQueue.Enqueue((byte)e.KeyChar);
            e.Handled = true;
        }

        private void TermainalDoWork(object sender, DoWorkEventArgs e)
        {
            while (false == _workerCancellationTokenSource.IsCancellationRequested)
            {
                if ((false == _inputQueue.IsEmpty) && IsCpuReadyToReceiveByte() && _inputQueue.TryDequeue(out byte toSend))
                {
                    _ram.WriteByte(InAddress, toSend);
                    SignalByteSent();
                }
                if (IsByteToReceiveAvaliable())
                {
                    char receivedChar = (char)_ram.ReadByte(OutAddress);
                    if (IsAscii(receivedChar))
                    {
                        Invoke(new MethodInvoker(() => terminalTextBox.AppendText(receivedChar.ToString())));
                    }
                    SignalByteReceived();
                }
            }
        }

        private bool IsByteToReceiveAvaliable()
        {
            return IsBitEqual(_ram.ReadByte(ControlAddress), BYTE_TO_RECEIVE_READY_FLAG_BIT, BYTE_FROM_CPU_AVALIABLE);
        }

        private void SignalByteReceived()
        {
            byte controlByte = _ram.ReadByte(ControlAddress);
            controlByte = (byte)(controlByte & ~(1 << BYTE_TO_RECEIVE_READY_FLAG_BIT));
            _ram.WriteByte(ControlAddress, controlByte);
        }  

        private bool IsCpuReadyToReceiveByte() 
        {
            return IsBitEqual(_ram.ReadByte(ControlAddress), BYTE_TO_SEND_READY_FLAG_BIT, CPU_READY_TO_RECEIVE);
        }

        private void SignalByteSent()
        {
            byte controlByte = _ram.ReadByte(ControlAddress);
            controlByte = (byte)(controlByte | (1 << BYTE_TO_SEND_READY_FLAG_BIT));
            _ram.WriteByte(ControlAddress, controlByte);
        }

        private static bool IsBitEqual(byte value, byte position, byte bit)
        {
            return ((value & (1 << position)) >> position) == bit;
        }

        private static bool IsAscii(char c)
        {
            return c < 0x80;
        }
    }
}
