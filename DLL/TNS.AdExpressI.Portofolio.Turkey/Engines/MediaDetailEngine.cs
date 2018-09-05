using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.FrameWork.WebResultUI;
using AbstractResult = TNS.AdExpressI.Portofolio.Engines;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class MediaDetailEngine : AbstractResult.MediaDetailEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">vehicleInformation</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">vehicleInformation</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool excel)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
            _excel = excel;
        }

        #endregion

        #region Abstract methods implementation
        /// <summary>
        /// Build Html result
        /// </summary>
        /// <returns></returns>
        protected override ResultTable ComputeResultTable()
        {
            throw new PortofolioException("The method or operation is not implemented.");
        }
        /// <summary>
        /// Build Html result
        /// </summary>
        /// <returns></returns>
        protected override string BuildHtmlResult()
        {
            switch (_vehicleInformation.Id)
            {
                case AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                    return GetDetailMediaHtml();
                default: throw new PortofolioException("The method to get data is not defined for this vehicle.");
            }
        }
        #endregion

        #region GetFormattedTableDetailMedia 
        /// <summary>
        /// Create a table with each week day the media's investment
        /// and the number of spot
        /// </summary>
        /// <returns>table with each week day the media's investment
        ///  and the number of spot</returns>
        public override DataTable GetFormattedTableDetailMedia(IPortofolioDAL portofolioDAL)
        {

            #region Variables
            DataTable dt = null, dtResult = null;
            DataRow newRow = null;
            DateTime dayDT;
            int currentLine = 0;
            string oldTimeSlot = "-1";
            string timeSlot;
            string dayString = "";
            bool start = true;
            #endregion

            List<UnitInformation> unitsList = _webSession.GetValidUnitForResult();

            DataSet ds = portofolioDAL.GetData();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && unitsList != null && unitsList.Count > 0)
            {
                dt = ds.Tables[0];

                #region Init table

                dtResult = new DataTable();
                dtResult.Columns.Add("timeSlot", System.Type.GetType("System.String"));
                for (int i = 0; i < dayName.Length; i++)
                {
                    dtResult.Columns.Add(dayName[i], System.Type.GetType("System.Double"));
                }
                dtResult.Columns.Add("IdUnit", System.Type.GetType("System.Int32"));

                #endregion

                #region for each table row
                foreach (DataRow row in dt.Rows)
                {

                    #region get Day of Week
                    dayDT = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0, 4)), int.Parse(row["date_media_num"].ToString().Substring(4, 2)), int.Parse(row["date_media_num"].ToString().Substring(6, 2)));
                    switch (dayDT.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            dayString = "Monday";
                            break;
                        case DayOfWeek.Tuesday:
                            dayString = "Tuesday";
                            break;
                        case DayOfWeek.Wednesday:
                            dayString = "Wednesday";
                            break;
                        case DayOfWeek.Thursday:
                            dayString = "Thursday";
                            break;
                        case DayOfWeek.Friday:
                            dayString = "Friday";
                            break;
                        case DayOfWeek.Saturday:
                            dayString = "Saturday";
                            break;
                        case DayOfWeek.Sunday:
                            dayString = "Sunday";
                            break;
                    }
                    #endregion

                    #region Get Current code ecran
                    timeSlot = row["tranche_horaire"].ToString();
                    #endregion

                    if (timeSlot != oldTimeSlot)
                    {

                        #region Create new lines
                        for (int i = 0; i < unitsList.Count; i++)
                        {
                            newRow = dtResult.NewRow();
                            dtResult.Rows.Add(newRow);
                        }
                        #endregion

                        oldTimeSlot = timeSlot;


                        if (!start)
                        {
                            currentLine += unitsList.Count;
                        }
                    }

                    for (int i = 0; i < unitsList.Count; i++)
                    {
                        dtResult.Rows[currentLine + i]["timeSlot"] = row["tranche_horaire"].ToString();
                        if (dtResult.Rows[currentLine + i][dayString] != null && dtResult.Rows[currentLine + i][dayString] != System.DBNull.Value)
                            dtResult.Rows[currentLine + i][dayString] = ((double)dtResult.Rows[currentLine + i][dayString]) + double.Parse(row[unitsList[i].Id.ToString()].ToString());
                        else
                            dtResult.Rows[currentLine + i][dayString] = double.Parse(row[unitsList[i].Id.ToString()].ToString());
                        dtResult.Rows[currentLine + i]["IdUnit"] = unitsList[i].Id.GetHashCode();
                    }
                    start = false;
                }
                #endregion
            }
            return dtResult;
        }
        #endregion

        /// <summary>
		/// Get Detail Media for Tv & Radio
		/// </summary>
		/// <param name="excel">true for excel result</param>
		/// <returns>HTML Code</returns>
		protected override GridResult GetDetailMediaGridResult()
        {

            UnitInformation unitInformation = null;
            string unitWebText = string.Empty;

            DataTable dt = null;
            StringBuilder t = new StringBuilder();
            string oldTimeSlot = "-1";
            string insertionDetailPath = "/PortfolioDetailMedia";
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;


            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);

            GridResult gridResult = new GridResult();
            dt = GetFormattedTableDetailMedia(portofolioDAL);


            #region	No data
            if (dt == null || dt.Rows.Count == 0)
            {
                gridResult.HasData = false;
                return gridResult;
            }
            #endregion

            List<UnitInformation> unitsList = _webSession.GetValidUnitForResult();
            int NbColumn = 9;
            object[,] gridData = new object[dt.Rows.Count, NbColumn + 2]; //+2 car ID et PID en plus
            List<object> columns = new List<object>();
            List<object> schemaFields = new List<object>();
            List<object> columnsFixed = new List<object>();

            columns.Add(new { headerText = "ID", key = "ID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "ID" });
            columns.Add(new { headerText = "PID", key = "PID", dataType = "number", width = "*", hidden = true });
            schemaFields.Add(new { name = "PID" });

            string colKey = "colKey1";
            string colKeyLabel = string.Empty;

            if (_vehicleInformation.Id == AdExpress.Constantes.Classification.DB.Vehicles.names.tv)
            {
                colKeyLabel = GestionWeb.GetWebWord(1575, _webSession.SiteLanguage);
            }

            columns.Add(new { headerText = colKeyLabel, key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            colKey = "colKey2";
            columns.Add(new { headerText = string.Empty, key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });
            // if (j == 0) columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });

            #region First line

            //Monday
            colKey = "Key654";
            columns.Add(new { headerText = GestionWeb.GetWebWord(654, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Tuesday
            colKey = "Key655";
            columns.Add(new { headerText = GestionWeb.GetWebWord(655, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            //Wednesday
            colKey = "Key656";
            columns.Add(new { headerText = GestionWeb.GetWebWord(656, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Thursday
            colKey = "Key657";
            columns.Add(new { headerText = GestionWeb.GetWebWord(657, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            // friday
            colKey = "Key658";
            columns.Add(new { headerText = GestionWeb.GetWebWord(658, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Saturday
            colKey = "Key659";
            columns.Add(new { headerText = GestionWeb.GetWebWord(659, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Sunday
            colKey = "Key660";
            columns.Add(new { headerText = GestionWeb.GetWebWord(660, _webSession.SiteLanguage), key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            #endregion

            int currentLineIndex = 0;
            foreach (DataRow dr in dt.Rows)
            {
                unitInformation = UnitsInformation.Get((AdExpress.Constantes.Web.CustomerSessions.Unit)long.Parse(dr["idUnit"].ToString()));

                if (unitsList.Contains(unitInformation))
                {

                    int k = 0;
                    gridData[currentLineIndex, k] = currentLineIndex; // Pour column ID

                    k++;
                    gridData[currentLineIndex, k] = -1; // Pour column PID

                    if (oldTimeSlot != dr["timeSlot"].ToString())
                    {


                        // Screen code
                        k++;
                        gridData[currentLineIndex, k] = GetScreenCodeText(dr["timeSlot"].ToString());


                    }
                    else
                    {
                        k++;
                        gridData[currentLineIndex, k] = string.Empty;
                    }

                    #region Column units
                    if (unitInformation.Id == AdExpress.Constantes.Web.CustomerSessions.Unit.euro
                            || unitInformation.Id == AdExpress.Constantes.Web.CustomerSessions.Unit.kEuro
                            || unitInformation.Id == AdExpress.Constantes.Web.CustomerSessions.Unit.rubles
                            || unitInformation.Id == AdExpress.Constantes.Web.CustomerSessions.Unit.usd)
                        unitWebText = GestionWeb.GetWebWord(471, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(unitInformation.WebTextId, _webSession.SiteLanguage) + ")";
                    else
                        unitWebText = GestionWeb.GetWebWord(unitInformation.WebTextId, _webSession.SiteLanguage);

                    k++;
                    gridData[currentLineIndex, k] = unitWebText;

                    #endregion

                    #region Column day of week
                    for (int i = 0; i < dayName.Length; i++)
                    {
                        if (dr[dayName[i]] != null && dr[dayName[i]] != System.DBNull.Value && !dr[dayName[i]].ToString().Equals("0"))
                        {

                            // idMedia = 2003 & dayOfWeek = Wednesday & ecran = 515
                            k++;
                            gridData[currentLineIndex, k] =
                                string.Format("<a href='{0}?idMedia={1}&dayOfWeek={2}&ecran={3}' target='_blank'>{4}</a>",
                                insertionDetailPath, _idMedia, dayName[i], dr["timeSlot"].ToString(), Units.ConvertUnitValueAndPdmToString(dr[dayName[i]], unitInformation.Id, false, fp));

                        }
                        else
                        {
                            k++;
                            gridData[currentLineIndex, k] = string.Empty;
                        }
                    }
                    #endregion

                    oldTimeSlot = dr["timeSlot"].ToString();

                    currentLineIndex++;


                }
            }
            //gridResult.NeedFixedColumns = true;
            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            gridResult.ColumnsFixed = columnsFixed;
            gridResult.Data = gridData;

            return gridResult;
        }
    }
}
