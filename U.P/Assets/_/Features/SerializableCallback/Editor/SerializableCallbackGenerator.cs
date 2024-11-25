using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;

namespace SerializableCallback.Editor
{
    public class SerializableCallback : Attribute
    {
        public SerializableCallback()
        {
        }
    }

    /// <inheritdoc />
    [Generator]
    public class SerializableCallbackGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var treesWithlassWithAttributes = context.Compilation.SyntaxTrees.Where(st => st.GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Any(p => p.DescendantNodes().OfType<AttributeSyntax>().Any()));

            foreach (var tree in treesWithlassWithAttributes)
            {
                var semanticModel = context.Compilation.GetSemanticModel(tree);
                var declaredClasses = tree
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Where(cd => cd.DescendantNodes().OfType<AttributeSyntax>().Any());
                foreach (var declaredClass in declaredClasses)
                {
                    var nodes = declaredClass
                        .DescendantNodes()
                        .OfType<AttributeSyntax>()
                        .FirstOrDefault(a => a.DescendantTokens().Any(dt =>
                            dt.IsKind(SyntaxKind.IdentifierToken) &&
                            dt.Parent != null &&
                            semanticModel.GetTypeInfo(dt.Parent).Type != null &&
                            semanticModel.GetTypeInfo(dt.Parent).Type?.Name == nameof(SerializableCallback)))
                        ?.DescendantTokens()
                        ?.Where(dt => dt.IsKind(SyntaxKind.IdentifierToken))
                        ?.ToList();
                    var relatedClass = semanticModel.GetTypeInfo(nodes.Last().Parent);
                    var classMethods = declaredClass.Members.Where(m => m.IsKind(SyntaxKind.MethodDeclaration))
                        .OfType<MethodDeclarationSyntax>();
                    foreach (var methodDeclaration in classMethods)
                    {
                        var mods = methodDeclaration.Modifiers; 
                        //public, static, etc...
                        var identifiers = methodDeclaration.Identifier; 
                        // Quite obvious => the name
                        var parameters =
                            methodDeclaration
                                .ParameterList;
                        //The list of the parameters, including type, name, default values
                        Debug.Log($"{mods} , {identifiers} , {parameters}");
                    }
                }
            }
        }
    }
}