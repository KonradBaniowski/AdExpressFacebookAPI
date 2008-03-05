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

namespace TNS.AdExpress.Web.Common.Results.Creatives {


    ///<summary>
    /// CreativeItem provides information for the display of a press creative
    /// </summary>
    ///  <author>G Ragneau</author>
    ///  <since>09/08/2007</since>
    ///  <stereotype>entity</stereotype>
    public class CreativePresse : CreativeItem {

        #region Variables
        /// <summary>
        /// Volume of the creative in page
        /// </summary>
        protected decimal _pageNumber = 0;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Volume of the creative in page
        /// </summary>
        public decimal PageNumber {
            get {
                return _pageNumber;
            }
            set {
                _pageNumber = value;
            }
        }
        #endregion

        #region Constructors
        ///<summary>Constructor</summary>
        ///<author>Guillaume Ragneau</author>
        ///<since>jeudi 09 aout 2007</since>
        public CreativePresse(long id): base(id) {
            this._sorting.Add(new VersionComparer(1987));
            this._sorting.Add(new BudgetComparer(1712));
            this._sorting.Add(new InsertNbComparer(144));
            this._sorting.Add(new PageNumberComparer(2251));
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
            CreativePresse item = new CreativePresse(id);
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
            CreativePresse itemPress = (CreativePresse)item;
            itemPress.Budget = Convert.ToDecimal(row["budget"]);
            itemPress.MediaNb = Convert.ToInt32(row["nbsupport"]);
            itemPress.PageNumber = Convert.ToDecimal(row["volume"]);
            itemPress.InsertNb = Convert.ToInt32(row["nbinsertion"]);

        }
        #endregion

        #region render
        /// <summary>
        /// render creative
        /// </summary>
        /// <param name="output">Output</param>
        public override void Render(System.Text.StringBuilder output) {

            output.AppendLine("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"1\"  bgcolor=\"#B1A3C1\">");

            output.AppendFormat("<tr><td class=\"creativeVisualCellLeft\">");
            if (_session.CustomerLogin.GetFlag(DBCst.Flags.ID_PRESS_CREATION_ACCESS_FLAG) != null) {
                //visuels disponible
                string[] files = this._path.Split(',');

                string images = string.Empty;
                string html = string.Empty;
                for (int j = 0; j < files.Length; j++) {
                    images += string.Format("{0}/{1},", WebCst.CreationServerPathes.IMAGES, files[j].Replace("/imagette",""));
                    html += "<a class=\"imageMD\" href=\"javascript:openPressCreation('{0}');\">" + string.Format("<img src=\"{0}/{1}\" border=\"0\"></a>", WebCst.CreationServerPathes.IMAGES, files[j], GestionWeb.GetWebWord(843, _session.SiteLanguage));
                }
                if (images.Length > 0) {
                    output.AppendFormat(html, images.Substring(0, images.Length - 1));
                }
                else {
                    output.AppendFormat("<p class=\"txtViolet12Bold\" valign=\"top\" width=\"240\">{0}</p>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
                }
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
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td><td>" + openBaliseA + "{1}" + closeBaliseA + "</td><td class=\"tdsepar2\"></td><td class=\"creativeCaption\">{2}</td><td class=\"tdsepar\">:</td><td>{3} €</td></tr>"
                , GestionWeb.GetWebWord(1106, _session.SiteLanguage)
                , this._advertiser
                , GestionWeb.GetWebWord(1712, _session.SiteLanguage)
                , this._Budget.ToString("### ### ### ### ##0"));
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td class=\"tdsepar\">:</td><td>{3}</td></tr>"
                , GestionWeb.GetWebWord(1110, _session.SiteLanguage)
                , this._group
                , GestionWeb.GetWebWord(144, _session.SiteLanguage)
                , this._insertNb.ToString("### ### ### ### ##0"));
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td class=\"tdsepar\">:</td><td>{3}</td></tr>"
                , GestionWeb.GetWebWord(858, _session.SiteLanguage)
                , this._product
                , GestionWeb.GetWebWord(2251, _session.SiteLanguage)
                , this._pageNumber.ToString("### ### ### ### ##0.###"));
            output.AppendFormat("<tr valign=\"top\"><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td class=\"tdsepar\">:</td><td>{3}</td></tr>"
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

    #region Page Number Comparer
    ///<summary>
    /// Creative Page Number Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 10 septembre 2007 11:24:28</since>
    public class PageNumberComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Page Number Comparer Caption</param>
        public PageNumberComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Page Number Comparer Caption ID</param>
        public PageNumberComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem Page Numbers
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return ((CreativePresse)x).PageNumber.CompareTo(((CreativePresse)y).PageNumber);
        }
        #endregion

    }
    #endregion

}
#endregion