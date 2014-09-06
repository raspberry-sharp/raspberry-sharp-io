#region References

using System;
using System.Configuration;
using Raspberry.IO.GeneralPurpose.Configuration;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents settings for <see cref="GpioConnection"/>.
    /// </summary>
    public class GpioConnectionSettings
    {
        #region Fields

        private decimal blinkDuration;
        private IGpioConnectionDriver driver;
        private decimal pollInterval;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnectionSettings"/> class.
        /// </summary>
        public GpioConnectionSettings()
        {
            Driver = DefaultDriver;
            BlinkDuration = DefaultBlinkDuration;
            PollInterval = DefaultPollInterval;
            Opened = true;
        }

        #endregion
        
        #region Constants

        /// <summary>
        /// Gets the default blink duration, in milliseconds.
        /// </summary>
        public const decimal DefaultBlinkDuration = 250;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GpioConnectionSettings"/> is opened on initialization.
        /// </summary>
        /// <value>
        ///   <c>true</c> if opened on initialization; otherwise, <c>false</c>.
        /// </value>
        public bool Opened { get; set; }

        /// <summary>
        /// Gets or sets the duration of the blink.
        /// </summary>
        /// <value>
        /// The duration of the blink, in milliseconds.
        /// </value>
        public decimal BlinkDuration
        {
            get { return blinkDuration; }
            set { blinkDuration = value >= 0 ? value : DefaultBlinkDuration; }
        }

        /// <summary>
        /// Gets or sets the driver.
        /// </summary>
        /// <value>
        /// The driver.
        /// </value>
        public IGpioConnectionDriver Driver
        {
            get { return driver; }
            set { driver = value ?? DefaultDriver; }
        }

        /// <summary>
        /// Gets or sets the poll interval.
        /// </summary>
        /// <value>
        /// The poll interval, in milliseconds.
        /// </value>
        public decimal PollInterval
        {
            get { return pollInterval; }
            set { pollInterval = value >= 0 ? value : DefaultPollInterval; }
        }

        /// <summary>
        /// Gets the default poll interval, in milliseconds.
        /// </summary>
        public static decimal DefaultPollInterval
        {
            get
            {
                var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
                return configurationSection != null
                           ? configurationSection.PollInterval
                           : GpioConnectionConfigurationSection.DefaultPollInterval;

            }
        }

        /// <summary>
        /// Gets the board connector revision.
        /// </summary>
        /// <value>
        /// The board connector revision.
        /// </value>
        public static int BoardConnectorRevision
        {
            get
            {
                var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
                if (configurationSection != null)
                    return configurationSection.BoardConnectorRevision;

                var board = Board.Current;
                if (board.Model == 'B' && board.Revision == 1)
                    return 1;

                if (board.Model == 'B' && board.Revision == 3)
                    return 3;

                return 2;
            }
        }

        /// <summary>
        /// Gets the default driver.
        /// </summary>
        public static IGpioConnectionDriver DefaultDriver
        {
            get
            {
                var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
                return configurationSection != null && !String.IsNullOrEmpty(configurationSection.DriverTypeName)
                           ? (IGpioConnectionDriver) Activator.CreateInstance(Type.GetType(configurationSection.DriverTypeName, true))
						: (Raspberry.Board.Current.IsRaspberryPi ? (IGpioConnectionDriver)new GpioConnectionDriver() : (IGpioConnectionDriver)new FileGpioConnectionDriver());
            }
        }

        #endregion
    }
}