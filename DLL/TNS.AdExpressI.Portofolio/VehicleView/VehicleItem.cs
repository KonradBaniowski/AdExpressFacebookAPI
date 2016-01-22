using System;
using System.Text;
using TNS.AdExpress.Domain.Web;
using System.Globalization;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpressI.Portofolio.VehicleView
{/// <summary>
    /// Contains information about one vehicle and a link to the vehicle view pop-up
    /// </summary>
    public class VehicleItem
    {
        #region Variables
        /// <summary>
        /// Vehicle parution date
        /// </summary>
        protected DateTime _parutionDate;
        /// <summary>
        /// Insertion number
        /// </summary>
        protected string _insertionNumber = string.Empty;
        /// <summary>
        /// Total investment
        /// </summary>
        protected string _totalInvestment = string.Empty;
        /// <summary>
        /// Vehicle cover item
        /// </summary>
        protected CoverItem _coverItem = null;
        /// <summary>
        /// Site language
        /// </summary>
        protected int _siteLanguage = 33;
        /// <summary>
        /// Indicates if can show cover image
        /// </summary>
        protected bool _showCover = true;

       
        #endregion

        #region Accesssors
        /// <summary>
        /// Get / Set vehicle parution date
        /// </summary>
        public DateTime ParutionDate
        {
            get { return _parutionDate; }
            set { _parutionDate = value; }
        }
        /// <summary>
        /// Get / Set Insertion number
        /// </summary>
        public string InsertionNumber
        {
            get { return _insertionNumber; }
            set { _insertionNumber = value; }
        }
        /// <summary>
        /// Get / Set  Total investment
        /// </summary>
        public string TotalInvestment
        {
            get { return _totalInvestment; }
            set { _totalInvestment = value; }
        }
        /// <summary>
        /// Get / Set  Vehicle cover item
        /// </summary>
        public CoverItem CoverItem
        {
            get { return _coverItem; }
            set { _coverItem = value; }
        }
        /// <summary>
        /// Get / Set  Site language
        /// </summary>
        public int SiteLanguage
        {
            get { return _siteLanguage; }
            set { _siteLanguage = value; }
        }
        /// <summary>
        /// Get / Set if can show cover image
        /// </summary>
        public bool ShowCover
        {
            get { return _showCover; }
            set { _showCover = value; }
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
        public VehicleItem(DateTime parutionDate, string insertionNumber, string totalInvestment, int siteLanguage, CoverItem coverItem)
        {
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
        public virtual string Render()
        {

            var sb = new StringBuilder(5000);
            var cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_siteLanguage].Localization);
            string day = string.Empty;

            day = string.Format("{0} {1}", DayString.GetCharacters(_parutionDate, cultureInfo), 
                DateString.dateTimeToDD_MM_YYYY(_parutionDate, _siteLanguage));

            sb.Append("<table cellpadding=0 cellspacing=0 width=100% border=0 >");
            if (  _coverItem != null && _coverItem.CoverLinkItem != null && !string.IsNullOrEmpty(_coverItem.CoverLinkItem.Render()))
                sb.AppendFormat("<tr><td class=\"portofolioSynthesis\" align=center ><a class=\"portofolioSynthesis\" href={1}>{0}</a></td><tr>"
                    , day, _coverItem.CoverLinkItem.Render());
            else sb.AppendFormat("<tr><td class=\"portofolioSynthesis\" align=center >{0}</td><tr>", day);
            sb.Append("<tr><td align=\"center\" class=\"portofolioSynthesis\" >");

            if (_showCover)
            {               
                    sb.Append(_coverItem.Render());               
            }

            sb.Append("</td></tr>");
            if (  _coverItem != null && _coverItem.CoverLinkItem != null && !string.IsNullOrEmpty(_coverItem.CoverLinkItem.Render()))
            {
                sb.AppendFormat("<tr><td class=\"portofolioSynthesis\" align=\"center\"><a class=\"portofolioSynthesis\" href={0}>{1} : {2}</a></td><tr>"
                    , _coverItem.CoverLinkItem.Render(), GestionWeb.GetWebWord(1398, _siteLanguage), _insertionNumber);
                sb.AppendFormat("<tr><td class=\"portofolioSynthesis\" align=\"center\"><a class=\"portofolioSynthesis\" href={0}>{1} : {2}</a></td><tr>"
                    , _coverItem.CoverLinkItem.Render(), GestionWeb.GetWebWord(1399, _siteLanguage), _totalInvestment);

            }
            else
            {
                sb.AppendFormat("<tr><td class=\"portofolioSynthesis\" align=\"center\">{0} : {1}</td><tr>"
                    , GestionWeb.GetWebWord(1398, _siteLanguage), _insertionNumber);
                sb.AppendFormat("<tr><td class=\"portofolioSynthesis\" align=\"center\">{0} : {1}</td><tr>"
                    , GestionWeb.GetWebWord(1399, _siteLanguage), _totalInvestment);
            }

            sb.Append("</table>");

            return sb.ToString();

        }

        #endregion
    }
}
