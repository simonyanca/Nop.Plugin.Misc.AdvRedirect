using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.IdentityModel.Tokens;
using Nop.Core;

using Nop.Core.Caching;
using Nop.Plugin.Misc.AdvRedirect.Models;
using Nop.Plugin.Misc.AdvRedirect.Models.Redirections;
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
        
        
        #endregion

        #region Ctor

        public RedirectMiddleware(IAuthenticationSchemeProvider schemes,  RedirectionsService redirectionService, RequestDelegate next)
        {
            Schemes = schemes ?? throw new ArgumentNullException(nameof(schemes));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _redirectionService = redirectionService;
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
                string redirectUrl = _redirectionService.ResolveRedirection(context.Request.Path.Value + context.Request.QueryString);
                if (!redirectUrl.IsNullOrEmpty())
                {
                    var parsed = HttpUtility.UrlEncode(redirectUrl);
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
    

