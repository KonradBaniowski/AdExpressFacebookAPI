using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Bastet.Web;

namespace TNS.AdExpress.Bastet.Functions {
    /// <summary>
    /// Fonctions javascript
    /// </summary>
    public class Script {

        #region Gestion de LanguageSelection
        /// <summary>
        /// Gestion de LanguageSelection
        /// </summary>
        /// <param name="pictShow">Image à montrer</param>
        /// <returns>Code JavaScript</returns>
        public static string LanguageSelectionScripts(bool pictShow) {

            StringBuilder script = new StringBuilder(500);

            //Fonction permettant de fermer le menu si ce on clique ailleurs
            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("\n\t var openMenuLS = null;");
            script.Append("\n\t function openMenuTestLS(){");
            script.Append("\n\t\t if ( openMenuLS!=null){");
            script.Append("\n\t\t\t if(window.event.srcElement.className != document.getElementById(openMenuLS).className){");
            script.Append("\n\t\t\t\t HideScrollerImageLS(openMenuLS);");
            script.Append("\n\t\t\t }");
            script.Append("\n\t\t }");
            script.Append("\n\t }");

            //Fonction affichant le scroller
            script.Append("\n function ShowScrollerImageLS(idControl) {");
            script.Append("\n\t var scroller = document.all['scroller_' + idControl ];");
            script.Append("\n\t openMenuLS = idControl ;");
            script.Append("\n\t if (scroller.style.display==\"none\") {");
            script.Append("\n\t\t scroller.style.display=\"\";");
            script.Append("\n\t } else {");
            script.Append("\n\t\t HideScrollerImageLS(idControl);");
            script.Append("\n\t }");
            script.Append("\n }");

            //Fonction pour fermer le scroller
            script.Append("\n function HideScrollerImageLS(idControl) {");
            script.Append("\n\t openMenuLS = null;");
            script.Append("\n\t var scroller = document.all['scroller_' + idControl];");
            script.Append("\n\t scroller.style.display=\"none\";");
            script.Append("\n }");

            //Fonction changeant la class d'un item, until pour la gestion des classes enrollover
            script.Append("\n function ChangeItemClassLS(obj, cssClass) {");
            script.Append("\n\t obj.className = cssClass;");
            script.Append("\n }");

            //Selection d'un élément dans LanguageSelectionWebControl
            script.Append("\n function ItemClickLS(idControl, obj, ItemId, ItemText, ItemImg, path) {");
            script.Append("\n\t	openMenuLS = null;");
            script.Append("\n\t	HideScrollerImageLS(idControl);");
            script.Append("\n\t window.location.href = path;");
            script.Append("\n }");

            script.Append("\n </script>");

            return script.ToString();
        }

        #endregion

        #region Cookies
        /// <summary>
        /// Cookies javascript functions
        /// </summary>
        /// <returns>Javascript code</returns>
        public static string CookiesJScript() {
            StringBuilder script = new StringBuilder(500);

            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");

            script.Append("\n function GetCookie(name) {");
            script.Append("\n\t if (document.cookie){");
            script.Append("\n\t var startIndex = document.cookie.indexOf(name);");
            script.Append("\n\t if (startIndex != -1) {");
            script.Append("\n\t var endIndex = document.cookie.indexOf(';', startIndex);");
            script.Append("\n\t if (endIndex == -1) endIndex = document.cookie.length;");
            script.Append("\n\t return unescape(document.cookie.substring(startIndex+name.length+1, endIndex));");
            script.Append("\n }");
            script.Append("\n }");
            script.Append("\n\t return null;");
            script.Append("\n }");

            script.Append("\n function setPermanentCookie(name,value) {");
            script.Append("\n\t var expire = new Date ();");
            script.Append("\n\t var path = '/';");
            script.Append("\n\t expire.setTime (expire.getTime() + 24 * 60 * 60 * 1000 * 365);");
            script.Append("\n\t document.cookie = name + '=' + escape(value) + '; expires=' + expire.toGMTString() + '; path='+path ;");
            script.Append("\n }");

            script.Append("\n function setCookie(name,value,days) {");
            script.Append("\n\t var expire = new Date ();");
            script.Append("\n\t var path = '/';");
            script.Append("\n\t expire.setTime (expire.getTime() + (24 * 60 * 60 * 1000) * days);");
            script.Append("\n\t document.cookie = name + '=' + escape(value) + '; expires=' + expire.toGMTString() + '; path='+path ;");
            script.Append("\n }");

            script.Append("\r\n function DeleteCookie(name) {");
            script.Append("\r\n\t  var expire = new Date ();");
            script.Append("\r\n\t  expire.setTime (expire.getTime() - (24 * 60 * 60 * 1000));");
            script.Append("\r\n\t  document.cookie = name + \"=; expires=\" + expire.toGMTString();");
            script.Append("\r\n  }");

            script.Append("\n</script>");

            return script.ToString();
        }
        /// <summary>
        /// Cookies
        /// </summary>
        /// <param name="siteLanguage">Site Language</param>
        /// <param name="link">Link</param>
        /// <returns>Code JavaScript</returns>
        public static string CookieScripts(int siteLanguage, string link) {

            StringBuilder script = new StringBuilder(500);

            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("\n var language = " + siteLanguage + ";");
            script.Append("\n var cook = GetCookie('" + WebApplicationParameters.WebSiteName + TNS.AdExpress.Bastet.Constantes.Web.Cookies.LANGUAGE + "');");
            script.Append("\n if (cook != null){");
            script.Append("\n if (language != cook){");
            script.Append("\n document.location=\"" + link + "\"+cook;");
            script.Append("\n }");
            script.Append("\n }");
            script.Append("\n</script>");

            return script.ToString();

        }
        /// <summary>
        /// Set cookie script
        /// </summary>
        /// <param name="siteLanguage">Site language</param>
        /// <returns>Code JavaScript</returns>
        public static string SetCookieScript(int siteLanguage) {

            StringBuilder script = new StringBuilder(500);
            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("\n setPermanentCookie('" + WebApplicationParameters.WebSiteName + TNS.AdExpress.Bastet.Constantes.Web.Cookies.LANGUAGE + "','" + siteLanguage + "');");
            script.Append("\n</script>");

            return script.ToString();
        }
        #endregion

    }
}
