using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using TNS.AdExpress.Domain.Translation;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PublicController : Controller
    {
        public ActionResult HomeNumbers()
        {
            var model = new HomeNumbersViewModel();
            model.Numbers = new Dictionary<string, string>();
            var file = HttpContext.Server.MapPath("~/Content/HomePageValues/HomeNumbers.xml");
            XDocument doc = XDocument.Load(file);
            //var labels = LabelsHelper.LoadPageLabels(33);
            foreach (XElement el in doc.Root.Elements())
            {
                var textValue = GestionWeb.GetWebWord(long.Parse(el.Attribute("webTextId").Value), 33);
                model.Numbers.Add(textValue, el.Attribute("value").Value);
            }
            return PartialView("HomeNumbers", model);
        }
    }
}