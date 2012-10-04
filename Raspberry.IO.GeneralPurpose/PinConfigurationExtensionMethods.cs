namespace Raspberry.IO.GeneralPurpose
{
    public static class PinConfigurationExtensionMethods
    {
        public static PinConfiguration Input(this ProcessorPin pin)
        {
            return new InputPinConfiguration(pin);
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

        public static T Named<T>(this T configuration, string name) where T : PinConfiguration
        {
            configuration.Name = name;
            return configuration;
        }

        public static T Reversed<T>(this T configuration) where T : PinConfiguration
        {
            configuration.IsReversed = !configuration.IsReversed;
            return configuration;
        }

        public static OutputPinConfiguration Active(this OutputPinConfiguration configuration)
        {
            configuration.IsActive = true;
            return configuration;
        }

        public static OutputPinConfiguration Inactive(this OutputPinConfiguration configuration)
        {
            configuration.IsActive = false;
            return configuration;
        }
    }
}