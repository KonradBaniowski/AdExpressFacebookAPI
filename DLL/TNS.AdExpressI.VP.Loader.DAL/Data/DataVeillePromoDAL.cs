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

namespace TNS.AdExpressI.VP.Loader.DAL.Data {
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public abstract class DataVeillePromoDAL : VeillePromoDAL, IDataVeillePromoDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        /// <param name="dataLanguage">Data Language</param>
        public DataVeillePromoDAL(DataBase dataBase):base(dataBase) {
        }
        #endregion

        #region IVeillePromoDAL Membres

        #region GetDataPromotionDetailList
        /// <summary>
        /// Get Data Promotion Detail List from a File data source Excel
        /// </summary>
        /// <param name="source">data source</param>
        /// <returns>Data Promotion Detail List</returns>
        public DataPromotionDetails GetDataPromotionDetailList(FileDataSource source) {

            #region Variables
            List<DataPromotionDetail> dataPromotionDetailList = new List<DataPromotionDetail>();
            Workbook excel = null;
            License license = null;
            DateTime dateFile = DateTime.Now;
            int startLineData = 2;
            int startColumnData = 0;

            int columnProduct = 0;
            int columnBrand = 1;
            int columnDateBegin = 2;
            int columnDateEnd = 3;
            int columnPromoDetail = 4;
            int columnVisualsCondition = 5;
            int columnTextCondition = 6;
            int columnBrandPromo = 7;
            int columnVisualsPromo = 8;

            long idProduct;
            long idBrand;
            DateTime dateBegin;
            DateTime dateEnd;
            long idSegment;
            long idCategory;
            long idCircuit;
            string promotionContent;
            List<string> conditionVisual;
            string conditionText;
            string promotionBrand;
            List<string> promotionVisual;
            #endregion

            try {

                #region Initialize Excel Object
                excel = new Workbook();
                license = new License();
                license.SetLicense("Aspose.Cells.lic");
                #endregion

                #region Load File
                FileStream fileStream = (FileStream)source.GetSource();
                try {
                    dateFile = DateTime.ParseExact(Path.GetFileNameWithoutExtension(fileStream.Name).Replace("Renault_", ""), "yyyyMM", CultureInfo.InvariantCulture);
                }
                catch (Exception e) {
                    throw new VeillePromoDALExcelException("Incorrect File Name: " + fileStream.Name, e);
                }
                try {
                    excel.Open((Stream)fileStream);
                }
                catch (Exception e) {
                    throw new VeillePromoDALExcelOpenFileException("Impossible to open excel file Name: " + fileStream.Name, e);
                }
                finally {
                    fileStream.Close();
                }
                #endregion

                Worksheet sheet = excel.Worksheets[0];
                Aspose.Cells.Cells cells = sheet.Cells;

                for (int line = startLineData; cells[line, startColumnData].Value != null; line++) {

                    #region Get Product
                    idProduct = AllClassification.GetProduct((string)cells[line, columnProduct].Value).Id;
                    #endregion

                    #region Get Brand
                    idBrand = AllClassification.GetBrand((string)cells[line, columnBrand].Value).Id;
                    #endregion

                    #region Get Segment
                    idSegment = AllClassification.GetSegmentByProductId(idProduct).Id;
                    #endregion

                    #region Get Category
                    idCategory = AllClassification.GetCategory(idProduct).Id;
                    #endregion

                    #region Get Circuit
                    idCircuit = AllClassification.GetCircuit(idBrand).Id;
                    #endregion

                    #region Get Date Begin
                    dateBegin = (DateTime)cells[line, columnDateBegin].Value;
                    #endregion

                    #region Get Date End
                    dateEnd = (DateTime)cells[line, columnDateEnd].Value;
                    #endregion

                    #region Get Promo Content
                    promotionContent = (string)cells[line, columnPromoDetail].Value;
                    #endregion

                    #region Get Visuals condition
                    conditionVisual = new List<string>(((string)cells[line, columnVisualsCondition].Value).Split(';'));
                    #endregion

                    #region Get Text Condition
                    conditionText = (string)cells[line, columnTextCondition].Value;
                    #endregion

                    #region Get Brand Promotion
                    promotionBrand = (string)cells[line, columnBrandPromo].Value;
                    #endregion

                    #region Get Visuals Promotion
                    promotionVisual = new List<string>(((string)cells[line, columnVisualsPromo].Value).Split(';'));
                    #endregion

                    dataPromotionDetailList.Add(new DataPromotionDetail(
                                idProduct,
                                idBrand,
                                dateBegin,
                                dateEnd,
                                idSegment,
                                idCategory,
                                idCircuit,
                                promotionContent,
                                conditionVisual,
                                conditionText,
                                promotionBrand,
                                promotionVisual));
                               
                }
                return new DataPromotionDetails(dateFile,  dataPromotionDetailList);
            }
            catch (System.Exception err) {
                throw (new VeillePromoDALException("Impossible to Get Data Promotion Detail List", err));
            }
            finally {
                if (excel != null) excel = null;
            }
        }
        #endregion

        #region Has Data
        /// <summary>
        /// Has data for the date traiment passed in parameter
        /// </summary>
        /// <param name="dateTraitment">Date Traitment</param>
        /// <returns>Has Data or not for the date traiment passed in parameter</returns>
        public bool HasData(DateTime dateTraitment) {

            #region Variables
            StringBuilder sql = new StringBuilder();
            Table tblData = _dataBase.GetTable(TableIds.dataPromotion);
            DataSet ds = null;
            #endregion

            try {

                #region Construct global query
                sql = new StringBuilder(200);
                sql.AppendFormat("select count(*) nb from {0} WHERE DATE_TRAITMENT = {1} ", tblData.Sql, dateTraitment.ToString("yyyyMM"));
                #endregion

                #region Execute Query
                ds = _source.Fill(sql.ToString());
                #endregion

                return (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count == 1 && ((Int64)ds.Tables[0].Rows[0]["nb"])>0);

            }
            catch (System.Exception err) {
                throw new VeillePromoDALDbException("DataAccess HasData Error. sql: " + sql.ToString(), err);
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

            #region Variables
            StringBuilder sql = new StringBuilder();
            Table tblData = _dataBase.GetTable(TableIds.dataPromotion);
            #endregion

            try {

                #region Construct global query
                sql = new StringBuilder(200);
                sql.AppendFormat("DELETE FROM {0} WHERE DATE_TRAITMENT>={1} AND DATE_TRAITMENT<={2} "
                    , tblData.Sql
                    , dateBeginTraitment.ToString("yyyyMM")
                    , dateEndTraitment.ToString("yyyyMM"));
                #endregion

                #region Execute Query
                _source.Delete(sql.ToString());
                #endregion

            }
            catch (System.Exception err) {
                throw new VeillePromoDALDbException("DataAccess DeleteData Error. sql: " + sql.ToString(), err);
            }
        }
        #endregion

        #region Insert Data
        /// <summary>
        /// Insert data Promotion Detail
        /// </summary>
        /// <param name="dateTraitment">Date Traitment</param>
        /// <param name="dataPromotionDetail">Data Promotion Detail</param>
        public void InsertDataPromotionDetail(DateTime dateTraitment, DataPromotionDetail dataPromotionDetail){

            #region Variables
            StringBuilder sql = new StringBuilder();
            Table tblData = _dataBase.GetTable(TableIds.dataPromotion);
            #endregion

            try {

                #region Construct global query
                sql = new StringBuilder(200);
                sql.AppendFormat("INSERT INTO {0} ", tblData.Sql);

                sql.Append("(ID_DATA_PROMOTION, ID_PRODUCT, ID_BRAND, DATE_BEGIN_NUM, DATE_END_NUM, ID_SEGMENT, ID_CATEGORY, ID_CIRCUIT, PROMOTION_CONTENT, CONDITION_VISUAL, CONDITION_TEXT, PROMOTION_BRAND, PROMOTION_VISUAL, ACTIVATION, DATE_TRAITMENT) ");

                sql.Append("VALUES ");

                sql.AppendFormat("({0}, {1}, {2}, to_date('{3}','YYYYMMDDHH24MISS'), to_date('{4}','YYYYMMDDHH24MISS'), {5}, {6}, {7}, '{8}', '{9}', '{10}', '{11}', '{12}', {13}, {14}) ",
                "(select PROMO03.SEQ_DATA_PROMOTION.NEXTVAL from dual)",
                dataPromotionDetail.IdProduct,
                dataPromotionDetail.IdBrand,
                dataPromotionDetail.DateBegin.ToString("yyyyMMddHHmmss"),
                dataPromotionDetail.DateEnd.ToString("yyyyMMddHHmmss"),
                dataPromotionDetail.IdSegment,
                dataPromotionDetail.IdCategory,
                dataPromotionDetail.IdCircuit,
                dataPromotionDetail.PromotionContent.Replace("'", "''"),
                string.Join(",", dataPromotionDetail.ConditionVisual.ConvertAll<string>(p => p.Replace("'", "''")).ToArray()),
                dataPromotionDetail.ConditionText.Replace("'", "''"),
                dataPromotionDetail.PromotionBrand.Replace("'", "''"),
                string.Join(",", dataPromotionDetail.PromotionVisual.ConvertAll<string>(p => p.Replace("'", "''")).ToArray()),
                ActivationValues.ACTIVATED,
                dateTraitment.ToString("yyyyMM"));

                #endregion
                
                #region Execute Query
                _source.Insert(sql.ToString());
                #endregion

            }
            catch (System.Exception err) {
                throw new VeillePromoDALDbException("DataAccess InsertDataPromotionDetail Error. sql: " + sql.ToString(), err);
            }
        }
        #endregion

        #endregion

    }
}
