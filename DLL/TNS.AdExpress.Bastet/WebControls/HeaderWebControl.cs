using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using TNS.AdExpress.Bastet.Common.Headers;

namespace TNS.AdExpress.Bastet.WebControls{
	
	/// <summary>
	/// Type d�fini pour la s�lection d'un type de page
	/// </summary>
	public enum PageType{
		/// <summary>
		/// Home Page
		/// </summary>
		homepage
	}

	/// <summary>
	/// Description r�sum�e de HeaderWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:HeaderWebControl runat=server></{0}:HeaderWebControl>")]
	public class HeaderWebControl : System.Web.UI.WebControls.WebControl{
		
		#region Variables
		/// <summary>
		/// Langage du site
		/// </summary>
		private int language;
		///<summary>
		/// Type de page
		/// </summary>
		///  <supplierCardinality>1</supplierCardinality>
		///  <directed>True</directed>
		///  <label>pageType</label>
		private PageType pageType;
		/// <summary>
		/// Indique le menu qui est actif
		/// </summary>
		private int activeMenu=-1;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou d�finit propri�t� de langage
		/// </summary>
		[Bindable(true),
		Description("Langue du site")]
		public int Language{
			get{return language;}
			set{language = value;}
		}

		/// <summary>
		/// Obtient ou d�finit le type de page d�terminant pour les menus pr�sents
		/// </summary>
		[Description("Page d'accueil ou page g�n�rique")]
		public PageType Type_de_page{
			get{return pageType;}
			set{pageType = value;}
		}

		/// <summary>
		/// Obtient ou d�fnit le menu qui est actif
		/// </summary>
		[Description("Sp�cifie l'index du menu actif(0...)"),
		DefaultValue(0)]
		public int ActiveMenu{
			get{return activeMenu;}
			set{activeMenu = value;}
		}
		#endregion

		#region Pr�Rendu
		/// <summary>
		/// Pr�Rendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if (!Page.IsClientScriptBlockRegistered("detectFlash")){
				string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/Scripts/FlashChecking.js\"></SCRIPT>";
				Page.RegisterClientScriptBlock("detectFlash",tmp);
			}
			base.OnPreRender (e);
		}
		#endregion

		#region Render (Affichage)
		/// <summary>
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output) {
			Hashtable headers = HeaderList.List;
			
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" width=\"100%\">");
			output.Write("\n<tr>");
			output.Write("\n<td colspan=\"2\">");
			output.Write("\n<table  class=\"header\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			output.Write("\n<td width=\"1%\">");
			output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
			output.Write("\nif(hasRightFlashVersion==true){");
			output.Write("\ndocument.writeln('<OBJECT id=\"Shockwaveflash1\" codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\"');");
			output.Write("\ndocument.writeln('height=\"31\" width=\"733\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" VIEWASTEXT>');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cx\" VALUE=\"19394\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cy\" VALUE=\"820\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"FlashVars\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Movie\" VALUE=\""+((Header)headers[pageType.ToString()]).FlashUrl+"\"');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Src\" VALUE=\""+((Header)headers[pageType.ToString()]).FlashUrl+"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"WMode\" VALUE=\"Window\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Play\" VALUE=\"-1\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Loop\" VALUE=\"-1\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Quality\" VALUE=\"High\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SAlign\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Menu\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Base\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"AllowScriptAccess\" VALUE=\"always\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Scale\" VALUE=\"ShowAll\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"DeviceFont\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"EmbedMovie\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"BGColor\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SWRemote\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"MovieData\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SeamlessTabbing\" VALUE=\"1\">');");
			output.Write("\ndocument.writeln('<embed src=\""+((Header)headers[pageType.ToString()]).FlashUrl+"\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\"');");
			output.Write("\ndocument.writeln('type=\"application/x-shockwave-flash\" width=\"733\" height=\"31\"> </embed>');");
			output.Write("\ndocument.writeln('</OBJECT>');");
			output.Write("\n}\nelse{");
			output.Write("\ndocument.writeln('<img src=\""+((Header)headers[pageType.ToString()]).MissingFlashUrl+"\" width=\"733\" height=\"31\">');");
			output.Write("\n}");
			output.Write("\n</script>");
			output.Write("\n</td>");
			output.Write("\n<td></td>");
			output.Write("\n</table>");//
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td colspan=\"2\" background=\"/Images/Common/dupli_1.gif\"><IMG height=\"1\" src=\"/Images/pixel.gif\" width=\"1\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr class=\"txtBlanc11\" style=\"BACKGROUND-IMAGE: url("+((Header)headers[pageType.ToString()]).ImgUrl+")\">");
			output.Write("\n<td>");
			output.Write("\n<p style=\"PADDING-LEFT: 10px; PADDING-BOTTOM: 40px; PADDING-TOP: 4px\">");
			int i=0;
			string menus="";
			string href="";
			string look="";
			string languageString,idSessionString; 
			bool firstParameter;
			HeaderMenuItem currentHeaderMenuItem=null;
			for(i=0; i<((Header)headers[pageType.ToString()]).MenuItems.Count; i++){
				languageString="";
				idSessionString="";				
				firstParameter=true;
				currentHeaderMenuItem=((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]);
				//Options dans l url suivant le menuItem
				//Differenciation et inactivation du menu actif si n�cessaire
				if(activeMenu==currentHeaderMenuItem.Id){
					href="#";
					look="roll03";
				}
				else{
					look="roll01";

					#region Gestion des Param�tres

//					#region Param�tre de la langue
//					if(currentHeaderMenuItem.GiveLanguage){
//						languageString="siteLanguage="+language;
//						if(firstParameter){
//							languageString="?"+languageString;
//							firstParameter=false;
//						}
//						else{
//							languageString="&"+languageString;
//						}
//					}
//					#endregion
//
//					#region Param�tre de l'identifiant de la session
//					if(currentHeaderMenuItem.GiveSession){
//						idSessionString="idSession="+Page.Request.QueryString.Get("idSession").ToString();
//						if(firstParameter){
//							idSessionString="?"+idSessionString;
//							firstParameter=false;
//						}
//						else{
//							idSessionString="&"+idSessionString;
//						}
//					}
//					#endregion

					#endregion

					href = ((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]).TargetUrl + languageString + idSessionString;
					//if(currentHeaderMenuItem.Target.Length>0)href+="\" target=\""+currentHeaderMenuItem.Target+"\"";
				}
				//menus += "\n<A class=\""+look+"\" href=\""+href+"\">"+GestionWeb.GetWebWord((int)((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]).IdMenu, language)+"</A> |";
				menus += "\n<A class=\""+look+"\" href=\""+href+"\">"+((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]).Text+"</A> |";
			}
			menus = menus.Substring(0, menus.Length-2);
			output.Write("{0}\n</p>",menus);
			output.Write("\n</td>");
			
//			// A remettre s'il on veut pouvoir avoir acc�s � la traduction du site
//			if (pageType==PageType.homepage){ 
//				string langButton="";
//				string link="";
//				switch (language){
//					case TNS.AdExpress.Constantes.DB.Language.ENGLISH:
//						langButton = "Version fran�aise";
//						link= this.Parent.Page.Request.Url.AbsolutePath.ToString()+"?siteLanguage="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
//						break;
//					default:
//						langButton = "English version";
//						link= this.Parent.Page.Request.Url.AbsolutePath.ToString()+"?siteLanguage="+TNS.AdExpress.Constantes.DB.Language.ENGLISH;
//						break;
//				}
//				output.Write("\n<td align=\"right\">\n<p style=\"PADDING-LEFT: 10px; PADDING-BOTTOM: 40px; PADDING-TOP: 4px\">| <a class=\"roll01\" href=\""+link+"\">"+langButton+" </a></p>\n</td>");
//			}

			output.Write("\n</tr>");
			output.Write("\n</table>");

		}
		#endregion

	}
}