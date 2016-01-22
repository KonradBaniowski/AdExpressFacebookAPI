#region Information
// Author : Y.rkaina
// Creation : 25/08/2006
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

using TNS.AdExpress.Anubis.Mnevis.Common;
using TNS.AdExpress.Anubis.Mnevis.Exceptions;

using TNS.AdExpress.Web.UI.Results;

using TNS.AdExpress.Constantes.Customer;
using CstRights = TNS.AdExpress.Constantes.Customer.Right;
using CstResult = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.BusinessFacade.Results;
using TNS.AdExpress.Web.BusinessFacade.Selections.Products;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Web.UI;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.FrameWork;
using TNS.FrameWork.Net.Mail;

using PDFCreatorPilotLib;
using TNS.FrameWork.DB.Common;

using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;
using TNS.Ares.Pdf;
using TNS.FrameWork.WebTheme;
#endregion

namespace TNS.AdExpress.Anubis.Mnevis.BusinessFacade{
	/// <summary>
	/// Generate the PDF document for Appm module (Media Plan).
	/// </summary>
	public class MnevisPdfSystem:Pdf {

		#region Variables
		private IDataSource _dataSource = null;
		/// <summary>
		/// Mnevis Configuration (usefull for PDF layout)
		/// </summary>
		private MnevisConfig _config = null;
		/// <summary>
		/// Customer Client request
		/// </summary>
		private DataRow _rqDetails = null;
		/// <summary>
		/// WebSession to process
		/// </summary>
		private WebSession _webSession = null;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MnevisPdfSystem(IDataSource dataSource, MnevisConfig config, DataRow rqDetails, WebSession webSession, Theme theme):
        base(theme.GetStyle("Mnevis")) {
		this._dataSource = dataSource;
		this._config = config;
		this._rqDetails = rqDetails;
		this._webSession = webSession;
		}
		#endregion

		#region Init
		internal string Init()
		{
			try
			{
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
			catch(System.Exception e)
			{
				throw(e);
			}
		}
		#endregion

		#region Fill
		internal void Fill() {

			try {

				#region MainPage
				MainPageDesign();
				#endregion

				#region Session Parameters
				SessionParameter();
				#endregion

				#region Impression
				MediaPlanImpression();
				#endregion
				
				#region Header and Footer
				this.AddHeadersAndFooters(
                    _webSession,
                    true,
					true,
                    GestionWeb.GetWebWord(2535, _webSession.SiteLanguage) + " - " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern),
					0,-1);
				#endregion

			}
			catch(System.Exception e) {
				throw(e);
			}
		}
		#endregion

		#region Send
		internal void Send(string fileName) {
			ArrayList to = new ArrayList();
			foreach(string s in _webSession.EmailRecipient) {
				to.Add(s);
			}
			SmtpUtilities mail = new SmtpUtilities(_config.CustomerMailFrom, to,
				Text.SuppressAccent(GestionWeb.GetWebWord(2010,_webSession.SiteLanguage)),
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
		private string GetFileName(DataRow rqDetails,ref string shortName)
		{
			string pdfFileName;

			try
			{

				pdfFileName = this._config.PdfPath;
				pdfFileName += @"\" + rqDetails["ID_LOGIN"].ToString() ;

				if(!Directory.Exists(pdfFileName))
				{
					Directory.CreateDirectory(pdfFileName);
				}
				shortName = DateTime.Now.ToString("yyyyMMdd_") 
					+ rqDetails["id_static_nav_session"].ToString()
					+ "_"
					+ TNS.Ares.Functions.GetRandomString(30,40);

				pdfFileName += @"\" + shortName + ".pdf";

				string checkPath = 	Regex.Replace(pdfFileName, @"(\.pdf)+", ".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);


				int i = 0;
				while(File.Exists(checkPath))
				{
					if(i<=1)
					{
						checkPath = Regex.Replace(pdfFileName, @"(\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					else
					{
						checkPath = Regex.Replace(pdfFileName, "(_"+i+@"\.pdf)+", "_"+(i+1)+".pdf", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					}
					i++;
				}
				return checkPath;
			}
			catch(System.Exception e)
			{
				throw(new MnevisPdfException("Unable to generate file name for request " + _rqDetails["id_static_nav_session"].ToString() +".",e));
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

            Picture picture = ((Picture)Style.GetTag("pictureTitle"));
            string imgPath = string.Empty;

            if (File.Exists(picture.Path)) {
                imgPath = picture.Path;
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path)) {
                imgPath = AppDomain.CurrentDomain.BaseDirectory + @"\" + picture.Path;
            }
            else {
                imgPath = picture.Path;
            }
			Image imgG = Image.FromFile(imgPath);
			
			double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
			double coef = Math.Min((double)1.0,w);
			w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
			coef = Math.Min((double)coef, w);
			
			int imgI = this.AddImageFromFilename(imgPath,TxImageCompressionType.itcFlate);
			this.PDFPAGE_ShowImage(imgI,
				(double)(this.PDFPAGE_Width/2 - coef*imgG.Width/2), (double)(this.WorkZoneBottom - coef*imgG.Height - 100),
				(double)(coef*imgG.Width),(double)(coef*imgG.Height),0);

            Style.GetTag("bigTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);

            this.PDFPAGE_TextOut(((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(GestionWeb.GetWebWord(1587, _webSession.SiteLanguage))) / 2), 
				((this.PDFPAGE_Height)/4)-75,0,GestionWeb.GetWebWord(1587,_webSession.SiteLanguage));

            Style.GetTag("bigTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
			
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(GestionWeb.GetWebWord(2535, _webSession.SiteLanguage)))/2,
                (this.PDFPAGE_Height) / 4, 0, "(" + GestionWeb.GetWebWord(2535, _webSession.SiteLanguage) + ")");			

			string str = string.Empty;
            str = GestionWeb.GetWebWord(1922, _webSession.SiteLanguage) + " " + Dates.DateToString(DateTime.Now, _webSession.SiteLanguage, TNS.AdExpress.Constantes.FrameWork.Dates.Pattern.customDatePattern);


            Style.GetTag("createdTitle").SetStylePdf(this, TxFontCharset.charsetANSI_CHARSET);
			this.PDFPAGE_TextOut((this.PDFPAGE_Width - this.PDFPAGE_GetTextWidth(str))/2, 
				1*this.PDFPAGE_Height/3,0,str);

		}
		#endregion

		#region MediaPlanIpression
        /// <summary>
        /// MediaPlanImpression
        /// </summary>
		private void MediaPlanImpression() {
	
			#region GETHTML
			String title="";
			StringBuilder htmlHeader=new StringBuilder(10000);
			MediaPlanResultData result=null;
			ArrayList partieHTML = new ArrayList();
			ArrayList versionsUIs = new ArrayList();
			Int64 module = _webSession.CurrentModule;
			int startIndex=0;
			ArrayList partieHTMLVersion = new ArrayList();
            string charSet = WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset;
            string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;

			try {

				#region Paramétrage des dates
				//Formatting date to be used in the tabs which use APPM Press table
				int dateBegin = int.Parse(TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType).ToString("yyyyMMdd"));
				int dateEnd = int.Parse(TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType).ToString("yyyyMMdd"));
				#endregion

				#region targets
				//base target
				Int64 idBaseTarget=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CstRights.type.aepmBaseTargetAccess));
				//additional target
				Int64 idAdditionalTarget=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CstRights.type.aepmTargetAccess));									
				#endregion

				#region Wave
				Int64 idWave=Int64.Parse(_webSession.GetSelection(_webSession.SelectionUniversAEPMWave,CstRights.type.aepmWaveAccess));									
				#endregion

				#region Obtention du résultat du calendrier d'action
				result=TNS.AdExpress.Web.UI.Results.APPM.MediaPlanUI.GetWithVersionExportHTML(_webSession,_dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,false,ref htmlHeader, ref partieHTML);				
				#endregion

				ExportVersionsVehicleUI exportVersionsVehicleUI=new ExportVersionsVehicleUI(_webSession,result.VersionsDetail,TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press);
				VersionsPluriMediaUI versionsUI=new VersionsPluriMediaUI(_webSession,result.VersionsDetail);
				title=GestionWeb.GetWebWord(1972, this._webSession.SiteLanguage);
				partieHTMLVersion=versionsUI.GetAPPMHtmlExport(_dataSource,title,ref versionsUIs);
				startIndex = DecoupageVersionHTML(partieHTMLVersion,true);
				BuildVersionPDF(versionsUIs,startIndex);
				DecoupageHTML(partieHTML,false);
			}
			catch(System.Exception err){
				throw(new MnevisPdfException("Unable to process Media Schedule Alert export result for request " + _rqDetails["id_static_nav_session"].ToString() + ".",err)); 
			}
			finally{
				_webSession.CurrentModule = module;
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
		private void DecoupageHTML(StringBuilder html,string strHtml, int nbLines,bool version) {

			int startIndex=0,oldStartIndex=0,tmp;
			ArrayList partieHTML = new ArrayList();
			double coef=0;

			int start=0;

			while((startIndex < strHtml.Length)&&(startIndex!=-1)) {
				tmp=0;
				
				if(start==0) {
					while((tmp<nbLines+4)&&(startIndex<strHtml.Length)&&(startIndex!=-1)) {
						startIndex=strHtml.IndexOf("<tr>",startIndex+1);
						tmp++;
					}
					start=1;
				}
				else {
					while((tmp<nbLines)&&(startIndex<strHtml.Length)&&(startIndex!=-1)) {
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
			
			for(int i=0; i<partieHTML.Count; i++) {
				if(i>0)
                    html.Append("\r\n\t<tr>\r\n\t\t<td>");
				html.Append(partieHTML[i].ToString());
                html.Append("</table>");
				if(version)
					SnapShots(html.ToString(),i,partieHTML.Count,true, ref coef);
				else
					SnapShots(html.ToString(),i,partieHTML.Count,false, ref coef);
                html = new StringBuilder();
			}
		}
		/// <summary>
		/// Découpage du code HTML pour l'export PDF du plan média (Visuels)
		/// </summary>
		/// <param name="html">Le code HTML à générer</param>
		/// <param name="strHtml">Le code HTML à découper</param>
		private void DecoupageHTML(StringBuilder html,string strHtml,bool version)
		{

			int startIndex=0,oldStartIndex=0,tmp;
			ArrayList partieHTML = new ArrayList();
			StringBuilder htmltmp=new StringBuilder(1000);
			htmltmp.Append(html.ToString());
			double coef=0;

			while((startIndex < strHtml.Length)&&(startIndex!=-1))
			{
				tmp=0;
				
				while((tmp<1)&&(startIndex<strHtml.Length)&&(startIndex!=-1))
				{
					startIndex=strHtml.IndexOf("<br>",startIndex+1);
					tmp++;
				}

				if(startIndex==-1)
					partieHTML.Add(strHtml.Substring(oldStartIndex,strHtml.Length-oldStartIndex));
				else
					partieHTML.Add(strHtml.Substring(oldStartIndex,startIndex-oldStartIndex));
				oldStartIndex=startIndex;
			}
			
			for(int i=0; i<partieHTML.Count-1; i++)
			{
                if (i > 0) {
                    html.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
                    html.Append("\r\n\t<tr>\r\n\t\t<td>");
                }
				htmltmp.Append(partieHTML[i].ToString());
                htmltmp.Append("</table>");
                if (version)
                    SnapShots(htmltmp.ToString(), i, partieHTML.Count, true, ref coef);
                else
                    SnapShots(htmltmp.ToString(), i, partieHTML.Count, false, ref coef);
                htmltmp = new StringBuilder(); 
			}
		}
		/// <summary>
		/// Découpage du code HTML pour l'export PDF du plan média (Visuels)
		/// </summary>
		/// <param name="html">Le code HTML à générer</param>
		/// <param name="strHtml">Le code HTML à découper</param>
		private void DecoupageHTML(ArrayList result,bool version) {
		
			ArrayList partieHTML = new ArrayList();
			StringBuilder htmltmp=new StringBuilder(1000);
			double coef=0;
			//htmltmp.Append(html.ToString());

			for(int i=0; i<result.Count; i++) {
                htmltmp.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
                htmltmp.Append("\r\n\t<tr>\r\n\t\t<td>");
				htmltmp.Append(result[i].ToString());
                htmltmp.Append("</table>");
				if(version)
					SnapShots(htmltmp.ToString(),i,result.Count,true, ref coef);
				else
					SnapShots(htmltmp.ToString(),i,result.Count,false, ref coef);
                htmltmp = new StringBuilder();
			}
		}
		/// <summary>
		/// Découpage du code HTML pour l'export PDF du plan média (Visuels)
		/// </summary>
		/// <param name="html">Le code HTML à générer</param>
		/// <param name="strHtml">Le code HTML à découper</param>
		private int DecoupageVersionHTML(ArrayList result,bool version) {
		
			ArrayList partieHTML = new ArrayList();
			StringBuilder htmltmp=new StringBuilder(1000);
			int startIndex=0;
			//htmltmp.Append(html.ToString());

			for(int i=0; i<result.Count; i++) {
                htmltmp.Append("<table cellSpacing=\"0\" cellPadding=\"0\"  border=\"0\">");
                htmltmp.Append("\r\n\t<tr>\r\n\t\t<td>");
				htmltmp.Append(result[i].ToString()); 
				htmltmp.Append("</table>");
                if (version) {
                    if (i == 0) {
                        startIndex = this.ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                    }
                    else {
                        this.ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                    }
                }
                else {
                    this.ConvertHtmlToPDF(htmltmp.ToString(),
                                                WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                                                WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                                                _config.WebServer,
                                                _config.Html2PdfLogin,
                                                _config.Html2PdfPass);
                }
                htmltmp = new StringBuilder();
			}
			return startIndex;
		}
		#endregion

		#region Création et Insertion d'une image dans une nouvelle page du PDF
		/// <summary>
		/// Création et Insertion d'une image dans une nouvelle page du PDF
		/// </summary>
		/// <param name="html">Le code HTML</param>
		/// <param name="i">Index d'une partie de code</param>
		/// <param name="partieHTML">Une partie du code HTML</param>
        private void SnapShots(string html, int currentIndexPart, int nbPart, bool version, ref double coef)
		{
		
			string filePath="";

			#region Création et Insertion d'une image dans une nouvelle page du PDF

			this.NewPage();
			this.PDFPAGE_Orientation = TxPDFPageOrientation.poPageLandscape;
						
			//here demo how to use GetImageBytes.
            byte[] data = this.ConvertHtmlToSnapJpgByte(html,
            WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
            WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
            _config.WebServer);

			filePath=Path.GetTempFileName();
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

            if ((currentIndexPart < (nbPart - 1)) || (nbPart == 1)) {
				w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
				coef = Math.Min((double)1.0,w);
				w = ((double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height);
				coef = Math.Min((double)coef, w);
			}

			if(version){

                if (currentIndexPart == (nbPart - 1)) {
					this.PDFPAGE_ShowImage(imgI,
						X1, 54,
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, (this.PDFPAGE_Height/2 - (coef * imgG.Height / 2))+5,
						coef * imgG.Width,
						coef * imgG.Height,
						0);
				}
			
			}
			else{

                if (currentIndexPart == (nbPart - 1)) {
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

			imgG.Dispose();
			imgG = null;

			#region Clean File
			File.Delete(filePath);
			#endregion

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

			if (versionsUIs != null) {
				
				int columnIndex = 0;
				foreach(VersionDetailUI item in  versionsUIs) {

					if (item.ExportAPPMVersion.NbVisuel<4)	{
						InsertImageInPDF(0,item,indexPage,startIndex);
						indexPage++;
					}
					else if (item.ExportAPPMVersion.NbVisuel>=4) {

						end=(int)Math.Ceiling((double)item.ExportAPPMVersion.NbVisuel/4);
						if(item.ExportAPPMVersion.NbVisuel%4==0)
							end=(int)(item.ExportAPPMVersion.NbVisuel/4)+1;
						
						for(int i=0;i<end;i++) {
											
							if(i==0) {
								InsertImageInPDF(0,item,indexPage,startIndex);
								indexPage++;
							}
							else {
								if(i==end-1) {
									InsertImageInPDF(i+(3*i),item,indexPage,startIndex);
									indexPage++;
								}
								else {
									InsertImageInPDF(i+(3*i),item,indexPage,startIndex);
									indexPage++;
								}
							}
							columnIndex++;
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
		protected void InsertImageInPDF(Int64 index,VersionDetailUI item,double indexPage,int startIndex) {
			string[] pathes = null;
			Int64 lastIndex=index+4;
			bool firstInsertion=true;

			if((indexPage%2)==0) {
				indexPage=(indexPage/2)+startIndex;
				firstInsertion=true;
			}
			else {
				indexPage=Math.Ceiling((double)indexPage/2)+(startIndex-1);
				firstInsertion=false;
			}

			this.SetCurrentPage((int)indexPage);
			
			if (_webSession.CurrentModule==TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE) { 
				pathes = item.ExportAPPMVersion.Path.Split(',');
			}
			else {
				pathes = item.ExportAPPMVersion.Path.Split(',');
			}

			Int64 end=0;
				
			if(pathes.Length<index+4)
				end=pathes.Length;
			else
				end=index+4;

			if((pathes.Length%4)==0) {
				if(pathes.Length > lastIndex) {
					end=lastIndex;
				}
				else if(pathes.Length == lastIndex) {
					end=lastIndex-1;
				}
				else {
					end=pathes.Length;
					index--;
				}
			}
			
			Image imgG ;
			double X1=0,Y1=0;
			int imgI =0;
			string path="";
			if(!firstInsertion)
				if(indexPage==startIndex)
					Y1=403;
				else
					Y1=384;
			else if(indexPage==startIndex)
					Y1=70;
				else
					Y1=51;
			X1=25;

			for(Int64 i=index;i<end;i++) {
				path="";
				path=pathes[i].Replace("/imagette","");
				path=pathes[i].Replace("/ImagesPresse","");
                imgG = Image.FromFile(_config.ScanPath + path);
                imgI = this.AddImageFromFilename(_config.ScanPath + path, TxImageCompressionType.itcFlate);

				double w = (double)(this.PDFPAGE_Width - this.LeftMargin - this.RightMargin)/(double)imgG.Width;
				double coef = Math.Min((double)1.0,w);
				w = (double)(this.WorkZoneBottom - this.WorkZoneTop)/(double)imgG.Height;
				coef = Math.Min((double)coef, w);
				
				if((end-index==3)&&(item.ExportAPPMVersion.NbVisuel%4!=0)) {
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						205,303,0);
					X1+=205+1;
				}
				else {
					this.PDFPAGE_ShowImage(imgI,
						X1, Y1,
						235,303,0);
					X1+=235+1;
				} 
			}
		} 
		#endregion

		#region SessionParameter
		/// <summary>
		/// Session parameter design
		/// </summary>
		private void SessionParameter() 
		{

            string classCss = string.Empty;
            StringBuilder html = new StringBuilder();

			try {

				#region Title
				html.Append("<TABLE cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
				html.Append("<TR height=\"25\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"14\">");
				html.Append("<TD class=\"txtViolet14Bold\">" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1752,_webSession.SiteLanguage)) + "</TD>");
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
				html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1755, _webSession.SiteLanguage)) + " :</TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"20\">");
				html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;&nbsp;&nbsp;"
					+ HtmlFunctions.GetPeriodDetail(_webSession)
					+ "</TD>");
				html.Append("</TR>");
				#endregion

				#region Wave
				//TODO WAVE
				html.Append("<TR height=\"7\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"1\" class=\"lightPurple\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR>");
				html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1771, _webSession.SiteLanguage)) + " :</TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"20\">");
				html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;&nbsp;&nbsp;"
					+ ((LevelInformation)_webSession.SelectionUniversAEPMWave.Nodes[0].Tag).Text
					+ "</TD>");
				html.Append("</TR>");
				#endregion

				#region Targets
				html.Append("<TR height=\"7\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"1\" class=\"lightPurple\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR>");
				html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1757, _webSession.SiteLanguage)) + " :</TD>");
				html.Append("</TR>");
				//Base target
				string targets = "'" + _webSession.GetSelection(_webSession.SelectionUniversAEPMTarget,CstRights.type.aepmTargetAccess) + "'";
				//Wave
				string idWave = ((LevelInformation)_webSession.SelectionUniversAEPMWave.Nodes[0].Tag).ID.ToString();
				DataSet ds = TargetListDataAccess.GetAEPMTargetListFromIDSDataAccess(idWave, targets, _webSession.Source);
				foreach(DataRow r in ds.Tables[0].Rows) {
					html.Append("<TR height=\"20\">");
					html.Append("<TD class=\"txtViolet11\" vAlign=\"top\">&nbsp;&nbsp;&nbsp;");
					html.Append(r["target"].ToString());
					html.Append("</TD>");
					html.Append("</TR>");
				}
				ds.Dispose();
				ds = null;
				#endregion

				#region Products
				const int nbLineByPage = 32;
				int currentLine = 12;
				html.Append("<TR height=\"7\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR height=\"1\" class=\"lightPurple\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR>");
				html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1759, _webSession.SiteLanguage)) + " :</TD>");
				html.Append("</TR>");
				//reference
				html.Append("<TR height=\"14\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR>");
				html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1677, _webSession.SiteLanguage)) + " :</TD>");
				html.Append("</TR>");
				html.Append("<TR align=\"center\">");
				html.Append("<TD><br>");
				if (_webSession.PrincipalProductUniverses != null && _webSession.PrincipalProductUniverses.Count > 0)
                    html.Append(DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[0], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.Source, 600, true, nbLineByPage, ref currentLine));

				html.Append("</TD>");
				html.Append("</TR>");
				//competitor
				html.Append("<TR height=\"14\">");
				html.Append("<TD></TD>");
				html.Append("</TR>");
				html.Append("<TR>");
				if (_webSession.PrincipalProductUniverses.Count > 1) {
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1678, _webSession.SiteLanguage)) + " :</TD>");
				}
				else {
					html.Append("<TD class=\"txtViolet11Bold\">&nbsp;" + Convertion.ToHtmlString(GestionWeb.GetWebWord(1668, _webSession.SiteLanguage)) + " :</TD>");
				}
				html.Append("</TR>");
				html.Append("<TR align=\"center\">");
				html.Append("<TD><br>");
				if (_webSession.PrincipalProductUniverses.Count > 1) {
                    html.Append(DisplayUniverse.ToHtml(_webSession.PrincipalProductUniverses[1], _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.Source, 600, true, nbLineByPage, ref currentLine));
				}
				else {
					ds = GroupSystem.ListFromSelection(_dataSource, _webSession);
					html.Append("<table class=\"txtViolet11Bold\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">");
					for(int i =0; i < ds.Tables[0].Rows.Count; i++) {
						html.Append("<tr>");
						if((i+1) < ds.Tables[0].Rows.Count) {
                            classCss = "violetBorderWithoutBottom";
						}
						else {
                            classCss = "violetBorder";
						}
                        html.Append("<td class=\"" + classCss + "\">&nbsp;&nbsp;");
						html.Append(ds.Tables[0].Rows[i][0].ToString());
						html.Append("</td>");
						html.Append("</tr>");
					}
					ds.Dispose();
					ds = null;
				}
				html.Append("</TD>");
				html.Append("</TR>");
				#endregion

                this.ConvertHtmlToPDF(html.ToString(),
                    WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Charset,
                    WebApplicationParameters.Themes[_webSession.SiteLanguage].Name,
                    _config.WebServer,
                    _config.Html2PdfLogin,
                    _config.Html2PdfPass);
			}
			catch(System.Exception e) {
				throw(new MnevisPdfException("Unable to build the session parameter page for request " + _rqDetails["id_static_nav_session"].ToString() + ".",e));
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
		private void mail_mailKoHandler(object source, string message) 
		{
			throw new Exceptions.MnevisPdfException("Echec lors de l'envoi mail client pour la session " + _webSession.IdSession + " : " + message);
		}
		#endregion

	}
}
