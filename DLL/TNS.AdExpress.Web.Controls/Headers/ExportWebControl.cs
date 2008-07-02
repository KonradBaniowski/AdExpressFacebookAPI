#region Informations
// Auteur:  
// Date de création: 
// Date de modification: 22/11/2004 (A. Obermeyer)
// Date de modification: 12/05/2005 (A. Dadouch)
//	18/08/2005	G. Facon	Ajout Icone PDF
//	30/05/2006	D. Mussuma	Ajout Icones texte,excel
//	G. Facon 01/08/2006 Gestion de l'accès au information de la page de résultat

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Composant gérant la possibilité de charger ou sauvegarder un univers.
	/// </summary>
	[ToolboxData("<{0}:ExportWebControl runat=server></{0}:ExportWebControl>")]
	public class ExportWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Date du Zoom
		/// </summary>
		protected string zoomDate="";
		/// <summary>
		/// Session du client (utile pour la langue)
		/// </summary>
		protected WebSession customerWebSession = null;

		/// <summary>
		/// Url à lancer pour faire l'impression excel de la page
		/// </summary>
		protected string printExcelUrl="";
		/// <summary>
		/// Url à lancer pour faire l'impression excel de la page
		/// </summary>
		protected string _forcePrintExcelUrl="";
		/// <summary>
		/// Url à lancer pour faire l'impression excel de la page pour l'affichage des unités (Plan média)
		/// </summary>
		protected string _forceValueExcelUrl="";
		/// <summary>
		/// Url à lancer pour faire l'export de la deuxieme page excel du Tableau de Bord
		/// </summary>
		protected string printBisExcelUrl="";
		/// <summary>
		/// Url à lancer pour faire l'export excel des données brutes de la page
		/// </summary>
		protected string exportExcelUrl="";
		/// <summary>
		/// Url à lancer pour faire l'export d'une image (utiliser dans les indicateurs pour les flash)
		/// </summary>
		protected string exportJpegUrl="";
		/// <summary>
		/// Url à lancer pour faire l'export remote Pdf (utiliser dans l'APPM)
		/// </summary>
		protected string _remotePdfUrl="";
		/// <summary>
		/// Url à lancer pour faire l'export remote Texte 
		/// </summary>
		protected string _remoteTextUrl="";
		/// <summary>
		/// Url à lancer pour faire l'export excel à distance 
		/// </summary>
		protected string _remoteExcelUrl="";
		/// <summary>
		/// Url à lancer pour faire l'impression excel de la page pour l'affichage des unités (Plan média)
		/// </summary>
		protected string _valueExcelUrl="";
		
		/// <summary>
		/// Possibilité d'imprimer l'export Excel
		/// </summary>
		protected bool printFormat;
		/// <summary>
		/// Possibilité d'imprimer l'export de la deuxieme page excel du Tableau de Bord
		/// </summary>
		protected bool printBisFormat;
		/// <summary>
		/// Possibilité d'exporter le résultat Excel des données brutes
		/// </summary>
		protected bool excelFormat;
		/// <summary>
		/// Possibilité de sauvegarder une image jpeg
		/// </summary>
		protected bool jpegFormat;
		/// <summary>
		/// Possibilité d'exporter le résultat Excel avec les unités (Plan média)
		/// </summary>
		protected bool _valueFormat;
		/// <summary>
		/// Possibilité de sauvegarder une image jpeg
		/// </summary>
		protected bool _jpegFormatFromWebPage=true;
		/// <summary>
		/// Possibilité de sauvegarder un pdf (en mode remone avec Anubis)
		/// </summary>
		protected bool _remotePdfFormat;
		/// <summary>
		/// Possibilité de sauvegarder un fichier texte (en mode remote avec Anubis)
		/// </summary>
		protected bool _remoteTextFormat;
		/// <summary>
		/// Possibilité de sauvegarder un fichier excel (en mode remote avec Anubis)
		/// </summary>
		protected bool _remoteExcelFormat;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Le composant doit il montrer l'option l'impression Excel
		/// </summary>
		//[Bindable(true),
		//Description("Option impression excel")]
		public bool PrintFormat{
			get{return printFormat;}
			//set{printFormat=value;}
		}

		/// <summary>
		/// Le composant doit il montrer l'option l'impression Excel
		/// </summary>
		//[Bindable(true),
		//Description("Option impression excel")]
		public bool ValueFormat{
			get{return _valueFormat;}
			//set{_valueFormat=value;}
		}

		/// <summary>
		/// Le composant doit il montrer l'option d'export Excel2 Tableau de Bord
		/// </summary>
		//[Bindable(true),
		//Description("option exporter (les années en colonnes) en fichier Ms Excel")]
		public bool PrintBisFormat{
			get{return printBisFormat;}
			//set{excelBisFormat=value;}
		}

		/// <summary>
		/// Le composant doit il montrer l'option d'export Excel
		/// </summary>
		//[Bindable(true),
		//Description("Option exporter en excel")]
		public bool ExcelFormat{
			get{return excelFormat;}
			//set{excelFormat=value;}
		}

		/// <summary>
		/// Le composant doit il montrer l'option d'export en jpeg
		/// </summary>
		//[Bindable(true),
		//Description("Option exporter en jpeg")]
		public bool JpegFormat{
			get{return jpegFormat;}
			//set{jpegFormat=value;}
		}

		/// <summary>
		/// Le composant doit il montrer l'option d'export en remote Pdf
		/// </summary>
		//[Bindable(true),
		//Description("Option exporter en jpeg")]
		public bool RemotePdfFormat{
			get{return _remotePdfFormat;}
			//set{jpegFormat=value;}
		}

		/// <summary>
		/// Le composant doit il montrer l'option d'export en remote Text
		/// </summary>
		//[Bindable(true),
		//Description("Option exporter en texte")]
		public bool RemoteTextFormat
		{
			get{return _remoteTextFormat;}
			//set{_remoteTextFormat=value;}
		}
		
		/// <summary>
		/// Le composant doit il montrer l'option d'export en remote excel
		/// </summary>
		//[Bindable(true),
		//Description("Option exporter en texte")]
		public bool RemoteExcelFormat {
			get{return _remoteExcelFormat;}
			//set{_remoteTextFormat=value;}
		}

		/// <summary>
		/// Le composant doit il montrer l'option d'export en jpeg
		/// </summary>
		[Bindable(true),
		Description("Option exporter en jpeg")]
		public bool JpegFormatFromWebPage{
			get{return _jpegFormatFromWebPage;}
			set{_jpegFormatFromWebPage=value;}
		}

		/// <summary>
		/// Possibilité de charger un univers
		/// </summary>
		[Bindable(true),
		Description("Option enregistrer mon résultat")]
		protected bool myAdExpress = true;
		/// <summary></summary>
		public bool MyAdExpress{
			get{return myAdExpress;}
			set{myAdExpress=value;}
		}
		
		/// <summary>
		/// Session du client
		/// </summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}

		/// <summary>
		/// Url d'impression 
		/// </summary>
		public string PrintExcelUrl{
			get{return printExcelUrl;}
			//set{printExcelUrl=value;}
		}

		/// <summary>
		/// Url d'impression Forcée
		/// </summary>
		public string ForcePrintExcelUrl{
			get{return _forcePrintExcelUrl;}
			set{_forcePrintExcelUrl=value;}
		}

		/// <summary>
		/// Url d'impression Forcée pour l'export des unités (Plan média)
		/// </summary>
		public string ForceValueExcelUrl{
			get{return _forceValueExcelUrl;}
			set{_forceValueExcelUrl=value;}
		}

		/// <summary>
		/// Url d'impression 2 
		/// </summary>
		public string PrintBisExcelUrl{
			get{return printBisExcelUrl;}
			//set{exportExcelBisUrl=value;}
		}

		/// <summary>
		/// Url d'export Excel des données brutes
		/// </summary>
		public string ExportExcelUrl{
			get{return exportExcelUrl;}
			//set{exportExcelUrl=value;}
		}

		/// <summary>
		/// Url pour l'affichage des images jpeg
		/// </summary>
		public string ExportJpegUrl{
			get{return exportJpegUrl;}
			//set{exportJpegUrl=value;}
		}

		/// <summary>
		/// Url pour lancer l'export vers Anubis
		/// </summary>
		public string RemotePdfUrl{
			get{return _remotePdfUrl;}
			//set{exportJpegUrl=value;}
		}

		/// <summary>
		/// Url pour lancer l'export Texte vers Anubis
		/// </summary>
		public string RemoteTextUrl
		{
			get{return _remoteTextUrl;}
			//set{_remoteTextUrl=value;}
		}
		
		/// <summary>
		/// Url pour lancer l'export Excel vers Anubis
		/// </summary>
		public string RemoteExcelUrl {
			get{return _remoteExcelUrl;}
			//set{_remoteTextUrl=value;}
		}

		/// <summary>
		/// Url d'Export Excel des unités 
		/// </summary>
		public string ValueExcelUrl
		{
			get{return _valueExcelUrl;}
			//set{_valueExcelUrl=value;}
		}

		/// <summary>
		/// Url d'export 
		/// </summary>
		public string ZoomDate{
			get{return zoomDate;}
			set{zoomDate=value;}
		}
		#endregion

		#region Evenements
		/// <summary>
		/// Prérendu du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if(!Page.ClientScript.IsClientScriptBlockRegistered("Scriptpopup")){
				string script="<script language=\"JavaScript\"> ";
				script+="function popupOpen(page,width,height,resizable){";
				script+="	window.open(page,'','width='+width+',height='+height+',toolbar=no,scrollbars='+resizable+',resizable='+resizable+'');";
				script+="}";
				script+="</script>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"Scriptpopup",script);
			}
		}

		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire</param>
		protected override void Render(HtmlTextWriter output){
			//customerWebSession.CustomerLogin.ModuleList();
			Module currentModule=customerWebSession.CustomerLogin.GetModule(customerWebSession.CurrentModule);
			ResultPageInformation currentPageResult=null;
			try{
				currentPageResult=(ResultPageInformation) currentModule.GetResultPageInformation(Convert.ToInt32(customerWebSession.CurrentTab));
			}
			catch(System.Exception){
				// Dans le cas de l'alerte portefeuille, l'id du "calendrier d'action" est 6 dans le fichier xml
				// mais il n'y a que 5 éléments obtenu dans le CurrentTab (ArrayList)
				// Du coup le calendrier se situe dans le 5ème élément du CurrentTab et ne correspond donc pas à son id
				// On force donc currentModule.ResultsPages[5] à 5 pour contourner ce problème.
				// Nous pourrions très bien changer l'id dans le fichier xml, mais cela posera problème pour les sessions des clients déjà sauvegarder.
				if(currentModule.Id==WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
					currentPageResult=(ResultPageInformation) currentModule.GetResultPageInformation(5);
			}
			
			// Excel d'impression
			printFormat=currentPageResult.CanDisplayPrintExcelPage();
			if(printFormat)this.printExcelUrl=currentPageResult.PrintExcelUrl;
			if(_forcePrintExcelUrl.Length>0){
				printFormat=true;
				this.printExcelUrl=_forcePrintExcelUrl;
			}

			// Excel d'impression Bis (inversion ligne/colonne)
			printBisFormat=currentPageResult.CanDisplayPrintBisExcelPage();
			if(printBisFormat)this.printBisExcelUrl=currentPageResult.PrintBisExcelUrl;

			// Excel d'impression des unités (Plan média)
			_valueFormat=currentPageResult.CanDisplayValueExcelPage();
			if(_valueFormat)this._valueExcelUrl=currentPageResult.ValueExcelUrl;
			if(_forceValueExcelUrl.Length>0){
				_valueFormat=true;
				this._valueExcelUrl=_forceValueExcelUrl;
			}
			
			// Excel des données brutes
			excelFormat=currentPageResult.CanDisplayRawExcelPage();
			if(excelFormat)this.exportExcelUrl=currentPageResult.RawExcelUrl;

			// Url export jpeg
			jpegFormat=(currentPageResult.CanDisplayExportJpegPage() & _jpegFormatFromWebPage);
			if(jpegFormat)this.exportJpegUrl=currentPageResult.ExportJpegUrl;

			// Url export remote Pdf
			_remotePdfFormat=currentPageResult.CanDisplayRemotePdfPage();
			if(_remotePdfFormat)_remotePdfUrl=currentPageResult.RemotePdfUrl;

			// Url export remote Texte
			_remoteTextFormat=currentPageResult.CanDisplayRemoteTextPage();
			if(_remoteTextFormat)_remoteTextUrl=currentPageResult.RemoteTextUrl;

			// Url export remote excel
			_remoteExcelFormat=currentPageResult.CanDisplayRemoteExcelPage();
			if(_remoteExcelFormat)_remoteExcelUrl=currentPageResult.RemoteExceltUrl;
			
			string tmp="";
			string type="2";
            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;
			if(zoomDate.Length>0) tmp="&zoomDate="+zoomDate;
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" bgcolor=\"#FFFFFF\">");
			output.Write("\n<tr>");
			output.Write("\n<td>");
			//debut tableau titre
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");
            output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\"/Images/Common/block_fleche.gif\" width=\"12\"></td>");
			output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" background=\"/Images/Common/block_dupli.gif\"><IMG height=\"1\" src=\"/Images/Common/pixel.gif\" width=\"13\"></td>");
			output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"+GestionWeb.GetWebWord(771,customerWebSession.SiteLanguage)+"</td>");
			output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/Images/pixel.gif\" width=\"1\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td></td>");
			output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n</table>");
			//fin tableau titre
			output.Write("\n</td>");
			output.Write("\n</tr>");	
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");

			//Option myAdExpress
			if (myAdExpress || printFormat || excelFormat || printBisFormat || jpegFormat){
//				output.Write("\n<tr>");
//				output.Write("\n<td><a class=\"roll03\" href=\"javascript:popupOpen('/Private/MyAdExpress/MySessionSavePopUp.aspx?idSession="+ customerWebSession.IdSession +"','470','210');\">&gt; "+ GestionWeb.GetWebWord(790,customerWebSession.SiteLanguage) +"</a>");		
//				output.Write("\n</td>");
//				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				output.Write("\n<table cellspacing=1 cellpading=0 border=0 bgcolor=\"#FFFFFF\">");
				output.Write("\n<tr>");
				output.Write("\n<td><img src=\"/Images/Common/pixel.gif\" width=\"7px\" height=\"1px\"></td>");
				//Option myAdExpress
				if (myAdExpress)
					output.Write("\n<td><a class=\"roll03\" href=\"javascript:popupOpen('/Private/MyAdExpress/MySessionSavePopUp.aspx?idSession="+ customerWebSession.IdSession +"&param="+generateNumber()+"','470','210','no');\"  onmouseover=\"saveButton.src='/Images/Common/button/save_down.gif';\" onmouseout=\"saveButton.src ='/Images/Common/button/save_up.gif';\"><img name=saveButton border=0 src=\"/Images/Common/button/save_up.gif\" alt=\""+ GestionWeb.GetWebWord(790,customerWebSession.SiteLanguage) +"\"></a>");	
				
				//Option impression
				if(printFormat)
					output.Write("\n<td><a href=\""+this.printExcelUrl+"?idSession="+this.customerWebSession.IdSession+tmp+"\" target=\"_blank\" class=\"roll03\" onmouseover=\"printButton.src='/Images/Common/button/print_down.gif';\" onmouseout=\"printButton.src ='/Images/Common/button/print_up.gif';\"><img name=printButton border=0 src=\"/Images/Common/button/print_up.gif\" alt=\""+ GestionWeb.GetWebWord(791,customerWebSession.SiteLanguage) +"\"></a>");
				
				//Option impression 2
				if(printBisFormat){
					tmp = "&type="+type;
					output.Write("\n<td><a href=\""+this.printBisExcelUrl+"?idSession="+this.customerWebSession.IdSession+tmp+"\" target=\"_blank\" class=\"roll03\" onmouseover=\"excelButton2.src='/Images/Common/button/printBis_down.gif';\" onmouseout=\"excelButton2.src ='/Images/Common/button/printBis_up.gif';\"><img name=excelButton2 border=0 src=\"/Images/Common/button/printBis_up.gif\" alt=\""+ GestionWeb.GetWebWord(1598,customerWebSession.SiteLanguage) +"\"></a>");
				}

				//Option impression
				if(_valueFormat)
					output.Write("\n<td><a href=\""+this._valueExcelUrl+"?idSession="+this.customerWebSession.IdSession+tmp+"\" target=\"_blank\" class=\"roll03\" onmouseover=\"valueExcelButton.src='/Images/Common/button/excel_unit_down.gif';\" onmouseout=\"valueExcelButton.src ='/Images/Common/button/excel_unit_up.gif';\"><img name=valueExcelButton border=0 src=\"/Images/Common/button/excel_unit_up.gif\" alt=\""+ GestionWeb.GetWebWord(1858,customerWebSession.SiteLanguage) +"\"></a>");

				//Option excel
				if(excelFormat)
					output.Write("\n<td><a href=\""+this.exportExcelUrl+"?idSession="+this.customerWebSession.IdSession+tmp+"\" target=\"_blank\" class=\"roll03\" onmouseover=\"excelButton.src='/Images/Common/button/excel_down.gif';\" onmouseout=\"excelButton.src ='/Images/Common/button/excel_up.gif';\"><img name=excelButton border=0 src=\"/Images/Common/button/excel_up.gif\" alt=\""+ GestionWeb.GetWebWord(791,customerWebSession.SiteLanguage) +"\"></a>");

				//Option jpeg
				if(jpegFormat)
					output.Write("\n<td><a href=\"javascript:popupOpen('"+this.exportJpegUrl+"?idSession="+ customerWebSession.IdSession +"','950','800','yes');\" onmouseover=\"jpgButton.src='/Images/Common/button/export_jpeg_down.gif';\" onmouseout=\"jpgButton.src ='/Images/Common/button/export_jpeg_up.gif';\"><img name=jpgButton border=0 src=\"/Images/Common/button/export_jpeg_up.gif\" alt=\""+GestionWeb.GetWebWord(1317,customerWebSession.SiteLanguage)+"\"></a>");
				
				//Option Remote Pdf (par Anubis)
				if(RemotePdfFormat){
					if(currentModule.Id==WebConstantes.Module.Name.INDICATEUR)
						output.Write("\n<td><a href=\"javascript:popupOpen('"+_remotePdfUrl+"?idSession="+ customerWebSession.IdSession +"','470','210','yes');\" onmouseover=\"remotePdfButton.src='/Images/Common/button/LogoPDF_down.gif';\" onmouseout=\"remotePdfButton.src ='/Images/Common/button/LogoPDF_up.gif';\"><img name=remotePdfButton border=0 src=\"/Images/Common/button/LogoPDF_up.gif\" alt=\""+GestionWeb.GetWebWord(1797,customerWebSession.SiteLanguage)+"\"></a>");
					else
						output.Write("\n<td><a href=\"javascript:popupOpen('"+_remotePdfUrl+"?idSession="+ customerWebSession.IdSession +"','600','400','yes');\" onmouseover=\"remotePdfButton.src='/Images/Common/button/LogoPDF_down.gif';\" onmouseout=\"remotePdfButton.src ='/Images/Common/button/LogoPDF_up.gif';\"><img name=remotePdfButton border=0 src=\"/Images/Common/button/LogoPDF_up.gif\" alt=\""+GestionWeb.GetWebWord(1797,customerWebSession.SiteLanguage)+"\"></a>");
				}

				//Option Remote Texte (par Anubis)
				if(RemoteTextFormat)
					output.Write("\n<td><a href=\"javascript:popupOpen('"+_remoteTextUrl+"?idSession="+ customerWebSession.IdSession +"','470','210','yes');\" onmouseover=\"remoteTextButton.src='/Images/Common/button/txt_export_down.gif';\" onmouseout=\"remoteTextButton.src ='/Images/Common/button/txt_export_up.gif';\"><img name=remoteTextButton border=0 src=\"/Images/Common/button/txt_export_up.gif\" alt=\""+GestionWeb.GetWebWord(1913,customerWebSession.SiteLanguage)+"\"></a>");
				
				//Option Remote Excel (par Anubis)
				if(RemoteExcelFormat)
					output.Write("\n<td><a href=\"javascript:popupOpen('"+_remoteExcelUrl+"?idSession="+ customerWebSession.IdSession +"','470','210','yes');\" onmouseover=\"remoteExcelButton.src='/Images/Common/button/excel_export_down.gif';\" onmouseout=\"remoteExcelButton.src ='/Images/Common/button/excel_export_up.gif';\"><img name=remoteExcelButton border=0 src=\"/Images/Common/button/excel_export_up.gif\" alt=\""+GestionWeb.GetWebWord(1923,customerWebSession.SiteLanguage)+"\"></a>");

				output.Write("\n</tr>");
				output.Write("\n</table>");
				output.Write("\n</td>");
				output.Write("\n</tr>");
			}
			//Option excel
//			if(excelFormat){
//				if(zoomDate.Length>0) tmp="&zoomDate="+zoomDate;
//				output.Write("\n<tr>");
//				output.Write("\n<td><a href=\""+this.exportExcelUrl+"?idSession="+this.customerWebSession.IdSession+tmp+"\" target=\"_blank\" class=\"roll03\">&gt; "+ GestionWeb.GetWebWord(791,customerWebSession.SiteLanguage) +"</a>");
//				output.Write("\n</td>");
//				output.Write("\n</tr>");
//			}
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n</table>");
		}
		
		#endregion

		#region Méthodes interne
		private static string generateNumber(){
			DateTime dt=DateTime.Now;
			return(dt.Year.ToString()+dt.Month.ToString()+dt.Day.ToString()+dt.Hour.ToString()+dt.Minute.ToString()+dt.Second.ToString()+dt.Millisecond.ToString()+new Random().Next(1000));
		}
		#endregion
	}
}
