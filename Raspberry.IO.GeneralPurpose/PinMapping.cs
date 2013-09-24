#region References

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using Raspberry.IO.GeneralPurpose.Configuration;

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
            var mapping = GpioConnectionSettings.BoardConnectorRevision == 1
                              ? new[]
                                    {
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
                                    }
                              : new[]
                                    {
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

            processorMappings = mapping.ToDictionary(p => p.Connector, p => p.Processor);
            connectorMappings = mapping.ToDictionary(p => p.Processor, p => p.Connector);
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
            if (processorMappings.TryGetValue(pin, out processorPin))
                return processorPin;
            else
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Connector pin {0} is not mapped to processor with pin layout revision {1}", pin.ToString().Replace("Pin", "-"), GpioConnectionSettings.BoardConnectorRevision));
        }

        /// <summary>
        /// Convert the specified processor pin to a connector pin.
        /// </summary>
        /// <param name="pin">The processor pin.</param>
        /// <returns>The connector pin.</returns>
        public static ConnectorPin ToConnector(this ProcessorPin pin)
        {
            ConnectorPin connectorPin;
            if (connectorMappings.TryGetValue(pin, out connectorPin))
                return connectorPin;
            else
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Processor pin {0} is not mapped to processor with pin layout revision {1}", (int) pin, GpioConnectionSettings.BoardConnectorRevision));
        }

        #endregion
    }
}