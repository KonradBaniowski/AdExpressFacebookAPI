using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;
using Aspose.Cells;
using System.IO;
using System.Globalization;
using TNS.AdExpressI.VP.Loader.DAL.Exceptions;
using TNS.AdExpress.VP.Loader.Domain.Classification;
using TNS.AdExpress.Constantes.DB;
using System.Data;

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
