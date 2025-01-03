using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MapperGenerator;

internal static class MapperExtensions
{
    public static MethodSignature GetMethodSignature(this TypeDeclarationSyntax typeDeclarationSyntax,
        AttributeSyntax attributeSyntax)
        => new(
            attributeSyntax.GetMethodReturnType(typeDeclarationSyntax),
            attributeSyntax.GetMethodName(),
            attributeSyntax.IsMapFromAttribute()
                ? attributeSyntax.GetArgumentValue()
                : typeDeclarationSyntax.Identifier.ToString(),
            typeDeclarationSyntax.GetVariableName());

    public static bool ShouldBeIgnored(this MemberDeclarationSyntax memberDeclarationSyntax, bool isFromAttribute,
        StringBuilder sb)
    {
        var attributeName = isFromAttribute
            ? nameof(FromMappingFunctionIgnore)
            : nameof(ToMappingIgnoreMapping);
        var result = memberDeclarationSyntax.HasAttribute(attributeName);
        if (result)
        {
            sb.AppendLine(
                $"  ///<remarks> mapping of {memberDeclarationSyntax.GetMemberName()} will be skipped </remarks>");
        }

        return result;
    }

    public static bool Contains(this AttributeSyntax attributeSyntax, params string[] attributeNames)
    {
        var attributeName = attributeSyntax.Name is GenericNameSyntax genericNameSyntax
            ? genericNameSyntax.Identifier.ValueText
            : attributeSyntax.Name.ToString();
        return attributeNames.Any(x => x.Equals(attributeName, StringComparison.OrdinalIgnoreCase));
    }

    public static string GetDestinationMemberName(this MemberDeclarationSyntax memberDeclarationSyntax)
    {
        if (memberDeclarationSyntax.HasAttribute(nameof(MapAs)))
            return memberDeclarationSyntax.GetAttribute(nameof(MapAs)).GetArgumentValue();

        return memberDeclarationSyntax switch
        {
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.Identifier.ValueText,
            _ => memberDeclarationSyntax.ToString()
        };
    }

    private static string GetAttributeName(this AttributeSyntax attribute)
        => attribute.Name is GenericNameSyntax genericNameSyntax
            ? genericNameSyntax.Identifier.ValueText
            : attribute.Name.ToString();

    public static string GetMemberSignature(this MemberDeclarationSyntax memberDeclarationSyntax, string variableName,
        bool isFromAttribute)
    {
        var mappingExtensionFunctionName = isFromAttribute
            ? nameof(FromMappingExtensionFunction)
            : nameof(ToMappingExtensionFunction);

        var functionAttributeName = isFromAttribute
            ? nameof(FromMappingFunction)
            : nameof(ToMappingFunction);
        return memberDeclarationSyntax switch
        {
            PropertyDeclarationSyntax propertyDeclarationSyntax =>
                propertyDeclarationSyntax.HasAttribute(mappingExtensionFunctionName)
                    ? $"{variableName}.{propertyDeclarationSyntax.Identifier.ToString()}.{propertyDeclarationSyntax.GetAttribute(mappingExtensionFunctionName).GetArgumentValue()}"
                    : propertyDeclarationSyntax.HasAttribute(functionAttributeName)
                        ? $"{propertyDeclarationSyntax.GetAttribute(functionAttributeName).GetArgumentValue()}({variableName}.{propertyDeclarationSyntax.Identifier.ToString()})"
                        : $"{variableName}.{propertyDeclarationSyntax.Identifier.ToString()}",

            TypeDeclarationSyntax typeDeclarationSyntax =>
                typeDeclarationSyntax.HasAttribute(mappingExtensionFunctionName)
                    ? typeDeclarationSyntax.GetAttribute(mappingExtensionFunctionName).GetArgumentValue()
                    : typeDeclarationSyntax.Identifier.ToString(),

            _ => throw new NotImplementedException(),
        };
    }


    private static string GetMemberName(this MemberDeclarationSyntax memberDeclarationSyntax)
        => memberDeclarationSyntax switch
        {
            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.Identifier.ValueText,
            TypeDeclarationSyntax typeDeclarationSyntax => typeDeclarationSyntax.Identifier.ToString(),
            _ => "N/A"
        };

    private static string GetMethodName(this AttributeSyntax attribute)
        => attribute.IsMapFromAttribute() ? "ToModel" : "ToSource";

    private static string GetMethodReturnType(this AttributeSyntax attribute, TypeDeclarationSyntax type)
        => attribute.IsMapFromAttribute()
            ? type.Identifier.ValueText
            : attribute.GetArgumentValue();


    public static bool IsMapFromAttribute(this AttributeSyntax attribute)
    {
        var attributeName = attribute.GetAttributeName();
        return attributeName.Equals(nameof(MapFrom), StringComparison.OrdinalIgnoreCase);
    }

    private static string GetVariableName(this TypeDeclarationSyntax type)
        => type.HasAttribute(nameof(MapFrom))
            ? "source"
            : "model";

    private static string GetArgumentValue(this AttributeSyntax attribute)
        => attribute.ArgumentList
            is not null
            ? attribute.GetFirstArgument().GetArgumentValue()
            : attribute.Name is GenericNameSyntax genericNameSyntax
                ? genericNameSyntax.GetArgumentValue()
                : "";

    private static string GetArgumentValue(this GenericNameSyntax genericNameSyntax)
        => genericNameSyntax.TypeArgumentList.Arguments.First().ToString();

    private static string GetArgumentValue(this AttributeArgumentSyntax? argument)
        => argument is null ? "" : argument.Expression.GetExpressionSyntaxValue();

    private static string GetExpressionSyntaxValue(this ExpressionSyntax expression)
        => expression switch
        {
            LiteralExpressionSyntax literalExpressionSyntax => literalExpressionSyntax.Token.ValueText,
            IdentifierNameSyntax identifierName => identifierName.Identifier.ToString(),
            InvocationExpressionSyntax invocationExpressionSyntax
                => invocationExpressionSyntax.ArgumentList.Arguments.FirstOrDefault()?.Expression
                    .GetExpressionSyntaxValue() ?? "",
            _ => ""
        };

    private static AttributeArgumentSyntax GetFirstArgument(this AttributeSyntax attribute)
        => attribute.ArgumentList!.Arguments[0];
}

internal class MethodSignature(string type, string name, string extendedType, string variableName)
{
    public void Deconstruct(
        out string returnType,
        out string methodName,
        out string extendedType1,
        out string variableName1)
    {
        returnType = type;
        methodName = name;
        extendedType1 = extendedType;
        variableName1 = variableName;
    }
}