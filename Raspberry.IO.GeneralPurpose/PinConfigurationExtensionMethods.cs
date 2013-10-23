#region References

using System;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides extension methods for pin configuration.
    /// </summary>
    public static class PinConfigurationExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Configures the specified pin as an input pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin configuration.</returns>
        public static InputPinConfiguration Input(this ProcessorPin pin)
        {
            return new InputPinConfiguration(pin);
        }

        /// <summary>
        /// Configures the specified pin as an output pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin configuration.</returns>
        public static OutputPinConfiguration Output(this ProcessorPin pin)
        {
            return new OutputPinConfiguration(pin);
        }

        /// <summary>
        /// Configures the specified pin as an input pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin configuration.</returns>
        public static InputPinConfiguration Input(this ConnectorPin pin)
        {
            return new InputPinConfiguration(pin.ToProcessor());
        }

        /// <summary>
        /// Configures the specified pin as an output pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin configuration.</returns>
        public static OutputPinConfiguration Output(this ConnectorPin pin)
        {
            return new OutputPinConfiguration(pin.ToProcessor());
        }

        /// <summary>
        /// Configures the specified input pin as a switch.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <returns>The pin configuration</returns>
        public static SwitchInputPinConfiguration Switch(this InputPinConfiguration pin)
        {
            return new SwitchInputPinConfiguration(pin);
        }

        /// <summary>
        /// Configures an action executed when the pin status changes.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The action.</param>
        /// <returns>The pin configuration.</returns>
        public static T OnStatusChanged<T>(this T configuration, Action<bool> action) where T : PinConfiguration
        {
            configuration.StatusChangedAction = action;
            return configuration;
        }

        /// <summary>
        /// Configures the name of the specified pin configuration.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <returns>The pin configuration.</returns>
        public static T Name<T>(this T configuration, string name) where T : PinConfiguration
        {
            configuration.Name = name;
            return configuration;
        }

        /// <summary>
        /// Reverts the bit value of the specified pin.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The pin configuration.</returns>
        public static T Revert<T>(this T configuration) where T : PinConfiguration
        {
            configuration.Reversed = !configuration.Reversed;
            return configuration;
        }

        /// <summary>
        /// Enables pull-up resistor.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The pin configuration.</returns>
        public static T PullUp<T>(this T configuration) where T : InputPinConfiguration
        {
            configuration.Resistor = PinResistor.PullUp;
            return configuration;
        }

        /// <summary>
        /// Enables pull-down resistor.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The pin configuration.</returns>
        public static T PullDown<T>(this T configuration) where T : InputPinConfiguration
        {
            configuration.Resistor = PinResistor.PullDown;
            return configuration;
        }

        /// <summary>
        /// Indicates the specified pin is enabled on connection.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The pin configuration.</returns>
        public static OutputPinConfiguration Enable(this OutputPinConfiguration configuration)
        {
            configuration.Enabled = true;
            return configuration;
        }

        /// <summary>
        /// Indicates the specified pin is disabled on connection.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The pin configuration.</returns>
        public static OutputPinConfiguration Disable(this OutputPinConfiguration configuration)
        {
            configuration.Enabled = false;
            return configuration;
        }

        /// <summary>
        /// Indicates the specified pin is enabled on connection.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The pin configuration.</returns>
        public static SwitchInputPinConfiguration Enable(this SwitchInputPinConfiguration configuration)
        {
            configuration.Enabled = true;
            return configuration;
        }

        /// <summary>
        /// Indicates the specified pin is disabled on connection.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The pin configuration.</returns>
        public static SwitchInputPinConfiguration Disable(this SwitchInputPinConfiguration configuration)
        {
            configuration.Enabled = false;
            return configuration;
        }

        #endregion
    }
}