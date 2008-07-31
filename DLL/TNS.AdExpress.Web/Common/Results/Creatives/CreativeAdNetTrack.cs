#region Info
/*
 * Author           : G RAGNEAU 
 * Date             : 21/08/2007
 * Modifications    :
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using System.Text;
using System.Data;

using TNS.AdExpress.Web.Common.Results.Creatives.Comparers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.WebResultUI.TableControl;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using FmkCst = TNS.AdExpress.Constantes.FrameWork;


namespace TNS.AdExpress.Web.Common.Results.Creatives {


    ///<summary>
    /// CreativeItem provides information for the display of a adnettrack creative
    /// </summary>
    ///  <author>G Ragneau</author>
    ///  <since>09/08/2007</since>
    ///  <stereotype>entity</stereotype>
    public class CreativeAdNetTrack : CreativeItem {

        #region Constantes
        /// <summary>
        /// Chemin du dossier des bannières
        /// </summary>
        public const string VIRTUAL_DIRECTORY = @"\adnettrackCreatives";
        /// <summary>
        /// Save link webtextId
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_ID = 874;
        /// <summary>
        /// Save link Help webtext id
        /// </summary>
        private const Int64 SAVE_LINK_LABEL_HELP_ID = 920;
        /// <summary>
        /// Jpeg id
        /// </summary>
        private const string JPEG_ID = "JPEG";
        /// <summary>
        /// Gif id
        /// </summary>
        private const string GIF_ID = "GIF";
        /// <summary>
        /// Flash id
        /// </summary>
        private const string FLASH_ID = "SWF";        
        /// <summary>
        /// Jpeg text
        /// </summary>
        private const string JPEG_TEXT = "JPEG";
        /// <summary>
        /// Gif text
        /// </summary>
        private const string GIF_TEXT = "GIF";
        /// <summary>
        /// Flash text
        /// </summary>
        private const string FLASH_TEXT = "FLASH";
        /// <summary>
        /// Chemin de la page des plans médias AdNetTrack
        /// </summary>
        private const string MEDIA_SCHEDULE_PATH = "/Private/Results/AdNetTrackMediaSchedule.aspx";
        #endregion

        #region Variables
        /// <summary>
        /// Banner Url
        /// </summary>
        protected string _url = string.Empty;
        /// <summary>
        /// Baner format
        /// </summary>
        protected string _format = string.Empty;
        /// <summary>
        /// Baner Dimension
        /// </summary>
        protected string _dimension = string.Empty;
        /// <summary>
        /// AdNetTrack Media plan paramteter : Product Id
        /// </summary>
        protected int _productId = -1;
        /// <summary>
        /// AdNetTrack Media plan paramteter : Zoom Date
        /// </summary>
        protected string _zoomDate = string.Empty;
        /// <summary>
        /// AdNetTrack Media plan paramteter : Advertiser Id
        /// </summary>
        protected int _advertiserId = -1;
        /// <summary>
        /// AdNetTrack Media plan paramteter : Url Parameters
        /// </summary>
        protected string _urlParameters = string.Empty;
        /// <summary>
        /// Creatives parameter : Univers Id
        /// </summary>
        protected int _universId = -1;
        /// <summary>
        /// Creatives parameter : Module Id
        /// </summary>
        protected Int64 _moduleId = -1;
        /// <summary>
        /// AdNetTrack Media plan parameter : paramètres quelconques
        /// </summary>
        protected string _parameters = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Banner Url
        /// </summary>
        public string Url {
            get {
                return _url;
            }
            set {
                _url = value;
            }
        }
        /// <summary>
        /// Get / Set Banner Format
        /// </summary>
        public string Format {
            get {
                return _format;
            }
            set {
                _format = value;
            }
        }
        /// <summary>
        /// Get / Set Banner Dimension
        /// </summary>
        public string Dimension {
            get {
                return _dimension;
            }
            set {
                _dimension = value;
            }
        }
        /// <summary>
        /// AdNetTrack Media plan paramteter : Product Id
        /// </summary>
        public int ProductId {
            get {
                return _productId;
            }
            set {
                _productId = value;
            }
        }
        /// <summary>
        /// AdNetTrack Media plan paramteter : Zoom Date
        /// </summary>    
        public string ZoomDate {
            get {
                return _zoomDate;
            }
            set {
                if (value != null)
                    _zoomDate = value;
                else
                    _zoomDate = string.Empty;
            }
        }
        /// <summary>
        /// AdNetTrack Media plan paramteter : Advertiser Id
        /// </summary>
        public int AdvertiserId {
            get {
                return _advertiserId;
            }
            set {
                _advertiserId = value;
            }
        }
        /// <summary>
        /// AdNetTrack Media plan paramteter : Url Parameters
        /// </summary>
        public string UrlParameters {
            get {
                return _urlParameters;
            }
            set {
                if (value != null)
                    _urlParameters = value;
                else
                    _urlParameters = string.Empty;
            }
        }
        /// <summary>
        /// Creatives parameter : Univers Id
        /// </summary>
        public int UniversId {
            get {
                return _universId;
            }
            set {
                _universId = value;
            }
        }
        /// <summary>
        /// Creatives parameter : module Id
        /// </summary>
        public Int64 ModuleId {
            get {
                return _moduleId;
            }
            set {
                _moduleId = value;
            }
        }
        /// <summary>
        /// AdNetTrack Media plan paramteter : Url Parameters Unknown
        /// </summary>
        public string Parameters {
            get {
                return _parameters;
            }
            set {
                if (value != null)
                    _parameters = value;
                else
                    _parameters = string.Empty;
            }
        }
        #endregion

        #region Constructors
        ///<summary>Constructor</summary>
        ///<author>Guillaume Ragneau</author>
        ///<since>jeudi 09 aout 2007</since>
        public CreativeAdNetTrack(long id)
            : base(id) {
        }
        #endregion

        #region GetInstance
        /// <summary>
        /// Build instance of CreativeRadio using data containded in row
        /// </summary>
        /// <param name="row">Data container</param>
        /// <param name="session">Web Session</param>
        /// <returns>New CreativeItem Instance</returns>
        public override CreativeItem GetInstance(DataRow row, WebSession session) {

            long id = Convert.ToInt64(row["version"]);
            CreativeAdNetTrack item = new CreativeAdNetTrack(id);
            item.Session = session;

            FieldInstance(row, item);

            return item;

        }
        /// <summary>
        /// Fill instance with data
        /// </summary>
        /// <param name="row">Data Container</param>
        /// <param name="item">item to fill</param>
        protected override void FieldInstance(DataRow row, CreativeItem item) {
            base.FieldInstance(row, item);
            CreativeAdNetTrack itemAdNet = (CreativeAdNetTrack)item;
            itemAdNet.Url = row["url"].ToString();
            itemAdNet.Format = row["format"].ToString();
            itemAdNet.Dimension = row["dimension"].ToString();
            itemAdNet._advertiserId = Convert.ToInt32(row["idAdvertiser"]);
            itemAdNet._urlParameters = this._urlParameters;
            itemAdNet._productId = Convert.ToInt32(row["idProduct"]);
            itemAdNet._zoomDate = this._zoomDate;
            itemAdNet._universId = this._universId;
            itemAdNet._moduleId = this._moduleId;
            itemAdNet._parameters = this._parameters;
        }
        #endregion

        #region render
        /// <summary>
        /// render creative
        /// </summary>
        /// <param name="output">Output</param>
        public override void Render(System.Text.StringBuilder output) {

            output.AppendLine("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"1\"  bgcolor=\"#B1A3C1\">");

            RenderBanner(output);
            RenderInfo(output);


            output.AppendLine("</table>");
        
        }
        /// <summary>
        /// Render Banner Creative Zone
        /// </summary>
        /// <param name="output"></param>
        private void RenderBanner(StringBuilder output) {


            #region Formatage des longueur et largeur du fichier
            string[] dimensionValue = null;
            string width = "";
            string height = "";
            if (_dimension.Length != 0) {
                dimensionValue = _dimension.Split('*');
                width = dimensionValue[0];
                height = dimensionValue[1];
            }
            else throw (new Exception("Le fichier ne possède pas de dimension"));
            #endregion

            #region Chemin complet du fichier
            string pathFile = VIRTUAL_DIRECTORY + @"\" + _path;
            #endregion



            output.AppendFormat("<tr><td class=\"creativeVisualCell\">");
            if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_DETAIL_INTERNET_ACCESS_FLAG)) {

                output.AppendFormat("\n<table>");
                output.AppendFormat("\n<tr>");
                output.AppendFormat("\n<td align=\"center\">");

                if (_format == FLASH_ID) {
                    // Bannière de type Flash
                    output.AppendFormat("\n <OBJECT classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=5,0,0,0\" width=\"{0}\" height=\"{1}\">",
                        width,
                        height);
                    output.AppendFormat("\n <PARAM name=\"movie\" value=\"{0}\">", pathFile);
                    output.Append("\n <PARAM name=\"play\" value=\"true\">");
                    output.Append("\n <PARAM name=\"quality\" value=\"high\">");
                    output.AppendFormat("\n <EMBED src=\"{0}\" play=\"true\" swliveconnect=\"true\" quality=\"high\" width=\"{1}\" height=\"{2}\">",
                        pathFile,
                        width,
                        height);
                    output.Append("\n </OBJECT>");
                }
                else {
                    // Bannière de type autre
                    output.Append("\n <p>");
                    output.AppendFormat("<a href=\"{0}\" target=\"_blank\"><img border=0 src=\"{1}\" border=\"0\"></a>",
                        _url,
                        pathFile);
                    output.Append("</p>");
                }

                output.AppendFormat("\n</td></tr>");
                output.AppendFormat("\n<tr><td align=\"center\">");

                if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_DOWNLOAD_ACCESS_FLAG)) {
                    output.AppendFormat("\n<a href={0} class=\"roll06\" title=\"{1}\">{2}</a>",
                        pathFile,
                        GestionWeb.GetWebWord(SAVE_LINK_LABEL_HELP_ID, _session .SiteLanguage),
                        GestionWeb.GetWebWord(SAVE_LINK_LABEL_ID, _session.SiteLanguage));
                }

                output.AppendFormat("\n</td></tr>");
                output.AppendFormat("\n</table>");

            }
            else {
                output.AppendFormat("<p class=\"txtViolet12Bold\" valign=\"top\" width=\"240\">{0}</p>", GestionWeb.GetWebWord(2250, _session.SiteLanguage));
            }


            output.AppendFormat("</td></tr>");

        }
        /// <summary>
        /// Render banner information
        /// </summary>
        /// <param name="output"></param>
        private void RenderInfo(StringBuilder output) {
            //Description

            #region GAD
            string openBaliseA = string.Empty;
            string closeBaliseA = string.Empty;

            if (_adressId != -1) {
                openBaliseA = "<a class=\"txtViolet11Underline\" href=\"javascript:openGad('" + Session.IdSession + "','" + _advertiser + "','" + _adressId + "');\">";
                closeBaliseA = "</a>";
            }
            #endregion

            //Nomenclature
            output.AppendLine("<tr class=\"creativeDescCell \"><td><table class=\"txtViolet11\">");
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td><td>" + openBaliseA + "{1}" + closeBaliseA + "</td></tr>"
                , GestionWeb.GetWebWord(1106, _session.SiteLanguage)
                , this._advertiser);
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td><td>{1}</td></tr>"
                , GestionWeb.GetWebWord(1110, _session.SiteLanguage)
                , this._group);
            if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.MEDIA_SCHEDULE_ADNETTRACK_ACCESS_FLAG)) {
                output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td><td>{1}</td></tr>"
                    , GestionWeb.GetWebWord(2154, _session.SiteLanguage)
                    , _product);
            }

            //Format
            string format = string.Empty;
            if(this._format.Equals(GIF_ID))
                format = GIF_TEXT;
            else if(this._format.Equals(JPEG_ID))
                format = JPEG_TEXT;
            else if (this._format.Equals(FLASH_ID))
                format = FLASH_TEXT;

            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td><td>{1}</td></tr>",
                GestionWeb.GetWebWord(2155, _session.SiteLanguage),
                format);

            //Liens plans médias
            if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.MEDIA_SCHEDULE_ADNETTRACK_ACCESS_FLAG)) {
                output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td>",
                    GestionWeb.GetWebWord(2156, _session.SiteLanguage));

                output.AppendFormat("<td><a href=\"#\" onclick=\"javascript:window.location='{0}?idSession={1}&idLevel={2}&id={3}&zoomDate={4}&urlParameters={5}&universId={13}&moduleId={14}&' + {15};\" class=\"roll06\">{6}</a> | <a href=\"#\" onclick=\"javascript:window.location='{0}?idSession={1}&idLevel={7}&id={8}&zoomDate={4}&urlParameters={5}&universId={13}&moduleId={14}&' + {15};\" class=\"roll06\">{9}</a> | <a href=\"#\" onclick=\"javascript:window.location='{0}?idSession={1}&idLevel={10}&id={11}&zoomDate={4}&urlParameters={5}&universId={13}&moduleId={14}&' + {15};\" class=\"roll06\">{12}</a></td>",
                    MEDIA_SCHEDULE_PATH,
                    _session.IdSession,
                    FmkCst.Results.AdNetTrackMediaSchedule.Type.advertiser.GetHashCode(),
                    _advertiserId,
                    _zoomDate,
                    _urlParameters,
                    GestionWeb.GetWebWord(857, _session.SiteLanguage),
                    FmkCst.Results.AdNetTrackMediaSchedule.Type.product.GetHashCode(),
                    _productId,
                    GestionWeb.GetWebWord(858, _session.SiteLanguage),
                    FmkCst.Results.AdNetTrackMediaSchedule.Type.visual.GetHashCode(),
                    _id,
                    GestionWeb.GetWebWord(1909, _session.SiteLanguage),
                    _universId,
                    _moduleId,
                    _parameters
                );
                output.Append("</tr>");
            }


            output.AppendLine("</table></td></tr>");
        }
        #endregion
    }
}

#region Comparers
namespace TNS.AdExpress.Web.Common.Results.Creatives.Comparers {
    
    #region Format Comparer
    ///<summary>
    /// Creative AdNetTrack Format Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 10 septembre 2007 11:24:28</since>
    public class AdNetTrackFormatComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">AdNetTrack Format Comparer Caption</param>
        public AdNetTrackFormatComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">AdNetTrack Format Comparer Caption ID</param>
        public AdNetTrackFormatComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem AdNetTrack Formats
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return ((CreativeAdNetTrack)x).Format.CompareTo(((CreativeAdNetTrack)y).Format);
        }
        #endregion

    }
    #endregion

}
#endregion