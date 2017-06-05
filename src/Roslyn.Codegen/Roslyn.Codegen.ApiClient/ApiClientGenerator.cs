using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestSharp;
using Roslyn.Codegen.ApiClient.Base;
using Roslyn.Codegen.ApiClient.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
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
                    var returnedType = ParseTypeName($"Task<{TypesHelper.GetTypeName(methodInfo.ReturnedType)}>");
                    var modifiers = new SyntaxToken[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword) };
                    var body = GetMethodBody(methodInfo, controllerInfo.Name);
                    var parameter = Parameter(Identifier(methodInfo.Data.Item2))
                        .WithType(ParseTypeName(TypesHelper.GetTypeName(methodInfo.Data.Item1)));
                                        
                    var methodDeclaration = 
                        MethodDeclaration(returnedType, $"{methodInfo.Name}")
                       .AddModifiers(modifiers)
                       .AddParameterListParameters(parameter)
                       .WithBody(body);

                    classMembers.Add(methodDeclaration);
                }
            }

            var classDeclaration = ClassDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SimpleBaseType(ParseTypeName("BaseClientApi")))
                .AddMembers(classMembers.ToArray());
            
            @namespace  = @namespace.AddMembers(classDeclaration);

            var classCode = @namespace.NormalizeWhitespace().ToFullString();
            return classCode;
        }

        private static BlockSyntax GetMethodBody(BaseApiMethodInfo methodInfo, string controllerName)
        {
            var body = new StringBuilder($"var taskCompletion = new TaskCompletionSource<IRestResponse>();{Environment.NewLine}var request = ");
            var dataText = methodInfo.Data != null 
                ? methodInfo.Data.Item2.ToString() 
                : "null";

            var methodParameters = $"\"{controllerName}\", \"{methodInfo.Name}\", {dataText}.ToString()";
            string httpRequestText;
            switch (methodInfo.Method)
            {
                case Method.GET:
                    httpRequestText = $"RestSharpExtensions.GetRequest({methodParameters});";
                    break;
                case Method.POST:
                    httpRequestText = $"RestSharpExtensions.PostRequest({methodParameters});";
                    break;
                case Method.PUT:
                    httpRequestText = $"RestSharpExtensions.PutRequest({methodParameters});";
                    break;
                case Method.DELETE:
                    httpRequestText = $"RestSharpExtensions.DeleteRequest({methodParameters});";
                    break;
                default:
                    throw new ArgumentException("methodInfo.Method");                    
            }

            body.AppendLine(httpRequestText + Environment.NewLine);
            body.AppendLine($"var handle = Client.ExecuteAsync(request, r => taskCompletion.SetResult(r));{Environment.NewLine}");
            body.AppendLine($"var response = await taskCompletion.Task;{Environment.NewLine}");
            body.AppendLine($"return JsonConvert.DeserializeObject<{TypesHelper.GetTypeName(methodInfo.ReturnedType)}>(response.Content);");

            return Block(ParseStatement(body.ToString()));
        }
    }
}