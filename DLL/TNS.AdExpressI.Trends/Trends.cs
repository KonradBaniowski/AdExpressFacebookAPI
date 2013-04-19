#region Information
/*
 * Author : D Mussuma
 * Creation : 20/11/2009
 * Updates :
 *      Author - Date - Description
 * 
 */
#endregion
using System;
using System.Reflection;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.WebResultUI;
using System.Data;
using TNS.AdExpressI.Trends.DAL;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.FrameWork.Date;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNSExceptions = TNS.AdExpress.Domain.Exceptions;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Trends
{
    public class Trends : ITrends
    {
        #region Constantes
        /// <summary>
        /// Level Column Id (media)
        /// </summary>
        protected const int LEVEL_HEADER_ID = 0;
        /// <summary>
        /// Group ID Beginning
        /// </summary>
        protected const int START_ID_GROUP = 1;

        protected const string EVOL = "_evol";
        protected const string PREV = "_prev";
        protected const string CUR = "_cur";
        protected const string SUB = "SUB_";
        #endregion

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;

        /// <summary>
        /// Current Module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
        /// <summary>
        /// List of valid units
        /// </summary>
        List<UnitInformation> _units = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public Trends(WebSession session)
        {
            _session = session;

            _module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);

            #region Selection of the media type
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new TNSExceptions.VehicleException("Selection of media type is not correct"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion

            // Get valid units that AdExpress has to display in the trend report according to the media
           _units = _session.GetValidUnitForResult();

        }
        #endregion

        #region IResults Members
        /// <summary>
        /// Gte tendencies result table
        /// </summary>
        /// <returns>Result table</returns>
        public virtual ResultTable GetResult()
        {           
            #region Load data from data layer
           
            DataSet ds = null;
            DataTable dtTotal = null;
            DataTable dtLevels = null;
            Dictionary<string, HeaderBase> elementsHeader = null;
            int cLine = 0;
            Type type;
            Cell cellUnit = null;
            CellUnitFactory[] cellUnitFactories = null;
            int indexInResultTable = -1;
            int level = -1;
            long idL2 = -1, oldIdL2 = -1 ,  idL3=-1;

            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the Trends result"));
            object[] parameters = new object[1];
            parameters[0] = _session;
            ITrendsDAL trendsDAL = (ITrendsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            ds = trendsDAL.GetData();
            int i=0;
            if (ds.Tables["TOTAL"] != null) dtTotal = ds.Tables["TOTAL"];
            if (ds.Tables["Levels"] != null) dtLevels = ds.Tables["Levels"];
            //No data
            if (dtTotal == null || dtTotal.Rows.Count == 0)
            {              
                return null;
            }
            #endregion

            #region Get Headers          
            Headers headers = GetHeaders(dtLevels, out elementsHeader);
            #endregion

            #region Init ResultTable
            Int32 nbline = dtLevels.Rows.Count + dtTotal.Rows.Count;
            ResultTable tab = new ResultTable(nbline, headers);
            // assembly loading
            System.Reflection.Assembly assembly = Assembly.Load(@"TNS.FrameWork.WebResultUI");
            cellUnitFactories = new CellUnitFactory[_units.Count];
             TNS.AdExpress.Domain.Level.GenericDetailLevel levelRequested=_session.DetailLevel;
             HeaderBase cHeader = null;
            #endregion

            #region Fill result table

             #region Total == Level 1
             if (!_session.PDM)
             {
                 CellLevel cLevelTotal = new CellLevel(-1, GestionWeb.GetWebWord(805, _session.SiteLanguage), 0, cLine);
                 cLine = tab.AddNewLine(LineType.total);
                 //Total label
                 tab[cLine, 1] = cLevelTotal;
                 foreach (UnitInformation currentUnit in _units)
                 {
                     type = assembly.GetType(currentUnit.CellType);
                     cellUnit = (Cell)type.InvokeMember("GetInstance", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, null, null, null);
                     cellUnit.StringFormat = currentUnit.StringFormat;
                     cellUnitFactories[i] = new CellUnitFactory((CellUnit)cellUnit);

                     //Current year column
                     cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + CUR];
                     if (_session.PDM) {
                         tab[cLine, cHeader.IndexInResultTable] = new CellPDM(Convert.ToDouble(dtTotal.Rows[0][currentUnit.DatabaseTrendsField + CUR].ToString()));
                         ((CellPDM)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                     }
                     else
                         tab[cLine, cHeader.IndexInResultTable] = cellUnitFactories[i].Get(Convert.ToDouble(dtTotal.Rows[0][currentUnit.DatabaseTrendsField + CUR].ToString()));

                     //Previous year column
                     cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + PREV];
                     if (_session.PDM) {
                         tab[cLine, cHeader.IndexInResultTable] = new CellPDM(Convert.ToDouble(dtTotal.Rows[0][currentUnit.DatabaseTrendsField + PREV].ToString()));
                         ((CellPDM)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                     }
                     else
                         tab[cLine, cHeader.IndexInResultTable] = cellUnitFactories[i].Get(Convert.ToDouble(dtTotal.Rows[0][currentUnit.DatabaseTrendsField + PREV].ToString()));

                     //Evolution column 
                     cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + EVOL];
                     tab[cLine, cHeader.IndexInResultTable] = new CellEvol(Convert.ToDouble(dtTotal.Rows[0][currentUnit.DatabaseTrendsField + EVOL].ToString()));
                     ((CellEvol)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                     i++;
                 }
             }
            i = 0;
            #endregion

            foreach (DataRow row in dtLevels.Rows)
            {
                #region Level 2
                level = 2;
                idL2 = _session.DetailLevel.GetIdValue(row, level);
                if (idL2 != oldIdL2)
                {
                    cLine = tab.AddNewLine(LineType.level1);                                      
                    tab[cLine, 1] = new CellLevel(
                     idL2
                      , _session.DetailLevel.GetLabelValue(row, level)
                      , null
                      , level
                      , cLine);
                    foreach (UnitInformation currentUnit in _units)
                    {
                        //Current year column
                        cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + CUR];
                        if (_session.PDM) {
                            tab[cLine, cHeader.IndexInResultTable] = new CellPDM(Convert.ToDouble(row[SUB + currentUnit.DatabaseTrendsField + CUR].ToString()));
                            ((CellPDM)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                        }
                        else
                            tab[cLine, cHeader.IndexInResultTable] = cellUnitFactories[i].Get(Convert.ToDouble(row[SUB + currentUnit.DatabaseTrendsField + CUR].ToString()));

                        //Previous year column
                        cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + PREV];
                        if (_session.PDM) {
                            tab[cLine, cHeader.IndexInResultTable] = new CellPDM(Convert.ToDouble(row[SUB + currentUnit.DatabaseTrendsField + PREV].ToString()));
                            ((CellPDM)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                        }
                        else
                            tab[cLine, cHeader.IndexInResultTable] = cellUnitFactories[i].Get(Convert.ToDouble(row[SUB + currentUnit.DatabaseTrendsField + PREV].ToString()));

                        //Evolution column 
                        cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + EVOL];
                       tab[cLine, cHeader.IndexInResultTable] = new CellEvol(Convert.ToDouble(row[SUB + currentUnit.DatabaseTrendsField + EVOL].ToString()));
                       ((CellEvol)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                        i++;
                    }
                }
                i = 0;
                #endregion

                #region Level 3
              
                if (_session.DetailLevel.GetNbLevels > 2)
                {
                    cLine = tab.AddNewLine(LineType.level2);
                    level = 3;
                    idL3 = _session.DetailLevel.GetIdValue(row, level);
                    tab[cLine, 1] = new CellLevel(
                      idL3
                      , _session.DetailLevel.GetLabelValue(row, level)
                      , null
                      , level
                      , cLine);
                    foreach (UnitInformation currentUnit in _units)
                    {
                        //Current year column
                        cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + CUR];
                        if (_session.PDM) {
                            tab[cLine, cHeader.IndexInResultTable] = new CellPDM(Convert.ToDouble(row[currentUnit.DatabaseTrendsField + CUR].ToString()));
                            ((CellPDM)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                        }
                        else
                            tab[cLine, cHeader.IndexInResultTable] = cellUnitFactories[i].Get(Convert.ToDouble(row[currentUnit.DatabaseTrendsField + CUR].ToString()));

                        //Previous year column
                        cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + PREV];
                        if (_session.PDM) {
                            tab[cLine, cHeader.IndexInResultTable] = new CellPDM(Convert.ToDouble(row[currentUnit.DatabaseTrendsField + PREV].ToString()));
                            ((CellPDM)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                        }
                        else
                            tab[cLine, cHeader.IndexInResultTable] = cellUnitFactories[i].Get(Convert.ToDouble(row[currentUnit.DatabaseTrendsField + PREV].ToString()));

                        //Evolution column 
                        cHeader = elementsHeader[currentUnit.Id.GetHashCode().ToString() + EVOL];
                        tab[cLine, cHeader.IndexInResultTable] = new CellEvol(Convert.ToDouble(row[currentUnit.DatabaseTrendsField + EVOL].ToString()));
                        ((CellEvol)tab[cLine, cHeader.IndexInResultTable]).StringFormat = "{0:percentWOSign}";
                        i++;
                    }
                }            
                i = 0;
                #endregion

                oldIdL2 = idL2;
            }
            #endregion

            return tab;

        }
        #endregion

         /// <summary>
        /// Init headers
        /// </summary>
        /// <param name="elementsHeaders">(ou) Header for each level element</param>
        /// <param name="dtMedia">List of medias with the detail level matching
        protected virtual Headers GetHeaders(DataTable dt, out Dictionary<string, HeaderBase> elementsHeader)
        {
            #region Build headers
            // Media classification column
            Headers headers = new Headers();
            Header headerTmp = null;
            HeaderGroup headerGroupTmp = null;
            long code = -1;
            int iUnits = 1;
            string yearCur = "", yearPrev = "";          
            long id = 1;            
            elementsHeader = new Dictionary<string, HeaderBase>();
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                    //Title label
                   code = 1504;
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.indoor:
                    //Vehicle label
                    code = 804;
                    break;
                default:
                    break;
            }
            if (_session.PeriodType ==  WebConstantes.CustomerSessions.Period.Type.cumlDate)
            {               
                string period = dt.Rows[0]["date_period"].ToString();
                if(WebApplicationParameters.CountryCode.Trim().Equals("35")){
                    //Particular case for Finland (no date in week)
                    string daysInMonth = DateTime.DaysInMonth(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2))).ToString();
                    yearCur = period.Substring(0, 4);
                    yearPrev = (int.Parse(yearCur) - 1).ToString();
                    _session.PeriodEndDate = period + daysInMonth;
                    _session.PeriodBeginningDate = yearCur + "0101";
                }else{
                TNS.FrameWork.Date.AtomicPeriodWeek weekCurrent = new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)));
                yearCur = weekCurrent.FirstDay.Year.ToString();
                yearPrev = (int.Parse(yearCur) - 1).ToString();
                _session.PeriodEndDate = weekCurrent.LastDay.ToString("yyyyMMdd");
                _session.PeriodBeginningDate = yearCur + "0101";
                }
                _session.Save();                         
            }
            else
            {
                yearCur = _session.PeriodBeginningDate.Substring(0, 4);
                int tempY = int.Parse(yearCur) - 1;
                yearPrev = tempY.ToString();		
            }
            Header headerTotal = new Header(true, GestionWeb.GetWebWord(code, _session.SiteLanguage), LEVEL_HEADER_ID);
            headers.Root.Add(headerTotal);
            elementsHeader.Add(LEVEL_HEADER_ID.ToString(), headerTotal);

            //Add each unit Group headers
         
            foreach (UnitInformation currentUnit in _units)
            {
                headerGroupTmp = new HeaderGroup(GestionWeb.GetWebWord(currentUnit.WebTextId, _session.SiteLanguage), false, START_ID_GROUP + iUnits);
                List<Header> heads = new List<Header>();
                //Current year column
                headerTmp = new Header(true, yearCur, id);
                elementsHeader.Add(currentUnit.Id.GetHashCode().ToString() + CUR, headerTmp);
                heads.Add(headerTmp);
                //Previous year column
                id++;
                headerTmp = new Header(true, yearPrev, id);
                elementsHeader.Add(currentUnit.Id.GetHashCode().ToString() + PREV, headerTmp);
                heads.Add(headerTmp);
                //Evolution column
                id++;
                headerTmp = new Header(true, GestionWeb.GetWebWord(1212, _session.SiteLanguage), id);
                elementsHeader.Add(currentUnit.Id.GetHashCode().ToString() + EVOL, headerTmp);
                heads.Add(headerTmp);
                //heads.Sort(delegate(Header h1, Header h2) { return h1.Label.CompareTo(h2.Label); });
                foreach (Header h in heads)
                {
                    headerGroupTmp.Add(h);
                }
                headers.Root.Add(headerGroupTmp);
                iUnits++;
                id++;
            }
            #endregion

            return headers;
        }
    }
}
