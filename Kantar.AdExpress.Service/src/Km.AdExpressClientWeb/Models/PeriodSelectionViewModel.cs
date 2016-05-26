﻿using Km.AdExpressClientWeb.Models.MediaSchedule;
using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class PeriodSelectionViewModel
    {
        public PeriodViewModel PeriodViewModel { get; set; }

        public List<NavigationNode> NavigationBar { get; set; }
        
        public PresentationModel Presentation { get; set; }

        public ErrorMessage ErrorMessage { get; set; }
        public long CurrentModule { get; set; }
    }
}