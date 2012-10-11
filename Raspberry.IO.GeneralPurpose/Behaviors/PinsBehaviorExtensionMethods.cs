namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public static class PinsBehaviorExtensionMethods
    {
        #region Methods

        public static void Start(this GpioConnection connection, PinsBehavior behavior)
        {
            foreach (var configuration in behavior.Configurations)
            {
                if (!connection.Contains(configuration))
                    connection.Add(configuration);
            }

            behavior.Start(connection);
        }

        public static void Stop(this GpioConnection connection, PinsBehavior behavior)
        {
            behavior.Stop();
        }

        #endregion
    }
}