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

namespace TNS.Ares.Pdf{
	/// <summary>
	/// Classe de base pour générer un résultat de type PDF
	/// </summary>
	public class Pdf:PDFDocument3Class{

		#region Variables
 
		#region Image position
		/// <summary>
		/// indique la position de l'image
		/// </summary>
		public enum imagePosition{
			/// <summary>
			/// Image located on the right of the header
			/// </summary>
			rightImage=0,
			/// <summary>
			/// Image located on the left of the header
			/// </summary>
			leftImage=1  
		}
		#endregion

		#region Layout
        /// <summary>
        /// Style
        /// </summary>
        private TNS.AdExpress.Domain.Theme.Style _style = null;
		/// <summary>
		/// Pdf left Margin
		/// </summary>
		private double _leftMargin = 15;
		/// <summary>
		/// Pdf left Margin
		/// </summary>
		private double _rightMargin = 15;
		/// <summary>
		/// Pdf left Margin
		/// </summary>
		private double _topMargin = 15;
		/// <summary>
		/// Pdf left Margin
		/// </summary>
		private double _bottomMargin = 15;
		/// <summary>
		/// LandScape pages watermark index
		/// </summary>
		private int poLandScapeWaterMk = -1;
		/// <summary>
		/// Portrait pages watermark index
		/// </summary>
		private int poPortraitWaterMk = -1;
		/// <summary>
		/// Size of the headers
		/// </summary>
		private double _headerHeight = 50;
		/// <summary>
		/// Size of the footers
		/// </summary>
		private double _footerHeight = 20;
		#endregion

		#endregion

		#region Accesseurs
        /// <summary>
        /// Get Style
        /// </summary>
        public TNS.AdExpress.Domain.Theme.Style Style {
            get { return _style; }
        }
		/// <summary>
		/// Get the size of the left margin
		/// </summary>
		public double LeftMargin{
			get{return _leftMargin;}
		}
		/// <summary>
		/// Get the size of the right margin
		/// </summary>
		public double RightMargin{
			get{return _rightMargin;}
		}
		/// <summary>
		/// Get the size of the top margin
		/// </summary>
		public double TopMargin{
			get{return _topMargin;}
		}
		/// <summary>
		/// Get the size of the bottom margin
		/// </summary>
		public double BottomMargin{
			get{return _bottomMargin;}
		}

		/// <summary>
		/// Get Beginning of the work zone of the document (topMargin+headerSize)
		/// </summary>					  
		public double WorkZoneTop{
			get{return _topMargin+_headerHeight;
			}
		}
		/// <summary>
		/// Get End of the work zone of the document (topMargin+headerSize)
		/// </summary>					  
		public double WorkZoneBottom{
			get{return this.PDFPAGE_Height-_bottomMargin-_footerHeight;
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
		public Pdf():base(){
		}
        /// <summary>
        /// Constructeur
        /// </summary>
        public Pdf(TNS.AdExpress.Domain.Theme.Style style)
            : base() {
            _style = style;
            _leftMargin = ((TNS.AdExpress.Domain.Theme.Box)(_style.GetTag("layout"))).Margin.MarginLeft;
            _rightMargin = ((TNS.AdExpress.Domain.Theme.Box)(_style.GetTag("layout"))).Margin.MarginRight;
            _topMargin = ((TNS.AdExpress.Domain.Theme.Box)(_style.GetTag("layout"))).Margin.MarginTop;
            _bottomMargin = ((TNS.AdExpress.Domain.Theme.Box)(_style.GetTag("layout"))).Margin.MarginBottom;
            _headerHeight = ((TNS.AdExpress.Domain.Theme.Box)(_style.GetTag("header"))).Height;
            _footerHeight = ((TNS.AdExpress.Domain.Theme.Box)(style.GetTag("footer"))).Height;
        }
		#endregion

		#region Init
		/// <summary>
		/// Initialize the PDF (Create it and get it ready for building process)
		/// </summary>
		public void Init(bool postDisplay, string fileName, string pdfCreatorPilotMail, string pdfCreatorPilotPass){
			try{
				this.StartEngine(pdfCreatorPilotMail,pdfCreatorPilotPass);
				this.FileName = fileName;
				this.AutoLaunch = postDisplay;
				this.Resolution = 96;
				this.BeginDoc ();
			}
			catch(System.Exception e){
				throw(new PdfException("Unable to initialize pdf building",e));
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
        public void AddHeadersAndFooters(WebSession webSession, imagePosition position, string title, int fromPage, int toPage, bool withCopyright) {
			try{
				if (fromPage < 0)
					fromPage = 0;
				if (toPage<0)
					toPage = this.PageCount;

				for(int i = fromPage; i < toPage; i++){
					this.SetCurrentPage(i);
					if (position == imagePosition.leftImage)
						this.PDFPAGE_Watermark = this.GetWaterMark(true, false, title);
					else if (position == imagePosition.rightImage)
                        this.PDFPAGE_Watermark = this.GetWaterMark(false, true, title);
					//Page number
					string str = "";
					str = GestionWeb.GetWebWord(894,webSession.SiteLanguage)+ " " + (i+1) + " " + GestionWeb.GetWebWord(2042,webSession.SiteLanguage) + " " + this.PageCount;
                    _style.GetTag("footerFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);

					this.PDFPAGE_TextOut(
						this.PDFPAGE_Width/2 - (this.PDFPAGE_GetTextWidth(str)/2)
						, this.WorkZoneBottom + this._footerHeight/2, 0, str);

					if(withCopyright){
                        string strCopyright = GestionWeb.GetWebWord(2266, webSession.SiteLanguage);
                        _style.GetTag("copyright").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
                        this.PDFPAGE_TextOut(
                            
							this.PDFPAGE_Width - this._rightMargin - ((this.PDFPAGE_GetTextWidth(str)*2))
							, this.WorkZoneBottom + this._footerHeight/2, 0, strCopyright);
					}
				}
			}
			catch(System.Exception e){
				throw(new PdfException("Unable to buil headers and footers",e));
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
        public void AddHeadersAndFooters(WebSession webSession, bool leftImage, bool rightImage, string title, int fromPage, int toPage) {
			try{
				if (fromPage < 0)
					fromPage = 0;
				if (toPage<0)
					toPage = this.PageCount;

				for(int i = fromPage; i < toPage; i++){
					this.SetCurrentPage(i);
                    this.PDFPAGE_Watermark = this.GetWaterMark(leftImage, rightImage, title);
					//Page number
                    string str = GestionWeb.GetWebWord(894, webSession.SiteLanguage) + " " + (i + 1) + " " + GestionWeb.GetWebWord(2042, webSession.SiteLanguage) + " " + this.PageCount;
                    _style.GetTag("footerFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
					this.PDFPAGE_TextOut(
						this.PDFPAGE_Width/2 - (this.PDFPAGE_GetTextWidth(str)/2)
						, this.WorkZoneBottom + this._footerHeight/2, 0, str);
				}
			}
			catch(System.Exception e){
				throw(new PdfException("Unable to buil headers and footers",e));
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
		/// <param name="fontColor">Font color of the watermark</param>
		/// <param name="font">Font of the headers and footers</param>
		/// <returns>Index of the watermark in th epdf document</returns>
        [Obsolete("Use GetWaterMark with theme")]
        private int GetWaterMark(string leftImage, string rightImage, string title, Color fontColor, Font font) {

			int w = -1,lImg,rImg;
			double coef;

			if(this.PDFPAGE_Orientation == TxPDFPageOrientation.poPageLandscape){

				if (this.poLandScapeWaterMk<0){
					w = poLandScapeWaterMk = this.CreateWaterMark();
				}
				else
					return this.poLandScapeWaterMk;
			}

			if(this.PDFPAGE_Orientation == TxPDFPageOrientation.poPagePortrait){
				if (this.poPortraitWaterMk<0){
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
			if((leftImage != null)&&(leftImage.Length > 0)){
				coef = 1.0;
				lImg = this.AddImageFromFilename(leftImage,TxImageCompressionType.itcFlate);
				Image lgImg = Image.FromFile(leftImage);
				if(lgImg.Height> _headerHeight)
					coef = _headerHeight / lgImg.Height;
				this.PDFPAGE_ShowImage(lImg,_leftMargin,_topMargin,lgImg.Width*coef,lgImg.Height*coef,0);
			}

			//right image
			if((rightImage != null)&&(rightImage.Length > 0)){
				coef = 1.0;
				rImg = this.AddImageFromFilename(rightImage,TxImageCompressionType.itcFlate);
				Image rgImg = Image.FromFile(rightImage);
				if(rgImg.Height> _headerHeight)
					coef = _headerHeight / rgImg.Height;
				this.PDFPAGE_ShowImage(rImg,this.PDFPAGE_Width - _rightMargin - (rgImg.Width*coef),_topMargin,rgImg.Width*coef,rgImg.Height*coef,0);
			}
			
			//title
			this.PDFPAGE_SetActiveFont(font.Name, font.Bold, font.Italic, font.Underline, font.Strikeout, Convert.ToDouble(font.SizeInPoints), 0);
			this.PDFPAGE_SetRGBColor(((double)fontColor.R)/256.0
				,((double)fontColor.G)/256.0
				,((double)fontColor.B)/256.0);
			//this.PDFPAGE_SetRGBColor(100.0/256.0,72.0/256.0,131.0/256.0);
			this.PDFPAGE_TextOut(
				this.PDFPAGE_Width/2 - (this.PDFPAGE_GetTextWidth(title)/2)
				, (_headerHeight)/2 + _topMargin - font.SizeInPoints/2, 0, title);

			//footer line
			this.PDFPAGE_SetLineWidth(1);
			this.PDFPAGE_MoveTo(_leftMargin, this.WorkZoneBottom);
			this.PDFPAGE_LineTo(this.PDFPAGE_Width - _rightMargin, this.WorkZoneBottom);
			this.PDFPAGE_FillAndStroke();
			this.SwitchedToWatermark = false;

			return w;

		}

        private int GetWaterMark(bool leftImage , bool rightImage , string title){

			int w = -1,lImg,rImg;
			double coef;
            string pathPicture = string.Empty;

			if(this.PDFPAGE_Orientation == TxPDFPageOrientation.poPageLandscape){

				if (this.poLandScapeWaterMk<0){
					w = poLandScapeWaterMk = this.CreateWaterMark();
				}
				else
					return this.poLandScapeWaterMk;
			}

			if(this.PDFPAGE_Orientation == TxPDFPageOrientation.poPagePortrait){
				if (this.poPortraitWaterMk<0){
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
			if(leftImage){
				coef = 1.0;
                pathPicture = ((TNS.AdExpress.Domain.Theme.Picture)_style.GetTag("pictureLeft")).Path;
				lImg = this.AddImageFromFilename(pathPicture,TxImageCompressionType.itcFlate);
                Image lgImg = Image.FromFile(pathPicture);
				if(lgImg.Height> _headerHeight)
					coef = _headerHeight / lgImg.Height;
				this.PDFPAGE_ShowImage(lImg,_leftMargin,_topMargin,lgImg.Width*coef,lgImg.Height*coef,0);
			}

			//right image
			if(rightImage){
				coef = 1.0;
                pathPicture = ((TNS.AdExpress.Domain.Theme.Picture)_style.GetTag("pictureRight")).Path;
                rImg = this.AddImageFromFilename(pathPicture, TxImageCompressionType.itcFlate);
                Image rgImg = Image.FromFile(pathPicture);
				if(rgImg.Height> _headerHeight)
					coef = _headerHeight / rgImg.Height;
				this.PDFPAGE_ShowImage(rImg,this.PDFPAGE_Width - _rightMargin - (rgImg.Width*coef),_topMargin,rgImg.Width*coef,rgImg.Height*coef,0);
			}
			
			//title
            _style.GetTag("headerFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);

            double fontSize = ((TNS.AdExpress.Domain.Theme.Font)_style.GetTag("headerFont")).Size;
			this.PDFPAGE_TextOut(
				this.PDFPAGE_Width/2 - (this.PDFPAGE_GetTextWidth(title)/2),
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

	}
}
