using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace SerialPortAsync
{
    /// <summary>
    ///     Collection of Methods to handle Serial Port
    /// </summary>
    public static class SerialDevices
    {
        public static List<string> ComPortNames(string VID, string PID)
        {
            var rk1 = Registry.LocalMachine;
            var rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            var pattern = string.Format("^VID_{0}.PID_{1}", VID, PID);
            var _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            var ports = new List<string>();

            foreach (var s3 in rk2.GetSubKeyNames())
            {
                var rk3 = rk2.OpenSubKey(s3);
                foreach (var s in rk3.GetSubKeyNames())
                    if (_rx.Match(s).Success)
                    {
                        var rk4 = rk3.OpenSubKey(s);
                        foreach (var s2 in rk4.GetSubKeyNames())
                        {
                            var rk5 = rk4.OpenSubKey(s2);
                            var rk6 = rk5.OpenSubKey("Device Parameters");
                            ports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
            }

            return ports;
        }

        /// <summary>
        ///     Extension Method Open and Configure Serial Port
        /// </summary>
        /// <param name="port">port name</param>
        /// <returns>this</returns>
        public static bool OpenEx(this SerialPort port)
        {
            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                //Logs.System.Error($"Serial Port {port.PortName} Exception:" + e.Message);
                return false;
            }

            if (!port.IsOpen) return false;
            port.DiscardInBuffer();
            port.DiscardOutBuffer();
            port.NewLine = SerialConstants.CarriageReturnNewLine;
            return true;
        }
    }
}