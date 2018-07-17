﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Insertions;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.Results
{

    public class GridResult
    {

        public GridResult()
        {
            this.LinkAllPeriod = null;
            this.Data = null;
        }

        public object[,] Data { get; set; }
        public List<object> Columns { get; set; }
        public List<object> Schema { get; set; }
        public List<object> ColumnsFixed { get; set; }
        public List<object> ColumnsNotAllowedSorting { get; set; }
        public string Title { get; set; }
        public bool NeedFixedColumns { get; set; }
        public bool HasData { get; set; }
        public bool HasMoreThanMaxRowsAllowed { get; set; }
        public bool HasMSCreatives { get; set; }
        public string Unit { get; set; }
        public List<WebConstantes.CustomerSessions.Unit> Units { get; set; }
        public Filter Filter { get; set; }
        public string LinkAllPeriod { get; set; }
        public int SortOrder { get; set; }
        public int SortKey { get; set; }
    }
}
