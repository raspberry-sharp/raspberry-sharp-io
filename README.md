Raspberry# IO
=============

Introduction
------------
Raspberry# IO is a .NET/Mono IO Library for Raspberry Pi. This project is an initiative of the [Raspberry#](http://www.raspberry-sharp.org) Community.

Current release is an early public release. Some features may not have been extensively tested.
Raspberry# IO currently supports GPIO input/output.

Support for extended IO (such as support for I2C peripherals) is planned for future releases.

Features
--------

### Raspberry.IO.GeneralPurpose
Raspberry.IO.GeneralPurpose provides a convenient way to use Raspberry Pi GPIO pins, while using .NET concepts, syntax and case.
You can easily add a reference to it in your Visual Studio projects using the **[Raspberry.IO.GeneralPurpose Nuget](https://www.nuget.org/packages/Raspberry.IO.GeneralPurpose)**.

It currently support the following features:
+ Access to GPIO pins through memory (using [BCM2835 C Library](http://www.open.com.au/mikem/bcm2835/)) or file (native) drivers
+ Addressing through **processor pin number or connector pin number**
+ Giving custom name to pins for more readable code
+ Various Rapsberry Pi revisions, for now **Raspberry B rev1 and rev2**, including rev2 P5 connector
+ Easy-of-use, declarative configuration of pins. Ability to revert the meaning (1/0) of pins; ability to **use an input pin as a switch button**
+ Firing of **events when pin status change** (input as well as output)
+ **High-level behaviors** for output pins, including *blink*, *pattern* and *chaser*
+ Controlled use of resources using a IDisposable component

See the [Wiki](https://github.com/raspberry-sharp/raspberry-sharp-io/wiki) for documentation and samples.