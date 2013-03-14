using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using TNS.AdExpressI.Visual;
using TNS.AdExpress.Domain.Web;
using System.Reflection;

using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
namespace AdExpress.Private.Results
{
    /// <summary>
    /// Popup affichant les créations presse en gd format
    /// La fenêtre prend en argument dans l'url la liste des fichiers dans un variable création
    /// </summary>
    public partial class ZoomCreationPopUp2 : TNS.AdExpress.Web.UI.WebPage
    {

        #region Variables
        /// <summary>
        /// Code résultat
        /// </summary>
        public string result = "";
        #endregion

        #region Evènements

        #region Chargement de la page
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string path = HttpContext.Current.Request.QueryString.Get("path");
            Int64 vehicleId = Int64.Parse(HttpContext.Current.Request.QueryString.Get("id_vehicle"));
            bool isBlur = false;
            bool.TryParse(HttpContext.Current.Request.QueryString.Get("is_blur"), out isBlur);
            string encrypt = HttpContext.Current.Request.QueryString.Get("crypt");
            string idSession = HttpContext.Current.Request.QueryString.Get("idSession");

            string decryptedFileName = TNS.AdExpress.Web.Core.Utilities.QueryStringEncryption.DecryptQueryString(path);
            result = "";

            Regex f = new Regex(@"\.swf");


            string creaPath = TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + path + "&id_vehicle=" + vehicleId.ToString() + "&idSession=" + idSession + "&is_blur=" + isBlur.ToString()+ "&crypt=" + encrypt;

            if (f.IsMatch(decryptedFileName))
            {
                // Flash banner
                result = string.Format("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\"100%\" height=\"100%\">");
                result += string.Format("\n <PARAM name=\"movie\" value=\"{0}\">", creaPath);
                result += string.Format("\n <PARAM name=\"play\" value=\"true\">");
                result += string.Format("\n <PARAM name=\"quality\" value=\"high\">");
                result += string.Format("\n <EMBED src=\"{0}\" play=\"true\" swliveconnect=\"true\" quality=\"high\" width=\"100%\" height=\"100%\">",
                      creaPath);
                result += string.Format("\n </OBJECT>");
            }
            else
            {
                result += "<img src=\"" + creaPath + "\">";
            }


        }
        #endregion

        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Evènement d'initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
