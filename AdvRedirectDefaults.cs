using Nop.Core;
using Nop.Core.Caching;

namespace Nop.Plugin.Misc.AdvRedirect
{
    //test
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class AdvRedirectDefaults
    {
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "Misc.AdvRedirect";
        public static string PrefixCacheKey => "Nop.Plugin.Misc.AdvRedirect";

        public static CacheKey ConfigurationsCacheKey => new("Nop.Plugin.Misc.AdvRedirect.Configurations-{0}", PrefixCacheKey);
        

    }
}