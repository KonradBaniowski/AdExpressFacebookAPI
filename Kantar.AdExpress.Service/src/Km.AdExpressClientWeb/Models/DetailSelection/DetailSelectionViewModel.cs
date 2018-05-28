using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.DetailSelection
{
    public class DetailSelectionViewModel
    {
        public DetailSelectionViewModel()
        {
            DetailSelectionWSModel = new DetailSelectionWSModel();
        }
        public Labels Labels { get; set; }
        public DetailSelectionWSModel DetailSelectionWSModel { get; set; }
        public string Message { get; set; }
    }

    public class DetailSelectionWSModel
    {
        public int SiteLanguage { get; set; }
        public string ModuleLabel { get; set; }
        public string NiveauDetailLabel { get; set; }
        public string GenericLevelDetailColumn { get; set; }
        public string UniteLabel { get; set; }
        public List<Tree> UniversMarket { get; set; }
        public List<Tree> UniversMedia { get; set; }
        public string MediasSelectedLabel { get; set; }
        public string SponsorshipMediasSelectedLabel { get; set; }
        public string Dates { get; set; }
        public string StudyPeriod { get; set; }
        public string ComparativePeriod { get; set; }
        public string ComparativePeriodType { get; set; }
        public string PeriodDisponibilityType { get; set; }
        public string IdSlogansLabel { get; set; }

        public bool ShowDate { get; set; }
        public bool ShowUnivers { get; set; }
        public bool ShowUniversDetails { get; set; }
        public bool ShowMarket { get; set; }
        public bool ShowGenericlevelDetail { get; set; }
        public bool ShowGenericLevelDetailColumn { get; set; }
        public bool ShowStudyPeriod { get; set; }
        public bool ShowComparativePeriod { get; set; }
        public bool ShowComparativePeriodType { get; set; }
        public bool ShowPeriodDisponibilityType { get; set; }
        public bool ShowUnity { get; set; }
        public bool ShowStudyType { get; set; }
        public bool ShowSponsorshipMedia { get; set; }
        public bool ShowIdSlogansLabel { get; set; }
    }
}