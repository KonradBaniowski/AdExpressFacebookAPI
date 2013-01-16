using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace TNS.AdExpress.Rolex.Loader.DAL.DbType
{
    [TableName(Name = "ROLEX03.PRESENCE_TYPE")]
    public class DataPresenceType
    {
        [MapField("ID_PRESENCE_TYPE"), NotNull, PrimaryKey]
        public long IdTypePresence { get; set; }

        [MapField("ID_LANGUAGE"), NotNull, PrimaryKey]
        public long IdLanguage { get; set; }

        [MapField("PRESENCE_TYPE"), NotNull]
        public String TypePresence { get; set; }
    }
}
