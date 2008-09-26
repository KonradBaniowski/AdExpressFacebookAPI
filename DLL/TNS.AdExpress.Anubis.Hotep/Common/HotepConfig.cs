#region Information
// Auteur Y. Rkaina
// Date de création: 10/07/2006
#endregion

using System;
using System.Drawing;

using TNS.AdExpress.Anubis.Hotep.DataAccess;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Hotep.Common{
	/// <summary>
	/// Static class containing every cofiguration elements for the Hotep Plug-in
	/// </summary>
	public class HotepConfig{

		#region Attributes

		#region Mail Properties
		/// <summary>
		/// Web Server
		/// </summary>
		private string _webServer = string.Empty;
		/// <summary>
		/// Serveur de mail d'envoie des résultats
		/// </summary>
		private string _customerMailServer;
		/// <summary>
		/// Port du serveur de mail d'envoie des résultats
		/// </summary>
		private int _customerMailPort=25;
		/// <summary>
		/// Mail d'envoie des résultats
		/// </summary>
		private string _customerMailFrom;
		/// <summary>
		/// Sujet des mails de résultats
		/// </summary>
		private string _customerMailSubject;
		/// <summary>
		/// Base path for storing PDF documents
		/// </summary>
		private string _pdfPath = "";
		/// <summary>
		/// Login for PDF Creator Pilot
		/// </summary>
		private string _pdfCreatorPilotLogin = "";
		/// <summary>
		/// Pass for PDF Creator Pilot
		/// </summary>
		private string _pdfCreatorPilotPass = "";
		/// <summary>
		/// Login for Html2Pdf
		/// </summary>
		private string _html2PdfLogin = "";
		/// <summary>
		/// Pass for Html2Pdf
		/// </summary>
		private string _html2PdfPass = "";
		#endregion

		#region Pdf Properties
		/// <summary>
		/// Pdf Author property
		/// </summary>
		private string _pdfAuthor = "";
		/// <summary>
		/// Pdf Subject property
		/// </summary>
		private string _pdfSubject = "";
		/// <summary>
		/// Pdf Title property
		/// </summary>
		private string _pdfTitle = "";
		/// <summary>
		/// Pdf Producer property
		/// </summary>
		private string _pdfProducer = "";
		/// <summary>
		/// Pdf KeyWords property
		/// </summary>
		private string _pdfKeyWords = "";
		#endregion

		#region Pdf Layout
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
		/// Size of the headers
		/// </summary>
		private double _headerHeight = 50;
		/// <summary>
		/// Size of the footers
		/// </summary>
		private double _footerHeight = 20;
		/// <summary>
		/// Header and footer font size
		/// </summary>
		private Font _headerFont = null;
		/// <summary>
		/// Default Font of the document
		/// </summary>
		private Font _defaultFont = null;
		/// <summary>
		/// Defualt font color of the document
		/// </summary>
		private Color _defaultFontColor = Color.Black;
		/// <summary>
		/// Size of the title fonty on the main page
		/// </summary>
		private Font _mainPageTitleFont = null;
		/// <summary>
		/// Size of the default font on the main page
		/// </summary>
		private Font _mainPageDefaultFont = null;
		/// <summary>
		/// Default font color on the main page
		/// </summary>
		private Color _mainPageFontColor = Color.Black;
		/// <summary>
		/// header font color on the main page
		/// </summary>
		private Color _headerFontColor = Color.Black;
		/// <summary>
		/// title font color on the main page
		/// </summary>
		private Color _titleFontColor = Color.Black;
		/// <summary>
		/// Main page title font color on the main page
		/// </summary>
		private Color _mainPageTitleFontColor = Color.Black;
		/// <summary>
		/// Titles font
		/// </summary>
		private Font _titleFont = null;
		#endregion

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		public HotepConfig(IDataSource dataSource){
			try{
				HotepConfigDataAccess.Load(dataSource,this);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

		#region Accesseurs

		#region Mail properties
		/// <summary>
		/// Get / Set Login for PDF Creator libraries (both PDFCreatorPIlot and HTML2PDF Add on
		/// </summary>
		public string PdfCreatorPilotLogin{
			get{return _pdfCreatorPilotLogin;}
			set{_pdfCreatorPilotLogin = value;}
		}
		/// <summary>
		/// Get / Set Pass for PDF Creator Pilot
		/// </summary>
		public string PdfCreatorPilotPass{
			get{return _pdfCreatorPilotPass;}
			set{_pdfCreatorPilotPass = value;}
		}
		/// <summary>
		/// Get / Set Pass for HTML2PDF Add_on library
		/// </summary>
		public string Html2PdfPass{
			get{return _html2PdfPass;}
			set{_html2PdfPass = value;}
		}
		/// <summary>
		/// Get / Set Login for HTML2PDF Add_on library
		/// </summary>
		public string Html2PdfLogin{
			get{return _html2PdfLogin;}
			set{_html2PdfLogin = value;}
		}
		/// <summary>
		/// Obtient le serveur web de résultats
		/// </summary>
		public string WebServer{
			get{return _webServer;}
			set{_webServer = value;}
		}
		/// <summary>
		/// Obtient le serveur des mails de résultats
		/// </summary>
		public string CustomerMailServer{
			get{return _customerMailServer;}
			set{_customerMailServer = value;}
		}
		/// <summary>
		/// Obtient le port du serveur des mails des résultats
		/// </summary>
		public int CustomerMailPort{
			get{return _customerMailPort;}
			set{_customerMailPort = value;}
		}
		/// <summary>
		/// Obtient le mail d'envoie des résultats 
		/// </summary>
		public string CustomerMailFrom{
			get{return _customerMailFrom;}
			set{_customerMailFrom = value;}
		}
		/// <summary>
		/// Obtient le sujet des mails de résultat
		/// </summary>
		public string CustomerMailSubject{
			get{return _customerMailSubject;}
			set{_customerMailSubject = value;}
		}
		/// <summary>
		/// Get / Set the base path for PDF documents
		/// </summary>
		public string PdfPath{
			get{return _pdfPath;}
			set{_pdfPath = value;}
		}
		#endregion

		#region Pdf Properties
		/// <summary>
		/// Get / Set the PDF author
		/// </summary>
		public string PdfAuthor{
			get{return _pdfAuthor;}
			set{_pdfAuthor = value;}
		}
		/// <summary>
		/// Get / Set the PDF subject
		/// </summary>
		public string PdfSubject{
			get{return _pdfSubject;}
			set{_pdfSubject = value;}
		}
		/// <summary>
		/// Get / Set the PDF title
		/// </summary>
		public string PdfTitle{
			get{return _pdfTitle;}
			set{_pdfTitle = value;}
		}
		/// <summary>
		/// Get / Set the PDF producer
		/// </summary>
		public string PdfProducer{
			get{return _pdfProducer;}
			set{_pdfProducer = value;}
		}
		/// <summary>
		/// Get / Set the PDF keywords
		/// </summary>
		public string PdfKeyWords{
			get{return _pdfKeyWords;}
			set{_pdfKeyWords = value;}
		}
		#endregion

		#region PdfLayout
		/// <summary>
		/// Get the size of the left margin
		/// </summary>
		public double LeftMargin{
			get{return _leftMargin;}
			set{_leftMargin=value;}
		}
		/// <summary>
		/// Get the size of the right margin
		/// </summary>
		public double RightMargin{
			get{return _rightMargin;}
			set{_rightMargin=value;}
		}
		/// <summary>
		/// Get the size of the top margin
		/// </summary>
		public double TopMargin{
			get{return _topMargin;}
			set{_topMargin=value;}
		}
		/// <summary>
		/// Get the size of the bottom margin
		/// </summary>
		public double BottomMargin{
			get{return _bottomMargin;}
			set{_bottomMargin=value;}
		}
		/// <summary>
		/// Get the height of the header
		/// </summary>
		public double HeaderHeight{
			get{return _headerHeight;}
			set{_headerHeight=value;}
		}
		/// <summary>
		/// Get the height of the footer
		/// </summary>
		public double FooterHeight{
			get{return _footerHeight;}
			set{_footerHeight=value;}
		}

		/// <summary>
		/// Get/Set the font of the headers
		/// </summary>
		public Font HeaderFont{
			get{return _headerFont;}
			set{_headerFont = value;}
		}
		/// <summary>
		/// Get / Set the title font on the main page
		/// </summary>
		public Font MainPageTitleFont{
			get{return _mainPageTitleFont;}
			set{_mainPageTitleFont = value;}
		}
		/// <summary>
		/// Get / Set the default font on the main page
		/// </summary>
		public Font MainPageDefaultFont{
			get{return _mainPageDefaultFont;}
			set{_mainPageDefaultFont = value;}
		}
		/// <summary>
		/// Get / Set the default font for the document
		/// </summary>
		public Font DefaultFont{
			get{return _defaultFont;}
			set{_defaultFont = value;}
		}
		/// <summary>
		/// Get / Set the titles font for the document
		/// </summary>
		public Font TitleFont{
			get{return _titleFont;}
			set{_titleFont = value;}
		}
		/// <summary>
		/// Get / Set the font color for the document
		/// </summary>
		public Color DefaultFontColor{
			get{return _defaultFontColor;}
			set{_defaultFontColor = value;}
		}
		/// <summary>
		/// Get / Set the font color for the main page
		/// </summary>
		public Color MainPageFontColor{
			get{return _mainPageFontColor;}
			set{_mainPageFontColor = value;}
		}
		/// <summary>
		/// Get / Set the title font color for the document
		/// </summary>
		public Color TitleFontColor{
			get{return _titleFontColor;}
			set{_titleFontColor = value;}
		}
		/// <summary>
		/// Get / Set the title font color for the main page
		/// </summary>
		public Color MainPageTitleFontColor{
			get{return _mainPageTitleFontColor;}
			set{_mainPageTitleFontColor = value;}
		}
		/// <summary>
		/// Get / Set the font color for the headers
		/// </summary>
		public Color HeaderFontColor{
			get{return _headerFontColor;}
			set{_headerFontColor = value;}
		}

		/// <summary>
		/// Set the font color for the document
		/// </summary>
		public void SetDefaultFontColor(string color){
			_defaultFontColor = GetColor(color);
		}
		/// <summary>
		/// Set the font color for the main Page
		/// </summary>
		public void SetMainPageFontColor(string color){
			_mainPageFontColor = GetColor(color);
		}
		/// <summary>
		/// Set the title font color for the main Page
		/// </summary>
		public void SetMainPageTitleFontColor(string color){
			_mainPageTitleFontColor = GetColor(color);
		}
		/// <summary>
		/// Set the titles font color for the document
		/// </summary>
		public void SetTitleFontColor(string color){
			_titleFontColor = GetColor(color);
		}
		/// <summary>
		/// Set the headers font color for the document
		/// </summary>
		public void SetHeadersFontColor(string color){
			_headerFontColor = GetColor(color);
		}

		private Color GetColor(string s){
			if(s==null)throw (new ArgumentNullException("Argument 'R,G,B' can not be null."));
			if(s==string.Empty)throw (new ArgumentException("Argument 'R,G,B' can not be empty."));
			string[] colors = s.Split(',');
			if(colors.Length != 3 )throw (new ArgumentException("Argument 'R,G,B' has not the valid number of colors."));
			return Color.FromArgb(int.Parse(colors[0]),int.Parse(colors[1]),int.Parse(colors[2]));
		}
		#endregion
		
		#endregion

	}
}
