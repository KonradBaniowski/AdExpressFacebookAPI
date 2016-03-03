using Kantar.AdExpress.Service.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kantar.AdExpress.Service.Core.Domain.BusinessService;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class PeriodService : IPeriodService
    {
        public PeriodResponse CalendarValidation(string idWebSession, string selectedStartDate, string selectedEndDate)
        {
            var result = new PeriodResponse();
            try
            {
                WebSession CustomerSession = (WebSession)WebSession.Load(idWebSession);
                DateTime lastDayEnable = DateTime.Now;

                DateTime startDate = new DateTime(Convert.ToInt32(selectedStartDate.Substring(6, 4)), Convert.ToInt32(selectedStartDate.Substring(3, 2)), Convert.ToInt32(selectedStartDate.Substring(0, 2)));
                DateTime endDate = new DateTime(Convert.ToInt32(selectedEndDate.Substring(6, 4)), Convert.ToInt32(selectedEndDate.Substring(3, 2)), Convert.ToInt32(selectedEndDate.Substring(0, 2)));

                CustomerSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly;
                CustomerSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;
                CustomerSession.PeriodBeginningDate = startDate.ToString("yyyyMMdd");
                CustomerSession.PeriodEndDate = endDate.ToString("yyyyMMdd");

                if (endDate < DateTime.Now || DateTime.Now < startDate)
                    CustomerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodEndDate);
                else
                    CustomerSession.CustomerPeriodSelected = new TNS.AdExpress.Web.Core.CustomerPeriod(CustomerSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));

                CustomerSession.Save();

                result.Success = true;


            }
            catch (Exception ex)
            {
                result.Success = false;

                result.ErrorMessage = "Une erreur est survenue. Impossible de sauvegarder les dates sélectionnées";//TODO : a mettre dans ressources
            }
            return result;

        }

        public PeriodResponse GetPeriod(string idWebSession)
        {
            var result = new PeriodResponse();
            try
            {
                WebSession webSession = (WebSession)WebSession.Load(idWebSession);

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                int startYear = dateDAL.GetCalendarStartDate();
                int endYear = DateTime.Now.Year;
                 result = new PeriodResponse
                {
                    StartYear = startYear,
                    EndYear = endYear,
                    SiteLanguage = webSession.SiteLanguage,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                result.Success = false;

                result.ErrorMessage = "Une erreur est survenue. Impossible de recupérer la date de début du calendrier";//TODO : a mettre dans ressources
            }           
            return result;
        }
    }
}
