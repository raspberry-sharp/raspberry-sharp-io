using System;
using UnitsNet;

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
                /// <param name="time">The time.</param>
                /// <returns>The distance travelled by sound in one second, in meters.</returns>
                public static Length ToDistance(TimeSpan time)
                {
                    if (time < TimeSpan.Zero)
                        return Length.FromMeters(double.MinValue);

                    return Length.FromMeters(time.TotalMilliseconds * 340 / 1000);
                }

                #endregion
            }
        }
    }
}