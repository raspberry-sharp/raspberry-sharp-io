namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    /// <summary>
    /// Provides extension methods for <see cref="PinsBehavior"/>.
    /// </summary>
    public static class PinsBehaviorExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Starts the specified behavior on the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="behavior">The behavior.</param>
        public static void Start(this GpioConnection connection, PinsBehavior behavior)
        {
            foreach (var configuration in behavior.Configurations)
            {
                if (!connection.Contains(configuration))
                    connection.Add(configuration);
            }

            behavior.Start(connection);
        }

        /// <summary>
        /// Stops the specified behavior.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="behavior">The behavior.</param>
        public static void Stop(this GpioConnection connection, PinsBehavior behavior)
        {
            behavior.Stop();
        }

        #endregion
    }
}