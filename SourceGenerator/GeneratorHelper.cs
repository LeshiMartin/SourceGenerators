using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator;

internal static class GeneratorHelper
{
    public static string GetNamespaceName(this SyntaxNode syntaxNode)
        => syntaxNode switch
        {
            FileScopedNamespaceDeclarationSyntax fileScopedNamespace => fileScopedNamespace.Name.ToString(),
            NamespaceDeclarationSyntax namespaceDeclaration => namespaceDeclaration.Name.ToString(),
            _ => throw new ArgumentOutOfRangeException()
        };
}