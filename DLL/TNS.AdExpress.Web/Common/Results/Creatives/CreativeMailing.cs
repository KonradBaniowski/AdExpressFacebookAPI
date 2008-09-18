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
using System.Text;

using TNS.AdExpress.Web.Common.Results.Creatives.Comparers;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.WebResultUI.TableControl;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Common.Results.Creatives {


    ///<summary>
    /// CreativeItem provides information for the display of a radio creative
    /// </summary>
    ///  <author>G Ragneau</author>
    ///  <since>09/08/2007</since>
    ///  <stereotype>entity</stereotype>
    public class CreativeMailing : CreativeItem {

        #region Variables
        /// <summary>
        /// Creative media
        /// </summary>
        protected string _media = string.Empty;
        /// <summary>
        /// Creative Weight
        /// </summary>
        protected decimal _weight = 0;
        /// <summary>
        /// Creative  Object Number
        /// </summary>
        protected int _nbobjet = 0;
        /// <summary>
        /// Creative Format
        /// </summary>
        protected string _format = string.Empty;
        /// <summary>
        /// Creative Mail Format
        /// </summary>
        protected string _mail_format = string.Empty;
        /// <summary>
        /// Creatuive Mail type
        /// </summary>
        protected string _mail_type = string.Empty;
        /// <summary>
        /// Creative Standard
        /// </summary>
        protected string _standard = string.Empty;
        /// <summary>
        /// Creative Mailing rapidity
        /// </summary>
        protected string _rapidity = string.Empty;
        /// <summary>
        /// Creative Content
        /// </summary>
        protected string _mail_content = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set Creative media
        /// </summary>
        public string Media{
            get {
                return _media;
            }
            set {
                _media = value;
            }
        }
        /// <summary>
        /// Get / Set Creative Weight
        /// </summary>
        public decimal Weight {
            get {
                return _weight;
            }
            set {
                _weight = value;
            }
        }
        /// <summary>
        /// Get / Set Creative  Object Number
        /// </summary>
        public int Nbobjet {
            get {
                return _nbobjet;
            }
            set {
                _nbobjet = value;
            }
        }
        /// <summary>
        /// Get / Set Creative Format
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
        /// Get / Set Creative Mail Format
        /// </summary>
        public string Mail_format {
            get {
                return _mail_format;
            }
            set {
                _mail_format = value;
            }
        }
        /// <summary>
        /// Get / Set Creatuive Mail type
        /// </summary>
        public string Mail_type {
            get {
                return _mail_type;
            }
            set {
                _mail_type = value;
            }
        }
        /// <summary>
        /// Get / Set Creative Standard
        /// </summary>
        public string Standard {
            get {
                return _standard;
            }
            set {
                _standard = value;
            }
        }
        /// <summary>
        /// Get / Set Creative Mailing rapidity
        /// </summary>
        public string Rapidity {
            get {
                return _rapidity;
            }
            set {
                _rapidity = value;
            }
        }
        /// <summary>
        /// Get / Set Creative Content
        /// </summary>
        public string Mail_content {
            get {
                return _mail_content;
            }
            set {
                _mail_content = value;
            }
        }
        #endregion

        #region Constructors
        ///<summary>Constructor</summary>
        ///<author>Guillaume Ragneau</author>
        ///<since>jeudi 09 aout 2007</since>
        public CreativeMailing(long id) : base(id) {
            this._sorting.Add(new SupportComparer(971));
            this._sorting.Add(new VersionComparer(1987));
            this._sorting.Add(new BudgetComparer(1712));
            this._sorting.Add(new InsertNbComparer(2247));
            this._sorting.Add(new ObjectNumberComparer(2249));
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
            CreativeMailing item = new CreativeMailing(id);
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
            CreativeMailing itemMailing = (CreativeMailing)item;
            itemMailing.Budget = Convert.ToInt32(row[UnitsInformation.List[WebCst.CustomerSessions.Unit.euro].Id.ToString()]);
            itemMailing.InsertNb = Convert.ToInt32(row["volume"]);
            itemMailing.Media = row["media"].ToString();
            itemMailing.Weight = Convert.ToDecimal(row["weight"]);
            itemMailing.Nbobjet = (row["nbobjet"] != DBNull.Value)?Convert.ToInt32(row["nbobjet"]):-1;
            itemMailing.Format = (row["format"] != DBNull.Value) ? row["format"].ToString() : string.Empty;
            itemMailing.Mail_format = (row["mail_format"] != DBNull.Value) ? row["mail_format"].ToString() : string.Empty;
            itemMailing.Mail_type = (row["mail_type"] != DBNull.Value) ? row["mail_type"].ToString() : string.Empty;
            int i = (row["standard"] != DBNull.Value) ? Convert.ToInt32(row["standard"]):-1;
            switch (i) {
                case DBCst.Format.ID_FORMAT_ORIGINAL:
                    itemMailing.Standard = GestionWeb.GetWebWord(2241, itemMailing.Session.SiteLanguage);
                    break;
                case DBCst.Format.ID_FORMAT_STANDARD:
                    itemMailing.Standard = GestionWeb.GetWebWord(2240, itemMailing.Session.SiteLanguage);
                    break;
                default:
                    itemMailing.Standard = string.Empty;
                    break;

            }
             
            itemMailing.Rapidity = (row["rapidity"] != DBNull.Value) ? row["rapidity"].ToString() : string.Empty;
            itemMailing.Mail_content = (row["mail_content"] != DBNull.Value) ? row["mail_content"].ToString().TrimEnd(',') : string.Empty;

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
            if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG)) {
                //visuels disponible
                string[] files = this._path.Split(',');

                char[] c = this.Id.ToString().ToCharArray();
                string pathWeb = string.Format("{0}/{1}/{2}/{3}/imagette"
                    , WebCst.CreationServerPathes.IMAGES_MD
                    , c[c.Length - 1]
                    , c[c.Length - 2]
                    , c[c.Length - 3]);

                string images = string.Empty;
                string html = string.Empty;
                for (int j = 0; j < files.Length; j++)
                {
                    if (files[j].Length > 0)
                    {

                        images += string.Format("{0}/{1},", pathWeb.Replace("/imagette", ""), files[j]);
                        html += "<a class=\"imageMD\" href=\"javascript:openPressCreation('{0}');\">" + string.Format("<img alt=\"{2}\" src=\"{0}/{1}\" border=\"0\"></a>", pathWeb, files[j], GestionWeb.GetWebWord(843, _session.SiteLanguage));
                    }
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
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td class=\"tdsepar\">:</td><td>" + openBaliseA + "{1}" + closeBaliseA + "</td><td class=\"tdsepar2\"></td><td class=\"creativeCaption\">{2}</td><td class=\"tdsepar\">:</td><td>{3} €</td></tr>"
                , GestionWeb.GetWebWord(1106, _session.SiteLanguage)
                , this._advertiser
                , GestionWeb.GetWebWord(1712, _session.SiteLanguage)
                , this._Budget.ToString("### ### ### ### ##0"));

            if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_VOLUME_MARKETING_DIRECT)) {
                output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
                    , GestionWeb.GetWebWord(1110, _session.SiteLanguage)
                    , this._group
                    , GestionWeb.GetWebWord(2247, _session.SiteLanguage)
                    , this._insertNb.ToString("### ### ### ### ##0"));
            }
            else {
                output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
                    , GestionWeb.GetWebWord(1110, _session.SiteLanguage)
                    , this._group
                    , GestionWeb.GetWebWord(2247, _session.SiteLanguage)
                    , "-");
            }
			if (_showProduct) {
				if (_session.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_POIDS_MARKETING_DIRECT)) {
					output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3} g</td></tr>"
						, GestionWeb.GetWebWord(858, _session.SiteLanguage)
						, this._product
						, GestionWeb.GetWebWord(2248, _session.SiteLanguage)
						, this._weight);
				}
				else {
					output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3} g</td></tr>"
						, GestionWeb.GetWebWord(858, _session.SiteLanguage)
						, this._product
						, GestionWeb.GetWebWord(2248, _session.SiteLanguage)
						, "-");
				}
			}
            output.AppendFormat("<tr><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
                , GestionWeb.GetWebWord(971, _session.SiteLanguage)
                , this._media
                , GestionWeb.GetWebWord(2249, _session.SiteLanguage)
                , (this._nbobjet > 0)?this._nbobjet.ToString():"-");

            StringBuilder desc = new StringBuilder();
            if (this._format.Length > 0) desc.AppendFormat("{0}<br/>", this._format);
            if (this._mail_content.Length > 0) desc.AppendFormat("{0}<br/>", this._mail_content.Replace(",", "<br/>"));
            if (this._mail_format.Length > 0) desc.AppendFormat("{0}<br/>", this._mail_format);
            if (this._mail_type.Length > 0) desc.AppendFormat("{0}<br/>", this._mail_type);
            if (this._rapidity.Length > 0) desc.AppendFormat("{0}<br/>", this._rapidity);
            if (this._standard.Length > 0) desc.AppendFormat("{0}<br/>", this._standard);

            if (desc.Length > 0) desc.Length = desc.Length - 3;

            output.AppendFormat("<tr valign=\"top\"><td class=\"creativeCaption\">{0}</td><td>:</td><td>{1}</td><td></td><td class=\"creativeCaption\">{2}</td><td>:</td><td>{3}</td></tr>"
                , GestionWeb.GetWebWord(1987, _session.SiteLanguage)
                , this._id
                , GestionWeb.GetWebWord(2239, _session.SiteLanguage)
                , desc.ToString());
            output.AppendLine("</table></td></tr>");
            output.AppendLine("</table>");

        }
        #endregion    
    }
}

#region Comparers
namespace TNS.AdExpress.Web.Common.Results.Creatives.Comparers {

    #region Support Comparer
    ///<summary>
    /// Creative Support Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 20 septembre 2007 11:24:28</since>
    public class SupportComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Support Comparer Caption</param>
        public SupportComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Product Comparer Caption</param>
        public SupportComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem Supports
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return ((CreativeMailing)x).Media.CompareTo(((CreativeMailing)y).Media);
        }
        #endregion

    }
    #endregion

    #region Object Number Comparer
    ///<summary>
    /// Creative Object Number Comparer
    /// </summary>
    /// <author>Guillaume.Ragneau</author>
    /// <since>lundi 20 septembre 2007 11:24:28</since>
    public class ObjectNumberComparer : CreativeComparer {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="caption">Object Number Comparer Caption</param>
        public ObjectNumberComparer(string caption) : base(caption) { }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="captionId">Object Number Comparer Caption</param>
        public ObjectNumberComparer(int captionId) : base(captionId) { }
        #endregion

        #region Methods
        /// <summary>
        /// Compare two CreativeItem Object Numbers
        /// </summary>
        /// <param name="x">First creative item to compare</param>
        /// <param name="y">Second creative item to compare</param>
        /// <returns>-1 if x > y, 0 if equelas and 1 if y > x</returns>
        public override int Compare(CreativeItem x, CreativeItem y) {
            return ((CreativeMailing)x).Nbobjet.CompareTo(((CreativeMailing)y).Nbobjet);
        }
        #endregion

    }
    #endregion
}
#endregion