using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.PromoPSA.Web.Domain.Translation;

namespace KMI.PromoPSA.Web.Functions {
    /// <summary>
    /// Fonctions javascript
    /// </summary>
    public class Script {

        #region Login

        #region Login Already In Use
        /// <summary>
        /// Login Already In Use
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string LoginAlreadyInUse(int language, Int64 userId) {
            StringBuilder script = new StringBuilder(500);

            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("\n if(confirm('" + GestionWeb.GetWebWord(31, language).Replace("\'", "\\\'") + "')){");
            script.Append("\n document.location=\"./disconnectUser.aspx?id=" + userId + "\";");
            script.Append("\n }");
            script.Append("\n else{");
            script.Append("\n document.location=\"./index.aspx\";");
            script.Append("\n }");
            script.Append("\n </script>");

            return script.ToString();
        }
        #endregion

        #region Log in
        /// <summary>
        /// Log in
        /// </summary>
        /// <param name="link">Link to home page</param>
        /// <returns>Script</returns>
        public static string Login(string link) {

            StringBuilder script = new StringBuilder(500);

            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.AppendFormat("\n document.location=\"{0}\";", link);
            script.Append("\n </script>");

            return script.ToString();
        }
        #endregion

        #endregion

    }
}
