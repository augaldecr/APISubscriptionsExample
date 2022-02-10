using ASP.NET_API.Data;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Entities;

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

            var route = httpContext.Request.Path.ToString();
            var isAFreeRoute = aPIRequestLimitConfiguration.FreeURLs.Any(r => route.Contains(r));

            if (isAFreeRoute)
            {
                await _next(httpContext);
                return;
            }

            var keyStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (keyStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Must provide the key in the header X-Api-Key");
                return;
            }

            if (keyStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Must provide only one key");
                return;
            }

            var key = keyStringValues[0];

            var keyDB = await context.APIKeys.Include(x => x.RestrictionsByDomain)
                                             .Include(x => x.RestrictionsByIP)
                                             .Include(x => x.User)
                                             .FirstOrDefaultAsync(x => x.Key == key);

            if (keyDB is null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("The provided key doesn't exist!");
                return;
            }

            if (!keyDB.Active)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("The provided key isn't active!");
                return;
            }

            if (keyDB.KeyType == Shared.Entities.KeyType.Free)
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var todayAPIRequestQuantity = await context.APIRequests.CountAsync(x =>
                        x.KeyId == keyDB.Id && x.RequestDate >= today && x.RequestDate < tomorrow);

                if (todayAPIRequestQuantity >= aPIRequestLimitConfiguration.FreediaryRequest)
                {
                    httpContext.Response.StatusCode = 429; // Too many request!
                    await httpContext.Response.WriteAsync("You have exceeded the number of daily requests. " +
                        "If you need to increase your daily limit, upgrade your subscription to a professional account.");
                    return;
                }
            }
            else if (keyDB.User.Debtor)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Please cancel the pending invoice");
                return;
            }

            var isValidByRestrictions = RequestIsValidByRestrictions(keyDB, httpContext);

            if (!isValidByRestrictions)
            {
                httpContext.Response.StatusCode = 403;
                return;
            }

            var apiRequest = new APIRequest
            {
                KeyId = keyDB.Id,
                RequestDate = DateTime.UtcNow,
            };

            await context.APIRequests.AddAsync(apiRequest);
            await context.SaveChangesAsync();

            await _next(httpContext);
        }

        private bool RequestIsValidByRestrictions(APIKey aPIKey, HttpContext httpContext)
        {
            var restrictionExist = aPIKey.RestrictionsByDomain.Any() || aPIKey.RestrictionsByIP.Any();

            if (!restrictionExist)
                return true;

            var isValidByDomainRestrictions = RequestIsValidByDomainRestrictions(aPIKey.RestrictionsByDomain, httpContext);

            var isValidByIPRestrictions = RequestIsValidByIPRestrictions(aPIKey.RestrictionsByIP, httpContext);

            return isValidByDomainRestrictions || isValidByIPRestrictions;
        }

        private bool RequestIsValidByDomainRestrictions(List<RestrictionByDomain> restrictionsByDomain, HttpContext httpContext)
        {
            if (restrictionsByDomain is null || restrictionsByDomain.Count == 0)
                return false;

            var referer = httpContext.Request.Headers["Referer"].ToString();

            if (referer == String.Empty)
                return false;

            Uri myUri = new Uri(referer);
            string host = myUri.Host;

            var isValidByDomainRestriction = restrictionsByDomain.Any(x => x.Domain == host);
            return isValidByDomainRestriction;

        }

        private bool RequestIsValidByIPRestrictions(List<RestrictionByIP> restrictionsByIP, HttpContext httpContext)
        {
            if (restrictionsByIP is null || restrictionsByIP.Count == 0)
                return false;

            var IP = httpContext.Connection.RemoteIpAddress.ToString();

            if (IP == String.Empty)
                return false;

            var isValidByDomainRestriction = restrictionsByIP.Any(x => x.IP == IP);
            return isValidByDomainRestriction;

        }
    }
}
