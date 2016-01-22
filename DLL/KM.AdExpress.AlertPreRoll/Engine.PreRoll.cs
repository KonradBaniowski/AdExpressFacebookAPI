using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using KM.AdExpress.AlertPreRoll.DAL;
using KM.AdExpress.AlertPreRoll.Tools;
using LinkSystem.LinkKernel.Core;
using LinkSystem.LinkKernel.CoreMonitor;
using LinkSystem.LinkKernel.Enums;
using LinkSystem.LinkMonitor;

namespace KM.AdExpress.AlertPreRoll
{
    public partial class Engine
    {
        private void DoPreRoll(TaskExecution taskExecution, CancellationToken cancellationToken,
                                         ManualResetEventSlim pausEvent, string userToken)
        {
            var taskLogHeader = string.Format("The DoPreRoll task '{0}' t=[{1}]", taskExecution.Name, userToken);

            AdvancedTrace.TraceInformation(string.Format("{0} is running ...", taskLogHeader), ModuleName);
            try
            {

                var xTask = XElement.Parse(taskExecution.XMLTask);

                var dal = new PreRollDAL(_connectionString, _providerDataAccess);
                using (var db = dal.PreRollDb.CreateDbManager())
                {

                    if (xTask.Attributes().Any(p => p.Name == "day"))
                    {
                        var date = DateTimeHelpers.YYYYMMDDToDateTime((string)xTask.Attribute("day"));
                        SendPreRollFile(dal, db, date);
                    }
                    else
                    {
                        if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
                        {
                            //Send saturday and sunday file after codification              
                            if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                                SendPreRollFile(dal, db, DateTime.Now.AddDays(-3));

                            SendPreRollFile(dal, db, DateTime.Now.AddDays(-2));
                        }
                    }


                    dal = null;
                }


                ReleaseTask(taskExecution);
            }
            catch (Exception exception)
            {
                ReleaseTask(taskExecution,
                          new LogLine("An error occured during Pre Roll treatment.", exception,
                                      eLogCategories.Fatal));

            }
            finally
            {
                AdvancedTrace.TraceInformation(string.Format("{0} is done.", taskLogHeader),
                                             ModuleName);
            }
        }

        private void SendPreRollFile(PreRollDAL dal, PreRollDb db, DateTime date)
        {
            var data = dal.Select(db, date, date);

            if (data.Any())
            {
                var xPrerolls = new XElement("PrerollItems", data.Select(p => new XElement("PrerollItem",
                                                                                           new XAttribute("IdProduit", p.IdProduct),
                                                                                           new XAttribute("Produit", p.Product),
                                                                                           new XAttribute("IdAnnonceur", p.IdAdvertiser),
                                                                                           new XAttribute("Annonceur", p.Advertiser),
                                                                                           new XAttribute("IdFamille", p.IdSector),
                                                                                           new XAttribute("Famille", p.Sector),
                                                                                           new XAttribute("IdClasse", p.IdSubSector),
                                                                                           new XAttribute("Classe", p.SubSector),
                                                                                           new XAttribute("IdGroupe", p.IdGroup),
                                                                                           new XAttribute("Groupe", p.Group_),
                                                                                           new XAttribute("IdVariete", p.IdSegment),
                                                                                           new XAttribute("Variete", p.Segment),
                                                                                           new XAttribute("IdCategorie", p.IdCategory),
                                                                                           new XAttribute("Categorie", p.Category),
                                                                                           new XAttribute("IdSupport", p.IdMedia),
                                                                                           new XAttribute("Support", p.Media),
                                                                                           new XAttribute("Version", p.Version),
                                                                                           new XAttribute("Date", p.DateMediaNum),
                                                                                           new XAttribute("NombreInsertions", p.Occurence),
                                                                                           new XAttribute("Url", string.Format("http://www.tnsadexpress.com/adnettrackCreatives/FR/Banners/{0}", p.Url)))));

                string filename = string.Format("Evaliant_Prerolls_{0}.xml", date.ToString("yyyyMMdd"));
                string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
                xPrerolls.Save(file);


                // Get the object used to communicate with the server.
                var request = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/TF1_evaliant/{1}", _ftpServer, filename));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // Credentials
                request.Credentials = new NetworkCredential(_ftpUserName, _ftpPassword);

                // Copy the contents of the file to the request stream.
                var sourceStream = new StreamReader(file);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                var response = (FtpWebResponse)request.GetResponse();
                AdvancedTrace.TraceInformation(string.Format("Upload File {0} Complete, status {1}", filename, response.StatusDescription), ModuleName);
                response.Close();

                //Send mail
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_customerMailServer);

                mail.From = new MailAddress(_customerMailFrom);
                foreach (string recipient in _customerMailRecipients)
                {
                    mail.To.Add(recipient);
                }

                mail.Subject = string.Format("Alerte Evaliant Pré-Rolls du {0}", date.ToString("dd/MM/yyyy"));
                ReportGenerator reportGenerator = new ReportGenerator(date, filename);
                mail.Body = reportGenerator.GetHtml();
                mail.IsBodyHtml = true;
                SmtpServer.Port = _customerMailPort;
                SmtpServer.Send(mail);

                File.Delete(file);
            }
        }
    }
}
