using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Raspberry.IO.GeneralPurpose
{
    public class Connection : IDisposable
    {
        private readonly IConnectionDriver driver;

        private readonly Dictionary<ProcessorPin, PinConfiguration> pins;
        private readonly Dictionary<string, PinConfiguration> namedPins;

        private readonly Timer timer;
        private readonly Dictionary<ProcessorPin, bool> pinValues = new Dictionary<ProcessorPin, bool>();

        public Connection(params PinConfiguration[] pins) : this(null, (IEnumerable<PinConfiguration>) pins){}

        public Connection(IEnumerable<PinConfiguration> pins) : this(null, pins){}

        public Connection(IConnectionDriver driver, params PinConfiguration[] pins) : this(driver, (IEnumerable<PinConfiguration>) pins){}

        public Connection(IConnectionDriver driver, IEnumerable<PinConfiguration> pins)
        {
            var pinList = pins.ToList();

            this.driver = driver ?? GetDefaultDriver();
            this.pins = pinList.ToDictionary(p => p.Pin);
            namedPins = pinList.Where(p => !string.IsNullOrEmpty(p.Name)).ToDictionary(p => p.Name);

            ExportPins();

            timer = new Timer(CheckInputPins, null, 0, 50);
        }

        private void ExportPins()
        {
            foreach (var pin in pins.Values)
            {
                driver.Export(pin);
                var outputPin = pin as OutputPinConfiguration;
                if (outputPin != null)
                    this[pin.Pin] = outputPin.GetEffective(outputPin.IsActive);
            }
        }

        private static IConnectionDriver GetDefaultDriver()
        {
            //TODO: Loads default driver for app.config file if any, otherwise use memory driver.
            return new ConnectionFileDriver();
        }

        private void CheckInputPins(object state)
        {
            var newPinValues = pins.Values
                .Where(p => p.Direction == PinDirection.Input)
                .Select(p => new {p.Pin, Value = this[p.Pin]})
                .ToDictionary(p => p.Pin, p => p.Value);

            foreach (var np in newPinValues)
            {
                var newPinValue = newPinValues[np.Key];
                if (!pinValues.ContainsKey(np.Key) || pinValues[np.Key] != newPinValue)
                {
                    InputPinChanged(this, new PinStatusEventArgs {Pin = pins[np.Key], IsActive = newPinValue});
                    pinValues[np.Key] = newPinValue;
                }
            }
        }

        public event EventHandler<PinStatusEventArgs> InputPinChanged;

        void IDisposable.Dispose()
        {
            timer.Dispose();
            Close();
        }

        public void Close()
        {
            foreach (var pin in pins.Values)
            {
                if (pin.Direction == PinDirection.Output)
                    driver.Write(pin.Pin, false);
                driver.Unexport(pin);
            }
        }
        
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
                return pin.Direction == PinDirection.Input
                    ? pin.GetEffective(driver.Read(pin.Pin))
                    : pinValues[pin.Pin];
            }
            set
            {
                if (pin.Direction == PinDirection.Output)
                {
                    var pinValue = pin.GetEffective(value);
                    driver.Write(pin.Pin, pinValue);

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
    }
}