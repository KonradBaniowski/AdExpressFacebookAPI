using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType
{
     [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".CATEGORY")]
    public class DataCategory
    {
        [MapField("ID_CATEGORY"), PrimaryKey, NotNull]
        public long IdCategory { get; set; }

        [MapField("ID_LANGUAGE"), PrimaryKey, NotNull]
        public long IdLanguage { get; set; }

        [MapField("ID_SEGMENT"), NotNull]
        public long IdSegment { get; set; }

        [MapField("CATEGORY"), NotNull]
        public string Category { get; set; }

        [MapField("ACTIVATION"), NotNull]
        public long Activation { get; set; }
    }
}
