using System.Net.Http;
using BMM.Api.Framework.HTTP;
using Tavis.UriTemplates;

namespace BMM.Api.Framework
{
    /// <summary>Interface for a resource client.</summary>
    public interface IClient
    {
        /// <summary>Builds a request.</summary>
        /// <param name="template">The template.</param>
        /// <param name="method">The method.</param>
        /// <param name="body">The body.</param>
        /// <returns>A Request.</returns>
        IRequest BuildRequest(UriTemplate template, HttpMethod method, object body = null);
    }
}