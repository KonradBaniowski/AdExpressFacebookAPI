
#region Informations
// Auteur: D. Mussuma
// Date de création : 15/01/2007
// Date de modification : 
#endregion

using System;
using System.Web.UI;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Insertions.CreativeResult;
using TNS.FrameWork.DB.Common;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Classification;
using IDataSource = System.Web.UI.IDataSource;

namespace TNS.AdExpress.Web.Controls.Results
{
    /// <summary>
    /// Contrôle donnant accès au téléchargement des créations radio et télévisions
    /// suivant  la disponibilité de la création.
    /// </summary>
    [ToolboxData("<{0}:AlertsDownloadCreationsWebControl runat=server></{0}:AlertsDownloadCreationsWebControl>")]
    public class AlertsDownloadCreationsWebControl : System.Web.UI.WebControls.WebControl
    {
        #region Variables
        /// <summary>
        /// Identifiant du media
        /// </summary>
        private string _idMedia = null;
        /// <summary>
        /// Identifiant du produit
        /// </summary>
        private string _idProduct = null;
        /// <summary>
        /// Date de début
        /// </summary>
        private string _dateBegin = null;
        /// <summary>
        /// Date de fin
        /// </summary>
        private string _dateEnd = null;
        /// <summary>
        /// Clé d'authentification
        /// </summary>
        private string _authentificationKey = null;

        /// <summary>
        /// Message d'erreur
        /// </summary>
        private string _errorMessage = null;


        /// <summary>
        /// Langue du site
        /// </summary>
        public int _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;

        /// <summary>
        /// Identifiant du média
        /// </summary>
        protected Int64 _idVehicle = 0;


        /// <summary>
        /// Rendu des zones de texte 
        /// </summary>
        public string explanationTextResult = "";

        /// <summary>
        /// Chemin du fichier  Real média en lecture
        /// </summary>
        protected string _pathReadingRealFile = null;

        /// <summary>
        /// Chemin du fichier  Real média en téléchargement
        /// </summary>
        protected string _pathDownloadingRealFile = null;

        /// <summary>
        /// Chemin du fichier  Windows média en lecture
        /// </summary>
        protected string _pathReadingWindowsFile = null;

        /// <summary>
        /// Chemin du fichier  Windows média en téléchargement
        /// </summary>
        protected string _pathDownloadingWindowsFile = null;

        /// <summary>
        /// Identifiant création
        /// </summary>
        string _creationfile = "";


        /// <summary>
        /// Média considéré
        /// </summary>
        protected CstClassification.DB.Vehicles.names _vehicle = 0;

        /// <summary>
        /// Indique si l'utilisateur à accès à la page
        /// </summary>
        protected bool _isAuthorizeToViewCreation = false;

        /// <summary>
        /// Indique si fichier real  media existe
        /// </summary>
        protected bool _realFormatFound = true;

        /// <summary>
        /// Indique si fichier windows media  existe
        /// </summary>
        protected bool _windowsFormatFound = true;

        /// <summary>
        /// Identifiant de la version
        /// </summary>
        protected string _idSlogan = null;
        #endregion


        #region Init
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {

            try
            {
                #region Récupération des paramètres de l'url
                //Récupération des paramètres de l'url
                string[] parametersList = null;
                if (Page.Request.QueryString.Get("parameters") != null)
                    parametersList = Page.Request.QueryString.Get("parameters").ToString().Split(',');

                if (parametersList != null)
                {

                    if (parametersList.Length > 0) _idMedia = parametersList[0].ToString();
                    if (parametersList.Length > 1) _idProduct = parametersList[1].ToString();
                    if (parametersList.Length > 2) _dateBegin = parametersList[2].ToString();
                    if (parametersList.Length > 3) _dateEnd = parametersList[3].ToString();
                    if (parametersList.Length > 4) _authentificationKey = parametersList[4].ToString();
                    _siteLanguage = (parametersList.Length >= 6) ? int.Parse(parametersList[5].ToString()) : TNS.AdExpress.Constantes.DB.Language.FRENCH;

                    //Calcul de la clé d'authentification pour le téléchargement de la création
                    if (_idProduct != null && _idMedia != null && _dateBegin != null && _dateEnd != null)
                    {
                        double creationdownloadKey = (Int64)Math.Abs((Int64)(Int64.Parse(_idProduct.ToString()) * 4)
                            + 9 * Int64.Parse(_idMedia) + ((int)(int.Parse(_dateBegin) * 5) / 2) - ((int)(int.Parse(_dateEnd) * 3) / 9));

                        if (creationdownloadKey != double.Parse(_authentificationKey)) _errorMessage = GestionWeb.GetWebWord(2123, _siteLanguage);
                    }

                }
                else _errorMessage = GestionWeb.GetWebWord(958, _siteLanguage);


                if (_errorMessage == null)
                {
                    if (Page.Request.QueryString.Get("idVehicle") != null)
                    {
                        _idVehicle = Int64.Parse(Page.Request.QueryString.Get("idVehicle"));
                        _vehicle = VehiclesInformation.DatabaseIdToEnum(_idVehicle);
                    }
                    if (Page.Request.QueryString.Get("creation") != null) _creationfile = Page.Request.QueryString.Get("creation");
                    if (Page.Request.QueryString.Get("idSlogan") != null) _idSlogan = Page.Request.QueryString.Get("idSlogan");
                }
                #endregion




            }
            catch (System.Exception ex)
            {
                _errorMessage = GestionWeb.GetWebWord(1489, _siteLanguage);
                OnMethodError(ex);
            }

            base.OnInit(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {
            try
            {
                if (_errorMessage == null)
                {
                    var dataSource = new OracleDataSource(new OracleConnection(Constantes.DB.Connection.CREATIVE_CONNECTION_STRING));
                    var session = new WebSession { SiteLanguage = _siteLanguage, CurrentModule = 0, Source = dataSource };
                    CoreLayer cl = WebApplicationParameters.CoreLayers[Constantes.Web.Layers.Id.creativePopUp];
                    if (cl == null) throw (new NullReferenceException("Core layer is null for the creative pop up"));
                    var title = GestionWeb.GetWebWord(876, _siteLanguage);
                    var param = new object[8];

                    param[0] = this.Page;
                    param[1] = _vehicle;
                    param[2] = _idSlogan;
                    param[3] = _idSlogan;
                    param[4] = session;
                    param[5] = title;
                    param[6] = true;
                    param[7] = true;
                    var creativePopUp = (ICreativePopUp)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                        , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false, System.Reflection.BindingFlags.CreateInstance
                        | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null);
                    creativePopUp.IdProduct = _idProduct;
                    creativePopUp.HasCreationDownloadRights = true;
                    creativePopUp.HasCreationReadRights = true;

                    output.Write(creativePopUp.CreativePopUpRender());



                }
                else
                {
                    //erreur				
                    SetErrorMessage(_errorMessage, output);
                }

            }
            catch (System.Exception ex)
            {
                _errorMessage = GestionWeb.GetWebWord(1489, _siteLanguage);
                OnMethodError(ex);
            }
        }
        #endregion

        #region Méthodes internes

        #region Messages d'erreur
        /// <summary>
        /// Definit le message d'erreur
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="output">Flux de sortie</param>
        private void SetErrorMessage(string message, HtmlTextWriter output)
        {
            output.Write("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
            output.Write("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");
            output.Write("<TD  align=\"center\" bgColor=\"#ffffff\" class=\"txtViolet11Bold\" >");
            output.Write(message);
            output.Write("</TD>");
            output.Write("</TR></TABLE>");
        }
        #endregion

        #endregion

        #region OnMethodError
        /// <summary>
        /// Appelé sur erreur à l'exécution 
        /// </summary>
        /// <param name="errorException">Exception</param>		
        protected void OnMethodError(Exception errorException)
        {
            TNS.AdExpress.Web.Exceptions.CustomerWebException cwe = null;
            try
            {
                BaseException err = (BaseException)errorException;
                cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(this.Page, err.Message, err.StackTrace, null);
            }
            catch (System.Exception)
            {
                try
                {
                    cwe = new TNS.AdExpress.Web.Exceptions.CustomerWebException(this.Page, errorException.Message, errorException.StackTrace, null);
                }
                catch (System.Exception es)
                {
                    throw (es);
                }
            }
            cwe.SendMail();
        }
        #endregion
    }
}
