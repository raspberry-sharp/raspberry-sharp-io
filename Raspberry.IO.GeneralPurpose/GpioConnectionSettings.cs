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

        private IGpioConnectionDriver driver;

        private TimeSpan blinkDuration;
        private TimeSpan pollInterval;

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
        /// Gets the default blink duration.
        /// </summary>
        public static readonly TimeSpan DefaultBlinkDuration = TimeSpan.FromMilliseconds(250);

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
        public TimeSpan BlinkDuration
        {
            get { return blinkDuration; }
            set { blinkDuration = value >= TimeSpan.Zero ? value : DefaultBlinkDuration; }
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
        /// The poll interval.
        /// </value>
        public TimeSpan PollInterval
        {
            get { return pollInterval; }
            set { pollInterval = value >= TimeSpan.Zero ? value : DefaultPollInterval; }
        }

        /// <summary>
        /// Gets the default poll interval.
        /// </summary>
        public static TimeSpan DefaultPollInterval
        {
            get
            {
                var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
                return TimeSpan.FromMilliseconds(configurationSection != null
                           ? (double)configurationSection.PollInterval
                           : (double)GpioConnectionConfigurationSection.DefaultPollInterval);

            }
        }

        /// <summary>
        /// Gets the board connector pinout.
        /// </summary>
        /// <value>
        /// The board connector pinout.
        /// </value>
        public static ConnectorPinout ConnectorPinout
        {
            get
            {
                var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
                if (configurationSection != null)
                {
                    switch (configurationSection.BoardConnectorRevision)
                    {
                        case 1:
                            return ConnectorPinout.Rev1;
                        case 2:
                            return ConnectorPinout.Rev2;
                        case 3:
                            return ConnectorPinout.Plus;
                    }
                }

                return Board.Current.ConnectorPinout;
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
                return (configurationSection != null && !String.IsNullOrEmpty(configurationSection.DriverTypeName))
                    ? (IGpioConnectionDriver) Activator.CreateInstance(Type.GetType(configurationSection.DriverTypeName, true))
                    : GetBestDriver(Board.Current.IsRaspberryPi ? GpioConnectionDriverCapabilities.None : GpioConnectionDriverCapabilities.CanWorkOnThirdPartyComputers);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the best driver for the specified capabilities.
        /// </summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <returns>The best driver, if found; otherwise, <c>null</c>.</returns>
        public static IGpioConnectionDriver GetBestDriver(GpioConnectionDriverCapabilities capabilities)
        {
            if ((GpioConnectionDriver.GetCapabilities() & capabilities) == capabilities)
                return new GpioConnectionDriver();
            if ((MemoryGpioConnectionDriver.GetCapabilities() & capabilities) == capabilities)
                return new MemoryGpioConnectionDriver();
            if ((FileGpioConnectionDriver.GetCapabilities() & capabilities) == capabilities)
                return new FileGpioConnectionDriver();

            return null;
        }

        #endregion
    }
}