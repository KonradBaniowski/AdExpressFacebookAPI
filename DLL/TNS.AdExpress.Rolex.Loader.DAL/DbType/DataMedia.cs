using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace TNS.AdExpress.Rolex.Loader.DAL.DbType
{
      [TableName(Name = "ROLEX03.SITE")]
    public class DataMedia
    {
        [MapField("ID_SITE"), NotNull, PrimaryKey]
          public long IdSite { get; set; }

        [MapField("ID_LANGUAGE"), NotNull, PrimaryKey]
        public long IdLanguage { get; set; }

        [MapField("SITE"), NotNull]
        public String Site { get; set; }

       
    }
}
