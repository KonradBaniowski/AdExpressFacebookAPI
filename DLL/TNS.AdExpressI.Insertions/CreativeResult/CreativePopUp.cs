using System;
using System.Reflection;
using System.Text;
using System.Web.UI;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using CstClassificationVehicle = TNS.AdExpress.Constantes.Classification.DB.Vehicles;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using System.IO;
using System.Data;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Insertions.DAL;
using System.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.Insertions.CreativeResult
{
    /// <summary>
    /// Class used to display tv and radio creatives in reading and streamin mode
    /// </summary>
    public abstract class CreativePopUp : ICreativePopUp
    {

        #region constantes
        /// <summary>
        /// Constante cookie pour fichier format windows media player 
        /// </summary>
        protected const string WINDOWS_MEDIA_PLAYER_FORMAT = "windowsPlayerFormat";
        /// <summary>
        ///  Constante cookie pour fichier format real media player 
        /// </summary>
        protected const string REAL_MEDIA_PLAYER_FORMAT = "realPalyerFormat";
        protected const string RA_EXTENSION = "ra";
        protected const string WMA_EXTENSION = "wma";
        protected const string WMV_EXTENSION = "wmv";
        protected const string RM_EXTENSION = "rm";
        protected const string MP4_EXTENSION = "MP4";
        protected const string VIDEO_240_SUBFOLDER = "240";
        protected const string MP3_EXTENSION = "MP3";
        protected const string AVI_EXTENSION = "AVI";
        #endregion

        #region Variables
        /// <summary>
        /// Identifiant d'une version
        /// </summary>
        protected string _idSlogan = null;
        /// <summary>
        /// Titre de la PopUp
        /// </summary>
        protected string _title = "";
        /// <summary>
        /// Session utilisateur
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Indique si l'utilisateur à le droit de lire les créations
        /// </summary>
        protected bool _hasCreationReadRights = false;
        /// <summary>
        /// Indique si l'utilisateur à le droit de télécharger les créations
        /// </summary>
        protected bool _hasCreationDownloadRights = false;
        /// <summary>
        /// Is New Real Audio File Path
        /// </summary>
        protected bool _isNewRealAudioFilePath = false;
        /// <summary>
        /// Is New Windows Audio File Path
        /// </summary>
        protected bool _isNewWindowsAudioFilePath = false;
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
        protected string _file = "";
        /// <summary>
        /// Média considéré
        /// </summary>
        protected CstClassification.DB.Vehicles.names _vehicle = 0;
        /// <summary>
        /// Pop Up
        /// </summary>
        protected Page _popUp;
        /// <summary>
        /// Video object Width
        /// </summary>
        protected int _width = 0;
        /// <summary>
        /// Video object Height
        /// </summary>
        protected int _height = 0;
        /// <summary>
        /// DAL
        /// </summary>
        protected IInsertionsDAL _dal;

        protected VehicleInformation _vehicleInformation;


        /// <summary>
        /// Chemin du fichier  média en téléchargement
        /// <remarks>Rajouter pour le passage en HTML5 avec la lecture via element video et audio</remarks>
        /// </summary>
        protected string _pathDownloadingFile = null;
        /// <summary>
        /// Chemin du fichier   média en lecture
        /// <remarks>Rajouter pour le passage en HTML5 avec la lecture via element video et audio</remarks>
        /// </summary>
        protected string _pathReadingFile = null;
        /// <summary>
        /// Vérifie si fichier video trouvé
        /// </summary>
        public bool IsVideoFileFound { get; set; }
        /// <summary>
        /// Vérifie si fichier audio trouvé
        /// </summary>
        public bool IsAudioFileFound { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idSlogan">Slogan identifier</param>
        /// <param name="file">File</param>
        /// <param name="webSession">WebSession</param>
        /// <param name="title">Title</param>
        public CreativePopUp(Page popUp, CstClassificationVehicle.names vehicle, string idSlogan, string file,
            WebSession webSession, string title, bool hasCreationReadRights, bool hasCreationDownloadRights)
        {
            IdProduct = null;
            _vehicle = vehicle;
            _idSlogan = idSlogan;
            _file = file;
            _webSession = webSession;
            _title = title;
            _hasCreationDownloadRights = hasCreationDownloadRights;
            _hasCreationReadRights = hasCreationReadRights;
            _popUp = popUp;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idSlogan">Slogan identifier</param>
        /// <param name="file">File</param>
        /// <param name="webSession">WebSession</param>      
        public CreativePopUp(CstClassificationVehicle.names vehicle, string idSlogan, string file,
            WebSession webSession, bool hasCreationReadRights, bool hasCreationDownloadRights)
        {
            IdProduct = null;
            _vehicle = vehicle;
            _idSlogan = idSlogan;
            _file = file;
            _webSession = webSession;
            _hasCreationDownloadRights = hasCreationDownloadRights;
            _hasCreationReadRights = hasCreationReadRights;

        }

        /// <summary>
        /// Identifiant Produit
        /// </summary>
        public string IdProduct { get; set; }

        /// <summary>
        /// Indique si l'utilisateur à le droit de lire les créations
        /// </summary>
        public bool HasCreationReadRights
        {
            get { return _hasCreationReadRights; }
            set { _hasCreationReadRights = value; }
        }

        /// <summary>
        /// Indique si l'utilisateur à le droit de télécharger les créations
        /// </summary>
        public bool HasCreationDownloadRights
        {
            get { return _hasCreationDownloadRights; }
            set { _hasCreationDownloadRights = value; }
        }

        #endregion

        public string PathDownloadingFile
        {
            get { return _pathDownloadingFile; }
            set { _pathDownloadingFile = value; }
        }

        public string PathReadingFile
        {
            get { return _pathReadingFile; }
            set { _pathReadingFile = value; }
        }

        #region Get Creative PopUp
        /// <summary>
        /// Get creative popup Html code
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string CreativePopUpRender()
        {
            //Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
            //lire ou de télécharger le fichier
            bool realFormatFound = false;
            bool windowsFormatFound = false;

            GetCreativePathes(ref realFormatFound, ref windowsFormatFound);

            var res = RenderCreative(realFormatFound, windowsFormatFound, true);

            return res.ToString();

        }
        /// <summary>
        /// Get creative popup Html code
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string CreativePopUpRenderWithoutOptions(int width, int height)
        {
            //Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
            //lire ou de télécharger le fichier
            bool realFormatFound = false;
            bool windowsFormatFound = false;

            _width = width;
            _height = height;

            GetCreativePathes(ref realFormatFound, ref windowsFormatFound);

            var res = RenderCreative(realFormatFound, windowsFormatFound, false);

            return res.ToString();

        }

        #endregion

        /// <summary>
        /// Set creative paths
        /// </summary>
        public virtual void SetCreativePaths()
        {
            //Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
            //lire ou de télécharger le fichier
            bool realFormatFound = false;
            bool windowsFormatFound = false;

            GetCreativePathes(ref realFormatFound, ref windowsFormatFound);


        }


        #region RenderCreative

        /// <summary>
        /// Render Creative
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Forma tFound</param>       
        /// <returns></returns>
        protected virtual StringBuilder RenderCreative(bool realFormatFound, bool windowsFormatFound, bool withOptions)
        {
            var res = new StringBuilder(2000);

            if ((realFormatFound || windowsFormatFound) && (_hasCreationReadRights || _hasCreationDownloadRights))
            {
                AddScripts();

                res.Append(
                    "<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"770\" border=\"0\" align=\"center\" height=\"100%\" ><TR>");
                //Rendu créations en lecture				
                if (_hasCreationReadRights)
                    res.Append(ManageCreationsDownloadWithCookies(_pathReadingRealFile, _pathReadingWindowsFile, true,
                                                                  realFormatFound, windowsFormatFound));

                if (withOptions)
                {
                    //Tableau des options
                    res.Append(GetCreationsOptionsRender(realFormatFound, windowsFormatFound, _pathDownloadingRealFile,
                                                         _pathDownloadingWindowsFile, false, 2079));
                }
            }
            else
            {
                AppendNoFileAvailable(res);
            }

            res.Append("</TR></TABLE>");
            return res;
        }

        #endregion

        #region AppendNoFileAvailable

        /// <summary>
        /// Append No File Available message
        /// </summary>
        /// <param name="res">String Builder</param>
        protected virtual void AppendNoFileAvailable(StringBuilder res)
        {
            //Aucun fichier n'est dispo	
            res.Append("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
            res.Append(
                "<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");
            res.Append("<TD  align=\"center\" class=\"txtViolet11Bold backGroundWhite\" >");
            res.Append(GestionWeb.GetWebWord(892, _webSession.SiteLanguage));
            res.Append("</TD>");
        }

        #endregion


        #region AddScripts

        /// <summary>
        /// Add Scripts
        /// </summary>
        protected virtual void AddScripts()
        {
            #region scripts

            //Gestion de l'ouverture du fichier windows media
            if (!_popUp.ClientScript.IsClientScriptBlockRegistered("GetObjectWindowsMediaPlayerRender"))
            {
                _popUp.ClientScript.RegisterClientScriptBlock(_popUp.GetType(), "GetObjectWindowsMediaPlayerRender",
                                                              GetObjectWindowsMediaPlayerRender(_webSession.SiteLanguage));
            }

            //Gestion de l'ouverture du fichier real media
            if (!_popUp.ClientScript.IsClientScriptBlockRegistered("GetObjectRealPlayer"))
            {
                _popUp.ClientScript.RegisterClientScriptBlock(_popUp.GetType(), "GetObjectRealPlayer",
                                                              GetObjectRealPlayer());
            }

            #endregion
        }

        #endregion


        #region GetCreativePathes

        /// <summary>
        /// Get Creative Pathes
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Forma tFound</param>       
        protected virtual void GetCreativePathes(ref bool realFormatFound, ref bool windowsFormatFound)
        {
            switch (_vehicle)
            {
                case CstClassificationVehicle.names.radio:
                case CstClassificationVehicle.names.radioGeneral:
                case CstClassificationVehicle.names.radioMusic:
                case CstClassificationVehicle.names.radioSponsorship:

                    IsRadioFileExists(ref realFormatFound, ref windowsFormatFound);
                    GetRadioCreativePathes();
                    break;
                case CstClassificationVehicle.names.tv:
                case CstClassificationVehicle.names.tvGeneral:
                case CstClassificationVehicle.names.tvSponsorship:
                case CstClassificationVehicle.names.tvAnnounces:
                case CstClassificationVehicle.names.tvNonTerrestrials:

                    IsTvFileExists(out realFormatFound, out windowsFormatFound);
                    GetTvCreativePathes();
                    break;

                case CstClassificationVehicle.names.others:

                    IsOthersFileExists(out realFormatFound, out windowsFormatFound);
                    GetOthersCreativePathes();
                    break;
                case CstClassificationVehicle.names.adnettrack:

                    IsEvaliantFileExists(out realFormatFound, out windowsFormatFound);
                    GetEvaliantCreativePathes();
                    break;
                default:
                    _webSession.Source.Close();
                    _popUp.Response.Redirect(string.Format("/Public/Message.aspx?msgTxt={0}&title={1}",
                                                           GestionWeb.GetWebWord(890, _webSession.SiteLanguage),
                                                           GestionWeb.GetWebWord(887, _webSession.SiteLanguage)));
                    break;
            }

        }

        #endregion

        /// <summary>
        /// Get Evaliant Creative Pathes
        /// </summary>
        protected virtual void GetEvaliantCreativePathes()
        {


            //Construction des chemins real et wm	
            if (_hasCreationReadRights)
                _pathReadingWindowsFile = _file;


            if (_hasCreationDownloadRights)
                _pathDownloadingWindowsFile = _file;// getCreativePath(CstWeb.CreationServerPathes.CREA_ADNETTRACK);


        }

        #region GetOthersCreativePathes

        /// <summary>
        /// Get Others Creative Pathes
        /// </summary>
        protected virtual void GetOthersCreativePathes()
        {
            Func<string, string> getCreativePath = s =>
                                                   string.Format("{0}/{1}/{2}/{3}.{4}", s, _file.Substring(0, 4),
                                                                 _file.Substring(4, 2), _file, WMV_EXTENSION);

            //Construction des chemins real et wm	
            if (_hasCreationReadRights)
                _pathReadingWindowsFile = getCreativePath(CstWeb.CreationServerPathes.READ_WM_PAN_EURO_SERVER);


            if (_hasCreationDownloadRights)
                _pathDownloadingWindowsFile = getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_PAN_EURO);


        }

        #endregion

        protected virtual void IsEvaliantFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            realFormatFound = false;
            windowsFormatFound = File.Exists(string.Format("{0}{1}"
                                                           , CstWeb.CreationServerPathes.LOCAL_PATH_EVALIANT
                                                           , _file.Replace(string.Format("{0}/", CstWeb.CreationServerPathes.CREA_ADNETTRACK), "").Replace("/", "\\")));
        }
        #region IsOthersFileExists

        /// <summary>
        /// Is Others FileExists
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Format Found</param>
        protected virtual void IsOthersFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            realFormatFound = false;

            windowsFormatFound = File.Exists(string.Format("{0}\\{1}\\{2}\\{3}.{4}",
                                                           CstWeb.CreationServerPathes.LOCAL_PATH_PAN_EURO,
                                                           _file.Substring(0, 4), _file.Substring(4, 2), _file,
                                                           WMV_EXTENSION));
        }

        #endregion


        #region GetTvCreativePathes

        /// <summary>
        /// Get Tv Creative Pathes
        /// </summary>
        protected virtual void GetTvCreativePathes()
        {
            Func<string, string, string> getCreativePath = (s, e) => string.Format("{0}/3/{1}/3{2}.{3}",
                                                                                   s, _file.Substring(0, 3),
                                                                                   _file, e);
            //Construction des chemins real et wm	
            if (_hasCreationReadRights)
            {
                //Fichiers en lectures (streaming)
                _pathReadingRealFile = getCreativePath(CstWeb.CreationServerPathes.READ_REAL_TV_SERVER, RM_EXTENSION);

                _pathReadingWindowsFile = getCreativePath(CstWeb.CreationServerPathes.READ_WM_TV_SERVER, WMV_EXTENSION);
            }

            Func<string, string, string> getCreativePath2 = (s, e) => string.Format("{0}/{3}/3/{1}/3{2}.{3}",
                                                                                    s, _file.Substring(0, 3),
                                                                                    _file, e);
            ;
            if (_hasCreationDownloadRights)
            {
                //Fichiers à télécharger 
                _pathDownloadingRealFile = getCreativePath2(CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER, RM_EXTENSION);

                _pathDownloadingWindowsFile = getCreativePath2(CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER,
                                                               WMV_EXTENSION);
            }
        }

        #endregion


        #region IsTvFileExists

        /// <summary>
        /// Is Tv File Exists
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Format Found</param>
        protected virtual void IsTvFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            Func<string, string, bool> isCreativeExists =
                (s, e) => File.Exists(string.Format("{0}\\{3}\\3\\{1}\\3{2}.{3}",
                                                    s, _file.Substring(0, 3),
                                                    _file, e));

            realFormatFound = isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO, RM_EXTENSION);

            windowsFormatFound = isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO, WMV_EXTENSION);
        }

        #endregion

        #region GetRadioCreativePathes

        /// <summary>
        /// Get Radio Creative Pathes
        /// </summary>
        protected virtual void GetRadioCreativePathes()
        {
            //Construction des chemins real et wm					
            if (_hasCreationReadRights)
            {
                Func<string, string, string> getCreativePath = (s, e) => string.Format("{0}/2/{1}/2{2}.{3}",
                                                                                       s, _idSlogan.Substring(0, 3),
                                                                                       _idSlogan, e);
                //Fichiers en lectures (streaming)
                _pathReadingRealFile = (_isNewRealAudioFilePath)
                                           ? getCreativePath(
                                               CstWeb.CreationServerPathes.READ_REAL_CREATIVES_RADIO_SERVER,
                                               RA_EXTENSION)
                                           : string.Format("{0}{1}", CstWeb.CreationServerPathes.READ_REAL_RADIO_SERVER,
                                                           _file);

                _pathReadingWindowsFile = (_isNewWindowsAudioFilePath)
                                              ? getCreativePath(
                                                  CstWeb.CreationServerPathes.READ_WM_CREATIVES_RADIO_SERVER,
                                                  WMA_EXTENSION)
                                              : string.Format("{0}{1}", CstWeb.CreationServerPathes.READ_WM_RADIO_SERVER,
                                                              _file.Replace("rm", WMA_EXTENSION));
            }

            if (_hasCreationDownloadRights)
            {
                Func<string, string, string> getCreativePath2 = (s, e) => string.Format("{0}/{3}/2/{1}/2{2}.{3}",
                                                                                        s, _idSlogan.Substring(0, 3),
                                                                                        _idSlogan, e);
                //Fichiers à télécharger 
                _pathDownloadingRealFile = (_isNewRealAudioFilePath)
                                               ? getCreativePath2(
                                                   CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER,
                                                   RA_EXTENSION)
                                               : CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER + _file;

                _pathDownloadingWindowsFile = (_isNewWindowsAudioFilePath)
                                                  ? getCreativePath2(
                                                      CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER,
                                                      WMA_EXTENSION)
                                                  : string.Format("{0}{1}",
                                                                  CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER,
                                                                  _file.Replace("rm", WMA_EXTENSION));
            }
        }

        #endregion


        #region IsRadioFileExists

        /// <summary>
        /// Is Radio File Exists
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Format Found</param>
        protected virtual void IsRadioFileExists(ref bool realFormatFound, ref bool windowsFormatFound)
        {
            Func<string, bool> isCreativeExists =
                e => _idSlogan != null && File.Exists(string.Format("{0}\\{3}\\2\\{1}\\2{2}.{3}",
                                                                    CstWeb.CreationServerPathes
                                                                          .LOCAL_PATH_CREATIVES_RADIO,
                                                                    _idSlogan.Substring(0, 3),
                                                                    _idSlogan, e));

            //Vérification de l'existence du fichier real
            if (isCreativeExists(RA_EXTENSION))
                _isNewRealAudioFilePath = realFormatFound = true;
            else if (File.Exists(string.Format("{0}{1}",
                                               CstWeb.CreationServerPathes.LOCAL_PATH_RADIO, _file.Replace("/", "\\"))))
                realFormatFound = true;


            //Vérification de l'existence du fichier wm
            if (isCreativeExists(WMA_EXTENSION))
                _isNewWindowsAudioFilePath = windowsFormatFound = true;
            else if (
                File.Exists(string.Format("{0}{1}", CstWeb.CreationServerPathes.LOCAL_PATH_RADIO,
                                          (_file.Replace("/", "\\")).Replace("rm", WMA_EXTENSION))))
                windowsFormatFound = true;
        }

        #endregion

        #region Gestion du Rendu

        #region Get Creations Options Render
        /// <summary>
        /// Obtient le rendu pour téléchargement d'une création
        /// </summary>
        /// <param name="realFormatFound">Indique si fichier real media existe</param>
        /// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
        /// <param name="path1">Chemin fichier real media</param>
        /// <param name="path2">Chemin fichier windows media </param>
        /// <param name="manageQuote">Indique si les quote sont gérés</param>
        /// <param name="code">Code traduction</param>
        /// <returns>rendu par défaut ouverture fichier</returns>
        protected virtual string GetCreationsOptionsRender(bool realFormatFound, bool windowsFormatFound, string path1, string path2, bool manageQuote, long code)
        {

            var result = new StringBuilder(1000);
            bool withDetail = false;

            DataSet ds = GetCreativeDS();

            if ((ds != null && ds.Tables[0].Rows.Count > 0) || (_hasCreationDownloadRights))
            {

                result.Append("<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"" + WebApplicationParameters.CustomStyles.CreativePopUpWidth + "\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD vAlign=\"top\">");
                //Détail version

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {

                    result.Append("<TR><TD><TABLE height=\"100%\"  ><TD>");

                    result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD style=\"vertical-align:top;\">");
                    result.Append(GestionWeb.GetWebWord(859, _webSession.SiteLanguage));
                    result.Append("</TD>");
                    result.Append("<TD>");
                    result.Append("&nbsp;:&nbsp;" + ds.Tables[0].Rows[0]["group_"].ToString().Replace("°", "<br/>&nbsp;&nbsp;&nbsp;"));
                    result.Append("</TD></TR>");

                    result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD style=\"vertical-align:top;\">");
                    result.Append(GestionWeb.GetWebWord(857, _webSession.SiteLanguage));
                    result.Append("</TD>");
                    result.Append("<TD>");
                    result.Append("&nbsp;:&nbsp;" + ds.Tables[0].Rows[0]["advertiser"].ToString().Replace("°", "<br/>&nbsp;&nbsp;&nbsp;"));
                    result.Append("</TD></TR>");

                    result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD style=\"vertical-align:top;\">");
                    result.Append(GestionWeb.GetWebWord(858, _webSession.SiteLanguage));
                    result.Append("</TD>");
                    result.Append("<TD>");
                    result.Append("&nbsp;:&nbsp;" + ds.Tables[0].Rows[0]["product"].ToString().Replace("°", "<br/>&nbsp;&nbsp;&nbsp;"));
                    result.Append("</TD></TR>");

                    result.Append("</TD></TABLE></TD></TR>");

                    result.Append("<TR><TD>&nbsp;</TD></TR>");

                    withDetail = true;
                }

                if (_hasCreationDownloadRights)
                {
                    if (windowsFormatFound)
                    {
                        result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_" + code + " class=\"txtViolet11Bold\">" +
                            GestionWeb.GetWebWord(code, _webSession.SiteLanguage) +
                            "</span></td></tr>");
                        result.Append("<TR><TD align=\"left\">");

                        string formatTypeText = !string.IsNullOrEmpty(path2)
                            && Path.GetExtension(path2).ToUpper().Trim() == string.Format(".{0}", AVI_EXTENSION)
                                               ? GestionWeb.GetWebWord(3003, _webSession.SiteLanguage)
                                               : GestionWeb.GetWebWord(2086, _webSession.SiteLanguage);
                        result.AppendFormat("<img src=/App_Themes/{0}/Images/Common/icoWindowsMediaPlayer.gif align=absmiddle>&nbsp;<a href=\"{1}\"  class=txtViolet11>{2}</a>"
                            , _popUp.Theme, path2, formatTypeText);
                        result.Append("</td></tr>");
                    }

                    if (realFormatFound)
                    {
                        if (!windowsFormatFound)
                        {
                            result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_" + code + " class=\"txtViolet11Bold\">" +
                                GestionWeb.GetWebWord(code, _webSession.SiteLanguage) +
                                "</span></td></tr>");
                        }
                        result.AppendFormat("<tr><td align=\"left\"><img src=/App_Themes/{0}/Images/Common/icoRealPlayer.gif align=absmiddle>&nbsp;<a href=\"{1}\"   class=txtViolet11>{2}</a>"
                            , _popUp.Theme, path1, GestionWeb.GetWebWord(2085, _webSession.SiteLanguage));
                        result.Append("</td></tr>");
                    }
                    if (!withDetail) result.Append("<TR><TD>&nbsp;</TD></TR>");
                }

                result.Append("</TD></TR></TBODY></TABLE></TD>");
            }

            return (result.ToString());
        }
        #endregion

        #region Get creative DS
        /// <summary>
        /// Get creative description
        /// </summary>
        /// <returns>DataSet</returns>
        protected virtual DataSet GetCreativeDS()
        {

            DataSet ds = null;

            if (CstClassification.DB.Vehicles.names.tv == _vehicle
                 || CstClassification.DB.Vehicles.names.tvGeneral == _vehicle
                 || CstClassification.DB.Vehicles.names.tvSponsorship == _vehicle
                 || CstClassification.DB.Vehicles.names.tvAnnounces == _vehicle
                 || CstClassification.DB.Vehicles.names.tvNonTerrestrials == _vehicle
                )
            {

                SetDAL();
                var vehicleDbId = _vehicleInformation == null ? VehiclesInformation.Get(_vehicle).DatabaseId : _vehicleInformation.DatabaseId;
                ds = _dal.GetVersion(_file, vehicleDbId);
            }

            return ds;
        }

        protected virtual void SetDAL()
        {
            if (_dal == null)
            {
                CoreLayer cl = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.insertionsDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for insertions DAL"));
                var param = new object[2];
                param[0] = _webSession;
                param[1] = _webSession.CurrentModule;
                _dal = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                                                                                           + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance
                                                                                                                                         | BindingFlags.Public, null, param, null, null);
            }
        }
        #endregion

        #region DownLoad Player Render

        /// <summary>
        /// Obtient le texte qui propose de télécharger le(s) lecteur(s) de fichier média
        /// </summary>
        /// <param name="realFormatFound">Indique si fichier real media existe</param>
        /// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
        /// <returns>Texte télécharger le(s) lecteur(s) de fichier média</returns>
        protected virtual string DownLoadPlayerRender(bool windowsFormatFound, bool realFormatFound)
        {

            StringBuilder result = new StringBuilder(1000);

            result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_ex class=\"txtViolet11Bold\">" +
            GestionWeb.GetWebWord(2087, _webSession.SiteLanguage) + "</span>");

            if (windowsFormatFound)
            {
                result.Append("&nbsp;<a href=\"http://www.microsoft.com/Windows/MediaPlayer/\"  target=\"_blank\" class=txtViolet11>"
                    + GestionWeb.GetWebWord(2088, _webSession.SiteLanguage) + "</a>");
            }

            if (realFormatFound)
            {
                if (windowsFormatFound) result.Append(",");
                result.Append("&nbsp;<a href=\"http://www.real.com\"  target=\"_blank\" class=txtViolet11>"
                    + GestionWeb.GetWebWord(2090, _webSession.SiteLanguage) + "</a>");
            }

            if (windowsFormatFound) result.Append(".</td></tr>");

            return (result.ToString());
        }
        #endregion

        #region Manage Creations Download With Cookies
        /// <summary>
        /// Ouvre un fichier media en fonction des cookies 
        /// </summary>
        /// <param name="path1">chemin fichier rm</param>
        /// <param name="path2">chemin fichier wmv</param>
        /// <param name="read">Indique si fichier en lecture</param>
        /// <param name="realFormatFound">Indique si format trouvé est Real </param>
        /// <param name="windowsFormatFound">Indique si format trouvé est Windows</param>
        /// <returns>script gesion cookies traitement fichier</returns>
        protected virtual string ManageCreationsDownloadWithCookies(string path1, string path2, bool read, bool realFormatFound, bool windowsFormatFound)
        {

            var res = new StringBuilder(2000);

            //Vérifie si le navigateur accepte les cookies
            if (_popUp.Request.Browser.Cookies)
            {


                //Si les cookies existent				
                HttpCookie cspotFileType = _popUp.Request.Cookies[CstWeb.Cookies.SpotFileType];

                //Ouvrir le fichier en lecture seule
                if (realFormatFound && windowsFormatFound)
                {

                    //Lire le type de média stocké en cookie
                    if (cspotFileType != null && cspotFileType.Value != null)
                    {

                        switch (cspotFileType.Value)
                        {

                            case WINDOWS_MEDIA_PLAYER_FORMAT:
                                res.AppendFormat("<script language=\"JavaScript\" type=\"text/javascript\">GetObjectWindowsMediaPlayerRender('{0}');</script>", path2);
                                break;

                            case REAL_MEDIA_PLAYER_FORMAT:
                                res.AppendFormat("<script language=\"JavaScript\" type=\"text/javascript\">GetObjectRealPlayer('{0}');</script>", path1);
                                break;

                            default:
                                break;
                        }
                    }
                    else
                    {
                        //Sinon lire un fichier média par défaut (en fonction du média player du client)
                        res.Append(ReadDefaultMediaPlayerRender(realFormatFound, windowsFormatFound, path1, path2));
                    }
                }
                else if (realFormatFound)
                {
                    res.AppendFormat("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectRealPlayer('{0}');", path1);
                    res.AppendFormat("\n setCookie('{0}','{1}');", CstWeb.Cookies.SpotFileType, REAL_MEDIA_PLAYER_FORMAT);
                    res.Append("</script>");
                }
                else
                {
                    res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectWindowsMediaPlayerRender('" + path2 + "');");
                    res.AppendFormat("\n setCookie('{0}','{1}');", CstWeb.Cookies.SpotFileType, WINDOWS_MEDIA_PLAYER_FORMAT);
                    res.Append("</script>");
                }
            }
            else
            {
                //Sinon lire un fichier média par défaut (en fonction du média player du client)
                res.Append(ReadDefaultMediaPlayerRender(realFormatFound, windowsFormatFound, path1, path2));
            }

            return res.ToString();
        }
        #endregion

        #region Read Default Media Player
        /// <summary>
        /// Rendu par défaut pour la lecture d'un fichier media
        /// </summary>
        /// <param name="realFormatFound">Indique si fichier real media existe</param>
        /// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
        /// <param name="path1">Chemin fichier real media</param>
        /// <param name="path2">Chemin fichier windows media </param>
        /// <returns>Rendu html lecture d'un fichier</returns>
        protected virtual string ReadDefaultMediaPlayerRender(bool realFormatFound, bool windowsFormatFound, string path1, string path2)
        {

            var res = new StringBuilder(2000);
            //Ajout des images à cliquer
            if (realFormatFound && windowsFormatFound)
            {

                //Les deux fichiers sont disponibles
                res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");

                //Si l'utilisateur possède le pugin Windows Media Player on charge le fichier windows Media 
                res.Append(" \n if(pluginlist.indexOf(\"Windows Media Player\")!=-1){");
                res.AppendFormat("	\n\t GetObjectWindowsMediaPlayerRender('{0}');", path2);
                res.AppendFormat(" \n\t setCookie('{0}','{1}');", CstWeb.Cookies.SpotFileType, WINDOWS_MEDIA_PLAYER_FORMAT);
                res.Append(" \n } ");

                //Sinon si l'utilisateur possède le pugin Windows RealPlayer on charge le fichier real Media 
                res.Append(" \n else if(pluginlist.indexOf(\"RealPlayer\")!=-1){ ");
                res.Append("	\n\t GetObjectRealPlayer('" + path1 + "');");
                res.Append(" \n\t setCookie('" + CstWeb.Cookies.SpotFileType + "','" + REAL_MEDIA_PLAYER_FORMAT + "');");
                res.Append(" \n } ");
                res.Append("\n else{ ");
                res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>');");
                res.AppendFormat("\n  document.write('{0}');", DownLoadPlayerRender(windowsFormatFound, realFormatFound));
                res.Append("  document.write('</TD></TR></TBODY></TABLE></TD>');");
                res.Append("\n } ");
                res.Append("</script>");
            }
            else
            {
                if (realFormatFound)
                {

                    //Seule le fichier real est disponible
                    res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectRealPlayer('" + path1 + "');");
                    res.AppendFormat("\n setCookie('{0}','{1}');", CstWeb.Cookies.SpotFileType, REAL_MEDIA_PLAYER_FORMAT);
                    res.Append("</script>");

                }
                else
                {

                    //Seul le fichier wm est dispo
                    res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectWindowsMediaPlayerRender('" + path2 + "');");
                    res.AppendFormat("\n setCookie('{0}','{1}');", CstWeb.Cookies.SpotFileType, WINDOWS_MEDIA_PLAYER_FORMAT);
                    res.Append("</script>");
                }
            }
            return res.ToString();
        }
        #endregion

        #region Windows Media Player Object
        /// <summary>
        /// Script d'ouveture d'un fichier Media Payer
        /// </summary>
        /// <returns>Script</returns>
        protected virtual string GetObjectWindowsMediaPlayerRender(int siteLanguage)
        {

            var res = new StringBuilder(2000);
            string width = (_width > 0) ? _width.ToString() : "352";
            string height = (_height > 0) ? _height.ToString() : "288";

            res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");
            res.Append(" function GetObjectWindowsMediaPlayerRender(filepath){");
            res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>');");
            //Lecture par Media player
            res.AppendFormat(" document.write('<object id=\"video1\"  classid=\"CLSID:22D6F312-B0F6-11D0-94AB-0080C74C7E95\" height=\"{0}\" width=\"{1}\" align=\"middle\"  codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,5,715\"  standby=\"{2}\" type=\"application/x-oleobject\">');"
                , height, width, GestionWeb.GetWebWord(1911, siteLanguage));
            res.Append(" document.write('<param name=\"FileName\" value='+filepath+' >');");
            res.Append(" document.write('<param name=\"AutoStart\" value=\"true\">');");
            res.Append(" document.write('<embed type=\"application/x-mplayer2\" pluginspage=\"http://www.microsoft.com/Windows/MediaPlayer/\"  src=\"'+filepath+'\" name=\"video1\" height=\"288\" width=\"352\" AutoStart=true>'); ");
            res.Append(" document.write('</embed>');");
            res.Append(" document.write('</object>');");
            res.Append(" document.write('</TD></TR></TBODY></TABLE></TD>');");
            res.Append(" }");
            res.Append("</script>");
            return res.ToString();
        }
        #endregion

        #region Real Player Object
        /// <summary>
        /// Script d'ouveture d'un fichier Real Payer
        /// </summary>
        /// <returns>Script</returns>
        protected virtual string GetObjectRealPlayer()
        {

            var res = new StringBuilder(2000);
            res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");
            res.Append(" function GetObjectRealPlayer(filepath){");
            res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>');");
            //Lecture par Real player
            res.Append(" document.write('<object classid=\"clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA\"  ID=\"Realaudio1\" height=\"288\" width=\"352\">');");
            res.Append(" document.write('<param name=\"console\" value=\"video\">');");
            res.Append(" document.write('<param name=\"controls\" value=\"all\">');");
            res.Append(" document.write('<param name=\"autostart\" value=\"true\">');");
            res.Append(" document.write('<param name=\"src\" value='+filepath+' >');");
            res.Append(" document.write('<embed  src='+filepath+' height=\"288\" width=\"352\" type=\"audio/x-pn-realaudio-plugin\"  controls=\"all\" pluginspage=\"http://www.real.com\" console=\"video\" autostart=\"true\">');");
            res.Append(" document.write('</embed>');");
            res.Append(" document.write('</object>');");
            res.Append(" document.write('</TD></TR></TBODY></TABLE></TD>');");
            res.Append(" }");
            res.Append("</script>");
            return res.ToString();
        }
        #endregion

        #endregion

    }
}
