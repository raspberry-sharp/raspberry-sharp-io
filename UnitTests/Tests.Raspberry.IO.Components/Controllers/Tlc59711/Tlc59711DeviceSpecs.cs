using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Raspberry.IO;
using Raspberry.IO.Components.Controllers.Tlc59711;
using Raspberry.IO.Interop;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Tests.Raspberry.IO.Components.Controllers.Tlc59711.Tlc59711DeviceSpecs
{
    [TestFixture]
    public class If_the_user_creates_a_new_device_instance_with_default_settings : Spec
    {
        private const int COMMAND_SIZE = 28;
        private const int NUMBER_OF_CHANNELS = 12;
        private const int PWM_WORDSIZE = 2;
        // 0x25, OUTTMG = 1, EXTGCK = 0, TMGRST = 1, DSPRPT = 1, BLANK = 1, BCB=0x7f, BCG=0x7f, BCR=0x7f
        private const string EXPECTED_SETTINGS = "10010110111111111111111111111111";

        private IMemory memory;
        private ITlc59711Device device;

        protected override void EstablishContext() {
            memory = new ManagedMemory(COMMAND_SIZE);
        }

        protected override void BecauseOf() {
            device = new Tlc59711Device(memory);
        }

        [Test]
        public void Should_the_device_set_to_BLANK() {
            device.Blank.Should().BeTrue();
        }

        [Test]
        public void Should_OUTTMG_set_to_TRUE() {
            device.ReferenceClockEdge.Should().BeTrue();
        }

        [Test]
        public void Should_EXTGCK_set_to_FALSE() {
            device.ReferenceClock.Should().BeFalse();
        }

        [Test]
        public void Should_TMGRST_set_to_TRUE() {
            device.DisplayTimingResetMode.Should().BeTrue();
        }

        [Test]
        public void Should_DSPRPT_set_to_TRUE() {
            device.DisplayRepeatMode.Should().BeTrue();
        }

        [Test]
        public void Should_the_memory_be_correctly_initialized() {
            var expected_settings = EXPECTED_SETTINGS.BitStringToArray(true);
            memory.Take(4)
                .Should()
                .ContainInOrder(expected_settings);
        }

        [Test]
        public void Should_all_channels_set_to_0x0000() {
            memory.Skip(4)
                .ToArray()
                .Should()
                .ContainInOrder(new byte[PWM_WORDSIZE * NUMBER_OF_CHANNELS]);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(memory, null)) {
                memory.Dispose();
                memory = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_creates_a_new_device_instance_using_his_own_settings_settings : Spec
    {
        private const int COMMAND_SIZE = 28;
        // 0x25, OUTTMG = 0, EXTGCK = 1, TMGRST = 0, DSPRPT = 0, BLANK = 0, BCB=0x00, BCG=0x00, BCR=0x00
        private const string EXPECTED_SETTINGS = "10010101000000000000000000000000";
        
        private IMemory memory;
        private ITlc59711Device device;
        private ITlc59711Settings settings;

        protected override void EstablishContext() {
            memory = new ManagedMemory(COMMAND_SIZE);
            settings = new Tlc59711Settings {
                Blank = false,
                ReferenceClockEdge = false,
                ReferenceClock = true,
                DisplayTimingResetMode = false,
                DisplayRepeatMode = false,
                BrightnessControlB = 0,
                BrightnessControlG = 0,
                BrightnessControlR = 0
            };
        }

        protected override void BecauseOf() {
            device = new Tlc59711Device(memory, settings);
        }

        [Test]
        public void Should_the_device_set_to_not_BLANK() {
            device.Blank.Should().BeFalse();
        }

        [Test]
        public void Should_OUTTMG_set_to_FALSE() {
            device.ReferenceClockEdge.Should().BeFalse();
        }

        [Test]
        public void Should_EXTGCK_set_to_TRUE() {
            device.ReferenceClock.Should().BeTrue();
        }

        [Test]
        public void Should_TMGRST_set_to_FALSE() {
            device.DisplayTimingResetMode.Should().BeFalse();
        }

        [Test]
        public void Should_DSPRPT_set_to_FALSE() {
            device.DisplayRepeatMode.Should().BeFalse();
        }

        [Test]
        public void Should_the_memory_be_correctly_initialized() {
            var expected_settings = EXPECTED_SETTINGS.BitStringToArray(true);
            memory.Take(4)
                .Should()
                .ContainInOrder(expected_settings);
        }

        protected override void Cleanup() {
            if (!ReferenceEquals(memory, null)) {
                memory.Dispose();
                memory = null;
            }
        }
    }

    [TestFixture]
    public class If_the_user_changes_a_setting_after_initialization : Spec
    {
        private class TestCase : TestCaseData
        {
            public TestCase(string description, Action<ITlc59711Settings> action) 
                : base(description, action) {}
        }

        private const int COMMAND_SIZE = 28;

        private static IEnumerable TestCases {
            get {
                // MAGIC_WORD, OUTTMG, EXTGCK, TMGRST, DSPRPT, BLANK, BCB, BCG, BCR
                yield return new TestCase("do nothing", s => { })
                    .Returns("10010110111111111111111111111111");
                yield return new TestCase("Blank", s => s.Blank = false)
                    .Returns("10010110110111111111111111111111");
                yield return new TestCase("BrightnessControlR", s => s.BrightnessControlR = 0)
                    .Returns("10010110111111111111111110000000");
                yield return new TestCase("BrightnessControlG", s => s.BrightnessControlG = 0)
                    .Returns("10010110111111111100000001111111");
                yield return new TestCase("BrightnessControlB", s => s.BrightnessControlB = 0)
                    .Returns("10010110111000000011111111111111");
                yield return new TestCase("DisplayRepeatMode", s => s.DisplayRepeatMode = false)
                    .Returns("10010110101111111111111111111111");
                yield return new TestCase("DisplayTimingResetMode", s => s.DisplayTimingResetMode = false)
                    .Returns("10010110011111111111111111111111");
                yield return new TestCase("ReferenceClock", s => s.ReferenceClock = true)
                    .Returns("10010111111111111111111111111111");
                yield return new TestCase("ReferenceClockEdge", s => s.ReferenceClockEdge = false)
                    .Returns("10010100111111111111111111111111");
            }
        }

        [Test, TestCaseSource("TestCases")]
        public string Should_the_resulting_memory_be_correct(string description, Action<ITlc59711Settings> action) {
            using (var memory = new ManagedMemory(COMMAND_SIZE)) {
                var device = new Tlc59711Device(memory);
                action(device);

                Debug.Print("Running action on: {0}", description);
                return memory
                    .Take(4)
                    .ToBitString();
            }
        }
    }

    [TestFixture]
    public class If_the_user_changes_PWM_data_after_initialization : Spec
    {
        private const int NUMBER_OF_CHANNELS = 12;
        private const int PWM_WORDSIZE = 2;
        private const int COMMAND_SIZE = 28;
        private const UInt16 VALUE = 0x1234;

        private static IEnumerable TestCases {
            get {
                for (var i = 0; i < NUMBER_OF_CHANNELS; i++) {
                    yield return new TestCaseData(i, VALUE)
                        .Returns(new byte[] {0x12, 0x34});
                }
            }
        }

        [Test,TestCaseSource("TestCases")]
        public byte[] Should_the_memory_be_set_to_the_correct_values(int channel, UInt16 value) {
            using (var memory = new ManagedMemory(COMMAND_SIZE)) {
                var device = new Tlc59711Device(memory);
                
                device.Channels.Set(channel, value);
                return memory
                    .Skip(4 + (channel * PWM_WORDSIZE))
                    .Take(2)
                    .ToArray();
            }
        }
    }
}