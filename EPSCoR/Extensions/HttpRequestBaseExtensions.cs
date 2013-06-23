using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Extensions
{
    public static class HttpRequestBaseExtensions
    {
        /// <summary>
        /// Returns true of the request specifies the return format should a json object.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsJsonRequest(this HttpRequestBase request)
        {
            return request.Headers["format"] == "json";
        }
    }
}