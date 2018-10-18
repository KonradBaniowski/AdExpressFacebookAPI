using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpressI.Portofolio.Engines;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.FrameWork.WebResultUI;
using WebCst = TNS.AdExpress.Constantes.Web;
using AbstractResult = TNS.AdExpressI.Portofolio.Engines;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class BreakdownEngine : AbstractResult.BreakdownEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        /// <param name="excel">Excel</param>
        public BreakdownEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool excel, DetailLevelItemInformation level)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, excel, level) {}
        #endregion

        #region Abstract methods implementation
        /// <summary>
        /// Build Html result
        /// </summary>
        /// <returns></returns>
        protected override string BuildHtmlResult()
        {
            throw new PortofolioException("The method or operation is not implemented.");
        }

        /// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override GridResult BuildGridResult()
        {
            return GetBreakdownGrid();
        }

        /// <summary>
        /// Build Html result
        /// </summary>
        /// <returns></returns>
        protected override ResultTable ComputeResultTable()
        {
            throw new PortofolioException("The method or operation is not implemented.");
        }
        #endregion

        #region GetStructureGrid
        /// <summary>
        /// Get structure grid
        /// <remarks>Used currently for vehicle , tv,others an radio</remarks>
        /// </summary>
        /// <returns></returns>
        protected virtual GridResult GetBreakdownGrid()
        {
            StringBuilder t = new StringBuilder(5000);
            DataSet ds = null;
            DataTable dt = null;
            List<UnitInformation> unitInformationList = new List<UnitInformation>();
            string P2 = "p2";
            string backGround = "backGroundWhite";
            string classCss = "acl1";
            string hourIntervallLabel = "";
            double totalUnit = 0;
            AdExpressCultureInfo cInfo = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;
            IFormatProvider fp = (_excel) ?
                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfoExcel
                : WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[6];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            parameters[5] = _level;

            GridResult gridResult = new GridResult();
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            ds = portofolioDAL.GetData();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                dt = ds.Tables[0];
                unitInformationList = _webSession.GetSelectedUnits();

                object[,] gridData = new object[dt.Rows.Count + 1, dt.Columns.Count + 2]; //Rows : +2 car hedaer et ligne total // Columns : +2 car ID et PID en plus
                List<object> columns = new List<object>();
                List<object> schemaFields = new List<object>();
                List<object> columnsFixed = new List<object>();

                columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
                schemaFields.Add(new { name = "ID" });
                columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
                schemaFields.Add(new { name = "PID" });

                //Heades
                string colKey = string.Empty;
                colKey = "LabelKey";
                columns.Add(new { headerText = GestionWeb.GetWebWord(1451, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
                schemaFields.Add(new { name = colKey });
                // if (j == 0) columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });


                for (int i = 0; i < unitInformationList.Count; i++)
                {
                    colKey = string.Format("key{0}-unit-{1}", i, unitInformationList[i].Id);
                    string typeOfData = "number";//number
                    string format = cInfo.GetFormatPatternFromStringFormat(UnitsInformation.Get(unitInformationList[i].Id).StringFormat);
                    if (unitInformationList[i].Id == WebCst.CustomerSessions.Unit.duration) format = "duration";
                    columns.Add(new { headerText = GestionWeb.GetWebWord(unitInformationList[i].WebTextId, _webSession.SiteLanguage), key = colKey, dataType = typeOfData, format = format, width = "*", columnCssClass = "colStyle", allowSorting = true });
                    schemaFields.Add(new { name = colKey });
                    // if (j == 0) columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });                  
                }

                //Total line
                int k = 0;
                gridData[0, k] = 0; // Pour column ID

                k++;
                gridData[0, k] = -1; // Pour column PID

                k++;
                gridData[0, k] = GestionWeb.GetWebWord(1401, _webSession.SiteLanguage);
                ////line total units
                for (int i = 0; i < unitInformationList.Count; i++)
                {
                    totalUnit = 0;
                    k++;
                    foreach (DataRow dr in dt.Rows)
                    {
                        totalUnit += (dr[unitInformationList[i].Id.ToString()] != System.DBNull.Value) ? double.Parse(dr[unitInformationList[i].Id.ToString()].ToString()) : 0;
                    }

                    gridData[0, k] = Units.ConvertUnitValue(totalUnit, unitInformationList[i].Id);
                }


                //One line by time interval
                int currentLineIndex = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    k = 0;
                    gridData[currentLineIndex, k] = currentLineIndex; // Pour column ID

                    k++;
                    gridData[currentLineIndex, k] = 0; // Pour column PID

                    k++;
                    gridData[currentLineIndex, k] = dr[_level.DataBaseField].ToString();

                    //Unit Value
                    for (int i = 0; i < unitInformationList.Count; i++)
                    {
                        k++;
                        gridData[currentLineIndex, k] = Units.ConvertUnitValue(dr[unitInformationList[i].Id.ToString()], unitInformationList[i].Id);
                    }

                    currentLineIndex++;
                }

                // gridResult.NeedFixedColumns = true;
                gridResult.HasData = true;
                gridResult.Columns = columns;
                gridResult.Schema = schemaFields;
                //gridResult.ColumnsFixed = columnsFixed;
                gridResult.Data = gridData;

            }
            else
            {
                gridResult.HasData = false;
            }



            return gridResult;
        }

        #endregion
    }
}
