using Km.AdExpressClientWeb.Helpers;
using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models.Contact;
using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

namespace Km.AdExpressClientWeb.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Index(string returnUrl = "/", int siteLanguage = -1)
        {
            if (siteLanguage == -1) siteLanguage = WebApplicationParameters.DefaultLanguage;

            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(Convert.ToInt32(siteLanguage));
            ViewBag.SiteLanguage = siteLanguage;
            var model = new ContactViewModel();
            model.QuestionsTagItem = new List<SelectListItem>();
            var labels = LabelsHelper.LoadPageLabels(siteLanguage);
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag1, Value = labels.QuestionTag1 });
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag2, Value = labels.QuestionTag2 });
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag3, Value = labels.QuestionTag3 });
            model.Labels = LabelsHelper.LoadPageLabels(siteLanguage);
            model.Labels.CurrentController = "Contact";
            model.PresentationModel = LoadPresentationBar(siteLanguage, false);
            switch (WebApplicationParameters.CountryCode)
            {
                case CountryCode.FINLAND:
                    model.ClientServicePhoneNumber = "+358 9 613 500";
                    model.ClientServiceEmail = "tnsfinland@tnsglobal.com";
                    break;
                case CountryCode.POLAND:
                    model.ClientServicePhoneNumber = "+48 71 78 75 300";
                    model.ClientServiceEmail = "aleksandra.misterska@kantarmedia.com";
                    break;
                default:
                    model.ClientServicePhoneNumber = "01 30 74 87 78";
                    model.ClientServiceEmail = "sc.adexpress@kantarmedia.com";
                    break;
            }
            return View(model);
        }

        //
        // GET: /Contact/ChangeLanguage
        [AllowAnonymous]
        public JsonResult ChangeLanguage(string returnUrl, int siteLanguage = -1)
        {
            if (siteLanguage == -1) siteLanguage = WebApplicationParameters.DefaultLanguage;

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.SiteLanguageName = PageHelper.GetSiteLanguageName(Convert.ToInt32(siteLanguage));
            ViewBag.SiteLanguage = siteLanguage;
            var model = new ContactViewModel();
            model.QuestionsTagItem = new List<SelectListItem>();
            var labels = LabelsHelper.LoadPageLabels(siteLanguage);
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag1, Value = labels.QuestionTag1 });
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag2, Value = labels.QuestionTag2 });
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag3, Value = labels.QuestionTag3 });
            model.Labels = LabelsHelper.LoadPageLabels(siteLanguage);
            model.Labels.CurrentController = "Contact";
            model.RedirectUrl = string.Format("{0}?siteLanguage={1}", returnUrl, siteLanguage);
            model.PresentationModel = LoadPresentationBar(siteLanguage, false);

            JsonResult jsonModel = Json(model, JsonRequestBehavior.AllowGet);
            return jsonModel;
        }

        public ActionResult ContactUs(ContactViewModel form, int siteLang = -1)
        {
            if (siteLang == -1) siteLang = WebApplicationParameters.DefaultLanguage;

            if (ModelState.IsValid)
            {
                var body =
                    "<p>Email From: {0} ({1}-{6})</p><p>Infos:</p><p>{2} {3} {4}</p><p>Message:</p><p>{5}</p>";
                var message = new MailMessage();

                switch (WebApplicationParameters.CountryCode)
                {
                    case CountryCode.FINLAND:
                        message.To.Add(new MailAddress("tnsfinland@tnsglobal.com"));
                        break;
                    case CountryCode.POLAND:
                        message.To.Add(new MailAddress("aleksandra.misterska@kantarmedia.com"));
                        break;
                    default:
                        message.To.Add(new MailAddress("sc.adexpress@kantarmedia.com"));
                        break;
                }
                message.Subject = form.QuestionTag;

                if (WebApplicationParameters.CountryCode == CountryCode.POLAND)
                {
                    message.Body = GestionWeb.GetWebWord(68, siteLang) + ": " + form.Company + "<br/>";
                    message.Body += GestionWeb.GetWebWord(67, siteLang) + ": " + form.Name + "<br/>";
                    message.Body += GestionWeb.GetWebWord(1976, siteLang) + ": " + form.JobTitle + "<br/>";
                    message.Body += GestionWeb.GetWebWord(71, siteLang) + ": " + form.PhoneNumber + "<br/>";
                    message.Body += GestionWeb.GetWebWord(1136, siteLang) + ": " + form.Mail + "</br>";
                    message.Body += GestionWeb.GetWebWord(70, siteLang) + ": " + form.Country + "<br/>";
                    message.Body += GestionWeb.GetWebWord(74, siteLang) + ": " + form.Comment + "<br/>";
                }
                else
                {
                    message.Body = string.Format(body,
                        form.Name,
                        form.Mail,
                        form.Country,
                        form.Company,
                        form.PhoneNumber,
                        form.Comment,
                        form.JobTitle);
                }

                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    if (WebApplicationParameters.CountryCode == CountryCode.POLAND)
                    {
                        message.From = new MailAddress("tswro-tech@kantarmedia.com");
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = new NetworkCredential("tswro-tech@kantarmedia.com", "wil0XAz3",
                            "smtp.office365.com");
                        smtp.Host = "smtp.office365.com";
                        smtp.Port = 587;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.EnableSsl = true;
                    }
                    smtp.Send(message);
                }

                TempData["success"] = GestionWeb.GetWebWord(1486, siteLang);
            }


            return RedirectToAction("Index", new {siteLanguage = siteLang});
        }

        private PresentationModel LoadPresentationBar(int siteLanguage, bool showCurrentSelection = true)
        {
            PresentationModel result = new PresentationModel
            {
                ModuleDescription = GestionWeb.GetWebWord(LanguageConstantes.ContactDescriptionCode, siteLanguage),
                ModuleTitle = GestionWeb.GetWebWord(LanguageConstantes.ContactLabelCode, siteLanguage)
            };
            return result;
        }
    }
}