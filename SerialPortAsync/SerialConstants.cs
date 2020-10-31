namespace SerialPortAsync
{
    /// <summary>
    ///     This class contain custom constants for serial communication
    /// </summary>
    public static class SerialConstants
    {
        /// <summary>
        ///     Default break between command and result
        /// </summary>
        public const int WaitTimeout = 100;

        /// <summary>
        ///     The time out read serial.
        /// </summary>
        public const int TimeOutReadSerial = 500;

        /// <summary>
        ///     Default New Line Characters used by SerialPort.NewLine
        /// </summary>
        public const string CarriageReturnNewLine = "\r\n";

        /// <summary>
        ///     Collection of possible Serial Baud Rates
        /// </summary>
        public static readonly int[] BaudRates =
        {
            300,
            600,
            1200,
            2400,
            4800,
            9600,
            19200,
            38400,
            57600,
            115200,
            230400,
            460800,
            921600
        };
    }
}