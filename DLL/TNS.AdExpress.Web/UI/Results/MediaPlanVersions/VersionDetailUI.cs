#region Informations
 // Author : G Facon / G Ragneau
 // Creation : 13/07/2006
 // Modification :
 //		Author - Date - description
 //     Y. Rkaina - 18 août 2006 - Ajout de la méthode GetHtmlExport(StringBuilder output)
#endregion

using System;
using System.Text;

using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork.Date;
using TNS.FrameWork;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using CstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.UI.Results.MediaPlanVersions{

	/// <summary>
	/// VersionDetailUI provides process methods to get html code to display a version
	/// </summary>
	public class VersionDetailUI{

		#region Constantes

		#endregion

		#region Variables
		/// <summary>
		/// Objet session
		/// </summary>
		protected WebSession _webSession=null;
		///<summary>Version to display</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		protected VersionItem _version;
		///<summary>Version details to display</summary>
		/// <author>rkaina</author>
		/// <since>lundi 21 août 2006</since>
		protected ExportVersionItem _exportVersion;
		///<summary>APPM Version details to display</summary>
		/// <author>rkaina</author>
		/// <since>lundi 29 août 2006</since>
		protected ExportAPPMVersionItem _exportAPPMVersion;
        /// <summary>
        /// MD Version to display
        /// </summary>
        protected ExportMDVersionItem _exportMDVersion;
        /// <summary>
        /// Outdoor Version to display
        /// </summary>
        protected ExportOutdoorVersionItem _exportOutdoorVersion;
		#endregion

		#region Accessors
		///<summary>Get / Set Version</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public VersionItem Version {
    		get {
        		return (_version);
    		}
    		set {
        		_version = value;
    		}
		}
		///<summary>Get / Set WebSession</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public WebSession Session {
			get {
				return (_webSession);
			}
			set {
				_webSession = value;
			}
		}
		///<summary>Get / Set Export Version</summary>
		/// <author>rkaina</author>
		/// <since>Lundi 21 août 2006</since>
		public ExportVersionItem ExportVersion {
			get {
				return (_exportVersion);
			}
			set {
				_exportVersion = value;
			}
		}
		///<summary>Get / Set Export Version for APPM</summary>
		/// <author>rkaina</author>
		/// <since>Lundi 29 août 2006</since>
		public ExportAPPMVersionItem ExportAPPMVersion {
			get {
				return (_exportAPPMVersion);
			}
			set {
				_exportAPPMVersion = value;
			}
		}
        ///<summary>Get / Set Export Version for MD</summary>
        public ExportMDVersionItem ExportMDVersion{
            get {
                return (_exportMDVersion);
            }
            set{
                _exportMDVersion = value;
            }
        }
        ///<summary>Get / Set Export Version for MD</summary>
        public ExportOutdoorVersionItem ExportOutdoorVersion {
            get {
                return (_exportOutdoorVersion);
            }
            set {
                _exportOutdoorVersion = value;
            }
        }
		#endregion
	
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="version">Version to display</param>
		public VersionDetailUI(WebSession webSession, VersionItem version){
			this._webSession = webSession;
			this._version = version;
		}
		/// <summary>
		/// Constructor with exportVersion
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="exportVersion">Version to display</param>
		public VersionDetailUI(WebSession webSession, ExportVersionItem exportVersion){
			this._webSession = webSession;
			this._exportVersion = exportVersion;
		}
		/// <summary>
		/// Constructor with exportVersion for APPM
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="exportAPPMVersion">Version to display</param>
		public VersionDetailUI(WebSession webSession, ExportAPPMVersionItem exportAPPMVersion){
			this._webSession = webSession;
			this._exportAPPMVersion = exportAPPMVersion;
		}
        /// <summary>
        /// Constructor with exportVersion for MD
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="exportMDVersion">Version to display</param>
        public VersionDetailUI(WebSession webSession, ExportMDVersionItem exportMDVersion){
            this._webSession = webSession;
            this._exportMDVersion = exportMDVersion;
        }
        /// <summary>
        /// Constructor with exportVersion for MD
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="exportOutdoorVersion">Version to display</param>
        public VersionDetailUI(WebSession webSession, ExportOutdoorVersionItem exportOutdoorVersion) {
            this._webSession = webSession;
            this._exportOutdoorVersion = exportOutdoorVersion;
        }
		#endregion

		#region Render

		#region Render Version
		/// <summary>Render Version</summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		public virtual void GetHtml(StringBuilder output)
		{
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

			//Table
            output.Append("<table align=\"left\"  cellpadding=0 cellspacing=0  border=\"0\" width=100% class=\"violetBackGroundV3\">");
			
			//Render Verion visual
            output.Append("<tr ><td  align=\"left\" class=\"sloganVioletBackGround\" >");
			this.RenderImage(output);
			output.Append("</td></tr>");

			//Render version nb cell
            output.Append("<tr align=\"left\"><td align=\"left\" nowrap=\"nowrap\" " + //<tr height=100% >
				((this._version.CssClass.Length>0)?"class=\"" + this._version.CssClass + "\">":"class=\"sloganVioletBackGround\">"));
			if(_webSession.SloganIdZoom<0){
				output.Append("<a href=\"javascript:get_version('"+this._version.Id+"');\" onmouseover=\"res_"+this._version.Id+".src='/App_Themes/"+themeName+"/Images/Common/button/result2_down.gif';\" onmouseout=\"res_"+this._version.Id+".src ='/App_Themes/"+themeName+"/Images/Common/button/result2_up.gif';\">");
				output.Append("<img name=\"res_" + this._version.Id + "\" border=0  align=\"left\" src=\"/App_Themes/"+themeName+"/Images/Common/button/result2_up.gif\">");//align=\"absmiddle\"
				output.Append("</a>");
			}
			output.Append("<div align=\"left\"><font align=\"left\"  size=1>");
			output.Append(" "+this._version.Id);
			output.Append("</font></div>");
			//output.Append("</td><td align=\"right\""+((this._version.CssClass.Length>0)?"class=\"" + this._version.CssClass + "\">":"\">"));
			//output.Append("<img src=\"/Images/Common/button/result2_up.gif\">");
			output.Append("</td></tr>");
			
			//Render version synthesis
			this.RenderSynthesis(output);

			//End table
			output.Append("</table>");
		}
		#endregion

		#region Render Version For Press Export UI
		/// <summary>Render Version For Press Export UI</summary>
		/// <param name="output">Le writer HTML vers lequel écrire </param>
		/// <param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		/// <param name="withDetail">Utiliser pour l'affichage des informations sur la version</param>
		public virtual void GetHtmlPressExport(StringBuilder output, Int64 index, bool withDetail)
		{
			//Table
			output.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" bgcolor=\"#ffffff\">");
			//Render Verion visual
			//output.Append("<tr><td align=\"left\" bgcolor=\"#E0D7EC\">");
			output.Append("<tr>");//output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
			output.Append("<TD>");
			output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
			output.Append("<tr>");
			output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
			this.RenderImageExport(output,index);
			output.Append("</td>");

			if(withDetail){
				output.Append("<td valign=\"top\"><TABLE width=\"300\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">");
				output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(176,_webSession.SiteLanguage) + "</td><td width=\"550\">: "+ Convertion.ToHtmlString(this._exportVersion.Advertiser) + "</td></tr>");
				output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(174,_webSession.SiteLanguage) + "</td><td>: "+ Convertion.ToHtmlString(this._exportVersion.Group) +"</td></tr>");
				output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(468,_webSession.SiteLanguage) + "</td><td>: "+ Convertion.ToHtmlString(this._exportVersion.Product) +"</td></tr>");
				//output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(1881,_webSession.SiteLanguage) + "</td><td>: rrr</td></tr>");
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(496,_webSession.SiteLanguage) + "</td><td>: "+ this._exportVersion.FirstInsertionDate.Substring(6,2) +"/"+ this._exportVersion.FirstInsertionDate.Substring(4,2) +"/"+ this._exportVersion.FirstInsertionDate.Substring(0,4) +"</td></tr>");
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(144,_webSession.SiteLanguage) + "</td><td>: "+ this._exportVersion.NbInsertion +"</td></tr>");
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1999,_webSession.SiteLanguage) + "</td><td>: "+ this._exportVersion.NbMedia +"</td></tr>");
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1423,_webSession.SiteLanguage) + "</td><td>: "+ this._exportVersion.ExpenditureEuro +"</td></tr>");
				output.Append("</TABLE></td>");
			} 
			output.Append("</tr>");

			output.Append("<tr width=100%><td width=100% nowrap " + 
				((this._exportVersion.CssClass.Length>0)?"class=\"" + this._exportVersion.CssClass.Replace("c","m") + "\">":"\">"));
			output.Append("<font size=1>");
			output.Append("&nbsp;"+this._exportVersion.Id);
			output.Append("</font></td>");
//			output.Append("</td></tr>");
//
//			output.Append("</TABLE>");
			output.Append("</tr>");

			//End table
			output.Append("</table>");
			output.Append("</td>");
			output.Append("</tr>");
			output.Append("</table>");
			output.Append("</td>");

		}
		#endregion

		#region Render Version For APPM Export UI
		/// <summary>Render Version For Export UI</summary>
		/// <param name="output">Le writer HTML vers lequel écrire </param>
		/// <param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		/// <param name="withDetail">Utiliser pour l'affichage des informations sur la version</param>
		public virtual void GetHtmlAPPMExport(StringBuilder output, Int64 index, bool withDetail)
		{

			bool mediaAgencyAccess=false;

			#region Media Agency rights
			//To check if the user has a right to view the media agency or not
			//mediaAgencyAccess flag is used in the rest of the classes which indicates whethere the user has access 
			//to media agency or not
			if (_webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MEDIA_AGENCY))
				mediaAgencyAccess=true;

			#endregion

			#region IdProduct
			//this is the id of the product selected from the products dropdownlist. 0 id refers to the whole univers i.e. if no prodcut is
			//selected its by default the whole univers and is represeted by product id 0.
			Int64 idProduct=0;
			string idProductString = _webSession.GetSelection(_webSession.CurrentUniversProduct,CustomerRightConstante.type.productAccess);
			if(TNS.AdExpress.Web.Functions.CheckedText.IsStringEmpty(idProductString)) {
				idProduct=Int64.Parse(idProductString);
			}
			#endregion

			//Table
			output.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" bgcolor=\"#ffffff\">");
			//Render Verion visual
			//output.Append("<tr><td align=\"left\" bgcolor=\"#E0D7EC\">");
			output.Append("<tr>");//output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
			output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
			output.Append("<tr>");
			output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
			this.RenderAPPMImageExport(output,index);
			output.Append("</td>");

			if(withDetail)
			{

				output.Append("<td valign=\"top\"><TABLE width=\"450\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">");
 
				
				//Nom du Produit
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1418,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convertion.ToHtmlString(this._exportAPPMVersion.Product) + "</td></tr>");
				//Nom de la marque
				if (_webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE)) {
					output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2001,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convertion.ToHtmlString(this._exportAPPMVersion.Brand) + "</td></tr>");
				}
				//Nom de l'announceur
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1667,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convertion.ToHtmlString(this._exportAPPMVersion.Advertiser) + "</td></tr>");
				//Nom de l'agence Media
				if(this._exportAPPMVersion.Agency!=null)
					if(mediaAgencyAccess && this._exportAPPMVersion.Agency.ToString().Length>0) {
						output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(731,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convertion.ToHtmlString(this._exportAPPMVersion.Agency) + "</td></tr>");
					}

				
					
				//Périod d'analyse
				//output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(381,_webSession.SiteLanguage) + "</td><td width=\"550\">:  "+ this._exportAPPMVersion.DateBegin +" "+GestionWeb.GetWebWord(125,_webSession.SiteLanguage)+" "+ this._exportAPPMVersion.DateEnd + "</td></tr>");
				//Date de la 1ere insertion de la version
				if(this._exportAPPMVersion.Parution!="") {
					output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2000,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ this._exportAPPMVersion.Parution.Substring(6,2) +"/"+ this._exportAPPMVersion.Parution.Substring(4,2) +"/"+ this._exportAPPMVersion.Parution.Substring(0,4) +"</td></tr>");
				}
				//Budget brut (euros)
				output.Append("<tr valign=\"top\"><td>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1669,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.Budget).ToString("# ### ##0.##") +"</td></tr>");
				//Nombre d'insertions
				output.Append("<tr valign=\"top\"><td>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1398,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.Insertions).ToString("# ### ##0.##") +"</td></tr>");
				//Poids de la version vs produit correspondant
				if(this._exportAPPMVersion.VersionWeight != "") {
					output.Append("<tr valign=\"top\"><td>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2002,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.VersionWeight).ToString("# ### ##0.##") +"%</td></tr>");
				}
				//Nombre des pages
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1385,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.Pages).ToString("# ### ##0.##") +"</td></tr>");
				//Nombre de supports utilisés
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1670,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.Supports).ToString("# ### ##0.##") +"</td></tr>");
				//Secteur de référence
				//if the competitor univers is not selected we print the groups of the products selected
				if(_webSession.CompetitorUniversAdvertiser.Count<2) 	{
					//output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1668,_webSession.SiteLanguage) + "</td><td>: "+ this._exportAPPMVersion.Group +"</td></tr>");
					string[] groups=this._exportAPPMVersion.Group.ToString().Split(',');
					output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1668,_webSession.SiteLanguage)) + "</td><td nowrap> : ");
					Array.Sort(groups);
					foreach(string gr in groups) {
						output.Append("&nbsp;&nbsp;&nbsp;"+gr+"&nbsp;&nbsp;");	
					}
					output.Append("</td></tr>");
				}
				if(this._exportAPPMVersion.PDV != "") {
					//Part de voix de la campagne
					output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1671,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.PDV).ToString("# ### ##0.##") +"%</td></tr>");
				}
				//cible selectionnée
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1672,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convertion.ToHtmlString(this._exportAPPMVersion.TargetSelected) +"</td></tr>");
				// nombre de GRP
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1673,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.GRPNumber).ToString("# ### ##0.##") +"</td></tr>");
				// nombre de GRP 15 et +
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1673,_webSession.SiteLanguage)) + " " +Convertion.ToHtmlString(this._exportAPPMVersion.BaseTarget) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.GRPNumberBase).ToString("# ### ##0.##") +"</td></tr>");
				//Indice GRP vs cible 15 ans à +																				   
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1674,_webSession.SiteLanguage)) + " vs " +Convertion.ToHtmlString(this._exportAPPMVersion.BaseTarget) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.IndiceGRP).ToString("# ### ##0.##") +"</td></tr>");
				// Coût GRP(cible selectionnée)					
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1675,_webSession.SiteLanguage)) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+ Convert.ToDouble(this._exportAPPMVersion.GRPCost).ToString("# ### ##0") +"</td></tr>");
				// Coût GRP(cible 15 ans et +)					
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1675,_webSession.SiteLanguage)) + " " + Convertion.ToHtmlString(this._exportAPPMVersion.BaseTarget) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+  Convert.ToDouble(this._exportAPPMVersion.GRPCostBase).ToString("# ### ##0") +"</td></tr>");
				//Indice coût GRP vs cible 15 ans à +
				output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1676,_webSession.SiteLanguage)) + " vs " +Convertion.ToHtmlString(this._exportAPPMVersion.BaseTarget) + "</td><td nowrap> : &nbsp;&nbsp;&nbsp;"+  Convert.ToDouble(this._exportAPPMVersion.IndiceGRPCost).ToString("# ### ##0.##") +"</td></tr>");
								
				output.Append("</TABLE></td>");

				//output.Append("<td bgcolor=\"#ffffff\" style=\"WIDTH: 30px; BORDER-RIGHT: white 0px solid;BORDER-LEFT: white 1px solid\"><font color=#ffffff>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font></td>");
			} 
			output.Append("</tr>");

			output.Append("<tr width=100%><td width=100% nowrap " + 
				((this._exportAPPMVersion.CssClass.Length>0)?"class=\"" + this._exportAPPMVersion.CssClass + "\">":"\">"));
			output.Append("<font size=1>");
			output.Append("&nbsp;"+this._exportAPPMVersion.Id);
			output.Append("</font>");
			//			output.Append("</td></tr>");
			//
			//			output.Append("</TABLE>");
			output.Append("</tr>");
			output.Append("<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 15px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");

			//End table
			output.Append("</table>");
		}
		#endregion

		#region Render Version For Tv Export UI
		/// <summary>Render Version For Tv Export UI</summary>
		/// <param name="output">Le writer HTML vers lequel écrire </param>
		/// <param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		/// <param name="withDetail">Utiliser pour l'affichage des informations sur la version</param>
		public virtual void GetHtmlTvExport(StringBuilder output, Int64 index, bool withDetail)
		{
			string[] cssColor = {"#644883","#CC99FF","#993300","#99CCFF","#003300","#003366","#000080","#333399","#800000","#FF6600","#808000",
								"#008000","#008080","#0000FF","#FF0000","#FF9900","#99CC00","#339966","#33CCCC","#3366FF","#800080","#FF00FF",
								"#FFCC00","#FFFF00","#00FF00","#00FFFF","#00CCFF","#993366","#FF99CC","#FFCC99","#FFFF99","#CCFFCC"};

			string color= cssColor[int.Parse(this._exportVersion.CssClass.Substring(2,(this._exportVersion.CssClass.Length-2)))];
			
			//Table
			output.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" bgcolor=\"#ffffff\">");
			//Render Verion visual
			output.Append("<tr><td>");
			output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
			//output.Append("<tr>");
			output.Append("<tr><td bgcolor=\"#ffffff\"></td></tr>");

			if(withDetail) {
				output.Append("<tr><td valign=\"top\"><TABLE width=\"320\" cellSpacing=\"0\" class=\"txtViolet11Bold\" border=\"0\" valign=\"top\">");
				output.Append("<tr><td nowrap colSpan=2 style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-LEFT: " + color  + 
							  " 1px solid; BORDER-TOP: " + color  + " 1px solid; BORDER-BOTTOM: " + color  + " 1px solid\" " + 
					         ((this._exportVersion.CssClass.Length>0)?"class=\"" + this._exportVersion.CssClass.Replace("c","m") + "\" style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-TOP: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">":"\" style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-TOP: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">"));
				output.Append("<font size=1>");
				output.Append("&nbsp;"+this._exportVersion.Id);
				output.Append("</font>");
				output.Append("</td></tr>");
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\" style=\"BORDER-RIGHT: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \">&nbsp;" + GestionWeb.GetWebWord(176,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\" style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.Advertiser  + "</td></tr>");
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \" >&nbsp;" + GestionWeb.GetWebWord(174,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.Group  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \" >&nbsp;" + GestionWeb.GetWebWord(468,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.Product  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(496,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.FirstInsertionDate.Substring(6,2) +"/"+ this._exportVersion.FirstInsertionDate.Substring(4,2) +"/"+ this._exportVersion.FirstInsertionDate.Substring(0,4) +"</td></tr>");
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(144,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.NbInsertion  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(1999,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.NbMedia  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(1423,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.ExpenditureEuro  +"</td></tr>"); 
				output.Append("</TABLE></td>");
				output.Append("<td bgcolor=\"#ffffff\" style=\"WIDTH: 40px; BORDER-RIGHT: white 0px solid;BORDER-LEFT: white 1px solid\"><font color=#ffffff>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font></td>");
			} 
			output.Append("</tr>");
			output.Append("<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 50px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");

			//End table
			output.Append("</table>");
			output.Append("</td>");
			output.Append("</tr>");
			output.Append("</table>");
			output.Append("</td>");
		}
		#endregion

		#region Render Version For Radio Export UI
		/// <summary>Render Version For Radio Export UI</summary>
		/// <param name="output">Le writer HTML vers lequel écrire </param>
		/// <param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		/// <param name="withDetail">Utiliser pour l'affichage des informations sur la version</param>
		public virtual void GetHtmlRadioExport(StringBuilder output, Int64 index, bool withDetail)
		{
			string[] cssColor = {"#644883","#CC99FF","#993300","#99CCFF","#003300","#003366","#000080","#333399","#800000","#FF6600","#808000",
									"#008000","#008080","#0000FF","#FF0000","#FF9900","#99CC00","#339966","#33CCCC","#3366FF","#800080","#FF00FF",
									"#FFCC00","#FFFF00","#00FF00","#00FFFF","#00CCFF","#993366","#FF99CC","#FFCC99","#FFFF99","#CCFFCC"};

			string color= cssColor[int.Parse(this._exportVersion.CssClass.Substring(2,(this._exportVersion.CssClass.Length-2)))];
			
			//Table
			output.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" bgcolor=\"#ffffff\">");
			//Render Verion visual
			output.Append("<tr><td>");
			output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
			//output.Append("<tr>");
			output.Append("<tr><td bgcolor=\"#ffffff\"></td></tr>");

			if(withDetail) 
			{
				output.Append("<tr><td valign=\"top\"><TABLE width=\"320\" cellSpacing=\"0\" class=\"txtViolet11Bold\" border=\"0\" valign=\"top\">");
				output.Append("<tr><td nowrap colSpan=2 style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-LEFT: " + color  + 
					" 1px solid; BORDER-TOP: " + color  + " 1px solid; BORDER-BOTTOM: " + color  + " 1px solid\" " + 
					((this._exportVersion.CssClass.Length>0)?"class=\"" + this._exportVersion.CssClass.Replace("c","m") + "\" style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-TOP: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">":"\" style=\"BORDER-RIGHT: " + color + " 1px solid; BORDER-TOP: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">"));
				output.Append("<font size=1>");
				output.Append("&nbsp;"+this._exportVersion.Id);
				output.Append("</font>");
				output.Append("</td></tr>");
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\" style=\"BORDER-RIGHT: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \">&nbsp;" + GestionWeb.GetWebWord(176,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\" style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.Advertiser  + "</td></tr>");
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \" >&nbsp;" + GestionWeb.GetWebWord(174,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.Group  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \" >&nbsp;" + GestionWeb.GetWebWord(468,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.Product  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(496,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.FirstInsertionDate.Substring(6,2) +"/"+ this._exportVersion.FirstInsertionDate.Substring(4,2) +"/"+ this._exportVersion.FirstInsertionDate.Substring(0,4) +"</td></tr>");
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(144,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.NbInsertion  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(1999,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.NbMedia  +"</td></tr>"); 
				output.Append("<tr valign=\"top\"><td width=\"150\" bgcolor=\"#ffffff\"  style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: " + color + " 1px solid; BORDER-LEFT: " + color + " 1px solid \"  nowrap>&nbsp;" + GestionWeb.GetWebWord(1423,_webSession.SiteLanguage) + "</td><td bgcolor=\"#ffffff\"  style=\"BORDER-TOP: #644883 1px solid; BORDER-RIGHT: " + color + " 1px solid; BORDER-BOTTOM: " + color + " 1px solid\">&nbsp; "+ this._exportVersion.ExpenditureEuro  +"</td></tr>"); 
				output.Append("</TABLE></td>");
				output.Append("<td bgcolor=\"#ffffff\" style=\"WIDTH: 40px; BORDER-RIGHT: white 0px solid;BORDER-LEFT: white 1px solid\"><font color=#ffffff>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font></td>");
			} 
			output.Append("</tr>");
			output.Append("<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 50px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");

			//End table
			output.Append("</table>");
			output.Append("</td>");
			output.Append("</tr>");
			output.Append("</table>");
			output.Append("</td>");
		}
		#endregion

        #region Render Version For MD Export UI
        /// <summary>Render Version For MD Export UI</summary>
        /// <param name="output">Le writer HTML vers lequel écrire </param>
        /// <param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
        /// <param name="withDetail">Utiliser pour l'affichage des informations sur la version</param>
        public virtual void GetHtmlMDExport(StringBuilder output, Int64 index, bool withDetail){
            //Table
            output.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" bgcolor=\"#ffffff\">");
            //Render Verion visual
            //output.Append("<tr><td align=\"left\" bgcolor=\"#E0D7EC\">");
            output.Append("<tr>");//output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
            output.Append("<TD>");
            output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
            output.Append("<tr>");
            output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
            this.RenderImageExport(output, index);
            output.Append("</td>");

            if (withDetail){
                output.Append("<td valign=\"top\"><TABLE width=\"300\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">");
                output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(176, _webSession.SiteLanguage) + "</td><td width=\"550\">: " + Convertion.ToHtmlString(this._exportMDVersion.Advertiser) + "</td></tr>");
                output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(174, _webSession.SiteLanguage) + "</td><td>: " + Convertion.ToHtmlString(this._exportMDVersion.Group) + "</td></tr>");
                output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(468, _webSession.SiteLanguage) + "</td><td>: " + Convertion.ToHtmlString(this._exportMDVersion.Product) + "</td></tr>");
                if (_webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_POIDS_MARKETING_DIRECT))
                    output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2220, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.Weight + "</td></tr>");
                output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1423, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.ExpenditureEuro + "</td></tr>");
                if (_webSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT))
                    output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2216, _webSession.SiteLanguage) + "</td><td>: " + Math.Round(this._exportMDVersion.Volume) + "</td></tr>");


                switch (this._exportMDVersion.IdMedia.ToString()) {

                    case CstDB.Media.PUBLICITE_NON_ADRESSEE:
                        output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.Format + "</td></tr>");
                        output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2222, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.MailFormat + "</td></tr>");
                        output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2221, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.MailType + "</td></tr>");
                        break;
                    case CstDB.Media.COURRIER_ADRESSE_GENERAL:
                        if(this._exportMDVersion.WpMailFormat == CstDB.Format.FORMAT_STANDARD)
                            output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, _webSession.SiteLanguage) + "</td><td>: " + GestionWeb.GetWebWord(2240, _webSession.SiteLanguage) + "</td></tr>");
                        else if (this._exportMDVersion.WpMailFormat == CstDB.Format.FORMAT_ORIGINAL)
                            output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, _webSession.SiteLanguage) + "</td><td>: " + GestionWeb.GetWebWord(2241, _webSession.SiteLanguage) + "</td></tr>");
                        output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2224, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.ObjectCount + "</td></tr>");
                        output.Append(GetMailContentList(GestionWeb.GetWebWord(2239, _webSession.SiteLanguage), this._exportMDVersion.MailContent));
                        break;
                    case CstDB.Media.COURRIER_ADRESSE_PRESSE:
                        output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.ObjectCount + "</td></tr>");
                        output.Append(GetMailContentList(GestionWeb.GetWebWord(2239, _webSession.SiteLanguage), this._exportMDVersion.MailContent));
                        break;
                    case CstDB.Media.COURRIER_ADRESSE_GESTION:
                        output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2224, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.ObjectCount + "</td></tr>");
                        output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2223, _webSession.SiteLanguage) + "</td><td>: " + this._exportMDVersion.MailingRapidity + "</td></tr>");
                        output.Append(GetMailContentList(GestionWeb.GetWebWord(2239, _webSession.SiteLanguage), this._exportMDVersion.MailContent));
                        break;
                }

                output.Append("</TABLE></td>");
            }
            output.Append("</tr>");

            output.Append("<tr width=100%><td width=100% nowrap " +
                ((this._exportMDVersion.CssClass.Length > 0) ? "class=\"" + this._exportMDVersion.CssClass.Replace("c", "m") + "\">" : "\">"));
            output.Append("<font size=1>");
            output.Append("&nbsp;" + this._exportMDVersion.Id);
            output.Append("</font></td>");
            //			output.Append("</td></tr>");
            //
            //			output.Append("</TABLE>");
            output.Append("</tr>");

            //End table
            output.Append("</table>");
            output.Append("</td>");
            output.Append("</tr>");
            output.Append("</table>");
            output.Append("</td>");

        }
        #endregion

        #region Render Version For Outdoor Export UI
        /// <summary>Render Version For Outdoor Export UI</summary>
        /// <param name="output">Le writer HTML vers lequel écrire </param>
        /// <param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
        /// <param name="withDetail">Utiliser pour l'affichage des informations sur la version</param>
        public virtual void GetHtmlOutdoorExport(StringBuilder output, Int64 index, bool withDetail) {
            //Table
            output.Append("<table cellpadding=0 cellspacing=0 width=100% border=\"0\" bgcolor=\"#ffffff\">");
            //Render Verion visual
            //output.Append("<tr><td align=\"left\" bgcolor=\"#E0D7EC\">");
            output.Append("<tr>");//output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
            output.Append("<TD>");
            output.Append("<table cellpadding=0 cellspacing=0 border=\"0\" bgcolor=\"#ffffff\">");
            output.Append("<tr>");
            output.Append("<td align=\"left\" bgcolor=\"#FFFFFF\">");
            this.RenderImageExport(output, index);
            output.Append("</td>");

            if (withDetail) {
                output.Append("<td valign=\"top\"><TABLE width=\"300\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">");
                output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(176, _webSession.SiteLanguage) + "</td><td width=\"550\">: " + Convertion.ToHtmlString(this._exportOutdoorVersion.Advertiser) + "</td></tr>");
                output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(174, _webSession.SiteLanguage) + "</td><td>: " + Convertion.ToHtmlString(this._exportOutdoorVersion.Group) + "</td></tr>");
                output.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(468, _webSession.SiteLanguage) + "</td><td>: " + Convertion.ToHtmlString(this._exportOutdoorVersion.Product) + "</td></tr>");
                output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1888, _webSession.SiteLanguage) + "</td><td>: " + this._exportOutdoorVersion.Id + "</td></tr>");
                output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1712, _webSession.SiteLanguage) + "</td><td>: " + this._exportOutdoorVersion.ExpenditureEuro + "</td></tr>");
                output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1604, _webSession.SiteLanguage) + "</td><td>: " + this._exportOutdoorVersion.NbBoards + "</td></tr>");
                output.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1999, _webSession.SiteLanguage) + "</td><td>: " + this._exportOutdoorVersion.NbMedia + "</td></tr>");
                output.Append("</TABLE></td>");
            }
            output.Append("</tr>");

            output.Append("<tr width=100%><td width=100% nowrap " +
                ((this._exportOutdoorVersion.CssClass.Length > 0) ? "class=\"" + this._exportOutdoorVersion.CssClass.Replace("c", "m") + "\">" : "\">"));
            output.Append("<font size=1>");
            output.Append("&nbsp;" + this._exportOutdoorVersion.Id);
            output.Append("</font></td>");
            //			output.Append("</td></tr>");
            //
            //			output.Append("</TABLE>");
            output.Append("</tr>");

            //End table
            output.Append("</table>");
            output.Append("</td>");
            output.Append("</tr>");
            output.Append("</table>");
            output.Append("</td>");

        }
        #endregion

		#endregion

		#region Méthodes

		///<summary>Render Version Visual</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected virtual void RenderImage( StringBuilder output ) 
		{
		
			string[] pathes = this._version.Path.Split(',');
			foreach(string path in pathes){
				output.Append("<img src=\""
					+((path.Length>0)?path:"images/common/Picto_Radio.gif")
					+ "\">");
			}
		
		}
		///<summary>Render Version SYnthesis</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		protected virtual void RenderSynthesis( StringBuilder output ) {
		}
		///<summary>Render Version Visual</summary>
		///  <author>rkaina</author>
		///  <since>mercredi 06 septembre 2006</since>
		protected virtual void RenderImageExport( StringBuilder output ) {
		
			string[] pathes = this._version.Path.Split(',');
			foreach(string path in pathes) {
				output.Append("<img src=\""
					+((path.Length>0)?path:"images/common/Picto_Radio.gif")
					+ "\">");
			}
		
		}
		///<summary>Render Version Visual For Export UI</summary>
		///<param name="output"></param>
		///<param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		///  <author>rkaina</author>
		///  <since>vendredi 18 août 2006</since>
		protected virtual void RenderImageExport( StringBuilder output, Int64 index ) {
			string[] pathes = this._version.Path.Split(',');
			foreach(string path in pathes) {
				output.Append("<img src=\""
					+((path.Length>0)?path:"images/common/Picto_Radio.gif")
					+ "\">");
			}
		}
		///<summary>Render Version Visual For Export UI</summary>
		///<param name="output"></param>
		///<param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
		///  <author>rkaina</author>
		///  <since>vendredi 18 août 2006</since>
		protected virtual void RenderAPPMImageExport( StringBuilder output, Int64 index ) {
			string[] pathes = this._version.Path.Split(',');
			foreach(string path in pathes) {
				output.Append("<img src=\""
					+((path.Length>0)?path:"images/common/Picto_Radio.gif")
					+ "\">");
			}
		}

        /// <summary>
        /// Renvoie le code Html de la liste des mail content pour le CA
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="mailContent">Liste des mails content</param>
        /// <returns></returns>
        private static string GetMailContentList(string label, string mailContent) {

            string HtmlTxt = string.Empty;
            bool first = true;
            string[] listItem;

            listItem = mailContent.Split(',');

            foreach (string item in listItem) {
                if (first) {
                    HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + label + "</td><td nowrap>: " + item + "</td></tr>";
                    first = false;
                }
                else
                    HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </td><td nowrap>&nbsp; " + item + "</td></tr>";
            }
            return HtmlTxt;

        }
		#endregion

	}
}
