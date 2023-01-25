
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.CodeInjector.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Redirections", "Admin/CodeInjector/GetRedirections/",
            new { controller = "CodeInjector", action = "GetRedirections" });
            endpointRouteBuilder.MapControllerRoute("Redirections", "Admin/CodeInjector/RedirectUpdate/",
            new { controller = "CodeInjector", action = "RedirectUpdate" });
            endpointRouteBuilder.MapControllerRoute("Redirections", "Admin/CodeInjector/RedirectRemove/",
            new { controller = "CodeInjector", action = "RedirectRemove" });

        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
