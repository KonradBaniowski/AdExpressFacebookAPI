using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class SelectItem
    {
        public string Text { get; set; }

        public string Value { get; set; }

        public bool slogan { get; set; }

        public bool Enabled { get; set; }

        public int GroupId { get; set; }

        public GroupSelection.GroupType GroupType { get; set; }

        public Int64 GroupTextId { get; set; }
    }
}
