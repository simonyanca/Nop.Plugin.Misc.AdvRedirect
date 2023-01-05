using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.AdvRedirect.Models
{
    /// <summary>
    /// Represents plugin configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        public string Url { get; set; }
        public string newUrl { get; set; }

        [Required]
        public Dictionary<string,string> Redirections { get; set; }

    }
}