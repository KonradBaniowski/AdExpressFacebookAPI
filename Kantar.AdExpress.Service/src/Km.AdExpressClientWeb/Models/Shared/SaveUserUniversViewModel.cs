using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class SaveUserUniversViewModel
    {
        public int SelectedUserGroupId { get; set; }
        public IEnumerable<SelectListItem> UserGroups { get; set; }
        public int SelectedUserUniversId { get; set; }
        public IEnumerable<SelectListItem> UserUnivers { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string SelectUniversGroup { get; set; }
        public string SelectUnivers { get; set; }
        public string UniversLabel { get; set; }
        public bool CanSetDefaultUniverse { get; set; }
        public bool SetDefaultUniverse { get; set; }
        public string DefaultUniverse { get; set; }
        public string Submit { get; set; }
    }
}