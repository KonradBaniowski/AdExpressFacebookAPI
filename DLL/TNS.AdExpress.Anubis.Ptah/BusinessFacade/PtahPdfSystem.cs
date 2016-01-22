using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using PDFCreatorPilotLib;
using TNS.AdExpress.Anubis.Ptah.Common;
using TNS.AdExpress.Anubis.Ptah.Exceptions;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpressI.Rolex.DAL;
using TNS.Ares.Pdf;
using TNS.Ares.Pdf.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Date;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.WebTheme;
using GenericDetailLevel = TNS.AdExpress.Domain.Level.GenericDetailLevel;
using Module = TNS.AdExpress.Constantes.Web.Module;

namespace TNS.AdExpress.Anubis.Ptah.BusinessFacade
{
    public class PtahPdfSystem : Pdf
    {
        #region Variables
        private IDataSource _dataSource = null;
        /// <summary>
        /// Appm Configuration (usefull for PDF layout)
        /// </summary>
        private PtahConfig _config = null;
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
        /// Constructeur
        /// </summary>
        public PtahPdfSystem(IDataSource dataSource, PtahConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
            :
        base(theme.GetStyle("Ptah"))
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
                throw new PtahPdfException("Error in Constructor Ptah Pdf System", e);
            }

        }
        #endregion

        #region Init
        public string Init()
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
                DocumentInfo_Creator = DocumentInfo_Author = _config.PdfAuthor;
                DocumentInfo_Subject = _config.PdfSubject;
                DocumentInfo_Title = GestionWeb.GetWebWord(751, _webSession.SiteLanguage);
                DocumentInfo_Producer = _config.PdfProducer;
                DocumentInfo_Keywords = _config.PdfKeyWords;
                return shortFName;
            }
            catch (System.Exception e)
            {
                throw new PtahPdfException("Error to initialize PtahPdfSystem in Init()", e);
            }
            finally
            {
                if (_config.UseImpersonate && _impersonateInformation != null) CloseImpersonation();
            }
        }
        #endregion

        #region Fill
        /// <summary>
        /// Fill
        /// </summary>
        public void Fill()
        {

            try
            {
                

                #region MainPage

                MainPageDesign();

                #endregion

                #region RolexFileResult

                RolexFileResult(AddVisuals, CreationServerPathes.LOCAL_PATH_ROLEX);

                #endregion

                #region Header and Footer

                string dateString = Web.Core.Utilities.Dates.DateToString(DateTime.Now, _webSession.SiteLanguage,
                                                                          TNS.AdExpress.Constantes.FrameWork.Dates
                                                                             .Pattern.customDatePattern);

                this.AddHeadersAndFooters(
                    _webSession,
                    imagePosition.leftImage,
                    GestionWeb.GetWebWord(2975, _webSession.SiteLanguage) + " - " + dateString,
                    0, -1, true);

                #endregion
            }
            catch (System.Exception e)
            {
                throw new PtahPdfException("Error to Fill Pdf in Fill()" + e.StackTrace + e.Source, e);
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

            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

            Style.GetTag("createdTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
            this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str)) / 2,
                1 * this.PDFPAGE_Height / 3, 0, str);

        }
        #endregion

        #region RolexFileResult

        /// <summary>
        /// addVisuals delegate
        /// </summary>
        /// <param name="addVisuals">add Visuals delegate</param>
        /// <param name="visualDitectory">visual Ditectory</param>
        private void RolexFileResult(Action<List<string>, string> addVisuals, string visualDitectory)
        {
            var html = new StringBuilder(1000);

            var arrList = new ArrayList {53, 76, 72};

            var detailLevel = new GenericDetailLevel(arrList);
            var module = ModulesList.GetModule(Module.Name.ROLEX);
            var param = new object[3];
            param[0] = _webSession;
            param[1] = _webSession.DetailPeriodBeginningDate;
            param[2] = _webSession.DetailPeriodEndDate;
            var rolexScheduleDAL =
                (IRolexDAL)
                AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\"
                                                                    + module.CountryDataAccessLayer.AssemblyName,
                                                                    module.CountryDataAccessLayer.Class, false,
                                                                    BindingFlags.CreateInstance
                                                                    | BindingFlags.Instance | BindingFlags.Public, null,
                                                                    param, null, null);

            using (
                var ds = rolexScheduleDAL.GetFileData(_webSession.GenericMediaDetailLevel,
                                                      _webSession.SelectedLevelsValue, detailLevel))
            {
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    long? oldIdSite = null;
                    long? oldIdPage = null;
                    long? oldIdLocation = null;

                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var dr = ds.Tables[0].Rows[i];

                        if (oldIdSite != Convert.ToInt64(dr["ID_SITE"]) ||
                            oldIdLocation != Convert.ToInt64(dr["ID_LOCATION"])
                            || oldIdPage != Convert.ToInt64(dr["ID_PAGE"]))
                        {
                            #region Content

                            html.Append(
                                "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" Valign=\"top\" width=\"100%\" height=\"100%\">");

                            #region Header

                            //SITE
                            if (oldIdSite != Convert.ToInt64(dr["ID_SITE"]))
                            {
                                html.Append("<tr><td class=\"RolexFilePopUpHeader\" style=\"white-space: nowrap\">");
                                html.AppendFormat("&nbsp;&nbsp;{0}", dr["SITE"].ToString());

                                html.Append("</td></tr>");
                            }

                            #endregion

                            //Content
                            html.Append("<tr  width=\"100%\"><td class=\"rofDescr\" width=\"100%\" style=\"white-space: nowrap\">");
                            html.Append("<ul class=\"rof\">");

                            //URL
                            html.AppendFormat(
                                " <li><span class=\"rofSp\"\" style=\"white-space: nowrap\"> <a class=\"roSite\" href=\"{0}\" target=\"_blank\" >{0}</a></span></li>",
                                 dr["URL"].ToString());

                            //Dates
                            string dateBegin =
                                DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_BEGIN_NUM"].ToString()),
                                                                _webSession.SiteLanguage);
                            string dateEnd =
                                DateString.YYYYMMDDToDD_MM_YYYY(Convert.ToInt32(dr["DATE_END_NUM"].ToString()),
                                                                _webSession.SiteLanguage);
                            html.AppendFormat(
                                " <li ><span><b>{0}:</b></span><span class=\"rofSp\" style=\"white-space: nowrap\"> {1} {2} {3} {4}</span></li>",
                                GestionWeb.GetWebWord(895, _webSession.SiteLanguage),
                                GestionWeb.GetWebWord(896, _webSession.SiteLanguage), dateBegin,
                                GestionWeb.GetWebWord(897, _webSession.SiteLanguage), dateEnd);

                            // Location
                            html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"rofSp\"\" style=\"white-space: nowrap\"> {1}</span></li>",
                                              GestionWeb.GetWebWord(1732, _webSession.SiteLanguage),
                                              dr["LOCATION"].ToString());

                            //PRESENCE TYPE               
                            html.AppendFormat(" <li><span style=\"white-space: nowrap\"><b>{0}:</b></span>",
                                              GestionWeb.GetWebWord(2957, _webSession.SiteLanguage));
                            html.Append(" <ul class=\"rofbd\">");
                            DataRow[] dataRows =
                                ds.Tables[0].Select(" ID_SITE =" + Convert.ToString(dr["ID_SITE"]) + " AND ID_LOCATION=" +
                                                    Convert.ToString(dr["ID_LOCATION"]) + " AND ID_PAGE=" +
                                                    Convert.ToString(dr["ID_PAGE"]));
                            foreach (DataRow dataRow in dataRows)
                            {
                                html.AppendFormat("<li><span class=\"rofSp\" style=\"white-space: nowrap\">{0}</span> </li>",
                                                  dataRow["PRESENCE_TYPE"].ToString());
                            }
                            html.Append(" </ul>");
                            html.Append(" </li>");

                            // Commentary
                            if (dr["COMMENTARY"] != DBNull.Value)
                                html.AppendFormat(" <li><span><b>{0}:</b></span><span class=\"rofSp\"\" style=\"white-space: nowrap\"> {1}</span>",
                                                  GestionWeb.GetWebWord(74, _webSession.SiteLanguage),
                                                  dr["COMMENTARY"].ToString());

                            html.Append("</ul></td></tr>");

                            html.Append("</table>");

                            #endregion

                            ConvertHtmlToPDF(html.ToString(),
                                             WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                                             WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                             _config.WebServer,
                                             _config.Html2PdfLogin,
                                             _config.Html2PdfPass);

                            #region Add Visuals

                            if (dr["VISUAL"] != DBNull.Value && dr["VISUAL"].ToString().Length > 0)
                            {
                                var visuals = new List<string>(dr["VISUAL"].ToString().Split(','));
                                addVisuals(visuals, visualDitectory);
                            }

                            #endregion
                        }

                        html = new StringBuilder(10000);

                        oldIdSite = Convert.ToInt64(dr["ID_SITE"]);
                        oldIdLocation = Convert.ToInt64(dr["ID_LOCATION"]);
                        oldIdPage = Convert.ToInt64(dr["ID_PAGE"]);
                    }
                }
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
                throw (new PtahPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
        }
        #endregion

        #region Send
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
                    GestionWeb.GetWebWord(2985, _webSession.SiteLanguage),
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
                throw new PtahPdfException("Error to Send mail to client in Send(string fileName)", e);
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
            throw new PtahPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
        }
        #endregion

        #region GetHtmlHeader


        /// <summary>
        /// Get Html Header
        /// </summary>
        /// <returns>Html Header</returns>
        protected override string GetHtmlHeader(string charset, string themeName, string serverName)
        {
            var html = new StringBuilder();
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
            html.Append("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/Rolex.css\" type=\"text/css\" rel=\"stylesheet\">");
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

        #region Add visuals

        /// <summary>
        /// Add Visuals
        /// </summary>
        /// <param name="imgList">visual list</param>
        /// <param name="visualDitectory">visual Ditectory </param>
        protected virtual void AddVisuals(List<string> imgList,string visualDitectory)
        {
            if (imgList != null && imgList.Count > 0)
            {
              
                foreach (string visual in imgList.Select(img => visualDitectory + img).Where(File.Exists))
                {
                    NewPage();
                   this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                    using (var imgG = Image.FromFile(visual))
                    {
                        int imgI = this.AddImageFromFilename(visual, TxImageCompressionType.itcFlate);
                        double w = (double) (this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double) imgG.Width;
                        double coef = coef = Math.Min((double) 1.0, w);
                        w = ((double) (this.WorkZoneBottom - (this.WorkZoneTop+20))/(double) imgG.Height);
                        coef = Math.Min((double) coef, w);
                        double X1 = (double) (this.PDFPAGE_Width/2 - (coef*imgG.Width)/2);
                        double Y1 = (double) (this.PDFPAGE_Height/2 - (coef*imgG.Height)/2);

                        PDFPAGE_ShowImage(imgI,
                                               X1, Y1,
                                               coef*imgG.Width,
                                               coef*imgG.Height,
                                               0);
                    }
                }
            }
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
