using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.GAD;

namespace TNS.AdExpress.Web.Controls.Results
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:GadWebControl runat=server></{0}:GadWebControl>")]
    public class GadWebControl : WebControl
    {
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;

        /// <summary>
        /// User session
        /// </summary>
        public WebSession CustomerWebSession
        {           
            set
            {
                _session = value;
            }
        }

        #region Prérender
        /// <summary>
        /// Préparation du rendu des niveaux de détails personnalisés.
        /// </summary>
        /// <param name="e">Sender</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            #region Script
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script type=\"text/javascript\">");
            js.Append("\r\nvar ficheUp = new Image();");
            js.Append("\r\nvar ficheDown = new Image();");
            js.Append("\r\nvar ficheUnavailable = new Image();");
            js.Append("\r\nficheUp.src = \"/App_Themes/" + Page.Theme + "/Images/Culture/button/bt_fiche_up.gif\";");
            js.Append("\r\nficheDown.src = \"/App_Themes/" + Page.Theme + "/Images/Culture/button/bt_fiche_down.gif\";");
            js.Append("\r\nficheUnavailable.src = \"/App_Themes/" + Page.Theme + "/Images/Culture/button/bt_fiche_off.gif\";");

            js.Append("function OpenGad(address){");
            js.AppendFormat(" contenu.innerHTML = \"<IFRAME style=\\\"border-style:none;\\\" SRC=\\\"\" + address +\"\\\" HEIGHT=\\\"660\\\" WIDTH=\\\"980\\\">{0}</IFRAME>\";", GestionWeb.GetWebWord(2971, _session.SiteLanguage));
            js.Append("	contenu.leftMargin = 0;");
            js.Append("	contenu.topMargin = 0;");
            js.Append("contenu.background=\"\";");
            js.Append("window.resizeTo(1000,700);");
            js.Append("}");
            js.Append("\r\n</script>");
            if (!Page.ClientScript.IsClientScriptBlockRegistered("FicheScript")) Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FicheScript", js.ToString());
            //if (!Page.ClientScript.IsClientScriptBlockRegistered("OpenWindow")) Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenWindow", TNS.AdExpress.Web.Functions.Script.OpenWindow());
            #endregion
        }
        #endregion

        protected override void RenderContents(HtmlTextWriter output)
        {
            try
            {
                //Récupération de l'adresse et de l'annonceur
                string idAddress = Page.Request.QueryString.Get("idAddress").ToString();
                string advertiser = Page.Request.QueryString.Get("advertiser").ToString();

                var html = new StringBuilder();
                var param = new object[3];
                param[0] = _session;
                param[1] = idAddress;
                param[2] = advertiser;
                Domain.Layers.CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.gad];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the GAD results"));
                var gadResults = (IGadResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                gadResults.Theme = Page.Theme;
                string data = gadResults.GetHtml();
                output.Write(data);

            }
            catch (Exception et)
            {
                output.Write(Web.Functions.Script.ErrorCloseScript(GestionWeb.GetWebWord(959, _session.SiteLanguage)) + " " + et.Message);
            }
         
        }
    }
}
