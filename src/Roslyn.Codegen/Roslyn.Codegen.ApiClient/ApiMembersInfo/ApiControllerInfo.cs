using Roslyn.Codegen.ApiClient.Base;
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
            Methods = new List<BaseApiMethodInfo>();
        }

        public string Name { get; private set; }

        public List<BaseApiMethodInfo> Methods { get; private set; }
    }
}
