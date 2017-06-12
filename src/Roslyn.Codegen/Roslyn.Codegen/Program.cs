using Roslyn.Codegen.ApiClient;
using Roslyn.Codegen.ApiClient.ApiMembersInfo;
using Roslyn.Codegen.Engine;
using Roslyn.Codegen.WorkspaceParser;
using System;
using System.Collections.Generic;

namespace Roslyn.Codegen
{
    class Program
    {
        static void Main(string[] args)
        {
            var controllerInfoes = ApiClientParser.GetApiControllersInfo();

            foreach(var controllerInfo in controllerInfoes)
            {
                FileHelper.GenerateFile(controllerInfo.Name, ApiClient.ApiClientGenerator.GetGeneratedApiClass(controllerInfo));
            }            
        }
    }
}