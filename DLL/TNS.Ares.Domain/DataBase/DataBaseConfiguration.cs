using System;
using System.Collections.Generic;
using System.Text;
using TNS.Ares.Domain.DataBaseDescription;
using TNS.FrameWork.DB.Common;

namespace TNS.Ares.Domain.DataBase {
    public class DataBaseConfiguration {

        #region Variables
        private static DataBaseDescription.DataBase _dataBase = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets target host, used for creating links in emails
        /// </summary>
        public static DataBaseDescription.DataBase DataBase {
            get { return (_dataBase); }
        }
        #endregion

        #region Load
        /// <summary>
        /// Load configuration from the given datasource
        /// </summary>
        /// <param name="source">Datasource containing Nyx configuration</param>
        public static void Load(IDataSource source) {
            _dataBase = new DataBaseDescription.DataBase(source);
        }
        #endregion
    }
}
