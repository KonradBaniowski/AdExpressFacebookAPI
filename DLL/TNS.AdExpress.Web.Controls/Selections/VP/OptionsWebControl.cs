#region Information
// Auteur : B.Masson
// Cr�� le : 07/12/2006
// Modifi� par: 
#endregion

#region Namespaces
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Data;

using System.Text;
using System.Windows.Forms;
//using Oracle.DataAccess.Client;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using AdExClassification=TNS.AdExpress.DataAccess.Classification;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

#endregion

namespace TNS.AdExpress.Web.Controls.Selections.VP{
	/// <summary>
	/// Composant pour la construction du code html pour le rappel de s�lection
	/// </summary>
	[DefaultProperty("Text"),  
		ToolboxData("<{0}:DetailSelectionWebControl runat=server></{0}:DetailSelectionWebControl>")]
    public class OptionsWebControl : System.Web.UI.WebControls.WebControl {

        #region Render
        /// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output){

            output.Write("<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");
            output.Write("<tr>");
            output.Write("<td>");

            output.Write("</td>");
            output.Write("</tr>");
            output.Write("</table>");

		}
		#endregion

	}
}
