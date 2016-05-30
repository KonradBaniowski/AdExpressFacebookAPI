using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Models.Alert
{
    public class CreateAlertModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Email { get; set; }
        public string PeriodicityDescription { get; set; }//Daily, Weekly, Monthly
        public int PeriodicityValue { get; set; } // on 21st
        public Labels Labels { get; set; }
        public int SelectedPeriodicityId { get; set;}
        public List<SelectListItem> Periodicity { get; set; }
        public List<SelectListItem> WeekDays { get; set; }

    }
}