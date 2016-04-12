using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Km.AdExpressClientWeb.Controllers
{
    public class PdfController : Controller
    {
        public ActionResult PDF(string fileName)
        {
            var file = HttpContext.Server.MapPath("~/Content/pdf/" + fileName);
            if (System.IO.File.Exists(file))
            {
                return RedirectToAction("OpenPDF",new { fileName = fileName });
            };
            return RedirectToAction("Index", "Home");
        }

        public FileStreamResult OpenPDF(string fileName)
        {
            var file = HttpContext.Server.MapPath("~/Content/pdf/" + fileName);
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }
    }
}