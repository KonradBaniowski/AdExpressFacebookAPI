#region Information
// Author: G. Facon
// Creation date: 26/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpress.Portofolio.DAL {
    /// <summary>
    /// Portofolio Data access Interface
    /// </summary>
    public interface IPortofolioDAL {
        /// <summary>
        /// Get Data for the Media portofolio
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetMediaPortofolio();
        /// <summary>
        /// Get Data for the portofolio calendar
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetDataCalendar();
        /// <summary>
        /// Get the following fields : Category, Media Owner, Interest Center and Periodicity for press
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetCategoryMediaSellerData();
        /// <summary>
        /// Get total investment and date of issue
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetInvestment();
        /// <summary>
        /// Get insertions number
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetInsertionNumber();
        /// <summary>
        /// Get type sale
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetTypeSale();
        /// <summary>
        /// Get Page number
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetPage();
        /// <summary>
        /// Get number of products, number of products in the pige, number of products in the media and number of advertisers
        /// </summary>
        /// <returns>Data Set</returns>
        object[] NumberProductAdvertiser();
        /// <summary>
        /// Get Encart 
        /// </summary>
        /// <returns>Data Set</returns>
        object[] NumberPageEncart();
        /// <summary>
        /// Get Investment By Media
        /// </summary>
        /// <returns>Data Set</returns>
        Hashtable GetInvestmentByMedia();
        /// <summary>
        /// Get adbreak data
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetEcranData();
        /// <summary>
        /// Get New Product
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetNewProduct();
        /// <summary>
        /// Get Tv Or Radio Struct Data
        /// </summary>
        /// <param name="HourBegin">Hour Begin</param>
        /// <param name="HourEnd">Hour End</param>
        /// <returns>Data Set</returns>
        DataSet GetTvOrRadioStructData(int HourBegin, int HourEnd);
        /// <summary>
        /// Get Press Struct Data
        /// </summary>
        /// <param name="ventilation">ventilation</param>
        /// <returns>Data Set</returns>
        DataSet GetPressStructData(PortofolioStructure.Ventilation ventilation);
        /// <summary>
        /// Get dates list
        /// </summary>
        /// <param name="conditionEndDate">Add condition end date</param>
        /// <returns>Data Set</returns>
        DataSet GetListDate(bool conditionEndDate);
        /// <summary>
        /// Get Commercial Break For Tv & Radio
        /// </summary>
        /// <returns>Data Set</returns>
        DataSet GetCommercialBreakForTvRadio();
        /// <summary>
        /// Is Belong To Tv Nat Thematiques
        /// </summary>
        /// <returns>True if belong To Tv Nat Thematiques</returns>
        bool IsBelongToTvNatThematiques();
    }
}
