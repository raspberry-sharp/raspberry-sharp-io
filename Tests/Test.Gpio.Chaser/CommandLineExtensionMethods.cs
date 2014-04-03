using System;
using System.Collections.Generic;
using System.Linq;
using Raspberry.IO.GeneralPurpose;

namespace Test.Gpio.Chaser
{
    internal static class CommandLineExtensionMethods
    {
        public static bool GetLoop(this IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-loop").Any();
        }

        public static bool GetRoundTrip(this IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-roundTrip").Any();
        }

        public static int GetWidth(this IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-width").Skip(1).Select(int.Parse).DefaultIfEmpty(1).First();
        }

        public static int GetSpeed(this IEnumerable<string> args)
        {
            return args.SkipWhile(a => a != "-speed").Skip(1).Select(int.Parse).DefaultIfEmpty(250).First();
        }

        public static IGpioConnectionDriver GetDriver(this IEnumerable<string> args)
        {
            var driverName = args.SkipWhile(a => a != "-driver").Skip(1).DefaultIfEmpty("").First();

            switch (driverName)
            {
                case "default":
                    return new GpioConnectionDriver();
                case "memory":
                    return new MemoryGpioConnectionDriver();
                case "file":
                    return new FileGpioConnectionDriver();
                case "":
                    return null;

                default:
                    throw new InvalidOperationException("Unsupported driver");
            }
        }
    }
}