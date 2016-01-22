using System;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Insertions.CreativeResult;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.UI;
using TNS.FrameWork.DB.Common;

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



    #region Page Load
    /// <summary>
    /// Page Load
    /// </summary>
    /// <param name="sender">Object sender</param>
    /// <param name="e">Event Args</param>
    protected void Page_Load(object sender, EventArgs e)
    {


        file = Page.Request.QueryString.Get("creation");
        //Connection       

        IDataSource dataSource = new OracleDataSource(new OracleConnection(TNS.AdExpress.Constantes.DB.Connection.CREATIVE_CONNECTION_STRING));
        var session = new WebSession { SiteLanguage = WebApplicationParameters.DefaultLanguage, CurrentModule = 0, Source = dataSource };
        var cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.creativePopUp];
        if (cl == null) throw (new NullReferenceException("Core layer is null for the creative pop up"));
        title = GestionWeb.GetWebWord(876, _siteLanguage);

        var param = new object[8];
        param[0] = this;
        param[1] = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv;
        param[2] = file;
        param[3] = file;
        param[4] = session;
        param[5] = title;
        param[6] = true;
        param[7] = false;
        var creativePopUp = (ICreativePopUp)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
            , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false, System.Reflection.BindingFlags.CreateInstance
            | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null);

        streamingCreationsResult = creativePopUp.CreativePopUpRenderWithoutOptions(800, 600);
    }
    #endregion
}