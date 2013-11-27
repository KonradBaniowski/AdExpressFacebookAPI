using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.Aphrodite.Domain;
using KMI.AdExpress.Aphrodite.Constantes;
using TNS.FrameWork.DB.Common;

namespace KMI.AdExpress.Aphrodite.DAL {
    /// <summary>
    /// Trends Data Access Interface
    /// </summary>
    public interface ITrendsDAL {
        /// <summary>
        /// Remove trends data for a media
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="source">Data source</param>
        void Remove(MediaTypeInformation mediaTypeInformation, IDataSource source);
        /// <summary>
        /// Remove trends data for a media
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="treatmentType">Treatment Type</param>
        /// <param name="source">Data source</param>
        void Remove(MediaTypeInformation mediaTypeInformation, Application.TreatmentType treatmentType, IDataSource source);
        /// <summary>
        /// Insert Data
        /// </summary>
        /// <param name="periodBeginning">Period Beginning</param>
        /// <param name="periodEnding">Period Ending</param>
        /// <param name="periodBeginningPrev">Period Beginning Prev</param>
        /// <param name="periodEndingPrev">Period Ending Prev</param>
        /// <param name="periodId">Period Id</param>
        /// <param name="period">Period</param>
        /// <param name="year">Year</param>
        /// <param name="comparativeType">Comparative type: comparative week or date to date</param>
        /// <param name="mediaTypeInformation">Media Type Information</param>
        /// <param name="cumul">Cumul</param>
        /// <param name="treatmentType">Treatment Type : Week or Month</param>
        /// <param name="source">Source</param>
        void InsertData(string periodBeginning, string periodEnding, string periodBeginningPrev, string periodEndingPrev, string periodId, string period, string year, string comparativeType, MediaTypeInformation mediaTypeInformation, string cumul, Application.TreatmentType treatmentType, IDataSource source);
        /// <summary>
		/// Insertion des Sous totaux dans la table
		/// </summary>
		/// <param name="datePeriode">Periode YYYYMM</param>
        /// <param name="source">Connection Oracle</param>
        /// <param name="mediaTypeInformation">Media Type Information</param>
        /// <param name="total">Total</param>
        /// <<param name="cumul">Cumul</param>
        /// <param name="type_tendency">Type tendency</param>
        /// <param name="treatmentType">Treatment Type</param>
        void InsertSubTotal(string datePeriode, IDataSource source, MediaTypeInformation mediaTypeInformation, bool total, string cumul, string type_tendency, Application.TreatmentType treatmentType);
        /// <summary>
        /// Compute periods and insert cumul data in table month or week
        /// </summary>
        /// <param name="mediaTypeInformation">Media type information</param>
        /// <param name="currentDay">Current Day</param>
        /// <param name="treatmentType">Treatment Type</param>
        /// <param name="source">Data source</param>
        void InsertCumul(MediaTypeInformation mediaTypeInformation, DateTime currentDay, Application.TreatmentType treatmentType, IDataSource source);
        /// <summary>
        /// Insert PDM
        /// </summary>
        /// <param name="mediaTypeInformation">Media Type Information</param>
        /// <param name="source">Connection Oracle</param>
        /// <param name="total">Total</param>
        void InsertPDM(MediaTypeInformation mediaTypeInformation, IDataSource source, bool total, Application.TreatmentType treatmentType);
    }
}
