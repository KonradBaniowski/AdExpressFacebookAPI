#region Information
// Author: D. Mussuma
// Creation date: 11/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Result;
using System.Globalization;
using TNS.AdExpress.Constantes;
namespace TNS.AdExpressI.Portofolio.Russia.Engines {

	/// <summary>
	/// Compute portofolio calendar of advertising activity' results
	/// </summary>
	public class CalendarEngine : TNS.AdExpressI.Portofolio.Engines.CalendarEngine {

	    #region Constructor
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public CalendarEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
		}

		#endregion

        #region calendar of advertising activity
        /// <summary>
        /// Get result table of calendar of advertising activity 
        /// </summary>
        /// <returns>Result Table</returns>
        protected override ResultTable ComputeResultTable() {

            #region Variables
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].textWrap;
            Headers headers = null;
            CellUnitFactory cellFactory = null;
            List<Int32> parutions = new List<Int32>();
            InitLine initLine = null;
            SetLine setLine = null;
            DataTable dtTotal = null, dtLevel1 = null, dtLevel2 = null, dtLevel3 = null;
            DataSet ds = null;
            Dictionary<Int64, List<int>> level1ChildsIndex = null;
            Dictionary<string, List<int>> level2ChildsIndex = null;
            DataRow rowL2 = null, rowL3 = null;
            bool isNewL1 = false, isNewL2 = false;
            long idL1 = long.MinValue, oldIdL1 = long.MinValue, idL2 = long.MinValue, oldIdL2 = long.MinValue, idL3 = long.MinValue, oldIdL3 = long.MinValue, tempId = long.MinValue, tempIdL2 = long.MinValue;
            int levelNb = _webSession.GenericProductDetailLevel.GetNbLevels;
            List<int> tempList = null;
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the present absent result"));
            object[] parameters = new object[1];
            AdExpressCellLevel[] cellLevels;
            #endregion

            #region Load data from data layer
            if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            parameters = new object[5];
            parameters[0] = _webSession;
            parameters[1] = _vehicleInformation;
            parameters[2] = _idMedia;
            parameters[3] = _periodBeginning;
            parameters[4] = _periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = portofolioDAL.GetData();//GetDataCalendar

            if (ds == null || ds.Tables.Count == 0 || ds.Tables["total"].Rows.Count == 0) {
                return null;
            }

            //Get total data
            dtTotal = ds.Tables["total"];

            //Get level 1 data
            if (levelNb > 0) dtLevel1 = ds.Tables["level1"];



            //Get level 2 data
            if (levelNb > 1) {
                dtLevel2 = ds.Tables["level2"];

                //Build Level 2 Indexes
                if (dtLevel2 != null && dtLevel2.Rows.Count > 0) {
                    level1ChildsIndex = new Dictionary<long, List<int>>();
                    for (int rowIndex = 0; rowIndex < dtLevel2.Rows.Count; rowIndex++) {
                        tempId = _webSession.GenericProductDetailLevel.GetIdValue(dtLevel2.Rows[rowIndex], 1);
                        if (level1ChildsIndex.ContainsKey(tempId)) {
                            level1ChildsIndex[tempId].Add(rowIndex);
                        }
                        else {
                            tempList = new List<int>();
                            tempList.Add(rowIndex);
                            level1ChildsIndex.Add(tempId, tempList);
                        }
                    }
                }
            }

            //Get level 3 data
            if (levelNb > 2) {
                dtLevel3 = ds.Tables["level3"];
                if (dtLevel3 != null && dtLevel3.Rows.Count > 0) {
                    level2ChildsIndex = new Dictionary<string, List<int>>();
                    for (int rowIndex = 0; rowIndex < dtLevel3.Rows.Count; rowIndex++) {
                        tempId = _webSession.GenericProductDetailLevel.GetIdValue(dtLevel3.Rows[rowIndex], 1);
                        tempIdL2 = _webSession.GenericProductDetailLevel.GetIdValue(dtLevel3.Rows[rowIndex], 2);
                        string keyL3 = tempId.ToString() + "-" + tempIdL2.ToString();
                        if (level2ChildsIndex.ContainsKey(keyL3))
                            level2ChildsIndex[keyL3].Add(rowIndex);
                        else {
                            tempList = new List<int>();
                            tempList.Add(rowIndex);
                            level2ChildsIndex.Add(keyL3, tempList);
                        }
                    }
                }
            }
            #endregion

            #region Nombre de lignes du tableau du tableau
            Int32 nbline = 0;
            if (levelNb == 1) nbline += GetCalendarSize(dtLevel1, parutions);
            if (levelNb == 2) nbline += GetCalendarSize(dtLevel2, parutions);
            if (levelNb == 3) nbline += GetCalendarSize(dtLevel3, parutions);
            #endregion

            #region Get Headers
            GetCalendarHeaders(out headers, out cellFactory, parutions);
            #endregion

            #region Init ResultTable
            ResultTable tab = new ResultTable(nbline, headers);
            cellLevels = new AdExpressCellLevel[levelNb + 1];
            #endregion

            #region Initialisation du type de ligne
            if (_webSession.GetSelectedUnit().Id == WebCst.CustomerSessions.Unit.versionNb) {
                initLine = new InitLine(InitListLine);
                setLine = new SetLine(SetListLine);
            }
            else {
                initLine = new InitLine(InitDoubleLine);
                setLine = new SetLine(SetDoubleLine);
            }
            #endregion

            #region Fill result table

            Int32 cLine = 0;

            #region Intialisation des totaux
            cLine = tab.AddNewLine(LineType.total);
            tab[cLine, 1] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805, _webSession.SiteLanguage), 0, cLine, _webSession, textWrap.NbChar, textWrap.Offset);
            /* If the customer don't have the right to media schedule module, we don't show the MS column
             * */
            if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
                tab[cLine, GetColumnIndex(ColumnName.mediaSchedule)] = new CellMediaScheduleLink(cellLevels[0], _webSession);

            initLine(tab, cLine, cellFactory, cellLevels[0]);
            setLine(tab, cLine, dtTotal.Rows[0]);
            #endregion

            #region Set Level 1
            //Set Level 1
            if (levelNb > 0) {
                foreach (DataRow row in dtLevel1.Rows) {
                    idL1 = _webSession.GenericProductDetailLevel.GetIdValue(row, 1);
                    if (idL1 > long.MinValue && idL1 != oldIdL1) {
                        oldIdL1 = idL1;
                        isNewL1 = true;
                    }
                    cLine = tab.AddNewLine(LineType.level1);
                    tab[cLine, GetColumnIndex(ColumnName.product)] = cellLevels[1] = new AdExpressCellLevel(idL1, _webSession.GenericProductDetailLevel.GetLabelValue(row, 1), cellLevels[0], 1, cLine, _webSession, textWrap.NbChar, textWrap.Offset);
                   
                    /* If the customer don't have the right to media schedule module, we don't show the MS column*/
                    if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
                        tab[cLine, GetColumnIndex(ColumnName.mediaSchedule)] = new CellMediaScheduleLink(cellLevels[1], _webSession);
                    initLine(tab, cLine, cellFactory, cellLevels[0]);
                    setLine(tab, cLine, row);
                    oldIdL2 = long.MinValue;
                    idL2 = long.MinValue;

                    #region Set Level 2
                    //Set Level 2
                    if (levelNb > 1 && isNewL1) {
                        List<int> tempL1ChildsIndex = level1ChildsIndex[Convert.ToInt64(idL1)];

                        for (int j = 0; j < tempL1ChildsIndex.Count; j++) {
                            rowL2 = dtLevel2.Rows[tempL1ChildsIndex[j]];
                            idL2 = _webSession.GenericProductDetailLevel.GetIdValue(rowL2, 2);
                            if (oldIdL2 != idL2) {
                                isNewL2 = true;
                            }
                            cLine = tab.AddNewLine(LineType.level2);
                            tab[cLine, GetColumnIndex(ColumnName.product)] = cellLevels[2] = new AdExpressCellLevel(idL2, _webSession.GenericProductDetailLevel.GetLabelValue(rowL2, 2), cellLevels[1], 2, cLine, _webSession, textWrap.NbChar, textWrap.Offset);
                           
                            /* If the customer don't have the right to media schedule module, we don't show the MS column*/
                            if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
                                tab[cLine, GetColumnIndex(ColumnName.mediaSchedule)] = new CellMediaScheduleLink(cellLevels[2], _webSession);
                            initLine(tab, cLine, cellFactory, cellLevels[1]);
                            setLine(tab, cLine, rowL2);
                            oldIdL2 = idL2;
                            oldIdL3 = long.MinValue;
                            idL3 = long.MinValue;

                            #region Set Level 3
                            //Set Level 3
                            if (levelNb > 2 && isNewL2) {

                                List<int> tempL2ChildsIndex = level2ChildsIndex[idL1.ToString() + "-" + idL2.ToString()];

                                for (int k = 0; k < tempL2ChildsIndex.Count; k++) {
                                    rowL3 = dtLevel3.Rows[tempL2ChildsIndex[k]];
                                    idL3 = _webSession.GenericProductDetailLevel.GetIdValue(rowL3, 3);
                                    cLine = tab.AddNewLine(LineType.level3);
                                    tab[cLine, GetColumnIndex(ColumnName.product)] = cellLevels[3] = new AdExpressCellLevel(idL3, _webSession.GenericProductDetailLevel.GetLabelValue(rowL3, 3), cellLevels[2], 3, cLine, _webSession, textWrap.NbChar, textWrap.Offset);
                                    
                                    /* If the customer don't have the right to media schedule module, we don't show the MS column*/
                                    if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
                                        tab[cLine, GetColumnIndex(ColumnName.mediaSchedule)] = new CellMediaScheduleLink(cellLevels[3], _webSession);

                                    initLine(tab, cLine, cellFactory, cellLevels[2]);
                                    setLine(tab, cLine, rowL3);
                                }
                            }
                            #endregion

                            isNewL2 = false;
                        }
                    }
                    #endregion

                    isNewL1 = false;
                    isNewL2 = false;
                }
            }
            #endregion

            #endregion

            return tab;
        }
        #endregion

        #region GetCellPDM
        /// <summary>
        /// Get Cell PDM
        /// </summary>
        /// <param name="unitValue">Value</param>
        /// <param name="reference">Reference Value</param>
        /// <returns>Cell PDM object</returns>
        protected override CellUnit GetCellPDM(CellUnit reference) {
            CellRussiaPDM cellPdm = new CellRussiaPDM(null, reference);
            cellPdm.StringFormat = "{0:percentWOSign}";
            return cellPdm;
        }
        #endregion

        #region InitLine
        /// <summary>
        /// Init all value on a line with a double type
        /// </summary>
        /// <param name="oTab">Result Table</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="cellFactory">Current cell type</param>
        /// <param name="parent">Parent level</param>
        protected override void InitDoubleLine(ResultTable oTab, int cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent) {
            //total
            if (!_webSession.Percentage) oTab[cLine, GetColumnIndex(ColumnName.total)] = cellFactory.Get(null);
            else {
                oTab[cLine, GetColumnIndex(ColumnName.total)] = GetCellPDM(null);
            }

            //pourcentage
            if (parent == null) oTab[cLine, GetColumnIndex(ColumnName.percentage)] = new CellPercent(null, null);
            else oTab[cLine, GetColumnIndex(ColumnName.percentage)] = new CellPercent(null, (CellUnit)oTab[parent.LineIndexInResultTable, GetColumnIndex(ColumnName.percentage)]);

            ((CellPercent)oTab[cLine, GetColumnIndex(ColumnName.percentage)]).StringFormat = "{0:percentWOSign}";

            //initialisation des autres colonnes
            for (int j = GetColumnIndex(ColumnName.data); j < oTab.DataColumnsNumber + 1; j++) {
                if (!_webSession.Percentage) oTab[cLine, j] = cellFactory.Get(null);
                else {
                    oTab[cLine, j] = GetCellPDM((CellUnit)oTab[cLine, GetColumnIndex(ColumnName.total)]);
                }
            }
        }
        #endregion

        #region SetLine
        /// <summary>
        /// Set all value on a line with a double type
        /// </summary>
        /// <param name="oTab">Result table</param>
        /// <param name="cLine">Current line</param>
        /// <param name="row">Current data Row</param>
        protected override void SetDoubleLine(ResultTable oTab, int cLine, DataRow row)
        {
            if (row != null)
            {
                //double valu = Convert.ToDouble(row[_webSession.GetSelectedUnit().Id.ToString()]);

                double valu = 0;

                if (row[_webSession.GetSelectedUnit().Id.ToString()] != System.DBNull.Value)
                {

                    valu = Convert.ToDouble(row[_webSession.GetSelectedUnit().Id.ToString()]);
                    ((CellUnit)oTab[cLine, GetColumnIndex(ColumnName.percentage)]).Value = valu;
                    ((CellUnit)oTab[cLine, GetColumnIndex(ColumnName.total)]).Value = valu;
                }
                //Affect value


                for (int i = 1, j = GetColumnIndex(ColumnName.data); j < oTab.DataColumnsNumber + 1; j++, i++)
                {
                    if (row.Table.Columns.Contains("D" + i.ToString()))
                    {

                        if (row["D" + i.ToString()] != System.DBNull.Value)
                        {
                            ((CellUnit)oTab[cLine, j]).Value = Convert.ToDouble(row["D" + i.ToString()]);
                        }
                    }
                }
            }
        }
        #endregion

        #region Calendar headers
        /// <summary>
        /// Calendar Headers and Cell factory
        /// </summary>
        /// <returns></returns>
        protected override void GetCalendarHeaders(out Headers headers, out CellUnitFactory cellFactory, List<Int32> parutions)
        {
            headers = new Headers();
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL));
            /* If the customer don't have the right to media schedule module, we don't show the MS column
             * */
            if (_webSession.CustomerLogin.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null)
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(TOTAL_COL, _webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage), TOTAL_COL));
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(POURCENTAGE_COL, _webSession.SiteLanguage), POURCENTAGE_COL));

            //une colonne par date de parution
            parutions.Sort();
            foreach (Int32 parution in parutions)
            {
                headers.Root.Add(new Header(true, Dates.YYYYMMDDToDD_MM_YYYY(parution.ToString(), _webSession.SiteLanguage), (long)parution));
            }
            if (!_webSession.Percentage)
            {
                cellFactory = _webSession.GetCellUnitFactory();
            }
            else
            {

                cellFactory = new CellUnitFactory(GetCellPDM(null));
            }
        }
        #endregion

        #region GetCalendarSize
        /// <summary>
        /// Calcul la taille du tableau de résultats d'un calendrier d'action
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="dt">table de données</param>
        /// <returns>nombre de ligne du tableau de résultats</returns>
        /// <param name="parutions">Parutions</param>
        protected override int GetCalendarSize(DataTable dt, List<Int32> parutions) {

            Int32 nbLine = 0;
            Int64 oldIdL1 = long.MinValue;
            Int64 oldIdL2 = long.MinValue;
            Int64 oldIdL3 = long.MinValue;
            Int64 cIdL1 = long.MinValue;
            Int64 cIdL2 = long.MinValue;
            Int64 cIdL3 = long.MinValue;
            Int32 dateBegin = Int32.Parse(_periodBeginning);
            Int32 dateEnd = Int32.Parse(_periodEnd);


            //for(Int32 i=1, currentDay=dateBegin; currentDay<=dateEnd;currentDay++, i++){
            //    if (dt.Columns.Contains("D" + i.ToString()) && !parutions.Contains(currentDay))
            //        parutions.Add(currentDay);
            //}

            DateTime dateStart = TNS.FrameWork.Date.DateString.YYYYMMDDToDateTime(_periodBeginning);
            DateTime currentDate = dateStart;
            DateTime dateStop = TNS.FrameWork.Date.DateString.YYYYMMDDToDateTime(_periodEnd);
            TimeSpan span = dateStop.Subtract(dateStart);
            int nbDays = span.Days + 1;
            int currentDay = 0;

            for (int i = 1; i <= nbDays; i++)
            {
                currentDay = Int32.Parse(TNS.FrameWork.Date.DateString.DateTimeToYYYYMMDD(currentDate));
                if (dt.Columns.Contains("D" + i.ToString()) && !parutions.Contains(currentDay))
                    parutions.Add(currentDay);
                currentDate = currentDate.AddDays(1);
            }

            int nbLevel = _webSession.GenericProductDetailLevel.GetNbLevels;
            foreach (DataRow row in dt.Rows) {
                cIdL1 = (1 > nbLevel) ? long.MinValue : _webSession.GenericProductDetailLevel.GetIdValue(row, 1);
                cIdL2 = (2 > nbLevel) ? long.MinValue : _webSession.GenericProductDetailLevel.GetIdValue(row, 2);
                cIdL3 = (3 > nbLevel) ? long.MinValue : _webSession.GenericProductDetailLevel.GetIdValue(row, 3);
                if (cIdL1 > long.MinValue && cIdL1 != oldIdL1) {
                    oldIdL1 = cIdL1;
                    oldIdL2 = oldIdL3 = long.MinValue;
                    nbLine++;
                }
                if (cIdL2 > long.MinValue && cIdL2 != oldIdL2) {
                    oldIdL2 = cIdL2;
                    oldIdL3 = long.MinValue;
                    nbLine++;
                }
                if (cIdL3 > long.MinValue && cIdL3 != oldIdL3) {
                    oldIdL3 = cIdL3;
                    nbLine++;
                }
            }
            return nbLine;
        }
        #endregion

    }
}
