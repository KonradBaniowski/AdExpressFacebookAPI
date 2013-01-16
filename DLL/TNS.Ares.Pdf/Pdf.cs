#region Informations
// Auteur: G. Ragneau, G. Facon
// Date de création: 16/08/2005
// Date de modification: 
#endregion

using System;
using System.Drawing;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

using PDFCreatorPilotLib;
using TNS.Ares.Pdf.Exceptions;
using TNS.FrameWork.WebTheme;
using HTML2PDFAddOn;
using HtmlSnap2;
using System.IO;
using System.Text;
using TNS.AdExpress.Domain.Web;

namespace TNS.Ares.Pdf
{
    /// <summary>
    /// Classe de base pour générer un résultat de type PDF
    /// </summary>
    public class Pdf : PDFDocument3Class
    {

        #region Variables

        #region Image position
        /// <summary>
        /// indique la position de l'image
        /// </summary>
        public enum imagePosition
        {
            /// <summary>
            /// Image located on the right of the header
            /// </summary>
            rightImage = 0,
            /// <summary>
            /// Image located on the left of the header
            /// </summary>
            leftImage = 1
        }
        #endregion

        #region Layout
        /// <summary>
        /// Style
        /// </summary>
        protected Style _style = null;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected double _leftMargin = 15;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected double _rightMargin = 15;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected double _topMargin = 15;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected double _bottomMargin = 15;
        /// <summary>
        /// LandScape pages watermark index
        /// </summary>
        protected int poLandScapeWaterMk = -1;
        /// <summary>
        /// Portrait pages watermark index
        /// </summary>
        protected int poPortraitWaterMk = -1;
        /// <summary>
        /// Size of the headers
        /// </summary>
        protected double _headerHeight = 50;
        /// <summary>
        /// Size of the footers
        /// </summary>
        protected double _footerHeight = 20;
        #endregion

        #endregion

        #region Accesseurs
        /// <summary>
        /// Get Style
        /// </summary>
        public Style Style
        {
            get { return _style; }
        }
        /// <summary>
        /// Get / Set the size of the left margin
        /// </summary>
        public double LeftMargin
        {
            get { return _leftMargin; }
            set { _leftMargin = value; }
        }
        /// <summary>
        /// Get the size of the right margin
        /// </summary>
        public double RightMargin
        {
            get { return _rightMargin; }
            set { _rightMargin = value; }
        }
        /// <summary>
        /// Get the size of the top margin
        /// </summary>
        public double TopMargin
        {
            get { return _topMargin; }
        }
        /// <summary>
        /// Get the size of the bottom margin
        /// </summary>
        public double BottomMargin
        {
            get { return _bottomMargin; }
        }

        /// <summary>
        /// Get Beginning of the work zone of the document (topMargin+headerSize)
        /// </summary>					  
        public double WorkZoneTop
        {
            get
            {
                return _topMargin + _headerHeight;
            }
        }
        /// <summary>
        /// Get End of the work zone of the document (topMargin+headerSize)
        /// </summary>					  
        public double WorkZoneBottom
        {
            get
            {
                return this.PDFPAGE_Height - _bottomMargin - _footerHeight;
            }
        }
        #endregion

        #region	Constructeur
        /// <summary>
        /// Constructeur (default values : 
        /// leftMargin=15;
        /// rightMargin=15;
        /// topMargin=15;
        /// bottomMargin=15;
        /// headerHeight=50;
        /// footerHeight=20;
        /// )
        /// </summary>
        public Pdf()
            : base()
        {
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        public Pdf(Style style)
            : base()
        {
            try
            {
                _style = style;
                _leftMargin = ((Box)(_style.GetTag("layout"))).Margin.MarginLeft;
                _rightMargin = ((Box)(_style.GetTag("layout"))).Margin.MarginRight;
                _topMargin = ((Box)(_style.GetTag("layout"))).Margin.MarginTop;
                _bottomMargin = ((Box)(_style.GetTag("layout"))).Margin.MarginBottom;
                _headerHeight = ((Box)(_style.GetTag("header"))).Height;
                _footerHeight = ((Box)(style.GetTag("footer"))).Height;
            }
            catch (Exception e)
            {
                throw new PdfException("Error in Constructor Pdf", e);
            }
        }
        #endregion

        #region Init
        /// <summary>
        /// Initialize the PDF (Create it and get it ready for building process)
        /// </summary>
        public virtual void Init(bool postDisplay, string fileName, string pdfCreatorPilotMail, string pdfCreatorPilotPass)
        {
            try
            {
                this.StartEngine(pdfCreatorPilotMail, pdfCreatorPilotPass);
                this.FileName = fileName;
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
        public virtual void AddHeadersAndFooters(WebSession webSession, imagePosition position, string title, int fromPage, int toPage, bool withCopyright)
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
                     + WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CompanyNameTexts.GetCompanyShortName(webSession.SiteLanguage) + " "
                     + GestionWeb.GetWebWord(2849, webSession.SiteLanguage);
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
        public virtual void AddHeadersAndFooters(WebSession webSession, bool leftImage, bool rightImage, string title, int fromPage, int toPage)
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

        #region GetWaterMark
        /// <summary>
        /// Create a watermark for the current page or return it if it already exists
        /// </summary>
        /// <param name="leftImage">Image located on the left of the header</param>
        /// <param name="rightImage">Image located on the right of the header</param>
        /// <param name="title">Title located in the header</param>
        protected virtual int GetWaterMark(bool leftImage, bool rightImage, string title)
        {

            int w = -1, lImg, rImg;
            double coef;
            string pathPicture = string.Empty;

            if (this.PDFPAGE_Orientation == TxPDFPageOrientation.poPageLandscape)
            {

                if (this.poLandScapeWaterMk < 0)
                {
                    w = poLandScapeWaterMk = this.CreateWaterMark();
                }
                else
                    return this.poLandScapeWaterMk;
            }

            if (this.PDFPAGE_Orientation == TxPDFPageOrientation.poPagePortrait)
            {
                if (this.poPortraitWaterMk < 0)
                {
                    w = poPortraitWaterMk = this.CreateWaterMark();
                }
                else
                    return this.poPortraitWaterMk;
            }

            //If watermarck does'nt exist, we build it and return it.


            TxPDFPageSize pSize = this.PDFPAGE_Size;
            TxPDFPageOrientation pOrien = this.PDFPAGE_Orientation;

            this.SwitchedToWatermark = true;

            this.PDFPAGE_Size = pSize;
            this.PDFPAGE_Orientation = pOrien;

            //left Image
            if (leftImage)
            {
                Picture picture = ((Picture)Style.GetTag("pictureLeft"));
                if (File.Exists(picture.Path))
                {
                    pathPicture = picture.Path;
                }
                else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path))
                {
                    pathPicture = AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path;
                }
                else
                {
                    pathPicture = picture.Path;
                }

                coef = 1.0;
                lImg = this.AddImageFromFilename(pathPicture, TxImageCompressionType.itcFlate);
                Image lgImg = Image.FromFile(pathPicture);
                if (lgImg.Height > _headerHeight)
                    coef = _headerHeight / lgImg.Height;
                this.PDFPAGE_ShowImage(lImg, _leftMargin, _topMargin, lgImg.Width * coef, lgImg.Height * coef, 0);
            }

            //right image
            if (rightImage)
            {
                Picture picture = ((Picture)Style.GetTag("pictureRight"));
                if (File.Exists(picture.Path))
                {
                    pathPicture = picture.Path;
                }
                else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path))
                {
                    pathPicture = AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path;
                }
                else
                {
                    pathPicture = picture.Path;
                }
                coef = 1.0;
                rImg = this.AddImageFromFilename(pathPicture, TxImageCompressionType.itcFlate);
                Image rgImg = Image.FromFile(pathPicture);
                if (rgImg.Height > _headerHeight)
                    coef = _headerHeight / rgImg.Height;
                this.PDFPAGE_ShowImage(rImg, this.PDFPAGE_Width - _rightMargin - (rgImg.Width * coef), _topMargin, rgImg.Width * coef, rgImg.Height * coef, 0);
            }

            //title
            _style.GetTag("headerFont").SetStylePdf(this, GetTxFontCharset());

            double fontSize = ((TNS.FrameWork.WebTheme.Font)_style.GetTag("headerFont")).Size;
            this.PDFPAGE_UnicodeTextOut(
                this.PDFPAGE_Width / 2 - (this.PDFPAGE_GetTextWidth(title) / 2),
                (_headerHeight) / 2 + _topMargin - fontSize / 2,
                0,
                title);

            //footer line
            this.PDFPAGE_SetLineWidth(1);
            this.PDFPAGE_MoveTo(_leftMargin, this.WorkZoneBottom);
            this.PDFPAGE_LineTo(this.PDFPAGE_Width - _rightMargin, this.WorkZoneBottom);
            this.PDFPAGE_FillAndStroke();
            this.SwitchedToWatermark = false;

            return w;

        }

        #endregion

        #region ConvertHtmlToPDF
        /// <summary>
        /// Transformation du code HTML en une nouvelle page PDF
        /// </summary>
        /// <param name="html">Le code HTML</param>
        /// <param name="charset">Charset</param>
        /// <param name="themeName">Theme Name</param>
        /// <param name="serverName">Server Name</param>
        /// <param name="html2PdfLogin">Html2Pdf Library Login</param>
        /// <param name="html2PdfPassword">Html2Pdf Library Password</param>
        /// <returns>Current Page Index of PDF</returns>
        public virtual int ConvertHtmlToPDF(string html, string charset, string themeName, string serverName, string html2PdfLogin, string html2PdfPassword)
        {
            this.NewPage();
            return ConvertHtmlToPDF(html, charset, themeName, serverName, html2PdfLogin, html2PdfPassword, this.GetCurrentPageIndex());
        }

        /// <summary>
        /// Transformation du code HTML en une nouvelle page PDF
        /// </summary>
        /// <param name="html">Le code HTML</param>
        /// <param name="charset">Charset</param>
        /// <param name="themeName">Theme Name</param>
        /// <param name="serverName">Server Name</param>
        /// <param name="html2PdfLogin">Html2Pdf Library Login</param>
        /// <param name="html2PdfPassword">Html2Pdf Library Password</param>
        /// <param name="pageNumber">Page Number</param>
        /// <returns>Current Page Index of PDF</returns>
        public virtual int ConvertHtmlToPDF(string html, string charset, string themeName, string serverName, string html2PdfLogin, string html2PdfPassword, int pageNumber)
        {

            #region Traitment
            try
            {

                #region Get Temp File
                string workFile = Path.GetTempFileName();
                #endregion

                #region HTML
                StreamWriter sw = null;

                try
                {
                    sw = File.CreateText(workFile);

                    #region Html Header
                    sw.WriteLine(this.GetHtmlHeader(charset, themeName, serverName));
                    #endregion

                    #region Html Content
                    sw.Write(html.ToString());
                    #endregion

                    #region Html Footer
                    sw.WriteLine(this.GetHtmlFooter());
                    #endregion

                }
                catch (Exception e)
                {
                    throw new PdfException("Impossible to write HTML to File '" + workFile + "'", e);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw = null;
                    }
                }
                #endregion

                #region Html file loading

                HTML2PDF2Class htmlTmp = null;

                try
                {

                    #region Create new Page Pdf
                    this.SetCurrentPage(pageNumber);
                    this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;
                    #endregion

                    htmlTmp = new HTML2PDF2Class();
                    htmlTmp.MarginLeft = Convert.ToInt32(this.LeftMargin);
                    htmlTmp.MarginTop = Convert.ToInt32(this.WorkZoneTop);
                    htmlTmp.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
                    htmlTmp.MinimalWidth = this.PDFPAGE_Width - Convert.ToInt32(this.LeftMargin) - Convert.ToInt32(this.RightMargin);
                    htmlTmp.StartHTMLEngine(html2PdfLogin, html2PdfPassword);
                    htmlTmp.ConnectToPDFLibrary(this);
                    htmlTmp.LoadHTMLFile(workFile);
                    htmlTmp.ConvertAll();

                }
                catch (Exception e)
                {
                    throw new PdfException("Impossible to Convert HTML file to PDF", e);
                }
                finally
                {
                    if (htmlTmp != null)
                    {
                        htmlTmp.DisconnectFromPDFLibrary();
                        htmlTmp.UnloadAll();
                        htmlTmp = null;
                    }
                }
                #endregion

                #region Clean File
                File.Delete(workFile);
                #endregion

                return this.GetCurrentPageIndex();
            }
            catch (System.Exception e)
            {
                throw (new PdfException("Impossible to Convert HTML to PDF", e));
            }
            #endregion
        }

        #endregion

        #region ConvertHtmlToSnapJpgByte
        /// <summary>
        /// Transformation du code HTML en une nouvelle page PDF
        /// </summary>
        /// <param name="html">Le code HTML</param>
        /// <param name="charset">Charset</param>
        /// <param name="themeName">Theme Name</param>
        /// <param name="serverName">Server Name</param>
        /// <param name="html2PdfLogin">Html2Pdf Library Login</param>
        /// <param name="html2PdfPassword">Html2Pdf Library Password</param>
        /// <returns>Current Page Index of PDF</returns>
        public virtual byte[] ConvertHtmlToSnapJpgByte(string html, string charset, string themeName, string serverName)
        {

            byte[] data = null;
            CHtmlSnapClass snap = null;
            string htmlFile = "";
            FileStream fs = null;
            StreamWriter writer = null;
            try
            {
                snap = new CHtmlSnapClass();
                snap.SetTimeOut(100000);
                snap.SetCode("21063505C78EB32A");

                string fileName = DateTime.Now.ToString("yyyyMMdd_")
                   + TNS.Ares.Functions.GetRandomString(30, 40);

                htmlFile = System.IO.Path.GetTempPath() + fileName + ".html";
                fs = File.OpenWrite(htmlFile);
                writer = new StreamWriter(fs, Encoding.UTF8);
                writer.Write(GetHtmlHeader(charset, themeName, serverName) + html.ToString() + GetHtmlFooter());
                writer.Close();
                writer = null;
                fs.Close();               
                fs = null;         
                snap.SnapUrl(htmlFile, "*");
                data = (byte[])snap.GetImageBytes(".jpg");

            }
            finally
            {
                if (snap != null)
                {
                    snap.Clear();
                    snap = null;
                }

                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
                if (fs != null)
                {
                    fs.Close();                   
                    fs = null;
                }
                if (File.Exists(htmlFile)) File.Delete(htmlFile);
            }
            return data;
        }

        #endregion

        #region Private Methods

        #region GetHtmlHeader
        /// <summary>
        /// Get Html Header
        /// </summary>
        /// <returns>Html Header</returns>
        protected virtual string GetHtmlHeader(string charset, string themeName, string serverName)
        {
            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE HTML >");
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
        #endregion

        #region GetHtmlHeader
        /// <summary>
        /// Get Html Header
        /// </summary>
        /// <returns>Html Header</returns>
        protected virtual string GetHtmlBodyStyle()
        {
            return "style=\"margin-top:0px;\"";
        }
        #endregion

        #region GetHtmlFooter
        /// <summary>
        /// Get Html Footer
        /// </summary>
        /// <returns>Html Footer</returns>
        protected virtual string GetHtmlFooter()
        {
            return "</form></body></HTML>";
        }
        #endregion

        #region GetTxFontCharset
        /// <summary>
        /// Get Text Font Charset
        /// </summary>
        /// <returns></returns>
        protected virtual TxFontCharset GetTxFontCharset()
        {
            return TxFontCharset.charsetANSI_CHARSET;
        }
        #endregion


        #endregion
    }
}
