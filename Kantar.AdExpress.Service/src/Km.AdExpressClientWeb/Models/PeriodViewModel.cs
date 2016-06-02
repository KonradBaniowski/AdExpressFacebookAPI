using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class PeriodViewModel
    {
        #region Constantes
        /// <summary>
        /// Nombre de jours à afficher pour la Sélection de périodes glissantes
        /// </summary>
        private const int DAYS_NUMBER = 31;
        /// <summary>
        /// Nombre de semaines à afficher pour la Sélection de périodes glissantes
        /// </summary>
        private const int WEEKS_NUMBER = 53;
        /// <summary>
        /// Nombre de mois à afficher pour la Sélection de périodes glissantes
        /// </summary>
        private int MONTHS_NUMBER = 12;
        /// <summary>
        /// Nombre d'année à afficher pour la Sélection de périodes glissantes
        /// </summary>
        private const int YEARS_NUMBER = 3;
        #endregion

        public PeriodViewModel()
        {

        }
        public PeriodViewModel(int currentModuleId)
        {
            if (currentModuleId == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR || currentModuleId == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE)
                MONTHS_NUMBER = 5;
        }

        public int SlidingYearsNb
        {
            get { return YEARS_NUMBER; }
        }

        public int SlidingMonthsNb
        {
            get { return MONTHS_NUMBER; }
        }

        public int SlidingWeeksNb
        {
            get { return WEEKS_NUMBER; }
        }

        public int SlidingDaysNb
        {
            get { return DAYS_NUMBER; }
        }

        public int SiteLanguage { get; set; }

        public string StartYear { get; set; }

        public string EndYear { get; set; }
        public string CalendarFormat { get; set; }
        public string LanguageName { get; set; }


    }
}