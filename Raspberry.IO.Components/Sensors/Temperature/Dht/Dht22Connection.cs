#region References

using System;
using UnitsNet;

#endregion

namespace Raspberry.IO.Components.Sensors.Temperature.Dht
{
    public class Dht22Connection : DhtConnection
    {
        #region Instance Management

        public Dht22Connection(IInputOutputBinaryPin pin, bool autoStart = true) : base(pin, autoStart){}

        #endregion

        #region Protected Methods

        protected override TimeSpan MinimumSamplingInterval
        {
            get { return TimeSpan.FromSeconds(2); }
        }

        protected override TimeSpan WakeupInterval
        {
            get { return TimeSpan.FromMilliseconds(1); }
        }

        protected override DhtData GetDhtData(int temperatureValue, int humidityValue)
        {
            return new DhtData
            {
                RelativeHumidity = Ratio.FromPercent(humidityValue/10d),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius(temperatureValue/10d)
            };
        }

        #endregion
    }
}