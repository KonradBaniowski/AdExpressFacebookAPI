﻿using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Insertions.Slovakia.CreativeResult
{
    public class CreativePopUp : Insertions.CreativeResult.CreativePopUp
    {
        protected string _associated_file = string.Empty;

        #region Constructor
        public CreativePopUp(Page popUp, Vehicles.names vehicle, string idSlogan, string file, WebSession webSession, string title, bool hasCreationReadRights, bool hasCreationDownloadRights)
           : base(popUp, vehicle, idSlogan, file, webSession, title, hasCreationReadRights, hasCreationDownloadRights)
        {
        }
        public CreativePopUp(Vehicles.names vehicle, string idSlogan, string file,
           WebSession webSession, bool hasCreationReadRights, bool hasCreationDownloadRights) : base(vehicle, idSlogan, file,
            webSession, hasCreationReadRights, hasCreationDownloadRights)
        {

        }
        #endregion

        public override string CreativePopUpRender()
        {
            bool flag = false;
            GetCreativePathes(ref flag, ref flag);
            return RenderCreative(flag, flag, true).ToString();
        }

        /// <summary>
        /// Set creative paths
        /// </summary>
        public override void SetCreativePaths()
        {
            bool flag = false;
            GetCreativePathes(ref flag, ref flag);
        }

        public override string CreativePopUpRenderWithoutOptions(int width, int height)
        {
            bool realFormatFound = false;
            bool windowsFormatFound = false;
            _width = width;
            _height = height;
            GetCreativePathes(ref realFormatFound, ref windowsFormatFound);
            return RenderCreative(realFormatFound, windowsFormatFound, false).ToString();
        }

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

        protected override void IsRadioFileExists(ref bool realFormatFound, ref bool windowsFormatFound)
        {
            SetDAL();
            _associated_file = _vehicleInformation == null ? _dal.GetVersionMinParutionDate(_idSlogan,
                VehiclesInformation.Get(_vehicle)) : _dal.GetVersionMinParutionDate(_idSlogan, _vehicleInformation);

            IsAudioFileFound = !string.IsNullOrEmpty(_associated_file) && File.Exists(string.Format("{0}\\{1}",
                CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO, _associated_file));

        }

        protected override void GetRadioCreativePathes()
        {
            if (HasCreationReadRights && !string.IsNullOrEmpty(_associated_file))
                _pathReadingFile = string.Format("{0}/{1}", CreationServerPathes.DOWNLOAD_RADIO_SERVER, _associated_file);
            if (!HasCreationDownloadRights)
                return;
            if (!string.IsNullOrEmpty(_associated_file))
                _pathDownloadingFile = string.Format("{0}/{1}", CreationServerPathes.DOWNLOAD_RADIO_SERVER, _associated_file);
        }

        protected override void IsTvFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            SetDAL();
            _associated_file = _vehicleInformation == null ? _dal.GetVersionMinParutionDate(_idSlogan, VehiclesInformation.Get(_vehicle))
                : _dal.GetVersionMinParutionDate(_idSlogan, _vehicleInformation);

            if (!string.IsNullOrEmpty(_associated_file))
            {

                const string format = "{0}\\{1}";
                var objArray = new object[2]
                    {
                        CreationServerPathes.LOCAL_PATH_VIDEO,
                        _associated_file
                    };

                realFormatFound = false;
                IsVideoFileFound = windowsFormatFound = File.Exists(string.Format(format, objArray));

            }
            else
            {
                realFormatFound = false;
                IsVideoFileFound = windowsFormatFound = false;
            }

        }

        protected override void GetTvCreativePathes()
        {
            if (HasCreationReadRights && !string.IsNullOrEmpty(_associated_file))
                _pathReadingFile = string.Format("{0}/{1}", CreationServerPathes.DOWNLOAD_TV_SERVER,
                    _associated_file);
            if (!HasCreationDownloadRights)
                return;
            if (!string.IsNullOrEmpty(_associated_file))
                _pathDownloadingFile = string.Format("{0}/{1}", CreationServerPathes.DOWNLOAD_TV_SERVER,
                    _associated_file);
        }
    }
}
