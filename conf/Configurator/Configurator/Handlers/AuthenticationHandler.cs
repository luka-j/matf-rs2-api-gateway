using Zitadel.Api;

namespace Configurator.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly string apiUrl;

        public AuthenticationHandler(string _apiUrl)
        {
            apiUrl = _apiUrl;
        }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var unauthResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);

            if (request.Headers.Authorization == null) {
                return unauthResponse;
            }
            string token = request.Headers.Authorization.ToString();
            try
            {
                var client = Clients.AuthService(new(apiUrl, ITokenProvider.Static(token)));
                var result = await client.GetMyUserAsync(new(), cancellationToken: cancellationToken);

                var response = await base.SendAsync(request, cancellationToken);
                return response;
            } catch {
                return unauthResponse;
            }
        }
    }
}
