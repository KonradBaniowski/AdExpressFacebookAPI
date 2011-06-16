using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;
using System.Globalization;
using TNS.AdExpress.VP.Loader.Domain.Classification;
using TNS.AdExpressI.VP.Loader.DAL.Data;
using TNS.AdExpress.Constantes.DB;
using System.Data;
using TNS.AdExpressI.VP.Loader.Exceptions;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using TNS.AdExpress.VP.Loader.Domain.Web;
using TNS.AdExpressI.VP.Loader.DAL.Exceptions;

namespace TNS.AdExpressI.VP.Loader.Data {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public abstract class DataVeillePromo : VeillePromo, IDataVeillePromo {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        /// <param name="dataLanguage">Data Language</param>
        public DataVeillePromo(DataBase dataBase)
            : base(dataBase) {
        }
        #endregion

        #region IVeillePromo Membres

        #region GetDataPromotionDetailList
        /// <summary>
        /// Get Data Promotion Detail List from a File data source Excel
        /// </summary>
        /// <param name="source">data source</param>
        /// <returns>Data Promotion Detail List</returns>
        public DataPromotionDetails GetDataPromotionDetailList(FileDataSource source) {
            IDataVeillePromoDAL veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);
            return veillePromoDAL.GetDataPromotionDetailList(source);
        }
        #endregion

        #region Delete Data
        /// <summary>
        /// Delete data between dateBegin parameter and dateEnd parameter
        /// </summary>
        /// <param name="dateBegin">Date Begin</param>
        /// <param name="dateEnd">Date End</param>
        public void DeleteData(DateTime dateBeginTraitment, DateTime dateEndTraitment) {
            IDataVeillePromoDAL veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);
            veillePromoDAL.DeleteData(dateBeginTraitment, dateEndTraitment);
        }
        #endregion

        #region Has Data
        /// <summary>
        /// Has data for the date traiment passed in parameter
        /// </summary>
        /// <param name="dateTraitment">Date Traitment</param>
        /// <returns>Has Data or not for the date traiment passed in parameter</returns>
        public bool HasData(DateTime dateTraitment) {
            IDataVeillePromoDAL veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);
            return veillePromoDAL.HasData(dateTraitment);
        }
        #endregion

        #region Insert Data
        /// <summary>
        /// Insert data Promotion Detail List
        /// </summary>
        /// <param name="dataPromotionDetails">Data Promotion Detail List</param>
        public void InsertDataPromotionDetails(DataPromotionDetails dataPromotionDetails){

            #region Variables
            IDataVeillePromoDAL veillePromoDAL = null;
            OracleTransaction transaction = null;
            #endregion

            try {
                veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);

                #region Open Transaction
                veillePromoDAL.Source.Open();
                transaction = ((OracleConnection)veillePromoDAL.Source.GetSource()).BeginTransaction();
                #endregion

                #region Traitment
                if (!veillePromoDAL.HasData(dataPromotionDetails.DateTraitment)) {
                    foreach (DataPromotionDetail cDataPromotionDetail in dataPromotionDetails.DataPromotionDetailList) {
                        veillePromoDAL.InsertDataPromotionDetail(dataPromotionDetails.DateTraitment, cDataPromotionDetail);
                    }
                }
                else {
                    throw new VeillePromoInsertDbException("Data Allready Exist for the date");
                }
                #endregion

                #region Commit Transaction
                transaction.Commit();
                #endregion

            }
            catch (Exception e) {
                if (transaction != null) transaction.Rollback();

                if (e is VeillePromoDALExcelException)
                    throw new VeillePromoExcelException("Impossible to Insert Data Promotion", e);
                if (e is VeillePromoDALExcelOpenFileException)
                    throw new VeillePromoExcelOpenFileException("Impossible to Insert Data Promotion", e);

                throw new VeillePromoInsertDbException("Impossible to Insert Data Promotion", e);
            }
        }
        #endregion

        #endregion

    }
}
