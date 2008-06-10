#region Information
// Auteur : A.Obermeyer
// Cr�� le : 01/03/2005
// Modifi� par: 
//	02/05/2005	K.Shehzad				Added the functionality of creating the Excel header for all the modules
//	12/08/2005	G. Facon				Nom de variables
//	09/02/2006	B. Masson & G.Facon		Refonte totale du header excel
//	G. Facon 01/08/2006 Gestion de l'acc�s au information de la page de r�sultat
#endregion

#region Namespaces
using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Web.UI.HtmlControls;

using Oracle.DataAccess.Client;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Selection;
using WebText = TNS.AdExpress.Web.Functions.Text;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using CustomerConstantes =TNS.AdExpress.Constantes.Customer;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using ClassificationConstant=TNS.AdExpress.Constantes.Classification.DB;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using DBCst = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.Date;
using TNS.FrameWork;
using TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.AdExpress.Domain.Web.Navigation;
using AdExClassification = TNS.AdExpress.DataAccess.Classification;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using ProductClassification=TNS.AdExpress.DataAccess.Classification.ProductBranch;
using TNS.AdExpress.Domain.Web;

#endregion

namespace TNS.AdExpress.Web.UI{
	/// <summary>
	/// Page utilis�e dans les exports excels
	/// It has methods which can be called to make an excel header in any excel file by sending the proper parameters
	/// to the proper method.
	/// </summary>
	public class ExcelWebPage: ResultWebPage{

		#region variables
		/// <summary>
		/// Liste des cl�s Css
		/// </summary>
		private ArrayList _cssKeys = new ArrayList();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ExcelWebPage():base(){
			
			base.Load +=new EventHandler(ExcelWebPage_Load);

			_formatFile=WebConstantes.ErrorManager.formatFile.excel;
			if(_webSession!=null)
				_webSession.OnUseFileExport();
		}
		#endregion

		#region GetLogo
		/// <summary>
		/// Genere le logo TNS pour les export excel
		/// </summary>
		/// <returns></returns>
        public static string GetLogo(WebSession webSession) {
			StringBuilder t = new System.Text.StringBuilder();
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			#region Logo TNS
			t.Append("<table cellpadding=0 cellspacing=0 width=100% >");
            t.Append("<tr><td><img src=\"/App_Themes/" + themeName + WebConstantes.Images.LOGO_TNS + "\"></td></tr>");
			t.Append(GetBlankLine());
			t.Append("</table><br>");
			#endregion

			return Convertion.ToHtmlString(t.ToString());
		}

		/// <summary>
		/// Genere le logo TNS pour les export excel
		/// </summary>
		/// <returns></returns>
        public static string GetAppmLogo(WebSession webSession) {
			StringBuilder t = new System.Text.StringBuilder();
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			#region Logo TNS
			t.Append("<table cellpadding=0 cellspacing=0 width=100% >");
            t.Append("<tr><td><img src=\"/App_Themes/" + themeName + WebConstantes.Images.LOGO_TNS + "\"></td>");
			t.Append("<td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>");
            t.Append("<td><img src=\"/App_Themes/" + themeName + WebConstantes.Images.LOGO_APPM + "\"></td></tr>");
			t.Append ("<tr><td colspan=8>&nbsp;</td></tr>");
			t.Append("</table><br>");
			#endregion

			return Convertion.ToHtmlString(t.ToString());
		}
		#endregion

		#region GetFooter
		/// <summary>
		/// G�n�re le pied de page html pour les exports Excel
		/// </summary>
		/// <returns>HTML</returns>
		public static string GetFooter(WebSession webSession) {
			StringBuilder t = new System.Text.StringBuilder();

			#region CopyRight TNS
			t.Append("<table cellpadding=0 cellspacing=0 width=100% >");
			t.Append(GetBlankLine());
			t.Append("<tr><td class=\"excelData\"> " + GestionWeb.GetWebWord(2266, webSession.SiteLanguage) +"&nbsp;"+ DateTime.Now.Year.ToString() + "</td></tr>");
			t.Append(GetBlankLine());
			t.Append("</table><br>");
			#endregion

			return Convertion.ToHtmlString(t.ToString());
		}
		#endregion
		
		#region GetExcelHeader (Nouvelle version avec XML)
		/// <summary>
		/// G�n�re l'en t�te html pour les exports Excel
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="levelDetail">Bool�en pour l'affichage du niveau de d�tail</param>
		/// <param name="units">Bool�en Unit�</param>
		/// <param name="resultType">Bool�en Type de r�sultat</param>
		/// <param name="insert">Bool�en Encart</param>
		/// <param name="media">Bool�en Support</param>
		/// <param name="dateFormatText">Bool�en pour afficher la date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <param name="title">Titre</param>
        /// <param name="zoomDate">Date de zoom</param>
        /// <param name="periodDisplayLevel">MediaSchedulePeriod</param>
		/// <returns>HTML</returns>
        private static string GetExcelHeader(WebSession webSession, bool levelDetail, bool units, bool resultType, bool insert, bool media, bool dateFormatText, string periodBeginning, string periodEnd, string title, string zoomDate, int periodDisplayLevel) {
			// PARAMETRES A GARDER :
			// bool dateFormatText, string periodBeginning, string periodEnd
			
			#region Variables
			StringBuilder t = new System.Text.StringBuilder();
			ArrayList detailSelections = null;
			#endregion

			try{

				#region D�but du tableau
                t.Append("<table class=\"BorderLevel\" cellpadding=0 cellspacing=0>");
				t.Append("<tr><td class=\"excelDataItalic\">"+GestionWeb.GetWebWord(512,webSession.SiteLanguage)+"</td></tr>");
				#endregion

				//webSession.CustomerLogin.ModuleList();
				Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
				try{
					detailSelections=((ResultPageInformation) currentModule.GetResultPageInformation((int)webSession.CurrentTab)).DetailSelectionItemsType;
				}
				catch(System.Exception){
					if(currentModule.Id==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
						detailSelections=((ResultPageInformation) currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
				}
				
				foreach(int currentType in detailSelections){
					switch((WebConstantes.DetailSelection.Type)currentType){
						case WebConstantes.DetailSelection.Type.moduleName:
							t.Append(GetModuleName(webSession));
							break;
						case WebConstantes.DetailSelection.Type.resultName:
							t.Append(GetResultName(webSession,currentModule));
							break;
						case WebConstantes.DetailSelection.Type.dateSelected:
                            if (zoomDate == null) zoomDate = "";
                            if (zoomDate.Length > 0)
                                t.Append(GetZoomDate(webSession, zoomDate, periodDisplayLevel));
                            else
							    t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));
							break;
                        case WebConstantes.DetailSelection.Type.studyDate:
                            t.Append(GetStudyDate(webSession));
                            break;
						case WebConstantes.DetailSelection.Type.unitSelected:
							t.Append(GetUnitSelected(webSession));
							break;
						case WebConstantes.DetailSelection.Type.vehicleSelected:
							t.Append(GetVehicleSelected(webSession));
							break;
						case WebConstantes.DetailSelection.Type.productSelected:
							t.Append(GetProductSelected(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.mediaLevelDetail:
							t.Append(GetMediaLevelDetail(webSession));
							break;
						case WebConstantes.DetailSelection.Type.productLevelDetail:
							t.Append(GetProductLevelDetail(webSession));
							break;
						case WebConstantes.DetailSelection.Type.competitorMediaSelected:
							t.Append(GetCompetitorMediaSelected(webSession));
							break;
						case WebConstantes.DetailSelection.Type.insetSelected:
							t.Append(GetInsetSelected(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.newInMedia:
							t.Append(GetNewInMedia(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.comparativeStudy:
							t.Append(GetComparativeStudy(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.formatSelected:
							t.Append(GetFormatSelected(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.daySelected:
							t.Append(GetDaySelected(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.timeSlotSelected:
							t.Append(GetTimeSlotSelected(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.targetSelected:
							t.Append(GetTargetSelected(webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.genericMediaLevelDetail:
							t.Append(GetGenericMediaLevelDetail(webSession));
							break;
						case WebConstantes.DetailSelection.Type.sloganSelected:
							t.Append(GetSloganSelected(webSession));
							break;
						case WebConstantes.DetailSelection.Type.genericProductLevelDetail:
							t.Append(GetGenericProductLevelDetail(webSession));
							break;
						default:
							break;
					}
				}
				t.Append(GetBlankLine());
				t.Append("</table><br>");

				// On lib�re htmodule pour pouvoir le sauvegarder dans les tendances
				//webSession.CustomerLogin.HtModulesList.Clear();
				
				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel",err)); 
			}
		}
		#endregion

		#region Nouvelles M�thodes priv�es

		#region Nom du module
		/// <summary>
		/// Nom du module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetModuleName(WebSession webSession){
			StringBuilder html = new StringBuilder();
			html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1859,webSession.SiteLanguage)+" :</font> ");
			html.Append(GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSession.CurrentModule),webSession.SiteLanguage));
			html.Append("</td></tr>");
			return(html.ToString());
		}
		#endregion

		#region Nom du r�sultat
		/// <summary>
		/// Nom du r�sultat
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="currentModule">Module</param>
		/// <returns>HTML</returns>
		private static string GetResultName(WebSession webSession, Module currentModule){
			string currentResult = "";
			StringBuilder html = new StringBuilder();
			try{
				currentResult = GestionWeb.GetWebWord((int)((ResultPageInformation)currentModule.GetResultPageInformation((int)webSession.CurrentTab)).IdWebText,webSession.SiteLanguage);
			}
			catch(System.Exception){
				if(currentModule.Id==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
					currentResult =GestionWeb.GetWebWord((int)((ResultPageInformation)currentModule.GetResultPageInformation(5)).IdWebText,webSession.SiteLanguage);
			}
			if(currentResult.Length > 0){
				html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(793,webSession.SiteLanguage) +" :</font> "+ currentResult +"</td></tr>");
			}
			return(html.ToString());
		}
		#endregion

		#region Dates s�lectionn�es
		/// <summary>
		/// Dates s�lectionn�es
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="currentModule">Module en cours</param>
		/// <param name="dateFormatText">Bool�en date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <returns>HTML</returns>
		/// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
		private static string GetDateSelected(WebSession webSession, Module currentModule, bool dateFormatText, string periodBeginning, string periodEnd){
			StringBuilder html = new StringBuilder();
			string startDate="";
			string endDate="";

			if( (currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE || currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO || currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION || currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO) && webSession.DetailPeriodBeginningDate.Length>0 && webSession.DetailPeriodBeginningDate!="0" && webSession.DetailPeriodEndDate.Length>0 && webSession.DetailPeriodEndDate!="0"){
				// Affichage de la p�riode mensuelle si elle est s�lectionn� dans les options de r�sultat
				startDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.DetailPeriodBeginningDate);
				endDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.DetailPeriodEndDate);				
				
				html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+ startDate);
				if(!startDate.Equals(endDate))
					html.Append(" - "+ endDate);
				html.Append("</td></tr>");
			}
			else{
				if(dateFormatText){
					startDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.PeriodBeginningDate);
					endDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.PeriodEndDate);			
				
					html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+startDate);
					if(!startDate.Equals(endDate))
						html.Append(" - "+endDate);
					html.Append("</td></tr>");
				}
				else{
					if(periodBeginning.Length==0||periodEnd.Length==0){
						startDate=WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage);
						endDate=WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage);
					
						html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+startDate);
						if(!startDate.Equals(endDate))
							html.Append(" - "+endDate);
						html.Append("</td></tr>");
					}
					else{

                        startDate = WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(periodBeginning, webSession.PeriodType), webSession.SiteLanguage);
                        endDate = WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(periodEnd, webSession.PeriodType), webSession.SiteLanguage);
					
						html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+startDate);
						if(!startDate.Equals(endDate))
							html.Append(" - "+endDate);
						html.Append("</td></tr>");
					}
				}
			}
			return(html.ToString());
		}
		#endregion

        #region Zoom P�riode
        /// <summary>
        /// Generate html code for zoom period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <param name="zoomDate">Date de zoom</param>
        /// <param name="periodDisplayLevel">Niveau de d�tail de l'affichage des p�riodes</param>
        /// <returns>Html code</returns>
        private static string GetZoomDate(WebSession webSession, string zoomDate, int periodDisplayLevel) {
            
            StringBuilder html = new StringBuilder();
            DateTime firstDayOfMonth;
            DateTime lastDayOfMonth;
            DateTime begin;
            DateTime end;

            if (periodDisplayLevel != -1) {
                switch ((WebConstantes.CustomerSessions.Period.DisplayLevel)periodDisplayLevel) {
                    case CstWeb.CustomerSessions.Period.DisplayLevel.weekly:

                        AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)));
                        begin = tmp.FirstDay.Date;
                        end = tmp.LastDay.Date;
                        begin = TNS.AdExpress.Web.Functions.Dates.Max(begin,
                                    TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                        end = TNS.AdExpress.Web.Functions.Dates.Min(end,
                            TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                        html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(1541, webSession.SiteLanguage) + " </font> " + begin.Date.ToString("dd/MM/yyyy"));
                        if (!begin.Equals(end))
                            html.Append(" - " + end.Date.ToString("dd/MM/yyyy"));
                        html.Append("</td></tr>");

                        return html.ToString();
                    case CstWeb.CustomerSessions.Period.DisplayLevel.monthly:

                        firstDayOfMonth = new DateTime(int.Parse(zoomDate.Substring(0, 4)), int.Parse(zoomDate.Substring(4, 2)), 1);
                        firstDayOfMonth = firstDayOfMonth.AddMonths(1);
                        lastDayOfMonth = firstDayOfMonth.AddDays(-1);

                        begin = new DateTime(lastDayOfMonth.Year, lastDayOfMonth.Month, 1);
                        end = lastDayOfMonth;
                        begin = TNS.AdExpress.Web.Functions.Dates.Max(begin,
                                    TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType));
                        end = TNS.AdExpress.Web.Functions.Dates.Min(end,
                            TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType));

                        html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(1541, webSession.SiteLanguage) + " </font> " + begin.Date.ToString("dd/MM/yyyy"));
                        if (!begin.Equals(end))
                            html.Append(" - " + end.Date.ToString("dd/MM/yyyy"));
                        html.Append("</td></tr>");

                        return html.ToString();
                    default:
                        return "";
                }
            }

            return (html.ToString());

        }
        #endregion

        #region P�riode de l'�tude
        /// <summary>
        /// Generate html code for study period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private static string GetStudyDate(WebSession webSession) {

            StringBuilder html = new StringBuilder();
            string startDate;
            string endDate;

            try {
                if (webSession.CustomerPeriodSelected != null) {
                    if (webSession.CustomerPeriodSelected.StartDate.Length > 0 && webSession.CustomerPeriodSelected.EndDate.Length > 0) {

                        startDate = DateString.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.StartDate.ToString(), webSession.SiteLanguage);
                        endDate = DateString.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.EndDate.ToString(), webSession.SiteLanguage);

                        html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(2291, webSession.SiteLanguage) + " </font> " + startDate);
                        if (!startDate.Equals(endDate))
                            html.Append(" - " + endDate);
                        html.Append("</td></tr>");

                    }
                }
            }
            catch (System.Exception) {
                html.Append("");            
            }

            return (html.ToString());

        }
        #endregion

		#region Unit�
		/// <summary>
		/// Unit�
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetUnitSelected(WebSession webSession){
			if(webSession.PreformatedTable.ToString().ToUpper().IndexOf("UNITS")==-1)
				return("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1313,webSession.SiteLanguage)+"</font> "+GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.XLSUnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td></tr>");
			return("");
		}
		#endregion

		#region Media s�lectionn�
		/// <summary>
		/// Media s�lectionn�
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetVehicleSelected(WebSession webSession){
			StringBuilder html = new StringBuilder();
			if (webSession.isMediaSelected()){
				html.Append(GetBlankLine());
				html.Append("<tr><td class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(190,webSession.SiteLanguage)+" :</font></td></tr>");
				html.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(webSession.SelectionUniversMedia, webSession.SiteLanguage));
			}
			return(html.ToString());
		}
		#endregion

		#region Produit s�lectionn�
		/// <summary>
		/// Produits s�lectionn�s
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		public static string GetProductSelected(WebSession webSession){
			StringBuilder t = new StringBuilder();
			//int idAdvertiser = 1;
			int universeCodeTitle = 1759;
			string productSelection = "";

			AdExpressUniverse adExpressUniverse = null;
			#region selection produit principale
			if (webSession.PrincipalProductUniverses.Count ==1) {
				//t.Append(GetBlankLine());
				productSelection += GetBlankLine();
				//t.Append("<TR><TD colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>");
				productSelection += "<TR><TD colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>";
				adExpressUniverse = webSession.PrincipalProductUniverses[0];
                //if (webSession.CustomerLogin.Connection == null) {
                //    TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(webSession.CustomerLogin.OracleConnectionString));
                //    webSession.CustomerLogin.Connection = (OracleConnection)dataSource.GetSource();
                //}
				//t.Append(TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.CustomerLogin.Connection));//TNS.AdExpress.Web.Functions.DisplayUniverse
				productSelection += TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.Source);
			}
			else if (webSession.PrincipalProductUniverses.Count > 1) {				
                //if (webSession.CustomerLogin.Connection == null) {
                //    TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(webSession.CustomerLogin.OracleConnectionString));
                //    webSession.CustomerLogin.Connection = (OracleConnection)dataSource.GetSource();
                //}
				for (int k = 0; k < webSession.PrincipalProductUniverses.Count; k++) {
					if (webSession.PrincipalProductUniverses.ContainsKey(k)) {
						t.Append(GetBlankLine());
						if (k > 0) {							
							 universeCodeTitle = 2301;
						}
						else {
							universeCodeTitle = 2302;
						}
						//t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>");
						//t.Append("<TR><TD colspan=4 class=\"txtViolet11Bold\" bgColor=\"#ffffff\" >" + webSession.PrincipalProductUniverses[k].Label + " </TD></TR>");
						productSelection += "<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>";
                        productSelection += "<TR><TD colspan=4 class=\"txtViolet11Bold whiteBackGround\" >" + webSession.PrincipalProductUniverses[k].Label + " </TD></TR>";
						adExpressUniverse = webSession.PrincipalProductUniverses[k];
						//t.Append(TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.CustomerLogin.Connection));
						productSelection += TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.Source);
					}
				}
			}
			#endregion	

			#region selection produit secondaire
			if (webSession.SecondaryProductUniverses.Count == 1) {
				if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
								|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
								) {
					if (webSession.SecondaryProductUniverses.ContainsKey(0)) {
						adExpressUniverse = webSession.SecondaryProductUniverses[0];
						universeCodeTitle = 1195;
					}
					else if (webSession.SecondaryProductUniverses.ContainsKey(1)) {
						adExpressUniverse = webSession.SecondaryProductUniverses[1];
						universeCodeTitle = 1196;
					}
				}
				else adExpressUniverse = webSession.SecondaryProductUniverses[0];

				//t.Append(GetBlankLine());
				//t.Append("<TR><TD colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>");		
				
				if(WebFunctions.Modules.IsDashBoardModule(webSession))				
					productSelection = GetBlankLine();
				else if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0
						&& webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE && webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.APPM.synthesis) {
					universeCodeTitle = 2302;
					productSelection = GetBlankLine();
				}
				else {
					productSelection += GetBlankLine();
				}
				productSelection += "<TR><TD colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>";				
                //if (webSession.CustomerLogin.Connection == null) {
                //    TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(webSession.CustomerLogin.OracleConnectionString));
                //    webSession.CustomerLogin.Connection = (OracleConnection)dataSource.GetSource();
                //}
				//t.Append(TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.CustomerLogin.Connection));
				productSelection += TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.Source);
			}
			else if (webSession.SecondaryProductUniverses.Count > 1) {
                //if (webSession.CustomerLogin.Connection == null) {
                //    TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(webSession.CustomerLogin.OracleConnectionString));
                //    webSession.CustomerLogin.Connection = (OracleConnection)dataSource.GetSource();
                //}
				for (int k = 0; k < webSession.SecondaryProductUniverses.Count; k++) {
					if (webSession.SecondaryProductUniverses.ContainsKey(k)) {
						t.Append(GetBlankLine());
						if (k > 0) {
							if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
							|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
							)
								universeCodeTitle = 1196;
							else universeCodeTitle = 2301;
						}
						else {
							if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
							|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
							)
								universeCodeTitle = 1195;
							else universeCodeTitle = 2302;
						}
						//t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>");
						//t.Append("<TR><TD colspan=4 class=\"txtViolet11Bold\" bgColor=\"#ffffff\" >" + webSession.SecondaryProductUniverses[k].Label + " </TD></TR>");
						//adExpressUniverse = webSession.SecondaryProductUniverses[k];
						//t.Append(TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.CustomerLogin.Connection));
						productSelection += "<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + " :</font></TD></TR>";
                        productSelection += "<TR><TD colspan=4 class=\"txtViolet11Bold whiteBackGround\" >" + webSession.SecondaryProductUniverses[k].Label + " </TD></TR>";
						adExpressUniverse = webSession.SecondaryProductUniverses[k];
						productSelection += TNS.AdExpress.Web.Functions.DisplayUniverse.ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.Source);
					}
				}
			}

			t.Append(productSelection);
			#endregion	

			#region Liste des supports de r�f�rence
			//// Alerte et Analyse portefeuille

			if (webSession.isReferenceMediaSelected()) {
				t.Append(GetBlankLine());
				t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(971, webSession.SiteLanguage) + " :</font></TD></TR>");
				t.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel((TreeNode)webSession.ReferenceUniversMedia, webSession.SiteLanguage));
			}
			#endregion

			return(t.ToString());
		}
		#endregion

		#region Niveau de d�tail support
		/// <summary>
		/// Niveau de d�tail support (M�dia d�taill� par)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetMediaLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1150,webSession.SiteLanguage)+"</font> "+WebFunctions.MediaDetailLevel.LevelMediaToExcelString(webSession)+"</td></tr>";
		}
		#endregion

		#region Niveau de d�tail support g�n�rique
		/// <summary>
		/// Niveau de d�tail support (M�dias d�taill�s par :)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetGenericMediaLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1150,webSession.SiteLanguage)+"</font> "+ webSession.GenericMediaDetailLevel.GetLabel(webSession.SiteLanguage) +"</td></tr>";
		}
		#endregion

		#region Niveau de d�tail produit g�n�rique
		/// <summary>
		/// Niveau de d�tail produit (Produits d�taill�s par :)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetGenericProductLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1124,webSession.SiteLanguage)+"</font> "+ webSession.GenericProductDetailLevel.GetLabel(webSession.SiteLanguage) +"</td></tr>";
		}
		#endregion

		#region Niveau de d�tail produit
		/// <summary>
		/// Niveau de d�tail produit (Produit d�taill� par)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetProductLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1124,webSession.SiteLanguage)+"</font> "+WebFunctions.ProductDetailLevel.LevelProductToExcelString(webSession)+"</td></tr>";
		}
		#endregion

		#region S�lection encart
		/// <summary>
		/// S�lection encart
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetInsetSelected(WebSession webSession){
			string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			ClassificationCst.DB.Vehicles.names vehicleType = (ClassificationCst.DB.Vehicles.names)int.Parse(Vehicle);
			if(vehicleType==ClassificationCst.DB.Vehicles.names.press ||vehicleType==ClassificationCst.DB.Vehicles.names.internationalPress){
				int code=0;
				switch(webSession.Insert){
					case  WebConstantes.CustomerSessions.Insert.total :
						code=1401;
						break;
					case  WebConstantes.CustomerSessions.Insert.withOutInsert :
						code=1403;
						break;
					case  WebConstantes.CustomerSessions.Insert.insert :
						code=1402;
						break;
					default:
						code=1400;
						break;
				}
				return("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1400,webSession.SiteLanguage)+"</font> "+GestionWeb.GetWebWord(code,webSession.SiteLanguage)+"</td></tr>");
			}
			return("");
		}
		#endregion

		#region Nouveau support dans :
		/// <summary>
		/// Nouveau support dans :
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetNewInMedia(WebSession webSession){
			int code=0;
			switch(webSession.NewProduct){
				case WebConstantes.CustomerSessions.NewProduct.pige:
					code=1421;
					break;
				case WebConstantes.CustomerSessions.NewProduct.support:
					code=1422;
					break;
			}
			return("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1449,webSession.SiteLanguage)+" "+GestionWeb.GetWebWord(code,webSession.SiteLanguage)+"</font></td></tr>");
		}
		#endregion

		#region Support s�lectionn�s pour les concurrents
		// Alerte et Analyse concurrentielle
		// Alerte et Analyse de potentielle
		// Analyse dynamique

		/// <summary>
		/// Support s�lectionn�s pour les concurrents
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetCompetitorMediaSelected(WebSession webSession){
			StringBuilder t = new StringBuilder();
			if(webSession.isCompetitorMediaSelected()){
				int idMedia=1;
				t.Append(GetBlankLine());
				t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1087,webSession.SiteLanguage) +"</font></td></tr>");
				while((TreeNode)webSession.CompetitorUniversMedia[idMedia]!=null){
					TreeNode tree=(TreeNode)webSession.CompetitorUniversMedia[idMedia];				
					t.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(((TreeNode)webSession.CompetitorUniversMedia[idMedia]),webSession.SiteLanguage));
					t.Append(GetBlankLine());
					idMedia++;
				}
			}
			return(t.ToString());
		}
		#endregion

		#region Etude comparative
		/// <summary>
		/// Etude comparative
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>HTML</returns>
		private static string GetComparativeStudy(WebSession webSession){
			if(webSession.ComparativeStudy)
				return("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1118,webSession.SiteLanguage)+"</font></td></tr>");
			return("");
		}
		#endregion

		#region Format s�lectionn�
		/// <summary>
		/// Format s�lectionn�
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetFormatSelected(WebSession webSession){
			return("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1420,webSession.SiteLanguage)+" :</font> "+ TNS.AdExpress.Web.Functions.Dates.GetFormat(webSession,webSession.Format) +"</td></tr>");
		}
		#endregion

		#region Jour nomm� s�lectionn�
		/// <summary>
		/// Jour nomm� s�lectionn�
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>HTML</returns>
		private static string GetDaySelected(WebSession webSession){
			string namedDay=string.Empty;
			switch(webSession.NamedDay){
				case CstWeb.Repartition.namedDay.Week_5_day:
					namedDay=GestionWeb.GetWebWord(1553, webSession.SiteLanguage);
					break;
				case CstWeb.Repartition.namedDay.Total:
					namedDay=GestionWeb.GetWebWord(848, webSession.SiteLanguage);
					break;
				case CstWeb.Repartition.namedDay.Week_end:
					namedDay=GestionWeb.GetWebWord(1561, webSession.SiteLanguage);
					break;
				default:
					namedDay=TNS.AdExpress.Web.Functions.Dates.getDay(webSession,webSession.NamedDay.ToString());
					break;
			}
			if(namedDay.Length>0){
				return("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1574,webSession.SiteLanguage)+" :</font> "+ namedDay +"</td></tr>");
			}
			return("");
		}
		#endregion

		#region Tranche horaire s�lectionn�e
		/// <summary>
		/// Tranche horaire s�lectionn�e
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>HTML</returns>
		private static string GetTimeSlotSelected(WebSession webSession){
			return("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1575,webSession.SiteLanguage)+" :</font> "+ TNS.AdExpress.Web.Functions.Dates.GetTimeSlice(webSession,webSession.TimeInterval) +"</td></tr>");
		}
		#endregion

		#region Wave et Cible s�lectionn�es
		/// <summary>
		/// Tranche horaire s�lectionn�e
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>HTML</returns>
		private static string GetTargetSelected(WebSession webSession){
			StringBuilder html = new StringBuilder();
			if (webSession.IsWaveSelected()){
				if(((LevelInformation)webSession.SelectionUniversAEPMWave.FirstNode.Tag).Text.Length>0){
					html.Append(GetBlankLine());
					html.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1762,webSession.SiteLanguage)+"</font> "+((LevelInformation) webSession.SelectionUniversAEPMWave.FirstNode.Tag).Text+"</td></tr>");
				}
			}
			if(!(webSession.CurrentTab==APPM.affinities)){									
				if (webSession.IsTargetSelected()){
					if(((LevelInformation)webSession.SelectionUniversAEPMTarget.LastNode.Tag).Text.Length>0){
						html.Append("<tr height=\"20\"><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1763,webSession.SiteLanguage)+"</font></td></tr>");
						// Affichage du TreeNode
						html.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(webSession.SelectionUniversAEPMTarget,webSession.SiteLanguage));
					}
				}
			}
			return(html.ToString());
		}
		#endregion

		#region Ligne s�paratrice
		/// <summary>
		/// Ligne s�paratrice
		/// </summary>
		/// <returns>HTML</returns>
		private static string GetBlankLine(){
			return("<tr><td>&nbsp;</td></tr>");
		}
		#endregion

		#region Affichage du produit s�lectionn� dans la pop up des insertions selon le niveau choisi (annonceur ou produit)
		/// <summary>
		/// Affichage du produit s�lectionn� dans la pop up des insertions selon le niveau choisi
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idElement">identifiant de l'�l�ment</param>
		/// <param name="level">Niveau de l'�l�ment s�lectionn�</param>
		/// <returns>HTML</returns>
		private static string GetProductSelectedForCreationsPopUp(WebSession webSession, Int64 idElement,int level){
			StringBuilder t = new StringBuilder();
			int webTexId = 0;
			bool elementIsAdvertiser = false;

			switch(webSession.PreformatedProductDetail){
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
					if(level==ResultConstantes.IDL3_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					else if (level==ResultConstantes.IDL3_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
					if(level==ResultConstantes.IDL1_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
					if(level==ResultConstantes.IDL1_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					else if (level==ResultConstantes.IDL2_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
					if(level==ResultConstantes.IDL1_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
					if(level==ResultConstantes.IDL1_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					else if (level==ResultConstantes.IDL3_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
					if(level==ResultConstantes.IDL3_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					else if (level==ResultConstantes.IDL3_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					else if (level==ResultConstantes.IDL3_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
					if(level==ResultConstantes.IDL2_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
					if (level==ResultConstantes.IDL2_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
					if(level==ResultConstantes.IDL3_INDEX){
						webTexId = 1106;	// Annonceur
						elementIsAdvertiser = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
					if (level==ResultConstantes.IDL3_INDEX){
						webTexId = 858;		// Produit
						elementIsAdvertiser = false;
					}
					break;
			}

			if(webTexId > 0){
				t.Append(GetBlankLine());
				t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(webTexId,webSession.SiteLanguage)+" : </font></TD></TR>");
				// R�cup�ration du libell�
				if(elementIsAdvertiser){ // Annonceur s�lectionn�
					AdExClassification.ProductBranch.PartialAdvertiserLevelListDataAccess elementName = new AdExClassification.ProductBranch.PartialAdvertiserLevelListDataAccess(idElement.ToString(),webSession.SiteLanguage,webSession.Source);
					t.Append("<TR><TD colspan=4 class=\"excelData\" >"+ elementName[idElement].ToString() +"</TD></TR>");
				}
				else{ // Produit s�lectionn�
                    AdExClassification.ProductBranch.PartialProductLevelListDataAccess elementName = new AdExClassification.ProductBranch.PartialProductLevelListDataAccess(idElement.ToString(),webSession.SiteLanguage,webSession.Source);
					t.Append("<TR><TD colspan=4 class=\"excelData\" >"+ elementName[idElement].ToString() +"</TD></TR>");
				}
			}
			return(t.ToString());
		}
		#endregion

		#region D�tail de s�lection des insertions dans la pop up des insertions
		/// <summary>
		/// D�tail de s�lection des insertions (pop up plan media, alerte plan media)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="mediaImpactedList">Liste des medias impact�s</param>
		/// <returns>HTML</returns>
		private static string GetMediaSelectedForCreationsPopUp(WebSession webSession,ListDictionary mediaImpactedList){
			StringBuilder t = new StringBuilder();
			string mediaSelectLabel="";
			Int64 mediaSelectId=0;
			AdExClassification.ClassificationLevelListDataAccess elementName = null;
			
			t.Append(GetBlankLine());
			t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1150,webSession.SiteLanguage) +"</font></TD></TR>");
			
			//Obtient le m�dia  s�lectionn�
			IEnumerator myEnumerator = mediaImpactedList.GetEnumerator();							
			foreach (DictionaryEntry de in mediaImpactedList ){
				if(long.Parse(de.Value.ToString())>-1){
					mediaSelectId = long.Parse(de.Value.ToString());
					mediaSelectLabel = de.Key.ToString().Trim();

					//Obtient le libell� du m�dia s�lectionn�
					switch(mediaSelectLabel){
						case DBCst.Fields.ID_VEHICLE :
                            elementName = new AdExClassification.MediaBranch.PartialVehicleListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1292,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case DBCst.Fields.ID_CATEGORY :
                            elementName = new AdExClassification.MediaBranch.PartialCategoryListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1382,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case DBCst.Fields.ID_INTEREST_CENTER :
                            elementName = new AdExClassification.MediaBranch.PartialInterestCenterListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1411,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case DBCst.Fields.ID_MEDIA_SELLER :
                            elementName = new AdExClassification.MediaBranch.PartialMediaSellerListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1383,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case DBCst.Fields.ID_MEDIA :
                            elementName = new AdExClassification.MediaBranch.PartialMediaListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(971,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case  DBCst.Fields.ID_SLOGAN :
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1888,webSession.SiteLanguage) +" : </font> "+ mediaSelectId.ToString() +"</TD></TR>");
							break;
						case  DBCst.Fields.ID_ADVERTISER :
                            elementName = new AdExClassification.ProductBranch.PartialAdvertiserLevelListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(857,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case  DBCst.Fields.ID_BRAND :
                            elementName = new AdExClassification.ProductBranch.PartialBrandLevelListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1889,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case  DBCst.Fields.ID_PRODUCT :
                            elementName = new AdExClassification.ProductBranch.PartialProductLevelListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(858,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case  DBCst.Fields.ID_SECTOR :
                            elementName = new AdExClassification.ProductBranch.PartialSectorLevelListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1103,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case  DBCst.Fields.ID_SUBSECTOR :
                            elementName = new AdExClassification.ProductBranch.PartialSubSectorLevelListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1931,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
						case  DBCst.Fields.ID_GROUP_ :
                            elementName = new AdExClassification.ProductBranch.PartialGroupLevelListDataAccess(mediaSelectId.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(1110,webSession.SiteLanguage) +" : </font> "+ elementName[mediaSelectId].ToString() +"</TD></TR>");
							break;
					}
				}					
			}
			return(t.ToString());
		}
		#endregion

		#region Version s�lectionn�e
		/// <summary>
		/// Version s�lectionn�e
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetSloganSelected(WebSession webSession){
			string tmp="";
			int nbCol=0;
			if(webSession.SloganIdZoom>0){
				tmp+="<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1888,webSession.SiteLanguage)+" :</font> "+webSession.SloganIdZoom.ToString()+"</td></tr>";
				return(tmp);
			}
			else{
				if(webSession.IdSlogans!=null && webSession.IdSlogans.Count > 0){
					tmp+="<tr><td colspan=1 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1888,webSession.SiteLanguage)+" :</font> </td>";
					foreach(Int64 currentSlogan in webSession.IdSlogans){
						if(nbCol>2){
							tmp+="</tr><tr><td>&nbsp;</td>";
							nbCol=0;
						}
						tmp+="<td class=\"excelData\">"+currentSlogan.ToString()+"</td>";
						nbCol++;
					}
					if(nbCol!=3)tmp+="<td colspan="+(3-nbCol).ToString()+">&nbsp;</td>";
					tmp+="</tr>";
					return(tmp);
				}
			}
			return("");
		}
		#endregion

		#endregion

		#region GetExcelHeader pour les pops up plan m�dia
		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel de la pop up plan m�dia
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dateFormatText">Date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <returns>HTML</returns>
		public static string GetExcelHeaderForMediaPlanPopUp(WebSession webSession, bool dateFormatText, string periodBeginning, string periodEnd){
			StringBuilder t=new StringBuilder(2000);
			try{
				//webSession.CustomerLogin.ModuleList();
				Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

				#region D�but du tableau
				t.Append("<table style=\"border:solid 1px #808080;\" cellpadding=0 cellspacing=0>");
				t.Append("<tr><td class=\"excelDataItalic\">"+GestionWeb.GetWebWord(512,webSession.SiteLanguage)+"</td></tr>");
				#endregion

				// R�sultat : Calendrier d'actions
				t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(793,webSession.SiteLanguage) +" :</font> "+ GestionWeb.GetWebWord(1474,webSession.SiteLanguage) +"</td></tr>");
				// Date
				t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));
                // P�riode de l'�tude
                t.Append(GetStudyDate(webSession));
				// Unit�
				t.Append(GetUnitSelected(webSession));
				// Support s�lectionn�
				t.Append(GetMediaSelectedForMediaPlanPopUp(webSession));
				// Niveau de d�tail
				t.Append(GetGenericMediaLevelDetail(webSession));
				// Produit s�lectionn�
				t.Append(GetProductSelectedForMediaPlanPopUp(webSession));

				t.Append(GetBlankLine());
				t.Append("</table><br>");

				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel",err)); 
			}
		}

        /// <summary>
        /// G�n�re l'en t�te html pour le fichier Excel de la pop up plan m�dia
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="dateFormatText">Date en format texte</param>
        /// <param name="periodBeginning">Date de d�but</param>
        /// <param name="periodEnd">Date de fin</param>
        /// <param name="zoomDate">Date de zoom</param>
        /// <param name="periodDisplayLevel">Niveau de d�tail de l'affichage des p�riodes</param>
        /// <returns>HTML</returns>
        public static string GetExcelHeaderForMediaPlanPopUp(WebSession webSession, bool dateFormatText, string periodBeginning, string periodEnd, string zoomDate, int periodDisplayLevel) {
            StringBuilder t = new StringBuilder(2000);
            try {
                //webSession.CustomerLogin.ModuleList();
                Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

                #region D�but du tableau
                t.Append("<table class=\"greyBorder\" cellpadding=0 cellspacing=0>");
                t.Append("<tr><td class=\"excelDataItalic\">" + GestionWeb.GetWebWord(512, webSession.SiteLanguage) + "</td></tr>");
                #endregion

                // R�sultat : Calendrier d'actions
                t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(793, webSession.SiteLanguage) + " :</font> " + GestionWeb.GetWebWord(1474, webSession.SiteLanguage) + "</td></tr>");
                
                // P�riode de l'�tude
                if (zoomDate == null) zoomDate = "";
                if (zoomDate.Length > 0)
                    t.Append(GetZoomDate(webSession, zoomDate, periodDisplayLevel));
                else
                    t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));
                //else
                //    t.Append(GetStudyDate(webSession));
                
                // Unit�
                t.Append(GetUnitSelected(webSession));
                // Support s�lectionn�
                t.Append(GetMediaSelectedForMediaPlanPopUp(webSession));
                // Niveau de d�tail
                t.Append(GetGenericMediaLevelDetail(webSession));
                // Produit s�lectionn�
                t.Append(GetProductSelectedForMediaPlanPopUp(webSession));

                t.Append(GetBlankLine());
                t.Append("</table><br>");

                return Convertion.ToHtmlString(t.ToString());
            }
            catch (System.Exception err) {
                throw (new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel", err));
            }
        }

		/// <summary>
		/// Affichage des supports s�lectionn�s dans la pop up plan m�dia
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetMediaSelectedForMediaPlanPopUp(WebSession webSession){
			return("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(190,webSession.SiteLanguage)+" :</font> "+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).Text+"</TD></TR>");
		}

		/// <summary>
		/// Affichage des produits s�lectionn�s dans la pop up plan m�dia
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetProductSelectedForMediaPlanPopUp(WebSession webSession){
			StringBuilder t = new StringBuilder();
			int webTexId=0;
			switch(webSession.ProductDetailLevel.LevelProduct){
				case TNS.AdExpress.Constantes.Classification.Level.type.sector:
					webTexId=965;
					break;
				case TNS.AdExpress.Constantes.Classification.Level.type.subsector:
					webTexId=966;
					break;
				case TNS.AdExpress.Constantes.Classification.Level.type.group:
					webTexId=967;
					break;
				case TNS.AdExpress.Constantes.Classification.Level.type.segment:
					webTexId=1894;
					break;
				case TNS.AdExpress.Constantes.Classification.Level.type.product:
					webTexId=1895;
					break;
				case TNS.AdExpress.Constantes.Classification.Level.type.advertiser:
					webTexId=813;
					break;
				case TNS.AdExpress.Constantes.Classification.Level.type.brand:
					webTexId=1585;
					break;
				case TNS.AdExpress.Constantes.Classification.Level.type.holding_company:
					webTexId=814;
					break;
			}
			t.Append(GetBlankLine());
			t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(webTexId,webSession.SiteLanguage)+"</font></TD></TR>");
			t.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel((TreeNode)webSession.ProductDetailLevel.ListElement,webSession.SiteLanguage));

			return(t.ToString());
		}
		#endregion

		#region GetExcelHeader pour les pops up plan m�dia AdNetTrack
		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel de la pop up plan m�dia AdNetTrack
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dateFormatText">Date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <returns>HTML</returns>
		public static string GetExcelHeaderForAdnettrackMediaPlanPopUp(WebSession webSession, bool dateFormatText, string periodBeginning, string periodEnd){
			StringBuilder t=new StringBuilder(2000);
			try{
				//webSession.CustomerLogin.ModuleList();
                Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

				#region D�but du tableau
                t.Append("<table class=\"greyBorder\" cellpadding=0 cellspacing=0>");
				t.Append("<tr><td class=\"excelDataItalic\">"+GestionWeb.GetWebWord(512,webSession.SiteLanguage)+"</td></tr>");
				#endregion

				// R�sultat : Plan M�dia
				t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(793,webSession.SiteLanguage) +" :</font> "+ GestionWeb.GetWebWord(751,webSession.SiteLanguage) +"</td></tr>");
				
				// Date
				t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));
				
				// Media s�lectionn� (Adnettrack)
				t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(190,webSession.SiteLanguage)+" :</font> "+ GestionWeb.GetWebWord(648,webSession.SiteLanguage).ToUpper() +"</TD></TR>");
				
				// Niveau de d�tail
				t.Append(GetAdNetTrackMediaLevelDetail(webSession));

				// El�ment s�lectionn� (Annonceur ou  produit ou visuel)
				if(webSession.AdNetTrackSelection.Id.ToString().Length>0){
					switch(webSession.AdNetTrackSelection.SelectionType){
						case AdNetTrackMediaSchedule.Type.advertiser:
							AdExClassification.ProductBranch.PartialAdvertiserLevelListDataAccess advertiser = new AdExClassification.ProductBranch.PartialAdvertiserLevelListDataAccess(webSession.AdNetTrackSelection.Id.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+" :</font> "+ advertiser[webSession.AdNetTrackSelection.Id].ToString() +"</TD></TR>");
							break;
						case AdNetTrackMediaSchedule.Type.product:
							AdExClassification.ProductBranch.PartialProductLevelListDataAccess product = new AdExClassification.ProductBranch.PartialProductLevelListDataAccess(webSession.AdNetTrackSelection.Id.ToString(),webSession.SiteLanguage,webSession.Source);
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+" :</font> "+ product[webSession.AdNetTrackSelection.Id].ToString() +"</TD></TR>");
							break;
						case AdNetTrackMediaSchedule.Type.visual:
							t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1909,webSession.SiteLanguage)+" :</font> "+ webSession.AdNetTrackSelection.Id.ToString() +"</TD></TR>");
							break;
					}
				}

				t.Append(GetBlankLine());
				t.Append("</table><br>");

				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel",err)); 
			}
		}

		/// <summary>
		/// Niveau de d�tail support pour AdNetTrack
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private static string GetAdNetTrackMediaLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1150,webSession.SiteLanguage)+"</font> "+ webSession.GenericAdNetTrackDetailLevel.GetLabel(webSession.SiteLanguage) +"</td></tr>";
		}
		#endregion

		#region GetExcelHeader pour les pops up insertions (Colonne cr�ation depuis les alertes concu, potentiel, etc.)
		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel de la pop up des insertions
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dateFormatText">Date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <param name="idElement">identifiant de l'�l�ment</param>
		/// <param name="level">Niveau de l'�l�ment s�lectionn�</param>
		/// <returns>HTML</returns>
		public static string GetExcelHeaderForCreationsPopUp(WebSession webSession, bool dateFormatText, string periodBeginning, string periodEnd, Int64 idElement,int level){
			StringBuilder t=new StringBuilder(2000);
			try{
				//webSession.CustomerLogin.ModuleList();
                Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

				#region D�but du tableau
                t.Append("<table class=\"BorderLevel\" cellpadding=0 cellspacing=0>");
				t.Append("<tr><td class=\"excelDataItalic\">"+GestionWeb.GetWebWord(512,webSession.SiteLanguage)+"</td></tr>");
				#endregion

				// R�sultat : Insertions
				t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(793,webSession.SiteLanguage) +" :</font> "+ GestionWeb.GetWebWord(940,webSession.SiteLanguage) +"</td></tr>");
				// P�riode
				t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));
				// Media
				t.Append(GetMediaSelectedForMediaPlanPopUp(webSession));
				// Support
				t.Append(GetCompetitorMediaSelected(webSession));
				// Produit s�lectionn�
				t.Append(GetProductSelectedForCreationsPopUp(webSession, idElement, level));

				t.Append(GetBlankLine());
				t.Append("</table><br>");

				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel",err)); 
			}
		}
		#endregion

		#region GetExcelHeader pour les pops up insertions (Colonne cr�ation depuis une pop up plan m�dia et un zoom plan m�dia)
		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel de la pop up des insertions
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dateFormatText">Date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <param name="mediaImpactedList">Liste des medias impact�s</param>
		/// <param name="idVehicle">Identifiant du media s�lectionn�</param>
		/// <returns>HTML</returns>
		public static string GetExcelHeaderForCreationsPopUpFromMediaPlan(WebSession webSession, bool dateFormatText, string periodBeginning, string periodEnd, ListDictionary mediaImpactedList, int idVehicle){
			// ListDictionary mediaImpactedList : permet de connaitre l'�l�ment s�lectionn�

			StringBuilder t=new StringBuilder(2000);
			try{
				//webSession.CustomerLogin.ModuleList();
                Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

				#region D�but du tableau
                t.Append("<table class=\"greyBorder\" cellpadding=0 cellspacing=0>");
				t.Append("<tr><td class=\"excelDataItalic\">"+GestionWeb.GetWebWord(512,webSession.SiteLanguage)+"</td></tr>");
				#endregion

				// R�sultat : Insertions
				t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(793,webSession.SiteLanguage) +" :</font> "+ GestionWeb.GetWebWord(940,webSession.SiteLanguage) +"</td></tr>");
				// P�riode
				t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));			
				// Media
				AdExClassification.MediaBranch.PartialVehicleListDataAccess vehicleName = new AdExClassification.MediaBranch.PartialVehicleListDataAccess(idVehicle.ToString(),webSession.SiteLanguage,webSession.Source);
				t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(190,webSession.SiteLanguage) +" : </font> "+ vehicleName[idVehicle].ToString() +"</TD></TR>");
				// El�ment du niveau s�lectionn� (L1 ou L2 ou L3 ou L4) depuis le picto creation du PM
				if(mediaImpactedList!=null)
					t.Append(GetMediaSelectedForCreationsPopUp(webSession, mediaImpactedList));

				t.Append(GetBlankLine());
				t.Append("</table><br>");

				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel",err)); 
			}
		}
		#endregion

		#region GetExcelHeader pour la pop up des insertions (presse), des spots (tv, radio) dans une alerte portefeuille
		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel de la pop up des insertions
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dateFormatText">Date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <param name="idVehicle">Identifiant du media s�lectionn�</param>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="allPeriod">Bool�en pour pr�ciser si nous avons toute la p�riode</param>
		/// <returns>HTML</returns>
		public static string GetExcelHeaderForCreationsPopUpFromPortofolio(WebSession webSession, bool dateFormatText, string periodBeginning, string periodEnd, int idVehicle, int idMedia, bool allPeriod){
			return(GetExcelHeaderForCreationsPopUpFromPortofolio(webSession, dateFormatText, periodBeginning, periodEnd, idVehicle, idMedia, allPeriod, "", ""));
		}

		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel de la pop up des insertions
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="dateFormatText">Date en format texte</param>
		/// <param name="periodBeginning">Date de d�but</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <param name="idVehicle">Identifiant du media s�lectionn�</param>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="allPeriod">Bool�en pour pr�ciser si nous avons toute la p�riode</param>
		/// <param name="nameDay">Jour nomm� en radio tv</param>
		/// <param name="codeEcran">Code �cran en radio tv</param>
		/// <returns>HTML</returns>
		public static string GetExcelHeaderForCreationsPopUpFromPortofolio(WebSession webSession, bool dateFormatText, string periodBeginning, string periodEnd, int idVehicle, int idMedia, bool allPeriod, string nameDay, string codeEcran){
			StringBuilder t=new StringBuilder(2000);
			try{
				//webSession.CustomerLogin.ModuleList();
                Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);

				#region D�but du tableau
				t.Append("<table style=\"border:solid 1px #808080;\" cellpadding=0 cellspacing=0>");
				t.Append("<tr><td class=\"excelDataItalic\">"+GestionWeb.GetWebWord(512,webSession.SiteLanguage)+"</td></tr>");
				#endregion

				// R�sultat : D�tail support
				t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(793,webSession.SiteLanguage) +" :</font> "+ GestionWeb.GetWebWord(1378,webSession.SiteLanguage) +"</td></tr>");
				// P�riode
				if(allPeriod){ // Toute la p�riode en :presse, radio, tv
					t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));
				}
				else{ 
					switch((ClassificationConstant.Vehicles.names)idVehicle){
						case ClassificationConstant.Vehicles.names.press:
						case ClassificationConstant.Vehicles.names.internationalPress:
							// Date de parution (presse)
							t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage) +" :</font> "+ TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY(periodBeginning,webSession.SiteLanguage) +"</td></tr>");
							break;
						case ClassificationConstant.Vehicles.names.radio:
						case ClassificationConstant.Vehicles.names.tv:
						case ClassificationConstant.Vehicles.names.others:
							// P�riode
							t.Append(GetDateSelected(webSession, currentModule, dateFormatText, periodBeginning, periodEnd));
							// jour nomm�
							t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1574,webSession.SiteLanguage) +" :</font> "+ TNS.AdExpress.Web.Functions.Dates.getDay(webSession,nameDay) +"</td></tr>");
							// code �cran
							t.Append("<tr><td colspan=4 class=\"excelData\"><font class=txtBoldGrisExcel>"+GestionWeb.GetWebWord(1431,webSession.SiteLanguage) +" :</font> "+ codeEcran +"</td></tr>");
							break;
					}
				}			
				// Media
				AdExClassification.MediaBranch.PartialVehicleListDataAccess vehicleName = new AdExClassification.MediaBranch.PartialVehicleListDataAccess(idVehicle.ToString(),webSession.SiteLanguage,webSession.Source);
				t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(190,webSession.SiteLanguage) +" : </font> "+ vehicleName[idVehicle].ToString() +"</TD></TR>");
				// Support
				AdExClassification.MediaBranch.PartialMediaListDataAccess mediaName = new AdExClassification.MediaBranch.PartialMediaListDataAccess(idMedia.ToString(),webSession.SiteLanguage,webSession.Source);
				t.Append("<TR><TD colspan=4 class=\"excelData\" ><font class=txtBoldGrisExcel>"+ GestionWeb.GetWebWord(971,webSession.SiteLanguage) +" : </font> "+ mediaName[idMedia].ToString() +"</TD></TR>");

				t.Append(GetBlankLine());
				t.Append("</table><br>");

				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel",err)); 
			}
		}
		#endregion

        #region GetExcelHeader pour le plan media version Quali
        /// <summary>
        /// G�n�re l'en t�te html pour le fichier Excel du plan media version Quali
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>HTML</returns>
        public static string GetExcelHeaderForCreativeMediaPlan(WebSession webSession) {

            StringBuilder t = new StringBuilder(2000);
            try {

                string dateBegin, dateEnd;
                dateBegin = DateString.dateTimeToDD_MM_YYYY((new DateTime(int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(webSession.PeriodBeginningDate.Substring(4, 2)), int.Parse(webSession.PeriodBeginningDate.Substring(6, 2)))), webSession.SiteLanguage);
                dateEnd = DateString.dateTimeToDD_MM_YYYY((new DateTime(int.Parse(webSession.PeriodEndDate.Substring(0, 4)), int.Parse(webSession.PeriodEndDate.Substring(4, 2)), int.Parse(webSession.PeriodEndDate.Substring(6, 2)))), webSession.SiteLanguage);

                #region Ajout CSS
                t.Append("<Style><!--");
                t.Append(".TexteTitreRappelScanpub { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #FFFFFF; font-weight:bold; background-color : #999999;}");
                t.Append(".TexteVioletPetitScanpub { font-family: Arial, Helvetica, sans-serif; font-size: 12px; color: #5A5A90; font-weight:bold }");
                t.Append(".TexteVioletGrandScanpub { font-family: Arial, Helvetica, sans-serif; font-size: 13px; color: #5A5A90; font-weight:bold }");
                t.Append(".TexteGris2Scanpub { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #454545; }");
                t.Append(".RappelBGColor {  background-color : #FF00FF }");
                t.Append(".Arial7rouge {  font-family: Arial, Helvetica, sans-serif; font-size: 8pt; font-weight: normal ; color: #333333 }");
                t.Append(".Arial7gris {  font-family: Arial, Helvetica, sans-serif; font-size: 8pt; font-weight: normal ; color: #666666 }");
                t.Append(".Arial10gris {  font-family: Arial, Helvetica, sans-serif; font-size: 10pt; font-weight: normal ; color: #666666 }");
                t.Append(".txtViolet11Bold{ font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #644883; font-weight: bold; }");
                t.Append("td.ptX {font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #FFFFFF; font-weight: bold; text-decoration : none; background-color: #808080; padding-left:5px; padding-right:5px; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0;	border-right-width: 1px; border-right-style:dotted;}");
                t.Append("td.paX {font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #FFFFFF; font-weight: bold; text-decoration : none; text-align:center; background-color: #666699; padding-left:5px; padding-right:5px; border-left-width:0px; border-top-width:0px; border-bottom-color:#FFFFFF; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#FFFFFF; border-right-width: 1px; border-right-style:dotted;}");
                t.Append("td.ppX{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold; text-decoration : none; text-align:center; border-left-width:0px; border-top-width:0px; border-bottom-width:0px; border-right-color:#C0C0C0; border-right-width: 1px; border-right-style:dotted; background-color: #C0C0C0; width:17px; }");
                t.Append("td.ppiX { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight: bold; text-decoration : none; text-align:center; border-left-width:0px; border-top-width:0px; border-bottom-width:0px; border-right-color:#644883; border-right-width: 1px; border-right-style:dotted; background-color: #666699; width:17px; }");
                t.Append("td.pdwX { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color : #000000; font-weight:normal; text-decoration : none; text-align:center; border-left-width:0px; border-top-width:0px; border-bottom-width:1px; border-bottom-color:#C0C0C0; border-bottom-style:dotted; border-right-color:#C0C0C0; border-top-color:#000000; border-right-width: 1px;  border-right-style:dotted; background-color: #808080; width:17px; }");
                t.Append("td.pdX { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color : #000000; font-weight:normal; text-decoration : none; text-align:center; border-left-width:0px; border-top-width:0px; border-bottom-width:1px; border-bottom-color:#C0C0C0; border-bottom-style:dotted; border-right-color:#C0C0C0; border-top-color:#000000; border-right-width: 1px;  border-right-style:dotted; background-color: #FFFFFF; width:17px;}");
                t.Append("td.L0X { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight: bold; text-decoration : none; text-align:left; background-color: #FFFFFF; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px;}");
                t.Append("td.L0Xnb { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight: bold; text-decoration : none; text-align:right; background-color: #FFFFFF; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px; text-align:right;}");
                t.Append("td.L1X { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #FFFFFF; font-weight: bold; text-decoration : none; text-align:left; background-color: #969696; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px;}");
                t.Append("td.L1Xnb { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #FFFFFF; font-weight: bold; text-decoration : none; text-align:right; background-color: #969696; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px; text-align:right}");
                t.Append("td.L2X { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight: bold; text-decoration : none; text-align:left; background-color: #C0C0C0; padding:0px,0px,0px,2px; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px;}");
                t.Append("td.L2Xnb { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight: bold; text-decoration : none; text-align:right; background-color: #C0C0C0; padding:0px,0px,0px,2px; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px; text-align:right}");
                t.Append("td.L3X { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight: bold; text-decoration : none; text-align:left; background-color: #FFFFFF; padding:0px,0px,0px,2px; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px;}");
                t.Append("td.L3Xnb { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight: bold; text-decoration : none; text-align:right; background-color: #FFFFFF; padding:0px,0px,0px,2px; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px; text-align:right}");
                t.Append("td.L4X { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight:normal; text-decoration : none; text-align:left; background-color: #CCCCFF; padding:0px,0px,0px,2px; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px;}");
                t.Append("td.L4Xnb { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color : #000000; font-weight:normal; text-decoration : none; text-align:right; background-color: #CCCCFF; padding:0px,0px,0px,2px; border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px;  border-right-style:dotted; padding-left:4px; padding-right:4px; text-align:right}");
                t.Append("td.p3X {	border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px; border-right-style:dotted; background-color: #FFFFFF; color:#FFFFFF; width:17px;}");
                t.Append("td.p4X {	border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px; border-right-style:dotted; background-color: #808080; color:#FFFFFF; width:17px;}");
                t.Append("td.p5X { border-left-width:0px; border-top-width:0px; border-bottom-color:#C0C0C0; border-bottom-width:1px; border-bottom-style:dotted; border-right-color:#C0C0C0; border-right-width: 1px; border-right-style:dotted; background-color: #C0C0C0; color:#C0C0C0; width:17px;}");

                #region Old code
                /*t.Append("td.p1{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;}");
                t.Append("td.p2{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
                t.Append("td.p2excel{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:0px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
                t.Append("td.pmannee{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-align:center;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
                t.Append("td.pmannee2{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-align:center;text-decoration : none;background-color: #644883;padding-left:5px;padding-right:5px;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-bottom-style:solid;border-right-width: 0px;}");
                t.Append("td.p10{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;text-align:center;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-right-color:#644883;border-right-width: 1px; border-right-style:solid;background-color: #B1A3C1;width:17px;}");
                t.Append("td.pmtotal{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #FFFFFF;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;}");
                t.Append("td.pmvehicle{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #9D9885;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;}");
                t.Append("td.pmcategory{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #B1A3C1;padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;}");
                t.Append("td.pmmediaxls1{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight:normal;text-decoration : none;background-color: #9999FF;padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
                t.Append("td.pmmediaxls2{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight:normal;text-decoration : none;background-color: #CCCCFF;	padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;}");
                t.Append("td.pmperiodxls{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;text-align:center;border-left-width:0px;border-top-width:0px;border-bottom-width:0px;border-right-color:#644883;border-right-width: 1px; border-right-style:solid;background-color: #B1A3C1;width:15pt;}");
                t.Append("td.pmperioditemxls1{border-left-width:0px;border-top-width:0px;border-bottom-color:#B1A3C1;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#B1A3C1;border-right-width: 1px; border-right-style:solid;background-color: #B1A3C1;width:15pt;}");
                t.Append("td.pmperioditemxls{border-left-width:0px;border-top-width:0px;border-bottom-color:#B1A3C1;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#B1A3C1;border-right-width: 1px; border-right-style:solid;background-color: #666699;width:15pt;}");
                t.Append("td.pmperioditemxls2{border-left-width:0px;border-top-width:0px;border-bottom-color:#B1A3C1;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#B1A3C1;border-right-width: 1px; border-right-style:solid;background-color: #9999FF;width:15pt;}");
                t.Append("td.pmtotalnb{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #FFFFFF;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;text-align:right;}");
                t.Append("td.pmvehiclenb{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #FFFFFF;font-weight: bold;text-decoration : none;background-color: #9D9885;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;text-align:right}");
                t.Append("td.pmcategorynb{font-family: Arial, Helvetica, sans-serif;font-size: 11px;color : #000000;font-weight: bold;text-decoration : none;background-color: #B1A3C1;padding:0px,0px,0px,2px;border-left-width:0px;border-top-width:0px;border-bottom-color:#FFFFFF;border-bottom-width:1px;border-bottom-style:solid;border-right-color:#FFFFFF;border-right-width: 1px; border-right-style:solid;padding-left:4px;padding-right:4px;text-align:right}");*/
                #endregion

                t.Append("--></Style>");
                #endregion

                t.Append("<table border=0 cellspacing=1 cellpadding=0 class=\"TexteTitreRappelScanpub\">");
                // Titre
                t.Append("<tr>");
                t.Append("<td align=center class=\"RappelBGColor\">&nbsp;&nbsp;" + GestionWeb.GetWebWord(1791, webSession.SiteLanguage) + "&nbsp;&nbsp;</td>");
                t.Append("</tr>");
                t.Append("<tr>");
                t.Append("<td>");
                t.Append("<table border=0 cellspacing=0 cellpadding=0 bgcolor=#ffffff width=100% >");
                // Media
                t.Append("<tr valign=top>");
                t.Append("<td class=Arial7rouge>&nbsp;&nbsp;" + GestionWeb.GetWebWord(1792, webSession.SiteLanguage) + "</td>");
                t.Append("<td class=Arial7gris>:");
                foreach (TreeNode currentNode in webSession.SelectionUniversMedia.Nodes) {
                    t.Append("&nbsp;" + ((LevelInformation)currentNode.Tag).Text + "<br>");
                }
                t.Append("</td>");
                t.Append("</tr>");
                //Produit
                t.Append("<tr valign=top>");
                t.Append("<td class=Arial7rouge>&nbsp;&nbsp;" + GestionWeb.GetWebWord(858, webSession.SiteLanguage) + "</td>");
                t.Append("<td class=Arial7gris>:&nbsp;");

                TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = webSession.PrincipalProductUniverses[0];
                List<NomenclatureElementsGroup> groups = adExpressUniverse.GetIncludes();
                string productLevelIdsList = "";
                ProductClassification.PartialProductLevelListDataAccess productLabels;
                List<long> levelIdsList;

                if (groups != null && groups.Count > 0) {
                    for (int i = 0; i < groups.Count; i++) {
                        productLevelIdsList = groups[0].GetAsString(TNSClassificationLevels.PRODUCT);
                        levelIdsList = groups[0].Get(TNSClassificationLevels.PRODUCT);
                        productLabels = new ProductClassification.PartialProductLevelListDataAccess(productLevelIdsList, webSession.SiteLanguage, webSession.CustomerLogin.Source);
                        foreach (long id in levelIdsList) {
                            t.Append(productLabels[id] + "&nbsp;&nbsp;<br>");
                        }
                    }
                }

                t.Append("</td>");
                t.Append("</tr>");

                //Version
                if (webSession.IdSlogans != null && webSession.IdSlogans.Count > 0) {
                    t.Append("<tr valign=top>");
                    t.Append("<td class=Arial7rouge>&nbsp;&nbsp;" + GestionWeb.GetWebWord(1888, webSession.SiteLanguage) + "</td>");
                    t.Append("<td class=Arial7gris>:&nbsp;");

                    foreach (long id in webSession.IdSlogans) {
                        t.Append(((LevelInformation)webSession.SelectionUniversMedia.Nodes[0].Tag).ID + "" + id + "&nbsp;&nbsp;<br>");
                    }

                    t.Append("</td>");
                    t.Append("</tr>");
                }

                //Date d�but
                t.Append("<tr>");
                t.Append("<td class=Arial7rouge>&nbsp;&nbsp;" + GestionWeb.GetWebWord(1793, webSession.SiteLanguage) + "</td>");
                t.Append("<td class=Arial7gris>:&nbsp;" + dateBegin + "</td>");
                t.Append("</tr>");
                // Date fin
                t.Append("<tr>");
                t.Append("<td class=Arial7rouge>&nbsp;&nbsp;" + GestionWeb.GetWebWord(1794, webSession.SiteLanguage) + "</td>");
                t.Append("<td class=Arial7gris>:&nbsp;" + dateEnd + "</td>");
                t.Append("</tr>");
                //Unit�
                t.Append("<tr>");
                t.Append("<td class=Arial7rouge>&nbsp;&nbsp;" + GestionWeb.GetWebWord(1795, webSession.SiteLanguage) + "</td>");
                t.Append("<td colspan=2 class=Arial7gris>:&nbsp;Euro</td>");
                t.Append("</tr>");
                t.Append("</table>");
                t.Append("</td>");
                t.Append("</tr>");
                t.Append("<tr bgcolor=#ffffff><td>&nbsp;</td></tr>");
                t.Append("<tr bgcolor=#ffffff><td>&nbsp;</td></tr>");
                t.Append("</table>");

                return Convertion.ToHtmlString(t.ToString());

            }
            catch (System.Exception err) {
                throw (new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des param�tres dans le fichier Excel", err));
            }

        }
        #endregion

        #region Overloaded methods
        /// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel des modules
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="levelDetail">boolean that indicates that whether we need to display details or not, details can be in the format
		/// 1:Produites d�taill�s par:
		/// 2:M�dia d�taill�s par:
		/// 3:Unit� d�taill�s par:
		/// </param>
		/// <param name="units">boolean that indicates that whether we need units or not</param>
		/// <param name="resultType">boolean that indicates that whether we need to display result type or not</param>
		/// <returns>Code HTML pour en t�te de fichier Excel</returns>
		public static string GetExcelHeader(WebSession webSession,bool levelDetail,bool units,bool resultType){
			return GetExcelHeader(webSession,levelDetail,units,resultType,false,false,false,"","","","",-1);
		}

		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel des modules
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="levelDetail">boolean that indicates that whether we need to display details or not, details can be in the format
		/// 1:Produites d�taill�s par:
		/// 2:M�dia d�taill�s par:
		/// 3:Unit� d�taill�s par:
		/// </param>
		/// <param name="units">boolean that indicates that whether we need units or not</param>
		/// <param name="resultType">boolean that indicates that whether we need to display result type or not</param>
		/// <param name="insert">boolean that indicates that whether we need to show insert for press or not</param>
		/// <param name="title">an optional title string to display the title</param>
		/// <returns>Code HTML pour en t�te de fichier Excel</returns>
		public static string GetExcelHeader(WebSession webSession,bool levelDetail,bool units,bool resultType,bool insert,string title){
			return GetExcelHeader(webSession,levelDetail,units,resultType,insert,false,false,"","",title,"",-1);
		}

		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel des modules
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="levelDetail">boolean that indicates that whether we need to display details or not, details can be in the format
		/// 1:Produites d�taill�s par:
		/// 2:M�dia d�taill�s par:
		/// 3:Unit� d�taill�s par:</param>
		/// <param name="units">boolean that indicates that whether we need units or not</param>
		/// <param name="resultType">boolean that indicates that whether we need to display result type or not</param>
		/// <param name="title">an optional title string to display the title</param>
		/// <returns>Code HTML pour en t�te de fichier Excel</returns>
		public static string GetExcelHeader(WebSession webSession,bool levelDetail,bool units,bool resultType,string title){
			return GetExcelHeader(webSession,levelDetail,units,resultType,false,false,false,"","",title,"",-1);
		}

		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel des modules
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="title">an optional title string to display the title</param>
		/// <returns>Code HTML pour en t�te de fichier Excel</returns>
		public static string GetExcelHeader(WebSession webSession,string title){
			return GetExcelHeader(webSession,false,false,false,false,false,false,"","",title,"",-1);
		}

        /// <summary>
        /// G�n�re l'en t�te html pour le fichier Excel des modules
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="units">boolean that indicates that whether we need units or not</param>
        /// <param name="dateFormatText">To display the date in the format like jan 2004-dec 2005</param>
        /// <returns>Code HTML pour en t�te de fichier Excel</returns>
        public static string GetExcelHeader(WebSession webSession, bool units, bool dateFormatText) {
            return GetExcelHeader(webSession, false, units, false, false, false, dateFormatText, "", "", "", "",-1);
        }

		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel des modules
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="units">boolean that indicates that whether we need units or not</param>
		/// <param name="dateFormatText">To display the date in the format like jan 2004-dec 2005</param>
        /// <param name="zoomDate">Date de zoom</param>
        /// <param name="periodDisplayLevel">Niveau de d�tail de l'affichage des p�riodes</param>
		/// <returns>Code HTML pour en t�te de fichier Excel</returns>
        public static string GetExcelHeader(WebSession webSession, bool units, bool dateFormatText, string zoomDate, int periodDisplayLevel) {
            return GetExcelHeader(webSession, false, units, false, false, false, dateFormatText, "", "", "", zoomDate, periodDisplayLevel);
		}

		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel des modules
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="units">boolean that indicates that whether we need units or not</param>
		/// <param name="periodBeginning">takes the beginnig date</param>
		/// <param name="periodEnd">takes the ending date</param>
		/// <returns>Code HTML pour en t�te de fichier Excel</returns>
		public static string GetExcelHeader(WebSession webSession,bool units,string periodBeginning,string periodEnd){
			return GetExcelHeader(webSession,false,units,false,false,false,false,periodBeginning,periodEnd,"","",-1);
		}

		/// <summary>
		/// G�n�re l'en t�te html pour le fichier Excel des modules
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="levelDetail">boolean that indicates that whether we need to display details or not, details can be in the format
		/// 1:Produites d�taill�s par:
		/// 2:M�dia d�taill�s par:
		/// 3:Unit� d�taill�s par:
		/// </param>
		/// <param name="units">boolean that indicates that whether we need units or not</param>
		/// <param name="resultType">boolean that indicates that whether we need to display result type or not</param>
		/// <param name="media">boolean that indicates that whether we need to display media or not</param>
		/// <param name="dateFormatText">To display the date in the format like jan 2004-dec 2005</param>
		/// <param name="title">an optional title string to display the title</param>
		/// <returns>Code HTML pour en t�te de fichier Excel</returns>
		public static string GetExcelHeader(WebSession webSession,bool levelDetail,bool units,bool resultType,bool media,bool dateFormatText,string title){
			return GetExcelHeader(webSession,levelDetail,units,resultType,false,media,dateFormatText,"","",title,"",-1);
		}
		#endregion

		#region M�thodes Priv�
		/// <summary>
		///  Calculates and Returns the Result type which was selected
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Returns resultType selected string</returns>	
		private static string StringTypeResult(WebSession webSession){		
			try{
				if(webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_CONCURENTIELLE
					|| webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE){
					switch(webSession.CurrentTab){
						case FrameWorkResultConstantes.CompetitorAlert.PORTEFEUILLE:
							return(GestionWeb.GetWebWord(1125,webSession.SiteLanguage));
						case FrameWorkResultConstantes.CompetitorAlert.COMMON:
							return(GestionWeb.GetWebWord(1127,webSession.SiteLanguage));
						case FrameWorkResultConstantes.CompetitorAlert.ABSENT:
							return(GestionWeb.GetWebWord(1126,webSession.SiteLanguage));
						case FrameWorkResultConstantes.CompetitorAlert.EXCLUSIF:
							return(GestionWeb.GetWebWord(1128,webSession.SiteLanguage));										
						default:				
							return("no value");
					}
				}
				else if(webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_POTENTIELS
					|| webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_POTENTIELS){
					switch(webSession.CurrentTab){
						case FrameWorkResultConstantes.MarketShare.PORTEFEUILLE:
							return(GestionWeb.GetWebWord(1125,webSession.SiteLanguage));
						case FrameWorkResultConstantes.MarketShare.FORCES:
							return(GestionWeb.GetWebWord(1157,webSession.SiteLanguage));
						case FrameWorkResultConstantes.MarketShare.POTENTIELS:
							return(GestionWeb.GetWebWord(1158,webSession.SiteLanguage));									
						default:				
							return("no value");
					}
				}
				else if(webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_DYNAMIQUE){
					switch(webSession.CurrentTab){
						case FrameWorkResultConstantes.DynamicAnalysis.PORTEFEUILLE:
							return(GestionWeb.GetWebWord(1125,webSession.SiteLanguage));
						case FrameWorkResultConstantes.DynamicAnalysis.LOYAL:
							return(GestionWeb.GetWebWord(1494,webSession.SiteLanguage));
						case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_DECLINE:
							return(GestionWeb.GetWebWord(1495,webSession.SiteLanguage));
						case FrameWorkResultConstantes.DynamicAnalysis.LOYAL_RISE:
							return(GestionWeb.GetWebWord(1496,webSession.SiteLanguage));
						case FrameWorkResultConstantes.DynamicAnalysis.LOST:
							return(GestionWeb.GetWebWord(1163,webSession.SiteLanguage));
						case FrameWorkResultConstantes.DynamicAnalysis.WON:
							return(GestionWeb.GetWebWord(1497,webSession.SiteLanguage));
						case FrameWorkResultConstantes.DynamicAnalysis.SYNTHESIS:
							return(GestionWeb.GetWebWord(1853,webSession.SiteLanguage));	
						default:				
							return("no value");
					}
				}
				else{
					return("no value");
				}

			}
			catch(Exception){
				return("no value");
			}		
		}

		/// <summary>
		/// Calculates and returns the vehicle that is being selected
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <returns>Returns the selected vehicle string</returns>	
		private static string StringMediaSelected(WebSession webSession){
			ClassificationConstant.Vehicles.names vehicletype;
			try{
				string mediaNames="";
				string mediaAccessList = webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);		
				string[] mediaList=mediaAccessList.Split(',');
 
				for(int i=0;i<mediaList.Length;i++){				
					if(i!=0){
						mediaNames+=",";
					}
					vehicletype=( ClassificationConstant.Vehicles.names)int.Parse(mediaList[i].ToString());
					switch(vehicletype){
						case ClassificationConstant.Vehicles.names.press:
							mediaNames+=GestionWeb.GetWebWord(204,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.radio:
							mediaNames+=GestionWeb.GetWebWord(205,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.tv:
							mediaNames+=GestionWeb.GetWebWord(206,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.mediasTactics:
							mediaNames+=GestionWeb.GetWebWord(1593,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.others:
							mediaNames+=GestionWeb.GetWebWord(1594,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.internet:
							mediaNames+=GestionWeb.GetWebWord(1301,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.outdoor:
							mediaNames+=GestionWeb.GetWebWord(1302,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.cinema:
							mediaNames+=GestionWeb.GetWebWord(1303,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.plurimedia:
							mediaNames+=GestionWeb.GetWebWord(1596,webSession.SiteLanguage);
							break;
						case ClassificationConstant.Vehicles.names.internationalPress:
							mediaNames+=GestionWeb.GetWebWord(646,webSession.SiteLanguage);
							break;
						default:
							mediaNames+="no value";
							break;
					}
				}
				return mediaNames;
			}
			catch(Exception){
				return("no value");
			}
		}

		/// <summary>
		/// Calculates and returns the detail media that is being selected
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <returns>Returns the detail media string</returns>	
		private static string StringLevelMedia(WebSession webSession){
			try{
				switch(webSession.PreformatedMediaDetail){
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
						return(GestionWeb.GetWebWord(1292,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
						return(GestionWeb.GetWebWord(1142,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
						return(GestionWeb.GetWebWord(1143,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
						return(GestionWeb.GetWebWord(1542,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
						return(GestionWeb.GetWebWord(1543,webSession.SiteLanguage));
					case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
						return(GestionWeb.GetWebWord(1544,webSession.SiteLanguage));
					default:
						return("no value");
				}
			}
			catch(Exception){
				return("no value");
			}
		}

		/// <summary>
		/// V�rifie si produits s�lectionn�s
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns>vrai si produis s�lectionn�s</returns>
		private static bool IsSelectionProductSelected(WebSession webSession){
			switch(webSession.CurrentModule){
				case CstWeb.Module.Name.TABLEAU_DE_BORD_PRESSE :
				case CstWeb.Module.Name.TABLEAU_DE_BORD_RADIO :
				case CstWeb.Module.Name.TABLEAU_DE_BORD_TELEVISION:
				case CstWeb.Module.Name.TABLEAU_DE_BORD_PAN_EURO:
					if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0) return true;
					else return false;
				default : return false;
			}
		}

		/// <summary>
		/// Ajoute la liste des cl�s des styles Css
		/// </summary>
		/// <param name="cssKeys"> liste des cl�s des styles Css</param>
		protected void AddCssStyles(ArrayList cssKeys){
				_cssKeys = cssKeys;
		}
		#endregion

		#region Abstract Methods
		/// <summary>
		/// Does not do anythong as from its page, there is no redirection
		/// </summary>
		/// <returns>string.empty</returns>
		protected override string GetNextUrlFromMenu() {
			return string.Empty;
		}
		#endregion

		#region Ev�nement
		/// <summary>
		/// Page Loading
		/// </summary>
		/// <param name="sender">Source Object</param>
		/// <param name="e">Arguments</param>
		private void ExcelWebPage_Load(object sender, EventArgs e) {
				
//			TNS.AdExpress.Web.UI.HtmlHeader header = new HtmlHeader(_cssKeys);
//			this.Response.ContentType = "application/vnd.ms-excel";
//			this.Page.Controls.AddAt(0,header);			
		}

		#endregion

	}
}
