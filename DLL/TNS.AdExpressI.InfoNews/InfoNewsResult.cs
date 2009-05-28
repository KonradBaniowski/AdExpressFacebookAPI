#region Informations
// Auteur: D. Mussuma
// Création: 26/05/2009
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Constantes.Web;
using DomainResults=TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using System.Collections;

namespace TNS.AdExpressI.InfoNews {
	
	public class InfoNewsResult : IInfoNewsResult {

		/// User session
		/// </summary>
		protected WebSession _session;
		/// <summary>
		/// I,nfo/news information
		/// </summary>
		protected DomainResults.InfoNews _infoNewsInformations;
		/// <summary>
		/// Theme name
		/// </summary>
		protected string _themeName ="";
		
		#region Constructor
		public InfoNewsResult(WebSession session,string themeName) {
			_session = session;
			_infoNewsInformations = WebApplicationParameters.InfoNewsInformations;
			_themeName = themeName;
		}
		#endregion

		#region IInfoNewsResult Implementation
		public virtual string GetHtml() {

			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
			int compteur = 0;
			int start = 1, nbFilesAdded=0;
			List<DomainResults.InfoNewsItem> sortedInfoNewsItems = (_infoNewsInformations!=null) ? _infoNewsInformations.GetSortedInfoNewsItems() : null;
			string[] files = null;
			#endregion

			if (sortedInfoNewsItems != null && sortedInfoNewsItems.Count > 0) {
				//For each directory
				for (int i = 0; i < sortedInfoNewsItems.Count;i++ ) {
					files = GetDirectoryFiles(sortedInfoNewsItems[i].PhysicalPath);
					if (files != null && files.Length > 0) {
						//Siort files by alpabetical label
						Array.Sort(files, Comparer.Default);

						if (start == 1) {
							t.Append("\n<table class=\"violetBorder txtViolet11Bold whiteBackGround\" cellpadding=0 cellspacing=0 width=\"650\">");
							t.Append("\n<tr onClick=\"javascript : showHideContent('info_" + sortedInfoNewsItems[i].Id.GetHashCode().ToString() + "');\" style=\"cursor : hand\">");
							t.Append("\n<td>&nbsp;" + GestionWeb.GetWebWord(sortedInfoNewsItems[i].WebTextId, _session.SiteLanguage) + "</td>");
							t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + _themeName + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
							t.Append("\n</tr></table>");
							start = 0;
						}
						else if (start == 0) {
							t.Append("\n<table class=\"violetBorderWithoutTop txtViolet11Bold whiteBackGround\"  cellpadding=0 cellspacing=0 width=\"650\">");
							t.Append("\n<tr onClick=\"javascript : showHideContent('info_" + sortedInfoNewsItems[i].Id.GetHashCode().ToString() + "');\" style=\"cursor : hand\">");
							t.Append("\n<td>&nbsp;" + GestionWeb.GetWebWord(sortedInfoNewsItems[i].WebTextId, _session.SiteLanguage) + "</td>");
							t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + _themeName + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
							t.Append("\n</tr></table>");
						}

						t.Append("<div id=\"info_" + sortedInfoNewsItems[i].Id.GetHashCode().ToString() + "Content\" style=\"MARGIN-LEFT: 0px; DISPLAY: none;\" >");
						t.Append("\n<table id=\"info_" + sortedInfoNewsItems[i].Id.GetHashCode().ToString() + "\" class=\"violetBorderWithoutTop txtViolet10 paleVioletBackGround\" width=\"650\">");
						compteur = 0;

						//Show all directoy's files						
						for (int j = files.Length-1; j >=0; j--) {
							if (Path.GetExtension(files[j].ToString()).ToUpper().Equals(".DB")) continue; ;
							//Limitation of Nb files to show							
							if (sortedInfoNewsItems[i].NbMaxItemsToShow > -1 && nbFilesAdded == sortedInfoNewsItems[i].NbMaxItemsToShow) break;							
							
							string logo = GetFileLogoExtension(Path.GetExtension(files[j].ToString()));
							string logoPath = (logo!=null && logo.Length>0) ? "<img src=\"/App_Themes/" + _themeName + "/Images/Common/"+logo+"\" border=0 alt=\"" + FormatFileName(sortedInfoNewsItems[i].Id, Path.GetFileNameWithoutExtension(files[j].ToString())) + "\">" : "";
							if (compteur == 0) {
								t.Append("\n<tr><td width=50%><a href=\"" + sortedInfoNewsItems[i].VirtualPath + Path.GetFileName(files[j].ToString()) + "\" target=\"_blank\">" + logoPath + "</a>&nbsp;<a href=\"" + sortedInfoNewsItems[i].VirtualPath + Path.GetFileName(files[j].ToString()) + "\" class=\"roll02\" target=\"_blank\">" + FormatFileName(sortedInfoNewsItems[i].Id, Path.GetFileNameWithoutExtension(files[j].ToString())) + "</a></td>");
								compteur = 1;
							}
							else {
								t.Append("\n<td width=50%><a href=\"" + sortedInfoNewsItems[i].VirtualPath + Path.GetFileName(files[j].ToString()) + "\" target=\"_blank\">" + logoPath + "</a>&nbsp;<a href=\"" + sortedInfoNewsItems[i].VirtualPath + Path.GetFileName(files[j].ToString()) + "\" class=\"roll02\" target=\"_blank\">" + FormatFileName(sortedInfoNewsItems[i].Id, Path.GetFileNameWithoutExtension(files[j].ToString())) + "</a></td></tr>");
								compteur = 0;
							}
							nbFilesAdded++;
							//Limitation of Nb files to show
							//if (sortedInfoNewsItems[i].NbMaxItemsToShow>-1 && (j+1) == sortedInfoNewsItems[i].NbMaxItemsToShow) break;
							
						}
						nbFilesAdded = 0;
						if (compteur == 1) {
							t.Append("\n<td>&nbsp;</td></tr>");
						}
						t.Append("\n</table>");
						t.Append("</div>");
					}

				}
			}
			return t.ToString();
		}
		#endregion

		#region methods

		/// <summary>
		/// Get directory files list 
		/// </summary>
		/// <param name="pathDirectory">Directory path</param>
		/// <returns>Liste des fichiers</returns>
		protected virtual string[] GetDirectoryFiles(string pathDirectory) {
			string[] filesList = Directory.GetFiles(pathDirectory);
			return (filesList);
		}

		/// <summary>
		/// Get file logo extension
		/// </summary>
		/// <param name="extension"></param>
		/// <returns></returns>
		protected virtual string GetFileLogoExtension(string extension) {
			string logo = "";
			switch (extension.ToUpper()) {
				case ".PDF":
					logo = "file_pdf.gif";
					break;
				case ".DOC":
				case ".DOCX":
					logo = "file_word.gif";
					break;
				case ".XLS":
				case ".XLSX":
					logo = "file_excel.gif";
					break;
				case ".PPT":
				case ".PPTX":
					logo = "file_powerpoint.gif";
					break;
			}
			return logo;
		}

		/// <summary>
		/// Formatage du nom du fichier en libellé MOIS ANNEE
		/// </summary>
		/// <param name="name">Nom de la plaquette</param>
		/// <param name="fileName">Nom du fichier</param>
		/// <param name="webSession">Session</param>
		/// <returns>Libellé</returns>
		protected virtual string FormatFileName(ModuleInfosNews.Directories idDirectory, string fileName) {
			return fileName;
		}
		#endregion
	}
}
