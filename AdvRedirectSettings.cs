using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.AdvRedirect
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class AdvRedirectSettings : ISettings
    {
        public string PhoneNumber { get; set; }
    }
}