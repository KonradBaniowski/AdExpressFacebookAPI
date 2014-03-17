using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace KMI.PromoPSA.JQGrid {
    public class GridModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext,
                                ModelBindingContext bindingContext) {
            try {
                var request = controllerContext.HttpContext.Request;
                return new GridSettings {
                    IsSearch = bool.Parse(request["_search"] ?? "false"),
                    PageIndex = int.Parse(request["page"] ?? "1"),
                    PageSize = int.Parse(request["rows"] ?? "10"),
                    SortColumn = request["sidx"] ?? "",
                    SortOrder = request["sord"] ?? "asc",
                    Where = Filter.Create(request["filters"] ?? "")
                };
            }
            catch {
                return null;
            }
        }
    }
}
