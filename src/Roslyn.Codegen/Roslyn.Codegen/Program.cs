using Roslyn.Codegen.ApiClient;
using Roslyn.Codegen.ApiClient.ApiMembersInfo;
using Roslyn.Codegen.Engine;
using System;
using System.Collections.Generic;

namespace Roslyn.Codegen
{
    class Program
    {
        static void Main(string[] args)
        {
            var controllerInfo = new ApiControllerInfo("Test");

            var getMethod = new GetApiMethodInfo("HttpGet", typeof(List<int>), new Tuple<Type, string>(typeof(int), "id"));
            controllerInfo.Methods.Add(getMethod);

            var postMethod = new PostApiMethodInfo("HttpPost", typeof(string), new Tuple<Type, string>(typeof(bool), "isSuccess"));
            controllerInfo.Methods.Add(postMethod);

            var deleteMethod = new GetApiMethodInfo("HttpDelete", typeof(Tuple<int, string>), new Tuple<Type, string>(typeof(decimal), "count"));
            controllerInfo.Methods.Add(deleteMethod);

            var putMethod = new PostApiMethodInfo("HttpPost", typeof(string), new Tuple<Type, string>(typeof(DateTime), "startDate"));
            controllerInfo.Methods.Add(putMethod);

            FileHelper.GenerateFile(controllerInfo.Name, ApiClient.ApiClientGenerator.GetGeneratedApiClass(controllerInfo));
        }
    }
}