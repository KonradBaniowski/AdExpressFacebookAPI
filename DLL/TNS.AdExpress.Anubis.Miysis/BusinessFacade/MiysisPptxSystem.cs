#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Slides;
using Aspose.Slides.Export;
using HtmlSnap2;
using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Anubis.Miysis.Exceptions;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.CampaignTypes;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpressI.Classification.DAL;
using TNS.AdExpressI.MediaSchedule;
using TNS.Classification.Universe;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;
using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Insertions.Cells;
using TNS.FrameWork.WebTheme;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using Font = TNS.FrameWork.WebTheme.Font;
using Picture = TNS.FrameWork.WebTheme.Picture;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Helper;
using TNS.AdExpress.Web.Helper.MediaPlanVersions;

#endregion

namespace TNS.AdExpress.Anubis.Miysis.BusinessFacade
{
    /// <summary>
    /// Generate the PDF document for MiysisPdfSystem module.
    /// </summary>
    public class MiysisPptxSystem 
    {

        #region Variables
        private List<String> _deleteFiles = new List<string>(); 
        private int _startIndexVisual = 0;
        private Presentation _pres = null;

        public String FileName ;
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

        #region Layout
        /// <summary>
        /// Style
        /// </summary>
        protected Style _style = null;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected float _leftMargin = 15;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected float _rightMargin = 15;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected float _topMargin = 15;
        /// <summary>
        /// Pdf left Margin
        /// </summary>
        protected float _bottomMargin = 15;
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
        protected float _headerHeight = 50;
        /// <summary>
        /// Size of the footers
        /// </summary>
        protected float _footerHeight = 20;

        public float WorkZoneTop
        {
            get
            {
                return _topMargin + _headerHeight;
            }
        }

        public float WorkZoneBottom
        {
            get
            {
                return _bottomMargin + _footerHeight;
            }
        }
        #endregion
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public MiysisPptxSystem(IDataSource dataSource, MiysisConfig config, DataRow rqDetails, WebSession webSession, Theme theme)
        {
            try
            {
                

                _style = theme.GetStyle("Miysis");

                _leftMargin = (float)((Box)(_style.GetTag("layout"))).Margin.MarginLeft;
                _rightMargin = (float)((Box)(_style.GetTag("layout"))).Margin.MarginRight;
                _topMargin = (float)((Box)(_style.GetTag("layout"))).Margin.MarginTop;
                _bottomMargin = (float)((Box)(_style.GetTag("layout"))).Margin.MarginBottom;
                _headerHeight = (float)((Box)(_style.GetTag("header"))).Height;
                _footerHeight = (float)((Box)(_style.GetTag("footer"))).Height;
                var license = new License();
                license.SetLicense("Aspose.Slides.lic");
                _dataSource = dataSource;
                _config = config;
                _rqDetails = rqDetails;
                _webSession = webSession;    
                _pres = new Presentation();
                _pres.SlideSize.Type = SlideSizeType.A4Paper;
                _pres.SlideSize.Size = new SizeF(1280, 793);
            }
            catch (Exception e)
            {
                throw new MiysisPptxException("Error in Constructor MiysisPdfSystem", e);
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
                FileName = GetFileName(_rqDetails, ref shortFName);
                IDocumentProperties dp = _pres.DocumentProperties;
                dp.Author = _config.PdfAuthor;
                dp.Subject = _config.PdfSubject;
                dp.Title= GetTitle();
                dp.Keywords = _config.PdfKeyWords;
                dp.Manager = _config.PdfProducer;
                return shortFName;
            }
            catch (Exception e)
            {
                throw new MiysisPptxException("Error to initialize MiysisPdfSystem in Init()", e);
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
                string dateString = Web.Core.Utilities.Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, Constantes.FrameWork.Dates.Pattern.customDatePattern);

                AddHeadersAndFooters(_webSession, GetTitle() + " - " + dateString, 0, -1, true);
                #endregion

            }
            catch (System.Exception e)
            {
                throw new MiysisPptxException("Error to Fill Pdf in Fill()" + e.Message + " : " + e.StackTrace + ":" + e.Source, e);
            }
        }
        #endregion

        #region Send
        /// <summary>
        /// Send mail
        /// </summary>
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
                    + "\"" + String.Format(GestionWeb.GetWebWord(3066, _webSession.SiteLanguage), _config.WebServer + "/AdExCustomerFiles/" + _webSession.CustomerLogin.IdLogin + "/" + fileName + ".pptx")
                    + "<br><br>"
                    + String.Format(GestionWeb.GetWebWord(1776, _webSession.SiteLanguage), "http://km-adexpress.kantarmedia.fr"),
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
                mail.mailKoHandler += mail_mailKoHandler;
                mail.SendWithoutThread(false);
            }
            catch (System.Exception e)
            {
                throw new MiysisPptxException("Error to Send mail to client in Send(string fileName)", e);
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

                pdfFileName += @"\" + shortName + ".pptx";

                string checkPath = Regex.Replace(pdfFileName, @"(\.pptx)+", ".pptx", RegexOptions.IgnoreCase | RegexOptions.Multiline);


                int i = 0;
                while (File.Exists(checkPath))
                {
                    if (i <= 1)
                    {
                        checkPath = Regex.Replace(pdfFileName, @"(\.pptx)+", "_" + (i + 1) + ".pptx", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    }
                    else
                    {
                        checkPath = Regex.Replace(pdfFileName, "(_" + i + @"\.pptx)+", "_" + (i + 1) + ".pptx", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    }
                    i++;
                }
                return checkPath;
            }
            catch (System.Exception e)
            {
                throw (new MiysisPptxException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() + ".", e));
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// save the doc
        /// </summary>
        public void EndDoc()
        {
            try
            {
                var path = Path.GetDirectoryName(FileName);
                if (!String.IsNullOrWhiteSpace(path))
                {

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    _pres.Save(FileName, SaveFormat.Pptx);

                    _deleteFiles.ForEach(p =>
                    {
                        if (File.Exists(p))
                        {
                            File.Delete(p);
                        }
                    });
                }
            }
            catch (Exception)
            {

                throw new PptxException("Erreur lors de la sauvegarde du fichier : " + FileName);
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
        public virtual void AddHeadersAndFooters(WebSession webSession,  string title, int fromPage, int toPage, bool withCopyright)
        {
            try
            {
                if (fromPage < 0)
                    fromPage = 0;
                if (toPage < 0)
                    toPage = _pres.Slides.Count;

                for (int i = fromPage; i < toPage; i++)
                {
                    var sld = _pres.Slides[i];
                    string pathPicture = string.Empty;
                    Picture picture = ((Picture) _style.GetTag("pictureLeft"));
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
                    
                     AddImageH( sld, pathPicture, _leftMargin, _topMargin, _headerHeight);

                    AddTextCenterHorizontalement(sld, title, (Font)_style.GetTag("headerFont"),  _topMargin + _headerHeight / 2);

                //Page number					
                    string str = GestionWeb.GetWebWord(894, webSession.SiteLanguage) + " " + (i + 1) + " " + GestionWeb.GetWebWord(2042, webSession.SiteLanguage) + " " + _pres.Slides.Count;

                    AddTextCenterHorizontalement(sld, str, (Font)_style.GetTag("footerFont"), _pres.SlideSize.Size.Height - WorkZoneBottom + _footerHeight / 2);
                    

                    if (withCopyright)
                    {
                        var widthCopyright = 150;
                        string strCopyright = GestionWeb.GetWebWord(2848, webSession.SiteLanguage) + " "
                     + WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CompanyNameTexts.GetCompanyShortName(webSession.SiteLanguage) + " "
                     + GestionWeb.GetWebWord(2849, webSession.SiteLanguage);
                        AddText(sld, strCopyright, (Font) _style.GetTag("copyright"),
                            _pres.SlideSize.Size.Width - _rightMargin - widthCopyright,
                            _pres.SlideSize.Size.Height - WorkZoneBottom + _footerHeight / 2, widthCopyright, 20);

                    }

                    float sizeLine = _pres.SlideSize.Size.Width - _leftMargin - _rightMargin;
                    float centerX = _pres.SlideSize.Size.Width / 2;
                    float linePosX = centerX - (sizeLine / 2);
                    sld.Shapes.AddAutoShape(ShapeType.Line, linePosX, _pres.SlideSize.Size.Height - _bottomMargin - _footerHeight, sizeLine, 1);
                }
            }
            catch (Exception e)
            {
                throw (new PptxException("Unable to buil headers and footers", e));
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
            var sld = _pres.Slides[0];
            
            string imgPath = string.Empty;
            var picture = (( Picture)_style.GetTag("pictureTitle"));
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

            AddImageCenterHorizontalement( sld, imgPath, _pres.SlideSize.Size.Height - WorkZoneBottom - 100, _pres.SlideSize.Size.Width - _leftMargin - _rightMargin - 300);

            AddTextCenterHorizontalement(sld, GetTitle(), (Font)_style.GetTag("bigTitle"), _pres.SlideSize.Size.Height / 4);

            var str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, Constantes.FrameWork.Dates.Pattern.customDatePattern);
            AddTextCenterHorizontalement(sld, str, (Font)_style.GetTag("createdTitle"), _pres.SlideSize.Size.Height / 3);
      
        }
        #endregion

        #region PPTX

        private void CreatePage()
        {
            ILayoutSlide layout;
            for (int i = 0; i < _pres.LayoutSlides.Count; i++)
            {
                layout = _pres.LayoutSlides[i];
                if (layout.LayoutType == SlideLayoutType.Blank)
                {
                    _pres.Slides[_pres.Slides.Count - 1].LayoutSlide = layout;

                    _pres.Slides.AddEmptySlide(layout);
                }
            }
        }

        private void AddText(ISlide sld, string text, Font font, float positionX, float positionY, float width, float heigth)
        {

            IAutoShape shp = sld.Shapes.AddAutoShape(ShapeType.Rectangle, positionX, positionY, width, heigth);
            shp.FillFormat.SolidFillColor.Color = Color.Transparent;
            shp.FillFormat.FillType = FillType.Solid;
            shp.LineFormat.Style = LineStyle.NotDefined;
            shp.ShapeStyle.LineColor.Color = Color.Transparent;
            //Then add a textframe inside it
            ITextFrame tf = shp.TextFrame;

            //Set a text
            tf.Text = text;
            IPortion port = tf.Paragraphs[0].Portions[0];
            port.PortionFormat.FillFormat.FillType = FillType.Solid;
            port.PortionFormat.FillFormat.SolidFillColor.Color = font.Color;
            port.PortionFormat.FontBold = font.IsBold ? NullableBool.True : NullableBool.False;
            port.PortionFormat.FontItalic = font.IsItalic ? NullableBool.True : NullableBool.False;
            port.PortionFormat.LatinFont = new FontData(font.Name);
            port.PortionFormat.FontHeight = (float)font.Size;
            IParagraph para1 = tf.Paragraphs[0];
            para1.ParagraphFormat.Alignment = TextAlignment.Left;
        }
        private void AddTextCenterHorizontalement( ISlide sld, string text, Font font, float positionY)
        {
            float centerX = _pres.SlideSize.Size.Width / 2;
            float textPosX = centerX - (_pres.SlideSize.Size.Width / 2);
            float textPosY = positionY;
            IAutoShape shp = sld.Shapes.AddAutoShape(ShapeType.Rectangle, textPosX, textPosY, _pres.SlideSize.Size.Width, 20);
            shp.FillFormat.SolidFillColor.Color = Color.Transparent;
            shp.FillFormat.FillType = FillType.Solid;
            shp.LineFormat.Style = LineStyle.NotDefined;
            shp.ShapeStyle.LineColor.Color = Color.Transparent;
            //Then add a textframe inside it
            ITextFrame tf = shp.TextFrame;

            //Set a text
            tf.Text = text;
            tf.TextFrameFormat.CenterText = NullableBool.True;
            IPortion port = tf.Paragraphs[0].Portions[0];
            port.PortionFormat.FillFormat.FillType = FillType.Solid;
            port.PortionFormat.FillFormat.SolidFillColor.Color = font.Color;
            port.PortionFormat.FontBold = font.IsBold?NullableBool.True:NullableBool.False;
            port.PortionFormat.FontItalic = font.IsItalic ? NullableBool.True : NullableBool.False;
            port.PortionFormat.LatinFont = new FontData(font.Name);
            port.PortionFormat.FontHeight = (float)font.Size;
            IParagraph para1 = tf.Paragraphs[0];
            para1.ParagraphFormat.Alignment = TextAlignment.Center;
        }

        private void AddImageW( ISlide sld, string path, float positionX, float positionY, float width = 0)
        {
            Image img = new Bitmap(path);
            IPPImage imgx = _pres.Images.AddImage(img);
            float imageSizeWidth = imgx.Width;
            float imageSizeHeight = imgx.Height;
            if (width != 0)
            {
                imageSizeWidth = width;
                imageSizeHeight = imgx.Height * imageSizeWidth / imgx.Width;
            }

            //Add Picture Frame with height and width equivalent of Picture
            sld.Shapes.AddPictureFrame(ShapeType.Rectangle, positionX, positionY, imageSizeWidth, imageSizeHeight, imgx);
        }

        private void AddImage(ISlide sld, string path, float positionX, float positionY, float width, float Heigth)
        {
            if (File.Exists(path))
            {


                Image img = new Bitmap(path);
                IPPImage imgx = _pres.Images.AddImage(img);
                float imageSizeWidth = width;
                float imageSizeHeight = Heigth;

                //Add Picture Frame with height and width equivalent of Picture
                sld.Shapes.AddPictureFrame(ShapeType.Rectangle, positionX, positionY, imageSizeWidth, imageSizeHeight, imgx);
            }
        }

        private void AddImageH(ISlide sld, string path, float positionX, float positionY, float height = 0)
        {
            Image img = new Bitmap(path);
            IPPImage imgx = _pres.Images.AddImage(img);
            float imageSizeWidth = imgx.Width;
            float imageSizeHeight = imgx.Height;
            if (height != 0)
            {
                imageSizeWidth = imgx.Width * imageSizeHeight / imgx.Height; ;
                imageSizeHeight = height;
            }

            //Add Picture Frame with height and width equivalent of Picture
            sld.Shapes.AddPictureFrame(ShapeType.Rectangle, positionX, positionY, imageSizeWidth, imageSizeHeight, imgx);
        }

        private void AddImageCenterHorizontalement( ISlide sld, string path, float positionY, float width)
        {
            Image img = new Bitmap(path);
            IPPImage imgx = _pres.Images.AddImage(img);

            float centerX = _pres.SlideSize.Size.Width / 2;

            var imageSizeWidth = width;
            var imageSizeHeight = imgx.Height * imageSizeWidth / imgx.Width;

            float imagePosX = centerX - (imageSizeWidth / 2);
            float imagePosY = positionY - imageSizeHeight;

            //Add Picture Frame with height and width equivalent of Picture
            sld.Shapes.AddPictureFrame(ShapeType.Rectangle, imagePosX, imagePosY, imageSizeWidth, imageSizeHeight, imgx);
        }

        private void AddImageFullPage( ISlide sld, string path)
        {
            Image img = new Bitmap(path);
            float heigth = _pres.SlideSize.Size.Height - _topMargin- _headerHeight - _bottomMargin - _footerHeight;
            float width = _pres.SlideSize.Size.Width - _leftMargin - _rightMargin;
            IPPImage imgx = _pres.Images.AddImage(img);
            float imageSizeWidth = imgx.Width;
            float imageSizeHeight = imgx.Height;
            if (imageSizeWidth > width)
            {
                imageSizeWidth = width;
                imageSizeHeight = imgx.Height * imageSizeWidth / imgx.Width;
            }
            if (imageSizeHeight > heigth)
            {
                imageSizeHeight = heigth;
                imageSizeWidth = imgx.Width * imageSizeHeight / imgx.Height;
            }

            //Add Picture Frame with height and width equivalent of Picture
            sld.Shapes.AddPictureFrame(ShapeType.Rectangle, 30, 50, imageSizeWidth, imageSizeHeight, imgx);
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
          if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule) {
            html.Append(GetComparativePeriodDetail(_webSession, Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA, false));
          }
         // Type Sélection comparative
         if (_webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule) {
            html.Append(GetComparativePeriodTypeDetail(_webSession, Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA));
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
                ToHtml(ref html, _webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource, 600, true, nbLineByPage, ref currentLine);
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

            SnapShots(html.ToString());

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
                && (detailSelections != null && detailSelections.Contains(WebConstantes.DetailSelection.Type.regionSelected.GetHashCode())))
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
                SnapShots(html.ToString());
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

                if (vehicles.Length == 1 && _webSession.DetailPeriod == WebConstantes.CustomerSessions.Period.DisplayLevel.dayly && result.VersionsDetail.Count > 0)
                {

                    var versionsUI = new VersionsPluriMediaUI(_webSession, period, "");
                    decoupageVersionHTML(html, versionsUI.GetExportMSCreativesHtml(ref creativeCells, _style), true, int.Parse(vehicles[0]));
                    var currentVehicle = VehiclesInformation.Get(idVehicle);
                    if (CanShowVisual(currentVehicle.Id ))                       
                        BuildVersionPDF(creativeCells);

                }
                #endregion

                #region Construction du tableaux global
                Double w, width;
                var timeSpan = period.End.Subtract(period.Begin);
                var nbToSplit = (Int64)Math.Round((double)timeSpan.Days / 7);

                switch (_webSession.DetailPeriod)
                {
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                        nbToSplit = (Int64)Math.Round((double)nbToSplit / 7);
                        break;
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
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
              
                decoupageHTML(html, ConvertionToHtmlString(result.HTMLCode), nbLines);
                html.Append("\r\n\t\t</td>\r\n\t</tr>");
                html.Append("</table>");
                #endregion

                html.Append("</BODY>");
                html.Append("</HTML>");

            }
            catch (Exception err)
            {
                throw (new MiysisPptxException("Unable to process Media Schedule Alert export result for request " + _rqDetails["id_static_nav_session"].ToString() + ".", err));
            }
            finally
            {
                _webSession.CurrentModule = module.Id;
            }
            #endregion

        }
        #endregion

        /// <summary>
        /// Get Html render to show universe selection into excel file
        /// </summary>
        /// <param name="adExpressUniverse">adExpress Universe</param>
        /// <param name="language">language</param>
        /// <param name="source">Data Source</param>
        /// <returns>Html render to show universe selection</returns>
        public void ToHtml(ref StringBuilder html, AdExpressUniverse adExpressUniverse, int language, int dataLanguage, IDataSource source, int witdhTable, bool paginate, int nbLineByPage, ref int currentLine)
        {
            List<NomenclatureElementsGroup> groups = null;
            int baseColSpan = 3;

            if (adExpressUniverse != null && adExpressUniverse.Count() > 0)
            {
                html.Append("<table style=\" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=" + witdhTable + "  >");
                //Groups of items excludes
                 groups = adExpressUniverse.GetExludes();
                 GetUniverseGroupForHtml(ref html, groups, baseColSpan, language, dataLanguage, source, AccessType.excludes, paginate, nbLineByPage, ref currentLine);

                //Groups of items includes
                groups = adExpressUniverse.GetIncludes();
                GetUniverseGroupForHtml(ref html, groups, baseColSpan, language, dataLanguage, source, AccessType.includes, paginate, nbLineByPage, ref currentLine);

                html.Append("</table>");
            }

        }

        /// <summary>
        /// Get Html render to show universe selection into excel file
        /// </summary>
        /// <param name="groups">universe groups</param>
        /// <param name="baseColSpan">base column span</param>
        /// <param name="language">language</param>
        /// <param name="source">Data Source</param>
        /// <param name="accessType">items access type</param>
        /// <returns>Html render to show universe selection</returns>
        private void GetUniverseGroupForHtml(ref StringBuilder html, List<NomenclatureElementsGroup> groups, int baseColSpan, int language, int dataLanguage, IDataSource source, AccessType accessType, bool paginate, int nbLineByPage, ref int currentLine)
        {


            #region Variables
            List<long> itemIdList = null;
            //int colSpan = 0;
            string checkBox = "";
            string buttonAutomaticChecked = "checked";
            string disabled = "disabled";
            ClassificationLevelListDAL universeItems = null;
            ClassificationLevelListDALFactory factoryLevels = null;
            Domain.Layers.CoreLayer cl = null;
            int code = 0;
            int colonne = 0;
            bool displayBorder = true;
            #endregion

            if (accessType == AccessType.includes) code = 2281;
            else code = 2282;


            if (groups != null && groups.Count > 0)
            {
                cl = WebApplicationParameters.CoreLayers[Constantes.Web.Layers.Id.classificationLevelList];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Detail selection control"));
                object[] param = new object[2];
                param[0] = source;
                param[1] = dataLanguage;
                factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                for (int i = 0; i < groups.Count; i++)
                {
                    List<long> levelIdsList = groups[i].GetLevelIdsList();
                    displayBorder = true;

                    html.Append(GetBlankLine(baseColSpan));
                    if (paginate) currentLine++;

                    if (i > 0 && accessType == AccessType.includes) code = 2368;
                    html.Append("<tr class=\"txtViolet11Bold\"><td colspan=" + baseColSpan + "  >" + GestionWeb.GetWebWord(code, language) + "&nbsp; : </td></tr>");
                    if (paginate) currentLine++;

                    //For each group's level
                    if (levelIdsList != null)
                    {
                        html.Append("<tr><td>");

                        for (int j = 0; j < levelIdsList.Count; j++)
                        {

                            //Level label
                            if (displayBorder) html.Append("<table class=\"UniverseHeaderStyle\"  cellpadding=0 cellspacing=0 width=\"100%\"  >");
                            else html.Append("<table class=\"UniverseHeaderStyleWithoutTop\"  cellpadding=0 cellspacing=0  width=\"100%\">");
                            html.Append("<tr class=\"txtViolet11Bold\" ><td colspan=" + baseColSpan + "  >&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language)) + " </td></tr>");
                            html.Append("</table>");
                            displayBorder = false;
                            if (paginate) currentLine++;

                            //Show items of the current level							
                            colonne = 0;
                            universeItems = factoryLevels.CreateDefaultClassificationLevelListDAL(UniverseLevels.Get(levelIdsList[j]), groups[i].GetAsString(levelIdsList[j]));
                            if (universeItems != null)
                            {
                                itemIdList = universeItems.IdListOrderByClassificationItem;
                                if (itemIdList != null && itemIdList.Count > 0)
                                {
                                    html.Append("<table class=\"UniverseItemsStyle\" width=\"100%\">");

                                    for (int k = 0; k < itemIdList.Count; k++)
                                    {

                                        //Current item label										
                                        checkBox = "<input type=\"checkbox\"  " + disabled + " " + buttonAutomaticChecked + "  ID=\"AdvertiserSelectionWebControl1_" + k + "\" name=\"AdvertiserSelectionWebControl1$" + k + "\">";
                                        if (colonne == 2)
                                        {

                                            html.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                                            html.Append(checkBox + "<label for=\"AdvertiserSelectionWebControl1_" + k + "\">" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</label>");
                                            html.Append("</td>");
                                            colonne = 1;
                                        }
                                        else if (colonne == 1)
                                        {
                                            html.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                                            html.Append(checkBox + "<label for=\"AdvertiserSelectionWebControl1_" + k + "\">" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</label>");
                                            html.Append("</td>");
                                            html.Append("</tr>");
                                            colonne = 0;

                                            if (paginate) currentLine++;
                                            //Pagination
                                            if (paginate && nbLineByPage > 0 && currentLine > 0 && (currentLine % nbLineByPage) == 0 && k < itemIdList.Count - 1)
                                            {
                                                html.Append("</table>");
                                                SnapShots(html.ToString());

                                                //Add Russian options
                                                html = new StringBuilder();
                                                html.Append("<table class=\"UniverseHeaderStyle\"  cellpadding=0 cellspacing=0  width=\"100%\">");
                                                html.Append("<tr class=\"txtViolet11Bold\" ><td colspan=" + baseColSpan + "  >&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, language)) + " </td></tr>");
                                                html.Append("</table>");
                                                html.Append("<table class=\"UniverseItemsStyle\" width=\"100%\">");
                                                currentLine = 0;
                                            }

                                        }
                                        else
                                        {
                                            html.Append("<tr>");
                                            html.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
                                            html.Append(checkBox + "<label for=\"AdvertiserSelectionWebControl1_" + k + "\">" + universeItems[Int64.Parse(itemIdList[k].ToString())] + "</label>");
                                            html.Append("</td>");
                                            colonne = 2;
                                        }
                                    }
                                    if (colonne != 0)
                                    {
                                        if (colonne == 2)
                                        {
                                            html.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                                            html.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                                        }
                                        else if (colonne == 1)
                                        {
                                            html.Append("<td class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
                                        }
                                        html.Append("</tr>");
                                        if (paginate) currentLine++;
                                    }
                                }
                                html.Append("</table>");
                            }

                        }
                        html.Append("</td></tr>");
                    }
                }
            }
        }

        #region Spacer line
        /// <summary>
        /// Spacer line
        /// </summary>
        /// <returns>HTML</returns>
        private static string GetBlankLine(int colspan)
        {
            return ("<tr><td colspan=" + colspan + ">&nbsp;</td></tr>");
        }
        #endregion

        #region decoupageVersionHTML
        /// <summary>
        /// Découpage du code HTML pour l'export PDF du plan média (Visuels)
        /// </summary>
        /// <param name="html">Le code HTML à générer</param>
        /// <param name="strHtml">Le code HTML à découper</param>
        private void decoupageVersionHTML(StringBuilder html, string strHtml, bool version, int vehicle)
        {
            int startIndex = 0, oldStartIndex = 0;
            
            var partieHTML = new ArrayList();
            var htmltmp = new StringBuilder(1000);
            htmltmp.Append(html);

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
                htmltmp.Append(partieHTML[i]);
                if (version)
                {
                    if (i == 0)
                    {
                        _startIndexVisual = SnapShots(htmltmp.ToString());
                    }
                    else
                    {
                        SnapShots(htmltmp.ToString());
                    }
                }
                else
                {
                    SnapShots(htmltmp.ToString());
                }
                htmltmp = new StringBuilder();
            }
        }
        #endregion

        #region decoupageHTML
        /// <summary>
        /// Découpage du code HTML pour l'export PDF du plan média (Tableau)
        /// </summary>
        /// <param name="html">Le code HTML à générer</param>
        /// <param name="strHtml">Le code HTML à découper</param>
        /// <param name="nbLines">Nombres de lignes pour le découpage</param>
        /// <param name="version">true if version</param>
        protected  virtual void decoupageHTML(StringBuilder html, string strHtml, int nbLines)
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
                html.Append(partieHTML[i]);
                if (i < partieHTML.Count - 1) html.Append("</table>");
                SnapShots(html.ToString());
                html = new StringBuilder();
            }
        }
        #endregion

        #region Création et Insertion d'une image dans une nouvelle page du PDF
        /// <summary>
        /// Création et Insertion d'une image dans une nouvelle page du PDF
        /// </summary>
        /// <param name="html">Le code HTML</param>
        private int SnapShots(string html)
        {

            string filePath = "";

            #region Création et Insertion d'une image dans une nouvelle page du PDF

            CreatePage();
            var currentPage = _pres.Slides.Count - 1;
            ISlide sld = _pres.Slides[_pres.Slides.Count-1];

            byte[] data = ConvertHtmlToSnapJpgByte(html,
            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].PdfContentEncoding,
            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
            _config.WebServer);

            filePath = Path.GetTempFileName();
            FileStream fs = File.OpenWrite(filePath);
            var  br = new BinaryWriter(fs);
            if (data != null)
                br.Write(data);
            br.Close();
            fs.Close();

            AddImageFullPage(sld, filePath);

            _deleteFiles.Add(filePath);

            #endregion

            return currentPage;
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
                   + Ares.Functions.GetRandomString(30, 40);

                htmlFile = Path.GetTempPath() + fileName + ".html";
                fs = File.OpenWrite(htmlFile);
                writer = new StreamWriter(fs, Encoding.UTF8);
                writer.Write(GetHtmlHeader(charset, themeName, serverName) + html + GetHtmlFooter());
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
                }

                if (writer != null)
                {
                    writer.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
                if (File.Exists(htmlFile)) File.Delete(htmlFile);
            }
            return data;
        }

        #endregion

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

        #region BuildVersionPDF(String title)
        /// <summary> 
        /// Render all versions controls
        /// </summary>
        /// <returns>Html code</returns>
        protected void BuildVersionPDF(SortedDictionary<Int64, List<CellCreativesInformation>> creativeCells)
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

                        if (nbVisuel == 1 || nbVisuel==0)
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
                            InsertImageInPDF(0, item, indexPage, _startIndexVisual, X1, Y1);
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

                            InsertImageInPDF(0, item, indexPage, _startIndexVisual, X1, Y1);
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
                                    InsertImageInPDF(0, item, indexPage, _startIndexVisual, X1, Y1);
                                }
                                else
                                {
                                    if (i == end - 1)
                                    {
                                        InsertImageInPDF(i + (4 * i), item, indexPage, _startIndexVisual, X1, Y1);
                                    }
                                    else
                                    {
                                        InsertImageInPDF(i + (4 * i), item, indexPage, _startIndexVisual, X1, Y1);
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
        protected void InsertImageInPDF(Int64 index, CellCreativesInformation item, double indexPage, int startIndex,
            int X1, int Y1)
        {
            Int64 lastIndex = index + 5;
            Int64 nbVisuel = 0;

            nbVisuel = item.Visuals.Count;

            Int64 end = 0;

            if (nbVisuel < lastIndex)
                end = nbVisuel;
            else
                end = lastIndex;

            if ((nbVisuel%5) == 0)
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
            string path = string.Empty;

            X1 += 10;
            Y1 += 7;
            string imagePath = string.Empty;
            if (nbVisuel>0)
            {
                for (Int64 i = index; i < end; i++)
                {
                    path = "";
                    path = item.Visuals[(int) i].ToLower().Replace("/imagette", "");
                    path = path.ToLower().Replace("/imagespresse", "");
                    path = path.ToLower().Replace("/imagesmd", "");
                    path = path.Replace("/", "\\");
                    path = path.StartsWith("\\") ? path.Substring(1) : path;
                    if (item.Vehicle.Id == DBCst.Vehicles.names.directMarketing)
                    {
                        imagePath = Path.Combine(_config.VMCScanPath, path);

                    }
                    else if (item.Vehicle.Id == DBCst.Vehicles.names.outdoor)
                    {
                        imagePath = Path.Combine(_config.OutdoorScanPath, path);

                    }
                    else
                    {
                        if (WebApplicationParameters.CountryCode == TNS.AdExpress.Constantes.Web.CountryCode.IRELAND)
                        {
                            imgG =
                                Bitmap.FromStream(
                                    (System.Net.WebRequest.Create(string.Format(path)).GetResponse().GetResponseStream()));
                            imgG.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tmp", i + ".jpg"));
                            imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tmp", i + ".jpg");
                            _deleteFiles.Add(imagePath);
                        }
                        else
                        {
                            imagePath = Path.Combine(_config.PressScanPath, path);

                        }
                    }

                    ISlide sld = _pres.Slides[(int) indexPage + startIndex];



                    if ((end - index == 4) && (nbVisuel != 5))
                    {
                        AddImage(sld, imagePath,
                            X1, Y1,
                            223, 296);
                        X1 += 223 + 1;
                    }
                    else if (end - index == 5)
                    {
                        AddImage(sld, imagePath,
                            X1, Y1,
                            213, 300);
                        X1 += 213 + 1;
                    }
                    else
                    {
                        AddImage(sld, imagePath,
                            X1, Y1,
                            223, 296);
                        X1 += 223 + 1;
                    }
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
            return GestionWeb.GetWebWord(2006, _webSession.SiteLanguage).Replace("é", "e"); ;
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


        #region Methode Override

        #region GetHtmlHeader
        /// <summary>
        /// Get Html Header
        /// </summary>
        /// <returns>Html Header</returns>
        protected string GetHtmlBodyStyle()
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
            throw new MiysisPptxException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
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
        protected TxFontCharset GetTxFontCharset()
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
        #endregion


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
