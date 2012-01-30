using System;
using System.Collections.Generic;
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

namespace TNS.AdExpressI.Insertions.CreativeResult{
    /// <summary>
    /// Class used to display tv and radio creatives in reading and streamin mode
    /// </summary>
    public class CreativePopUp{

        #region constantes
        /// <summary>
        /// Constante cookie pour fichier format windows media player 
        /// </summary>
        protected const string WINDOWS_MEDIA_PLAYER_FORMAT = "windowsPlayerFormat";
        /// <summary>
        ///  Constante cookie pour fichier format real media player 
        /// </summary>
        protected const string REAL_MEDIA_PLAYER_FORMAT = "realPalyerFormat";
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
        public CreativePopUp(Page popUp, CstClassificationVehicle.names vehicle, string idSlogan, string file, WebSession webSession, string title, bool hasCreationReadRights, bool hasCreationDownloadRights){
            _vehicle = vehicle;
            _idSlogan = idSlogan;
            _file = file;
            _webSession = webSession;
            _title = title;
            _hasCreationDownloadRights = hasCreationDownloadRights;
            _hasCreationReadRights = hasCreationReadRights;
            _popUp = popUp;
        }
        #endregion

        #region Get Creative PopUp
        /// <summary>
        /// Get creative popup Html code
        /// </summary>
        /// <returns>Html code</returns>
        public virtual string CreativePopUpRender()
        {

            #region Construction et vérification des chemins d'accès aux fichiers real ou wm
            //Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
            //lire ou de télécharger le fichier
            bool realFormatFound = true;
            bool windowsFormatFound = true;

            switch (_vehicle){

                case CstClassification.DB.Vehicles.names.radio:
                case CstClassification.DB.Vehicles.names.radioGeneral:
                case CstClassification.DB.Vehicles.names.radioMusic:
                case CstClassification.DB.Vehicles.names.radioSponsorship:
                    //Vérification de l'existence du fichier real
                    if (_idSlogan != null && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO + @"\ra\2\" + _idSlogan.Substring(0, 3) + @"\2" + _idSlogan + ".ra")){
                        _isNewRealAudioFilePath = realFormatFound = true;
                    }
                    else if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO + _file.Replace("/", "\\"))){
                        realFormatFound = true;
                    }
                    else realFormatFound = false;

                    //Vérification de l'existence du fichier wm
                    if (_idSlogan != null && File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO + @"\wma\2\" + _idSlogan.Substring(0, 3) + @"\2" + _idSlogan + ".wma")){
                        _isNewWindowsAudioFilePath = windowsFormatFound = true;
                    }
                    else if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_RADIO + (_file.Replace("/", "\\")).Replace("rm", "wma"))){
                        windowsFormatFound = true;
                    }
                    else windowsFormatFound = false;

                    #region Test
#if DEBUG
                    //isNewRealAudioFilePath = isNewWindowsAudioFilePath = true;
#endif

                    #endregion

                    //Construction des chemins real et wm					
                    if (_hasCreationReadRights){
                        //Fichiers en lectures (streaming)
                        _pathReadingRealFile = (_isNewRealAudioFilePath) ? CstWeb.CreationServerPathes.READ_REAL_CREATIVES_RADIO_SERVER + "/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".ra" : CstWeb.CreationServerPathes.READ_REAL_RADIO_SERVER + _file;
                        _pathReadingWindowsFile = (_isNewWindowsAudioFilePath) ? CstWeb.CreationServerPathes.READ_WM_CREATIVES_RADIO_SERVER + "/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".wma" : CstWeb.CreationServerPathes.READ_WM_RADIO_SERVER + _file.Replace("rm", "wma");
                    }

                    if (_hasCreationDownloadRights){
                        //Fichiers à télécharger 
                        _pathDownloadingRealFile = (_isNewRealAudioFilePath) ? CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER + "/ra/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".ra" : CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER + _file;
                        _pathDownloadingWindowsFile = (_isNewWindowsAudioFilePath) ? CstWeb.CreationServerPathes.DOWNLOAD_CREATIVES_RADIO_SERVER + "/wma/2/" + _idSlogan.Substring(0, 3) + "/2" + _idSlogan + ".wma" : CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER + _file.Replace("rm", "wma");
                    }
                    break;

                case CstClassification.DB.Vehicles.names.tv:
                case CstClassification.DB.Vehicles.names.tvGeneral:
                case CstClassification.DB.Vehicles.names.tvSponsorship:
                case CstClassification.DB.Vehicles.names.tvAnnounces:
                case CstClassification.DB.Vehicles.names.tvNonTerrestrials:

                    if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO + @"\rm\3\" + _file.Substring(0, 3) + @"\3" + _file + ".rm")){
                        realFormatFound = true;
                    }
                    else realFormatFound = false;
                    if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO + @"\wmv\3\" + _file.Substring(0, 3) + @"\3" + _file + ".wmv")){
                        windowsFormatFound = true;
                    }
                    else windowsFormatFound = false;

                    //Construction des chemins real et wm	
                    if (_hasCreationReadRights){
                        //Fichiers en lectures (streaming)
                        _pathReadingRealFile = CstWeb.CreationServerPathes.READ_REAL_TV_SERVER + "/3/" + _file.Substring(0, 3) + "/3" + _file + ".rm";
                        _pathReadingWindowsFile = CstWeb.CreationServerPathes.READ_WM_TV_SERVER + "/3/" + _file.Substring(0, 3) + "/3" + _file + ".wmv";
                    }

                    if (_hasCreationDownloadRights){
                        //Fichiers à télécharger 
                        _pathDownloadingRealFile = CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER + "/rm/3/" + _file.Substring(0, 3) + "/3" + _file + ".rm";
                        _pathDownloadingWindowsFile = CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER + "/wmv/3/" + _file.Substring(0, 3) + "/3" + _file + ".wmv";
                    }
                    break;

                case CstClassification.DB.Vehicles.names.others:

                    realFormatFound = false;

                    if (File.Exists(CstWeb.CreationServerPathes.LOCAL_PATH_PAN_EURO + @"\" + _file.Substring(0, 4) + @"\" + _file.Substring(4, 2) + @"\" + _file + ".wmv")){
                        windowsFormatFound = true;
                    }
                    else windowsFormatFound = false;

                    //Construction des chemins real et wm	
                    if (_hasCreationReadRights){
                        _pathReadingWindowsFile = CstWeb.CreationServerPathes.READ_WM_PAN_EURO_SERVER + "/" + _file.Substring(0, 4) + "/" + _file.Substring(4, 2) + "/" + _file + ".wmv";
                    }

                    if (_hasCreationDownloadRights){
                        _pathDownloadingWindowsFile = CstWeb.CreationServerPathes.DOWNLOAD_PAN_EURO + "/" + _file.Substring(0, 4) + "/" + _file.Substring(4, 2) + "/" + _file + ".wmv";
                    }
                    break;
                default:
                    _webSession.Source.Close();
                    _popUp.Response.Redirect("/Public/Message.aspx?msgTxt=" + GestionWeb.GetWebWord(890, _webSession.SiteLanguage) + "&title=" + GestionWeb.GetWebWord(887, _webSession.SiteLanguage));
                    break;
            }
            #endregion

            #region Design Tableau d'images real ou wm suivant la disponibilité des fichiers

            #region Test
#if DEBUG
            //realFormatFound = true;
            //windowsFormatFound = true;
#endif

            #endregion

            StringBuilder res = new StringBuilder(2000);

            if ((realFormatFound || windowsFormatFound) && (_hasCreationReadRights || _hasCreationDownloadRights)){

                #region scripts
                //Gestion de l'ouverture du fichier windows media
                if (!_popUp.ClientScript.IsClientScriptBlockRegistered("GetObjectWindowsMediaPlayerRender")){
                    _popUp.ClientScript.RegisterClientScriptBlock(_popUp.GetType(), "GetObjectWindowsMediaPlayerRender", GetObjectWindowsMediaPlayerRender(_webSession.SiteLanguage));
                }

                //Gestion de l'ouverture du fichier real media
                if (!_popUp.ClientScript.IsClientScriptBlockRegistered("GetObjectRealPlayer")){
                    _popUp.ClientScript.RegisterClientScriptBlock(_popUp.GetType(), "GetObjectRealPlayer", GetObjectRealPlayer());
                }
                #endregion

                res.Append("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"770\" border=\"0\" align=\"center\" height=\"100%\" ><TR>");
                //Rendu créations en lecture				
                if (_hasCreationReadRights) res.Append(ManageCreationsDownloadWithCookies(_pathReadingRealFile, _pathReadingWindowsFile, true, realFormatFound, windowsFormatFound));

                //Tableau des options
                res.Append(GetCreationsOptionsRender(realFormatFound, windowsFormatFound, _pathDownloadingRealFile, _pathDownloadingWindowsFile, false, 2079));
            }
            else{
                //Aucun fichier n'est dispo	
                res.Append("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
                res.Append("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");
                res.Append("<TD  align=\"center\" class=\"txtViolet11Bold backGroundWhite\" >");
                res.Append(GestionWeb.GetWebWord(892, _webSession.SiteLanguage));
                res.Append("</TD>");
            }
            
            res.Append("</TR></TABLE>");

            return res.ToString();

            //streamingCreationsResult = res.ToString();
            #endregion

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

            StringBuilder result = new StringBuilder(1000);
            bool withDetail = false;

            DataSet ds = GetCreativeDS();
            
            if ((ds != null && ds.Tables[0].Rows.Count > 0) || (_hasCreationDownloadRights)){

                result.Append("<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"394\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD vAlign=\"top\">");
                //Détail version

                if (ds != null && ds.Tables[0].Rows.Count > 0){

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

                    result.Append("<TR height=100%><TD>&nbsp;</TD></TR>");

                    withDetail = true;
                }

                if (_hasCreationDownloadRights){
                    if (windowsFormatFound){
                        result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_" + code + " class=\"txtViolet11Bold\">" +
                            GestionWeb.GetWebWord(code, _webSession.SiteLanguage) +
                            "</span></td></tr>");
                        result.Append("<TR><TD align=\"left\">");
                        result.Append("<img src=/App_Themes/" + _popUp.Theme + "/Images/Common/icoWindowsMediaPlayer.gif align=absmiddle>&nbsp;<a href=\"" + path2 + "\"  class=txtViolet11>" + GestionWeb.GetWebWord(2086, _webSession.SiteLanguage) + "</a>");
                        result.Append("</td></tr>");
                    }

                    if (realFormatFound){
                        if (!windowsFormatFound){
                            result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_" + code + " class=\"txtViolet11Bold\">" +
                                GestionWeb.GetWebWord(code, _webSession.SiteLanguage) +
                                "</span></td></tr>");
                        }
                        result.Append("<tr><td align=\"left\"><img src=/App_Themes/" + _popUp.Theme + "/Images/Common/icoRealPlayer.gif align=absmiddle>&nbsp;<a href=\"" + path1 + "\"   class=txtViolet11>" + GestionWeb.GetWebWord(2085, _webSession.SiteLanguage) + "</a>");
                        result.Append("</td></tr>");
                    }
                    if (!withDetail) result.Append("<TR height=100%><TD>&nbsp;</TD></TR>");
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
        protected virtual  DataSet GetCreativeDS() {

            DataSet ds = null;

            if (CstClassification.DB.Vehicles.names.tv == _vehicle
                 || CstClassification.DB.Vehicles.names.tvGeneral == _vehicle
                 || CstClassification.DB.Vehicles.names.tvSponsorship == _vehicle
                 || CstClassification.DB.Vehicles.names.tvAnnounces == _vehicle
                 || CstClassification.DB.Vehicles.names.tvNonTerrestrials == _vehicle
                ) {

                CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertionsDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for insertions DAL"));
                object[] param = new object[2];
                param[0] = _webSession;
                param[1] = _webSession.CurrentModule;
                IInsertionsDAL dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);
                ds = dalLayer.GetVersion(_file,VehiclesInformation.Get(_vehicle).DatabaseId);
            }

            return ds;
        }
        #endregion

        #region DownLoad Player Render
        /// <summary>
        /// Obtient le texte qui propose de télécharger le(s) lecteur(s) de fichier média
        /// </summary>
        /// <param name="realFormatFound">Indique si fichier real media existe</param>
        /// <param name="windowsFormatFound">Indique si fichier windows media existe</param>
        /// <param name="webSession">Session du client</param>
        /// <returns>Texte télécharger le(s) lecteur(s) de fichier média</returns>
        protected virtual string DownLoadPlayerRender(bool windowsFormatFound, bool realFormatFound)
        {

            StringBuilder result = new StringBuilder(1000);

            result.Append("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_ex class=\"txtViolet11Bold\">" +
            GestionWeb.GetWebWord(2087, _webSession.SiteLanguage) + "</span>");

            if (windowsFormatFound){
                result.Append("&nbsp;<a href=\"http://www.microsoft.com/Windows/MediaPlayer/\"  target=\"_blank\" class=txtViolet11>" + GestionWeb.GetWebWord(2088, _webSession.SiteLanguage) + "</a>");
            }

            if (realFormatFound){
                if (windowsFormatFound) result.Append(",");
                result.Append("&nbsp;<a href=\"http://www.real.com\"  target=\"_blank\" class=txtViolet11>" + GestionWeb.GetWebWord(2090, _webSession.SiteLanguage) + "</a>");
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

            StringBuilder res = new StringBuilder(2000);
            HttpCookie cspotFileType = null;

            //Vérifie si le navigateur accepte les cookies
            if (_popUp.Request.Browser.Cookies){
                //if(false){

                //Si les cookies existent				
                cspotFileType = _popUp.Request.Cookies[CstWeb.Cookies.SpotFileType];

                //Ouvrir le fichier en lecture seule
                if (realFormatFound && windowsFormatFound){

                    //Lire le type de média stocké en cookie
                    if (cspotFileType != null && cspotFileType.Value != null){

                        switch (cspotFileType.Value){

                            case WINDOWS_MEDIA_PLAYER_FORMAT:
                                res.Append("<script language=\"JavaScript\" type=\"text/javascript\">GetObjectWindowsMediaPlayerRender('" + path2 + "');</script>");
                                break;

                            case REAL_MEDIA_PLAYER_FORMAT:
                                res.Append("<script language=\"JavaScript\" type=\"text/javascript\">GetObjectRealPlayer('" + path1 + "');</script>");
                                break;

                            default:
                                break;
                        }
                    }
                    else{
                        //Sinon lire un fichier média par défaut (en fonction du média player du client)
                        res.Append(ReadDefaultMediaPlayerRender(realFormatFound, windowsFormatFound, path1, path2));
                    }
                }
                else if (realFormatFound){
                    res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectRealPlayer('" + path1 + "');");
                    res.Append("\n setCookie('" + CstWeb.Cookies.SpotFileType + "','" + REAL_MEDIA_PLAYER_FORMAT + "');");
                    res.Append("</script>");
                }
                else{
                    res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectWindowsMediaPlayerRender('" + path2 + "');");
                    res.Append("\n setCookie('" + CstWeb.Cookies.SpotFileType + "','" + WINDOWS_MEDIA_PLAYER_FORMAT + "');");
                    res.Append("</script>");
                }
            }
            else{
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

            StringBuilder res = new StringBuilder(2000);
            //Ajout des images à cliquer
            if (realFormatFound && windowsFormatFound){

                //Les deux fichiers sont disponibles
                res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");

                //Si l'utilisateur possède le pugin Windows Media Player on charge le fichier windows Media 
                res.Append(" \n if(pluginlist.indexOf(\"Windows Media Player\")!=-1){");
                res.Append("	\n\t GetObjectWindowsMediaPlayerRender('" + path2 + "');");
                res.Append(" \n\t setCookie('" + CstWeb.Cookies.SpotFileType + "','" + WINDOWS_MEDIA_PLAYER_FORMAT + "');");
                res.Append(" \n } ");

                //Sinon si l'utilisateur possède le pugin Windows RealPlayer on charge le fichier real Media 
                res.Append(" \n else if(pluginlist.indexOf(\"RealPlayer\")!=-1){ ");
                res.Append("	\n\t GetObjectRealPlayer('" + path1 + "');");
                res.Append(" \n\t setCookie('" + CstWeb.Cookies.SpotFileType + "','" + REAL_MEDIA_PLAYER_FORMAT + "');");
                res.Append(" \n } ");
                res.Append("\n else{ ");
                res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>');");
                res.Append("\n  document.write('" + DownLoadPlayerRender(windowsFormatFound, realFormatFound) + "');");
                res.Append("  document.write('</TD></TR></TBODY></TABLE></TD>');");
                res.Append("\n } ");
                res.Append("</script>");
            }
            else{
                if (realFormatFound){

                    //Seule le fichier real est disponible
                    res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectRealPlayer('" + path1 + "');");
                    res.Append("\n setCookie('" + CstWeb.Cookies.SpotFileType + "','" + REAL_MEDIA_PLAYER_FORMAT + "');");
                    res.Append("</script>");

                }
                else{

                    //Seul le fichier wm est dispo
                    res.Append("<script language=\"JavaScript\" type=\"text/javascript\">\n GetObjectWindowsMediaPlayerRender('" + path2 + "');");
                    res.Append("\n setCookie('" + CstWeb.Cookies.SpotFileType + "','" + WINDOWS_MEDIA_PLAYER_FORMAT + "');");
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

            StringBuilder res = new StringBuilder(2000);
            res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");
            res.Append(" function GetObjectWindowsMediaPlayerRender(filepath){");
            res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>');");
            //Lecture par Media player
            res.Append(" document.write('<object id=\"video1\"  classid=\"CLSID:22D6F312-B0F6-11D0-94AB-0080C74C7E95\" height=\"288\" width=\"352\" align=\"middle\"  codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,5,715\"  standby=\"" + GestionWeb.GetWebWord(1911, siteLanguage) + "\" type=\"application/x-oleobject\">');");
            res.Append(" document.write('<param name=\"FileName\" value='+filepath+' >');");
            res.Append(" document.write('<param name=\"AutoStart\" value=\"true\">');");
            res.Append(" document.write('<embed type=\"application/x-mplayer2\" pluginspage=\"http://www.microsoft.com/Windows/MediaPlayer/\"  src='+filepath+' name=\"video1\" height=\"288\" width=\"352\" AutoStart=true>'); ");
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

            StringBuilder res = new StringBuilder(2000);
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
