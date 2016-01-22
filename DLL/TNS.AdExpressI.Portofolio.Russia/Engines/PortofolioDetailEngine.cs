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
using TNS.AdExpress.Web.Core.Result;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.FrameWork.Collections;
using TNS.AdExpress.Constantes;

namespace TNS.AdExpressI.Portofolio.Russia.Engines {
	/// <summary>
	/// Compute portofolio detail's results
	/// </summary>
	public class PortofolioDetailEngine : TNS.AdExpressI.Portofolio.Engines.PortofolioDetailEngine {

        #region Constructor
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public PortofolioDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool showInsertions, bool showCreatives)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, showInsertions, showCreatives) {
		}

		#endregion

        #region ComputeResultTable
        /// <summary>
        /// Get Result Table for portofolio detail
        /// </summary>
        /// <returns>ResultTable</returns>
        protected override ResultTable ComputeResultTable() {

            #region Variables
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].textWrap;
            CellUnitFactory[] cellFactories = null;
            AffectLine[] lineDelegates = null;
            string[] columnsName = null;
            Headers headers = null;
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
            int insertions = 0;
            int creatives = 0;
            AdExpressCellLevel[] cellLevels;
            #endregion
            
            #region Load data from data layer
            ds = GetDataForResultTable();

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

            #region Get Headers
            if (_showInsertions) insertions = 1;
            if (_showCreatives) creatives = 1;

            GetPortofolioHeaders(out headers, out cellFactories, out lineDelegates, out columnsName);
            #endregion

            #region Init ResultTable
            Int32 nbline = 0;
            if (levelNb == 1) nbline += GetPortofolioSize(dtLevel1);
            if (levelNb == 2) nbline += GetPortofolioSize(dtLevel2);
            if (levelNb == 3) nbline += GetPortofolioSize(dtLevel3);
            ResultTable tab = new ResultTable(nbline, headers);
            cellLevels = new AdExpressCellLevel[levelNb + 1];
            #endregion

            #region Fill result table

            Int32 cLine = 0;

            #region Intialisation des totaux
            cLine = tab.AddNewLine(LineType.total);
            tab[cLine, 1] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805, _webSession.SiteLanguage), 0, cLine, _webSession, textWrap.NbChar,textWrap.Offset);
            //Creatives
            if (_showCreatives) tab[cLine, 1 + creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
            if (_showInsertions) tab[cLine, 1 + creatives + insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
            if (_showMediaSchedule) tab[cLine, 2 + creatives + insertions] = new CellMediaScheduleLink(cellLevels[0], _webSession);

            AffectPortefolioLine(cellFactories, lineDelegates, columnsName, dtTotal.Rows[0], tab, cLine, true);
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
                    tab[cLine, 1] = cellLevels[1] = new AdExpressCellLevel(idL1, _webSession.GenericProductDetailLevel.GetLabelValue(row, 1), cellLevels[0], 1, cLine, _webSession, textWrap.NbChar, textWrap.Offset);
                   
                    //Creatives
                    if (creatives > 0) tab[cLine, 1 + creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
                    if (insertions > 0) tab[cLine, 1 + creatives + insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
                    if (_showMediaSchedule) tab[cLine, 2 + creatives + insertions] = new CellMediaScheduleLink((AdExpressCellLevel)tab[cLine, 1], _webSession);
                    AffectPortefolioLine(cellFactories, lineDelegates, columnsName, row, tab, cLine, true);
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
                            tab[cLine, 1] = cellLevels[2] = new AdExpressCellLevel(idL2, _webSession.GenericProductDetailLevel.GetLabelValue(rowL2, 2), cellLevels[1], 2, cLine, _webSession, textWrap.NbChar, textWrap.Offset);
                           
                            //Creatives
                            if (creatives > 0) tab[cLine, 1 + creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
                            if (insertions > 0) tab[cLine, 1 + creatives + insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
                            if (_showMediaSchedule) tab[cLine, 2 + creatives + insertions] = new CellMediaScheduleLink((AdExpressCellLevel)tab[cLine, 1], _webSession);
                            AffectPortefolioLine(cellFactories, lineDelegates, columnsName, rowL2, tab, cLine, true);
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
                                    tab[cLine, 1] = cellLevels[3] = new AdExpressCellLevel(idL3, _webSession.GenericProductDetailLevel.GetLabelValue(rowL3, 3), cellLevels[2], 3, cLine, _webSession, textWrap.NbChar, textWrap.Offset);
                                    
                                    //Creatives
                                    if (creatives > 0) tab[cLine, 1 + creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
                                    if (insertions > 0) tab[cLine, 1 + creatives + insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[cLine, 1], _webSession, _webSession.GenericProductDetailLevel);
                                    if (_showMediaSchedule) tab[cLine, 2 + creatives + insertions] = new CellMediaScheduleLink((AdExpressCellLevel)tab[cLine, 1], _webSession);

                                    AffectPortefolioLine(cellFactories, lineDelegates, columnsName, rowL3, tab, cLine, true);
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

        #region Affect Portefolio Line
        /// <summary>
        /// Affect Line unit of double type in a specific cell
        /// </summary>
        /// <param name="oTab">Result Table</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="cCol">Current Column</param>
        /// <param name="cellFactory">Current Cell</param>
        /// <param name="value">Value to assign</param>
        /// <param name="isLeaf"></param>
        protected override void AffectDoubleLine(ResultTable oTab, int cLine, int cCol, CellUnitFactory cellFactory, object value, bool isLeaf) {
            if (oTab[cLine, cCol] == null) {
                oTab[cLine, cCol] = cellFactory.Get(null);
            }
            if (value != null && value != System.DBNull.Value)
            {
                //Affect value
                if (isLeaf) {
                    oTab[cLine, cCol] = cellFactory.Get(Convert.ToDouble(value));
                }
            }
        }
        /// <summary>
        /// Affect Line unit of object List type in a specific cell
        /// </summary>
        /// <param name="oTab">Result Table</param>
        /// <param name="cLine">Current Line</param>
        /// <param name="cCol">Current Column</param>
        /// <param name="cellFactory">Current Cell</param>
        /// <param name="value">Value to assign</param>
        /// <param name="isLeaf"></param>
        protected override void AffectListLine(ResultTable oTab, int cLine, int cCol, CellUnitFactory cellFactory, object value, bool isLeaf) {
            if (oTab[cLine, cCol] == null) {
                oTab[cLine, cCol] = cellFactory.Get(null);
            }
            if (value != null && value != System.DBNull.Value && isLeaf)
            {
                //Get values
                string[] tIds = value.ToString().Split(',');
                //Affect value
                for (int i = 0; i < tIds.Length; i++) {
                    oTab[cLine, cCol] = cellFactory.Get(Convert.ToInt64(tIds[i]));
                }
            }
        }
        #endregion

        #region Compute line numbers in result table from preformated data table
        /// <summary>
        /// Get the number of line from the database data
        /// </summary>
        /// <param name="tabData">Data</param>
        /// <returns>Number of lines</returns>
        protected override Int32 GetPortofolioSize(DataTable dt) {

            Int32 nbLine = 0;
            Int64 oldIdL1 = long.MinValue;
            Int64 oldIdL2 = long.MinValue;
            Int64 oldIdL3 = long.MinValue;
            Int64 cIdL1 = long.MinValue;
            Int64 cIdL2 = long.MinValue;
            Int64 cIdL3 = long.MinValue;

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

        #region Portofolio headers
        /// <summary>
        /// Portofolio Headers and Cell factory
        /// </summary>
        /// <returns></returns>
        protected override void GetPortofolioHeaders(out Headers headers, out CellUnitFactory[] cellFactories, out AffectLine[] lineDelegates, out string[] columnsName)
        {
            TextWrap textWrap = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].textWrap;
            int insertions = 0;
            int creatives = 0;
            int mediaSchedule = 0;
            int iNbCol = 1;
            int columnIndex = (_showMediaSchedule) ? 2 : 1;
            System.Reflection.Assembly assembly = null;// System.Reflection.Assembly.Load(@"TNS.FrameWork.WebResultUI");
            Type type;
            Cell cellUnit;

            headers = new TNS.FrameWork.WebResultUI.Headers();
            // Product column
            headers.Root.Add(new Header(true, GestionWeb.GetWebWord(PROD_COL, _webSession.SiteLanguage), PROD_COL, textWrap.NbChar, textWrap.Offset));
            // Creatives column     
            if (_showCreatives)
            {
                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(CREATIVES_COL, _webSession.SiteLanguage), CREATIVES_COL));
                creatives = 1;
            }
            // Insertions column
            if (_showInsertions)
            {
                headers.Root.Add(new HeaderCreative(false, GestionWeb.GetWebWord(INSERTIONS_LIST_COL, _webSession.SiteLanguage), INSERTIONS_LIST_COL));
                insertions = 1;
            }
            if (_showMediaSchedule)
            {
                // Media schedule column
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
                mediaSchedule = 1;
            }
            List<UnitInformation> unitInformationList = _webSession.GetValidUnitForResult();
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvGeneral:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvSponsorship:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvNonTerrestrials:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvAnnounces:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioGeneral:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioSponsorship:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioMusic:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.cinema:
                    iNbCol += creatives + insertions + mediaSchedule + unitInformationList.Count;
                    break;                                                                                                            
                default:
                    throw new PortofolioException("Vehicle unknown.");
            }
            cellFactories = new CellUnitFactory[iNbCol];
            lineDelegates = new AffectLine[iNbCol];
            columnsName = new string[iNbCol];
            cellFactories[0] = null;
            cellFactories[1] = null;
            if (_showCreatives) columnsName[1 + creatives] = null;
            if (_showInsertions) columnsName[1 + creatives + insertions] = null;

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioGeneral:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioSponsorship:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radioMusic:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvGeneral:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvSponsorship:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvNonTerrestrials:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tvAnnounces:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.cinema:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                   
                    foreach (UnitInformation currentUnit in unitInformationList)
                    {
                        headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true, GestionWeb.GetWebWord(currentUnit.WebTextId, _webSession.SiteLanguage), currentUnit.WebTextId, WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].textWrap.NbCharHeader, WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].textWrap.Offset));
                        assembly = System.Reflection.Assembly.Load(currentUnit.Assembly);
                        type = assembly.GetType(currentUnit.CellType);
                        cellUnit = (Cell)type.InvokeMember("GetInstance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, null, null);
                        cellUnit.StringFormat = currentUnit.StringFormat;
                        columnsName[columnIndex + creatives + insertions] = currentUnit.Id.ToString();
                        cellFactories[columnIndex + creatives + insertions] = new CellUnitFactory((CellUnit)cellUnit);
                        if (cellUnit is CellIdsNumber)
                        {
                            lineDelegates[columnIndex + creatives + insertions] = new AffectLine(AffectListLine);
                        }
                        else
                        {
                            lineDelegates[columnIndex + creatives + insertions] = new AffectLine(AffectDoubleLine);
                        }

                        columnIndex++;
                    }
                    break;

                default:
                    throw new PortofolioException("Vehicle unknown.");
            }

        }
        #endregion
	}
}
