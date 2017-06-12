using RestSharp;

namespace Roslyn.Codegen.ApiClient.Base
{
    /// <summary>
    /// Base REST API client implementation 
    /// </summary>
    public class BaseClientApi
    {
        protected RestClient Client { get; private set; }

        public BaseClientApi(string entryPoint)
        {
            Client = new RestClient(entryPoint);
        }

        
    }
}
