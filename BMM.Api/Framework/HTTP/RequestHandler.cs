using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BMM.Api.Framework.Exceptions;
using BMM.Api.Framework.JsonConverter;
using Newtonsoft.Json;

namespace BMM.Api.Framework.HTTP
{
    public class RequestHandler : IRequestHandler
    {
        protected readonly ISimpleHttpClient HttpClient;
        private readonly IResponseDeserializer _responseDeserializer;
        private readonly IConnection _connection;
        private readonly IRequestInterceptor _interceptor;
        private readonly IBadRequestThrower _badRequestThrower;
        private readonly JsonSerializerSettings _jsonSettings;

        public RequestHandler(ISimpleHttpClient httpClient, IConnection connection, IRequestInterceptor interceptor, IBadRequestThrower badRequestThrower,
            IResponseDeserializer responseDeserializer)
        {
            HttpClient = httpClient;
            _connection = connection;
            _interceptor = interceptor;
            _badRequestThrower = badRequestThrower;
            _responseDeserializer = responseDeserializer;
            _jsonSettings = new JsonSerializerSettings {ContractResolver = new IgnoreIdContractResolver()};
        }

        public async Task<T> GetResolvedResponse<T>(
            IRequest request,
            IDictionary<string, string> customHeaders = default,
            CancellationToken? cancellationToken = null)
        {
            using (HttpResponseMessage response = await GetResponse(request, customHeaders, cancellationToken))
                return await _responseDeserializer.DeserializeResponse<T>(response);
        }

        public async Task<HttpResponseMessage> GetResponse(
            IRequest request,
            IDictionary<string, string> customHeaders = default,
            CancellationToken? cancellationToken = null)
        {
            var requestMessage = await BuildRequestMessage(request, customHeaders);
            return await GetResponseByHttpRequest(requestMessage, cancellationToken);
        }

        protected async Task<HttpRequestMessage> BuildRequestMessage(IRequest request, IDictionary<string, string> customHeaders = default)
        {
            var httpRequest = new HttpRequestMessage(request.Method, request.Uri);

            if (request.Body != null)
            {
                if (request.Body is HttpContent content)
                {
                    httpRequest.Content = content;
                }
                else if (request.Body is string body)
                {
                    httpRequest.Content = new StringContent(body);
                }
                else
                {
                    var jsonBody = JsonConvert.SerializeObject(request.Body, Formatting.Indented, _jsonSettings);
                    httpRequest.Content = new StringContent(jsonBody);
                    httpRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
            }

            if (_interceptor != null)
                await _interceptor.InterceptRequest(request, customHeaders);

            foreach (var kv in request.Headers)
            {
                httpRequest.Headers.Add(kv.Key, kv.Value);
            }

            return httpRequest;
        }

        protected virtual async Task<HttpResponseMessage> GetResponseByHttpRequest(HttpRequestMessage httpRequest, CancellationToken? cancellationToken = null)
        {
            if (_connection != null && _connection.GetStatus() != ConnectionStatus.Online)
            {
                throw new InternetProblemsException(new NoInternetException());
            }

            var response = await SendRequest(httpRequest, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException(httpRequest, response);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedException(httpRequest, response);
                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new ForbiddenException(httpRequest, response);
                if (response.StatusCode == HttpStatusCode.ExpectationFailed)
                    throw new UserDoesNotExistInApiException();
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    await _badRequestThrower.ThrowExceptionForBadRequest(response);
                throw new ResponseException(httpRequest, response);
            }

            return response;
        }

        protected virtual async Task<HttpResponseMessage> SendRequest(HttpRequestMessage httpRequest, CancellationToken? token)
        {
            try
            {
                if (token != null)
                {
                    return await HttpClient.SendAsync(httpRequest, (CancellationToken)token);
                }

                return await HttpClient.SendAsync(httpRequest);
            }
            catch (Exception ex)
            {
                token?.ThrowIfCancellationRequested();

                throw new InternetProblemsException(ex);
            }
        }
    }
}