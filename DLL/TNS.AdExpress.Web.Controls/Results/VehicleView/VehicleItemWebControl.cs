using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Date;
using System.Globalization;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS.AdExpress.Web.Controls.Results.VehicleView{
    /// <summary>
    /// Contains information about one vehicle and a link to the vehicle view pop-up
    /// </summary>
    public class VehicleItemWebControl : WebControl{

        #region Variables
        /// <summary>
        /// Vehicle parution date
        /// </summary>
        private DateTime _parutionDate;
        /// <summary>
        /// Insertion number
        /// </summary>
        private string _insertionNumber = string.Empty;
        /// <summary>
        /// Total investment
        /// </summary>
        private string _totalInvestment = string.Empty;
        /// <summary>
        /// Vehicle cover item
        /// </summary>
        private CoverItemWebControl _coverItem = null;
        /// <summary>
        /// Site language
        /// </summary>
        private int _siteLanguage = 33;
        #endregion

        #region Accesssors
        /// <summary>
        /// Get / Set vehicle parution date
        /// </summary>
        public DateTime ParutionDate {
            get { return _parutionDate; }
            set { _parutionDate = value; }
        }
        /// <summary>
        /// Get / Set Insertion number
        /// </summary>
        public string InsertionNumber {
            get { return _insertionNumber; }
            set { _insertionNumber = value; }
        }
        /// <summary>
        /// Get / Set  Total investment
        /// </summary>
        public string TotalInvestment {
            get { return _totalInvestment; }
            set { _totalInvestment = value; }
        }
        /// <summary>
        /// Get / Set  Vehicle cover item
        /// </summary>
        public CoverItemWebControl CoverItem {
            get { return _coverItem; }
            set { _coverItem = value; }
        }
        /// <summary>
        /// Get / Set  Site language
        /// </summary>
        public int SiteLanguage {
            get { return _siteLanguage; }
            set { _siteLanguage = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parutionDate">Vehicle parution date</param>
        /// <param name="insertionNumber">Insertion number</param>
        /// <param name="totalInvestment">Total investment</param>
        /// <param name="siteLanguage">Site language</param>
        /// <param name="coverItem">Vehicle cover item</param>
        public VehicleItemWebControl(DateTime parutionDate, string insertionNumber, string totalInvestment, int siteLanguage, CoverItemWebControl coverItem){
            _parutionDate = parutionDate;
            _insertionNumber = insertionNumber;
            _totalInvestment = totalInvestment;
            _siteLanguage = siteLanguage;
            _coverItem = coverItem;
        }
        #endregion

        #region Render
        /// <summary>
        /// Item Render
        /// </summary>
        /// <returns>Html code</returns>
        public string Render() {
            
            StringBuilder sb = new StringBuilder(5000);
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_siteLanguage].Localization);
            string day = string.Empty;

            day = DayString.GetCharacters(_parutionDate, cultureInfo) + " " + DateString.dateTimeToDD_MM_YYYY(_parutionDate, _siteLanguage); 

            sb.Append("<table cellpadding=0 cellspacing=0 width=100% border=0 >");
            sb.Append("<tr><td class=\"portofolioSynthesis\" align=center >" + day + "</td><tr>");
            sb.Append("<tr><td align=\"center\" class=\"portofolioSynthesis\" >");

            sb.Append(_coverItem.Render());

            sb.Append("</td></tr>");

            sb.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, _siteLanguage) + " : " + _insertionNumber + "</td><tr>");
            sb.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1399, _siteLanguage) + " :" + _totalInvestment + "</td><tr>");

            sb.Append("</table>");

            return sb.ToString();

        }
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="output">output</param>
        protected override void Render(HtmlTextWriter output) {
            output.Write(Render());
        }
        #endregion

    }
}
