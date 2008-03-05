#region Informations
// Auteur: G. Facon
// Date de création: 28/06/2004
// Date de modification: 06/07/2004
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.ComponentModel;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;



namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Contrôle qui permet la navigation dans un module
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:RecallWebControl runat=server></{0}:RecallWebControl>")]
	public class RecallWebControl : System.Web.UI.WebControls.WebControl{
	
		#region Variables
		/// <summary>
		/// Liste des liens à montrer
		/// </summary>
		protected ArrayList linkToShow=null;		
		/// <summary>
		/// Session du client (utile pour la langue)
		/// </summary>
		protected WebSession webSession = null;
		/// <summary>
		/// Url suivante
		/// </summary>
		protected string nextUrl="";
		/// <summary>
		/// Le client est authorisé à aller au résultat
		/// </summary>
		protected bool canGoToResult=false;
		#endregion

		#region Accesseurs

		/// <summary>
		/// Obtient ou définit les liens à montrer
		/// </summary>
		public ArrayList LinkToShow{
			get{return linkToShow;}
			set{linkToShow=value;}
		}

		/// <summary>
		/// Session du client
		/// </summary>
		public WebSession CustomerWebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// Obtient l'adresse de la page suivante
		/// </summary>
		public string NextUrl{
			get{return(this.nextUrl);}
		}

		/// <summary>
		/// Obtient ou définit si le client peux aller au résultat
		/// </summary>
		public bool CanGoToResult{
			get{return(canGoToResult);}
			set{this.canGoToResult=value;}
		}



		#endregion


		/// <summary>
		/// Affichage page option
		/// </summary>
		[Bindable(false),
		Description("Option choix de l'unité")]
		protected bool optionalPage = false;
		/// <summary></summary>
		public bool OptionalPage{
			get{return optionalPage;}
			set{optionalPage=value;}
		}


		#region Evènements

		#region Initialisation
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			base.OnInit (e);
			if(this.Page.IsPostBack){
				string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
				if(nomInput==this.ID){
					string valueInput=Page.Request.Form.GetValues("__EVENTARGUMENT")[0];
					Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
					foreach(SelectionPageInformation currentPage in currentModuleDescription.SelectionsPages){
						if(currentPage.Id==int.Parse(valueInput)){
							nextUrl=currentPage.Url;
							break;
						}
					}
					foreach(OptionalPageInformation currentPage in currentModuleDescription.OptionalsPages){
						if(currentPage.Id==int.Parse(valueInput)){
							nextUrl=currentPage.Url;
							break;
						}
					}

					if(int.Parse(valueInput)==9999)nextUrl=webSession.LastReachedResultUrl;
				}
			}
		}
		#endregion

		#region Prérendu

		/// <summary>
		/// Prérendu du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if(!Page.ClientScript.IsClientScriptBlockRegistered("ScriptRecallpopup")){
				string script="<script language=\"JavaScript\"> ";
				script+="function popupRecallOpen(page,width,height){";
				script+="	window.open(page,'','width='+width+',height='+height+',toolbar=no,scrollbars=yes,resizable=no');";
				script+="}";
				script+="</script>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ScriptRecallpopup",script);
			}
		}
		#endregion

		#region Rendu

		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			string HTML="";
			string HTMLLinks="";

			string iconeName="";
			string resultHTML="";
			string endBaliseHTML="";
			Int64 textCode=0;
			bool linkExist=false;
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" bgcolor=\"#FFFFFF\">");
			output.Write("\n<tr>");
			output.Write("\n<td>");
			//debut tableau titre
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");
			output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/Images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\"/Images/Common/block_fleche.gif\" width=\"12\"></td>");
			output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" background=\"/Images/Common/block_dupli.gif\"><IMG height=\"1\" src=\"/Images/Common/pixel.gif\" width=\"13\"></td>");
			output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"+GestionWeb.GetWebWord(795,webSession.SiteLanguage)+"</td>");
			output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/Images/pixel.gif\" width=\"1\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td></td>");
			output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n</table>");
			//fin tableau titre
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");

			output.Write("\n<td>");
			output.Write("\n<table cellspacing=1 cellpading=0 border=0 bgcolor=\"#FFFFFF\">");
			output.Write("\n<tr>");
			output.Write("\n<td><img src=\"/Images/Common/pixel.gif\" width=\"7px\" height=\"1px\"></td>");
			output.Write("\n<td><a class=\"roll03\" href=\"javascript:popupRecallOpen('/Private/Selection/DetailSelection.aspx?idSession="+ webSession.IdSession +"','660','700');\"  onmouseover=\"loupeButton.src='/Images/Common/button/loupe_down.gif';\" onmouseout=\"loupeButton.src ='/Images/Common/button/loupe_up.gif';\"><img name=loupeButton border=0 src=\"/Images/Common/button/loupe_up.gif\" alt=\""+ GestionWeb.GetWebWord(853,webSession.SiteLanguage) +"\"></a>");		
			output.Write("\n</tr>");
			output.Write("\n</table>");
			output.Write("\n</td>");

			//output.Write("\n<td><A class=\"roll03\" href=\"javascript:popupRecallOpen('/Private/Selection/DetailSelection.aspx?idSession="+ webSession.IdSession +"','660','700');\">&gt;&nbsp;"+GestionWeb.GetWebWord(853,webSession.SiteLanguage)+"</td>");					
			output.Write("\n</TR>");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			HTML+="\n<tr>";
			HTML+="\n<td class=\"txtGris11Bold\">";
			HTML+=GestionWeb.GetWebWord(796,webSession.SiteLanguage);
			HTML+="\n</td>";
			HTML+="\n</tr>";
			HTML+="\n<TR>";
			HTML+="\n<TD height=\"5\"></TD>";
			HTML+="\n</TR>";

			//Exemple, a changer ultérieurement
			//webSession.CustomerLogin.moduleList();
			bool premier=true;
			Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
			foreach(SelectionPageInformation currentPage in currentModuleDescription.SelectionsPages){
				if(premier){
					HTMLLinks+="\n<tr><td><table cellspacing=5 cellpadding=0 bgcolor=#FFFFFF width=\"100%\">";
					HTMLLinks+="\n<tr>";
					HTMLLinks+="\n<td><img src=\"/Images/Common/pixel.gif\" width=\"7px\" height=\"1px\"></td>";
					premier=false;
				}
				if((linkToShow==null || linkToShow.Contains(currentPage.Id))&&currentPage.ShowLink){
					linkExist=true;
					iconeName=currentPage.IconeName;
					textCode=currentPage.IdWebText;
					HTMLLinks+="\n<td><A class=\"roll03\" href=\"javascript:__doPostBack('"+this.ID+"','"+currentPage.Id.ToString()+"');\"onmouseover=\""+iconeName+"Button.src='/Images/Common/button/"+iconeName+"_down.gif';\" onmouseout=\""+iconeName+"Button.src ='/Images/Common/button/"+iconeName+"_up.gif';\"><img name="+iconeName+"Button border=0 src=\"/Images/Common/button/"+iconeName+"_up.gif\" alt=\""+GestionWeb.GetWebWord(int.Parse(textCode.ToString()),webSession.SiteLanguage)+"\"></a>";	
					HTMLLinks+="\n</td>";
				}
			}
			if(!premier){
				HTMLLinks+="\n<td width=100%><img src=\"/Images/Common/pixel.gif\"></td>";
				HTMLLinks+="\n</tr>\n</table></td></tr>";
			}
			if(canGoToResult){
				resultHTML+="\n<tr>";
				resultHTML+="\n<td class=\"txtGris11Bold\">";
				resultHTML+=GestionWeb.GetWebWord(975,webSession.SiteLanguage);
				resultHTML+="\n</td>";
				resultHTML+="\n</tr>";
				resultHTML+="\n<tr>";
				resultHTML+="\n<td>";
				resultHTML+="\n<table cellspacing=5 cellpadding=0 bgcolor=#FFFFFF width=\"100%\">";
				resultHTML+="\n<tr>";
				resultHTML+="\n<td><img src=\"/Images/Common/pixel.gif\" width=\"7px\" height=\"1px\"></td>";
				resultHTML+="\n<td><A class=\"roll03\" href=\"javascript:__doPostBack('"+this.ID+"','9999');\" onmouseover=\"resultButton.src='/Images/Common/button/result_down.gif';\" onmouseout=\"resultButton.src ='/Images/Common/button/result_up.gif';\"><img name=resultButton border=0 src=\"/Images/Common/button/result_up.gif\" alt=\""+GestionWeb.GetWebWord(871,webSession.SiteLanguage)+"\"></a>";							
				resultHTML+="\n</td>";
				resultHTML+="\n<td width=100%><img src=\"/Images/Common/pixel.gif\"></td>";
				resultHTML+="\n</tr>";
				resultHTML+="\n</table>";
			}
			endBaliseHTML+="\n<TR>";
			endBaliseHTML+="\n<TD height=\"5\"></TD>";
			endBaliseHTML+="\n</TR>";
			if(linkExist){
				output.Write(HTML);
				output.Write(HTMLLinks);
			}
			if(canGoToResult){
				output.Write(resultHTML);
			}
			output.Write(endBaliseHTML);



			HTML="";
			HTMLLinks="";
			premier=true;
			linkExist=false;

			if(optionalPage){
				output.Write("\n</TR>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
				HTML+="\n<tr>";
				HTML+="\n<td class=\"txtGris11Bold\">";
				HTML+=GestionWeb.GetWebWord(1129,webSession.SiteLanguage);
				HTML+="\n</td>";
				HTML+="\n</tr>";
				HTML+="\n<TR>";
				HTML+="\n<TD height=\"5\"></TD>";
				HTML+="\n</TR>";

				
				foreach(OptionalPageInformation currentPage in currentModuleDescription.OptionalsPages){
					if(premier){
						HTMLLinks+="\n<tr><td><table cellspacing=5 cellpadding=0 bgcolor=#FFFFFF width=\"100%\">";
						HTMLLinks+="\n<tr>";
						HTMLLinks+="\n<td><img src=\"/Images/Common/pixel.gif\" width=\"7px\" height=\"1px\"></td>";
						premier=false;
					}
					linkExist=true;
					iconeName=currentPage.IconeName;
					textCode=currentPage.IdWebText;
					HTMLLinks+="\n<td><A class=\"roll03\" href=\"javascript:__doPostBack('"+this.ID+"','"+currentPage.Id.ToString()+"');\"onmouseover=\""+iconeName+"Button.src='/Images/Common/button/"+iconeName+"_down.gif';\" onmouseout=\""+iconeName+"Button.src ='/Images/Common/button/"+iconeName+"_up.gif';\"><img name="+iconeName+"Button border=0 src=\"/Images/Common/button/"+iconeName+"_up.gif\" alt=\""+GestionWeb.GetWebWord(int.Parse(textCode.ToString()),webSession.SiteLanguage)+"\"></a>";	
					HTMLLinks+="\n</td>";				
				}
				if(!premier){
					HTMLLinks+="\n<td width=100%><img src=\"/Images/Common/pixel.gif\"></td>";
					HTMLLinks+="\n</tr>\n</table></td></tr>";
				}
			
				
				HTMLLinks+="\n<tr>";
				HTMLLinks+="\n<td>";
				
			
				HTMLLinks+="\n<TR>";
				HTMLLinks+="\n<TD height=\"5\"></TD>";
				HTMLLinks+="\n</TR>";
			
			
			}
			if(linkExist){
				output.Write(HTML);
				output.Write(HTMLLinks);
			}

			output.Write("</table>");
			
		}
		#endregion

		#endregion
	}
}
