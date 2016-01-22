using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
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
using TNS.AdExpress.VP.Loader.Domain.Web;
using TNS.AdExpressI.VP.Loader.DAL.Exceptions;
using System.IO;
using TNS.AdExpress.VP.Loader.Domain;

namespace TNS.AdExpressI.VP.Loader.Data {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public abstract class DataVeillePromo : VeillePromo, IDataVeillePromo
    {

        #region Variables
        /// <summary>
        /// Transaction
        /// </summary>
        protected OracleTransaction _transaction = null;
        /// <summary>
        /// Data Access
        /// </summary>
        protected IDataVeillePromoDAL _veillePromoDAL = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        /// <param name="dataLanguage">Data Language</param>
        public DataVeillePromo(DataBase dataBase)
            : base(dataBase) {
           _veillePromoDAL = (IDataVeillePromoDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + ApplicationParameters
               .CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].AssemblyName,
               ApplicationParameters.CoreLayers[TNS.AdExpress.VP.Loader.Domain.Constantes.Constantes.Layers.Id.dataAccess].Class,
               false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _dataBase }, null, null);
        }
        #endregion

        #region IVeillePromo Membres
        /// <summary>
        /// Begin Transaction
        /// </summary>
        public void BeginTransaction()
        {
            _veillePromoDAL.Source.Open();
            _transaction = ((OracleConnection)_veillePromoDAL.Source.GetSource()).BeginTransaction();
        }
        /// <summary>
        /// Commit Transaction
        /// </summary>
        public void CommitTransaction()
        {
            if (_transaction != null) _transaction.Commit();
            if (_veillePromoDAL != null && _veillePromoDAL.Source != null) _veillePromoDAL.Source.Close();
        }
        /// <summary>
        /// Rollback Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            if (_transaction != null) _transaction.Rollback();
            if (_veillePromoDAL != null && _veillePromoDAL.Source!=null) _veillePromoDAL.Source.Close();
        }

        #region GetDataPromotionDetailList
        /// <summary>
        /// Get Data Promotion Detail List from a File data source Excel
        /// </summary>
        /// <param name="source">data source</param>
        /// <returns>Data Promotion Detail List</returns>
        public DataPromotionDetails GetDataPromotionDetailList(FileDataSource source) {
            try {
                return _veillePromoDAL.GetDataPromotionDetailList(source);
            }
            catch (Exception e)
            {
                if (e is VeillePromoDALExcelInvalidDateException)
                    throw new VeillePromoExcelInvalidDateException(((VeillePromoDALExcelInvalidDateException)e).CellExcel, "Impossible Load Data Promotion", e);
                if (e is VeillePromoDALExcelCellException)
                    throw new VeillePromoExcelCellException(((VeillePromoDALExcelCellException)e).CellExcel, "Impossible Load Data Promotion", e);
                if (e is VeillePromoDALExcelOpenFileException)
                    throw new VeillePromoExcelOpenFileException("Impossible Load Data Promotion", e);
                if (e is VeillePromoDALExcelVisualException)
                    throw new VeillePromoExcelVisualException("Impossible Load Data Promotion", e);
                if (e is VeillePromoDALIncorrectPictureFileNameException)
                    throw new VeillePromoIncorrectPictureFileNameException("Impossible Load Data Promotion", e);
                if (e is VeillePromoDALIncorrectPictureFileNameNumberException)
                    throw new VeillePromoIncorrectPictureFileNameNumberException("Impossible Load Data Promotion", e);
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
        /// <param name="idVehicle">id Vehicle</param>
        public void DeleteData(DateTime dateBeginTraitment, DateTime dateEndTraitment,long idVehicle) {
            _veillePromoDAL.DeleteData(dateBeginTraitment, dateEndTraitment, idVehicle);
        }
        #endregion

        #region Has Data

        /// <summary>
        /// Has data for the date traiment passed in parameter
        /// </summary>
        /// <param name="dateTraitment">Date Traitment</param>
        /// <param name="idVehicle">id Vehicle</param>
        /// <returns>Has Data or not for the date traiment passed in parameter</returns>
        public bool HasData(DateTime dateTraitmentBegin, DateTime dateTraitmentEnd,long idVehicle) {
            return _veillePromoDAL.HasData(dateTraitmentBegin, dateTraitmentEnd, idVehicle);
        }
        #endregion

        #region Get Picture File Name
        /// <summary>
        /// Get Picture File Name
        /// </summary>
        /// <param name="fileList">File List</param>
        /// <returns>Picture File Name List</returns>
        public Dictionary<string, PictureMatching> GetPictureFileName(List<string> fileList) {
            return _veillePromoDAL.GetPictureFileName(fileList);
        }
        #endregion

        #region Insert Data
        /// <summary>
        /// Insert data Promotion Detail List
        /// </summary>
        /// <param name="dataPromotionDetails">Data Promotion Detail List</param>
        public void InsertDataPromotionDetails(DataPromotionDetails dataPromotionDetails){

            #region Variables
            var pictureMatchingPromVisuList = new List<PictureMatching>();
            var pictureMatchingCondVisuList = new List<PictureMatching>();
            #endregion

            try {

                #region Traitment

                #region Data Db Insertions
                if (!_veillePromoDAL.HasData(dataPromotionDetails.DateTraitment, dataPromotionDetails.DateTraitment
                    , Vehicles.names.webPromotion.GetHashCode())) {
                    foreach (DataPromotionDetail cDataPromotionDetail in dataPromotionDetails.DataPromotionDetailList) {

                        Dictionary<string, PictureMatching> pictureMatchingPromVisuListTemp = GetPictureFileName(cDataPromotionDetail.PromotionVisual);
                        Dictionary<string, PictureMatching> pictureMatchingCondVisuListTemp = GetPictureFileName(cDataPromotionDetail.ConditionVisual);
                       
                        var dataPromotionDetailTemp = new DataPromotionDetail(
                                                                            cDataPromotionDetail.IdProduct
                                                                            , cDataPromotionDetail.IdBrand
                                                                            , cDataPromotionDetail.DateBegin
                                                                            , cDataPromotionDetail.DateEnd
                                                                            , cDataPromotionDetail.IdSegment
                                                                            , cDataPromotionDetail.IdCategory
                                                                            , cDataPromotionDetail.IdCircuit
                                                                            , cDataPromotionDetail.PromotionContent
                                                                            , (pictureMatchingCondVisuListTemp.Values
                                                                                                              .Select(
                                                                                                                  pictureMatching
                                                                                                                  =>
                                                                                                                  pictureMatching
                                                                                                                      .PathOut)).ToList()
                                                                            , cDataPromotionDetail.ConditionText
                                                                            , cDataPromotionDetail.PromotionBrand
                                                                            , (pictureMatchingPromVisuListTemp.Values
                                                                                                              .Select(
                                                                                                                  pictureMatching
                                                                                                                  =>
                                                                                                                  pictureMatching
                                                                                                                      .PathOut)).ToList()
                                                                            , cDataPromotionDetail.ExcluWeb
                                                                            ,cDataPromotionDetail.IsNational
                                                                            ,cDataPromotionDetail.IdVehicle
                                                                        );


                        _veillePromoDAL.InsertDataPromotionDetail(dataPromotionDetails.DateTraitment, dataPromotionDetailTemp);

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

            }
            catch (Exception e) {
                throw new VeillePromoInsertDbException("Impossible to Insert Data Promotion", e);
            }
        }
        #endregion

        #endregion

    }
}
