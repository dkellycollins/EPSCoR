using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        public static bool IsJsonRequest(this HttpRequestBase request)
        {
            return request.Headers["format"] == "json";
        }
    }
}