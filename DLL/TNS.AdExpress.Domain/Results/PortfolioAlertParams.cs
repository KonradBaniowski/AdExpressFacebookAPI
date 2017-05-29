
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.Results
{
    public class PortfolioAlertParams
    {
        public int TypeAlertId { get; set; }
        public int MediaId { get; set; }
        public int LanguageId { get; set; }
        public string AlertName { get; set; }
        public string SectorListId { get; set; }
        public string SubSectorListId { get; set; }
        public string GroupListId { get; set; }
        public string SegmentListId { get; set; }
        public string MediaName { get; set; }
        public List<string> EmailList { get; set; }
        public bool Inset { get; set; }
        public bool Autopromo { get; set; }

    }
}
