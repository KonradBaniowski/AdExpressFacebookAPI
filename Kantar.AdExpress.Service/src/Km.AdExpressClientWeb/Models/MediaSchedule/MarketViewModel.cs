using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TNS.Classification.Universe;

namespace Km.AdExpressClientWeb.Models.MediaSchedule
{
    public class MarketViewModel
    {
        public string KeyWordLabel { get; set; }// 893
        public string KeyWordDescription { get; set; }//phrase under search input 2287
        public string ErrorMessage { get; set; }//930
        public string KeyWord { get; set; }
        public string ElementLabel { get; set; }//
        public string BranchLabel { get; set; }//
        public Dimension Dimension { get; set; }
        public List<UniversBranch> Branches { get; set; }

    }

    public class UniversBranch
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public int LabelId { get; set; }
        public bool IsSelected { get; set; } 
        public List<UniversLevel> UniversLevels { get; set; }
    }

    public class UniversLevel
    {
        public long LevelId { get; set; }
        public int LabelId { get; set; }
        public string Label { get; set; }
        public int Capacity { get; set; }
        public string OverLimitMessage { get; set; }//2286
        public string SecurityMessage { get; set; }//2285
        public string ExceptionMessage { get; set; }//922

        public List<UniversItem> UniversItems { get; set; }
    }

    public class UniversItem
    {
        
        public long ID { get; set; }
        public int LabelId { get; set; }
        public string Label { get; set; }

    }
}