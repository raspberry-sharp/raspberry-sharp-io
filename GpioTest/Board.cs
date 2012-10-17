#region References

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace GpioTest
{
    /// <summary>
    /// Represents the Raspberry Pi mainboard.
    /// </summary>
    internal class Board
    {
        #region Fields

        private static readonly Lazy<Board> board = new Lazy<Board>(LoadBoard);
        private readonly Dictionary<string, string> settings;

        private const string raspberryPiProcessor = "BCM2708";
        
        #endregion

        #region Instance Management

        private Board(Dictionary<string, string> settings)
        {
            this.settings = settings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current mainboard configuration.
        /// </summary>
        public static Board Current
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
            get
            {
                string hardware;
                return settings.TryGetValue("Hardware", out hardware) ? hardware : null;
            }
        }

        /// <summary>
        /// Gets the board firmware version.
        /// </summary>
        public int Firmware
        {
            get
            {
                string revision;
                int firmware;
                if (settings.TryGetValue("Revision", out revision) && !string.IsNullOrEmpty(revision) && int.TryParse(revision, out firmware))
                    return firmware;

                return 0;
            }
        }

        /// <summary>
        /// Gets the board revision.
        /// </summary>
        public int Revision
        {
            get
            {
                var firmware = Firmware;
                switch (firmware)
                {
                    case 2:
                    case 3:
                        return 1;
                    case 4:
                    case 5:
                    case 6:
                        return 2;

                    default:
                        return 0;
                }
            }
        }

        #endregion

        #region Private Helpers

        private static Board LoadBoard()
        {
            try
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

                return new Board(settings);
            }
            catch
            {
                return new Board(new Dictionary<string, string>());
            }
        }

        #endregion
    }
}