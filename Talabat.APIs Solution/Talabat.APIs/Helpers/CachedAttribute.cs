using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter // Action Filter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
            // ay 7d hy3ml create obj mn class da hyb2a m7tag yb3tly 3dd el swany lly hyt3mlo cache feha
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            /// el function de bbtnfz 2bl ma el endPoint ttnfz
            /// Get Cached Response From Redis Db
            /// Ask CLR For Creating Obj From Class 'ResponseCacheService' = Explicitly
            /// Go To Allow Dependacny Injection

            var responseCacheService = context.HttpContext.RequestServices.GetService<IResponseCacheService>();
            // context => bta3 el action lly bytnfz f el la7za de

            // Cache Key [Unique]
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);


            // Get Cached Response From Redis Db
            var cachedResponse = await responseCacheService.GetCachedResponse(cacheKey);


            if (!string.IsNullOrEmpty(cachedResponse))
            {
                ContentResult contentResult = new ContentResult()
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                context.Result = contentResult;
            } // => If EndPoint Cached
            
            /// if resposnse not cached
            /// need to execute endpoint
            /// next => Delegate for Action Being Execution

            var executedEndpointContext =  await next.Invoke(); // => Execute The EndPoint => EndPoint lly m7tot 3leha Attribute

            // Check for executedEndPointContext
            if (executedEndpointContext.Result is OkObjectResult executedEndpointResult) 
            {
                // hn3mlha caching
                await responseCacheService.CachceResponseAsync
                    (cacheKey, executedEndpointResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            /// Unique
            /// by3br 3n el Value [Response]
            /// Generate From Request
            /// /api/products?pageIndex=1&pageSize=5&sort=name
            ///     URL Path     Query1    Query2     Query3  == 3 Key ValuePair

            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path); // api/products

            foreach (var (key , value) in request.Query)
            {
                keyBuilder.Append($"|{key}-{value}");

                // api/products|pageIndex-1
                // api/products|pageIndex-1 | pageSize-5
                // api/products|pageIndex-1 | pageSize-5 | sort-name
            }

            return keyBuilder.ToString();
        }
    }
}
