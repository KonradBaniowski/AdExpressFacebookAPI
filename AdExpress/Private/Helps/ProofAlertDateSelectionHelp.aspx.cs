using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.SessionState;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

public partial class Private_Helps_ProofAlertDateSelectionHelp : WebPage{

    #region Evènements

    #region Chargement
    /// <summary>
    /// Chargement de la page
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Aguments</param>
    protected void Page_Load(object sender, EventArgs e){
        //Modification de la langue pour les Textes AdExpress
        TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls, _siteLanguage);
    }
    #endregion

    #endregion

}
