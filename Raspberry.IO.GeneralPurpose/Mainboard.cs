#region References

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the Raspberry Pi mainboard.
    /// </summary>
    public class Mainboard
    {
        #region Fields

        private static readonly Lazy<Mainboard> board = new Lazy<Mainboard>(LoadBoard);
        private readonly Dictionary<string, string> settings;

        private const string raspberryPiProcessor = "BCM2708";
        
        #endregion

        #region Instance Management

        private Mainboard(Dictionary<string, string> settings)
        {
            this.settings = settings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current mainboard configuration.
        /// </summary>
        public static Mainboard Current
        {
            get { return board.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a Raspberry Pi.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a Raspberry Pi; otherwise, <c>false</c>.
        /// </value>
        public bool IsRaspberryPi
        {
            get { return Processor == raspberryPiProcessor; }
        }

        /// <summary>
        /// Gets the processor.
        /// </summary>
        public string Processor
        {
            get { return settings["Hardware"]; }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        public string SerialNumber
        {
            get { return settings["Serial"]; }
        }

        /// <summary>
        /// Gets the firmware revision.
        /// </summary>
        public int FirmwareRevision
        {
            get { return int.Parse(settings["Revision"]); }
        }

        /// <summary>
        /// Gets the board revision.
        /// </summary>
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

        #endregion

        #region Private Helpers

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

        #endregion
    }
}