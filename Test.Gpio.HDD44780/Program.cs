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
            var settings = new Hd44780LcdConnectionSettings
                               {
                                   ScreenWidth = 20,
                                   ScreenHeight = 2,
                               };

            using (var connection = new Hd44780LcdConnection(settings,
                ConnectorPin.P1Pin22.ToProcessor(),

                ConnectorPin.P1Pin18.ToProcessor(),

                ConnectorPin.P1Pin16.ToProcessor(),
                ConnectorPin.P1Pin15.ToProcessor(),
                ConnectorPin.P1Pin13.ToProcessor(),
                ConnectorPin.P1Pin11.ToProcessor()))
            {
                connection.SetCustomCharacter(1, new byte[] {0x0, 0x0, 0x04, 0xe, 0x1f, 0x0, 0x0});
                connection.SetCustomCharacter(2, new byte[] {0x0, 0x0, 0x1f, 0xe, 0x04, 0x0, 0x0});

                connection.Clear();

                connection.WriteLine("R# IP Config", 50.0m);
                connection.WriteLine(Environment.OSVersion, 50.0m);
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
                        connection.Write(t, 50.0m);

                        for (var i = 0; i < 20; i++)
                        {
                            if (Console.KeyAvailable)
                            {
                                var c = Console.ReadKey(true).Key;

                                switch (c)
                                {
                                    case ConsoleKey.F5:
                                        connection.Clear();
                                        break;

                                    case ConsoleKey.F6:
                                        connection.CursorBlinking = !connection.CursorBlinking;
                                        break;

                                    case ConsoleKey.F7:
                                        connection.CursorEnabled = !connection.CursorEnabled;
                                        break;

                                    case ConsoleKey.F8:
                                        connection.DisplayEnabled = !connection.DisplayEnabled;
                                        break;

                                    case ConsoleKey.F9:
                                        connection.Move(-1);
                                        break;

                                    case ConsoleKey.F10:
                                        connection.Move(1);
                                        break;

                                    default:
                                        return;
                                }
                            }

                            Timer.Sleep(100);
                        }
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
