

using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Classification.DAL;

namespace TNS.AdExpressI.Insertions.France.CreativeResult
{
    public class CreativePopUp : TNS.AdExpressI.Insertions.CreativeResult.CreativePopUp
    {
        protected string _dateMedia = string.Empty;

        public CreativePopUp(Page popUp, Vehicles.names vehicle, string idSlogan, string file, WebSession webSession, string title, bool hasCreationReadRights, bool hasCreationDownloadRights)
            : base(popUp, vehicle, idSlogan, file, webSession, title, hasCreationReadRights, hasCreationDownloadRights)
        {
        }
        public CreativePopUp(Vehicles.names vehicle, string idSlogan, string file,
           WebSession webSession, bool hasCreationReadRights, bool hasCreationDownloadRights) : base(vehicle, idSlogan, file,
            webSession, hasCreationReadRights, hasCreationDownloadRights)
        {

        }
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

        protected override DataSet GetCreativeDS()
        {
            DataSet dataSet = null;
            if (!string.IsNullOrEmpty(IdProduct))
            {
                CoreLayer coreLayer = WebApplicationParameters.CoreLayers[Layers.Id.classification];
                if (coreLayer == null)
                    throw new NullReferenceException("Core layer is null for the Classification DAL");
                var args = new object[] { _webSession };
                dataSet = ((IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, coreLayer.AssemblyName), coreLayer.Class
                    , false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, args, null, null)).GetAllProduct(IdProduct);
            }
            else if (Vehicles.names.tv == _vehicle)
            {
                SetDAL();
                dataSet = _dal.GetVersion(_file, _vehicleInformation == null ? VehiclesInformation.Get(_vehicle).DatabaseId : _vehicleInformation.DatabaseId);
            }
            return dataSet;
        }

        protected override void IsEvaliantFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            realFormatFound = false;

            IsVideoFileFound = windowsFormatFound = File.Exists(string.Format("{0}{1}"
                                                                 , CreationServerPathes.LOCAL_PATH_EVALIANT
                                                                 , _file.Replace(string.Format("{0}/", CreationServerPathes.CREA_ADNETTRACK), "").Replace("/", "\\")));
        }

        protected override void GetEvaliantCreativePathes()
        {
            if (HasCreationReadRights)
                _pathReadingFile = _file;
            if (!HasCreationDownloadRights)
                return;
            _pathDownloadingFile = _file;
        }

        protected override void IsTvFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            SetDAL();
            _dateMedia = _vehicleInformation == null ? _dal.GetVersionMinParutionDate(_idSlogan, VehiclesInformation.Get(_vehicle))
                : _dal.GetVersionMinParutionDate(_idSlogan, _vehicleInformation);

            if (!string.IsNullOrEmpty(_dateMedia))
            {

                const string format = "{0}\\{1}\\{2}\\3{3}.{4}";
                var objArray = new object[5]
                    {
                        CreationServerPathes.LOCAL_PATH_VIDEO,
                        this._dateMedia.Substring(0, 4),
                        "240",
                        _idSlogan,
                        MP4_EXTENSION
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

        protected override void IsDoohFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            // _idSlogan contains the name of the creative file : 8******.mp4
            // We need to remove 8 at the beginning in order to check date media in Slogan table
            string idSlogan = _idSlogan.Substring(1);
            SetDAL();
            _dateMedia = _vehicleInformation == null ? _dal.GetVersionMinParutionDate(idSlogan, VehiclesInformation.Get(_vehicle))
                : _dal.GetVersionMinParutionDate(idSlogan, _vehicleInformation);

            if (!string.IsNullOrEmpty(_dateMedia))
            {

                const string format = "{0}\\{1}\\{2}.{3}";
                var objArray = new object[4]
                    {
                        CreationServerPathes.LOCAL_PATH_VIDEO,
                        this._dateMedia.Substring(0, 4),
                        _idSlogan,
                        MP4_EXTENSION
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

        protected override void IsRadioFileExists(ref bool realFormatFound, ref bool windowsFormatFound)
        {
            SetDAL();
            _dateMedia = _vehicleInformation == null ? _dal.GetVersionMinParutionDate(_idSlogan,
                VehiclesInformation.Get(_vehicle)) : _dal.GetVersionMinParutionDate(_idSlogan, _vehicleInformation);

            IsAudioFileFound = !string.IsNullOrEmpty(_dateMedia) && File.Exists(string.Format("{0}\\{1}\\{2}\\2{3}.{2}",
                CreationServerPathes.LOCAL_PATH_CREATIVES_RADIO, _dateMedia.Substring(0, 4), "MP3", _idSlogan));

        }

        protected override void IsOthersFileExists(out bool realFormatFound, out bool windowsFormatFound)
        {
            realFormatFound = false;


            const string format = "{0}\\{1}\\{2}\\{3}.{4}";
            var objArray = new object[5]
      {
         CreationServerPathes.LOCAL_PATH_PAN_EURO,
        _file.Substring(0, 4),
        _file.Substring(4, 2),
        _file,
         MP4_EXTENSION
      };

            IsAudioFileFound = windowsFormatFound = File.Exists(string.Format(format, objArray));


        }

        protected override void GetTvCreativePathes()
        {
            SetDateMedia();
            if (HasCreationReadRights && !string.IsNullOrEmpty(_dateMedia))
                _pathReadingFile = string.Format("{0}/{1}/{2}/3{3}.{4}", CreationServerPathes.DOWNLOAD_TV_SERVER,
                    _dateMedia.Substring(0, 4), "240", _idSlogan, MP4_EXTENSION);
            if (!HasCreationDownloadRights)
                return;
            if (!string.IsNullOrEmpty(_dateMedia))
                _pathDownloadingFile = string.Format("{0}/{1}/{2}/3{3}.{4}", CreationServerPathes.DOWNLOAD_TV_SERVER,
                    _dateMedia.Substring(0, 4), "240", _idSlogan, MP4_EXTENSION);
        }

        protected override void GetDoohCreativePathes()
        {
            SetDateMedia();
            if (HasCreationReadRights && !string.IsNullOrEmpty(_dateMedia))
                _pathReadingFile = string.Format("{0}/{1}/{2}.{3}", CreationServerPathes.DOWNLOAD_TV_SERVER,
                    _dateMedia.Substring(0, 4), _idSlogan, MP4_EXTENSION);
            if (!HasCreationDownloadRights)
                return;
            if (!string.IsNullOrEmpty(_dateMedia))
                _pathDownloadingFile = string.Format("{0}/{1}/{2}.{3}", CreationServerPathes.DOWNLOAD_TV_SERVER,
                    _dateMedia.Substring(0, 4), _idSlogan, MP4_EXTENSION);
        }

        protected override void GetRadioCreativePathes()
        {
            SetDateMedia();
            if (HasCreationReadRights && !string.IsNullOrEmpty(_dateMedia))
                _pathReadingFile = string.Format("{0}/{1}/{2}/2{3}.{2}", CreationServerPathes.DOWNLOAD_RADIO_SERVER, _dateMedia.Substring(0, 4), "MP3", _idSlogan);
            if (!HasCreationDownloadRights)
                return;
            if (!string.IsNullOrEmpty(_dateMedia))
                _pathDownloadingFile = string.Format("{0}/{1}/{2}/2{3}.{2}", CreationServerPathes.DOWNLOAD_RADIO_SERVER, _dateMedia.Substring(0, 4), "MP3", _idSlogan);
        }

        protected override void GetOthersCreativePathes()
        {
            var func = (Func<string, string>)(s => string.Format("{0}/{1}/{2}/{3}.{4}", s, _file.Substring(0, 4), _file.Substring(4, 2), _file, MP4_EXTENSION));
            if (HasCreationReadRights)
                _pathReadingFile = func(CreationServerPathes.DOWNLOAD_PAN_EURO);
            if (!HasCreationDownloadRights)
                return;
            _pathDownloadingFile = func(CreationServerPathes.DOWNLOAD_PAN_EURO);
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

        protected virtual void SetDateMedia()
        {
            if (!string.IsNullOrEmpty(_dateMedia))
                return;
            SetDAL();

            string idSlogan = _idSlogan;

            if (_vehicle == Vehicles.names.dooh)
                idSlogan = _idSlogan.Substring(1);

            _dateMedia = _vehicleInformation == null ? _dal.GetVersionMinParutionDate(idSlogan, VehiclesInformation.Get(_vehicle)) : _dal.GetVersionMinParutionDate(idSlogan, _vehicleInformation);
        }
    }
}
