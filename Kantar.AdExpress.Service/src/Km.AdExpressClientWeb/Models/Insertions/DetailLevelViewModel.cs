using Kantar.AdExpress.Service.Core.Domain;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.Insertions
{
    public class DetailLevelViewModel
    {
        public List<DetailLevel> Items { get; set; }
        public Labels Labels { get; set; }
    }
}