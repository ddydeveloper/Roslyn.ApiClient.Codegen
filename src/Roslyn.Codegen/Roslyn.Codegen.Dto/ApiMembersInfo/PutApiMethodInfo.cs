using Roslyn.Codegen.ApiClient.Base;
using System;

namespace Roslyn.Codegen.ApiClient.ApiMembersInfo
{
    public class PutApiMethodInfo : BaseApiMethodInfo
    {
        public PutApiMethodInfo(string name, Type returnedType, Tuple<Type, string> data) 
            : base(name, RestSharp.Method.POST, returnedType, data)
        {

        }
    }
}
