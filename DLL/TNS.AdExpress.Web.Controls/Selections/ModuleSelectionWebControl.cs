using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Contrôle affichant la liste des Modules
	/// </summary>
	
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:ModuleSelectionWebControl runat=server></{0}:ModuleSelectionWebControl>")]
	public class ModuleSelectionWebControl :System.Web.UI.WebControls.WebControl{
	
		#region Variables
		/// <summary>
		/// WebSession
		/// </summary>
		protected WebSession webSession=null;
		/// <summary>
		/// Identifiant de la Session
		/// </summary>
		protected string idSession;

		#endregion

		#region Accesseurs
		/// <summary>
		/// Définit l'identifiant de la session
		/// </summary>
		public virtual WebSession CustomerSession{
			set{this.webSession=value;}
		}
		
		#endregion

		#region Evènements

		#region PréRendu
		/// <summary>
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if (!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")){
				string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"detectFlash",tmp);
			}

			base.OnPreRender (e);
		}
		#endregion

		#region Affichage
		/// <summary>
		/// Génère un flux HTML affichant la liste des Modules 
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output){
			
			// Liste Globale
			string HTML="";
			// Partie 1 des groupes Modules
			string HTML1="";
			// Liste des modules'
			string HTML2="";
			// Partie 2 des groupes Modules
			string HTML3="";
			int start =0;

			if(webSession!=null){
				Int64 idGroupModuleOld=-1;
				Int64 idGroupModule;
				Int64 idModule;			

				HTML+="<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">";
				HTML+="	<TBODY> ";
				HTML+=" <tr> ";
				HTML+=" <td vAlign=\"top\" rowSpan=\"20\"><IMG height=\"13\" src=\"../images/Common/fleche_1.gif\" width=\"13\"></td> ";
				HTML+="	<td bgColor=\"#ffffff\" rowSpan=\"20\"><IMG height=\"1\" src=\"../images/Common/pixel.gif\" width=\"8\"></td>";
				HTML+="	<td bgColor=\"#ffffff\"><IMG height=\"8\" src=\"../images/Common/pixel.gif\" width=\"1\"></td>";
				HTML+="	<td bgColor=\"#ffffff\" rowSpan=\"20\"><IMG height=\"1\" src=\"../images/Common/pixel.gif\" width=\"8\"></td>";
				HTML+="	</tr>";

				// Parcours de la table des droits sur les modules
                DataTable dt=webSession.CustomerLogin.GetCustomerModuleListHierarchy();
				foreach(DataRow currentRow in dt.Rows){
					idGroupModule=(Int64)currentRow["idGroupModule"];

					if(idGroupModuleOld!=idGroupModule){

						if(start==1) {
							HTML+=HTML1+HTML2+HTML3;
							HTML1="";
							HTML2="";
							HTML3="";		 
						}

						start=0;
						HTML1+="	<tr> ";
						HTML1+="	<td><IMG id=\"IMG1\" height=\"8\" src=\"/images/Common/bande.gif\" width=\"400\"></td> ";
						HTML1+="	</tr> ";
						HTML1+="	<tr> ";
						HTML1+="	<td bgColor=\"#ffffff\"><IMG height=\"8\" src=\"/images/Common/pixel.gif\" width=\"1\"></td> ";
						HTML1+="	</tr> ";
						HTML1+="	<tr> ";
						HTML1+="	<td vAlign=\"top\" bgColor=\"#ffffff\"> ";
						HTML1+="	<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\"> ";
						HTML1+="	<tr> ";
						HTML1+="	<td vAlign=\"top\"> ";
						//NE PAS METTRE DE SOIMPLE QUOTE DANS LE CODE HTML A PARTIR DE LA!!!!!!
						HTML1+="	\n<script language=\"JavaScript\" type=\"text/javascript\">\nif(hasRightFlashVersion==true){\n";
						#region Objet Flash
						HTML1+="	\ndocument.write('<OBJECT codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" ');";
						HTML1+="	\ndocument.write('height=\"62\" width=\"90\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"_cx\" VALUE=\"2381\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"_cy\" VALUE=\"1640\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"FlashVars\" VALUE=\"\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Movie\" VALUE=\""+ModulesList.GetModuleGroupFlash(idGroupModule)+"\">');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Src\" VALUE=\""+ModulesList.GetModuleGroupFlash(idGroupModule)+"\"> ');";
					
				
						HTML1+="	\ndocument.write('<PARAM NAME=\"WMode\" VALUE=\"Window\">');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Play\" VALUE=\"-1\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Loop\" VALUE=\"-1\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Quality\" VALUE=\"High\">');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"SAlign\" VALUE=\"\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Menu\" VALUE=\"-1\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Base\" VALUE=\"\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"AllowScriptAccess\" VALUE=\"always\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"Scale\" VALUE=\"ShowAll\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"DeviceFont\" VALUE=\"0\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"EmbedMovie\" VALUE=\"0\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"BGColor\" VALUE=\"\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"SWRemote\" VALUE=\"\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"MovieData\" VALUE=\"\"> ');";
						HTML1+="	\ndocument.write('<PARAM NAME=\"SeamlessTabbing\" VALUE=\"1\"> ');";
						HTML1+="	\ndocument.write('<embed src=\""+ModulesList.GetModuleGroupFlash(idGroupModule)+"\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" ');";
						HTML1+="	\ndocument.write('type=\"application/x-shockwave-flash\" width=\"90\" height=\"62\"> </embed> ');";
						HTML1+="	\ndocument.write('</OBJECT>'); ";
						HTML1+="	}\nelse{";
						//int tmp;
						//HTML1+="	document.write('<img src=\"/Images/Common/FlashReplacement"+ModulesListRules.getModuleGroupFlash(idGroupModule).Substring(tmp = ModulesListRules.getModuleGroupFlash(idGroupModule).LastIndexOf("/"),ModulesListRules.getModuleGroupFlash(idGroupModule).Length-tmp).Replace("swf","gif")+"\" width=\"90\" height=\"62\">');}";
						HTML1+="	document.write('<img src="+ModulesList.GetMissingModuleGroupFlash(idGroupModule)+" width=\"90\" height=\"62\">');}";
						#endregion
						HTML1+="	\n</script>\n</td> ";
						HTML1+="	<td><IMG height=\"1\" src=\"/images/Common/pixel.gif\" width=\"10\"></td> ";
						HTML1+="	<td vAlign=\"top\"> ";
						HTML1+=" <table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" width=300> ";
						HTML1+=" <tr> ";
						HTML1+=" <td class=\"txtViolet11Bold\" colSpan=\"2\">"+ GestionWeb.GetWebWord((int)ModulesList.GetModuleGroupIdWebTxt(idGroupModule),webSession.SiteLanguage)+"</td> ";
						HTML1+="	</tr> ";	
						HTML1+="<tr>";
						HTML1+=" <td class=\"txtViolet10\" colSpan=\"2\">"+ GestionWeb.GetWebWord((int)ModulesList.GetModuleGroupDescriptionWebTextId(idGroupModule),webSession.SiteLanguage)+"</td> ";
						HTML1+="</tr>";
					}
					idModule=(Int64)currentRow["idModule"];
					HTML2+="	<tr> ";
					HTML2+="	<td width=\"20\"><IMG height=\"1\" src=\"/images/Common/pixel.gif\" width=\"20\"></td> ";
					HTML2+="	<td width=\"280\"><IMG height=\"1\" src=\"/images/Common/pixel.gif\" width=\"280\"></td> ";
					HTML2+="	</tr> ";
					HTML2+="	<tr> ";
					HTML2+="	<td height=\"13\"><IMG height=\"13\" src=\"/images/Common/fleche_2.gif\" width=\"13\"></td> ";
					HTML2+="	<td> ";					
					HTML2+="	<p>&nbsp;<A class=\"roll02\" href=\""+this.Parent.Page.Request.RawUrl.ToString()+"&m="+idModule+"\">"+GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(idModule),webSession.SiteLanguage)+"</A></p> ";
					HTML2+="	</td> ";
					HTML2+="	</tr> ";				
					
					if(idGroupModuleOld!=idGroupModule){
						
						
						HTML3+="	</table> ";
						HTML3+="	</td> ";
						HTML3+="	</tr> ";
						HTML3+="<tr><td height=\"13\" width=\"13\"></td></tr>";
						HTML3+="	</table> ";	
						HTML3+="	</td> ";
						HTML3+="	</tr> ";
						//HTML3+="	<tr> ";
						idGroupModuleOld=idGroupModule;
						start=1;
					}
				}
				
				HTML+=HTML1+HTML2+HTML3;			
				
				HTML+="	<tr> ";
				HTML+=" <td bgColor=\"#ffffff\"><IMG height=\"8\" src=\"../images/Common/pixel.gif\" width=\"1\"></td> ";
				HTML+="	</tr> ";
				HTML+="	</TBODY> ";
				HTML+="	</table> ";

			}else{HTML="Liste des modules";}

			

			
			output.Write(HTML);
		}
		#endregion

		#endregion


	}
}
