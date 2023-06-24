using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Services
{
    public interface IResponseCacheService
    {
        // Post Cahce Respone in Redis DB
        Task CachceResponseAsync(string cacheKey, object response , TimeSpan timeToLive );
        // hyb3t el response ay kan hya eh 3l4an obj bya5od mn kolo
        // time span => el time lly hto3odo fe el Memory

        // Get Response

        Task<string?> GetCachedResponse(string cacheKey);
    }
}
