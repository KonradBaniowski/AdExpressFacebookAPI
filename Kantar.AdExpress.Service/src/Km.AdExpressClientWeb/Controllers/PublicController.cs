using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PublicController : Controller
    {
        public ActionResult HomeNumbers(int siteLanguage=33)
        {
            var model = new HomeNumbersViewModel();
            model.Numbers = new Dictionary<string, string>();
            var file = HttpContext.Server.MapPath(string.Format("~/Configuration/{0}/HomeNumbers.xml",WebApplicationParameters.CountryCode));
            XDocument doc = XDocument.Load(file);
            foreach (XElement el in doc.Root.Elements())
            {
                var textValue = GestionWeb.GetWebWord(long.Parse(el.Attribute("webTextId").Value), siteLanguage);
                model.Numbers.Add(textValue, el.Attribute("value").Value);
            }
            return PartialView("HomeNumbers", model);
        }
    }
}