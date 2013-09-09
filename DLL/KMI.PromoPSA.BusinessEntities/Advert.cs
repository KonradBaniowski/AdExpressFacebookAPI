using System;

namespace KMI.PromoPSA.BusinessEntities
{
    /// <summary>
    /// Advert class
    /// </summary>
    [Serializable]
    public class Advert
    {

        public long IdForm { get; set; }

        public long Status { get; set; }

        public long IdDataPromotion { get; set; }

        public long IdProduct { get; set; }

        public long IdBrand { get; set; }

        public long DateBeginNum { get; set; }

        public long DateEndNum { get; set; }

        public long IdSegment { get; set; }

        public long IdCircuit { get; set; }

        public long IdCategory { get; set; }

        public string PromotionContent { get; set; }

        public string ConditionText { get; set; }

        public string Script { get; set; }

        public long ExcluWeb { get; set; }

    }
}
