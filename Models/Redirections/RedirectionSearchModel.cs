using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.AdvRedirect.Models.Redirections
{
    public record RedirectionSearchModel : BaseSearchModel
    {
        public ColumnOptions[] Columns { get; set; }
        public Order[] order { get; set; }
    }

    public class ColumnOptions
    {
        public string data { get; set; }
        public string name { get; set; }

        public bool searchable { get; set; }

        public bool orderable { get; set; }

    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

}
