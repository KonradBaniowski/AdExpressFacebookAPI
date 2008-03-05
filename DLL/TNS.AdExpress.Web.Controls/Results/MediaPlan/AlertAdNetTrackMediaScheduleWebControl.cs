#region Informations
// Auteur: G. Facon 
// Date de création: 20/03/2007
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using AjaxPro;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork.WebResultUI;
using ControlsExceptions = TNS.AdExpress.Web.Controls.Exceptions;
using TNS.FrameWork;

using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;


namespace TNS.AdExpress.Web.Controls.Results.MediaPlan{
	/// <summary>
	/// Affiche le résultat d'une alerte plan media AdNetTrack
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:AlertAdNetTrackMediaScheduleWebControl runat=server></{0}:AlertAdNetTrackMediaScheduleWebControl>")]
	public class AlertAdNetTrackMediaScheduleWebControl : AlertMediaPlanResultWebControl{
		
		
		#region variables
		/// <summary>
		/// Paramètres Url
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
        /// <summary>
        /// la page courante (pagination)
        /// </summary>
        protected string _currentPage = string.Empty;
		#endregion

        #region Accesseurs
        /// <summary>
		/// Paramètres Url
		/// </summary>
		[Bindable(true),
		Category("Url parameters Description"),
		DefaultValue("")]
		public string UrlParameters {
			get{return _urlParameters;}
			set{_urlParameters=value;}
        }
        /// <summary>
        /// Get/Set L'identificateur de l'univers
        /// </summary>
        public int UniversId {
            get { return _universId; }
            set { _universId = value; }
        }
        /// <summary>
        /// Get/Set L'identificateur du module
        /// </summary>
        public Int64 ModuleId {
            get { return _moduleId; }
            set { _moduleId = value; }
        }
        /// <summary>
        /// Get/Set la page courante (pagination)
        /// </summary>
        public string CurrentPage {
            get { return _currentPage; }
            set { _currentPage = value; }
        }
        #endregion

        #region Javascript
        /// <summary>
		/// Génère le code JavaSript pour les méthodes de demande des résultats 
		/// </summary>
		/// <returns></returns>
		protected override string AjaxEventScript(){
			StringBuilder js=new StringBuilder(1000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");

			js.Append("\r\n\t var oParams = new Object();");		
			js.Append("\r\n\n SetParameters(oParams);"); 

			js.Append("\r\nfunction get_"+this.ID+"(){");
			js.Append("\r\n\t"+this.GetType().Namespace+"."+this.GetType().Name+".GetData('"+_customerWebSession.IdSession+"',oParams,get_"+this.ID+"_callback);");
			js.Append("\r\n}");

			js.Append("\r\nfunction get_"+this.ID+"_callback(res){");
			js.Append("\r\n\tvar oN=document.getElementById('res_"+this.ID+"');");
			js.Append("\r\n\toN.innerHTML=res.value;");
			js.Append("\r\n}\r\n");
			js.Append("\r\naddEvent(window, \"load\", get_"+this.ID+");");

			#region  Paramètres utiliser lors de l'appel Ajax des méthodes
			js.Append("\r\n\n function SetParameters(obj){");			
			js.Append("\r\n\t obj.ZoomDate = '"+_zoomDate+"';");
			js.Append("\r\n\t obj.UrlParameters = '"+_urlParameters+"';");
			js.Append("\r\n }");
			#endregion

			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}
		
		#endregion

		#region Méthodes publiques

		#region Méthodes Ajax
		/// <summary>
		/// Obtention du code HTML à insérer dans le composant
		/// </summary>
		/// <param name="sessionId">Session du client</param>
		/// <param name="o">Tableaux de paramètres</param>
		/// <returns>Code HTML</returns>
		[AjaxPro.AjaxMethod]
		public string GetData(string sessionId,AjaxPro.JavaScriptObject o){
			WebSession webSession=null;
			string html;
			try{
				//Obtention de la session
				webSession=(WebSession)WebSession.Load(sessionId);
				
				//Chargement des paramètres
				this.LoadParams(o);

				//Obtention du résultat au format html
				html=GetHTML(webSession);

//				webSession.Save();
			}
			catch(System.Exception err){
				return(OnAjaxMethodError(err,webSession));
			}
			return(html);
		}

		#endregion

		#endregion

		#region Méthodes internes

		#region GetHTML
		/// <summary>
		/// Calcul le résultat du plan media
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTMl</returns>
        public override string GetHTML(WebSession webSession)
        {

            StringBuilder html = new StringBuilder(10000);
            MediaPlanResultData result = null;
            Int64 module = webSession.CurrentModule;
            object[,] tab = null;
            MediaSchedulePeriod period;

            try
            {

                #region Period Detail
                DateTime begin;
                DateTime end;
                if (_zoomDate != null && _zoomDate != string.Empty)
                {
                    if (webSession.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                    {
                        begin = WebFunctions.Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                        end = WebFunctions.Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    }
                    else
                    {
                        begin = WebFunctions.Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                        end = WebFunctions.Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    }
                    begin = WebFunctions.Dates.Max(begin,
                        WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                    end = WebFunctions.Dates.Min(end,
                        WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                    webSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

                }
                else
                {
                    begin = WebFunctions.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType);
                    end = WebFunctions.Dates.getPeriodEndDate(_customerWebSession.PeriodEndDate, _customerWebSession.PeriodType);
                    if (webSession.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        webSession.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                    }
                    period = new MediaSchedulePeriod(begin, end, webSession.DetailPeriod);

                }
                #endregion

                #region Data
                tab = TNS.AdExpress.Web.Rules.Results.GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(webSession, period, (Int64) DBClassificationConstantes.Vehicles.names.adnettrack);
                #endregion

                #region HTML Code
                result = TNS.AdExpress.Web.UI.Results.GenericMediaScheduleUI.GetAdNetTrackHtml(webSession, period, tab, _zoomDate);


                html.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
                html.Append("\r\n\t<tr height=\"1\">\r\n\t\t<td>");
                html.Append("\r\n\t\t</td>\r\n\t</tr>");
                html.Append("\r\n\t<tr>\r\n\t\t<td>");
                html.Append(result.HTMLCode);
                html.Append("\r\n\t\t</td>\r\n\t</tr>");
                html.Append("</table>");
                #endregion


            }
            catch (System.Exception err)
            {
                return (OnAjaxMethodError(err, webSession));
            }
            finally
            {
                webSession.CurrentModule = module;
            }
            return (html.ToString());
        }
		#endregion

		#region GetExcel
		/// <summary>
		/// Calcul le résultat du plan media
		/// </summary>
		/// <param name="webSession">Session du client</param>
        /// <param name="period">Study Period</param>
		/// <returns>Code HTMl pour MS Excel</returns>
		public  string GetExcel(WebSession webSession, MediaSchedulePeriod period){
			StringBuilder html=new StringBuilder(10000);
			MediaPlanResultData result=null;
			Int64 module = webSession.CurrentModule;
			object[,] tab = null;
			try{

                #region Data
                tab = TNS.AdExpress.Web.Rules.Results.GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(webSession, period, (Int64)DBClassificationConstantes.Vehicles.names.adnettrack);
                #endregion


				if(tab!=null && tab.GetLength(0)>0){

					#region Obtention du résultat du calendrier d'action				
					result = TNS.AdExpress.Web.UI.Results.GenericMediaScheduleUI.GetAdNetTrackExcel(webSession, period, tab, _zoomDate);
					#endregion
				
					#region Construction du tableaux global				
					html.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
					#endregion
											
					html.Append("\r\n\t<tr height=\"1\">\r\n\t\t<td>");
					html.Append("\r\n\t\t</td>\r\n\t</tr>");
					html.Append("\r\n\t<tr>\r\n\t\t<td>");
					html.Append(result.HTMLCode);
					html.Append("\r\n\t\t</td>\r\n\t</tr>");
					html.Append("</table>");
				
				}else{
					html.Append("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");		
				}

			}
			catch(System.Exception err){
				throw new ControlsExceptions.AlertAdNetTrackMediaScheduleWebControlException("Impossible de générer l'Excel du plan média AdNetTrack",err);
			}
			finally{
				webSession.CurrentModule = module;
			}
			return(Convertion.ToHtmlString(html.ToString()));
		}
		#endregion

		#region Chargement des paramètres AjaxPro.JavaScriptObject et WebSession
		/// <summary>
		/// Charge les paramètres navigant entre le client et le serveur
		/// </summary>
		/// <param name="o">Tableau de paramètres javascript</param>
		protected void LoadParams(AjaxPro.JavaScriptObject o){
			if(o!=null){
				
				if(o.Contains("ZoomDate")){
					_zoomDate = o["ZoomDate"].Value.Replace("\"",""); 					
				}

				if(o.Contains("UrlParameters")){
					_urlParameters = o["UrlParameters"].Value.Replace("\"",""); 					
				}

			}
		}
		#endregion

		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){

            #region Period Detail
            MediaSchedulePeriod period;
            DateTime begin;
            DateTime end;
            if (_zoomDate != null && _zoomDate != string.Empty)
            {
                if (_customerWebSession.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                {
                    begin = WebFunctions.Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    end = WebFunctions.Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                }
                else
                {
                    begin = WebFunctions.Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    end = WebFunctions.Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                }
                begin = WebFunctions.Dates.Max(begin,
                    WebFunctions.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType));
                end = WebFunctions.Dates.Min(end,
                    WebFunctions.Dates.getPeriodEndDate(_customerWebSession.PeriodEndDate, _customerWebSession.PeriodType));

                _customerWebSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

            }
            else
            {
                begin = WebFunctions.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType);
                end = WebFunctions.Dates.getPeriodEndDate(_customerWebSession.PeriodEndDate, _customerWebSession.PeriodType);
                if (_customerWebSession.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    _customerWebSession.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                }
                period = new MediaSchedulePeriod(begin, end, _customerWebSession.DetailPeriod);

            }
            #endregion


			switch(_renderType){
				case RenderType.html:					
					output.WriteLine(GetHTML(_customerWebSession));						
					break;				
				case RenderType.excel:	
					Int64 module = _customerWebSession.CurrentModule;							
				
					try{
//						_customerWebSession.CurrentModule = WebConstantes.Module.Name.ALERTE_PLAN_MEDIA;
                        output.WriteLine(ExcelFunction.GetLogo(_customerWebSession));
                        output.WriteLine(ExcelFunction.GetExcelHeaderForAdnettrackMediaPlanPopUp(_customerWebSession, false, period.Begin.ToString("yyyyMMdd"), period.End.ToString("yyyyMMdd")));
						output.WriteLine(GetExcel(_customerWebSession, period));
						output.WriteLine(ExcelFunction.GetFooter(_customerWebSession));
					}
					catch(System.Exception err){
						throw new ControlsExceptions.AlertAdNetTrackMediaScheduleWebControlException("Impossible de générer l'Excel du plan média AdNetTrack",err);
					}
					finally{
						_customerWebSession.CurrentModule = module;
					}
					break;
			}
		}
		#endregion
	}
}
