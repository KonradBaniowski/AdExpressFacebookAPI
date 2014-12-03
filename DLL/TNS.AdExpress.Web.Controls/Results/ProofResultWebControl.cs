#region Information
// Auteur : D. Mussuma
// Date de création : 31/01/2007
// Date de modification
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Rules.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpress.Web.Controls.Results
{
	/// <summary>
	/// Affiche la fiche justificative d'un produit
	/// </summary>
	[ToolboxData("<{0}:ProofResultWebControl runat=server></{0}:ProofResultWebControl>")]
	public class ProofResultWebControl : System.Web.UI.WebControls.WebControl
	{
		#region Variables
		
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idSession = null;
		/// <summary>
		/// Identifiant du media
		/// </summary>
		private string _idMedia = null;
		/// <summary>
		/// Identifiant du produit
		/// </summary>
		private string _idProduct = null;
		/// <summary>
		/// Date de codification
		/// </summary>
		private string _dateFacial = null;
		/// <summary>
		/// Date parution
		/// </summary>
		private string _dateParution = null;
		/// <summary>
		/// Page
		/// </summary>
		private string _pageNumber = null;
	

		/// <summary>
		/// Liste des visuels 
		/// </summary>
		protected WebSession _customerWebSession=null;
		
		/// <summary>
		/// Message d'erreur
		/// </summary>
		private string _errorMessage = null;
		
		/// <summary>
		/// Tableau de résultats
		/// </summary>
		private DataTable _dtResult = null;
		/// <summary>
		/// Chemin web du fichier
		/// </summary>
		string _pathWeb = null;
		/// <summary>
		/// Chemin web du fichier
		/// </summary>
		string _pathWeb2 = null;

		/// <summary>
		/// Liste des visules
		/// </summary>
		string[] _fileList = null;

		/// <summary>
		/// Indique si le(s) visuels existe(nt)
		/// </summary>
		protected bool _isVisualExist = false;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit la session client 
		/// </summary>
		public WebSession CustomerWebSession{
			get{return _customerWebSession;}
			set{_customerWebSession = value;}
		}

		/// <summary>
		/// Obtient l'identifiant du support 
		/// </summary>
		public string IdMedia{
			get{return _idMedia;}
			
		}

		/// <summary>
		/// Obtient l'identifiant du produit 
		/// </summary>
		public string IdProduct{
			get{return _idProduct;}
			
		}

		/// <summary>
		/// Obtient l'identifiant de la page  
		/// </summary>
		public string PageNumber{
			get{return _pageNumber;}
			
		}

		/// <summary>
		/// Obtient la date de codification
		/// </summary>
		public string DateMediaNum{
			get{return _dateParution;}
			
		}

		/// <summary>
		/// Obtient la date de parution
		/// </summary>
		public string DateFacial{
			get{return _dateFacial;}
			
		}

		/// <summary>
		/// Indique si le(s) visuels existe(nt)
		/// </summary>
		public bool IsVisualExist {
			get { return _isVisualExist; }
		}
		#endregion
	
		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			try{
				//Récupération des paramètres de l'url
				_idMedia	= Page.Request.QueryString.Get("idMedia");
				_idProduct	= Page.Request.QueryString.Get("idProduct");
				_dateFacial		= Page.Request.QueryString.Get("dateFacial");
				_dateParution = Page.Request.QueryString.Get("dateParution");
				_pageNumber		= Page.Request.QueryString.Get("page");
			}
			catch(System.Exception){
				_errorMessage = WebFunctions.Script.ErrorCloseScript(GestionWeb.GetWebWord(958, _customerWebSession.SiteLanguage));
			}

			base.OnInit (e);
		}
		#endregion

		#region PreRender
		/// <summary>
		/// Construction de la liste de checkbox
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {
            _pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateFacial + "/imagette/";
            _pathWeb2 = CstWeb.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateFacial + "/";
			

			#region GetData
			_dtResult = ProofRules.GetProofFileData(_customerWebSession, _idMedia, _idProduct,_dateFacial, _pageNumber,_dateParution);
			#endregion

			// Affichage de l'image taille réelle
			if (!this.Page.ClientScript.IsClientScriptBlockRegistered("ViewAdZoom")){
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ViewAdZoom",WebFunctions.Script.ViewAdZoom());
			}
			// Ouverture de la popup chemin de fer
			if (!Page.ClientScript.IsClientScriptBlockRegistered("PortofolioCreationWithAnchor")) {
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"PortofolioCreationWithAnchor",WebFunctions.Script.PortofolioCreationWithAnchor());
			}

			if (_dtResult!=null && _dtResult.Rows.Count > 0){
				foreach(DataRow row in _dtResult.Rows){

					#region Construction de la liste des images
					if(row["visual"].ToString().Length > 0){
						_fileList = row["visual"].ToString().Split(',');
						_customerWebSession.Visuals = new ArrayList();
						_customerWebSession.Visuals.Add(row["visual"].ToString());
						_customerWebSession.Save();
					}
					#endregion

					#region Scripts				
					if(_fileList!=null){						

						if (!this.Page.ClientScript.IsClientScriptBlockRegistered("PreloadImages")){
							this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"PreloadImages",WebFunctions.Script.PreloadImages(_fileList,_pathWeb2));
						}
					}
					#endregion
				}
				if (_fileList != null) _isVisualExist = true;
			}
		}

		#endregion
		
		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
			

			#region Variables		
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;
			int i = 0;
			#endregion
			
			if (_dtResult!=null && _dtResult.Rows.Count > 0){
				foreach(DataRow row in _dtResult.Rows){

					#region Construction de la liste des images
					if (row["visual"].ToString().Length > 0) {
						_fileList = row["visual"].ToString().Split(',');
					}
					#endregion

					
					#region Rendu html

					#region Début tableau général
                    output.Write("<TABLE  class=\"whiteBackGround\" style=\"MARGIN-TOP: 5px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
					output.Write("cellPadding=\"0\" cellSpacing=\"3\" align=\"center\" border=\"0\" width=\"900\" >");
					output.Write("<tr align=\"center\"><td>");
					#endregion

					#region Début tableau fiche
					output.Write("\n<table cellPadding=\"0\" cellSpacing=\"0\" border=\"0\" class=\"lightPurple\" width=\"100%\">");
					//Header
					output.Write("<tr>");
					output.Write("<td  vAlign=\"top\" colspan=\"3\">");
					output.Write("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
					output.Write("<td>");
					output.Write("<!-- fleche -->");
					output.Write("<td style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></td>");
                    output.Write("<td  vAlign=\"top\" width=\"100%\" class=\"popuptitle1\" style=\"BACKGROUND-POSITION-X: right; BACKGROUND-IMAGE: url(/App_Themes/" + themeName + "/Images/Common/bandeau_titre.gif);BACKGROUND-REPEAT: repeat-y\">");
					output.Write(GestionWeb.GetWebWord(1766, _customerWebSession.SiteLanguage));
					output.Write("</td>");
					output.Write("</td>");
					output.Write("</table>");
					output.Write("</td>");
					output.Write("</tr>");
                    output.Write("\n<tr><td colspan=\"3\" class=\"whiteBackGround\" style=\"HEIGHT: 3px;\" ></td></tr>");
					output.Write("\n<tr valign=\"top\">");
					#endregion				
					
					#region Table Visuel Zoom
					output.Write("\n<td width=\"50%\"  valign=\"middle\">");
					output.Write("\n<table cellPadding=\"3\" cellSpacing=\"0\" border=\"0\" align=\"center\" width=\"100%\">");
					output.Write("\n<tr><td rowspan=\"19\" align=\"center\" valign=\"middle\">");
					if(_fileList!=null){
						output.Write("<img name=\"displayImg\" src='"+_pathWeb2+_fileList.GetValue(0).ToString()+"' border=\"0\">");
						_isVisualExist = true;
					}
					else{
                        output.Write("<img src=\"/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif\">");
					}
					output.Write("\n</td></tr>");
					output.Write("\n</table>");
					output.Write("\n</td>");
					#endregion
					
					#region Colone séparatrice
					output.Write("\n<td bgcolor=\"#FFFFFF\">&nbsp;</td>");
					#endregion
					
					#region Table Informations
					output.Write("\n<td>");
					output.Write("\n<table cellPadding=\"3\" cellSpacing=\"0\" border=\"0\" style=\"text-align:left\" width=\"100%\">");
					
					#region Visuel de la couverture du media + Nom du media + Date
					// Chemin du répertoire virtuel de la couverture
					string pathCouv = _pathWeb+CstWeb.CreationServerPathes.COUVERTURE;
					
					// Chemin du répertoire contenant le visuel de la couverture presse
                    string pathCouv2 = CstWeb.CreationServerPathes.LOCAL_PATH_IMAGE + _idMedia + @"\" + _dateFacial + @"\" + CstWeb.CreationServerPathes.COUVERTURE;
					// Pour test en localhost :
                    //string pathCouv2="\\\\localhost\\ImagesPresse\\"+_idMedia+"\\"+_dateFacial+"\\"+CstWeb.CreationServerPathes.COUVERTURE;

					output.Write("\n<tr>");
					if(File.Exists(pathCouv2)){
						output.Write("\n<td colspan=\"2\" align=\"center\"><img src='"+pathCouv+"' border=\"0\" width=\"100\" height=\"141\"><br><font class=\"txtViolet14Bold\">"+ row["Media"] +"</font>");
                        if (row["datePublication"] != System.DBNull.Value) output.Write("<br><font class=txtViolet11>" + Dates.DateToString((DateTime)row["datePublication"], _customerWebSession.SiteLanguage) + "</font>");
                        if (row["number_page_media"] != System.DBNull.Value) output.Write("<br><font class=txtViolet11>" + row["number_page_media"].ToString() + "&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CstWeb.CustomerSessions.Unit.pages].WebTextId, _customerWebSession.SiteLanguage)) + "</font>");
					}
					else{
                        output.Write("\n<td colspan=\"2\" align=\"center\"><img src=\"/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif\"><br><font class=\"txtViolet14Bold\">" + row["Media"] + "</font>");
                        if (row["datePublication"] != System.DBNull.Value) output.Write("<br><font class=txtViolet11>" +  Dates.DateToString((DateTime)row["datePublication"], _customerWebSession.SiteLanguage) + "</font>");
                        if (row["number_page_media"] != System.DBNull.Value) output.Write("<br><font class=txtViolet11>" + row["number_page_media"].ToString() + "&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[CstWeb.CustomerSessions.Unit.pages].WebTextId, _customerWebSession.SiteLanguage)) + "</font>");

					}
					output.Write("\n</tr>");
					#endregion
					
					#region Informations
					output.Write("\n<tr><td colspan=\"2\" bgcolor=\"#FFFFFF\" style=\"HEIGHT: 1px;\" ></td></tr>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(894, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet14Bold\">"+ row["media_paging"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\" width=\"50%\">&nbsp;"+ GestionWeb.GetWebWord(857, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\" style=\"white-space:nowrap;\">"+ row["advertiser"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(858, _customerWebSession.SiteLanguage) +"</td>");
                    output.Write("\n<td class=\"txtViolet11\" style=\"white-space:nowrap;\">" + row["product"] + "</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(859, _customerWebSession.SiteLanguage) +"</td>");
                    output.Write("\n<td class=\"txtViolet11\" style=\"white-space:nowrap;\">" + row["group_"] + "</td>");					
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1767, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ row["area_page"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1768, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ Decimal.Parse(row["area_mmc"].ToString()).ToString("# ### ##0.##") +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1769, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ row["location"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1420, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ row["format"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1438, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ row["color"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1426, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ row["rank_sector"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1427, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ row["rank_group_"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1428, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ row["rank_media"] +"</td>");
					output.Write("\n</tr>");
					output.Write("\n<tr valign=\"top\">");
					output.Write("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;"+ GestionWeb.GetWebWord(1770, _customerWebSession.SiteLanguage) +"</td>");
					output.Write("\n<td class=\"txtViolet11\">"+ Decimal.Parse(row["expenditure_euro"].ToString()).ToString("# ### ##0.##") +"</td>");
					output.Write("\n</tr>");
					#endregion
					
					if(_fileList!=null){

						#region Vignettes
						output.Write("\n<tr><td colspan=\"2\" bgcolor=\"#FFFFFF\" style=\"HEIGHT: 1px;\" ></td></tr>");
						output.Write("\n<tr><td colspan=\"2\" align=\"center\">");
						foreach(string currentFile in _fileList) {
							output.Write("<a href=\"#\"><img src='"+_pathWeb+currentFile+"' border=\"0\" width=\"50\" height=\"64\" onMouseOver=\"javascript:viewAdZoom('Img"+i+"');\"></a>&nbsp;");
							i++;
						}
						output.Write("\n</td></tr>");
						#endregion
					
						#region Lien du chemin de fer
						output.Write("\n<tr><td colspan=\"2\" bgcolor=\"#FFFFFF\" style=\"HEIGHT: 1px;\"></td></tr>");
						output.Write("\n<tr><td colspan=\"2\" align=\"center\">");
                        output.Write("<a href=\"javascript:portofolioCreationWithAnchor('" + _customerWebSession.IdSession + "','" + _idMedia + "','" + _dateParution + "','" + _dateFacial + "','" + row["Media"] + "','" + row["number_page_media"].ToString() + "','" + _pageNumber + "');\" class=\"roll04\">" + GestionWeb.GetWebWord(1397, _customerWebSession.SiteLanguage) + "</a>");
						output.Write("\n</td></tr>");
						#endregion

					}

					output.Write("\n</table>");
					output.Write("\n</td>");
					#endregion
					
					#region Fin tableau fiche
					output.Write("\n</tr>");
					output.Write("\n</table>");
					#endregion

					#region Fin tableau général
					output.Write("</td></tr></table></td></tr></table>");
					#endregion

					#endregion

				}
			}
			else{
				output.Write("<div align=\"center\" class=\"txtBlanc11Bold\">"+ GestionWeb.GetWebWord(177,_customerWebSession.SiteLanguage) +"</div>");
			}
		}
		#endregion
	}
}
