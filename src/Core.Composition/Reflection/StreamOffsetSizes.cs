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
        String = 0x01,
        Guid = 0x02,
        Blob = 0x04,
    }
}