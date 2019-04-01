using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpressI.Classification.DAL.ProductBrand
{
    public class SpotSubTypeLevelListDAL : ClassificationLevelListDAL
    {

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public SpotSubTypeLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.spotSubType), language, source) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        /// <param name="dbSchema">Schema</param>
        public SpotSubTypeLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.spotSubType), language, source, dbSchema)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public SpotSubTypeLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.spotSubType), idList, language, source) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        /// <param name="dbSchema">Schema</param>
        public SpotSubTypeLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.spotSubType), idList, language, source, dbSchema)
        {
        }
        #endregion

    }
}
