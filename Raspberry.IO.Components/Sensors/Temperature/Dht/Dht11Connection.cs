#region References

using System;
using UnitsNet;

#endregion

namespace Raspberry.IO.Components.Sensors.Temperature.Dht
{
    public class Dht11Connection : DhtConnection
    {
        #region Instance Management

        public Dht11Connection(IInputOutputBinaryPin pin, bool autoStart = true) : base(pin, autoStart){}

        #endregion

        #region Protected Methods

        protected override TimeSpan MinimumSamplingInterval
        {
            get { return TimeSpan.FromSeconds(1); }
        }

        protected override TimeSpan WakeupInterval
        {
            get { return TimeSpan.FromMilliseconds(18); }
        }

        protected override DhtData GetDhtData(int temperatureValue, int humidityValue)
        {
            return new DhtData
            {
                RelativeHumidity = Ratio.FromPercent(humidityValue/256d),
                Temperature = UnitsNet.Temperature.FromDegreesCelsius(temperatureValue/256d)
            };
        }

        #endregion
    }
}