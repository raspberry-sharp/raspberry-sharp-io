#region References

using System;
using Raspberry.IO.InterIntegratedCircuit;
using Raspberry.IO.Components.Displays.Ssd1306.Fonts;

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
        private readonly int displayWidth;
        private readonly int displayHeight;
        private int cursorX;
        private int cursorY;

        private readonly object syncObject = new object();

        #endregion

        #region Instance Management

        /// <summary>
        /// Initializes a new instance of the <see cref="Ssd1306Connection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="displayWidth">The display displayWidth.</param>
        /// <param name="displayHeight">The display displayHeight.</param>
        public Ssd1306Connection(I2cDeviceConnection connection, int displayWidth = 128, int displayHeight = 64)
        {
            this.connection = connection;
            this.displayWidth = displayWidth;
            this.displayHeight = displayHeight;
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
            {
                for (var y = 0; y < displayWidth * displayHeight / 8; y++)
                    connection.Write(0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
            }
        }

        /// <summary>
        /// Inverts the display.
        /// </summary>
        public void InvertDisplay()
        {
            SendCommand(Command.DisplayInvert);
        }

        /// <summary>
        /// Sets the display to normal mode.
        /// </summary>
        public void NormalDisplay()
        {
            SendCommand(Command.DisplayNormal);
        }

        /// <summary>
        /// Turns the display on.
        /// </summary>
        public void On()
        {
            SendCommand(Command.DisplayOn);
        }

        /// <summary>
        /// Turns the display off.
        /// </summary>
        public void Off()
        {
            SendCommand(Command.DisplayOff);
        }

        /// <summary>
        /// Sets the current cursor position to the specified column and row.
        /// </summary>
        /// <param name="column">Column.</param>
        /// <param name="row">Row.</param>
        public void GotoXY(int column, int row)
        {
            SendCommand(
                (byte)(0xB0 + row),							//set page address
                (byte)(0x00 + (8 * column & 0x0F)),			//set column lower address
                (byte)(0x10 + ((8 * column >> 4) & 0x0F))	//set column higher address
            );
            
            cursorX = column;
            cursorY = row;
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="font">Font.</param>
        /// <param name="text">Text.</param>
        public void DrawText(IFont font, string text)
        {
            var charset = font.GetData();
            foreach (var character in text)
            {
                var charIndex = -1;
                for(var i = 0; i < charset.Length; i++)
                {
                    if (charset[i][0] == character)
                    {
                        charIndex = i;
                        break;
                    }
                }
                if (charIndex == -1)
                    continue;

                var fontData = charset[charIndex];
                int fontWidth = fontData[1];
                int fontLength = fontData[2];
                for (var y = 0; y < (fontLength / fontWidth); y++)
                {
                    SendCommand(
                        (byte)(0xB0 + cursorY + y),    //set page address
                        (byte)(0x00 + (cursorX & 0x0F)),    //set column lower address
                        (byte)(0x10 + ((cursorX>>4) & 0x0F))  //set column higher address
                    );      

                    var data = new byte[fontWidth + 1];
                    data[0] = 0x40;
                    Array.Copy(fontData, (y * fontWidth) + 3, data, 1, fontWidth);
                    DrawStride(data);
                }
                
                cursorX += fontWidth;
            }
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="image">Image.</param>
        public void DrawImage(byte[] image)
        {
            var data = new byte[image.Length + 1];
            data[0] = 0x40;
            Array.Copy(image, 0, data, 1, image.Length);

            DrawStride(data);
        }

        /// <summary>
        /// Activates the scroll.
        /// </summary>
        public void ActivateScroll()
        {
            SendCommand(Command.ActivateScroll);
        }

        /// <summary>
        /// Deactivates the scroll.
        /// </summary>
        public void DeactivateScroll()
        {
            SendCommand(Command.DeactivateScroll);
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

        /// <summary>
        /// Sets the contrast (brightness) of the display. Default is 127.
        /// </summary>
        /// <param name="contrast">A number between 0 and 255. Contrast increases as the value increases.</param>
        public void SetContrast(int contrast)
        {
            if (contrast < 0 || contrast > 255) throw new ArgumentOutOfRangeException("contrast", "Contrast must be between 0 and 255.");
            SendCommand(Command.SetContrast, (byte)contrast);
        }

        #endregion

        #region Private Helpers

        private void SendCommand(params byte[] commands)
        {
            lock (syncObject)
            {
                foreach (byte command in commands)
                    connection.Write(0x00, command);
            }
        }

        private void Initialize()
        {
            SendCommand(
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
                Command.SetContrast, 0x7F,
                Command.SetPreCharge, 0x22,
                Command.SetVComDetect, 0x40,
                Command.DisplayAllOnResume,
                Command.DisplayNormal
            );

            SendCommand(
                Command.ColumnAddress, 0, (byte)(displayWidth - 1),
                Command.PageAddress, 0, (byte)((displayHeight / 8) - 1)
            );

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

