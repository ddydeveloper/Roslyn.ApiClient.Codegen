using System.Collections.Generic;

namespace Roslyn.Codegen.ApiClient
{
    /// <summary>
    /// WebAPI controller info
    /// </summary>
    public class ApiControllerInfo
    {
        public ApiControllerInfo(string name)
        {
            Name = name;
            Methods = new List<ApiMethodInfo>();
        }

        public string Name { get; private set; }

        public List<ApiMethodInfo> Methods { get; private set; }
    }
}
