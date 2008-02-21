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

using System.IO;
using WebCst = TNS.AdExpress.Constantes.Web;
using TradCst = TNS.AdExpress.Constantes.DB.Language;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using WebFunctions = TNS.AdExpress.Web.Functions;

public partial class Private_Informations_DataUpdate2 : TNS.AdExpress.Web.UI.WebPage {

    #region Constantes
    /// <summary>
    /// Répertoire des fichiers
    /// </summary>
    public const string LOCAL_PATH_DATE_UPDATE = @"\\hera\AdexDatas\MediaList\";
    /// <summary>
    /// Nom du répertoire virtuel IIS
    /// </summary>
    public const string LINK_DATE_UPDATE = "/MediaList/";
    #endregion

    #region Variables
    /// <summary>
    /// Identifiant de session
    /// </summary>
    public string idsession;
    /// <summary>
    /// Texte Explicatif et lien pointant vers le fichier Excel de la liste des supports disponibles
    /// </summary>
    public string _link = "";
    #endregion

    #region Chargement de la page
    /// <summary>
    /// Chargement de la page
    /// </summary>
    /// <param name="sender">Objet</param>
    /// <param name="e">Arguments</param>
    public void Page_Load(object sender, EventArgs e) {
        try {

            #region Rollover
            closeRollOverWebControl.ImageUrl = "/Images/" + _webSession.SiteLanguage + "/button/fermer_up.gif";
            closeRollOverWebControl.RollOverImageUrl = "/Images/" + _webSession.SiteLanguage + "/button/fermer_down.gif";
            #endregion

            #region Construction du lien du fichier Excel de la liste des supports disponibles

            // Pour test en localhost :
            //string pathDirectory = AppDomain.CurrentDomain.BaseDirectory+"MediaList/";
            //string linkFile="/MediaList/";

            string pathDirectory = LOCAL_PATH_DATE_UPDATE;
            string linkFile = LINK_DATE_UPDATE;

            if(_webSession.SiteLanguage == TNS.AdExpress.Constantes.DB.Language.FRENCH) {
                if(File.Exists(pathDirectory + "Liste supports en alerte.xls")) {
                    _link = "<a href=\"" + linkFile + "Liste supports en alerte.xls\" target=\"_blank\" class=\"roll05\">" + GestionWeb.GetWebWord(1832, _webSession.SiteLanguage) + "</a>";
                }
                else _link = GestionWeb.GetWebWord(1833, _webSession.SiteLanguage); // Fichier non disponible
            }
            else if(_webSession.SiteLanguage == TNS.AdExpress.Constantes.DB.Language.ENGLISH) {
                if(File.Exists(pathDirectory + "List of vehicles on alert.xls")) {
                    _link = "<a href=\"" + linkFile + "List of vehicles on alert.xls\" target=\"_blank\" class=\"roll05\">" + GestionWeb.GetWebWord(1832, _webSession.SiteLanguage) + "</a>";
                }
                else _link = GestionWeb.GetWebWord(1833, _webSession.SiteLanguage); // Fichier non disponible
            }
            #endregion

        }
        catch(System.Exception exc) {
            if(exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
            }
        }
    }
    #endregion

    #region Evènements

    #region Fermer
    /// <summary>
    /// Bouton Fermer
    /// </summary>
    /// <param name="sender">Objet</param>
    /// <param name="e">Arguments</param>
    protected void closeRollOverWebControl_Click(object sender, EventArgs e) {
        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeScript", WebFunctions.Script.CloseScript());
    }
    #endregion

    #endregion

}
