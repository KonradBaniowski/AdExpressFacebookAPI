
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
        public UniversBranchResult(int siteLanguage)
        {
            ControllerDetails = new ControllerDetails();
            Trees = new List<Tree>();
            Branches = new List<UniversBranch>();
            SiteLanguage = siteLanguage;
            MaxUniverseItems = 0;
        }
        public UniversBranchResult()
        {
            ControllerDetails = new ControllerDetails();
            Trees = new List<Tree>();
            Branches = new List<UniversBranch>();
            MaxUniverseItems = 0;
        }
        public UniversBranchResult(int siteLanguage,int defaultBranchId, int count)
        {
            ControllerDetails = new ControllerDetails();
            Trees = new List<Tree>(count);
            Branches = new List<UniversBranch>();
            SiteLanguage = siteLanguage;
            DefaultBranchId = defaultBranchId;
            MaxUniverseItems = 0;
        }
        public List<UniversBranch> Branches { get; set; }
        public int SiteLanguage { get; set; }
        public int DefaultBranchId { get; set; }
        public List<Tree> Trees { get; set; }
        public ControllerDetails ControllerDetails { get; set; }
        public int MaxUniverseItems { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UniversLevel
    {
        public long Id { get; set; }
        public int LabelId { get; set; }
        public string Label { get; set; }
        public long Capacity { get; set; }
        public string OverLimitMessage { get; set; }
        public string SecurityMessage { get; set; }
        public string ExceptionMessage { get; set; }
        public long BranchId { get; set; }

        public List<UniversItem> UniversItems { get; set; }
    }

    public class Tree
    {
        public long LabelId { get; set; }
        public int Id { get; set; }
        public AccessType AccessType { get; set; }
        public List<UniversLevel> UniversLevels { get; set; }
        public string Label { get; set; }
        public bool IsDefaultActive { get; set; }
    }
}
