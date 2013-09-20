using System;

namespace KMI.PromoPSA.BusinessEntities
{
     [Serializable]
   public class AdvertStatus
    {
        
        public long IdForm { get; set; }

        public long Activation { get; set; }

        public long IdLogin { get; set; }

        public long LoadDate { get; set; }

       public long IdDataPromotion { get; set; }
         
       
    }
}
