using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpressI.Classification.DAL.MediaBrand
{
    public class CanalLevelListDAL : ClassificationLevelListDAL
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public CanalLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.canal), language, source) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public CanalLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.canal), language, source, dbSchema)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public CanalLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.canal), idList, language, source) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public CanalLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.canal), idList, language, source, dbSchema)
        {
        }
        #endregion
    }
}
