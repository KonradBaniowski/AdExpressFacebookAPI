using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpressI.Insertions.Default.CreativeResult;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.UI;

public partial class Public_CreativeView : WebPage {

    #region Variables
    /// <summary>
    /// Titre de la PopUp
    /// </summary>
    public string title = "";
    /// <summary>
    /// Vehicle Id
    /// </summary>
    private int vehicleId = 3;
    /// <summary>
    /// Rendu des crétaions en lecture
    /// </summary>
    public string streamingCreationsResult = "";
    /// <summary>
    /// Rendu des zones de texte 
    /// </summary>
    public string explanationTextResult = "";
    /// <summary>
    /// Chemin du fichier  Real média en lecture
    /// </summary>
    protected string pathReadingRealFile = null;
    /// <summary>
    /// Chemin du fichier  Real média en téléchargement
    /// </summary>
    protected string pathDownloadingRealFile = null;
    /// <summary>
    /// Chemin du fichier  Windows média en lecture
    /// </summary>
    protected string pathReadingWindowsFile = null;
    /// <summary>
    /// Chemin du fichier  Windows média en téléchargement
    /// </summary>
    protected string pathDownloadingWindowsFile = null;
    /// <summary>
    /// Code html de fermeture du flash d'attente
    /// </summary>
    public string divClose = "";
    /// <summary>
    /// Identifiant création
    /// </summary>
    string file = "";
    #endregion

    #region constantes
    /// <summary>
    /// Constante cookie pour fichier format windows media player 
    /// </summary>
    private const string WINDOWS_MEDIA_PLAYER_FORMAT = "windowsPlayerFormat";
    /// <summary>
    ///  Constante cookie pour fichier format real media player 
    /// </summary>
    private const string REAL_MEDIA_PLAYER_FORMAT = "realPalyerFormat";
    #endregion

    #region Page Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object sender</param>
    /// <param name="e">Event Args</param>
    protected void Page_Load(object sender, EventArgs e) {

        title = GestionWeb.GetWebWord(876, WebApplicationParameters.DefaultLanguage);
        file = Page.Request.QueryString.Get("creation");

        WebSession session = new WebSession();
        session.SiteLanguage = WebApplicationParameters.DefaultLanguage;
        session.CurrentModule = 0;
        CreativePopUp creativePopUp = new CreativePopUp(this, TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv, "", file, session, title, true, false);
        streamingCreationsResult = creativePopUp.CreativePopUpRenderWithoutOptions(800,600);
    }
    #endregion

}