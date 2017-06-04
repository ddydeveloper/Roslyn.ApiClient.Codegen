using Newtonsoft.Json;
using RestSharp;

namespace Roslyn.Codegen.ApiClient.Extensions
{
    public static class RestSharpExtensions
    {
        public static IRestRequest GetRequest(string controller, string action, string id)
        {
            return JsonRequest(Method.GET, controller, action, id);
        }

        public static IRestRequest PostRequest(string controller, string action, object data)
        {
            var request = JsonRequest(Method.POST, controller, action);
            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                request.AddParameter("application/json; charset=utf-8", jsonData, ParameterType.RequestBody);
            }

            return request;
        }

        public static IRestRequest DeleteRequest(string controller, string action, string id)
        {
            return JsonRequest(Method.DELETE, controller, action, id);
        }

        public static IRestRequest PutRequest(string controller, string action, object data)
        {
            var request = JsonRequest(Method.PUT, controller, action);
            if (data != null)
            {
                var jsonData = JsonConvert.SerializeObject(data);
                request.AddParameter("application/json; charset=utf-8", jsonData, ParameterType.RequestBody);
            }

            return request;
        }
        
        private static RestRequest JsonRequest(Method method, string controller, string action, string id = null)
        {
            var resourceAddress = $"{controller}/{action}";
            if (!string.IsNullOrEmpty(id))
            {
                resourceAddress = $"{resourceAddress}/{id}";
            }

            var request = new RestRequest(resourceAddress, method) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");
            return request;
        }
    }
}
