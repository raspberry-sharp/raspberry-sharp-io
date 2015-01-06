#region References

using System;
using Raspberry.IO.InterIntegratedCircuit;

#endregion

namespace Raspberry.IO.Components.Displays.Ssd1306
{
    /// <summary>
    /// Represents a connection with an Ssd1306 I2C OLED display.
    /// </summary>
    public class Ssd1306Connection
    {
        #region Fields

        private readonly I2cDeviceConnection connection;
        private int displayWidth = 128, displayHeight = 64;
        private int cursorX = 0, cursorY = 0;
        private object syncObject = new object();

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Ssd1306Connection"/> class at default width=128 and height=64.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Ssd1306Connection (I2cDeviceConnection connection) : this(connection, 128, 64) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ssd1306Connection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="width">The display width.</param>
        /// <param name="height">The display height.</param>
        public Ssd1306Connection(I2cDeviceConnection connection, int width, int height)
        {
            this.connection = connection;
            this.displayWidth = width;
            this.displayHeight = height;
            Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clears the screen.
        /// </summary>
        public void ClearScreen()
        {
            lock (syncObject)
            for (int y = 0; y < displayWidth * displayHeight / 8; y++)
            {
                connection.Write(new byte[] { 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            }
        }

        /// <summary>
        /// Inverts the display.
        /// </summary>
        public void InvertDisplay()
        {
            SendCommand(new byte[] {
                Command.DisplayInvert
            });
        }

        /// <summary>
        /// Sets the display to normal mode.
        /// </summary>
        public void NormalDisplay()
        {
            SendCommand(new byte[] {
                Command.DisplayNormal
            });
        }

        /// <summary>
        /// Turns the display on.
        /// </summary>
        public void On()
        {
            SendCommand(new byte[] {
                Command.DisplayOn
            });
        }

        /// <summary>
        /// Turns the display off.
        /// </summary>
        public void Off()
        {
            SendCommand(new byte[] {
                Command.DisplayOff
            });
        }

        /// <summary>
        /// Sets the current cursor position to <column>, <row>.
        /// </summary>
        /// <param name="column">Column.</param>
        /// <param name="row">Row.</param>
        public void GotoXY(int column, int row)
        {
            SendCommand(new byte[] {
                (byte)(0xB0 + row),							//set page address
                (byte)(0x00 + (8 * column & 0x0F)),			//set column lower address
                (byte)(0x10 + ((8 * column >> 4) & 0x0F))	//set column higher address
            });
            this.cursorX = column;
            this.cursorY = row;
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="font">Font.</param>
        /// <param name="text">Text.</param>
        public void DrawText(IFont font, string text)
        {
            var charset = font.GetData();
            for (int c = 0; c < text.Length; c++)
            {
                int charIndex = -1;
                for(int i = 0; i < charset.Length; i++)
                {
                    if (charset[i][0] == text[c])
                    {
                        charIndex = i;
                        break;
                    }
                }
                if (charIndex == -1) continue;
                //
                byte[] fontData = charset[charIndex];
                int fontWidth = fontData[1];
                int fontLength = fontData[2];
                for (int y = 0; y < (fontLength / fontWidth); y++)
                {
                    SendCommand(new byte[]{
                        (byte)(0xB0 + this.cursorY + y),    //set page address
                        (byte)(0x00 + (cursorX & 0x0F)),    //set column lower address
                        (byte)(0x10 + ((cursorX>>4)&0x0F))  //set column higher address
                    });      
                    var data = new byte[fontWidth + 1];
                    data[0] = 0x40;
                    Array.Copy(fontData, (y * fontWidth) + 3, data, 1, fontWidth);
                    DrawStride(data);
                }
                this.cursorX += fontWidth;
            }
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="image">Image.</param>
        public void DrawImage(byte[] image)
        {
            var data = new byte[image.Length + 1];
            data [0] = 0x40;
            Array.Copy(image, 0, data, 1, image.Length);
            DrawStride(data);
        }

        /// <summary>
        /// Activates the scroll.
        /// </summary>
        public void ActivateScroll()
        {
            SendCommand(new byte[] {
                Command.ActivateScroll
            });
        }

        /// <summary>
        /// Deactivates the scroll.
        /// </summary>
        public void DeactivateScroll()
        {
            SendCommand(new byte[] {
                Command.DeactivateScroll
            });
        }

        /// <summary>
        /// Sets the scroll properties.
        /// </summary>
        /// <param name="direction">Direction.</param>
        /// <param name="scrollSpeed">Scroll speed.</param>
        /// <param name="startLine">Start line.</param>
        /// <param name="endLine">End line.</param>
        public void SetScrollProperties(ScrollDirection direction, ScrollSpeed scrollSpeed, int startLine, int endLine)
        {
            SendCommand(new byte[] {
                (byte)(Command.SetScrollDirection | (byte)direction),
                0x00,
                (byte)startLine,
                (byte)scrollSpeed,
                (byte)endLine,
                0x00,
                0xFF
            });
        }

        #endregion

        #region Private Helpers

        private void SendCommand(byte[] commands)
        {
            lock (syncObject)
            for (int c = 0; c < commands.Length; c++)
            {
                connection.Write(new byte[] { 0x00, commands[c] });
            }
        }

        private void Initialize()
        {
            SendCommand(new byte[] {
                Command.DisplayOff,
                Command.SetDisplayClockDivider, 0x80,
                Command.SetMultiplex, 0x3F,
                Command.SetDisplayOffset, 0x00,
                Command.SetStartLine | 0x0,
                Command.ChargePump, 0x14,
                Command.MemoryMode, 0x00,
                Command.SegRemap | 0x1,
                Command.ComScanDecrement,
                Command.SetComPins, 0x12,
                Command.SetContrast, 0x9F,
                Command.SetPreCharge, 0x22,
                Command.SetVComDetect, 0x40,
                Command.DisplayAllOnResume,
                Command.DisplayNormal
            });
            SendCommand(new byte[] {
                Command.ColumnAddress, 0, (byte)(displayWidth - 1),
                Command.PageAddress, 0, (byte)((displayHeight / 8) - 1)
            });
            ClearScreen();
        }

        private void DrawStride(byte[] data)
        {
            lock(syncObject)
            connection.Write(data);
        }

        #endregion
    }
}

