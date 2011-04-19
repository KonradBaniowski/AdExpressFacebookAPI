using System;
using System.Collections.Generic;
using System.Text;

namespace KMI.P3.Web.Functions
{
    public class Script
    {
        #region Script de redirection vers la page d'erreur du site
        /// <summary>
        /// Script de redirection vers la page d'erreur du site
        /// </summary>
        /// <returns>Code du script</returns>
        public static string RedirectError(string siteLanguage)
        {
            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
            t.Append(" window.document.location.href='/Public/Message.aspx?msgCode=5&siteLanguage=" + siteLanguage + "';");
            t.Append("\n</script>");
            return t.ToString();

        }

        #endregion

        #region Gestion de l'évènements "enter"
        /// <summary>
        /// JavaScript permettant d'intercepter l'évènement "enter" et de faire un appel serveur imputé à un contrôle quelconque passé en paramètre.
        /// </summary>
        /// <remarks>
        /// Enregistré sous le nom trapEnter
        /// </remarks>
        /// <param name="validatedComponent">Composant qui sera vu comme le responsable du postBack</param>
        /// <returns>Code JavaScript</returns>
        public static string TrapEnter(string validatedComponent)
        {
            string scriptTrapEnter = "";
            scriptTrapEnter += "\n<script language=\"JavaScript\">";
            scriptTrapEnter += "\nfunction trapEnter(evt){";
            scriptTrapEnter += "\nevt = (evt) ? evt : ((event) ? event : null);";
            scriptTrapEnter += "\nif((evt.which && evt.which == 13)||(evt.keyCode && evt.keyCode == 13)){";
            scriptTrapEnter += "\n__doPostBack('" + validatedComponent + "','');}";
            scriptTrapEnter += "\n}\n</script>";
            return scriptTrapEnter;
        }
        #endregion

        #region DisabledTextSelection
        public static string DisabledTextSelection(string controlId)
        {
            StringBuilder js = new StringBuilder();
            js.Append("\n <script language=\"JavaScript1.2\">");
            js.Append("\r\nfunction DisabledTextSelection_" + controlId + "(element){");
            js.Append("\r\n//if IE4+");
            js.Append("\r\nif (element.addEventListener)");
            js.Append("\r\nelement.addEventListener('selectstart', new Function (\"return false\"), false);");
            js.Append("\r\nelse if (element.attachEvent)");
            js.Append("\r\nelement.attachEvent('onselectstart', new Function (\"return false\"));");
            js.Append("\r\n//if NS6");
            js.Append("\r\nif (window.sidebar){");
            js.Append("\r\nif (element.addEventListener)");
            js.Append("\r\nelement.addEventListener('mousedown', new Function (\"return false\"), false);");
            js.Append("\r\nelse if (element.attachEvent)");
            js.Append("\r\nelement.attachEvent('onmousedown', new Function (\"return false\"));");
            js.Append("\r\nif (element.addEventListener)");
            js.Append("\r\nelement.addEventListener('click', new Function (\"return false\"), false);");
            js.Append("\r\nelse if (element.attachEvent)");
            js.Append("\r\nelement.attachEvent('onclick', new Function (\"return false\"));");
            js.Append("\r\n}");
            js.Append("\r\n}");
            js.Append("\r\n</script>");
            return js.ToString();
        }
        #endregion
    }
}
