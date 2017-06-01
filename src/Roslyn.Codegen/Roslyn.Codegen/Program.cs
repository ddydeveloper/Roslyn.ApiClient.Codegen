using Roslyn.Codegen.ApiClient;
using Roslyn.Codegen.Engine;
using System.Collections.Generic;

namespace Roslyn.Codegen
{
    class Program
    {
        static void Main(string[] args)
        {
            var controllerInfo = new ApiControllerInfo("Test");

            var getMethod = new ApiMethodInfo("GetHttpMethod", RestSharp.Method.GET, typeof(int));
            getMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(int), "intParam"));
            getMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(string), "stringParam"));
            controllerInfo.Methods.Add(getMethod);

            var postMethod = new ApiMethodInfo("PostHttpMethod", RestSharp.Method.POST, typeof(string));
            postMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(bool), "boolParam"));
            postMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(System.Tuple<int, int>), "tupleParam"));
            controllerInfo.Methods.Add(postMethod);

            controllerInfo.Methods.Add(new ApiMethodInfo("PutHttpMethod", RestSharp.Method.PUT, typeof(List<int>)));
            controllerInfo.Methods.Add(new ApiMethodInfo("DeleteHttpMethod", RestSharp.Method.DELETE, typeof(List<dynamic>)));

            FileHelper.GenerateFile(controllerInfo.Name, ApiClient.ApiClientGenerator.GetGeneratedApiClass(controllerInfo));
        }
    }
}