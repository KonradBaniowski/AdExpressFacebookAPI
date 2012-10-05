using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace TNS.AdExpress.Rolex.Loader.DAL.DbType
{
    [TableName(Name = "ROLEX03.TYPE_PRESENCE")]
    public class DataTypePresence
    {
        [MapField("ID_TYPE_PRESENCE"), NotNull, PrimaryKey]
        public long IdTypePresence { get; set; }

        [MapField("ID_LANGUAGE"), NotNull, PrimaryKey]
        public long IdLanguage { get; set; }

        [MapField("TYPE_PRESENCE"), NotNull]
        public String TypePresence { get; set; }
    }
}
