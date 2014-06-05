#region References

using System;
using System.Collections.Generic;

#endregion

namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// The PWM channels of a TLC59711 device cluster
    /// </summary>
    internal sealed class Tlc59711ClusterChannels : IPwmChannels
    {
        #region Structures
        private struct Mapping
        {
            private readonly ITlc59711Device device;
            private readonly int channelIndex;

            public Mapping(ITlc59711Device device, int channelIndex) {
                this.device = device;
                this.channelIndex = channelIndex;
            }

            public ITlc59711Device Device {
                get { return device; }
            }
            public int ChannelIndex {
                get { return channelIndex; }
            }
        }
        #endregion

        #region Fields
        private readonly List<Mapping> deviceMap = new List<Mapping>();
        #endregion

        #region Instance Management

        /// <summary>
        /// Creates a new instance of the <see cref="Tlc59711ClusterChannels"/> class.
        /// </summary>
        /// <param name="devices">TLC59711 devices</param>
        public Tlc59711ClusterChannels(IEnumerable<ITlc59711Device> devices)
        {
            if (devices == null)
                throw new ArgumentNullException("devices");

            foreach (var device in devices)
            {
                if (ReferenceEquals(device, null))
                    continue;

                for (var i = 0; i < device.Channels.Count; i++)
                    deviceMap.Add(new Mapping(device, i));
            }
        }

        #endregion
        
        #region Properties
        /// <summary>
        /// Indexer, which will allow client code to use [] notation on the class instance itself to modify PWM channel values.
        /// </summary>
        /// <param name="index">channel index</param>
        /// <returns>The current PWM value from <paramref name="index"/></returns>
        public ushort this[int index] {
            get { return Get(index); }
            set { Set(index, value); }
        }
        /// <summary>
        /// Returns the number of channels.
        /// </summary>
        public int Count {
            get { return deviceMap.Count; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the PWM value at the specified channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index</param>
        /// <returns>The PWM value at the specified channel <paramref name="index"/></returns>
        public ushort Get(int index) {
            var mapping = deviceMap[index];
            return mapping.Device.Channels[mapping.ChannelIndex];
        }

        /// <summary>
        /// Sets the PWM value at channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index</param>
        /// <param name="value">The PWM value</param>
        public void Set(int index, ushort value) {
            var mapping = deviceMap[index];
            mapping.Device.Channels[mapping.ChannelIndex] = value;
        }
        #endregion
    }
}