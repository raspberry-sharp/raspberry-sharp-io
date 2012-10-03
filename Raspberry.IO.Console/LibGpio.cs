//-----------------------------------------------------------------------
// <copyright file="LibGpio.cs" company="Andrew Bradford">
//     Copyright (C) 2012 Andrew Bradford
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
//     associated documentation files (the "Software"), to deal in the Software without restriction, 
//     including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//     and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject 
//     to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//     WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//     COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//     ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

namespace PiSharp.LibGpio
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
 
    using Entities;
    
    /// <summary>
    /// Library for interfacing with the Raspberry Pi GPIO ports. 
    /// This class is implemented as a singleton - no attempt should be made to instantiate it
    /// </summary>
    public class LibGpio
    {   
        /// <summary>
        /// Stores the singleton instance of the class
        /// </summary>
        private static LibGpio instance;

        /// <summary>
        /// Stores the configured directions of the GPIO ports
        /// </summary>
        private Dictionary<BroadcomPinNumber, Direction> directions = new Dictionary<BroadcomPinNumber, Direction>();

        /// <summary>
        ///  Prevents a default instance of the LibGpio class from being created.
        /// </summary>
        private LibGpio()
        {
            var gpioPath = GetGpioPath();
            if (!Directory.Exists(gpioPath))
            {
                Directory.CreateDirectory(gpioPath);
            }
        }

        /// <summary>
        /// Gets the class instance 
        /// </summary>
        public static LibGpio Gpio
        {
            get
            {
                if (instance == null)
                    instance = new LibGpio();

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the library should be used in test mode.
        /// Note: If running under Windows or Mac OSx, test mode is assumed.
        /// </summary>
        public bool TestMode
        {
            get;
            set;
        }

        /// <summary>
        /// Configures a GPIO channel for use
        /// </summary>
        /// <param name="pinNumber">The Raspberry Pi pin number to configure</param>
        /// <param name="direction">The direction to configure the pin for</param>
        public void SetupChannel(RaspberryPinNumber pinNumber, Direction direction)
        {
            SetupChannel(ConvertToBroadcom(pinNumber), direction);
        }

        /// <summary>
        /// Configures a GPIO channel for use
        /// </summary>
        /// <param name="pinNumber">The physical pin number to configure</param>
        /// <param name="direction">The direction to configure the pin for</param>
        public void SetupChannel(PhysicalPinNumber pinNumber, Direction direction)
        {
            SetupChannel(ConvertToBroadcom(pinNumber), direction);
        }

        /// <summary>
        /// Configures a GPIO channel for use
        /// </summary>
        /// <param name="pinNumber">The Broadcom pin number to configure</param>
        /// <param name="direction">The direction to configure the pin for</param>
        public void SetupChannel(BroadcomPinNumber pinNumber, Direction direction)
        {
            var outputName = string.Format(CultureInfo.InvariantCulture,"gpio{0}", (int)pinNumber);
            var gpioPath = Path.Combine(GetGpioPath(), outputName);

            // If already exported, unexport it before continuing
            if (Directory.Exists(gpioPath))
                UnExport(pinNumber);

            // Now export the channel
            Export(pinNumber);

            // Set the IO direction
            SetDirection(pinNumber, direction);

            Console.WriteLine(string.Format("[PiSharp.LibGpio] Broadcom GPIO number '{0}', configured for use", pinNumber));
        }

        /// <summary>
        /// Outputs a value to a GPIO pin
        /// </summary>
        /// <param name="pinNumber">The pin number to use</param>
        /// <param name="value">The value to output</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an attempt to use an incorrectly configured channel is made
        /// </exception>
        public void OutputValue(RaspberryPinNumber pinNumber, bool value)
        {
            OutputValue(ConvertToBroadcom(pinNumber), value);
        }

        /// <summary>
        /// Outputs a value to a GPIO pin
        /// </summary>
        /// <param name="pinNumber">The pin number to use</param>
        /// <param name="value">The value to output</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an attempt to use an incorrectly configured channel is made
        /// </exception>
        public void OutputValue(PhysicalPinNumber pinNumber, bool value)
        {
            OutputValue(ConvertToBroadcom(pinNumber), value);
        }

        /// <summary>
        /// Outputs a value to a GPIO pin
        /// </summary>
        /// <param name="pinNumber">The pin number to use</param>
        /// <param name="value">The value to output</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an attempt to use an incorrectly configured channel is made
        /// </exception>
        public void OutputValue(BroadcomPinNumber pinNumber, bool value)
        {
            // Check that the output is configured
            if (!directions.ContainsKey(pinNumber))
                throw new InvalidOperationException("Attempt to output value on un-configured pin");

            // Check that the channel is not being used incorrectly
            if (directions[pinNumber] == Direction.Input)
                throw new InvalidOperationException("Attempt to output value on pin configured for input");

            var gpioId = string.Format("gpio{0}", (int)pinNumber);
            var filePath = Path.Combine(gpioId, "value");
            using (var streamWriter = new StreamWriter(Path.Combine(GetGpioPath(), filePath), false))
            {
                streamWriter.Write(value ? "1" : "0");
            }

            Console.WriteLine(string.Format("[PiSharp.LibGpio] Broadcom GPIO number '{0}', outputting value '{1}'", pinNumber, value));
        }

        /// <summary>
        /// Reads a value form a GPIO pin
        /// </summary>
        /// <param name="pinNumber">The pin number to read</param>
        /// <returns>The value at that pin</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an attempt to use an incorrectly configured channel is made
        /// </exception>
        public bool ReadValue(RaspberryPinNumber pinNumber)
        {
            return ReadValue(ConvertToBroadcom(pinNumber));
        }

        /// <summary>
        /// Reads a value form a GPIO pin
        /// </summary>
        /// <param name="pinNumber">The pin number to read</param>
        /// <returns>The value at that pin</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an attempt to use an incorrectly configured channel is made
        /// </exception>
        public bool ReadValue(PhysicalPinNumber pinNumber)
        {
            return ReadValue(ConvertToBroadcom(pinNumber));
        }

        /// <summary>
        /// Reads a value form a GPIO pin
        /// </summary>
        /// <param name="pinNumber">The pin number to read</param>
        /// <returns>The value at that pin</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an attempt to use an incorrectly configured channel is made
        /// </exception>
        public bool ReadValue(BroadcomPinNumber pinNumber)
        {
            // Check that the output is configured
            if (!directions.ContainsKey(pinNumber))
                throw new InvalidOperationException("Attempt to read value from un-configured pin");

            // Check that the channel is not being used incorrectly
            if (directions[pinNumber] == Direction.Output)
                throw new InvalidOperationException("Attempt to read value form pin configured for output");

            var gpioId = string.Format("gpio{0}", (int)pinNumber);
            var filePath = Path.Combine(gpioId, "value");
            using (var streamReader = new StreamReader(new FileStream(Path.Combine(GetGpioPath(), filePath), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var rawValue = streamReader.ReadToEnd();
                Console.WriteLine(string.Format("[PiSharp.LibGpio] Broadcom GPIO number '{0}', has input value '{1}'", pinNumber, rawValue));
                if (rawValue == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the Broadcom GPIO pin number form the Raspberry Pi pin number
        /// </summary>
        /// <param name="pinNumber">The Raspberry Pi pin number</param>
        /// <returns>The equivalent Broadcom ID</returns>
        private static BroadcomPinNumber ConvertToBroadcom(RaspberryPinNumber pinNumber)
        {
            switch (pinNumber)
            {
                case RaspberryPinNumber.Seven:
                    return BroadcomPinNumber.Four;

                case RaspberryPinNumber.Zero:
                    return BroadcomPinNumber.Seventeen;

                case RaspberryPinNumber.One:
                    return BroadcomPinNumber.Eighteen;

                case RaspberryPinNumber.Two:
                    return BroadcomPinNumber.TwentyOne;

                case RaspberryPinNumber.Three:
                    return BroadcomPinNumber.TwentyTwo;

                case RaspberryPinNumber.Four:
                    return BroadcomPinNumber.TwentyThree;

                case RaspberryPinNumber.Five:
                    return BroadcomPinNumber.TwentyFour;

                case RaspberryPinNumber.Six:
                    return BroadcomPinNumber.TwentyFive;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the Broadcom GPIO pin number form the physical pin number
        /// </summary>
        /// <param name="pinNumber">The physical pin number</param>
        /// <returns>The equivalent Broadcom ID</returns>
        private static BroadcomPinNumber ConvertToBroadcom(PhysicalPinNumber pinNumber)
        {
            switch (pinNumber)
            {
                case PhysicalPinNumber.Seven:
                    return BroadcomPinNumber.Four;

                case PhysicalPinNumber.Eleven:
                    return BroadcomPinNumber.Seventeen;

                case PhysicalPinNumber.Twelve:
                    return BroadcomPinNumber.Eighteen;

                case PhysicalPinNumber.Thirteen:
                    return BroadcomPinNumber.TwentyOne;

                case PhysicalPinNumber.Fifteen:
                    return BroadcomPinNumber.TwentyTwo;

                case PhysicalPinNumber.Sixteen:
                    return BroadcomPinNumber.TwentyThree;

                case PhysicalPinNumber.Eighteen:
                    return BroadcomPinNumber.TwentyFour;

                case PhysicalPinNumber.TwentyTwo:
                    return BroadcomPinNumber.TwentyFive;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Unexports a given GPIO pin number
        /// </summary>
        /// <param name="pinNumber">The pin number to unexport</param>
        private void UnExport(BroadcomPinNumber pinNumber)
        {
            using (var streamWriter = new StreamWriter(Path.Combine(GetGpioPath(), "unexport"), false))
            {
                streamWriter.Write((int)pinNumber);
            }

            Console.WriteLine(string.Format("[PiSharp.LibGpio] Broadcom GPIO number '{0}', un-exported", pinNumber));
        }

        /// <summary>
        /// Exports a given GPIO pin
        /// </summary>
        /// <param name="pinNumber">The pin number to export</param>
        private void Export(BroadcomPinNumber pinNumber)
        {
            using (var streamWriter = new StreamWriter(Path.Combine(GetGpioPath(), "export"), false))
                streamWriter.Write((int)pinNumber);

            Console.WriteLine(string.Format("[PiSharp.LibGpio] Broadcom GPIO number '{0}', been exported", pinNumber));
        }

        /// <summary>
        /// Sets the direction of a GPIO channel
        /// </summary>
        /// <param name="pinNumber">The pin number to set the direction of</param>
        /// <param name="direction">The direction to set it to</param>
        private void SetDirection(BroadcomPinNumber pinNumber, Direction direction)
        {
            var gpioId = string.Format("gpio{0}", (int)pinNumber);
            var filePath = Path.Combine(gpioId, "direction");
            using (var streamWriter = new StreamWriter(Path.Combine(GetGpioPath(), filePath), false))
                streamWriter.Write(direction == Direction.Input ? "in" : "out");

            // Internally log the direction of this pin, so we can perform safety checks later.
            if (directions.ContainsKey(pinNumber))
                directions[pinNumber] = direction;
            else
                directions.Add(pinNumber, direction);

            Console.WriteLine(string.Format("[PiSharp.LibGpio] Broadcom GPIO number '{0}', now set to direction '{1}'", pinNumber, direction));
        }

        /// <summary>
        /// Gets the GPIO path to write to
        /// </summary>
        /// <returns>The correct path for the IO and mode</returns>
        private string GetGpioPath()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows
                || Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.WinCE)
            {
                // If we're running under Windows, use a Windows format test path
                return "C:\\RasPiGpioTest";
            }
            else if (TestMode || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // If we're in Test mode or running on a Mac, use a Unix style test path
                return "/tmp/RasPiGpioTest";
            }
            else
            {
                // Otherwise use the the GPIO folder for the Raspberry Pi
                return "/sys/class/gpio";
            }
        }
    }
}
