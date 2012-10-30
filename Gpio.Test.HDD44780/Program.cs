#region References

using System;
using System.Linq;
using System.Net.NetworkInformation;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Gpio.Test.HDD44780
{
    class Program
    {
        static void Main(string[] args)
        {
            var registerSelect = ConnectorPin.P1Pin22.ToProcessor();
            var clock = ConnectorPin.P1Pin18.ToProcessor();
            var data1 = ConnectorPin.P1Pin16.ToProcessor();
            var data2 = ConnectorPin.P1Pin15.ToProcessor();
            var data3 = ConnectorPin.P1Pin13.ToProcessor();
            var data4 = ConnectorPin.P1Pin11.ToProcessor();

            using (var connection = new Hdd44780LcdConnection(
                registerSelect,
                clock,
                data1, data2, data3, data4, 
                20, 2))
            {

                connection.WriteLine("R# IP Config");
                connection.WriteLine(Environment.OSVersion);
                Timer.Sleep(2000);

                const char download = (char) 126;
                const char upload = (char) 127;

                while (true)
                {
                    foreach (var i in NetworkInterface.GetAllNetworkInterfaces().Where(i => i.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                    {
                        connection.Clear();
                        connection.WriteLine("{0}: {1}", i.Name, i.OperationalStatus);
                        connection.Write(i.GetIPProperties().UnicastAddresses.Select(a => a.Address.ToString()).FirstOrDefault() ?? "(unassigned)");

                        Timer.Sleep(2000);
                        
                        connection.Clear();
                        connection.WriteLine(i.GetPhysicalAddress());
                        connection.Write("{0}{1} {2}{3}", download, FormatByteCount(i.GetIPv4Statistics().BytesReceived), upload, FormatByteCount(i.GetIPv4Statistics().BytesSent));

                        Timer.Sleep(2000);

                        if (Console.KeyAvailable)
                            return;
                    }
                }
            }
        }

        private static string FormatByteCount(long byteCount)
        {
            if (byteCount < 1024)
                return string.Format("{0}B", byteCount);
            if (byteCount < 1024 * 1024)
                return string.Format("{0:0.0}KB", byteCount/1024.0m);
            if (byteCount < 1024 * 1024 * 1024)
                return string.Format("{0:0.0}MB", byteCount / (1024 * 1024.0m));

            return string.Format("{0:0.0}GB", byteCount / (1024 * 1024 * 1024.0m));
        }
    }
}
