#region Informations
// Auteur: B.Masson
// Date de création: 27/09/2006
// Date de modification:
// Le 03/10/2006 Par B.Masson > Amélioration du design pour le cas de l'image gif
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Translation;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Description résumée de InformationWebControl.
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
		/// Indique si la position est à gauche ou non
		/// </summary>
		protected bool inLeftMenu = true;
		/// <summary>
		/// Couleur de fond du composant
		/// </summary>
		protected string _backgroundColor="#ffffff";
        /// <summary>
        /// Animation flash path
        /// </summary>
        protected string _flashPath = string.Empty;
        /// <summary>
        /// Flash Replacement path
        /// </summary>
        protected string _flashReplacementPath = string.Empty;
        /// <summary>
        /// Animation flash one line path
        /// </summary>
        protected string _flashOneLinePath = string.Empty;
        /// <summary>
        /// Flash one line replacement path
        /// </summary>
        protected string _flashOneLineReplacementPath = string.Empty;
		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Obtient ou définit la langue des textes du composant
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
		/// Obtient ou définit la couleur de fond du composant
		/// </summary>
		[Bindable(true), 
		Category("Appearance"), 
		DefaultValue("#ffffff"),
		Description("Couleur de fond du composant")]
		public string BackGroundColor{
			get{return(_backgroundColor);}
			set{_backgroundColor=value;}
		}

        /// <summary>
        /// Set or Get animation flash path
        /// </summary>
        public string FlashPath {
            get { return (_flashPath); }
            set { _flashPath = value; }
        }

        /// <summary>
        /// Set or Get Flash Replacement path
        /// </summary>
        public string FlashReplacementPath {
            get { return (_flashReplacementPath); }
            set { _flashReplacementPath = value; }
        }

        /// <summary>
        /// Set or Get Animation flash one line path
        /// </summary>
        public string FlashOneLinePath {
            get { return (_flashOneLinePath); }
            set { _flashOneLinePath = value; }
        }

        /// <summary>
        /// Set or Get Flash one line replacement path
        /// </summary>
        public string FlashOneLineReplacementPath {
            get { return (_flashOneLineReplacementPath); }
            set { _flashOneLineReplacementPath = value; }
        }
		#endregion

		#region Evènements

		#region PreRender
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

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		/// <remarks>
		/// Le flash est transparent pour permettre d'avoir au 1er plan :
		/// Le menu contextuel du clic droit et les menus déroulants des différentes options à gauche
		/// C'est pour cela que le flash et l'image sont encapsulé dans un tableau avec une couleur de 
		/// fond rose
		/// </remarks>
		protected override void Render(HtmlTextWriter output){
			if(inLeftMenu){
				// Design du composant dans le menu de gauche
				output.Write("\n<table width=\"100%\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\""+_backgroundColor+"\">");
				output.Write("\n<tr><td><IMG height=10 src=\"/images/Common/pixel.gif\" width=\"1\"></td></tr>");
				output.Write("\n<tr><td>");

                output.Write("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"pinkBackGround\"><tr><td>");
				output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
				output.Write("\nif(hasRightFlashVersion==true){");
				output.Write("\ndocument.writeln('<object id=\"infoOptionFlash\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"185\" height=\"48\" VIEWASTEXT>');");
				output.Write("\ndocument.writeln('<param name=\"movie\" value=\""+_flashPath+"\">');");
				output.Write("\ndocument.writeln('<param name=\"quality\" value=\"high\">');");
				output.Write("\ndocument.writeln('<param name=\"menu\" value=\"false\">');");
				output.Write("\ndocument.writeln('<param name=\"wmode\" value=\"transparent\">');");
				output.Write("\ndocument.writeln('<embed src=\""+_flashPath+"\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"185\" height=\"48\"></embed>');");
				output.Write("\ndocument.writeln('</object></td>');");
				output.Write("\n}\nelse{");
				output.Write("\ndocument.writeln('<img src=\""+_flashReplacementPath+"\"></td>');");
				output.Write("\n}");
				output.Write("\n</script>");
				/* Remarque pour la ligne ci-dessous :
				 * Le </TD> a été placé document.writeln à la fin du flash et de l'image.
				 * Car dans le cas où l'image est affichée, le fait de ne pas avoir le </TD> à la fin
				 * rajoute un espace supplémentaire sous l'image, ce qui donne une grosse bordure rose
				 * en dessous, ce qui n'est pas esthétique.
				 * */
				output.Write("</tr></table>");

				output.Write("\n</td></tr>");
				output.Write("\n<tr><td><IMG height=\"10\" src=\"/images/Common/pixel.gif\" width=\"1\"></td></tr>");
				output.Write("\n</table>");
			}
			else{
				// Design sur une ligne du composant situé en haut des pops up
                output.Write("\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"paleVioletBackGround\">");
				output.Write("\n<tr><td>");
				output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
				output.Write("\nif(hasRightFlashVersion==true){");
				output.Write("\ndocument.writeln('<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"400\" height=\"20\">');");
				output.Write("\ndocument.writeln('<param name=\"movie\" value=\""+_flashOneLinePath+"\">');");
				output.Write("\ndocument.writeln('<param name=\"quality\" value=\"high\">');");
				output.Write("\ndocument.writeln('<param name=\"menu\" value=\"false\">');");
				output.Write("\ndocument.writeln('<param name=\"wmode\" value=\"transparent\">');");
                output.Write("\ndocument.writeln('<embed src=\""+_flashOneLinePath+"\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"400\" height=\"20\"></embed>');");
				output.Write("\ndocument.writeln('</object></td>');");
				output.Write("\n}\nelse{");
				output.Write("\ndocument.writeln('<img src=\""+_flashOneLineReplacementPath+"\"></td>');");
				output.Write("\n}");
				output.Write("\n</script>");
				/* Remarque pour la ligne ci-dessous :
				 * Le </TD> a été placé document.writeln à la fin du flash et de l'image.
				 * Car dans le cas où l'image est affichée, le fait de ne pas avoir le </TD> à la fin
				 * rajoute un espace supplémentaire sous l'image, ce qui donne une grosse bordure rose
				 * en dessous, ce qui n'est pas esthétique.
				 * */
				output.Write("\n</tr>");
				output.Write("\n</table>");
			}
		}
		#endregion

		#endregion

	}
}
