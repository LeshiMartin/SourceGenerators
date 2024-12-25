using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator;

internal static class GeneratorHelper
{
    public static bool HasAttribute(this SyntaxNode syntaxNode, string attributeName)
        => syntaxNode switch
        {
            ClassDeclarationSyntax classDeclarationSyntax => classDeclarationSyntax.HasAttribute(attributeName),
            RecordDeclarationSyntax recordDeclarationSyntax => recordDeclarationSyntax.HasAttribute(attributeName),
            EnumDeclarationSyntax enumDeclarationSyntax => enumDeclarationSyntax.HasAttribute(attributeName),
            StructDeclarationSyntax structDeclarationSyntax => structDeclarationSyntax.HasAttribute(attributeName),
            _ => throw new NotImplementedException(),
        };

    private static bool HasAttribute(this ClassDeclarationSyntax syntaxNode, string attributeName)
        => syntaxNode.AttributeLists.HasAttribute(attributeName);

    private static bool HasAttribute(this StructDeclarationSyntax syntaxNode, string attributeName)
        => syntaxNode.AttributeLists.HasAttribute(attributeName);

    private static bool HasAttribute(this EnumDeclarationSyntax syntaxNode, string attributeName)
        => syntaxNode.AttributeLists.HasAttribute(attributeName);

    private static bool HasAttribute(this RecordDeclarationSyntax syntaxNode, string attributeName)
        => syntaxNode.AttributeLists.HasAttribute(attributeName);

    private static bool HasAttribute(this SyntaxList<AttributeListSyntax> attributeListSyntax, string attributeName)
        => attributeListSyntax
            .Any(at => at.Attributes
                .Any(att => att.Name.ToString().Equals(attributeName, StringComparison.OrdinalIgnoreCase)));

    public static string GetNamespaceName(this SyntaxNode syntaxNode, string fallBackValue = "")
        => syntaxNode switch
        {
            FileScopedNamespaceDeclarationSyntax fileScopedNamespace => fileScopedNamespace.Name.ToString(),
            NamespaceDeclarationSyntax namespaceDeclaration => namespaceDeclaration.Name.ToString(),
            _ => fallBackValue
        };

    public static string GetNameMethod(this EnumDeclarationSyntax @enum)
    {
        var enumName = @enum.Identifier.ToString();

        var docSb = new StringBuilder()
            .AppendLine("/// <summary>")
            .AppendLine("/// Gets the name of the enum <paramref name=\"source\" /> for:");
        var methodSb = new StringBuilder();
        methodSb.AppendLine(
            $$"""
                  public static string GetName(this {{enumName}} source)
                   => source switch 
                    {
              """);

        foreach (var member in @enum.Members)
        {
            var memberName = member.Identifier.ToString();
            var stringRep = member.GetDesiredMemberName(memberName);
            docSb.AppendLine($"/// <li> <see cref=\"{enumName}.{memberName}\"/> returns {stringRep} ;</li>");
            methodSb.AppendLine($"       {enumName}.{memberName} => \"{stringRep}\",");
        }

        methodSb
            .AppendLine("        _ => throw new ArgumentOutOfRangeException()")
            .AppendLine("     };");
        docSb.AppendLine("///</summary>")
            .AppendLine("///<param name=\"source\">The enum from which the name will be retrieved</param>")
            .AppendLine(
                "/// <exception cref=\"ArgumentOutOfRangeException\"> if <paramref name=\"source\" /> cannot be matched </exception>")
            .AppendLine("///<returns> The string representation of <paramref name=\"source\"/> </returns>");

        return docSb.AppendLine(methodSb.ToString()).ToString();
    }

    public static string GetDescriptionMethod(this EnumDeclarationSyntax @enum)
    {
        var enumName = @enum.Identifier.ToString();

        var docSb = new StringBuilder()
            .AppendLine("/// <summary>")
            .AppendLine("/// Gets the description of the enum <paramref name=\"source\" /> for:");
        var methodSb = new StringBuilder();
        methodSb.AppendLine(
            $$"""
                  public static string GetDescription(this {{enumName}} source)
                   => source switch 
                    {
              """);

        foreach (var member in @enum.Members)
        {
            var memberName = member.Identifier.ToString();
            var stringRep = member.GetDesiredMemberDescription(memberName);
            docSb.AppendLine($"/// <li> <see cref=\"{enumName}.{memberName}\"/> returns {stringRep} ;</li>");
            methodSb.AppendLine($"       {enumName}.{memberName} => \"{stringRep}\",");
        }

        methodSb
            .AppendLine("        _ => throw new ArgumentOutOfRangeException()")
            .AppendLine("     };");
        docSb.AppendLine("///</summary>")
            .AppendLine("///<param name=\"source\">The enum from which the description will be retrieved</param>")
            .AppendLine(
                "/// <exception cref=\"ArgumentOutOfRangeException\"> if <paramref name=\"source\" /> cannot be matched </exception>")
            .AppendLine("///<returns> The description of <paramref name=\"source\"/> </returns>");

        return docSb.AppendLine(methodSb.ToString()).ToString();
    }

    private static string GetDesiredMemberDescription(this EnumMemberDeclarationSyntax member, string memberName)
    {
        var attributeSyntax = member.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(at =>
                at.Name.ToString().Equals(nameof(EnumDescription), StringComparison.InvariantCultureIgnoreCase));
        var argument = attributeSyntax?.ArgumentList?.Arguments.FirstOrDefault();
        if (argument?.Expression is LiteralExpressionSyntax literalExpressionSyntax)
            return literalExpressionSyntax.Token.ValueText ?? memberName;
        return memberName;
    }

    private static string GetDesiredMemberName(this EnumMemberDeclarationSyntax member, string memberName)
    {
        var attributeSyntax = member.AttributeLists
            .SelectMany(x => x.Attributes)
            .FirstOrDefault(at =>
                at.Name.ToString().Equals(nameof(EnumName), StringComparison.InvariantCultureIgnoreCase));
        var argument = attributeSyntax?.ArgumentList?.Arguments.FirstOrDefault();
        if (argument?.Expression is LiteralExpressionSyntax literalExpressionSyntax)
            return literalExpressionSyntax.Token.ValueText ?? memberName;
        return memberName;
    }
}