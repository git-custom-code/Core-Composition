namespace CustomCode.Core.Composition.Reflection
{
    using System;
    using System.Collections.Generic;
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

        #region Data

        /// <summary>
        /// Gets the collection of section headers within the portable executable.
        /// </summary>
        private List<SlimSectionHeader> SectionHeaders { get; } = new List<SlimSectionHeader>();

        #endregion

        #region Logic

        /// <summary>
        /// Check if the underlying file is a valid .net assembly by checking the ms-dos header
        /// as well as the pe-header signatures and the clr header.
        /// </summary>
        /// <returns> True if the file is a valid .net assembly, false otherwise. </returns>
        public bool IsValidNetAssembly()
        {
            if (File.Exists(Path))
            {
                using (var stream = File.OpenRead(Path))
                using (var reader = new BinaryReader(stream))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    if (IsValidPortableExecutable(reader))
                    {
                        return IsValidNetAssembly(reader);
                    }
                }
            }

            return false;
        }

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
        /// Check if the underlying file is a valid .net assembly by checking the ms-dos header
        /// as well as the pe-header signatures and the clr header.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <returns> True if the file is a valid .net assembly, false otherwise. </returns>

        private bool IsValidNetAssembly(BinaryReader reader)
        {
            // coff header
            reader.BaseStream.Position += 2;
            var numberOfSections = reader.ReadUInt16();
            reader.BaseStream.Position += 16;

            // optional header
            var optionalMagic = reader.ReadUInt16();
            var isPe32Image = (optionalMagic == 0x10b);
            if (isPe32Image)
            {
                reader.BaseStream.Position += 90;
            }
            else
            {
                reader.BaseStream.Position += 86;
            }
            var dataDirectoryCount = reader.ReadUInt32();
            if (dataDirectoryCount < 15)
            {
                return false;
            }

            // read clr data directory
            reader.BaseStream.Position += 112; // skip first 14 directories 
            var clrVirtualAddress = reader.ReadUInt32();
            var clrSize = reader.ReadUInt32();
            if (clrSize != 72) // clr header has a fixed length of 72 bytes
            {
                return false;
            }

            reader.BaseStream.Position += (dataDirectoryCount - 15) * 8;

            // read section headers
            if (SectionHeaders.Count != numberOfSections)
            {
                for (var i = 0; i < numberOfSections; ++i)
                {
                    reader.BaseStream.Position += 12;

                    SectionHeaders.Add(new SlimSectionHeader(
                        reader.ReadUInt32(),
                        reader.ReadUInt32(),
                        reader.ReadUInt32()));

                    reader.BaseStream.Position += 16;
                }
            }

            uint clrHeaderOffset;
            if (!TryResolveAddress(clrVirtualAddress, out clrHeaderOffset))
            {
                return false;
            }

            // read clr header
            reader.BaseStream.Position = clrHeaderOffset;

            var headerSize = reader.ReadUInt32();

            reader.BaseStream.Position += 4;

            var metaDataDirectoryAddress = reader.ReadUInt32();
            var metaDataDirectorySize = reader.ReadUInt32();

            if (headerSize != 72 || metaDataDirectoryAddress == 0 || metaDataDirectorySize == 0)
            {
                return false;
            }

            return true;
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

        /// <summary>
        /// Try to resolve a relative virtual address that points to a location within a section header
        /// to an absolute file address.
        /// </summary>
        /// <param name="relativeAddress"> The relative virtual address to be resolved. </param>
        /// <param name="absoluteAddress"> The absolute file address or 0. </param>
        /// <returns> True if the address could be resolved, false otherwise. </returns>
        private bool TryResolveAddress(uint realtiveAddress, out uint absoluteAddress)
        {
            foreach (var header in SectionHeaders)
            {
                if (header.TryResolveAddress(realtiveAddress, out absoluteAddress))
                {
                    return true;
                }
            }

            absoluteAddress = 0;
            return false;
        }
    }

    #endregion
}