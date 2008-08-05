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
using System.Data;

using TNS.AdExpress.Web.Common.Results.Creatives.Comparers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.WebResultUI.TableControl;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBClassifCst = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpress.Web.Common.Results.Creatives {


    ///<summary>
    /// CreativeItem provides information for the display of a radio creative
    /// </summary>
    ///  <author>G Ragneau</author>
    ///  <since>09/08/2007</since>
    ///  <stereotype>entity</stereotype>
    public class CreativeRadio : CreativeItem {

        #region Variables
        /// <summary>
        /// Sport duration
        /// </summary>
        protected TimeSpan _duration;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Sport duration
        /// </summary>
        public TimeSpan Duration {
            get { return _duration; }
            set { _duration = value; }
        }
        #endregion

        #region Constructors
        ///<summary>Constructor</summary>
        ///<author>Guillaume Ragneau</author>
        ///<since>jeudi 09 aout 2007</since>
        public CreativeRadio(long id): base(id) {
            this._sorting.Add(new VersionComparer(1987));
            this._sorting.Add(new BudgetComparer(1712));
            this._sorting.Add(new InsertNbComparer(2253));
            this._sorting.Add(new TVDurationComparer(1933));
            this._sorting.Add(new MediaNbComparer(2252));
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
            CreativeRadio item = new CreativeRadio(id);
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
            CreativeRadio itemRadio = (CreativeRadio)item;
            itemRadio.Budget = Convert.ToDecimal(row["budget"]);
            itemRadio.MediaNb = Convert.ToInt32(row["nbsupport"]);
            itemRadio.Duration = new TimeSpan(0, 0, Convert.ToInt32(row["duree"]));
            itemRadio.InsertNb = Convert.ToInt32(row["nbinsertion"]);

        }
        #endregion

        #region render
        /// <summary>
        /// render creative
        /// </summary>
        /// <param name="output">Output</param>
        public override void Render(System.Text.StringBuilder output) {

            output.AppendLine("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"1\" class=\"violetBackGroundV3\">");

            output.AppendFormat("<tr><td class=\"creativeVisualCell\">");
            if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_RADIO_CREATION_ACCESS_FLAG)) {
                output.AppendFormat("<a href=\"javascript:openDownload('{0},{1}','{2}','{3}');\" class=\"audioFileBackGround\"></a>", this._path, this._id, this._session.IdSession, (int)DBClassifCst.Vehicles.names.radio);
            }
            else {
                output.AppendFormat("<p class=\"txtViolet12Bold\" valign=\"top\" width=\"240\">{0}</p>", GestionWeb.GetWebWord(2250, _session.SiteLanguage));
            }

            output.AppendFormat("</td></tr>");

            //Description

            #region GAD
            string openBaliseA = string.Empty;
            string closeBaliseA = string.Empty;

            if (_adressId != -1) {
                openBaliseA = "<a class=\"txtViolet11Underline\" href=\"javascript:openGad('" + Session.IdSession + "','" + _advertiser + "','" + _adressId + "');\">";
                closeBaliseA = "</a>";
            }
            #endregion

            output.AppendLine("<tr class=\"creativeDescCell \"><td><table class=\"txtViolet11\">");
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td><td>"+openBaliseA+"{1}"+closeBaliseA+"</td><td class=\"tdsepar2\"></td><td class=\"creativeCaption\">{2}</td><td class=\"tdsepar\">:</td><td>{3} €</td></tr>"
                , GestionWeb.GetWebWord(1106, _session.SiteLanguage)
                , this._advertiser
                , GestionWeb.GetWebWord(1712, _session.SiteLanguage)
                , this._Budget.ToString("### ### ### ### ##0"));
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
                , GestionWeb.GetWebWord(1110, _session.SiteLanguage)
                , this._group
                , GestionWeb.GetWebWord(2253, _session.SiteLanguage)
                , this._insertNb.ToString("### ### ### ### ##0"));
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
                , GestionWeb.GetWebWord(858, _session.SiteLanguage)
                , this._product
                , GestionWeb.GetWebWord(1933, _session.SiteLanguage)
                , string.Format("{0} h {1} m {2}",this._duration.Hours.ToString("00"),this._duration.Minutes.ToString("00"),this._duration.Seconds.ToString("00")));
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
                , GestionWeb.GetWebWord(1987, _session.SiteLanguage)
                , this._id
                , GestionWeb.GetWebWord(2252, _session.SiteLanguage)
                , this._mediaNb.ToString("### ### ### ### ##0"));

            output.AppendLine("</table></td></tr>");
            output.AppendLine("</table>");

        }
        #endregion

    }
}

#region Comparers
namespace TNS.AdExpress.Web.Common.Results.Creatives.Comparers {

    #region Duration Comparer
    ///<summary>
    /// Creative TV Duration Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 20 septembre 2007 11:24:28</since>
    public class RadioDurationComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Product Comparer Caption</param>
        public RadioDurationComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Product Comparer Caption</param>
        public RadioDurationComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem products
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return ((CreativeRadio)x).Duration.CompareTo(((CreativeRadio)y).Duration);
        }
        #endregion

    }
    #endregion

}
#endregion