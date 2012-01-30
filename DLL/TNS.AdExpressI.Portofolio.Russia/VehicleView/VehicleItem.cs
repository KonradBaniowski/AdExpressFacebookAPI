using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Web;
using System.Globalization;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Portofolio.VehicleView;

namespace TNS.AdExpressI.Portofolio.Russia.VehicleView
{/// <summary>
    /// Contains information about one vehicle and a link to the vehicle view pop-up
    /// </summary>
    public class VehicleItem : TNS.AdExpressI.Portofolio.VehicleView.VehicleItem
    {
          #region Variables
        /// <summary>
        /// Total investment usd
        /// </summary>
        protected string _totalInvestmentUsd = string.Empty;
        #endregion

        #region Accesssors
       
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
        public VehicleItem(DateTime parutionDate, string insertionNumber, string totalInvestment, int siteLanguage, CoverItem coverItem):
            base( parutionDate,  insertionNumber,  totalInvestment,  siteLanguage,  coverItem)
        {
          
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parutionDate">Vehicle parution date</param>
        /// <param name="insertionNumber">Insertion number</param>
        /// <param name="totalInvestment">Total investment</param>
        /// <param name="siteLanguage">Site language</param>
        /// <param name="coverItem">Vehicle cover item</param>
        public VehicleItem(DateTime parutionDate, string insertionNumber, string totalInvestment, string totalInvestmentUsd, int siteLanguage, CoverItem coverItem) :
            base(parutionDate, insertionNumber, totalInvestment, siteLanguage, coverItem)
        {
            _totalInvestmentUsd = totalInvestmentUsd;

        }
        #endregion

        #region Render
        /// <summary>
        /// Item Render
        /// </summary>
        /// <returns>Html code</returns>
        public override string Render() {
            
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
            sb.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(2748, _siteLanguage) + " :" + _totalInvestment + "</td><tr>");
            sb.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(2749, _siteLanguage) + " :" + _totalInvestmentUsd + "</td><tr>");

            sb.Append("</table>");

            return sb.ToString();

        }
       
        #endregion
    }
}
