#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Raspberry.IO.Interop;

#endregion

namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// A chained cluster of Adafruit's 12-channel 16bit PWM/LED driver TLC59711. 
    /// The devices should be connected together with their SDTI/SDTO pins.
    /// </summary>
    public class Tlc59711Cluster : ITlc59711Cluster
    {
        #region Constants
        private const int COMMAND_SIZE = Tlc59711Device.COMMAND_SIZE;
        #endregion

        #region Fields
        private readonly ITlc59711Device[] devices;
        private readonly IPwmChannels channels;
        #endregion

        #region Instance Management

        /// <summary>
        /// Creates a new instance of the <see cref="Tlc59711Cluster"/> class.
        /// </summary>
        /// <param name="memory">Memory to work with.</param>
        /// <param name="numberOfDevices">Number of <see cref="ITlc59711Device"/>s connected together.</param>
        public Tlc59711Cluster(IMemory memory, int numberOfDevices) {
            if (ReferenceEquals(memory, null))
                throw new ArgumentNullException("memory");
            if (numberOfDevices <= 0)
                throw new ArgumentOutOfRangeException("numberOfDevices", "You cannot create a cluster with less than one device.");

            var minimumRequiredMemorySize = (numberOfDevices * COMMAND_SIZE);
            if (memory.Length < minimumRequiredMemorySize) {
                var message = string.Format("For {0} device(s) you have to provide a minimum of {1} bytes of memory.", numberOfDevices, minimumRequiredMemorySize);
                throw new InsufficientMemoryException(message);
            }
            
            devices = CreateDevices(memory, numberOfDevices).ToArray();
            channels = new Tlc59711ClusterChannels(devices);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Tlc59711Cluster"/> class.
        /// </summary>
        /// <param name="devices">The devices, that are chained together</param>
        public Tlc59711Cluster(IEnumerable<ITlc59711Device> devices) {
            this.devices = devices.ToArray();
            channels = new Tlc59711ClusterChannels(this.devices);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Number of TLC59711 devices chained together
        /// </summary>
        public int Count {
            get { return devices.Length; }
        }

        /// <summary>
        /// Returns the TLC59711 device at the requested position
        /// </summary>
        /// <param name="index">TLC59711 index</param>
        /// <returns>TLC59711 device</returns>
        public ITlc59711Device this[int index] {
            get { return devices[index]; }
        }

        /// <summary>
        /// The PWM channels
        /// </summary>
        public IPwmChannels Channels {
            get { return channels; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerator`1"/> object.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ITlc59711Device> GetEnumerator() {
            return ((IEnumerable<ITlc59711Device>) devices)
                .GetEnumerator();
        }

        /// <summary>
        /// Returns the TLC59711 device at the requested position
        /// </summary>
        /// <param name="index">TLC59711 index</param>
        /// <returns>TLC59711 device</returns>
        public ITlc59711Device Get(int index) {
            return devices[index];
        }

        /// <summary>
        /// Set BLANK on/off at all connected devices.
        /// </summary>
        /// <param name="blank">If set to <c>true</c> all outputs are forced off.</param>
        public void Blank(bool blank) {
            foreach (var device in devices) {
                device.Blank = blank;
            }
        }
        #endregion

        #region Private Helpers
        private static IEnumerable<ITlc59711Device> CreateDevices(IMemory memory, int numberOfDevices) {
            for (var i = 0; i < numberOfDevices; i++) {
                var subset = new MemorySubset(memory, i * COMMAND_SIZE, COMMAND_SIZE, false);
                yield return new Tlc59711Device(subset);
            }
        }
        #endregion
    }
}