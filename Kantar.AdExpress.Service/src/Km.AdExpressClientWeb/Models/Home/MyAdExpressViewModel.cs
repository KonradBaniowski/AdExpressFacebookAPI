using Kantar.AdExpress.Service.Core.Domain;
using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.Home
{
    public class MyAdExpressViewModel
    {
        public PresentationModel PresentationModel { get; set; }
        public Labels Labels { get; set; }
        public AdExpressUniversResponse SavedResults { get; set; }
        public AdExpressUniversResponse SavedUnivers { get; set; }
        public List<Alert> Alerts { get; set; }
    }
}