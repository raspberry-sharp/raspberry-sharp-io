#region References

using System;
using Raspberry.Timers;
using System.Text;

#endregion

namespace Raspberry.IO.Components.Leds.GroveBar
{
    /// <summary>
    /// Represents a connection with Grove Led Bar module.
    /// @see http://www.seeedstudio.com/wiki/Grove_-_LED_Bar
    /// </summary>
    public class GroveBarConnection : IDisposable
    {
        #region Fields

        private const uint CommandMode = 0x0000;
        private static readonly TimeSpan delay = TimeSpan.FromTicks(1);

        private readonly IOutputBinaryPin dataPin;
        private readonly IInputOutputBinaryPin clockPin;
        private string currentLedsStatus = "0000000000";

        #endregion

        #region Instance Management

        public GroveBarConnection(IOutputBinaryPin dataPin, IInputOutputBinaryPin clockPin)
        {
            this.dataPin = dataPin;
            this.clockPin = clockPin;
            Initialize();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
                
        #region Methods

        /// <summary>
        /// Sets status of leds from a binary string eg: "0010101011", where "0" is off and "1" is on
        /// </summary>
        /// <param name="ledsString">Leds string.</param>
        public void SetFromString(string ledsString)
        {
            currentLedsStatus = ledsString;
            SendData(CommandMode);
            var indexBits = (uint)Convert.ToInt32(ledsString, 2);
            for (int i = 0; i < 12; i++)
            {
                var state = (uint)((indexBits & 0x0001) > 0 ? 0x00FF : 0x0000);
                SendData(state);
                indexBits = indexBits >> 1;
            }
            LatchData();
        }

        /// <summary>
        /// Sets the level of the leds bar.
        /// </summary>
        /// <param name="level">Level.</param>
        public void SetLevel(int level)
        {
            var status = new StringBuilder(new string('0', 10));
            for (int i = 0; i < level; i++)
            {
                status[i] = '1';
            }
            currentLedsStatus = status.ToString();
            SendData(CommandMode);
            for (int i = 0; i < 12; i++)
            {
                var state = (uint)((i < level) ? 0x00FF : 0x0000);
                SendData(state);
            }
            LatchData();
        }

        /// <summary>
        /// Turn on a single led at a given position (0-9)
        /// </summary>
        /// <param name="position">Position.</param>
        public void On(int position)
        {
            var status = new StringBuilder(currentLedsStatus);
            status[position] = '1';
            currentLedsStatus = status.ToString();
            SetFromString(currentLedsStatus);
        }
        
        /// <summary>
        /// Turn off a single led at a given position (0-9)
        /// </summary>
        /// <param name="position">Position.</param>
        public void Off(int position)
        {
            var status = new StringBuilder(currentLedsStatus);
            status[position] = '0';
            currentLedsStatus = status.ToString();
            SetFromString(currentLedsStatus);
        }

        /// <summary>
        /// Turn all leds on.
        /// </summary>
        public void AllOn()
        {
            currentLedsStatus = new string('1', 10);
            SetFromString(currentLedsStatus);
        }
        
        /// <summary>
        /// Turn all leds off.
        /// </summary>
        public void AllOff()
        {
            currentLedsStatus = new string('0', 10);
            SetFromString(currentLedsStatus);
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            dataPin.Dispose();
            clockPin.Dispose();
        }

        #endregion

        #region Private Helpers

        private void Initialize()
        {
            dataPin.Write(false);
            HighResolutionTimer.Sleep(delay);
            for(int i = 0; i < 4; i++)
            {
                dataPin.Write(true);
                dataPin.Write(false);
            }

        }

        private void SendData(uint data)
        {
            // Send 16 bit data
            for(int i = 0; i < 16; i++)
            {
                bool state = ((data & 0x8000) > 0);
                dataPin.Write(state);
                state = !clockPin.Read();
                clockPin.Write(state);
                data <<= 1;
            }
        }

        private void LatchData()
        {
            dataPin.Write(false);
            HighResolutionTimer.Sleep(delay);
            for(int i = 0; i < 4; i++)
            {
                dataPin.Write(true);
                dataPin.Write(false);
            }
        }

        #endregion
    }
}

