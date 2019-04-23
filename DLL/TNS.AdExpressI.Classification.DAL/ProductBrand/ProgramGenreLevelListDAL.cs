﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpressI.Classification.DAL.ProductBrand
{
    public class ProgramGenreLevelListDAL : ClassificationLevelListDAL
    {
        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public ProgramGenreLevelListDAL(int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.programGenre), language, source) {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        /// <param name="dbSchema">Schema</param>
        public ProgramGenreLevelListDAL(int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.programGenre), language, source, dbSchema)
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        public ProgramGenreLevelListDAL(string idList, int language, IDataSource source)
			: base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.programGenre), idList, language, source) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        ///<param name="idList">classification items' identifier list</param>
        /// <param name="language">Data language</param>
        /// <param name="source">Data source</param>
        /// <param name="dbSchema">Schema</param>
        public ProgramGenreLevelListDAL(string idList, int language, IDataSource source, string dbSchema)
            : base(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.programGenre), idList, language, source, dbSchema)
        {
        }
        #endregion
    }
}