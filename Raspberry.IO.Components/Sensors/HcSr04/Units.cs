namespace Raspberry.IO.Components.Sensors.HcSr04
{
    internal static class Units
    {
        public static class Velocity
        {
            public static class Sound
            {
                public static decimal ToDistance(decimal time)
                {
                    if (time == decimal.MinValue)
                        return decimal.MinValue;

                    return (time * 340) / (2 * 1000m);
                }
            }
        }
    }
}