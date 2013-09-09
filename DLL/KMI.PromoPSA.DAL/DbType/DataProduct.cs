using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType
{
    [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".PRODUCT")]
    public class DataProduct
    {
        [MapField("ID_PRODUCT"), PrimaryKey, NotNull]
        public long IdProduct { get; set; }

        [MapField("ID_LANGUAGE"), PrimaryKey, NotNull]
        public long IdLanguage { get; set; }

        [MapField("ID_CATEGORY"), NotNull]
        public long IdCategory { get; set; }

        [MapField("PRODUCT"), NotNull]
        public string Product { get; set; }

        [MapField("ACTIVATION"), NotNull]
        public long Activation { get; set; }
    }
}
