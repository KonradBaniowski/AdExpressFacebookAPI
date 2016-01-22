using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AlertPreRoll.Domain
{
    public class PreRollItem
    {
        public long IdProduct { get; set; }
        public string Product { get; set; }
        public long IdAdvertiser { get; set; }
        public string Advertiser { get; set; }
        public long IdSector { get; set; }
        public string Sector { get; set; }
        public long IdSubSector { get; set; }
        public string SubSector { get; set; }
        public long IdGroup { get; set; }
        public string Group_ { get; set; }
        public long IdSegment { get; set; }
        public string Segment { get; set; }
        public long IdCategory { get; set; }
        public string Category { get; set; }
        public long IdMedia { get; set; }
        public string Media { get; set; }
        public long DateMediaNum { get; set; }
        public long Occurence { get; set; }
        public long Version { get; set; }
        public string Url { get; set; }
    }
}
