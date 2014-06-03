using System;
using System.Linq;
using Raspberry.IO.Components.Controllers.Tlc59711;
using Raspberry.IO.SerialPeripheralInterface;

namespace Test.Spi.TLC59711
{
    internal class Program
    {
        private const string DEVICE_FILEPATH = "/dev/spidev0.0";
        private const bool INITIALIZE_WITH_DEFAULT = true;

        private static int numberOfDevices = 1;
        private static ushort maxBrightness = (UInt16)Math.Pow(2, 13);

        private static void Main() {
            PrintIntroduction();
            AskUserForParameters();
            
            using(var deviceConnection = new Tlc59711Connection(new NativeSpiConnection(DEVICE_FILEPATH), INITIALIZE_WITH_DEFAULT, numberOfDevices)) {
                var devices = deviceConnection.Devices;
                var channels = devices.Channels;

                devices.Blank(false);
                deviceConnection.Update();

                var numberOfChannels = channels.Count;

                var ledBrightness = new ushort[numberOfChannels];

                // initialize all PWM channels from 0% (channel 0) to 100% (channel n)
                var frac = maxBrightness / (numberOfChannels - 1);
                for (var i = 1; i < numberOfChannels; i++) {
                    ledBrightness[i] = (UInt16) (frac * i);
                }
                
                var fadeDirection = Enumerable.Range(1, numberOfChannels)
                    .Select(channel => 1)
                    .ToArray();

                // Fade in/out loop
                while (true) {
                    for (var i = 0; i < numberOfChannels; i++) {
                        if (ledBrightness[i] == maxBrightness) {
                            fadeDirection[i] = -1;
                        }
                        if (ledBrightness[i] == 0) {
                            fadeDirection[i] = 1;
                        }
                        
                        channels[i] = ledBrightness[i];
                        ledBrightness[i] = (ushort) (ledBrightness[i] + fadeDirection[i]);
                    }
                    deviceConnection.Update();
                }
            }
        }

        private static void AskUserForParameters() {
            Console.Clear();
            Console.Write(
@"The program will use {0}.

How many devices are connected?
The maximum number is limited by your system configuration (Default: 1): ", DEVICE_FILEPATH);

            var numberOfDevicesLine = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(numberOfDevicesLine)) {
                numberOfDevices = (int) uint.Parse(numberOfDevicesLine);
            }
            Console.Write(
@"
Please enter the maximum brightness level
(min: {0}, max: {1}, default: {2}): ", ushort.MinValue, ushort.MaxValue, maxBrightness);
            var maxBrightnessLine = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(maxBrightnessLine)) {
                maxBrightness = ushort.Parse(maxBrightnessLine);
            }

            Console.WriteLine("Press CTRL-C to end.");
        }

        private static void PrintIntroduction() {
            Console.WriteLine(
@"The program will fade in/out all TLC59711 channels using the system's SPI 
driver. Make sure that you have enabled the spi-bcm2708 kernel module. 

In Raspbian, comment out the following line in the file
/etc/modprobe.d/raspi-blacklist.conf:

#blacklist spi-bcm2708

Make sure that your (first) TLC59711 is connected as follows:
MOSI (pin 19) -> DI
SCLK (pin 23) -> CI

Press CTRL-C to abort or any key to continue.
");

            Console.ReadKey();
        }
    }
}

