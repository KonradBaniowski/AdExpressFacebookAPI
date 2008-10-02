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
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Result;

using TNS.AdExpressI.NewCreatives.DAL;
using TNS.AdExpressI.NewCreatives.Exceptions;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Collections;
using TNS.Classification.Universe;
#endregion

namespace TNS.AdExpressI.NewCreatives {
    
    /// <summary>
    /// Default new creatives
    /// </summary>
    public abstract class NewCreativesResult:INewCreativesResult {

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current vehicle univers
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Navigation.Module _module;
        /// <summary>
        /// Sector ID
        /// </summary>
        protected Int64 _idSector = -1;
        /// <summary>
        /// Begining Date
        /// </summary>
        protected string _beginingDate;
        /// <summary>
        /// End Date
        /// </summary>
        protected string _endDate;
        #endregion

        #region Accessors
        /// <summary>
        /// Get User session
        /// </summary>
        public WebSession Session {
            get { return _session; }
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
            _session = session;
            _idSector = GetSectorId();
            _beginingDate = session.PeriodBeginningDate;
            _endDate = session.PeriodEndDate;
            _module = Navigation.ModulesList.GetModule(session.CurrentModule);

            #region Sélection du vehicle
            string vehicleSelection = session.GetSelection(session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if(vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new NewCreativesException("Uncorrect Media Selection"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion
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
            LineType[] lineTypes = new LineType[4] { LineType.total, LineType.level1, LineType.level2, LineType.level3 };
            Headers headers = null;
            Int64 iCurLine = 0;
            int iNbLine = 0;
            int iNbLevels = 0;
            ArrayList parutions = new ArrayList();
            #endregion

            #region Chargement des données
            if(_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[4];
            parameters[0] = _session;
            parameters[1] = _idSector;
            parameters[2] = _beginingDate;
            parameters[3] = _endDate;
            INewCreativeResultDAL newCreativesDAL = (INewCreativeResultDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
            ds = newCreativesDAL.GetData();

            if(ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0) dt = ds.Tables[0];
            else return (tab);
            #endregion

            return null;
        }
        #endregion

        #region Protected method
        /// <summary>
        /// Get sector ID
        /// </summary>
        /// <returns>Sector ID value</returns>
        protected virtual Int64 GetSectorId() {
            Int64 sectorId = -1;
            List<long> savedAdvertisers = null;
            string saveAdvertisersString="";
            NomenclatureElementsGroup nomenclatureElementsGroup = null;
            if(_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0) {
                nomenclatureElementsGroup = _session.PrincipalProductUniverses[0].GetGroup(0);
                if(nomenclatureElementsGroup != null) {
                    savedAdvertisers = nomenclatureElementsGroup.Get(TNSClassificationLevels.SECTOR);
                    saveAdvertisersString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.SECTOR);
                    if(saveAdvertisersString != null && saveAdvertisersString.Length > 0)
                        sectorId = Int64.Parse(saveAdvertisersString);
                }
            }
            return (sectorId);
        }
        #endregion

    }

}
