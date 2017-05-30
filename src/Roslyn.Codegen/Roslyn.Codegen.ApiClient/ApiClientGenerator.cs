using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Codegen.ApiClient.Helpers;
using System;
using System.Collections.Generic;

namespace Roslyn.Codegen.ApiClient
{
    public static class ApiClientGenerator
    {
        public static string GetGeneratedApiClass(ApiControllerInfo controllerInfo)
        {
            var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Generated")).NormalizeWhitespace()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Generic")))
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Roslyn.Codegen.ApiClient.Base")));

            var classDeclaration = SyntaxFactory.ClassDeclaration($"{controllerInfo.Name}ClientApi")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseClientApi")));

            var classMethods = new List<MemberDeclarationSyntax>();

            foreach (var methodInfo in controllerInfo.Methods)
            {
                var parameters = new List<ParameterSyntax>();
                var methodBody = methodInfo.Method.ToLowerInvariant() == "get"
                    ? $"//Http get{Environment.NewLine}throw new NotImplementedException();"
                    : $"//Http post{Environment.NewLine}throw new NotImplementedException();";

                foreach(var parameter in methodInfo.Parameters)
                {
                    parameters.Add(SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Item2))
                        .WithType(SyntaxFactory.ParseTypeName(TypesHelper.GetTypeName(parameter.Item1))));
                }

                var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName($"{TypesHelper.GetTypeName(methodInfo.ReturnedType)}"), $"{methodInfo.Name}{methodInfo.Method}")
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddParameterListParameters(parameters.ToArray())
                    .WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement((methodBody))));

                classMethods.Add(methodDeclaration);
            }

            classDeclaration = classDeclaration.AddMembers(classMethods.ToArray());

            @namespace  = @namespace.AddMembers(classDeclaration);

            var classCode = @namespace.NormalizeWhitespace().ToFullString();
            return classCode;
        }
    }
}
