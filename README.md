<h1>Raspberry# IO</h1>

<h2>Introduction</h2>
Raspberry# IO is a .NET/Mono IO Library for Raspberry Pi.

Development is in a preliminary stage, project structure is subject to frequent changes.
Raspberry# IO currently support basic GPIO input/output.

Support for extended IO (such as support for I2C peripherals).

<h2>Features</h2>
Raspberry# IO provides a convenient way to use Raspberry Pi GPIO, while using .NET concepts, syntax and case.

<ul>
 <li>Support for both memory (through libBCM2835 library) and file (native) access to GPIO</li>
 <li>Support for processor and connector pin addressing</li>
 <li>Support aliasing of pins for better customizing</li>
 <li>Support both Raspberry B rev1 and rev2 pins mapping, as well as rev2 P5 connector</li>
 <li>Easy-of-use, declarative configuration of pins</li>
 <li>Support event firing when an input pin status change</li>
 <li>Controlled use of resources using a IDisposable component</li>
</ul>