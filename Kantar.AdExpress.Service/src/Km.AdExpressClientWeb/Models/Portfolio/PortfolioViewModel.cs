using Km.AdExpressClientWeb.Models.MediaSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.Portfolio
{
  
    public class MediaSelectionViewModel
    {
        public List<Kantar.AdExpress.Service.Core.Domain.Media> Medias { get; set; }
        public bool Multiple { get; set; }
        public List<int> IdMediasCommon { get; set; }

        public List<NavigationNode> NavigationBar { get; set; }

        public ErrorMessage ErrorMessage { get; set; }

        public PresentationModel Presentation { get; set; }

    }

}