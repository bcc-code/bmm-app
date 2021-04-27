using System;
using System.Net.Http;

namespace BMM.Api.Framework.Exceptions
{
    public class ResponseException : Exception
    {
        public ResponseException(HttpRequestMessage request, HttpResponseMessage response)
            : base(response.StatusCode.ToString())
        {
            Request = request;
            Response = response;
        }

        public HttpRequestMessage Request { get; }

        public HttpResponseMessage Response { get; }

        public override string ToString()
        {
            return base.ToString() + "\n\n" +
                   Request.Method + " " + Request.RequestUri + " HTTP/" + Request.Version + "\n" +
                   Request.Headers + "\n" +
                   "\n" + "\n" +
                   "HTTP/" + Response.Version + " " + Response.StatusCode + "\n" +
                   Response.Headers + "\n" +
                   Response.Content.ReadAsStringAsync().Result;
        }
    }
}