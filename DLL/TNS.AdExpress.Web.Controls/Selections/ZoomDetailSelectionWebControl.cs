#region Information
// Auteur : Y.R'kaina
// Créé le : 05/09/2007
// Modifié par: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Selections {

    /// <summary>
    /// Composant pour la construction du code html pour le rappel de sélection (dans le cas d'un zoom)
    /// </summary>
    class ZoomDetailSelectionWebControl : DetailSelectionWebControl {

        #region Variables
        /// <summary>
        /// Le libelle de la date de zoom
        /// </summary>
        private string _dateLabel = string.Empty;
        /// <summary>
        /// la date de zoom
        /// </summary>
        private string _zoomDate = string.Empty;
        /// <summary>
        /// Afficher on non la date de zoom
        /// </summary>
        private bool _showZoom = true;
        #endregion

        #region Accesseurs
        /// <summary>
        /// Get/Set Le libelle de la date de zoom
        /// </summary>
        public string DateLabel {
            get { return _dateLabel; }
            set { _dateLabel = value; }
        }
        /// <summary>
        /// Get/Set la date de zoom
        /// </summary>
        public string ZoomDate {
            get { return _zoomDate; }
            set { _zoomDate = value; }
        }
        /// <summary>
        /// Get/Set si on doit afficher ou non la date de zoom
        /// </summary>
        public bool ShowZoom {
            get { return _showZoom; }
            set { _showZoom = value; }
        }
        #endregion

        #region GetDateSelected
        /// <summary>
		/// Dates sélectionnées
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="currentModule">Module en cours</param>
		/// <param name="dateFormatText">Booléen date en format texte</param>
		/// <param name="periodBeginning">Date de début</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <returns>HTML</returns>
		/// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
        protected override string GetDateSelected(WebSession webSession, Module currentModule, bool dateFormatText, string periodBeginning, string periodEnd) {

            if (ShowZoom) {
                if (_zoomDate.Length > 0)
                    return ("<tr><td colspan=4 class=\"" + this.CssTitleData + "\"><font class=\"" + this.CssTitle + "\">" + _dateLabel + " :</font> " + _zoomDate + "</td></tr>");
                else
                    return "";
            }
            else
                return base.GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd);

        }
        #endregion
    }
}
