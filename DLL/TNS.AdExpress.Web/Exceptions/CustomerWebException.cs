#region Informations
// Auteur: G. Facon
// Date de création: 23/12/2004 
// Date de modification: 23/12/2004 
#endregion



using System;
using System.Windows.Forms;
using System.Text;
using System.Globalization;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork.Date;
using CstCustomerSession = TNS.AdExpress.Constantes.Web.CustomerSessions;
using Webfunctions = TNS.AdExpress.Web.Functions;
using TNSMail = TNS.FrameWork.Net.Mail;
using TNS.FrameWork;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Exceptions{


    /// <summary>
    /// Classe gestion exceptions Avec envoie de mail lorsqu'il y a une erreur sur une page Web.
    /// Cette exception doit être lancé qui si l'utilisateur est authentifié.
    /// </summary>
    [Serializable]
    public class CustomerWebException : System.Exception{

        #region Variables
        /// <summary>
        /// Session du client
        /// </summary>
        protected WebSession webSession;
        /// <summary>
        /// Page Web qui lance l'erreur
        /// </summary>
        protected System.Web.UI.Page page;
        /// <summary>
        /// stackTrace
        /// </summary>
        protected string stackTrace;
        /// <summary>
        /// Nom du serveur où la page s'execute
        /// </summary>
        protected string serverName;
        /// <summary>
        /// Url demandée
        /// </summary>
        protected string url;
        /// <summary>
        /// Browser
        /// </summary>
        protected string browser;
        /// <summary>
        /// Version du Browser
        /// </summary>
        protected string versionBrowser;
        /// <summary>
        /// Sous version du browser
        /// </summary>
        protected string minorVersionBrowser;
        /// <summary>
        /// UserAgent
        /// </summary>
        protected string userAgent;
        /// <summary>
        /// Système d'exploitation
        /// </summary>
        protected string os;
        /// <summary>
        /// Adresse IP du client
        /// </summary>
        protected string userHostAddress;
        /// <summary>
        /// Platforme
        /// </summary>
        protected string platform;

        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="page">Page Web qui lance l'erreur</param>
        /// <param name="webSession">Session du client</param>
        public CustomerWebException(System.Web.UI.Page page, WebSession webSession):base(){
            this.webSession = webSession;
            this.browser = page.Request.Browser.Browser;
            this.versionBrowser = page.Request.Browser.Version;
            this.minorVersionBrowser = page.Request.Browser.MinorVersion.ToString();
            this.os = page.Request.Browser.Platform;
            this.userAgent = page.Request.UserAgent;
            this.userHostAddress = page.Request.UserHostAddress;
            this.url = page.Request.Url.ToString();
            this.page = page;
            this.serverName = page.Server.MachineName;
            this.platform = page.Request.Browser.Platform;
        }

        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="webSession">Session du client</param>
        public CustomerWebException(WebSession webSession):base(){
            this.webSession = webSession;
            this.browser = webSession.Browser;
            this.versionBrowser = webSession.BrowserVersion;
            this.minorVersionBrowser = "";
            this.os = webSession.CustomerOs;
            this.userAgent = webSession.UserAgent;
            this.userHostAddress = webSession.CustomerIp;
            this.url = webSession.LastWebPage;
            this.page = null;
            this.serverName = webSession.ServerName;
            this.platform = webSession.CustomerOs;
        }


        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="page">Page Web qui lance l'erreur</param>
        /// <param name="message">Message d'erreur</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="stackTrace">stackTrace</param>
        public CustomerWebException(System.Web.UI.Page page, string message, string stackTrace, WebSession webSession):base(message){
            this.webSession = webSession;
            this.browser = page.Request.Browser.Browser;
            this.versionBrowser = page.Request.Browser.Version;
            this.minorVersionBrowser = page.Request.Browser.MinorVersion.ToString();
            this.os = page.Request.Browser.Platform;
            this.userAgent = page.Request.UserAgent;
            this.userHostAddress = page.Request.UserHostAddress;
            this.url = page.Request.Url.ToString();
            this.platform = page.Request.Browser.Platform;
            this.page = page;
            this.serverName = page.Server.MachineName;
            this.stackTrace = stackTrace;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message d'erreur</param>
        /// <param name="webSession">Session du client</param>
        /// <param name="stackTrace">stackTrace</param>
        public CustomerWebException(string message, string stackTrace, WebSession webSession):base(message){
            this.webSession = webSession;
            this.browser = webSession.Browser;
            this.versionBrowser = webSession.BrowserVersion;
            this.minorVersionBrowser = "";
            this.os = webSession.CustomerOs;
            this.userAgent = webSession.UserAgent;
            this.userHostAddress = webSession.CustomerIp;
            this.url = webSession.LastWebPage;
            this.page = null;
            this.serverName = webSession.ServerName;
            this.platform = webSession.CustomerOs;
            this.stackTrace = stackTrace;
        }
        #endregion

        #region Méthodes internes
        /// <summary>
        /// Envoie un mail d'erreur
        /// </summary>
        public void SendMail()
        {
            string body = "";

			if(webSession==null){
				body+="<html><b><u>"+serverName+":</u></b><br>"+"<font color=#FF0000>Erreur client:<br></font>";
				body+="<hr>";
				body+="<u>Page requested:</u><br><a href="+url+">"+url+"</a><br>";
				body+="<u>Browser:</u><br>"+browser+" "+versionBrowser+" "+minorVersionBrowser+"<br>"+userAgent+"<br>";
                body += "<u>Operating system:</u><br>" + os + "<br>" + userHostAddress + "<br>";
                body += "<u>Error Message:</u><br>" + Message + "<br>";
				body+="<u>Source:</u><br>"+Source+"<br>";
				body+="<u>StackTrace:</u><br>"+((!string.IsNullOrEmpty(stackTrace)) ? stackTrace.Replace("at ","<br>at ") :"")+"<br>";
				body+="<hr>";
				body+="</html>";
			}
			else{
                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);

				#region Identifiaction du client
				body+="<html><b><u>"+serverName+":</u></b><br>"+"<font color=#FF0000>Client Error:<br></font>";
				body+="Session number: "+webSession.IdSession+"<br>";
				body+="Login: "+webSession.CustomerLogin.Login+"<br>";
				body+="Password: "+webSession.CustomerLogin.PassWord+"<br>";
				#endregion

				#region Message d'erreur
				body+="<hr>";
                body += "<u>Page requested:</u><br><a href=" + url + ">" + url + "</a><br>";
                body += "<u>Browser:</u><br>" + browser + " " + versionBrowser + " " + minorVersionBrowser + "<br>" + userAgent + "<br>";
                body += "<u>Operating system:</u><br>" + platform + "<br>" + userHostAddress + "<br>";
                body += "<u>Error Message:</u><br>" + Message + "<br>";
				body+="<u>Source:</u><br>"+Source+"<br>";
				body+="<u>StackTrace:</u><br>"+stackTrace.Replace("at ","<br>at ")+"<br>";
				body+="<hr>";
				#endregion

				// Module
				body+="<u>Module:</u> "+Convertion.ToHtmlString(GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSession.CurrentModule),webSession.SiteLanguage))+"<br>";
				// Unité
				body+="<u>Unit:</u> "+GestionWeb.GetWebWord(webSession.GetSelectedUnit().WebTextId,webSession.SiteLanguage)+"<br>";	
			
				#region période
				string periodText="";
				switch(webSession.PeriodType){
					case CstCustomerSession.Period.Type.nLastMonth:
						periodText=webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(783,webSession.SiteLanguage);					
						break;
					case CstCustomerSession.Period.Type.nLastYear:
						periodText=webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(781,webSession.SiteLanguage);					
						break;
					case CstCustomerSession.Period.Type.previousMonth:
						periodText=GestionWeb.GetWebWord(788,webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.previousYear:
						periodText=GestionWeb.GetWebWord(787,webSession.SiteLanguage);
						break;
						// Année courante		
					case  CstCustomerSession.Period.Type.currentYear:
						periodText=GestionWeb.GetWebWord(1228,webSession.SiteLanguage);
						break;
						// Année N-2
					case CstCustomerSession.Period.Type.nextToLastYear:
						periodText=GestionWeb.GetWebWord(1229,webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.dateToDateMonth:
						string monthBegin;
						string monthEnd;
						if(webSession.PeriodBeginningDate.ToString().Length<=0){
							periodText="?";
							break;
						}
						if(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2))<10){
							monthBegin=TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(5,1)),cultureInfo,10);
						}
						else{
							monthBegin=TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2)),cultureInfo,10);
						}
						if(int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2))<10){
							monthEnd=TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(5,1)),cultureInfo,10);
						}
						else{
							monthEnd=TNS.FrameWork.Date.MonthString.GetCharacters(int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2)),cultureInfo,10);
						}					
						periodText=GestionWeb.GetWebWord(846,webSession.SiteLanguage)+" "+monthBegin+" "+GestionWeb.GetWebWord(847,webSession.SiteLanguage)+" "+monthEnd;					
						break;
					case CstCustomerSession.Period.Type.dateToDateWeek:
						if(webSession.PeriodBeginningDate.ToString().Length<=0){
							periodText="?";
							break;
						}
						AtomicPeriodWeek tmp=new AtomicPeriodWeek(int.Parse(webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(webSession.PeriodBeginningDate.ToString().Substring(4,2)));
						periodText=tmp.FirstDay.Date.ToString("dd/MM/yyyy");
						tmp=new AtomicPeriodWeek(int.Parse(webSession.PeriodEndDate.Substring(0,4)),int.Parse(webSession.PeriodEndDate.ToString().Substring(4,2)));
						periodText+=" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+"";
						periodText+=" "+tmp.LastDay.Date.ToString("dd/MM/yyyy")+"";
						break;
					case CstCustomerSession.Period.Type.nLastWeek:
						periodText=webSession.PeriodLength.ToString()+" "+GestionWeb.GetWebWord(784,webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.previousWeek:
						periodText=GestionWeb.GetWebWord(789,webSession.SiteLanguage);
						break;
					case CstCustomerSession.Period.Type.dateToDate:
						string dateBegin;
						string dateEnd;
						if(webSession.PeriodBeginningDate.ToString().Length<=0){
							periodText="?";
							break;
						}
						dateBegin = DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodBeginningDate.ToString(),webSession.SiteLanguage);
						dateEnd = DateString.YYYYMMDDToDD_MM_YYYY(webSession.PeriodEndDate.ToString(),webSession.SiteLanguage);
						periodText=GestionWeb.GetWebWord(896,webSession.SiteLanguage)+" "+dateBegin+" "+GestionWeb.GetWebWord(897,webSession.SiteLanguage)+" "+dateEnd;
						break;
				}

				body+="<u>Period:</u> "+periodText+"<br>";
				// Etude comparative
				if(webSession.ComparativeStudy){
					body+="<u>Comparative study:</u> "+GestionWeb.GetWebWord(1118,webSession.SiteLanguage)+"<br>";

                }
                #endregion

                TNS.FrameWork.DB.Common.IDataSource dataSource = webSession.CustomerDataFilters.DataSource;

				#region univers Média
				// Media
				if (webSession.isMediaSelected()){
                    body += "<u>Media:</u> " + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSession.SelectionUniversMedia, false, false, false, 600, false, false, webSession.SiteLanguage, 2, 1, false, webSession.DataLanguage, dataSource);
				}
                             
				int i=2;
				int idMedia=1;
				if(webSession.isCompetitorMediaSelected()){
					System.Text.StringBuilder mediaSB=new System.Text.StringBuilder(1000);
					mediaSB.Append("<table>");
					mediaSB.Append("<TR><TD></TD>");
					mediaSB.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					mediaSB.Append("<label>"+GestionWeb.GetWebWord(1087,webSession.SiteLanguage)+"</label></TD>");
					mediaSB.Append("</TR>");

					while((TreeNode)webSession.CompetitorUniversMedia[idMedia]!=null){	
						TreeNode tree=(TreeNode)webSession.CompetitorUniversMedia[idMedia];				
						mediaSB.Append("<TR height=\"20\">");
						mediaSB.Append("<TD>&nbsp;</TD>");
						mediaSB.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">"+TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((TreeNode)webSession.CompetitorUniversMedia[idMedia],false,true,true,600,true,false,webSession.SiteLanguage,2,i,false,webSession.DataLanguage,dataSource)+"</TD>");
						mediaSB.Append("</TR>");
						mediaSB.Append("<TR height=\"5\">");
						mediaSB.Append("<TD></TD>");
						mediaSB.Append("<TD bgColor=\"#ffffff\"></TD>");
						mediaSB.Append("</TR>");
						mediaSB.Append("<TR height=\"7\">");
						mediaSB.Append("<TD colSpan=\"2\"></TD>");
						mediaSB.Append("</TR>");
						i++;
						idMedia++;
					}
					mediaSB.Append("</table><br>");
					body+=mediaSB.ToString();
				}

				// Partie détail média
				if(webSession.SelectionUniversMedia.FirstNode!=null && webSession.SelectionUniversMedia.FirstNode.Nodes.Count>0){
					System.Text.StringBuilder detailMedia=new System.Text.StringBuilder(1000);
					detailMedia.Append("<table>");
					detailMedia.Append("<TR><TD></TD>");
					detailMedia.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					detailMedia.Append("<label>"+GestionWeb.GetWebWord(1194,webSession.SiteLanguage)+"</label></TD>");
					detailMedia.Append("</TR>");				
					detailMedia.Append("<TR height=\"20\">");
					detailMedia.Append("<TD>&nbsp;</TD>");
                    detailMedia.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((TreeNode)webSession.SelectionUniversMedia.FirstNode, false, true, true, 600, true, false, webSession.SiteLanguage, 2, i, false, webSession.DataLanguage, dataSource) + "</TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"5\">");
					detailMedia.Append("<TD></TD>");
					detailMedia.Append("<TD bgColor=\"#ffffff\"></TD>");
					detailMedia.Append("</TR>");
					detailMedia.Append("<TR height=\"7\">");
					detailMedia.Append("<TD colSpan=\"2\"></TD>");
					detailMedia.Append("</TR>");
					i++;
					detailMedia.Append("</table><br>");
					body+=detailMedia.ToString();			
				}
             
				// Détail référence média
			
				if(webSession.isReferenceMediaSelected()){
					System.Text.StringBuilder referenceDetailMedia=new System.Text.StringBuilder(1000);
					referenceDetailMedia.Append("<table>");
					referenceDetailMedia.Append("<TR><TD></TD>");
					referenceDetailMedia.Append("<TD class=\"txtViolet11Bold\" bgColor=\"#ffffff\">&nbsp;");
					referenceDetailMedia.Append("<label>"+GestionWeb.GetWebWord(1194,webSession.SiteLanguage)+"</label></TD>");
					referenceDetailMedia.Append("</TR>");				
					
									
					referenceDetailMedia.Append("<TR height=\"20\">");
					referenceDetailMedia.Append("<TD>&nbsp;</TD>");
                    referenceDetailMedia.Append("<TD align=\"center\" vAlign=\"top\" bgColor=\"#ffffff\">" + TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml((TreeNode)webSession.ReferenceUniversMedia, false, true, true, 600, true, false, webSession.SiteLanguage, 2, i, false, webSession.DataLanguage, dataSource) + "</TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"5\">");
					referenceDetailMedia.Append("<TD></TD>");
					referenceDetailMedia.Append("<TD bgColor=\"#ffffff\"></TD>");
					referenceDetailMedia.Append("</TR>");
					referenceDetailMedia.Append("<TR height=\"7\">");
					referenceDetailMedia.Append("<TD colSpan=\"2\"></TD>");
					referenceDetailMedia.Append("</TR>");
					i++;
					referenceDetailMedia.Append("</table><br>");
					body+=referenceDetailMedia.ToString();			
			
				}
				#endregion

                #region Principal Media Universes
              
                if (webSession.IsPrincipalMediaUniversesSelected())
                {
                    int mediaCode = 2540;
                    //Selection media principale
                    if (webSession.PrincipalProductUniverses.Count == 1)
                    {
                        if (webSession.PrincipalMediaUniverses[0].ContainsLevel(TNSClassificationLevels.REGION, AccessType.includes))mediaCode = 2680;
                        body += "<u>" + GestionWeb.GetWebWord(mediaCode, webSession.SiteLanguage) + "</u><br>";
                        body += TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(webSession.PrincipalMediaUniverses[0], webSession.SiteLanguage, webSession.DataLanguage, dataSource, 600);                     
                    }
                    else if (webSession.PrincipalMediaUniverses.Count > 1)
                    {
                        for (int k = 0; k < webSession.PrincipalMediaUniverses.Count; k++)
                        {
                            body += "<u>" + GestionWeb.GetWebWord(mediaCode, webSession.SiteLanguage) + "</u><br>";
                            body += TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(webSession.PrincipalMediaUniverses[k], webSession.SiteLanguage, webSession.DataLanguage, dataSource, 600);
                        }
                    }
                }
                #endregion

                #region Univers produit

                AdExpressUniverse adExpressUniverse = null;
				int universeCodeTitle = 1759;
				if (webSession.isAdvertisersSelected()) {
					

					//Selection produit principale
					if (webSession.PrincipalProductUniverses.Count == 1) {
						body += "<u>" + GestionWeb.GetWebWord(1759, webSession.SiteLanguage) + "</u><br>";
                        body += TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(webSession.PrincipalProductUniverses[0], webSession.SiteLanguage, webSession.DataLanguage, dataSource, 600);
					}
					else if (webSession.PrincipalProductUniverses.Count > 1) {
						for (int k = 0; k < webSession.PrincipalProductUniverses.Count; k++) {
							if (webSession.PrincipalProductUniverses.ContainsKey(k)) {
								if (k > 0) {
									universeCodeTitle = 2301;
								}
								else {
									universeCodeTitle = 2302;
								}
							}
							body += "<u>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + "</u><br>";
                            body += TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(webSession.PrincipalProductUniverses[k], webSession.SiteLanguage, webSession.DataLanguage, dataSource, 600);
						}
					}
				}

    
                if (webSession.isReferenceAdvertisersSelected())
                {
                    adExpressUniverse = null;
                    universeCodeTitle = 1759;

					//Selection produit secondaire
					if (webSession.SecondaryProductUniverses.Count == 1) {						
						if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
								|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
								) {
							if (webSession.SecondaryProductUniverses.ContainsKey(0)) {
								adExpressUniverse = webSession.SecondaryProductUniverses[0];
								universeCodeTitle = 1195;
							}
							else if (webSession.SecondaryProductUniverses.ContainsKey(1)) {
								adExpressUniverse = webSession.SecondaryProductUniverses[1];
								universeCodeTitle = 1196;
							}
						}
						else adExpressUniverse = webSession.SecondaryProductUniverses[0];
						body += "<u>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + "</u><br>";
                        body += TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(adExpressUniverse, webSession.SiteLanguage, webSession.DataLanguage, dataSource, 600);
					}
					else if (webSession.SecondaryProductUniverses.Count > 1) {
						for (int k = 0; k < webSession.SecondaryProductUniverses.Count; k++) {
							if (k > 0) {
								if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
								|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
								)
									universeCodeTitle = 1196;
								else universeCodeTitle = 2301;
							}
							else {
								if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
								|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
								)
									universeCodeTitle = 1195;
								else universeCodeTitle = 2302;
							}
							body += "<u>" + GestionWeb.GetWebWord(universeCodeTitle, webSession.SiteLanguage) + "</u><br>";
                            body += TNS.AdExpress.Web.Functions.DisplayUniverse.ToHtml(webSession.SecondaryProductUniverses[k], webSession.SiteLanguage, webSession.DataLanguage, dataSource, 600);
						}
					}
				}
				if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR) {
					body += TNS.AdExpress.Web.BusinessFacade.Selections.Products.SectorsSelectedBusinessFacade.GetExcelSectorsSelected(webSession);
				}

				#endregion
			
				body+="</html>";
			}
			TNSMail.SmtpUtilities errorMail=new TNSMail.SmtpUtilities(WebApplicationParameters.ConfigurationDirectoryRoot+WebConstantes.ErrorManager.CUSTOMER_ERROR_MAIL_FILE);
          //Set Charset
            string charset = "";
            try
            {
               charset= WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Charset;
            }catch (Exception){}
            if (!string.IsNullOrEmpty(charset))
            {
                Encoding encoding = null;
                try
                {
                    encoding = Encoding.GetEncoding(charset);
                }
                catch (Exception) { }
                if (encoding != null)
                {
                    errorMail.SubjectEncoding = encoding;
                    errorMail.BodyEncoding = encoding;
                }

                errorMail.CharsetTextHtml = charset;
                errorMail.CharsetTextPlain = charset;
            }
            //Send mail
            errorMail.SendWithoutThread("Error AdExpress Client ("+serverName+")",Convertion.ToHtmlString(body),true,false);
		
		}
		#endregion

    }
}
