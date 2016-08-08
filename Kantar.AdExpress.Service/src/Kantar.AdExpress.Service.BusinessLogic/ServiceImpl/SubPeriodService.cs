using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork.Date;
using WebCst = TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class SubPeriodService : ISubPeriodService
    {
        public SubPeriod GetSubPeriod(string idWebSession, string zoomDate)
        {
            SubPeriod subPeriod = new SubPeriod();

            WebSession CustomerSession = (WebSession)WebSession.Load(idWebSession);
            subPeriod.HideExistButton = (CustomerSession.CurrentModule == WebCst.Module.Name.FACEBOOK);
            int periodIndex = 0;
            int i = -1;
            string labBegin = CustomerSession.PeriodBeginningDate;
            string labEnd = CustomerSession.PeriodEndDate;
            string themeName = WebApplicationParameters.Themes[CustomerSession.SiteLanguage].Name;

            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[CustomerSession.SiteLanguage].Localization);
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[CustomerSession.SiteLanguage].CultureInfo;

            WebCst.CustomerSessions.Period.Type periodType = CustomerSession.PeriodType;
            WebCst.CustomerSessions.Period.DisplayLevel periodDisplay = CustomerSession.DetailPeriod;
            string periodBegin = CustomerSession.PeriodBeginningDate;
            string periodEnd = CustomerSession.PeriodEndDate;
            string realPeriodBegin = CustomerSession.PeriodBeginningDate;
            string realPeriodEnd = CustomerSession.PeriodEndDate;

            DateTime begin = Dates.GetPeriodBeginningDate(realPeriodBegin, CustomerSession.PeriodType);
            DateTime today = DateTime.Now.Date;

            realPeriodBegin = begin.ToString("yyyyMMdd");
            realPeriodEnd = Dates.GetPeriodEndDate(realPeriodEnd, CustomerSession.PeriodType).ToString("yyyyMMdd");

            if (periodDisplay == WebCst.CustomerSessions.Period.DisplayLevel.weekly)
            {
                periodType = WebCst.CustomerSessions.Period.Type.dateToDateWeek;
                var tmp = new AtomicPeriodWeek(new DateTime(int.Parse(realPeriodBegin.Substring(0, 4)), int.Parse(realPeriodBegin.Substring(4, 2)), int.Parse(realPeriodBegin.Substring(6, 2))));
                periodBegin = string.Format("{0}{1}", tmp.FirstDay.AddDays(3).Year, tmp.Week.ToString("0#"));
                tmp = new AtomicPeriodWeek(new DateTime(int.Parse(realPeriodEnd.Substring(0, 4)), int.Parse(realPeriodEnd.Substring(4, 2)), int.Parse(realPeriodEnd.Substring(6, 2))));
                periodEnd = string.Format("{0}{1}", tmp.FirstDay.AddDays(3).Year, tmp.Week.ToString("0#"));
                subPeriod.PeriodLabel = GestionWeb.GetWebWord(2277, CustomerSession.SiteLanguage);
            }
            else
            {
                periodType = WebCst.CustomerSessions.Period.Type.dateToDateMonth;
                periodBegin = realPeriodBegin.Substring(0, 6);
                periodEnd = realPeriodEnd.Substring(0, 6);
                subPeriod.PeriodLabel = GestionWeb.GetWebWord(2276, CustomerSession.SiteLanguage);
            }

            subPeriod.LeaveZoom = GestionWeb.GetWebWord(2309, CustomerSession.SiteLanguage);

            string currentPeriod = periodBegin;
            periodIndex = -1;
            i = -1;
            List<string> activePeriods = null;


            /****************FirstDate/Lastate*************/
            subPeriod.BeginPeriodLabel = Dates.DateToString(Dates.getZoomBeginningDate(realPeriodBegin, periodType), CustomerSession.SiteLanguage);
            subPeriod.EndPeriodLabel = Dates.DateToString(Dates.getZoomEndDate(realPeriodEnd, periodType), CustomerSession.SiteLanguage);
            subPeriod.AllPeriodLabel = string.Format("{0} {1} {2} {3}",
                GestionWeb.GetWebWord(896, CustomerSession.SiteLanguage),
                subPeriod.BeginPeriodLabel,
                GestionWeb.GetWebWord(897, CustomerSession.SiteLanguage),
                subPeriod.EndPeriodLabel);
            /************************************/

            subPeriod.Items = new List<SubPeriodItem>();
            SubPeriodItem item = new SubPeriodItem();

            do
            {
                i++;

                item = new SubPeriodItem();

                item.Period = currentPeriod;

                if (periodDisplay == WebCst.CustomerSessions.Period.DisplayLevel.weekly)
                    item.Label = currentPeriod.Substring(4, 2);
                else
                    item.Label = MonthString.GetCharacters(int.Parse(currentPeriod.Substring(4, 2)), cultureInfo, 1);

                AppendPeriod(CustomerSession, periodType, currentPeriod, i, ref item, ref periodIndex, realPeriodBegin, realPeriodEnd, ref labBegin, ref labEnd, activePeriods);

                if(zoomDate == "")
                {
                    zoomDate = item.Period;
                }
                if (item.Period == zoomDate)
                {
                    item.IsSelected = true;
                    subPeriod.FirstPeriodLabel = item.PeriodLabel;
                }
                else
                    item.IsSelected = false;

                subPeriod.Items.Add(item);

                if (periodType != WebCst.CustomerSessions.Period.Type.dateToDateWeek)
                {
                    currentPeriod = (new DateTime(int.Parse(currentPeriod.Substring(0, 4)), int.Parse(currentPeriod.Substring(4, 2)), 1)).AddMonths(1).ToString("yyyyMM");
                }
                else
                {
                    var tmp = new AtomicPeriodWeek(int.Parse(currentPeriod.Substring(0, 4)), int.Parse(currentPeriod.Substring(4, 2)));
                    tmp.Increment();
                    currentPeriod = string.Format("{0}{1}", tmp.Year, tmp.Week.ToString("0#"));
                }

            //Voir avec youssef R.
            //} while (periodEnd != currentPeriod);
            } while (int.Parse(periodEnd) >= int.Parse(currentPeriod));


            //item = new SubPeriodItem();

            //item.Period = currentPeriod;

            //if (periodDisplay == WebCst.CustomerSessions.Period.DisplayLevel.weekly)
            //    item.Label = currentPeriod.Substring(4, 2);
            //else
            //    item.Label = MonthString.GetCharacters(int.Parse(currentPeriod.Substring(4, 2)), cultureInfo, 1);

            //AppendPeriod(CustomerSession, periodType, currentPeriod, i, ref item, ref periodIndex, realPeriodBegin, realPeriodEnd, ref labBegin, ref labEnd, activePeriods);

            //if (item.Period == zoomDate)
            //{
            //    item.IsSelected = true;
            //    subPeriod.FirstPeriodLabel = item.PeriodLabel;
            //}
            //else
            //    item.IsSelected = false;

            //subPeriod.Items.Add(item);

            return subPeriod;
        }

        #region AppendPeriod
        private void AppendPeriod(WebSession CustomerSession, WebCst.CustomerSessions.Period.Type periodType, string currentPeriod, int i, ref SubPeriodItem item, ref int periodIndex, string globalDateBegin, string globalDateEnd, ref string labBegin, ref string labEnd, List<string> activePeriods)
        {
            string tmpBegin = string.Empty;
            string tmpEnd = string.Empty;

            if (globalDateBegin.Length > 0)
            {
                tmpBegin = Dates.DateToString(
                    Dates.Max(Dates.getZoomBeginningDate(currentPeriod, periodType),
                    Dates.GetPeriodBeginningDate(globalDateBegin, WebCst.CustomerSessions.Period.Type.dateToDate))
                    , CustomerSession.SiteLanguage);
            }
            else
            {
                tmpBegin = Dates.DateToString(Dates.getZoomBeginningDate(currentPeriod, periodType), CustomerSession.SiteLanguage);
            }

            if (globalDateEnd.Length > 0)
            {
                tmpEnd = Dates.DateToString(
                    Dates.Min(Dates.getZoomEndDate(currentPeriod, periodType),
                    Dates.GetPeriodEndDate(globalDateEnd, WebCst.CustomerSessions.Period.Type.dateToDate))
                    , CustomerSession.SiteLanguage);
            }
            else
            {
                tmpEnd = Dates.DateToString(Dates.getZoomEndDate(currentPeriod, periodType), CustomerSession.SiteLanguage);
            }
            if (tmpEnd != tmpBegin)
            {
                item.PeriodLabel = string.Format("{0} {1} {2} {3}",
                GestionWeb.GetWebWord(896, CustomerSession.SiteLanguage),
                tmpBegin,
                GestionWeb.GetWebWord(897, CustomerSession.SiteLanguage),
                tmpEnd);
            }
            else
            {
                item.PeriodLabel = string.Format("{0}", tmpBegin);
            }
        }
        #endregion
    }
}
