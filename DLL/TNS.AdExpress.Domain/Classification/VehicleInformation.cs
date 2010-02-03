#region Information
//  Author : Y. R'kaina
//  Creation  date: 06/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Units; 
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.Classification {
    /// <summary>
    /// Vehicle description
    /// </summary>
    public class VehicleInformation {

        #region Variables
        /// <summary>
        /// Vehicle id
        /// </summary>
        private Vehicles.names _id;
        /// <summary>
        /// Data base id
        /// </summary>
        private Int64 _databaseId;
        /// <summary>
        /// Show insertions
        /// </summary>
        private bool _showInsertions;
        /// <summary>
        /// Show creations
        /// </summary>
        private bool _showCreations;
        /// <summary>
        /// Show active media
        /// </summary>
        private bool _showActiveMedia;
        /// <summary>
        /// True if we need to calculate the last available date for this vehicle
        /// </summary>
        private bool _needLastAvailableDate;
        /// <summary>
        /// Allowed units list
        /// </summary>
        private AllowUnits _allowedUnitsList;
        /// <summary>
        /// Allowed media level items list
        /// </summary>
        private List<DetailLevelItemInformation.Levels> _allowedMediaLevelItemsList;
        /// <summary>
        /// Default media selection parent
        /// </summary>
        private DetailLevelItemInformation.Levels _defaultMediaSelectionParent;
        /// <summary>
        /// Media selection parents list
        /// </summary>
        private List<DetailLevelItemInformation.Levels> _mediaSelectionParentsList;
        /// <summary>
        /// Detail column id
        /// </summary>
        private Int64 _detailColumnId;
		/// <summary>
		/// Allowed recap media level items list
		/// </summary>
		private List<DetailLevelItemInformation.Levels> _allowedRecapMediaLevelItemsList;
        /// <summary>
		/// Allowed selection media level items list
        /// <remarks>Used in generic component</remarks>
        /// </summary>
		private List<DetailLevelItemInformation> _allowedMediaSelectionLevelItemsList = new List<DetailLevelItemInformation>();
        /// <summary>
        /// Media selection default detail level list
        /// <remarks>Used in generic component</remarks>
        /// </summary>
		private List<TNS.AdExpress.Domain.Level.GenericDetailLevel> _defaultMediaSelectionDetailLevels = new List<TNS.AdExpress.Domain.Level.GenericDetailLevel>();

		/// <summary>
		/// Media selection default detail level
		/// <remarks>Used in generic component</remarks>
		/// </summary>
		private TNS.AdExpress.Domain.Level.GenericDetailLevel _defaultMediaSelectionDetailLevel = null;
		/// <summary>
		/// Allowed autopromo
		/// </summary>
		private bool _autopromo=false;
		/// <summary>
		/// Allowed universe levels
		/// </summary>
		private List<long> _allowedUniverseLevels = new List<long>();

        /// <summary>
        /// Trends Media selection default detail level
        /// </summary>
        private TNS.AdExpress.Domain.Level.GenericDetailLevel _trendsDefaultMediaSelectionDetailLevel = null;
        /// <summary>
        /// Determine if show trends weekly selection
        /// </summary>
        private bool _showTrendsWeeklySelection = false;

        /// <summary>
        /// Determine if show trends monthly selection
        /// </summary>
        private bool _showTrendsMonthlySelection = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Vehicle id</param>
        /// <param name="databaseId">Data base id</param>
        /// <param name="showInsertions">Show insertions</param>
        /// <param name="showCreations">Show creations</param>
        /// <param name="showActiveMedia">Show avtive media</param>
        /// <param name="needLastAvailableDate">True if we need to calculate the last available date for this vehicle</param>
        /// <param name="allowedUnitsList">Allowed units list</param>
        /// <param name="allowedMediaLevelItemsList">Allowed media level items list</param>
        /// <param name="mediaSelectionParentsList">Media selection parents list</param>
        /// <param name="detailColumnId">Detail column id</param>
        public VehicleInformation(string id, 
                                  Int64 databaseId, 
                                  bool showInsertions, 
                                  bool showCreations,
                                  bool showActiveMedia,
                                  bool needLastAvailableDate,
                                  AllowUnits allowedUnitsList, 
                                  List<DetailLevelItemInformation.Levels> allowedMediaLevelItemsList,
                                  string defaultMediaSelectionParent,
                                  List<DetailLevelItemInformation.Levels> mediaSelectionParentsList, 
                                  Int64 detailColumnId) {

            if (id == null || id.Length == 0) throw (new ArgumentException("Invalid paramter vehicle id"));
            if (allowedUnitsList == null) throw (new ArgumentException("Invalid paramter allowed units list"));
            else _allowedUnitsList = allowedUnitsList;
            _allowedMediaLevelItemsList = allowedMediaLevelItemsList;
            _mediaSelectionParentsList = mediaSelectionParentsList;
            try {
                _id = (Vehicles.names)Enum.Parse(typeof(Vehicles.names), id, true);
                if(defaultMediaSelectionParent.Length>0)
                    _defaultMediaSelectionParent = (DetailLevelItemInformation.Levels)Enum.Parse(typeof(DetailLevelItemInformation.Levels), defaultMediaSelectionParent, true);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("Invalid parameter  vehicle id", err));
            }
            _databaseId = databaseId;
            _showInsertions = showInsertions;
            _showCreations = showCreations;
            _showActiveMedia = showActiveMedia;
            _needLastAvailableDate = needLastAvailableDate;
            _detailColumnId = detailColumnId;

        }
		 /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Vehicle id</param>
        /// <param name="databaseId">Data base id</param>
        /// <param name="showInsertions">Show insertions</param>
        /// <param name="showCreations">Show creations</param>
        /// <param name="needLastAvailableDate">True if we need to calculate the last available date for this vehicle</param>
        /// <param name="allowedUnitsList">Allowed units list</param>
        /// <param name="allowedMediaLevelItemsList">Allowed media level items list</param>
        /// <param name="mediaSelectionParentsList">Media selection parents list</param>
        /// <param name="detailColumnId">Detail column id</param>
		/// <param name="allowedRecapMediaLevelItemsList">Allowed recap media level items list</param>
		public VehicleInformation(string id,
								  Int64 databaseId,
								  bool showInsertions,
								  bool showCreations,
                                  bool showActiveMedia,
                                  bool needLastAvailableDate,
                                  AllowUnits allowedUnitsList,
								  List<DetailLevelItemInformation.Levels> allowedMediaLevelItemsList,
								  string defaultMediaSelectionParent,
								  List<DetailLevelItemInformation.Levels> mediaSelectionParentsList,
								  Int64 detailColumnId,List<DetailLevelItemInformation.Levels> allowedRecapMediaLevelItemsList):
            this(id, databaseId, showInsertions, showCreations, showActiveMedia, needLastAvailableDate, allowedUnitsList, allowedMediaLevelItemsList, defaultMediaSelectionParent, mediaSelectionParentsList, detailColumnId) {
			_allowedRecapMediaLevelItemsList = allowedRecapMediaLevelItemsList;
		}
        #endregion

        #region Accessors
        /// <summary>
        /// Get vehicle id
        /// </summary>
        public Vehicles.names Id {
            get { return _id; }
        }
        /// <summary>
        /// Get database id
        /// </summary>
        public Int64 DatabaseId {
            get { return _databaseId; }
        }
        /// <summary>
        /// Get show insertions authorization
        /// </summary>
        public bool ShowInsertions {
            get { return _showInsertions; }
        }
        /// <summary>
        /// Get show creations authorization
        /// </summary>
        public bool ShowCreations {
            get { return _showCreations; }
        }
        /// <summary>
        /// Get if we need to calculate the last available date for this vehicle
        /// </summary>
        public bool ShowActiveMedia {
            get { return _showActiveMedia; }
        }
        /// <summary>
        /// Get show active media
        /// </summary>
        public bool NeedLastAvailableDate {
            get { return _needLastAvailableDate; }
        }
        /// <summary>
        /// Get allowed units enum list
        /// </summary>
        public List<CustomerSessions.Unit> AllowedUnitEnumList {
            get { return _allowedUnitsList.AllowUnitList; }
        }
        /// <summary>
        /// Get allowed UnitInformation list
        /// </summary>
        public List<UnitInformation> AllowedUnitInformationList {
            get {
                List<UnitInformation> list = new List<UnitInformation>();
                foreach (CustomerSessions.Unit currentUnit in _allowedUnitsList.AllowUnitList)
                    list.Add(UnitsInformation.Get(currentUnit));
                return list;
            }
        }
        /// <summary>
        /// Get allowed base UnitInformation list
        /// </summary>
        public List<CustomerSessions.Unit> AllowedBaseUnitInformationList {
            get {
                List<CustomerSessions.Unit> list = new List<CustomerSessions.Unit>();
                UnitInformation unit;
                foreach (CustomerSessions.Unit currentUnit in _allowedUnitsList.AllowUnitList) {
                    unit = UnitsInformation.Get(currentUnit);
                    if (unit.BaseId != CustomerSessions.Unit.none)
                        list.Add(unit.BaseId);
                    else
                        list.Add(unit.Id);
                }
                return list;
            }
        }

        /// <summary>
        /// Get allowed Unit list
        /// </summary>
        public AllowUnits AllowUnits {
            get {return _allowedUnitsList;}
        }

        /// <summary>
        /// Get allowed media level items enum list
        /// </summary>
        public List<DetailLevelItemInformation.Levels> AllowedMediaLevelItemsEnumList {
            get { return _allowedMediaLevelItemsList; }
        }
		/// <summary>
		/// Get allowed recap media level items enum list
		/// </summary>
		public List<DetailLevelItemInformation.Levels> AllowedRecapMediaLevelItemsEnumList {
			get { return _allowedRecapMediaLevelItemsList; }
		}
		
        /// <summary>
        /// Get allowed media level items information list
        /// </summary>
        public List<DetailLevelItemInformation> AllowedMediaLevelItemsInformationList {
            get {
                List<DetailLevelItemInformation> list = new List<DetailLevelItemInformation>();
                foreach(DetailLevelItemInformation.Levels currentLevel in _allowedMediaLevelItemsList)
                    list.Add(DetailLevelItemsInformation.Get(currentLevel.GetHashCode()));
                return list;
            }
        }
		/// <summary>
		/// Get allowed recap media level items information list
		/// </summary>
		public List<DetailLevelItemInformation> AllowedRecapMediaLevelItemsInformationList {
			get {
				List<DetailLevelItemInformation> list = new List<DetailLevelItemInformation>();
				foreach (DetailLevelItemInformation.Levels currentLevel in _allowedRecapMediaLevelItemsList)
					list.Add(DetailLevelItemsInformation.Get(currentLevel.GetHashCode()));
				return list;
			}
		}
        /// <summary>
        /// Get Default media selection parent
        /// </summary>
        public DetailLevelItemInformation.Levels DefaultMediaSelectionParent {
            get { return _defaultMediaSelectionParent; }
        }
        /// <summary>
        /// Get media selection parents items enum list
        /// </summary>
        public List<DetailLevelItemInformation.Levels> MediaSelectionParentsItemsEnumList {
            get { return _mediaSelectionParentsList; }
        }
        /// <summary>
        /// Get media selection parents items information list
        /// </summary>
        public List<DetailLevelItemInformation> MediaSelectionParentsItemsInformationList {
            get {
                List<DetailLevelItemInformation> list = new List<DetailLevelItemInformation>();
                foreach (DetailLevelItemInformation.Levels currentLevel in _mediaSelectionParentsList)
                    list.Add(DetailLevelItemsInformation.Get(currentLevel.GetHashCode()));
                return list;
            }
        }
        /// <summary>
        /// Get detail column id
        /// </summary>
        public Int64 DetailColumnId {
            get { return _detailColumnId; }
        }

		/// <summary>
		/// Get /Set Media selection parents list
		/// <remarks>Used in generic component</remarks>
		/// </summary>
		public List<DetailLevelItemInformation> AllowedMediaSelectionLevelItemsList {
			get { return _allowedMediaSelectionLevelItemsList; }
			set { _allowedMediaSelectionLevelItemsList = value; }
		}
		/// <summary>
		/// Get /set Media selection default levels list
		/// <remarks>Used in generic component</remarks>
		/// </summary>
		public List<TNS.AdExpress.Domain.Level.GenericDetailLevel> DefaultMediaSelectionDetailLevels {
			get { return _defaultMediaSelectionDetailLevels; }
			set { _defaultMediaSelectionDetailLevels = value; }
		}
		/// <summary>
		/// Get /Set Media selection default detail level
		/// <remarks>Used in generic component</remarks>
		/// </summary>
		public TNS.AdExpress.Domain.Level.GenericDetailLevel DefaultMediaSelectionDetailLevel {
			get { return _defaultMediaSelectionDetailLevel; }
			set { _defaultMediaSelectionDetailLevel = value; }
		}
        /// <summary>
        /// Get /Set Trends Media selection default detail level
        /// <remarks>Used in generic component</remarks>
        /// </summary>
        public TNS.AdExpress.Domain.Level.GenericDetailLevel TrendsDefaultMediaSelectionDetailLevel
        {
            get { return _trendsDefaultMediaSelectionDetailLevel; }
            set { _trendsDefaultMediaSelectionDetailLevel = value; }
        }
			 /// <summary>
        /// Get auto promo authorization
        /// </summary>
        public bool Autopromo {
            get { return _autopromo; }
			set { _autopromo = value; }
        }
		
		/// <summary>
		/// Get /Set allowed universe levels
		/// <remarks>Used in generic component of universe media ou product selection</remarks>
		/// </summary>
		public List<long> AllowedUniverseLevels {
			get { return _allowedUniverseLevels; }
			set { _allowedUniverseLevels = value; }
		}
        /// <summary>
        /// Get if Show Trends Monthly Selection 
        /// </summary>
        public bool ShowTrendsMonthlySelection
        {
            get { return _showTrendsMonthlySelection; }
            set { _showTrendsMonthlySelection = value; }
        }
        /// <summary>
        /// Get if Show Trends weekly Selection 
        /// </summary>
        public bool ShowTrendsWeeklySelection
        {
            get { return _showTrendsWeeklySelection; }
            set { _showTrendsWeeklySelection = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get unit from specified baseId unit
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <returns>if baseId exist return baseId or unit if not</returns>
        public CustomerSessions.Unit GetUnitFromBaseId(CustomerSessions.Unit baseIdUnit){

            foreach (UnitInformation currentUnit in AllowedUnitInformationList)
                if (currentUnit.BaseId == baseIdUnit)
                    return currentUnit.Id;

            return baseIdUnit;
        }
        #endregion

    }
}
