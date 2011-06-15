using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TNS.AdExpress.VP.Loader.Domain.Constantes {
    public class Constantes {

        #region Common Layers
        /// <summary>
        /// contains all constantes to identify the common web site layers
        /// </summary>
        public class Layers {
            public enum Id {
                /// <summary>
                /// Classification layer Id
                /// </summary>
                classification = 0,
                /// <summary>
                /// Classification DAL layer Id
                /// </summary>
                classificationDAL = 1,
                /// <summary>
                /// Rules layer id
                /// </summary>
                rules = 2,
                /// <summary>
                /// Data Access layer id
                /// </summary>
                dataAccess = 3,
            }
        }

        #endregion

    }
}
