using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;

namespace KM.AdExpress.Framework.MediaSelection
{
    public static class IconSelector
    {
        public static string getIcon(Vehicles.names element)
        {
            var icon = string.Empty;
            switch (element)
            {
                case Vehicles.names.cinema:
                    icon = "iconkantar-video163";
                    break;
                case Vehicles.names.search:
                    icon = "iconkantar-internet94";
                    break;
                case Vehicles.names.press:
                case Vehicles.names.magazine:
                    icon = "iconkantar-news12";
                    break;
                case Vehicles.names.tv:
                    icon = "iconkantar-television20";
                    break;
                case Vehicles.names.evaliantMobile:
                    icon = "iconkantar-cellphone55";
                    break;
                case Vehicles.names.mms:
                case Vehicles.names.internet:
                    icon = "iconkantar-planetary2";
                    break;
                case Vehicles.names.directMarketing:
                    icon = "iconkantar-mail114";
                    break;
                //case Vehicles.names.emailing:
                //    icon = "iconkantar-envelope82";
                //    break;
                case Vehicles.names.adnettrack:
                    icon = "iconkantar-monitor74";
                    break;
                case Vehicles.names.internationalPress:
                case Vehicles.names.newspaper:
                    icon = "iconkantar-newspapers5";
                    break;
                case Vehicles.names.outdoor:
                    icon = "iconkantar-commercial";
                    break;
                case Vehicles.names.dooh:
                    icon = "iconkantar-dooh";
                    break;
                case Vehicles.names.radio:
                    icon = "iconkantar-radio46";
                    break;
                case Vehicles.names.tvSponsorship:
                    icon = "iconkantar-wireless-connectivity79";
                    break;
                case Vehicles.names.mailValo:
                    icon = "iconkantar-envelope82";
                    break;
                default:
                    icon = "iconkantar-window50";
                    break;

            }
            return icon;
        }
    }
}
