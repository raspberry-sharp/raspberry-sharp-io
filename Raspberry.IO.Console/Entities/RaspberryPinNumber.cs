//-----------------------------------------------------------------------
// <copyright file="RaspberryPinNumber.cs" company="Andrew Bradford">
//     Copyright (C) 2012 Andrew Bradford
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
//     associated documentation files (the "Software"), to deal in the Software without restriction, 
//     including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
//     and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject 
//     to the following conditions:
//     
//     The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//     
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//     WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//     COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//     ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
//-----------------------------------------------------------------------

namespace PiSharp.LibGpio.Entities
{
    /// <summary>
    /// The Raspberry Pi GPIO ID
    /// </summary>
    public enum RaspberryPinNumber
    {
        /// <summary>
        /// Invalid pin number - used to established a known invalid value to un-initialised variables.
        /// </summary>
        Zero = 0,

        /// <summary>
        /// GPIO Pin 1
        /// </summary>
        One,

        /// <summary>
        /// GPIO Pin 2
        /// </summary>
        Two,

        /// <summary>
        /// GPIO Pin 3
        /// </summary>
        Three,

        /// <summary>
        /// GPIO Pin 4
        /// </summary>
        Four,

        /// <summary>
        /// GPIO Pin 5
        /// </summary>
        Five,

        /// <summary>
        /// GPIO Pin 6
        /// </summary>
        Six,

        /// <summary>
        /// GPIO Pin 7
        /// </summary>
        Seven,
    }
}
