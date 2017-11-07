namespace CustomCode.Core.Composition.Reflection
{
    public sealed class SlimSectionHeader
    {
        #region Dependencies

        /// <summary>
        /// Standard ctor.
        /// </summary>
        /// <param name="virtualAddress"> The address of the first byte of the section relative to the image base. </param>
        /// <param name="sizeOfRawData"> The size of the section in byte. </param>
        /// <param name="pointerToRawData"> The file pointer to the first page of the section within the COFF file. </param>
        public SlimSectionHeader(uint virtualAddress, uint sizeOfRawData, uint pointerToRawData)
        {
            PointerToRawData = pointerToRawData;
            SizeOfRawData = sizeOfRawData;
            VirtualAddress = virtualAddress;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the size of the section in byte.
        /// </summary>
        public uint SizeOfRawData { get; }

        /// <summary>
        /// Gets the file pointer to the first page of the section within the COFF file.
        /// </summary>
        public uint PointerToRawData { get; }

        /// <summary>
        /// Gets the address of the first byte of the section relative to the image.
        /// </summary>
        public uint VirtualAddress { get; }

        #endregion

        #region Logic

        /// <summary>
        /// Try to resolve a relative virtual address that points to a location within this section
        /// to an absolute file address.
        /// </summary>
        /// <param name="relativeAddress"> The relative virtual address to be resolved. </param>
        /// <param name="absoluteAddress"> The absolute file address or 0. </param>
        /// <returns> True if the address could be resolved, false otherwise. </returns>
        public bool TryResolveAddress(uint relativeAddress, out uint absoluteAddress)
        {
            if (VirtualAddress <= relativeAddress && VirtualAddress + SizeOfRawData >= relativeAddress)
            {
                absoluteAddress = PointerToRawData + (relativeAddress - VirtualAddress);
                return true;
            }

            absoluteAddress = 0;
            return false;
        }

        #endregion
    }
}