using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Plugin.Misc.AdvRedirect.Domain;
using Nop.Plugin.Misc.AdvRedirect.Models.Redirections;

namespace Nop.Plugin.Misc.AdvRedirect.Services
{
    public interface IDataIOService
    {
        void Import(string csvText);

		Task<string> Export();
	}
}