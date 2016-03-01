using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.MediaSchedule
{
    public class PresentationModel
    {
        public int SiteLanguage { get; set; }

        public List<SavedUnivers> SavedUnivers { get;set;}

        public long SaveUniversCode { get; set; }
        public long LoadUniversCode { get; set; }
        public long MediaScheduleCode { get; set; }
        public long UserUniversCode { get; set; }
    }

    public class SavedUnivers
    {
        public int Id { get; set; }
        public int Count { get; set;}
        public string Name { get; set; }
        public List<UniversLevel> Levels { get; set; }
    }

}