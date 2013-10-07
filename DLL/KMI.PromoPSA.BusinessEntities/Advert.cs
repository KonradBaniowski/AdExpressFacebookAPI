﻿using System;

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

        public string PromotionBrand { get; set; }

        public string PromotionContent { get; set; }

        public string PromotionVisual { get; set; }

        public string ConditionText { get; set; }

        public string Script { get; set; }

        public long ExcluWeb { get; set; }

        public long DateMediaNum { get; set; }

        public long Activation { get; set; }

        public long LoadDate { get; set; }

        public long IdVehicle { get; set; }

        public string VehicleName {
            get {
                switch (IdVehicle) { 
                    case 1:
                        return "Presse";
                    case 3:
                        return "Tv";
                    case 7:
                        return "Internet";
                    case 8:
                        return "Publicité Extérieur";
                    default:
                        return "";
                }
            }
        }

        public string ActivationName {
            get {
                switch (Activation) {
                    case 20:
                        return "Codifiée";
                    case 30:
                        return "Rejetée";
                    case 40:
                        return "A Codifier";
                    case 90:
                        return "Litige";
                    default:
                        return "";
                }
            }
        }

        public string DateMediaNumFormated {
            get {
                return DateMediaNum.ToString().Substring(6, 2) + "/" + 
                       DateMediaNum.ToString().Substring(4, 2) + "/" + 
                       DateMediaNum.ToString().Substring(0, 4);
            }
        }

        public string LoadDateFormated {
            get {
                return LoadDate.ToString().Substring(4, 2) + "/" +
                       LoadDate.ToString().Substring(0, 4);
            }
        }

    }
}
