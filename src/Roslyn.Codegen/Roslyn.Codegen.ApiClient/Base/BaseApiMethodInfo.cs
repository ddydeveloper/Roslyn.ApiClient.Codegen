using RestSharp;
using System;

namespace Roslyn.Codegen.ApiClient.Base
{
    /// <summary>
    /// WebAPI controller method info
    /// </summary>
    public abstract class BaseApiMethodInfo
    {
        public BaseApiMethodInfo(string name, Method method, Type returnedType, Tuple<Type, string> data)
        {
            Name = name;
            Method = method;
            ReturnedType = returnedType;
            Data = data;
        }

        /// <summary>
        /// Name of method
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Http method
        /// </summary>
        public Method Method { get; private set; }
        
        /// <summary>
        /// Method returned type
        /// </summary>
        public Type ReturnedType { get; set; }

        /// <summary>
        /// Method parameter
        /// </summary>
        public Tuple<Type, string> Data { get; set; }
    }
}
