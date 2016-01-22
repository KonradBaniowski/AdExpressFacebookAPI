using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using PDFCreatorPilotLib;
using TNS.AdExpress.Anubis.Amon.Common;
using TNS.AdExpress.Anubis.Amon.Exceptions;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.UI;
using TNS.AdExpressI.Rolex;
using TNS.Ares.Pdf;
using TNS.Ares.Pdf.Exceptions;
using TNS.FrameWork;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.WebTheme;

namespace TNS.AdExpress.Anubis.Amon.BusinessFacade
{
    /// <summary>
    /// Generate the PDF document for Rolex schedule.
    /// </summary>
    public class AmonPdfSystem : Pdf
    {
        #region Variables
        protected IDataSource _dataSource = null;
        /// <summary>
        /// Appm Configuration (usefull for PDF layout)
        /// </summary>
        protected AmonConfig _config = null;
        /// <summary>
        /// Customer Client request
        /// </summary>
        protected DataRow _rqDetails = null;
        /// <summary>
        /// WebSession to process
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Impersonate Information
        /// </summary>
        private ImpersonateInformation _impersonateInformation = null;
        /// <summary>
        /// Impersonation
        /// </summary>
        private Impersonation _oImp = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AmonPdfSystem(IDataSource dataSource, AmonConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            :
        base(theme.GetStyle("Amon"))
        {
            try
            {
                this._dataSource = dataSource;
                this._config = config;
                this._rqDetails = rqDetails;
                this._webSession = webSession;
            }
            catch (Exception e)
            {
                throw new AmonPdfException("Error in Constructor Amon Pdf System", e);
            }

        }
        #endregion

        #region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <returns>short File Name</returns>
        internal string Init()
        {
            try
            {
                if (_config.UseImpersonate)
                {
                    _impersonateInformation = _config.ImpersonateConfig;
                    OpenImpersonation();
                }

                string shortFName = "";
                string fName = GetFileName(_rqDetails, ref shortFName);
                bool display = false;
#if(DEBUG)
                display = true;
#endif
                Init(true, fName, _config.PdfCreatorPilotLogin, _config.PdfCreatorPilotPass);
                this.DocumentInfo_Creator = this.DocumentInfo_Author = _config.PdfAuthor;
                this.DocumentInfo_Subject = _config.PdfSubject;
                this.DocumentInfo_Title = GestionWeb.GetWebWord(751, _webSession.SiteLanguage);
                this.DocumentInfo_Producer = _config.PdfProducer;
                this.DocumentInfo_Keywords = _config.PdfKeyWords;
                return shortFName;
            }
            catch (System.Exception e)
            {
                throw new AmonPdfException("Error to initialize AmonPdfSystem in Init()", e);
            }
            finally
            {
                if (_config.UseImpersonate && _impersonateInformation != null) CloseImpersonation();
            }
        }
        #endregion

        #region Fill
        internal void Fill()
        {

            try
            {
              
                #region MainPage
                MainPageDesign();
                #endregion

                #region SessionParameter
                SessionParameter();
                #endregion

                #region Rolex Result
                RolexSchedulResult(HtmlSplitting, Convertion.ToHtmlString);
                #endregion

                #region Header and Footer
                string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

                this.AddHeadersAndFooters(
                _webSession,
                imagePosition.leftImage,
                GestionWeb.GetWebWord(2975, _webSession.SiteLanguage) + " - " + dateString,
                0, -1, true);
                #endregion

            }
            catch (Exception e)
            {
                throw new AmonPdfException("Error to Fill Pdf in Fill()" + e.StackTrace + e.Source, e);
            }
        }



        #endregion

        #region RolexSchedulResult
        /// <summary>
        /// Apeend Rolex Schedul Result
        /// </summary>
        private void RolexSchedulResult(Action<StringBuilder, string, int, int> htmlSplitting, Func<string, string> convertToHtml)
        {
            var html = new StringBuilder(10000);
            try
            {
                Domain.Web.Navigation.Module _module = ModulesList.GetModule(Constantes.Web.Module.Name.ROLEX);
                object[] param = new object[1] { _webSession };
                var rolexResult =
                    (IRolexResults)
                    AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                                                                        + @"Bin\" +
                                                                        _module.CountryRulesLayer.AssemblyName,
                                                                        _module.CountryRulesLayer.Class, false,
                                                                        BindingFlags.CreateInstance
                                                                        | BindingFlags.Instance | BindingFlags.Public,
                                                                        null, param, null, null);
                rolexResult.ResultControlId = "RolexPdfSchedulResult";
                string[] result = rolexResult.GetPDFHtml();

                if (result != null && result.Length > 0)
                {
                 
                    var periodBegin = new DateTime(int.Parse(rolexResult.PeriodBeginningDate.Substring(0, 4)),
                        int.Parse(rolexResult.PeriodBeginningDate.Substring(4, 2)), int.Parse(rolexResult.PeriodBeginningDate.Substring(6, 2)));
                    var periodEnd = new DateTime(int.Parse(rolexResult.PeriodEndDate.Substring(0, 4)),
                        int.Parse(rolexResult.PeriodEndDate.Substring(4, 2)), int.Parse(rolexResult.PeriodEndDate.Substring(6, 2)));

                    var timeSpan = periodEnd.Subtract(periodBegin);
                    var nbToSplit = (Int64)Math.Round((double)timeSpan.Days / 7);

                    int nbLines = (nbToSplit > 50) ? 40 : 32;

                    htmlSplitting(html, convertToHtml(result[0]), nbLines, 2);
                    html.Append("\r\n\t\t</td>\r\n\t</tr>");
                    html.Append("</table>");

                    if (result.Length > 1)
                    {
                        html = new StringBuilder(10000);
                        htmlSplitting(html, convertToHtml(result[1]), nbLines, 2);
                        html.Append("\r\n\t\t</td>\r\n\t</tr>");
                        html.Append("</table>");
                    }
                }

            }
            catch (Exception err)
            {
                throw (new AmonPdfException("Unable to process Rolex Pdf Schedule result for request " + _rqDetails["id_static_nav_session"].ToString() + ".", err));
            }
        }
        #endregion

        #region HtmlSplitting
        /// <summary>
        /// Html Splitting
        /// </summary>
        /// <param name="html">html builder</param>
        /// <param name="strHtml">html string</param>
        /// <param name="nbLines">nb lines</param>
        protected virtual void HtmlSplitting(StringBuilder html, string strHtml, int nbLines, int nbLineHeader)
        {
            int startIndex = 0, oldStartIndex = 0;
            var partieHTML = new ArrayList();
            var resultTableHeader = string.Empty;
            if (strHtml.Length > 0)
            {
                for (int i = 0; i < nbLineHeader; i++)
                    startIndex = strHtml.IndexOf("</tr>", startIndex + 1);
                resultTableHeader = strHtml.Substring(0, startIndex);
            }
            double coef = 0;
            int start = 0;
            startIndex = 0;

            while ((startIndex < strHtml.Length) && (startIndex != -1))
            {
                int tmp = 0;

                if (start == 0)
                {
                    while ((tmp < nbLines + 4) && (startIndex < strHtml.Length) && (startIndex != -1))
                    {
                        startIndex = strHtml.IndexOf("<tr>", startIndex + 1);
                        tmp++;
                    }
                    start = 1;
                }
                else
                {
                    while ((tmp < nbLines) && (startIndex < strHtml.Length) && (startIndex != -1))
                    {
                        startIndex = strHtml.IndexOf("<tr>", startIndex + 1);
                        tmp++;
                    }
                }
                if (startIndex == -1)
                    partieHTML.Add(strHtml.Substring(oldStartIndex, strHtml.Length - oldStartIndex));
                else
                    partieHTML.Add(strHtml.Substring(oldStartIndex, startIndex - oldStartIndex));
                oldStartIndex = startIndex;
            }

            for (int i = 0; i < partieHTML.Count; i++)
            {
                if (i > 0) html.AppendFormat("{0}</tr>", resultTableHeader);
                html.Append(partieHTML[i].ToString());
                if (i < partieHTML.Count - 1) html.Append("</table>");
                SnapShots(html.ToString(), i, partieHTML.Count, ref coef);
                html = new StringBuilder();
            }
        }
        #endregion

        #region GetFileName
        /// <summary>
        /// Generate a valid file name from customer request
        /// </summary>
        /// <param name="rqDetails">Details of the customer request</param>
        /// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
        /// <returns>Complet File Name String (path + short name)</returns>
        private string GetFileName(DataRow rqDetails, ref string shortName)
        {
            try
            {

                string pdfFileName = this._config.PdfPath;
                pdfFileName += @"\" + rqDetails["ID_LOGIN"].ToString();

                if (!Directory.Exists(pdfFileName))
                {
                    Directory.CreateDirectory(pdfFileName);
                }
                shortName = DateTime.Now.ToString("yyyyMMdd_")
                    + rqDetails["id_static_nav_session"].ToString()
                    + "_"
                    + TNS.Ares.Functions.GetRandomString(30, 40);

                pdfFileName += @"\" + shortName + ".pdf";

                string checkPath = Regex.Replace(pdfFileName, @"(\.pdf)+", ".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);


                int i = 0;
                while (File.Exists(checkPath))
                {
                    if (i <= 1)
                    {
                        checkPath = Regex.Replace(pdfFileName, @"(\.pdf)+", "_" + (i + 1) + ".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    }
                    else
                    {
                        checkPath = Regex.Replace(pdfFileName, "(_" + i + @"\.pdf)+", "_" + (i + 1) + ".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    }
                    i++;
                }
                return checkPath;
            }
            catch (Exception e)
            {
                throw (new AmonPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
        }
        #endregion

        #region MainPage
        /// <summary>
        /// Design Main Page
        /// </summary>
        /// <returns></returns>
        private void MainPageDesign()
        {

            this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;
            Picture picture = ((Picture)Style.GetTag("pictureTitle"));
            string imgPath = string.Empty;

            if (File.Exists(picture.Path))
            {
                imgPath = picture.Path;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path))
            {
                imgPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path;
            }
            else
            {
                imgPath = picture.Path;
            }

            Image imgG = Image.FromFile(imgPath);

            double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
            double coef = Math.Min((double)1.0, w);
            w = (double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height;
            coef = Math.Min((double)coef, w);

            int imgI = this.AddImageFromFilename(imgPath, TxImageCompressionType.itcFlate);
            this.PDFPAGE_ShowImage(imgI,
                (double)(this.PDFPAGE_Width / 2 - coef * imgG.Width / 2), (double)(this.WorkZoneBottom - coef * imgG.Height - 100),
                (double)(coef * imgG.Width), (double)(coef * imgG.Height), 0);

            Style.GetTag("bigTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
            string str = "";

            this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(GestionWeb.GetWebWord(2975, _webSession.SiteLanguage))) / 2,
                    (this.PDFPAGE_Height) / 4, 0, GestionWeb.GetWebWord(2975, _webSession.SiteLanguage));

            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Web.Functions.Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, Constantes.FrameWork.Dates.Pattern.customDatePattern);

            Style.GetTag("createdTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
            this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str)) / 2,
                1 * this.PDFPAGE_Height / 3, 0, str);

        }
        #endregion

        #region SnapShots
        /// <summary>
        /// Création et Insertion d'une image dans une nouvelle page du PDF
        /// </summary>
        /// <param name="html">Le code HTML</param>
        /// <param name="i">Index d'une partie de code</param>
        /// <param name="partieHTML">Une partie du code HTML</param>
        private void SnapShots(string html, int currentIndexPart, int nbpart, ref double coef)
        {

            string filePath = "";

            #region Création et Insertion d'une image dans une nouvelle page du PDF

            this.NewPage();
            this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

            byte[] data = this.ConvertHtmlToSnapJpgByte(html,
            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
            _config.WebServer);

            filePath = Path.GetTempFileName();
            FileStream fs = File.OpenWrite(filePath);
            var br = new BinaryWriter(fs);
            if (data != null)
                br.Write(data);
            br.Close();
            fs.Close();

            Image imgG;
            int imgI = 0;
            double X1 = 10;

            imgG = Image.FromFile(filePath);
            imgI = this.AddImageFromFilename(filePath, TxImageCompressionType.itcFlate);

            double w = 0;
            if ((currentIndexPart < (nbpart - 1)) || (nbpart == 1))
            {
                w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
                coef = Math.Min((double)1.0, w);
                w = ((double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height);
                coef = Math.Min((double)coef, w);
            }


            this.PDFPAGE_ShowImage(imgI,
                X1, 50,
                coef * imgG.Width,
                coef * imgG.Height,
                0);
          
            imgG.Dispose();
            imgG = null;

            #region Clean File
            File.Delete(filePath);
            #endregion

            #endregion
        }

        #endregion

        #region SessionParameter
        /// <summary>
        /// Session Parameter
        /// </summary>
        private void SessionParameter()
        {


#if DEBUG
            var dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.rolex);
#else
             var dataSource = _webSession.Source;
#endif
            var html = new StringBuilder();

            html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");

            #region Title
            html.Append("<TR height=\"25\">");// html.Append("<TR height=\"25\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"14\">");
            html.Append("<TD class=\"mi\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752, _webSession.SiteLanguage)) + "</TD>");
            html.Append("</TR>");
            #endregion

            #region Period
            html.Append("<TR height=\"7\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"1\" class=\"lightPurple\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1755, _webSession.SiteLanguage)) + " :</TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"20\">");
            html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                + HtmlFunctions.GetPeriodDetail(_webSession)
                + "</TD>");
            html.Append("</TR>");
            #endregion

            #region Detail levels
            if (_webSession.GenericMediaDetailLevel != null && _webSession.GenericMediaDetailLevel.Levels.Count > 0)
            {
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2871, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"20\">");
                html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;");
                html.Append(Convertion.ToHtmlString(_webSession.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage)));
                html.Append("</TD>");
                html.Append("</TR>");
            }
            #endregion

            #region Sites
            bool withSites = false;
            if (_webSession.SelectionUniversMedia != null && _webSession.SelectionUniversMedia.Nodes.Count > 0)
            {
                List<long> sites =
                  new List<string>(
                      _webSession.GetSelection(_webSession.SelectionUniversMedia,
                                               Constantes.Customer.Right.type.siteAccess).Split(',')).ConvertAll(
                                                   Convert.ToInt64);
                withSites = true;



                string temp = ExcelWebPage.AppendSelectedItems(_webSession, 2977, sites,
                                                               DetailLevelItemInformation.Levels.site,
                                                               ExcelWebPage.GetClassificationLevelListDAL);
                //double width = 863;
                //double w = 1;
                if (sites.Count > 30)
                {
                    this.ConvertHtmlToPDF(html.ToString(),
                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                _config.WebServer,
                _config.Html2PdfLogin,
                _config.Html2PdfPass);

                    html = new StringBuilder();
                    int nbLines = 40;
                    temp = string.Format("{0}{1}{2}", "<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">", temp, "</TABLE>");

                    html.Append(temp);
                    this.ConvertHtmlToPDF(temp,
                 WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                 WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                 _config.WebServer,
                 _config.Html2PdfLogin,
                 _config.Html2PdfPass);
                }
                else
                {
                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append(temp);
                    this.ConvertHtmlToPDF(html.ToString(),
                 WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                 WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                 _config.WebServer,
                 _config.Html2PdfLogin,
                 _config.Html2PdfPass);
                }


            }
            #endregion

            #region Locations

            bool withLocations = false;
            if (_webSession.SelectedLocations != null && _webSession.SelectedLocations.Count > 0)
            {
                withLocations = true;
                if (withSites)
                {
                    html = new StringBuilder();
                    html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
                }
                else
                {
                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                }

                html.Append(ExcelWebPage.AppendSelectedItems(_webSession, 1439, _webSession.SelectedLocations, DetailLevelItemInformation.Levels.location,
                    ExcelWebPage.GetClassificationLevelListDAL));

            }
            #endregion

            #region Presence Types
            if (_webSession.SelectedPresenceTypes != null && _webSession.SelectedPresenceTypes.Count > 0)
            {
                if (withSites && !withLocations)
                {
                    html = new StringBuilder();
                    html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
                }
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append(ExcelWebPage.AppendSelectedItems(_webSession, 2978, _webSession.SelectedPresenceTypes, DetailLevelItemInformation.Levels.presenceType,
                    ExcelWebPage.GetClassificationLevelListDAL));

            }
            #endregion



            html.Append("</TABLE>");

            this.ConvertHtmlToPDF(html.ToString(),
                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                _config.WebServer,
                _config.Html2PdfLogin,
                _config.Html2PdfPass);

        }
        #endregion

        #region Send
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="fileName">file Name</param>
        internal void Send(string fileName)
        {

            try
            {
                var to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient)
                {
                    to.Add(s);
                }
                var mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GestionWeb.GetWebWord(2986, _webSession.SiteLanguage),
                    GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer)
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                mail.SubjectEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.BodyEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.mailKoHandler += mail_mailKoHandler;
                mail.SendWithoutThread(false);
            }
            catch (Exception e)
            {
                throw new AmonPdfException("Error to Send mail to client in Send(string fileName)", e);
            }

        }
        #endregion

        #region Evenement Envoi mail client
        /// <summary>
        /// Rise exception when the customer mail has not been sent
        /// </summary>
        /// <param name="source">Error source></param>
        /// <param name="message">Error message</param>
        private void mail_mailKoHandler(object source, string message)
        {
            throw new AmonPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
        }
        #endregion

        #region Impersonation Methods
        /// <summary>
        /// Open Impersonation
        /// </summary>
        /// <returns></returns>
        public void OpenImpersonation()
        {

            if (_impersonateInformation != null)
            {
                CloseImpersonation();
                _oImp = new Impersonation();
                _oImp.ImpersonateValidUser(_impersonateInformation.UserName, _impersonateInformation.Domain, _impersonateInformation.Password, Impersonation.LogonType.LOGON32_LOGON_NEW_CREDENTIALS);
            }
        }
        /// <summary>
        /// Close Impersonation
        /// </summary>
        public void CloseImpersonation()
        {
            if (_oImp != null)
                _oImp.UndoImpersonation();
            _oImp = null;
        }
        #endregion

        #region Init
        /// <summary>
        /// Initialize the PDF (Create it and get it ready for building process)
        /// </summary>
        public override void Init(bool postDisplay, string fileName, string pdfCreatorPilotMail, string pdfCreatorPilotPass)
        {
            try
            {
                this.StartEngine(pdfCreatorPilotMail, pdfCreatorPilotPass);
                this.FileName = Path.GetTempPath() + "\\" + Path.GetFileName(fileName);
                this.AutoLaunch = postDisplay;
                this.Resolution = 96;

                this.BeginDoc();

            }
            catch (System.Exception e)
            {
                throw (new PdfException("Unable to initialize pdf building", e));
            }
        }
        #endregion

    }
}
