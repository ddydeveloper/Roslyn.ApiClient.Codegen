using Roslyn.Codegen.ApiClient.Base;
using System;

namespace Roslyn.Codegen.ApiClient.ApiMembersInfo
{
    public class GetApiMethodInfo : BaseApiMethodInfo
    {
        public GetApiMethodInfo(string name, Type returnedType, Tuple<Type, string> data) 
            : base(name, RestSharp.Method.GET, returnedType, data)
        { 

        }
    }
}
