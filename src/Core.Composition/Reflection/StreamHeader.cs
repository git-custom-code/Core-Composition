namespace CustomCode.Core.Composition.Reflection
{
    /// <summary>
    /// This type represents a stream header inside of the clr section of a portable executable.
    /// </summary>
    public sealed class StreamHeader
    {
        #region Dependencies

        /// <summary>
        /// Creates a new instance of the <see cref="StreamHeader"/> type.
        /// </summary>
        /// <param name="relativeVirtualAddress"> The header's relative virtual address. </param>
        /// <param name="size"> The size of the header in bytes. </param>
        /// <param name="name"> The unique name of the header. </param>
        public StreamHeader(uint relativeVirtualAddress, uint size, string name)
        {
            Name = name;
            RelativeVirtualAddress = relativeVirtualAddress;
            Size = size;
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets the unique name of the header.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the header's relative virtual address.
        /// </summary>
        public uint RelativeVirtualAddress { get; }

        /// <summary>
        /// Gets the size of the header in bytes.
        /// </summary>
        public uint Size { get; }

        #endregion

        #region Logic

        /// <summary>
        /// Convert this instance to a human readable string representation.
        /// </summary>
        /// <returns> A human readable string representation of this instance. </returns>
        public override string ToString()
        {
            return Name;
        }

        #endregion
    }
}