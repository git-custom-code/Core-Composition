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
        Module                 = 1ul <<  0,
        TypeRef                = 1ul <<  1,
        TypeDef                = 1ul <<  2,
        Field                  = 1ul <<  4,
        Method                 = 1ul <<  6,
        Param                  = 1ul <<  8,
        InterfaceImpl          = 1ul <<  9,
        MemberRef              = 1ul << 10,
        Constant               = 1ul << 11,
        CustomAttribute        = 1ul << 12,
        FieldMarshal           = 1ul << 13,
        DeclSecurity           = 1ul << 14,
        ClassLayout            = 1ul << 15,
        FieldLayout            = 1ul << 16,
        StandAloneSig          = 1ul << 17,
        EventMap               = 1ul << 18,
        Event                  = 1ul << 20,
        PropertyMap            = 1ul << 21,
        Property               = 1ul << 23,
        MethodSemantics        = 1ul << 24,
        MethodImpl             = 1ul << 25,
        ModuleRef              = 1ul << 26,
        TypeSpec               = 1ul << 27,
        ImplMap                = 1ul << 28,
        FieldRVA               = 1ul << 29,
        Assembly               = 1ul << 32,
        AssemblyProcessor      = 1ul << 33,
        AssemblyOS             = 1ul << 34,
        AssemblyRef            = 1ul << 35,
        AssemblyRefProcessor   = 1ul << 36,
        AssemblyRefOS          = 1ul << 37,
        File                   = 1ul << 38,
        ExportedType           = 1ul << 39,
        ManifestResource       = 1ul << 40,
        NestedClass            = 1ul << 41,
        GenericParam           = 1ul << 42,
        MethodSpec             = 1ul << 43,
        GenericParamConstraint = 1ul << 44,
    }
}