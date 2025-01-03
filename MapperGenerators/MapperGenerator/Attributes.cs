// ReSharper disable ClassNeverInstantiated.Global

namespace MapperGenerator;

/// <summary>
/// Attribute to specify from which <paramref name="Name"/> to map
/// </summary>
/// <param name="Name">The name of the class from which to map from</param>
public class MapFrom(string Name) : Attribute;

/// <summary>
/// Attribute to specify from which <typeparamref name="T"/> to map
/// </summary>
/// <typeparam name="T">The type from which to map from</typeparam>
public class MapFrom<T> : Attribute where T : class;

/// <summary>
/// Attribute to specify To which <paramref name="Name"/> to map
/// </summary>
/// <param name="Name">The name of the class to which to map to</param>
public class MapTo(string Name) : Attribute;

/// <summary>
/// Attribute to specify to which <typeparamref name="T"/> to map
/// </summary>
/// <typeparam name="T">The type to which to map to</typeparam>
public class MapTo<T> : Attribute where T : class;

/// <summary>
/// Attribute specifying when the mapping is being done,  what property name to be used
/// </summary>
/// <param name="Name">The name that will be used as property</param>
public class MapAs(string Name) : Attribute;

/// <summary>
/// A member attribute specifying which ExtensionFunction should be used on the property when
/// the mapping <see cref="MapFrom{T}"/> or <see cref="MapFrom"/> is being done
/// </summary>
/// <remarks>It should not start with <c>.</c></remarks>
/// <param name="functionExpression">The extension function</param>
public class FromMappingExtensionFunction(string functionExpression) : Attribute;

/// <summary>
/// A mapping function that will be used when <see cref="MapFrom{T}"/> or <see cref="MapFrom"/> is being performed
/// </summary>
/// <param name="functionExpression"></param>
/// <remarks>Don't use function invocation <c>()</c> and the function should accept only one property
/// and to be the same type as being mapped from
/// </remarks>
public class FromMappingFunction(string functionExpression) : Attribute;

/// <summary>
/// A member attribute specifying which ExtensionFunction should be used on the property when
/// the mapping <see cref="MapTo{T}"/> or <see cref="MapTo"/> is being done
/// </summary>
/// <remarks>It should not start with <c>.</c></remarks>
/// <param name="functionExpression">The extension function</param>
public class ToMappingExtensionFunction(string functionExpression) : Attribute;

/// <summary>
/// A mapping function that will be used when <see cref="MapTo{T}"/> or <see cref="MapTo"/> is being performed
/// </summary>
/// <param name="functionExpression"></param>
/// <remarks>Don't use function invocation <c>()</c> and the function should accept only one property
/// and to be the same type as being mapped to
/// </remarks>
public class ToMappingFunction(string functionExpression) : Attribute;

/// <summary>
/// Attribute specifying that the Property should be ignored from the mapping
/// when <see cref="MapTo"/> or <see cref="MapTo{T}"/> is being done
/// </summary>
public class ToMappingIgnoreMapping : Attribute;

/// <summary>
/// Attribute specifying that the Property should be ignored from the mapping
/// when <see cref="MapFrom{T}"/> or <see cref="MapFrom"/> is being done
/// </summary>
public class FromMappingFunctionIgnore : Attribute;