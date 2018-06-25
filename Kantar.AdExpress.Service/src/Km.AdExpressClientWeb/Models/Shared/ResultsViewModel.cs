using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class ResultsViewModel
    {
        public NavigationBarViewModel NavigationBar { get; set; }
        public PresentationModel Presentation { get; set; }
        public Labels Labels { get; set; }
        public bool IsAlertVisible { get; set; }
        public List<ExportTypeViewModel> ExportTypeViewModels { get; set; }
        public bool GrpAvailable { get; set; }
    }
}