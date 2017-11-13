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

        /// <summary>
        /// Gets the collection of stream headers within the portable executable's clr section.
        /// </summary>
        private List<StreamHeader> StreamHeaders { get; } = new List<StreamHeader>();

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
        /// Check if the underlying file is a valid .net assembly by checking the ms-dos header
        /// as well as the pe-header signatures and the clr header.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <returns> True if the file is a valid .net assembly, false otherwise. </returns>

        private bool IsValidNetAssembly(BinaryReader reader)
        {
            var numberOfSections = ReadCoffHeader(reader);
            var dataDirectoryCount = ReadOptionalHeader(reader);
            if (dataDirectoryCount < 15)
            {
                return false;
            }

            var clr = ReadDataDirectories(reader, dataDirectoryCount);
            if (clr.size != 72)
            {
                return false;
            }

            ReadSectionHeaders(reader, numberOfSections);
            var metadata = ReadClrHeader(reader, clr.virtualAddress);
            if (metadata.headerSize != 72 || metadata.address == 0 || metadata.size == 0)
            {
                return false;
            }

            var numberOfStreams = ReadMetadataHeader(reader, metadata.address);
            if (numberOfStreams == 0)
            {
                return false;
            }

            ReadStreamHeader(reader, numberOfStreams, metadata.address);
            return true;
        }

        /// <summary>
        /// Read the relevant bits of the portable executable's coff header.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <returns> The number of <see cref="SectionHeaders"/> stored inside of the portable executable. </returns>
        private uint ReadCoffHeader(BinaryReader reader)
        {
            reader.BaseStream.Position += 2;
            var numberOfSections = reader.ReadUInt16();
            reader.BaseStream.Position += 16;
            return numberOfSections;
        }

        /// <summary>
        /// Read the relevant bits of the portable executable's optional header.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <returns> The number of data directories stored inside of the portable executable. </returns>
        private uint ReadOptionalHeader(BinaryReader reader)
        {
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
            return dataDirectoryCount;
        }

        /// <summary>
        /// Read the relevant bits of the portable executable's data directories (i.e. the location of the clr data directory).
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <param name="dataDirectoryCount"> The number of data directories as stored in the optional header. </param>
        /// <returns> The relative virtual address and size of the clr data directory. </returns>
        private (uint virtualAddress, uint size) ReadDataDirectories(BinaryReader reader, uint dataDirectoryCount)
        {
            reader.BaseStream.Position += 112; // skip first 14 directories 
            var clrVirtualAddress = reader.ReadUInt32();
            var clrSize = reader.ReadUInt32();
            reader.BaseStream.Position += (dataDirectoryCount - 15) * 8;
            return (clrVirtualAddress, clrSize);
        }

        /// <summary>
        /// Read the relevant bits of the portable executable's section headers (see <see cref="SlimSectionHeader"/> for more details).
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <param name="numberOfSections"> The number of section headers as stored in the coff header. </param>
        /// <remarks> Calling this method will populate the <see cref="SectionHeaders"/> property. </remarks>
        private void ReadSectionHeaders(BinaryReader reader, uint numberOfSections)
        {
            SectionHeaders.Clear();
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

        /// <summary>
        /// Read the relevant bits of the portable executable's clr header.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <param name="clrVirtualAddress"> The relative virtual address of the clr header as stored in the clr data directory. </param>
        /// <returns> The size and relative virtual address of the metadata header and directory. </returns>
        private (uint headerSize, uint address, uint size) ReadClrHeader(BinaryReader reader, uint clrVirtualAddress)
        {
            uint clrHeaderOffset;
            if (!TryResolveAddress(clrVirtualAddress, out clrHeaderOffset))
            {
                return (0, 0, 0);
            }

            reader.BaseStream.Position = clrHeaderOffset;

            var headerSize = reader.ReadUInt32();
            reader.BaseStream.Position += 4;
            var metaDataDirectoryAddress = reader.ReadUInt32();
            var metaDataDirectorySize = reader.ReadUInt32();

            return (headerSize, metaDataDirectoryAddress, metaDataDirectorySize);
        }

        /// <summary>
        /// Read the relevant bits of the portable executable's metadata header.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <param name="metaDataDirectoryAddress"> The relative virtual address of the metadata directory as storeed in the clr header. </param>
        /// <returns> The number of metadata streams stored in the portable executable. </returns>
        private uint ReadMetadataHeader(BinaryReader reader, uint metaDataDirectoryAddress)
        {
            uint metadataOffset;
            if (!TryResolveAddress(metaDataDirectoryAddress, out metadataOffset))
            {
                return 0;
            }

            reader.BaseStream.Position = metadataOffset;
            var metadataSignature = reader.ReadUInt32();
            if (metadataSignature != 0x424A5342)
            {
                return 0;
            }

            reader.BaseStream.Position += 8;
            var versionStringLength = reader.ReadUInt32();
            reader.BaseStream.Position += (versionStringLength + 2);
            var numberOfStreams = reader.ReadUInt16();
            return numberOfStreams;
        }

        /// <summary>
        /// Read the relevant bits of the portable executable's metadata stream header.
        /// </summary>
        /// <param name="reader"> The reader to the underlying file. </param>
        /// <param name="numberOfStreams"> The number of metadata streams as stored in the metadata header. </param>
        /// <param name="metaDataDirectoryAddress"> The relative virtual address of the metadata directory. </param>
        private void ReadStreamHeader(BinaryReader reader, uint numberOfStreams, uint metaDataDirectoryAddress)
        {
            StreamHeaders.Clear();
            for (var i = 0; i < numberOfStreams; ++i)
            {
                StreamHeaders.Add(new StreamHeader(
                    metaDataDirectoryAddress + reader.ReadUInt32(),
                    reader.ReadUInt32(),
                    reader.ReadNullTerminatedFourByteAlignedString()));
            }
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