using System;
using System.Collections.Generic;
using Roslyn.Codegen.ApiClient;
using Roslyn.Codegen.ApiClient.ApiMembersInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslyn.Codegen.WorkspaceParser
{
    public static class ApiClientParser
    {
        public static List<ApiControllerInfo> GetApiControllersInfo()
        {
            string solutionPath = @"C:\Users\...\PathToSolution\MySolution.sln";
            var msWorkspace = MSBuildWorkspace.Create();

            var solution = msWorkspace.OpenSolutionAsync(solutionPath).Result;

            var result = new List<ApiControllerInfo>();
            var controllerInfo = new ApiControllerInfo("Test");

            var getMethod = new GetApiMethodInfo("HttpGet", typeof(List<int>), new Tuple<Type, string>(typeof(int), "id"));
            controllerInfo.Methods.Add(getMethod);

            var postMethod = new PostApiMethodInfo("HttpPost", typeof(string), new Tuple<Type, string>(typeof(bool), "isSuccess"));
            controllerInfo.Methods.Add(postMethod);

            var deleteMethod = new GetApiMethodInfo("HttpDelete", typeof(Tuple<int, string>), new Tuple<Type, string>(typeof(decimal), "count"));
            controllerInfo.Methods.Add(deleteMethod);

            var putMethod = new PostApiMethodInfo("HttpPost", typeof(string), new Tuple<Type, string>(typeof(DateTime), "startDate"));
            controllerInfo.Methods.Add(putMethod);

            result.Add(controllerInfo);

            return result;
        }
    }    
}
