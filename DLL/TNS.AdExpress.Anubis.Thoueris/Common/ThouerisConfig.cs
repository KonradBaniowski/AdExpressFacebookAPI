#region Information
//Author : Y. Rkaina
//Creation : 10/08/2006
#endregion

using System;
using System.Drawing;

using TNS.AdExpress.Anubis.Thoueris.DataAccess;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Thoueris.Common
{
	/// <summary>
    /// Static class containing every cofiguration elements for the ThouerisConfig Plug-in
	/// </summary>
	public class ThouerisConfig{

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

        #region Theme Properties
        /// Theme xml file Path
        /// </summary>
        private string _themePath;
        #endregion

        #region Scan Properties
        /// <summary>
        /// Press Scan xml file Path
        /// </summary>
        private string _pressScanPath;
        /// <summary>
        /// VMC Scan xml file Path
        /// </summary>
        private string _vmcScanPath;
        /// <summary>
        /// Outdoor Scan xml file Path
        /// </summary>
        private string _outdoorScanPath;
        #endregion

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
        public ThouerisConfig(IDataSource dataSource)
        {
			try{
                ThouerisConfigDataAccess.Load(dataSource, this);
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

        #region Theme Properties
        /// Get / Set Theme xml file Path
        /// </summary>
        public string ThemePath {
            get { return _themePath; }
            set { _themePath = value; }
        }
        #endregion

        #region Scan Properties
        /// Get / Set Press Scan xml file Path
        /// </summary>
        public string PressScanPath {
            get { return _pressScanPath; }
            set { _pressScanPath = value; }
        }
        /// Get / Set VMC Scan xml file Path
        /// </summary>
        public string VMCScanPath {
            get { return _vmcScanPath; }
            set { _vmcScanPath = value; }
        }
        /// Get / Set Outdoor Scan xml file Path
        /// </summary>
        public string OutdoorScanPath {
            get { return _outdoorScanPath; }
            set { _outdoorScanPath = value; }
        }
        #endregion
		
		#endregion

		#region Method
		#endregion

	}
}
