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
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Common.Results.Creatives {


    ///<summary>
    /// CreativeItem provides information for the display of a outdoor creative
    /// </summary>
    ///  <author>G Ragneau</author>
    ///  <since>09/08/2007</since>
    ///  <stereotype>entity</stereotype>
    public class CreativeOutdoor : CreativeItem {

        #region Constructors
        ///<summary>Constructor</summary>
        ///<author>Guillaume Ragneau</author>
        ///<since>jeudi 09 aout 2007</since>
        public CreativeOutdoor(long id) : base(id) {
            this._sorting.Add(new VersionComparer(1987));
            this._sorting.Add(new BudgetComparer(1712));
            this._sorting.Add(new InsertNbComparer(1604));
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

            CreativeOutdoor item = new CreativeOutdoor(id);
            item.Session = session;
			item.ShowProduct = session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
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
            CreativeOutdoor itemPress = (CreativeOutdoor)item;
            itemPress.Budget = Convert.ToDecimal(row[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()]);
            itemPress.MediaNb = Convert.ToInt32(row["nbsupport"]);
            itemPress.InsertNb = Convert.ToInt32(row[UnitsInformation.List[WebCst.CustomerSessions.Unit.numberBoard].Id.ToString()]);

        }
        #endregion

        #region render
        /// <summary>
        /// render creative
        /// </summary>
        /// <param name="output">Output</param>
        public override void Render(System.Text.StringBuilder output) {

            output.AppendLine("<table width=\"100%\" cellpadding=\"2\" cellspacing=\"1\" class=\"violetBackGroundV3\">");

            output.AppendFormat("<tr><td class=\"creativeVisualCellLeft\">");
            if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG)) {
                //visuels disponible
                string[] files = this._path.Split(',');

                char[] c = this.Id.ToString().ToCharArray();
                string pathWeb = string.Format("{0}/{1}/{2}/{3}/imagette"
                    , WebCst.CreationServerPathes.IMAGES_OUTDOOR
                    , c[c.Length - 1]
                    , c[c.Length - 2]
                    , c[c.Length - 3]);

                string images = string.Empty;
                string html = string.Empty;
                for (int j = 0; j < files.Length; j++) {
                    images += string.Format("{0}/{1},", pathWeb.Replace("/imagette", ""), files[j]);
                    html += "<a class=\"imageMD\" href=\"javascript:openPressCreation('{0}');\">" + string.Format("<img src=\"{0}/{1}\" border=\"0\"></a>", pathWeb, files[j], GestionWeb.GetWebWord(843, _session.SiteLanguage));
                }
                if (images.Length > 0) {
                    output.AppendFormat(html, images.Substring(0, images.Length - 1));
                }
                else {
                    output.AppendFormat("<p class=\"txtViolet12Bold\" valign=\"top\" width=\"300\">{0}</p>", GestionWeb.GetWebWord(843, _session.SiteLanguage));
                }
            }
            else {
                output.AppendFormat("<p class=\"txtViolet12Bold\" valign=\"top\" width=\"300\">{0}</p>", GestionWeb.GetWebWord(2250, _session.SiteLanguage));
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
                , GestionWeb.GetWebWord(1604, _session.SiteLanguage)
                , this._insertNb.ToString("### ### ### ### ##0"));
			if (_showProduct) {
				output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
					, GestionWeb.GetWebWord(858, _session.SiteLanguage)
					, this._product
					, GestionWeb.GetWebWord(2252, _session.SiteLanguage)
					, this._mediaNb.ToString("### ### ### ### ##0"));
			}
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td></tr>"
                , GestionWeb.GetWebWord(1987, _session.SiteLanguage)
                , this._id);

            output.AppendLine("</table></td></tr>");
            output.AppendLine("</table>");

        }
        #endregion
    }
}
