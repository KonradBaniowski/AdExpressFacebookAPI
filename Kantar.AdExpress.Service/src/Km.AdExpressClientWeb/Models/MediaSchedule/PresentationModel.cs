using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.MediaSchedule
{
    public class PresentationModel
    {
        public int SiteLanguage { get; set; }

        public List<UserUniversGroup> UserUniversGroups { get;set;}

        public long SaveUniversCode { get; set; }
        public long LoadUniversCode { get; set; }
        public long ModuleCode { get; set; }
        public long UserUniversCode { get; set; }
        public long ErrorMsgCode { get; set; }
        public long ModuleDecriptionCode { get; set; }
        public bool ShowUserSavedGroups { get; set; }
        public bool ShowCurrentSelection { get; set; }
    }

    public class UserUniversGroup
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public List<ClientUnivers> ClientUnivers { get; set; }
    }

    public class ClientUnivers
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<UniversLevel> Levels { get; set; }
    }

}