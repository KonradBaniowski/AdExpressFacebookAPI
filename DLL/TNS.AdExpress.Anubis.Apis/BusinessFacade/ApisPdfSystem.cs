using System;
using System.Collections;
using System.Text;
using TNS.AdExpress.Anubis.Apis.Common;
using TNS.AdExpress.Anubis.Apis.Exceptions;
using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Ares.Pdf.Exceptions;
using TNS.FrameWork.DB.Common;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork.WebTheme;
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Anubis.Apis.BusinessFacade {
    /// <summary>
    /// Celebrities PDF system
    /// </summary>
    public class ApisPdfSystem : Miysis.BusinessFacade.MiysisPdfSystem {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public ApisPdfSystem(IDataSource dataSource, ApisConfig config, DataRow rqDetails, WebSession webSession, Theme theme)      
            : base(dataSource, config, rqDetails, webSession, theme) {                       
        }
        #endregion

        #region Get Title
        /// <summary>
        /// Get Title
        /// </summary>
        /// <returns>Title</returns>
        protected override string GetTitle() {
            return GestionWeb.GetWebWord(2949, _webSession.SiteLanguage);
        }
        #endregion

        #region Get Mail Content
        /// <summary>
        /// Get Mail Content
        /// </summary>
        /// <returns>Mail content</returns>
        protected override string GetMailContent() {
            return GestionWeb.GetWebWord(2966, _webSession.SiteLanguage);
        }
        /// <summary>
        /// Get Module
        /// </summary>
        /// <returns>module</returns>
        protected override Domain.Web.Navigation.Module GetModule()
        {
            return ModulesList.GetModule(WebConstantes.Module.Name.CELEBRITIES);
        }

        #endregion

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="fileName">file Name</param>
        public override void Send(string fileName)
        {
            try
            {
                var to = new ArrayList();
                foreach (string s in _webSession.EmailRecipient)
                {
                    to.Add(s);
                }
                var mail = new SmtpUtilities(_config.CustomerMailFrom, to,
                    GetMailContent(),
                    GestionWeb.GetWebWord(1750, _webSession.SiteLanguage) + "\"" + _webSession.ExportedPDFFileName
                    + "\"" + String.Format(GestionWeb.GetWebWord(1751, _webSession.SiteLanguage), _config.WebServer)
                    + "<br><br>"
                    + GestionWeb.GetWebWord(1776, _webSession.SiteLanguage),
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
            catch (Exception e)
            {
                throw new ApisPdfException("Error to Send mail to client in Send(string fileName)", e);
            }
        }

        #region AddHeadersAndFooters
        /// <summary>
        /// Add Header and footer to the page between i and j
        /// </summary>
        /// <param name="position">Image position on the header</param>
        /// <param name="title">Title in the header</param>
        /// <param name="fromPage">Beginning page</param>
        /// <param name="toPage">End page</param>
        /// <param name="withCopyright">Indication du CopyRight</param>
        /// <param name="webSession">La WebSession</param>
        public override void AddHeadersAndFooters(WebSession webSession, imagePosition position, string title, int fromPage, int toPage, bool withCopyright)
        {
            try
            {
                if (fromPage < 0)
                    fromPage = 0;
                if (toPage < 0)
                    toPage = this.PageCount;

                for (int i = fromPage; i < toPage; i++)
                {
                    this.SetCurrentPage(i);
                    if (position == imagePosition.leftImage)
                        this.PDFPAGE_Watermark = GetWaterMark(true, false, title);
                    else if (position == imagePosition.rightImage)
                        this.PDFPAGE_Watermark = GetWaterMark(false, true, title);
                    //Page number					
                    string str = GestionWeb.GetWebWord(894, webSession.SiteLanguage) + " " + (i + 1) + " " + GestionWeb.GetWebWord(2042, webSession.SiteLanguage) + " " + this.PageCount;
                    _style.GetTag("footerFont").SetStylePdf(this, GetTxFontCharset());

                    this.PDFPAGE_UnicodeTextOut(
                        this.PDFPAGE_Width / 2 - (this.PDFPAGE_GetTextWidth(str) / 2)
                        , this.WorkZoneBottom + this._footerHeight / 2, 0, str);

                    if (withCopyright)
                    {
                        string strCopyright = GestionWeb.GetWebWord(2848, webSession.SiteLanguage) + " "
                     + " "
                     + GestionWeb.GetWebWord(2841, webSession.SiteLanguage);
                        _style.GetTag("copyright").SetStylePdf(this, GetTxFontCharset());
                        this.PDFPAGE_UnicodeTextOut(

                            this.PDFPAGE_Width - this._rightMargin - ((this.PDFPAGE_GetTextWidth(str) * 2))
                            , this.WorkZoneBottom + this._footerHeight / 2, 0, strCopyright);
                    }
                }
            }
            catch (System.Exception e)
            {
                throw (new PdfException("Unable to buil headers and footers", e));
            }

        }

        /// <summary>
        /// Add Header and footer to the page between i and j
        /// </summary>
        /// <param name="leftImage">Image located on the left of the header</param>
        /// <param name="rightImage">Image located on the right of the header</param>
        /// <param name="title">Title in the header</param>
        /// <param name="fromPage">Beginning page</param>
        /// <param name="fontColor">Font Color</param>
        /// <param name="toPage">End page</param>
        /// <param name="font">Police</param>
        /// <param name="webSession">Session</param>
        public override void AddHeadersAndFooters(WebSession webSession, bool leftImage, bool rightImage, string title, int fromPage, int toPage)
        {
            try
            {
                if (fromPage < 0)
                    fromPage = 0;
                if (toPage < 0)
                    toPage = this.PageCount;

                for (int i = fromPage; i < toPage; i++)
                {
                    this.SetCurrentPage(i);
                    this.PDFPAGE_Watermark = this.GetWaterMark(leftImage, rightImage, title);
                    //Page number
                    string str = GestionWeb.GetWebWord(894, webSession.SiteLanguage) + " " + (i + 1) + " " + GestionWeb.GetWebWord(2042, webSession.SiteLanguage) + " " + this.PageCount;
                    _style.GetTag("footerFont").SetStylePdf(this, GetTxFontCharset());
                    this.PDFPAGE_UnicodeTextOut(
                        this.PDFPAGE_Width / 2 - (this.PDFPAGE_GetTextWidth(str) / 2)
                        , this.WorkZoneBottom + this._footerHeight / 2, 0, str);
                }
            }
            catch (System.Exception e)
            {
                throw (new PdfException("Unable to buil headers and footers", e));
            }

        }
        #endregion

    }
}
