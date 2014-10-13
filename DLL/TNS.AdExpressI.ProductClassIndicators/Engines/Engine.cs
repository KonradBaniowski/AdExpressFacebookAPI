#region Information
/*
 * Author : G Ragneau
 * Created on : 17/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Classification.Universe;

using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.Date.DAL;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;



namespace TNS.AdExpressI.ProductClassIndicators.Engines
{
    /// <summary>
    /// Define default behaviours of engines like global process, lengendaries, properties...
    /// </summary>
    public abstract class Engine
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Type of output
        /// </summary>
        protected bool _excel = false;
        /// <summary>
        /// Evolution
        /// </summary>
        protected bool _evolution = true;
        /// <summary>
        /// Type of output
        /// </summary>
        protected bool _pdf = false;
        /// <summary>
        /// Data Access Layer
        /// </summary>
        protected IProductClassIndicatorsDAL _dalLayer = null;

        #region Rules Engine attributes
        /// <summary>
        /// Specify if the table contains advertisers as references or competitors
        /// </summary>
        protected int _isPersonalized = 0;
        /// <summary>
        /// List of advertiser as references
        /// </summary>
        protected List<long> _referenceIDS = new List<long>();
        /// <summary>
        /// List of advertisers as competitors
        /// </summary>
        protected List<long> _competitorIDS = new List<long>();
        /// <summary>
        /// Begin of the period
        /// </summary>
        protected DateTime _periodBegin;
        /// <summary>
        /// End of the period
        /// </summary>
        protected DateTime _periodEnd;
        /// <summary>
        /// Year ID
        /// </summary>
        protected int _iYearID = 0;
		/// <summary>
		/// Year ID N-1
		/// </summary>
		protected int _iYearN1ID = 0;
        /// <summary>
        /// Year ID as a string
        /// </summary>
        protected string _strYearID = string.Empty;
		/// <summary>
		/// Year ID N-1 as a string
		/// </summary>
		protected string _strYearN1ID = string.Empty;
        #endregion

        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Get / Set Excel format ?
        /// </summary>
        public bool Excel
        {
            get { return _excel; }
            set { _excel = value; }
        }
        /// <summary>
        /// Get / Set PDF format ?
        /// </summary>
        public bool Pdf {
            get { return _pdf; }
            set { _pdf = value; }
        }
        /// <summary>
        /// Get / Set Evolution
        /// </summary>
        public bool Evolution {
            get { return _evolution; }
            set { _evolution = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="dalLayer">Data Access Layer</param>
        public Engine(WebSession session, IProductClassIndicatorsDAL dalLayer)
        {
            _session = session;
            this._dalLayer = dalLayer;

            #region Competitors & references
            if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].Contains(0)
                && _session.SecondaryProductUniverses[1].GetGroup(0).Contains(TNSClassificationLevels.ADVERTISER))
            {
                _competitorIDS = _session.SecondaryProductUniverses[1].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
            }
            if (_competitorIDS == null) _competitorIDS = new List<long>();
            if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].Contains(0)
                && _session.SecondaryProductUniverses[0].GetGroup(0).Contains(TNSClassificationLevels.ADVERTISER))
            {
                _referenceIDS = _session.SecondaryProductUniverses[0].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
            }
            if (_referenceIDS == null) _referenceIDS = new List<long>();
            #endregion

            #region Period
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string absolutEndPeriod = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

            if (int.Parse(absolutEndPeriod) < int.Parse(_session.PeriodBeginningDate))
                throw new NoDataException();
            _periodBegin = FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType);
            _periodEnd = FctUtilities.Dates.getPeriodEndDate(absolutEndPeriod, _session.PeriodType);
            FctUtilities.Dates.GetYearSelected(_session, ref _strYearID, ref _iYearID, _periodBegin);
			_iYearN1ID = (_iYearID == 1) ? 2 : 1;
			if (_iYearN1ID > 0)
				_strYearN1ID = _iYearN1ID.ToString();
            #endregion

            #region Evolution
            VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
            if (vehicleInfo != null && vehicleInfo.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.plurimedia && WebApplicationParameters.HidePlurimediaEvol)
                _evolution = false;
            #endregion
        }
        #endregion

        #region Access Points
        /// <summary>
        /// Process to build a result
        /// </summary>
        /// <returns>String container filled with html code</returns>
        public abstract StringBuilder GetResult();
        /// <summary>
        /// Process to build a result
        /// </summary>
        /// <returns>ResultTable</returns>
        public virtual ResultTable GetResultTable() { return null; }
        #endregion

        #region Method
        public virtual CellLabel GetCellLabel(String label)
        {
            return new CellLabel(label);
        }
        #endregion

        /// <summary>
        /// Get unit
        /// </summary>
        /// <returns>unit</returns>
        protected virtual UnitInformation GetUnit()
        {
            return UnitsInformation.List[UnitsInformation.DefaultKCurrency];

        }

    }
}
