#region Informations
// Auteur: Y. R'kaina 
// Date de création: 25/02/2008
// Date de modification: 
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS.AdExpress.Web.Controls.Headers {
    /// <summary>
    /// Used to show Flash in ASP .NET pages
    /// </summary>
    [ToolboxData("<{0}:FlashWebControl runat=server></{0}:FlashWebControl>")]
    public class FlashWebControl : WebControl {

        #region Variables
        /// <summary>
        /// Flash URL
        /// </summary>
        private string flashUrl = string.Empty;
        /// <summary>
        /// Missing Flash URL
        /// </summary>
        private string missingFlashUrl = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// SET or GET flash URL
        /// </summary>
        public string FlashUrl {
            get { return flashUrl; }
            set { flashUrl = value; }
        }

        /// <summary>
        /// SET or GET missing flash URL
        /// </summary>
        public string MissingFlashUrl {
            get { return missingFlashUrl; }
            set { missingFlashUrl = value; }
        }
        #endregion

        #region Events

        #region Render
        /// <summary>
		/// Generate the HTML code for flash web control
		/// </summary>
		/// <param name="output">Output</param>
        protected override void Render(HtmlTextWriter output) {
                
            output.Write("<script  type=\"text/javascript\" language=\"javascript\">");
            output.Write("if(hasRightFlashVersion){");
			output.Write("document.writeln('<OBJECT id=\"Object1\" codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\"');");
            output.Write("document.writeln('height=\"484\" width=\"733\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" VIEWASTEXT>');");
			output.Write("document.writeln('<PARAM NAME=\"_cx\" VALUE=\"19394\">');");
            output.Write("document.writeln('<PARAM NAME=\"_cy\" VALUE=\"12806\">');");
			output.Write("document.writeln('<PARAM NAME=\"FlashVars\" VALUE=\"\">');");										
			output.Write("document.writeln('<PARAM NAME=\"Movie\" VALUE=\""+flashUrl+"\">');");
			output.Write("document.writeln('<PARAM NAME=\"Src\" VALUE=\""+flashUrl+"\">');");
            output.Write("document.writeln('<PARAM NAME=\"WMode\" VALUE=\"Transparent\">');");
            output.Write("document.writeln('<PARAM NAME=\"Play\" VALUE=\"-1\">');");
            output.Write("document.writeln('<PARAM NAME=\"Loop\" VALUE=\"-1\">');");
            output.Write("document.writeln('<PARAM NAME=\"Quality\" VALUE=\"High\">');");
            output.Write("document.writeln('<PARAM NAME=\"SAlign\" VALUE=\"\">');");
            output.Write("document.writeln('<PARAM NAME=\"Menu\" VALUE=\"0\">');");
            output.Write("document.writeln('<PARAM NAME=\"Base\" VALUE=\"\">');");
            output.Write("document.writeln('<PARAM NAME=\"AllowScriptAccess\" VALUE=\"always\">');");
            output.Write("document.writeln('<PARAM NAME=\"Scale\" VALUE=\"ShowAll\">');");
            output.Write("document.writeln('<PARAM NAME=\"DeviceFont\" VALUE=\"0\">');");
            output.Write("document.writeln('<PARAM NAME=\"EmbedMovie\" VALUE=\"0\">');");
            output.Write("document.writeln('<PARAM NAME=\"BGColor\" VALUE=\"\">');");
            output.Write("document.writeln('<PARAM NAME=\"SWRemote\" VALUE=\"\">');");
            output.Write("document.writeln('<PARAM NAME=\"MovieData\" VALUE=\"\">');");
            output.Write("document.writeln('<PARAM NAME=\"SeamlessTabbing\" VALUE=\"1\">');");
            output.Write("document.writeln('<embed src=\""+flashUrl+"\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\"');");
            output.Write("document.writeln('type=\"application/x-shockwave-flash\" width=\"733\" height=\"484\"> </embed>');");
            output.Write("document.writeln('</OBJECT>');");
            output.Write("}");
            output.Write("else{");
            output.Write("document.writeln('<img src=\""+missingFlashUrl+"\" width=\"733\" height=\"484\">');");
			output.Write("}");
            output.Write("</script>");

        }
        #endregion

        #endregion

    }
}
