#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Raspberry.IO.GeneralPurpose.Configuration;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    public class GpioConnection : IDisposable
    {
        #region Fields

        private readonly Dictionary<ProcessorPin, PinConfiguration> pinConfigurations;
        private readonly Dictionary<string, PinConfiguration> namedPins;

        private readonly Timer timer;
        private readonly Dictionary<ProcessorPin, bool> pinValues = new Dictionary<ProcessorPin, bool>();
        private readonly Dictionary<ProcessorPin, EventHandler<PinStatusEventArgs>> pinEvents = new Dictionary<ProcessorPin, EventHandler<PinStatusEventArgs>>();
        private readonly Dictionary<ProcessorPin, bool> pinRawValues = new Dictionary<ProcessorPin, bool>();
        
        public const int DefaultBlinkDuration = 250;

        #endregion

        #region Instance Management

        public GpioConnection(params PinConfiguration[] pins) : this(true, null, (IEnumerable<PinConfiguration>) pins){}

        public GpioConnection(IEnumerable<PinConfiguration> pins) : this(true, null, pins){}

        public GpioConnection(IConnectionDriver driver, params PinConfiguration[] pins) : this(true, driver, (IEnumerable<PinConfiguration>) pins){}

        public GpioConnection(IConnectionDriver driver, IEnumerable<PinConfiguration> pins) : this(true, driver, pins){}

        public GpioConnection(bool open, params PinConfiguration[] pins) : this(open, null, (IEnumerable<PinConfiguration>) pins){}

        public GpioConnection(bool open, IConnectionDriver driver, params PinConfiguration[] pins) : this(open, driver, (IEnumerable<PinConfiguration>) pins){}

        public GpioConnection(bool open, IEnumerable<PinConfiguration> pins) : this(open, null, pins){}

        public GpioConnection(bool open, IConnectionDriver driver, IEnumerable<PinConfiguration> pins)
        {
            Driver = driver ?? GetDefaultDriver();
            Pins = new ConnectionPins(this);

            var pinList = pins.ToList();
            pinConfigurations = pinList.ToDictionary(p => p.Pin);

            namedPins = pinList.Where(p => !string.IsNullOrEmpty(p.Name)).ToDictionary(p => p.Name);

            timer = new Timer(CheckInputPins, null, Timeout.Infinite, Timeout.Infinite);
            if (open)
                Open();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion

        #region Properties

        public bool IsOpened { get; private set; }

        public IConnectionDriver Driver { get; private set; }

        public bool this[string name]
        {
            get { return this[namedPins[name].Pin]; }
            set { this[namedPins[name].Pin] = value; }
        }

        public bool this[ConnectorPin pin]
        {
            get { return this[pin.ToProcessor()]; }
            set { this[pin.ToProcessor()] = value; }
        }

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
                    throw new InvalidOperationException("Input pin value cannot be set");

            }
        }

        public bool this[ProcessorPin pin]
        {
            get { return this[pinConfigurations[pin]]; }
            set { this[pinConfigurations[pin]] = value; }
        }

        public ConnectionPins Pins { get; private set; }

        #endregion

        #region Methods

        public void Open()
        {
            lock (timer)
            {
                if (IsOpened)
                    return;

                foreach (var pin in pinConfigurations.Values)
                    Export(pin);

                timer.Change(250, 50);
                IsOpened = true;
            }
        }

        public void Close()
        {
            lock (timer)
            {
                if (!IsOpened)
                    return;

                timer.Dispose();
                foreach (var pin in pinConfigurations.Values)
                    Unexport(pin);

                IsOpened = false;
            }
        }

        public void Clear()
        {
            foreach (var pinConfiguration in pinConfigurations.Values)
                Unexport(pinConfiguration);

            pinConfigurations.Clear();
            namedPins.Clear();
            pinRawValues.Clear();
            pinValues.Clear();
        }

        public void Add(PinConfiguration pin)
        {
            if (pinConfigurations.ContainsKey(pin.Pin))
                throw new InvalidOperationException("This pin is already present on the connection");
            if (!string.IsNullOrEmpty(pin.Name) && namedPins.ContainsKey(pin.Name))
                throw new InvalidOperationException("A pin with the same name is already present on the connection");

            pinConfigurations.Add(pin.Pin, pin);

            if (!string.IsNullOrEmpty(pin.Name))
                namedPins.Add(pin.Name, pin);

            Export(pin);
        }

        public bool Contains(string pinName)
        {
            return namedPins.ContainsKey(pinName);
        }

        public bool Contains(ConnectorPin pin)
        {
            return pinConfigurations.ContainsKey(pin.ToProcessor());
        }

        public bool Contains(ProcessorPin pin)
        {
            return pinConfigurations.ContainsKey(pin);
        }

        public bool Contains(PinConfiguration configuration)
        {
            return pinConfigurations.ContainsKey(configuration.Pin);
        }

        public void Remove(string pinName)
        {
            Remove(namedPins[pinName]);
        }

        public void Remove(ConnectorPin pin)
        {
            Remove(pinConfigurations[pin.ToProcessor()]);
        }

        public void Remove(ProcessorPin pin)
        {
            Remove(pinConfigurations[pin]);
        }

        public void Remove(PinConfiguration configuration)
        {
            Unexport(configuration);

            pinConfigurations.Remove(configuration.Pin);
            if (!string.IsNullOrEmpty(configuration.Name))
                namedPins.Remove(configuration.Name);

            pinRawValues.Remove(configuration.Pin);
            pinValues.Remove(configuration.Pin);
        }

        public void Toggle(string pinName)
        {
            this[pinName] = !this[pinName];
        }

        public void Toggle(ProcessorPin pin)
        {
            this[pin] = !this[pin];
        }

        public void Toggle(ConnectorPin pin)
        {
            this[pin] = !this[pin];
        }

        public void Toggle(PinConfiguration configuration)
        {
            this[configuration] = !this[configuration];
        }

        public void Blink(string pinName, int duration = DefaultBlinkDuration)
        {
            Toggle(pinName);
            Thread.Sleep(duration);
            Toggle(pinName);
        }

        public void Blink(ProcessorPin pin, int duration = DefaultBlinkDuration)
        {
            Toggle(pin);
            Thread.Sleep(duration);
            Toggle(pin);
        }

        public void Blink(ConnectorPin pin, int duration = DefaultBlinkDuration)
        {
            Toggle(pin);
            Thread.Sleep(duration);
            Toggle(pin);
        }

        public void Blink(PinConfiguration configuration, int duration = DefaultBlinkDuration)
        {
            Toggle(configuration);
            Thread.Sleep(duration);
            Toggle(configuration);
        }

        #endregion

        #region Events

        public event EventHandler<PinStatusEventArgs> PinStatusChanged;

        #endregion

        #region Protected Methods

        protected void OnPinStatusChanged(PinStatusEventArgs e)
        {
            var handler = PinStatusChanged;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        #region Private Helpers

        private IEnumerable<PinConfiguration> Configurations
        {
            get { return pinConfigurations.Values; }
        }

        private static IConnectionDriver GetDefaultDriver()
        {
            var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
            if (configurationSection != null && !string.IsNullOrEmpty(configurationSection.DriverTypeName))
                return (IConnectionDriver) Activator.CreateInstance(Type.GetType(configurationSection.DriverTypeName, true));
            else
                return new MemoryConnectionDriver();
        }

        private void Export(PinConfiguration configuration)
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

            Driver.Export(configuration);
            var outputConfiguration = configuration as OutputPinConfiguration;
            if (outputConfiguration != null)
                this[configuration.Pin] = outputConfiguration.GetEffective(outputConfiguration.Enabled);
            else
            {
                var pinValue = Driver.Read(configuration.Pin);
                pinRawValues[configuration.Pin] = pinValue;

                var switchConfiguration = configuration as SwitchInputPinConfiguration;
                if (switchConfiguration != null)
                {
                    pinValues[configuration.Pin] = switchConfiguration.Enabled;
                    OnPinStatusChanged(new PinStatusEventArgs {Configuration = configuration, Enabled = pinValues[configuration.Pin]});
                }
                else
                {
                    pinValues[configuration.Pin] = configuration.GetEffective(pinValue);
                    OnPinStatusChanged(new PinStatusEventArgs { Configuration = configuration, Enabled = pinValues[configuration.Pin] });
                }
            }
        }

        private void Unexport(PinConfiguration configuration)
        {
            if (configuration.Direction == PinDirection.Output)
            {
                Driver.Write(configuration.Pin, false);
                OnPinStatusChanged(new PinStatusEventArgs { Enabled = false, Configuration = configuration });
            }

            Driver.Unexport(configuration);

            EventHandler<PinStatusEventArgs> handler;
            if (pinEvents.TryGetValue(configuration.Pin, out handler))
            {
                PinStatusChanged -= handler;
                pinEvents.Remove(configuration.Pin);
            }
        }

        private void CheckInputPins(object state)
        {
            var newPinValues = pinConfigurations.Values
                .Where(p => p.Direction == PinDirection.Input)
                .Select(p => new {p.Pin, Value = Driver.Read(p.Pin)})
                .ToDictionary(p => p.Pin, p => p.Value);

            foreach (var np in newPinValues)
            {
                var oldPinValue = pinRawValues[np.Key];
                var newPinValue = np.Value;

                pinRawValues[np.Key] = newPinValue;
                if (oldPinValue != newPinValue)
                {
                    var pin = (InputPinConfiguration) pinConfigurations[np.Key];
                    var switchPin = pin as SwitchInputPinConfiguration;

                    if (switchPin != null)
                    {
                        if (pin.GetEffective(newPinValue))
                        {
                            pinValues[np.Key] = !pinValues[np.Key];
                            OnPinStatusChanged(new PinStatusEventArgs {Configuration = pin, Enabled = pinValues[np.Key]});
                        }
                    }
                    else
                    {
                        pinValues[np.Key] = pin.GetEffective(newPinValue);
                        OnPinStatusChanged(new PinStatusEventArgs {Configuration = pin, Enabled = pinValues[np.Key]});
                    }
                }
            }
        }

        private PinConfiguration GetConfiguration(string pinName)
        {
            return namedPins[pinName];
        }

        private PinConfiguration GetConfiguration(ProcessorPin pin)
        {
            return pinConfigurations[pin];
        }

        #endregion

        #region Inner Classes

        public class ConnectionPin
        {
            private readonly GpioConnection connection;
            private readonly HashSet<EventHandler<PinStatusEventArgs>> events = new HashSet<EventHandler<PinStatusEventArgs>>();

            public ConnectionPin(GpioConnection connection, PinConfiguration pinConfiguration)
            {
                this.connection = connection;
                Configuration = pinConfiguration;
            }

            public PinConfiguration Configuration { get; private set; }

            public bool Value
            {
                get { return connection[Configuration]; }
                set { connection[Configuration] = value; }
            }

            public void Toggle()
            {
                connection.Toggle(Configuration);
            }

            public void Blink(int duration = DefaultBlinkDuration)
            {
                connection.Blink(Configuration, duration);
            }

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

            private void ConnectionPinStatusChanged(object sender, PinStatusEventArgs pinStatusEventArgs)
            {
                if (pinStatusEventArgs.Configuration.Pin != Configuration.Pin)
                    return;

                foreach (var eventHandler in events)
                    eventHandler(sender, pinStatusEventArgs);
            }
        }

        public class ConnectionPins : IEnumerable<ConnectionPin>
        {
            private readonly GpioConnection connection;

            internal ConnectionPins(GpioConnection connection)
            {
                this.connection = connection;
            }

            public ConnectionPin this[ProcessorPin pin]
            {
                get { return new ConnectionPin(connection, connection.GetConfiguration(pin)); }
            }

            public ConnectionPin this[string name]
            {
                get { return new ConnectionPin(connection, connection.GetConfiguration(name)); }
            }

            public ConnectionPin this[ConnectorPin pin]
            {
                get { return this[pin.ToProcessor()]; }
            }

            public ConnectionPin this[PinConfiguration pin]
            {
                get { return new ConnectionPin(connection, pin); }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<ConnectionPin> GetEnumerator()
            {
                return connection.Configurations.Select(c => new ConnectionPin(connection, c)).GetEnumerator();
            }
        }

        #endregion
    }
}