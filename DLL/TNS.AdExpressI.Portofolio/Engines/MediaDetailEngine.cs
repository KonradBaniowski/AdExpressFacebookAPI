#region Information
// Author: D. Mussuma
// Creation date: 12/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;


using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;

using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.FrameWork.Collections;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpressI.Portofolio.Engines
{
    /// <summary>
    /// Compute media detail's results
    /// </summary>
    public class MediaDetailEngine : Engine
    {

        #region Variables
        /// <summary>
		/// Determine if render will be into excel file
		/// </summary>
		protected bool _excel = false;
        /// <summary>
        /// Day name of Week
        /// </summary>
        private string[] dayName = new string[7] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicle">Vehicle</param>
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
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public MediaDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool excel)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
            _excel = excel;
        }

        #endregion

        #region Public methods

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
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    return GetDetailMediaHtml();
                default: throw new PortofolioException("The method to get data is not defined for this vehicle.");
            }
        }
        #endregion

        #endregion

        #region GetFormattedTableDetailMedia 
        /// <summary>
        /// Create a table with each week day the media's investment
        /// and the number of spot
        /// </summary>
        /// <returns>table with each week day the media's investment
        ///  and the number of spot</returns>
        virtual public DataTable GetFormattedTableDetailMedia(IPortofolioDAL portofolioDAL)
        {

            #region Variables
            DataTable dt = null, dtResult = null;
            DataRow newRow = null;
            DateTime dayDT;
            int currentLine = 0;
            int oldEcranCode = -1;
            int ecranCode;
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
                dtResult.Columns.Add("screenCode", System.Type.GetType("System.Int64"));
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
                    ecranCode = int.Parse(row["code_ecran"].ToString());
                    #endregion

                    if (ecranCode != oldEcranCode)
                    {

                        #region Create new lines
                        for (int i = 0; i < unitsList.Count; i++)
                        {
                            newRow = dtResult.NewRow();
                            dtResult.Rows.Add(newRow);
                        }
                        #endregion

                        oldEcranCode = ecranCode;

                        if (!start)
                        {
                            currentLine += unitsList.Count;
                        }
                    }

                    for (int i = 0; i < unitsList.Count; i++)
                    {
                        dtResult.Rows[currentLine + i]["screenCode"] = long.Parse(row["code_ecran"].ToString());
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

        #region GetDetailMediaHtml 
        /// <summary>
        /// Get Detail Media for Tv & Radio
        /// </summary>
        /// <param name="excel">true for excel result</param>
        /// <returns>HTML Code</returns>
        protected string GetDetailMediaHtml()
        {

            #region Variables
            string classStyleValue = "acl2";
            bool color = false;
            UnitInformation unitInformation = null;
            string unitWebText = string.Empty;
            string cssClass = "sc1";
            //bool isTvNatThematiques = false;
            string style = "cursorHand";
            DataTable dt = null;
            StringBuilder t = new StringBuilder(20000);
            long oldEcranCode = -1;
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo;
            #endregion

            #region Get Data
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);

            dt = GetFormattedTableDetailMedia(portofolioDAL);
            #endregion

            #region	No data
            if (dt == null || dt.Rows.Count == 0)
            {
                return GetNoDataMessageHtml();
            }
            #endregion

            //Checks if media belong to TV Nat Thematics
            //string idThematicCategory = TNS.AdExpress.Domain.Lists.GetIdList(WebCst.GroupList.ID.category, WebCst.GroupList.Type.thematicTv);
            //if(idThematicCategory!=null && idThematicCategory.Length>0)
            //isTvNatThematiques = portofolioDAL.IsMediaBelongToCategory(_idMedia,idThematicCategory);

            List<UnitInformation> unitsList = _webSession.GetValidUnitForResult();

            //if (isTvNatThematiques) style = "";
            if (!_excel)
            {//&& !isTvNatThematiques
             //Link to acccess all spot detail
                GetAllPeriodInsertions(t, GestionWeb.GetWebWord(1836, _webSession.SiteLanguage));
            }

            t.Append("<table border=0 cellpadding=0 cellspacing=0 >");

            #region First line
            t.Append("\r\n\t<tr height=\"20px\" >");
            t.Append("<td class=\"p2 violetBorderTop\" colspan=2>&nbsp;</td>");
            //Monday
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(654, _webSession.SiteLanguage) + "</td>");
            // Tuesday
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(655, _webSession.SiteLanguage) + "</td>");
            //Wednesday
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(656, _webSession.SiteLanguage) + "</td>");
            // Thursday
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(657, _webSession.SiteLanguage) + "</td>");
            // friday
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(658, _webSession.SiteLanguage) + "</td>");
            // Saturday
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(659, _webSession.SiteLanguage) + "</td>");
            // Sunday
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(660, _webSession.SiteLanguage) + "</td>");
            t.Append("</tr>");
            #endregion

            #region Table
            foreach (DataRow dr in dt.Rows)
            {

                #region Get current information for current unit result
                unitInformation = UnitsInformation.Get((TNS.AdExpress.Constantes.Web.CustomerSessions.Unit)long.Parse(dr["idUnit"].ToString()));
                #endregion

                if (unitsList.Contains(unitInformation))
                {

                    #region Define color line
                    if (oldEcranCode != long.Parse(dr["screenCode"].ToString()))
                    {
                        color = !color;
                    }
                    #endregion

                    #region Init line
                    if (color)
                    {
                        t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" class=\"violetBackGroundV2\">");
                    }
                    else
                    {
                        t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='greyBackGround';\" class=\"greyBackGround\">");
                    }
                    #endregion

                    #region First column : screenCode
                    if (oldEcranCode != long.Parse(dr["screenCode"].ToString()))
                    {
                        // Screen code
                        t.Append("<td class=\"p2\" rowspan=" + unitsList.Count + " align=\"left\" nowrap>" + GetScreenCodeText(dr["screenCode"].ToString()) + "</td>");
                    }
                    #endregion

                    #region Column units
                    if (unitInformation.Id == WebCst.CustomerSessions.Unit.euro
                        || unitInformation.Id == WebCst.CustomerSessions.Unit.kEuro
                        || unitInformation.Id == WebCst.CustomerSessions.Unit.rubles
                        || unitInformation.Id == WebCst.CustomerSessions.Unit.usd)
                        unitWebText = GestionWeb.GetWebWord(471, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(unitInformation.WebTextId, _webSession.SiteLanguage) + ")";
                    else
                        unitWebText = GestionWeb.GetWebWord(unitInformation.WebTextId, _webSession.SiteLanguage);
                    if (color)
                    {
                        t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + unitWebText + "</td>");
                    }
                    else
                    {
                        t.Append("<td class=\"" + classStyleValue + "\" align=\"left\" nowrap>" + unitWebText + "</td>");
                    }
                    #endregion

                    #region Column day of week
                    for (int i = 0; i < dayName.Length; i++)
                    {
                        if (dr[dayName[i]] != null && dr[dayName[i]] != System.DBNull.Value && !dr[dayName[i]].ToString().Equals("0"))
                        {

                            t.Append("<td class=\"" + cssClass + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, _webSession.SiteLanguage) + "\">");
                            if (!_excel)//&& !isTvNatThematiques
                                t.Append("<a href=\"javascript:portofolioDetailMedia('" + _webSession.IdSession + "','" + _idMedia + "','" + dayName[i] + "','" + dr["screenCode"].ToString() + "');\" class=\"txtLinkBlack11\"> ");

                            t.Append(Units.ConvertUnitValueAndPdmToString(dr[dayName[i]], unitInformation.Id, false, fp));

                            if (!_excel)//&& !isTvNatThematiques
                                t.Append("</a>");
                            t.Append("</td>");
                        }
                        else
                        {
                            t.Append("<td class=\"" + classStyleValue + "\" align=\"right\" nowrap>&nbsp;</td>");
                        }
                    }
                    #endregion

                    #region End line
                    t.Append("</tr>");
                    #endregion

                    oldEcranCode = long.Parse(dr["screenCode"].ToString());

                }

            }
            #endregion

            t.Append("</table>");
            return t.ToString();

        }
        #endregion

        #region GetScreenCodeText
        /// <summary>
        /// Get screen code formated depending on country specificities
        /// </summary>
        /// <param name="screenCode">screen code that we get from DAL</param>
        /// <returns>Screen code formated</returns>
        /// <remarks>We've added this method for Russia</remarks>
        virtual protected string GetScreenCodeText(string screenCode)
        {

            return screenCode;

        }

        protected override GridResult BuildGridResult()
        {
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    return GetDetailMediaGridResult();
                default: throw new PortofolioException("The method to get data is not defined for this vehicle.");
            }
        }
        #endregion


        /// <summary>
		/// Get Detail Media for Tv & Radio
		/// </summary>
		/// <param name="excel">true for excel result</param>
		/// <returns>HTML Code</returns>
		protected GridResult GetDetailMediaGridResult()
        {

           
          
            UnitInformation unitInformation = null;
            string unitWebText = string.Empty;          
                
            DataTable dt = null;
            StringBuilder t = new StringBuilder();
            long oldEcranCode = -1;
            string insertionDetailPath = "/Portfolio/InsertionDetailResult";
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
            columns.Add(new { headerText = string.Empty, key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });

            colKey = "colKey2";
            columns.Add(new { headerText = string.Empty, key = colKey, dataType = "string", width = "*" });
            schemaFields.Add(new { name = colKey });
            // if (j == 0) columnsFixed.Add(new { columnKey = colKey, isFixed = true, allowFixing = false });

            #region First line

            //Monday
            colKey = "Key654";
            columns.Add(new { headerText = GestionWeb.GetWebWord(654, _webSession.SiteLanguage), key = colKey, dataType = "number", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Tuesday
            colKey = "Key655";
            columns.Add(new { headerText = GestionWeb.GetWebWord(655, _webSession.SiteLanguage), key = colKey, dataType = "number", width = "*" });
            schemaFields.Add(new { name = colKey });

            //Wednesday
            colKey = "Key656";
            columns.Add(new { headerText = GestionWeb.GetWebWord(656, _webSession.SiteLanguage), key = colKey, dataType = "number", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Thursday
            colKey = "Key657";
            columns.Add(new { headerText = GestionWeb.GetWebWord(657, _webSession.SiteLanguage), key = colKey, dataType = "number", width = "*" });
            schemaFields.Add(new { name = colKey });

            // friday
            colKey = "Key658";
            columns.Add(new { headerText = GestionWeb.GetWebWord(658, _webSession.SiteLanguage), key = colKey, dataType = "number", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Saturday
            colKey = "Key659";
            columns.Add(new { headerText = GestionWeb.GetWebWord(659, _webSession.SiteLanguage), key = colKey, dataType = "number", width = "*" });
            schemaFields.Add(new { name = colKey });

            // Sunday
            colKey = "Key660";
            columns.Add(new { headerText = GestionWeb.GetWebWord(660, _webSession.SiteLanguage), key = colKey, dataType = "number", width = "*" });
            schemaFields.Add(new { name = colKey });

            #endregion

            int currentLineIndex = 0;
            foreach (DataRow dr in dt.Rows)
            {
                unitInformation = UnitsInformation.Get((WebCst.CustomerSessions.Unit)long.Parse(dr["idUnit"].ToString()));

                if (unitsList.Contains(unitInformation))
                {
                   
                    if (oldEcranCode != long.Parse(dr["screenCode"].ToString()))
                    {
                        int k = 0;
                        gridData[currentLineIndex, k] = currentLineIndex; // Pour column ID

                        k++;
                        gridData[currentLineIndex, k] = -1; // Pour column PID

                        // Screen code
                        k++;
                        gridData[currentLineIndex, k] = GetScreenCodeText(dr["screenCode"].ToString());

                        #region Column units
                        if (unitInformation.Id == WebCst.CustomerSessions.Unit.euro
                            || unitInformation.Id == WebCst.CustomerSessions.Unit.kEuro
                            || unitInformation.Id == WebCst.CustomerSessions.Unit.rubles
                            || unitInformation.Id == WebCst.CustomerSessions.Unit.usd)
                            unitWebText = GestionWeb.GetWebWord(471, _webSession.SiteLanguage) + " (" + GestionWeb.GetWebWord(unitInformation.WebTextId, _webSession.SiteLanguage) + ")";
                        else
                            unitWebText = GestionWeb.GetWebWord(unitInformation.WebTextId, _webSession.SiteLanguage);

                        #endregion

                        #region Column day of week
                        for (int i = 0; i < dayName.Length; i++)
                        {
                            if (dr[dayName[i]] != null && dr[dayName[i]] != System.DBNull.Value && !dr[dayName[i]].ToString().Equals("0"))
                            {
                             
                                k++;
                                gridData[currentLineIndex, k] = string.Format("<a href='javascript:window.open(\"/{0}?{1}&{2}&{3}&{4}\", \"\", \"width=auto, height=auto\");'>{5}</a>",
                                    insertionDetailPath, _webSession.IdSession, _idMedia, dayName[i], dr["screenCode"].ToString(), Units.ConvertUnitValueAndPdmToString(dr[dayName[i]], unitInformation.Id, false, fp));

                            }
                            else
                            {
                                k++;
                                gridData[currentLineIndex, k] = string.Empty;
                            }
                        }
                        #endregion

                        oldEcranCode = long.Parse(dr["screenCode"].ToString());
                    }
                    currentLineIndex++;


                }
            }
            //gridResult.NeedFixedColumns = true;
            gridResult.HasData = true;
            gridResult.Columns = columns;
            gridResult.Schema = schemaFields;
            //gridResult.ColumnsFixed = columnsFixed;
            gridResult.Data = gridData;

            return gridResult;
        }

    }
}
