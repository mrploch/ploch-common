using System;
using System.Collections.Generic;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Specifies that an enumeration field has mappings to specified string representations,
///     which can be used for type conversion or other purposes within Windows Management Instrumentation (WMI) objects.
/// </summary>
/// <remarks>
///     This attribute is applicable to enumeration fields and provides a way to assign multiple string representations
///     to a single enum value. It is useful in scenarios where different equivalent representations are required for the same value,
///     such as in handling WMI data or other external string-based data sources.
/// </remarks>
/// <remarks>
///     Initializes a new instance of the <see cref="EnumMappingAttribute" /> class.
///     Defines an attribute used for mapping enumeration fields to one or more string representations.
/// </remarks>
/// <remarks>
///     <para>
///         The <see cref="EnumMappingAttribute" /> can be applied to enumeration fields to associate their values
///         with specific string representations. This is particularly useful when dealing with scenarios like Windows Management
///         Instrumentation (WMI) or other systems where string mappings are required for interoperability or data type conversion.
///     </para>
///     <para>
///         This attribute allows for the configuration of multiple string mappings for a single enum value. It also includes options to:
///         <list type="bullet">
///             <item>
///                 <term>
///                     <see cref="CaseSensitive" />
///                 </term>
///                 <description>Determines whether string comparisons should be case-sensitive.</description>
///             </item>
///             <item>
///                 <term>
///                     <see cref="IncludeActualEnumName" />
///                 </term>
///                 <description>If set to <c>true</c>, includes the enum’s actual name as part of the string mappings.</description>
///             </item>
///         </list>
///     </para>
///     <para>
///         When used with enum fields that belong to external systems or mappings (e.g., translating WMI strings to enum values),
///         this attribute can greatly reduce manual mapping and maintenance efforts while improving consistency.
///     </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public sealed class EnumMappingAttribute(params string?[] names) : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EnumMappingAttribute" /> class.
    ///     Defines an attribute for mapping an enumeration field to one or more string representations.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This attribute is specifically designed for use with enumeration fields in scenarios where certain enum values need
    ///         to be associated with specific string representations. This can be useful for working with external data sources such as
    ///         Windows Management Instrumentation (WMI), where string-based data needs to be mapped to strongly typed enum values.
    ///     </para>
    ///     <para>
    ///         The attribute can handle multiple mappings for a single enum value. It also supports case-sensitivity and an option
    ///         to include the actual enumeration name as part of the mapped strings.
    ///     </para>
    /// </remarks>
    public EnumMappingAttribute() : this([ null ])
    { }

    /// <summary>
    ///     Gets the collection of string values that represent the mapping of
    ///     the enumeration field to different WMI object names.
    /// </summary>
    /// <remarks>
    ///     The <see cref="Names" /> property provides a list of names or mappings
    ///     associated with the enumeration field. These names can be provided
    ///     via the <see cref="EnumMappingAttribute" /> attribute
    ///     and are typically used for resolving or matching WMI data.
    /// </remarks>
    /// <value>
    ///     An <see cref="IEnumerable{T}" /> of nullable strings representing the mapped names.
    /// </value>
    /// <example>
    ///     The <see cref="Names" /> property can be accessed to retrieve all the WMI object mappings
    ///     for a particular enumeration field. These mappings may include additional or alternate
    ///     names that correspond to the enumeration value when querying management objects.
    /// </example>
    /// <seealso cref="EnumMappingAttribute" />
    public IEnumerable<string?> Names { get; } = names;

    /// <summary>
    ///     Gets or sets a value indicating whether string comparisons for this mapping
    ///     are treated as case-sensitive.
    /// </summary>
    /// <remarks>
    ///     The <see cref="CaseSensitive" /> property determines whether the mapping of
    ///     enumeration fields to WMI object names should respect case in string comparisons.
    ///     When set to <c>true</c>, string matching will enforce case-sensitive rules; otherwise,
    ///     comparisons are case-insensitive by default.
    /// </remarks>
    /// <value>
    ///     A <see cref="bool" /> that is <c>true</c> if the string comparisons are
    ///     case-sensitive; otherwise, <c>false</c>.
    /// </value>
    /// <example>
    ///     Use the <see cref="CaseSensitive" /> property to control whether enumeration
    ///     mappings should consider case sensitivity in name matching.
    /// </example>
    /// <seealso cref="EnumMappingAttribute" />
    public bool CaseSensitive { get; set; } = false;

    /// <summary>
    ///     Gets or sets a value indicating whether the actual enumeration field name
    ///     should be included in the mapping alongside additional mappings defined
    ///     through the <see cref="EnumMappingAttribute" />.
    /// </summary>
    /// <remarks>
    ///     The <see cref="IncludeActualEnumName" /> property determines if the enumeration's
    ///     original name should be added to the list of available mappings within
    ///     <see cref="EnumMappingAttribute" />. When set to <c>true</c>,
    ///     the actual enum name is appended to the names list used for WMI object lookups or
    ///     other mapping purposes.
    /// </remarks>
    /// <value>
    ///     A <see cref="bool" /> indicating whether the enumeration field name is included
    ///     in the mapping. Defaults to <c>false</c>.
    /// </value>
    /// <example>
    ///     The <see cref="IncludeActualEnumName" /> property can be used in scenarios where
    ///     both custom names and the original enum names need to be part of WMI-related
    ///     mappings. For example, if the custom attribute specifies additional WMI mappings
    ///     and <see cref="IncludeActualEnumName" /> is <c>true</c>, the enum field itself can
    ///     also be used to map or match corresponding WMI data.
    /// </example>
    /// <seealso cref="EnumMappingAttribute" />
    public bool IncludeActualEnumName { get; set; } = false;
}
