using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class ErrorMessage
    {
        public string EmptySelection { get; set; }

        public string SearchErrorMessage { get; set; }

        public string SocialErrorMessage { get; set; }

        public string UnitErrorMessage { get; set; }
    }
}