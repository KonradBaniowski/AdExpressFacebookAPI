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

namespace TNS.AdExpressI.VP.Loader.DAL.Classification {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public abstract class ClassifVeillePromoDAL : VeillePromoDAL, IClassifVeillePromoDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        public ClassifVeillePromoDAL(DataBase dataBase):base(dataBase) {
        }
        #endregion

        #region IVeillePromoDAL Membres

        #region Get All Product
        /// <summary>
        /// Get Data all product from data base
        /// </summary>
        /// <returns>Data all product from data base</returns>
        public DataSet GetAllProduct() {

            #region Variables
            StringBuilder sql = new StringBuilder();
            View viewAllProduct = _dataBase.GetView(ViewIds.allPromoProduct);
            DataSet ds = null;
            #endregion

            try {

                #region Construct global query
                sql = new StringBuilder(200);
                sql.AppendFormat("select ID_SEGMENT, SEGMENT, ID_CATEGORY, CATEGORY, ID_PRODUCT, PRODUCT from {0} order by SEGMENT, ID_SEGMENT, CATEGORY, ID_CATEGORY, PRODUCT, ID_PRODUCT ", viewAllProduct.Sql);
                #endregion

                #region Execute Query
                ds = _source.Fill(sql.ToString());
                #endregion

                return (ds);

            }
            catch (System.Exception err) {
                throw new VeillePromoDALDbException("DataAccess GetAllProduct Error. sql: " + sql.ToString(), err);
            }
        }
        #endregion

        #region Get All Brand
        /// <summary>
        /// Get Data all Brand from data base
        /// </summary>
        /// <returns>Data all Brand from data base</returns>
        public DataSet GetAllBrand() {

            #region Variables
            StringBuilder sql = new StringBuilder();
            View viewAllBrand = _dataBase.GetView(ViewIds.allPromoBrand);
            DataSet ds = null;
            #endregion

            try {

                #region Construct global query
                sql = new StringBuilder(200);
                sql.AppendFormat("select ID_CIRCUIT, CIRCUIT, ID_BRAND, BRAND from {0} ORDER BY CIRCUIT, ID_CIRCUIT, BRAND, ID_BRAND ", viewAllBrand.Sql);
                #endregion

                #region Execute Query
                ds = _source.Fill(sql.ToString());
                #endregion

                return (ds);

            }
            catch (System.Exception err) {
                throw new VeillePromoDALDbException("DataAccess GetAllBrand Error. sql: " + sql.ToString(), err);
            }
        }
        #endregion

        #endregion

    }
}
