#region Information
// Author: G. Facon
// Creation date: 17/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using WebCst=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Domain.Level;
using DBCst=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpressI.Web.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Portofolio.DAL;
using System.Reflection;

namespace TNS.AdExpressI.Portofolio {
    /// <summary>
    /// Portofolio Results
    /// </summary>
    public abstract class PortofolioResults:IPortofolioResults {

        #region Constantes
        protected const long TOTAL_LINE_INDEX=0;
        protected const long DETAILED_PORTOFOLIO_EURO_COLUMN_INDEX=2;
        protected const long DETAILED_PORTOFOLIO_INSERTION_COLUMN_INDEX=3;
        protected const long DETAILED_PORTOFOLIO_DURATION_COLUMN_INDEX=4;
        protected const long DETAILED_PORTOFOLIO_MMC_COLUMN_INDEX=4;
        protected const long DETAILED_PORTOFOLIO_PAGE_COLUMN_INDEX=5;
        protected const int PROD_COL = 1164;
        protected const int INSERTIONS_LIST_COL = 2245;
        protected const int CREATIVES_COL = 1994;
        protected const int PM_COL = 751;
        protected const int EUROS_COL = 1423;
        protected const int MM_COL = 1424;
        protected const int SPOTS_COL = 939;
        protected const int INSERTIONS_COL = 940;
        protected const int PAGE_COL =943;
        protected const int PAN_COL = 1604;
        protected const int DURATION_COL = 1435;
        protected const int VOLUME = 2216;
        #endregion

        #region Variables
        /// <summary>
        /// Customer session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Vehicle
        /// </summary>
        protected DBClassificationConstantes.Vehicles.names _vehicle;
        /// <summary>
        /// Date begin
        /// </summary>
        protected string _periodBeginning;
        /// <summary>
        /// Date end
        /// </summary>
        protected string _periodEnd;
        /// <summary>
        /// Current Module
        /// </summary>
        protected TNS.AdExpress.Domain.Web.Navigation.Module _module;
        /// <summary>
        /// Show creations in the result
        /// </summary>
        protected bool _showCreatives;
        /// <summary>
        /// Show insertions in the result
        /// </summary>
        protected bool _showInsertions;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        protected PortofolioResults(WebSession webSession) {
            if(webSession==null) throw (new ArgumentNullException("Customer session is null"));
            _webSession=webSession;
            try {
                // Set Vehicle
                _vehicle=GetVehicleName();
                // Period
                _periodBeginning = GetDateBegin();
                _periodEnd = GetDateEnd();
                // Module
                _module=ModulesList.GetModule(webSession.CurrentModule);
                _showCreatives=ShowCreatives();
                _showInsertions=ShowCreatives();
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible to set parameters",err));
            }
        }
        #endregion

        #region IResult Membres

        #region HTML for:SYNTHESIS, NOVELTY, DETAIL MEDIA, PERFORMANCES
        /// <summary>
        /// Get HTML code for some portofolio result
        ///  - SYNTHESIS
        ///  - NOVELTY
        ///  - DETAIL_MEDIA
        ///  - PERFORMANCES
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="webSession">Customer session</param>
        /// <returns>HTML Code</returns>
        public string GetHtml(Page page) {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion

        #region Result table for: DETAIL PORTOFOLIO, CALENDAR
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <returns>Result Table</returns>
        public ResultTable GetResultTable() {
            try {
                switch(_webSession.CurrentTab) {
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                        return GetPortofolioResultTable();
                    //case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                    //    return WebRules.Results.PortofolioRules.GetCalendar(webSession);
                    default:
                        return null;
                }
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible de calculer le résultat d'une analyse de portefeuille",err));
            }
        }
        #endregion

        #endregion

        #region Rules

        #region Détail du portefeuille
        /// <summary>
        /// Obtient le tableau contenant l'ensemble des résultats
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Tableau de résultats</returns>
        public ResultTable GetPortofolioResultTable() {

            #region Variables
            ResultTable tab=null;
            DataTable dt =null;
            Headers headers=null;
            CellUnitFactory[] cellFactories=null;
            AdExpressCellLevel[] cellLevels;
            LineType[] lineTypes = new LineType[5] { LineType.total,LineType.level1,LineType.level2,LineType.level3,LineType.level4 };
            string[] columnsName=null;
            int iCurLine=0;
            int iNbLine=0;
            int iNbLevels=0;
            int insertions=0,creatives=0;
            
            #endregion

            // Get Data
            dt=GetDataForResultTable().Tables[0];
            // Table nb lines
            iNbLine=GetPortofolioSize(dt);

            #region Initialisation du tableau de résultats
            if(_showInsertions)insertions = 1;
            if(_showCreatives) creatives=1;
            GetPortofolioHeaders(headers,cellFactories,columnsName);
            tab = new ResultTable(iNbLine,headers);
            #endregion

            #region Traitement du tableau de résultats

            #region Intialisation des totaux
            iNbLevels = _webSession.GenericProductDetailLevel.GetNbLevels;
            cellLevels = new AdExpressCellLevel[iNbLevels+1];
            tab.AddNewLine(LineType.total);
            tab[iCurLine,1] = cellLevels[0] = new AdExpressCellLevel(0,GestionWeb.GetWebWord(805,_webSession.SiteLanguage),0,iCurLine,_webSession);
            //Creatives
            if(_showCreatives) tab[iCurLine,1+creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);
            if(_showInsertions) tab[iCurLine,1+creatives+insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);

            tab[iCurLine,2+creatives+insertions] = new CellMediaScheduleLink(cellLevels[0],_webSession);
            AffectPortefolioLine(cellFactories,columnsName,null,tab,iCurLine,false);
            #endregion

            int i = 1;
            long dCurLevel=0;
            foreach(DataRow row in dt.Rows) {
                //pour chaque niveau
                for(i=1;i <= iNbLevels;i++) {
                    //nouveau niveau i
                    dCurLevel = _webSession.GenericProductDetailLevel.GetIdValue(row,i);
                    if(dCurLevel > 0 && (cellLevels[i]==null || dCurLevel!=cellLevels[i].Id)) {
                        for(int j = i+1;j < cellLevels.Length;j++) {
                            cellLevels[j] = null;
                        }
                        iCurLine++;
                        tab.AddNewLine(lineTypes[i]);
                        tab[iCurLine,1] = cellLevels[i] = new AdExpressCellLevel(dCurLevel,_webSession.GenericProductDetailLevel.GetLabelValue(row,i),cellLevels[i-1],i,iCurLine,_webSession);
                        if(row.Table.Columns.Contains("id_address") && row["id_address"]!=System.DBNull.Value) {
                            cellLevels[i].AddressId = Convert.ToInt64(row["id_address"]);
                        }
                        //Creatives
                        if(creatives>0) tab[iCurLine,1+creatives] = new CellOneLevelCreativesLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);
                        if(insertions > 0) tab[iCurLine,1+creatives+insertions] = new CellOneLevelInsertionsLink((AdExpressCellLevel)tab[iCurLine,1],_webSession,_webSession.GenericProductDetailLevel);
                        tab[iCurLine,2+creatives+insertions] = new CellMediaScheduleLink((AdExpressCellLevel)tab[iCurLine,1],_webSession);
                        //feuille ou niveau parent?
                        if(i != iNbLevels) {
                            AffectPortefolioLine(cellFactories,columnsName,null,tab,iCurLine,false);
                        }
                        else {
                            AffectPortefolioLine(cellFactories,columnsName,row,tab,iCurLine,true);
                        }
                    }
                }
            }
            #endregion

            return tab;
        }

        #endregion

        #endregion

        #region Get Data
        /// <summary>
        /// Get data for ResultTable result
        /// </summary>
        /// <returns></returns>
        protected DataSet GetDataForResultTable() {
            DataSet ds=null;

            switch(_webSession.CurrentModule) {
                case WebCst.Module.Name.ALERTE_PORTEFEUILLE:
                    if(_module.CountryDataAccessLayer==null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
                    object[] parameters=new object[1];
                    parameters[0]=_webSession;
                    parameters[1]=_vehicle;
                    parameters[2]=_periodBeginning;
                    parameters[1]=_periodEnd;
                    IPortofolioDAL portofolioDAL=(IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory+@"Bin\"+_module.CountryDataAccessLayer.AssemblyName,_module.CountryDataAccessLayer.Class,false,BindingFlags.CreateInstance|BindingFlags.Instance|BindingFlags.Public,null,parameters,null,null,null);
                    //Portofolio.IResults result=(Portofolio.IResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory+@"Bin\"+module.CountryRulesLayer.AssemblyName,module.CountryRulesLayer.Class);
                    ds=portofolioDAL.GetMediaPortofolio();
                    //ds=PortofolioDataAccess.GetData(_webSession,vehicle,_module.ModuleType,periodBeginning,_periodEnd);
                    break;
                case WebCst.Module.Name.ANALYSE_PORTEFEUILLE:
                    //ds = PortofolioAnalysisDataAccess.GetGenericData(_webSession,vehicleName);
                    break;
                default:
                    throw (new PortofolioException("Invalid module"));
            }
            if(ds==null || ds.Tables[0]==null || ds.Tables[0].Rows.Count==0) {
                throw(new PortofolioException("DataSet for ResultTable is null"));
            }
            return(null);
        }
        #endregion

        #region Methods

        #region Affect Portefolio Line
        /// <summary>
        /// Affect Portefolio Line
        /// </summary>
        /// <param name="tCellFactories">Cell Factory</param>
        /// <param name="columnsName">Column names table</param>
        /// <param name="dr">DataRow</param>
        /// <param name="oTab">Result table</param>
        /// <param name="iLineIndex">Line Index</param>
        /// <param name="isLeaf">Is Leaf</param>
        protected void AffectPortefolioLine(CellUnitFactory[] tCellFactories,string[] columnsName,DataRow dr,ResultTable oTab,int iLineIndex,bool isLeaf) {

            for(int i = 0;i < tCellFactories.Length;i++) {
                if(tCellFactories[i] != null) {
                    oTab[iLineIndex,i+1] = tCellFactories[i].Get(0.0);
                    if(dr != null) {
                        if(isLeaf) {
                            oTab.AffectValueAndAddToHierarchy(1,iLineIndex,i+1,Convert.ToDouble(dr[columnsName[i]]));
                        }
                    }
                }
            }
        } 
        #endregion

        #region Portofolio headers
        /// <summary>
        /// Portofolio Headers and Cell factory
        /// </summary>
        /// <returns></returns>
        protected void GetPortofolioHeaders(Headers headers,CellUnitFactory[] cellFactories,string[] columnsName) {
            int insertions = 0;
            int creatives =0;
            int iNbCol=0;

            headers = new TNS.FrameWork.WebResultUI.Headers();
            // Product column
            headers.Root.Add(new Header(true,GestionWeb.GetWebWord(PROD_COL,_webSession.SiteLanguage),PROD_COL));
            // Insertions column
            if(_showInsertions) {
                headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(INSERTIONS_LIST_COL,_webSession.SiteLanguage),INSERTIONS_LIST_COL));
                insertions = 1;
            }
            // Creatives column     
            if(_showCreatives) {
                headers.Root.Add(new HeaderCreative(false,GestionWeb.GetWebWord(CREATIVES_COL,_webSession.SiteLanguage),CREATIVES_COL));
                creatives=1;
            }
            // Media schedule column
            headers.Root.Add(new HeaderMediaSchedule(false,GestionWeb.GetWebWord(PM_COL,_webSession.SiteLanguage),PM_COL));

            headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(EUROS_COL,_webSession.SiteLanguage),EUROS_COL));
            switch(_vehicle) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(MM_COL,_webSession.SiteLanguage),MM_COL));
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(PAGE_COL,_webSession.SiteLanguage),PAGE_COL));
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(INSERTIONS_COL,_webSession.SiteLanguage),INSERTIONS_COL));
                    iNbCol = 6+creatives+insertions;
                    cellFactories = new CellUnitFactory[iNbCol];
                    columnsName = new string[iNbCol];
                    columnsName[3+creatives+insertions] = "mmpercol";
                    columnsName[4+creatives+insertions] = "pages";
                    columnsName[5+creatives+insertions] = "insertion";
                    cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellMMC(0.0));
                    cellFactories[4+creatives+insertions] = new CellUnitFactory(new CellPage(0.0));
                    cellFactories[5+creatives+insertions] = new CellUnitFactory(new CellInsertion(0.0));
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                    if(_webSession.CustomerLogin.GetFlag(TNS.AdExpress.Constantes.DB.Flags.ID_VOLUME_MARKETING_DIRECT) != null) {
                        headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(VOLUME,_webSession.SiteLanguage),VOLUME));
                        iNbCol = 4 + creatives+insertions;
                        cellFactories = new CellUnitFactory[iNbCol];
                        columnsName = new string[iNbCol];
                        columnsName[3 + creatives+insertions] = "volume";
                        cellFactories[3 + creatives+insertions] = new CellUnitFactory(new CellVolume(0.0));
                    }
                    else {
                        iNbCol = 3 + creatives+insertions;
                        cellFactories = new CellUnitFactory[iNbCol];
                        columnsName = new string[iNbCol];
                    }
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(DURATION_COL,_webSession.SiteLanguage),DURATION_COL));
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(SPOTS_COL,_webSession.SiteLanguage),SPOTS_COL));
                    iNbCol = 5+creatives+insertions;
                    columnsName = new string[iNbCol];
                    columnsName[3+creatives+insertions] = "duration";
                    columnsName[4+creatives+insertions] = "insertion";
                    cellFactories = new CellUnitFactory[iNbCol];
                    cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellDuration(0.0));
                    cellFactories[4+creatives+insertions] = new CellUnitFactory(new CellNumber(0.0));
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                    headers.Root.Add(new TNS.FrameWork.WebResultUI.Header(true,GestionWeb.GetWebWord(PAN_COL,_webSession.SiteLanguage),PAN_COL));
                    iNbCol = 4+creatives+insertions;
                    columnsName = new string[iNbCol];
                    columnsName[3+creatives+insertions] = "insertion";
                    cellFactories = new CellUnitFactory[iNbCol];
                    cellFactories[3+creatives+insertions] = new CellUnitFactory(new CellNumber(0.0));
                    break;
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                    iNbCol = 3+creatives+insertions;
                    columnsName = new string[iNbCol];
                    cellFactories = new CellUnitFactory[iNbCol];
                    break;
                default:
                    throw new PortofolioException("Média non traité.");
            }
            cellFactories[0] = null;
            cellFactories[1] = null;
            if(_showCreatives) columnsName[1+creatives] = null;
            if(_showInsertions) columnsName[1+creatives+insertions] = null;
            columnsName[2+creatives+insertions] = "euro";
            cellFactories[2+creatives+insertions] = new CellUnitFactory(new CellEuro(0.0));
        }
        #endregion

        #region Dates
        /// <summary>
        /// Get begin date for the 2 module types
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// </summary>
        /// <returns>Begin date</returns>
        protected string GetDateBegin() {
            switch(_webSession.CurrentModule) {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate,_webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodBeginningDate);
            }
            return (null);
        }

        /// <summary>
        /// Get ending date for the 2 module types
        /// </summary>
        /// - Portofolio Alert
        /// - Portofolio analysis
        /// <returns>Ending date</returns>
        protected string GetDateEnd() {
            switch(_webSession.CurrentModule) {
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PORTEFEUILLE:
                    return (Dates.getPeriodEndDate(_webSession.PeriodEndDate,_webSession.PeriodType).ToString("yyyyMMdd"));
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return (_webSession.PeriodEndDate);
            }
            return (null);
        }
        #endregion

        #region Vehicle Selection
        /// <summary>
        /// Get Vehicle Selection
        /// </summary>
        /// <returns>Vehicle label</returns>
        protected string GetVehicle() {
            string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
            if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw (new PortofolioException("The media selection is invalid"));
            return (vehicleSelection);
        }
        /// <summary>
        /// Get vehicle selection
        /// </summary>
        /// <returns>Vehicle</returns>
        protected DBClassificationConstantes.Vehicles.names GetVehicleName() {
            try {
                return ((DBClassificationConstantes.Vehicles.names)int.Parse(GetVehicle()));
            }
            catch(System.Exception err) {
                throw (new PortofolioException("Impossible to retreive vehicle selection"));
            }
        }
        #endregion

        #region Get lines number for the portofolio result
        /// <summary>
        /// Get lines number for the portofolio result
        /// </summary>
        /// <param name="dt">Data table</param>
        /// <returns>Lines number</returns>
        protected int GetPortofolioSize(DataTable dt) {

            #region Variables
            Int64 OldL1Id=0;
            Int64 cL1Id=0;
            Int64 nbL1Id=0;
            Int64 OldL2Id=0;
            Int64 cL2Id=0;
            Int64 nbL2Id=0;
            Int64 OldL3Id=0;
            Int64 cL3Id=0;
            Int64 nbL3Id=0;
            Int64 nbLine=0;
            #endregion

            if(dt!=null && dt.Rows.Count>0) {
                foreach(DataRow dr in dt.Rows) {
                    cL1Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,1);
                    if(cL1Id > 0 && cL1Id!=OldL1Id) {
                        nbL1Id++;
                        OldL1Id=cL1Id;
                        OldL2Id=OldL3Id=-1;
                    }
                    cL2Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,2);
                    if(cL2Id>0 && OldL2Id!=cL2Id) {
                        nbL2Id++;
                        OldL2Id=cL2Id;
                        OldL3Id=-1;
                    }
                    cL3Id = _webSession.GenericProductDetailLevel.GetIdValue(dr,3);
                    if(cL3Id>0 && OldL3Id!=cL3Id) {
                        nbL3Id++;
                        OldL3Id=cL3Id;
                    }
                }
            }
            if((nbL1Id>0) || (nbL2Id>0) || (nbL3Id>0)) {
                nbLine=nbL1Id+nbL2Id+nbL3Id+1;
            }
            return (int)nbLine;
        }
        #endregion

        #region Insertion and Creations
        /// <summary>
        /// Determine if the result shows the insertion column
        /// </summary>
        /// <returns>True if the Insertion column is shown</returns>
        protected bool ShowInsertions() {
            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
                if(_module.ModuleType==WebCst.Module.Type.alert &&
					(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product))) {
                    return (true);
                    break;
                }
            }
            return (false);
        }
        /// <summary>
        /// Determine if the result shows the creation column
        /// </summary>
        /// <returns>True if the creation column is shown</returns>
        protected bool ShowCreatives() {
            foreach(DetailLevelItemInformation item in _webSession.GenericProductDetailLevel.Levels) {
                if(_module.ModuleType==WebCst.Module.Type.alert &&
                    _webSession.CustomerLogin.GetFlag(DBCst.Flags.ID_SLOGAN_ACCESS_FLAG)!=null &&
					(item.Id.Equals(DetailLevelItemInformation.Levels.advertiser)
					|| item.Id.Equals(DetailLevelItemInformation.Levels.product))) {
                    return (true);
                    break;
                }
            }
            return (false);
        }
        #endregion
        #endregion
    }
}
