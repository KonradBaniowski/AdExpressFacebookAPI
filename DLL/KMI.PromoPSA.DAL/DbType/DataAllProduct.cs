using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType
{
    [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".ALL_PRODUCT")]
    public class DataAllProduct
    {
        [MapField("ID_PRODUCT")]
        public long IdProduct { get; set; }

        [MapField("PRODUCT")]
        public string Product { get; set; }

        [MapField("ID_CATEGORY")]
        public long IdCategory { get; set; }

        [MapField("CATEGORY")]
        public string Category { get; set; }

        [MapField("ID_SEGMENT")]
        public long IdSegment { get; set; }

        [MapField("SEGMENT")]
        public string Segment { get; set; }
    }
}
