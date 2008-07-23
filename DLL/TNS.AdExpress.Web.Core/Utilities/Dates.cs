#region Information
/*
 * auteur : 
 * créé le :
 * modification : 
 *      22/07/2004 - - Guillaume Ragneau
 *      22/07/2008 - Déplacement depuis TNS.AdExpress.Web.Functions
 *      
 * */
#endregion

using System;
using System.Collections;
using System.Text;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstFrequency = TNS.AdExpress.Constantes.Customer.DB.Frequency;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;

using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Date;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.Core.Utilities{

    /// <summary>
	/// Set of function usefull to treat periods
	/// </summary>
	public class Dates{

        #region Check validity of the period depending on data delivering frequency
        /// <summary>
        /// Check period depending on data delivering frequency
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="EndPeriod">End period</param>
        /// <returns>Period End</returns>
        public static string CheckPeriodValidity( WebSession _session, string EndPeriod){
												
            Int64 frequency = _session.CustomerLogin.GetIdFrequency(_session.CurrentModule);
            switch (frequency){
                case CstFrequency.ANNUAL:
                    //if the studied year is not entirely loaded (== current year or december not loaded)
                    if (int.Parse(EndPeriod.Substring(0,4)) >= int.Parse(_session.LastAvailableRecapMonth.Substring(0,4)))
                        throw new NoDataException();
                    break;
                case CstFrequency.MONTHLY:
                    return GetAbsoluteEndPeriod(_session,EndPeriod,1);
                case CstFrequency.TWO_MONTHLY:
                    return GetAbsoluteEndPeriod(_session,EndPeriod,2);
                case CstFrequency.QUATERLY:					
                    return GetAbsoluteEndPeriod(_session,EndPeriod,3);
                case CstFrequency.SEMI_ANNUAL:
                    return GetAbsoluteEndPeriod(_session,EndPeriod,6);
                case CstFrequency.DAILY:
                    throw new DeliveryFrequencyException("Unvalid Data delivering frequency(" + CstFrequency.DAILY.ToString() + ")");
                case CstFrequency.SEMI_MONTHLY:
                    throw new DeliveryFrequencyException("Unvalid Data delivering frequency(" + CstFrequency.SEMI_MONTHLY.ToString() + ")");
                case CstFrequency.WEEKLY:
                    throw new DeliveryFrequencyException("Unvalid Data delivering frequency(" + CstFrequency.WEEKLY.ToString() + ")");
            }

            return EndPeriod;
        }
        /// <summary>
        /// Get the end of the period depending on data loading
        /// </summary>
        /// <param name="webSession">User session</param>
        /// <param name="EndPeriod">End of the period</param>
        /// <param name="divisor">diviseur</param>
        /// <returns>End of the period</returns>
        private static string GetAbsoluteEndPeriod(WebSession _session, string EndPeriod, int divisor)
        {

            string absoluteEndPeriod = "0";

            //If selected year is lower or equal to data loadin year, then get last loaded month of the last complete trimester
            if (_session.LastAvailableRecapMonth != null && _session.LastAvailableRecapMonth.Length >= 6
                && int.Parse(EndPeriod.Substring(0, 4)) <= int.Parse(_session.LastAvailableRecapMonth.Substring(0, 4))
                )
            {
                //if selected year is equal to last loaded year, get last complete trimester
                //Else get last selected month of the previous year
                if (int.Parse(EndPeriod.Substring(0, 4)) == int.Parse(_session.LastAvailableRecapMonth.Substring(0, 4)))
                {
                    absoluteEndPeriod = _session.LastAvailableRecapMonth.Substring(0, 4) + (int.Parse(_session.LastAvailableRecapMonth.Substring(4, 2)) - int.Parse(_session.LastAvailableRecapMonth.Substring(4, 2)) % divisor).ToString("00");
                    //if study month is greather than the last loaded month, get back to the last loaded month
                    if (int.Parse(EndPeriod) > int.Parse(absoluteEndPeriod))
                    {
                        EndPeriod = absoluteEndPeriod;
                    }
                }
            }
            else
            {
                EndPeriod = EndPeriod.Substring(0, 4) + "00";
            }

            return EndPeriod;
        }
        #endregion

		#region Get Period Label
		/// <summary>
		///  Get Period label in product class analysis depending on selected year
		/// </summary>
		/// <param name="_session">User session</param>
		/// <param name="period">period</param>
		/// <returns>Label describing period in Product Class Analysis depending on selected year</returns>
		public static string getPeriodLabel(WebSession _session,CstPeriod.Type period){

            string beginPeriod="";
			string endPeriod="";
			string year="";
			

			switch(period){
				case CstWeb.CustomerSessions.Period.Type.currentYear :
					beginPeriod=_session.PeriodBeginningDate;
					endPeriod= CheckPeriodValidity(_session, _session.PeriodEndDate);
					
					break;
				case CstWeb.CustomerSessions.Period.Type.previousYear :
					year=(int.Parse(_session.PeriodBeginningDate.Substring(0,4))-1).ToString();
					beginPeriod=year+_session.PeriodBeginningDate.Substring(4);
					//endPeriod= year+_session.PeriodEndDate.Substring(4);
					endPeriod= year+CheckPeriodValidity(_session, _session.PeriodEndDate).Substring(4);
                    						
					break;
				default :
					throw new Exception("getPeriodLabel(_session _session,TNS.AdExpress.Constantes.Web.CustomerSessions.Period period)-->Impossible de déterminer le type de période.");
			}

			return Convertion.ToHtmlString(switchPeriod(_session,beginPeriod,endPeriod));
        }

        /// <summary>
		/// Display of period in product classs analysis
		/// </summary>
		/// <param name="_session">User session</param>
		/// <param name="beginPeriod">Period beginning</param>
		/// <param name="endPeriod">End of period</param>
		/// <returns>Selected period</returns>
		public static string switchPeriod(WebSession _session,string beginPeriod,string endPeriod){
				
			string periodText;
			switch(_session.PeriodType){

				case CstPeriod.Type.nLastMonth:
				case CstPeriod.Type.dateToDateMonth:
				case CstPeriod.Type.previousMonth:
				case CstPeriod.Type.currentYear:
				case CstPeriod.Type.previousYear:
				case CstPeriod.Type.nextToLastYear:			

					if(beginPeriod!=endPeriod)
						periodText=MonthString.Get(int.Parse(beginPeriod.Substring(4,2)),_session.SiteLanguage,0)+"-"+MonthString.Get(int.Parse(endPeriod.Substring(4,2)),_session.SiteLanguage,0)+" "+beginPeriod.Substring(0,4);
					else
						periodText=MonthString.Get(int.Parse(beginPeriod.Substring(4,2)),_session.SiteLanguage,0)+" "+beginPeriod.Substring(0,4);				
					break;
				default:
					throw new Exception("switchPeriod(_session _session,string beginPeriod,string endPeriod)-->Unable to determine type of period.");
			
			}

			return periodText;
		}

        #endregion
    }
}
