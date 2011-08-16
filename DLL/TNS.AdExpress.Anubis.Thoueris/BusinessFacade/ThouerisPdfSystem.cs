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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Reflection;

using TNS.AdExpress.Anubis.Thoueris.Common;
using TNS.AdExpress.Anubis.Thoueris.Exceptions;

using TNS.AdExpress.Web.UI.Results;

using TNS.AdExpress.Constantes.Customer;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Rules.Results;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultConstantes = TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;
using System.Globalization;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using DomainLevel = TNS.AdExpress.Domain.Level;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.Ares;
using TNS.Ares.Pdf;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Anubis.Thoueris.Common;
using TNS.AdExpress.Anubis.Thoueris.Exceptions;
using TNS.AdExpressI.VP;
using TNS.AdExpressI.VP.DAL;
using TNS.AdExpress.Anubis.Thoueris.Exceptions;
using TNS.AdExpress.Domain.Level;
#endregion

namespace TNS.AdExpress.Anubis.Thoueris.BusinessFacade
{
    /// <summary>
    /// Generate the PDF document for Thoueris Pdf System module.
    /// </summary>
    public class ThouerisPdfSystem : Pdf
    {

        #region Variables
        private IDataSource _dataSource = null;
        /// <summary>
        /// Appm Configuration (usefull for PDF layout)
        /// </summary>
        private ThouerisConfig _config = null;
        /// <summary>
        /// Customer Client request
        /// </summary>
        private DataRow _rqDetails = null;
        /// <summary>
        /// WebSession to process
        /// </summary>
        private WebSession _webSession = null;
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
        public ThouerisPdfSystem(IDataSource dataSource, ThouerisConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            :
        base(theme.GetStyle("Thoueris"))
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
                throw new ThouerisPdfException("Error in Constructor Thoueris Pdf System", e);
            }

        }
        #endregion

        #region Init
        internal string Init()
        {
            try
            {
                string shortFName = "";
                string fName = GetFileName(_rqDetails, ref shortFName);
                bool display = false;
#if(DEBUG)
                display = true;
#endif
                base.Init(true, fName, _config.PdfCreatorPilotLogin, _config.PdfCreatorPilotPass);
                this.DocumentInfo_Creator = this.DocumentInfo_Author = _config.PdfAuthor;
                this.DocumentInfo_Subject = _config.PdfSubject;
                this.DocumentInfo_Title = GestionWeb.GetWebWord(751, _webSession.SiteLanguage);
                this.DocumentInfo_Producer = _config.PdfProducer;
                this.DocumentInfo_Keywords = _config.PdfKeyWords;
                return shortFName;
            }
            catch (System.Exception e)
            {
                throw new ThouerisPdfException("Error to initialize SelketPdfSystem in Init()", e);
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

                #region Vp Result
                VpResult();
                #endregion

                #region Header and Footer
                string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

                this.AddHeadersAndFooters(
                _webSession,
                imagePosition.leftImage,
                GestionWeb.GetWebWord(2858, _webSession.SiteLanguage) + " - " + dateString,
                0, -1, true);
                #endregion

            }
            catch (System.Exception e)
            {
                throw new ThouerisPdfException("Error to Fill Pdf in Fill()" + e.StackTrace + e.Source, e);
            }
        }
        #endregion

        #region Send
        internal void Send(string fileName)
        {


            try
            {
                ArrayList to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient)
                {
                    to.Add(s);
                }
                SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GestionWeb.GetWebWord(2898, _webSession.SiteLanguage),
                    GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer)
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage),
                    true, _config.CustomerMailServer, _config.CustomerMailPort);
                mail.SubjectEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.BodyEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].ContentEncoding);
                mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
                mail.SendWithoutThread(false);
            }
            catch (System.Exception e)
            {
                throw new ThouerisPdfException("Error to Send mail to client in Send(string fileName)", e);
            }

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
            catch (System.Exception e)
            {
                throw (new ThouerisPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
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

            Style.GetTag("bigTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
            string str = "";

            this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(GestionWeb.GetWebWord(2858, _webSession.SiteLanguage))) / 2,
                    (this.PDFPAGE_Height) / 4, 0, GestionWeb.GetWebWord(2858, _webSession.SiteLanguage));

            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

            Style.GetTag("createdTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
            this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str)) / 2,
                1 * this.PDFPAGE_Height / 3, 0, str);

        }
        #endregion

        #region SessionParameter
        private void SessionParameter()
        {

            //Formatting date to be used in the query
            string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd");
            string dateEnd = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd");

            StringBuilder html = new StringBuilder();

            html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");

            #region Title
            html.Append("<TR height=\"25\">");
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

            #region Personnalized level
            html.Append("<TR height=\"7\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"1\" class=\"lightPurple\">");
            html.Append("<TD></TD>");
            html.Append("</TR>");
            html.Append("<TR>");
            html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1896, _webSession.SiteLanguage)) + " :</TD>");
            html.Append("</TR>");
            html.Append("<TR height=\"20\">");
            html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;");
            DetailLevelItemInformation persoLevel = DetailLevelItemsInformation.Get(_webSession.PersonnalizedLevel);
            html.Append(Convertion.ToHtmlString(GestionWeb.GetWebWord(persoLevel.WebTextId, _webSession.SiteLanguage)));
            html.Append("</TD>");
            html.Append("</TR>");
            #endregion

            #region Concurrents
            bool withConcurrents = false;
            if (_webSession.SelectionUniversMedia != null && _webSession.SelectionUniversMedia.Nodes.Count > 0)
            {
                withConcurrents = true;
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2869, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD align=\"left\">");                
                html.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversMedia, false, false, true, 600, true, false, _webSession.SiteLanguage, 2, 1, true, _webSession.DataLanguage, _webSession.Source, true));
                html.Append("</TD>");
                html.Append("</TR>");
            }
            #endregion

            #region product
            if (_webSession.SelectionUniversProduct != null && _webSession.SelectionUniversProduct.Nodes.Count > 0)
            {
                if (withConcurrents)
                {
                    html.Append("</TABLE>");
                    this.ConvertHtmlToPDF(html.ToString(),
              WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
              WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
              _config.WebServer,
              _config.Html2PdfLogin,
              _config.Html2PdfPass);
                     html = new StringBuilder();
                    html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
                }
                html.Append("<TR height=\"7\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR height=\"1\" class=\"lightPurple\">");
                html.Append("<TD></TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
                html.Append("</TR>");
                html.Append("<TR>");
                html.Append("<TD align=\"left\">");
                html.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversProduct, false, false, true, 600, true, false, _webSession.SiteLanguage, 2, 1, true, _webSession.DataLanguage, _webSession.Source, true));
                html.Append("</TD>");
                html.Append("</TR>");
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

        #region VP  result
        /// <summary>
        /// Get VP promo file string
        /// </summary>
        protected virtual void VpResult()
        {

            #region GETHTML
            StringBuilder html = new StringBuilder(10000), tempHtml = new StringBuilder(1000), headerHtml = new StringBuilder(1000);
            int rowSpan = 1, colSpan = 0;
            bool newL1 = false, newL2 = false, newL3 = false;
            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(WebConstantes.Module.Name.VP);
            object[] param = new object[1] { _webSession };
            IVeillePromo vpResult = (IVeillePromo)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            VeillePromoScheduleData veillePromoScheduleData = vpResult.GetData();
            const int nbLineByPage = 30, NB_HEADER_LINES = 2;
            int currentHtmlLine = 0;
            try
            {

                if (veillePromoScheduleData != null && veillePromoScheduleData.Data != null && veillePromoScheduleData.Data.Count > 0)
                {
                    //Generic Media Detail Level
                    TNS.AdExpress.Domain.Level.GenericDetailLevel detailLevel = _webSession.GenericMediaDetailLevel;

                    //Number of levels
                    int nbLevels = _webSession.GenericMediaDetailLevel.GetNbLevels;

                    //weeks
                    Dictionary<long, long> weekList = veillePromoScheduleData.WeekList;

                    //data
                    List<object[]> data = veillePromoScheduleData.Data;

                    html.Append("<table cellspacing=\"0\" cellpadding=\"0\" >");

                    #region Start list of monthes
                    //Start list of monthes
                    rowSpan = 2;
                    html.Append("<tr>");
                    html.AppendFormat("<td rowspan=\"{0}\" class=\"ptVp\"> &nbsp;{1}</td>", rowSpan, GestionWeb.GetWebWord(2872, _webSession.SiteLanguage));
                    int curMonth = -1, curYear = -1, oldYear = -1, curWeek = -1, oldMonth = -1, start = -1;
                    Int64 oldIdL1 = -1, idL1 = -1, oldIdL2 = -1, idL2 = -1, oldIdL3 = -1, idL3 = -1;
                    int levelColSpan = 1, width = 31;
                    string promoContent = "";
                    foreach (KeyValuePair<long, long> kpv in weekList)
                    {
                        long key = kpv.Key;
                        curMonth = int.Parse(key.ToString().Substring(4, 2));
                        curYear = int.Parse(key.ToString().Substring(2, 2));
                        curWeek = int.Parse(key.ToString().Substring(6, 2));
                        if (oldMonth != curMonth && start > -1)
                        {
                            string monthString = "&nbsp;", yearString = "";
                            if (colSpan > 1)
                            {
                                monthString = MonthString.GetHTMLCharacters(oldMonth, _webSession.SiteLanguage, 3);
                                yearString = (oldYear.ToString().Length == 1) ? "0" + oldYear.ToString() : oldYear.ToString();
                            }
                            html.AppendFormat(" <td colspan=\"{0}\" class=\"ptm\">{1} {2}</td>", colSpan, monthString, yearString);
                            colSpan = 0;
                        }

                        tempHtml.AppendFormat(" <td class=\"vpw\">{0}</td>", (curWeek.ToString().Length == 1 ? "0" + curWeek.ToString() : curWeek.ToString()));
                        start = 0;
                        oldMonth = curMonth;
                        oldYear = curYear;
                        colSpan++;
                    }
                    if (start > -1)
                    {
                        string monthString = "&nbsp;", yearString = "";
                        if (colSpan > 1)
                        {
                            monthString = MonthString.GetHTMLCharacters(oldMonth, _webSession.SiteLanguage, 3);
                            yearString = (oldYear.ToString().Length == 1) ? "0" + oldYear.ToString() : oldYear.ToString();
                        }
                        html.AppendFormat(" <td colspan=\"{0}\" class=\"ptm\">{1} {2}</td>", colSpan, monthString, yearString);
                        colSpan = 0;
                    }
                    html.Append("</tr>");
                    //End list of monthes
                    #endregion


                    #region Start list of weeks
                    //Start list of weeks
                    if (tempHtml.Length > 0)
                    {
                        html.Append("<tr>");
                        html.Append(tempHtml.ToString());
                        html.Append("</tr>");
                    }
                    //End list of weeks
                    #endregion

                    currentHtmlLine = NB_HEADER_LINES;

                    //Header html
                    headerHtml.Append(html.ToString());

                    //bool newLevel = false;
                    string brand = "";
                    rowSpan = 0;
                    int firstVpItemColIndex = -1;
                    if (nbLevels == 1) firstVpItemColIndex = CstResult.VeillePromo.L1_COLUMN_INDEX + 1;
                    if (nbLevels == 2) firstVpItemColIndex = CstResult.VeillePromo.L2_COLUMN_INDEX + 1;
                    if (nbLevels == 3) firstVpItemColIndex = CstResult.VeillePromo.L3_COLUMN_INDEX + 1;

                    #region Table content

                    for (int i = 0; i < data.Count; i++)
                    {
                        object[] dr = data[i];
                        if (nbLevels >= 1) idL1 = Convert.ToInt64(dr[CstResult.VeillePromo.L1_ID_COLUMN_INDEX]);
                        if (nbLevels >= 2) idL2 = Convert.ToInt64(dr[CstResult.VeillePromo.L2_ID_COLUMN_INDEX]);
                        if (nbLevels >= 3) idL3 = Convert.ToInt64(dr[CstResult.VeillePromo.L3_ID_COLUMN_INDEX]);
                        levelColSpan = weekList.Count;

                        //Start row level 1
                        if (nbLevels >= 1 && idL1 != oldIdL1)
                        {
                            html.AppendFormat("<tr><td class=\"{0}\">", ((i == 0) ? "L1Vp" : "L1VpbBis"));
                            html.Append(Convert.ToString(dr[CstResult.VeillePromo.L1_COLUMN_INDEX]));
                            html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                            newL1 = true;
                            if (nbLevels > 1) newL2 = true;
                            currentHtmlLine++;
                            //Pagination
                            Paginate(ref html, headerHtml, ref  currentHtmlLine, NB_HEADER_LINES, nbLineByPage);

                        }
                        //End row level 1

                        //Start row level 2
                        if (nbLevels >= 2 && (idL2 != oldIdL2 || newL2))
                        {
                            html.AppendFormat("<tr><td class=\"{0}\">", ((newL1) ? "L2Vp" : "L2VpBis"));
                            html.AppendFormat("&nbsp; {0}", Convert.ToString(dr[CstResult.VeillePromo.L2_COLUMN_INDEX]));
                            html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                            newL2 = true;
                            if (nbLevels > 2) newL3 = true;
                            currentHtmlLine++;
                            //Pagination
                            Paginate(ref html, headerHtml, ref  currentHtmlLine, NB_HEADER_LINES, nbLineByPage);

                        }
                        //End row level 2

                        //Start row level 3
                        if (nbLevels >= 3 && (idL3 != oldIdL3 || newL3))
                        {
                            html.AppendFormat("<tr><td class=\"{0}\">", ((newL2) ? "L3Vp" : "L3VpBis"));
                            html.AppendFormat("&nbsp;&nbsp; {0}", Convert.ToString(dr[CstResult.VeillePromo.L3_COLUMN_INDEX]));
                            html.AppendFormat("</td><td colspan=\"{0}\" class=\"p3\">&nbsp;</td></tr>", levelColSpan);
                            newL3 = true;
                            currentHtmlLine++;
                            //Pagination
                            Paginate(ref html, headerHtml, ref  currentHtmlLine, NB_HEADER_LINES, nbLineByPage);

                        }
                        //End row level 3

                        //***********************Start promotions rows*********************************************************************

                        //rowSpan++;
                        colSpan = 0;
                        rowSpan = 1;
                        html.AppendFormat("<tr><td  rowspan=\"{0}\" class=\"L4Vp\"></td>", rowSpan);

                        for (int j = firstVpItemColIndex; j < dr.Length; j++)
                        {
                            bool endIncomplete = false, startIncomplete = false;
                            if (dr[j] != null)
                            {
                                VeillePromoItem vpi = (VeillePromoItem)dr[j];
                                if (vpi.ItemType == CstResult.VeillePromo.itemType.presentStart
                                    || vpi.ItemType == CstResult.VeillePromo.itemType.presentStartIncomplete)
                                {
                                    if (vpi.ItemType == CstResult.VeillePromo.itemType.presentStartIncomplete) startIncomplete = true;
                                    brand = vpi.Brand;
                                    for (int k = j; k < dr.Length; k++)
                                    {
                                        VeillePromoItem vpi2 = (VeillePromoItem)dr[k];
                                        if ((j == k) || (vpi2.ItemType == CstResult.VeillePromo.itemType.extended
                                        || vpi2.ItemType == CstResult.VeillePromo.itemType.endIncomplete))
                                        {
                                            if (vpi2.ItemType == CstResult.VeillePromo.itemType.endIncomplete) endIncomplete = true;
                                            colSpan++;
                                            j = k;
                                        }
                                        else
                                        {
                                            j = k - 1;
                                            break;
                                        }

                                    }

                                    html.AppendFormat("<td colspan=\"{0}\" class=\"vp2\" width=\"{1}\">", colSpan, width * colSpan);
                                    promoContent = vpResult.SplitContent(vpi.PromotionContent, colSpan);
                                    string promoPeriod = (vpi.DateBegin.Day.ToString().Length == 1) ? "0" + vpi.DateBegin.Day.ToString() : vpi.DateBegin.Day.ToString();
                                    promoPeriod += "/" + ((vpi.DateBegin.Month.ToString().Length == 1) ? "0" + vpi.DateBegin.Month.ToString() : vpi.DateBegin.Month.ToString());
                                    promoPeriod += " - " + ((vpi.DateEnd.Day.ToString().Length == 1) ? "0" + vpi.DateEnd.Day.ToString() : vpi.DateEnd.Day.ToString());
                                    promoPeriod += "/" + ((vpi.DateEnd.Month.ToString().Length == 1) ? "0" + vpi.DateEnd.Month.ToString() : vpi.DateEnd.Month.ToString());

                                    html.Append("<table class=\"vp\">");
                                    html.AppendFormat("<tr><td class=\"{0}\">", vpi.CssClass);
                                    //string promoAnchor = "<a class=\"tooltip\"  href=\"javascript:displayPromoFile_" + _resultControlId + "(" + vpi.IdDataPromotion + ", true);\">";
                                    //promoAnchor += "<em><span></span><b>" + (Convertion.ToHtmlString(brand)) + " : " + promoPeriod + "</b><br/>" + (Convertion.ToHtmlString(vpi.PromotionContent)).Replace("'", "\'") + "</em>";


                                    //html.Append(promoAnchor);
                                    if (startIncomplete) html.Append("<span class=\"flg\">&lsaquo;&nbsp;</span>");
                                    //Add promotion period
                                    if (colSpan >= CstResult.VeillePromo.NB_MIN_WEEKS_TO_SHOW_PERIOD)
                                    {
                                        html.Append(promoPeriod);
                                    }
                                    if (endIncomplete) html.Append("<span class=\"fld\">&nbsp;&rsaquo;</span>");
                                    //html.Append("</a>");
                                    html.Append("</td></tr>");
                                    //Add promotion Content
                                    html.Append("<tr><td class=\"vpb\">");
                                    //html.Append(promoAnchor);
                                    html.Append(promoContent);
                                    html.Append("</a>");
                                    html.Append("</td></tr>");
                                    html.Append("</table>");
                                    html.Append("</td>");
                                }
                                else
                                {
                                    html.AppendFormat(" <td class=\"p3\">&nbsp;</td>");
                                }
                            }
                            else
                            {
                                html.AppendFormat(" <td class=\"p3\">&nbsp;</td>");
                            }
                            colSpan = 0;
                            endIncomplete = false;
                            startIncomplete = false;
                        }

                        html.Append("</tr>");
                        currentHtmlLine++;

                        //***********************End promotions rows*********************************************************************

                        //Pagination
                        Paginate(ref html, headerHtml, ref  currentHtmlLine, NB_HEADER_LINES, nbLineByPage);

                        oldIdL1 = idL1;
                        oldIdL2 = idL2;
                        oldIdL3 = idL3;
                        newL1 = newL2 = newL3 = false;
                    }
                    #endregion
                    if (currentHtmlLine > NB_HEADER_LINES)
                    {
                        html.Append("</table>");
                        SnapShots(Convertion.ToHtmlString(html.ToString()));
                    }

                }
                else
                {

                    html.Append("<div align=\"center\" class=\"vpResNoData\">" + GestionWeb.GetWebWord(177, _webSession.SiteLanguage) + "</div>");
                    SnapShots(Convertion.ToHtmlString(html.ToString()));

                }

            }
            catch (System.Exception err)
            {
                throw (new ThouerisPdfException("Unable to process vp Schedule export result for request " + _rqDetails["id_static_nav_session"].ToString() + ".", err));
            }

            #endregion
        }
        #endregion
        #region Création et Insertion d'une image dans une nouvelle page du PDF
        /// <summary>
        /// Création et Insertion d'une image dans une nouvelle page du PDF
        /// </summary>
        /// <param name="html">Le code HTML</param>      
        private void SnapShots(string html)
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
            double coef = 0;

            imgG = Image.FromFile(filePath);
            imgI = this.AddImageFromFilename(filePath, TxImageCompressionType.itcFlate);

            double w = 0;
            w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin) / (double)imgG.Width;
            coef = Math.Min((double)1.0, w);
            w = ((double)(this.WorkZoneBottom - this.WorkZoneTop) / (double)imgG.Height);
            coef = Math.Min((double)coef, w);


            this.PDFPAGE_ShowImage(imgI,
                X1, 40,
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


        #endregion

        #region Methode Override

        #region GetHtmlHeader


        /// <summary>
        /// Get Html Header
        /// </summary>
        /// <returns>Html Header</returns>
        protected override string GetHtmlHeader(string charset, string themeName, string serverName)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
            html.Append("<HTML>");
            html.Append("<HEAD>");
            html.Append("<META http-equiv=\"Content-Type\" content=\"text/html; charset=" + charset + "\">");
            html.Append("<meta content=\"Microsoft Visual Studio .NET 7.1\" name=\"GENERATOR\">");
            html.Append("<meta content=\"C#\" name=\"CODE_LANGUAGE\">");
            html.Append("<meta content=\"JavaScript\" name=\"vs_defaultClientScript\">");
            html.Append("<meta content=\"http://schemas.microsoft.com/intellisense/ie5\" name=\"vs_targetSchema\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/AdExpress.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/GenericUI.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/MediaSchedule.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/Vp.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
            html.Append("<meta http-equiv=\"expires\" content=\"0\">");
            html.Append("<meta http-equiv=\"pragma\" content=\"no-cache\">");
            html.Append("<meta name=\"Cache-control\" content=\"no-cache\">");
            html.Append("</HEAD>");
            html.Append("<body " + GetHtmlBodyStyle() + ">");
            html.Append("<form>");
            return html.ToString();
        }


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
        private void mail_mailKoHandler(object source, string message)
        {
            throw new Exceptions.ThouerisPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
        }
        #endregion

        protected void Paginate(ref StringBuilder html, StringBuilder headerHtml, ref int currentHtmlLine, int NB_HEADER_LINES, int nbLineByPage)
        {
            //Pagination
            if (currentHtmlLine > NB_HEADER_LINES && ((currentHtmlLine % nbLineByPage) == 0))
            {
                html.Append("</table>");
                SnapShots(Convertion.ToHtmlString(html.ToString()));

                html = new StringBuilder(10000);
                html.Append(headerHtml.ToString());
                currentHtmlLine = NB_HEADER_LINES;//2 For header line : months line + weeks line
            }

        }


    }
}
