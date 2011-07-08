using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.VP.Loader.Domain;

namespace TNS.AdExpressI.VP.Loader.DAL.Data {
    /// <summary>
    /// Promo Veille Interface
    /// </summary>
    public interface IDataVeillePromoDAL : IVeillePromoDAL {

        /// <summary>
        /// Get Data Promotion Detail List from a File data source Excel
        /// </summary>
        /// <param name="source">data source</param>
        /// <returns>Data Promotion Detail List</returns>
        DataPromotionDetails GetDataPromotionDetailList(FileDataSource source);
        /// <summary>
        /// Has data for the date traiment passed in parameter
        /// </summary>
        /// <param name="dateTraitment">Date Traitment</param>
        /// <returns>Has Data or not for the date traiment passed in parameter</returns>
        bool HasData(DateTime dateTraitmentBegin, DateTime dateTraitmentEnd);
        /// <summary>
        /// Get Picture File Name
        /// </summary>
        /// <param name="fileList">File List</param>
        /// <returns>Picture File Name List</returns>
        Dictionary<string, PictureMatching> GetPictureFileName(List<string> fileList);
        /// <summary>
        /// Delete data between dateBegin parameter and dateEnd parameter
        /// </summary>
        /// <param name="dateBegin">Date Begin</param>
        /// <param name="dateEnd">Date End</param>
        void DeleteData(DateTime dateBegin, DateTime dateEnd);
        /// <summary>
        /// Insert data Promotion Detail
        /// </summary>
        /// <param name="dateTraitment">Date Traitment</param>
        /// <param name="dataPromotionDetail">Data Promotion Detail</param>
        void InsertDataPromotionDetail(DateTime dateTraitment, DataPromotionDetail dataPromotionDetail);
    }
}
