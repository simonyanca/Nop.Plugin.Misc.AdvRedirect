using System;
using System.Collections.Generic;
using System.Text.Json;
using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public class RedirectionsService
    {
        private IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;

        public RedirectionsService(IStaticCacheManager staticCacheManager, IStoreContext storeContext)
        {
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
        }

        private CacheKey GetCacheKey()
        {
            var storeId = _storeContext.GetActiveStoreScopeConfigurationAsync().Result;
            var key = _staticCacheManager.PrepareKeyForDefaultCache(AdvRedirectDefaults.ConfigurationsCacheKey, storeId);
            return key;
        }

        public Dictionary<string,string> Redirections
        {
            get 
            {
                return _staticCacheManager.Get<Dictionary<string, string>>(GetCacheKey(), () => LoadAsync());
            }
        }

        private Dictionary<string,string> LoadAsync()
        {
            if (!System.IO.File.Exists("redirections.json"))
                return new Dictionary<string, string>();
            string json = System.IO.File.ReadAllText("redirections.json");
            Dictionary<string, string> dic = JsonSerializer.Deserialize<Dictionary<string,string>>(json);
            return dic;
        }

        public async void SaveAsync()
        {
            string json = JsonSerializer.Serialize(Redirections);
            await System.IO.File.WriteAllTextAsync("redirections.json", json);
        }
    }
}
