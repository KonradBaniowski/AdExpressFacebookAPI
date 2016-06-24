using System.Collections.Generic;

namespace TNS.AdExpress.Domain.Results
{
    public class GridResultExport
    {
        public List<InfragisticData> Data { get; set; }
        public List<InfragisticColumn> Columns { get; set; }
        public bool HasData { get; set; }
    }
}