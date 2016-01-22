using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KMI.PromoPSA.JQGrid {
    [ModelBinder(typeof(GridModelBinder))]
    public class GridSettings {

        public bool IsSearch { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public Filter Where { get; set; }

    }
}
