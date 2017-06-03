namespace Roslyn.Codegen.ApiClient.Base
{
    /// <summary>
    /// Base REST API client implementation 
    /// </summary>
    public class BaseClientApi
    {
        protected string EntryPoint { get; set; }

        public BaseClientApi(string entryPoint)
        {
            EntryPoint = entryPoint;
        }
    }
}
