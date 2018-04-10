namespace CustomCode.Core.Composition.Reflection
{
    using System;

    /// <summary>
    /// Enumeration that defines the id's of metadata tables stored in the metadatastream of a portable executable file.
    /// </summary>
    /// <remarks>
    /// See partition II of the common language infrastructure for more details:
    /// https://www.visualstudio.com/license-terms/ecma-c-common-language-infrastructure-standards/
    /// </remarks>
    [Flags]
    public enum MetadataTable : ulong
    {
        /// <summary> The Module metadata table. </summary>
        Module = 1ul <<  0,
        /// <summary> The TypeRef metadata table. </summary>
        TypeRef = 1ul <<  1,
        /// <summary> The TypeDef metadata table. </summary>
        TypeDef = 1ul <<  2,
        /// <summary> The Field metadata table. </summary>
        Field = 1ul <<  4,
        /// <summary> The Method metadata table. </summary>
        Method = 1ul <<  6,
        /// <summary> The Param metadata table. </summary>
        Param = 1ul <<  8,
        /// <summary> The InterfaceImpl metadata table. </summary>
        InterfaceImpl = 1ul <<  9,
        /// <summary> The MemberRef metadata table. </summary>
        MemberRef = 1ul << 10,
        /// <summary> The Constant metadata table. </summary>
        Constant = 1ul << 11,
        /// <summary> The CustomAttribute metadata table. </summary>
        CustomAttribute = 1ul << 12,
        /// <summary> The FieldMarshal metadata table. </summary>
        FieldMarshal = 1ul << 13,
        /// <summary> The DeclSecurity metadata table. </summary>
        DeclSecurity = 1ul << 14,
        /// <summary> The ClassLayout metadata table. </summary>
        ClassLayout = 1ul << 15,
        /// <summary> The FieldLayout metadata table. </summary>
        FieldLayout = 1ul << 16,
        /// <summary> The StandAloneSig metadata table. </summary>
        StandAloneSig = 1ul << 17,
        /// <summary> The EventMap metadata table. </summary>
        EventMap = 1ul << 18,
        /// <summary> The Event metadata table. </summary>
        Event = 1ul << 20,
        /// <summary> The PropertyMap metadata table. </summary>
        PropertyMap = 1ul << 21,
        /// <summary> The Property metadata table. </summary>
        Property = 1ul << 23,
        /// <summary> The MethodSemantics metadata table. </summary>
        MethodSemantics = 1ul << 24,
        /// <summary> The MethodImpl metadata table. </summary>
        MethodImpl = 1ul << 25,
        /// <summary> The ModuleRef metadata table. </summary>
        ModuleRef = 1ul << 26,
        /// <summary> The TypeSpec metadata table. </summary>
        TypeSpec = 1ul << 27,
        /// <summary> The ImplMap metadata table. </summary>
        ImplMap = 1ul << 28,
        /// <summary> The FieldRVA metadata table. </summary>
        FieldRVA = 1ul << 29,
        /// <summary> The Assembly metadata table. </summary>
        Assembly = 1ul << 32,
        /// <summary> The AssemblyProcessor metadata table. </summary>
        AssemblyProcessor = 1ul << 33,
        /// <summary> The AssemblyOS metadata table. </summary>
        AssemblyOS = 1ul << 34,
        /// <summary> The AssemblyRef metadata table. </summary>
        AssemblyRef = 1ul << 35,
        /// <summary> The AssemblyRefProcessor metadata table. </summary>
        AssemblyRefProcessor = 1ul << 36,
        /// <summary> The AssemblyRefOS metadata table. </summary>
        AssemblyRefOS = 1ul << 37,
        /// <summary> The File metadata table. </summary>
        File = 1ul << 38,
        /// <summary> The ExportedType metadata table. </summary>
        ExportedType = 1ul << 39,
        /// <summary> The ManifestResource metadata table. </summary>
        ManifestResource = 1ul << 40,
        /// <summary> The NestedClass metadata table. </summary>
        NestedClass = 1ul << 41,
        /// <summary> The GenericParam metadata table. </summary>
        GenericParam = 1ul << 42,
        /// <summary> The MethodSpec metadata table. </summary>
        MethodSpec = 1ul << 43,
        /// <summary> The GenericParamConstraint metadata table. </summary>
        GenericParamConstraint = 1ul << 44,
    }
}