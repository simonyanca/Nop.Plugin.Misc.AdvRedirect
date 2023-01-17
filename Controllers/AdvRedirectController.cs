using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.AdvRedirect.Models;
using Nop.Plugin.Misc.AdvRedirect.Models.Redirections;
using Nop.Plugin.Misc.AdvRedirect.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Services.Messages;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;
using Nop.Web.Framework.Models.Extensions;
using Nop.Plugin.Tax.Avalara.Models.Log;
using Nop.Plugin.Misc.AdvRedirect.Entity;

namespace Nop.Plugin.Misc.AdvRedirect.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AdvRedirectController : BasePluginController
    {
        private readonly INotificationService _notificationService;

        #region Fields
        private readonly RedirectionsService _redirectionsService;
        #endregion

        #region Ctor

        public AdvRedirectController(RedirectionsService redirectionsService, INotificationService notificationService)
        {
            _redirectionsService = redirectionsService;
            _notificationService = notificationService;
        }

        #endregion


        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("GetRedirections")]
        public async Task<IActionResult> GetRedirections(RedirectionSearchModel searchModel)
        {
            var data = (await _redirectionsService.GetAsync(searchModel)).AsQueryable();

            if(searchModel.order.Any())
                if (searchModel.order[0].dir == "asc")
                    data = data.OrderBy(a => a.GetType().GetProperty(searchModel.Columns[searchModel.order[0].column].data).GetValue(a, null));
                else
                    data = data.OrderByDescending(a => a.GetType().GetProperty(searchModel.Columns[searchModel.order[0].column].data).GetValue(a, null));

            var paged = await data.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);

            var model = new RedirectionsListModel().PrepareToGrid(searchModel, paged,  () =>
            {
                return paged.Select(l => l.ToModel<RedirectionModel>());
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
            string errors = await _redirectionsService.AddRedirectionRuleAsync(model.ToEntity<RedirectionRuleEntity>());
            if (!errors.IsNullOrEmpty())
                return ErrorJson($"{errors}");
            else
                _redirectionsService.SaveAsync();

            return Json(new { Result = errors.IsNullOrEmpty() });
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
            _redirectionsService.RemoveRedirection(model.Id);
            _redirectionsService.SaveAsync();
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