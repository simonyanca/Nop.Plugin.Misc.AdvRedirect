using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.AdvRedirect.Components;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Misc.AdvRedirect
{   
    public class AdvRedirect : BasePlugin, IMiscPlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly WidgetSettings _widgetSettings;
        private readonly ISettingService _settingService;

        public bool HideInWidgetList => true;

        public AdvRedirect(  IWebHelper webHelper, ILocalizationService localizationService, WidgetSettings widgetSettings, ISettingService settingService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _widgetSettings = widgetSettings;
            _settingService = settingService;
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(WidgetsAdvRedirectViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.Footer });
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/AdvRedirect/Configure";
        }

        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.AdvRedirect.Fields.PhoneNumber"] = "Phone number"
            });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(AdvRedirectDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(AdvRedirectDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }
}