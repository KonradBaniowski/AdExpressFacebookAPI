using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UserUniversGroup
    {
        public long Id { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public List<UserUnivers> UserUnivers { get; set; }
    }

    public class UserUnivers
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long ParentId { get; set; }
        public string ParentDescription { get; set; }
        public List<UniversLevel> Levels { get; set; }
    }

    public class UniversGroupsResponse
    {
        public List<UserUniversGroup> UniversGroups { get; set; }
        public int SiteLanguage { get; set; }
    }

    public class UniversGroupSaveResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int UserUniversId { get; set; }
    }
    public class UniversGroupSaveRequest
    {
        public long UniversGroupId { get; set; }
        public long? UserUniversId { get; set; }
        public List<Tree> Trees { get; set;}
        public string Name { get; set; }
        public Dimension Dimension { get; set; }
        public string WebSessionId { get; set; }
        public long IdUniverseClientDescription { get; set; }
        public List<long> MediaIds { get; set; }
    }

    public class UniversResponse
    {
        public List<Tree> Trees { get; set; }

        public List<long> UniversMediaIds { get; set; }
        public string Message { get; set; }
        public long ModuleId { get; set; }
    }

}
