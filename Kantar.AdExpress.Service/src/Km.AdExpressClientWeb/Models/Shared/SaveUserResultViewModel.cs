using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class SaveUserResultViewModel
    {
        public int SelectedUserFolderId { get; set; }
        public IEnumerable<SelectListItem> UserFolders { get; set; }
        public int SelectedUserResultId { get; set; }
        public IEnumerable<SelectListItem> UserResults { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string SelectFolder { get; set; }
        public string SelectResult { get; set; }
        public string ResultLabel { get; set; }
        public string SubmitLabel { get; set; }
    }
}