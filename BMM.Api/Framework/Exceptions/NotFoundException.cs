using System.Net.Http;

namespace BMM.Api.Framework.Exceptions
{
    public class NotFoundException : ResponseException
    {
        public NotFoundException(HttpRequestMessage request, HttpResponseMessage response) : base(request, response)
        { }
    }
}