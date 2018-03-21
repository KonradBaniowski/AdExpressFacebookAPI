#region Information
//Author : Y. Rkaina
//Creation : 10/08/2006
#endregion

#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Anubis.Miysis.Exceptions;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Insertions.Cells;
using TNS.Ares.Pdf;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.CampaignTypes;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Helper.MediaPlanVersions;
using TNS.AdExpress.Web.Helper;
#endregion

namespace TNS.AdExpress.Anubis.Miysis.BusinessFacade
{
    /// <summary>
    /// Generate the PDF document for MiysisPdfSystem module.
    /// </summary>
    public class MiysisPdfSystem : Pdf
    {

        #region Variables
        protected IDataSource _dataSource = null;
        /// <summary>
        /// Appm Configuration (usefull for PDF layout)
        /// </summary>
        protected MiysisConfig _config = null;
        /// <summary>
        /// Customer Client request
        /// </summary>
        protected DataRow _rqDetails = null;
        /// <summary>
        /// WebSession to process
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Position des visuels dans le pdf dans le cas de 4 visuels par page
        /// </summary>
        private enum imgPositionItems
        {
            /// <summary>
            /// Position : Haut, gauche
            /// </summary>
            TOP_LEFT = 0,
            /// <summary>
            /// Position : Haut, droite
            /// </summary>
            TOP_RIGHT = 1,
            /// <summary>
            /// Position : Bas, gauche
            /// </summary>
            BOTTOM_LEFT = 2,
            /// <summary>
            /// Position : Bas, droite
            /// </summary>
            BOTTOM_RIGHT = 3
        };
        /// <summary>
        /// Position des visuels dans le pdf
        /// </summary>
        private enum imgPosition
        {
            /// <summary>
            /// Position : Haut
            /// </summary>
            TOP = 0,
            /// <summary>
            /// Position : Bas
            /// </summary>
            BOTTOM = 1,
            /// <summary>
            /// Dépend du traitement
            /// </summary>
            DYNAMIC = -1
        };
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public MiysisPdfSystem(IDataSource dataSource, MiysisConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            :
        base(theme.GetStyle("Miysis"))
        {
            try
            {
                _dataSource = dataSource;
                _config = config;
                _rqDetails = rqDetails;
                _webSession = webSession;               
            }
            catch (Exception e)
            {
                throw new MiysisPdfException("Error in Constructor MiysisPdfSystem", e);
            }

        }
        #endregion

        #region Init
        /// <summary>
        /// Initialization
        /// </summary>
        /// <returns>short File Name</returns>
        public string Init()
        {
            try
            {
                var shortFName = "";
                var fName = GetFileName(_rqDetails, ref shortFName);

                Init(true, fName, _config.PdfCreatorPilotLogin, _config.PdfCreatorPilotPass);
                DocumentInfo_Creator = DocumentInfo_Author = _config.PdfAuthor;
                DocumentInfo_Subject = _config.PdfSubject;
                DocumentInfo_Title = GetTitle();
                DocumentInfo_Producer = _config.PdfProducer;
                DocumentInfo_Keywords = _config.PdfKeyWords;
                return shortFName;
            }
            catch (Exception e)
            {
                throw new MiysisPdfException("Error to initialize MiysisPdfSystem in Init()", e);
            }
        }
        #endregion

        #region Fill
        /// <summary>
        /// Fill
        /// </summary>
        public virtual void Fill()
        {

            try
            {

                #region MainPage
                MainPageDesign();
                #endregion

                #region SessionParameter
                SessionParameter();
                #endregion

                #region Impression
                MediaPlanImpression();
                #endregion

                #region Header and Footer
                string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, Constantes.FrameWork.Dates.Pattern.customDatePattern);

                AddHeadersAndFooters(
                _webSession,
                imagePosition.leftImage,
                GetTitle() + " - " + dateString,
                0, -1, true);
                #endregion

            }
            catch (System.Exception e)
            {
                throw new MiysisPdfException("Error to Fill Pdf in Fill()" + e.Message + " : "+ e.StackTrace +":"+e.Source, e);
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="fileName">file Name</param>
        public virtual void Send(string fileName)
        {           

            try
            {
                ArrayList to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient)
                {
                    to.Add(s);
                }
                SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GetMailContent(),
                    GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + " \"" + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(3066, _webSession.SiteLanguage), _config.WebServer + "/AdExCustomerFiles/" + _webSession.CustomerLogin.IdLogin + "/" + fileName + ".pdf")
                    + "<br><br>"
                    + String.Format(GestionWeb.GetWebWord(1776, _webSession.SiteLanguage), _config.WebServer),
                    //+ String.Format(GestionWeb.GetWebWord(1776, _webSession.SiteLanguage), "http://adexpress.kantarmedia.ie"),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                try
                {
                    mail.SubjectEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                    mail.BodyEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                    string charset = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
                    if (!string.IsNullOrEmpty(charset))
                    {
                        mail.CharsetTextHtml = charset;
                        mail.CharsetTextPlain = charset;
                    }
                }
                catch (Exception) { }
                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);
            }
            catch (System.Exception e)
            {
                throw new MiysisPdfException("Error to Send mail to client in Send(string fileName)", e);
            }

        }

        public virtual void SendOffice365(string fileName)
        {
            MailMessage msg = new MailMessage();

            foreach (string s in _webSession.EmailRecipient)
            {
                msg.To.Add("youssef.rkaina@kantarmedia.com");
            }

            msg.From = new MailAddress("tswro-tech@kantarmedia.com");
            msg.Subject = GetMailContent();
            msg.Body = GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + " \"" + _webSession.ExportedPDFFileName
                       + "\"" +
                       String.Format(GestionWeb.GetWebWord(3066, _webSession.SiteLanguage),
                           _config.WebServer + "/AdExCustomerFiles/" + _webSession.CustomerLogin.IdLogin + "/" +
                           fileName + ".pdf")
                       + "<br><br>"
                       + String.Format(GestionWeb.GetWebWord(1776, _webSession.SiteLanguage), _config.WebServer);
            msg.Priority = MailPriority.High;
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("tswro-tech@kantarmedia.com", "wil0XAz3", "smtp.office365.com");
            client.Host = "smtp.office365.com";
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            client.Send(msg);
        }

        #endregion

        #region Méthode Interne

        #region File IO management

        #region GetFileName
        /// <summary>
        /// Generate a valid file name from customer request
        /// </summary>
        /// <param name="rqDetails">Details of the customer request</param>
        /// <param name="shortName">Return value : short name of the File (the method return the complet path)</param>
        /// <returns>Complet File Name String (path + short name)</returns>
        private string GetFileName(DataRow rqDetails, ref string shortName)
        {
            string pdfFileName;

            try
            {

                pdfFileName = this._config.PdfPath;
                pdfFileName += @"\" + rqDetails["ID_LOGIN"].ToString();

                if (!Directory.Exists(pdfFileName))
                {
                    Directory.CreateDirectory(pdfFileName);
                }
                shortName = DateTime.Now.ToString("yyyyMMdd_")
                    + rqDetails["id_static_nav_session"].ToString()
                    + "_"
                    + TNS.Ares.Functions.GetRandomMailString(30, 40);

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
            catch (System.Exception e)
            {
                throw (new MiysisPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
        }
        #endregion

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

            Style.GetTag("bigTitle").SetStylePdf(this, GetTxFontCharset());
            string str = "";

            this.PDFPAGE_UnicodeTextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(GetTitle())) / 2,
                    (this.PDFPAGE_Height) / 4, 0, GetTitle());

        
            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

            Style.GetTag("createdTitle").SetStylePdf(this, GetTxFontCharset());
            this.PDFPAGE_UnicodeTextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str)) / 2,
                1 * this.PDFPAGE_Height / 3, 0, str);

        }
        #endregion

        #region SessionParameter
        /// <summary>
        /// Session Parameter
        /// </summary>
        protected  virtual void SessionParameter()
        {

            //Formatting date to be used in the query
            var dateBegin = Web.Core.Utilities.Dates.GetPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd");
            var dateEnd = Web.Core.Utilities.Dates.GetPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd");

            var html = new StringBuilder();

            html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");

            #region Title
            html.Append("<TR height=\"25\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"14\">");
            html.Append("<TD class=\"mi\">" + ConvertionToHtmlString(GestionWeb.GetWebWord(1752, _webSession.SiteLanguage)) + "</TD>");
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
            html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(1755, _webSession.SiteLanguage)) + " :</TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"20\">");
            html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                + HtmlFunctions.GetPeriodDetail(_webSession)
                + "</TD>");
            html.Append("</TR>");
          // Période comparative
          if (_webSession.ComparativeStudy && TNS.AdExpress.Domain.Web.WebApplicationParameters.UseComparativeMediaSchedule) {
            html.Append(GetComparativePeriodDetail(_webSession, TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA, false));
          }
         // Type Sélection comparative
         if (_webSession.ComparativeStudy && TNS.AdExpress.Domain.Web.WebApplicationParameters.UseComparativeMediaSchedule) {
            html.Append(GetComparativePeriodTypeDetail(_webSession, TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA));
         }
            #endregion

            #region Média
            //Média
            html.Append("<TR height=\"7\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"1\" class=\"lightPurple\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(1292, _webSession.SiteLanguage)) + " :</TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD align=\"left\">");
            html.Append(DisplayTreeNode.ToHtml(_webSession.SelectionUniversMedia, false, false, false, 600, false, false, _webSession.SiteLanguage, 2, 1, true, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource));
            html.Append("</TD>");
            html.Append("</TR>");
            #endregion
            
            #region Unité
            //Unité
            html.Append("<TR height=\"7\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(1795, _webSession.SiteLanguage)) + " :</TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"20\">");
            html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                + GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage)
                + "</TD>");
            html.Append("</TR>");
            #endregion            
            
            #region Group format
        if (WebApplicationParameters.VehiclesFormatInformation.Use) {
            if (VehiclesInformation.Contains(Constantes.Classification.DB.Vehicles.names.adnettrack)) {

                var formatIdList = _webSession.GetValidFormatSelectedList(_webSession.GetVehiclesSelected(), true);
                if (formatIdList.Count > 0) {
                    var strFormatList = new List<string>();
                    var vehiclesFormatList =
                        VehiclesFormatList.GetList(
                            new List<VehicleInformation>(_webSession.GetVehiclesSelected().Values).ConvertAll
                                (p => p.DatabaseId));
                    foreach (var cFilterItem in vehiclesFormatList.Values) {
                        if (formatIdList.Contains(cFilterItem.Id)) {
                            strFormatList.Add(cFilterItem.Label);
                        }
                    }

                    html.Append("<TR height=\"7\">");
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
                    html.Append("<TD></TD>");
                    html.Append("</TR>");
                    html.Append("<TR>");
                    html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2928, _webSession.SiteLanguage)) + " :</TD>");
                    html.Append("</TR>");
                    html.Append("<TR height=\"20\">");
                    html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                        + string.Join(", ", strFormatList.ToArray())
                        + "</TD>");
                    html.Append("</TR>");


                }
            }

        }       
        #endregion

            #region produit
            bool first = true;
            const int nbLineByPage = 32;
            int currentLine = 10;
            if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
            {

                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");

                html.Append("<TR align=\"left\">");
                html.Append("<TD>");
                html.Append(ConvertionToHtmlString(DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource, 600, true, nbLineByPage, ref currentLine)));
                html.Append("</TD>");
                html.Append("</TR>");

            }
            #endregion            

            #region Product Detail Level
            if ((_webSession.ProductDetailLevel != null && _webSession.ProductDetailLevel.ListElement != null) || _webSession.SloganIdZoom > -1)
            {
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(518, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");
                if (_webSession.ProductDetailLevel != null && _webSession.ProductDetailLevel.ListElement != null)
                {
                    html.Append("<TR>");
                    html.Append("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;");
                    #region level
                    int code = -1;
                    switch (((LevelInformation)_webSession.ProductDetailLevel.ListElement.Tag).Type)
                    {
                        case CstRights.type.sectorAccess:
                            code = 175;
                            break;
                        case CstRights.type.subSectorAccess:
                            code = 1931;
                            break;
                        case CstRights.type.groupAccess:
                            code = 859;
                            break;
                        case CstRights.type.advertiserAccess:
                            code = 857;
                            break;
                        case CstRights.type.productAccess:
                            code = 858;
                            break;
                        case CstRights.type.brandAccess:
                            code = 1889;
                            break;
                        case CstRights.type.subBrandAccess:
                            code = 2650;
                            break;
                        case CstRights.type.holdingCompanyAccess:
                            code = 1589;
                            break;
                        case CstRights.type.segmentAccess:
                            code = 592;
                            break;

                    }
                    #endregion
                    html.Append(ConvertionToHtmlString(string.Format("{0} \"{1}\"",
                        GestionWeb.GetWebWord(code, _webSession.SiteLanguage),
                        ((LevelInformation)_webSession.ProductDetailLevel.ListElement.Tag).Text
                        )
                    ));
                    html.Append("</TD>");
                    html.Append("</TR>");
                }
                if (_webSession.SloganIdZoom > -1)
                {
                    html.Append("<TR>");
                    html.Append("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;");
                    html.Append(ConvertionToHtmlString(string.Format("{0} \"{1}\"",
                        GestionWeb.GetWebWord(1987, _webSession.SiteLanguage),
                        _webSession.SloganIdZoom
                        )
                    ));
                    html.Append("</TD>");
                    html.Append("</TR>");
                }
            }
            #endregion

            #region Search Legend
            string vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if (vehicleSelection.Length > 0 && VehiclesInformation.Contains(ClassificationCst.DB.Vehicles.names.search)
                            && vehicleSelection.Contains(VehiclesInformation.Get(ClassificationCst.DB.Vehicles.names.search).DatabaseId.ToString())) {
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(3012, _webSession.SiteLanguage)) + "</TD>");
                html.Append("</TR>");
            }
            #endregion

            //tsest

            byte[] data = this.ConvertHtmlToSnapJpgByte(html.ToString(),
            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
            _config.WebServer);

            html.Append("</TABLE>");

            ConvertHtmlToPDF(html.ToString(),
                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                _config.WebServer,
                _config.Html2PdfLogin,
                _config.Html2PdfPass);

            //Add Russian options
            html = new StringBuilder();
           
            #region Campaingn type (RU)
            if (WebApplicationParameters.AllowCampaignTypeOption && CampaignTypesInformation.ContainsKey(_webSession.CampaignType))
            {
                List<CampaignTypeInformation> campaignTypes = _webSession.GetValidCampaignTypeForResult();

                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(2671, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"20\">");
                html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
                    + GestionWeb.GetWebWord(CampaignTypesInformation.Get(_webSession.CampaignType).WebTextId, _webSession.SiteLanguage)
                    + "</TD>");
                html.Append("</TR>");

            }
            #endregion

            first = true; 
            currentLine = 5;

            #region Region Set
            Domain.Web.Navigation.Module currentModule = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
            ArrayList detailSelections = currentModule.GetResultPageInformation((int)_webSession.CurrentTab).DetailSelectionItemsType;
            if (_webSession.PrincipalMediaUniverses != null && _webSession.PrincipalMediaUniverses.Count > 0
                && (detailSelections != null && detailSelections.Contains(DetailSelection.Type.regionSelected.GetHashCode())))
            {

                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(2680, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");

                html.Append("<TR align=\"left\">");
                html.Append("<TD>");
                html.Append(ConvertionToHtmlString(DisplayUniverse.ToHtml(_webSession.PrincipalMediaUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource, 600, true, nbLineByPage, ref currentLine)));
                html.Append("</TD>");
                html.Append("</TR>");

            }
            #endregion

            #region Profession and Name set
            if (_webSession.PrincipalProfessionUniverses != null && _webSession.PrincipalProfessionUniverses.Count > 0)
            {
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + ConvertionToHtmlString(GestionWeb.GetWebWord(2965, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");

                html.Append("<TR align=\"left\">");
                html.Append("<TD>");
                html.Append(ConvertionToHtmlString(DisplayUniverse.ToHtml(_webSession.PrincipalProfessionUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource, 600, true, nbLineByPage, ref currentLine)));
                html.Append("</TD>");
                html.Append("</TR>");
            }
            #endregion

            if (html.Length > 0)
            {               
                html =  new StringBuilder("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">" + html.ToString() + "</TABLE>");
                this.ConvertHtmlToPDF(html.ToString(),
                   WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                   WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                   _config.WebServer,
                   _config.Html2PdfLogin,
                   _config.Html2PdfPass);
            }

           
        }
        #endregion

        #region MediaPlanIpression
        /// <summary>
        /// Media Plan html render
        /// </summary>
        protected virtual void MediaPlanImpression()
        {

            #region GETHTML
            var html = new StringBuilder(10000);
            var module = GetModule();

            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CstRights.type.vehicleAccess).Split(',');
            try
            {

                #region result
                var idVehicle = Int64.Parse(vehicles[0]);

                var begin = Web.Core.Utilities.Dates.GetPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                var end = Web.Core.Utilities.Dates.GetPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);

                MediaSchedulePeriod period;
                if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && _webSession.CurrentModule == Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod, _webSession.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod);

                object[] param;
                if (vehicles.Length == 1) {
                    param = new object[3];
                    param[2] = idVehicle;
                }
                else
                    param = new object[2];

                param[0] = _webSession;
                param[1] = period;

                var mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                mediaScheduleResult.Module = module;
                var result = mediaScheduleResult.GetPDFHtml();
                var creativeCells = new SortedDictionary<Int64, List<CellCreativesInformation>>();

                if (vehicles.Length == 1 && _webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.dayly && result.VersionsDetail.Count > 0)
                {

                    var versionsUI = new VersionsPluriMediaUI(_webSession, period, "");
                    int startIndexVisual = decoupageVersionHTML(html, versionsUI.GetExportMSCreativesHtml(ref creativeCells, base.Style), true, int.Parse(vehicles[0]));
                    var currentVehicle = VehiclesInformation.Get(idVehicle);
                    if (CanShowVisual(currentVehicle.Id ))                       
                        BuildVersionPDF(creativeCells, startIndexVisual);

                }
                #endregion

                #region Construction du tableaux global
                Double w, width;
                var timeSpan = period.End.Subtract(period.Begin);
                var nbToSplit = (Int64)Math.Round((double)timeSpan.Days / 7);

                switch (_webSession.DetailPeriod)
                {
                    case CustomerSessions.Period.DisplayLevel.weekly:
                        nbToSplit = (Int64)Math.Round((double)nbToSplit / 7);
                        break;
                    case CustomerSessions.Period.DisplayLevel.monthly:
                        nbToSplit = (Int64)Math.Round((double)nbToSplit / 30);
                        break;
                }

                int nbLines;
                if (nbToSplit < 5)
                {
                    width = 863;
                    w = 1;
                    nbLines = (int)Math.Round(w * (width / 20)) - 4;
                }
                else
                {
                    w = 0.652;
                    width = 996 + (133 * (nbToSplit - 5));
                    nbLines = (int)Math.Round(w * (width / 20)) - 4;
                }
              
                decoupageHTML(html, ConvertionToHtmlString(result.HTMLCode), nbLines, false);
                html.Append("\r\n\t\t</td>\r\n\t</tr>");
                html.Append("</table>");
                #endregion

                html.Append("</BODY>");
                html.Append("</HTML>");

            }
            catch (Exception err)
            {
                throw (new MiysisPdfException("Unable to process Media Schedule Alert export result for request " + _rqDetails["id_static_nav_session"].ToString() + ".", err));
            }
            finally
            {
                _webSession.CurrentModule = module.Id;
            }
            #endregion

        }
        #endregion

        #region Découpage du code HTML

        #region decoupageHTML
        /// <summary>
        /// Découpage du code HTML pour l'export PDF du plan média (Tableau)
        /// </summary>
        /// <param name="html">Le code HTML à générer</param>
        /// <param name="strHtml">Le code HTML à découper</param>
        /// <param name="nbLines">Nombres de lignes pour le découpage</param>
        /// <param name="version">true if version</param>
        protected  virtual void decoupageHTML(StringBuilder html, string strHtml, int nbLines, bool version)
        {
            int startIndex = 0, oldStartIndex = 0;
            var partieHTML = new ArrayList();
            var resultTableHeader = string.Empty;
            if (strHtml.Length > 0)
            {
                for (int i = 0; i < 3; i++)
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
                if (i > 0) html.Append(resultTableHeader);
                html.Append(partieHTML[i].ToString());
                if (i < partieHTML.Count - 1) html.Append("</table>");
                if (version)
                    SnapShots(html.ToString(), i, partieHTML.Count, true, ref coef);
                else
                    SnapShots(html.ToString(), i, partieHTML.Count, false, ref coef);
                html = new StringBuilder();
            }
        }
        #endregion

        #region decoupageVersionHTML
        /// <summary>
        /// Découpage du code HTML pour l'export PDF du plan média (Visuels)
        /// </summary>
        /// <param name="html">Le code HTML à générer</param>
        /// <param name="strHtml">Le code HTML à découper</param>
        private int decoupageVersionHTML(StringBuilder html, string strHtml, bool version, int vehicle)
        {
            int startIndex = 0, oldStartIndex = 0;
            int startIndexVisual = 0;
            var partieHTML = new ArrayList();
            var htmltmp = new StringBuilder(1000);
            htmltmp.Append(html.ToString());

            while ((startIndex < strHtml.Length) && (startIndex != -1))
            {
                int tmp = 0;

                while ((tmp < 1) && (startIndex < strHtml.Length) && (startIndex != -1))
                {
                    startIndex = strHtml.IndexOf("<br>", startIndex + 1);
                    tmp++;
                }

                if (startIndex == -1)
                    partieHTML.Add(strHtml.Substring(oldStartIndex, strHtml.Length - oldStartIndex));
                else
                    partieHTML.Add(strHtml.Substring(oldStartIndex, startIndex - oldStartIndex));
                oldStartIndex = startIndex;
            }

            for (int i = 0; i < partieHTML.Count - 1; i++) {
                if ((((DBCst.Vehicles.names)vehicle == DBCst.Vehicles.names.radio) || ((DBCst.Vehicles.names)vehicle == DBCst.Vehicles.names.tv)) && (i > 0))
                {
                    htmltmp.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
                    htmltmp.Append("<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 50px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\"></td></tr>");
                }
                htmltmp.Append(partieHTML[i].ToString());
                if (version)
                {
                    if (i == 0)
                    {
                        startIndexVisual = ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                    }
                    else
                    {
                        ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                    }
                }
                else
                {
                    ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                }
                htmltmp = new StringBuilder();
            }
            return startIndexVisual;
        }
        #endregion

        #endregion

        #region Création et Insertion d'une image dans une nouvelle page du PDF
        /// <summary>
        /// Création et Insertion d'une image dans une nouvelle page du PDF
        /// </summary>
        /// <param name="html">Le code HTML</param>
        /// <param name="i">Index d'une partie de code</param>
        /// <param name="partieHTML">Une partie du code HTML</param>
        private void SnapShots(string html, int currentIndexPart, int nbpart, bool version, ref double coef)
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
            BinaryWriter br = new BinaryWriter(fs);
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

            if (version)
            {
                if (currentIndexPart == (nbpart - 1))
                {
                    this.PDFPAGE_ShowImage(imgI,
                        X1, 64,
                        coef * imgG.Width,
                        coef * imgG.Height,
                        0);
                }
                else
                {
                    this.PDFPAGE_ShowImage(imgI,
                        X1, this.PDFPAGE_Height / 2 - (coef * imgG.Height / 2),
                        coef * imgG.Width,
                        coef * imgG.Height,
                        0);
                }
            }
            else
            {
                if (currentIndexPart == (nbpart - 1))
                {
                    this.PDFPAGE_ShowImage(imgI,
                        X1, 40,
                        coef * imgG.Width,
                        coef * imgG.Height,
                        0);
                }
                else
                {
                    this.PDFPAGE_ShowImage(imgI,
                        X1, this.PDFPAGE_Height / 2 - (coef * imgG.Height / 2),
                        coef * imgG.Width,
                        coef * imgG.Height,
                        0);
                }
            }

            imgG.Dispose();
            imgG = null;

            #region Clean File
            File.Delete(filePath);
            #endregion

            #endregion
        }

        #endregion

        #region BuildVersionPDF(String title)
        /// <summary> 
        /// Render all versions controls
        /// </summary>
        /// <returns>Html code</returns>
        protected void BuildVersionPDF(SortedDictionary<Int64, List<CellCreativesInformation>> creativeCells, int startIndex)
        {

            ArrayList partitHTMLVersion = new ArrayList();
            StringBuilder htmltmp = new StringBuilder(1000);
            int end = 0;
            double indexPage = 0;
            int indexFirstCase = 0, indexSecondCase = 0, indexThirdCase = 0, X1 = 0, Y1 = 0, nbLines = 0;
            bool first = true, second = true;
            long nbVisuel = 0;

            if (creativeCells != null)
            {

                foreach (List<CellCreativesInformation> currentList in creativeCells.Values)
                {
                    foreach (CellCreativesInformation item in currentList)
                    {

                        nbVisuel = item.Visuals.Count;

                        if (nbVisuel == 1)
                        {

                            if (indexPage == 0)
                            {
                                switch ((imgPositionItems)indexFirstCase)
                                {
                                    case imgPositionItems.TOP_LEFT: X1 = 29; Y1 = 77; break;
                                    case imgPositionItems.TOP_RIGHT: X1 = 557; Y1 = 77; break;
                                    case imgPositionItems.BOTTOM_LEFT: X1 = 29; Y1 = 404; break;
                                    case imgPositionItems.BOTTOM_RIGHT: X1 = 557; Y1 = 404; break;
                                }
                            }
                            else
                            {
                                switch ((imgPositionItems)indexFirstCase)
                                {
                                    case imgPositionItems.TOP_LEFT: X1 = 27; Y1 = 75; break;
                                    case imgPositionItems.TOP_RIGHT: X1 = 554; Y1 = 75; break;
                                    case imgPositionItems.BOTTOM_LEFT: X1 = 27; Y1 = 401; break;
                                    case imgPositionItems.BOTTOM_RIGHT: X1 = 554; Y1 = 401; break;
                                }
                            }
                            indexFirstCase++;
                            InsertImageInPDF(0, item, indexPage, startIndex, X1, Y1);
                            if ((indexFirstCase == 1) || (indexFirstCase == 3))
                            {
                                nbLines++;
                            }
                            if (indexFirstCase == 4)
                            {
                                indexPage++;
                                indexFirstCase = 0;
                            }
                        }
                        else if (nbVisuel < 5)
                        {

                            if (first)
                            {
                                if ((nbLines % 2) == 0)
                                {
                                    indexPage = (nbLines / 2);
                                }
                                else
                                {
                                    indexPage = Math.Ceiling((double)(nbLines / 2));
                                    indexSecondCase--;
                                }
                                first = false;
                            }

                            if (indexPage == 0)
                            {
                                switch ((imgPosition)indexSecondCase)
                                {
                                    case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                    case imgPosition.DYNAMIC:
                                    case imgPosition.BOTTOM: X1 = 25; Y1 = 393; break;
                                }
                            }
                            else
                            {
                                switch ((imgPosition)indexSecondCase)
                                {
                                    case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                    case imgPosition.DYNAMIC:
                                    case imgPosition.BOTTOM: X1 = 25; Y1 = 393; break;
                                }
                            }

                            InsertImageInPDF(0, item, indexPage, startIndex, X1, Y1);
                            if (indexSecondCase == -1)
                                indexPage++;
                            indexSecondCase++;
                            nbLines++;
                            if (indexSecondCase == 2)
                            {
                                indexPage++;
                                indexSecondCase = 0;
                            }
                        }
                        else if (nbVisuel >= 5)
                        {

                            end = (int)Math.Ceiling((double)nbVisuel / 4);

                            for (int i = 0; i < end; i++)
                            {

                                if ((first) || (second))
                                {
                                    if ((nbLines % 2) == 0)
                                    {
                                        indexPage = (nbLines / 2);
                                    }
                                    else
                                    {
                                        indexPage = Math.Ceiling((double)(nbLines / 2));
                                        indexThirdCase--;
                                    }
                                    first = false;
                                    second = false;
                                }

                                if (indexPage == 0)
                                {
                                    switch ((imgPosition)indexThirdCase)
                                    {
                                        case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                        case imgPosition.DYNAMIC:
                                        case imgPosition.BOTTOM: X1 = 25; Y1 = 397; break;
                                    }
                                }
                                else
                                {
                                    switch ((imgPosition)indexThirdCase)
                                    {
                                        case imgPosition.TOP: X1 = 25; Y1 = 70; break;
                                        case imgPosition.DYNAMIC:
                                        case imgPosition.BOTTOM: X1 = 25; Y1 = 394; break;
                                    }
                                }

                                if (i == 0)
                                {
                                    InsertImageInPDF(0, item, indexPage, startIndex, X1, Y1);
                                }
                                else
                                {
                                    if (i == end - 1)
                                    {
                                        InsertImageInPDF(i + (4 * i), item, indexPage, startIndex, X1, Y1);
                                    }
                                    else
                                    {
                                        InsertImageInPDF(i + (4 * i), item, indexPage, startIndex, X1, Y1);
                                    }
                                }

                                if (indexThirdCase == -1)
                                    indexPage++;
                                indexThirdCase++;

                                if (indexThirdCase == 2)
                                {
                                    indexPage++;
                                    indexThirdCase = 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Insertion des images (New version)
        ///<summary>Insertion des images New version</summary>
        ///<param name="output"></param>
        ///<param name="index">l'index est utilsé pour traiter le cas des verions avec plus de 4 visuels</param>
        ///  <author>rkaina</author>
        ///  <since>vendredi 18 août 2006</since>
        protected void InsertImageInPDF(Int64 index, CellCreativesInformation item, double indexPage, int startIndex, int X1, int Y1)
        {
            Int64 lastIndex = index + 5;
            Int64 nbVisuel = 0;

            this.SetCurrentPage((int)indexPage + startIndex);

            nbVisuel = item.Visuals.Count;

            Int64 end = 0;

            if (nbVisuel < lastIndex)
                end = nbVisuel;
            else
                end = lastIndex;

            if ((nbVisuel % 5) == 0)
            {
                if (nbVisuel > lastIndex)
                {
                    end = lastIndex;
                }
                else if (nbVisuel == lastIndex)
                {
                    end = lastIndex - 1;
                }
                else
                {
                    end = nbVisuel;
                    index--;
                }
            }

            Image imgG;
            int imgI = 0;
            string path = "";

            for (Int64 i = index; i < end; i++)
            {
                path = "";
                path = item.Visuals[(int)i].Replace("/imagette", "");
                path = path.Replace("/ImagesPresse", "");
                path = path.Replace("/ImagesMD", "");

                if (item.Vehicle.Id == DBCst.Vehicles.names.directMarketing)
                {
                    imgG = Image.FromFile(_config.VMCScanPath + path);
                    imgI = this.AddImageFromFilename(_config.VMCScanPath + path, TxImageCompressionType.itcFlate);
                }
                else if (item.Vehicle.Id == DBCst.Vehicles.names.outdoor)
                {
                    imgG = Image.FromFile(_config.OutdoorScanPath + path);
                    imgI = this.AddImageFromFilename(_config.OutdoorScanPath + path, TxImageCompressionType.itcFlate);
                }
                else
                {
                    if (WebApplicationParameters.CountryCode == TNS.AdExpress.Constantes.Web.CountryCode.IRELAND) {
                        imgG = Bitmap.FromStream((System.Net.WebRequest.Create(string.Format(path)).GetResponse().GetResponseStream()));
                        imgG.Save(AppDomain.CurrentDomain.BaseDirectory + @"/tmp/" + i + ".jpg");
                        imgI = this.AddImageFromFilename(AppDomain.CurrentDomain.BaseDirectory + @"/tmp/" + i + ".jpg", TxImageCompressionType.itcFlate);
                        File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"/tmp/" + i + ".jpg");
                    }
                    else {
                        imgG = Image.FromFile(_config.PressScanPath + path);
                        imgI = this.AddImageFromFilename(_config.PressScanPath + path, TxImageCompressionType.itcFlate);
                    }
                }

                double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
                double coef = Math.Min((double)1.0, w);
                w = (double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height;
                coef = Math.Min((double)coef, w);

                if ((end - index == 4) && (nbVisuel != 5))
                {
                    this.PDFPAGE_ShowImage(imgI,
                        X1, Y1,
                        192, 296, 0);
                    X1 += 192 + 1;
                }
                else if (end - index == 5)
                {
                    this.PDFPAGE_ShowImage(imgI,
                        X1, Y1,
                        213, 300, 0);
                    X1 += 213 + 1;
                }
                else
                {
                    this.PDFPAGE_ShowImage(imgI,
                        X1, Y1,
                        223, 296, 0);
                    X1 += 223 + 1;
                }
            }
        }
        #endregion

        #region Get Title
        /// <summary>
        /// Get Title
        /// </summary>
        /// <returns>Title</returns>
        protected virtual string GetTitle() {
            return GestionWeb.GetWebWord(751, _webSession.SiteLanguage);
        }
        #endregion

        #region Get Mail Content
        /// <summary>
        /// Get Mail Content
        /// </summary>
        /// <returns>Mail content</returns>
        protected virtual string GetMailContent() {
            return GestionWeb.GetWebWord(2006, _webSession.SiteLanguage).Replace("é","e");
        }
        #endregion

        /// <summary>
        /// Get Module
        /// </summary>
        /// <returns>module</returns>
        protected virtual Domain.Web.Navigation.Module GetModule()
        {
            return ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
        }

        #endregion

        #region Methode Override

        #region GetHtmlHeader
        /// <summary>
        /// Get Html Header
        /// </summary>
        /// <returns>Html Header</returns>
        protected override string GetHtmlBodyStyle()
        {
            return string.Empty;
        }
        #endregion

        #endregion

        #region Evenement Envoi mail client
        /// <summary>
        /// Rise exception when the customer mail has not been sent
        /// </summary>
        /// <param name="source">Error source></param>
        /// <param name="message">Error message</param>
        protected virtual void mail_mailKoHandler(object source, string message)
        {
            throw new MiysisPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
        }
        #endregion        

        protected bool CanShowVisual(DBCst.Vehicles.names vh)
        {

            switch (vh)
            {
                case DBCst.Vehicles.names.tv:
                case DBCst.Vehicles.names.tvAnnounces:
                case DBCst.Vehicles.names.tvGeneral:
                case DBCst.Vehicles.names.tvNicheChannels:
                case DBCst.Vehicles.names.tvNonTerrestrials:
                case DBCst.Vehicles.names.tvSponsorship:               
                case DBCst.Vehicles.names.radio:
                case DBCst.Vehicles.names.radioGeneral:
                case DBCst.Vehicles.names.radioMusic:
                case DBCst.Vehicles.names.radioSponsorship:                
                    return false;
                default: return true;
            }

            ;
        }

        #region GetTxFontCharset
        /// <summary>
        /// Get Text Font Charset
        /// </summary>
        /// <returns></returns>
        protected override TxFontCharset GetTxFontCharset()
        {
           return _config.PdfCreatorPilotCharsets[_webSession.SiteLanguage.ToString()];
            
        }
        #endregion

        protected virtual string ConvertionToHtmlString(string htmlText)
        {
            //Patch Russia don't use method ConvertionToHtmlString
            if (WebApplicationParameters.CountryCode == "7") return htmlText;
            return Convertion.ToHtmlString(htmlText);
        }

        #region Private Methods

        #region Comparative Period Detail
        /// <summary>
        /// Dates sélectionnées
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="currentModule">Module en cours</param>
        /// <param name="dateFormatText">Booléen date en format texte</param>
        /// <returns>HTML</returns>
        /// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
        private string GetComparativePeriodDetail(WebSession webSession, int currentModuleId, bool dateFormatText) {
        
            StringBuilder html = new StringBuilder();
            string dateBegin;
            string dateEnd;
            DateTime dateBeginDT;
            DateTime dateEndDT;

            if (currentModuleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                // get date begin and date end according to period type
                dateBeginDT = Web.Core.Utilities.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
                dateEndDT = Web.Core.Utilities.Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);

                // get comparative date begin and date end
                dateBeginDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateBeginDT.Date, webSession.ComparativePeriodType);
                dateEndDT = TNS.AdExpress.Web.Core.Utilities.Dates.GetPreviousYearDate(dateEndDT.Date, webSession.ComparativePeriodType);

                // Formating date begin and date end
                html.Append("<tr height=\"20\">");
                html.Append("<td class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(2292, webSession.SiteLanguage) + "</td></tr>");
                html.Append("<tr height=\"20\">");
		        html.Append("<td class=\"txtViolet11\" vAlign=\"top\">&nbsp;");
                if (dateFormatText) {
                    dateBegin = Web.Core.Utilities.Dates.getPeriodTxt(webSession, dateBeginDT.ToString("yyyyMMdd"));
                    dateEnd = Web.Core.Utilities.Dates.getPeriodTxt(webSession, dateEndDT.ToString("yyyyMMdd"));
                }
                else {
                    dateBegin = Web.Core.Utilities.Dates.DateToString(Web.Core.Utilities.Dates.GetPeriodBeginningDate(dateBeginDT.ToString("yyyyMMdd"), webSession.PeriodType), webSession.SiteLanguage);
                    dateEnd = Web.Core.Utilities.Dates.DateToString(Web.Core.Utilities.Dates.GetPeriodBeginningDate(dateEndDT.ToString("yyyyMMdd"), webSession.PeriodType), webSession.SiteLanguage);
                }
                if (!dateBegin.Equals(dateEnd)) {
                    html.Append(Convertion.ToHtmlString(GestionWeb.GetWebWord(896, webSession.SiteLanguage)) + " ");
                    html.Append(dateBegin);
                    html.Append(" " + GestionWeb.GetWebWord(897, webSession.SiteLanguage) + " ");
                    html.Append(dateEnd);
                }
                else {
                    html.Append(dateBegin);
                }

                html.Append("</td></tr>");
            }
            return (html.ToString());
        }
        #endregion

        #region Comparative Period Type Detail
        /// <summary>
        /// Dates sélectionnées
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="currentModule">Module en cours</param>
        /// <returns>HTML</returns>
        /// <remarks>Date format to be like for example novembre 2004 - janvier 2005</remarks>
        private string GetComparativePeriodTypeDetail(WebSession webSession, int currentModuleId) {
            StringBuilder html = new StringBuilder();

            if (currentModuleId == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA) {
                // Formating date begin and date end
                html.Append("<tr height=\"20\">");
                html.Append("<td class=\"txtViolet11Bold\">&nbsp;" + GestionWeb.GetWebWord(2293, webSession.SiteLanguage) + "</td></tr>");
                html.Append("<tr height=\"20\">");
                html.Append("<td class=\"txtViolet11\" vAlign=\"top\">&nbsp;");
                html.Append(HtmlFunctions.GetComparativePeriodTypeDetail(webSession, currentModuleId));
                html.Append("</td></tr>");
            }
            return (html.ToString());
        }
        #endregion

        #endregion

    }
}
