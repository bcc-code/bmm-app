using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BMM.Api.Framework.HTTP
{
    public interface IRequest
    {
        object Body { get; set; }

        Dictionary<string, string> Headers { get; set; }

        HttpMethod Method { get; set; }

        Uri Uri { get; set; }
    }
}