#region Information
// Author: Y. Rkaina && D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
//using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;

//using TNS.AdExpress.Web.Exceptions;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Units;
using TNS.FrameWork.DB.Common;
using AdExClassification = TNS.AdExpress.DataAccess.Classification;

namespace TNS.AdExpressI.Portofolio.DAL
{
    /// <summary>
    /// Portofolio Data Access Layer
    /// </summary>
    public abstract class PortofolioDAL : IPortofolioDAL
    {

        #region Variables
        /// <summary>
        /// Customer session
        /// </summary>
        protected WebSession _webSession;
        /// <summary>
        /// Current Module
        /// </summary>
        protected Module _module;
        /// <summary>
        /// Vehicle name
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// Media Id
        /// </summary>
        protected Int64 _idMedia;
        /// <summary>
        /// Begining Date
        /// </summary>
        protected string _beginingDate;
        /// <summary>
        /// End Date
        /// </summary>
        protected string _endDate;
        /// <summary>
        /// Screen code
        /// </summary>
        protected string _adBreak;
        /// <summary>
        /// Beginning hour interval list
        /// </summary>
        protected Dictionary<string, double> _hourBeginningList = null;
        /// <summary>
        /// End hour interval list
        /// </summary>
        protected Dictionary<string, double> _hourEndList = null;
        /// <summary>
        /// Ventilation type list for press result
        /// </summary>
        protected List<PortofolioStructure.Ventilation> _ventilationTypeList = null;
        /// <summary>
        /// Level
        /// </summary>
        protected DetailLevelItemInformation _level;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate)
        {
            if (webSession == null) throw (new ArgumentNullException("Customer session is null"));
            if (beginingDate == null || beginingDate.Length == 0) throw (new ArgumentException("Begining Date is invalid"));
            if (endDate == null || endDate.Length == 0) throw (new ArgumentException("End Date is invalid"));
            if (vehicleInformation == null) throw (new ArgumentNullException("vehicleInformation session is null"));
            _webSession = webSession;
            _beginingDate = beginingDate;
            _endDate = endDate;
            _vehicleInformation = vehicleInformation;
            _idMedia = idMedia;
            try
            {
                // Module
                _module = ModulesList.GetModule(webSession.CurrentModule);

            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to set parameters", err));
            }

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="adBreak"></param>		
        protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, string adBreak) :
        this(webSession, vehicleInformation, idMedia, beginingDate, endDate)
        {
            _adBreak = adBreak;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="hour Beginning List">hour Beginning List</param>
        /// <param name="hourEndList">hour EndList</param>		
        protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
        : this(webSession, vehicleInformation, idMedia, beginingDate, endDate)
        {
            _hourBeginningList = hourBeginningList;
            _hourEndList = hourEndList;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="adBreak"></param>
        /// <param name="ventilationTypeList">ventilation Type List</param>
        protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, List<PortofolioStructure.Ventilation> ventilationTypeList)
            :
        this(webSession, vehicleInformation, idMedia, beginingDate, endDate)
        {
            _ventilationTypeList = ventilationTypeList;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="vehicleInformation">Vehicle name</param>
        /// <param name="idMedia">Media Id</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        /// <param name="level">Level</param>
        protected PortofolioDAL(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string beginingDate, string endDate, DetailLevelItemInformation level)
            : this(webSession, vehicleInformation, idMedia, beginingDate, endDate)
        {
            _level = level;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected PortofolioDAL()
        {
        }
        #endregion

        #region IPortofolioDAL Membres

        /// <summary>
        /// Get synthesis data
        /// </summary>
        /// <returns></returns>
        public virtual DataSet GetData()
        {
            Engines.Engine res = null;

            switch (_webSession.CurrentTab)
            {
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    res = new Engines.PortofolioDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                    res = new Engines.CalendarEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                    switch (_vehicleInformation.Id)
                    {
                        case DBClassificationConstantes.Vehicles.names.others:
                        case DBClassificationConstantes.Vehicles.names.tv:
                        case DBClassificationConstantes.Vehicles.names.tvGeneral:
                        case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                        case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                        case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                        case DBClassificationConstantes.Vehicles.names.radio:
                        case DBClassificationConstantes.Vehicles.names.radioGeneral:
                        case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                        case DBClassificationConstantes.Vehicles.names.radioMusic:
                            res = new Engines.MediaDetailEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
                            break;
                        default:
                            throw (new PortofolioDALException("Impossible to identified current vehicle "));
                    }
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                    res = GetStructData();
                    break;
                default:
                    throw (new PortofolioDALException("Impossible to identified current tab "));
            }

            return res.GetData();
        }

        /// <summary>
        /// Get Inssertion data
        /// </summary>
        /// <returns></returns>
        public virtual DataSet GetInsertionData()
        {
            var res = new Engines.InsertionDetailEngine(_webSession,
                _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _adBreak);
            return res.GetData();
        }

        #region Synthesis membres
        /// <summary>
        /// Get synthesis data
        /// </summary>
        /// <param name="synthesisDataType">Synthesis Data Type</param>
        /// <returns></returns>
        public virtual DataSet GetSynthisData(PortofolioSynthesis.dataType synthesisDataType)
        {
            var res = new Engines.SynthesisEngine(_webSession,
                _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, synthesisDataType);
            return res.GetData();
        }

        /// <summary>
        /// récupère les écrans
        /// </summary>
        /// <returns>Ecrans</returns>
        public virtual DataSet GetEcranData()
        {
            var res = new Engines.SynthesisEngine(_webSession,
                _vehicleInformation, _module, _idMedia, _beginingDate, _endDate);
            return res.GetEcranData();
        }

        /// <summary>
        /// Get Custom Ecran Data
        /// </summary>
        /// <returns>Ecrans</returns>
        public virtual DataSet GetCustomEcranData()
        {
            return null;
        }

        #region TableOfIssue
        /// <summary>
        /// Implements  data access layer for table of issue. 
        /// </summary>
        /// <returns>Data Set with Data Table Result</returns>
        public virtual DataSet TableOfIssue()
        {
            throw new NotImplementedException("This methods is not implemented");
        }
        #endregion


        private DataSet GetPortfolioAlertParamsUniverse(long alertId, IDataSource source)
        {

            #region Construction de la requête
            StringBuilder sql = new StringBuilder(800);

            sql.Append("select " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert, " + DBConstantes.Tables.ALERT_PREFIXE + ".alert, " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert_type, " + DBConstantes.Tables.ALERT_PREFIXE + ".email_list, " + DBConstantes.Tables.ALERT_PREFIXE + ".date_beginning, " + DBConstantes.Tables.ALERT_PREFIXE + ".date_end, ");
            sql.Append(" " + DBConstantes.Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE + ".rank, ");
            sql.Append(" " + DBConstantes.Tables.ALERT_UNIVERSE_PREFIXE + ".reference_universe, ");
            sql.Append(" " + DBConstantes.Tables.ALERT_UNIVERSE_DETAIL_PREFIXE + ".id_alert_universe_type, " + DBConstantes.Schema.LOGIN_SCHEMA + ".listnum_to_char(universe_list) as universe_list ");
            sql.Append(" from " + DBConstantes.Schema.LOGIN_SCHEMA + "." + DBConstantes.Tables.ALERT + " " + DBConstantes.Tables.ALERT_PREFIXE + ",");
            sql.Append(" " + DBConstantes.Schema.LOGIN_SCHEMA + "." + DBConstantes.Tables.ALERT_UNIVERSE_ASSIGNMENT + " " + DBConstantes.Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE + ",");
            sql.Append(" " + DBConstantes.Schema.LOGIN_SCHEMA + "." + DBConstantes.Tables.ALERT_UNIVERSE + " " + DBConstantes.Tables.ALERT_UNIVERSE_PREFIXE + ",");
            sql.Append(" " + DBConstantes.Schema.LOGIN_SCHEMA + "." + DBConstantes.Tables.ALERT_UNIVERSE_DETAIL + " " + DBConstantes.Tables.ALERT_UNIVERSE_DETAIL_PREFIXE);
            sql.Append(" where " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert = " + DBConstantes.Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE + ".id_alert ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE + ".id_alert_universe = " + DBConstantes.Tables.ALERT_UNIVERSE_PREFIXE + ".id_alert_universe ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_UNIVERSE_PREFIXE + ".id_alert_universe = " + DBConstantes.Tables.ALERT_UNIVERSE_DETAIL_PREFIXE + ".id_alert_universe ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert = " + alertId);
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".date_beginning <= sysdate ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".date_end >= sysdate ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" and " + DBConstantes.Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" and " + DBConstantes.Tables.ALERT_UNIVERSE_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" and " + DBConstantes.Tables.ALERT_UNIVERSE_DETAIL_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" order by " + DBConstantes.Tables.ALERT_UNIVERSE_ASSIGNMENT_PREFIXE + ".rank");
            #endregion

            #region Execution de la requête
            try
            {
                return (source.Fill(sql.ToString()));
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible de charger la configuration des univers de l'alerte (" + alertId.ToString() + ") : " + sql.ToString() + " - " + err.Message, err));
            }
            #endregion

        }

        private DataSet GetPortfolioAlertParamsFlag(long alertId, IDataSource source)
        {

            #region Construction de la requête
            StringBuilder sql = new StringBuilder(350);

            sql.Append("select " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert, " + DBConstantes.Tables.ALERT_PREFIXE + ".alert, ");
            sql.Append(" " + DBConstantes.Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE + ".id_alert_flag ");
            sql.Append(" from " + DBConstantes.Schema.LOGIN_SCHEMA + "." + DBConstantes.Tables.ALERT + " " + DBConstantes.Tables.ALERT_PREFIXE + ",");
            sql.Append(" " + DBConstantes.Schema.LOGIN_SCHEMA + "." + DBConstantes.Tables.ALERT_FLAG_ASSIGNMENT + " " + DBConstantes.Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE + " ");
            sql.Append(" where " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert = " + DBConstantes.Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE + ".id_alert ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert = " + alertId);
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".date_beginning <= sysdate ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".date_end >= sysdate ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" and " + DBConstantes.Tables.ALERT_FLAG_ASSIGNMENT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            #endregion

            #region Execution de la requête
            try
            {
                return (source.Fill(sql.ToString()));
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible de charger la configuration des flags de l'alerte (" + alertId.ToString() + ") : " + sql.ToString() + " - " + err.Message, err));
            }
            #endregion

        }

        private DataSet GetPortfolioAlertFlagsFromLogin(long alertId, IDataSource source)
        {

            #region Construction de la requête
            StringBuilder sql = new StringBuilder(350);

            sql.Append("select fpa.id_flag");
            sql.Append(" from " + DBConstantes.Schema.LOGIN_SCHEMA + "." + DBConstantes.Tables.ALERT + " " + DBConstantes.Tables.ALERT_PREFIXE + ",");
            sql.Append(" " + DBConstantes.Schema.LOGIN_SCHEMA + ".FLAG_PROJECT_ASSIGNMENT fpa ");
            sql.Append(" where " + DBConstantes.Tables.ALERT_PREFIXE + ".id_alert = " + alertId);
            sql.Append(" and fpa.ID_PROJECT = 1 ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".ID_LOGIN = fpa.ID_LOGIN ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".date_beginning <= sysdate ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".date_end >= sysdate ");
            sql.Append(" and " + DBConstantes.Tables.ALERT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            sql.Append(" and fpa.activation < " + DBConstantes.ActivationValues.UNACTIVATED);
            #endregion

            #region Execution de la requête
            try
            {
                return (source.Fill(sql.ToString()));
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible de charger la configuration des flags de l'alerte (" + alertId.ToString() + ") : " + sql.ToString() + " - " + err.Message, err));
            }
            #endregion

        }

        public virtual PortfolioAlertParams GetPortfolioAlertParams(long alertId)
        {

            PortfolioAlertParams alertParams = new PortfolioAlertParams
            {
                TypeAlertId = 0,
                MediaId = 0,
                AlertName = string.Empty,
                SectorListId = string.Empty,
                SubSectorListId = string.Empty,
                GroupListId = string.Empty,
                SegmentListId = string.Empty,
                MediaName = string.Empty,
                LanguageId = 33,
                Inset = true,
                Autopromo = true,
                EmailList = new List<string>(),
                AdvertisingAgencyRight = false
            };

            TNS.FrameWork.DB.Common.IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);

            // Get Alert Parameters
            DataSet dsUniverse = GetPortfolioAlertParamsUniverse(alertId, source);
            DataSet dsFlag = GetPortfolioAlertParamsFlag(alertId, source);
            DataSet dsFlagsRightFromLogin = GetPortfolioAlertFlagsFromLogin(alertId, source);

            if (dsUniverse != null && dsUniverse.Tables.Count > 0 && dsUniverse.Tables[0] != null && dsUniverse.Tables[0].Rows.Count > 0)
            {
                alertParams.TypeAlertId = int.Parse(dsUniverse.Tables[0].Rows[0]["id_alert_type"].ToString());
                alertParams.AlertName = dsUniverse.Tables[0].Rows[0]["alert"].ToString();
                var emails = dsUniverse.Tables[0].Rows[0]["email_list"].ToString().Split(',');
                foreach (string currentEmail in emails)
                {
                    alertParams.EmailList.Add(currentEmail);
                }
                foreach (DataRow currentRow in dsUniverse.Tables[0].Rows)
                {
                    switch (Int64.Parse(currentRow["id_alert_universe_type"].ToString()))
                    {
                        case DBConstantes.AlertUniverseType.FAMILLE_VALUE: // 1
                            alertParams.SectorListId = currentRow["universe_list"].ToString();
                            break;
                        case DBConstantes.AlertUniverseType.CLASSE_VALUE: // 2
                            alertParams.SubSectorListId = currentRow["universe_list"].ToString();
                            break;
                        case DBConstantes.AlertUniverseType.GROUPE_VALUE: // 3
                            alertParams.GroupListId = currentRow["universe_list"].ToString();
                            break;
                        case DBConstantes.AlertUniverseType.VARIETE_VALUE: // 4
                            alertParams.SegmentListId = currentRow["universe_list"].ToString();
                            break;
                        case DBConstantes.AlertUniverseType.SUPPORT_VALUE: // 7
                            alertParams.MediaId = int.Parse(currentRow["universe_list"].ToString());
                            break;
                    }
                }
                AdExClassification.MediaBranch.PartialMediaListDataAccess mediaNameA = new AdExClassification.MediaBranch.PartialMediaListDataAccess(alertParams.MediaId.ToString(), alertParams.LanguageId, source);
                alertParams.MediaName = mediaNameA[alertParams.MediaId].ToString();
            }
            if (dsFlag != null && dsFlag.Tables.Count > 0 && dsFlag.Tables[0] != null && dsFlag.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow currentFlagRow in dsFlag.Tables[0].Rows)
                {
                    // Si précisé dans la table alert_flag_assignment, on veut du hors encart et/ou hors autopromo
                    switch (Int64.Parse(currentFlagRow["id_alert_flag"].ToString()))
                    {
                        case 1:
                            alertParams.Inset = false; // False pour hors encart
                            break;
                        case 2:
                            alertParams.Autopromo = false; // False pour hors autopromo
                            break;
                    }
                }
            }
            if (dsFlagsRightFromLogin != null && dsFlagsRightFromLogin.Tables.Count > 0 && dsFlagsRightFromLogin.Tables[0] != null && dsFlagsRightFromLogin.Tables[0].Rows.Count > 0)
            {
                alertParams.AdvertisingAgencyRight = dsFlagsRightFromLogin.Tables[0].Select("id_flag = " + DBConstantes.Flags.ID_PRESS_MEDIA_AGENCY_FLAG).Length != 0;
            }

            return alertParams;
        }


        public virtual DataSet GetPortfolioAlertData(long alertId, long alertTypeId, string dateMediaNum)
        {
            #region Constantes
            const string AUTOPROMO_GROUP_ID = "549";
            const string AUTOPROMO_SEGMENT_ID = "56202";
            #endregion

            TNS.FrameWork.DB.Common.IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);

            PortfolioAlertParams alertParams = GetPortfolioAlertParams(alertId);


            #region Construction de la requête
            StringBuilder sql = new StringBuilder(5000);

            // Select
            sql.Append("select advertiser, ");
            sql.Append("product, ");
            sql.Append("sector, ");
            sql.Append("group_, ");
            sql.Append("area_page, ");
            sql.Append("area_mmc,format, ");
            sql.Append("wp.expenditure_euro, ");
            sql.Append("wp.date_media_num, ");
            sql.Append("wp.media_paging, ");
            sql.Append("rank_sector, ");
            sql.Append("rank_group_, ");
            sql.Append("rank_media, ");
            sql.Append("color, ");
            sql.Append("location, ");
            sql.Append("wp.date_media_num, ");
            sql.Append("media, ");
            sql.Append("LPAD(RTRIM(wp.Media_paging,' '),10,'0') as ChampPage, ");
            sql.Append("wp.id_advertisement, ");
            sql.Append("visual, ");
            sql.Append("wp.date_cover_num, ");

            sql.Append("GROUP_ADVERTISING_AGENCY, ");
            sql.Append("ADVERTISING_AGENCY, ");
            sql.Append("gad.id_address ");

            // From
            sql.Append(" from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.ALERT_DATA_PRESS + " wp ");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".advertiser ad");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".product pr");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".sector se");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_ gr");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media me ");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".color co ");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".format fo ");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".location lo ");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".data_location dl ");

            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".GROUP_ADVERTISING_AGENCY gaa ");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".ADVERTISING_AGENCY aa ");
            sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".GAD gad ");

            // Conditions
            sql.Append(" where wp.id_media=" + alertParams.MediaId);
            sql.Append(" and wp.insertion=1 ");
            sql.Append(" and wp.date_media_num = " + dateMediaNum);
            sql.Append(" and co.id_color(+)=wp.id_color ");
            sql.Append(" and fo.id_format(+)=wp.id_format ");
            sql.Append(" and lo.id_location(+)=dl.id_location ");
            sql.Append(" and dl.id_media(+)=wp.id_media ");
            sql.Append(" and dl.date_media_num(+) =wp.date_media_num ");
            sql.Append(" and dl.id_advertisement (+)=wp.id_advertisement ");

            sql.Append(" and gaa.ID_GROUP_ADVERTISING_AGENCY =wp.ID_GROUP_ADVERTISING_AGENCY ");
            sql.Append(" and aa.ID_ADVERTISING_AGENCY=wp.ID_ADVERTISING_AGENCY ");
            sql.Append(" and gad.ID_ADVERTISER (+)= wp.ID_ADVERTISER ");


            sql.Append(" and co.id_language=" + DBConstantes.Language.FRENCH);
            sql.Append(" and fo.id_language=" + DBConstantes.Language.FRENCH);
            sql.Append(" and lo.id_language (+)=" + DBConstantes.Language.FRENCH);

            sql.Append(" and gaa.id_language=" + DBConstantes.Language.FRENCH);
            sql.Append(" and aa.id_language=" + DBConstantes.Language.FRENCH);
            //sql.Append(" and gad.id_language_i=" + DBConstantes.Language.FRENCH);

            sql.Append(" and co.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and fo.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and lo.activation(+)<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and dl.activation(+)<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

            sql.Append(" and aa.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and gaa.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            //sql.Append(" and gad.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

            sql.Append(" and ad.id_advertiser=wp.id_advertiser ");
            sql.Append(" and pr.id_product=wp.id_product ");
            sql.Append(" and se.id_sector=wp.id_sector ");
            sql.Append(" and gr.id_group_=wp.id_group_ ");
            sql.Append(" and  me.id_media=wp.id_media ");

            sql.Append(" and ad.id_language=" + DBConstantes.Language.FRENCH);
            sql.Append(" and pr.id_language=" + DBConstantes.Language.FRENCH);
            sql.Append(" and se.id_language=" + DBConstantes.Language.FRENCH);
            sql.Append(" and gr.id_language=" + DBConstantes.Language.FRENCH);
            sql.Append(" and me.id_language=" + DBConstantes.Language.FRENCH);

            sql.Append(" and ad.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and pr.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and se.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and gr.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and me.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

            #region Univers produit de l'alerte
            bool first = true;
            string tmp = "";
            if (alertParams.SectorListId.Length > 0)
            {
                if (first)
                {
                    tmp += " and (";
                    first = false;
                }
                else tmp += " or ";
                tmp += " wp.id_sector in(" + alertParams.SectorListId + ") ";
            }
            if (alertParams.SubSectorListId.Length > 0)
            {
                if (first)
                {
                    tmp += " and (";
                    first = false;
                }
                else tmp += " or ";
                tmp += " wp.id_subsector in(" + alertParams.SubSectorListId + ") ";
            }
            if (alertParams.GroupListId.Length > 0)
            {
                if (first)
                {
                    tmp += " and (";
                    first = false;
                }
                else tmp += " or ";
                tmp += " wp.id_group_ in(" + alertParams.GroupListId + ") ";
            }
            if (alertParams.SegmentListId.Length > 0)
            {
                if (first)
                {
                    tmp += " and (";
                    first = false;
                }
                else tmp += " or ";
                tmp += " wp.id_segment in(" + alertParams.SegmentListId + ") ";
            }
            if (tmp.Length > 0)
            {
                tmp += ") ";
                sql.Append(tmp);
            }
            #endregion

            if (!alertParams.Autopromo)
            {
                // HORS AUTOPROMO
                //sql.Append(" and wp.id.group_ not in(549) and wp.id_segment not in(56202)");
                sql.Append(" and wp.id_group_ not in(" + AUTOPROMO_GROUP_ID + ") and wp.id_segment not in(" + AUTOPROMO_SEGMENT_ID + ")");
            }
            if (!alertParams.Inset)
            {
                // HORS ENCART
                sql.Append(" and wp.id_inset is null"); // ??? C avec encart : is not null ???
            }

            // Order by
            sql.Append(" order by wp.Id_type_page, ChampPage, wp.id_product, wp.id_advertisement");
            #endregion

            #region Execution de la requête
            try
            {
                return source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new Exception("Impossible de charger les données du détail du support : " + sql, err));
            }
            #endregion
        }

        #endregion

        #region Get Struct Data
        /// <summary>
        /// Get structure data 
        /// </summary>
		/// <remarks>Used for tv or radio</remarks>
		/// <param name="hourBegin">Hour Begin</param>
		/// <param name="hourEnd">Hour End</param>
        /// <returns>DataSet</returns>
		protected virtual Engines.StructureEngine GetStructData()
        {
            Engines.StructureEngine res = null;

            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                    res = new Engines.StructureEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _hourBeginningList, _hourEndList);
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    res = new Engines.StructureEngine(_webSession, _vehicleInformation, _module, _idMedia, _beginingDate, _endDate, _ventilationTypeList);
                    break;
                default:
                    throw (new PortofolioDALException("Impossible to identified current vehicle "));
            }
            return res;
        }
        #endregion


        #region Investment By Media
        /// <summary>
        /// Get Investment By Media
        /// </summary>
        /// <returns>Data</returns>
        public virtual Hashtable GetInvestmentByMedia()
        {

            #region Variables
            string sql = "";
            Hashtable htInvestment = new Hashtable();
            string product = null;
            string productsRights = null;
            string mediaRights = null;
            string listProductHap = null;
            string euroFieldNameSumWithAlias = null;
            string insertionFieldNameSumWithAlias = null;
            #endregion

            try
            {
                product = GetProductData();
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                euroFieldNameSumWithAlias = string.Format("sum({0}) as {1}", UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField,
                    UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString());
                insertionFieldNameSumWithAlias = string.Format("sum({0}) as {1}",
                    UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField,
                    UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].Id.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to build the request for GetInvestmentByMedia() : " + sql, err));
            }

            #region Construction de la requête

            sql += string.Format(" select {0},{1},date_cover_num date1", insertionFieldNameSumWithAlias, euroFieldNameSumWithAlias);
            sql += string.Format("  from {0}  {1}", WebApplicationParameters.GetDataTable(TableIds.dataPress, _webSession.IsSelectRetailerDisplay).Sql,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql += string.Format(" where id_media={0} ", _idMedia);
            if (_beginingDate.Length > 0)
                sql += string.Format(" and date_media_num>={0} ", _beginingDate);
            if (_endDate.Length > 0)
                sql += string.Format("  and date_media_num<={0} ", _endDate);
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;
            sql += " group by date_cover_num ";
            #endregion

            #region Execution de la requête
            try
            {
                DataSet ds = _webSession.Source.Fill(sql);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow current in ds.Tables[0].Rows)
                    {
                        var value1 = new string[2];
                        value1[0] = current[UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()].ToString();
                        value1[1] = current[UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].Id.ToString()].ToString();
                        htInvestment.Add(current["date1"], value1);
                    }
                }
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("error when getting investment by media", err));
            }
            #endregion

            return (htInvestment);
        }
        #endregion

        #region Structure

        #endregion

        #region Get dates list
        /// <summary>
        /// Get dates list
        /// </summary>
        /// <param name="conditionEndDate">Add condition end date</param>
        /// <returns>DataSet</returns>
        public virtual DataSet GetListDate(bool conditionEndDate, DBConstantes.TableType.Type tableType)
        {

            string tableName;
            try
            {
                switch (tableType)
                {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id,
                            WebConstantes.Module.Type.alert, _webSession.IsSelectRetailerDisplay);
                        break;
                    case DBConstantes.TableType.Type.dataVehicle:
                        tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id,
                            WebConstantes.Module.Type.analysis, _webSession.IsSelectRetailerDisplay);
                        break;
                    default:
                        throw (new PortofolioDALException("Table type unknown"));
                }
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Error when getting table name", err));
            }

            #region Construction de la requête
            var sql = new StringBuilder(500);

            sql.Append(" select distinct date_media_num ");

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress)
            {
                sql.Append(", disponibility_visual, number_page_media ");
                sql.Append(", date_cover_num ");
            }
            else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine)
            {
                sql.Append(", disponibility_visual , number_page_media ");
                sql.Append(", date_media_num as date_cover_num");
            }
            sql.Append(", media ");
            sql.Append(" from ");
            sql.Append(tableName);

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress)
            {
                sql.AppendFormat(",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).SqlWithPrefix);
                sql.AppendFormat(",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).SqlWithPrefix);
            }
            sql.AppendFormat(",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix);
            sql.AppendFormat(" where {0}.id_media={1}.id_media" + " ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix);
            sql.AppendFormat(" and {0}.id_media={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _idMedia);
            sql.AppendFormat(" and {0}.id_language={1} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                _webSession.DataLanguage);

            // Période			

            if (_beginingDate.Length > 0)
                sql.AppendFormat(" and {0}.date_media_num>={1}",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _beginingDate);
            if (_endDate.Length > 0 && conditionEndDate)
                sql.AppendFormat(" and {0}.date_media_num<={1}",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _endDate);

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress)
            {

                sql.AppendFormat(" and {0}.date_debut(+) = {1}.date_media_num ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.id_project(+) = {1} ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix, CstProject.ADEXPRESS_ID);
                sql.AppendFormat(" and {0}.id_media(+) = {1}.id_media ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.applicationMedia).Prefix,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.id_media=al.id_media(+)",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.date_media_num=al.DATE_ALARM(+)",
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                sql.AppendFormat(" and {0}.id_media(+)={1} ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _idMedia);
                sql.AppendFormat(" and {0}.ID_LANGUAGE_I(+)={1} ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _webSession.DataLanguage);
                sql.AppendFormat(" and  {0}.DATE_ALARM(+)>={1} ",
                    WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _beginingDate);
                if (_endDate.Length > 0 && conditionEndDate)
                    sql.AppendFormat(" and  {0}.DATE_ALARM(+)<={1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.alarmMedia).Prefix, _endDate);

            }
            // Tri			
            sql.AppendFormat(" order by {0}.date_media_num desc",
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

            #endregion

            #region Execution de la requête
            try
            {
                return (_webSession.Source.Fill(sql.ToString()));
            }
            catch (Exception err)
            {
                throw (new PortofolioDALException("Erreur lors de la sélection de la table", err));
            }
            #endregion

        }
        #endregion

        #region Is Media belong To Category
        /// <summary>
        /// Cheks if media belong to Category
        /// </summary>
        /// <param name="idMedia">Id media</param>
        /// <param name="idCategory">Id Category</param>
        /// <returns>True if media belong to Category</returns>
        public bool IsMediaBelongToCategory(Int64 idMedia, string idCategory)
        {

            StringBuilder t = new StringBuilder(1000);
            DataTable dt = null;

            t.AppendFormat(" select  {0}.id_media,{0}.media,{1}.id_category ",
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix);
            t.AppendFormat(" from  {0},{1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).SqlWithPrefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).SqlWithPrefix);
            t.AppendFormat(" " + ",{0} ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).SqlWithPrefix);
            t.Append(" where ");
            t.AppendFormat(" {0}.id_basic_media ={1}.id_basic_media ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix);
            t.AppendFormat(" and {0}.id_category = {1}.id_category ", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix,
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix);
            t.AppendFormat(" and {0}.id_category in ({1}) ",
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix, idCategory);
            t.AppendFormat(" and {0}.id_language = {1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category).Prefix, _webSession.DataLanguage);
            t.AppendFormat(" and {0}.id_language = {1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix,
                _webSession.DataLanguage);
            t.AppendFormat(" and {0}.id_language = {1}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia).Prefix,
                _webSession.DataLanguage);
            t.AppendFormat(" and {0}.id_media in ( {1})  ",
                WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media).Prefix, idMedia);
            t.Append("  order by media ");

            #region Execution de la requête
            try
            {
                dt = _webSession.Source.Fill(t.ToString()).Tables[0];

                if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0) return true;
                else return false;
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to know if the media belong to Category  : " + t, err));
            }
            #endregion

        }
        #endregion

        #endregion

        #region Methods

        #region Get Product Data
        /// <summary>
        /// Récupère la liste produit de référence
        /// </summary>
        /// <returns>la liste produit de référence</returns>
        protected virtual string GetProductData()
        {
            string sql = "";
            if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
                sql = _webSession.PrincipalProductUniverses[0].
                    GetSqlConditions(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);

            return sql;
        }
        #endregion

        #region Get Table Data
        /// <summary>
        /// Get Table
        /// </summary>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Throw when the vehicle is unknown
        /// </exception>
        /// <returns>Table name</returns>
        //protected virtual string GetTableData() {
        //    switch (_vehicleInformation.Id) {
        //        case DBClassificationConstantes.Vehicles.names.internationalPress:
        //            return DBConstantes.Tables.ALERT_DATA_PRESS_INTER;
        //        case DBClassificationConstantes.Vehicles.names.press:
        //            return DBConstantes.Tables.ALERT_DATA_PRESS;
        //        case DBClassificationConstantes.Vehicles.names.radio:
        //            return DBConstantes.Tables.ALERT_DATA_RADIO;
        //        case DBClassificationConstantes.Vehicles.names.tv:
        //        case DBClassificationConstantes.Vehicles.names.others:
        //            return DBConstantes.Tables.ALERT_DATA_TV;
        //        case DBClassificationConstantes.Vehicles.names.outdoor:
        //            return DBConstantes.Tables.ALERT_DATA_OUTDOOR;
        //        case DBClassificationConstantes.Vehicles.names.directMarketing:
        //            return DBConstantes.Tables.ALERT_DATA_MARKETING_DIRECT;
        //        default:
        //            throw new PortofolioDALException("GetTableData()-->Vehicle unknown.");
        //    }
        //}
        #endregion

        #region Get Select Data Ecran
        /// <summary>
        /// Get Select Data Ecran
        /// </summary>
        /// <returns>SQL</returns>
        protected virtual string GetSelectDataEcran()
        {
            var sql = string.Empty;
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.internet:
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                    sql += " select  distinct date_media_num, commercial_break, ID_COBRANDING_ADVERTISER";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " , NUMBER_spot_com_break nbre_spot";
                    sql += string.Format(" ,{0} ",
                        UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField);
                    break;

                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                    sql += "select  distinct date_media_num, id_commercial_break ";
                    sql += " ,duration_commercial_break as ecran_duration";
                    sql += " ,NUMBER_MESSAGE_COMMERCIAL_BREA nbre_spot ";
                    sql += string.Format(" ,{0} ",
                         UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].DatabaseField);
                    break;
                default:
                    throw new PortofolioDALException("getTable(DBClassificationConstantes.Vehicles.value idMedia)-->Vehicle unknown.");
            }
            return sql;
        }
        #endregion

        #region Get Table Data New Product
        /// <summary>
        /// Get Table Data New Product
        /// </summary>
        /// <returns>SQL</returns>
		protected virtual string getTableDataNewProduct()
        {
            string sql = "";
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.press:
                    sql += string.Format("{0} {1}",
                        WebApplicationParameters.GetDataTable(TableIds.dataPressAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    sql += string.Format("{0} {1}",
                        WebApplicationParameters.GetDataTable(TableIds.dataNewspaperAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.magazine:
                    sql += string.Format("{0} {1}",
                        WebApplicationParameters.GetDataTable(TableIds.dataMagazineAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.color).SqlWithPrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.format).SqlWithPrefix);
                    return sql;
                case DBClassificationConstantes.Vehicles.names.radio:
                    sql += string.Format("{0} {1}", WebApplicationParameters.GetDataTable(TableIds.dataRadioAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += " ," + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix;
                    return sql;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    sql += string.Format("{0} {1}",
                        WebApplicationParameters.GetDataTable(TableIds.dataTvAlert, _webSession.IsSelectRetailerDisplay).Sql,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql += string.Format(" ,{0}", WebApplicationParameters.DataBaseDescription.GetTable(TableIds.product).SqlWithPrefix);
                    return sql;
                default:
                    throw new PortofolioDALException(" Vehicle unknown.");
            }
        }
        #endregion

        #region Get excluded products
        /// <summary>
        /// Get excluded products
        /// </summary>
        /// <param name="sql">String builder</param>
        /// <returns></returns>
        protected virtual string GetExcludeProducts(string prefix)
        {
            // Exclude product 
            string sql = string.Empty;
            ProductItemsList prList = null; ;
            if (Product.Contains(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)
                && (prList = Product.GetItemsList(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID)) != null)
                sql = prList.GetExcludeItemsSql(true, prefix);
            return sql;
        }

        public virtual long CountData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
