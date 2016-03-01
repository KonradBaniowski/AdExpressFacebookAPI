using Km.AdExpressClientWeb.Models.MediaSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class PeriodSelectionViewModel
    {
        public PeriodViewModel PeriodViewModel { get; set; }

        public List<MediaPlanNavigationNode> NavigationBar { get; set; }
        public PresentationModel Presentation { get; set; }
    }
}