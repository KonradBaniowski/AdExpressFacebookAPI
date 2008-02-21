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

public partial class Private_Helps_GlobalDateSelectionHelp : System.Web.UI.Page {

    #region Variables
    /// <summary>
    /// Langue du site
    /// </summary>
    public int _siteLanguage = 33;
    #endregion

    #region Evènements

    #region Chargement
    /// <summary>
    /// Chargement de la page
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Aguments</param>
    protected void Page_Load(object sender, EventArgs e) {

        #region Textes et langage du site
        if(Page.Request.QueryString.Get("siteLanguage") != null)
            _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
        //Modification de la langue pour les Textes AdExpress
        TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[1].Controls, _siteLanguage);
        #endregion

    }
    #endregion

    #endregion

}
