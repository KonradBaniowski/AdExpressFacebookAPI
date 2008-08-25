#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
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
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Page Excel d'un calendrier d'action d'un plan Média Concurentiel.
	/// </summary>
	public partial class CompetitorMediaPlanResults : TNS.AdExpress.Web.UI.ExcelWebPage{

        //#region Variables
        ///// <summary>
        ///// Code HTML du résultat
        ///// </summary>
        //public string result="";		
        ///// <summary>
        ///// Identifiant de session
        ///// </summary>
        //public string idsession="";
        //#endregion

        //#region Evènements

        //#region Constructeur
        ///// <summary>
        ///// Constructeur
        ///// </summary>
        //public CompetitorMediaPlanResults():base(){			
        //    idsession=HttpContext.Current.Request.QueryString.Get("idSession");
        //}
        //#endregion

        //#region Chargement de la page
        ///// <summary>
        ///// Evènement de chargement de la page
        ///// </summary>
        ///// <param name="sender">Objet qui lance l'évènement</param>
        ///// <param name="e">Arguments</param>
        //protected void Page_Load(object sender, System.EventArgs e){
        //    try{

        //        #region Calcul du résultat
        //        // On charge les données
        //        result=CompetitorMediaPlanAnalysisUI.GetMediaPlanAnalysisExcelUI(CompetitorMediaPlanAnalysisRules.GetFormattedTable(_webSession),_webSession);
        //        #endregion
        //    }
        //    catch(System.Exception exc){
        //        if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
        //            this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
        //        }
        //    }
        //}
        //#endregion

        //#region Déchargement de la page
        ///// <summary>
        ///// Evènement de déchargement de la page
        ///// </summary>
        ///// <param name="sender">Objet qui lance l'évènement</param>
        ///// <param name="e">Arguments</param>
        //protected void Page_UnLoad(object sender, System.EventArgs e){			
        //}
        //#endregion
		
        ///// <summary>
        ///// DeterminePostBackMode
        ///// </summary>
        ///// <returns></returns>
        //protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
        //    System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
        //    return tmp;
        //}	
        //#endregion

        //#region Code généré par le Concepteur Web Form
        ///// <summary>
        ///// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
        ///// </summary>
        ///// <param name="e">Arguments</param>
        //override protected void OnInit(EventArgs e) {
        //    //
        //    // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
        //    //
        //    InitializeComponent();
        //    base.OnInit(e);
        //}
		
        ///// <summary>
        ///// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        ///// le contenu de cette méthode avec l'éditeur de code.
        ///// </summary>
        //private void InitializeComponent() {
           
        //    this.Unload += new System.EventHandler(this.Page_UnLoad);
        //}
        //#endregion
	}
}
