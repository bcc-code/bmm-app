using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BMM.Api.Framework.HTTP
{
    public class Request : IRequest
    {
        public Request()
        {
            Headers = new Dictionary<string, string>();
        }

        public Uri Uri { get; set; }
        public HttpMethod Method { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public object Body { get; set; }
    }
}