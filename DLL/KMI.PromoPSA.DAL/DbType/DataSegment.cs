using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.DAL.DbType
{
    [TableName(Name = Constantes.Db.PROMO_SCHEMA + ".SEGMENT")]
    public class DataSegment
    {
        [MapField("ID_SEGMENT")]
        public long IdSegment { get; set; }

        [MapField("ID_LANGUAGE"), PrimaryKey, NotNull]
        public long IdLanguage { get; set; }

        [MapField("SEGMENT"), NotNull]
        public string Segment { get; set; }

        [MapField("ACTIVATION"), NotNull]
        public long Activation { get; set; }
    }
}
