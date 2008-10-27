#region Information
//Authors: Y. Rkaina
//Date of Creation: 05/06/2006
//Date of modification:
#endregion

using System;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;

using TNS.AdExpress.Anubis.Satet;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using SatetExceptions=TNS.AdExpress.Anubis.Satet.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using SatetFunctions=TNS.AdExpress.Anubis.Satet.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using CsteCustomer=TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;
using System.Reflection;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Theme;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de MediaPlan.
	/// </summary>
	public class MediaPlan
	{

		#region Calendrier d'actions
		/// <summary>
		/// Calendrier d'actions
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.AdExpress.Domain.Theme.Style style) {

            #region Variables
			string currentCategoryName=string.Empty;
			string prevYearString=string.Empty;
			int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
            Domain.Web.Navigation.Module module;
            object[] param;
            DateTime begin;
            DateTime end;
            MediaSchedulePeriod period;

            #endregion

            #region targets
            //base target
            Int64 idBaseTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmBaseTargetAccess));
			//additional target
            Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
            Int64 idWave = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave, CsteCustomer.Right.type.aepmWaveAccess));									
			#endregion

		    #region Period Detail
            begin = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
            end = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
            if (webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3)) {
                webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
            }
            period = new MediaSchedulePeriod(begin, end, webSession.DetailPeriod);
            #endregion

			#region Données resultats
            module = ModulesList.GetModule(webSession.CurrentModule);
            
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Media Schedule result"));
            param = new object[2];
            param[0] = webSession;
            param[1] = period;
            IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            mediaScheduleResult.Module = module;

            //tab=TNS.AdExpress.Web.Rules.Results.APPM.MediaPlanRules.GetFormattedTable(webSession,dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,webSession.DetailPeriod);
            mediaScheduleResult.GetRawData(excel,style);
            #endregion	

        }
		#endregion
	}
}
