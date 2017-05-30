using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Roslyn.Codegen.ApiClient.Helpers
{
    public static class TypesHelper
    {
        /// <summary>
        /// Get correct type name for generic classes
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetTypeName(Type t)
        {
            var isGenericType = t.GetTypeInfo().IsGenericType;

            if (!isGenericType)
            {
                return t.Name;
            }

            if (t.IsNested && isGenericType)
            {
                throw new NotImplementedException();
            }

            string txt = t.Name.Substring(0, t.Name.IndexOf('`')) + "<";
            int cnt = 0;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (cnt > 0) txt += ", ";
                txt += GetTypeName(arg);
                cnt++;
            }
            return txt + ">";
        }
    }
}
