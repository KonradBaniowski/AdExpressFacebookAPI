using System.Collections.Generic;
using TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class Media
    {
        public string icon { get; set; }
        public Vehicles.names MediaEnum { get; set; }
        public long Id { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }

    public class MediaResponse
    {
        public MediaResponse(int siteLanguage, long currentModule )
        {
            ControllerDetails = new ControllerDetails();
            Media = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
            SiteLanguage = siteLanguage;
            MultipleSelection = (currentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA|| currentModule == CstWeb.Module.Name.ANALYSE_MANDATAIRES) ? true : false;
            CanRefineMediaSupport = (currentModule == CstWeb.Module.Name.NEW_CREATIVES)? false :true;
        }
        public MediaResponse()
        {
            ControllerDetails = new ControllerDetails();
            Media = new List<Core.Domain.Media>();
            MediaCommon = new List<int>();
        }
        public List<Media> Media { get; set; }
        public int SiteLanguage { get; set; }
        public List<int> MediaCommon { get; set; }
        public ControllerDetails ControllerDetails { get; set;}
        public bool MultipleSelection { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool CanRefineMediaSupport { get; set;}
    }

    public class ControllerDetails
    {
        public ControllerDetails()
            {
            ModuleCode = 0;
            Name = string.Empty;
            ModuleId = 0;
            ModuleIcon = "icon-chart"; //Par defaut
        }
        public long ModuleCode { get; set; }
        public string Name { get; set; }
        public long ModuleId { get; set; }
        public string ModuleIcon { get; set; }
    }
}
