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
using System.Linq;
using System.Data;
using System.Reflection;
using TNS.AdExpressI.VP.Loader.Exceptions;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;
using System.Reflection;
using TNS.AdExpress.VP.Loader.Domain.Web;
using TNS.AdExpressI.VP.Loader.DAL.Exceptions;
using System.IO;
using TNS.AdExpress.VP.Loader.Domain;

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
            try {
                IDataVeillePromoDAL veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);
                return veillePromoDAL.GetDataPromotionDetailList(source);
            }
            catch (Exception e) {
                
                if (e is VeillePromoDALExcelOpenFileException)
                    throw new VeillePromoExcelOpenFileException("Impossible Load Data Promotion", e);
                if (e is VeillePromoDALExcelVisualException)
                    throw new VeillePromoExcelVisualException("Impossible Load Data Promotion", e);
                if (e is VeillePromoDALExcelException)
                    throw new VeillePromoExcelException("Impossible to Load Data Promotion", e);
                throw new VeillePromoDALExcelException("Impossible to Load Data Promotion", e);
            }
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

        #region Get Picture File Name
        /// <summary>
        /// Get Picture File Name
        /// </summary>
        /// <param name="fileList">File List</param>
        /// <returns>Picture File Name List</returns>
        public Dictionary<string, PictureMatching> GetPictureFileName(List<string> fileList) {
            IDataVeillePromoDAL veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);
            return veillePromoDAL.GetPictureFileName(fileList);
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
            List<PictureMatching> pictureMatchingPromVisuList = new List<PictureMatching>();
            List<PictureMatching> pictureMatchingCondVisuList = new List<PictureMatching>();
            #endregion

            try {
                veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName, ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null, null);

                #region Open Transaction
                veillePromoDAL.Source.Open();
                transaction = ((OracleConnection)veillePromoDAL.Source.GetSource()).BeginTransaction();
                #endregion

                #region Traitment

                #region Data Db Insertions
                if (!veillePromoDAL.HasData(dataPromotionDetails.DateTraitment)) {
                    foreach (DataPromotionDetail cDataPromotionDetail in dataPromotionDetails.DataPromotionDetailList) {

                        Dictionary<string, PictureMatching> pictureMatchingPromVisuListTemp = GetPictureFileName(cDataPromotionDetail.PromotionVisual);
                        Dictionary<string, PictureMatching> pictureMatchingCondVisuListTemp = GetPictureFileName(cDataPromotionDetail.ConditionVisual);
                       
                        DataPromotionDetail dataPromotionDetailTemp = new DataPromotionDetail(
                                                                            cDataPromotionDetail.IdProduct
                                                                            , cDataPromotionDetail.IdBrand
                                                                            , cDataPromotionDetail.DateBegin
                                                                            , cDataPromotionDetail.DateEnd
                                                                            , cDataPromotionDetail.IdSegment
                                                                            , cDataPromotionDetail.IdCategory
                                                                            , cDataPromotionDetail.IdCircuit
                                                                            , cDataPromotionDetail.PromotionContent
                                                                            , (from pictureMatching in pictureMatchingCondVisuListTemp.Values select pictureMatching.PathOut).ToList<string>()
                                                                            , cDataPromotionDetail.ConditionText
                                                                            , cDataPromotionDetail.PromotionBrand
                                                                            , (from pictureMatching in pictureMatchingPromVisuListTemp.Values select pictureMatching.PathOut).ToList<string>()
                                                                        );


                        veillePromoDAL.InsertDataPromotionDetail(dataPromotionDetails.DateTraitment, dataPromotionDetailTemp);

                        pictureMatchingPromVisuList.AddRange(pictureMatchingPromVisuListTemp.Values);
                        pictureMatchingCondVisuList.AddRange(pictureMatchingCondVisuListTemp.Values);
                    }
                }
                else {
                    throw new VeillePromoInsertDbException("Data Allready Exist for the date");
                }
                #endregion

                #region Copy Pictures
                foreach (PictureMatching cPictureMatching in pictureMatchingPromVisuList) {
                    File.Copy(cPictureMatching.PathIn, Path.Combine(ApplicationParameters.Parameters.VisualPromotionPathOut, cPictureMatching.PathOut), true);
                }
                foreach (PictureMatching cPictureMatching in pictureMatchingCondVisuList) {
                    File.Copy(cPictureMatching.PathIn, Path.Combine(ApplicationParameters.Parameters.VisualConditionPathOut, cPictureMatching.PathOut), true);
                }
                #endregion

                #endregion

                #region Commit Transaction
                transaction.Commit();
                #endregion

            }
            catch (Exception e) {
                if (transaction != null) transaction.Rollback();

                throw new VeillePromoInsertDbException("Impossible to Insert Data Promotion", e);
            }
        }
        #endregion

        #endregion

    }
}
