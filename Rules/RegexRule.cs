using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AdvRedirect.Rules
{
    public class RegexRule : IRule
    {
        private Regex _regex;
        private string _redirectUrl;

        public RegexRule(string pattern, string redirectUrl)
        {
            _redirectUrl = redirectUrl;
            _regex = new Regex(pattern);
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
            return _regex.Match(url).Success;
        }
    }
}
