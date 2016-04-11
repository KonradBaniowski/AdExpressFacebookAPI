using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
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
        public string Company { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNumber { get; set; }
        public string Mail { get; set; }
        public string Country { get; set; }
        public string QuestionTag { get; set; }
        public string Comment { get; set; }
    }
}