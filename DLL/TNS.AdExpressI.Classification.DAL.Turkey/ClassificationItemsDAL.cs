using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;

namespace TNS.AdExpressI.Classification.DAL.Turkey
{
    public class ClassificationItemsDAL : DAL.ClassificationItemsDAL
    {
        #region Constantes
        /// <summary>
        /// PROGRAM LEVEL ID
        /// </summary>
        private const long PROGRAM_LEVEL_ID = 40;
        /// <summary>
        /// PROGRAM TYPOLOGY LEVEL ID
        /// </summary>
        private const long PROGRAM_TYPOLOGY_LEVEL_ID = 41;
        /// <summary>
        /// SPOT SUB TYPE LEVEL ID
        /// </summary>
        private const long SPOT_SUB_TYPE_LEVEL_ID = 42;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="dimension">Dimension</param>
        public ClassificationItemsDAL(WebSession session, TNS.Classification.Universe.Dimension dimension) : base(session, dimension) {}
        #endregion

        protected override View GetCustomView(Dimension dimension, string classificationLevelLabel)
        {
            var programLevel = UniverseLevels.Get(PROGRAM_LEVEL_ID);
            var programTypologyLevel = UniverseLevels.Get(PROGRAM_TYPOLOGY_LEVEL_ID);
            var spotSubTypeLevel = UniverseLevels.Get(SPOT_SUB_TYPE_LEVEL_ID);

            if(classificationLevelLabel == programLevel.TableName)
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMediaProgram);

            if (classificationLevelLabel == programTypologyLevel.TableName)
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMediaProgram);

            if (classificationLevelLabel == spotSubTypeLevel.TableName)
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMediaSpotType);

            return GetView(dimension);
        }

        protected override View GetCustomView(Dimension dimension, string classificationLevelLabel, string selectedItemTableName)
        {
            var programLevel = UniverseLevels.Get(PROGRAM_LEVEL_ID);
            var programTypologyLevel = UniverseLevels.Get(PROGRAM_TYPOLOGY_LEVEL_ID);
            var spotSubTypeLevel = UniverseLevels.Get(SPOT_SUB_TYPE_LEVEL_ID);

            if (selectedItemTableName == programLevel.TableName)
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMediaProgram);

            if (selectedItemTableName == programTypologyLevel.TableName)
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMediaProgram);

            if (selectedItemTableName == spotSubTypeLevel.TableName)
                return WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allMediaSpotType);

            return GetCustomView(dimension, classificationLevelLabel);
        }
    }
}
