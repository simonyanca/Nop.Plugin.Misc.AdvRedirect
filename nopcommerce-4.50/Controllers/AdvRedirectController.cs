
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.AdvRedirect.Models;
using Nop.Plugin.Misc.AdvRedirect.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Nop.Web.Framework.Models.Extensions;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Misc.AdvRedirect.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AdvRedirectController : BasePluginController
    {

        #region Fields
        private readonly RedirectionsService _redirectionsService;
        #endregion

        #region Ctor

        public AdvRedirectController(RedirectionsService redirectionsService)
        {
            _redirectionsService = redirectionsService;
        }

        #endregion


        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("GetRedirections")]
        public async Task<IActionResult> GetRedirections(RedirectionSearchModel searchModel)
        {
            var data =  (await _redirectionsService.GetAllRedirectionsAsync(searchModel));

            var model = new RedirectionsListModel().PrepareToGrid(searchModel, data,  () =>
            {
                return data.Select(l => l.ToModel<RedirectionModel>());
            });

            return Json(model);
        }

      

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("RedirectAdd")]
        public async Task<IActionResult> RedirectAdd(RedirectionModel model)
        {
            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());
            
            string errors = await _redirectionsService.InsertRedirectionsAsync(model.ToEntity<RedirectionRule>());
            if (!string.IsNullOrEmpty(errors))
                return ErrorJson($"{errors}");

            return Json(new { Result = string.IsNullOrEmpty(errors) });
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            ConfigurationModel model = new ConfigurationModel()
            {
                SearchModel = new RedirectionSearchModel() { AvailablePageSizes = "10,20,30" }
            };

            foreach (RedirectionTypeEnum r in (RedirectionTypeEnum[])Enum.GetValues(typeof(RedirectionTypeEnum)))
            {
                model.AvailableTypes.Add(new SelectListItem() { Text = r.ToString(), Selected = !model.AvailableTypes.Any(), Value = r.ToString() });
            }
            
            return View("~/Plugins/Misc.AdvRedirect/Views/Configure.cshtml", model); 
        }


        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("RedirectRemove")]
        public IActionResult RedirectRemove(RedirectionModel model)
        {
            _redirectionsService.DeleteRedirectionAsync(model.ToEntity<RedirectionRule>());
            return Json(new { Result = true });
        }


        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return  Configure();

            return Configure();
        }

        #endregion
    }
}