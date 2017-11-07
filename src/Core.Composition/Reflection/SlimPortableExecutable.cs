namespace CustomCode.Core.Composition.Reflection
{
    using System;
    using System.IO;

    /// <summary>
    /// A minimal (and by no means complete) implemenation of a parser for the portable executable
    /// file format. This type is used as replacement for the missing AppDomain and ReflectionOnlyLoad
    /// capacities of .Net Standard.
    /// </summary>
    public sealed class SlimPortableExecutable
    {
        #region Dependencies

        /// <summary>
        /// Creates a new instance of the <see cref="SlimPortableExecutable"/> type.
        /// </summary>
        /// <param name="path"> The (full) path of the portable executale file. </param>
        public SlimPortableExecutable(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;
        }

        /// <summary>
        /// Gets the (full) path of the portable executale file.
        /// </summary>
        public string Path { get; }

        #endregion

        #region Logic

        /// <summary>
        /// Check if the underlying file is a valid portable executable by checking the ms-dos header
        /// as well as the pe-header signatures.
        /// </summary>
        /// <returns> True if the file is a valid portable executable, false otherwise. </returns>
        public bool IsValidPortableExecutable()
        {
            if (File.Exists(Path))
            {
                using (var stream = File.OpenRead(Path))
                using (var reader = new BinaryReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    return IsValidPortableExecutable(reader);
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the underlying file is a valid portable executable by checking the ms-dos header
        /// as well as the pe-header signatures.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <returns> True if the file is a valid portable executable, false otherwise. </returns>
        private bool IsValidPortableExecutable(BinaryReader reader)
        {
            // should at least have enough space to contain a dos-header
            if (reader.BaseStream.Length < 64)
            {
                return false;
            }

            // check the "magic" MZ (5a 4d) bytes that are used as a signature for the dos-header
            var dosSignature = reader.ReadUInt16();
            if (dosSignature != 0x5a4d)
            {
                return false;
            }

            // move the stream to the pe-header offset (inside of the dos-header, always located at byte 60)
            reader.BaseStream.Position = 60;
            var peHeaderOffset = reader.ReadUInt32();
            if (peHeaderOffset == 0)
            {
                peHeaderOffset = 0x80;
            }

            // Ensure there is at least enough room for the following structures:
            //     24 byte PE Signature & Header
            //     28 byte Standard Fields         (24 bytes for PE32+)
            //     68 byte NT Fields               (88 bytes for PE32+)
            // >= 128 byte Data Dictionary Table
            if (peHeaderOffset > reader.BaseStream.Length - 256)
            {
                return false;
            }

            // Check the PE signature.  Should equal 'PE\0\0'.
            reader.BaseStream.Position = peHeaderOffset;
            var peSignature = reader.ReadUInt32();
            if (peSignature != 0x00004550)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}