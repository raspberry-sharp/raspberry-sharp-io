#region References

using System;
using System.Collections.Generic;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a connected pin.
    /// </summary>
    public class ConnectedPin
    {
        #region Fields

        private readonly GpioConnection connection;
        private readonly HashSet<EventHandler<PinStatusEventArgs>> events = new HashSet<EventHandler<PinStatusEventArgs>>();

        #endregion

        #region Instance Management

        internal ConnectedPin(GpioConnection connection, PinConfiguration pinConfiguration)
        {
            this.connection = connection;
            Configuration = pinConfiguration;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public PinConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConnectedPin"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get { return connection[Configuration]; }
            set { connection[Configuration] = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Toggles this pin.
        /// </summary>
        public void Toggle()
        {
            connection.Toggle(Configuration);
        }

        /// <summary>
        /// Blinks the pin.
        /// </summary>
        /// <param name="duration">The blink duration.</param>
        public void Blink(TimeSpan duration = new TimeSpan())
        {
            connection.Blink(Configuration, duration);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when pin status changed.
        /// </summary>
        public event EventHandler<PinStatusEventArgs> StatusChanged
        {
            add
            {
                if (events.Count == 0)
                    connection.PinStatusChanged += ConnectionPinStatusChanged;
                events.Add(value);
            }
            remove
            {
                events.Remove(value);
                if (events.Count == 0)
                    connection.PinStatusChanged -= ConnectionPinStatusChanged;
            }
        }

        #endregion

        #region Private Helpers

        private void ConnectionPinStatusChanged(object sender, PinStatusEventArgs pinStatusEventArgs)
        {
            if (pinStatusEventArgs.Configuration.Pin != Configuration.Pin)
                return;

            foreach (var eventHandler in events)
                eventHandler(sender, pinStatusEventArgs);
        }

        #endregion
    }
}