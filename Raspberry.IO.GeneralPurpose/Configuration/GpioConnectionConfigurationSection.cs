#region References

using System.Configuration;

#endregion

namespace Raspberry.IO.GeneralPurpose.Configuration
{
    public class GpioConnectionConfigurationSection : ConfigurationSection
    {
        #region Properties

        [ConfigurationProperty("driver")]
        public string DriverTypeName
        {
            get { return (string) this["driver"]; }
            set { this["driver"] = value; }
        }

        #endregion
    }
}
