namespace CustomCode.Core.Composition.Reflection
{
    using System;

    /// <summary>
    /// Enumeration that codes if string, guid or blob stream indices are 2 or 4 byte long.
    /// </summary>
    /// <remarks>
    /// See partition II of the common language infrastructure for more details:
    /// https://www.visualstudio.com/license-terms/ecma-c-common-language-infrastructure-standards/
    /// </remarks>
    [Flags]
    public enum StreamOffsetSizes : byte
    {
        /// <summary> The index size for the string stream. </summary>
        String = 0x01,
        /// <summary> The index size for the guid stream. </summary>
        Guid = 0x02,
        /// <summary> The index size for the blob stream. </summary>
        Blob = 0x04,
    }
}