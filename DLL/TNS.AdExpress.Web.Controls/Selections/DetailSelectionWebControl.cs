#region Information
// Auteur : B.Masson
// Créé le : 07/12/2006
// Modifié par: 
#endregion

#region Namespaces
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Data;

using System.Text;
using System.Windows.Forms;
//using Oracle.DataAccess.Client;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using AdExClassification=TNS.AdExpress.DataAccess.Classification;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

#endregion

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Composant pour la construction du code html pour le rappel de sélection
	/// </summary>
	[DefaultProperty("Text"),  
		ToolboxData("<{0}:DetailSelectionWebControl runat=server></{0}:DetailSelectionWebControl>")]
	public class DetailSelectionWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Session
		/// </summary>
		private WebSession _webSession;
		/// <summary>
		/// Date de début
		/// </summary>
		private string _periodBeginning = String.Empty;
		/// <summary>
		/// Date de fin
		/// </summary>
		private string _periodEnd = String.Empty;
		/// <summary>
		/// Format de date en texte
		/// </summary>
		private bool _dateFormatText = false;
		/// <summary>
		/// Spécifie si on affiche le détail de sélection pour l'export Excel
		/// </summary>
		private RenderType _renderType = RenderType.html;

		/// <summary>
		/// Style du titre global du tableau
		/// </summary>
		private string _cssTitleGlobal = String.Empty;
		/// <summary>
		/// Style d'un titre
		/// </summary>
		private string _cssTitle = String.Empty;
		/// <summary>
		/// Style d'une donnée d'un titre
		/// </summary>
		private string _cssTitleData = String.Empty;
		/// <summary>
		/// Style pour le niveau 1
		/// </summary>
		private string _cssLevel1 = String.Empty;
		/// <summary>
		/// Style pour le niveau 2
		/// </summary>
		private string _cssLevel2 = String.Empty;
		/// <summary>
		/// Style pour le niveau 3
		/// </summary>
		private string _cssLevel3 = String.Empty;
        /// <summary>
        /// Right border css for level 1
        /// </summary>
        private string _cssRightBorderLevel1 = String.Empty;
        /// <summary>
        /// Right border css for level 2
        /// </summary>
        private string _cssRightBorderLevel2 = String.Empty;
        /// <summary>
        /// Right border css for level 3
        /// </summary>
        private string _cssRightBorderLevel3 = String.Empty;
        /// <summary>
        /// Right Bottom border css for level 1
        /// </summary>
        private string _cssRightBottomBorderLevel1 = String.Empty;
        /// <summary>
        /// Right Bottom border css for level 2
        /// </summary>
        private string _cssRightBottomBorderLevel2 = String.Empty;
        /// <summary>
        /// Right Bottom border css for level 3
        /// </summary>
        private string _cssRightBottomBorderLevel3 = String.Empty;
		/// <summary>
		/// Style pour la bordure d'un niveau
		/// </summary>
		private string _cssBorderLevel = String.Empty;
		/// <summary>
		/// Style vérifié du titre global du tableau > GetCssTitleGlobalHTML()
		/// </summary>
		private string cssTitleGlobal="";
		/// <summary>
		/// Style vérifié d'un titre > GetCssTitleHTML()
		/// </summary>
		private string cssTitle="";
		/// <summary>
		/// Style vérifié d'une donnée d'un titre > GetCssTitleDataHTML()
		/// </summary>
		private string cssTitleData="";
		/// <summary>
		/// Style vérifié pour le niveau 1 > GetCssLevel1HTML()
		/// </summary>
		private string cssLevel1="";
		/// <summary>
		/// Style vérifié pour le niveau 2 > GetCssLevel2HTML()
		/// </summary>
		private string cssLevel2="";
		/// <summary>
		/// Style vérifié pour le niveau 3 > GetCssLevel3HTML()
		/// </summary>
		private string cssLevel3="";
        /// <summary>
        /// Style verified for level 1 > GetCssRightBorderLevel1HTML()
        /// </summary>
        private string cssRightBorderLevel1 = "";
        /// <summary>
        /// Style verified for level 2 > GetCssRightBorderLevel2HTML()
        /// </summary>
        private string cssRightBorderLevel2 = "";
        /// <summary>
        /// Style verified for level 3 > GetCssRightBorderLevel3HTML()
        /// </summary>
        private string cssRightBorderLevel3 = "";
        /// <summary>
        /// Style verified for level 1 > GetCssRightBottomBorderLevel1HTML()
        /// </summary>
        private string cssRightBottomBorderLevel1 = "";
        /// <summary>
        /// Style verified for level 2 > GetCssRightBottomBorderLevel2HTML()
        /// </summary>
        private string cssRightBottomBorderLevel2 = "";
        /// <summary>
        /// Style verified for level 3 > GetCssRightBottomBorderLevel3HTML()
        /// </summary>
        private string cssRightBottomBorderLevel3 = "";
		/// <summary>
		/// Style vérifié pour la bordure d'un niveau > GetCssBorderLevelHTML()
		/// </summary>
		private string cssBorderLevel="";
		#endregion

		#region Accesseurs
		/// <summary>
		/// Session
		/// </summary>
		public WebSession WebSession{
			get{return _webSession;}
			set{_webSession=value;}
		}
		/// <summary>
		/// Date de début
		/// </summary>
		public string PeriodBeginning{
			get{return _periodBeginning;}
			set{_periodBeginning=value;}
		}
		/// <summary>
		/// Date de fin
		/// </summary>
		public string PeriodEnd{
			get{return _periodEnd;}
			set{_periodEnd=value;}
		}
		/// <summary>
		/// Format de date en texte
		/// </summary>
		public bool DateFormatText{
			get{return _dateFormatText;}
			set{_dateFormatText=value;}
		}
		/// <summary>
		/// Style pour le niveau 1
		/// </summary>
		public string CssLevel1{
			get{return _cssLevel1;}
			set{
				_cssLevel1=value;
				cssLevel1=GetCssLevel1HTML();
			}
		}
		/// <summary>
		/// Style pour le niveau 2
		/// </summary>
		public string CssLevel2{
			get{return _cssLevel2;}
			set{
				_cssLevel2=value;
				cssLevel2=GetCssLevel2HTML();
			}
		}
		/// <summary>
		/// Style pour le niveau 3
		/// </summary>
		public string CssLevel3{
			get{return _cssLevel3;}
			set{
				_cssLevel3=value;
				cssLevel3=GetCssLevel3HTML();
			}
		}
        /// <summary>
        /// Style for level 1
        /// </summary>
        public string CssRightBorderLevel1 {
            get { return _cssRightBorderLevel1; }
            set {
                _cssRightBorderLevel1 = value;
                cssRightBorderLevel1 = GetCssRightBorderLevel1HTML();
            }
        }
        /// <summary>
        /// Style for level 2
        /// </summary>
        public string CssRightBorderLevel2 {
            get { return _cssRightBorderLevel2; }
            set {
                _cssRightBorderLevel2 = value;
                cssRightBorderLevel2 = GetCssRightBorderLevel2HTML();
            }
        }
        /// <summary>
        /// Style for level 3
        /// </summary>
        public string CssRightBorderLevel3 {
            get { return _cssRightBorderLevel3; }
            set {
                _cssRightBorderLevel3 = value;
                cssRightBorderLevel3 = GetCssRightBorderLevel3HTML();
            }
        }
        /// <summary>
        /// Style for level 1
        /// </summary>
        public string CssRightBottomBorderLevel1 {
            get { return _cssRightBottomBorderLevel1; }
            set {
                _cssRightBottomBorderLevel1 = value;
                cssRightBottomBorderLevel1 = GetCssRightBottomBorderLevel1HTML();
            }
        }
        /// <summary>
        /// Style for level 2
        /// </summary>
        public string CssRightBottomBorderLevel2 {
            get { return _cssRightBottomBorderLevel2; }
            set {
                _cssRightBottomBorderLevel2 = value;
                cssRightBottomBorderLevel2 = GetCssRightBottomBorderLevel2HTML();
            }
        }
        /// <summary>
        /// Style for level 3
        /// </summary>
        public string CssRightBottomBorderLevel3 {
            get { return _cssRightBottomBorderLevel3; }
            set {
                _cssRightBottomBorderLevel3 = value;
                cssRightBottomBorderLevel3 = GetCssRightBottomBorderLevel3HTML();
            }
        }
		/// <summary>
		/// Style du titre global du tableau
		/// </summary>
		public string CssTitleGlobal{
			get{return _cssTitleGlobal;}
			set{
				_cssTitleGlobal=value;
				cssTitleGlobal=GetCssTitleGlobalHTML();
			}
		}
		/// <summary>
		/// Style d'un titre
		/// </summary>
		public string CssTitle{
			get{return _cssTitle;}
			set{
				_cssTitle=value;
				cssTitle=GetCssTitleHTML();
			}
		}
		/// <summary>
		/// Style d'une donnée d'un titre
		/// </summary>
		public string CssTitleData{
			get{return _cssTitleData;}
			set{
				_cssTitleData=value;
				cssTitleData=GetCssTitleDataHTML();
			}
		}
		/// <summary>
		/// Style d'une donnée d'un titre
		/// </summary>
		public string CssBorderLevel{
			get{return _cssBorderLevel;}
			set{
				_cssBorderLevel=value;
				cssBorderLevel=GetCssBorderLevelHTML();
			}
		}
		/// <summary>
		/// Spécifie si on affiche le détail de sélection pour l'export Excel
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("RenderType.html"),
		Description("Type de rendu")] 
		public RenderType OutputType {
			get{return _renderType;}
			set{_renderType = value;}
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			switch(_renderType){
				case RenderType.html:
					output.Write(GetHeader());
					break;
				case RenderType.rawExcel:
				case RenderType.excel:
					output.Write(GetHeader());
					break;
			}
		}
		#endregion

		#region Vérification des styles CSS
		/// <summary>
		/// Vérification du style
		/// </summary>
		/// <returns>HTML</returns>
		private string GetCssTitleGlobalHTML(){
			if(_cssTitleGlobal != null && _cssTitleGlobal.Length > 0)return ("class=\""+_cssTitleGlobal+"\"");
			return("");
		}
		/// <summary>
		/// Vérification du style
		/// </summary>
		/// <returns>HTML</returns>
		private string GetCssTitleHTML(){
			if(_cssTitle != null && _cssTitle.Length > 0)return ("class=\""+_cssTitle+"\"");
			return("");
		}
		/// <summary>
		/// Vérification du style
		/// </summary>
		/// <returns>HTML</returns>
		private string GetCssTitleDataHTML(){
			if(_cssTitleData != null && _cssTitleData.Length > 0)return ("class=\""+_cssTitleData+"\"");
			return("");
		}
		/// <summary>
		/// Vérification du style
		/// </summary>
		/// <returns>HTML</returns>
		private string GetCssLevel1HTML(){
			if(_cssLevel1 != null && _cssLevel1.Length > 0)return ("class=\""+_cssLevel1+"\"");
			return("");
		}
		/// <summary>
		/// Vérification du style
		/// </summary>
		/// <returns>HTML</returns>
		private string GetCssLevel2HTML(){
			if(_cssLevel2 != null && _cssLevel2.Length > 0)return ("class=\""+_cssLevel2+"\"");
			return("");
		}
		/// <summary>
		/// Vérification du style
		/// </summary>
		/// <returns>HTML</returns>
		private string GetCssLevel3HTML(){
			if(_cssLevel3 != null && _cssLevel3.Length > 0)return ("class=\""+_cssLevel3+"\"");
			return("");
		}
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssRightBorderLevel1HTML() {
            if (_cssRightBorderLevel1 != null && _cssRightBorderLevel1.Length > 0) return ("class=\"" + _cssRightBorderLevel1 + "\"");
            return ("");
        }
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssRightBorderLevel2HTML() {
            if (_cssRightBorderLevel2 != null && _cssRightBorderLevel2.Length > 0) return ("class=\"" + _cssRightBorderLevel2 + "\"");
            return ("");
        }
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssRightBorderLevel3HTML() {
            if (_cssRightBorderLevel3 != null && _cssRightBorderLevel3.Length > 0) return ("class=\"" + _cssRightBorderLevel3 + "\"");
            return ("");
        }
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssRightBottomBorderLevel1HTML() {
            if (_cssRightBottomBorderLevel1 != null && _cssRightBottomBorderLevel1.Length > 0) return ("class=\"" + _cssRightBottomBorderLevel1 + "\"");
            return ("");
        }
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssRightBottomBorderLevel2HTML() {
            if (_cssRightBottomBorderLevel2 != null && _cssRightBottomBorderLevel2.Length > 0) return ("class=\"" + _cssRightBottomBorderLevel2 + "\"");
            return ("");
        }
        /// <summary>
        /// Vérification du style
        /// </summary>
        /// <returns>HTML</returns>
        private string GetCssRightBottomBorderLevel3HTML() {
            if (_cssRightBottomBorderLevel3 != null && _cssRightBottomBorderLevel3.Length > 0) return ("class=\"" + _cssRightBottomBorderLevel3 + "\"");
            return ("");
        }
		/// <summary>
		/// Vérification du style
		/// </summary>
		/// <returns></returns>
		private string GetCssBorderLevelHTML(){
			if(_cssBorderLevel != null && _cssBorderLevel.Length > 0)return ("class=\""+_cssBorderLevel+"\"");
			return("");
		}
		#endregion

		#region GetHeader
		/// <summary>
		/// Génère l'en tête html pour les exports Excel
		/// </summary>
		/// <returns>HTML</returns>
		public string GetHeader(){
			
			#region Variables
			StringBuilder t = new System.Text.StringBuilder();
			ArrayList detailSelections = null;
			#endregion

			try{

				#region Début du tableau
                t.Append("<table class=\"greyBorder\" cellpadding=0 cellspacing=0 width=100% >");
				switch(OutputType){
					case RenderType.excel:
					case RenderType.rawExcel:
						t.Append("<tr><td "+cssTitleGlobal+">"+GestionWeb.GetWebWord(512,_webSession.SiteLanguage)+"</td></tr>");
						break;
				}
				#endregion

				//_webSession.CustomerLogin.ModuleList();
				Module currentModule = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
				try{
					detailSelections=((ResultPageInformation) currentModule.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
				}
				catch(System.Exception){
					if(currentModule.Id==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
						detailSelections=((ResultPageInformation) currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
				}
				
				foreach(int currentType in detailSelections){
					switch((WebConstantes.DetailSelection.Type)currentType){
						case WebConstantes.DetailSelection.Type.moduleName:
							t.Append(GetModuleName(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.resultName:
							t.Append(GetResultName(_webSession,currentModule));
							break;
						case WebConstantes.DetailSelection.Type.dateSelected:
							t.Append(GetDateSelected(_webSession, currentModule, _dateFormatText, _periodBeginning, _periodEnd));
							break;
                        case WebConstantes.DetailSelection.Type.studyDate:
                            t.Append(GetStudyDate(_webSession));
                            break;
                        case WebConstantes.DetailSelection.Type.comparativeDate:
                            t.Append(GetComparativeDate(_webSession));
                            break;
                        case WebConstantes.DetailSelection.Type.comparativePeriodType:
                            t.Append(GetComparativePeriodTypeDetail(_webSession));
                            break;
                        case WebConstantes.DetailSelection.Type.periodDisponibilityType:
                            t.Append(GetPeriodDisponibilityTypeDetail(_webSession));
                            break;
						case WebConstantes.DetailSelection.Type.unitSelected:
							t.Append(GetUnitSelected(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.vehicleSelected:
							t.Append(GetVehicleSelected(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.productSelected:
							t.Append(GetProductSelected(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.mediaLevelDetail:
							t.Append(GetMediaLevelDetail(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.productLevelDetail:
							t.Append(GetProductLevelDetail(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.competitorMediaSelected:
							t.Append(GetCompetitorMediaSelected(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.insetSelected:
							t.Append(GetInsetSelected(_webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.newInMedia:
							t.Append(GetNewInMedia(_webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.comparativeStudy:
							t.Append(GetComparativeStudy(_webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.formatSelected:
							t.Append(GetFormatSelected(_webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.daySelected:
							t.Append(GetDaySelected(_webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.timeSlotSelected:
							t.Append(GetTimeSlotSelected(_webSession)); 
							break;
						case WebConstantes.DetailSelection.Type.targetSelected:
							t.Append(GetTargetSelected(_webSession)); 
							break;
                        case WebConstantes.DetailSelection.Type.genericColumnLevelDetail:
                            t.Append(GetGenericColumnLevelDetail(_webSession));
                            break;    
						case WebConstantes.DetailSelection.Type.genericMediaLevelDetail:
							t.Append(GetGenericMediaLevelDetail(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.sloganSelected:
							t.Append(GetSloganSelected(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.genericProductLevelDetail:
							t.Append(GetGenericProductLevelDetail(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.sponsorshipFormSelected:
							t.Append(GetSponsorshipFormSelected(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.programTypeSelected:
							t.Append(GetProgramTypeSelected(_webSession));
							break;
						case WebConstantes.DetailSelection.Type.percentageAlignmentSelected :
							t.Append(GetPercentageAlignment(_webSession));
							break;
						default:
							break;
					}
				}
				t.Append(GetBlankLine());
				t.Append("</table><br>");

				// On libère htmodule pour pouvoir le sauvegarder dans les tendances
				//_webSession.CustomerLogin.HtModulesList.Clear();
				
				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.ExcelWebPageException("Impossible de construire le rappel des paramètres dans le fichier Excel",err)); 
			}
		}
        #endregion

		#region GetLogo
		/// <summary>
		/// Genere le logo TNS pour les export excel
		/// </summary>
		/// <returns></returns>
		public string GetLogo(WebSession webSession) {
			
			StringBuilder t = new System.Text.StringBuilder();
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

			t.Append("<table cellpadding=0 cellspacing=0 width=100% >");
            t.Append("<tr><td><img src=\"/App_Themes/" + themeName + WebConstantes.Images.LOGO_TNS + "\"></td>");
			switch (webSession.CurrentModule) {
				case WebConstantes.Module.Name.DONNEES_DE_CADRAGE:
				case WebConstantes.Module.Name.BILAN_CAMPAGNE:								
					t.Append("<td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>");
					t.Append("<td><img src=\"/App_Themes/" + themeName + WebConstantes.Images.LOGO_APPM + "\"></td></tr>");
					t.Append("<tr><td colspan=8>&nbsp;</td></tr>");
					break;
				default: 
					t.Append("</tr>");
					t.Append(GetBlankLine());
					break;
			}			
			t.Append("</table><br>");
			
			return Convertion.ToHtmlString(t.ToString());
		}
		#endregion

		#region GetFooter
		/// <summary>
		/// Génère le pied de page html pour les exports Excel
		/// </summary>
		/// <returns>HTML</returns>
		public string GetFooter() {
			StringBuilder t = new System.Text.StringBuilder();

			#region CopyRight TNS
			t.Append("<table cellpadding=0 cellspacing=0 width=100% >");
			t.Append(GetBlankLine());
			t.Append("<tr><td " + cssTitleData + "> " + GestionWeb.GetWebWord(2266, _webSession.SiteLanguage) + "  " + DateTime.Now.Year.ToString() + "</td></tr>");
			t.Append(GetBlankLine());
			t.Append("</table><br>");
			#endregion

			return Convertion.ToHtmlString(t.ToString());
		}
		#endregion

		#region Méthodes internes d'affichage par rapport à la déclaration dans XML

		#region Nom du module
		/// <summary>
		/// Nom du module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetModuleName(WebSession webSession){
			StringBuilder html = new StringBuilder();
			html.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1859,webSession.SiteLanguage)+" :</font> ");
			html.Append(GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSession.CurrentModule),webSession.SiteLanguage));
			html.Append("</td></tr>");
			return(html.ToString());
		}
		#endregion

		#region Nom du résultat
		/// <summary>
		/// Nom du résultat
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="currentModule">Module</param>
		/// <returns>HTML</returns>
		private string GetResultName(WebSession webSession, Module currentModule){
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
				html.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(793,webSession.SiteLanguage) +" :</font> "+ currentResult +"</td></tr>");
			}
			return(html.ToString());
		}
		#endregion

		#region Dates sélectionnées
		/// <summary>
		/// Dates sélectionnées
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="currentModule">Module en cours</param>
		/// <param name="dateFormatText">Booléen date en format texte</param>
		/// <param name="periodBeginning">Date de début</param>
		/// <param name="periodEnd">Date de fin</param>
		/// <returns>HTML</returns>
		/// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
		protected virtual string GetDateSelected(WebSession webSession, Module currentModule, bool dateFormatText, string periodBeginning, string periodEnd){
			StringBuilder html = new StringBuilder();
			string startDate="";
			string endDate="";

			if( (currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE || currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO || currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION || currentModule.Id==WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO) && webSession.DetailPeriodBeginningDate.Length>0 && webSession.DetailPeriodBeginningDate!="0" && webSession.DetailPeriodEndDate.Length>0 && webSession.DetailPeriodEndDate!="0"){
				// Affichage de la période mensuelle si elle est sélectionné dans les options de résultat
				startDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.DetailPeriodBeginningDate);
				endDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.DetailPeriodEndDate);				
				
				html.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+ startDate);
				if(!startDate.Equals(endDate))
					html.Append(" - "+ endDate);
				html.Append("</td></tr>");
			}
			else{
				if(dateFormatText){
					startDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.PeriodBeginningDate);
					endDate=WebFunctions.Dates.getPeriodTxt(webSession,webSession.PeriodEndDate);			
				
					html.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+startDate);
					if(!startDate.Equals(endDate))
						html.Append(" - "+endDate);
					html.Append("</td></tr>");
				}
				else{
					if(periodBeginning.Length==0||periodEnd.Length==0){
						startDate=WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage);
						endDate=WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage);
					
						html.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+startDate);
						if(!startDate.Equals(endDate))
							html.Append(" - "+endDate);
						html.Append("</td></tr>");
					}
					else{
						// Predefined date
						startDate=WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(periodBeginning,webSession.PeriodType),webSession.SiteLanguage);
						endDate=WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(periodEnd,webSession.PeriodType),webSession.SiteLanguage);
					
						html.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" :</font> "+startDate);
						if(!startDate.Equals(endDate))
							html.Append(" - "+endDate);
						html.Append("</td></tr>");
					}
				}
			}
			return(html.ToString());
		}
		#endregion

        #region Période de l'étude
        /// <summary>
        /// Generate html code for study period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetStudyDate(WebSession webSession) {
         
                StringBuilder html = new StringBuilder();
                string startDate;
                string endDate;

				startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.StartDate.ToString(), webSession.SiteLanguage);
				endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.EndDate.ToString(), webSession.SiteLanguage);

                html.Append("<tr><td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2291, webSession.SiteLanguage) + " </font> " + startDate);
                if (!startDate.Equals(endDate))
                    html.Append(" - " + endDate);
                html.Append("</td></tr>");

                return (html.ToString());

        }
        #endregion

        #region Péiode comparative
        /// <summary>
        /// Generate html code for comparative period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetComparativeDate(WebSession webSession) {

                StringBuilder html = new StringBuilder();
                string startDate;
                string endDate;

				startDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeStartDate.ToString(), webSession.SiteLanguage);
				endDate = WebFunctions.Dates.YYYYMMDDToDD_MM_YYYY(webSession.CustomerPeriodSelected.ComparativeEndDate.ToString(), webSession.SiteLanguage);

                html.Append("<tr><td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2292, webSession.SiteLanguage) + " </font> " + startDate);
                if (!startDate.Equals(endDate))
                    html.Append(" - " + endDate);
                html.Append("</td></tr>");

                return (html.ToString());

        }
        #endregion

        #region Type de la période comparative
        /// <summary>
        /// Generate html code for comparative period detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetComparativePeriodTypeDetail(WebSession webSession) {

            switch (webSession.CustomerPeriodSelected.ComparativePeriodType) {
                case WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate:
                    return ("<tr><td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2293, webSession.SiteLanguage) + "</font> " + GestionWeb.GetWebWord(2295, webSession.SiteLanguage) + "</td></tr>");
                case WebConstantes.globalCalendar.comparativePeriodType.dateToDate:
                    return ("<tr><td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2293, webSession.SiteLanguage) + "</font> " + GestionWeb.GetWebWord(2294, webSession.SiteLanguage) + "</td></tr>");
                default:
                    return "";
            }

        }
        #endregion

        #region Type de la disponibilité des données
        /// <summary>
        /// Generate html code for period disponibility type detail
        /// </summary>
        /// <param name="webSession">User Session</param>
        /// <returns>Html code</returns>
        private string GetPeriodDisponibilityTypeDetail(WebSession webSession) {

            switch (webSession.CustomerPeriodSelected.PeriodDisponibilityType) {
                case WebConstantes.globalCalendar.periodDisponibilityType.currentDay:
                    return ("<tr><td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2296, webSession.SiteLanguage) + "</font> " + GestionWeb.GetWebWord(2297, webSession.SiteLanguage) + "</td></tr>");
                case WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod:
                    return ("<tr><td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2296, webSession.SiteLanguage) + "</font> " + GestionWeb.GetWebWord(2298, webSession.SiteLanguage) + "</td></tr>");
                default:
                    return "";
            }
        }
        #endregion

        #region Unité
        /// <summary>
		/// Unité
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetUnitSelected(WebSession webSession){
			if(webSession.PreformatedTable.ToString().ToUpper().IndexOf("UNITS")==-1)
				return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1313,webSession.SiteLanguage)+"</font> "+Convertion.ToHtmlString(GestionWeb.GetWebWord(webSession.GetSelectedUnit().WebTextId,webSession.SiteLanguage))+"</td></tr>");
			return("");
		}
		#endregion

		#region Type de pourcentage
		/// <summary>
		/// Type Pourcentage
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetPercentageAlignment(WebSession webSession){
			switch(webSession.PercentageAlignment){
				case WebConstantes.Percentage.Alignment.vertical :
					return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1236,webSession.SiteLanguage)+" : </font> "+GestionWeb.GetWebWord(2065,webSession.SiteLanguage)+"</td></tr>");					
				case WebConstantes.Percentage.Alignment.horizontal :
					return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1236,webSession.SiteLanguage)+" : </font> "+GestionWeb.GetWebWord(2064,webSession.SiteLanguage)+"</td></tr>");				
				default :
					return("");
			}			
		}
		#endregion

		#region Media sélectionné
		/// <summary>
		/// Media sélectionné
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetVehicleSelected(WebSession webSession){
			StringBuilder html = new StringBuilder();
			if (webSession.isMediaSelected()){
				html.Append(GetBlankLine());
				html.Append("<tr><td "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(190,webSession.SiteLanguage)+" :</font></td></tr>");
				html.Append(ToExcel(webSession.SelectionUniversMedia,cssLevel1,cssLevel2,cssLevel3));
			}
			return(html.ToString());
		}
		#endregion

		#region Produit sélectionné
		/// <summary>
		/// Produits sélectionnés
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetProductSelected(WebSession webSession){
			StringBuilder t = new StringBuilder();
			int idAdvertiser = 1;

			#region Liste des supports de référence
			// Alerte et Analyse portefeuille
		
			if(webSession.isReferenceMediaSelected()){
				t.Append(GetBlankLine());
				t.Append("<TR><TD colspan=4 " + cssTitleData + " ><font " + cssTitle + ">&nbsp;" + GestionWeb.GetWebWord(971, webSession.SiteLanguage) + " :</font></TD></TR>");
				t.Append(ToExcel((System.Windows.Forms.TreeNode)webSession.ReferenceUniversMedia,cssLevel1,cssLevel2,cssLevel3));
			}
			#endregion

			AdExpressUniverse adExpressUniverse = null;
			#region selection produit
			if (webSession.PrincipalProductUniverses.Count ==1) {
				t.Append(GetBlankLine()); 
				t.Append("<TR><TD colspan=4 " + cssTitleData + " ><font " + cssTitle + ">" + GestionWeb.GetWebWord(1759, webSession.SiteLanguage) + " :</font></TD></TR>");
				adExpressUniverse = webSession.PrincipalProductUniverses[0];
                //if (webSession.CustomerLogin.Connection == null) {
                //    TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(webSession.CustomerLogin.OracleConnectionString));
                //    webSession.CustomerLogin.Connection = (OracleConnection)dataSource.GetSource();
                //}
				t.Append(ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.Source));//TNS.AdExpress.Web.Functions.DisplayUniverse
			}
			else if (webSession.PrincipalProductUniverses.Count > 1) {
				
                //if (webSession.CustomerLogin.Connection == null) {
                //    TNS.FrameWork.DB.Common.IDataSource dataSource = new TNS.FrameWork.DB.Common.OracleDataSource(new OracleConnection(webSession.CustomerLogin.OracleConnectionString));
                //    webSession.CustomerLogin.Connection = (OracleConnection)dataSource.GetSource();
                //}
				for (int k = 0; k < webSession.PrincipalProductUniverses.Count; k++) {
					if(webSession.PrincipalProductUniverses.ContainsKey(k)){
						t.Append(GetBlankLine());
						t.Append("<TR><TD colspan=4 " + cssTitleData + " ><font " + cssTitle + ">" + GestionWeb.GetWebWord(1759, webSession.SiteLanguage) + " :</font></TD></TR>");
                        t.Append("<TR><TD colspan=4 class=\"txtViolet11Bold whiteBackGround\" ><font>" + webSession.PrincipalProductUniverses[k].Label + " </font></TD></TR>");
						adExpressUniverse = webSession.PrincipalProductUniverses[k];
						t.Append(ToExcel(adExpressUniverse, webSession.SiteLanguage, webSession.Source));
					}
				}
			}
			#endregion	

			#region Liste d'annonceurs / Liste des produits en affinage (Inactivated by Dédé 06/12/2007)
			//// Alerte et Analyse Plan média
			//// Alerte et Analyse Concurrentielle et Potentiel pour 'AFFINER'
			//// Alerte Portefeuille pour 'AFFINER'

			//if (webSession.isAdvertisersSelected() && !webSession.isCompetitorAdvertiserSelected()){
			//    t.Append(GetBlankLine());
			//    if(((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess  ||	((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(814,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess ||	((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(813,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess ||	((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(1585,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess ||	((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(815,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess ||	((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(965,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess ||	((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(966,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess ||	((LevelInformation)webSession.SelectionUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(964,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    // Affichage du System.Windows.Forms.TreeNode
			//    t.Append(ToExcel(webSession.CurrentUniversAdvertiser,cssLevel1,cssLevel2,cssLevel3));
			//}
			#endregion

			#region Liste de produits (Inactivated by Dédé 06/12/2007)
			//// Indicateurs et tableaux dynamiques
			//// Tableaux de bord

			//if (webSession.isSelectionProductSelected()){
			//    t.Append(GetBlankLine());
			//    if(((LevelInformation)webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess ||	((LevelInformation)webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(815,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess ||	((LevelInformation)webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(965,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess ||	((LevelInformation)webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(966,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess ||	((LevelInformation)webSession.SelectionUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(964,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    // Affichage du System.Windows.Forms.TreeNode
			//    t.Append(ToExcel(webSession.SelectionUniversProduct,cssLevel1,cssLevel2,cssLevel3));
				
			//    if(webSession.CurrentModule==195) {
			//        //t.Append("<tr height=\"20\">");
			//        //t.Append("<td colspan=4 vAlign=\"top\" >"+TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetExcelSectorsSelected(webSession)+"</td>");
			//        //t.Append("</tr>");

			//        t.Append(GetSectorsSelected());
			//    }
			//}
			//else if(IsSelectionProductSelected(webSession)){
			//    if(((LevelInformation)webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess ||	((LevelInformation)webSession.CurrentUniversProduct.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException) {
			//        t.Append(GetBlankLine());
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(965,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    // Affichage du System.Windows.Forms.TreeNode
			//    t.Append(ToExcel(webSession.CurrentUniversProduct,cssLevel1,cssLevel2,cssLevel3));
			//}
			#endregion

			#region Annonceurs de références (Inactivated by Dédé 06/12/2007)
			//// Indicateurs et tableaux dynamiques

			//if (webSession.isReferenceAdvertisersSelected()){
			//    t.Append(GetBlankLine());
			//    if(((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess ||	((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(814,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess||	((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(813,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess||	((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(1585,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess ||	((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(815,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess ||	((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(965,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess ||	((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(966,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    else if(((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupAccess ||	((LevelInformation)webSession.ReferenceUniversAdvertiser.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.groupException ) {
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(964,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE ){
			//        t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(1195,webSession.SiteLanguage)+"</font></td></tr>");
			//    }
			//    // Affichage du System.Windows.Forms.TreeNode
			//    t.Append(ToExcel(webSession.ReferenceUniversAdvertiser,cssLevel1,cssLevel2,cssLevel3));
			//}
			#endregion

			#region Annonceurs Concurrents (Inactivated by Dédé 06/12/2007)
			//// Alerte et Analyse Plan Média Concurrentiel
			//// Indicateurs et tableaux dynamiques
			//// Chronopresse

			//if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE ){
			//    idAdvertiser=0;
			//}
			//if(webSession.isCompetitorAdvertiserSelected()){
			//    while(webSession.CompetitorUniversAdvertiser[idAdvertiser]!=null){
			//        System.Windows.Forms.TreeNode tree=null;
			//        if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE ){
			//            tree=(System.Windows.Forms.TreeNode)webSession.CompetitorUniversAdvertiser[idAdvertiser];
			//        }
			//        else{
			//            tree=((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[idAdvertiser]).TreeCompetitorAdvertiser;
			//        }
			//        if(tree.FirstNode!=null){
			//            t.Append(GetBlankLine());
			//            if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess ||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException ) {
			//                t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(814,webSession.SiteLanguage)+"</font></td></tr>");
			//            }
			//            else if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException ) {
			//                t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(813,webSession.SiteLanguage)+"</font></td></tr>");
			//            }
			//            else if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException ) {
			//                t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(1585,webSession.SiteLanguage)+"</font></td></tr>");
			//            }
			//            else if(((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productAccess ||	((LevelInformation)tree.FirstNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.productException ) {
			//                t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(815,webSession.SiteLanguage)+"</font></td></tr>");
			//            }
			//            if(webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR || webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE ){
			//                t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(1196,webSession.SiteLanguage)+"</font></td></tr>");
			//            }
			//            if(webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR && webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE ){
			//                t.Append("<TR>");
			//                //t.Append("<TD colspan=4 class=\"txtViolet11Bold\" bgColor=\"#ffffff\">");
			//                t.Append("<TD colspan=4 "+cssTitleData+">");
			//                t.Append("<Label>"+(string)(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[idAdvertiser]).NameCompetitorAdvertiser)+"</Label>");
			//                t.Append("</TD></TR>");
			//            }
			//            // Affichage du System.Windows.Forms.TreeNode	
			//            if(webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR && webSession.CurrentModule!=TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE ){
			//                t.Append(ToExcel((System.Windows.Forms.TreeNode)(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[idAdvertiser]).TreeCompetitorAdvertiser),cssLevel1,cssLevel2,cssLevel3));
			//            }
			//            else{
			//                t.Append(ToExcel(tree,cssLevel1,cssLevel2,cssLevel3));
			//            }
			//            idAdvertiser++;
			//        }
			//        else{
			//            idAdvertiser++;
			//        }
			//    }
			//}
			#endregion

			return(t.ToString());
		}
		#endregion

		#region Niveau de détail support (Média détaillé par)
		/// <summary>
		/// Niveau de détail support (Média détaillé par)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetMediaLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1150,webSession.SiteLanguage)+"</font> "+WebFunctions.MediaDetailLevel.LevelMediaToExcelString(webSession)+"</td></tr>";
		}
		#endregion

		#region Niveau de détail produit (Produit détaillé par)
		/// <summary>
		/// Niveau de détail produit (Produit détaillé par)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetProductLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1124,webSession.SiteLanguage)+"</font> "+WebFunctions.ProductDetailLevel.LevelProductToExcelString(webSession)+"</td></tr>";
		}
		#endregion

		#region Niveau de détail support générique
		/// <summary>
		/// Niveau de détail support (Médias détaillés par :)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetGenericMediaLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1886,webSession.SiteLanguage)+"</font> "+ webSession.GenericMediaDetailLevel.GetLabel(webSession.SiteLanguage) +"</td></tr>";
		}
		#endregion

		#region Niveau de détail produit générique
		/// <summary>
		/// Niveau de détail produit (Produits détaillés par :)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetGenericProductLevelDetail(WebSession webSession){
			return"<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1886,webSession.SiteLanguage)+"</font> "+ webSession.GenericProductDetailLevel.GetLabel(webSession.SiteLanguage) +"</td></tr>";
		}
		#endregion

        #region Niveau de détail colonne générique
        /// <summary>
        /// Niveau de détail colonne
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>HTML</returns>
        private string GetGenericColumnLevelDetail(WebSession webSession) {
            return "<tr><td colspan=4 " + cssTitleData + "><font " + cssTitle + ">" + GestionWeb.GetWebWord(2300, webSession.SiteLanguage) + "</font> " + webSession.GenericColumnDetailLevel.GetLabel(webSession.SiteLanguage) + "</td></tr>";
        }
        #endregion

		#region Sélection encart
		/// <summary>
		/// Sélection encart
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetInsetSelected(WebSession webSession){
			//string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
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
				return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1400,webSession.SiteLanguage)+"</font> "+GestionWeb.GetWebWord(code,webSession.SiteLanguage)+"</td></tr>");
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
		private string GetNewInMedia(WebSession webSession){
			int code=0;
			switch(webSession.NewProduct){
				case WebConstantes.CustomerSessions.NewProduct.pige:
					code=1421;
					break;
				case WebConstantes.CustomerSessions.NewProduct.support:
					code=1422;
					break;
			}
			return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1449,webSession.SiteLanguage)+" "+GestionWeb.GetWebWord(code,webSession.SiteLanguage)+"</font></td></tr>");
		}
		#endregion

		#region Support sélectionnés pour les concurrents
		// Alerte et Analyse concurrentielle
		// Alerte et Analyse de potentielle
		// Analyse dynamique

		/// <summary>
		/// Support sélectionnés pour les concurrents
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetCompetitorMediaSelected(WebSession webSession){
			StringBuilder t = new StringBuilder();
			if(webSession.isCompetitorMediaSelected()){
				int idMedia=1;
				t.Append(GetBlankLine());
				t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(1087,webSession.SiteLanguage) +"</font></td></tr>");
				while((System.Windows.Forms.TreeNode)webSession.CompetitorUniversMedia[idMedia]!=null){
					System.Windows.Forms.TreeNode tree=(System.Windows.Forms.TreeNode)webSession.CompetitorUniversMedia[idMedia];				
					t.Append(ToExcel(((System.Windows.Forms.TreeNode)webSession.CompetitorUniversMedia[idMedia]),cssLevel1,cssLevel2,cssLevel3));
					t.Append(GetBlankLine());
					idMedia++;
				}
			}
			return(t.ToString());
		}
		#endregion

		#region Genre émissions\ émissions
		/// <summary>
		/// Genre émissions\ émissions sélectionnés pour le parrainage
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetProgramTypeSelected(WebSession webSession){
			StringBuilder t = new StringBuilder();
			if(webSession.IsCurrentUniversProgramTypeSelected()){				
				t.Append(GetBlankLine());
				t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(2066,webSession.SiteLanguage) +"</font></td></tr>");
				t.Append(ToExcel(((System.Windows.Forms.TreeNode)webSession.CurrentUniversProgramType),cssLevel1,cssLevel2,cssLevel3));
				t.Append(GetBlankLine());
			}
			return(t.ToString());
		}
		#endregion

		#region Formes de parrainnage
		
		/// <summary>
		/// Formes de parrainnage sélectionnés pour le parrainage
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetSponsorshipFormSelected(WebSession webSession){
			StringBuilder t = new StringBuilder();
			if(webSession.IsCurrentUniversSponsorshipFormSelected()){
				
				t.Append(GetBlankLine());
				t.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+ GestionWeb.GetWebWord(2067,webSession.SiteLanguage) +"</font></td></tr>");
				t.Append(ToExcel(((System.Windows.Forms.TreeNode)webSession.CurrentUniversSponsorshipForm),cssLevel1,cssLevel2,cssLevel3));
				t.Append(GetBlankLine());
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
		private string GetComparativeStudy(WebSession webSession){
			if(webSession.ComparativeStudy)
				return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1118,webSession.SiteLanguage)+"</font></td></tr>");
			return("");
		}
		#endregion

		#region Format sélectionné
		/// <summary>
		/// Format sélectionné
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetFormatSelected(WebSession webSession){
			return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1420,webSession.SiteLanguage)+" :</font> "+ TNS.AdExpress.Web.Functions.Dates.GetFormat(webSession,webSession.Format) +"</td></tr>");
		}
		#endregion

		#region Jour nommé sélectionné
		/// <summary>
		/// Jour nommé sélectionné
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>HTML</returns>
		private string GetDaySelected(WebSession webSession){
			string namedDay=string.Empty;
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
            string[] dayNames = cultureInfo.DateTimeFormat.DayNames;
			switch(webSession.NamedDay){
				case WebConstantes.Repartition.namedDay.Week_5_day:
					namedDay=GestionWeb.GetWebWord(1553, webSession.SiteLanguage);
					break;
				case WebConstantes.Repartition.namedDay.Total:
					namedDay=GestionWeb.GetWebWord(848, webSession.SiteLanguage);
					break;
				case WebConstantes.Repartition.namedDay.Week_end:
					namedDay=GestionWeb.GetWebWord(1561, webSession.SiteLanguage);
					break;
                default:
                    if (webSession.NamedDay.GetHashCode() == 7)
                        namedDay = cultureInfo.TextInfo.ToTitleCase(dayNames[0]);
                    else if (webSession.NamedDay.GetHashCode() >= 0 && webSession.NamedDay.GetHashCode() < 7)
                        namedDay = cultureInfo.TextInfo.ToTitleCase(dayNames[webSession.NamedDay.GetHashCode()]);
                    break;
			}
			if(namedDay.Length>0){
				return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1574,webSession.SiteLanguage)+" :</font> "+ namedDay +"</td></tr>");
			}
			return("");
		}
		#endregion

		#region Tranche horaire sélectionnée
		/// <summary>
		/// Tranche horaire sélectionnée
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>HTML</returns>
		private string GetTimeSlotSelected(WebSession webSession){
			return("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1575,webSession.SiteLanguage)+" :</font> "+ TNS.AdExpress.Web.Functions.Dates.GetTimeSlice(webSession,webSession.TimeInterval) +"</td></tr>");
		}
		#endregion

		#region Wave et Cible sélectionnées
		/// <summary>
		/// Tranche horaire sélectionnée
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>HTML</returns>
		private string GetTargetSelected(WebSession webSession){
			StringBuilder html = new StringBuilder();
			if (webSession.IsWaveSelected()){
				if(((LevelInformation)webSession.SelectionUniversAEPMWave.FirstNode.Tag).Text.Length>0){
					html.Append(GetBlankLine());
					html.Append("<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1762,webSession.SiteLanguage)+"</font> "+((LevelInformation) webSession.SelectionUniversAEPMWave.FirstNode.Tag).Text+"</td></tr>");
				}
			}
			if(!(webSession.CurrentTab==APPM.affinities)){									
				if (webSession.IsTargetSelected()){
					if(((LevelInformation)webSession.SelectionUniversAEPMTarget.LastNode.Tag).Text.Length>0){
						html.Append("<tr height=\"20\"><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1763,webSession.SiteLanguage)+"</font></td></tr>");
						// Affichage du System.Windows.Forms.TreeNode
						html.Append(ToExcel(webSession.SelectionUniversAEPMTarget,cssLevel1,cssLevel2,cssLevel3));
					}
				}
			}
			return(html.ToString());
		}
		#endregion

		#region Ligne séparatrice
		/// <summary>
		/// Ligne séparatrice
		/// </summary>
		/// <returns>HTML</returns>
		private static string GetBlankLine(){
			return("<tr><td>&nbsp;</td></tr>");
		}
		#endregion

		#region Version sélectionnée
		/// <summary>
		/// Version sélectionnée
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>HTML</returns>
		private string GetSloganSelected(WebSession webSession){
			string tmp="";
			int nbCol=0;
			if(webSession.SloganIdZoom>0){
				tmp+="<tr><td colspan=4 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1888,webSession.SiteLanguage)+" :</font> "+webSession.SloganIdZoom.ToString()+"</td></tr>";
				return(tmp);
			}
			else{
				if(webSession.IdSlogans!=null && webSession.IdSlogans.Count > 0){
					tmp+="<tr><td colspan=1 "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1888,webSession.SiteLanguage)+" :</font> </td>";
					foreach(Int64 currentSlogan in webSession.IdSlogans){
						if(nbCol>2){
							tmp+="</tr><tr><td>&nbsp;</td>";
							nbCol=0;
						}
						tmp+="<td "+cssTitleData+">"+currentSlogan.ToString()+"</td>";
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

		#region Méthodes internes d'affichage des TreeNodes
		/// <summary>
		/// Affichage d'un arbre pour l'export Excel
		/// </summary>
		/// <param name="root">Arbre</param>
		/// <param name="cssLevel1">Style CSS du niveau 1</param>
		/// <param name="cssLevel2">Style CSS du niveau 2</param>
		/// <param name="cssLevel3">Style CSS du niveau 3</param>
		/// <returns>Code HTML</returns>
        public string ToExcel(System.Windows.Forms.TreeNode root, string cssLevel1, string cssLevel2, string cssLevel3)
        {
			int maxLevel=0;
			GetNbLevels(root,1,ref maxLevel);
			StringBuilder html = new StringBuilder();
			int nbTD=1;
			ToExcel(root,ref html,0,maxLevel-1,ref nbTD);
			return(html.ToString());
		}

		/// <summary>
		/// Donne le nombre de niveau d'un arbre
		/// </summary>
		/// <param name="root">Arbre</param>
		/// <param name="level">Niveau de l'arbre</param>
		/// <param name="max">Nombre maximum de niveau</param>
        private void GetNbLevels(System.Windows.Forms.TreeNode root, int level, ref int max)
        {
			if(max<level)max=level;
			foreach(System.Windows.Forms.TreeNode currentNode in root.Nodes){
				GetNbLevels(currentNode,level+1,ref max);
			}
		}

		/// <summary>
		/// Donne le nom du style CSS selon le niveau de l'arbre
		/// </summary>
		/// <param name="level">Niveau de l'arbre</param>
		/// <returns>Nom du style CSS</returns>
		private string GetLevelCss(int level){
			switch(level){
				case 1:
					return(cssLevel1);
				case 2:
					return(cssLevel2);
				case 3:
					return(cssLevel3);
				default:
					return(cssLevel1);
			}
		}

        /// <summary>
        /// Donne le nom du style CSS selon le niveau de l'arbre
        /// </summary>
        /// <param name="level">Niveau de l'arbre</param>
        /// <returns>Nom du style CSS</returns>
        private string GetRightBorderLevelCss(int level) {
            switch (level) {
                case 1:
                    return (cssRightBorderLevel1);
                case 2:
                    return (cssRightBorderLevel2);
                case 3:
                    return (cssRightBorderLevel3);
                default:
                    return (cssRightBorderLevel1);
            }
        }

        /// <summary>
        /// Donne le nom du style CSS selon le niveau de l'arbre
        /// </summary>
        /// <param name="level">Niveau de l'arbre</param>
        /// <returns>Nom du style CSS</returns>
        private string GetRightBottomBorderLevelCss(int level) {
            switch (level) {
                case 1:
                    return (cssRightBottomBorderLevel1);
                case 2:
                    return (cssRightBottomBorderLevel2);
                case 3:
                    return (cssRightBottomBorderLevel3);
                default:
                    return (cssRightBottomBorderLevel1);
            }
        }

		/// <summary>
		/// Affichage d'un arbre pour l'export Excel
		/// </summary>
		/// <param name="root">Arbre</param>
		/// <param name="html">Code html</param>
		/// <param name="level">Niveau de l'arbre</param>
		/// <param name="maxLevel">Nombre maximum de niveaud e l'arbre</param>
		/// <param name="nbTD">Nombre de cellule TD</param>
		/// <returns>True si le nombre maximum de TD a été atteint, sinon false</returns>
		/// <remarks>
		/// - Actuellement, la méthode gère 3 niveaux d'affichage mais elle est générique.
		/// Par conséquent, 3 styles sont définis. Il est possible de rajouter des niveaux de style CSS dans le 'switch case' correspondant
		/// dans la méthode ci-après et ajouter les niveaux dans la méthode GetLevelCss(int level)
		/// - Affichage sur 3 colonnes dans le dernier niveau
		/// </remarks>
        private bool ToExcel(System.Windows.Forms.TreeNode root, ref StringBuilder html, int level, int maxLevel, ref int nbTD)
        {
            #region Variables
            string img = "";
            string rightBorder = string.Empty;
            string rightBottomBorder = string.Empty;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            #endregion

			#region Border Color
			switch(OutputType){
				case RenderType.html:
                    rightBorder = "violetRightBorder";
                    rightBottomBorder = "violetRightBottomBorder";
					break;
				case RenderType.excel:
				case RenderType.rawExcel:
                    rightBorder = "greyRightBorder";
                    rightBottomBorder = "greyRightBottomBorder";
					break;
			}
			#endregion

			#region Checkbox
			// Non cocher
			if(!root.Checked) img="<img src=/App_Themes/"+themeName+"/Images/Common/checkbox_not_checked.GIF>";
				// Cocher
			else if(root.Checked) img="<img src=/App_Themes/"+themeName+"/Images/Common/checkbox.GIF>";
			#endregion

			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
				html.Append("<td "+GetLevelCss(level)+" >"+img+"&nbsp;&nbsp;&nbsp;&nbsp;"+((LevelInformation)root.Tag).Text+"</td>");	
			}
			else{
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
				if(level!=0)html.Append("<tr \""+cssBorderLevel+"\"><td colspan=4 "+GetLevelCss(level)+" >"+img+"&nbsp;&nbsp;&nbsp;&nbsp;"+((LevelInformation)root.Tag).Text+"</td></tr>");
				// On prépare l'affichage du dernier niveau de l'arbre (nouvelle ligne) si on affiche le père d'une feuille
				if(level==maxLevel-1 && root.Nodes.Count>0)
					html.Append("<tr>");
			}
			// Boucle sur chaque noeud de l'arbre
			foreach(System.Windows.Forms.TreeNode currentNode in root.Nodes){
				// Si le niveau inférieur indique qu'il faut changer de ligne et que la demande n'a pas été faite par le dernier fils
				if(ToExcel(currentNode,ref html,level+1,maxLevel,ref nbTD) && currentNode!=root.LastNode){
					html.Append("</tr><tr>");
				}
			}
			//On est dans le niveau père des feuilles et il a des fils on fait les bordures
			if(level==maxLevel-1 && root.Nodes.Count>0){
				if(nbTD!=1){
					// On ajoute des cellules vides (td avec colspan)pour avoir le bon nombre de cellules (TD)
					html.Append("<td "+GetLevelCss(level+1)+" colspan="+(((int)(4-nbTD)).ToString())+">&nbsp;</td>");
					// Bordure de droite
					html.Append("<td "+GetRightBorderLevelCss(level+1)+" >&nbsp;</td></tr>");
				}
				// Bordure en bas et bordure à droite
				html.Append("<tr><td colspan=4 "+GetRightBottomBorderLevelCss(level+1)+">&nbsp;</td></tr>");
				nbTD=1;
			}
			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// On test si on est dans la dernière colonne, 
				// On affiche une cellule vide pour faire la bordure de droite
				// On prépare le changement de ligne commander au niveau supperieur par le return true
				if(nbTD==3){
					nbTD=1;
					html.Append("<td "+GetRightBorderLevelCss(level)+">&nbsp;</td></tr>");
					return(true);
				}
				nbTD++;
			}
			// On indique au niveau suppérieur que l'on ne doit pas changer de ligne
			return(false);
		}
		#endregion

		#region Autres méthodes internes

		#region Vérifie si produits sélectionnés pour les tableaux de bords
		/// <summary>
		/// Vérifie si produits sélectionnés pour les tableaux de bords
		/// </summary>
		/// <returns>Vrai si produis sélectionnés</returns>
		private static bool IsSelectionProductSelected(WebSession webSession){
			switch(webSession.CurrentModule){
				case WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE :
				case WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO :
				case WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION:
				case WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO:
					if(webSession.CurrentUniversProduct!=null && webSession.CurrentUniversProduct.Nodes.Count>0)
						return true;
					else 
						return false;
				default : return false;
			}
		}
		#endregion

		#region Familles sélectionnées
		/// <summary>
		/// Familles sélectionnées
		/// </summary>
		/// <returns>HTML</returns>
		private string GetSectorsSelected() {
			// Indicateurs et tableaux dynamiques
			// Tableaux de bord

			DataSet ds=TNS.AdExpress.Web.DataAccess.Selections.Products.SectorsSelectedDataAccess.getData(_webSession);
			if(ds==null)return "";
			StringBuilder html = new StringBuilder(2000);
			DataTable sectors = ds.Tables[0];

			html.Append(GetBlankLine());
			html.Append("<tr><td "+cssTitleData+"><font "+cssTitle+">"+GestionWeb.GetWebWord(1601,_webSession.SiteLanguage)+" :</font></td></tr>");

			foreach(DataRow dr in sectors.Rows){
				html.Append("<tr><td "+cssTitleData+">"+dr["sector"].ToString()+"</td></tr>");
			}
			return html.ToString();
		}
		#endregion

		#endregion

		#region Méthodes internes nouveaux univers produit
		/// <summary>
		/// Get Html render to show universe selection into excel file
		/// </summary>
		/// <param name="adExpressUniverse">adExpress Universe</param>
		/// <param name="language">language</param>
		/// <param name="connection">DB connection</param>
		/// <returns>Html render to show universe selection</returns>
		private string ToExcel(AdExpressUniverse adExpressUniverse, int language, TNS.FrameWork.DB.Common.IDataSource source) {
		
			#region Variables
			StringBuilder html = new StringBuilder();
			List<NomenclatureElementsGroup> groups = null;
			int baseColSpan = 4;
			#endregion

			//Groups of items excludes
			groups = adExpressUniverse.GetExludes();
            html.Append(GetUniverseGroupForExcel(groups,baseColSpan,language,source,AccessType.excludes));

			//Groups of items includes
			groups = adExpressUniverse.GetIncludes();
            html.Append(GetUniverseGroupForExcel(groups,baseColSpan,language,source,AccessType.includes));


			return html.ToString();
		}

		/// <summary>
		/// Get Html render to show universe selection into excel file
		/// </summary>
		/// <param name="groups">universe groups</param>
		/// <param name="baseColSpan">base column span</param>
		/// <param name="language">language</param>
		/// <param name="connection">DB connection</param>
		/// <param name="accessType">items access type</param>
		/// <returns>Html render to show universe selection</returns>
		private  string GetUniverseGroupForExcel(List<NomenclatureElementsGroup> groups, int baseColSpan, int language, TNS.FrameWork.DB.Common.IDataSource source, AccessType accessType) {

            #region Variables
            int level = 1;
            ArrayList itemIdList = null;
            bool lineClosed = false;
            int colSpan = 0;
            string rightBorder = string.Empty;
            string rightBottomBorder = string.Empty;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            string img = "<img src=/App_Themes/" + themeName + "/Images/Common/checkbox.GIF>";
            TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess universeItems = null;
            int code = 0;
            StringBuilder html = new StringBuilder();
            #endregion

			#region Border Color
			switch (OutputType) {
				case RenderType.html:
                    rightBorder = "violetRightBorder";
                    rightBottomBorder = "violetRightBottomBorder";
					break;
				case RenderType.excel:
				case RenderType.rawExcel:
                    rightBorder = "greyRightBorder";
                    rightBottomBorder = "greyRightBottomBorder";
					break;
			}
			#endregion

			if (accessType == AccessType.includes) code = 2281;
			else code = 2282;


			if (groups != null && groups.Count > 0) {


				for (int i = 0; i < groups.Count; i++) {
					List<long> levelIdsList = groups[i].GetLevelIdsList();

					html.Append(GetBlankLine());
					if (i > 0 && accessType == AccessType.includes) code = 2368;
					html.Append("<tr class=\"excelData\"><td colspan=" + baseColSpan + "  ><font class=txtBoldGrisExcel>" + GestionWeb.GetWebWord(code, language) + "&nbsp; : </font></td></tr>");

					//For each group's level
					if (levelIdsList != null) {

						for (int j = 0; j < levelIdsList.Count; j++) {

							//Level label
							level = 1;
							
							html.Append("<tr class=\"BorderLevel\"><td colspan=" + baseColSpan + "  " + GetLevelCss(level) + " >" + GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language) + " </td></tr>");

							//Show items of the current level
							level = 2;
							html.Append("<tr>");
                            universeItems = new TNS.AdExpress.DataAccess.Classification.ClassificationLevelListDataAccess(UniverseLevels.Get(levelIdsList[j]).TableName,groups[i].GetAsString(levelIdsList[j]),_webSession.DataLanguage,source);
							if (universeItems != null) {
								itemIdList = universeItems.IdListOrderByClassificationItem;
								if (itemIdList != null && itemIdList.Count > 0) {
									for (int k = 0; k < itemIdList.Count; k++) {

										//Compute colspan of current cell
										if ((itemIdList.Count == (k + 1)) && (itemIdList.Count % (baseColSpan - 1)) > 0) colSpan = ((baseColSpan - 1) - (itemIdList.Count % (baseColSpan - 1))) + 1;

										//Current item label
										lineClosed = false;
										html.Append("<td  " + GetLevelCss(level) + " colspan=" + colSpan + " >" + img + "&nbsp;&nbsp;&nbsp;&nbsp;" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</td>");
										if (k > 0 && ((k + 1) % (baseColSpan - 1)) == 0) {
											lineClosed = true;
											html.Append("<td  " + GetRightBorderLevelCss(level) + ">&nbsp;</td></tr>");//Items are showed on three columns
										}
										colSpan = 0;
									}
									if (!lineClosed) {
										html.Append("<td  " + GetRightBorderLevelCss(level) + ">&nbsp;</td></tr>");
									}
								}

								// Bordure en bas et bordure à droite
								html.Append("<tr><td colspan=" + baseColSpan + "  " + GetRightBottomBorderLevelCss(level) + ">&nbsp;</td></tr>");
							}
							html.Append("</tr>");
						}
					}
				}
			}

			return html.ToString();
		}

		#endregion

	}
}
