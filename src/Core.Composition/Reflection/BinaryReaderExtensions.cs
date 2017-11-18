namespace CustomCode.Core.Composition.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Extension methods for the <see cref="BinaryReader"/> type.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        #region Logic

        /// <summary>
        /// Read a string that is terminated by a '\0' character.
        /// </summary>
        /// <param name="reader"> The reader to be read from. </param>
        /// <returns> The read string value (without any trailing '\0' characters). </returns>

        public static string ReadNullTermString(this BinaryReader reader)
        {
            var buffer = new List<char>();
            char current;

            while ((current = reader.ReadChar()) != '\0')
            {
                buffer.Add(current);
            }

            return new String(buffer.ToArray());
        }

        /// <summary>
        /// Read a string that is terminated by a '\0' character and aligned to the next 4-byte boundary.
        /// </summary>
        /// <param name="reader"> The reader to be read from. </param>
        /// <returns> The read string value (without any trailing '\0' characters). </returns>
        public static string ReadNullTerminatedFourByteAlignedString(this BinaryReader reader)
        {
            var buffer = new List<char>();
            char nextChar;

            do
            {
                nextChar = reader.ReadChar();
                buffer.Add(nextChar);
            } while (nextChar != '\0' || reader.BaseStream.Position % 4 != 0);

            return new String(buffer.TakeWhile(b => !b.Equals('\0')).ToArray());
        }

        #endregion
    }
}