#region References

using System;
using System.Linq;
using System.Net.NetworkInformation;
using Raspberry.IO.GeneralPurpose;
using Raspberry.Timers;

#endregion

namespace Test.Gpio.HD44780
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

            using (var connection = new HD44780LcdConnection(
                registerSelect,
                clock,
                data1, data2, data3, data4, 
                20, 2))
            {
                connection.SetCustomCharacter(1, new byte[] {0x0, 0x0, 0x04, 0xe, 0x1f, 0x0, 0x0});
                connection.SetCustomCharacter(2, new byte[] {0x0, 0x0, 0x1f, 0xe, 0x04, 0x0, 0x0});

                connection.Clear();

                connection.WriteLine("R# IP Config");
                connection.WriteLine(Environment.OSVersion);
                Timer.Sleep(2000);

                while (true)
                {
                    foreach (var t in NetworkInterface.GetAllNetworkInterfaces()
                        .Where(i => i.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        .SelectMany(i => new[]
                                             {
                                                 string.Format("{0}: {1}", i.Name, i.OperationalStatus)
                                                 + Environment.NewLine
                                                 + (i.GetIPProperties().UnicastAddresses.Select(a => a.Address.ToString()).FirstOrDefault() ?? "(unassigned)"),

                                                 i.GetPhysicalAddress().ToString()
                                                 + Environment.NewLine
                                                 + string.Format("\u0001{0} \u0002{1}", FormatByteCount(i.GetIPv4Statistics().BytesReceived), FormatByteCount(i.GetIPv4Statistics().BytesSent))
                                             }))
                    {
                        connection.Clear();
                        connection.Write(t);

                        if (Console.KeyAvailable)
                        {
                            var c = Console.ReadKey(true).KeyChar;

                            switch (c)
                            {
                                case 'e':
                                    connection.Clear();
                                    break;

                                case 'b':
                                    connection.BlinkEnabled = !connection.BlinkEnabled;
                                    break;

                                case 'c':
                                    connection.CursorEnabled = !connection.CursorEnabled;
                                    break;

                                case 'd':
                                    connection.DisplayEnabled = !connection.DisplayEnabled;
                                    break;

                                default:
                                    return;
                            }
                        }

                        Timer.Sleep(2000);
                    }
                }
            }
        }

        private static string FormatByteCount(long byteCount)
        {
            if (byteCount < 1024)
                return string.Format("{0}B", byteCount);
            if (byteCount < 1024*1024)
                return string.Format("{0:0.0}KB", byteCount/1024.0m);
            if (byteCount < 1024*1024*1024)
                return string.Format("{0:0.0}MB", byteCount/(1024*1024.0m));

            return string.Format("{0:0.0}GB", byteCount/(1024*1024*1024.0m));
        }
    }
}
