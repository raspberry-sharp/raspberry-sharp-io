namespace Raspberry.IO.GeneralPurpose.Behaviors
{
    public static class PinsBehaviorExtensionMethods
    {
        public static void Start(this Connection connection, PinsBehavior behavior)
        {
            behavior.Start(connection);
        }

        public static void Stop(this Connection connection, PinsBehavior behavior)
        {
            behavior.Stop();
        }
    }
}