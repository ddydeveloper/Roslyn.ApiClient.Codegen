using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Codegen.ApiClient.Helpers;
using System;
using System.Collections.Generic;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslyn.Codegen.ApiClient
{
    public static class ApiClientGenerator
    {
        public static string GetGeneratedApiClass(ApiControllerInfo controllerInfo)
        {
            var @namespace = NamespaceDeclaration(ParseName("Generated")).NormalizeWhitespace()
                .AddUsings(UsingDirective(ParseName("System")))
                .AddUsings(UsingDirective(ParseName("System.Collections.Generic")))
                .AddUsings(UsingDirective(ParseName("Roslyn.Codegen.ApiClient.Base")));

            var className = $"{controllerInfo.Name}ClientApi";

            var classDeclaration = ClassDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SimpleBaseType(ParseTypeName("BaseClientApi")));
            
            var classMembers = new List<MemberDeclarationSyntax>();

            var fieldName = "entryPoint";

            var variableDeclaration = VariableDeclaration(ParseTypeName("string"))
                .AddVariables(VariableDeclarator($"_{fieldName}"));

            var fieldDeclaration = FieldDeclaration(variableDeclaration)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword));

            classMembers.Add(fieldDeclaration);

            var ctorDeclaration = ConstructorDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(Parameter(Identifier(fieldName))
                .WithType(ParseTypeName(TypesHelper.GetTypeName(typeof(string)))))
                .WithBody(Block(ParseStatement(($"_{fieldName} = {fieldName};"))))
                .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer).AddArgumentListArguments(Argument(IdentifierName("entryPoint"))));


            classMembers.Add(ctorDeclaration);

            foreach (var methodInfo in controllerInfo.Methods)
            {
                var parameters = new List<ParameterSyntax>();
                var methodBody = methodInfo.Method == RestSharp.Method.GET
                    ? $"//Http get{Environment.NewLine}throw new NotImplementedException();"
                    : $"//Http post{Environment.NewLine}throw new NotImplementedException();";

                foreach (var parameter in methodInfo.Parameters)
                {
                    parameters.Add(Parameter(Identifier(parameter.Item2))
                        .WithType(ParseTypeName(TypesHelper.GetTypeName(parameter.Item1))));

                    var methodDeclaration = MethodDeclaration(ParseTypeName($"{TypesHelper.GetTypeName(methodInfo.ReturnedType)}"), $"{methodInfo.Name}{methodInfo.Method}")
                       .AddModifiers(Token(SyntaxKind.PublicKeyword))
                       .AddParameterListParameters(parameters.ToArray())
                       .WithBody(Block(ParseStatement((methodBody))));

                    classMembers.Add(methodDeclaration);
                }
            }

            classDeclaration = classDeclaration.AddMembers(classMembers.ToArray());

            @namespace  = @namespace.AddMembers(classDeclaration);

            var classCode = @namespace.NormalizeWhitespace().ToFullString();
            return classCode;
        }
    }
}
