using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class GroupItems
    {
        public GroupSelection.GroupType GroupType { get; set; }

        public int GroupId { get; set; }

        public Int64 GroupTextId { get; set; }

        public string GroupName { get; set; }

        public List<SelectItem> Items { get; set; }
    }
}
