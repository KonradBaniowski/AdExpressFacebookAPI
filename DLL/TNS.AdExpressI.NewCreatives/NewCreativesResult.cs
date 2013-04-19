#region Information
/*
 * Author : B.Masson
 * Creation : 29/09/2008
 * Updates :
 *      Date        Author      Description
 */
#endregion

#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using DALClassif = TNS.AdExpress.DataAccess.Classification;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using Navigation = TNS.AdExpress.Domain.Web.Navigation;

using TNS.AdExpress;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Translation;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpressI.NewCreatives.DAL;
using TNS.AdExpressI.NewCreatives.Exceptions;

using TNS.FrameWork.Date;
using TNS.FrameWork.Collections;
using TNS.FrameWork.WebResultUI;

using TNS.Classification.Universe;
using WebCst = TNS.AdExpress.Constantes.Web;
#endregion

namespace TNS.AdExpressI.NewCreatives {
    
    /// <summary>
    /// Default new creatives
    /// </summary>
    public abstract class NewCreativesResult:INewCreativesResult {

        #region Constantes
        protected const int PROD_COL = 1164;
        protected const int TOTAL_COL = 1401;
        protected const int POURCENTAGE_COL = 1236;
        protected const int VERSION_COL = 1994;
        protected const int PM_COL = 751;
        #endregion

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Current vehicle univers
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Navigation.Module _module;
        /// <summary>
        /// Sector IDs
        /// </summary>
        protected string _idSectors = "";
        /// <summary>
        /// Begining Date
        /// </summary>
        protected string _beginingDate;
        /// <summary>
        /// End Date
        /// </summary>
        protected string _endDate;
        /// <summary>
        /// Show creative column
        /// </summary>
        protected bool _showCreative = false;
        /// <summary>
        /// Define if show media schedule Link
        /// </summary>
        protected bool _showMediaSchedule = false;
        #endregion

        #region Accessors
        /// <summary>
        /// Get User session
        /// </summary>
        public WebSession WebSession {
            get { return _webSession; }
        }
        /// <summary>
        /// Get Current Vehicle
        /// </summary>
        public VehicleInformation VehicleInformationObject {
            get { return _vehicleInformation; }
        }
        /// <summary>
        /// Get Current Module
        /// </summary>
        public Navigation.Module CurrentModule {
            get { return _module; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">user session</param>
        public NewCreativesResult(WebSession session) {
            _webSession = session;
            _idSectors = GetSectorIds();
            _beginingDate = session.PeriodBeginningDate;
            _endDate = session.PeriodEndDate;
            _module = Navigation.ModulesList.GetModule(session.CurrentModule);

            #region Sélection du vehicle
            string vehicleSelection = session.GetSelection(session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if(vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new NewCreativesException("Uncorrect Media Selection"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion

            _showMediaSchedule = session.CustomerLogin.GetModule(AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) != null;
        }
        #endregion

        #region GetData
        /// <summary>
        /// Compute new creatives
        /// </summary>
        /// <returns>Compute Data</returns>
        public ResultTable GetData(){

            #region Variables
            ResultTable tab = null;
            DataSet ds = null;
            DataTable dt = null;
            CellUnitFactory cellFactory = null;
            AdExpressCellLevel[] cellLevels;
            var lineTypes = new LineType[4] { LineType.total, LineType.level1, LineType.level2, LineType.level3 };
            Headers headers = null;
            Int32 iCurLine = 0;
            Int32 iNbLine = 0;
            Int32 iNbLevels = 0;
            var parutions = new ArrayList();

            InitLine initLine = null;
            SetLine setLine = null;
            #endregion

            #region Chargement des données
            if(_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[4];
            parameters[0] = _webSession;
            parameters[1] = _idSectors;
            parameters[2] = _beginingDate;
            parameters[3] = _endDate;
            INewCreativeResultDAL newCreativesDAL = (INewCreativeResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = newCreativesDAL.GetData();

            if(ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) dt = ds.Tables[0];
            else return (tab);
            #endregion

            #region Nombre de lignes du tableau du tableau
            iNbLine = GetCalendarSize(dt, parutions);
            #endregion

            #region Headers
            GetCalendarHeaders(out headers, out cellFactory, parutions);
            #endregion

            #region Initialisation du tableau de résultats
            tab = new ResultTable(iNbLine, headers);
            #endregion

            #region Initialisation du type de ligne
            initLine = InitListLine;
            setLine = SetListLine;
            #endregion

            #region Traitement du tableau de résultats
            int i = 1;

            #region Intialisation des totaux
            iNbLevels = _webSession.GenericProductDetailLevel.GetNbLevels;
            cellLevels = new AdExpressCellLevel[iNbLevels + 1];
            tab.AddNewLine(LineType.total);
            tab[iCurLine, 1] = cellLevels[0] = new AdExpressCellLevel(0, GestionWeb.GetWebWord(805, _webSession.SiteLanguage), 0, iCurLine, _webSession);
            int iCol = 3;
            if (_showCreative) {
                iCol++;                
                SetCellCreativesLink(tab, iCurLine, iCol, cellLevels, 0);
            }
            if (_showMediaSchedule) { 
                iCol++; 
                tab[iCurLine, iCol] = new CellMediaScheduleLink(cellLevels[0], _webSession); 
            }
            initLine(tab, iCurLine, cellFactory, cellLevels[0]);
            #endregion

            i = 1;
            long dCurLevel = 0;
            DetailLevelItemInformation.Levels level;
            foreach(DataRow row in dt.Rows) {
                //pour chaque niveau
                for(i = 1; i <= iNbLevels; i++) {
                    //nouveau niveau i
                    dCurLevel = _webSession.GenericProductDetailLevel.GetIdValue(row, i);
                    if(dCurLevel >= 0 && (cellLevels[i] == null || dCurLevel != cellLevels[i].Id)) {
                        for(int j = i + 1; j < cellLevels.Length; j++) {
                            cellLevels[j] = null;
                        }
                        iCurLine = tab.AddNewLine(lineTypes[i]);
                        tab[iCurLine, 1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel, _webSession.GenericProductDetailLevel.GetLabelValue(row, i), cellLevels[i - 1], i, iCurLine, _webSession);
                        if(_webSession.GenericProductDetailLevel.DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels.advertiser) == i) {
                            if(row["id_address"] != DBNull.Value) {
                                cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
                            }
                        }
                        level = _webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(i);

                        // version
                        iCol = 3;
                        if(_showCreative){
                            iCol++;                           
                            SetCellCreativesLink(tab, iCurLine, iCol, cellLevels, i);
                        }
                        if (_showMediaSchedule) {
                            iCol++;
                            tab[iCurLine, iCol] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine, 1], _webSession);
                        }

                        initLine(tab, iCurLine, cellFactory, cellLevels[i - 1]);
                    }
                }
                setLine(tab, iCurLine, row);
            }
            #endregion

            return tab;
        }
        #endregion

        #region InitLine
        protected delegate void InitLine(ResultTable oTab, Int32 cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent);
        protected void InitListLine(ResultTable oTab, Int32 cLine, CellUnitFactory cellFactory, AdExpressCellLevel parent)
        {
            int i = 2;
            //total
            oTab[cLine, i] = cellFactory.Get(0.0);

            //pourcentage
            i++;
            if(parent == null) 
                oTab[cLine, i] = new CellVersionNbPDM(null);
            else 
                oTab[cLine, i] = new CellVersionNbPDM((CellIdsNumber)oTab[parent.LineIndexInResultTable, 2]);
            i++;
            if (_showCreative) i++;
            if (_showMediaSchedule) i++;
            //initialisation des autres colonnes
            for(int j = i; j < oTab.DataColumnsNumber + 1; j++) {
                oTab[cLine, j] = cellFactory.Get(0.0);
            }
        }
        #endregion

        #region SetLine
        protected delegate void SetLine(ResultTable oTab, Int32 iLineIndex, DataRow dr);
        protected void SetListLine(ResultTable oTab, Int32 cLine, DataRow row)
        {
            if(row != null) {
                Int32 lCol = oTab.GetHeadersIndexInResultTable(row["date_creation"].ToString());
                //Get values
                string[] tIds = row[_webSession.GetSelectedUnit().Id.ToString()].ToString().Split(',');
                //Affect value
                for(int i = 0; i < tIds.Length; i++) {
                    oTab.AffectValueAndAddToHierarchy(1, cLine, lCol, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 3, Convert.ToInt64(tIds[i]));
                    oTab.AffectValueAndAddToHierarchy(1, cLine, 2, Convert.ToInt64(tIds[i]));
                }
            }
        }
        #endregion

        #region Protected method
        /// <summary>
        /// Get sector ID
        /// </summary>
        /// <returns>Sector ID value</returns>
        protected virtual string GetSectorIds() {
        
            if(_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
            {
                NomenclatureElementsGroup nomenclatureElementsGroup = _webSession.PrincipalProductUniverses[0].GetGroup(0);
                if(nomenclatureElementsGroup != null) {
                    return nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.SECTOR);                   
                }
            }
            return ("");
        }
        #endregion

        #region GetCalendarSize
        /// <summary>
        /// Calcul la taille du tableau de résultats des nouvelles créations
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <param name="dt">table de données</param>
        /// <returns>nombre de ligne du tableau de résultats</returns>
        /// <param name="parutions">Parutions</param>
        protected virtual int GetCalendarSize(DataTable dt, ArrayList parutions) {

            #region Variable
            Int64 OldL1Id = -1;
            Int64 cL1Id = 0;
            Int64 nbL1Id = 0;
            Int64 OldL2Id = -1;
            Int64 cL2Id = 0;
            Int64 nbL2Id = 0;
            Int64 OldL3Id = -1;
            Int64 cL3Id = 0;
            Int64 nbL3Id = 0;
            Int64 nbLine = 0;
            int dateTmp = -1;
            #endregion

            foreach(DataRow dr in dt.Rows) {
                cL1Id = _webSession.GenericProductDetailLevel.GetIdValue(dr, 1);
                if(cL1Id >= 0 && cL1Id != OldL1Id) {
                    nbL1Id++;
                    OldL1Id = cL1Id;
                    OldL2Id = OldL3Id = -1;
                }
                cL2Id = _webSession.GenericProductDetailLevel.GetIdValue(dr, 2);
                if(cL2Id >= 0 && OldL2Id != cL2Id) {
                    nbL2Id++;
                    OldL2Id = cL2Id;
                    OldL3Id = -1;
                }
                cL3Id = _webSession.GenericProductDetailLevel.GetIdValue(dr, 3);
                if(cL3Id >= 0 && OldL3Id != cL3Id) {
                    nbL3Id++;
                    OldL3Id = cL3Id;
                }
                
                //if(!parutions.Contains(dr["date_creation"])) {
                //    parutions.Add(dr["date_creation"]);
                //}
                dateTmp = int.Parse(dr["date_creation"].ToString());
                if(!parutions.Contains(dateTmp)) {
                    parutions.Add(dateTmp);
                }

            }

            if((nbL1Id > 0) || (nbL2Id > 0) || (nbL3Id > 0)) {
                nbLine = nbL1Id + nbL2Id + nbL3Id + 1;
            }
            return (int)nbLine;
        }
        #endregion

        #region Calendar headers
        /// <summary>
        /// Calendar Headers and Cell factory
        /// </summary>
        protected virtual void GetCalendarHeaders(out Headers headers, out CellUnitFactory cellFactory, ArrayList parutions) {
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

            if (_showMediaSchedule) {
                // Media schedule column
                headers.Root.Add(new HeaderMediaSchedule(false, GestionWeb.GetWebWord(PM_COL, _webSession.SiteLanguage), PM_COL));
            }

            // Une colonne par date de parution
            parutions.Sort();
            foreach(Int32 parution in parutions) {
                switch(_webSession.DetailPeriod) {
                    case WebCst.CustomerSessions.Period.DisplayLevel.monthly:
                        headers.Root.Add(new Header(true
                            , MonthString.GetCharacters(int.Parse(parution.ToString().Substring(4, 2)), cultureInfo, 0) + " " + parution.ToString().Substring(0, 4)
                            , (long)parution));
                        break;
                    case WebCst.CustomerSessions.Period.DisplayLevel.weekly:
                        headers.Root.Add(new Header(true
                            , string.Format("S{0} - {1}", parution.ToString().Substring(4, 2), parution.ToString().Substring(0, 4))
                            , (long)parution));
                        break;
                    case WebCst.CustomerSessions.Period.DisplayLevel.dayly:
                        headers.Root.Add(new Header(true, Dates.YYYYMMDDToDD_MM_YYYY(parution.ToString(), _webSession.SiteLanguage), (long)parution));
                        break;
                    default:
                        break;
                }
                
            }
            if(!_webSession.Percentage) {
                cellFactory = _webSession.GetCellUnitFactory();
            }
            else {
                cellPDM = new CellPDM(0.0);
                cellPDM.StringFormat = "{0:percentWOSign}";
                cellFactory = new CellUnitFactory(cellPDM);
            }
        }
        #endregion


        protected virtual bool CanShowCreative()
        {
            return
                (_vehicleInformation.ShowCreations
                 &&
                 (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser) ||
                  _webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.product)));
        }

        protected virtual  void SetCellCreativesLink(ResultTable tab, int iCurLine, int iCol, AdExpressCellLevel[] cellLevels,int i)
        {
            tab[iCurLine, iCol] = new CellCreativesLink(cellLevels[i], _webSession, _webSession.GenericProductDetailLevel, string.Empty, -1);
        }

    }

}
