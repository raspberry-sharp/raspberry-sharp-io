#region References

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

#endregion

namespace Raspberry.IO.Components.Displays.Hd44780
{
    /// <summary>
    /// Represents encoding for HD44780 LCD with Japanese character set (ROM code A00)
    /// Based on http://lcd-linux.sourceforge.net/pdfdocs/hd44780.pdf
    /// And http://en.wikipedia.org/wiki/Katakana
    /// </summary>
    public class Hd44780A00Encoding : Encoding
    {
        #region Fields

        private static readonly Dictionary<char, byte> charMap = GetMap().GroupBy(p => p.Key, p => p.Value).ToDictionary(g => g.Key, g => g.First());
        private static readonly Dictionary<byte, char> byteMap = GetMap().GroupBy(p => p.Value, p => p.Key).ToDictionary(g => g.Key, g => g.First());

        private const byte missingChar = 0x3F;
        private const char missingByte = '\uFFFD';

        #endregion

        #region Properties

        /// <summary>
        /// Gets the supported characters.
        /// </summary>
        public static IEnumerable<char> SupportedCharacters
        {
            get { return charMap.Keys.Except(new[]{'\r', '\n'}); }
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// When overridden in a derived class, calculates the number of bytes produced by encoding a set of characters from the specified character array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="index">The index of the first character to encode.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <returns>
        /// The number of bytes produced by encoding the specified characters.
        /// </returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// When overridden in a derived class, encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="charIndex">The index of the first character to encode.</param>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <param name="bytes">The byte array to contain the resulting sequence of bytes.</param>
        /// <param name="byteIndex">The index at which to start writing the resulting sequence of bytes.</param>
        /// <returns>
        /// The actual number of bytes written into <paramref name="bytes" />.
        /// </returns>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            Array.Copy(
                chars
                    .Skip(charIndex)
                    .Take(charCount)
                    .Select(c =>
                                {
                                    byte b;
                                    return charMap.TryGetValue(c, out b) ? b : missingChar;
                                })
                    .ToArray(),
                0,
                bytes,
                byteIndex,
                charCount);

            return charCount;
        }

        /// <summary>
        /// When overridden in a derived class, calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="index">The index of the first byte to decode.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <returns>
        /// The number of characters produced by decoding the specified sequence of bytes.
        /// </returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// When overridden in a derived class, decodes a sequence of bytes from the specified byte array into the specified character array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="byteIndex">The index of the first byte to decode.</param>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <param name="chars">The character array to contain the resulting set of characters.</param>
        /// <param name="charIndex">The index at which to start writing the resulting set of characters.</param>
        /// <returns>
        /// The actual number of characters written into <paramref name="chars" />.
        /// </returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            Array.Copy(
                bytes
                    .Skip(byteIndex)
                    .Take(byteCount)
                    .Select(b =>
                                {
                                    char c;
                                    return byteMap.TryGetValue(b, out c) ? c : missingByte;
                                })
                    .ToArray(),
                0,
                chars,
                charIndex,
                byteCount);

            return byteCount;
        }

        /// <summary>
        /// When overridden in a derived class, calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <returns>
        /// The maximum number of bytes produced by encoding the specified number of characters.
        /// </returns>
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        /// <summary>
        /// When overridden in a derived class, calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <returns>
        /// The maximum number of characters produced by decoding the specified number of bytes.
        /// </returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        #endregion

        #region Private Helpers

        private static IEnumerable<KeyValuePair<char, byte>> GetMap()
        {
            // CR/LF
            yield return new KeyValuePair<char, byte>('\u000A', 0x0A);
            yield return new KeyValuePair<char, byte>('\u000D', 0x0A);

            // Custom characters
            yield return new KeyValuePair<char, byte>('\u0000', 0x00);
            yield return new KeyValuePair<char, byte>('\u0001', 0x01);
            yield return new KeyValuePair<char, byte>('\u0002', 0x02);
            yield return new KeyValuePair<char, byte>('\u0003', 0x03);
            yield return new KeyValuePair<char, byte>('\u0004', 0x04);
            yield return new KeyValuePair<char, byte>('\u0005', 0x05);
            yield return new KeyValuePair<char, byte>('\u0006', 0x06);
            yield return new KeyValuePair<char, byte>('\u0007', 0x07);
/*
            yield return new KeyValuePair<char, byte>(' ', 0x08);
            yield return new KeyValuePair<char, byte>(' ', 0x09);
            yield return new KeyValuePair<char, byte>(' ', 0x0A);
            yield return new KeyValuePair<char, byte>(' ', 0x0B);
            yield return new KeyValuePair<char, byte>(' ', 0x0C);
            yield return new KeyValuePair<char, byte>(' ', 0x0D);
            yield return new KeyValuePair<char, byte>(' ', 0x0E);
            yield return new KeyValuePair<char, byte>(' ', 0x0F);

            yield return new KeyValuePair<char, byte>(' ', 0x10);
            yield return new KeyValuePair<char, byte>(' ', 0x11);
            yield return new KeyValuePair<char, byte>(' ', 0x12);
            yield return new KeyValuePair<char, byte>(' ', 0x13);
            yield return new KeyValuePair<char, byte>(' ', 0x14);
            yield return new KeyValuePair<char, byte>(' ', 0x15);
            yield return new KeyValuePair<char, byte>(' ', 0x16);
            yield return new KeyValuePair<char, byte>(' ', 0x17);
            yield return new KeyValuePair<char, byte>(' ', 0x18);
            yield return new KeyValuePair<char, byte>(' ', 0x19);
            yield return new KeyValuePair<char, byte>(' ', 0x1A);
            yield return new KeyValuePair<char, byte>(' ', 0x1B);
            yield return new KeyValuePair<char, byte>(' ', 0x1C);
            yield return new KeyValuePair<char, byte>(' ', 0x1D);
            yield return new KeyValuePair<char, byte>(' ', 0x1E);
            yield return new KeyValuePair<char, byte>(' ', 0x1F);
*/
            yield return new KeyValuePair<char, byte>(' ', 0x20);
            // Variants
            yield return new KeyValuePair<char, byte>('\u0009', 0x20);
            yield return new KeyValuePair<char, byte>('\u000B', 0x20);
            yield return new KeyValuePair<char, byte>('\u000C', 0x20);
            yield return new KeyValuePair<char, byte>('\u0085', 0x20);
            yield return new KeyValuePair<char, byte>('\u00A0', 0x20);
            yield return new KeyValuePair<char, byte>('\u1680', 0x20);
            yield return new KeyValuePair<char, byte>('\u180E', 0x20);
            yield return new KeyValuePair<char, byte>('\u2000', 0x20);
            yield return new KeyValuePair<char, byte>('\u2001', 0x20);
            yield return new KeyValuePair<char, byte>('\u2002', 0x20);
            yield return new KeyValuePair<char, byte>('\u2003', 0x20);
            yield return new KeyValuePair<char, byte>('\u2004', 0x20);
            yield return new KeyValuePair<char, byte>('\u2005', 0x20);
            yield return new KeyValuePair<char, byte>('\u2006', 0x20);
            yield return new KeyValuePair<char, byte>('\u2007', 0x20);
            yield return new KeyValuePair<char, byte>('\u2008', 0x20);
            yield return new KeyValuePair<char, byte>('\u2009', 0x20);
            yield return new KeyValuePair<char, byte>('\u200A', 0x20);
            yield return new KeyValuePair<char, byte>('\u2028', 0x20);
            yield return new KeyValuePair<char, byte>('\u2029', 0x20);
            yield return new KeyValuePair<char, byte>('\u202F', 0x20);
            yield return new KeyValuePair<char, byte>('\u205F', 0x20);
            yield return new KeyValuePair<char, byte>('\u3000', 0x20);

            yield return new KeyValuePair<char, byte>('!', 0x21);

            yield return new KeyValuePair<char, byte>('"', 0x22);
            //Variants
            yield return new KeyValuePair<char, byte>('“', 0x22);
            yield return new KeyValuePair<char, byte>('”', 0x22);
            yield return new KeyValuePair<char, byte>('„', 0x22);
            yield return new KeyValuePair<char, byte>('‟', 0x22);

            yield return new KeyValuePair<char, byte>('#', 0x23);
            yield return new KeyValuePair<char, byte>('$', 0x24);
            yield return new KeyValuePair<char, byte>('%', 0x25);
            yield return new KeyValuePair<char, byte>('&', 0x26);

            yield return new KeyValuePair<char, byte>('\'', 0x27);
            // Variants
            yield return new KeyValuePair<char, byte>('‘', 0x2F);
            yield return new KeyValuePair<char, byte>('’', 0x2F);
            yield return new KeyValuePair<char, byte>('‛', 0x2F);
            yield return new KeyValuePair<char, byte>('′', 0x2F);

            yield return new KeyValuePair<char, byte>('(', 0x28);
            yield return new KeyValuePair<char, byte>(')', 0x29);
            yield return new KeyValuePair<char, byte>('*', 0x2A);
            yield return new KeyValuePair<char, byte>('+', 0x2B);
            yield return new KeyValuePair<char, byte>(',', 0x2C);

            yield return new KeyValuePair<char, byte>('-', 0x2D);
            // Variants
            yield return new KeyValuePair<char, byte>('‐', 0x2D);
            yield return new KeyValuePair<char, byte>('‒', 0x2D);
            yield return new KeyValuePair<char, byte>('–', 0x2D);
            yield return new KeyValuePair<char, byte>('—', 0x2D);
            yield return new KeyValuePair<char, byte>('―', 0x2D);

            yield return new KeyValuePair<char, byte>('.', 0x2E);

            yield return new KeyValuePair<char, byte>('/', 0x2F);
            // Variants
            yield return new KeyValuePair<char, byte>('⁄', 0x2F);

            yield return new KeyValuePair<char, byte>('0', 0x30);
            yield return new KeyValuePair<char, byte>('1', 0x31);
            yield return new KeyValuePair<char, byte>('2', 0x32);
            yield return new KeyValuePair<char, byte>('3', 0x33);
            yield return new KeyValuePair<char, byte>('4', 0x34);
            yield return new KeyValuePair<char, byte>('5', 0x35);
            yield return new KeyValuePair<char, byte>('6', 0x36);
            yield return new KeyValuePair<char, byte>('7', 0x37);
            yield return new KeyValuePair<char, byte>('8', 0x38);
            yield return new KeyValuePair<char, byte>('9', 0x39);
            yield return new KeyValuePair<char, byte>(':', 0x3A);
            yield return new KeyValuePair<char, byte>(';', 0x3B);

            yield return new KeyValuePair<char, byte>('<', 0x3C);
            // Variant
            yield return new KeyValuePair<char, byte>('‹', 0x3C);

            yield return new KeyValuePair<char, byte>('=', 0x3D);
            // Variant
            yield return new KeyValuePair<char, byte>('゠', 0x3D);

            yield return new KeyValuePair<char, byte>('>', 0x3E);
            // Variant
            yield return new KeyValuePair<char, byte>('›', 0x3E);

            yield return new KeyValuePair<char, byte>('?', 0x3F);
            // Variant
            yield return new KeyValuePair<char, byte>('¿', 0x3F);

            yield return new KeyValuePair<char, byte>('@', 0x40);

            yield return new KeyValuePair<char, byte>('A', 0x41);
            // Variants
            yield return new KeyValuePair<char, byte>('À', 0x41);
            yield return new KeyValuePair<char, byte>('Á', 0x41);
            yield return new KeyValuePair<char, byte>('Â', 0x41);
            yield return new KeyValuePair<char, byte>('Ã', 0x41);
            yield return new KeyValuePair<char, byte>('Ä', 0x41);
            yield return new KeyValuePair<char, byte>('Å', 0x41);

            yield return new KeyValuePair<char, byte>('B', 0x42);

            yield return new KeyValuePair<char, byte>('C', 0x43);
            // Variant
            yield return new KeyValuePair<char, byte>('Ç', 0x43);

            yield return new KeyValuePair<char, byte>('D', 0x44);

            yield return new KeyValuePair<char, byte>('E', 0x45);
            // Variants
            yield return new KeyValuePair<char, byte>('È', 0x45);
            yield return new KeyValuePair<char, byte>('É', 0x45);
            yield return new KeyValuePair<char, byte>('Ê', 0x45);
            yield return new KeyValuePair<char, byte>('Ë', 0x45);

            yield return new KeyValuePair<char, byte>('F', 0x46);
            yield return new KeyValuePair<char, byte>('G', 0x47);
            yield return new KeyValuePair<char, byte>('H', 0x48);

            yield return new KeyValuePair<char, byte>('I', 0x49);
            // Variants
            yield return new KeyValuePair<char, byte>('Ì', 0x49);
            yield return new KeyValuePair<char, byte>('Í', 0x49);
            yield return new KeyValuePair<char, byte>('Î', 0x49);
            yield return new KeyValuePair<char, byte>('Ï', 0x49);

            yield return new KeyValuePair<char, byte>('J', 0x4A);
            yield return new KeyValuePair<char, byte>('K', 0x4B);
            yield return new KeyValuePair<char, byte>('L', 0x4C);
            yield return new KeyValuePair<char, byte>('M', 0x4D);

            yield return new KeyValuePair<char, byte>('N', 0x4E);
            // Variant
            yield return new KeyValuePair<char, byte>('Ñ', 0x4E);

            yield return new KeyValuePair<char, byte>('O', 0x4F);
            // Variants
            yield return new KeyValuePair<char, byte>('Ò', 0x4F);
            yield return new KeyValuePair<char, byte>('Ó', 0x4F);
            yield return new KeyValuePair<char, byte>('Ô', 0x4F);
            yield return new KeyValuePair<char, byte>('Õ', 0x4F);
            yield return new KeyValuePair<char, byte>('Ö', 0x4F);
            yield return new KeyValuePair<char, byte>('Ø', 0x4F);

            yield return new KeyValuePair<char, byte>('P', 0x50);
            yield return new KeyValuePair<char, byte>('Q', 0x51);
            yield return new KeyValuePair<char, byte>('R', 0x52);
            yield return new KeyValuePair<char, byte>('S', 0x53);
            yield return new KeyValuePair<char, byte>('T', 0x54);

            yield return new KeyValuePair<char, byte>('U', 0x55);
            // Variants
            yield return new KeyValuePair<char, byte>('Ù', 0x55);
            yield return new KeyValuePair<char, byte>('Ú', 0x55);
            yield return new KeyValuePair<char, byte>('Û', 0x55);
            yield return new KeyValuePair<char, byte>('Ü', 0x55);

            yield return new KeyValuePair<char, byte>('V', 0x56);
            yield return new KeyValuePair<char, byte>('W', 0x57);
            yield return new KeyValuePair<char, byte>('X', 0x58);

            yield return new KeyValuePair<char, byte>('Y', 0x59);
            // Variant
            yield return new KeyValuePair<char, byte>('Ý', 0x59);

            yield return new KeyValuePair<char, byte>('Z', 0x5A);
            yield return new KeyValuePair<char, byte>('[', 0x5B);
            yield return new KeyValuePair<char, byte>('¥', 0x5C);
            yield return new KeyValuePair<char, byte>(']', 0x5D);
            yield return new KeyValuePair<char, byte>('^', 0x5E);

            yield return new KeyValuePair<char, byte>('_', 0x5F);
            // Variant
            yield return new KeyValuePair<char, byte>('‗', 0x5F);

            yield return new KeyValuePair<char, byte>('`', 0x60);

            yield return new KeyValuePair<char, byte>('a', 0x61);
            // Variants
            yield return new KeyValuePair<char, byte>('à', 0x61);
            yield return new KeyValuePair<char, byte>('á', 0x61);
            yield return new KeyValuePair<char, byte>('â', 0x61);
            yield return new KeyValuePair<char, byte>('ã', 0x61);
            yield return new KeyValuePair<char, byte>('å', 0x61);

            yield return new KeyValuePair<char, byte>('b', 0x62);

            yield return new KeyValuePair<char, byte>('c', 0x63);
            // Variant
            yield return new KeyValuePair<char, byte>('ç', 0x63);

            yield return new KeyValuePair<char, byte>('d', 0x64);

            yield return new KeyValuePair<char, byte>('e', 0x65);
            // Variants
            yield return new KeyValuePair<char, byte>('è', 0x65);
            yield return new KeyValuePair<char, byte>('é', 0x65);
            yield return new KeyValuePair<char, byte>('ê', 0x65);
            yield return new KeyValuePair<char, byte>('ë', 0x65);

            yield return new KeyValuePair<char, byte>('f', 0x66);
            yield return new KeyValuePair<char, byte>('g', 0x67);
            yield return new KeyValuePair<char, byte>('h', 0x68);

            yield return new KeyValuePair<char, byte>('i', 0x69);
            // Variants
            yield return new KeyValuePair<char, byte>('ì', 0x69);
            yield return new KeyValuePair<char, byte>('í', 0x69);
            yield return new KeyValuePair<char, byte>('î', 0x69);
            yield return new KeyValuePair<char, byte>('ï', 0x69);

            yield return new KeyValuePair<char, byte>('j', 0x6A);
            yield return new KeyValuePair<char, byte>('k', 0x6B);
            yield return new KeyValuePair<char, byte>('l', 0x6C);
            yield return new KeyValuePair<char, byte>('m', 0x6D);

            yield return new KeyValuePair<char, byte>('n', 0x6E);
            // Variant
            yield return new KeyValuePair<char, byte>('ñ', 0x6E);

            yield return new KeyValuePair<char, byte>('o', 0x6F);
            // Variants
            yield return new KeyValuePair<char, byte>('ò', 0x6F);
            yield return new KeyValuePair<char, byte>('ó', 0x6F);
            yield return new KeyValuePair<char, byte>('ô', 0x6F);
            yield return new KeyValuePair<char, byte>('õ', 0x6F);
            yield return new KeyValuePair<char, byte>('ö', 0x6F);
            yield return new KeyValuePair<char, byte>('ø', 0x6F);

            yield return new KeyValuePair<char, byte>('p', 0x70);
            yield return new KeyValuePair<char, byte>('q', 0x71);
            yield return new KeyValuePair<char, byte>('r', 0x72);
            yield return new KeyValuePair<char, byte>('s', 0x73);
            yield return new KeyValuePair<char, byte>('t', 0x74);

            yield return new KeyValuePair<char, byte>('u', 0x75);
            // Variants
            yield return new KeyValuePair<char, byte>('ù', 0x75);
            yield return new KeyValuePair<char, byte>('ú', 0x75);
            yield return new KeyValuePair<char, byte>('û', 0x75);
            yield return new KeyValuePair<char, byte>('ü', 0x75);

            yield return new KeyValuePair<char, byte>('v', 0x76);
            yield return new KeyValuePair<char, byte>('w', 0x77);
            yield return new KeyValuePair<char, byte>('x', 0x78);

            yield return new KeyValuePair<char, byte>('y', 0x79);
            // Variants
            yield return new KeyValuePair<char, byte>('ý', 0x79);
            yield return new KeyValuePair<char, byte>('ÿ', 0x79);

            yield return new KeyValuePair<char, byte>('z', 0x7A);
            yield return new KeyValuePair<char, byte>('{', 0x7B);
            yield return new KeyValuePair<char, byte>('|', 0x7C);
            yield return new KeyValuePair<char, byte>('}', 0x7D);
            yield return new KeyValuePair<char, byte>('→', 0x7E);
            yield return new KeyValuePair<char, byte>('←', 0x7F);

            yield return new KeyValuePair<char, byte>(' ', 0x80);
            yield return new KeyValuePair<char, byte>(' ', 0x81);
            yield return new KeyValuePair<char, byte>(' ', 0x82);
            yield return new KeyValuePair<char, byte>(' ', 0x83);
            yield return new KeyValuePair<char, byte>(' ', 0x84);
            yield return new KeyValuePair<char, byte>(' ', 0x85);
            yield return new KeyValuePair<char, byte>(' ', 0x86);
            yield return new KeyValuePair<char, byte>(' ', 0x87);
            yield return new KeyValuePair<char, byte>(' ', 0x88);
            yield return new KeyValuePair<char, byte>(' ', 0x89);
            yield return new KeyValuePair<char, byte>(' ', 0x8A);
            yield return new KeyValuePair<char, byte>(' ', 0x8B);
            yield return new KeyValuePair<char, byte>(' ', 0x8C);
            yield return new KeyValuePair<char, byte>(' ', 0x8D);
            yield return new KeyValuePair<char, byte>(' ', 0x8E);
            yield return new KeyValuePair<char, byte>(' ', 0x8F);

            yield return new KeyValuePair<char, byte>(' ', 0x90);
            yield return new KeyValuePair<char, byte>(' ', 0x91);
            yield return new KeyValuePair<char, byte>(' ', 0x92);
            yield return new KeyValuePair<char, byte>(' ', 0x93);
            yield return new KeyValuePair<char, byte>(' ', 0x94);
            yield return new KeyValuePair<char, byte>(' ', 0x95);
            yield return new KeyValuePair<char, byte>(' ', 0x96);
            yield return new KeyValuePair<char, byte>(' ', 0x97);
            yield return new KeyValuePair<char, byte>(' ', 0x98);
            yield return new KeyValuePair<char, byte>(' ', 0x99);
            yield return new KeyValuePair<char, byte>(' ', 0x9A);
            yield return new KeyValuePair<char, byte>(' ', 0x9B);
            yield return new KeyValuePair<char, byte>(' ', 0x9C);
            yield return new KeyValuePair<char, byte>(' ', 0x9D);
            yield return new KeyValuePair<char, byte>(' ', 0x9E);
            yield return new KeyValuePair<char, byte>(' ', 0x9F);

            yield return new KeyValuePair<char, byte>(' ', 0xA0);
            yield return new KeyValuePair<char, byte>('▫', 0xA1);
//            yield return new KeyValuePair<char, byte>('', 0xA2);
//            yield return new KeyValuePair<char, byte>('', 0xA3);
            yield return new KeyValuePair<char, byte>('ヽ', 0xA4);
            yield return new KeyValuePair<char, byte>('・', 0xA5);
            yield return new KeyValuePair<char, byte>('ヲ', 0xA6);
            yield return new KeyValuePair<char, byte>('ァ', 0xA7);
            yield return new KeyValuePair<char, byte>('ィ', 0xA8);
            yield return new KeyValuePair<char, byte>('ゥ', 0xA9);
            yield return new KeyValuePair<char, byte>('ェ', 0xAA);
            yield return new KeyValuePair<char, byte>('ォ', 0xAB);
            yield return new KeyValuePair<char, byte>('ャ', 0xAC);
            yield return new KeyValuePair<char, byte>('ュ', 0xAD);
            yield return new KeyValuePair<char, byte>('ョ', 0xAE);
            yield return new KeyValuePair<char, byte>('ッ', 0xAF);

            yield return new KeyValuePair<char, byte>('ー', 0xB0);
            yield return new KeyValuePair<char, byte>('ア', 0xB1);
            yield return new KeyValuePair<char, byte>('イ', 0xB2);
            yield return new KeyValuePair<char, byte>('ウ', 0xB3);
            yield return new KeyValuePair<char, byte>('エ', 0xB4);
            yield return new KeyValuePair<char, byte>('オ', 0xB5);
            yield return new KeyValuePair<char, byte>('カ', 0xB6);
            yield return new KeyValuePair<char, byte>('キ', 0xB7);
            yield return new KeyValuePair<char, byte>('ク', 0xB8);
            yield return new KeyValuePair<char, byte>('ケ', 0xB9);
            yield return new KeyValuePair<char, byte>('コ', 0xBA);
            yield return new KeyValuePair<char, byte>('サ', 0xBB);
            yield return new KeyValuePair<char, byte>('シ', 0xBC);
            yield return new KeyValuePair<char, byte>('ス', 0xBD);
            yield return new KeyValuePair<char, byte>('セ', 0xBE);
            yield return new KeyValuePair<char, byte>('ソ', 0xBF);

            yield return new KeyValuePair<char, byte>('タ', 0xC0);
            yield return new KeyValuePair<char, byte>('チ', 0xC1);
            yield return new KeyValuePair<char, byte>('ツ', 0xC2);
            yield return new KeyValuePair<char, byte>('テ', 0xC3);
            yield return new KeyValuePair<char, byte>('ト', 0xC4);
            yield return new KeyValuePair<char, byte>('ナ', 0xC5);
            yield return new KeyValuePair<char, byte>('ニ', 0xC6);
            yield return new KeyValuePair<char, byte>('ヌ', 0xC7);
            yield return new KeyValuePair<char, byte>('ネ', 0xC8);
            yield return new KeyValuePair<char, byte>('ノ', 0xC9);
            yield return new KeyValuePair<char, byte>('ハ', 0xCA);
            yield return new KeyValuePair<char, byte>('ヒ', 0xCB);
            yield return new KeyValuePair<char, byte>('フ', 0xCC);
            yield return new KeyValuePair<char, byte>('ヘ', 0xCD);
            yield return new KeyValuePair<char, byte>('ホ', 0xCE);
            yield return new KeyValuePair<char, byte>('マ', 0xCF);

            yield return new KeyValuePair<char, byte>('ミ', 0xD0);
            yield return new KeyValuePair<char, byte>('ム', 0xD1);
            yield return new KeyValuePair<char, byte>('メ', 0xD2);
            yield return new KeyValuePair<char, byte>('モ', 0xD3);
            yield return new KeyValuePair<char, byte>('ヤ', 0xD4);
            yield return new KeyValuePair<char, byte>('ユ', 0xD5);
            yield return new KeyValuePair<char, byte>('ヨ', 0xD6);
            yield return new KeyValuePair<char, byte>('ラ', 0xD7);
            yield return new KeyValuePair<char, byte>('リ', 0xD8);
            yield return new KeyValuePair<char, byte>('ル', 0xD9);
            yield return new KeyValuePair<char, byte>('レ', 0xDA);
            yield return new KeyValuePair<char, byte>('ロ', 0xDB);
            yield return new KeyValuePair<char, byte>('ワ', 0xDC);
            yield return new KeyValuePair<char, byte>('ン', 0xDD);

            yield return new KeyValuePair<char, byte>('゛', 0xDE);

            yield return new KeyValuePair<char, byte>('゜', 0xDF);
            // Variant
            yield return new KeyValuePair<char, byte>('°', 0xDF);

            yield return new KeyValuePair<char, byte>('α', 0xE0);

            yield return new KeyValuePair<char, byte>('ä', 0xE1);
            // Variant
            yield return new KeyValuePair<char, byte>('ӓ', 0xE1);

            yield return new KeyValuePair<char, byte>('β', 0xE2);
            // Variant
            yield return new KeyValuePair<char, byte>('ß', 0xE2);

            yield return new KeyValuePair<char, byte>('ε', 0xE3);
            yield return new KeyValuePair<char, byte>('μ', 0xE4);
            yield return new KeyValuePair<char, byte>('σ', 0xE5);
            yield return new KeyValuePair<char, byte>('ρ', 0xE6);
            yield return new KeyValuePair<char, byte>('ɡ', 0xE7);
            yield return new KeyValuePair<char, byte>('√', 0xE8);
//            yield return new KeyValuePair<char, byte>('', 0xE9);
            yield return new KeyValuePair<char, byte>('ј', 0xEA);
            yield return new KeyValuePair<char, byte>('\u033D', 0xEB);

            yield return new KeyValuePair<char, byte>('¢', 0xEC);
            // Variants
            yield return new KeyValuePair<char, byte>('\u023B', 0xEC);
            yield return new KeyValuePair<char, byte>('￠', 0xEC);

//            yield return new KeyValuePair<char, byte>('', 0xED);
            yield return new KeyValuePair<char, byte>('ñ', 0xEE);
            yield return new KeyValuePair<char, byte>('ö', 0xEF);

            yield return new KeyValuePair<char, byte>('ρ', 0xF0);
//            yield return new KeyValuePair<char, byte>('', 0xF1);
            yield return new KeyValuePair<char, byte>('θ', 0xF2);
            yield return new KeyValuePair<char, byte>('∞', 0xF3);
            yield return new KeyValuePair<char, byte>('Ω', 0xF4);
            yield return new KeyValuePair<char, byte>('ü', 0xF5);
            yield return new KeyValuePair<char, byte>('Σ', 0xF6);
            yield return new KeyValuePair<char, byte>('π', 0xF7);
//            yield return new KeyValuePair<char, byte>('', 0xF8);

            yield return new KeyValuePair<char, byte>('У', 0xF9);
            // Variant
            yield return new KeyValuePair<char, byte>('у', 0xF9);

//            yield return new KeyValuePair<char, byte>('', 0xFA);
//            yield return new KeyValuePair<char, byte>('', 0xFB);
//            yield return new KeyValuePair<char, byte>('', 0xFC);
            yield return new KeyValuePair<char, byte>('÷', 0xFD);
            yield return new KeyValuePair<char, byte>(' ', 0xFE);
            yield return new KeyValuePair<char, byte>('█', 0xFF);
        }

        #endregion
    }
}