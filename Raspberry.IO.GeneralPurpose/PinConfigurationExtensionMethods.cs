namespace Raspberry.IO.GeneralPurpose
{
    public static class PinConfigurationExtensionMethods
    {
        public static PinConfiguration Input(this ProcessorPin pin)
        {
            return new InputPinConfiguration(pin);
        }

        public static SwitchInputPinConfiguration Switch(this InputPinConfiguration pin)
        {
            return new SwitchInputPinConfiguration(pin);
        }

        public static PinConfiguration Output(this ProcessorPin pin)
        {
            return new OutputPinConfiguration(pin);
        }

        public static InputPinConfiguration Input(this ConnectorPin pin)
        {
            return new InputPinConfiguration(pin.ToProcessor());
        }

        public static OutputPinConfiguration Output(this ConnectorPin pin)
        {
            return new OutputPinConfiguration(pin.ToProcessor());
        }

        public static T Name<T>(this T configuration, string name) where T : PinConfiguration
        {
            configuration.Name = name;
            return configuration;
        }

        public static T Revert<T>(this T configuration) where T : PinConfiguration
        {
            configuration.Reversed = !configuration.Reversed;
            return configuration;
        }

        public static OutputPinConfiguration Enable(this OutputPinConfiguration configuration)
        {
            configuration.Enabled = true;
            return configuration;
        }

        public static OutputPinConfiguration Disable(this OutputPinConfiguration configuration)
        {
            configuration.Enabled = false;
            return configuration;
        }

        public static SwitchInputPinConfiguration Enable(this SwitchInputPinConfiguration configuration)
        {
            configuration.Enabled = true;
            return configuration;
        }

        public static SwitchInputPinConfiguration Disable(this SwitchInputPinConfiguration configuration)
        {
            configuration.Enabled = false;
            return configuration;
        }
    }
}