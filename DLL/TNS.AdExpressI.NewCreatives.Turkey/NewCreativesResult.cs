﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.NewCreatives.DAL;
using TNS.AdExpressI.NewCreatives.Exceptions;
using TNS.FrameWork.Date;
using TNS.FrameWork.WebResultUI;
using GenericDetailLevel = TNS.AdExpress.Domain.Level.GenericDetailLevel;
using WebCst = TNS.AdExpress.Constantes.Web;



namespace TNS.AdExpressI.NewCreatives.Turkey
{
    public class NewCreativesResult : NewCreatives.NewCreativesResult
    {
        /// <summary>
        /// Define if we can show products
        /// </summary>
        protected bool _showProduct;

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public NewCreativesResult(WebSession session)
            : base(session)
        {
        }
        #endregion

        protected override void SetAddressId(int i, DataRow row, AdExpressCellLevel[] cellLevels)
        {
        }

        #region Calendar headers
        /// <summary>
        /// Calendar Headers and Cell factory
        /// </summary>
        protected override void GetCalendarHeaders(out Headers headers, out CellUnitFactory cellFactory, ArrayList parutions)
        {
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
            CellPDM cellPDM = null;

            headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(TOTAL_COL, _webSession.SiteLanguage), TOTAL_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(POURCENTAGE_COL, _webSession.SiteLanguage), POURCENTAGE_COL));

            _showCreative = CanShowCreative();
            // Add Creative column
            if (_showCreative)
            {
                headers.Root.Add(new HeaderInsertions(false, GestionWeb.GetWebWord(VERSION_COL, _webSession.SiteLanguage), VERSION_COL));
            }

            if (_showMediaSchedule)
            {
                // Media schedule column
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
            }

            // Une colonne par date de parution
            parutions.Sort();
            foreach (Int32 parution in parutions)
            {
                switch (_webSession.DetailPeriod)
                {
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly:
                        headers.Root.Add(new Header(true
                            , MonthString.GetCharacters(int.Parse(parution.ToString().Substring(4, 2)), cultureInfo, 0) + " " + parution.ToString().Substring(0, 4)
                            , (long)parution));
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly:
                        headers.Root.Add(new Header(true
                            , string.Format("S{0} - {1}", parution.ToString().Substring(4, 2), parution.ToString().Substring(0, 4))
                            , (long)parution));
                        break;
                    case AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly:
                        headers.Root.Add(new Header(true, AdExpress.Web.Core.Utilities.Dates.DateToString(Dates.YYYYMMDDToDD_MM_YYYY(parution.ToString()).Value, _webSession.SiteLanguage), (long)parution));
                        break;
                    default:
                        break;
                }

            }
            if (!_webSession.Percentage)
            {
                //cellFactory = _webSession.GetCellUnitFactory();
                UnitInformation selectedUnit = UnitsInformation.Get(CustomerSessions.Unit.versionNb);
                var cellIdsNumber =   new CellIdsNumber();
                cellIdsNumber.StringFormat = selectedUnit.StringFormat;
                cellFactory = new CellUnitFactory(cellIdsNumber);

            }
            else
            {
                cellPDM = new CellPDM(0.0);
                cellPDM.StringFormat = "{0:percentWOSign}";
                cellFactory = new CellUnitFactory(cellPDM);
            }
        }
        #endregion

        public override GridResult GetGridResult()
        {
           
            long nbRows = CountData();
            if (nbRows == 0)
            {
                GridResult gridResult = new GridResult();
                gridResult.HasData = false;
                return gridResult;
            }
            if (nbRows > AdExpress.Constantes.Web.Core.MAX_ALLOWED_DATA_ROWS)
            {
                GridResult gridResult = new GridResult();
                gridResult.HasData = true;
                gridResult.HasMoreThanMaxRowsAllowed = true;
                return (gridResult);
            }

            return base.GetGridResult();
        }

        protected override void SetListLine(ResultTable oTab, Int32 cLine, DataRow row)
        {
            if (row != null)
            {
                Int32 lCol = oTab.GetHeadersIndexInResultTable(row["date_creation"].ToString());
                //Get values
                string[] tIds = row[CustomerSessions.Unit.versionNb.ToString()].ToString().Split(',');
                //Affect value
                for (int i = 0; i < tIds.Length; i++)
                {
                    oTab.AffectValueAndAddToHierarchy(1, cLine, lCol, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 3, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 2, Convert.ToInt64(tIds[i]));
                }
            }
        }

        public override ResultTable GetNewCreativeDetailsData()
        {
            ResultTable tab = null;
            DataTable dt = null;
            int iNbLine = 0;

            #region Chargement des données
            if (_module.CountryDataAccessLayer == null)
                throw (new NullReferenceException("DAL layer is null for the new creative result"));
            var parameters = new object[4];
            parameters[0] = _webSession;
            parameters[1] = _idSectors;
            parameters[2] = _beginingDate;
            parameters[3] = _endDate;
            var newCreativesDAL = (INewCreativeResultDAL)AppDomain.CurrentDomain.
                CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, _module.CountryDataAccessLayer.AssemblyName),
                _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
           var ds = newCreativesDAL.GetNewCreativeDetailsData();

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) dt = ds.Tables[0];
            else return (tab);
            #endregion

            #region Product detail level (Generic)
            // Initialisation to product
            var levels = new ArrayList { 10 };
            _webSession.GenericProductDetailLevel = new GenericDetailLevel(levels,
                TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
            #endregion

            #region Columns levels (Generic)
            var _columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(_vehicleInformation.DetailColumnId);

            var columnIdList = _columnItemList.Select(column => (int)column.Id).Select(dummy => (long)dummy).ToList();

            _webSession.GenericInsertionColumns = new GenericColumns(columnIdList);

            #endregion

            #region Rigths management des droits
            // Show creatives

            if (_webSession.CustomerLogin.ShowCreatives(_vehicleInformation.Id) && _vehicleInformation.ShowCreations) _showCreative = true;
       
          
            //Show column product
            bool showProduct = _webSession.CustomerLogin.CustormerFlagAccess(AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            #endregion

            #region Table nb rows
            iNbLine = dt.Rows.Count;
            #endregion

            #region Initialisation of result table
            try
            {
                var headers = new Headers();
                _columnItemList = WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(_vehicleInformation.DetailColumnId);

                foreach (GenericColumnItemInformation Column in _columnItemList)
                {

                    switch (Column.Id)
                    {
                        case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
                        case GenericColumnItemInformation.Columns.visual://Visual press
                            if (_showCreative)
                                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                            break;                      
                        case GenericColumnItemInformation.Columns.planMedia://Plan media
                            if (_showMediaSchedule)
                                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                            break;                                            
                        case GenericColumnItemInformation.Columns.product:
                            if (showProduct && WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicleInformation.DetailColumnId, Column.Id))
                                headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                            break;
                        default:
                            if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicleInformation.DetailColumnId, Column.Id))
                                headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(Column.WebTextId, _webSession.SiteLanguage), Column.WebTextId));
                            break;
                    }

                }

                tab = new ResultTable(iNbLine, headers);

                SetResultTable(dt, tab);
            }
            catch (System.Exception err)
            {
                throw (new NewCreativesException("Error while initiating headers of New creative details", err));
            }
            #endregion

            throw new NotImplementedException();
        }

        public override GridResult GetNewCreativeDetailsGridResult()
        {
            throw new NotImplementedException();
        }

        #region SetResultTable
        /// <summary>
        /// SetResultTable
        /// </summary>
        /// <param name="dt">Data Table</param>
        /// <param name="tab">Result table</param>
        protected virtual void SetResultTable(DataTable dt, ResultTable tab)
        {

            string dateMediaNum = string.Empty;
            DateTime dateMedia;
            int iCurLine = 0;
            int iCurColumn = 0;
            string[] files;
            string listVisual = "";
            Cell curCell = null;
            string date = "";
            string temp = string.Empty;
            Assembly assembly;
            Type type;

            try
            {
                // assembly loading
                assembly = Assembly.Load(@"TNS.FrameWork.WebResultUI");
            

                //string blur = TNS.AdExpress.Web.Functions.Rights.HasPressCopyright(_idMedia) ? string.Empty : "blur/";
                foreach (DataRow row in dt.Rows)
                {
                    dateMedia = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0, 4)), int.Parse(row["date_media_num"].ToString().Substring(4, 2)), int.Parse(row["date_media_num"].ToString().Substring(6, 2)));
                    dateMediaNum = dateMedia.DayOfWeek.ToString();

                    //if (_dayOfWeek == dateMediaNum || _allPeriod)
                    //{

                        tab.AddNewLine(LineType.level1);
                        iCurColumn = 1;
                        foreach (GenericColumnItemInformation Column in _columnItemList)
                        {
                            switch (Column.Id)
                            {
                               
                                case GenericColumnItemInformation.Columns.associatedFile://Visual radio/tv
                                    if (_showCreative)
                                    {
                                        switch (_vehicleInformation.Id)
                                        {
                                          
                                            case Vehicles.names.tv:
                                            case Vehicles.names.tvGeneral:
                                            case Vehicles.names.tvSponsorship:
                                            case Vehicles.names.tvAnnounces:
                                            case Vehicles.names.tvNonTerrestrials:
                                            case Vehicles.names.others:
                                                if (row[Column.DataBaseField].ToString().Length > 0)
                                                    tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(
                                                        Convert.ToString(row[Column.DataBaseField]), _webSession, GetTvId());
                                                else
                                                    tab[iCurLine, iCurColumn++] = new CellTvCreativeLink(string.Empty,
                                                        _webSession, GetTvId());

                                                break;
                                        }
                                    }
                                    break;
                               
                                case GenericColumnItemInformation.Columns.planMedia://Plan media
                                    if (_showMediaSchedule)
                                        tab[iCurLine, iCurColumn++] = new CellInsertionMediaScheduleLink(_webSession, Convert.ToInt64(row["id_product"]), 1);
                                    break;
                                case GenericColumnItemInformation.Columns.dateParution:// Parution Date and  diffusion Date
                                case GenericColumnItemInformation.Columns.dateDiffusion:
                                case GenericColumnItemInformation.Columns.dateMediaNum:
                                case GenericColumnItemInformation.Columns.Day:
                                case GenericColumnItemInformation.Columns.Month:
                                case GenericColumnItemInformation.Columns.Year:
                                case GenericColumnItemInformation.Columns.MonthYear:
                                case GenericColumnItemInformation.Columns.DayName:
                                    
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        date = row[Column.DataBaseField].ToString();
                                        if (date.Length > 0)
                                            curCell.SetCellValue((object)new DateTime(int.Parse(date.Substring(0, 4)),
                                                int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))));
                                        else
                                            curCell.SetCellValue(null);
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    
                                    break;
                                case GenericColumnItemInformation.Columns.ProgramBeginningTime:
                                case GenericColumnItemInformation.Columns.ProgramEndingTime:
                                case GenericColumnItemInformation.Columns.CommercialBreakBeginningTime:
                                case GenericColumnItemInformation.Columns.CommercialBreakEndingTime:
                                    var s = row[Column.DataBaseField].ToString();
                                    if (!string.IsNullOrEmpty(s))
                                    {
                                        DateTime d = ToDateTime(row[Column.DataBaseField].ToString());
                                        tab[iCurLine, iCurColumn++] = new CellDate(d, string.Format("{{0:{0}}}", Column.StringFormat));
                                    }
                                    else
                                    {
                                        tab[iCurLine, iCurColumn++] = new CellLabel("");
                                    }
                                    break;
                              
                                case GenericColumnItemInformation.Columns.product:
                                    if (_showProduct && WebApplicationParameters.
                                        GenericColumnsInformation.IsVisible(_vehicleInformation.DetailColumnId, Column.Id))
                                    {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        curCell.SetCellValue( row[Column.DataBaseField]);
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.WeekNumber:
                                    var weekNb = row[Column.DataBaseField].ToString();
                                    if (string.IsNullOrEmpty(weekNb))
                                    {
                                        tab[iCurLine, iCurColumn++] = new CellLabel("");
                                    }
                                    else
                                    {
                                        tab[iCurLine, iCurColumn++] = new CellLabel(weekNb.Insert(4, " / "));
                                    }
                                    break;
                                case GenericColumnItemInformation.Columns.LanguageOfTheAd:
                                    var languageId = row[Column.DataBaseField].ToString();
                                    if (string.IsNullOrEmpty(languageId))
                                    {
                                        tab[iCurLine, iCurColumn++] = new CellLabel("");
                                    }
                                    else
                                    {
                                        tab[iCurLine, iCurColumn++] = new CellLabel(GetLanguageLabel(languageId));
                                    }
                                    break;
                                default:
                                    if (WebApplicationParameters.GenericColumnsInformation.IsVisible(_vehicleInformation.DetailColumnId, Column.Id))
                                    {
                                        type = assembly.GetType(Column.CellType);
                                        curCell = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                                        curCell.StringFormat = string.Format("{{0:{0}}}", Column.StringFormat);
                                        var columnValue = string.IsNullOrEmpty(Column.DataBaseAliasField)
                                            ? row[Column.DataBaseField]
                                            : row[Column.DataBaseAliasField];
                                        curCell.SetCellValue( columnValue);
                                        tab[iCurLine, iCurColumn++] = curCell;
                                    }
                                    break;
                            }
                        }
                        iCurLine++;
                    //}

                }
            }
            catch (System.Exception err)
            {
                throw (new NewCreativesException("Error while generating result table of new creative details", err));
            }
        }
        #endregion

        protected virtual int GetTvId()
        {
            return _vehicleInformation.Id.GetHashCode();
        }

        private DateTime ToDateTime(string datetime, char dateSpliter = '-', char timeSpliter = ':',
          char millisecondSpliter = ',')
        {
            datetime = datetime.Trim();
            datetime = datetime.Replace("  ", " ");
            string[] body = datetime.Split(' ');
            string[] date = body[0].Split(dateSpliter);
            int hour = 0, minute = 0, second = 0, millisecond = 0;
            if (body.Length == 2)
            {
                string[] tpart = body[1].Split(millisecondSpliter);
                string[] time = tpart[0].Split(timeSpliter);
                hour = Convert.ToInt32(time[0]);
                minute = Convert.ToInt32(time[1]);
                if (time.Length == 3) second = Convert.ToInt32(time[2]);
                if (tpart.Length == 2) millisecond = Convert.ToInt32(tpart[1]);
            }
            return new DateTime(1970, 1, 1, hour, minute, second, millisecond);
        }

        #region GetLanguageLabel
        private string GetLanguageLabel(string languageId)
        {
            switch (languageId)
            {
                case "90":
                    return GestionWeb.GetWebWord(3241, _webSession.SiteLanguage).ToUpper();
                case "44":
                    return GestionWeb.GetWebWord(2478, _webSession.SiteLanguage).ToUpper();
                default:
                    return "";
            }
        }
        #endregion

    }
}
