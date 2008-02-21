#region Informations
// Auteur: D. Mussuma
// Date de création: 14/02/2007
// Date de modification :
#endregion

using System;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using RulesResults=TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.FrameWork;
using FrameWorkResults=TNS.AdExpress.Constantes.FrameWork.Results;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Web.Controls.Exceptions;
using Oracle.DataAccess.Client;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Contrôle détail des créations d'un support utilisé par les alertes mail
	/// </summary>
	[ToolboxData("<{0}:AlertsCreationsResultsWebControl runat=server></{0}:AlertsCreationsResultsWebControl>")]
	public class AlertsCreationsResultsWebControl : System.Web.UI.WebControls.WebControl{
		#region Variables
		
		
		/// <summary>
		/// Identifiant du media
		/// </summary>
		private string _idMedia = null;
		/// <summary>
		/// Identifiant du produit
		/// </summary>
		private string _idProduct = null;
		/// <summary>
		/// Date de début
		/// </summary>
		private string _dateBegin = null;
		/// <summary>
		/// Date de fin
		/// </summary>
		private string _dateEnd = null;
		/// <summary>
		/// Clé d'authentification
		/// </summary>
		private string _authentificationKey = null;
					
		/// <summary>
		/// Message d'erreur
		/// </summary>
		private string _errorMessage = null;
		
		/// <summary>
		/// Tableau de résultats
		/// </summary>
		private object[,]  _tableResult = null;		

		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
	
		/// <summary>
		/// Identifiant du média
		/// </summary>
		protected Int64 _idVehicle = 0;

		/// <summary>
		/// Indique si l'utilisateur à accès à la page
		/// </summary>
		protected bool _isAuthorizeToViewMediaDetail = false;
		#endregion

		#region Accesseurs
		

		/// <summary>
		/// Obtient l'identifiant du support 
		/// </summary>
		public string IdMedia{
			get{return _idMedia;}
			
		}

		/// <summary>
		/// Obtient l'identifiant du produit 
		/// </summary>
		public string IdProduct{
			get{return _idProduct;}
			
		}

		/// <summary>
		/// Obtient l'identifiant de la page  
		/// </summary>
		public string AuthentificationKey{
			get{return _authentificationKey;}
			
		}

		/// <summary>
		/// Obtient la date de début
		/// </summary>
		public string DateBegin{
			get{return _dateBegin;}
			
		}

		/// <summary>
		/// Obtient la date de fin
		/// </summary>
		public string DateEnd{
			get{return _dateEnd;}
			
		}
		#endregion

		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			try{
				//Récupération des paramètres de l'url
				if(Page.Request.QueryString.Get("mediaId")!=null) _idMedia = Page.Request.QueryString.Get("mediaId");
				if(Page.Request.QueryString.Get("productId")!=null) _idProduct = Page.Request.QueryString.Get("productId");
				if(Page.Request.QueryString.Get("dateBegin")!=null) _dateBegin = Page.Request.QueryString.Get("dateBegin");
				if(Page.Request.QueryString.Get("dateEnd")!=null) _dateEnd = Page.Request.QueryString.Get("dateEnd");
				if(Page.Request.QueryString.Get("key")!=null) _authentificationKey = Page.Request.QueryString.Get("key");
				if(Page.Request.QueryString.Get("siteLanguage") == null) _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
				else _siteLanguage = int.Parse(Page.Request.QueryString.Get("siteLanguage").ToString());
				
				//Calcul de la clé d'authentification
				double computedKey = 0;
				if(_idProduct!=null && _idMedia != null && _idMedia!=null && _dateBegin!=null && _dateEnd !=null){
					 computedKey = (Int64)Math.Abs((Int64)(Int64.Parse(_idProduct.ToString())*2)
						+9*Int64.Parse(_idMedia)+((int)(int.Parse(_dateBegin)*5)/2)-((int)(int.Parse(_dateEnd)*3)/7));
				}
				
				if(computedKey != double.Parse(_authentificationKey))_errorMessage = GestionWeb.GetWebWord(2123,_siteLanguage); //_isAuthorizeToViewMediaDetail = true;

			}
			catch(System.Exception ex){
				_errorMessage = GestionWeb.GetWebWord(958, _siteLanguage);
				OnMethodError(ex);
			}

			base.OnInit (e);
		}
		#endregion

		#region PreRender
		/// <summary>
		/// Pré rendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if(_errorMessage==null){

				try{
				
					#region GetData
			
					//Connection
					string login ="gfacon", password="sandie5";
					string oracleConnectionString="User Id="+login+"; Password="+password+""+TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING;
					TNS.FrameWork.DB.Common.IDataSource dataSource = new OracleDataSource(new OracleConnection(oracleConnectionString));

					//Recupère l'identifiant du média (vehicle)
					DataSet dsVehicle = TNS.AdExpress.Web.DataAccess.Results.AlertsInsertionsCreationsDataAccess.GetIdVehicle(dataSource,_idMedia,_siteLanguage.ToString());
			
					if(dsVehicle!=null  && dsVehicle.Tables[0].Rows.Count>0){
						_idVehicle = Int64.Parse(dsVehicle.Tables[0].Rows[0]["id_vehicle"].ToString());
						_tableResult = RulesResults.AlertsInsertionsCreationsRules.GetData(_idVehicle,dataSource,_idMedia, _idProduct, _dateBegin, _dateEnd,_siteLanguage.ToString());
					}
					#endregion
			
					#region Scripts
					string pagePath = null;
					if (!this.Page.ClientScript.IsClientScriptBlockRegistered("openAlertsDownload")){
						pagePath = "/Private/ALerts/AlertsDownloadCreationsPopUp.aspx";
						this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openAlertsDownload",WebFunctions.Script.OpenAlertsDownload(pagePath));
					}
				
					#endregion
				}
				catch(System.Exception ex){
					_errorMessage = GestionWeb.GetWebWord(1489, _siteLanguage);
					OnMethodError(ex);
				}
			}
		}

		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			
			if(_errorMessage==null){
				#region Variables			
				double creationdownloadKey = 0;
				#endregion
			
				if (_tableResult!=null && _tableResult.GetLength(0) > 0){

					//Calcul de la clé d'authentification pour le téléchargement de la création
					 creationdownloadKey = (Int64)Math.Abs((Int64)(Int64.Parse(_idProduct.ToString())*4)
						+9*Int64.Parse(_idMedia)+((int)(int.Parse(_dateBegin)*5)/2)-((int)(int.Parse(_dateEnd)*3)/9));

					output.Write(GetMediaInsertionsCreationsResultsUI(_tableResult,_idVehicle,_siteLanguage,_idMedia,_idProduct,_dateBegin,_dateEnd,creationdownloadKey.ToString()));
				}
				else{
					output.Write("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
					output.Write("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");			
					output.Write("<TD  align=\"center\" bgColor=\"#ffffff\" class=\"txtViolet11Bold\" >");
					output.Write( GestionWeb.GetWebWord(177,_siteLanguage));
					output.Write("</TD>");
					output.Write("</TR></TABLE>");
				}
			}
			else{
				output.Write("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
				output.Write("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");			
				output.Write("<TD  align=\"center\" bgColor=\"#ffffff\" class=\"txtViolet11Bold\" >");
				output.Write( _errorMessage);
				output.Write("</TD>");
				output.Write("</TR></TABLE>");
			}
		}
		#endregion

		#region HTML UI	
		

		#region Radio
		/// <summary>
		/// Retourne le code html affichant le détails des insertions radio:
		///		data vide : code HTML d'un message d'erruer
		///		data non vide : code HTML du tableau présentant le détail radio insertion par insertion
		///			Génération du code d'export Excel
		///			Enregistrement du script d'ouverture de "AccessDownloadCreationsPopUp.aspx"
		///			Rappel des paramètres
		///			Génération du tableau des insertions ordonnées par Catégorie > Support > Date
		/// </summary>
		/// <param name="data">Tableau contenant les données à afficher</param>
		/// <param name="siteLanguage">Langue du site</param>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="dateBegin">Date debut  des calculs</param>
		/// <param name="dateEnd">Date de fin des calculs</param>
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <param name="key">Clé d'authentification</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string GetUIEmpty(int language)
		/// </remarks>
		private static string GetUIRadio(object[,] data, int siteLanguage,string idMedia,string idProduct,string dateBegin,string dateEnd, Int64 idVehicle,string key){
			
			StringBuilder HtmlTxt = new StringBuilder(1000);
			string ColSpan="13";
			const string CLASSE_1="p6";
			const string CLASSE_2="p7";
			string oldDate = "";
			bool first = true;
			string classe="";
			int i=0;
			string idSlogan = "";
			string file = "";
			
			#region Détail spots radio sans gestion des colonnes génériques

			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;BORDER:SOLID 5px #ffffff;\"");
			HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">");
			#endregion

			#region Pas de données à afficher
			if (data[0,0]==null){
				return HtmlTxt.Append(GetUIEmpty(siteLanguage)).ToString();
			}
			#endregion

			// si droit accroche
			ColSpan="16";								
				
			#region Paramètres (vehicle, dates...)
			HtmlTxt.Append("<TR>");
			HtmlTxt.Append("<TD colSpan=\""+ColSpan+"\" valign=\"center\" class=\"txtViolet14Bold\" >");
			HtmlTxt.Append(data[0, CstWeb.RadioInsertionsColumnIndex.VEHICLE_INDEX].ToString());
			if(dateBegin.Equals(dateEnd))HtmlTxt.Append( " " +  Dates.dateToString(Dates.getPeriodBeginningDate(dateBegin,Constantes.Web.CustomerSessions.Period.Type.dateToDate),siteLanguage));
			else{
				HtmlTxt.Append( " " + GestionWeb.GetWebWord(896, siteLanguage) + Dates.dateToString(Dates.getPeriodBeginningDate(dateBegin,Constantes.Web.CustomerSessions.Period.Type.dateToDate),siteLanguage)
					+ GestionWeb.GetWebWord(897, siteLanguage) + Dates.dateToString(Dates.getPeriodEndDate(dateEnd, Constantes.Web.CustomerSessions.Period.Type.dateToDate),siteLanguage)+"<br>");
			}
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");			
			#endregion

			#region Tableaux d'insertions
			i = 0;
			string currentCategory="";
			string currentMedia="";

			#region Niveau Catégorie
			while(i<data.GetLength(0) && data[i,0]!=null){
				currentCategory = data[i, CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX].ToString();
				#region Infos Catégorie				
				HtmlTxt.Append("<tr><td colSpan=\""+ColSpan+"\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");
				HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
				HtmlTxt.Append("<TD class=\"insertionCategory\" style=\"BACKGROUND-POSITION-X: right; BACKGROUND-IMAGE: url(/Images/Common/bandeau_titre.gif);BACKGROUND-REPEAT: repeat-y\">");
				HtmlTxt.Append(currentCategory);
				HtmlTxt.Append("</TD></tr>");
				HtmlTxt.Append("</table></td></TR>");							
				#endregion

				#region Niveau Support
				while(i<data.GetLength(0) && data[i,0]!=null && currentCategory.CompareTo(data[i,CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX].ToString())==0){
					currentMedia = data[i, CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX].ToString();
					#region Infos Support
					HtmlTxt.Append("<TR>");
					//					  HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\"13\">");
					HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\""+ColSpan+"\">");
					HtmlTxt.Append(currentMedia);
					HtmlTxt.Append("</TD>");
					HtmlTxt.Append("</TR>");							
					#endregion

					#region Entêtes de tableaux

					oldDate = "";
					first = true;
					classe="";

					HtmlTxt.Append("<tr>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(869,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1881,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1384,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1383,siteLanguage)+"</td>");						
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(860,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(861,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(862,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(863,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(864,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(865,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(866,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(867,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,siteLanguage)+"</td>");					
					HtmlTxt.Append("</tr>");
					#endregion

					#region Tableau de détail d'un support
					while(i<data.GetLength(0) && data[i,0]!=null && currentMedia.CompareTo(data[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX].ToString())==0){
						if (oldDate.CompareTo(data[i,CstWeb.RadioInsertionsColumnIndex.DATE_INDEX].ToString())!=0){
							//nouvelle date
							first = true;
							oldDate=(string)data[i,CstWeb.RadioInsertionsColumnIndex.DATE_INDEX];
							HtmlTxt.Append("<TR>"); 							
							HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\""+ColSpan+"\">&nbsp;&nbsp;");
							HtmlTxt.Append(oldDate);
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>"); 
						}
				
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}

						file = (data[i, CstWeb.RadioInsertionsColumnIndex.FILE_INDEX] != null && data[i, CstWeb.RadioInsertionsColumnIndex.FILE_INDEX].ToString().CompareTo("") != 0) ? data[i, CstWeb.RadioInsertionsColumnIndex.FILE_INDEX].ToString() : "";
						idSlogan = (data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX] != null && data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString().CompareTo("0") != 0) ? data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString() : "";
						
						if (file.Length > 0 || idSlogan.Length > 0 ){
							HtmlTxt.Append("<tr><td class=\"" + classe + "\" nowrap><a href=\"javascript:openAlertsDownload('" + file + "','" + idMedia + "," + idProduct + "," + dateBegin + "," + dateEnd + "," + key + "," + siteLanguage + "','" + idVehicle + "','" + idSlogan + "');\"><img border=\"0\" src=\"/Images/Common/Picto_Radio.gif\"></a></td>");
						}
						else{
							HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap></td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
						if(data[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString()+"</td>");
						else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;</td>");
							
						
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap >"+data[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString()+"</td>");
							
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"center\">"+data[i,CstWeb.RadioInsertionsColumnIndex.TOP_DIFFUSION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.RANK_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.RANK_WAP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.DURATION_BREAK_WAP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_WAP_NB_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("</tr>");

						i++;
					}
					#endregion

				}

				#endregion
			
			}
			#endregion

			#endregion

			HtmlTxt.Append("</TABLE>");
			#endregion					
			
			return Convertion.ToHtmlString(HtmlTxt.ToString());
		}
		#endregion		

		#region TV
		/// <summary>
		/// Retourne le code html affichant le détails des insertions TV:
		///		data vide : code HTML d'un message d'erruer
		///		data non vide : code HTML du tableau présentant le détail radio insertion par insertion
		///			Génération du code d'export Excel
		///			Enregistrement du script d'ouverture de "AccessDownloadCreationsPopUp.aspx"
		///			Rappel des paramètres
		///			Génération du tableau des insertions ordonnées par Catégorie > Support > Date
		/// </summary>
		/// <param name="data">Tableau contenant les données à afficher</param>
		/// <param name="siteLanguage">Langue du site</param>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="dateBegin">Date debut  des calculs</param>
		/// <param name="dateEnd">Date de fin des calculs</param>
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <param name="key">Clé d'authentification</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string GetUIEmpty(int language)
		/// </remarks>
		private static string GetUITV(object[,] data, int siteLanguage,string idMedia,string idProduct,string dateBegin,string dateEnd, Int64 idVehicle,string key){
		
			StringBuilder HtmlTxt = new StringBuilder(1000);
			StringBuilder script = new StringBuilder(500);
			
			#region variables
			string ColSpan="13";
			string paramColSpan="11";
			int i = 0;
			string oldDate = "";
			bool first = true;
			string classe="";
//			int colToShow=0;
		
			#endregion

			#region constantes
			const string CLASSE_1="p6";
			const string CLASSE_2="p7";
			#endregion
		
			#region Détail spots télévision sans gestion des colonnes génériques
				
				
			// si droit accroche
			ColSpan="16";
			paramColSpan="14";				
				
			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;BORDER:SOLID 5px #ffffff;\"");
			HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">");
			#endregion

			#region Pas de données à afficher
			if (data[0,0]==null){
				return HtmlTxt.Append(GetUIEmpty(siteLanguage)).ToString();
			}
			#endregion
					
			
			#region Paramètres (vehicle, dates...)
			HtmlTxt.Append("<TR>");
			HtmlTxt.Append("<TD colSpan=\""+ColSpan+"\" valign=\"center\" class=\"txtViolet14Bold\" >");
			HtmlTxt.Append(data[0, CstWeb.TVInsertionsColumnIndex.VEHICLE_INDEX].ToString());
			if(dateBegin.Equals(dateEnd))HtmlTxt.Append( " " +  Dates.dateToString(Dates.getPeriodBeginningDate(dateBegin,Constantes.Web.CustomerSessions.Period.Type.dateToDate),siteLanguage));
			else{
				HtmlTxt.Append( " " + GestionWeb.GetWebWord(896, siteLanguage) + Dates.dateToString(Dates.getPeriodBeginningDate(dateBegin,Constantes.Web.CustomerSessions.Period.Type.dateToDate),siteLanguage)
					+ GestionWeb.GetWebWord(897, siteLanguage) + Dates.dateToString(Dates.getPeriodEndDate(dateEnd, Constantes.Web.CustomerSessions.Period.Type.dateToDate),siteLanguage)+"<br>");
			}
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");	

							
			#endregion

			#region Tableaux d'insertions
				
			string currentCategory="";
			string currentMedia="";

			#region Niveau Catégorie
			while(i<data.GetLength(0) && data[i,0]!=null){
				currentCategory = data[i, CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX].ToString();

				#region Infos Catégorie
				//HtmlTxt.Append("<TR><td>&nbsp;</td></tr>");
				HtmlTxt.Append("<tr><td colSpan=\""+paramColSpan+"\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");//Si affiche accroche
				HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
				HtmlTxt.Append("<TD class=\"insertionCategory\" style=\"BACKGROUND-POSITION-X: right; BACKGROUND-IMAGE: url(/Images/Common/bandeau_titre.gif);BACKGROUND-REPEAT: repeat-y\">");
				HtmlTxt.Append(currentCategory);
				HtmlTxt.Append("</TD></tr>");
				HtmlTxt.Append("</table></td></TR>");							
				#endregion

				#region Niveau Support
				while(i<data.GetLength(0) && data[i,0]!=null && currentCategory.CompareTo(data[i,CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX].ToString())==0){
					currentMedia = data[i, CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX].ToString();
					#region Infos Support
					HtmlTxt.Append("<TR>");
					HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\""+paramColSpan+"\">"); //Si affiche accroche
					HtmlTxt.Append(currentMedia);
					HtmlTxt.Append("</TD>");
					HtmlTxt.Append("</TR>");							
					#endregion

					#region Entêtes de tableaux

					oldDate = "";
					first = true;
					classe="";

					HtmlTxt.Append("<tr>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(869,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,siteLanguage)+"</td>");
					//Accroche
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1881,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1384,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1383,siteLanguage)+"</td>");
						
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(860,siteLanguage)+"</td>");
					//Code écran
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(495,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(861,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(862,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(863,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(864,siteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,siteLanguage)+"</td>");					
					HtmlTxt.Append("</tr>");
					#endregion

					#region Tableau de détail d'un support
					while(i<data.GetLength(0) && data[i,0]!=null && currentMedia.CompareTo(data[i,CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX].ToString())==0){
						if (oldDate.CompareTo(data[i,CstWeb.TVInsertionsColumnIndex.DATE_INDEX].ToString())!=0){
							//nouvelle date
							first = true;
							oldDate=(string)data[i,CstWeb.TVInsertionsColumnIndex.DATE_INDEX];
							HtmlTxt.Append("<TR>");
							HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\""+paramColSpan+"\">&nbsp;&nbsp;");
							HtmlTxt.Append(oldDate);
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>"); 
						}
			
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						if(data[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX].ToString().CompareTo("")!=0								
							){
							HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap><a href=\"javascript:openAlertsDownload('"+data[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX].ToString()+"','"+idMedia+","+idProduct+","+dateBegin+","+dateEnd+","+key+","+siteLanguage+"','"+idVehicle+"');\"><img border=\"0\" src=\"/Images/Common/Picto_pellicule.gif\"></a></td>");
						}
						else{
							HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap></td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
						if(data[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString()+"</td>");
						else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;</td>");							
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap >"+data[i,CstWeb.TVInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString()+"</td>");							
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.RANK_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.BREAK_DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td>");
						HtmlTxt.Append("</tr>");


						i++;
					}
					#endregion

				}

				#endregion
		
			}
			#endregion

			#endregion

			HtmlTxt.Append("</TABLE>");

			#endregion
			
			return Convertion.ToHtmlString(HtmlTxt.ToString());

		}
		#endregion
		
		#region HTML UI 
		/// <summary>
		/// Construction du code HTML affichant le détail des insertions d'un média (vehicle) quelqconque sur une certaine période respectant le type de période présent dans la session
		///		Extraction des données
		///		Construction du code HTML en fonction du média considéré
		/// </summary>
		/// <param name="tab">Tableau contenant les données à afficher</param>
		/// <param name="siteLanguage">Langue du site</param>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="dateBegin">Date debut  des calculs</param>
		/// <param name="dateEnd">Date de fin des calculs</param>
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <param name="key">Clé d'authentification</param>
		/// <remarks>
		/// Utilise les méthodes:
		///		<code>public static object[,] MediaInsertionsCreationsRules.GetData(WebSession webSession,string idMediaLevel1,string idMediaLevel2,string idMediaLevel3,string idAdvertiser ,int dateBegin, int dateEnd,bool isMediaDetail,string idVehicleFromTab,ListDictionary mediaImpactedList)</code>
		///		<code>private static string GetUIPress(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl,bool isMediaDetail)</code>
		///		<code>private static string GetUIRadio(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		///		<code>private static string GetUITV(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		///		<code>GetUIOutDoor(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		/// </remarks>
		/// <returns>Code HTML détail des insertions</returns>
		public static string GetMediaInsertionsCreationsResultsUI(object[,] tab,Int64 idVehicle, int siteLanguage,string idMedia,string idProduct,string dateBegin,string dateEnd, string key){		

			#region variables
					
//			string PeriodEndDate="";
//			string PeriodBeginningDate="";
//			
			#endregion
				

			#region Pas de données à afficher
			if (tab==null || tab[0,0]==null) {
				
					return "<TABLE width=\"500\" bgColor=\"#ffffff\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;BORDER:SOLID 5px #ffffff;\""
						+"cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">"
						+GetUIEmpty(siteLanguage);
			}
			#endregion

			try{				

				#region Construction du txt HTLM
				switch((CstClassification.DB.Vehicles.names)idVehicle){					
					case CstClassification.DB.Vehicles.names.radio:
						return GetUIRadio(tab,siteLanguage,idMedia,idProduct,dateBegin,dateEnd,idVehicle,key);
					case CstClassification.DB.Vehicles.names.tv:
					case CstClassification.DB.Vehicles.names.others:
						return GetUITV(tab,siteLanguage,idMedia,idProduct,dateBegin,dateEnd,idVehicle,key);
					
					default:
						throw new AlertsCreationsResultsWebControlException("Le vehicle demandé n'est pas un cas traité");
				}
				#endregion
					
			}
			catch(System.Exception err){
				throw(new AlertsCreationsResultsWebControlException("Impossible d'obtenir les données",err));
			}
					
		}
		#endregion

		#region Pas de Creations
		/// <summary>
		/// Génère le code html précisant qu'il n 'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <returns>Code html Généré</returns>
		public static string GetUIEmpty(int language){
			StringBuilder HtmlTxt = new StringBuilder(10000);
			
			HtmlTxt.Append("<TR vAlign=\"top\" >");
			HtmlTxt.Append("<TD vAlign=\"top\">");
			HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			HtmlTxt.Append("<tr><td></td></tr>");
			HtmlTxt.Append("<TR vAlign=\"top\">");
			HtmlTxt.Append("<TD vAlign=\"top\" height=\"50\" class=\"txtViolet11Bold\">");
			HtmlTxt.Append(GestionWeb.GetWebWord(841,language));			
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
			HtmlTxt.Append("</TABLE>");
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");

			return HtmlTxt.ToString();
		}

		/// <summary>
		/// Génère le code html précisant qu'il n 'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <param name="code">code traduction</param>
		/// <returns>Code html Généré</returns>
		public static string GetUIEmpty(int language,int code){
			StringBuilder HtmlTxt = new StringBuilder(10000);
			
			HtmlTxt.Append("<TR vAlign=\"top\" >");
			HtmlTxt.Append("<TD vAlign=\"top\">");
			HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			HtmlTxt.Append("<tr><td></td></tr>");
			HtmlTxt.Append("<TR vAlign=\"top\">");
			HtmlTxt.Append("<TD vAlign=\"top\" height=\"50\" class=\"txtViolet11Bold\">");
			HtmlTxt.Append(GestionWeb.GetWebWord(code,language));			
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
			HtmlTxt.Append("</TABLE>");
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");

			return HtmlTxt.ToString();
		}
		#endregion

		#endregion

		#region OnMethodError
		/// <summary>
		/// Appelé sur erreur à l'exécution 
		/// </summary>
		/// <param name="errorException">Exception</param>		
		protected void OnMethodError(Exception errorException) {
			TNS.AdExpress.Web.Exceptions.CustomerWebException cwe=null;
			try{
				BaseException err=(BaseException)errorException;
				cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(this.Page,err.Message,err.StackTrace,null);
			}
			catch(System.Exception){
				try{
					cwe=new TNS.AdExpress.Web.Exceptions.CustomerWebException(this.Page,errorException.Message,errorException.StackTrace,null);
				}
				catch(System.Exception es){
					throw(es);
				}
			}
			cwe.SendMail();			
		}
		#endregion
	}
}
