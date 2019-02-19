﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpressI.Classification.DAL.ProductBrand
{
    public class PurchasingAgencyLevelListDAL : ClassificationLevelListDAL
    {

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public PurchasingAgencyLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.product), language, source) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        /// <param name="dbSchema">Schema</param>
        public PurchasingAgencyLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.product), language, source, dbSchema)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public PurchasingAgencyLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.product), idList, language, source) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        /// <param name="dbSchema">Schema</param>
        public PurchasingAgencyLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.product), idList, language, source, dbSchema)
        {
        }
        #endregion

    }
}
