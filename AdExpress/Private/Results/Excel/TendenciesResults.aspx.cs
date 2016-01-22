#region Informations
/*
Auteur : A.Obermeyer
Cr�ation : 10/02/2005
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
        /// Code HTML du r�sultat
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
        /// <param name="sender">Objet qui lance l'�v�nement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                Response.ContentType = "application/vnd.ms-excel";

                #region S�lection du vehicle
                string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, Right.type.vehicleAccess);
                DBClassificationConstantes.Vehicles.names vehicleName = (DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new WebExceptions.TendenciesDataAccessException("La s�lection de m�dias est incorrecte"));
                #endregion

                VehicleInformation vhInfo = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
                _webSession.DetailLevel = vhInfo.TrendsDefaultMediaSelectionDetailLevel;

                _webSession.Save();
                //#region Calcul du r�sultat
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

        #region D�chargement de la page
        /// <summary>
        /// Ev�nement de d�chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'�v�nement</param>
        /// <param name="e">Arguments</param>
        private void Page_UnLoad(object sender, System.EventArgs e)
        {
        }
        #endregion
        #region DeterminePostBackMode
        /// <summary>
        /// Evaluation de l'�v�nement PostBack:
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

        #region Code g�n�r� par le Concepteur Web Form
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Argument</param>
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette m�thode avec l'�diteur de code.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
