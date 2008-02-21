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
using System.Drawing;
using System.Web.SessionState;

public partial class Private_Helps_ProofResultsHelp : System.Web.UI.Page
{

    #region Variables
    /// <summary>
    /// Langue du site
    /// </summary>
    public int _siteLanguage = 33;
    #endregion

    #region EvÃ¨nements

    #region Chargement
    /// <summary>
    /// Chargement de la page
    /// </summary>
    /// <param name="sender">Objet qui lance l'Ã©vÃ¨nement</param>
    /// <param name="e">Aguments</param>
    protected void Page_Load(object sender, System.EventArgs e)
    {

        #region Textes et langage du site
        if (Page.Request.QueryString.Get("siteLanguage") != null)
            _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
        //Modification de la langue pour les Textes AdExpress
        TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls, _siteLanguage);
        #endregion

    }
    #endregion

    #endregion

}
