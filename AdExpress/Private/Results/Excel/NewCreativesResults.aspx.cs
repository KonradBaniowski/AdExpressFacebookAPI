﻿#region Information
// Auteur B.Masson
// Date de création : 03/10/08
#endregion

#region Namespace
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;

using NewCreatives = TNS.AdExpressI.NewCreatives;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;
#endregion

namespace AdExpress.Private.Results.Excel {
    
    /// <summary>
    /// New creatives excel result
    /// </summary>
    public partial class NewCreativesResults : TNS.AdExpress.Web.UI.ExcelWebPage {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public NewCreativesResults(): base() {
        }
        #endregion

        #region DeterminePostBackMode
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
            _resultWebControl.CustomerWebSession = _webSession;
            return base.DeterminePostBackMode();
        }
        #endregion

        #region On PreInit
        /// <summary>
        /// On preinit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {
            base.OnPreInit(e);
        }
        #endregion

        #region Chargement de la page
        /// <summary>
        /// Chargement de la page
        /// </summary>
        /// <param name="sender">page</param>
        /// <param name="e">arguments</param>
        protected void Page_Load(object sender, EventArgs e) {
            try {
                Response.ContentType = "application/vnd.ms-excel";

                Domain.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
                if(module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the new creatives excel result"));
            }
            catch(System.Exception exc) {
                if(exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
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
        private void Page_UnLoad(object sender, System.EventArgs e) {
        }
        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Evènement d'initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        override protected void OnInit(EventArgs e) {
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
        private void InitializeComponent() {

        }
        #endregion

    }
}