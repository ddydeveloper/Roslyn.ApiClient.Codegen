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

            var className = $"{controllerInfo.Name}ClientApi";

            var classDeclaration = SyntaxFactory.ClassDeclaration(className)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("BaseClientApi")));
            
            var classMembers = new List<MemberDeclarationSyntax>();

            var fieldName = "entryPoint";

            var variableDeclaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("string"))
                .AddVariables(SyntaxFactory.VariableDeclarator($"_{fieldName}"));

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(variableDeclaration)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

            classMembers.Add(fieldDeclaration);

            var ctorDeclaration = SyntaxFactory.ConstructorDeclaration(className)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("entryPoint")).WithType(SyntaxFactory.ParseTypeName(TypesHelper.GetTypeName(typeof(string)))))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement(($"_{fieldName} = {fieldName};"))));

            classMembers.Add(ctorDeclaration);

            foreach (var methodInfo in controllerInfo.Methods)
            {
                var parameters = new List<ParameterSyntax>();
                var methodBody = methodInfo.Method == RestSharp.Method.GET
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

                classMembers.Add(methodDeclaration);
            }

            classDeclaration = classDeclaration.AddMembers(classMembers.ToArray());

            @namespace  = @namespace.AddMembers(classDeclaration);

            var classCode = @namespace.NormalizeWhitespace().ToFullString();
            return classCode;
        }
    }
}
