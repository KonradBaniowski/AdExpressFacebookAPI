using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType
{
    [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".CIRCUIT")]
    public class DataCircuit
    {
        [MapField("ID_CIRCUIT"), PrimaryKey, NotNull]
        public long IdCircuit { get; set; }

        [MapField("ID_LANGUAGE"), PrimaryKey, NotNull]
        public long IdLanguage { get; set; }

        [MapField("CIRCUIT"), NotNull]
        public string Circuit { get; set; }

        [MapField("ACTIVATION"), NotNull]
        public long Activation { get; set; }
    }
}
