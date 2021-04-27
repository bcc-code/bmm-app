using System.Net.Http;

namespace BMM.Api.Framework.Exceptions
{
    public class ForbiddenException : ResponseException
    {
        public ForbiddenException(HttpRequestMessage httpRequest, HttpResponseMessage response) : base(httpRequest, response)
        { }
    }
}