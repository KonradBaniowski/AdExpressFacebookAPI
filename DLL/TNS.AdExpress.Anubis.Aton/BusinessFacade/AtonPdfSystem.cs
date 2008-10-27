#region Information
// Author : Y. R'kaina
// Creation : 09/02/2007
// Modifications :
#endregion

using System;
using System.Web;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using TNS.AdExpress.Anubis.Aton.Common;
using TNS.AdExpress.Anubis.Aton.Exceptions;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Aton.UI;
using AtonFunctions=TNS.AdExpress.Anubis.Aton.Functions;
using TNSAnubisConstantes=TNS.AdExpress.Anubis.Constantes;

using TNS.AdExpress.Constantes.Customer;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Constantes.Web;

using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using APPMRules = TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Web.UI;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using Dundas.Charting.WinControl;
using HTML2PDFAddOn;
using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using TNS.FrameWork.WebResultUI;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Theme;

namespace TNS.AdExpress.Anubis.Aton.BusinessFacade{
	/// <summary>
	/// Description résumée de AtonPdfSystem.
	/// </summary>
	public class AtonPdfSystem:Pdf {

		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Aton Configuration (usefull for PDF layout)
		/// </summary>
		private AtonConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		/// <summary>
		/// Identifiant cible Sector Data de base
		/// </summary>
		protected Int64 _idBaseTarget=0;
		/// <summary>
		/// Identifiant cible Sector Data suppplémentaire
		/// </summary>
		protected Int64 _idAdditionalTarget=0;
		/// <summary>
		/// Identifiant vague Sector Data 
		/// </summary>
		protected Int64 _idWave=0;
		/// <summary>
		/// Date de début
		/// </summary>
		protected int _dateBegin=0;
		/// <summary>
		/// Date de fin
		/// </summary>
		protected int _dateEnd = 0;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public AtonPdfSystem(IDataSource dataSource, AtonConfig config, DataRow rqDetails, WebSession webSession,Theme theme):
        base(theme.GetStyle("Aton")) {
			this._dataSource = dataSource;
			this._config = config;
			this._rqDetails = rqDetails;
			this._webSession = webSession;
		}
		#endregion

		#region Init
		internal string Init(){
			try{
				string shortFName = "";
				string fName =  GetFileName(_rqDetails, ref shortFName);
				bool display = false;
#if(DEBUG)
				display = true;
#endif
				base.Init(true,fName,_config.PdfCreatorPilotLogin,_config.PdfCreatorPilotPass);
				this.DocumentInfo_Creator = this.DocumentInfo_Author = _config.PdfAuthor;
				this.DocumentInfo_Subject = _config.PdfSubject;
				this.DocumentInfo_Title = _config.PdfTitle;
				this.DocumentInfo_Producer = _config.PdfProducer;
				this.DocumentInfo_Keywords = _config.PdfKeyWords;
				return shortFName;
			}
			catch(System.Exception e){
				throw(new AtonPdfException("Impossible d'initialiser le fichier PDF (méthode Init())",e));
			}
		}
		#endregion

		#region Fill
		internal void Fill(){

			try{

				#region MainPage
				MainPageDesign();
				#endregion

				#region Session Parameters
				SessionParameter();
				#endregion

				#region Synthesis
				Synthesis();
				#endregion

				#region Average
				Average();
				#endregion

				#region Seasonality Graph
				if (_webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly)
					SeasonalityGraph();
				#endregion

				#region Family Graph
				FamilyGraph();
				#endregion

				#region Periodicity Graph
				PeriodicityGraph();
				#endregion

				#region Affinities
				Affinities();
				#endregion

				#region Header and Footer
				this.AddHeadersAndFooters(
                    _webSession, true, true,
                    GestionWeb.GetWebWord(2182, _webSession.SiteLanguage) + " - " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern),
					0,-1);
				#endregion

			}
			catch(System.Exception e){
				throw(new AtonPdfException("Erreur lors de la création du fichier PDF",e));
			}
		}
		#endregion

		#region Send
		internal void Send(string fileName){
			ArrayList to = new ArrayList();
			foreach(string s in _webSession.EmailRecipient){
				to.Add(s);
			}
			SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
				WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(2116,_webSession.SiteLanguage)),
				WebFunctions.Text.SuppressAccent(GestionWeb.GetWebWord(1750,_webSession.SiteLanguage)+"\""+_webSession.ExportedPDFFileName
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
					+ TNS.AdExpress.Anubis.Common.Functions.GetRandomString(30,40);

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
				throw(new AtonPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
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
				throw(new AtonPdfException("Unable to provide the working directory", e));
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
		
			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

            string imgPath = ((TNS.AdExpress.Domain.Theme.Picture)Style.GetTag("pictureTitle")).Path;
			System.Drawing.Image imgG = System.Drawing.Image.FromFile(imgPath);
            string pdfTitle = GestionWeb.GetWebWord(2182, _webSession.SiteLanguage);
			
			double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
			double coef = Math.Min((double)1.0,w);
			w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
			coef = Math.Min((double)coef, w);
			
			int imgI = this.AddImageFromFilename(imgPath,TxImageCompressionType.itcFlate);
			this.PDFPAGE_ShowImage(imgI,
				(double)(this.PDFPAGE_Width/2 - coef*imgG.Width/2), (double)(this.WorkZoneBottom - coef*imgG.Height - 100),
				(double)(coef*imgG.Width),(double)(coef*imgG.Height),0);

            Style.GetTag("bigTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
            this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(pdfTitle)) / 2,
                (this.PDFPAGE_Height) / 4, 0, pdfTitle);

            string str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);
            Style.GetTag("createdTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str))/2, 
				1*this.PDFPAGE_Height/3,0,str);

		}
		#endregion

		#region SessionParameter
		/// <summary>
		/// Session parameter design
		/// </summary>
		private void SessionParameter(){

			StreamWriter sw = null;
            string classCss = string.Empty;

			try{
				this.NewPage();

				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				string workFile = GetWorkDirectory() + @"\SessionParameter" + _rqDetails["id_static_nav_session"].ToString() + ".htm";

                sw = AtonFunctions.Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

				#region Title
				sw.WriteLine("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD class=\"txtViolet14Bold\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752,_webSession.SiteLanguage)) + "</TD>");
				sw.WriteLine("</TR>");
				#endregion

				#region Period
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1755, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"20\">");
				sw.WriteLine("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;&nbsp;&nbsp;"
					+ HtmlFunctions.GetPeriodDetail(_webSession)
					+ "</TD>");
				sw.WriteLine("</TR>");
				#endregion

				#region Wave
				//TODO WAVE
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1771, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"20\">");
				sw.WriteLine("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;&nbsp;&nbsp;"
					+ ((LevelInformation)_webSession.SelectionUniversAEPMWave.Nodes[0].Tag).Text
					+ "</TD>");
				sw.WriteLine("</TR>");
				#endregion

				#region Targets
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1757, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("</TR>");
				//Base target
				string targets = "'" + _webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CstRights.type.aepmTargetAccess) + "'";
				//Wave
				string idWave = ((LevelInformation)_webSession.SelectionUniversAEPMWave.Nodes[0].Tag).ID.ToString();
				DataSet ds = TargetListDataAccess.GetAEPMTargetListFromIDSDataAccess(idWave, targets, _webSession.Source);
				foreach(DataRow r in ds.Tables[0].Rows){
					sw.WriteLine("<TR height=\"20\">");
					sw.WriteLine("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;&nbsp;&nbsp;");
					sw.WriteLine( Convertion.ToHtmlString(r["target"].ToString()));
					sw.WriteLine("</TD>");
					sw.WriteLine("</TR>");
				}
				ds.Dispose();
				ds = null;
				#endregion

				#region Products
				const int nbLineByPage = 32;
				int currentLine = 12;
				sw.WriteLine("<TR height=\"7\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("</TR>");
				//reference
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1677, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR align=\"center\">");
				sw.WriteLine("<TD><br>");
				//sw.WriteLine(WebFunctions.DisplayTreeNode.ToHtml(_webSession.CurrentUniversAdvertiser,false,true,true,600,true,false,_webSession.SiteLanguage,3,1,false));								
				if(_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count>0)
                    sw.WriteLine(WebFunctions.DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.Source, 600, true, nbLineByPage, ref currentLine));
				sw.WriteLine("</TD>");
				sw.WriteLine("</TR>");
				//competitor
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR>");
				sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1668, _webSession.SiteLanguage)) + " :</TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR align=\"center\">");
				sw.WriteLine("<TD><br>");
				ds = GroupSystem.ListFromSelection(_dataSource, _webSession);
				sw.WriteLine("<table class=\"txtViolet11Bold\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">");
				for(int i =0; i < ds.Tables[0].Rows.Count; i++){
					sw.WriteLine("<tr>");
					if((i+1) < ds.Tables[0].Rows.Count){
                        classCss = "violetBorderWithoutBottom";
					}
					else{
                        classCss = "violetBorder";
					}
                    sw.WriteLine("<td class=\"" + classCss + "\">&nbsp;&nbsp;");
					sw.WriteLine(ds.Tables[0].Rows[i][0].ToString());
					sw.WriteLine("</td>");
					sw.WriteLine("</tr>");
				}
				ds.Dispose();
				ds = null;
				sw.WriteLine("</TD>");
				sw.WriteLine("</TR>");
				#endregion

				#region Html file loading
				AtonFunctions.Functions.CloseHtmlFile(sw);
				HTML2PDF2Class html = new HTML2PDF2Class();
				html.MarginLeft = Convert.ToInt32(this.LeftMargin);
				html.MarginTop = Convert.ToInt32(this.WorkZoneTop);
				html.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
				html.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
				html.ConnectToPDFLibrary (this);
				html.LoadHTMLFile(workFile);
				html.ConvertAll();
				html.DisconnectFromPDFLibrary ();
				#endregion

				#region Clean File
				File.Delete(workFile);
				#endregion
			
			}
			catch(System.Exception e){
				try{
					sw.Close();
				}
				catch(System.Exception e2){}
				throw(new AtonPdfException("Unable to build the session parameter page for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e));
			}
		}
		#endregion

		#region Synthesis
		/// <summary>
		/// Synthesis
		/// </summary>
		private void Synthesis(){
		
			StreamWriter sw = null;

			try{

				#region targets
				//base target
				_idBaseTarget=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CstRights.type.aepmBaseTargetAccess));
				//additional target
				_idAdditionalTarget=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CstRights.type.aepmTargetAccess));									
				#endregion

				#region Wave
				_idWave=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMWave,CstRights.type.aepmWaveAccess));									
				#endregion

				this.NewPage();

				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				string workFile = GetWorkDirectory() + @"\Synthesis_" + _rqDetails["id_static_nav_session"].ToString() + ".htm";

                sw = AtonFunctions.Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

				#region Title
				sw.WriteLine("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD class=\"txtViolet14Bold\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2114,_webSession.SiteLanguage)) + "</TD>");
				sw.WriteLine("</TR>");
				#endregion
				
				#region result
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR align=\"center\"><td>");

				ResultTable resultTable = APPMRules.SectorDataSynthesisRules.GetSynthesisFormattedTable(_webSession,int.Parse(_webSession.PeriodBeginningDate),int.Parse(_webSession.PeriodEndDate),_idBaseTarget,_idAdditionalTarget);
				WebControlResultTable webControlResultTable=new WebControlResultTable();

				webControlResultTable.Data=resultTable;
				webControlResultTable.CssL1="synthesisLv1";
				webControlResultTable.CssL2="synthesisLv2";
				webControlResultTable.CssTitle="title";
				webControlResultTable.Title=GestionWeb.GetWebWord(2114 ,_webSession.SiteLanguage);

				sw.WriteLine(webControlResultTable.GetHTML());

				sw.WriteLine("</td></tr>");
				#endregion

				#region Html file loading
				sw.WriteLine("</table>");
				AtonFunctions.Functions.CloseHtmlFile(sw);
				HTML2PDF2Class html = new HTML2PDF2Class();
				html.MarginLeft = 0;
				html.MarginTop = Convert.ToInt32(this.WorkZoneTop);
				html.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
				html.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
				html.ConnectToPDFLibrary (this);
				html.LoadHTMLFile(workFile);
				html.ConvertAll();
				html.DisconnectFromPDFLibrary ();
				#endregion

				#region Clean File
				File.Delete(workFile);
				#endregion
			
			}
			catch(System.Exception e){
				try{
					sw.Close();
				}
				catch(System.Exception e2){}
				throw(new AtonPdfException("Unable to process the synthesis result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e)); 
			}
		
		}
		#endregion

		#region Average
		/// <summary>
		/// Average
		/// </summary>
		private void Average(){
		
			StreamWriter sw = null;

			try{

				this.NewPage();

				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				string workFile = GetWorkDirectory() + @"\Average_" + _rqDetails["id_static_nav_session"].ToString() + ".htm";

                sw = AtonFunctions.Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

				#region Title

				#region Initialisation
				string productDetail="";

				if(_webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser)
					productDetail=GestionWeb.GetWebWord(1106,_webSession.SiteLanguage);
				else if(_webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand)
					productDetail=GestionWeb.GetWebWord(1889,_webSession.SiteLanguage);
				else if(_webSession.PreformatedProductDetail == WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product)
					productDetail=GestionWeb.GetWebWord(858,_webSession.SiteLanguage);
				#endregion

				sw.WriteLine("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD class=\"txtViolet14Bold\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(2081,_webSession.SiteLanguage)) + " par " + productDetail + "</TD>");
				sw.WriteLine("</TR>");
				#endregion
				
				#region result
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR align=\"center\"><td>");

				ResultTable resultTable = APPMRules.SectorDataAverageRules.GetAverageFormattedTable(_webSession,int.Parse(_webSession.PeriodBeginningDate),int.Parse(_webSession.PeriodEndDate),_idBaseTarget,_idAdditionalTarget);
				WebControlResultTable webControlResultTable=new WebControlResultTable();

				webControlResultTable.Data=resultTable;
				webControlResultTable.CssL1="synthesisLv1";
				webControlResultTable.CssL2="synthesisLv2";
				webControlResultTable.CssL3="avgLv0";
				webControlResultTable.CssLHeader="h2";

				sw.WriteLine(webControlResultTable.GetHTML());

				sw.WriteLine("</td></tr>");
				#endregion

				#region Html file loading
				sw.WriteLine("</table>");
				AtonFunctions.Functions.CloseHtmlFile(sw);
				HTML2PDF2Class html = new HTML2PDF2Class();
				html.MarginLeft = 0;
				html.MarginTop = Convert.ToInt32(this.WorkZoneTop);
				html.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
				html.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
				html.ConnectToPDFLibrary (this);
				html.LoadHTMLFile(workFile);
				html.ConvertAll();
				html.DisconnectFromPDFLibrary ();
				#endregion

				#region Clean File
				File.Delete(workFile);
				#endregion
			
			}
			catch(System.Exception e){
				try{
					sw.Close();
				}
				catch(System.Exception e2){}
				throw(new AtonPdfException("Unable to process the average result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e)); 
			}
		
		}
		#endregion
		
		#region Affinities
		/// <summary>
		/// Affinities
		/// </summary>
		private void Affinities(){
		
			StreamWriter sw = null;

			try{

				this.NewPage();

				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				string workFile = GetWorkDirectory() + @"\Affinities_" + _rqDetails["id_static_nav_session"].ToString() + ".htm";

                sw = AtonFunctions.Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

				#region Title
				sw.WriteLine("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR height=\"14\">");
				sw.WriteLine("<TD class=\"txtViolet14Bold\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1687,_webSession.SiteLanguage)) + "</TD>");
				sw.WriteLine("</TR>");
				#endregion
				
				#region result
				sw.WriteLine("<TR height=\"25\">");
				sw.WriteLine("<TD></TD>");
				sw.WriteLine("</TR>");
				sw.WriteLine("<TR align=\"center\"><td>");

				ResultTable resultTable = APPMRules.SectorDataAffintiesRules.GetData(_webSession,_webSession.Source,int.Parse(_webSession.PeriodBeginningDate),int.Parse(_webSession.PeriodEndDate),_idBaseTarget,_idWave);
				WebControlResultTable webControlResultTable=new WebControlResultTable();

				webControlResultTable.Data=resultTable;
				webControlResultTable.CssL1="affinitiesLv1";
				webControlResultTable.CssLTotal="lv0";
				webControlResultTable.CssLHeader="h2";

				sw.WriteLine(webControlResultTable.GetHTML());

				sw.WriteLine("</td></tr>");
				#endregion

				#region Html file loading
				sw.WriteLine("</table>");
				AtonFunctions.Functions.CloseHtmlFile(sw);
				HTML2PDF2Class html = new HTML2PDF2Class();
				html.MarginLeft = 0;
				html.MarginTop = Convert.ToInt32(this.WorkZoneTop);
				html.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
				html.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
				html.ConnectToPDFLibrary (this);
				html.LoadHTMLFile(workFile);
				html.ConvertAll();
				html.DisconnectFromPDFLibrary ();
				#endregion

				#region Clean File
				File.Delete(workFile);
				#endregion
			
			}
			catch(System.Exception e){
				try{
					sw.Close();
				}
				catch(System.Exception e2){}
				throw(new AtonPdfException("Unable to process the affinities result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e)); 
			}
		
		}
		#endregion

		#region Seasonality
		private void SeasonalityGraph(){
		
			DataTable seasonalityPlanData;
	
			try{

				#region Initialisation
				_dateBegin = int.Parse(_webSession.PeriodBeginningDate);
				_dateEnd = int.Parse(_webSession.PeriodEndDate);
				#endregion

				#region Données
				//Données				
				seasonalityPlanData=APPMRules.SectorDataSeasonalityRules.GetSeasonalityPreformatedData(_webSession,_dataSource,_idWave,_dateBegin,_dateEnd,_idBaseTarget,_idAdditionalTarget);
				#endregion

				SetSeasonalityGraph("Seasonality_",seasonalityPlanData,true);
				SetSeasonalityGraph("Seasonality_",seasonalityPlanData,false);

			}
			catch(System.Exception ex){
				throw(new AtonPdfException("impossible de générer le graphique Seasonality",ex)); 
			}
		}
		#endregion

		#region Interest Family
		private void FamilyGraph(){
			
			DataTable InterestFamilyPlanData;
			
			try{

				#region Initialisation
				_dateBegin = int.Parse(_webSession.PeriodBeginningDate);
				_dateEnd = int.Parse(_webSession.PeriodEndDate);
				#endregion

				#region Données
				//Données				
				InterestFamilyPlanData=TNS.AdExpress.Web.Rules.Results.APPM.AnalyseFamilyInterestPlanRules.InterestFamilyPlan(_webSession, this._dataSource,this._idWave,this._dateBegin,this._dateEnd,this._idBaseTarget,this._idAdditionalTarget);
				#endregion

				SetInterestFamilyGraph("InterestFamily_",InterestFamilyPlanData,false);
				if (_webSession.Unit==WebConstantes.CustomerSessions.Unit.grp)
					SetInterestFamilyGraph("InterestFamily_",InterestFamilyPlanData,true);

			}
			catch(System.Exception ex){
				throw(new AtonPdfException(" impossible de générer le graphique Interest Family",ex)); 
			}
		}
		#endregion

		#region Periodicity
		private void PeriodicityGraph(){
			
			DataTable periodicityPlanData;
			
			try{

				#region Données
				//Données				
				periodicityPlanData=TNS.AdExpress.Web.Rules.Results.APPM.PeriodicityPlanRules.PeriodicityPlan(_webSession, this._dataSource,this._idWave,this._dateBegin,this._dateEnd,this._idBaseTarget,this._idAdditionalTarget);
				#endregion

				SetPeriodicityGraph("periodicity_",periodicityPlanData);

			}
			catch(System.Exception ex){
				throw(new AtonPdfException(" impossible de générer le graphique Périodicités de presse",ex)); 
			}
		}
		#endregion

		#region Set Seasonality Graph
		/// <summary>
		/// Creation du Graphe pour la planche seasonality
		/// </summary>
		/// <param name="fileName">Nom du fichier temporaire</param>
		/// <param name="seasonalityPlanData">Data table</param>
		/// <param name="isUnitGraph">Boolean pour différencier entre le graphe des unités et celui des distributions</param>
		private void SetSeasonalityGraph(string fileName, DataTable seasonalityPlanData, bool isUnitGraph){
		
			Image img = null;

			try{
				
				string workFile = GetWorkDirectory() + @"\"+fileName+"" + _rqDetails["id_static_nav_session"]+ ".bmp";

				UISeasonalityGraph graph = new UISeasonalityGraph(_webSession, _dataSource, _config, seasonalityPlanData,Style);

				this.NewPage();
				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				#region Title

                Style.GetTag("SeasonalityGraphBigTitleFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
				this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1139,_webSession.SiteLanguage));
				#endregion

				#region Set Design
				if(isUnitGraph)
					graph.SetDesignModeUnitGraph();
				else
					graph.SetDesignModeDistributionGraph();

				graph.SaveAsImage(workFile,ChartImageFormat.Bmp);
				img = Image.FromFile(workFile);
				double coef = Math.Min(1.0, ((double)this.PDFPAGE_Width/((double)img.Width)));
				coef = Math.Min(coef, ((double)(this.WorkZoneBottom - this.WorkZoneTop - 40)/((double)img.Height)));
				coef+=coef*0.1;
				int i = this.AddImageFromFilename(workFile, TxImageCompressionType.itcFlate);
				this.PDFPAGE_ShowImage(i, 
					(this.PDFPAGE_Width / 2) - (coef * img.Width /2),
					this.PDFPAGE_Height/2 - (coef * img.Height / 2),
					coef * img.Width,
					coef * img.Height,
					0);
				img.Dispose();
				img = null;
				#endregion
				
				#region Clean File
				File.Delete(workFile);
				#endregion

			}			
			catch(System.Exception e){
				try{
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new AtonPdfException(" impossible de générer le graphique Seasonality",e)); 
			}
		}
		#endregion

		#region Set Interest Family Graph
		/// <summary>
		/// Creation du Graphe pour la planche interest family
		/// </summary>
		/// <param name="fileName">Nom du fichier temporaire</param>
		/// <param name="seasonalityPlanData">Data table</param>
		/// <param name="isUnitGraph">Boolean pour différencier entre le graphe des unités et celui des distributions</param>
		private void SetInterestFamilyGraph(string fileName, DataTable InterestFamilyPlanData, bool isGRPGraph){
		
			Image img = null;

			try{
				
				string workFile = GetWorkDirectory() + @"\"+fileName+"" + _rqDetails["id_static_nav_session"]+ ".bmp";

				UIFamilyGraph graph = new UIFamilyGraph(_webSession, _dataSource, _config, InterestFamilyPlanData,Style);

				this.NewPage();
				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				#region Title

                Style.GetTag("PeriodicityGraphBigTitleGrpFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
				this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1777,_webSession.SiteLanguage));
				#endregion

				#region Set Design
				if (isGRPGraph)
					graph.SetGRPDesignMode();
				else
					graph.SetDesignMode();

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
				#endregion
				
				#region Clean File
				File.Delete(workFile);
				#endregion

			}			
			catch(System.Exception e){
				try{
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new AtonPdfException(" impossible de générer le graphique Interest Family",e)); 
			}
		}
		#endregion

		#region Set Periodicity Graph
		/// <summary>
		/// Creation du Graphe pour la planche periodicity
		/// </summary>
		/// <param name="fileName">Nom du fichier temporaire</param>
		/// <param name="seasonalityPlanData">Data table</param>
		/// <param name="isUnitGraph">Boolean pour différencier entre le graphe des unités et celui des distributions</param>
		private void SetPeriodicityGraph(string fileName, DataTable periodicityPlanData){
		
			Image img = null;

			try{
				
				string workFile = GetWorkDirectory() + @"\"+fileName+"" + _rqDetails["id_static_nav_session"]+ ".bmp";

				UIPeriodicityGraph graph = new UIPeriodicityGraph(_webSession, _dataSource, _config, periodicityPlanData,Style);

				this.NewPage();
				this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

				#region Title
                Style.GetTag("PeriodicityGraphBigTitleFont").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
				this.PDFPAGE_TextOut(this.LeftMargin, this.WorkZoneTop + 25.0, 0, GestionWeb.GetWebWord(1754,_webSession.SiteLanguage));
				#endregion

				#region Set Design
				graph.SetDesignMode();

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
				#endregion
				
				#region Clean File
				File.Delete(workFile);
				#endregion

			}			
			catch(System.Exception e){
				try{
					img.Dispose();
					img = null;
				}
				catch(System.Exception e2){}
				throw(new AtonPdfException(" impossible de générer le graphique Périodicités de presse",e)); 
			}
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
			throw new Exceptions.AtonPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
		}
		#endregion

	}
}
