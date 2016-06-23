using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.SocialMedia
{
        public class ResultsViewModel
        {
            public List<NavigationNode> NavigationBar { get; set; }
            public PresentationModel Presentation { get; set; }
            public Labels Labels { get; set; }
        }
}