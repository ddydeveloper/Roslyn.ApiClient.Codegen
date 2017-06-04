using Roslyn.Codegen.ApiClient.Base;
using System;

namespace Roslyn.Codegen.ApiClient.ApiMembersInfo
{
    public class DeleteApiMethodInfo : BaseApiMethodInfo
    {
        public DeleteApiMethodInfo(string name, Type returnedType, Tuple<Type, string> data) 
            : base(name, RestSharp.Method.GET, returnedType, data)
        {

        }
    }
}
