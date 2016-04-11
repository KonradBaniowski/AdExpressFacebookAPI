using Km.AdExpressClientWeb.I18n;
using Km.AdExpressClientWeb.Models.Contact;
using Km.AdExpressClientWeb.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;

namespace Km.AdExpressClientWeb.Controllers
{
    public class ContactController : Controller
    {
        public ActionResult Index()
        {
            var model = new ContactViewModel();
            model.QuestionsTagItem = new List<SelectListItem>();
            var labels = LabelsHelper.LoadPageLabels(33);
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag1, Value = labels.QuestionTag1 });
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag2, Value = labels.QuestionTag2 });
            model.QuestionsTagItem.Add(new SelectListItem { Text = labels.QuestionTag3, Value = labels.QuestionTag3 });
            model.Labels = LabelsHelper.LoadPageLabels(33);
            model.Labels.CurrentController = "Contact";
            model.PresentationModel = LoadPresentationBar(33, false);
            return View(model);
        }

        public ActionResult ContactUs(ContactViewModel form)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1}-{6})</p><p>Infos:</p><p>{2} {3} {4}</p><p>Message:</p><p>{5}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("sc.adexpress@kantarmedia.com"));
                message.To.Add(new MailAddress("romain.vacquerie@kantarmedia.com"));
                message.Subject = form.QuestionTag;
                message.Body = string.Format(body, 
                    form.Name,
                    form.Mail,
                    form.Country,
                    form.Company,
                    form.PhoneNumber,
                    form.Comment,
                    form.JobTitle);
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Send(message);
                  
                }
            }
            return RedirectToAction("Index", "Home");
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