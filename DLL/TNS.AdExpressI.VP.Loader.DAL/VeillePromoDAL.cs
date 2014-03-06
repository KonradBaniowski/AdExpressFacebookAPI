using System;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpressI.VP.Loader.DAL {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public class VeillePromoDAL:IVeillePromoDAL {

        #region Variables
        /// <summary>
        /// Data Source
        /// </summary>
        protected IDataSource _source = null;
        /// <summary>
        /// Data Base
        /// </summary>
        protected DataBase _dataBase = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        public VeillePromoDAL(DataBase dataBase) {
            if (dataBase == null) throw new ArgumentNullException("DataBase parameter is null");
            _source = GetSource(dataBase);
            _dataBase = dataBase;
        }
        #endregion

        #region Assessor
        /// <summary>
        /// Get Source
        /// </summary>
        public IDataSource Source { get { return _source; } }
        #endregion

        #region Protected Methods

        #region Get Source
        /// <summary>
        /// Get Source used for build Query
        /// </summary>
        /// <returns>IDataSource object</returns>
        protected virtual IDataSource GetSource(DataBase dataBase) {
            return dataBase.GetDefaultConnection(DefaultConnectionIds.vPromo);
        }
        #endregion

        #endregion

    }
}
