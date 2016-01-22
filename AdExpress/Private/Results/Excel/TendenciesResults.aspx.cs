#region Informations
/*
Auteur : A.Obermeyer
Création : 10/02/2005
*/
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using TNS.AdExpress.Constantes.Customer;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;

namespace AdExpress.Private.Results.Excel
{

    /// <summary>
    /// Export Excel pour le module Tendances
    /// </summary>
    public partial class TendenciesResults : TNS.AdExpress.Web.UI.ExcelWebPage
    {

        #region Variables
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string result = "";
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public TendenciesResults()
            : base()
        {
        }
        #endregion

        #region Chargement de la page
        /// <summary>
        /// Chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Response.ContentType = "application/vnd.ms-excel";

                #region Sélection du vehicle
                string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
                DBClassificationConstantes.Vehicles.names vehicleName = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.TendenciesDataAccessException("La sélection de médias est incorrecte"));
                #endregion

                VehicleInformation vhInfo = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
                _webSession.DetailLevel = vhInfo.TrendsDefaultMediaSelectionDetailLevel;

                _webSession.Save();
                //#region Calcul du résultat
                //result=TNS.AdExpress.Web.UI.Results.TendenciesUI.GetExcelTendenciesUI(_webSession,vehicleName,true);
                //#endregion

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }

        }
        #endregion

        #region Déchargement de la page
        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        private void Page_UnLoad(object sender, System.EventArgs e)
        {
        }
        #endregion
        #region DeterminePostBackMode
        /// <summary>
        /// Evaluation de l'évènement PostBack:
        ///		base.DeterminePostBackMode();
        ///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            _resultWebControl.CustomerWebSession = _webSession;
            return tmp;
        }
        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Argument</param>
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
