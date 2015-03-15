using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using FluentAssertions;
using NUnit.Framework;
using Raspberry.IO.SerialPeripheralInterface;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Tests.Raspberry.IO.SerialPeripheralInterface.NativeSpiConnectionSpecs
{
    [TestFixture]
    public class If_the_user_creates_a_native_SPI_connection_instance_providing_appropriate_connection_settings : Spec {
        private const int BITS_PER_WORD = 8;
        private const int DELAY = 500;
        private const SpiMode SPI_MODE = SpiMode.Mode2;
        private const int SPEED_IN_HZ = 500000;
        
        private SpiConnectionSettings settings;
        private NativeSpiConnection connection;
        private ISpiControlDevice controlDevice;

        protected override void EstablishContext() {
            settings = new SpiConnectionSettings {
                BitsPerWord = BITS_PER_WORD,
                Delay = DELAY,
                Mode = SPI_MODE,
                MaxSpeed = SPEED_IN_HZ
            };

            controlDevice = A.Fake<ISpiControlDevice>();
        }

        protected override void BecauseOf() {
            connection = new NativeSpiConnection(controlDevice, settings);
        }

        [Test]
        public void Should_it_write_the_max_speed_in_Hz_to_the_control_device() {
            UInt32 speed = SPEED_IN_HZ;
            
            controlDevice
                .CallsTo(device => device.Control(NativeSpiConnection.SPI_IOC_WR_MAX_SPEED_HZ, ref speed))
                .MustHaveHappened(Repeated.Exactly.Once);

            controlDevice
                .CallsTo(device => device.Control(NativeSpiConnection.SPI_IOC_RD_MAX_SPEED_HZ, ref speed))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_it_write_the_bits_per_word_to_the_control_device() {
            byte bitsPerWord = BITS_PER_WORD;
            
            controlDevice
                .CallsTo(device => device.Control(NativeSpiConnection.SPI_IOC_WR_BITS_PER_WORD, ref bitsPerWord))
                .MustHaveHappened(Repeated.Exactly.Once);

            controlDevice
                .CallsTo(device => device.Control(NativeSpiConnection.SPI_IOC_RD_BITS_PER_WORD, ref bitsPerWord))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_it_write_the_spi_mode_to_the_control_device() {
            var spiMode = (UInt32)SPI_MODE;
            
            controlDevice
                .CallsTo(device => device.Control(NativeSpiConnection.SPI_IOC_WR_MODE, ref spiMode))
                .MustHaveHappened(Repeated.Exactly.Once);

            controlDevice
                .CallsTo(device => device.Control(NativeSpiConnection.SPI_IOC_RD_MODE, ref spiMode))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_it_set_the_delay() {
            connection.Delay.Should().Be(DELAY);
        }

        [Test]
        public void Should_it_set_the_speed() {
            connection.MaxSpeed.Should().Be(SPEED_IN_HZ);
        }

        [Test]
        public void Should_it_set_the_SPI_mode() {
            connection.Mode.Should().Be(SPI_MODE);
        }

        [Test]
        public void Should_it_set_the_bits_per_word() {
            connection.BitsPerWord.Should().Be(BITS_PER_WORD);
        }
    }

    [TestFixture]
    public class If_the_user_requests_a_spi_transfer_buffer : Spec {
        private const int BITS_PER_WORD = 16;
        private const int DELAY = 500;
        private const SpiMode SPI_MODE = SpiMode.Mode2;
        private const int SPEED_IN_HZ = 500000;
        private const int REQUESTED_SIZE = 100;

        private SpiConnectionSettings settings;
        private ISpiControlDevice controlDevice;
        private NativeSpiConnection connection;
        private ISpiTransferBuffer buffer;

        protected override void EstablishContext() {
            settings = new SpiConnectionSettings {
                BitsPerWord = BITS_PER_WORD, 
                Delay = DELAY, 
                Mode = SPI_MODE, 
                MaxSpeed = SPEED_IN_HZ
            };

            controlDevice = A.Fake<ISpiControlDevice>();
            connection = new NativeSpiConnection(controlDevice, settings);
        }

        protected override void BecauseOf() {
            buffer = connection.CreateTransferBuffer(REQUESTED_SIZE, SpiTransferMode.ReadWrite);
        }

        [Test]
        public void Should_the_buffer_be_initialized_with_the_connections_wordsize() {
            buffer.ControlStructure.BitsPerWord.Should().Be(BITS_PER_WORD);
        }
        
        [Test]
        public void Should_the_buffer_be_initialized_with_the_connections_delay() {
            buffer.ControlStructure.Delay.Should().Be(DELAY);
        }

        [Test]
        public void Should_the_buffer_be_initialized_with_the_connections_speed() {
            buffer.ControlStructure.Speed.Should().Be(SPEED_IN_HZ);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_starts_a_single_data_transfer : Spec {
        private const int BITS_PER_WORD = 16;
        private const int DELAY = 500;
        private const int SPEED_IN_HZ = 500000;
        private const int IOCTL_PINVOKE_RESULT_CODE = 1;
        private const int SPI_IOC_MESSAGE_1 = 0x40206b00;

        private ISpiControlDevice controlDevice;
        private NativeSpiConnection connection;
        private ISpiTransferBuffer buffer;
        private int result;
        private SpiTransferControlStructure controlStructure;

        protected override void EstablishContext() {
            // SPI control structure we expect to see during the P/Invoke call
            controlStructure = new SpiTransferControlStructure {
                BitsPerWord = BITS_PER_WORD,
                Length = 5,
                Delay = DELAY,
                ChipSelectChange = 1,
                Speed = SPEED_IN_HZ
            };
            
            controlDevice = A.Fake<ISpiControlDevice>();
            controlDevice
                .CallsTo(device => device.Control(A<uint>.Ignored, ref controlStructure))
                .WithAnyArguments()
                .Returns(IOCTL_PINVOKE_RESULT_CODE);

            connection = new NativeSpiConnection(controlDevice);

            buffer = A.Fake<ISpiTransferBuffer>();
            buffer
                .CallsTo(b => b.ControlStructure)
                .Returns(controlStructure);
        }

        protected override void BecauseOf() {
            result = connection.Transfer(buffer);
        }

        [Test]
        public void Should_the_buffers_control_structure_be_sent_to_the_IOCTL_device() {
            controlDevice
                .CallsTo(device => device.Control(SPI_IOC_MESSAGE_1, ref controlStructure))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_it_return_the_pinvoke_result_code() {
            result.Should().Be(IOCTL_PINVOKE_RESULT_CODE);
        }
    }

    [TestFixture]
    public class If_the_user_starts_a_multi_data_transfer : Spec
    {
        private const int BITS_PER_WORD = 16;
        private const int DELAY = 500;
        private const int SPEED_IN_HZ = 500000;
        private const int IOCTL_PINVOKE_RESULT_CODE = 1;
        private const int SPI_IOC_MESSAGE_1 = 0x40206b00;

        private ISpiControlDevice controlDevice;
        private NativeSpiConnection connection;
        private ISpiTransferBufferCollection collection;
        private ISpiTransferBuffer buffer;
        private int result;
        private SpiTransferControlStructure controlStructure;

        protected override void EstablishContext() {
            controlDevice = A.Fake<ISpiControlDevice>();
            controlDevice
                .CallsTo(device => device.Control(A<uint>.Ignored, A<SpiTransferControlStructure[]>.Ignored))
                .Returns(IOCTL_PINVOKE_RESULT_CODE);

            connection = new NativeSpiConnection(controlDevice);

            // SPI control structure we expect to see during the P/Invoke call
            controlStructure = new SpiTransferControlStructure {
                BitsPerWord = BITS_PER_WORD,
                Length = 5,
                Delay = DELAY,
                ChipSelectChange = 1,
                Speed = SPEED_IN_HZ
            };

            buffer = A.Fake<ISpiTransferBuffer>();
            buffer
                .CallsTo(b => b.ControlStructure)
                .Returns(controlStructure);
            
            // setup fake collection to return our "prepared" fake buffer
            collection = A.Fake<ISpiTransferBufferCollection>();
            collection
                .CallsTo(c => c.Length)
                .Returns(1);
            collection
                .CallsTo(c => c.GetEnumerator())
                .ReturnsLazily(call => new List<ISpiTransferBuffer>{buffer}.GetEnumerator());
        }

        protected override void BecauseOf() {
            result = connection.Transfer(collection);
        }

        [Test]
        public void Should_the_buffers_control_structure_be_sent_to_the_IOCTL_device() {
            controlDevice
                .CallsTo(device => device.Control(SPI_IOC_MESSAGE_1, A<SpiTransferControlStructure[]>.That.Matches(s => Predicate(s))))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private bool Predicate(IEnumerable<SpiTransferControlStructure> control_structures) {
            return control_structures.Contains(controlStructure);
        }

        [Test]
        public void Should_it_return_the_pinvoke_result_code() {
            result.Should().Be(IOCTL_PINVOKE_RESULT_CODE);
        }
    }

}