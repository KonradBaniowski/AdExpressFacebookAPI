using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using CstWeb = TNS.AdExpress.Constantes.Web;
namespace TNS.AdExpressI.Insertions.Poland.CreativeResult
{
    public class CreativePopUp : Insertions.CreativeResult.CreativePopUp
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idSlogan">Slogan identifier</param>
        /// <param name="file">File</param>
        /// <param name="webSession">WebSession</param>
        /// <param name="title">Title</param>
        /// <param name="hasCreationReadRights">True if has Creation Read Rights</param>
        /// <param name="hasCreationDownloadRights">true if has Creation Download Rights</param>
        public CreativePopUp(Page popUp, Vehicles.names vehicle, string idSlogan, string file, WebSession webSession,
                             string title, bool hasCreationReadRights, bool hasCreationDownloadRights) :
                                 base(
                                 popUp, vehicle, idSlogan, file, webSession, title, hasCreationReadRights,
                                 hasCreationDownloadRights)
        {
        }

        public CreativePopUp(Vehicles.names vehicle, string idSlogan, string file,
           WebSession webSession, bool hasCreationReadRights, bool hasCreationDownloadRights) : base(vehicle, idSlogan, file,
            webSession, hasCreationReadRights, hasCreationDownloadRights)
        {

        }
        #endregion

        #region Old Code
        ///// <summary>
        ///// Get Creative Pathes
        ///// </summary>
        ///// <param name="realFormatFound">True if real Format Found</param>
        ///// <param name="windowsFormatFound">True if windows Forma tFound</param>       
        //protected override void GetCreativePathes(ref bool realFormatFound, ref bool windowsFormatFound)
        //{
        //       Func<string, string, string,bool> isCreativeExists = (s, e,v) =>
        //              File.Exists(string.Format("{0}\\{3}\\{1}\\{2}.{3}"
        //             , s, v.Substring(0, 5), v, e));

        //   Func<string, string, string,string> getCreativePath = (s, e,v) => 
        //    string.Format("{0}/{3}/{1}/{2}.{3}",s, v.Substring(0, 5), v, e);

        //    switch (_vehicle)
        //    {
        //        case Vehicles.names.radio:

        //            //Vérification de l'existence du fichier real
        //            if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO, RA_EXTENSION,_idSlogan))
        //                realFormatFound = true;

        //            //Vérification de l'existence du fichier wm
        //            if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO, WMA_EXTENSION, _idSlogan))
        //                windowsFormatFound = true;


        //            //Construction des chemins real et wm					
        //            if (_hasCreationReadRights)
        //            {
        //                //Fichiers en lectures (streaming)
        //                _pathDownloadingRealFile =
        //                    _pathReadingRealFile =
        //                    getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER, RA_EXTENSION, _idSlogan);

        //                _pathDownloadingWindowsFile =
        //                    _pathReadingWindowsFile = getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER,
        //                    WMA_EXTENSION, _idSlogan);

        //            }
        //            break;

        //        case Vehicles.names.tv:

        //            if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO, RM_EXTENSION, _file))
        //                realFormatFound = true;

        //            if (isCreativeExists(CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO, WMV_EXTENSION, _file))
        //                windowsFormatFound = true;

        //            //Construction des chemins real et wm	

        //            if (_hasCreationReadRights)
        //            {
        //                //Fichiers à télécharger 
        //                _pathReadingRealFile =  _pathDownloadingRealFile =
        //                    getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER, RM_EXTENSION, _file);

        //                _pathReadingWindowsFile = _pathDownloadingWindowsFile =
        //                    getCreativePath(CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER, WMV_EXTENSION, _file);

        //            }
        //            break;


        //        default:
        //            _webSession.Source.Close();
        //            _popUp.Response.Redirect(string.Format("/Public/Message.aspx?msgTxt={0}&title={1}",
        //                                                   GestionWeb.GetWebWord(890, _webSession.SiteLanguage),
        //                                                   GestionWeb.GetWebWord(887, _webSession.SiteLanguage)));
        //            break;
        //    }
        //}
        #endregion

        #region IsRadioFileExists
        /// <summary>
        /// Is Radio File Exists
        /// </summary>
        /// <param name="realFormatFound">True if real Format Found</param>
        /// <param name="windowsFormatFound">True if windows Format Found</param>
        protected override void IsRadioFileExists(ref bool realFormatFound, ref bool windowsFormatFound)
        {
            Func<string, bool> isCreativeExists =
                e => _idSlogan != null && File.Exists(string.Format("{0}\\{1}\\{2}.{3}",
                                                                    CstWeb.CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO,
                                                                    _idSlogan.Substring(0, 5),
                                                                    _idSlogan, e));

            IsAudioFileFound = isCreativeExists(MP3_EXTENSION);
        }
        #endregion

        protected override void GetRadioCreativePathes()
        {
            if (HasCreationReadRights)
                _pathReadingFile = string.Format("{0}\\{1}\\{2}.{3}", CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER, _idSlogan.Substring(0, 5), _idSlogan, MP3_EXTENSION);
            if (!HasCreationDownloadRights)
                return;

            _pathDownloadingFile = string.Format("{0}\\{1}\\{2}.{3}", CstWeb.CreationServerPathes.DOWNLOAD_RADIO_SERVER, _idSlogan.Substring(0, 5), _idSlogan, MP3_EXTENSION);
        }

        #region CreativePopUpRender
        public override string CreativePopUpRender()
        {
            bool flag = false;
            GetCreativePathes(ref flag, ref flag);
            return RenderCreative(flag, flag, true).ToString();
        }
        #endregion

        #region SetCreativePaths
        public override void SetCreativePaths()
        {
            bool flag = false;
            GetCreativePathes(ref flag, ref flag);
        }
        #endregion

        #region CreativePopUpRenderWithoutOptions
        public override string CreativePopUpRenderWithoutOptions(int width, int height)
        {
            bool realFormatFound = false;
            bool windowsFormatFound = false;
            _width = width;
            _height = height;
            GetCreativePathes(ref realFormatFound, ref windowsFormatFound);
            return RenderCreative(realFormatFound, windowsFormatFound, false).ToString();
        }
        #endregion

        #region RenderCreative
        protected override StringBuilder RenderCreative(bool realFormatFound, bool windowsFormatFound, bool withOptions)
        {
            var res = new StringBuilder(2000);
            if (!string.IsNullOrEmpty(_pathReadingFile)
                && (IsVideoFileFound || IsAudioFileFound)
                && (HasCreationReadRights || HasCreationDownloadRights))
            {
                res.Append("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"770\" border=\"0\" align=\"center\" height=\"100%\" ><TR>");
                if (HasCreationReadRights)
                {
                    switch (_vehicle)
                    {
                        case Vehicles.names.radio:
                        case Vehicles.names.radioGeneral:
                        case Vehicles.names.radioSponsorship:
                        case Vehicles.names.radioMusic:
                            res.Append(ReadAudioFile(_pathReadingFile));
                            break;
                        case Vehicles.names.tv:
                        case Vehicles.names.adnettrack:
                        case Vehicles.names.tvGeneral:
                        case Vehicles.names.tvSponsorship:
                        case Vehicles.names.tvAnnounces:
                        case Vehicles.names.tvNonTerrestrials:
                        case Vehicles.names.others:
                            res.Append(ReadVideoFile(_pathReadingFile));
                            break;

                    }
                }
                if (withOptions)
                    res.Append(GetCreationsOptionsRender(realFormatFound, windowsFormatFound, _pathDownloadingFile, _pathDownloadingFile, false, 2079L));
            }
            else
                this.AppendNoFileAvailable(res);
            res.Append("</TR></TABLE>");
            return res;
        }
        #endregion

        #region ReadAudioFile
        protected virtual string ReadAudioFile(string path)
        {
            var res = new StringBuilder(2000);
            res.Append("<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>");
            res.AppendFormat("<audio src=\"{0}\"  controls autoplay preload=\"auto\">", path);
            res.AppendFormat("{0}", GestionWeb.GetWebWord(3022L, _webSession.SiteLanguage));
            GetOldBrowserPlayer(path, res);
            res.AppendFormat("</audio >");
            res.Append("</TD></TR></TBODY></TABLE></TD>");
            return res.ToString();
        }
        #endregion

        #region ReadVideoFile
        protected virtual string ReadVideoFile(string path)
        {
            var res = new StringBuilder(2000);
            string str1 = _width > 0 ? _width.ToString() : "352";
            string str2 = _height > 0 ? _height.ToString() : "288";
            res.Append("<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>");
            res.AppendFormat("<video src=\"{0}\" width=\"{1}\" height=\"{2}\" controls autoplay preload=\"auto\">", path, str1, str2);
            res.AppendFormat("{0}", GestionWeb.GetWebWord(3021L, _webSession.SiteLanguage));
            GetOldBrowserPlayer(path, res);
            res.AppendFormat("</video >");
            res.Append("</TD></TR></TBODY></TABLE></TD>");
            return res.ToString();
        }
        #endregion

        #region GetOldBrowserPlayer
        protected virtual void GetOldBrowserPlayer(string path, StringBuilder res)
        {
            res.Append("<object id=\"video1\"  classid=\"CLSID:22D6F312-B0F6-11D0-94AB-0080C74C7E95\" height=\"288\" width=\"352\" align=\"middle\"");
            res.Append(" codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,5,715\" ");
            res.Append(" standby=\"Chargement en cours ...\" type=\"application/x-oleobject\">");
            res.AppendFormat("<param name=\"FileName\" value=\"{0}\" >", path);
            res.AppendFormat("<param name=\"AutoStart\" value=\"true\">");
            res.Append(" <embed type=\"application/x-mplayer2\" pluginspage=\"http://www.microsoft.com/Windows/MediaPlayer/\"  ");
            res.AppendFormat(" src=\"{0}\" name=\"video1\" height=\"288\" width=\"352\" AutoStart=true>", path);
            res.Append(" </embed></object>");
        }
        #endregion

        #region GetCreationsOptionsRender
        protected override string GetCreationsOptionsRender(bool realFormatFound, bool windowsFormatFound, string path1, string path2, bool manageQuote, long code)
        {
            var result = new StringBuilder(1000);
            bool flag = false;
            DataSet creativeDs = GetCreativeDS();
            if (creativeDs != null && creativeDs.Tables[0].Rows.Count > 0 || HasCreationDownloadRights)
            {
                result.AppendFormat("<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"{0}\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD vAlign=\"top\">"
                    , WebApplicationParameters.CustomStyles.CreativePopUpWidth);
                if (creativeDs != null && creativeDs.Tables[0].Rows.Count > 0)
                {
                    result.Append("<TR><TD><TABLE height=\"100%\"  ><TD>");
                    result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD style=\"vertical-align:top;\">");
                    result.Append(GestionWeb.GetWebWord(859L, _webSession.SiteLanguage));
                    result.Append("</TD>");
                    result.Append("<TD>");
                    result.AppendFormat("&nbsp;:&nbsp;{0}", creativeDs.Tables[0].Rows[0]["group_"].ToString().Replace("°", "<br/>&nbsp;&nbsp;&nbsp;"));
                    result.Append("</TD></TR>");
                    result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD style=\"vertical-align:top;\">");
                    result.Append(GestionWeb.GetWebWord(857L, _webSession.SiteLanguage));
                    result.Append("</TD>");
                    result.Append("<TD>");
                    result.AppendFormat("&nbsp;:&nbsp;{0}", creativeDs.Tables[0].Rows[0]["advertiser"].ToString().Replace("°", "<br/>&nbsp;&nbsp;&nbsp;"));
                    result.Append("</TD></TR>");
                    result.Append("<TR class=\"txtViolet11Bold\" nowrap><TD style=\"vertical-align:top;\">");
                    result.Append(GestionWeb.GetWebWord(858L, _webSession.SiteLanguage));
                    result.Append("</TD>");
                    result.Append("<TD>");
                    result.AppendFormat("&nbsp;:&nbsp;{0}", creativeDs.Tables[0].Rows[0]["product"].ToString().Replace("°", "<br/>&nbsp;&nbsp;&nbsp;"));
                    result.Append("</TD></TR>");
                    result.Append("</TD></TABLE></TD></TR>");
                    result.Append("<TR><TD>&nbsp;</TD></TR>");
                    flag = true;
                }
                if (HasCreationDownloadRights)
                {
                    if (IsVideoFileFound || IsAudioFileFound)
                    {
                        result.AppendFormat("<tr vAlign=\"middle\"><td align=\"left\" ><span  id=txt_{0} class=\"txtViolet11Bold\">{1}</span></td></tr>"
                            , code, GestionWeb.GetWebWord(code, _webSession.SiteLanguage));
                        result.Append("<TR><TD align=\"left\">");
                        GetFormatTypeText(result, path2);
                        result.Append("</td></tr>");
                    }
                    if (!flag)
                        result.Append("<TR><TD>&nbsp;</TD></TR>");
                }
                result.Append("</TD></TR></TBODY></TABLE></TD>");
            }
            return result.ToString();
        }
        #endregion

        #region GetFormatTypeText
        protected virtual void GetFormatTypeText(StringBuilder result, string path)
        {
            var theme = !string.IsNullOrEmpty(_popUp.Theme) ? _popUp.Theme : WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            switch (_vehicle)
            {
                case Vehicles.names.radio:
                case Vehicles.names.radioGeneral:
                case Vehicles.names.radioSponsorship:
                case Vehicles.names.radioMusic:
                    string webWord1 = GestionWeb.GetWebWord(3024L, _webSession.SiteLanguage);
                    result.AppendFormat("<img src=\"/App_Themes/{0}/Images/Common/icoMp3.png\" align=absmiddle>&nbsp;<a href=\"{1}\"  class=txtViolet11>{2}</a>", theme, path, webWord1);
                    break;
                case Vehicles.names.tv:
                case Vehicles.names.adnettrack:
                case Vehicles.names.tvGeneral:
                case Vehicles.names.tvSponsorship:
                case Vehicles.names.tvAnnounces:
                case Vehicles.names.tvNonTerrestrials:
                case Vehicles.names.others:
                    string webWord2 = GestionWeb.GetWebWord(3023L, _webSession.SiteLanguage);
                    result.AppendFormat("<img src=\"/App_Themes/{0}/Images/Common/icoMp4.png\" align=absmiddle>&nbsp;<a href=\"{1}\"  class=txtViolet11>{2}</a>", theme, path, webWord2);
                    break;

            }
        }
        #endregion

        protected override void IsTvFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            Func<string, bool> isCreativeExists =
                e => _idSlogan != null && File.Exists(string.Format("{0}\\{1}\\{2}.{3}",
                         CstWeb.CreationServerPathes.LOCAL_PATH_VIDEO,
                         _idSlogan.Substring(0, 5),
                         _idSlogan, e));

            IsVideoFileFound = windowsFormatFound = isCreativeExists(MP4_EXTENSION);
            realFormatFound = false;
        }

        protected override void GetTvCreativePathes()
        {
            if (HasCreationReadRights)
                _pathReadingFile = string.Format("{0}\\{1}\\{2}.{3}", CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER,
                    _idSlogan.Substring(0, 5), _idSlogan, MP4_EXTENSION);

            if (!HasCreationDownloadRights)
                return;

            _pathDownloadingFile = string.Format("{0}\\{1}\\{2}.{3}", CstWeb.CreationServerPathes.DOWNLOAD_TV_SERVER,
                _idSlogan.Substring(0, 5), _idSlogan, MP4_EXTENSION);
        }
    }
}
