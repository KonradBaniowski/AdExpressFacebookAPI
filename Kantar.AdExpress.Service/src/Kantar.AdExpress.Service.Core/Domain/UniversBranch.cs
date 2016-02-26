
using System.Collections.Generic;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UniversBranch
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public bool IsSelected { get; set; }
        public List<UniversLevel> UniversLevels { get; set; }
    }

    public class UniversBranchResult

    {
        public List<UniversBranch> Branches { get; set; }
        public int SiteLanguage { get; set; }
        public int DefaultBranchId { get; set; }
    }

    public class UniversLevel
    {
        public long Id { get; set; }
        public int LabelId { get; set; }
        public string Label { get; set; }
        public int Capacity { get; set; }
        public string OverLimitMessage { get; set; }//2286
        public string SecurityMessage { get; set; }//2285
        public string ExceptionMessage { get; set; }//922

        public long BranchId { get; set; }

        //public List<UniversItem> UniversItems { get; set; }
    }
}
