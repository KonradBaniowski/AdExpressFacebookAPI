#region Information
//Author : Y. Rkaina
//Creation : 10/08/2006
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
using System.Windows.Forms;
using System.Reflection;

using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Anubis.Miysis.Exceptions;
using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.AdExpress.Anubis.Common;
using TNSAnubisConstantes=TNS.AdExpress.Anubis.Constantes;

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
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using PDFCreatorPilotLib;
using HtmlSnap2;
using TNS.FrameWork.DB.Common;

using HTML2PDFAddOn;

using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using System.Globalization;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using DomainLevel = TNS.AdExpress.Domain.Level;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
#endregion

namespace TNS.AdExpress.Anubis.Miysis.BusinessFacade{
	/// <summary>
	/// Generate the PDF document for MiysisPdfSystem module.
	/// </summary>
	public class MiysisPdfSystem : Pdf{

		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Appm Configuration (usefull for PDF layout)
		/// </summary>
		private MiysisConfig _config = null;
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
		private enum imgPositionItems {
			/// <summary>
			/// Position : Haut, gauche
			/// </summary>
			TOP_LEFT=0,
			/// <summary>
			/// Position : Haut, droite
			/// </summary>
			TOP_RIGHT=1,
			/// <summary>
			/// Position : Bas, gauche
			/// </summary>
			BOTTOM_LEFT=2,
			/// <summary>
			/// Position : Bas, droite
			/// </summary>
			BOTTOM_RIGHT=3
		};
		/// <summary>
		/// Position des visuels dans le pdf
		/// </summary>
		private enum imgPosition {
			/// <summary>
			/// Position : Haut
			/// </summary>
			TOP=0,
			/// <summary>
			/// Position : Bas
			/// </summary>
			BOTTOM=1,
			/// <summary>
			/// Dépend du traitement
			/// </summary>
			DYNAMIC=-1
		};
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
        public MiysisPdfSystem(IDataSource dataSource, MiysisConfig config, DataRow rqDetails, WebSession webSession)
            :
        base(config.LeftMargin, config.RightMargin, config.TopMargin, config.BottomMargin,
        config.HeaderHeight, config.FooterHeight) {
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
				this.DocumentInfo_Title = GestionWeb.GetWebWord(751, _webSession.SiteLanguage);
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
		internal void Fill(){

			try{

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
				string dateString = Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);

				this.AddHeadersAndFooters(
				_webSession,
				@"Images\Common\logo_Tns.bmp",
				imagePosition.leftImage,
				GestionWeb.GetWebWord(751, _webSession.SiteLanguage) + " - " + dateString,
				0, -1, _config.HeaderFontColor, _config.HeaderFont, true, _webSession);
				#endregion
				
				
			}
			catch(System.Exception e){
				throw(e);
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
				Text.SuppressAccent(GestionWeb.GetWebWord(2006,_webSession.SiteLanguage)),
				Text.SuppressAccent(GestionWeb.GetWebWord(1750,_webSession.SiteLanguage)+"\""+_webSession.ExportedPDFFileName
				+ "\""+String.Format(GestionWeb.GetWebWord(1751,_webSession.SiteLanguage),_config.WebServer)
				//				+ "<br><br>" + "<a href=\"http://www.tnsadexpress.com/AdExCustomerFiles/" + _webSession.CustomerLogin.IdLogin + "/" + fileName + ".pdf\">" + _webSession.ExportedPDFFileName +"</a>"
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
				throw(new MiysisPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
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
				throw(new MiysisPdfException("Unable to provide the working directory", e));
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
	
			string imgPath = @"Images\" + _webSession.SiteLanguage + @"\LogoAdExpress.jpg";
			Image imgG = Image.FromFile(imgPath);
			
			double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
			double coef = Math.Min((double)1.0,w);
			w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
			coef = Math.Min((double)coef, w);
			
			int imgI = this.AddImageFromFilename(imgPath,TxImageCompressionType.itcFlate);
			this.PDFPAGE_ShowImage(imgI,
				(double)(this.PDFPAGE_Width/2 - coef*imgG.Width/2), (double)(this.WorkZoneBottom - coef*imgG.Height - 100),
				(double)(coef*imgG.Width),(double)(coef*imgG.Height),0);


			this.PDFPAGE_SetRGBColor(((double)_config.MainPageFontColor.R)/256.0
				,((double)_config.MainPageFontColor.G)/256.0
				,((double)_config.MainPageFontColor.B)/256.0);
			this.PDFPAGE_SetActiveFont(_config.MainPageTitleFont.Name,
				_config.MainPageTitleFont.Bold,
				_config.MainPageTitleFont.Italic,
				_config.MainPageTitleFont.Underline,
				_config.MainPageTitleFont.Strikeout,
				_config.MainPageTitleFont.SizeInPoints,TxFontCharset.charsetANSI_CHARSET);
			
			string str = "";
			
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(GestionWeb.GetWebWord(751, _webSession.SiteLanguage))) / 2,
					(this.PDFPAGE_Height) / 4, 0, GestionWeb.GetWebWord(751, _webSession.SiteLanguage));			
			
            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);
				
			this.PDFPAGE_SetActiveFont(_config.MainPageDefaultFont.Name,
				_config.MainPageDefaultFont.Bold,
				_config.MainPageDefaultFont.Italic,
				_config.MainPageDefaultFont.Underline,
				_config.MainPageDefaultFont.Strikeout,
				_config.MainPageDefaultFont.SizeInPoints,
				TxFontCharset.charsetANSI_CHARSET);
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str))/2, 
				1*this.PDFPAGE_Height/3,0,str);

		}
		#endregion

		#region SessionParameter
		private void SessionParameter(){
		
		//Formatting date to be used in the query
		string dateBegin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd");
		string dateEnd = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd");

		StreamWriter sw = null;

		this.NewPage();

		this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

		string workFile = GetWorkDirectory() + @"\Campaign_" + _rqDetails["id_static_nav_session"].ToString() + ".htm";

		sw = TNS.AdExpress.Anubis.Common.Functions.GetHtmlFile(workFile, _webSession, _config.WebServer);

		#region Title
		sw.WriteLine("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
		sw.WriteLine("<TR height=\"25\">");
		sw.WriteLine("<TD></TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR height=\"14\">");
		//sw.WriteLine("<TD style=\"font-family: Arial, Helvetica, sans-serif; font-size: 20px; color: #644883; font-weight: bold;\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752,_webSession.SiteLanguage)) + "</TD>");
		sw.WriteLine("<TD class=\"mi\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752, _webSession.SiteLanguage)) + "</TD>");
		sw.WriteLine("</TR>");
		#endregion
		
		#region Period
		sw.WriteLine("<TR height=\"7\">");
		sw.WriteLine("<TD></TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
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
		//TODO Média
		sw.WriteLine("<TR height=\"7\">");
		sw.WriteLine("<TD></TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
		sw.WriteLine("<TD></TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR>");
		sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1292, _webSession.SiteLanguage)) + " :</TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR>");
		sw.WriteLine("<TD align=\"left\">");
		sw.WriteLine(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(_webSession.SelectionUniversMedia,false,false,false,600,false,false,_webSession.SiteLanguage,2,1,true));
		sw.WriteLine("</TD>");
		sw.WriteLine("</TR>");
		#endregion

		#region Unité
		//TODO Unité
		sw.WriteLine("<TR height=\"7\">");
		sw.WriteLine("<TD></TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
		sw.WriteLine("<TD></TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR>");
		sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1795, _webSession.SiteLanguage)) + " :</TD>");
		sw.WriteLine("</TR>");
		sw.WriteLine("<TR height=\"20\">");
		sw.WriteLine("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;"
			+GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[_webSession.Unit],_webSession.SiteLanguage)
			+ "</TD>");
		sw.WriteLine("</TR>");
		#endregion

        #region produit
        bool first = true;
		const int nbLineByPage = 32;
		int currentLine = 10;
		if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0) {

			sw.WriteLine("<TR height=\"7\">");
			sw.WriteLine("<TD></TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
			sw.WriteLine("<TD></TD>");
			sw.WriteLine("</TR>");
			sw.WriteLine("<TR>");
			sw.WriteLine("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
			sw.WriteLine("</TR>");

			sw.WriteLine("<TR align=\"center\">");
			sw.WriteLine("<TD>");
            sw.WriteLine(Convertion.ToHtmlString(TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.Source, 600, true, nbLineByPage, ref currentLine)));
			sw.WriteLine("</TD>");
			sw.WriteLine("</TR>");

		}
        if ((_webSession.ProductDetailLevel != null && _webSession.ProductDetailLevel.ListElement != null) || _webSession.SloganIdZoom > -1) {
            sw.WriteLine("<TR height=\"7\">");
            sw.WriteLine("<TD></TD>");
            sw.WriteLine("</TR>");
			sw.WriteLine("<TR height=\"1\" class=\"lightPurple\">");//bgColor=\"#DED8E5\"
            sw.WriteLine("<TD></TD>");
            sw.WriteLine("</TR>");
            sw.WriteLine("<TR>");
            sw.WriteLine("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(518, _webSession.SiteLanguage)) + " :</TD>");
            sw.WriteLine("</TR>");
            if (_webSession.ProductDetailLevel != null && _webSession.ProductDetailLevel.ListElement != null)
            {
                sw.WriteLine("<TR>");
                sw.WriteLine("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;");
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
                    case CstRights.type.holdingCompanyAccess:
                        code = 1589;
                        break;
                    case CstRights.type.segmentAccess:
                        code = 592;
                        break;

                }
                #endregion
                sw.WriteLine(Convertion.ToHtmlString(string.Format("{0} \"{1}\"",
                    GestionWeb.GetWebWord(code, _webSession.SiteLanguage),
                    ((LevelInformation)_webSession.ProductDetailLevel.ListElement.Tag).Text
                    )
                ));
                sw.WriteLine("</TD>");
                sw.WriteLine("</TR>");
            }
            if (_webSession.SloganIdZoom > -1)
            {
                sw.WriteLine("<TR>");
                sw.WriteLine("<TD align=\"left\" class=\"txtViolet11Bold\">&nbsp;");
                sw.WriteLine(Convertion.ToHtmlString(string.Format("{0} \"{1}\"",
                    GestionWeb.GetWebWord(1987, _webSession.SiteLanguage),
                    _webSession.SloganIdZoom
                    )
                ));
                sw.WriteLine("</TD>");
                sw.WriteLine("</TR>");
            }
        }
        #endregion

		#region Html file loading
		sw.WriteLine("</TABLE>");
		TNS.AdExpress.Anubis.Common.Functions.CloseHtmlFile(sw);
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

		#region MediaPlanIpression
		private void MediaPlanImpression(){
	
			#region GETHTML
			StringBuilder html=new StringBuilder(10000);
			StringBuilder htmlHeader=new StringBuilder(10000);
			ArrayList versionsUIs = new ArrayList();
			int startIndexVisual=0;
			Int64 nbWeek=0;
			int nbLines=0;
            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
            MediaScheduleData result = null;
            MediaSchedulePeriod period = null;
            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);
            DateTime begin;
            DateTime end;
            object[] param = null;

            string[] vehicles = _webSession.GetSelection(_webSession.SelectionUniversMedia, CstRights.type.vehicleAccess).Split(',');
			try{

                html.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
				html.Append("<HTML>");
				html.Append("<HEAD>");
				html.Append("<META http-equiv=\"Content-Type\" content=\"text/html; charset="+charSet+"\">");
                html.Append("<META http-equiv=\"content-encoding\" content=\"utf-8\">");
				html.Append("<meta content=\"Microsoft Visual Studio .NET 7.1\" name=\"GENERATOR\">");
				html.Append("<meta content=\"C#\" name=\"CODE_LANGUAGE\">");
				html.Append("<meta content=\"JavaScript\" name=\"vs_defaultClientScript\">");
				html.Append("<meta content=\"http://schemas.microsoft.com/intellisense/ie5\" name=\"vs_targetSchema\">");
				html.Append("<LINK href=\"" + _config.WebServer +"/App_Themes" + "/" + themeName + "/Css/AdExpress.css\" type=\"text/css\" rel=\"stylesheet\">");
				html.Append("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/GenericUI.css\" type=\"text/css\" rel=\"stylesheet\">");
				html.Append("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/MediaSchedule.css\" type=\"text/css\" rel=\"stylesheet\">");
				html.Append("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
				html.Append("<meta http-equiv=\"expires\" content=\"0\">");
				html.Append("<meta http-equiv=\"pragma\" content=\"no-cache\">");
				html.Append("<meta name=\"Cache-control\" content=\"no-cache\">");
				html.Append("</HEAD>");
				html.Append("<body>");

				htmlHeader.Append(html.ToString());

                #region result
                Int64 idVehicle = Int64.Parse(vehicles[0]);
                //result = GenericMediaScheduleUI.GetPdf(
                //    GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1), _webSession, period, htmlHeader, ref nbWeek, idVehicle);

                begin = Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                end = Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
                if (_webSession.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3)) {
                    _webSession.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                }
                period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod);
                
                if (vehicles.Length == 1)
                    param = new object[3];
                else
                    param = new object[2];

                param[0] = _webSession;
                param[1] = period;
                if(vehicles.Length==1)
                    param[2] = idVehicle;

                IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                mediaScheduleResult.Module = module;
                result = mediaScheduleResult.GetPDFHtml();

				#endregion

				#region Construction du tableaux global
				html.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
				#endregion

				html.Append("\r\n\t<tr>\r\n\t\t<td>");
				
				Double w,width;

				if(nbWeek<5) {
					width=863;
					w=1;
					nbLines=(int)Math.Round(w*(width/20))-4;
				}
				else {
					w=0.652;
					width=996+(133*(nbWeek-5));
					nbLines=(int)Math.Round(w*(width/20))-4;
				}

				decoupageHTML(html,Convertion.ToHtmlString(result.HTMLCode),nbLines,htmlHeader,false);
				html.Append("\r\n\t\t</td>\r\n\t</tr>");
				html.Append("</table>");
				html.Append("</BODY>");
				html.Append("</HTML>");
			}
			catch(System.Exception err){
				throw(new MiysisPdfException("Unable to process Media Schedule Alert export result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",err)); 
			}
			finally{
				_webSession.CurrentModule = module.Id;
			}
			#endregion

		}
		#endregion

		#region Découpage du code HTML
		/// <summary>
		/// Découpage du code HTML pour l'export PDF du plan média (Tableau)
		/// </summary>
		/// <param name="html">Le code HTML à générer</param>
		/// <param name="strHtml">Le code HTML à découper</param>
		/// <param name="nbLines">Nombres de lignes pour le découpage</param>
		/// <param name="htmlHeader">L'entête HTML</param>
		private void decoupageHTML(StringBuilder html,string strHtml, int nbLines,StringBuilder htmlHeader,bool version){

			int startIndex=0,oldStartIndex=0,tmp;
			ArrayList partieHTML = new ArrayList();
            string resultTableHeader = GetResultTableHeader(strHtml);
			double coef=0;

			int start=0;

			 while((startIndex < strHtml.Length)&&(startIndex!=-1)){
				tmp=0;
				
				if(start==0){
					while((tmp<nbLines+4)&&(startIndex<strHtml.Length)&&(startIndex!=-1)){
						startIndex=strHtml.IndexOf("<tr>",startIndex+1);
						tmp++;
					}
				start=1;
				}
				else{
					while((tmp<nbLines)&&(startIndex<strHtml.Length)&&(startIndex!=-1)){
						startIndex=strHtml.IndexOf("<tr>",startIndex+1);
						tmp++;
					}
				}
				if(startIndex==-1)
					partieHTML.Add(strHtml.Substring(oldStartIndex,strHtml.Length-oldStartIndex));
				else
					partieHTML.Add(strHtml.Substring(oldStartIndex,startIndex-oldStartIndex));
				oldStartIndex=startIndex;
			}
			
			for(int i=0; i<partieHTML.Count; i++){
				if(i>0)
                    html.Append(GetHeader(htmlHeader, resultTableHeader));
				html.Append(partieHTML[i].ToString());
				html.Append(GetEnd());
				if(version)
					SnapShots(ref html,i,partieHTML,true, ref coef);
				else
					SnapShots(ref html,i,partieHTML,false, ref coef);
			}
		}
		#endregion

		#region Fermeture du code HTML
		/// <summary>
		/// Fermeture du code HTML
		/// </summary>
		/// <returns></returns>
		private string GetEnd(){
			StringBuilder html = new StringBuilder(10000);
			html.Append("</table>");
			html.Append("</body>");
			html.Append("</HTML>");
			return(html.ToString());
		}
		#endregion

		#region En tête du code HTML
        /// <summary>
        /// En tête du code HTML
        /// </summary>
        /// <param name="htmlHeader">L'en tête du tableau (Media Plan)</param>
        /// <returns></returns>
        private string GetHeader(StringBuilder htmlHeader) {
            StringBuilder html = new StringBuilder(10000);

            html.Append(htmlHeader.ToString());
            html.Append("\r\n\t<tr>\r\n\t\t<td>");
            return (html.ToString());
        }
		/// <summary>
		/// En tête du code HTML
		/// </summary>
		/// <param name="htmlHeader">L'en tête du tableau (Media Plan)</param>
        /// <param name="resultTableHeader">Result Table Header</param>
        /// <returns>En tête du code HTML</returns>
        private string GetHeader(StringBuilder htmlHeader, string resultTableHeader) {
			StringBuilder html = new StringBuilder(10000);
			
			html.Append(htmlHeader.ToString());
            html.Append(resultTableHeader);
			html.Append("\r\n\t<tr>\r\n\t\t<td>");
			return(html.ToString());
		}
        /// <summary>
        /// Result Table Header
        /// </summary>
        /// <param name="htmlHeader">Table Result HTML</param>
        /// <returns>Table Result Header HTML</returns>
        private string GetResultTableHeader(string htmlHeader) {
            
            int startIndex = 0;
            
            for(int i=0; i<3; i++)
                startIndex = htmlHeader.IndexOf("</tr>", startIndex+1);

            return (htmlHeader.Substring(0,startIndex));
        
        }
		#endregion

		#region En tête du code HTML (Visuel)
		/// <summary>
		/// En tête du code HTML
		/// </summary>
		/// <returns></returns>
		private string GetHeader(){

			StringBuilder html = new StringBuilder(10000);
            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

			html.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
			html.Append("<HTML>");
			html.Append("<HEAD>");
			html.Append("<META http-equiv=\"Content-Type\" content=\"text/html; charset="+charSet+"\">");
			html.Append("<meta content=\"Microsoft Visual Studio .NET 7.1\" name=\"GENERATOR\">");
			html.Append("<meta content=\"C#\" name=\"CODE_LANGUAGE\">");
			html.Append("<meta content=\"JavaScript\" name=\"vs_defaultClientScript\">");
			html.Append("<meta content=\"http://schemas.microsoft.com/intellisense/ie5\" name=\"vs_targetSchema\">");
			html.Append("<LINK href=\"" + _config.WebServer + "/App_Themes" + "/" + themeName + "/Css/AdExpress.css\" type=\"text/css\" rel=\"stylesheet\">");
			html.Append("<LINK href=\"" + _config.WebServer  + "/App_Themes" + "/" + themeName + "/Css/GenericUI.css\" type=\"text/css\" rel=\"stylesheet\">");
			html.Append("<LINK href=\"" + _config.WebServer  + "/App_Themes" + "/" + themeName + "/Css/MediaSchedule.css\" type=\"text/css\" rel=\"stylesheet\">");
            html.Append("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
			html.Append("<meta http-equiv=\"expires\" content=\"0\">");
			html.Append("<meta http-equiv=\"pragma\" content=\"no-cache\">");
			html.Append("<meta name=\"Cache-control\" content=\"no-cache\">");
			html.Append("</HEAD>");
			html.Append("<body>");
			html.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
            html.Append("\r\n\t<tr>\r\n\t\t<td>");

			return(html.ToString());
		}
		#endregion

		#region Création et Insertion d'une image dans une nouvelle page du PDF
		/// <summary>
		/// Création et Insertion d'une image dans une nouvelle page du PDF
		/// </summary>
		/// <param name="html">Le code HTML</param>
		/// <param name="i">Index d'une partie de code</param>
		/// <param name="partieHTML">Une partie du code HTML</param>
		private void SnapShots(ref StringBuilder html, int i, ArrayList partieHTML,bool version, ref double coef){
		
			CHtmlSnapClass snap;
			IntPtr  hBitmap;
			string filePath="";

			#region Création et Insertion d'une image dans une nouvelle page du PDF

			this.NewPage();

			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

			hBitmap = IntPtr.Zero;
			snap = new CHtmlSnapClass();
			snap.SetTimeOut(100000);
			snap.SetCode("21063505C78EB32A");
			snap.SnapHtmlString(html.ToString(), "*");

			html=new StringBuilder(10000);
						
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
			double X1=10;

			imgG = Image.FromFile(filePath);
			imgI = this.AddImageFromFilename(filePath,TxImageCompressionType.itcFlate);

			double w = 0;
			if((i<(partieHTML.Count-1))||(partieHTML.Count==1))	{
				w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
				coef = Math.Min((double)1.0,w);
				w = ((double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height);
				coef = Math.Min((double)coef, w);
			}

			if(version) {
				if(i==(partieHTML.Count-1))	{
					this.PDFPAGE_ShowImage(imgI,
						X1, 64,
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, this.PDFPAGE_Height/2 - (coef * imgG.Height / 2),
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
			}
			else{
				if(i==(partieHTML.Count-1)) {
					this.PDFPAGE_ShowImage(imgI,
						X1, 40,
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, this.PDFPAGE_Height/2 - (coef * imgG.Height / 2),
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
			}

			imgG.Dispose();
			imgG = null;

			#region Clean File
			File.Delete(filePath);
			#endregion

			#endregion
		}

		#endregion

		#region Transformation du code HTML en une nouvelle page PDF
		/// <summary>
		/// Transformation du code HTML en une nouvelle page PDF
		/// </summary>
		/// <param name="html">Le code HTML</param>
		/// <param name="i">Index d'une partie de code</param>
		/// <param name="partieHTML">Une partie du code HTML</param>
		private int HtmlToPDF(ref StringBuilder html, int i, ArrayList partieHTML,bool version) {
		
			StreamWriter sw = null;

			#region Trasformation du code HTML

			this.NewPage();

			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;

			string workFile = GetWorkDirectory() + @"\SessionParameter" + _rqDetails["id_static_nav_session"].ToString() + ".htm";
			
			sw = File.CreateText(workFile);
			
			sw.Write(html.ToString());

			
			try {
				html=new StringBuilder(10000);
							
				#region Html file loading
				TNS.AdExpress.Anubis.Common.Functions.CloseHtmlFile(sw);
				HTML2PDFClass htmlTmp = new HTML2PDFClass();
				htmlTmp.MarginLeft = Convert.ToInt32(this.LeftMargin);
				htmlTmp.MarginTop = Convert.ToInt32(this.WorkZoneTop);
				htmlTmp.MarginBottom = Convert.ToInt32(this.PDFPAGE_Height - this.WorkZoneBottom + 1);
				htmlTmp.StartHTMLEngine(_config.Html2PdfLogin, _config.Html2PdfPass);
				htmlTmp.ConnectToPDFLibrary (this);
				htmlTmp.LoadFromFile(workFile);
				htmlTmp.ConvertAll();
				htmlTmp.ClearCache();
				htmlTmp.ConvertAll();
				htmlTmp.DisconnectFromPDFLibrary ();
				#endregion

				#region Clean File
				File.Delete(workFile);
				#endregion

				return this.GetCurrentPageIndex();
			}
			catch(System.Exception e) {
				try {
					sw.Close();
				}
				catch(System.Exception e2){}
				throw(new MiysisPdfException("Unable to build the session parameter page for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e));
			}

			#endregion
		}

		#endregion

		#region BuildVersionPDF(String title) 
		/// <summary> 
		/// Render all versions controls
		/// </summary>
		/// <returns>Html code</returns>
		protected void BuildVersionPDF(ArrayList versionsUIs,int startIndex) {

			ArrayList partitHTMLVersion = new ArrayList();
			StringBuilder htmltmp=new StringBuilder(1000);
			int end=0;
			double indexPage=0;
			int indexFirstCase=0, indexSecondCase=0, indexThirdCase=0, X1=0, Y1=0, nbLines=0;
			bool first=true, second=true;
            long nbVisuel = 0;

			if (versionsUIs != null) {
				
					foreach(VersionDetailUI item in versionsUIs) {

                        if(item.ExportVersion != null)
                            nbVisuel = item.ExportVersion.NbVisuel;
                        else if(item.ExportMDVersion != null)
                            nbVisuel = item.ExportMDVersion.NbVisuel;
                        else if(item.ExportOutdoorVersion != null)
                            nbVisuel = item.ExportOutdoorVersion.NbVisuel;

						if (nbVisuel==1)	{
							
							if(indexPage==0) {
								switch((imgPositionItems)indexFirstCase) {
									case imgPositionItems.TOP_LEFT: X1=29; Y1=77; break;
									case imgPositionItems.TOP_RIGHT: X1=557; Y1=77; break;
									case imgPositionItems.BOTTOM_LEFT: X1=29; Y1=404; break;
									case imgPositionItems.BOTTOM_RIGHT: X1=557; Y1=404; break;
								}
							}
							else {
								switch((imgPositionItems)indexFirstCase) {
									case imgPositionItems.TOP_LEFT: X1=27; Y1=75; break;
									case imgPositionItems.TOP_RIGHT: X1=554; Y1=75; break;
									case imgPositionItems.BOTTOM_LEFT: X1=27; Y1=401; break;
									case imgPositionItems.BOTTOM_RIGHT: X1=554; Y1=401; break;
								}
							}				
							indexFirstCase++;
							InsertImageInPDF(0,item,indexPage,startIndex,X1,Y1);
							if((indexFirstCase==1)||(indexFirstCase==3)) {
								nbLines++;
							}
							if (indexFirstCase==4) {
								indexPage++;
								indexFirstCase=0;
							}
						}
						else if (nbVisuel<5) {
							
							if(first) {
								if((nbLines%2)==0) {
									indexPage=(nbLines/2);
								}
								else {
									indexPage=Math.Ceiling((double)(nbLines/2));
									indexSecondCase--;	
								}
									first=false;
							}

							if(indexPage==0) {
								switch((imgPosition)indexSecondCase) {
									case imgPosition.TOP: X1=25; Y1=70; break;
									case imgPosition.DYNAMIC:
									case imgPosition.BOTTOM: X1=25; Y1=393; break;
								}
							}
							else {
								switch((imgPosition)indexSecondCase) {
									case imgPosition.TOP: X1=25; Y1=70; break;
									case imgPosition.DYNAMIC:		
									case imgPosition.BOTTOM: X1=25; Y1=393; break;
								}
							}

							InsertImageInPDF(0,item,indexPage,startIndex,X1,Y1);
							if(indexSecondCase==-1)
								indexPage++;
							indexSecondCase++;
							nbLines++;
							if (indexSecondCase==2) {
								indexPage++;
								indexSecondCase=0;
							}
						}
						else if (nbVisuel>=5) {
						
							end=(int)Math.Ceiling((double)nbVisuel/4);
						
							for(int i=0;i<end;i++) {

								if((first)||(second)) {
									if((nbLines%2)==0) {
										indexPage=(nbLines/2);
									}
									else {
										indexPage=Math.Ceiling((double)(nbLines/2));
										indexThirdCase--;	
									}
									first=false;
									second=false;
								}

								if(indexPage==0) {
									switch((imgPosition)indexThirdCase) {
										case imgPosition.TOP: X1=25; Y1=70; break;
										case imgPosition.DYNAMIC:
										case imgPosition.BOTTOM: X1=25; Y1=397; break;
									}
								}
								else {
									switch((imgPosition)indexThirdCase) {
										case imgPosition.TOP: X1=25; Y1=70; break;
										case imgPosition.DYNAMIC:		
										case imgPosition.BOTTOM: X1=25; Y1=394; break;
									}
								}
														
								if(i==0) {
									InsertImageInPDF(0,item,indexPage,startIndex,X1,Y1);
								}
								else {
									if(i==end-1) {
										InsertImageInPDF(i+(4*i),item,indexPage,startIndex,X1,Y1);
									}
									else {
										InsertImageInPDF(i+(4*i),item,indexPage,startIndex,X1,Y1);
									}
								}
								
								if(indexThirdCase==-1)
									indexPage++;
								indexThirdCase++;
							
								if (indexThirdCase==2) {
									indexPage++;
									indexThirdCase=0;
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
		protected void InsertImageInPDF(Int64 index,VersionDetailUI item,double indexPage,int startIndex, int X1, int Y1) {
			string[] pathes = null;
			Int64 lastIndex=index+5;
			bool firstInsertion=true;
            bool marketingDirect = false;
            bool outdoor = false;
            Int64 nbVisuel = 0;

			this.SetCurrentPage((int)indexPage+startIndex);
			
			if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE) { 
				pathes = item.ExportVersion.Path.Split(',');
                nbVisuel = item.ExportVersion.NbVisuel;
			}
			else {
                if (item.ExportVersion != null) {
                    pathes = item.ExportVersion.Path.Split(',');
                    nbVisuel = item.ExportVersion.NbVisuel;
                }
                else if (item.ExportMDVersion != null) {
                    pathes = item.ExportMDVersion.Path.Split(',');
                    nbVisuel = item.ExportMDVersion.NbVisuel;
                    marketingDirect = true;
                }
                else if (item.ExportOutdoorVersion != null) {
                    pathes = item.ExportOutdoorVersion.Path.Split(',');
                    nbVisuel = item.ExportOutdoorVersion.NbVisuel;
                    outdoor = true;
                }

			}

			Int64 end=0;
				
			if(pathes.Length<lastIndex)
				end=pathes.Length;
			else
				end=lastIndex;

			if((pathes.Length%5)==0) {
				if(pathes.Length>lastIndex) {
					end=lastIndex;
				} 
				else if(pathes.Length==lastIndex) {
					end=lastIndex-1;
				} 
				else {
					end=pathes.Length;
					index--;
				}
			}
			
			Image imgG ;
			int imgI =0;
			string path="";
			
			for(Int64 i=index;i<end;i++) {
				path="";
				path=pathes[i].Replace("/imagette","");
				path=pathes[i].Replace("/ImagesPresse","");

                if (marketingDirect){
                    imgG = Image.FromFile(CreationServerPathes.LOCAL_PATH_MD_IMAGE + path);
                    imgI = this.AddImageFromFilename(CreationServerPathes.LOCAL_PATH_MD_IMAGE + path, TxImageCompressionType.itcFlate);
                }
                else if (outdoor) {
                    imgG = Image.FromFile(CreationServerPathes.LOCAL_PATH_OUTDOOR + path);
                    imgI = this.AddImageFromFilename(CreationServerPathes.LOCAL_PATH_OUTDOOR + path, TxImageCompressionType.itcFlate);
                }
                else {
                    imgG = Image.FromFile(CreationServerPathes.LOCAL_PATH_IMAGE + path);
                    imgI = this.AddImageFromFilename(CreationServerPathes.LOCAL_PATH_IMAGE + path, TxImageCompressionType.itcFlate);
                }

				double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
				double coef = Math.Min((double)1.0,w);
				w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
				coef = Math.Min((double)coef, w);
				
				if((end-index==4)&&(nbVisuel!=5)) {
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						192,296,0);
					X1+=192+1;
				}
				else if(end-index==5){
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						213,300,0);
					X1+=213+1;
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						223,296,0);
					X1+=223+1;
				} 
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
			throw new Exceptions.MiysisPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
		}
		#endregion

	}
}
