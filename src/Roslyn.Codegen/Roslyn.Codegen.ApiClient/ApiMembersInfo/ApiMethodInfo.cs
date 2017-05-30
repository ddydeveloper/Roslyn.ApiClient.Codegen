using System;
using System.Collections.Generic;

namespace Roslyn.Codegen.ApiClient
{
    /// <summary>
    /// WebAPI controller method info
    /// </summary>
    public class ApiMethodInfo
    {
        public ApiMethodInfo(string name, string method, Type returnedType)
        {
            Name = name;
            Method = method;
            ReturnedType = returnedType;

            Parameters = new List<Tuple<Type, string>>();
        }

        public string Name { get; private set; }

        public string Method { get; private set; }
        
        public Type ReturnedType { get; set; }

        public List<Tuple<Type, string>> Parameters { get; private set; }
    }
}
