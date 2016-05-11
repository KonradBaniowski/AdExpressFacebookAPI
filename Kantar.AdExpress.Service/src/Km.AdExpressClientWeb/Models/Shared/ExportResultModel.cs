using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Models.Shared
{
    public class ExportResultModel
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string Email { get; set; }
        public List<SelectListItem> ExportType { get; set; }
        public int SelectedExportTypeId { get; set; }
        public Labels Labels { get; set; }
    }
}