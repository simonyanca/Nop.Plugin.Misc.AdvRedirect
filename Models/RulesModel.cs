using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AdvRedirect.Entity;
using Nop.Plugin.Misc.AdvRedirect.Rules;

namespace Nop.Plugin.Misc.AdvRedirect.Models
{
    public class RulesModel
    {
        public List<IRule> Rules;
        public Dictionary<string, string> Match;
    }
}
