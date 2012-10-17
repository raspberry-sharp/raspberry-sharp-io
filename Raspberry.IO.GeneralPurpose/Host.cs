#region References

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents the host.
    /// </summary>
    internal class Host
    {
        #region Fields

        private static readonly Lazy<Host> board = new Lazy<Host>(LoadBoard);
        private readonly Dictionary<string, string> settings;

        private const string raspberryPiProcessor = "BCM2708";
        
        #endregion

        #region Instance Management

        private Host(Dictionary<string, string> settings)
        {
            this.settings = settings;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current mainboard configuration.
        /// </summary>
        public static Host Current
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
            get
            {
                string hardware;
                return settings.TryGetValue("Hardware", out hardware) && string.Equals(hardware, raspberryPiProcessor, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Gets the board revision.
        /// </summary>
        public int BoardRevision
        {
            get
            {
                if (!IsRaspberryPi)
                    throw new NotSupportedException("Host is not a Raspberry Pi");

                string revision;
                int firmware;
                if (settings.TryGetValue("Revision", out revision) && !string.IsNullOrEmpty(revision) && int.TryParse(revision, out firmware))
                {
                    switch(firmware)
                    {
                        case 2:
                        case 3:
                            return 1;
                        case 4:
                        case 5:
                        case 6:
                            return 2;
                    }
                }

                throw new NotSupportedException(string.Format("Raspberry Pi revision '{0}' not supported", revision));
            }
        }

        #endregion

        #region Private Helpers

        private static Host LoadBoard()
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

                return new Host(settings);
            }
            catch
            {
                return new Host(new Dictionary<string, string>());
            }
        }

        #endregion
    }
}