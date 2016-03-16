using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.MediaSchedule
{
    public class ResultsViewModel
    {
        public List<NavigationNode> NavigationBar { get; set; }
        public PresentationModel Presentation { get; set; }
    }
}