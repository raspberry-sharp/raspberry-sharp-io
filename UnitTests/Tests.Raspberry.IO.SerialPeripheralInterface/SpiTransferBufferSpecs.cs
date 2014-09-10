using System;
using FluentAssertions;
using NUnit.Framework;
using Raspberry.IO.SerialPeripheralInterface;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Tests.Raspberry.IO.SerialPeripheralInterface.SpiTransferBufferSpecs
{
    [TestFixture]
    public class If_the_user_creates_an_spi_transfer_buffer_for_transmission_only : Spec {
        private const int REQUESTED_SIZE = 500;
        private ISpiTransferBuffer buffer;

        protected override void BecauseOf() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.Write);
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_for_transmission_data() {
            buffer.Tx.Should().NotBeNull();
        }

        [Test]
        public void Should_the_structure_contain_no_memory_buffer_to_receive_data() {
            buffer.Rx.Should().BeNull();
        }

        [Test]
        public void Should_the_buffer_size_be_equal_to_the_requested_size() {
            buffer.Length.Should().Be(REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transmission_data_buffer_size_be_equal_to_the_requested_size() {
            buffer.Tx.Length.Should().Be(REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transfer_mode_be_write_only() {
            buffer.TransferMode.Should().Be(SpiTransferMode.Write);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_TX() {
            buffer.ControlStructure.Tx.Should().Be(unchecked((UInt64) buffer.Tx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_RX() {
            buffer.ControlStructure.Rx.Should().Be(0L);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_length() {
            buffer.ControlStructure.Length.Should().Be(REQUESTED_SIZE);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_creates_an_spi_transfer_buffer_for_receive_only : Spec
    {
        private const int REQUESTED_SIZE = 500;
        private ISpiTransferBuffer buffer;

        protected override void BecauseOf() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.Read);
        }

        [Test]
        public void Should_the_structure_contain_no_memory_buffer_for_transmission_data() {
            buffer.Tx.Should().BeNull();
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_to_receive_data() {
            buffer.Rx.Should().NotBeNull();
        }

        [Test]
        public void Should_the_buffer_size_be_equal_to_the_requested_size() {
            buffer.Length.Should().Be(REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_receive_data_buffer_size_be_equal_to_the_requested_size() {
            buffer.Rx.Length.Should().Be(REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transfer_mode_be_read_only() {
            buffer.TransferMode.Should().Be(SpiTransferMode.Read);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_RX() {
            buffer.ControlStructure.Rx.Should().Be(unchecked((UInt64)buffer.Rx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_TX() {
            buffer.ControlStructure.Tx.Should().Be(0L);
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_length() {
            buffer.ControlStructure.Length.Should().Be(REQUESTED_SIZE);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_creates_an_spi_transfer_buffer_for_transmission_and_receive : Spec
    {
        private const int REQUESTED_SIZE = 500;
        private ISpiTransferBuffer buffer;

        protected override void BecauseOf() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.ReadWrite);
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_for_transmission_data() {
            buffer.Tx.Should().NotBeNull();
        }

        [Test]
        public void Should_the_structure_contain_a_memory_buffer_to_receive_data() {
            buffer.Rx.Should().NotBeNull();
        }

        [Test]
        public void Should_the_buffer_size_be_equal_to_the_requested_size() {
            buffer.Length.Should().Be(REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_transmission_data_buffer_size_be_equal_to_the_requested_size() {
            buffer.Tx.Length.Should().Be(REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_receive_data_buffer_size_be_equal_to_the_requested_size() {
            buffer.Rx.Length.Should().Be(REQUESTED_SIZE);
        }

        [Test]
        public void Should_the_data_buffers_not_have_the_same_memory_address() {
            buffer.Rx.Pointer.Should().NotBe(buffer.Tx.Pointer);
        }

        [Test]
        public void Should_the_transfer_mode_have_set_the_READ_flag() {
            buffer.TransferMode.HasFlag(SpiTransferMode.Read).Should().BeTrue();
        }

        [Test]
        public void Should_the_transfer_mode_have_set_the_WRITE_flag() {
            buffer.TransferMode.HasFlag(SpiTransferMode.Write).Should().BeTrue();
        }


        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_RX() {
            buffer.ControlStructure.Rx.Should().Be(unchecked((UInt64)buffer.Rx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_address_for_TX() {
            buffer.ControlStructure.Tx.Should().Be(unchecked((UInt64)buffer.Tx.Pointer.ToInt64()));
        }

        [Test]
        public void Should_the_transfer_structure_have_the_correct_memory_length() {
            buffer.ControlStructure.Length.Should().Be(REQUESTED_SIZE);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_changes_various_transfer_settings : Spec {
        private const int REQUESTED_SIZE = 100;
        private const int REQUESTED_BITS_PER_WORD = 16;
        private const bool REQUESTED_CHIP_SELECT_CHANGE = true;
        private const int REQUESTED_DELAY_IN_USEC = 100;
        private const int REQUESTED_SPEED_IN_HZ = 1000000;
        private SpiTransferBuffer buffer;

        protected override void EstablishContext() {
            buffer = new SpiTransferBuffer(REQUESTED_SIZE, SpiTransferMode.Write);
        }

        protected override void BecauseOf() {
            buffer.BitsPerWord = REQUESTED_BITS_PER_WORD;
            buffer.ChipSelectChange = REQUESTED_CHIP_SELECT_CHANGE;
            buffer.Delay = REQUESTED_DELAY_IN_USEC;
            buffer.Speed = REQUESTED_SPEED_IN_HZ;
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_wordsize() {
            buffer.ControlStructure.BitsPerWord.Should().Be(REQUESTED_BITS_PER_WORD);
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_chip_select_change_value() {
            // ReSharper disable once UnreachableCode
			buffer.ControlStructure.ChipSelectChange.Should().Be(REQUESTED_CHIP_SELECT_CHANGE ? (byte)1 : (byte)0);
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_delay() {
            buffer.ControlStructure.Delay.Should().Be(REQUESTED_DELAY_IN_USEC);
        }

        [Test]
        public void Should_the_control_structure_have_the_requested_speed() {
            buffer.ControlStructure.Speed.Should().Be(REQUESTED_SPEED_IN_HZ);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(buffer, null)) {
                buffer.Dispose();
                buffer = null;
            }
        }
    }
}