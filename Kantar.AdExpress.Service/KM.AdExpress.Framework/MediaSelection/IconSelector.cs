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
     

        public static string GetSvg(Vehicles.names element)
        {
            var icon = string.Empty;
            switch (element)
            {
                case Vehicles.names.cinema:
                    icon = "pictoCinema.svg";
                    break;
                case Vehicles.names.search:
                    icon = "pictoPaidSearch.svg";
                    break;
                case Vehicles.names.press:
                case Vehicles.names.magazine:
                    icon = "pictoPresse.svg";
                    break;
                case Vehicles.names.tv:
                    icon = "pictoTelevision.svg";
                    break;
                case Vehicles.names.evaliantMobile:
                    icon = "pictoEvaliantMobile.svg";
                    break;
                case Vehicles.names.mms:
                case Vehicles.names.internet:
                    icon = "pictoInternetDisplay.svg";
                    break;
                case Vehicles.names.directMarketing:
                    icon = "pictoCourrier.svg";
                    break;              
                case Vehicles.names.adnettrack:
                    icon = "pictoInternetEvaliant.svg";
                    break;
                case Vehicles.names.internationalPress:
                case Vehicles.names.newspaper:
                    icon = "pictoPresseInternationale.svg";
                    break;
                case Vehicles.names.outdoor:
                    icon = "pictoPubliciteExterieure.svg";
                    break;
                case Vehicles.names.dooh:
                    icon = "pictoDOOH.svg";
                    break;
                case Vehicles.names.radio:
                    icon = "pictoRadio.svg";
                    break;
                case Vehicles.names.tvSponsorship:
                    icon = "pictoTVPan-Euro.svg";
                    break;
                case Vehicles.names.mailValo:
                    icon = "pictoCourrierCreations.svg";
                    break;
                case Vehicles.names.plurimedia:
                    icon = "pictoPlurimedia.svg";
                    break;
                case Vehicles.names.plurimediaExtended:
                    icon = "pictoPlurimediaElargi.svg";
                    break;
                case Vehicles.names.audioDigital:
                    icon = "pictoAudioDigital.svg";
                    break;
                case Vehicles.names.others:
                    icon = "pictoTVPan-Euro.svg";
                    break;
                case Vehicles.names.social:
                    icon = "pictoSocial.svg";
                    break;
                default:
                    icon = "iconkantar-window50";
                    break;

            }
            return icon;
        }

        public static string GetHealthIcon(long id)
        {
            var icon = string.Empty;
            switch (id)
            {
                case 1 :
                    icon = "icon-user";
                    break;
                case 2 :              
                    icon = "iconkantar-news12";
                    break;
                case 3 :
                    icon = "iconkantar-planetary2";
                    break;                
                case 4 :
                    icon = "icon-people";
                    break;
                case 5 :                   
                    icon = "icon-chemistry";
                    break;
                

            }
            return icon;
        }
    }
}
