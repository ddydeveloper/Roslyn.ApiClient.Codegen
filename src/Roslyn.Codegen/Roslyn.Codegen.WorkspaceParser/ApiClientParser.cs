using System;
using System.Collections.Generic;
using Roslyn.Codegen.ApiClient;
using Roslyn.Codegen.ApiClient.ApiMembersInfo;
using Microsoft.CodeAnalysis.MsBuild;

namespace Roslyn.Codegen.WorkspaceParser
{
    public static class ApiClientParser
    {
        public static List<ApiControllerInfo> GetApiControllersInfo()
        {
            //var ws = MSBuildWorkspace.Create();

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
