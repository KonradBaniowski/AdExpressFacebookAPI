#region Informations
// Auteur: Y. Rkaina
// Date de création: 02-Juin.-2006 11:19:12
// Date de modification:
#endregion

using System;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using TefnoutExceptions = TNS.AdExpress.Anubis.Tefnout.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TefnoutFunctions = TNS.AdExpress.Anubis.Tefnout.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using CsteCustomer=TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpressI.VP.DAL;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;

namespace TNS.AdExpress.Anubis.Tefnout.UI
{
	/// <summary>
    /// Export veille promo
	/// </summary>
	public class Plan
    {

        #region SetExcelSheet
        /// <summary>
		/// Analyse des parts de voix
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {

            WebSession _session = null;
            string _periodBeginningDate = string.Empty, _periodEndDate = string.Empty;
            IVeillePromoDAL vpScheduleDAL =null;
            object[] param = null;
            DataSet ds =null;
            if (webSession == null) throw new NullReferenceException(" parameter session cannot be null ");
            _session = webSession;

            Domain.Web.Navigation.Module _module = ModulesList.GetModule(WebConstantes.Module.Name.VP);

            if (_session.PeriodType == WebConstantes.CustomerSessions.Period.Type.allHistoric)
            {

                param = new object[1];
                param[0] = _session;
                vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName), 
                    _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                ds = vpScheduleDAL.GetMinMaxPeriod();
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    string periodBeginningDate = ds.Tables[0].Rows[0]["DATE_BEGIN_NUM"].ToString();
                    var dateBegin = new DateTime(int.Parse(periodBeginningDate.Substring(0, 4)), 
                        int.Parse(periodBeginningDate.Substring(4, 2)), 1);
                    dateBegin = dateBegin.AddMonths(-1);
                    _periodBeginningDate = dateBegin.ToString("yyyyMMdd");
                    string periodEndDate = ds.Tables[0].Rows[0]["DATE_END_NUM"].ToString();
                    var dateEnd = new DateTime(int.Parse(periodEndDate.Substring(0, 4)),
                        int.Parse(periodEndDate.Substring(4, 2)), int.Parse(periodEndDate.Substring(6, 2)));
                    dateEnd = dateEnd.AddMonths(1);
                    int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                    _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
                }
            }
            else
            {
                var dateBegin = new DateTime(int.Parse(_session.PeriodBeginningDate.Substring(0, 4)),
                    int.Parse(_session.PeriodBeginningDate.Substring(4, 2)), 1);
                dateBegin = dateBegin.AddMonths(-1);
                _periodBeginningDate = dateBegin.ToString("yyyyMMdd");
                var dateEnd = new DateTime(int.Parse(_session.PeriodEndDate.Substring(0, 4)),
                    int.Parse(_session.PeriodEndDate.Substring(4, 2)), int.Parse(_session.PeriodEndDate.Substring(6, 2)));
                dateEnd = dateEnd.AddMonths(1);
                int days = DateTime.DaysInMonth(dateEnd.Year, dateEnd.Month);
                _periodEndDate = dateEnd.ToString("yyyyMM") + days.ToString();
            }
           param = new object[3];
            param[0] = _session;
            param[1] = _periodBeginningDate;
            param[2] = _periodEndDate;
            vpScheduleDAL = (IVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName), 
                _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            ds = vpScheduleDAL.GetBenchMarkData();


            if (ds != null &&   ds.Tables[0] != null && ds.Tables[0].Rows.Count> 0)
            {
                Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
                sheet.Name = GestionWeb.GetWebWord(2891, _session.SiteLanguage);
                Cells cells = sheet.Cells;
                int cellRow = 0;
                int cellCol = 0;
                DataTable dt = ds.Tables[0];

                #region Header
                //Segment title
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2877, _session.SiteLanguage));
                cells[cellRow, cellCol].Style.HorizontalAlignment = TextAlignmentType.Center;
                style.GetTag("PlanRowTitleFirstRowFirstCol").SetStyleExcel(cells[cellRow, cellCol]);
                

                //Type de pièce  title  
                cellCol++;
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2880, _session.SiteLanguage));
                cells[cellRow, cellCol].Style.HorizontalAlignment = TextAlignmentType.Center;
                style.GetTag("PlanRowTitleFirstRow").SetStyleExcel(cells[cellRow, cellCol]);
                

                //Enseigne title   
                cellCol++;
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2876, _session.SiteLanguage));
                style.GetTag("PlanRowTitleFirstRow").SetStyleExcel(cells[cellRow, cellCol]);
                

                  //Période de validité title   
                cellCol++;
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2887, _session.SiteLanguage));
                style.GetTag("PlanRowTitleFirstRow").SetStyleExcel(cells[cellRow, cellCol]);
                

                //Mois de départ   
                cellCol++;
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2888, _session.SiteLanguage));
                style.GetTag("PlanRowTitleFirstRow").SetStyleExcel(cells[cellRow, cellCol]);
                


                //Contenu de la promotion title   
                cellCol++;
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2889, _session.SiteLanguage));
                style.GetTag("PlanRowTitleFirstRow").SetStyleExcel(cells[cellRow, cellCol]);

                //Exclu Web   
                cellCol++;
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2997, _session.SiteLanguage));
                style.GetTag("PlanRowTitleFirstRow").SetStyleExcel(cells[cellRow, cellCol]);


                //Media type   
                cellCol++;
                cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(363, _session.SiteLanguage));
                style.GetTag("PlanRowTitleFirstRow").SetStyleExcel(cells[cellRow, cellCol]);

                
                #endregion

                #region Content
                foreach (DataRow dr in dt.Rows)
                {
                    cellCol = 0;
                    cellRow++;
                    //Segment title
                    cells[cellRow, cellCol].PutValue(dr["SEGMENT"].ToString());
                    style.GetTag("PlanRowDefaultFirstCol").SetStyleExcel(cells[cellRow, cellCol]);

                    //Type de pièce  title  
                    cellCol++;
                    cells[cellRow, cellCol].PutValue(dr["PRODUCT"].ToString());
                    style.GetTag("PlanRowDefault").SetStyleExcel(cells[cellRow, cellCol]);

                    //Enseigne title   
                    cellCol++;
                    cells[cellRow, cellCol].PutValue(dr["BRAND"].ToString());
                    style.GetTag("PlanRowDefault").SetStyleExcel(cells[cellRow, cellCol]);

                    //Période de validité title   
                    cellCol++;
                    cells[cellRow, cellCol].PutValue(DateString.YYYYMMDDToDD_MM_YYYY(dr["DATE_BEGIN_NUM"].ToString(),
                        _session.SiteLanguage) + "-" + DateString.YYYYMMDDToDD_MM_YYYY(dr["DATE_END_NUM"].ToString(),_session.SiteLanguage));
                    style.GetTag("PlanRowDefault").SetStyleExcel(cells[cellRow, cellCol]);

                    //Mois de départ   
                    cellCol++;
                    cells[cellRow, cellCol].PutValue(WebFunctions.Dates.GetMonthLabel(Convert.ToInt32(dr["DATE_BEGIN_NUM"].ToString().Substring(4, 2)),
                        _session.SiteLanguage) + " " + dr["DATE_BEGIN_NUM"].ToString().Substring(2, 2));
                    style.GetTag("PlanRowDefault").SetStyleExcel(cells[cellRow, cellCol]);

                    //Contenu de la promotion title   
                    cellCol++;
                    if (dr["PROMOTION_CONTENT"] != System.DBNull.Value && dr["PROMOTION_CONTENT"].ToString().Length>0)
                    cells[cellRow, cellCol].PutValue(dr["PROMOTION_CONTENT"].ToString());
                    style.GetTag("PlanRowDefault").SetStyleExcel(cells[cellRow, cellCol]);
                

                    //Exclu Web
                    cellCol++;
                    if (dr["EXCLU_WEB"] != DBNull.Value && Convert.ToInt32(dr["EXCLU_WEB"].ToString())==1)
                        cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2998, _session.SiteLanguage));
                    else cells[cellRow, cellCol].PutValue(GestionWeb.GetWebWord(2999, _session.SiteLanguage));
                    style.GetTag("PlanRowDefault").SetStyleExcel(cells[cellRow, cellCol]);

                    //Media type   
                    cellCol++;
                    cells[cellRow, cellCol].PutValue(dr["VEHICLE"].ToString());
                    style.GetTag("PlanRowDefault").SetStyleExcel(cells[cellRow, cellCol]);
               
                }
                #endregion

                sheet.AutoFitColumns();
                sheet.AutoFitRows();
            }
            

		}
		#endregion
      
	}
}
