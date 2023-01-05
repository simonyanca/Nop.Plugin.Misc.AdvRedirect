using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Plugin.Misc.AdvRedirect.Services;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.AdvRedirect.Infrastructure
{
    public static class CustomExtensions
    {
        public static void UseCustomRedirect(this IApplicationBuilder application)
        {
            application.UseMiddleware<RedirectMiddleware>();
        }
    }

    public class RedirectMiddleware
    {
        #region Fields

        private readonly RedirectionsService _redirectionService;
        private readonly RequestDelegate _next;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Ctor

        public RedirectMiddleware(IAuthenticationSchemeProvider schemes, RedirectionsService redirectionService, ISettingService settingService, IStoreContext storeContext, RequestDelegate next)
        {
            Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _redirectionService = redirectionService;
            _settingService = settingService;
            _storeContext = storeContext;

        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke middleware actions
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == "GET" )
            {
                var newUrl = "";
                if (_redirectionService.Redirections.TryGetValue(context.Request.Path.Value, out newUrl))
                {
                    var parsed = HttpUtility.UrlEncode(newUrl);
                    if (parsed.StartsWith("%2f"))
                        parsed = "/" + parsed.Substring(3);

                    context.Request.Path = parsed;
                    context.Response.Redirect(parsed, true);
                }
            }

            await _next(context);
        }

        #endregion
    }
}
    

