using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Raspberry.IO.GeneralPurpose
{
    public class Mainboard
    {
        private static readonly Lazy<Mainboard> board = new Lazy<Mainboard>(LoadBoard);

        private readonly Dictionary<string, string> settings;

        private Mainboard(Dictionary<string, string> settings)
        {
            this.settings = settings;
        }

        public static Mainboard Current
        {
            get { return board.Value; }
        }

        public string Processor
        {
            get { return settings["Hardware"]; }
        }

        public string SerialNumber
        {
            get { return settings["Serial"]; }
        }

        public int FirmwareRevision
        {
            get { return int.Parse(settings["Revision"]); }
        }

        public int BoardRevision
        {
            get
            {
                var firmware = FirmwareRevision;
                if (firmware <= 3)
                    return 1;
                else if (firmware <= 6)
                    return 2;
                else
                    throw new NotSupportedException("Raspberry board not supported");
            }
        }

        private static Mainboard LoadBoard()
        {
            const string filePath = "/proc/cpuinfo";
            var settings = File.ReadAllLines(filePath)
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(l =>
                            {
                                var separator = l.IndexOf(':');
                                if (separator < 0)
                                    return new KeyValuePair<string, string>(l, null);
                                else
                                    return new KeyValuePair<string, string>(l.Substring(0, separator).Trim(), l.Substring(separator + 1).Trim());
                            })
                .ToDictionary(p => p.Key, p => p.Value);

            return new Mainboard(settings);
        }
    }
}