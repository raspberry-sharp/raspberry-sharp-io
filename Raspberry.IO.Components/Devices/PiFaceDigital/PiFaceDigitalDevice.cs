using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raspberry.IO.SerialPeripheralInterface;

namespace Raspberry.IO.Components.Devices.PiFaceDigital
{
    /// <summary>
    /// Controls a PiFace Digital device on a Raspberry Pi.
    /// Pins on the board have software counterparts to control or read state of the physical pins.
    /// Polling is required for the input pin values as the PiFace Digital has no interrupt support.
    /// 
    /// This driver uses a NativeSpiConnection which requires the native SPI driver be enabled
    /// </summary>
    public class PiFaceDigitalDevice : IDisposable 
    {

        #region Constants

        private const byte IOCON_SEQOP = 0x00;
        private const byte All_Pins_Input = 0xFF;
        private const byte All_Pins_Output = 0x00;
        private const byte CMD_WRITE = 0x40;
        private const byte CMD_READ = 0x41;

        #endregion

        #region Enums

        /// <summary>
        /// Registers on the MCP23S17 chip
        /// </summary>
        internal enum mcp23s17Register
        {
            IODIRA = 0x00,
            IODIRB = 0x01,
            IOCON = 0x0A,
            GPPUB = 0x0D,
            GPIOA = 0x12,
            GPIOB = 0x13
        };

        #endregion

        #region Fields
 
        private string driverName;
        
        private INativeSpiConnection spiConnection = null;
         
        /// <summary>
        /// Re-usable buffer for reading input pins state to reduce the polling overhead
        /// </summary>
        private ISpiTransferBuffer inputPollBuffer;

        /// <summary>
        /// Last known state of the inputs, used to optimize detecting changes
        /// </summary>
        private byte CachedInputState;

        bool disposed = false;
        
        #endregion

        #region Instance Management

        /// <summary>
        ///  Create the device with a custom driver name
        /// </summary>
        /// <param name="driverName">name of the device driver e.g. /dev/...</param>
        public PiFaceDigitalDevice(string driverName = "/dev/spidev0.0")
        {
            this.driverName = driverName;
            InitPiFace();
        }

        /// <summary>
        ///  Create the device injecting an SPI connection
        /// </summary>
        /// <param name="nativeSpiConnection">connection to the SPI driver.</param>
        public PiFaceDigitalDevice(INativeSpiConnection nativeSpiConnection)
        {
            spiConnection = nativeSpiConnection;
            InitPiFace();
        }


        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }


        // Protected implementation of Dispose pattern. 
        protected void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                spiConnection.Dispose();
                inputPollBuffer.Dispose();
            }

            disposed = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Software proxy for the input pins of the PiFace Digital Board
        /// </summary>
        public PiFaceInputPin[] InputPins { get; private set; }

        /// <summary>
        /// Software proxy for the output pins of the PiFace Digital Board
        /// </summary>
        public PiFaceOutputPin[] OutputPins { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Set up the MCP23S17 for the PiFace Digital board
        /// </summary>
        private void InitPiFace()
        {
            InputPins = new PiFaceInputPin[8];
            for (int pinNo = 0; pinNo < 8; pinNo++)
            {
                InputPins[pinNo] = new PiFaceInputPin(pinNo);
            }

            OutputPins = new PiFaceOutputPin[8];
            for (int pinNo = 0; pinNo < 8; pinNo++)
            {
                OutputPins[pinNo] = new PiFaceOutputPin(pinNo);
            }

            if (spiConnection == null)
            {
                SpiConnectionSettings spiSettings = new SpiConnectionSettings { BitsPerWord = 8, Delay = 1, MaxSpeed = 5000000, Mode = SpiMode.Mode0 };
                spiConnection = new NativeSpiConnection(driverName, spiSettings);
            }

            Write(mcp23s17Register.IOCON, IOCON_SEQOP);
            SetAllOutputPins(0);

            // initialize output and input pins
            Write(mcp23s17Register.IODIRA, All_Pins_Output);
            Write(mcp23s17Register.IODIRB, All_Pins_Input);

            // set resistor on all input pins to pull up
            Write(mcp23s17Register.GPPUB, 0xFF);

            // set outputs
            UpdatePiFaceOutputPins();

            // Create re-usable buffer for polling input pins
            CreateReusableBufferForInputPolling();

            // Get the initial software input pin state and compare to actual inputs
            CachedInputState = PiFaceInputPin.AllPinState(InputPins);
            PollInputPins();
        }

        /// <summary>
        /// Sets up the transfer buffer for reading input pins;
        /// </summary>
        private void CreateReusableBufferForInputPolling()
        {
            inputPollBuffer = spiConnection.CreateTransferBuffer(3, SpiTransferMode.ReadWrite);
            inputPollBuffer.Tx[0] = CMD_READ;
            inputPollBuffer.Tx[1] = (byte)mcp23s17Register.GPIOB;
            inputPollBuffer.Tx[2] = 0;
        }

        private int Write(mcp23s17Register port, byte data)
        {
            ISpiTransferBuffer transferBuffer = spiConnection.CreateTransferBuffer(3, SpiTransferMode.Write);
            transferBuffer.Tx[0] = CMD_WRITE;
            transferBuffer.Tx[1] = (byte)port;
            transferBuffer.Tx[2] = data;
            var result = spiConnection.Transfer(transferBuffer);
            return result;
        }

        internal byte Read(mcp23s17Register port)
        {
            ISpiTransferBuffer transferBuffer = spiConnection.CreateTransferBuffer(3, SpiTransferMode.ReadWrite);
            transferBuffer.Tx[0] = CMD_READ;
            transferBuffer.Tx[1] = (byte)port;
            transferBuffer.Tx[2] = 0;
            var result = spiConnection.Transfer(transferBuffer);
            return transferBuffer.Rx[2];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configure the output pins with a byte that has one bit per pin and set the pins on the PiFaceDigital
        /// </summary>
        /// <param name="allPinsState"></param>
        public void SetAllOutputPins(byte allPinsState)
        {
            foreach (var oPin in OutputPins)
            {
                oPin.Update(allPinsState);
            }
            Write(mcp23s17Register.GPIOA, allPinsState);
        }

        /// <summary>
        /// Update PiFace board with the current vales of the software output pins
        /// </summary>
        public void UpdatePiFaceOutputPins()
        {
            Write(mcp23s17Register.GPIOA, PiFaceOutputPin.AllPinState(OutputPins));
        }

        /// <summary>
        /// Read the state of the input pins. Will trigger any Onchanged events registered
        /// </summary>
        public void PollInputPins()
        {
            var result = spiConnection.Transfer(inputPollBuffer);
            var state = inputPollBuffer.Rx[2];
            if (state != CachedInputState)
            {
                CachedInputState = state;
                PiFaceInputPin.SetAllPinStates(InputPins, state);
            }
        }

        #endregion
    }
}
