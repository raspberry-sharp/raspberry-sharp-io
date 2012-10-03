Raspberry# IO
=============

Raspberry# IO is a .NET/Mono IO Library for Raspberry Pi.

Development is in a preliminary stage, project structure is subject to frequent changes.
Raspberry# IO currently support basic GPIO input/output.

Support for extended IO (such as support for I2C peripherals).

Features
========

Raspberry# IO provides a convenient way to use Raspberry Pi GPIO, while using .NET concepts, syntax and case.

 . Support for both memory (through libBCM2835 library) and file (native) access to GPIO
 . Support for processor and connector pin addressing
 . Support aliasing of pins for better customizing
 . Support both Raspberry B rev1 and rev2 pins mapping, as well as rev2 P5 connector
 . Easy-of-use, declarative configuration of pins
 . Support event firing when an input pin status change
 . Controlled use of resources using a IDisposable component