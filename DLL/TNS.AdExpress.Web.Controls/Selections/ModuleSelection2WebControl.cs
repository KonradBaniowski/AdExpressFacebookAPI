#region Informations
// Auteur: B.Masson, G.Facon
// Date de création : 15/01/2007
// Date de modification :
// 02/08/2007 Par B.Masson > Changement de l'info bulle
#endregion

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Text;
using Oracle.DataAccess.Client;
using TNS.AdExpress;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.Controls.Selections {
	/// <summary>
	/// Composant d'affichage des modules
	/// </summary>
	[DefaultProperty("Text"),
		ToolboxData("<{0}:ModuleSelection2WebControl runat=server></{0}:ModuleSelection2WebControl>")]
	public class ModuleSelection2WebControl : System.Web.UI.WebControls.WebControl {

		#region Variables
		/// <summary>
		/// WebSession
		/// </summary>
		protected WebSession webSession = null;
		/// <summary>
		/// Identifiant de la Session
		/// </summary>
		protected string idSession;
		/// <summary>
		/// URL du fichier CSS
		/// </summary>
		protected string _cssUrlPath = string.Empty;
		/// <summary>
		/// URL de l'image flèche devant les modules
		/// </summary>
		protected string _imageModuleUrlPath = string.Empty;
		/// <summary>
		/// Css des modules
		/// </summary>
		protected string _moduleCss = string.Empty;
		/// <summary>
		/// Css des groupes de modules
		/// </summary>
		protected string _moduleGroupCss = string.Empty;
		/// <summary>
		/// Css des groupes de modules avec une couleur de fond
		/// </summary>
		protected string _moduleGroupWithBackgroundCss = string.Empty;
		/// <summary>
		/// Css des sous groupes de modules
		/// </summary>
		protected string _moduleSubGroupCss = string.Empty;
		/// <summary>
		/// Css du texte d'information
		/// </summary>
		protected string _moduleGroupInformationCss = string.Empty;
		/// <summary>
		/// Taille de la colonne de gauche
		/// </summary>
		protected Unit _columnLeftWidth = new Unit("10px");
		/// <summary>
		/// Taille de la colonne de droite
		/// </summary>
		protected Unit _columnInformationWidth = new Unit("45%");
        /// <summary>
        /// Description images list
        /// </summary>
        protected string _descriptionImageList = string.Empty;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Définit l'identifiant de la session
		/// </summary>
		public virtual WebSession CustomerSession {
			set { this.webSession = value; }
		}
		/// <summary>
		/// Obtient ou défini le lien du fichier css
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string CssUrlPath {
			get { return (_cssUrlPath); }
			set { _cssUrlPath = value; }
		}
		/// <summary>
		/// Obtient ou défini l'image flèche devant les modules
		/// </summary>
		[Bindable(true),
		Category("Image"),
		DefaultValue("")]
		public string ImageModuleUrlPath {
			get { return (_imageModuleUrlPath); }
			set { _imageModuleUrlPath = value; }
		}
		/// <summary>
		/// Obtient ou défini le css des modules
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string ModuleCss {
			get { return (_moduleCss); }
			set { _moduleCss = value; }
		}
		/// <summary>
		/// Obtient ou défini le css des groupes de modules
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string ModuleGroupCss {
			get { return (_moduleGroupCss); }
			set { _moduleGroupCss = value; }
		}
		/// <summary>
		/// Obtient ou défini le css des groupes de modules avec une couleur de fond
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string ModuleGroupWithBackgroundCss {
			get { return (_moduleGroupWithBackgroundCss); }
			set { _moduleGroupWithBackgroundCss = value; }
		}
		/// <summary>
		/// Obtient ou défini le css des sous groupes de modules
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string ModuleSubGroupCss {
			get { return (_moduleSubGroupCss); }
			set { _moduleSubGroupCss = value; }
		}
		/// <summary>
		/// Obtient ou défini le css des informations des groupes de modules
		/// </summary>
		[Bindable(true),
		Category("Apparence"),
		DefaultValue("")]
		public string ModuleGroupInformationCss {
			get { return (_moduleGroupInformationCss); }
			set { _moduleGroupInformationCss = value; }
		}

		/// <summary>
		/// Obtient ou défini la taille de la colonne de gauche
		/// </summary>
		[Bindable(true),
		Category("Disposition"),
		DefaultValue("")]
		public Unit ColumnLeftWidth {
			get { return (_columnLeftWidth); }
			set { _columnLeftWidth = value; }
		}
		/// <summary>
		/// Obtient ou défini la taille de la colonne de droite
		/// </summary>
		[Bindable(true),
		Category("Disposition"),
		DefaultValue("45%")]
		public Unit ColumnInformationWidth {
			get { return (_columnInformationWidth); }
			set { _columnInformationWidth = value; }
		}
        /// <summary>
        /// Get or Set the description images list
        /// </summary>
        [Bindable(true),
		Category("Apparence"),
		DefaultValue(""),
        ThemeableAttribute(true)]
        public string DescriptionImageList {
            get { return (_descriptionImageList); }
            set { _descriptionImageList = value; }
        }
		#endregion

		#region Evènements

		#region PréRendu
		/// <summary>
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
            if(!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")) {
				string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "detectFlash", tmp);
			}
            if(!Page.ClientScript.IsClientScriptBlockRegistered("moduleSelectionCss")) {
				if (_cssUrlPath.Length > 0) {
					string tmp2 = "\n<LINK href=\"" + _cssUrlPath + "\" type=\"text/css\" rel=\"stylesheet\">";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "moduleSelectionCss", tmp2);
				}
			}
            if(!Page.ClientScript.IsClientScriptBlockRegistered("mootoolsCore")) {
                string tmp3 = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/mootools.v1.11.js\"></SCRIPT>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "mootoolsCore", tmp3);
            }
            if(!Page.ClientScript.IsClientScriptBlockRegistered("mootoolsTips")) {
                string tmp4 = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/mootools.Tips.js\"></SCRIPT>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "mootoolsTips", tmp4);
            }
			base.OnPreRender(e);
		}
		#endregion

		#region Rendu
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {

			string moduleGroupInformation = "";
			string htmlModuleGroups = "";
			string htmlModules = "";
			string html = "";
			bool first = true;
            Hashtable descriptionImagesModulesList = new Hashtable();
            string[] imageModuleList = null;
            string[] imageModuleString = null;

			if (webSession != null) {
				Int64 idGroupModuleOld = -1;
				Int64 idGroupModule;
				Int64 idModuleCategoryOld = -1;
				Int64 idModuleCategory = -1;
				Int64 idModule;
				string moduleLabel = null;

                imageModuleList = _descriptionImageList.Split(',');
                foreach (string imageModule in imageModuleList) {
                    imageModuleString = imageModule.Split(':');
                    descriptionImagesModulesList.Add(Convert.ToInt64(imageModuleString[0]), imageModuleString[1]);
                }

				html += "\r\n<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"" + this.Width.ToString() + "\">";
                DataTable dt=webSession.CustomerLogin.GetCustomerModuleListHierarchy();
                foreach(DataRow currentRow in dt.Rows) {
					idGroupModule = (Int64)currentRow["idGroupModule"];
                    idModule = (Int64)currentRow["idModule"];
                    if (ModulesList.GetModule(idModule) != null) {
                        if (idGroupModuleOld != idGroupModule) {
                            html += htmlModuleGroups + htmlModules + moduleGroupInformation;
                            htmlModuleGroups = "";
                            htmlModules = "";
                            moduleGroupInformation = "";
                            // On réinitialise les catégories qd on change de groupe de module
                            idModuleCategory = -1;
                            idModuleCategoryOld = -1;
                        }
                        if (currentRow["idModuleCategory"] != System.DBNull.Value)
                            idModuleCategory = (Int64)currentRow["idModuleCategory"];
                        else
                            idModuleCategory = -1;

                        if (htmlModuleGroups.Length == 0) {
                            // Groupe de module
                            htmlModuleGroups += "\r\n<tr>";
                            htmlModuleGroups += "\r\n<td width=\"" + _columnLeftWidth.ToString() + "\">&nbsp;</td>";
                            htmlModuleGroups += "\r\n<td " + ((_moduleGroupCss.Length > 0) ? " class=\"" + _moduleGroupCss + "\"" : "") + ">" + GestionWeb.GetWebWord((int)ModulesList.GetModuleGroupIdWebTxt(idGroupModule), webSession.SiteLanguage) + "</td>";
                            if (first) {
                                // Pour le tout 1er groupe de module, on ne met pas de couleur de fond violet
                                htmlModuleGroups += "\r\n<td " + ((_moduleGroupCss.Length > 0) ? " class=\"" + _moduleGroupCss + "\"" : "") + ">&nbsp;</td>";
                                first = false;
                            }
                            else {
                                // A partir du 2ème groupe de module, on appelle le CSS adapté avec couleur de fond
                                htmlModuleGroups += "\r\n<td " + ((_moduleGroupWithBackgroundCss.Length > 0) ? " class=\"" + _moduleGroupWithBackgroundCss + "\"" : "") + ">&nbsp;</td>";
                            }
                            htmlModuleGroups += "\r\n</tr>";

                            // Table des modules
                            htmlModules += "\r\n<tr><td>&nbsp;</td><td><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
                        }

                        // Catégorie de modules
                        if (idModuleCategoryOld != idModuleCategory && idModuleCategory != -1) {
                            idModuleCategoryOld = idModuleCategory;
                            htmlModules += "\r\n<tr><td>&nbsp;&nbsp;&nbsp;<font class=" + _moduleSubGroupCss + ">" + GestionWeb.GetWebWord((int)ModulesList.GetModuleCategoryWebTxt(idModuleCategory), webSession.SiteLanguage) + "</font></td></tr>";
                        }

                        // Module
                        moduleLabel = GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(idModule), webSession.SiteLanguage);
                        htmlModules += "\r\n<tr><td>&nbsp;&nbsp;" + ((_imageModuleUrlPath.Length > 0) ? "&nbsp;<img src=\"" + _imageModuleUrlPath + "\">" : "")
                            + "&nbsp;<a class=\"Tips1\" href=\"" + this.Parent.Page.Request.RawUrl.ToString() + "&m=" + idModule + "\"  "//onclick=\"return false;\" " 
                            + InformationModuleHtml(idModule, (string)descriptionImagesModulesList[idModule]) + " >" + moduleLabel + "</a></td></tr>";

                        if (idGroupModuleOld != idGroupModule && moduleGroupInformation.Length == 0) {
                            idGroupModuleOld = idGroupModule;
                            // Informations
                            moduleGroupInformation += "</table><br></td>"
                                + "<td width=\"" + _columnInformationWidth.ToString() + "\" valign=top " + ((_moduleGroupInformationCss.Length > 0) ? " class=\"" + _moduleGroupInformationCss + "\"" : "") + ">" + GestionWeb.GetWebWord((int)ModulesList.GetModuleGroupDescriptionWebTextId(idGroupModule), webSession.SiteLanguage) + "</td>"
                                + "</tr>";
                        }
                    }
				}
				if (htmlModuleGroups.Length > 0 && htmlModules.Length > 0) html += htmlModuleGroups + htmlModules + moduleGroupInformation;
				html += "</table>";
			}
			else {
				html += "Liste des modules";
			}
			output.Write(html);
		}
		#endregion

		#endregion

		#region Méthode privée
		/// <summary>
		/// Ecriture du code html pour l'affichage des informations d'un module (au passage de la souris)
		/// </summary>
		/// <param name="moduleId">Identifiant du module</param>
        /// <param name="descriptionImagePath">Description image path</param>
		/// <returns>HTML</returns>
        private string InformationModuleHtml(Int64 moduleId, string descriptionImagePath) {
            StringBuilder t = new StringBuilder(1000);

            string moduleLabel = GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(moduleId), webSession.SiteLanguage);
            string moduleDescription = GestionWeb.GetWebWord((int)ModulesList.GetModuleDescriptionWebTextId(moduleId), webSession.SiteLanguage);
            //string imagePath = ModulesList.GetModuleDescriptionImageName(moduleId);

            // Exemple : <a href="#" onclick="return false;" class="Tips" title="::<table bgcolor=#644883 border=0 cellpadding=0 cellspacing=1 width=100% class=moduleTableInfoBulle><tr><td colspan=2 class=title>Bilan forces/potentiels</td></tr><tr><td style=padding-left:10px><img src=images/06.png width=40 height=40></td><td><div align=justify>Diagnostiquer les points forts / faibles du portefeuille d\'un support: analyse des niveaux de part de marché de secteurs / annonceurs versus la moyenne</div></td></tr></table>">Tendances</a>

            t.Append(" title=\"::<table border=0 cellpadding=0 cellspacing=1 width=100% class=moduleTableInfoBulle>");
            t.Append("<tr>");
            t.Append("<td colspan=2 class=title>" + (Convertion.ToHtmlString(moduleLabel)).Replace("'", "\'") + "</td>");
            t.Append("</tr>");
            t.Append("<tr>");
            //t.Append("<td style=padding-left:10px>" + ((imagePath.Length > 0) ? "<img src=" + imagePath + ">" : "&nbsp;") + "</td>");
            t.Append("<td style=padding-left:10px>" + ((descriptionImagePath.Length > 0) ? "<img src=" + descriptionImagePath + ">" : "&nbsp;") + "</td>");
            t.Append("<td>");
            t.Append("<div align=justify>" + (Convertion.ToHtmlString(moduleDescription)).Replace("'", "\'") + "</div>");
            t.Append("</td>");
            t.Append("</tr>");
            t.Append("<tr><td colspan=2 height=5></td></tr>");
            t.Append("</table>\" ");

            return t.ToString();
        }
		#endregion

	}
}
