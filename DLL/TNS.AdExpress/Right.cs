
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using WebCst=TNS.AdExpress.Constantes.Web;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.DataAccess;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Constantes.Customer.DB;
using TNS.AdExpress.Domain.Classification;



namespace TNS.AdExpress {
    /// <summary>
    /// AdExpress customer rights
    /// </summary>
    [System.Serializable]
    public class Right {

        #region Variables
        /// <summary>
        /// clé : type de liste dans la nomenclature en accès ou en exception 
        /// valeur : liste correspondant à la clé 
        /// </summary>
        protected Dictionary<CustomerCst.Right.type, string[]> _rights;
        /// <summary>
        /// Modules list
        /// </summary>
        protected Dictionary<Int64, Module> _modulesRights;
        /// <summary>
        /// hashtable : clé idFlag
        /// valeur : Flag
        /// </summary>
        protected Dictionary<Int64, string> _flagsRights;
        /// <summary>
        /// Module frequencies
        /// module id, frequency id
        /// </summary>
        [System.NonSerialized]
        protected Dictionary<Int64, Int64> _moduleFrequencies;
        /// <summary>
        /// ModuleAssignement Alerts AdExpress
        /// </summary>
        [System.NonSerialized]
        protected ModuleAssignment _moduleAssignmentAlertsAdExpress;
        /// <summary>
        /// Key : Right Banners Type
        /// valeur : Value List assignment
        /// </summary>
        protected Dictionary<CustomerCst.RightBanners.Type, Int64[]> _rightsBannersAssignementList;
        /// <summary>
        /// identifiant login
        /// </summary>		
        protected Int64 _loginId;
        /// <summary>
        /// login
        /// </summary>
        protected string _login;
        /// <summary>
        /// mot de passe
        /// </summary>
        protected string _password;
        /// <summary>
        /// Nbre de lignes dans la base de données que l'on compare
        /// avec les listes des droits clients
        /// </summary>
        protected int nbLineBD;
        /// <summary>
        /// Vérifie si les droits ont été déterminés
        /// </summary>
        protected bool rightDetermined;
        /// <summary>
        /// Indique si l'utilisateur a le droit de se connecter
        /// </summary>
        protected bool rightValidated;
        /// <summary>
        /// Chaîne de connection à la base de données
        /// </summary>
        protected string connectionString;
        /// <summary>
        /// bool indiquant si c'est la première connection au site
        /// true si première connection
        /// </summary>
        protected bool firstRequest = true;
        /// <summary>
        /// DB Connection
        /// </summary>
        [System.NonSerialized]
        private IDataSource _source = null;
        /// <summary>
        /// date de connection
        /// </summary>		
        protected DateTime _connectionDate;
        /// <summary>
        /// Date de modification des droits utilisateur
        /// </summary>
        protected DateTime _lastModificationDate;
        /// <summary>
        /// Site language
        /// </summary>
        protected int _siteLanguage = 33;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="login">Customer Login</param>
        /// <param name="password">Customer Password</param>
        public Right(string login,string password) {
            if(login==null || login.Length==0) throw (new ArgumentException("Invalid login parameter"));
            if(password==null || password.Length==0) throw (new ArgumentException("Invalid password parameter"));
            _login=login;
            _password=password;
            _connectionDate=DateTime.Now;
            try {
                _loginId=RightDAL.GetLoginId(Source,_login,_password);
            }
            catch(System.Exception err) {
                throw (new RightException("Impossible to access to the Database",err));
            }
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="login">Customer Login</param>
		/// <param name="password">Customer Password</param>
		/// <param name="sitelanguage">Site language</param>
		public Right(string login, string password,int sitelanguage) {
			if (login == null || login.Length == 0) throw (new ArgumentException("Invalid login parameter"));
			if (password == null || password.Length == 0) throw (new ArgumentException("Invalid password parameter"));
			_login = login;
			_password = password;
			_connectionDate = DateTime.Now;
			_siteLanguage = sitelanguage;
			try {
				
				_loginId = RightDAL.GetLoginId(Source, _login, _password);
			}
			catch (System.Exception err) {
				throw (new RightException("Impossible to access to the Database", err));
			}
		}
        #endregion

        #region Accessors
        /// <summary>
        /// Get Customer Data Source
        /// </summary>
        public IDataSource Source {
            get {
                if(_source==null){
					string nlsSort = "";
					if (WebApplicationParameters.AllowedLanguages.ContainsKey(long.Parse(_siteLanguage.ToString()))) {
						nlsSort = WebApplicationParameters.AllowedLanguages[long.Parse(_siteLanguage.ToString())].NlsSort;
					}
					_source = WebApplicationParameters.DataBaseDescription.GetCustomerConnection(_login, _password, nlsSort,CustomerConnectionIds.adexpr03);
                }
                return (_source);
            }
        }

        /// <summary>
        /// Retourne un  string correspondant aux éléments d'une liste représentant
        /// un type de droit (famille,media,annonceur...). Ces listes sont soient en accès soient en exception.
        /// </summary>
        /// <param name="typeRight">Choix d'une liste en accès ou en exception</param>
        /// <returns>string d'une liste</returns>
        public string this[CustomerCst.Right.type typeRight] {
            get {
                try {
                  
                    if(!_rights.ContainsKey(typeRight)||_rights[typeRight]==null) return ("");
                    string list="";
                    foreach(string currentItem in _rights[typeRight]) {
                        list+=currentItem+",";
                    }
                    return (list.Substring(0,list.Length-1));
                }
                catch(System.Exception e) {
                    throw (new RightException("Impossible to retreive right list",e));
                }
            }
        }

        /// <summary>
        /// Get login Id
        /// </summary>
        public Int64 IdLogin {
            get { return _loginId; }
        }

        /// <summary>
        /// Get login
        /// </summary>
        public string Login {
            get { return _login; }
        }

        /// <summary>
        /// Get password
        /// </summary>
        public string PassWord {
            get { return _password; }
        }


        #endregion

        #region AdExpress Access
        /// <summary>
        /// Vérifie l'existence du projet adExpress 
        /// avec au moins un module.
        /// Si true assigne idLogin
        /// </summary>
        /// <returns></returns>
        public bool CanAccessToAdExpress() {			
            return (RightDAL.CanAccessToAdExpressDB(Source,_loginId));

        }

        /// <summary>
        /// vérifie le Login-mot de passe
        /// </summary>
        /// <returns>true si login-mot de passe correct, false sinon</returns>
        public bool CheckLogin(IDataSource source) {
			
            _loginId=RightDAL.GetLoginId(source,_login,_password);
            if(_loginId==-1) return (false);
            return (true);

        } 
        #endregion

        #region Right last modification date
        /// <summary>
        /// Customer rights have been modified ?
        /// </summary>
        /// <returns>True if the rights have been modified</returns>
        protected bool isRightModifiedDB() {			
            _lastModificationDate=RightDAL.LastModificationDate(_source,_loginId);
            if(_lastModificationDate<_connectionDate)return false;
            return true;
        }
        #endregion

        #region Template exists
        /// <summary>
        /// Check if some product tempates exist
        /// </summary>
        /// <returns>True if some templates exist</returns>
        public bool IsProductTemplateExist() {
					
            return(RightDAL.IsProductTemplateExist(Source,_loginId));
        }

        /// <summary>
        /// Check if some media tempates exist
        /// </summary>
        /// <returns>True if some templates exist</returns>
        public  bool IsMediaTemplateExist() {
			
            return(RightDAL.IsMediaTemplateExist(Source,_loginId));
        }
        #endregion

        #region Set Rights
        /// <summary>
        /// Remplit les droits d'un utilisateur dans _rights
        /// </summary>
        /// <returns>_rights</returns>
        public void SetRights() {

            #region Variables
            DataSet ds;
            string vh="";
            string vehicleAccess="";
            string vehicleException="";
            string categoryAccess="";
            string categoryException="";
            string mediaAccess="";
            string mediaException="";
            string listVehicleForRecap="";
            _rights=new Dictionary<CustomerCst.Right.type,string[]>();
            #endregion

            #region Product Template
            if(IsProductTemplateExist()) {
				ds=RightDAL.GetProductTemplate(Source,_loginId);
                try {
                    if(ds!=null&&ds.Tables!=null&&ds.Tables[0]!=null&&ds.Tables[0].Rows!=null) {
                        foreach(DataRow row in ds.Tables[0].Rows) {
                            //sector en accès
                            if((Int64)row[2]==MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.sectorAccess,row[0].ToString().Split(','));
                            }
                            //sector en exception
                            if((Int64)row[2]==MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.sectorException,row[0].ToString().Split(','));
                            }
                            //subsector en accès
                            if((Int64)row[2]==MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.subSectorAccess,row[0].ToString().Split(','));
                            }
                            //subsector en exception
                            if((Int64)row[2]==MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.subSectorException,row[0].ToString().Split(','));
                            }
                            //Group en accès
                            if((Int64)row[2]==MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.groupAccess,row[0].ToString().Split(','));
                            }
                            //Group en exception
                            if((Int64)row[2]==MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.groupException,row[0].ToString().Split(','));
                            }
                            //Segment en accès
                            if((Int64)row[2]==MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.segmentAccess,row[0].ToString().Split(','));
                            }
                            //Segment en exception
                            if((Int64)row[2]==MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.segmentException,row[0].ToString().Split(','));
                            }
                            //Holding_company en accès
                            if((Int64)row[2]==MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.holdingCompanyAccess,row[0].ToString().Split(','));
                            }
                            //Holding_company en exception
                            if((Int64)row[2]==MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.holdingCompanyException,row[0].ToString().Split(','));
                            }
                            //advertiser en accès
                            if((Int64)row[2]==MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.advertiserAccess,row[0].ToString().Split(','));
                            }
                            //advertiser en exception
                            if((Int64)row[2]==MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.advertiserException,row[0].ToString().Split(','));
                            }
                        }
                    }
                }
                catch(System.Exception err) {
                    throw (new RightException("Impossible to load template product right",err));
                }
            }
            #endregion

            #region Customer rights
			 ds=RightDAL.GetProductRights(Source,_loginId);
            try {
                if(ds!=null&&ds.Tables!=null&&ds.Tables[0]!=null&&ds.Tables[0].Rows!=null) {
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        //sector en accès
                        if((Int64)row[2]==MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.sectorAccess)) {
                                _rights.Add(CustomerCst.Right.type.sectorAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.sectorAccess]=listValue(_rights[CustomerCst.Right.type.sectorAccess],row[0].ToString().Split(',')).Split(',');
                        }
                        //sector en exception
                        if((Int64)row[2]==MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.sectorException)) {
                                _rights.Add(CustomerCst.Right.type.sectorException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.sectorException]=listValue(_rights[CustomerCst.Right.type.sectorException],row[0].ToString().Split(',')).Split(',');
                        }
                        //subsector en accès
                        if((Int64)row[2]==MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {

                            if(!_rights.ContainsKey(CustomerCst.Right.type.subSectorAccess)) {
                                _rights.Add(CustomerCst.Right.type.subSectorAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.subSectorAccess]=listValue(_rights[CustomerCst.Right.type.subSectorAccess],row[0].ToString().Split(',')).Split(',');
                        }
                        //subsector en exception
                        if((Int64)row[2]==MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.subSectorException)) {
                                _rights.Add(CustomerCst.Right.type.subSectorException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.subSectorException]=listValue(_rights[CustomerCst.Right.type.subSectorException],row[0].ToString().Split(',')).Split(',');
                        }
                        //Group en accès
                        if((Int64)row[2]==MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.groupAccess)) {
                                _rights.Add(CustomerCst.Right.type.groupAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.groupAccess]=listValue(_rights[CustomerCst.Right.type.groupAccess],row[0].ToString().Split(',')).Split(',');
                        }
                        //Group en exception
                        if((Int64)row[2]==MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.groupException)) {
                                _rights.Add(CustomerCst.Right.type.groupException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.groupException]=listValue(_rights[CustomerCst.Right.type.groupException],row[0].ToString().Split(',')).Split(',');
                        }
                        //Segment en accès
                        if((Int64)row[2]==MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.segmentAccess)) {
                                _rights.Add(CustomerCst.Right.type.segmentAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.segmentAccess]=listValue(_rights[CustomerCst.Right.type.segmentAccess],row[0].ToString().Split(',')).Split(',');
                        }
                        //Segment en exception
                        if((Int64)row[2]==MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.segmentException)) {
                                _rights.Add(CustomerCst.Right.type.segmentException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.segmentException]=listValue(_rights[CustomerCst.Right.type.segmentException],row[0].ToString().Split(',')).Split(',');
                        }
                        //Holding_company en accès
                        if((Int64)row[2]==MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.holdingCompanyAccess)) {
                                _rights.Add(CustomerCst.Right.type.holdingCompanyAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.holdingCompanyAccess]=listValue(_rights[CustomerCst.Right.type.holdingCompanyAccess],row[0].ToString().Split(',')).Split(',');
                        }
                        //Holding_company en exception
                        if((Int64)row[2]==MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.holdingCompanyException)) {
                                _rights.Add(CustomerCst.Right.type.holdingCompanyException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.holdingCompanyException]=listValue(_rights[CustomerCst.Right.type.holdingCompanyException],row[0].ToString().Split(',')).Split(',');
                        }
                        //advertiser en accès
                        if((Int64)row[2]==MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.advertiserAccess)) {
                                _rights.Add(CustomerCst.Right.type.advertiserAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.advertiserAccess]=listValue(_rights[CustomerCst.Right.type.advertiserAccess],row[0].ToString().Split(',')).Split(',');
                        }
                        //advertiser en exception
                        if((Int64)row[2]==MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.advertiserException)) {
                                _rights.Add(CustomerCst.Right.type.advertiserException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.advertiserException]=listValue(_rights[CustomerCst.Right.type.advertiserException],row[0].ToString().Split(',')).Split(',');
                        }
                    }
                }
            }
            catch(System.Exception err) {
                throw (new RightException("Impossible to load product right",err));
            }
            #endregion

            #region Media Template
            if(IsMediaTemplateExist()) {
				ds=RightDAL.GetMediaTemplate(Source,_loginId);
                try {
                    if(ds!=null&&ds.Tables!=null&&ds.Tables[0]!=null&&ds.Tables[0].Rows!=null) {
                        foreach(DataRow row in ds.Tables[0].Rows) {
                            //Vehicle en accès
                            if((Int64)row[2]==MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                vh=row[0].ToString();
                                _rights.Add(CustomerCst.Right.type.vehicleAccess,vh.Split(','));
                                vehicleAccess=vh;
                                // On ajoute AdNetTrack si Internet est présent
                            }
                            //Vehicle en exception
                            if((Int64)row[2]==MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.vehicleException,row[0].ToString().Split(','));
                                vehicleException=row[0].ToString();
                            }
                            //Category en accès
                            if((Int64)row[2]==MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.categoryAccess,row[0].ToString().Split(','));
                                categoryAccess=row[0].ToString();
                            }
                            //Category en exception
                            if((Int64)row[2]==MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.categoryException,row[0].ToString().Split(','));
                                categoryException=row[0].ToString();
                            }
                            //média en accès
                            if((Int64)row[2]==MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.mediaAccess,row[0].ToString().Split(','));
                                mediaAccess=row[0].ToString();
                            }
                            //média en exception
                            if((Int64)row[2]==MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                                _rights.Add(CustomerCst.Right.type.mediaException,row[0].ToString().Split(','));
                                mediaException=row[0].ToString();
                            }

                        }
                    }
                }
                catch(System.Exception err) {
                    throw (new RightException("Impossible to load template media right",err));
                }
            }
            #endregion

            #region Customer media rights
			 ds=RightDAL.GetMediaRights(Source,_loginId);
            try {
                if(ds!=null&&ds.Tables!=null&&ds.Tables[0]!=null&&ds.Tables[0].Rows!=null) {
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        //Vehicle en accès
                        if((Int64)row[2]==MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.vehicleAccess)) {
                                _rights.Add(CustomerCst.Right.type.vehicleAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.vehicleAccess]=listValue((string[])_rights[CustomerCst.Right.type.vehicleAccess],row[0].ToString().Split(',')).Split(',');
                            if(vehicleAccess.Length>0) vehicleAccess+=","+row[0].ToString();
                            else vehicleAccess+=row[0].ToString();
                        }
                        //Vehicle en exception
                        if((Int64)row[2]==MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.vehicleException)) {
                                _rights.Add(CustomerCst.Right.type.vehicleException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.vehicleException]=listValue((string[])_rights[CustomerCst.Right.type.vehicleException],row[0].ToString().Split(',')).Split(',');
                            if(vehicleException.Length>0) vehicleException+=","+row[0].ToString();
                            else vehicleException=row[0].ToString();

                        }
                        //Category en accès
                        if((Int64)row[2]==MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.categoryAccess)) {
                                _rights.Add(CustomerCst.Right.type.categoryAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.categoryAccess]=listValue((string[])_rights[CustomerCst.Right.type.categoryAccess],row[0].ToString().Split(',')).Split(',');
                            if(categoryAccess.Length>0) categoryAccess+=","+row[0].ToString();
                            else categoryAccess=row[0].ToString();
                        }
                        //Category en exception
                        if((Int64)row[2]==MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.categoryException)) {
                                _rights.Add(CustomerCst.Right.type.categoryException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.categoryException]=listValue((string[])_rights[CustomerCst.Right.type.categoryException],row[0].ToString().Split(',')).Split(',');
                            if(categoryException.Length>0) categoryException+=","+row[0].ToString();
                            else categoryException=row[0].ToString();
                        }
                        //média en accès
                        if((Int64)row[2]==MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_NOT_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.mediaAccess)) {
                                _rights.Add(CustomerCst.Right.type.mediaAccess,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.mediaAccess]=listValue((string[])_rights[CustomerCst.Right.type.mediaAccess],row[0].ToString().Split(',')).Split(',');
                            if(mediaAccess.Length>0) mediaAccess+=","+row[0].ToString();
                            else mediaAccess=row[0].ToString();

                        }
                        //média en exception
                        if((Int64)row[2]==MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1])==ExceptionValues.IS_EXCEPTION) {
                            if(!_rights.ContainsKey(CustomerCst.Right.type.mediaException)) {
                                _rights.Add(CustomerCst.Right.type.mediaException,row[0].ToString().Split(','));
                            }
                            else _rights[CustomerCst.Right.type.mediaException]=listValue((string[])_rights[CustomerCst.Right.type.mediaException],row[0].ToString().Split(',')).Split(',');
                            if(mediaException.Length>0) mediaException+=","+row[0].ToString();
                            else mediaException=row[0].ToString();
                        }
                    }
                }
            }
            catch(System.Exception err) {
                throw (new RightException("Impossible to load media template right",err));
            }
            #endregion

            #region Recap
            try {
                IDataSource recapSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
                ds = RightDAL.GetProductClassAnalysisRights(recapSource, _rights);
                if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows != null) {
                    foreach (DataRow row in ds.Tables[0].Rows) listVehicleForRecap += row[0] + ",";
                    if (listVehicleForRecap.Length > 0) {
                        _rights.Add(CustomerCst.Right.type.vehicleAccessForRecap, listVehicleForRecap.Substring(0, listVehicleForRecap.Length - 1).Split(','));
                    }
                }
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to load recap right", err));
            }
            #endregion

        }
        #endregion

        #region Module rights
        /// <summary>
        /// Clear Modules list
        /// </summary>
        public void ClearModulesList() {
            _modulesRights.Clear();
        }

        /// <summary>
        /// Set module rights
        /// </summary>
        public void SetModuleRights() {
            DataSet ds;
            try {
				ds=RightDAL.GetModulesRights(Source,_loginId);
                _modulesRights=new Dictionary<Int64,Module>();
                if(ds!=null&&ds.Tables!=null&&ds.Tables[0]!=null&&ds.Tables[0].Rows!=null) {
                    foreach(DataRow row in ds.Tables[0].Rows) {
                        _modulesRights.Add((Int64)row[1],ModulesList.GetModule((Int64)row[1]));
                    }
                }
            }
            catch(System.Exception err) {
				throw (new RightException("Impossible to retreive module rights", err));
            }
        }
        /// <summary>
        /// Get Modules list
        /// </summary>
        /// <returns>modules lists</returns>
        public DataTable GetCustomerModuleListHierarchy() {
            DataSet ds=null;
            try {
                if(_modulesRights==null) SetModuleRights();
				ds=RightDAL.GetModulesRights(Source,_loginId);
                if(ds!=null && ds.Tables[0]!=null && ds.Tables[0].Rows!=null) {
                    ds.Tables[0].TableName="listModule";
                    ds.Tables[0].Columns[0].ColumnName="idGroupModule";
                    ds.Tables[0].Columns[1].ColumnName="idModule";
                    ds.Tables[0].Columns[2].ColumnName="idModuleCategory";
                }
                else throw (new RightException("Impossible to load module classification"));
            }
            catch(System.Exception err) {
				throw (new RightException("Impossible to build DataTable module hierarchy", err));
            }
            return (ds.Tables[0]);
        }

        /// <summary>
        /// Get Module
        /// </summary>
        /// <param name="moduleId">module Id</param>
        /// <returns>Module</returns>
        public Module GetModule(Int64 moduleId) {
            try {
                if(_modulesRights==null||_modulesRights.Keys.Count==0) SetModuleRights();
                if(_modulesRights.ContainsKey(moduleId)) return (_modulesRights[moduleId]);
                return (null);
            }
            catch(System.Exception err) {
                throw (new RightException("Impossible to build DataTable module hierarchy",err));
            }
        }
        #endregion

        #region Frequency
        /// <summary>
        /// Set Module frequencies
        /// </summary>
        private void SetModuleFrequencies() {
            DataSet ds;
            try {
                 ds = RightDAL.GetModuleFrequencies(Source, _loginId);
                _moduleFrequencies = new Dictionary<Int64, Int64>();
                if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows != null) {
                    foreach (DataRow row in ds.Tables[0].Rows) {
                        _moduleFrequencies.Add((Int64)row[0], (Int64)row[1]);
                    }
                }
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to retreive module frequencies", err));
            }
        }

        /// <summary>
        /// Méthode pour récupérer la fréquence d'un module
        /// </summary>
        /// <param name="moduleId">identifiant du module</param>
        /// <returns>Valeur de la fréquence</returns>
        public Int64 GetIdFrequency(Int64 moduleId) {
            try {
                if (_moduleFrequencies == null) SetModuleFrequencies();
                if (_moduleFrequencies[moduleId] != null) {
                    return _moduleFrequencies[moduleId];
                }
                else return Frequency.DEFAULT;

            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to retreive module frequency", err));
            }
        }
        #endregion

        #region ModuleAssignmentAlertsAdExpress
        /// <summary>
        /// Set Module frequencies
        /// </summary>
        private void SetModuleAssignmentAlertsAdExpress() {
            DataSet ds;
            try {
               ds = RightDAL.GetModuleAssignment(Source, _loginId);
                _moduleAssignmentAlertsAdExpress = null;
                if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows != null) {
                    foreach (DataRow row in ds.Tables[0].Rows) {
                        if(WebCst.Module.Name.ALERT_ADEXPRESS == (Int64)row[0])
                            _moduleAssignmentAlertsAdExpress = new ModuleAssignment((Int64)row[0], (DateTime)row[1], (DateTime)row[2], (Int64)row[3], ((row[4].ToString().Length > 0) ? (Int64)row[4] : -1));
                    }
                    if (_moduleAssignmentAlertsAdExpress == null)
                        _moduleAssignmentAlertsAdExpress = new ModuleAssignment(WebCst.Module.Name.ALERT_ADEXPRESS, DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-10), 0, -1);
                }
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to build SetModuleAssignmentAlertsAdExpress", err));
            }
        }


        /// <summary>
        /// HasModuleAssignment (A utiliser seulement pour les Alertes AdExpress !!!)
        /// </summary>
        /// <param name="moduleId">module Id</param>
        /// <returns>boolean</returns>
        public bool HasModuleAssignmentAlertsAdExpress() {
            try {
                if (_moduleAssignmentAlertsAdExpress == null) SetModuleAssignmentAlertsAdExpress();
                if (_moduleAssignmentAlertsAdExpress != null && _moduleAssignmentAlertsAdExpress.NbAlert>=0) return true;
                return (false);
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to build HasModuleAssignmentAlertsAdExpress", err));
            }
        }

        /// <summary>
        /// IsModuleAssignmentValidDate (A utiliser seulement pour les Alertes AdExpress !!!)
        /// </summary>
        /// <param name="moduleId">module Id</param>
        /// <returns>boolean</returns>
        public bool IsModuleAssignmentValidDateAlertsAdExpress() {
            try {
                if (_moduleAssignmentAlertsAdExpress == null) SetModuleAssignmentAlertsAdExpress();
                if (_moduleAssignmentAlertsAdExpress != null && _moduleAssignmentAlertsAdExpress.NbAlert >= 0 && _moduleAssignmentAlertsAdExpress.DateBegin < DateTime.Now && _moduleAssignmentAlertsAdExpress.DateEnd > DateTime.Now) return true;
                return (false);
            }
            catch (System.Exception err) {
                throw (new RightException("Impossible to build IsModuleAssignmentValidDateAlertsAdExpress", err));
            }
        }

        /// <summary>
        /// Méthode pour récupérer le nb d'alerte maximale de l'utilisateur (A utiliser seulement pour les Alertes AdExpress !!!)
        /// </summary>
        /// <param name="moduleId">identifiant du module</param>
        /// <returns>Nb Alerts</returns>
        public Int64 GetNbAlertsAdExpress() {
            try {
                if (_moduleAssignmentAlertsAdExpress == null) SetModuleAssignmentAlertsAdExpress();
                if (_moduleAssignmentAlertsAdExpress!=null && _moduleAssignmentAlertsAdExpress.NbAlert >= 0) {
                    return _moduleAssignmentAlertsAdExpress.NbAlert;
                }
                else return 0;

        }
            catch (System.Exception err) {
                throw (new RightException("Impossible to Get Nb Alerts AdExpress", err));
            }
        }
        #endregion

        #region Banners Format Assignement
        /// <summary>
        /// Set Banners Format Assignement
        /// </summary>
        public void SetBannersAssignement() {
            try {
                _rightsBannersAssignementList = new Dictionary<CustomerCst.RightBanners.Type, Int64[]>();
                var ds = RightDAL.GetBannersFormatAssignement(Source, _loginId);
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows != null) {
                    foreach (DataRow row in ds.Tables[0].Rows)
                        _rightsBannersAssignementList.Add(CustomerCst.RightBanners.Type.FormatEvaliantInternet, new List<string>(row[0].ToString().Split(',')).ConvertAll<Int64>(Int64.Parse).ToArray());
                }
            }
            catch (Exception err) {
                throw (new RightException("Impossible to build SetBannersFormatAssignement", err));
            }
        }
        /// <summary>
        /// Get Banners Format Assignement 
        /// </summary>
        /// <returns>Format identifier list</returns>
        public List<Int64> GetBannersFormatAssignement(List<Constantes.Customer.RightBanners.Type> typeRightBannersList) {
            try
            {
                var rightFormatList = new List<Int64>();
                if (_rightsBannersAssignementList == null) SetBannersAssignement();
                if (_rightsBannersAssignementList != null)
                {
                    foreach (var type in typeRightBannersList)
                    {
                        if(_rightsBannersAssignementList.ContainsKey(type))
                        {
                            foreach (var cFormatIdList in _rightsBannersAssignementList[type])
                            {
                                if(!rightFormatList.Contains(cFormatIdList))
                                    rightFormatList.Add(cFormatIdList);
                            }
                        }
                    }
                }
                return rightFormatList;
            }
            catch (Exception err) {
                throw (new RightException("Impossible to Get Banners Format Assignement", err));
            }
        }
        #endregion

        #region Flags rights

		/// <summary>
		/// Set flags rights
		/// </summary>
		public void SetFlagsRights() {			
			DataSet ds;
			try {
				ds = RightDAL.GetFlagsRights(Source, _loginId);
				_flagsRights = new Dictionary<Int64, string>();
				if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows != null) {
					foreach (DataRow row in ds.Tables[0].Rows) {
						_flagsRights.Add((Int64)row[0], row[1].ToString());
					}
				}
			}
			catch (System.Exception err) {
				throw (new RightException("Impossible to retreive flags rights", err));
			}
		}		

		/// <summary>
		/// Indicate if the customer have access to a flag
		/// </summary>
		/// <param name="flagId">Flag id</param>
		/// <returns>True if the customer have access to the flag</returns>
		public bool CustormerFlagAccess(Int64 flagId) {
			try {				
				
				if(!Domain.AllowedFlags.ContainFlag(flagId))return true;
				if (_flagsRights == null) SetFlagsRights();
				if (_flagsRights.ContainsKey(flagId)) return (true);
				return (false);
			}
			catch (System.Exception err) {
				throw (new RightException("Impossible to retreive flags rights",err));
			}
		}

        #region Media agency / Flag
       
        /// <summary>
        /// Verify if vehicles Id list have all media agencies flags
        /// </summary>
        /// <param name="vehicleIds">vehicle Ids</param>
        /// <returns>True if vehicles Id list have all media agencies flags</returns>
        public bool CustomerMediaAgencyFlagAccess(List<Int64> vehicleIds)
        {
            try
            {
                if (VehiclesInformation.ContainsMediaAgencyFlag(vehicleIds))
                {
                    List<Int64> ids = VehiclesInformation.GetMediaAgencyFlagIds(vehicleIds);
                    for (int i = 0; i < ids.Count; i++)
                    {
                        if(!Domain.AllowedFlags.ContainFlag(ids[i])) throw new RightException(" Media Agency flag "+ ids[i].ToString() +" must be defined in both files Vehicles.XML and Flags.xml" );
                        if (_flagsRights == null || _flagsRights.Count == 0 
                            || !_flagsRights.ContainsKey(ids[i]))
                             return (false);                       
                    }
                    return true;
                }
                return false;
            }
            catch (System.Exception err)
            {
                throw (new RightException("Impossible to retreive Media Agency flags rights", err));
            }
        }
        /// <summary>
        /// Verify if vehicles Id list have all media agencies flags
        /// </summary>
        /// <param name="vehicleId">vehicle Id</param>
        /// <returns>True if vehicles Id list have all media agencies flags</returns>
        public bool CustomerMediaAgencyFlagAccess(Int64 vehicleId)
        {
            try
            {
                if (VehiclesInformation.ContainsMediaAgencyFlag(vehicleId))
                {
                   Int64 id = VehiclesInformation.GetMediaAgencyFlagId(vehicleId);
                   if (id != long.MinValue)
                    {
                        if (!Domain.AllowedFlags.ContainFlag(id)) throw new RightException(" Media Agency flag " + id.ToString() + " must be defined in both files Vehicles.XML and Flags.xml");
                        if (_flagsRights == null || _flagsRights.Count == 0
                            || !_flagsRights.ContainsKey(id))
                            return (false);
                    }
                    return true;
                }
                return false;
            }
            catch (System.Exception err)
            {
                throw (new RightException("Impossible to retreive Media Agency flags rights", err));
            }
        }

        /// <summary>
        /// Check if customer has at least one media agency flag
        /// </summary>
        /// <returns>True if customer has at least one media agency flag</returns>
        public bool HasAtLeastOneMediaAgencyFlag()
        {
            if (_flagsRights != null || _flagsRights.Count > 0)
            {
                List<Int64> ids = VehiclesInformation.GetAllMediaAgencyFlagIds();
                for (int i = 0; i < ids.Count; i++)
                {
                    if(_flagsRights.ContainsKey(ids[i]))return true;
                }
            }
            return false;
        }
        #endregion

        #region Creation flag / vehicle
        /// <summary>
        /// Indicate if the customer can access to the creative of a specific vehicle
        /// </summary>
        /// <param name="vehicleId">vehicle id</param>
        /// <returns>True, if the customer has access</returns>
        public bool ShowCreatives(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleId) {			
			switch (vehicleId) {
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper:
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.magazine:
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_PRESS_CREATION_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_PRESS_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_PRESS_CREATION_ACCESS_FLAG] != null);
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_RADIO_CREATION_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_RADIO_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_RADIO_CREATION_ACCESS_FLAG] != null);
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_TV_CREATION_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_TV_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_TV_CREATION_ACCESS_FLAG] != null);
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_DETAIL_INTERNET_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_DETAIL_INTERNET_ACCESS_FLAG) && _flagsRights[Flags.ID_DETAIL_INTERNET_ACCESS_FLAG] != null);
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG] != null);
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.instore:
                    if (!Domain.AllowedFlags.ContainFlag(Flags.ID_INSTORE_CREATION_ACCESS_FLAG)) return true;
                    return (_flagsRights.ContainsKey(Flags.ID_INSTORE_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_INSTORE_CREATION_ACCESS_FLAG] != null);
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG] != null);
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_OTHERS_CREATION_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_OTHERS_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_OTHERS_CREATION_ACCESS_FLAG] != null);
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
					if (!Domain.AllowedFlags.ContainFlag(Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG)) return true;
					return (_flagsRights.ContainsKey(Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG) && _flagsRights[Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG] != null);
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                    if (!Domain.AllowedFlags.ContainFlag(Flags.ID_DETAIL_INTERNET_ACCESS_FLAG)) return true;
                    return (_flagsRights.ContainsKey(Flags.ID_DETAIL_INTERNET_ACCESS_FLAG) && _flagsRights[Flags.ID_DETAIL_INTERNET_ACCESS_FLAG] != null);
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                    if (!Domain.AllowedFlags.ContainFlag(Flags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG)) return true;
                    return (_flagsRights.ContainsKey(Flags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG) && _flagsRights[Flags.ID_DETAIL_EVALIANT_MOBILE_ACCESS_FLAG] != null);
                default:
					return (false);
			}
		}
        #endregion
        #endregion

        #region Recap Access Vehicle list
        /// <summary>
        /// Get vehicle list for product class analysis
        /// </summary>
        /// <remarks>A vehicle is accessible so at least an element from this vehicle is accessible.</remarks>
        /// <returns>vehicle list</returns>
        public string GetProductClassVehicleList() {
            string listVehicle="";
            if(this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap]!=null) {
                listVehicle=this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap];
            }
            return (listVehicle);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Retreive string (addition tab1 plus tab2)
        /// </summary>
        /// <param name="tab1">right string table</param>
        /// <param name="tab2">right string table</param>
        /// <returns>string</returns>
        protected string listValue(string[] tab1,string[] tab2) {

            string res="";
            int i=0;
            int j=0;
            bool notExist=true;
            for(i=0;i<tab1.Length;i++) {
                res+=tab1[i]+",";
            }
            for(i=0;i<tab2.Length;i++) {
                notExist=true;
                for(j=0;j<tab1.Length;j++) {
                    if(tab1[j]==tab2[i]) {
                        notExist=false;
                    }
                }
                if(notExist) {
                    res+=tab2[i]+",";
                }
            }
            return (res.Substring(0,res.Length-1));
        }
        #endregion
    }
}
