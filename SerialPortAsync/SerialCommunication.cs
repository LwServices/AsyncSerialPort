using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace SerialPortAsync
{
    /// <summary>
    ///     Base Class to communicate to device based beko devices
    /// </summary>
    public class SerialCommunication
    {
        /// <summary>
        ///     Baud Rate
        /// </summary>
        private readonly int _baudRate;

        /// <summary>
        ///     Port Name
        /// </summary>
        private readonly string _portName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SerialCommunication" /> class.
        ///     Constructor: Set Default Port name and BaudRate
        /// </summary>
        /// <param name="portName">Serial Port Name (e.g. COM3)</param>
        /// <param name="baudRate">Serial Port Speed (9600,57600,...)</param>
        protected SerialCommunication(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate;
        }

        /// <summary>
        ///     Get all active Ports on current machine
        /// </summary>
        /// <returns>collection of port names</returns>
        public static IEnumerable<string> GetAllPorts()
        {
            return SerialPort.GetPortNames().ToList();
        }

        /// <summary>
        ///     Send Command to Device via Serial Port Calls Command Methods with default Param
        /// </summary>
        /// <param name="command">command char</param>
        /// <returns>Result from Device</returns>
        protected string Command(string command)
        {
            if (!string.IsNullOrWhiteSpace(_portName) && _baudRate != 0)
                return Command(command, _portName, _baudRate);

            //Logs.System.Error("Serial Port/Speed are not configured");
            return string.Empty;
        }

        /// <summary>
        ///     Send Command to device via Serial Port
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="portName">Serial Port Name</param>
        /// <param name="baudRate">Serial Port Speed</param>
        /// <returns>Result from Device</returns>
        private string Command(string command, string portName, int baudRate)
        {
            using (var port = new SerialPort(portName, baudRate))
            {
                if (!port.OpenEx()) return string.Empty;
                port.WriteLine(command);
                Thread.Sleep(SerialConstants.WaitTimeout);
                try
                {
                    port.ReadTimeout = SerialConstants.TimeOutReadSerial;
                    return port.ReadLine();
                }
                catch (Exception)
                {
                    //Logs.System.Error("No response from serial device");
                    return string.Empty;
                }
            }
        }
    }
}