#region References

using System;
using Raspberry.IO.Components.Controllers.Pca9685;
using Raspberry.IO.GeneralPurpose;

#endregion

namespace Test.Gpio.PCA9685
{
    public class Pca9685Options
    {
        #region Properties

        public PwmChannel Channel { get; set; }

        public UnitsNet.Frequency PwmFrequency { get; set; }

        public int DeviceAddress { get; set; }

        public ConnectorPin SdaPin { get; set; }

        public ConnectorPin SclPin { get; set; }

        public int PwmOn { get; set; }

        public int PwmOff { get; set; }

        public bool ShowHelp { get; set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format(
                "Channel={1}{0}SdaPin={3}{0}SclPin={4}{0}DeviceAddress=0x{5:X}{0}PwmFrequency={2}{0}PwmOn={6}{0}PwmOff={7}{0}",
                Environment.NewLine,
                Channel,
                PwmFrequency,
                SdaPin,
                SclPin,
                DeviceAddress,
                PwmOn,
                PwmOff);
        }

        #endregion
    }
}