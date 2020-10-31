using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace SerialPortAsync
{
    /// <inheritdoc cref="IObservable{T}" />
    /// <summary>
    ///     Observable Serial Port
    /// </summary>
    public class BufferedSerialPort : IDisposable
    {
        private const int ReadTimeout = 500;
        private const int WriteTimeout = 500;

        /// <summary>
        ///     Data Queue
        /// </summary>
        private readonly Queue<string> _queue = new Queue<string>();

        /// <summary>
        ///     Nested Serial Port
        /// </summary>
        private readonly SerialPort _serialPort;

        /// <summary>
        ///     Buffer
        /// </summary>
        private string _rawBuffer;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="portName">Port Name</param>
        /// <param name="baudRate">Baud Rate</param>
        /// <param name="parity">Parity</param>
        /// <param name="dataBits">Data Bits</param>
        /// <param name="stopBits">Stop Bits</param>
        /// <param name="separator">Separator Char</param>
        /// <param name="ignore">Ignored Char</param>
        public BufferedSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits,
            char separator, string ignore)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

            //            _serialPort.Handshake = Handshake.XOnXOff;
            //            _serialPort.RtsEnable = true;
            //_serialPort.CDHolding
            //_serialPort.NewLine = SerialConstants.CarriageReturnNewLine;
            //_serialPort.DtrEnable = true;

            Separator = separator;
            Ignore = ignore.ToCharArray();
        }

        public BufferedSerialPort(string portName, int baudRate, char separator, string ignore) : this(portName, baudRate, Parity.None, 8, StopBits.One, separator, ignore)
        {
        }

        public int Count => _queue.Count;

        /// <summary>
        ///     Ignore Char
        /// </summary>
        public char[] Ignore { get; set; }

        /// <summary>
        ///     Separator char
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            if (_serialPort != null) _serialPort.DataReceived -= SerialPortOnDataReceived;

            _serialPort?.Close();
            _serialPort?.Dispose();
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public string Dequeue()
        {
            return _queue.Dequeue();
        }

        /// <summary>
        ///     Subscribe
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public bool Init()
        {
            try
            {
                _serialPort.ReadTimeout = ReadTimeout;
                _serialPort.Open();
                _serialPort.DataReceived += SerialPortOnDataReceived;
                _serialPort.ErrorReceived += _serialPort_ErrorReceived;
                _serialPort.PinChanged += _serialPort_PinChanged;
                _serialPort.Disposed += _serialPort_Disposed;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return false;
            }

            return true;
        }

        private void _serialPort_Disposed(object sender, EventArgs e)
        {
            Console.WriteLine("Serial Port Disposed");

            var error = new Exception("Serial Port ist weg :)");
            //this.Dispose();
        }

        private void _serialPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Console.WriteLine(e.EventType);
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine(e.EventType);
        }

        /// <summary>
        ///     Serial Port Event on Data Received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort.IsOpen && e.EventType == SerialData.Chars)
            {
                var existing = _rawBuffer + ((SerialPort) sender).ReadExisting();
                existing = existing.Replace(Ignore.ToString(), "");
                var split = existing.Split(Separator);

                split.ToList().GetRange(0, split.Length - 1).ForEach(p =>
                {
                    if (p.Length > 0) _queue.Enqueue(p);
                });

                _rawBuffer = split[split.Length - 1] == string.Empty ? string.Empty : split[split.Length - 1];

                _serialPort.Write("\n");
            }
        }

        public event EventHandler DataRecieved;

        protected virtual void OnDataRecieved()
        {
            var eventArgs = new EventArgs();

            DataRecieved?.Invoke(this, EventArgs.Empty);
        }

        public void Close()
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}