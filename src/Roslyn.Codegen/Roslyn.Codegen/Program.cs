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

            var getMethod = new ApiMethodInfo("GetHttpMethod", "Get", typeof(int));
            getMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(int), "intParam"));
            getMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(string), "stringParam"));
            controllerInfo.Methods.Add(getMethod);

            var postMethod = new ApiMethodInfo("PostHttpMethod", "Post", typeof(string));
            postMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(bool), "boolParam"));
            postMethod.Parameters.Add(new System.Tuple<System.Type, string>(typeof(System.Tuple<int, int>), "tupleParam"));
            controllerInfo.Methods.Add(postMethod);

            controllerInfo.Methods.Add(new ApiMethodInfo("PutHttpMethod", "Put", typeof(List<int>)));
            controllerInfo.Methods.Add(new ApiMethodInfo("DeleteHttpMethod", "Delete", typeof(List<dynamic>)));

            FileHelper.GenerateFile(controllerInfo.Name, ApiClient.ApiClientGenerator.GetGeneratedApiClass(controllerInfo));
        }
    }
}