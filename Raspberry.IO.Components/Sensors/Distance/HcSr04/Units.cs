namespace Raspberry.IO.Components.Sensors.Distance.HcSr04
{
    internal static class Units
    {
        /// <summary>
        /// Velocity related conversions
        /// </summary>
        public static class Velocity
        {
            /// <summary>
            /// Sound velocity related conversions
            /// </summary>
            public static class Sound
            {
                #region Methods

                /// <summary>
                /// Converts a time to a distance.
                /// </summary>
                /// <param name="time">The time, in milliseconds.</param>
                /// <returns>The distance travelled by sound in one second, in meters.</returns>
                public static decimal ToDistance(decimal time)
                {
                    if (time == decimal.MinValue)
                        return decimal.MinValue;

                    return (time*340)/1000m;
                }

                #endregion
            }
        }
    }
}