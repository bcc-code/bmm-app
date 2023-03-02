using System;
using System.Collections.Generic;
using System.Net.Http;
using BMM.Api.Framework.HTTP;

namespace BMM.Api.Test.Helper
{
    public class TestRequestProvider
    {
        private const string HttpBinUrl = "https://jsonplaceholder.typicode.com";

        public static IRequest GetRequest => new Request
        {
            Method = HttpMethod.Get,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(HttpBinUrl + "/posts")
        };

        public static IRequest PostRequest => new Request
        {
            Method = HttpMethod.Post,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(HttpBinUrl + "/posts")
        };

        public static IRequest PutRequest => new Request
        {
            Method = HttpMethod.Put,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(HttpBinUrl + "/posts/1")
        };

        public static IRequest DeleteRequest => new Request
        {
            Method = HttpMethod.Delete,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(HttpBinUrl + "/posts/1")
        };
    }
}