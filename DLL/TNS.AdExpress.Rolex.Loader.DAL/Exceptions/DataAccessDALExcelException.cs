using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Rolex.Loader.Domain;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Rolex.Loader.DAL.Exceptions
{
 public   class DataAccessDALExcelException : BaseException
    {
        #region Variables
        /// <summary>
        /// Cell Excel
        /// </summary>
        private readonly CellExcel _cellExcel;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        public DataAccessDALExcelException(CellExcel cellExcel)
            : base() {
                _cellExcel = cellExcel;
        }

     /// <summary>
     /// Constructeur
     /// </summary>
     /// <param name="cellExcel">cell excel </param>
     /// <param name="message">Message d'erreur</param>
     public DataAccessDALExcelException(CellExcel cellExcel, string message)
            : base(message) {
                _cellExcel = cellExcel;
        }

     /// <summary>
     /// Constructeur
     /// </summary>
     /// <param name="cellExcel">cell excel </param>
     /// <param name="message">Message d'erreur</param>
     /// <param name="innerException">Exception source</param>
     public DataAccessDALExcelException(CellExcel cellExcel, string message, System.Exception innerException)
            : base(message, innerException) {
                _cellExcel = cellExcel;
        }
        #endregion

        #region Accessor
        /// <summary>
        /// Get Cell Excel
        /// </summary>
        public CellExcel CellExcel { get { return _cellExcel; } }
        #endregion
    }
}
