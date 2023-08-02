using Zitadel.Api;

namespace Configurator.Handlers
{
    public class AuthenticationHandler
    {
        private readonly string apiUrl;
        private readonly RequestDelegate next;
        private readonly string relm;

        public AuthenticationHandler(RequestDelegate _next, string _relm, string _apiUrl)
        {
            next = _next;
            relm = _relm;
            apiUrl = _apiUrl;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                try
                {
                    string token = context.Request.Headers.Authorization.ToString();

                    var client = Clients.AuthService(new(apiUrl, ITokenProvider.Static(token)));
                    var result = await client.GetMyUserAsync(new());
                    if (result != null)
                    {
                        await next(context);
                        return;
                    }
                } catch
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

            }
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }
}
