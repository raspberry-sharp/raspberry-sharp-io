using System;
using NDesk.Options;
using Raspberry.IO.Components.Controllers.Pca9685;
using Raspberry.IO.GeneralPurpose;

namespace Test.Gpio.Pca9685
{
    public class Pca9685Options
    {
        public PwmChannel Channel { get; set; }

        public int PwmFrequency { get; set; }

        public int DeviceAddress { get; set; }

        public ConnectorPin SdaPin = ConnectorPin.P1Pin03;

        public ConnectorPin SclPin = ConnectorPin.P1Pin05;

        public int PwmOn { get; set; }

        public int PwmOff { get; set; }

        public bool ShowHelp { get; set; }

        public Pca9685Options(string[] args)
        {
            PwmFrequency = 60;
            SdaPin = ConnectorPin.P1Pin03;
            SclPin = ConnectorPin.P1Pin05;
            DeviceAddress = 0x40;
            PwmOn = 150;
            PwmOff = 600;

            var p = new OptionSet {
                { "c|Channel=",  v => Channel =(PwmChannel) Enum.Parse(typeof(PwmChannel), v)},
                { "f|PwmFrequency=",  v => PwmFrequency = int.Parse(v) },
                { "b|PwmOn=",  v => PwmOn = int.Parse(v) },
                { "e|PwmOff=",  v => PwmOff = int.Parse(v) },
                { "h|?:", v => ShowHelp = true }
            };
            p.Parse(args);

            if (ShowHelp)
            {
                Console.WriteLine("Options:");
                p.WriteOptionDescriptions(Console.Out);
            }
        }

        public override string ToString()
        {
            return string.Format(
                "Channel={1}{0}SdaPin={3}{0}SclPin={4}{0}DeviceAddress=0x{5:X}{0}PwmFrequency={2}{0}PwmOn={6}{0}PwmOff={7}{0}"
                , Environment.NewLine
                , Channel
                , PwmFrequency
                , SdaPin
                , SclPin
                , DeviceAddress
                , PwmOn
                , PwmOff);
        }

    }
}