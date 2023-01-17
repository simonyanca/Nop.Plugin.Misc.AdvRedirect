using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using LinqToDB;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Misc.AdvRedirect.Entity;
using Nop.Plugin.Misc.AdvRedirect.Models;
using Nop.Plugin.Misc.AdvRedirect.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Rules;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public class RedirectionsService
    {
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly CacheKey _rulesCacheKey;
        private static List<RedirectionRuleEntity> _data = GetData().Result.ToList();

        private RulesModel _rules
        {
            get
            {
                return _staticCacheManager.Get(_rulesCacheKey, () =>
                    new RulesModel()
                    {
                        Match = _data.Where(r => r.Type == RedirectionTypeEnum.Match).ToDictionary(r => r.Pattern, r => r.RedirectUrl),
                        Rules = _data.Where(r => r.Type == RedirectionTypeEnum.RegularExpresion).Select(r => (IRule)(new RegexRule(r.Pattern, r.RedirectUrl))).ToList()
                    });
            }
        }
        
        private async static Task<IEnumerable<RedirectionRuleEntity>> GetData()
        {
            string json = await System.IO.File.ReadAllTextAsync("redirections.json");
            return JsonSerializer.Deserialize<IEnumerable<RedirectionRuleEntity>>(json);
        }

        public RedirectionsService(IStaticCacheManager staticCacheManager, IStoreContext storeContext)
        {
            _staticCacheManager = staticCacheManager;
            var storeId = storeContext.GetActiveStoreScopeConfigurationAsync().Result;
            _rulesCacheKey = staticCacheManager.PrepareKeyForDefaultCache(new CacheKey(AdvRedirectDefaults.CacheKeyString + "-Rules"), storeId);
        }


        public async Task<IEnumerable<RedirectionRuleEntity>> GetAsync(RedirectionSearchModel model)
        {
            var qry = await GetData();
            return qry;
        }

        public string ResolveRedirection(string url)
        {
            string redirectUrl;
            
            if (_rules.Match.TryGetValue(url, out redirectUrl))
                return redirectUrl;

            IRule rule = _rules.Rules.FirstOrDefault(r => r.Match(url));
            if (rule != null)
                return rule.RedirectUrl;

            return null;
        }

        private static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public async Task<string> AddRedirectionRuleAsync(RedirectionRuleEntity ent)
        {
            switch(ent.Type)
            {
                case RedirectionTypeEnum.Match:
                    if (_data.Any(r => r.Type == RedirectionTypeEnum.Match && r.Pattern == ent.Pattern))
                        return "Redirección ya existe";

                    if (!ent.RedirectUrl.StartsWith("/"))
                        ent.RedirectUrl = "/" + ent.RedirectUrl;
                    break;
                case RedirectionTypeEnum.RegularExpresion:
                    if (!IsValidRegex(ent.Pattern))
                        return "Expresión regular no válida";
                    break;
            }

            ent.Id = _data.Any() ? _data.Max(r => r.Id) + 1 : 1;
            _data.Add(ent);
            
            await _staticCacheManager.RemoveAsync(_rulesCacheKey);
            return null;
        }

        public async void RemoveRedirection(int id)
        {
            var item = _data.First(r => r.Id == id);
            _data.Remove(item);
            await _staticCacheManager.RemoveAsync(_rulesCacheKey);
        }

        public async void SaveAsync()
        {
            string json = JsonSerializer.Serialize(_data);
            await System.IO.File.WriteAllTextAsync("redirections.json", json);
        }
    }
}
