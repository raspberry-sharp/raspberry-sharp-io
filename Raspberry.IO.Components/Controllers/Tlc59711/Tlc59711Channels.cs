#region References

using System;
using Raspberry.IO.Interop;

#endregion

namespace Raspberry.IO.Components.Controllers.Tlc59711
{
    /// <summary>
    /// The PWM channels of a TLC59711
    /// </summary>
    internal sealed class Tlc59711Channels : IPwmChannels
    {
        #region Constants
        private const int NUMBER_OF_CHANNELS = 12;
        #endregion

        #region Fields
        private readonly IMemory memory;
        private readonly int channelOffset;
        #endregion

        #region Instance Management
        /// <summary>
        /// Creates a new instance of the <see cref="Tlc59711Channels"/> class.
        /// </summary>
        /// <param name="memory">Memory to work with</param>
        /// <param name="offset">Byte offset to the first channel index</param>
        public Tlc59711Channels(IMemory memory, int offset) {
            this.memory = memory;
            channelOffset = offset;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the number of channels.
        /// </summary>
        public int Count {
            get { return NUMBER_OF_CHANNELS; }
        }
        /// <summary>
        /// Indexer, which will allow client code to use [] notation on the class instance itself to modify PWM channel values.
        /// </summary>
        /// <param name="index">channel index</param>
        /// <returns>The current PWM value from <paramref name="index"/></returns>
        public UInt16 this[int index] {
            get { return Get(index); }
            set { Set(index, value); }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Returns the PWM value at the specified channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index</param>
        /// <returns>The PWM value at the specified channel <paramref name="index"/></returns>
        public UInt16 Get(int index) {
            ThrowOnInvalidChannelIndex(index);
            
            var offset = channelOffset + (index * 2);
            
            var high = memory.Read(offset);
            var low = memory.Read(offset + 1);

            return unchecked((UInt16)((high << 8) | low));
        }

        /// <summary>
        /// Sets the PWM value at channel <paramref name="index"/>.
        /// </summary>
        /// <param name="index">Channel index</param>
        /// <param name="value">The PWM value</param>
        public void Set(int index, UInt16 value) {
            ThrowOnInvalidChannelIndex(index);
            
            var offset = channelOffset + (index * 2);

            memory.Write(offset, unchecked((byte)(value >> 8)));
            memory.Write(offset + 1, unchecked((byte)value));
        }
        #endregion

        #region Private Helpers

        private static void ThrowOnInvalidChannelIndex(int index) {
            if (index >= 0 && index < NUMBER_OF_CHANNELS) {
                return;
            }

            var message = string.Format("The index must be greater or equal than 0 and lower than {0}.", NUMBER_OF_CHANNELS);
            throw new ArgumentOutOfRangeException("index", index, message);
        }

        #endregion
    }
}