#region References

using System;
using System.Collections.Generic;
using System.Linq;
using Raspberry.Timers;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Represents a connection to the GPIO pins.
    /// </summary>
    public class GpioConnection : IDisposable
    {
        #region Fields

        private readonly GpioConnectionSettings settings;

        private readonly Dictionary<ProcessorPin, PinConfiguration> pinConfigurations;
        private readonly Dictionary<string, PinConfiguration> namedPins;

        private readonly ITimer timer;
        private readonly Dictionary<ProcessorPin, bool> pinValues = new Dictionary<ProcessorPin, bool>();
        private readonly Dictionary<ProcessorPin, EventHandler<PinStatusEventArgs>> pinEvents = new Dictionary<ProcessorPin, EventHandler<PinStatusEventArgs>>();

        private ProcessorPins inputPins = ProcessorPins.None;
        private ProcessorPins pinRawValues = ProcessorPins.None;

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection"/> class.
        /// </summary>
        /// <param name="pins">The pins.</param>
        public GpioConnection(params PinConfiguration[] pins) : this(null, (IEnumerable<PinConfiguration>) pins){}

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection"/> class.
        /// </summary>
        /// <param name="pins">The pins.</param>
        public GpioConnection(IEnumerable<PinConfiguration> pins) : this(null, pins){}

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="pins">The pins.</param>
        public GpioConnection(GpioConnectionSettings settings, params PinConfiguration[] pins) : this(settings, (IEnumerable<PinConfiguration>) pins){}

        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConnection"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="pins">The pins.</param>
        public GpioConnection(GpioConnectionSettings settings, IEnumerable<PinConfiguration> pins)
        {
            this.settings = settings ?? new GpioConnectionSettings();
            Pins = new ConnectedPins(this);

            var pinList = pins.ToList();
            pinConfigurations = pinList.ToDictionary(p => p.Pin);

            namedPins = pinList.Where(p => !string.IsNullOrEmpty(p.Name)).ToDictionary(p => p.Name);
            
            timer = Timer.Create();

            timer.Interval = this.settings.PollInterval;
            timer.Action = CheckInputPins;

            if (this.settings.Opened)
                Open();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether connection is opened.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connection is opened; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpened { get; private set; }

        /// <summary>
        /// Gets or sets the status of the pin having the specified name.
        /// </summary>
        public bool this[string name]
        {
            get { return this[namedPins[name].Pin]; }
            set { this[namedPins[name].Pin] = value; }
        }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        public bool this[ConnectorPin pin]
        {
            get { return this[pin.ToProcessor()]; }
            set { this[pin.ToProcessor()] = value; }
        }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        public bool this[PinConfiguration pin]
        {
            get { return pinValues[pin.Pin]; }
            set
            {
                if (pin.Direction == PinDirection.Output)
                {
                    var pinValue = pin.GetEffective(value);
                    Driver.Write(pin.Pin, pinValue);

                    pinValues[pin.Pin] = value;
                    OnPinStatusChanged(new PinStatusEventArgs {Enabled = value, Configuration = pin});
                }
                else
                    throw new InvalidOperationException("Value of input pins cannot be modified");

            }
        }

        /// <summary>
        /// Gets or sets the status of the specified pin.
        /// </summary>
        public bool this[ProcessorPin pin]
        {
            get { return this[pinConfigurations[pin]]; }
            set { this[pinConfigurations[pin]] = value; }
        }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        public ConnectedPins Pins { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void Open()
        {
            lock (timer)
            {
                if (IsOpened)
                    return;

                foreach (var pin in pinConfigurations.Values)
                    Allocate(pin);

                timer.Start(TimeSpan.FromMilliseconds(10));
                IsOpened = true;
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            lock (timer)
            {
                if (!IsOpened)
                    return;

                timer.Stop();
                foreach (var pin in pinConfigurations.Values)
                    Release(pin);

                IsOpened = false;
            }
        }

        /// <summary>
        /// Clears pin attached to this connection.
        /// </summary>
        public void Clear()
        {
            lock (pinConfigurations)
            {
                foreach (var pinConfiguration in pinConfigurations.Values)
                    Release(pinConfiguration);

                pinConfigurations.Clear();
                namedPins.Clear();
                pinValues.Clear();

                pinRawValues = ProcessorPins.None;
                inputPins = ProcessorPins.None;
            }
        }

        /// <summary>
        /// Adds the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Add(PinConfiguration pin)
        {
            lock (pinConfigurations)
            {
                if (pinConfigurations.ContainsKey(pin.Pin))
                    throw new InvalidOperationException("This pin is already present on the connection");
                if (!string.IsNullOrEmpty(pin.Name) && namedPins.ContainsKey(pin.Name))
                    throw new InvalidOperationException("A pin with the same name is already present on the connection");

                pinConfigurations.Add(pin.Pin, pin);

                if (!string.IsNullOrEmpty(pin.Name))
                    namedPins.Add(pin.Name, pin);

                lock (timer)
                {
                    if (IsOpened)
                        Allocate(pin);
                }
            }
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string pinName)
        {
            return namedPins.ContainsKey(pinName);
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ConnectorPin pin)
        {
            return pinConfigurations.ContainsKey(pin.ToProcessor());
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ProcessorPin pin)
        {
            return pinConfigurations.ContainsKey(pin);
        }

        /// <summary>
        /// Determines whether the connection contains the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        /// <returns>
        ///   <c>true</c> if the connection contains the specified pin; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(PinConfiguration configuration)
        {
            return pinConfigurations.ContainsKey(configuration.Pin);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        public void Remove(string pinName)
        {
            Remove(namedPins[pinName]);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Remove(ConnectorPin pin)
        {
            Remove(pinConfigurations[pin.ToProcessor()]);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Remove(ProcessorPin pin)
        {
            Remove(pinConfigurations[pin]);
        }

        /// <summary>
        /// Removes the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        public void Remove(PinConfiguration configuration)
        {
            lock (pinConfigurations)
            {
                lock (timer)
                {
                    if (IsOpened)
                        Release(configuration);
                }

                pinConfigurations.Remove(configuration.Pin);
                if (!string.IsNullOrEmpty(configuration.Name))
                    namedPins.Remove(configuration.Name);
                pinValues.Remove(configuration.Pin);

                var pin = (ProcessorPins)((uint)1 << (int)configuration.Pin);
                inputPins = inputPins & ~pin;
                pinRawValues = pinRawValues & ~pin;
            }
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        public void Toggle(string pinName)
        {
            this[pinName] = !this[pinName];
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toggle(ProcessorPin pin)
        {
            this[pin] = !this[pin];
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        public void Toggle(ConnectorPin pin)
        {
            this[pin] = !this[pin];
        }

        /// <summary>
        /// Toggles the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        public void Toggle(PinConfiguration configuration)
        {
            this[configuration] = !this[configuration];
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pinName">Name of the pin.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(string pinName, TimeSpan duration = new TimeSpan())
        {
            Toggle(pinName);
            Sleep(duration);
            Toggle(pinName);
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(ProcessorPin pin, TimeSpan duration = new TimeSpan())
        {
            Toggle(pin);
            Sleep(duration);
            Toggle(pin);
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(ConnectorPin pin, TimeSpan duration = new TimeSpan())
        {
            Toggle(pin);
            Sleep(duration);
            Toggle(pin);
        }

        /// <summary>
        /// Blinks the specified pin.
        /// </summary>
        /// <param name="configuration">The pin configuration.</param>
        /// <param name="duration">The duration.</param>
        public void Blink(PinConfiguration configuration, TimeSpan duration = new TimeSpan())
        {
            Toggle(configuration);
            Sleep(duration);
            Toggle(configuration);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the status of a pin changed.
        /// </summary>
        public event EventHandler<PinStatusEventArgs> PinStatusChanged;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the <see cref="PinStatusChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Raspberry.IO.GeneralPurpose.PinStatusEventArgs"/> instance containing the event data.</param>
        protected void OnPinStatusChanged(PinStatusEventArgs e)
        {
            var handler = PinStatusChanged;
            if (handler != null)
                handler(this, e);
        }

        #endregion
        
        #region Internal Methods

        internal PinConfiguration GetConfiguration(string pinName)
        {
            return namedPins[pinName];
        }

        internal PinConfiguration GetConfiguration(ProcessorPin pin)
        {
            return pinConfigurations[pin];
        }

        internal IEnumerable<PinConfiguration> Configurations
        {
            get { return pinConfigurations.Values; }
        }
        
        #endregion

        #region Private Helpers

        private IGpioConnectionDriver Driver
        {
            get { return settings.Driver; }
        }

        private void Sleep(TimeSpan duration)
        {
            Timer.Sleep(duration <= TimeSpan.Zero ? settings.BlinkDuration : duration);
        }

        private void Allocate(PinConfiguration configuration)
        {
            if (configuration.StatusChangedAction != null)
            {
                var handler = new EventHandler<PinStatusEventArgs>((sender, args) =>
                                                                       {
                                                                           if (args.Configuration == configuration)
                                                                               configuration.StatusChangedAction(args.Enabled);
                                                                       });
                pinEvents[configuration.Pin] = handler;
                PinStatusChanged += handler;
            }

            Driver.Allocate(configuration.Pin, configuration.Direction);
            var outputConfiguration = configuration as OutputPinConfiguration;
            if (outputConfiguration != null)
                this[configuration.Pin] = outputConfiguration.Enabled;
            else
            {
                var inputConfiguration = (InputPinConfiguration) configuration;
                var pinValue = Driver.Read(inputConfiguration.Pin);

                var pin = (ProcessorPins)((uint)1 << (int)inputConfiguration.Pin);
                inputPins = inputPins | pin;
                pinRawValues = Driver.Read(inputPins);

                if (inputConfiguration.Resistor != PinResistor.None && (Driver.GetCapabilities() & GpioConnectionDriverCapabilities.CanSetPinResistor) > 0)
                    Driver.SetPinResistor(inputConfiguration.Pin, inputConfiguration.Resistor);

                var switchConfiguration = inputConfiguration as SwitchInputPinConfiguration;
                if (switchConfiguration != null)
                {
                    pinValues[inputConfiguration.Pin] = switchConfiguration.Enabled;
                    OnPinStatusChanged(new PinStatusEventArgs { Configuration = inputConfiguration, Enabled = pinValues[inputConfiguration.Pin] });
                }
                else
                {
                    pinValues[inputConfiguration.Pin] = inputConfiguration.GetEffective(pinValue);
                    OnPinStatusChanged(new PinStatusEventArgs { Configuration = inputConfiguration, Enabled = pinValues[inputConfiguration.Pin] });
                }
            }
        }

        private void Release(PinConfiguration configuration)
        {
            if (configuration.Direction == PinDirection.Output)
            {
                Driver.Write(configuration.Pin, false);
                OnPinStatusChanged(new PinStatusEventArgs { Enabled = false, Configuration = configuration });
            }

            Driver.Release(configuration.Pin);

            EventHandler<PinStatusEventArgs> handler;
            if (pinEvents.TryGetValue(configuration.Pin, out handler))
            {
                PinStatusChanged -= handler;
                pinEvents.Remove(configuration.Pin);
            }
        }

        private void CheckInputPins()
        {
            var newPinValues = Driver.Read(inputPins);
            
            var changes = newPinValues ^ pinRawValues;
            if (changes == ProcessorPins.None)
                return;

            var notifiedConfigurations = new List<PinConfiguration>();
            foreach (var np in changes.Enumerate())
            {
                var processorPin = (ProcessorPins) ((uint) 1 << (int) np);
                var oldPinValue = (pinRawValues & processorPin) != ProcessorPins.None;
                var newPinValue = (newPinValues & processorPin) != ProcessorPins.None;

                if (oldPinValue != newPinValue)
                {
                    var pin = (InputPinConfiguration) pinConfigurations[np];
                    var switchPin = pin as SwitchInputPinConfiguration;

                    if (switchPin != null)
                    {
                        if (pin.GetEffective(newPinValue))
                        {
                            pinValues[np] = !pinValues[np];
                            notifiedConfigurations.Add(pin);
                        }
                    }
                    else
                    {
                        pinValues[np] = pin.GetEffective(newPinValue);
                        notifiedConfigurations.Add(pin);
                    }
                }
            }

            pinRawValues = newPinValues;

            // Only fires events once all states have been modified.
            foreach (var pin in notifiedConfigurations)
                OnPinStatusChanged(new PinStatusEventArgs {Configuration = pin, Enabled = pinValues[pin.Pin]});
        }

        #endregion
    }
}