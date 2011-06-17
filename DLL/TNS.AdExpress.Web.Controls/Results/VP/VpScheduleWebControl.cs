#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
//		G Ragneau - 08/08/2006 - Set GetHtml as public so as to access it 
//		G Ragneau - 08/08/2006 - GetHTML : Force media plan alert module and restaure it after process (<== because of version zoom);
//		G Ragneau - 05/05/2008 - GetHTML : implement layers
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using AjaxPro;
using TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using FrmFct = TNS.FrameWork.WebResultUI.Functions;
using TNS.FrameWork.Date;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.WebResultUI;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CustomCst = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Insertions;
namespace TNS.AdExpress.Web.Controls.Results.VP{
	/// <summary>
	/// Affiche le résultat d'une alerte plan media
	/// </summary>
	[DefaultProperty("Text"),
      ToolboxData("<{0}:GenericMediaScheduleWebControl runat=server></{0}:GenericMediaScheduleWebControl>")]
    public class VpScheduleWebControl : System.Web.UI.WebControls.WebControl
    {

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        #endregion

        #region PréRender
        /// <summary>
		/// Prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) 
		{

			base.OnPreRender (e);
		
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){

			base.Render(output);
		}
		#endregion

		#endregion

	}
}

