using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nop.Core;
using Nop.Plugin.Misc.AdvRedirect;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.AdvRedirect.Components
{
    /// <summary>
    /// Represents view component to embed tracking script on pages
    /// </summary>
    public class WidgetsAdvRedirectViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly AdvRedirectSettings _settings;

        #endregion

        #region Ctor

        public WidgetsAdvRedirectViewComponent(ICustomerService customerService,
            IWorkContext workContext,
            AdvRedirectSettings settings)
        {
            _customerService = customerService;
            _workContext = workContext;
            _settings = settings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Misc.AdvRedirect/Views/PublicInfo.cshtml", _settings.PhoneNumber);
        }

        #endregion
    }
}