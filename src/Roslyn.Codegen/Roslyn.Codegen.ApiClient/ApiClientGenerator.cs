using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestSharp;
using Roslyn.Codegen.ApiClient.Base;
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
                .AddUsings(UsingDirective(ParseName("Roslyn.Codegen.ApiClient.Base")))
                .AddUsings(UsingDirective(ParseName("Newtonsoft.Json")))
                .AddUsings(UsingDirective(ParseName("RestSharp")))
                .AddUsings(UsingDirective(ParseName("System.Threading.Tasks")))
                .AddUsings(UsingDirective(ParseName("Roslyn.Codegen.ApiClient.Extensions")));

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
                if (methodInfo.Data != null)
                {
                    var parameter = Parameter(Identifier(methodInfo.Data.Item2))
                        .WithType(ParseTypeName(TypesHelper.GetTypeName(methodInfo.Data.Item1)));

                    var methodDeclaration = MethodDeclaration(ParseTypeName($"Task<{TypesHelper.GetTypeName(methodInfo.ReturnedType)}>"), $"{methodInfo.Name}")
                       .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword))
                       .AddParameterListParameters(parameter)
                       .WithBody(Block(ParseStatement(GetMethodBody(methodInfo, controllerInfo.Name))));

                    classMembers.Add(methodDeclaration);
                }
            }

            classDeclaration = classDeclaration.AddMembers(classMembers.ToArray());

            @namespace  = @namespace.AddMembers(classDeclaration);

            var classCode = @namespace.NormalizeWhitespace().ToFullString();
            return classCode;
        }

        private static string GetMethodBody(BaseApiMethodInfo methodInfo, string controllerName)
        {
            var startText = $"TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();{Environment.NewLine}var request = ";
            var dataText = methodInfo.Data != null 
                ? methodInfo.Data.Item2.ToString() 
                : "null";

            var methodParameters = $"\"{controllerName}\", \"{methodInfo.Name}\", {dataText}.ToString()";
            string httpRequestText;
            switch (methodInfo.Method)
            {
                case RestSharp.Method.GET:
                    httpRequestText = $"RestSharpExtensions.GetRequest({methodParameters});";
                    break;
                case RestSharp.Method.POST:
                    httpRequestText = $"RestSharpExtensions.PostRequest({methodParameters});";
                    break;
                case RestSharp.Method.PUT:
                    httpRequestText = $"RestSharpExtensions.PutRequest({methodParameters});";
                    break;
                case RestSharp.Method.DELETE:
                    httpRequestText = $"RestSharpExtensions.DeleteRequest({methodParameters});";
                    break;
                default:
                    throw new ArgumentException("methodInfo.Method");                    
            }

            var requestText = $"{httpRequestText}";
            var handleText = $"var handle = Client.ExecuteAsync({Environment.NewLine}request, r => taskCompletion.SetResult(r));";
            var resultText = $"var response = await taskCompletion.Task;";
            var returnText = $"return JsonConvert.DeserializeObject<{TypesHelper.GetTypeName(methodInfo.ReturnedType)}>(response.Content);";
            var fullText = $"{startText}{httpRequestText}{Environment.NewLine}{handleText}{Environment.NewLine}{resultText}{Environment.NewLine}{returnText}";

            return fullText;
        }
    }
}