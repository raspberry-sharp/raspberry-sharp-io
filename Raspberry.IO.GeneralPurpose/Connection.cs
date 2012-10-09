#region References

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Raspberry.IO.GeneralPurpose.Configuration;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    public class Connection : IDisposable
    {
        #region Fields

        private readonly Dictionary<ProcessorPin, PinConfiguration> pins;
        private readonly Dictionary<string, PinConfiguration> namedPins;

        private readonly Timer timer;
        private readonly Dictionary<ProcessorPin, bool> pinValues = new Dictionary<ProcessorPin, bool>();
        private readonly Dictionary<ProcessorPin, bool> pinRawValues = new Dictionary<ProcessorPin, bool>();

        #endregion

        #region Instance Management

        public Connection(params PinConfiguration[] pins) : this(null, true, (IEnumerable<PinConfiguration>) pins){}

        public Connection(IEnumerable<PinConfiguration> pins) : this(null, true, pins){}

        public Connection(IConnectionDriver driver, params PinConfiguration[] pins) : this(driver, true, (IEnumerable<PinConfiguration>) pins){}

        public Connection(IConnectionDriver driver, IEnumerable<PinConfiguration> pins) : this(driver, true, pins){}

        public Connection(IConnectionDriver driver, bool open, params PinConfiguration[] pins) : this(driver, open, (IEnumerable<PinConfiguration>) pins){}

        public Connection(IConnectionDriver driver, bool open, IEnumerable<PinConfiguration> pins)
        {
            var pinList = pins.ToList();

            Driver = driver ?? GetDefaultDriver();
            this.pins = pinList.ToDictionary(p => p.Pin);
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
            get
            {
                return pinValues[pin.Pin];
            }
            set
            {
                if (pin.Direction == PinDirection.Output)
                {
                    var pinValue = pin.GetEffective(value);
                    Driver.Write(pin.Pin, pinValue);

                    pinValues[pin.Pin] = value;
                }
                else
                    throw new InvalidOperationException("Input pin value cannot be set");

            }
        }

        public bool this[ProcessorPin pin]
        {
            get
            {
                return this[pins[pin]];
            }
            set
            {
                this[pins[pin]] = value;
            }
        }

        #endregion

        #region Methods

        public void Open()
        {
            lock (timer)
            {
                if (IsOpened)
                    return;

                ExportPins();
                timer.Change(0, 50);
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
                foreach (var pin in pins.Values)
                {
                    if (pin.Direction == PinDirection.Output)
                        Driver.Write(pin.Pin, false);
                    Driver.Unexport(pin);
                }

                IsOpened = false;
            }
        }

        public void Toggle(string pin)
        {
            this[pin] = !this[pin];
        }

        public void Toggle(ProcessorPin pin)
        {
            this[pin] = !this[pin];
        }

        public void Toggle(ConnectorPin pin)
        {
            this[pin] = !this[pin];
        }

        public void Toggle(PinConfiguration pin)
        {
            this[pin] = !this[pin];
        }

        #endregion

        #region Events

        public event EventHandler<PinStatusEventArgs> InputPinChanged;

        #endregion

        #region Private Helpers

        private void ExportPins()
        {
            foreach (var pin in pins.Values)
            {
                Driver.Export(pin);
                var outputPin = pin as OutputPinConfiguration;
                if (outputPin != null)
                    this[pin.Pin] = outputPin.GetEffective(outputPin.Enabled);
            }
        }

        private static IConnectionDriver GetDefaultDriver()
        {
            var configurationSection = ConfigurationManager.GetSection("gpioConnection") as GpioConnectionConfigurationSection;
            if (configurationSection != null && !string.IsNullOrEmpty(configurationSection.DriverTypeName))
                return (IConnectionDriver) Activator.CreateInstance(Type.GetType(configurationSection.DriverTypeName, true));
            else
                return new ConnectionMemoryDriver();
        }

        private void CheckInputPins(object state)
        {
            var newPinValues = pins.Values
                .Where(p => p.Direction == PinDirection.Input)
                .Select(p => new {p.Pin, Value = Driver.Read(p.Pin)})
                .ToDictionary(p => p.Pin, p => p.Value);

            foreach (var np in newPinValues)
            {
                var init = !pinRawValues.ContainsKey(np.Key);
                var oldPinValue = pinRawValues.ContainsKey(np.Key) && pinRawValues[np.Key];
                var newPinValue = np.Value;

                pinRawValues[np.Key] = newPinValue;
                if (init || oldPinValue != newPinValue)
                {
                    var pin = (InputPinConfiguration) pins[np.Key];
                    var switchPin = pin as SwitchInputPinConfiguration;

                    if (switchPin != null)
                    {
                        if (init)
                        {
                            pinValues[np.Key] = switchPin.Enabled;
                            InputPinChanged(this, new PinStatusEventArgs {Pin = pin, Enabled = pinValues[np.Key]});
                        }
                        else if (pin.GetEffective(newPinValue))
                        {
                            pinValues[np.Key] = !pinValues[np.Key];
                            InputPinChanged(this, new PinStatusEventArgs {Pin = pin, Enabled = pinValues[np.Key]});
                        }
                    }
                    else
                    {
                        pinValues[np.Key] = pin.GetEffective(newPinValue);
                        InputPinChanged(this, new PinStatusEventArgs {Pin = pin, Enabled = pinValues[np.Key]});
                    }
                }
            }
        }

        #endregion
    }
}