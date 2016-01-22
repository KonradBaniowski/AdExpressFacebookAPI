using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType
{
    [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".ALL_BRAND")]
    public class DataAllBrand
    {
        [MapField("ID_BRAND")]
        public long IdBrand { get; set; }

        [MapField("BRAND")]
        public long Brand { get; set; }

        [MapField("ID_CIRCUIT")]
        public long IdCircuit { get; set; }

        [MapField("CIRCUIT")]
        public long Circuit { get; set; }
    }
}
