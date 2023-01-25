
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using Nop.Plugin.Misc.CodeInjector.Extensions;

namespace Nop.Plugin.Misc.CodeInjector.Components
{
    /// <summary>
    /// Represents the view component to display logo
    /// </summary>
    public class CodeViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CodeViewComponent(IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            string jsonString = JsonSerializer.Serialize(additionalData);
            var dynamicObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonString)!;

            bool o = additionalData.GetPropValue<bool>("IsHomePage");
            return new HtmlContentViewComponentResult(new HtmlString(o.ToString()));
        }

        #endregion
    }
}