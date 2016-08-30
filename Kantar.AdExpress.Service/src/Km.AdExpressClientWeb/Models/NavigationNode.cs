using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models
{

    public class NavigationNode
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        public int Position { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Action { get; set; }

        public string Controller { get; set; }

        public string IconCssClass { get; set; }

        public bool IsDisabled{ get; set; }

    }
}