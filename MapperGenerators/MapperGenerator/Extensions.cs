using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MapperGenerator
{
    public static class Extensions
    {
        public static bool HasAttribute(this SyntaxNode syntaxNode, string attributeName)
        {

            return syntaxNode switch
            {
                ClassDeclarationSyntax classDeclarationSyntax => classDeclarationSyntax.HasAttribute(attributeName),
                RecordDeclarationSyntax recordDeclarationSyntax => recordDeclarationSyntax.HasAttribute(attributeName),
                EnumDeclarationSyntax enumDeclarationSyntax => enumDeclarationSyntax.HasAttribute(attributeName),
                StructDeclarationSyntax structDeclarationSyntax => structDeclarationSyntax.HasAttribute(attributeName),
                TypeDeclarationSyntax typeDeclarationSyntax => typeDeclarationSyntax.HasAttribute(attributeName),
                PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.HasAttribute(attributeName),
                _ => false,
            };
        }

        public static AttributeSyntax GetAttribute(this SyntaxNode syntaxNode, string attributeName)
            => syntaxNode switch
            {
                ClassDeclarationSyntax classDeclarationSyntax => classDeclarationSyntax.GetAttribute(attributeName),
                RecordDeclarationSyntax recordDeclarationSyntax => recordDeclarationSyntax.GetAttribute(attributeName),
                EnumDeclarationSyntax enumDeclarationSyntax => enumDeclarationSyntax.GetAttribute(attributeName),
                StructDeclarationSyntax structDeclarationSyntax => structDeclarationSyntax.GetAttribute(attributeName),
                TypeDeclarationSyntax typeDeclarationSyntax => typeDeclarationSyntax.GetAttribute(attributeName),
                PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.GetAttribute(attributeName),
                _ => throw new NotImplementedException(),
            };

        public static string GetNamespaceName(this SyntaxNode syntaxNode, string fallBackValue = "")
            => syntaxNode switch
            {
                FileScopedNamespaceDeclarationSyntax fileScopedNamespace => fileScopedNamespace.Name.ToString(),
                NamespaceDeclarationSyntax namespaceDeclaration => namespaceDeclaration.Name.ToString(),
                _ => fallBackValue
            };

        private static AttributeSyntax GetAttribute(this ClassDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.GetAttribute(attributeName);

        private static AttributeSyntax GetAttribute(this StructDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.GetAttribute(attributeName);

        private static AttributeSyntax GetAttribute(this EnumDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.GetAttribute(attributeName);

        private static AttributeSyntax GetAttribute(this RecordDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.GetAttribute(attributeName);

        private static AttributeSyntax GetAttribute(this TypeDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.GetAttribute(attributeName);

        private static AttributeSyntax GetAttribute(this PropertyDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.GetAttribute(attributeName);


        private static bool HasAttribute(this ClassDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.HasAttribute(attributeName);

        private static bool HasAttribute(this StructDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.HasAttribute(attributeName);

        private static bool HasAttribute(this EnumDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.HasAttribute(attributeName);

        private static bool HasAttribute(this RecordDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.HasAttribute(attributeName);

        private static bool HasAttribute(this TypeDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.HasAttribute(attributeName);

        private static bool HasAttribute(this PropertyDeclarationSyntax syntaxNode, string attributeName)
            => syntaxNode.AttributeLists.HasAttribute(attributeName);

        private static bool HasAttribute(this SyntaxList<AttributeListSyntax> attributeListSyntax, string attributeName)
            => attributeListSyntax
                .Any(at => at.Attributes.Any(att =>
                    att.Name.ToString().Equals(attributeName, StringComparison.OrdinalIgnoreCase)));

        private static AttributeSyntax GetAttribute(this SyntaxList<AttributeListSyntax> attributeListSyntax,
            string attributeName)
            => attributeListSyntax
                .SelectMany(x => x.Attributes)
                .First(at => at.Name.ToString().Equals(attributeName, StringComparison.OrdinalIgnoreCase));
    }
}