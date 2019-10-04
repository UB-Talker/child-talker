using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using Child_Talker.Utilities.Autoscan;
using Timer = System.Timers.Timer;

namespace Child_Talker.Utilities.HardwareIntegrations
{
    /// <summary>
    /// provides the code to integrate autoscan with Arduino
    /// This should be updated away from Serial Communication in the future
    /// </summary>
    public class ArduinoIntegration
    {
        public static ArduinoIntegration _instance;
        public static ArduinoIntegration Instance => _instance ?? (_instance = new ArduinoIntegration());


        private static Autoscan2 scan = Autoscan2.Instance;
        /// <summary>
        /// true if a device is connected
        /// </summary>
        public static bool DeviceConnected = false;
        /// <summary>
        /// handshake code required from attached device
        /// </summary>
        private const string HandshakeCode = "1997";

        private static SerialPort _attachedPort;
        /// <summary>
        /// Information on The connected Device
        /// </summary>
        public static SerialPort AttachedPort
        {
            get => _attachedPort;
            private set
            {
                _attachedPort?.Close();
                _attachedPort = value;
                _attachedPort.WriteLine("Z");
                _attachedPort.DataReceived += DataReceived;
                Handshake.Join();
                Console.WriteLine("\n\n Arduino Connected on "+_attachedPort.PortName+"\n");
                DeviceConnected = true;
            }
        }

        private static readonly Thread Handshake = new Thread(HandshakeProtocol);

        private static readonly Thread Checkup = new Thread(() =>
        {   
            if(!DeviceConnected) return;
            Timer t = new Timer(550) {AutoReset = true};
            t.Elapsed += (s, e) =>
            {
                try
                {
                    AttachedPort.WriteLine("Z");
                }
                catch (TimeoutException timeoutException)
                {
                    DeviceConnected = false;
                    Handshake.Start();
                }

                t.Start();
            };
        });

        /// <summary>
        /// provides the code to integrate autoscan with Arduino
        /// This should be updated away from Serial Communication in the future
        /// </summary>
        static ArduinoIntegration()
        {
            string lastPort = Properties.HardareIntegration.Default.Active_COM_Port;
            Handshake.Start();
        }


        private static void HandshakeProtocol()
        {
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");
            // Display each port name to the console.
            
            foreach (string port in ports)
            {
                try
                {
                    Console.WriteLine(port);
                    SerialPort testPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One)
                    {
                        ReadTimeout = 500,
                        WriteTimeout = 500
                    };

                    testPort.Open();
                    for (var i = 0; i < 3; i++)
                    {
                        // checked repeatedly in case there was an issue in the serial communication
                        if (HandshakeCode != testPort.ReadLine()) continue;
                        AttachedPort = testPort;
                        break;
                    }
                    if(DeviceConnected) break;
                    testPort.Close();
                }
                catch (TimeoutException e)
                {
                    continue;
                }
            }


        }
        
        
        /// keeps hold event from happening repeatedly
        private static bool reverseIsHeld = false;

        /// keeps hold event from happening repeatedly
        private static bool selectIsHeld = false;

        /// keeps hold event from happening repeatedly
        private static bool goBackIsHeld = false;

        private static void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (scan.TimerMode == Autoscan2.TimerModes.Off) return;
            // Show all the incoming data in the port's buffer
            string command = _attachedPort.ReadLine();
            Console.WriteLine(command);
            scan.ActiveWindow.Dispatcher.Invoke(() =>
            {
                switch (command)
                {
                    case "R_DOWN" when !reverseIsHeld: //Reverse
                        reverseIsHeld = true;
                        scan.ReverseHoldIntegration();
                        break;
                    case "S_DOWN" when !selectIsHeld: //Select
                        selectIsHeld = true;
                        scan.SelectHoldIntegration();
                        break;
                    case "G_DOWN" when !goBackIsHeld: //GoBack
                        goBackIsHeld = true;
                        scan.GoBackHoldIntegration();
                        break;
                    case "R_UP": //Reverse
                        scan.ReversePressIntegration();
                        reverseIsHeld = false;
                        break;
                    case "S_UP": //Select
                        scan.SelectPressIntegration();
                        selectIsHeld = false;
                        break;
                    case "G_UP": //GoBack
                        scan.GoBackPressIntegration();
                        goBackIsHeld = false;
                        break;
                }
            });

        }

    }
}
