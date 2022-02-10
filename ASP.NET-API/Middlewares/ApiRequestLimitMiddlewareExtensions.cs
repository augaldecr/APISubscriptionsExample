using ASP.NET_API.Data;
using Shared.DTOs;

namespace ASP.NET_API.Middlewares
{
    public static class ApiRequestLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseAPIRequestLimit(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ApiRequestLimitMiddleware>();
        }
    }

    public class ApiRequestLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiRequestLimitMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
        {
            var aPIRequestLimitConfiguration = new ApiRequestLimitConfig();
            _configuration.GetRequiredSection("APIRequestLimit").Bind(aPIRequestLimitConfiguration);

            var keyStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (keyStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Must provide the key in the header X-Api-Key");
                return;
            }

            await _next(httpContext);
        }
    }
}
