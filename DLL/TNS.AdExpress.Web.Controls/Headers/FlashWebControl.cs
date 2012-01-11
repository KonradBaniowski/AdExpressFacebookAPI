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

        #region PréRendu
        /// <summary>
        /// PréRendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e) {
            
            if(!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")) {
                string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"detectFlash",tmp);
            }

            base.OnPreRender(e);
        }
        #endregion

        #region Render
        /// <summary>
		/// Generate the HTML code for flash web control
		/// </summary>
		/// <param name="output">Output</param>
        protected override void Render(HtmlTextWriter output) {
                
            output.Write("<script  type=\"text/javascript\" language=\"javascript\">");
            //output.Write("(new Image()).src = (\"https\" == document.location.href.slice(0,5) ? \"https://\" : \"http://\") + \"admon1.count.brat-online.ro/j0=,,,;+,id=\" + escape(\"9fe6428063874356b8e5c0823aa576ac.swf\") + \"+url=\" + escape(document.location.href) + \";;;\";");
            output.Write("if(hasRightFlashVersion){");
			output.Write("document.writeln('<OBJECT id=\"Object1\" codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\"');");
            output.Write("document.writeln('height=\""+this.Height+"\" width=\""+this.Width+"\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" VIEWASTEXT>');");
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
            output.Write("document.writeln('type=\"application/x-shockwave-flash\" width=\""+this.Width+"\" height=\""+this.Height+"\"> </embed>');");
            output.Write("document.writeln('</OBJECT>');");
            output.Write("}");
            output.Write("else{");
            output.Write("document.writeln('<img src=\""+missingFlashUrl+"\" width=\""+this.Width+"\" height=\""+this.Height+"\">');");
			output.Write("}");
            output.Write("</script>");
            //output.Write("<noscript> ");
            //output.Write("<div style=\"display: inline;\"><img src=\"http://_SITE_.count.brat-online.ro/j0=,,,;+,id=_ID_;;;\" width=\"1\" height=\"1\" alt=\"\" /></div> ");
            //output.Write("</noscript> ");

        }
        #endregion

        #endregion

    }
}
