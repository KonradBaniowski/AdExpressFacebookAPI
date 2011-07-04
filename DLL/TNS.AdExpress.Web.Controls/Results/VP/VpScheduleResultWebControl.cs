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
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
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
using TNS.AdExpressI.VP;
namespace TNS.AdExpress.Web.Controls.Results.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:GenericMediaScheduleWebControl runat=server></{0}:GenericMediaScheduleWebControl>")]
    public class VpScheduleResultWebControl : VpScheduleAjaxResultBaseWebControl {

        #region Evènements

        #region PréRender
        /// <summary>
        /// Prérendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
           
            base.OnPreRender(e);
        }
        #endregion

        #endregion

        #region GetHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected override string GetAjaxHTML() {
            TNS.AdExpress.Domain.Web.Navigation.Module _module = ModulesList.GetModule(WebConstantes.Module.Name.VP);
            object[] param = param = new object[1] { _webSession };
            IVeillePromo vpScheduleResult = (IVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryRulesLayer.AssemblyName, _module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            return (vpScheduleResult.GetHtml());
        }
        #endregion

    }
}

