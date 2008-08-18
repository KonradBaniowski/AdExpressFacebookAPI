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

public partial class Private_Helps_ProofAdvertiserSelectionHelp : System.Web.UI.Page{

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
    protected void Page_Load(object sender, EventArgs e){

        #region Textes et langage du site
        if (Page.Request.QueryString.Get("siteLanguage") != null)
            _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
        //Modification de la langue pour les Textes AdExpress
        TNS.AdExpress.Domain.Translation.Translate.SetAllTextLanguage(this,_siteLanguage);
        #endregion

    }
    #endregion

    #endregion

}
