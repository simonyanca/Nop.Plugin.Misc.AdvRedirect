using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.AdvRedirect.Models;
using Nop.Plugin.Misc.AdvRedirect.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AdvRedirect.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AdvRedirectController : BasePluginController
    {

        #region Fields
        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly MessageTemplatesSettings _messageTemplatesSettings;
        private readonly RedirectionsService _redirectionsService;
        #endregion

        #region Ctor

        public AdvRedirectController(
             RedirectionsService redirectionsService,
             IEmailAccountService emailAccountService,
             IGenericAttributeService genericAttributeService,
             ILocalizationService localizationService,
             ILogger logger,
             IMessageTemplateService messageTemplateService,
             IMessageTokenProvider messageTokenProvider,
             INotificationService notificationService,
             ISettingService settingService,
             IStaticCacheManager staticCacheManager,
             IStoreContext storeContext,
             IStoreMappingService storeMappingService,
             IStoreService storeService,
             IWorkContext workContext,
             MessageTemplatesSettings messageTemplatesSettings)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _notificationService = notificationService;
            _settingService = settingService;
            _redirectionsService = redirectionsService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _messageTemplatesSettings = messageTemplatesSettings;
        }

        #endregion


        #region Methods

        private async Task PrepareModel(ConfigurationModel model)
        {
            model.Redirections = _redirectionsService.Redirections;
            await Task.FromResult<bool>(true);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            await PrepareModel(model);
            return View("~/Plugins/Misc.AdvRedirect/Views/Configure.cshtml", model);
        }


        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("RemoveRedirection")]
        public async Task<IActionResult> RemoveRedirection(string url)
        {
            _redirectionsService.Redirections.Remove(url);
            _redirectionsService.SaveAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("AddRedirection")]
        public async Task<IActionResult> AddRedirection(string url, string newUrl)
        {
            _redirectionsService.Redirections[url] = newUrl;
            _redirectionsService.SaveAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }



        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            //set API key
            _redirectionsService.SaveAsync();
            
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}