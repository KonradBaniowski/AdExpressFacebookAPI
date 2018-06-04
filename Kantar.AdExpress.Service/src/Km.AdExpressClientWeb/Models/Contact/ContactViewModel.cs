using Km.AdExpressClientWeb.Models.Shared;
using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Models.Contact
{
    public class ContactViewModel
    {
        //DISPLAY
        public Labels Labels { get; set; }
        public PresentationModel PresentationModel { get; set; }
        public List<SelectListItem> QuestionsTagItem { get; set; }

        //FORM
        [Required]
        public string Company { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Mail { get; set; }
        [Required]
        public string Country { get; set; }
        public string QuestionTag { get; set; }
        [Required]
        public string Comment { get; set; }
        public string RedirectUrl { get; set; }

        public string ClientServicePhoneNumber { get; set; }
        public string ClientServiceEmail { get; set; }
    }
}