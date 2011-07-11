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
using TNS.AdExpress.VP.Loader.Domain;

namespace TNS.AdExpressI.VP.Loader.DAL.Data
{
    /// <summary>
    /// Promo Veille Class
    /// </summary>
    public abstract class DataVeillePromoDAL : VeillePromoDAL, IDataVeillePromoDAL
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataBase">Data Base</param>
        /// <param name="dataLanguage">Data Language</param>
        public DataVeillePromoDAL(DataBase dataBase)
            : base(dataBase)
        {
        }
        #endregion

        #region IVeillePromoDAL Membres

        #region GetDataPromotionDetailList
        /// <summary>
        /// Get Data Promotion Detail List from a File data source Excel
        /// </summary>
        /// <param name="source">data source</param>
        /// <returns>Data Promotion Detail List</returns>
        public DataPromotionDetails GetDataPromotionDetailList(FileDataSource source)
        {

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
            List<string> conditionVisual = null;
            string conditionText;
            string promotionBrand;
            List<string> promotionVisual = null;
            #endregion

            try
            {

                #region Initialize Excel Object
                excel = new Workbook();
                license = new License();
                license.SetLicense("Aspose.Cells.lic");
                #endregion

                #region Load File
                FileStream fileStream = (FileStream)source.GetSource();
                try
                {
                    dateFile = DateTime.ParseExact(Path.GetFileNameWithoutExtension(fileStream.Name).Replace("Renault_", ""), "yyyyMM", CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    throw new VeillePromoDALExcelException("Incorrect File Name: " + fileStream.Name, e);
                }
                try
                {
                    excel.Open((Stream)fileStream);
                }
                catch (Exception e)
                {
                    throw new VeillePromoDALExcelOpenFileException("Impossible to open excel file Name: " + fileStream.Name, e);
                }
                finally
                {
                    fileStream.Close();
                }
                #endregion

                Worksheet sheet = excel.Worksheets[0];
                Aspose.Cells.Cells cells = sheet.Cells;

                for (int line = startLineData; cells[line, startColumnData].Value != null; line++)
                {

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
                    if(cells[line, columnVisualsCondition].Value != null)
                        conditionVisual = (new List<string>(((string)cells[line, columnVisualsCondition].Value).Split(';'))).ConvertAll<string>(file => System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileStream.Name),file.Trim())));

                    if (conditionVisual != null) {
                        for(int i=0; i<conditionVisual.Count; i++){
                            if (!File.Exists(conditionVisual[i])) {
                                string[] files = Directory.GetFiles(Path.GetDirectoryName(conditionVisual[i]), Path.GetFileName(conditionVisual[i]) + ".*", SearchOption.TopDirectoryOnly);
                                if (files.Length > 1) throw new VeillePromoDALIncorrectPictureFileNameNumberException("Impossible to retrieve the file. " + files.Length + " files are found");
                                if (files.Length <= 0) throw new VeillePromoDALIncorrectPictureFileNameException("Impossible to retrieve the file '" + conditionVisual[i] + "'");
                                conditionVisual[i] = files[0];
                            }

                        }
                    }
                    #endregion

                    #region Get Text Condition

                    conditionText = (cells[line, columnTextCondition].Value != null) ? (string)cells[line, columnTextCondition].Value : null;
                    #endregion

                    #region Get Brand Promotion

                    promotionBrand = (cells[line, columnBrandPromo].Value != null) ? (string)cells[line, columnBrandPromo].Value : null;
                    #endregion

                    #region Get Visuals Promotion
                    if (cells[line, columnVisualsPromo].Value != null)
                        promotionVisual = new List<string>(((string)cells[line, columnVisualsPromo].Value).Split(';')).ConvertAll<string>(file => System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileStream.Name), file.Trim()))); 
                    if (promotionVisual != null) {
                        for (int i = 0; i < promotionVisual.Count; i++) {
                            if (!File.Exists(promotionVisual[i])) {
                                string[] files = Directory.GetFiles(Path.GetDirectoryName(promotionVisual[i]), Path.GetFileName(promotionVisual[i]) + ".*", SearchOption.TopDirectoryOnly);
                                if (files.Length > 1) throw new VeillePromoDALIncorrectPictureFileNameNumberException("Impossible to retrieve the file. " + files.Length + " files are found");
                                if (files.Length <= 0) throw new VeillePromoDALIncorrectPictureFileNameException("Impossible to retrieve the file '" + promotionVisual[i] + "'");
                                promotionVisual[i] = files[0];
                            }
                        }
                    }
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
                return new DataPromotionDetails(dateFile, dataPromotionDetailList);
            }
            finally
            {
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
        public bool HasData(DateTime dateTraitmentBegin, DateTime dateTraitmentEnd)
        {

            #region Variables
            StringBuilder sql = new StringBuilder();
            Table tblData = _dataBase.GetTable(TableIds.dataPromotion);
            DataSet ds = null;
            #endregion

            try
            {

                #region Construct global query
                sql = new StringBuilder(200);
                sql.AppendFormat("select count(*) nb from {0} WHERE LOAD_DATE>={1} AND LOAD_DATE<={2} ", tblData.Sql, dateTraitmentBegin.ToString("yyyyMM"), dateTraitmentEnd.ToString("yyyyMM"));
                #endregion

                #region Execute Query
                ds = _source.Fill(sql.ToString());
                #endregion

                return (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count == 1 && (Convert.ToInt64(ds.Tables[0].Rows[0]["nb"]) > 0));

            }
            catch (System.Exception err)
            {
                throw new VeillePromoDALDbException("DataAccess HasData Error. sql: " + sql.ToString(), err);
            }
        }
        #endregion

        #region Get Picture File Name
        /// <summary>
        /// Get Picture File Name
        /// </summary>
        /// <param name="fileList">File List</param>
        /// <returns>Picture File Name List</returns>
        public Dictionary<string, PictureMatching> GetPictureFileName(List<string> fileList) {

            #region Variables
            StringBuilder sql = new StringBuilder();
            DataSet ds = null;
            #endregion

            try {

                Dictionary<string, PictureMatching> filePictureList = new Dictionary<string, PictureMatching>();

                if (fileList != null) {
                    foreach (string cFile in fileList) {

                        #region Construct global query
                        sql = new StringBuilder(200);
                        sql.AppendFormat("SELECT {0}SEQ_VISUAL.NEXTVAL as fileName FROM dual ", _dataBase.GetSchema(SchemaIds.promo03).Sql);
                        #endregion

                        #region Execute Query
                        ds = _source.Fill(sql.ToString());
                        #endregion

                        filePictureList.Add(cFile, new PictureMatching(cFile, ds.Tables[0].Rows[0]["fileName"].ToString() + Path.GetExtension(cFile)));
                    }
                }
                return filePictureList;
            }
            catch (System.Exception err) {
                throw new VeillePromoDALDbException("DataAccess DeleteData Error. sql: " + sql.ToString(), err);
            }
        }
        #endregion

        #region Delete Data
        /// <summary>
        /// Delete data between dateBegin parameter and dateEnd parameter
        /// </summary>
        /// <param name="dateBegin">Date Begin</param>
        /// <param name="dateEnd">Date End</param>
        public void DeleteData(DateTime dateBeginTraitment, DateTime dateEndTraitment)
        {

            #region Variables
            StringBuilder sql = new StringBuilder();
            Table tblData = _dataBase.GetTable(TableIds.dataPromotion);
            #endregion

            try
            {

                #region Construct global query
                sql = new StringBuilder(200);
                sql.AppendFormat("DELETE FROM {0} WHERE LOAD_DATE>={1} AND LOAD_DATE<={2} "
                    , tblData.Sql
                    , dateBeginTraitment.ToString("yyyyMM")
                    , dateEndTraitment.ToString("yyyyMM"));
                #endregion

                #region Execute Query
                _source.Delete(sql.ToString());
                #endregion

            }
            catch (System.Exception err)
            {
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
        public void InsertDataPromotionDetail(DateTime dateTraitment, DataPromotionDetail dataPromotionDetail)
        {

            #region Variables
            StringBuilder sql = new StringBuilder();
            Table tblData = _dataBase.GetTable(TableIds.dataPromotion);
            #endregion

            try
            {

                #region Construct global query
                sql = new StringBuilder(200);
                string promoContent = (!string.IsNullOrEmpty(dataPromotionDetail.PromotionContent) ? dataPromotionDetail.PromotionContent.Replace("'", "''") : "");
                string conditionText = (!string.IsNullOrEmpty(dataPromotionDetail.ConditionText) ? dataPromotionDetail.ConditionText.Replace("'", "''") : "");
                string promotionBrand = (!string.IsNullOrEmpty(dataPromotionDetail.PromotionBrand) ? dataPromotionDetail.PromotionBrand.Replace("'", "''") : "");
                string conditionVisual = (dataPromotionDetail.ConditionVisual != null && dataPromotionDetail.ConditionVisual.Count > 0) ?
                string.Join(",", dataPromotionDetail.ConditionVisual.ConvertAll<string>(p => p.Replace("'", "''")).ToArray()) : "";
                string promotionVisual = (dataPromotionDetail.PromotionVisual != null && dataPromotionDetail.PromotionVisual.Count > 0) ?
                    string.Join(",", dataPromotionDetail.PromotionVisual.ConvertAll<string>(p => p.Replace("'", "''")).ToArray()) : "";
                
                sql.AppendFormat("INSERT INTO {0} ", tblData.Sql);

                sql.Append("(ID_DATA_PROMOTION, ID_LANGUAGE_DATA_I, ID_PRODUCT, ID_BRAND, DATE_BEGIN_NUM, DATE_END_NUM, ID_SEGMENT, ID_CATEGORY, ID_CIRCUIT, PROMOTION_CONTENT, CONDITION_VISUAL, CONDITION_TEXT, PROMOTION_BRAND, PROMOTION_VISUAL, ACTIVATION, LOAD_DATE) ");

                sql.Append("VALUES ");

                sql.AppendFormat("({0}, 33, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}', '{9}', '{10}', '{11}', '{12}', {13}, {14}) ",
                "PROMO03.SEQ_DATA_PROMOTION.NEXTVAL",
                dataPromotionDetail.IdProduct,
                dataPromotionDetail.IdBrand,
               Convert.ToInt64( dataPromotionDetail.DateBegin.ToString("yyyyMMdd")),
                Convert.ToInt64( dataPromotionDetail.DateEnd.ToString("yyyyMMdd")),
                dataPromotionDetail.IdSegment,
                dataPromotionDetail.IdCategory,
                dataPromotionDetail.IdCircuit,
                promoContent,
                conditionVisual,
                conditionText,
                promotionBrand,
                promotionVisual,
                ActivationValues.ACTIVATED,
                 Convert.ToInt64( dateTraitment.ToString("yyyyMM")));

                #endregion

                #region Execute Query
                _source.Insert(sql.ToString());
                #endregion

            }
            catch (System.Exception err)
            {
                throw new VeillePromoDALDbException("DataAccess InsertDataPromotionDetail Error. sql: " + sql.ToString(), err);
            }
        }
        #endregion

        #endregion

        

    }
}
