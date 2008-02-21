#region Informations
// Auteur: B.Masson
// Date de cr�ation: 27/09/2006
// Date de modification:
// Le 03/10/2006 Par B.Masson > Am�lioration du design pour le cas de l'image gif
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Translation;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Description r�sum�e de InformationWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:InformationWebControl runat=server></{0}:InformationWebControl>")]
	public class InformationWebControl : System.Web.UI.WebControls.WebControl,IOptionWebControl{

		#region Variables
		/// <summary>
		/// Langue des textes du composant
		/// </summary>
		protected int language=33;
		/// <summary>
		/// Indique si la position est � gauche ou non
		/// </summary>
		protected bool inLeftMenu = true;
		/// <summary>
		/// Couleur de fond du composant
		/// </summary>
		protected string _backgroundColor="#ffffff";
		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Obtient ou d�finit la langue des textes du composant
		/// </summary>
		[Bindable(true),
		Description("Langue du site"),
		DefaultValue(33)]
		public int  Language{
			get{return language;}
			set{language=value;}
		}
		/// <summary>
		/// Indique si le composant est dans la menu de gauche ou non
		/// </summary>
		[Bindable(true),
		Description("Emplacement du composant dans le menu de gauche")]
		public bool InLeftMenu{
			get{return inLeftMenu;}
			set{inLeftMenu=value;}
		}

		/// <summary>
		/// Obtient ou d�finit la couleur de fond du composant
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("#ffffff"),
		Description("Couleur de fond du composant")]
		public string BackGroundColor{
			get{return(_backgroundColor);}
			set{_backgroundColor=value;}
		}
		#endregion

		#region Ev�nements

		#region PreRender
		/// <summary>
		/// Pr�Rendu
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

		#region Render
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		/// <remarks>
		/// Le flash est transparent pour permettre d'avoir au 1er plan :
		/// Le menu contextuel du clic droit et les menus d�roulants des diff�rentes options � gauche
		/// C'est pour cela que le flash et l'image sont encapsul� dans un tableau avec une couleur de 
		/// fond rose
		/// </remarks>
		protected override void Render(HtmlTextWriter output){
			if(inLeftMenu){
				// Design du composant dans le menu de gauche
				output.Write("\n<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\""+_backgroundColor+"\">");
				output.Write("\n<tr><td><IMG height=10 src=\"/images/Common/pixel.gif\" width=\"1\"></td></tr>");
				output.Write("\n<tr><td>");

				output.Write("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#FF0099\"><tr><td>");
				output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
				output.Write("\nif(hasRightFlashVersion==true){");
				output.Write("\ndocument.writeln('<object id=\"infoOptionFlash\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"185\" height=\"48\" VIEWASTEXT>');");
				output.Write("\ndocument.writeln('<param name=\"movie\" value=\"/Flash/"+language+"/infoOptions.swf\">');");
				output.Write("\ndocument.writeln('<param name=\"quality\" value=\"high\">');");
				output.Write("\ndocument.writeln('<param name=\"menu\" value=\"false\">');");
				output.Write("\ndocument.writeln('<param name=\"wmode\" value=\"transparent\">');");
				output.Write("\ndocument.writeln('<embed src=\"/Flash/"+language+"/infoOptions.swf\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"185\" height=\"48\"></embed>');");
				output.Write("\ndocument.writeln('</object></td>');");
				output.Write("\n}\nelse{");
				output.Write("\ndocument.writeln('<img src=\"/Images/"+language+"/FlashReplacement/infoOptions.gif\"></td>');");
				output.Write("\n}");
				output.Write("\n</script>");
				/* Remarque pour la ligne ci-dessous :
				 * Le </TD> a �t� plac� document.writeln � la fin du flash et de l'image.
				 * Car dans le cas o� l'image est affich�e, le fait de ne pas avoir le </TD> � la fin
				 * rajoute un espace suppl�mentaire sous l'image, ce qui donne une grosse bordure rose
				 * en dessous, ce qui n'est pas esth�tique.
				 * */
				output.Write("</tr></table>");

				output.Write("\n</td></tr>");
				output.Write("\n<tr><td><IMG height=\"10\" src=\"/images/Common/pixel.gif\" width=\"1\"></td></tr>");
				output.Write("\n</table>");
			}
			else{
				// Design sur une ligne du composant situ� en haut des pops up
				output.Write("\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#DED8E5\">");
				output.Write("\n<tr><td>");
				output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
				output.Write("\nif(hasRightFlashVersion==true){");
				output.Write("\ndocument.writeln('<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"400\" height=\"20\">');");
				output.Write("\ndocument.writeln('<param name=\"movie\" value=\"/Flash/"+language+"/infoOptionsOneLine.swf\">');");
				output.Write("\ndocument.writeln('<param name=\"quality\" value=\"high\">');");
				output.Write("\ndocument.writeln('<param name=\"menu\" value=\"false\">');");
				output.Write("\ndocument.writeln('<param name=\"wmode\" value=\"transparent\">');");
				output.Write("\ndocument.writeln('<embed src=\"/Flash/"+language+"/infoOptionsOneLine.swf\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"400\" height=\"20\"></embed>');");
				output.Write("\ndocument.writeln('</object></td>');");
				output.Write("\n}\nelse{");
				output.Write("\ndocument.writeln('<img src=\"/Images/"+language+"/FlashReplacement/infoOptionsOneLine.gif\"></td>');");
				output.Write("\n}");
				output.Write("\n</script>");
				/* Remarque pour la ligne ci-dessous :
				 * Le </TD> a �t� plac� document.writeln � la fin du flash et de l'image.
				 * Car dans le cas o� l'image est affich�e, le fait de ne pas avoir le </TD> � la fin
				 * rajoute un espace suppl�mentaire sous l'image, ce qui donne une grosse bordure rose
				 * en dessous, ce qui n'est pas esth�tique.
				 * */
				output.Write("\n</tr>");
				output.Write("\n</table>");
			}
		}
		#endregion

		#endregion

	}
}
