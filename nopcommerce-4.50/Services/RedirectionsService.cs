using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Models;
using Nop.Plugin.Misc.AdvRedirect.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Rules;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Caching;
using Nop.Services.Catalog;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public class RedirectionsService : IRedirectionsService
    {
        private readonly IRepository<RedirectionRule> _redirectionRuleEntityRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;



        public RedirectionsService( IRepository<RedirectionRule> redirectionRuleEntityRepository, IStaticCacheManager staticCacheManager, IStoreContext storeContext)
        {
            _redirectionRuleEntityRepository = redirectionRuleEntityRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
        }

        private async Task<(Dictionary<string,string> mach, Dictionary<string,string> qryMach, IEnumerable<IRule> regex)> GetAllRulesAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            CacheKey key = _staticCacheManager.PrepareKeyForDefaultCache(AdvRedirectDefaults.RedirectionsRulesCacheKey, store);

            var data = await _staticCacheManager.GetAsync<(Dictionary<string, string>, Dictionary<string, string>, IEnumerable<IRule>)>(key, async () =>
            {
                var data = await _redirectionRuleEntityRepository.Table
                .Where(r => r.StoreId == store.Id)
                .ToListAsync();

                Dictionary<string, string> mach = data.Where(r => (RedirectionTypeEnum)r.Type == RedirectionTypeEnum.Match && !r.UseQueryString).ToDictionary(r => r.Pattern, r => r.RedirectUrl);
				Dictionary<string, string> qryMach = data.Where(r => (RedirectionTypeEnum)r.Type == RedirectionTypeEnum.Match && r.UseQueryString).ToDictionary(r => r.Pattern, r => r.RedirectUrl);
				IEnumerable<IRule> regex = await data.Where(r => (RedirectionTypeEnum)r.Type == RedirectionTypeEnum.RegularExpresion).Select(r => (IRule)new RegexRule(r.Pattern, r.RedirectUrl, r.UseQueryString)).ToListAsync();

                return (mach, qryMach, regex);
            });

            return data;
        }


        public async Task<IPagedList<RedirectionRule>> GetAllRedirectionsAsync(RedirectionSearchModel searchModel)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var qry = _redirectionRuleEntityRepository.Table.Where(r => r.StoreId == store.Id);

            if (!string.IsNullOrEmpty(searchModel.Pattern)) qry = qry.Where(r => r.Pattern.Contains(searchModel.Pattern));
            if (!string.IsNullOrEmpty(searchModel.RedirectUrl)) qry = qry.Where(r => r.RedirectUrl.Contains(searchModel.RedirectUrl));

            string colName = searchModel.Columns[searchModel.Order[0].Column].Data;

            if(colName == nameof(RedirectionRule.Pattern)) 
                qry = searchModel.Order[0].Dir == "asc"? qry.OrderBy(r => r.Pattern) : qry.OrderByDescending(r => r.Pattern);
            else if (colName == nameof(RedirectionRule.RedirectUrl))
                qry = searchModel.Order[0].Dir == "asc" ? qry.OrderBy(r => r.RedirectUrl) : qry.OrderByDescending(r => r.RedirectUrl);
            else if (colName == nameof(RedirectionRule.Id))
                qry = searchModel.Order[0].Dir == "asc" ? qry.OrderBy(r => r.Id) : qry.OrderByDescending(r => r.Id);
            else if (colName == nameof(RedirectionRule.Type))
                qry = searchModel.Order[0].Dir == "asc" ? qry.OrderBy(r => r.Type) : qry.OrderByDescending(r => r.Type);
            else if (colName == nameof(RedirectionRule.UseQueryString))
                qry = searchModel.Order[0].Dir == "asc" ? qry.OrderBy(r => r.UseQueryString) : qry.OrderByDescending(r => r.UseQueryString);

            var records = new PagedList<RedirectionRule>(await qry.ToListAsync(), searchModel.Page-1, searchModel.PageSize);

            return records;
        }

        public async Task<string> ResolveRedirection(HttpRequest request)
        {
            var rules = await GetAllRulesAsync();

            string redirectUrl = "";
			if (rules.qryMach.TryGetValue(request.Path + request.QueryString.ToString(), out redirectUrl))
				return redirectUrl;

			if (rules.mach.TryGetValue(request.Path, out redirectUrl))
                return redirectUrl;

			IRule rule = rules.regex.FirstOrDefault(r => r.Match(request.Path, request.QueryString.ToString()));
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


        public virtual async void DeleteRedirectionAsync(RedirectionRule ent)
        {
            var item = _redirectionRuleEntityRepository.Table.First(r => r.Id == ent.Id);
            await _redirectionRuleEntityRepository.DeleteAsync(item);
        }

        public async Task<string> InsertRedirectionsAsync(RedirectionRule ent)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            ent.Pattern = ent.Pattern.Trim();
            
			switch ((RedirectionTypeEnum)ent.Type)
            {
                case RedirectionTypeEnum.Match:
                    
                    if (_redirectionRuleEntityRepository.Table.Any(r => r.StoreId == store.Id && r.Pattern == ent.Pattern))
                        return "Redirección ya existe";

                    if (!ent.RedirectUrl.StartsWith("/"))
                        ent.RedirectUrl = "/" + ent.RedirectUrl;

                    break;
                case RedirectionTypeEnum.RegularExpresion:
                    if (!IsValidRegex(ent.Pattern))
                        return "Expresión regular no válida";
                    break;
            }

            ent.UseQueryString = ent.Type == (int)RedirectionTypeEnum.Match && ent.Pattern.Contains("?") ? true : ent.UseQueryString;
            ent.StoreId = store.Id;
            await _redirectionRuleEntityRepository.InsertAsync(ent);
            
            return null;
        }

    }
}
