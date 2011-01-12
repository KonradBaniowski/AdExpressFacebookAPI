using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Headers {
    /// <summary>
    /// Composant affichant le titre  et le descriptif d'une page indépendante d'un module
    /// le descriptif peut contenir plusieurs texts
    /// </summary>
    [ToolboxData("<{0}:CustomizePageTitleWebControl runat=server></{0}:CustomizePageTitleWebControl>")]
    public class CustomizePageTitleWebControl : PageTitleWebControl {

        #region Propriétés
        /// <summary>
        /// Code descriptif de la page
        /// </summary>
        [Bindable(true),
        Description("liste de codes de traduction du champ descriptif")]
        protected string _codeDescriptionList;
        /// <summary></summary>
        public string CodeDescriptionList {
            get { return _codeDescriptionList; }
            set { _codeDescriptionList = value; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Description Render
        /// </summary>
        /// <param name="output">Le writer HTML vers lequel écrire</param>
        override protected void DescriptionRender(HtmlTextWriter output) {

            string[] codeList = _codeDescriptionList.Split(',');
            int index = 1;

            output.Write("\n<tr>");
            output.Write("\n<td class=\"txtBlanc11Bold\">");
            foreach (string s in codeList) {
                output.Write(GestionWeb.GetWebWord(Convert.ToInt64(s), siteLang));
                if(index<codeList.Length)
                    output.Write("&nbsp;");
                index++;
            }
            output.Write("\n</td>");
            output.Write("\n</tr>");

        }
        #endregion

    }
}
