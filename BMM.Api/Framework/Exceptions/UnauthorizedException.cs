using System.Net.Http;

namespace BMM.Api.Framework.Exceptions
{
    public class UnauthorizedException : ResponseException
    {
        public UnauthorizedException(HttpRequestMessage request, HttpResponseMessage response) : base(request, response)
        { }
    }
}