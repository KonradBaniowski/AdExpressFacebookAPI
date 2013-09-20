using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType
{
    [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".BRAND")]
    public class DataBrand
    {
        [MapField("ID_BRAND"), PrimaryKey, NotNull]
        public long IdBrand { get; set; }

        [MapField("ID_LANGUAGE"), PrimaryKey, NotNull]
        public long IdLanguage { get; set; }

        [MapField("ID_CIRCUIT"), NotNull]
        public long IdCircuit { get; set; }

        [MapField("BRAND"), NotNull]
        public string Brand { get; set; }

        [MapField("ACTIVATION"), NotNull]
        public long Activation { get; set; }
    }
}
