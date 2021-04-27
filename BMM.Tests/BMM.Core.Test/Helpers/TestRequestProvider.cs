using System;
using System.Collections.Generic;
using System.Net.Http;
using BMM.Api.Framework.HTTP;

namespace BMM.Core.Test.Helpers
{
    public class TestRequestProvider
    {
        private static string _baseUrl = "https://jsonplaceholder.typicode.com";

        public static IRequest GetRequest => new Request
        {
            Method = HttpMethod.Get,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(_baseUrl + "/posts")
        };

        public static IRequest PostRequest => new Request
        {
            Method = HttpMethod.Post,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(_baseUrl + "/posts")
        };

        public static IRequest PutRequest => new Request
        {
            Method = HttpMethod.Put,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(_baseUrl + "/posts/1")
        };

        public static IRequest DeleteRequest => new Request
        {
            Method = HttpMethod.Delete,
            Headers = new Dictionary<string, string>(),
            Uri = new Uri(_baseUrl + "/posts/1")
        };
    }
}