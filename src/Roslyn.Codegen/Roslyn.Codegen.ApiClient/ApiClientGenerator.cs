using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestSharp;
using Roslyn.Codegen.ApiClient.Base;
using Roslyn.Codegen.ApiClient.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslyn.Codegen.ApiClient
{
    public static class ApiClientGenerator
    {
        public static string GetGeneratedApiClass(ApiControllerInfo controllerInfo)
        {
            var className = $"{controllerInfo.Name}ClientApi";

            //Add ApiClient contructor with params
            var classMembers = new List<MemberDeclarationSyntax>
            {
                GetConstructorDeclaration(className, "entryPoint")
            };
            
            //Add ApiController methods
            foreach (var methodInfo in controllerInfo.Methods)
            {
                if (methodInfo.Data != null)
                {
                    classMembers.Add(GetMethodDeclaration(methodInfo, controllerInfo.Name));
                }
            }

            //Create public ApiClient class which implement BaseClientApi base type
            var classDeclaration = ClassDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SimpleBaseType(ParseTypeName("BaseClientApi")))
                .AddMembers(classMembers.ToArray());

            //Create namespace and adding created ApiClient class
            var @namespace = NamespaceDeclaration(ParseName("ApiClient"))
                .AddUsings(GetUsingDirectives().ToArray())
                .AddMembers(classDeclaration);

            return @namespace.NormalizeWhitespace().ToFullString();
        }

        #region Private static methods
        
        private static List<UsingDirectiveSyntax> GetUsingDirectives()
        {
            var result = new List<UsingDirectiveSyntax>
            {
                UsingDirective(ParseName("System")),
                UsingDirective(ParseName("System.Collections.Generic")),
                UsingDirective(ParseName("System.Threading.Tasks")),
                UsingDirective(ParseName("Newtonsoft.Json")),
                UsingDirective(ParseName("RestSharp")),
                UsingDirective(ParseName("Roslyn.Codegen.ApiClient.Base")),
                UsingDirective(ParseName("Roslyn.Codegen.ApiClient.Extensions"))
            };

            return result;
        }

        private static MemberDeclarationSyntax GetFieldDeclaration(string fieldName)
        {
            var variableDeclaration = VariableDeclaration(ParseTypeName("string"))
                .AddVariables(VariableDeclarator($"_{fieldName}"));

            var fieldDeclaration = FieldDeclaration(variableDeclaration)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword));

            return fieldDeclaration;
        }

        private static ConstructorDeclarationSyntax GetConstructorDeclaration(string className, string argumentName)
        {
            var ctorDeclaration = ConstructorDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(Parameter(Identifier(argumentName))
                .WithType(ParseTypeName(TypesHelper.GetTypeName(typeof(string)))))
                .WithBody(Block(ParseStatement("")))
                .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer).AddArgumentListArguments(Argument(IdentifierName(argumentName))));

            return ctorDeclaration;
        }

        private static MethodDeclarationSyntax GetMethodDeclaration(BaseApiMethodInfo methodInfo, string controllerName)
        {
            var returnedType = ParseTypeName($"Task<{TypesHelper.GetTypeName(methodInfo.ReturnedType)}>");
            var modifiers = new SyntaxToken[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword) };
            var body = GetMethodBody(methodInfo, controllerName);

            var parameter = Parameter(Identifier(methodInfo.Data.Item2))
                .WithType(ParseTypeName(TypesHelper.GetTypeName(methodInfo.Data.Item1)));

            var methodDeclaration = MethodDeclaration(returnedType, $"{methodInfo.Name}")
               .AddModifiers(modifiers)
               .AddParameterListParameters(parameter)
               .WithBody(body)
               .WithConstraintClauses(List<TypeParameterConstraintClauseSyntax>());

            return methodDeclaration;
        }

        private static BlockSyntax GetMethodBody(BaseApiMethodInfo methodInfo, string controllerName)
        {
            var statements = new List<StatementSyntax>
            {
                ParseStatement($"var taskCompletion = new TaskCompletionSource<IRestResponse>();")
            };

            var httpRequestBeginning = "var request = ";
            var dataText = methodInfo.Data != null 
                ? methodInfo.Data.Item2.ToString() 
                : "null";

            var methodParameters = $"\"{controllerName}\", \"{methodInfo.Name}\", {dataText}.ToString()";
            string httpRequestText;
            switch (methodInfo.Method)
            {
                case Method.GET:
                    httpRequestText = $"{httpRequestBeginning}RestSharpExtensions.GetRequest({methodParameters});";
                    break;
                case Method.POST:
                    httpRequestText = $"{httpRequestBeginning}RestSharpExtensions.PostRequest({methodParameters});";
                    break;
                case Method.PUT:
                    httpRequestText = $"{httpRequestBeginning}RestSharpExtensions.PutRequest({methodParameters});";
                    break;
                case Method.DELETE:
                    httpRequestText = $"{httpRequestBeginning}RestSharpExtensions.DeleteRequest({methodParameters});";
                    break;
                default:
                    throw new ArgumentException("methodInfo.Method");                    
            }

            statements.Add(ParseStatement(httpRequestText));
            statements.Add(ParseStatement($"var handle = Client.ExecuteAsync(request, r => taskCompletion.SetResult(r));"));
            statements.Add(ParseStatement($"var response = await taskCompletion.Task;"));
            statements.Add(ParseStatement($"return JsonConvert.DeserializeObject<{TypesHelper.GetTypeName(methodInfo.ReturnedType)}>(response.Content);"));

            return Block(statements);
        }

        #endregion //Private static methods
    }
}