using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BMM.Api.Framework;
using BMM.Api.Framework.HTTP;
using BMM.Api.Framework.JsonConverter;
using BMM.Api.Implementation.Models;
using Newtonsoft.Json;
using Tavis.UriTemplates;

namespace BMM.Api.Test.Helper
{
    public class TestRequestInterceptor : IRequestInterceptor
    {
        public Task InterceptRequest(IRequest request)
        {
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Accept", "application/json");

            var tcs = new TaskCompletionSource<Token>();
            Task.Run(
                async () =>
                {
                    try
                    {
                        var token = await GetToken();

                        var authHeader = token.Username + ":" + token.AuthenticationToken;
                        var bytes = Encoding.UTF8.GetBytes(authHeader);
                        request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(bytes);

                        tcs.SetResult(token);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });

            return tcs.Task;
        }

        private async Task<Token> GetToken()
        {
            var settings = new JsonSerializerSettings {ContractResolver = new UnderscoreMappingResolver()};
            var client = new HttpClient {BaseAddress = new Uri("https://bmm-api.brunstad.org/")};

            var authRequest = new HttpRequestMessage(HttpMethod.Post, new UriTemplate(ApiUris.Login).Resolve());
            authRequest.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            authRequest.Content =
                new StringContent($"{{ \"username\": \"{TestSecrets.Username}\", \"password\": \"{TestSecrets.LoginPassword}\" }}",
                    Encoding.UTF8, "application/json");

            try
            {
                var authResponse = await client.SendAsync(authRequest);

                var user = JsonConvert.DeserializeObject<User>(
                    await authResponse.Content.ReadAsStringAsync(),
                    settings);

                return new Token(TestSecrets.Username, user.Token);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}