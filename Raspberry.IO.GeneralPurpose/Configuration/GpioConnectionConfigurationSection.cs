using System;
using System.Configuration;

namespace Raspberry.IO.GeneralPurpose.Configuration
{
    public class GpioConnectionConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("driver")]
        public string DriverTypeName
        {
            get { return (string)this["driver"]; }
            set { this["driver"] = value; }
        }
    }
}
