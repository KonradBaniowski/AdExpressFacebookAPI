using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.ResultOptions
{
    public class SelectControl
    {
        public string Id { get; set; }

        public string SelectedId { get; set; }

        public List<SelectItem> Items { get; set; }

        public bool Visible { get; set; }
    }
}
