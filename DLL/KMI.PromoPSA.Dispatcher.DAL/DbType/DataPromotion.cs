using System;
using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace KMI.PromoPSA.Dispatcher.DAL.DbType
{
    [TableName(Name = "PROMO03.DATA_PROMOTION")]
    public class DataPromotion
    {
        [MapField("ID_DATA_PROMOTION"), PrimaryKey, NotNull]
        public long IdDataPromotion { get; set; }

        [MapField("ID_LANGUAGE_DATA_I"), NotNull]
        public long IdLanguageDataI { get; set; }

        [MapField("ID_PRODUCT")]
        public long IdProduct { get; set; }

        [MapField("ID_BRAND")]
        public long IdBrand { get; set; }

        [MapField("DATE_BEGIN_NUM")]
        public long DateBeginNum { get; set; }

        [MapField("DATE_END_NUM")]
        public long DateEndNum { get; set; }

        [MapField("ID_SEGMENT")]
        public long IdSegment { get; set; }

        [MapField("ID_CIRCUIT")]
        public long IdCircuit { get; set; }

        [MapField("ID_CATEGORY")]
        public long IdCategory { get; set; }

        [MapField("PROMOTION_CONTENT")]
        public string PromotionContent { get; set; }

        [MapField("CONDITION_TEXT")]
        public string ConditionText { get; set; }

        [MapField("SCRIPT")]
        public string Script { get; set; }

        [MapField("EXCLU_WEB"), NotNull]
        public long ExcluWeb { get; set; }

        [MapField("ACTIVATION"), NotNull]
        public long Activation { get; set; }

        [MapField("ID_FORM")]
        public long? IdForm { get; set; }

        [MapField("LOAD_DATE")]
        public long LoadDate { get; set; }
    }
}
