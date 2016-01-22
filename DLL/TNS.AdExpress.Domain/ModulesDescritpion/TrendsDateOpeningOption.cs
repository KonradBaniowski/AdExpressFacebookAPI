using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpress.Domain.ModulesDescritpion
{
    public class TrendsDateOpeningOption
    {
        #region Variables
        /// <summary>
        /// Vehicle id
        /// </summary>
        protected Vehicles.names _id;

        /// <summary>
        /// Default opening option Id
        /// </summary>
        protected Tendencies.DateOpeningOption _defaultOptionId;

        /// <summary>
        /// Default Date opening options Id
        /// </summary>
        protected List<Tendencies.DateOpeningOption> _optionsIds = new List<Tendencies.DateOpeningOption>();
        #endregion

        #region Accessors
        /// <summary>
        /// Get vehicle id
        /// </summary>
        public Vehicles.names Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Get default option identifier
        /// </summary>
        public Tendencies.DateOpeningOption DefaultOptionId
        {
            get { return _defaultOptionId; }
        }

        /// <summary>
        /// Get option identifier List
        /// </summary>
        public List<Tendencies.DateOpeningOption> OptionsIds
        {
            get { return _optionsIds; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="defaultOptionId">default Option Identifiers</param>
        /// <param name="optionsIds">options Identifiers</param>
        public TrendsDateOpeningOption(Vehicles.names id, Tendencies.DateOpeningOption defaultOptionId, List<Tendencies.DateOpeningOption> optionsIds)
        {
            _id = id;
            _defaultOptionId = defaultOptionId;
            if (optionsIds == null || optionsIds.Count==0) throw new ArgumentNullException(" Invalid optionsIds parameter ");
            _optionsIds = optionsIds;
        }
        #endregion
    }
}
