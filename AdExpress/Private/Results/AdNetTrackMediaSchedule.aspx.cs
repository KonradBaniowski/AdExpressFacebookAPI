#region Informations
// Auteur: G. Facon
// Date de création: 26/02/2007
// Date de modification:
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
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassifCst = TNS.AdExpress.Constantes.Classification.DB;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.DB.Common;

namespace AdExpress.Private.Results{
	/// <summary>
	/// Page du Plan Média AdNetTrack
	/// </summary>
	public partial class AdNetTrackMediaSchedule : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables MMI
		
		#endregion

		#region Variables
		/// <summary>
		/// Resultat
		/// </summary>
		public string result="";
        /// <summary>
        /// Le lien vers le résultat
        /// </summary>
		public string _returnLink="";
		/// <summary>
		/// Type de sélection produit
		/// </summary>
		TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type selectionType;
		/// <summary>
		/// Valeur de la sélection produit
		/// </summary>
		Int64 id = 0;
		/// <summary>
		/// Zoom Date
		/// </summary>
		protected string _zoomDate = string.Empty;
        /// <summary>
        /// La liste de parametres
        /// </summary>
		protected string _urlParameters = string.Empty;
        /// <summary>
        /// L'identificateur de l'univers
        /// </summary>
        protected int _universId = -1;
        /// <summary>
        /// L'identificateur du module
        /// </summary>
        protected Int64 _moduleId = -1;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public AdNetTrackMediaSchedule():base(){
		}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){		
			try{
		
				#region Url Suivante
				//				_nextUrl=this.recallWebControl.NextUrl;
				if(_nextUrl.Length!=0){
					DBFunctions.closeDataBase(_webSession);
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
				}
			
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok"){
					_webSession.Save();				
				}
				#endregion
			
				#region Texte et langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				#endregion
					
				#region Chargement des paramètres
				 selectionType=(TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type) int.Parse(HttpContext.Current.Request.QueryString.Get("idLevel"));
				 id=Int64.Parse(HttpContext.Current.Request.QueryString.Get("id"));
				_zoomDate = HttpContext.Current.Request.QueryString.Get("zoomDate");
				_urlParameters = HttpContext.Current.Request.QueryString.Get("urlParameters");
                if(HttpContext.Current.Request.QueryString.Get("universId").Length > 0)
                    _universId = int.Parse(HttpContext.Current.Request.QueryString.Get("universId"));
                if(HttpContext.Current.Request.QueryString.Get("moduleId").Length > 0)
                    _moduleId = int.Parse(HttpContext.Current.Request.QueryString.Get("moduleId"));
                string page = HttpContext.Current.Request.QueryString.Get("page");
				if(_urlParameters !=null && _urlParameters.Length>0)
					AlertAdNetTrackMediaScheduleWebControl1.UrlParameters = _urlParameters;
                AlertAdNetTrackMediaScheduleWebControl1.ModuleId = _moduleId;
                AlertAdNetTrackMediaScheduleWebControl1.UniversId = _universId;
                AlertAdNetTrackMediaScheduleWebControl1.CurrentPage = page;
				_webSession.AdNetTrackSelection=new AdNetTrackProductSelection(selectionType,id);
                #endregion 

                #region Return Button
                Random rand = new Random();
                _returnLink = "&nbsp;<a href=\"/Private/Results/Creatives.aspx?idSession=" + _webSession.IdSession + "&ids=" + _urlParameters + "&zoomDate=" + _zoomDate + "&idUnivers=" + _universId + "&moduleId=" + _moduleId + "&param=" + rand.Next()+ "&page=" + ((page!=null && page.Length > 0)?page:"1") + "&vehicleId=" + DBClassifCst.Vehicles.names.adnettrack.GetHashCode() + "\" class=\"roll06\">" + GestionWeb.GetWebWord(2159, _webSession.SiteLanguage) + "</a>";
				#endregion

                #region Period Management
                int dateBegin = 0;
                int dateEnd = 0;

                DateTime begin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                DateTime today = DateTime.Today.Date;

                if (begin < today.AddDays(1 - today.Day).AddMonths(-3))
                {//Older than 4M

                    SubPeriodSelectionWebControl1.IsZoomEnabled = true;

                    if (!IsPostBack)
                    {
                        string zoomDate = Page.Request.QueryString.Get("zoomDate");
                        if (zoomDate != null && zoomDate.Length > 0)
                        {
                            ZoomParam.Value = zoomDate;
                        }
                        else
                        {
                            ZoomParam.Value = begin.ToString("yyyyMM");
                        }
                        SubPeriodSelectionWebControl1.ZoomDate = ZoomParam.Value;
                    }
                    SubPeriodSelectionWebControl1.ZoomDate = ZoomParam.Value;
                    AlertAdNetTrackMediaScheduleWebControl1.ZoomDate = ZoomParam.Value;

                }
                else
                {
                    SubPeriodSelectionWebControl1.IsZoomEnabled = false;
                }
                _zoomDate = ZoomParam.Value;

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


			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}

		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns>DeterminePostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			AlertAdNetTrackMediaScheduleWebControl1.CustomerWebSession =	_webSession;
            SubPeriodSelectionWebControl1.WebSession = _webSession;
			OptionLayerWebControl1.CustomerWebSession = _webSession;
			MenuWebControl1.CustomerWebSession = _webSession;
			return(tmp);
		}
		#endregion

		#region Initialisation
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

		#region Prérender
		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			try{
				#region MAJ _webSession
				_webSession.Save();
				#endregion

				MenuWebControl1.ForbidHelpPages = true;
				//MenuWebControl1.ForceHelp = "/Private/Results/Helps/.../ .aspx?idSession="+_webSession.IdSession;
				MenuWebControl1.ForcePrint = "/Private/Results/Excel/AdNetTrackMediaSchedule.aspx?idSession="+_webSession.IdSession+"&idLevel="+selectionType.GetHashCode()+"&id="+id+"&zoomDate="+_zoomDate;
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#endregion

	}
}
