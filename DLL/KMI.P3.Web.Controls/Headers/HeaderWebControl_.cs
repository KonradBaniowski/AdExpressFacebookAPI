//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Text;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using KMI.P3.Domain.Web.Navigation;
//using KMI.P3.Domain.Translation;

//namespace KMI.P3.Web.Controls.Headers
//{

//    #region Page Type
//    /// <summary>
//    /// Type défini pour la sélection d'un type de page
//    /// </summary>
//    public enum PageType
//    {
//        /// <summary>
//        /// Mode Deconnecte
//        /// </summary>
//        disconnect,
//        /// <summary>
//        /// Mode Connecte
//        /// </summary>
//        connect
//    }
//    #endregion

//    [DefaultProperty("Text")]
//    [ToolboxData("<{0}:HeaderWebControl runat=server></{0}:HeaderWebControl>")]
//    public class HeaderWebControl_ : System.Web.UI.WebControls.WebControl
//    {
//        #region Variables
//        /// <summary>
//        /// Langage du site
//        /// </summary>
//        private int _language;
//        ///<summary>
//        /// Type de page
//        /// </summary>
//        ///  <supplierCardinality>1</supplierCardinality>
//        ///  <directed>True</directed>
//        ///  <label>pageType</label>
//        private PageType _pageType;
//        /// <summary>
//        /// Indique le menu qui est actif
//        /// </summary>
//        private int _activeMenu = -1;
//        #endregion

//        #region Accesseurs
//        /// <summary>
//        /// Obtient ou définit propriété de langage
//        /// </summary>
//        [Bindable(true),
//        Description("Langue du site")]
//        public int Language
//        {
//            get { return _language; }
//            set { _language = value; }
//        }

//        /// <summary>
//        /// Obtient ou définit le type de page déterminant pour les menus présents
//        /// </summary>
//        [Description("Page d'accueil ou page générique")]
//        public PageType Type_de_page
//        {
//            get { return _pageType; }
//            set { _pageType = value; }
//        }

//        /// <summary>
//        /// Obtient ou défnit le menu qui est actif
//        /// </summary>
//        [Description("Spécifie l'index du menu actif(0...)"),
//        DefaultValue(0)]
//        public int ActiveMenu
//        {
//            get { return _activeMenu; }
//            set { _activeMenu = value; }
//        }

//        #endregion

//        #region Property
//        /// <summary>
//        /// Css Class Last Item
//        /// </summary>
//        private string _cssClassLastItem;
//        /// <summary>
//        /// Set Css Class Last Item
//        /// </summary>
//        public string CssClassLastItem
//        {
//            set { _cssClassLastItem = value; }
//        }
//        /// <summary>
//        /// Css Class Item
//        /// </summary>
//        private string _cssClassItem;
//        /// <summary>
//        /// Set Css Class Item
//        /// </summary>
//        public string CssClassItem
//        {
//            set { _cssClassItem = value; }
//        }
//        #endregion

//        #region Evènements

//        #region PréRendu
//        /// <summary>
//        /// PréRendu
//        /// </summary>
//        /// <param name="e">Arguments</param>
//        protected override void OnPreRender(EventArgs e)
//        {
//            base.OnPreRender(e);
//        }
//        #endregion

//        #region Render (Affichage)
//        /// <summary>
//        /// Génère ce contrôle dans le paramètre de sortie spécifié.
//        /// </summary>
//        /// <param name="output"> Le writer HTML vers lequel écrire </param>
//        protected override void Render(HtmlTextWriter output)
//        {

//            Dictionary<string, WebHeader> headers = WebHeaders.HeadersList;
//            int i = 0;
//            string menus = "";
//            string href = "";
//            string languageString, idSessionString;
//            bool firstParameter;

//            output.Write("\n\t\t\t\t\t<div class=\"" + this.CssClass + "\">");
//            output.Write("\n\t\t\t\t\t\t<ul>");
//            int nbLinks = headers[_pageType.ToString()].MenuItems.Count;
//            foreach (WebHeaderMenuItem currentHeaderMenuItem in headers[_pageType.ToString()].MenuItems)
//            {

//                languageString = "";
//                idSessionString = "";
//                firstParameter = true;

//                //Options dans l'url suivant le menuItem
//                //Differenciation et inactivation du menu actif si nécessaire
//                if (_activeMenu == currentHeaderMenuItem.IdMenu)
//                {
//                    href = "#";
//                }
//                else
//                {

//                    #region Gestion des Paramètres

//                    #region Paramètre de la langue
//                    if (currentHeaderMenuItem.GiveLanguage)
//                    {
//                        languageString = "siteLanguage=" + _language;
//                        if (firstParameter)
//                        {
//                            languageString = "?" + languageString;
//                            firstParameter = false;
//                        }
//                        else
//                        {
//                            languageString = "&" + languageString;
//                        }
//                    }
//                    #endregion

//                    #region Paramètre de l'identifiant de la session
//                    if (currentHeaderMenuItem.GiveSession)
//                    {
//                        idSessionString = "idSession=" + Page.Request.QueryString.Get("idSession").ToString();
//                        if (firstParameter)
//                        {
//                            idSessionString = "?" + idSessionString;
//                            firstParameter = false;
//                        }
//                        else
//                        {
//                            idSessionString = "&" + idSessionString;
//                        }
//                    }
//                    #endregion

//                    #endregion

//                    href = (currentHeaderMenuItem).TargetUrl + languageString + idSessionString;
//                    if (currentHeaderMenuItem.Target.Length > 0)
//                        href += "\" target=\"" + currentHeaderMenuItem.Target + "\"";
//                }

//                output.Write("\n\t\t\t\t\t\t\t\t<li>");

//                if (currentHeaderMenuItem.DisplayInPopUp)
//                    menus = "\n\t\t\t\t\t\t\t\t\t<a " + ((i == nbLinks - 1) ? "class=\"" + this._cssClassLastItem + "\"" : "class=\"" + this._cssClassItem + "\"") + " href=\"javascript:popupOpenBis('" + href + "','975','600','yes');\">" + GestionWeb.GetWebWord((int)currentHeaderMenuItem.IdMenu, _language) + "</a>";
//                else
//                    menus = "\n\t\t\t\t\t\t\t\t\t<a " + ((i == nbLinks - 1) ? "class=\"" + this._cssClassLastItem + "\"" : "class=\"" + this._cssClassItem + "\"") + " href=\"" + href + "\">" + GestionWeb.GetWebWord((int)currentHeaderMenuItem.IdMenu, _language) + "</a>";

//                output.Write("{0}\n\t\t\t\t\t\t\t\t</li>", menus);
//            }

//            output.Write("\n\t\t\t\t\t\t</ul>");
//            output.Write("\n\t\t\t\t\t</div>");

//        }
//        #endregion

//        #endregion

//        #region Private Methods
//        /// <summary>
//        /// Get Path and Query
//        /// </summary>
//        /// <returns>Path string</returns>
//        private string GetLinkPath()
//        {

//            string link = this.Parent.Page.Request.Url.PathAndQuery.ToString();
//            string[] linkParam = link.Split('?');
//            string[] paramsList;
//            bool verif = true;

//            link = linkParam[0];

//            if (linkParam.Length > 1)
//            {
//                paramsList = linkParam[1].Split('&');
//                for (int i = 0; i < paramsList.Length; i++)
//                    if (!paramsList[i].Contains("siteLanguage"))
//                        if (verif)
//                        {
//                            link += "?" + paramsList[i];
//                            verif = false;
//                        }
//                        else
//                            link += "&" + paramsList[i];
//            }

//            if (verif)
//                link += "?siteLanguage=";
//            else
//                link += "&siteLanguage=";

//            return link;
//        }
//        #endregion
//    }
//}
