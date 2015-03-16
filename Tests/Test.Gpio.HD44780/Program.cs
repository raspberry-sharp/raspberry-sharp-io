#region References

using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Raspberry.IO.Components.Displays.Hd44780;

#endregion

namespace Test.Gpio.HD44780
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HD-44780 Sample: Display IP configuration on LCD screen");

            var settings = new Hd44780LcdConnectionSettings
            {
                ScreenWidth = 20,
                ScreenHeight = 2,
            };

            using (var configuration = ConfigurationLoader.FromArguments(args))
            using (var connection = new Hd44780LcdConnection(settings, configuration.Pins))
            {
                connection.SetCustomCharacter(1, new byte[] {0x0, 0x0, 0x04, 0xe, 0x1f, 0x0, 0x0});
                connection.SetCustomCharacter(2, new byte[] {0x0, 0x0, 0x1f, 0xe, 0x04, 0x0, 0x0});

                if (args.Contains("viewMap", StringComparer.InvariantCultureIgnoreCase))
                {
                    connection.Clear();
                    DisplayCharMap(connection);
                }

                connection.Clear();
                connection.WriteLine("R# IP Config");
                connection.WriteLine(Environment.OSVersion);

                Thread.Sleep(TimeSpan.FromSeconds(2));

                var delay = 0m;
                while (true)
                {
                    foreach (var t in NetworkInterface.GetAllNetworkInterfaces()
                        .Where(i => i.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        .SelectMany(i => new[]
                                             {
                                                 string.Format("{0}: {1}", i.Name, i.OperationalStatus)
                                                 + Environment.NewLine
                                                 + string.Format("\u0002{0} \u0001{1}", FormatByteCount(i.GetIPv4Statistics().BytesReceived), FormatByteCount(i.GetIPv4Statistics().BytesSent)),

                                                 "IP  " + (i.GetIPProperties().UnicastAddresses.Select(a => a.Address.ToString()).FirstOrDefault() ?? "(unassigned)")
                                                 + Environment.NewLine
                                                 + "MAC " + i.GetPhysicalAddress().ToString()
                                             }))
                    {
                        connection.Clear();
                        connection.Write(t, delay);

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

                                    case ConsoleKey.F11:
                                        delay = 50.0m - delay;
                                        break;

                                    default:
                                        connection.BacklightEnabled = false;
                                        return;
                                }
                            }

                            Thread.Sleep(TimeSpan.FromSeconds(2d /20));
                        }
                    }
                }
            }
        }

        #region Private Helpers

        private static void DisplayCharMap(Hd44780LcdConnection connection)
        {
            var idx = 0;
            foreach (var group in Hd44780A00Encoding.SupportedCharacters.GroupBy(c => (idx++/40)))
            {
                var s1 = new string(@group.Take(20).ToArray());
                var s2 = new string(@group.Skip(20).Take(20).ToArray());

                connection.Clear();

                connection.WriteLine(s1);
                connection.WriteLine(s2);

                Thread.Sleep(2000);
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

        #endregion
    }
}
