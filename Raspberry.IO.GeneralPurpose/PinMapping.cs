#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Diagnostics;

#endregion

namespace Raspberry.IO.GeneralPurpose
{
    /// <summary>
    /// Provides helper methods for mapping pins between processor and connectors
    /// </summary>
    public static class PinMapping
    {
        #region Fields

        private static readonly Dictionary<ProcessorPin, ConnectorPin> connectorMappings;
        private static readonly Dictionary<ConnectorPin, ProcessorPin> processorMappings;

        #endregion

        #region Instance Management

        static PinMapping()
        {
            var mapping = /* Value is not used but required for anonymous type */ new[]{ new
            {
                Processor = ProcessorPin.Pin0, Connector = ConnectorPin.P1Pin03
            }};

            var uname = GetUname();
            if (uname.ToLower().Contains("cubie"))
            {
                mapping = new[] {
                    new {Processor = ProcessorPin.Pin3, Connector = ConnectorPin.CB3_CN8Pin5},
                    new {Processor = ProcessorPin.Pin4, Connector = ConnectorPin.CB3_CN8Pin6},
                    new {Processor = ProcessorPin.Pin5, Connector = ConnectorPin.CB3_CN8Pin7},
                    new {Processor = ProcessorPin.Pin6, Connector = ConnectorPin.CB3_CN8Pin8},
                    new {Processor = ProcessorPin.Pin7, Connector = ConnectorPin.CB3_CN8Pin9},
                    new {Processor = ProcessorPin.Pin8, Connector = ConnectorPin.CB3_CN8Pin10},
                    new {Processor = ProcessorPin.Pin9, Connector = ConnectorPin.CB3_CN8Pin11},
                    new {Processor = ProcessorPin.Pin10, Connector = ConnectorPin.CB3_CN8Pin12},
                    new {Processor = ProcessorPin.Pin11, Connector = ConnectorPin.CB3_CN8Pin15},
                    new {Processor = ProcessorPin.Pin12, Connector = ConnectorPin.CB3_CN8Pin16},
                    new {Processor = ProcessorPin.Pin13, Connector = ConnectorPin.CB3_CN8Pin17},
                    new {Processor = ProcessorPin.Pin14, Connector = ConnectorPin.CB3_CN8Pin18},
                    new {Processor = ProcessorPin.Pin15, Connector = ConnectorPin.CB3_CN8Pin19},
                    new {Processor = ProcessorPin.Pin16, Connector = ConnectorPin.CB3_CN8Pin20},
                    new {Processor = ProcessorPin.Pin17, Connector = ConnectorPin.CB3_CN8Pin21},
                    new {Processor = ProcessorPin.Pin18, Connector = ConnectorPin.CB3_CN8Pin22},
                    new {Processor = ProcessorPin.Pin19, Connector = ConnectorPin.CB3_CN8Pin23},
                    new {Processor = ProcessorPin.Pin20, Connector = ConnectorPin.CB3_CN8Pin25},
                    //
                    new {Processor = ProcessorPin.Pin21, Connector = ConnectorPin.CB3_CN9Pin3},
                    new {Processor = ProcessorPin.Pin22, Connector = ConnectorPin.CB3_CN9Pin4},
                    new {Processor = ProcessorPin.Pin23, Connector = ConnectorPin.CB3_CN9Pin5},
                    new {Processor = ProcessorPin.Pin24, Connector = ConnectorPin.CB3_CN9Pin6},
                    new {Processor = ProcessorPin.Pin25, Connector = ConnectorPin.CB3_CN9Pin7},
                    new {Processor = ProcessorPin.Pin26, Connector = ConnectorPin.CB3_CN9Pin8},
                    new {Processor = ProcessorPin.Pin27, Connector = ConnectorPin.CB3_CN9Pin9},
                    new {Processor = ProcessorPin.Pin28, Connector = ConnectorPin.CB3_CN9Pin10},
                    new {Processor = ProcessorPin.Pin29, Connector = ConnectorPin.CB3_CN9Pin11},
                    new {Processor = ProcessorPin.Pin30, Connector = ConnectorPin.CB3_CN9Pin12},
                    new {Processor = ProcessorPin.Pin31, Connector = ConnectorPin.CB3_CN9Pin13},
                    new {Processor = ProcessorPin.Pin32, Connector = ConnectorPin.CB3_CN9Pin14}
                };
            } 
            else
            {
                if (GpioConnectionSettings.ConnectorPinout ==  ConnectorPinout.Rev1)
                    mapping = new[] {
                        new {Processor = ProcessorPin.Pin0, Connector = ConnectorPin.P1Pin3},
                        new {Processor = ProcessorPin.Pin1, Connector = ConnectorPin.P1Pin5},
                        new {Processor = ProcessorPin.Pin4, Connector = ConnectorPin.P1Pin7},
                        new {Processor = ProcessorPin.Pin7, Connector = ConnectorPin.P1Pin26},
                        new {Processor = ProcessorPin.Pin8, Connector = ConnectorPin.P1Pin24},
                        new {Processor = ProcessorPin.Pin9, Connector = ConnectorPin.P1Pin21},
                        new {Processor = ProcessorPin.Pin10, Connector = ConnectorPin.P1Pin19},
                        new {Processor = ProcessorPin.Pin11, Connector = ConnectorPin.P1Pin23},
                        new {Processor = ProcessorPin.Pin14, Connector = ConnectorPin.P1Pin8},
                        new {Processor = ProcessorPin.Pin15, Connector = ConnectorPin.P1Pin10},
                        new {Processor = ProcessorPin.Pin17, Connector = ConnectorPin.P1Pin11},
                        new {Processor = ProcessorPin.Pin18, Connector = ConnectorPin.P1Pin12},
                        new {Processor = ProcessorPin.Pin21, Connector = ConnectorPin.P1Pin13},
                        new {Processor = ProcessorPin.Pin22, Connector = ConnectorPin.P1Pin15},
                        new {Processor = ProcessorPin.Pin23, Connector = ConnectorPin.P1Pin16},
                        new {Processor = ProcessorPin.Pin24, Connector = ConnectorPin.P1Pin18},
                        new {Processor = ProcessorPin.Pin25, Connector = ConnectorPin.P1Pin22}
                    };
                else if (GpioConnectionSettings.ConnectorPinout == ConnectorPinout.Rev2)
                    mapping = new[] {
                        new {Processor = ProcessorPin.Pin2, Connector = ConnectorPin.P1Pin3},
                        new {Processor = ProcessorPin.Pin3, Connector = ConnectorPin.P1Pin5},
                        new {Processor = ProcessorPin.Pin4, Connector = ConnectorPin.P1Pin7},
                        new {Processor = ProcessorPin.Pin7, Connector = ConnectorPin.P1Pin26},
                        new {Processor = ProcessorPin.Pin8, Connector = ConnectorPin.P1Pin24},
                        new {Processor = ProcessorPin.Pin9, Connector = ConnectorPin.P1Pin21},
                        new {Processor = ProcessorPin.Pin10, Connector = ConnectorPin.P1Pin19},
                        new {Processor = ProcessorPin.Pin11, Connector = ConnectorPin.P1Pin23},
                        new {Processor = ProcessorPin.Pin14, Connector = ConnectorPin.P1Pin8},
                        new {Processor = ProcessorPin.Pin15, Connector = ConnectorPin.P1Pin10},
                        new {Processor = ProcessorPin.Pin17, Connector = ConnectorPin.P1Pin11},
                        new {Processor = ProcessorPin.Pin18, Connector = ConnectorPin.P1Pin12},
                        new {Processor = ProcessorPin.Pin27, Connector = ConnectorPin.P1Pin13},
                        new {Processor = ProcessorPin.Pin22, Connector = ConnectorPin.P1Pin15},
                        new {Processor = ProcessorPin.Pin23, Connector = ConnectorPin.P1Pin16},
                        new {Processor = ProcessorPin.Pin24, Connector = ConnectorPin.P1Pin18},
                        new {Processor = ProcessorPin.Pin25, Connector = ConnectorPin.P1Pin22},
                        new {Processor = ProcessorPin.Pin28, Connector = ConnectorPin.P5Pin3},
                        new {Processor = ProcessorPin.Pin29, Connector = ConnectorPin.P5Pin4},
                        new {Processor = ProcessorPin.Pin30, Connector = ConnectorPin.P5Pin5},
                        new {Processor = ProcessorPin.Pin31, Connector = ConnectorPin.P5Pin6}
                    };
                else //if (GpioConnectionSettings.ConnectorPinout == ConnectorPinout.Plus)
                    mapping = new[] {
                        new {Processor = ProcessorPin.Pin2, Connector = ConnectorPin.P1Pin3},
                        new {Processor = ProcessorPin.Pin3, Connector = ConnectorPin.P1Pin5},
                        new {Processor = ProcessorPin.Pin4, Connector = ConnectorPin.P1Pin7},
                        new {Processor = ProcessorPin.Pin5, Connector = ConnectorPin.P1Pin29},
                        new {Processor = ProcessorPin.Pin6, Connector = ConnectorPin.P1Pin31},
                        new {Processor = ProcessorPin.Pin7, Connector = ConnectorPin.P1Pin26},
                        new {Processor = ProcessorPin.Pin8, Connector = ConnectorPin.P1Pin24},
                        new {Processor = ProcessorPin.Pin9, Connector = ConnectorPin.P1Pin21},
                        new {Processor = ProcessorPin.Pin10, Connector = ConnectorPin.P1Pin19},
                        new {Processor = ProcessorPin.Pin11, Connector = ConnectorPin.P1Pin23},
                        new {Processor = ProcessorPin.Pin12, Connector = ConnectorPin.P1Pin32},
                        new {Processor = ProcessorPin.Pin13, Connector = ConnectorPin.P1Pin33},
                        new {Processor = ProcessorPin.Pin14, Connector = ConnectorPin.P1Pin8},
                        new {Processor = ProcessorPin.Pin15, Connector = ConnectorPin.P1Pin10},
                        new {Processor = ProcessorPin.Pin16, Connector = ConnectorPin.P1Pin36},
                        new {Processor = ProcessorPin.Pin17, Connector = ConnectorPin.P1Pin11},
                        new {Processor = ProcessorPin.Pin18, Connector = ConnectorPin.P1Pin12},
                        new {Processor = ProcessorPin.Pin19, Connector = ConnectorPin.P1Pin35},
                        new {Processor = ProcessorPin.Pin20, Connector = ConnectorPin.P1Pin38},
                        new {Processor = ProcessorPin.Pin21, Connector = ConnectorPin.P1Pin40},
                        new {Processor = ProcessorPin.Pin22, Connector = ConnectorPin.P1Pin15},
                        new {Processor = ProcessorPin.Pin23, Connector = ConnectorPin.P1Pin16},
                        new {Processor = ProcessorPin.Pin24, Connector = ConnectorPin.P1Pin18},
                        new {Processor = ProcessorPin.Pin25, Connector = ConnectorPin.P1Pin22},
                        new {Processor = ProcessorPin.Pin26, Connector = ConnectorPin.P1Pin37},
                        new {Processor = ProcessorPin.Pin27, Connector = ConnectorPin.P1Pin13}
                    };
            }

            processorMappings = mapping.ToDictionary(p => p.Connector, p => p.Processor);
            connectorMappings = mapping.ToDictionary(p => p.Processor, p => p.Connector);

        }

        static string GetUname()
        {
            string output = "";
            // Start the child process.
            using (var p = new Process())
            {
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "uname";
                p.StartInfo.Arguments = "-a";
                p.Start();
                // Do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // p.WaitForExit();
                // Read the output stream first and then wait.
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            //Console.WriteLine("[DEBUG] 'uname -a' => " + output);
            return output;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert the specified connector pin to a processor pin.
        /// </summary>
        /// <param name="pin">The connector pin.</param>
        /// <returns>The processor pin.</returns>
        public static ProcessorPin ToProcessor(this ConnectorPin pin)
        {
            ProcessorPin processorPin;
            if (!processorMappings.TryGetValue(pin, out processorPin))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Connector pin {0} is not mapped to processor with pin layout revision {1}", pin.ToString().Replace("Pin", "-"), GpioConnectionSettings.ConnectorPinout));
            
            return processorPin;
        }

        /// <summary>
        /// Convert the specified processor pin to a connector pin.
        /// </summary>
        /// <param name="pin">The processor pin.</param>
        /// <returns>The connector pin.</returns>
        public static ConnectorPin ToConnector(this ProcessorPin pin)
        {
            ConnectorPin connectorPin;
            if (!connectorMappings.TryGetValue(pin, out connectorPin))
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Processor pin {0} is not mapped to processor with pin layout revision {1}", (int) pin, GpioConnectionSettings.ConnectorPinout));
            
            return connectorPin;
        }

        #endregion
    }
}