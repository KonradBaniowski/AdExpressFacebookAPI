#region Information
/*
 * auteur : A. Obermeyer
 * créé le : 24/09/2004
 * modifié le : 
 * par :
 * 19/12/2004 A. Obermeyer Intégration de WebPage
 * */
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.UI.Results;

using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DataAccessFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using CstClassification = TNS.AdExpress.Constantes.Classification;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Liste des créations pour l'alerte concurrentielle
	/// </summary>
	public partial class CompetitorAlertCreationsResults : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables
		/// <summary>
		/// Code html du résultat
		/// </summary>
		public string result = "";
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CompetitorAlertCreationsResults():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion					

                #region Language Management
                InformationWebControl1.Language = _webSession.SiteLanguage;
                #endregion

                #region Period Management
                int dateBegin = 0;
                int dateEnd = 0;

                DateTime begin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                DateTime end = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
                DateTime today = DateTime.Today.Date;

                if (begin < today.AddDays(1 - today.Day).AddMonths(-3))
                {//Older than 4M

                    SubPeriodSelectionWebControl1.IsZoomEnabled = true;

                    if (IsPostBack)
                    {

                        SubPeriodSelectionWebControl1.ZoomDate = ZoomParam.Value;
                        begin = WebFunctions.Dates.Max(
                            WebFunctions.Dates.getZoomBeginningDate(ZoomParam.Value, WebConstantes.CustomerSessions.Period.Type.dateToDateMonth),
                            begin);
                        end = WebFunctions.Dates.Min(
                            WebFunctions.Dates.getZoomEndDate(ZoomParam.Value, WebConstantes.CustomerSessions.Period.Type.dateToDateMonth),
                            end);

                    }
                    else
                    {
                        end = begin.AddDays(1 - begin.Day).AddMonths(1).AddDays(-1);
                        ZoomParam.Value = begin.ToString("yyyyMM");
                        SubPeriodSelectionWebControl1.ZoomDate = ZoomParam.Value;
                    }
                }
                else
                {
                    SubPeriodSelectionWebControl1.IsZoomEnabled = false;
                }
                dateBegin = begin.Year * 10000 + begin.Month * 100 + begin.Day;
                dateEnd = end.Year * 10000 + end.Month * 100 + end.Day;
                
                #region PostBack Script
                //Call the internal method Page.RegisterPostBackScript()
                MethodInfo methodInfo =
                typeof(Page).GetMethod("RegisterPostBackScript",
                          BindingFlags.Instance | BindingFlags.NonPublic);

                if (methodInfo != null)
                {
                    methodInfo.Invoke(Page, new object[] { });
                }
                #endregion

                #endregion

                #region Resultat
                string[] ids=null;
				string idVehicle=_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
						
				try {
                    if ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == CstClassification.DB.Vehicles.names.internet)
                        SubPeriodSelectionWebControl1.Visible = false;

					ids = Page.Request.QueryString.Get("ids").Split(',');

					result = CompetitorAlertCreationsResultsUI.GetAlertCompetitorCreationsResultsUI(_webSession, dateBegin, dateEnd, idVehicle,Int64.Parse(ids[0]),int.Parse(ids[1]), this.Page);
					
                    if (result.Length <= 0){
						MenuWebControl2.Visible = false;
					}
					else{
						MenuWebControl2.ForcePrint = "/Private/Results/Excel/CompetitorAlertCreations.aspx?idSession=" + _webSession.IdSession + "&ids=" + idVehicle + "," + ids[0] + "," + ids[1];
					}

				}
				catch (System.Exception exc) {
					//DataAccessFunctions.closeDataBase(_webSession);
					//Response.Write(WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _webSession.SiteLanguage)));
					if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
						this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
					}
				}
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}

		#endregion

		#region DeterminePostBack
		/// <summary>
		/// Determine Post Back Event
		/// </summary>
		/// <returns>Pschitt</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection pstBack = base.DeterminePostBackMode ();

			MenuWebControl2.CustomerWebSession = _webSession;
			MenuWebControl2.ForbidHelpPages = true;

            SubPeriodSelectionWebControl1.WebSession = _webSession;
            _webSession.DetailPeriod =  WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;

			return pstBack;
		}

		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){
			
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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
            this.Unload += new System.EventHandler(this.Page_UnLoad);
          
		}
		#endregion
	}
}
