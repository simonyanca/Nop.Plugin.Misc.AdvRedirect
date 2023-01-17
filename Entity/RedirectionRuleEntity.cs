using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Misc.AdvRedirect.Models;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AdvRedirect.Entity
{
    public class RedirectionRuleEntity: BaseEntity
    {
        [Required]
        public string Pattern { get; set; }

        [Url]
        public string RedirectUrl { get; set; }

        [Required]
        public RedirectionTypeEnum Type { get; set; }
    }


}
