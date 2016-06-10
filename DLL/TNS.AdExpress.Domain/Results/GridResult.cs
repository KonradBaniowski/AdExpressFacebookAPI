using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Insertions;

namespace TNS.AdExpress.Domain.Results
{

    public class GridResult
    {
        public object[,] Data { get; set; }
        public List<object> Columns { get; set; }
        public List<object> Schema { get; set; }
        public List<object> ColumnsFixed { get; set; }
        public string Title { get; set; }
        public bool NeedFixedColumns { get; set; }
        public bool HasData { get; set; }
        public bool HasMSCreatives { get; set; }
        public string Unit { get; set; }
        public Filter Filter { get; set; }
        public bool isOneColumnLine { get; set; }
}
}
