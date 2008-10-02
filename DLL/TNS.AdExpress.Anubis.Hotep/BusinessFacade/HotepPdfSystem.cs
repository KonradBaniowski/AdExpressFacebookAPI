#region Information
//Author : Y. Rkaina 
//Creation : 11/07/2006
#endregion

#region Using
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;
using System.Windows.Forms;

using TNS.AdExpress.Anubis.Hotep.Common;
using TNS.AdExpress.Anubis.Hotep.Exceptions;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;
using TNS.AdExpress.Anubis.Hotep.UI;
using TNSAnubisConstantes=TNS.AdExpress.Anubis.Constantes;

using TNS.AdExpress.Constantes.Customer;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using CstWeb = TNS.AdExpress.Constantes.Web;

using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.Exceptions;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using Dundas.Charting.WinControl;
using HTML2PDFAddOn;
using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;
using HtmlSnap2;
using System.Globalization;
using Oracle.DataAccess.Client;

using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpressI.ProductClassIndicators;
using TNS.AdExpressI.ProductClassIndicators.DAL;
using TNS.AdExpressI.ProductClassIndicators.Engines;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpress.Anubis.Hotep.BusinessFacade{
	/// <summary>
	/// Generate the PDF document for Indicateurs module.
	/// </summary>
	public class HotepPdfSystem : Pdf{

		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Appm Configuration (usefull for PDF layout)
		/// </summary>
		private HotepConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Liste des indicateurs (présents dans le PDF)
		/// </summary>
		private ArrayList _titleList = null;
        /// <summary>
        /// Product Class Indicator
        /// </summary>
        private IProductClassIndicators _productClassIndicator;
        /// <summary>
        /// Product Class Indicator DAL
        /// </summary>
        private IProductClassIndicatorsDAL _productClassIndicatorDAL;
		#endregion

		#region Constructeur
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSource">DataSource</param>
        /// <param name="config">Config</param>
        /// <param name="rqDetails">Request details</param>
        /// <param name="webSession">Web session</param>
		public HotepPdfSystem(IDataSource dataSource, HotepConfig config, DataRow rqDetails, WebSession webSession):
			base(config.LeftMargin, config.RightMargin, config.TopMargin, config.BottomMargin,
			config.HeaderHeight,config.FooterHeight){
			this._dataSource = dataSource;
			this._config = config;
			this._rqDetails = rqDetails;
			this._webSession = webSession;
            this._titleList = new ArrayList();
            WebNavigation.Module module = WebNavigation.ModulesList.GetModule(CstWeb.Module.Name.INDICATEUR);
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Indicator result"));
            object[] param = new object[1] { webSession };
            _productClassIndicator = (IProductClassIndicators)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            _productClassIndicator.Pdf = true;
            _productClassIndicatorDAL = (IProductClassIndicatorsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
		}
		#endregion

		#region Init
        /// <summary>
        /// Init
        /// </summary>
        /// <returns>File name</returns>
		internal string Init(){
			try{
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);
				
                base.Init(true,fName,_config.PdfCreatorPilotLogin,_config.PdfCreatorPilotPass);
				this.DocumentInfo_Creator = this.DocumentInfo_Author = _config.PdfAuthor;
				this.DocumentInfo_Subject = _config.PdfSubject;
				this.DocumentInfo_Title = _config.PdfTitle;
				this.DocumentInfo_Producer = _config.PdfProducer;
				this.DocumentInfo_Keywords = _config.PdfKeyWords;
				return shortFName;
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

		#region Fill
        /// <summary>
        /// Fill
        /// </summary>
		internal void Fill(){

			try{

				#region Session Parameters
				SessionParameter();
				#endregion

				#region IndicatorSynthesis
				IndicatorSynthesis();
				#endregion

				#region IndicatorSeasonality
				IndicatorSeasonality();
				#endregion

				#region Indicator Media Strategy
				IndicatorMediaStrategy();
				#endregion

				#region IndicatorPalmares (advertiserChart)
				IndicatorPalmares(CstResult.PalmaresRecap.ElementType.advertiser);
				#endregion

				#region IndicatorPalmares (referenceChart)
				IndicatorPalmares(CstResult.PalmaresRecap.ElementType.product);
				#endregion

				#region IndicatorEvolution (advertiserChart)
				IndicatorEvolution(CstResult.EvolutionRecap.ElementType.advertiser);
				#endregion

				#region IndicatorEvolution (referenceChart)
				IndicatorEvolution(CstResult.EvolutionRecap.ElementType.product);
				#endregion

				#region Indicator Novelty
				IndicatorNovelty();
				#endregion

				#region MainPage
				MainPageDesign();
				#endregion

				#region Header and Footer
                string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

				this.AddHeadersAndFooters(
                _webSession,
				@"Images\Common\logo_Tns.bmp",
				imagePosition.leftImage,
                GestionWeb.GetWebWord(1053, _webSession.SiteLanguage) + " - " + dateString,
				0,-1,_config.HeaderFontColor,_config.HeaderFont,true,_webSession); 
				#endregion
				
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

		#region Send
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="fileName">File name</param>
		internal void Send(string fileName){
			ArrayList to = new ArrayList();
			foreach(string s in _webSession.EmailRecipient){
				to.Add(s);
			}
			SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
				Text.SuppressAccent(GestionWeb.GetWebWord(1971,_webSession.SiteLanguage)),
				Text.SuppressAccent(GestionWeb.GetWebWord(1750,_webSession.SiteLanguage)+"\""+_webSession.ExportedPDFFileName
				+ "\""+String.Format(GestionWeb.GetWebWord(1751,_webSession.SiteLanguage),_config.WebServer)
				+ "<br><br>"
				+ GestionWeb.GetWebWord(1776,_webSession.SiteLanguage)),
				true, _config.CustomerMailServer, _config.CustomerMailPort);
			mail.mailKoHandler += new TNS.FrameWork.Net.Mail.SmtpUtilities.mailKoEventHandler(mail_mailKoHandler);
			mail.SendWithoutThread(false);
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
		private string GetFileName(DataRow rqDetails,ref string shortName){
			string pdfFileName;

			try{

				pdfFileName = this._config.PdfPath;
				pdfFileName += @"\" + rqDetails["ID_LOGIN"].ToString() ;

				if(!Directory.Exists(pdfFileName)){
					Directory.CreateDirectory(pdfFileName);
				}
				shortName = DateTime.Now.ToString("yyyyMMdd_") 
					+ rqDetails["id_static_nav_session"].ToString()
					+ "_"
					+ Functions.GetRandomString(30,40);

				pdfFileName += @"\" + shortName + ".pdf";

				string checkPath = 	Regex.Replace(pdfFileName, @"(\.pdf)+", ".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);


				int i = 0;
				while(File.Exists(checkPath)){
					if(i<=1){
						checkPath = Regex.Replace(pdfFileName, @"(\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					else{
						checkPath = Regex.Replace(pdfFileName, "(_"+i+@"\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					i++;
				}
				return checkPath;
			}
			catch(System.Exception e){
				throw(new HotepPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
			}
		}
		#endregion

		#region GetWorkDirectory
		/// <summary>
		/// Provide a work directory. Create it if it doesn't exist or return a path to it
		/// </summary>
		/// <returns></returns>
		internal string GetWorkDirectory(){
			try{
				if(!Directory.Exists(@"tmp_"+_rqDetails["id_static_nav_session"].ToString())){
					Directory.CreateDirectory(@"tmp_"+_rqDetails["id_static_nav_session"].ToString());
				}
				return (@"tmp_"+_rqDetails["id_static_nav_session"].ToString());
			}
			catch(System.Exception e){
				throw(new HotepPdfException("Unable to provide the working directory", e));
			}
		}
		#endregion

		#endregion

		#region MainPage
		/// <summary>
		/// Design Main Page
		/// </summary>
		/// <returns></returns>
		private void MainPageDesign(){

            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

			this.SetCurrentPage(0);

			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;
	
			string imgPath = @"Images\" + _webSession.SiteLanguage + @"\LogoAdExpress220.jpg";
			Image imgG = Image.FromFile(imgPath);
			
			double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
			double coef = Math.Min((double)1.0,w);
			w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
			coef = Math.Min((double)coef, w);
			
			int imgI = this.AddImageFromFilename(imgPath,TxImageCompressionType.itcFlate);

			StreamWriter sw = null;
			string workFile = GetWorkDirectory() + @"\SessionParameter" + _rqDetails["id_static_nav_session"].ToString() + ".htm";
			string str = "";

            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

			#region headers
			sw = File.CreateText(workFile);
			sw.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
			sw.WriteLine("<HTML>");
			sw.WriteLine("<HEAD>");
            sw.WriteLine("<META http-equiv=\"Content-Type\" content=\"text/html; charset=" + charSet + "\">");
			sw.WriteLine("<meta content=\"Microsoft Visual Studio .NET 7.1\" name=\"GENERATOR\">");
			sw.WriteLine("<meta content=\"C#\" name=\"CODE_LANGUAGE\">");
			sw.WriteLine("<meta content=\"JavaScript\" name=\"vs_defaultClientScript\">");
			sw.WriteLine("<meta content=\"http://schemas.microsoft.com/intellisense/ie5\" name=\"vs_targetSchema\">");
            sw.WriteLine("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/AdExpress.css\" type=\"text/css\" rel=\"stylesheet\">");
            sw.WriteLine("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/GenericUI.css\" type=\"text/css\" rel=\"stylesheet\">");
            sw.WriteLine("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/MediaSchedule.css\" type=\"text/css\" rel=\"stylesheet\">");
			sw.WriteLine("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
			sw.WriteLine("<meta http-equiv=\"expires\" content=\"0\">");
			sw.WriteLine("<meta http-equiv=\"pragma\" content=\"no-cache\">");
			sw.WriteLine("<meta name=\"Cache-control\" content=\"no-cache\">");
			sw.WriteLine("</HEAD>");
			sw.WriteLine("<body>");
			sw.WriteLine("<form style=\"WIDTH: 300px; HEIGHT: 75px\">");
			#endregion

			#region result
			sw.WriteLine("<TABLE id=\"Table1\" align=\"center\" cellSpacing=\"1\" cellPadding=\"1\" width=\"1100\" border=\"0\" style=\"WIDTH: 1100px; HEIGHT: 2px\">");
			sw.WriteLine("<P>&nbsp;</P>");
			sw.WriteLine("<P>&nbsp;</P>");
			sw.WriteLine("<P>&nbsp;</P>");
			sw.WriteLine("<TD style=\"HEIGHT: 43px\">");
            sw.WriteLine("<P class=\"MsoNormal\" style=\"mso-layout-grid-align: none\" align=\"center\"><B><SPAN style=\"FONT-SIZE: 40pt; COLOR: #644883; FONT-FAMILY: 'Arial Bold'; mso-bidi-font-family: 'Arial Bold'\">" + GestionWeb.GetWebWord(1053, _webSession.SiteLanguage) + "</SPAN></B><SPAN style=\"FONT-SIZE: 10pt; COLOR: black; FONT-FAMILY: 'Arial Bold'; mso-bidi-font-family: 'Arial Bold'\">");
			sw.WriteLine("<o:p></o:p></SPAN></P>");
			sw.WriteLine("</TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<TR>");
			sw.WriteLine("<TD><P class=\"MsoNormal\" style=\"mso-layout-grid-align: none\" align=\"center\"><SPAN style=\"COLOR: #644883; FONT-FAMILY: Arial\">"+ Convertion.ToHtmlString(str) +"</SPAN></P></TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<P>&nbsp;</P>");
			sw.WriteLine("<P>&nbsp;</P>");
			sw.WriteLine("<TR>");
			sw.WriteLine("<TD>&nbsp;</TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<TR>");
			sw.WriteLine("<TD>&nbsp;</TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<TR>");
			sw.WriteLine("<TD align=\"center\">");
			sw.WriteLine("<TABLE  class=\"txtViolet11Bold\" id=\"Table1\" style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" cellSpacing=\"0\" cellPadding=\"0\" width=\"600\" align=\"center\">");
			sw.WriteLine("<TR>");
			sw.WriteLine("<TD colSpan=\"2\" style=\"BORDER-BOTTOM: #644883 1px solid;\">"+ Convertion.ToHtmlString(GestionWeb.GetWebWord(1979,_webSession.SiteLanguage)) +"</TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<TR>");
			sw.WriteLine("<TD>&nbsp;</TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<TR>");
			int line=2;
			foreach(string indicatorName in _titleList){
				if((line%2==0)&&(line>2)){
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR>");
				}
				sw.WriteLine("<TD><li style=\"LIST-STYLE-TYPE: square\">"+indicatorName+"</li></TD>");
				line++;
			}
			sw.WriteLine("</TR>");
			sw.WriteLine("</TABLE>");
			sw.WriteLine("</FIELDSET>");

			sw.WriteLine("</TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("</TABLE>");
			sw.WriteLine("</form>");
			sw.WriteLine("</body>");		

			#region Html file loading
			Functions.CloseHtmlFile(sw);
			HTML2PDFClass html = new HTML2PDFClass();
			html.MarginLeft = Convert.ToInt32(this.LeftMargin);
			html.MarginTop = Convert.ToInt32(this.WorkZoneTop);
			html.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
			html.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
			html.ConnectToPDFLibrary (this);
			html.LoadFromFile(workFile);
			html.ConvertAll();
			html.ClearCache();
			html.ConvertAll();
			html.DisconnectFromPDFLibrary ();
			#endregion

			#region Clean File
			File.Delete(workFile);
			#endregion

			this.PDFPAGE_ShowImage(imgI,
				(double)(this.PDFPAGE_Width/2 - coef*imgG.Width/2), (double)(this.WorkZoneBottom - coef*imgG.Height - 50),
				(double)(coef*imgG.Width),(double)(coef*imgG.Height),0);

			#endregion

		}
		#endregion

		#region SessionParameter
		/// <summary>
		/// Session parameter design
		/// </summary>
		private void SessionParameter(){

			StreamWriter sw = null;
			int nbLinesEnd=0;
			int j=0;
			bool showProductSelection = false;
			IList nbLinesSelectionMedia=new ArrayList(), nbLinesSelectionDetailMedia=new ArrayList(), nbLinesSelectionProduct=new ArrayList();

			try{
				this.NewPage();

				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				string workFile = GetWorkDirectory() + @"\SessionParameter" + _rqDetails["id_static_nav_session"].ToString() + ".htm";

                sw = Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

				IList SelectionMedia = ToHtml(_webSession.SelectionUniversMedia,false,false,600,false,_webSession.SiteLanguage,2,1,true,20,ref nbLinesEnd,ref nbLinesSelectionMedia);
				nbLinesEnd=27;

				IList SelectionDetailMedia =ToHtml((TreeNode)_webSession.SelectionUniversMedia.FirstNode,true,true,600,true,_webSession.SiteLanguage,3,1,false,21,ref nbLinesEnd,ref nbLinesSelectionDetailMedia);
				if(nbLinesSelectionDetailMedia.Count==0)
					nbLinesEnd=21;
				else if(nbLinesSelectionDetailMedia.Count==1){
					nbLinesEnd=17-(int)nbLinesSelectionDetailMedia[0];
					if(nbLinesEnd<0)
						nbLinesEnd=28;
				}
				else
					nbLinesEnd=27-(int)nbLinesSelectionDetailMedia[nbLinesSelectionDetailMedia.Count-1];

				#region Title
				sw.WriteLine("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD style=\"font-family: Arial, Helvetica, sans-serif; font-size: 20px; color: #644883; font-weight: bold;\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752,_webSession.SiteLanguage)) + "</TD>");
				sw.WriteLine("</TR>");
				#endregion

				#region Etude comparative
				if(_webSession.ComparativeStudy){
					sw.WriteLine("<TR height=\"7\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR>");
					sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1118, _webSession.SiteLanguage)) + " </TD>");
					sw.WriteLine("</TR>");
				}
				#endregion

				#region Period
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1755, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"20\">");
				sw.WriteLine("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
					+ HtmlFunctions.GetPeriodDetail(_webSession)
					+ "</TD>");
				sw.WriteLine("</TR>");
				#endregion

				#region Média
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1292, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD align=\"left\">");
				sw.WriteLine(SelectionMedia[0].ToString());
				sw.WriteLine("</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("</TR>");
				#endregion

				if (SelectionDetailMedia !=null && SelectionDetailMedia.Count > 0) {

					#region Détail Média
				    mediaDetail(ref sw,SelectionDetailMedia,true,0);
					for(j=1; j<SelectionDetailMedia.Count; j++){

						#region Html file loading
						htmlFileLoading(sw,workFile);
						#endregion

						this.NewPage();

						this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                        sw = Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

						#region Détail Média
						mediaDetail(ref sw,SelectionDetailMedia,false,j);
						#endregion
					}
					#endregion

				}

				#region Unité
				if (SelectionDetailMedia == null || SelectionDetailMedia.Count == 0) {
					sw.WriteLine("<TR height=\"7\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR>");
					sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1795, _webSession.SiteLanguage)) + " :</TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR height=\"20\">");
					sw.WriteLine("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
						+ GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[_webSession.Unit], _webSession.SiteLanguage)
						+ "</TD>");
					sw.WriteLine("</TR>");
				}
				#endregion
				
				#region produit
				// Détail produit
				if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0) {
					int nbLineByPage = 43;
					int currentLine = 13;
					if (_webSession.ComparativeStudy) currentLine = currentLine + 3;
					if (SelectionDetailMedia == null || SelectionDetailMedia.Count == 0) currentLine = currentLine + 4;//6
					if (SelectionDetailMedia != null && SelectionDetailMedia.Count > 0) {
						currentLine = 3;
						nbLineByPage = 34;
						#region Html file loading
						htmlFileLoading(sw, workFile);
						#endregion

						this.NewPage();

						this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

                        sw = Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);
						sw.WriteLine("<TABLE id=\"Table1\" align=\"center\" cellSpacing=\"1\" cellPadding=\"1\" width=\"1100\" border=\"0\" style=\"WIDTH: 1100px; HEIGHT: 2px\">");

					}
					sw.WriteLine("<TR height=\"7\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR>");
					sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
					sw.WriteLine("<TR><TD>&nbsp;</TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR>");
					sw.WriteLine("<TD align=\"center\">");

                    sw.WriteLine(Convertion.ToHtmlString(TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.Source, 600, true, nbLineByPage, ref currentLine)));
					showProductSelection = true;

					if (showProductSelection) {
						sw.WriteLine("<br>");
						sw.WriteLine("<div class=\"txtViolet11Bold\" align=\"left\" >" + "&nbsp;&nbsp;" + GestionWeb.GetWebWord(1601, _webSession.SiteLanguage) + "</div>");
						sw.WriteLine(TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetSectorsSelected(_webSession, false));
					}
					sw.WriteLine("</TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("</TR>");

				}
				#endregion

				#region Unité
				if (SelectionDetailMedia != null && SelectionDetailMedia.Count > 0) {
					sw.WriteLine("<TR height=\"7\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
					sw.WriteLine("<TD></TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR>");
					sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1795, _webSession.SiteLanguage)) + " :</TD>");
					sw.WriteLine("</TR>");
					sw.WriteLine("<TR height=\"20\">");
					sw.WriteLine("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
						+ GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[_webSession.Unit], _webSession.SiteLanguage)
						+ "</TD>");
					sw.WriteLine("</TR>");
				}
				#endregion

				#region Html file loading
				sw.WriteLine("</TABLE>");
				htmlFileLoading(sw,workFile);
				#endregion

				_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1752,_webSession.SiteLanguage)));
			
			}
			catch(System.Exception e){
				try{
					sw.Close();
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException("Unable to build the session parameter page for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e));
			}
		}
		#endregion

		#region Indicator Synthesis (similar to AdExpress)
		/// <summary>
		/// Indicator Synthesis design
		/// </summary>
		private void IndicatorSynthesis(){

			StreamWriter sw = null;

			try{
				this.NewPage();

				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				string workFile = GetWorkDirectory() + @"\Campaign_" + _rqDetails["id_static_nav_session"].ToString() + ".htm";

                sw = Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

				#region Title
				sw.WriteLine("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD style=\"font-family: Arial, Helvetica, sans-serif; font-size: 20px; color: #644883; font-weight: bold;\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1664,_webSession.SiteLanguage)) + "</TD>");
				sw.WriteLine("</TR>");
				#endregion
				
				#region result
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR align=\"center\"><td>");
                sw.WriteLine(_productClassIndicator.GetSummary());
				sw.WriteLine("</td></tr>");
				#endregion

				#region Html file loading
				sw.WriteLine("</table>");
				Functions.CloseHtmlFile(sw);
				HTML2PDFClass html = new HTML2PDFClass();
				html.MarginLeft = Convert.ToInt32(this.LeftMargin);
				html.MarginTop = Convert.ToInt32(this.WorkZoneTop);
				html.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
				html.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
				html.ConnectToPDFLibrary (this);
				html.LoadFromFile(workFile);
				html.ConvertAll();
				html.ClearCache();
				html.ConvertAll();
				html.DisconnectFromPDFLibrary ();
				#endregion

				#region Clean File
				File.Delete(workFile);
				#endregion

				_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1664,_webSession.SiteLanguage)));
			
			}
			catch(System.Exception e){
				try{
					sw.Close();
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException("Unable to process the synthesis result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e)); 
			}
		}
		#endregion

		#region Indicator Seasonality
		/// <summary>
		/// Graphiques Indicator Seasonality
		/// </summary>
		private void IndicatorSeasonality(){

			StreamWriter sw = null;
			Image img = null;
			object[,] tab=null;
            EngineSeasonality engine = new EngineSeasonality(_webSession, _productClassIndicatorDAL);
		
			#region Load Data
			if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal){
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                tab = engine.GetChartData();
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;}
			else{
                tab = engine.GetChartData();
			}
			#endregion

			try{
				if(tab.Length!=0){

					this.NewPage();

					this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

					string workFile = GetWorkDirectory() + @"\Grp_" + _rqDetails["id_static_nav_session"]+ ".bmp";

					#region Title
					this.PDFPAGE_SetActiveFont(_config.TitleFont.Name, _config.TitleFont.Bold,_config.TitleFont.Italic,
						_config.TitleFont.Underline, _config.TitleFont.Strikeout, 16, 0);
					this.PDFPAGE_SetRGBColor(((double)_config.TitleFontColor.R)/256.0
						,((double)_config.TitleFontColor.G)/256.0
						,((double)_config.TitleFontColor.B)/256.0);
					this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1139 ,_webSession.SiteLanguage));
					#endregion

					#region GRP graph
			
					UISeasonalityGraph graph = new UISeasonalityGraph(_webSession, _dataSource, _config, tab);
					graph.BuildSeasonality();
					graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
					img = Image.FromFile(workFile);
					double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
					coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
					int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
					this.PDFPAGE_ShowImage(i, 
						(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
						this.PDFPAGE_Height/2 - (coef * img.Height / 2),
						coef * img.Width,
						coef * img.Height,
						0);
					img.Dispose();
					img = null;
					graph.Dispose();
					graph = null;
			
					#region Clean File
					File.Delete(workFile);
					#endregion
				
					#endregion

					_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1139,_webSession.SiteLanguage)));

				}

			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException("impossible de générer les graphiques Indicator Seasonality.",ex)); 
			}
			
		}
		#endregion

		#region Indicator Palmares
		/// <summary>
		/// Graphiques Indicator Palmares
		/// </summary>
		private void IndicatorPalmares(FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType){

			StreamWriter sw = null;
			Image img = null;
			bool isPresent=false;
		
			#region Load Data
            object[,] tab = null;
            EngineTop engine = new EngineTop(_webSession, _productClassIndicatorDAL);
            tab = engine.GetData(CstResult.PalmaresRecap.typeYearSelected.currentYear, tableType);
			#endregion

			try{
				if(tab.GetLongLength(0)!=1 && double.Parse(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString())!=0){

					this.NewPage();

					this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

					string workFile = GetWorkDirectory() + @"\Grp_" + _rqDetails["id_static_nav_session"]+ ".bmp";

					#region Title
					this.PDFPAGE_SetActiveFont(_config.TitleFont.Name, _config.TitleFont.Bold,_config.TitleFont.Italic,
						_config.TitleFont.Underline, _config.TitleFont.Strikeout, 16, 0);
					this.PDFPAGE_SetRGBColor(((double)_config.TitleFontColor.R)/256.0
						,((double)_config.TitleFontColor.G)/256.0
						,((double)_config.TitleFontColor.B)/256.0);
					this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1980 ,_webSession.SiteLanguage));
					#endregion

					#region GRP graph
			
					UIPalmaresGraph graph = new UIPalmaresGraph(_webSession, _dataSource, _config, tab);
					graph.BuildPalmares(tableType);
					graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
					img = Image.FromFile(workFile);
					double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
					coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
					int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
					this.PDFPAGE_ShowImage(i, 
						(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
						this.PDFPAGE_Height/2 - (coef * img.Height / 2),
						coef * img.Width,
						coef * img.Height,
						0);
					img.Dispose();
					img = null;
					graph.Dispose();
					graph = null;
			
			
					#region Clean File
					File.Delete(workFile);
					#endregion
				
					#endregion

					foreach(string indicatorName in _titleList){
						if(indicatorName.Equals(Convertion.ToHtmlString(GestionWeb.GetWebWord(1165,_webSession.SiteLanguage))))
							isPresent=true;
					}
					if(!isPresent)
					_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1165,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException("impossible de générer les graphiques Indicator Palmares.",ex)); 
			}
			
		}
		#endregion

		#region Indicator Novelty (similar to AdExpress)
		/// <summary>
		/// Indicator Novelty design
		/// </summary>
		private void IndicatorNovelty(){
			
			int verifResult=0;					

			try{
				
				IList result = new ArrayList(); 
				IList resultAdvertiser = new ArrayList(); 

				#region result
                EngineNovelty engine = new EngineNovelty(_webSession, _productClassIndicatorDAL);
                engine.ClassifLevel = CstResult.Novelty.ElementType.product;
                result = GetIndicatorNoveltyGraphicHtmlUI(engine.GetData(), _webSession, CstResult.Novelty.ElementType.product);
                engine.ClassifLevel = CstResult.Novelty.ElementType.advertiser;
                resultAdvertiser = GetIndicatorNoveltyGraphicHtmlUI(engine.GetData(), _webSession, CstResult.Novelty.ElementType.advertiser);
				#endregion

				#region Html file loading
				if(!((((string)result[0]).Length<30) || result[0].Equals("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(1224,_webSession.SiteLanguage)+"</div>"))){
					for(int i=0;i<result.Count;i++){
						if(i==0)
							addNoveltyPicture(result,i,true);
						else
							addNoveltyPicture(result,i,false);
					}
					verifResult++;
				}

				if(!(((string)resultAdvertiser[0]).Length<50 || resultAdvertiser[0].Equals("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(1224,_webSession.SiteLanguage)+"</div>"))){
					for(int i=0;i<resultAdvertiser.Count;i++){
						if((i==0)&&(verifResult==0))
							addNoveltyPicture(resultAdvertiser,i,true);
						else
							addNoveltyPicture(resultAdvertiser,i,false);
					}
					verifResult++;
				}
				
				if(verifResult >= 1){
				_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1197,_webSession.SiteLanguage)));
				}
				#endregion
		
			}
			catch(System.Exception e){
				throw(new HotepPdfException("Unable to process the Indicator Novelty result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e)); 
			}
		}
		#endregion

		#region Indicator Evolution
		/// <summary>
		/// Graphiques Indicator Evolution
		/// </summary>
		private void IndicatorEvolution(FrameWorkConstantes.Results.EvolutionRecap.ElementType tableType){

			StreamWriter sw = null;
			Image img = null;
			bool isPresent=false;
		
			#region Load Data
            object[,] tab = null;
            EngineEvolution engine = new EngineEvolution(_webSession, _productClassIndicatorDAL);
            tab = engine.GetData(tableType);
			#endregion

			#region Cas année N-2
			DateTime PeriodBeginningDate = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
			#endregion

			try{
				if((tab.GetLongLength(0)!=0)&&(!((PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-2) && DateTime.Now.Year<=_webSession.DownLoadDate)
					|| (PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-3) && DateTime.Now.Year>_webSession.DownLoadDate)))){

					this.NewPage();

					this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

					string workFile = GetWorkDirectory() + @"\Grp_" + _rqDetails["id_static_nav_session"]+ ".bmp";

					#region Title
					this.PDFPAGE_SetActiveFont(_config.TitleFont.Name, _config.TitleFont.Bold,_config.TitleFont.Italic,
						_config.TitleFont.Underline, _config.TitleFont.Strikeout, 16, 0);
					this.PDFPAGE_SetRGBColor(((double)_config.TitleFontColor.R)/256.0
						,((double)_config.TitleFontColor.G)/256.0
						,((double)_config.TitleFontColor.B)/256.0);
					this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1207 ,_webSession.SiteLanguage));
					#endregion

					#region GRP graph
			
					UIEvolutionGraph graph = new UIEvolutionGraph(_webSession, _dataSource, _config, tab);
					graph.BuildEvolution(tableType);
					graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
					img = Image.FromFile(workFile);
					double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
					coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
					int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
					this.PDFPAGE_ShowImage(i, 
						(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
						this.PDFPAGE_Height/2 - (coef * img.Height / 2),
						coef * img.Width,
						coef * img.Height,
						0);
					img.Dispose();
					img = null;
					graph.Dispose();
					graph = null;
			
			
					#region Clean File
					File.Delete(workFile);
					#endregion
				
					#endregion

					foreach(string indicatorName in _titleList){
						if(indicatorName.Equals(Convertion.ToHtmlString(GestionWeb.GetWebWord(1207,_webSession.SiteLanguage))))
							isPresent=true;
					}
					if(!isPresent)
						_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1207,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException(" impossible de générer les graphiques Indicator Evolution.",ex)); 
			}
		}
		#endregion

		#region Indicator Media Strategy
		/// <summary>
		/// Graphiques Indicator Media Strategy
		/// </summary>
		private void IndicatorMediaStrategy(){

			StreamWriter sw = null;
			Image img = null;
			object [,] tab=null;
            EngineMediaStrategy engine = new EngineMediaStrategy(_webSession, _productClassIndicatorDAL);
		
			#region Load Data
			if(_webSession.ComparaisonCriterion == TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal){
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal;
                tab = engine.GetChartData();
				_webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal;}
			else{
                tab = engine.GetChartData();
			}
			#endregion

			try{
				if(tab.GetLongLength(0)!=0){

					this.NewPage();

					this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

					string workFile = GetWorkDirectory() + @"\Grp_" + _rqDetails["id_static_nav_session"]+ ".bmp";

					#region Title
					this.PDFPAGE_SetActiveFont(_config.TitleFont.Name, _config.TitleFont.Bold,_config.TitleFont.Italic,
						_config.TitleFont.Underline, _config.TitleFont.Strikeout, 16, 0);
					this.PDFPAGE_SetRGBColor(((double)_config.TitleFontColor.R)/256.0
						,((double)_config.TitleFontColor.G)/256.0
						,((double)_config.TitleFontColor.B)/256.0);
					this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1227 ,_webSession.SiteLanguage));
					#endregion

					#region GRP graph
			
					UIMediaStrategyGraph graph = new UIMediaStrategyGraph(_webSession, _dataSource, _config, tab);
					graph.BuildMediaStrategy();
					graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
					img = Image.FromFile(workFile);
					double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
					coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
					int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
					this.PDFPAGE_ShowImage(i, 
						(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
						this.PDFPAGE_Height/2 - (coef * img.Height / 2),
						coef * img.Width,
						coef * img.Height,
						0);
					img.Dispose();
					img = null;
					graph.Dispose();
					graph = null;
			
			
					#region Clean File
					File.Delete(workFile);
					#endregion
				
					#endregion

					_titleList.Add(Convertion.ToHtmlString(GestionWeb.GetWebWord(1227,_webSession.SiteLanguage)));
				}
			}
			catch(System.Exception ex){
				try{
					sw.Close();
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new HotepPdfException(" impossible de générer les graphiques Indicator Media Strategy.",ex)); 
			}
		}
		#endregion

		#region Aucun Résultat
		/// <summary>
		/// Affichage d'un message d'erreur
		/// </summary>
		/// <returns></returns>
		private string noResult(string message){
			System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
			t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
			t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
			t.Append("</td></tr></table>");
			return t.ToString();
		} 
		#endregion

		#region Affichage d'un arbre
		/// <summary>
		///  Affichage d'un arbre au format HTML
		/// </summary>
		/// <param name="root">Arbre à afficher</param>
		/// <param name="write">Arbre en lecture où en écriture</param>
		/// <param name="displayArrow">Affichage de la flêche</param>
		/// <param name="displayCheckbox">Affichage de la checkbox</param>
		/// <param name="witdhTable">Largeur de la table</param>
		/// <param name="displayBorderTable">Affichage de la bordure</param>
		/// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
		/// <param name="SiteLanguage">Langue</param>
		/// <param name="showHideContent">Index ShowHideCOntent ?</param>
		/// <param name="typetree">Type d'arbre ?</param>
		/// <param name="div">Afficher les div true si c'est le cas</param>
		/// <returns>tableau correspondant à l'arbre</returns>
		private IList ToHtml(TreeNode root,bool displayArrow,bool displayCheckbox,int witdhTable,bool displayBorderTable,int SiteLanguage,int typetree,int showHideContent,bool div,int nbLinesStart, ref int nbLinesEnd,ref IList nbLinesSelection){
		
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			int i=0;
			int start=0;
			int colonne=0;
			string buttonAutomaticChecked="";
			string disabled="";
			string tmp="";
			int j=0;
			int nbLines=0;
			IList htmlTableList = new ArrayList();
			int debut=0;

			foreach(TreeNode currentNode in root.Nodes){
				if(start==0){
					if(displayBorderTable){
						t.Append("<table align=\"center\"  style=\"border-bottom :#644883 1px solid; border-top :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  width="+witdhTable+"  >");
						start=1;
					}
					else{
						t.Append("<table bordercolor=#ffffff class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width="+witdhTable+">");
					}

				}
				else{
					if(displayBorderTable){
						t.Append("<table align=\"center\"  style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\" width="+witdhTable+">");
					} 
					else{
						t.Append("<table align=\"center\"  bordercolor=#ffffff class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width="+witdhTable+">");
					}
				}
				t.Append("<tr>");
				t.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");		
				if(displayCheckbox){
					//En lecture et Non cocher
					if(!currentNode.Checked){
						disabled="disabled";
						buttonAutomaticChecked="";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){ 
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
						else{
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" onclick=\"integration2('"+((LevelInformation)currentNode.Tag).ID+"',"+j+")\" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
					}
					//En lecture et cocher
					else if(currentNode.Checked){
						disabled="disabled";
						buttonAutomaticChecked="checked";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
						else{
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" onclick=\"integration2('"+((LevelInformation)currentNode.Tag).ID+"',"+j+")\" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
					}
				}
				else{t.Append("&nbsp;");}
				t.Append(""+((LevelInformation)currentNode.Tag).Text+"");
				t.Append("</label>");
				t.Append("</td>");
				t.Append("<td width=\"15\"></td>");
				t.Append("</tr>");
				t.Append("</table>");

				nbLines++;

				#region découpage au niveau du père
				if ((nbLines >= nbLinesStart)&&(debut==0))
				{
					htmlTableList.Add(t.ToString());
					nbLinesSelection.Add(nbLines);
					debut=1;
					nbLines=0;
					start=0;
					t = new System.Text.StringBuilder(1000);
				}
				else if(nbLines == 30)
				{
					htmlTableList.Add(t.ToString());
					nbLinesSelection.Add(nbLines);
					nbLines=0;
					start=0;
					t = new System.Text.StringBuilder(1000);
				}
				#endregion
				
				if(currentNode.Nodes.Count>0)
				{
					if(displayBorderTable){
						if(div){
							t.Append("<div id=\""+((LevelInformation)currentNode.Tag).ID+"Content"+showHideContent+"\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
						}
						t.Append("<table align=\"center\"  style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" bgcolor=#DED8E5 width="+witdhTable+">");
					}
					else{
						if(div){
							t.Append("<div style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
						}
						t.Append("<table align=\"center\"  bordercolor=#ffffff bgcolor=#DED8E5 width="+witdhTable+">");
					}
				}

				colonne=0;
				i=0;
			
				while(i<currentNode.Nodes.Count){
					
					if(displayArrow){
						//En lecture et Non cocher
						if(!currentNode.Nodes[i].Checked){
							disabled="disabled";
							buttonAutomaticChecked="";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else{
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
						}
						//En lecture et cocher
						else if(currentNode.Nodes[i].Checked){
							disabled="disabled";
							buttonAutomaticChecked="checked";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else{
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1:"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
						
						}
				
					}
					else{t.Append("&nbsp;");}

					if(colonne==2){
								
						t.Append("<td class=\"txtViolet10\" width=33%>");						
						t.Append(tmp);						
						t.Append("</td>");
						colonne=1;
					}
					else if(colonne==1){
						t.Append("<td class=\"txtViolet10\" width=33%>");								
						t.Append(tmp);
						t.Append("</td>");								
						t.Append("</tr>");
						colonne=0;
					}
					else{
						t.Append("<tr>");
						t.Append("<td class=\"txtViolet10\" width=33%>");
						t.Append(tmp);
						t.Append("</td>");								
						colonne=2;
						nbLines++;
					}

					i++;

					#region découpage du code HTML au niveau des fils
					if (((nbLines >= nbLinesStart)&&(debut==0)&&(colonne==0))||((nbLines >= nbLinesStart)&&(debut==0)&&(i>=currentNode.Nodes.Count))){
						
						t.Append("</table>");
						
						htmlTableList.Add(t.ToString());
						nbLinesSelection.Add(nbLines);
						debut=1;
						nbLines=0;
						start=0;
						t = new System.Text.StringBuilder(1000);

						t.Append("<table align=\"center\" bordercolor=#ffffff class=\"txtViolet11Bold\" style=\"WIDTH: 100%\">");

						if(currentNode.Nodes.Count>0)
						{
							if(displayBorderTable)
							{
								if(div)
								{
									t.Append("<div  id=\""+((LevelInformation)currentNode.Tag).ID+"Content"+showHideContent+"\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
								}
								t.Append("<table align=\"center\" style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" bgcolor=#DED8E5 width="+witdhTable+">");
							}
							else
							{
								if(div)
								{
									t.Append("<div style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
								}
								t.Append("<table align=\"center\" bordercolor=#ffffff bgcolor=#DED8E5 width="+witdhTable+">");
							}
						}
					}
					else if(((nbLines == 30)&&(colonne==0))||((nbLines == 30)&&(i>=currentNode.Nodes.Count)))
					{
						t.Append("</table>");
							
						htmlTableList.Add(t.ToString());
						nbLinesSelection.Add(nbLines);
						nbLines=0;
						start=0;
						t = new System.Text.StringBuilder(1000);
							
						t.Append("<table align=\"center\" bordercolor=#ffffff class=\"txtViolet11Bold\"  style=\"WIDTH: 100%\">");

						if(currentNode.Nodes.Count>0)
						{
							if(displayBorderTable)
							{
								if(div)
								{
									t.Append("<div  id=\""+((LevelInformation)currentNode.Tag).ID+"Content"+showHideContent+"\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
								}
								t.Append("<table align=\"center\" style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" bgcolor=#DED8E5 width="+witdhTable+">");
							}
							else
							{
								if(div)
								{
									t.Append("<div  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
								}
								t.Append("<table align=\"center\" bordercolor=#ffffff bgcolor=#DED8E5 width="+witdhTable+">");
							}
						}
					}
					#endregion

				}	
				if(currentNode.Nodes.Count>0){
					t.Append("</table>");
					if(div){
						t.Append("</div>");				
					}
				}
				
			}

			if((nbLines>0)&&(nbLines<30)){
				htmlTableList.Add(t.ToString());
				nbLinesSelection.Add(nbLines);
			}

			return htmlTableList;
		}
		#endregion

		#region Détail Média
		private void mediaDetail(ref StreamWriter sw,IList SelectionDetailMedia,bool first,int j){
			
			// Détail référence média
			if(_webSession.isReferenceMediaSelected()){
							
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				if(first){
					sw.WriteLine("<TR>");
					sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" +Convertion.ToHtmlString(GestionWeb.GetWebWord(1194,_webSession.SiteLanguage))+" :</TD>");
					sw.WriteLine("<TR><TD>&nbsp;</TD>");
					sw.WriteLine("</TR>");
				}
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD align=\"center\">");
				sw.WriteLine(SelectionDetailMedia[j].ToString());
				sw.WriteLine("</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("</TR>");
			}

			// Détail Média
			if(_webSession.SelectionUniversMedia.FirstNode!=null && _webSession.SelectionUniversMedia.FirstNode.Nodes.Count>0){
			
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" bgColor=\"#DED8E5\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				if(first){
					sw.WriteLine("<TR>");
					sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1194,_webSession.SiteLanguage))+" :</TD>");
					sw.WriteLine("<TR><TD>&nbsp;</TD>");
					sw.WriteLine("</TR>");
				}
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD align=\"center\">");
				sw.WriteLine(SelectionDetailMedia[j].ToString());
				sw.WriteLine("</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("</TR>");
			}
		}
		#endregion

		#region Html file loading
		private void htmlFileLoading(StreamWriter sw, String workFile){
		
			#region Html file loading
			Functions.CloseHtmlFile(sw);
			HTML2PDFClass html = new HTML2PDFClass();
			html.MarginLeft = Convert.ToInt32(this.LeftMargin);
			html.MarginTop = Convert.ToInt32(this.WorkZoneTop);
			html.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
			html.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
			html.ConnectToPDFLibrary (this);
			html.LoadFromFile(workFile);
			html.ConvertAll();
			html.ClearCache();
			html.ConvertAll();
			html.DisconnectFromPDFLibrary ();
			#endregion

			#region Clean File
			File.Delete(workFile);
			#endregion

		}
		#endregion

		#region Sortie HTML
		/// <summary>
		/// Crée le code HTML pour l'affichage du tableau (version graphique) de résultats qui permettra de détecter les réels nouveaux produits
		/// ou annonceurs des démarrages de campagne. Par nouveau il faut comprendre, un annonceur ou produit actif sur le
		/// dernier mois , mais inactif (pas d'investissement) depuis le début de l'anné.
		/// </summary>
		/// <param name="page">Page qui affiche les nouveautés</param>
		/// <param name="tab">tableau de résultats </param>
		/// <param name="webSession">session client</param>
		/// <param name="elementType">élément annonceur ou référence</param>
		/// <returns>Code</returns>
		//public static string GetIndicatorNoveltyGraphicHtmlUI(Page page,object[,] tab,WebSession webSession,ConstResults.Novelty.ElementType elementType){
		private IList GetIndicatorNoveltyGraphicHtmlUI(object[,] tab,WebSession webSession,CstResult.Novelty.ElementType elementType){
	
			#region variables
			string currentMonth="";
			bool PreviousYearActiveMonth=false;
			DateTime currentMonthDate;			
			string classe="";
			string classe2="";
			string classe3="";
			string pluszero="";
			string AdvertiserAccessList="";
			string[] AdvertiserSplitter={","};
			string CompetitorAdvertiserAccessList="";			
			ArrayList AdvertiserAccessListArr=null;
			ArrayList CompetitorAdvertiserAccessListArr=null;
			string IdElementToPersonnalize="";
			#endregion

			#region variable pour le découpage du code HTML
			int nbLines=0;
			IList htmlTableList = new ArrayList();
			#endregion

			#region Pas de données à afficher
			if(tab.GetLength(0)==0)
			{
				if(elementType==CstResult.Novelty.ElementType.product){
					htmlTableList.Add(GestionWeb.GetWebWord(1238,webSession.SiteLanguage));
					return(htmlTableList);
				}
				else{
					htmlTableList.Add(GestionWeb.GetWebWord(1239,webSession.SiteLanguage));
					return(htmlTableList);
				}
			}
			#endregion
			
			if(webSession.ComparativeStudy){

				#region identifiant annonceurs référence et concurrents 
				AdvertiserAccessList = webSession.GetSelection(webSession.ReferenceUniversAdvertiser,CstRights.type.advertiserAccess);		
				if(webSession.CompetitorUniversAdvertiser[0]!=null){
					CompetitorAdvertiserAccessList = webSession.GetSelection((TreeNode)webSession.CompetitorUniversAdvertiser[0],CstRights.type.advertiserAccess);		
				}

				#region recuperation éléments références et concurrents
				if(TNS.AdExpress.Web.Functions.CheckedText.IsStringEmpty(AdvertiserAccessList)){				
					AdvertiserAccessListArr = new ArrayList(AdvertiserAccessList.Split(','));
				}
				if(TNS.AdExpress.Web.Functions.CheckedText.IsStringEmpty(CompetitorAdvertiserAccessList)){
					CompetitorAdvertiserAccessListArr = new ArrayList(CompetitorAdvertiserAccessList.Split(','));
				}			
				#endregion

				#endregion

				#region construction du tableau de nouveautés
				System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
				//Debut tableau
				t.Append("\n<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");

				//fin période étudiée
				//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
				//du dernier mois dispo en BDD
				//traitement de la notion de fréquence
				string absolutEndPeriod = TNS.AdExpress.Web.Functions.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
				if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
					throw new NoDataException();
				DateTime PeriodEndDate = TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);
				//Mois actif
				currentMonth = TNS.AdExpress.Web.Functions.Dates.CurrentActiveMonth(PeriodEndDate,webSession);
				//Date mois actif
				currentMonthDate = TNS.AdExpress.Web.Functions.Dates.GetDateFromAlias(currentMonth);

				#region ligne libellés
				classe2="p7";
				classe="p9";
				classe3="p9ind";
				//Ligne Libellés tableau				
				//Colonne libéllés produits ou annonceurs
				if(CstResult.Novelty.ElementType.advertiser==elementType)
					t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1222,webSession.SiteLanguage)+"</td>");
				else t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1223,webSession.SiteLanguage)+"</td>");
				//Période d'inactivité
				t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1220,webSession.SiteLanguage)+"</td>");			
				//Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
				t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

				//Cellules mois  de l'année N-1
				for (int j=1;j<=12;j++){
					if(j<10)pluszero="0";
					else pluszero="";
					t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+j+"-"+PeriodEndDate.AddYears(-1).Year+"</td>");
				}
				//Colonne separation année N/N-1
				t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

				//cellules mois de la période N
				for(int m=1;m<currentMonthDate.Month;m++){
					if(m<10)pluszero="0";
					else pluszero="";
					t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+m+"-"+PeriodEndDate.Year+"</td>");
				}
				//Colonne separation miis actif année N / et mois inactif année N
				t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
				//Colonne mois en cours (KE)
				if(currentMonthDate.Month<10)pluszero="0";
				else pluszero="";
				t.Append("<td  nowrap align=center class=\"p2\">"+GestionWeb.GetWebWord(1221,webSession.SiteLanguage)+"<br>"+pluszero+currentMonthDate.Month+"-"+currentMonthDate.Year+"</td>");
				t.Append("\n</tr>");
			
				#endregion

				#region lignes produits ou annonceurs
				
				for(int i=0;i<tab.GetLength(0);i++){

					#region personnalisation des éléments de références et concurrents
					if(tab[i,CstResult.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX]!=null){
						IdElementToPersonnalize = tab[i,CstResult.Novelty.PERSONNALISATION_ELEMENT_COLUMN_INDEX].ToString();
						if(AdvertiserAccessListArr!=null && AdvertiserAccessListArr.Contains(IdElementToPersonnalize)){
							classe2="p15"; 
							classe="p151";
						}
						else if(CompetitorAdvertiserAccessListArr !=null && CompetitorAdvertiserAccessListArr.Contains(IdElementToPersonnalize)){							
							classe2="p14"; 
							classe="p141";	
						}
						else{						
							classe2="p7";
							classe="p9";
						}
					}
					#endregion

					t.Append("<tr>");
					//Colonne libéllé produit ou annonceur 
					if(tab[i,CstResult.Novelty.ELEMENT_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe2+">"+tab[i,CstResult.Novelty.ELEMENT_COLUMN_INDEX].ToString()+"</td>");
					else t.Append("<td nowrap  class="+classe2+">&nbsp;</td>");
					//Colonne Période d'inactivité (nbre de mois)
					if(tab[i,CstResult.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX]!=null)t.Append("<td nowrap  class="+classe+">"+tab[i,CstResult.Novelty.INACTIVITY_PERIOD_COLUMN_INDEX].ToString()+"</td>");
					else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
					//Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
					t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

					//Cellules mois  de l'année N-1
					for (int k=1;k<=12;k++){
						//Colonne libéllés dernier mois actifs année N-1 (kE)
						if(tab[i,CstResult.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX]!=null && !tab[i,CstResult.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].ToString().Equals("")){
							//PreviousYearActiveMonth = tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_LABEL_COLUMN_INDEX].ToString();
							if(tab[i,CstResult.Novelty.LATEST_ACTIVE_MONTH_ID_COLUMN_INDEX].ToString().Equals(k.ToString())){							
								//								t.Append("<td nowrap  class=pmcategorynb>"+double.Parse(tab[i,ConstResults.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
								t.Append("<td nowrap  class=pmcategorynb>"+TNS.AdExpress.Web.Functions.Units.ConvertUnitValueToString(tab[i,CstResult.Novelty.LATEST_ACTIVE_MONTH_INVEST_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
								PreviousYearActiveMonth=true;
							}
							else if(PreviousYearActiveMonth) t.Append("<td nowrap class="+classe3+">&nbsp;</td>");
							else t.Append("<td nowrap  >&nbsp;</td>");
						}
						else t.Append("<td nowrap >&nbsp;</td>");
					}
					//Colonne separation année N/N-1
					t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
					//cellules mois de la période N
					for(int l=1;l<currentMonthDate.Month;l++){
						if(PreviousYearActiveMonth)t.Append("<td nowrap class="+classe3+">&nbsp;</td>");
						else t.Append("<td nowrap >&nbsp;</td>");
					}
					PreviousYearActiveMonth=false;
					//Colonne separation mois actif année N / et mois inactif année N
					t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

					//Colonne mois en cours (KE)
					if(tab[i,CstResult.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX]!=null && !tab[i,CstResult.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString().Equals("-"))
						//						t.Append("<td nowrap  class=\"pmcategorynb\">"+double.Parse(tab[i,ConstResults.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
						t.Append("<td nowrap  class=\"pmcategorynb\">"+TNS.AdExpress.Web.Functions.Units.ConvertUnitValueToString(tab[i,CstResult.Novelty.CURRENT_MONTH_INVEST_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
					else t.Append("<td nowrap  class="+classe+">&nbsp;</td>");
					t.Append("\n</tr>");

					nbLines++;

					//Fin d'une ligne on crée une image toutes les 30 lignes

					if(nbLines%35==0){

						//Insertion de 30 lignes html dans l'image	
						#region footer
						t.Append("\n</TD></TR></TABLE>");
						t.Append("</form></body></html>");					
						#endregion

						htmlTableList.Add(t.ToString());

						t = new System.Text.StringBuilder(10000);

						GetHtmlHeader(t);
						t.Append("\n<TABLE border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"p2\"><TR><TD>");

						#region ligne libellés
						classe2="p7";
						classe="p9";
						classe3="p9ind";
						//Ligne Libellés tableau				
						//Colonne libéllés produits ou annonceurs
						if(CstResult.Novelty.ElementType.advertiser==elementType)
							t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1222,webSession.SiteLanguage)+"</td>");
						else t.Append("<tr ><td  nowrap class=\"p2\">"+GestionWeb.GetWebWord(1223,webSession.SiteLanguage)+"</td>");
						//Période d'inactivité
						t.Append("<td  nowrap  class=\"p2\">"+GestionWeb.GetWebWord(1220,webSession.SiteLanguage)+"</td>");			
						//Colonne separation nbre de mois d'inactivité/ graphique dernier mois actif sur N-1
						t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

						//Cellules mois  de l'année N-1
						for (int j=1;j<=12;j++){
							if(j<10)pluszero="0";
							else pluszero="";
							t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+j+"-"+PeriodEndDate.AddYears(-1).Year+"</td>");
						}
						//Colonne separation année N/N-1
						t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");

						//cellules mois de la période N
						for(int m=1;m<currentMonthDate.Month;m++){
							if(m<10)pluszero="0";
							else pluszero="";
							t.Append("<td nowrap  class=\"p2\">&nbsp;"+pluszero+m+"-"+PeriodEndDate.Year+"</td>");
						}
						//Colonne separation miis actif année N / et mois inactif année N
						t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
						//Colonne mois en cours (KE)
						if(currentMonthDate.Month<10)pluszero="0";
						else pluszero="";
						t.Append("<td  nowrap align=center class=\"p2\">"+GestionWeb.GetWebWord(1221,webSession.SiteLanguage)+"<br>"+pluszero+currentMonthDate.Month+"-"+currentMonthDate.Year+"</td>");
						t.Append("\n</tr>");
						#endregion
						
					}
				}
				#endregion

				//Fin tableau
				t.Append("\n</TD></TR></TABLE>");

				#region tableau légende
				t.Append("<table><tr><td nowrap class=pmcategorynb width=25>&nbsp;&nbsp;</td><td class=txtNoir11>"+GestionWeb.GetWebWord(1225,webSession.SiteLanguage)+"</td></tr>");
				t.Append("<tr><td nowrap class="+classe3+" width=25>&nbsp;&nbsp;</td><td class=txtNoir11>"+GestionWeb.GetWebWord(1226,webSession.SiteLanguage)+"</td></tr></table>");
				#endregion

				#endregion

				htmlTableList.Add(t.ToString());
				return(htmlTableList);
			}
			else{
				htmlTableList.Add("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(1224,webSession.SiteLanguage)				
					+"</div>");
				return(htmlTableList);

			}			
		}
		#endregion

		#region Get HTML Header
		private void GetHtmlHeader(System.Text.StringBuilder html){

            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

			#region headers
			html.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
			html.Append("<HTML>");
			html.Append("<HEAD>");
			html.Append("<META http-equiv=\"Content-Type\" content=\"text/html; charset="+charSet+"\">");
			html.Append("<meta content=\"Microsoft Visual Studio .NET 7.1\" name=\"GENERATOR\">");
			html.Append("<meta content=\"C#\" name=\"CODE_LANGUAGE\">");
			html.Append("<meta content=\"JavaScript\" name=\"vs_defaultClientScript\">");
			html.Append("<meta content=\"http://schemas.microsoft.com/intellisense/ie5\" name=\"vs_targetSchema\">");
            html.Append("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/AdExpress.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/GenericUI.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/MediaSchedule.css\" type=\"text/css\" rel=\"stylesheet\">");
			html.Append("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
			html.Append("<meta http-equiv=\"expires\" content=\"0\">");
			html.Append("<meta http-equiv=\"pragma\" content=\"no-cache\">");
			html.Append("<meta name=\"Cache-control\" content=\"no-cache\">");
			html.Append("</HEAD>");
			html.Append("<body>");
			html.Append("<form>");
			html.Append("<TR height=\"25\">");
			html.Append("<TD></TD>");
			html.Append("</TR>");
			html.Append("<TR align=\"center\"><td>");
			#endregion

		}
		#endregion

		#region Ajout de l'image qui correspond au tableau de nouveautés 
        /// <summary>
        /// addNoveltyPicture
        /// </summary>
        /// <param name="result"></param>
        /// <param name="i"></param>
        /// <param name="withTitle"></param>
		private void addNoveltyPicture(IList result,int i,bool withTitle){
		
			CHtmlSnapClass snap;
			IntPtr  hBitmap;
			StringBuilder htmlTMP=null;
			string filePath="";
		
			htmlTMP=new StringBuilder();

			GetHtmlHeader(htmlTMP);
			htmlTMP.Append(result[i]);
			htmlTMP.Append("</td></tr>");

			this.NewPage();

			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

			if(withTitle){

				#region Title
				this.PDFPAGE_SetActiveFont(_config.TitleFont.Name, _config.TitleFont.Bold,_config.TitleFont.Italic,
					_config.TitleFont.Underline, _config.TitleFont.Strikeout, 16, 0);
				this.PDFPAGE_SetRGBColor(((double)_config.TitleFontColor.R)/256.0
					,((double)_config.TitleFontColor.G)/256.0
					,((double)_config.TitleFontColor.B)/256.0);
				this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1197 ,_webSession.SiteLanguage));
				#endregion

			}

			hBitmap = IntPtr.Zero;
			snap = new CHtmlSnapClass();
			snap.SetTimeOut(100000);
			snap.SetCode("21063505C78EB32A");
			snap.SnapHtmlString(htmlTMP.ToString(), "*");
						
			//GetBitmapHandle returns a  bitmap handle, it must be deleted like above, or there will
			//be memory leaks.  
			hBitmap = (IntPtr)snap.GetBitmapHandle();

			//here demo how to use GetImageBytes.
			byte[] data = (byte[])snap.GetImageBytes(".jpg");	
			filePath=GetWorkDirectory() + @"\Campaign_" + _rqDetails["id_static_nav_session"].ToString() + ".jpg";
			FileStream  fs = File.OpenWrite(filePath); 	
			BinaryWriter br = new BinaryWriter(fs); 

			br.Write(data);
			br.Close();
			fs.Close();

			Image imgG ;
			int imgI =0;
			double X1=2;

			imgG = Image.FromFile(filePath);
			imgI = this.AddImageFromFilename(filePath,TxImageCompressionType.itcFlate);

			double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
			double coef = Math.Min((double)1.0,w);
			w = ((double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height);
			coef = Math.Min((double)coef, w);

	
			this.PDFPAGE_ShowImage(imgI,
				X1, this.PDFPAGE_Height/2 - (coef * imgG.Height / 2),
				coef * imgG.Width,
				coef * imgG.Height,
				0);

			imgG.Dispose();
			imgG = null;

			#region Clean File
			File.Delete(filePath);
			#endregion
		}
		#endregion
		
		#endregion

		#region Evenement Envoi mail client
		/// <summary>
		/// Rise exception when the customer mail has not been sent
		/// </summary>
		/// <param name="source">Error source></param>
		/// <param name="message">Error message</param>
		private void mail_mailKoHandler(object source, string message){
			throw new Exceptions.HotepPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
		}
		#endregion
	}
}
