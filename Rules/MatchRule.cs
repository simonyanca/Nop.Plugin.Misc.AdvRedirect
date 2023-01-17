using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Rules
{
    public class MatchRule : IRule
    {
        private string _matchUrl;
        private string _redirectUrl;

        public MatchRule(string url, string redirectUrl)
        {
            _redirectUrl = redirectUrl;
            _matchUrl = url;
        }


        public string RedirectUrl
        {
            get
            {
                return _redirectUrl;
            }
        }

        public bool Match(string url)
        {
            return _matchUrl == url;
        }
    }
}
