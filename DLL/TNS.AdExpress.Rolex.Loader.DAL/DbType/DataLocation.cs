using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace TNS.AdExpress.Rolex.Loader.DAL.DbType
{
      [TableName(Name = "ROLEX03.EMPLACEMENT")]
    public class DataLocation
    {
        [MapField("ID_EMPLACEMENT"), NotNull, PrimaryKey]
        public long IdEmplacement { get; set; }

        [MapField("ID_LANGUAGE"), NotNull, PrimaryKey]
        public long IdLanguage { get; set; }

        [MapField("EMPLACEMENT"), NotNull]
        public String Emplacement { get; set; }
    }
}
