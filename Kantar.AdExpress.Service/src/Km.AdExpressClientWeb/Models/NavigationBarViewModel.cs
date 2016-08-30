using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{
    public class NavigationBarViewModel
    {
        public List<NavigationNode> NavigationNodes { get; set; }

        public Labels Labels { get; set; }
    }
}