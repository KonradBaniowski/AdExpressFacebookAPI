using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace TNS.AdExpress.Rolex.Loader.DAL.DbType
{
    [TableName(Name = "ROLEX03.DATA_ROLEX")]
    public class DataRolex
    {
        [MapField("ID_DATA_ROLEX"), NotNull, PrimaryKey]
        public long IdDataRolex { get; set; }

        [MapField("ID_SITE"), NotNull]
        public long IdSite { get; set; }

        [MapField("ID_EMPLACEMENT"), NotNull]
        public long IdEmplacement { get; set; }

        [MapField("ID_LANGUAGE_DATA"), NotNull]
        public long IdLanguage { get; set; }

        [MapField("LOAD_DATE"), NotNull]
        public long LoadDate { get; set; }
    }
}
