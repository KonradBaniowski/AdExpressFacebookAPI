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
        public bool RememberEmail { get; set; }
        public string ExportType { get; set; }
        public Labels Labels { get; set; }
    }
}