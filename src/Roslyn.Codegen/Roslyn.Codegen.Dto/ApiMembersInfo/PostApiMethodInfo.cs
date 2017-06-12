using Roslyn.Codegen.ApiClient.Base;
using System;

namespace Roslyn.Codegen.ApiClient.ApiMembersInfo
{
    public class PostApiMethodInfo : BaseApiMethodInfo
    {
        public PostApiMethodInfo(string name, Type returnedType, Tuple<Type, string> data) 
            : base(name, RestSharp.Method.POST, returnedType, data)
        {

        }
    }
}
