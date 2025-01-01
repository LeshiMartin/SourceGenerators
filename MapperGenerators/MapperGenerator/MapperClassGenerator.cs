using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MapperGenerator;

[Generator]
public class MapperClassGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node.HasAttribute(nameof(MapFrom)) || node.HasAttribute(nameof(MapTo)),
                (ctx, _) => (TypeDeclarationSyntax)ctx.Node)
            .Where(x => x is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());
        context.RegisterSourceOutput(compilation, Execute);
    }


    private void Execute(SourceProductionContext context,
        (Compilation Left, ImmutableArray<TypeDeclarationSyntax> Right) tuple)
    {
        // if (!Debugger.IsAttached)
        // {
        //     Debugger.Launch();
        // }

        var (compilation, types) = tuple;
        if (types.IsEmpty) return;

        var mainSb = new StringBuilder()
            .AppendLine("// Auto generated code");
        var @namespace = $"namespace {compilation.AssemblyName};";
        var usings = new HashSet<string>();
        var classStart = """
                         public static class GeneratedMapperExtensions
                         {
                         """;
        var methodsSb = new StringBuilder();
        foreach (var type in types)
        {
            foreach (var attribute in type.AttributeLists
                         .SelectMany(x => x.Attributes)
                         .Where(c => c.Contains(nameof(MapFrom), nameof(MapTo))))
            {
                usings.Add($"using {type.GetNamespaceName(compilation.AssemblyName ?? "")};");
                var (returnType, methodName, extendedType, variableName) = type.GetMethodSignature(attribute);
                var documentationSb = new StringBuilder()
                    .AppendLine("  ///<summary>")
                    .AppendLine($"  /// It will map from {extendedType} to {returnType}");
                var method = new StringBuilder()
                    .AppendLine($$"""
                                    public static {{returnType}} {{methodName}}(this {{extendedType}} {{variableName}})
                                        => new ()
                                            {
                                  """);

                var members = type.Members
                    .Where(x => !x.ShouldBeIgnored(attribute.IsMapFromAttribute(), documentationSb)).ToArray();
                documentationSb.AppendLine("  ///<br/>Mapping will be done as:");
                for (var i = 0; i < members.Length; i++)
                {
                    var typeMember = members[i];
                    var suffix = i == members.Length - 1 ? "" : ", ";
                    var destinationMember = typeMember.GetDestinationMemberName();
                    var mappingFunction =$"{destinationMember} = {variableName}.{typeMember.GetMemberSignature(attribute.IsMapFromAttribute())}";
                    documentationSb.AppendLine($"  ///<br/><c>{mappingFunction}</c>");
                    method.AppendLine(
                        $"            {mappingFunction}{suffix}");
                }

                method.AppendLine("          };");
                documentationSb.AppendLine("  /// </summary>");
                methodsSb.Append(documentationSb.Append(method));
            }
        }

        mainSb.AppendLine(string.Join("\n", usings))
            .AppendLine(@namespace)
            .AppendLine(classStart)
            .Append(methodsSb)
            .Append("}");

        var final = mainSb.ToString();

        context.AddSource("MapperExtensionsGenerated.g.cs", final);
    }
}