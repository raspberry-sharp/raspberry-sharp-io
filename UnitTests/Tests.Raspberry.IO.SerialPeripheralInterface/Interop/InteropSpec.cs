using System;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using Raspberry.IO.SerialPeripheralInterface;

// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace Tests.Raspberry.IO.SerialPeripheralInterface.InteropSpec
{
    [TestFixture]
    public class If_the_user_requests_the_size_of_N_spi_transfer_messages : Spec {
        private static IEnumerable TestCases {
            get {
                yield return new TestCaseData(0).Returns(Interop.SPI_IOC_MESSAGE_BASE);
                yield return new TestCaseData(1).Returns((UInt32)0x40206b00);
                yield return new TestCaseData(2).Returns((UInt32)0x40406b00);
                yield return new TestCaseData(3).Returns((UInt32)0x40606b00);
            }
        }

        [Test,TestCaseSource("TestCases")]
        public UInt32 Shall_the_result_be_correct(int numberOfMessages) {
            var result = Interop.GetSpiMessageRequest(numberOfMessages);
            Debug.Print("SPI_IOC_MESSAGE({0}) = {1:x}", numberOfMessages, result);
            return result;
        }
    }
}
