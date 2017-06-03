namespace Generated
{
    using System;
    using System.Collections.Generic;
    using Roslyn.Codegen.ApiClient.Base;

    public class TestClientApi : BaseClientApi
    {
        private string _entryPoint;
        public TestClientApi(String entryPoint): base (entryPoint)
        {
            _entryPoint = entryPoint;
        }

        public Int32 GetHttpMethodGET(Int32 intParam)
        {
            //Http get
            throw new NotImplementedException();
        }

        public Int32 GetHttpMethodGET(Int32 intParam, String stringParam)
        {
            //Http get
            throw new NotImplementedException();
        }

        public String PostHttpMethodPOST(Boolean boolParam)
        {
            //Http post
            throw new NotImplementedException();
        }

        public String PostHttpMethodPOST(Boolean boolParam, Tuple<Int32, Int32> tupleParam)
        {
            //Http post
            throw new NotImplementedException();
        }
    }
}
