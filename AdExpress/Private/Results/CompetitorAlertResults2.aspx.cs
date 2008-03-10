
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebBF=TNS.AdExpress.Web.BusinessFacade;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.WebResultUI;

namespace AdExpress.Private.Results
{
	/// <summary>
	/// Page Alerte concurrentielle
	/// </summary>
	public class CompetitorAlertResults2 : TNS.AdExpress.Web.UI.ResultWebPage
	{

		#region Variables MMI
		/// <summary>
		/// Contr�le Titre du module
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleTitleWebControl Moduletitlewebcontrol2;
		/// <summary>
		/// Contr�le Options des r�sultats
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ResultsOptionsWebControl ResultsOptionsWebControl1;
		/// <summary>
		/// Contr�le passerelle vres les autres modules
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleBridgeWebControl ModuleBridgeWebControl1;
		/// <summary>
		/// Contr�le menu d'ent�te 
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.HeaderWebControl HeaderWebControl1;
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Niveau de d�tail produit
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Results.DetailProductLevelWebControl DetailProductLevelWebControl2;
		/// <summary>
		/// Bouton ok
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl okImageButton;
		/// <summary>
		/// Texte du d�tail produit
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText detailProductAdExpressText;
		/// <summary>
		/// Initialisation des produits
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.InitializeProductWebControl InitializeProductWebControl1;
		/// <summary>
		/// Controle tableau r�sultat
		/// </summary>
		protected TNS.FrameWork.WebResultUI.WebControlResultTable webControlResultTable;
		#endregion
				
		#region Variables
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession;
		/// <summary>
		/// Code HTML du r�sultat
		/// </summary>
		public string result="";		
		/// <summary>
		/// Liste des annonceurs s�lectionn�s
		/// </summary>
		protected string listAdvertiserUI="";
		/// <summary>
		/// Texte : Agence m�dia
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Translation.AdExpressText mediaAgencyText;
		/// <summary>
		/// WebControl affichant la liste des ann�es pour les agences m�dias
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Results.MediaAgencyYearWebControl MediaAgencyYearWebControl1;
		/// <summary>
		/// Script de gestion du rappel annonceur
		/// </summary>
		public string scriptAdvertiserRemind;
		
		
		protected TNS.AdExpress.Web.Controls.Headers.InformationWebControl InformationWebControl1;	
		/// <summary>
		/// Bool�en pr�cisant si l'on doit afficher les agences m�dias
		/// </summary>
		public bool displayMediaAgencyList=true;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public CompetitorAlertResults2():base()
		{
			// Chargement de la Session
			try
			{				
				idsession=HttpContext.Current.Request.QueryString.Get("idSession");				
			}
			catch(System.Exception )
			{
				Response.Write(WebFunctions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage."));
				Response.Flush();
			}
		}
		#endregion

		#region Ev�nements

		#region Javascript
		/// <summary>
		/// G�n�re les javascripts utilis�s pour le controle des listes + Ouverture du popup pour la page d'aide
		/// </summary>
		/// <returns>Code Javascript</returns>
		private string Pagination(String result) {
			StringBuilder script=new StringBuilder(2000);
			string newResult = "";
			newResult = result.Replace("\"","\\\"");
			newResult = newResult.Replace("'","\\'");
			newResult = newResult.Replace("\r\n\t\t","");
			newResult = newResult.Replace("\r\n\t","");

			script.Append("<script language=\"JavaScript\">");

			script.Append("\r\nvar op = -497;");
			script.Append("\r\ntab = new Array();");
			script.Append("\r\nvar result;");
			script.Append("\r\nresult = '" + newResult + "';");
//			script.Append("\r\nresult.replace(\"\"\",\"\\\"\");");
			script.Append("\r\ntab = result.split(\"</tr>\");");
			script.Append("\r\n\nfunction next(){");
			script.Append("\r\nop=op+500;");
			script.Append("\r\nvar html='<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0>'+tab[0]+tab[1]+tab[2];");
			script.Append("\r\nvar i;");
			script.Append("\r\nfor(i=op;i<op+500;i++)");
			script.Append("\r\n{");
			script.Append("\r\nhtml+=tab[i];");
			script.Append("\r\n}");
			script.Append("\r\nhtml+='</table>';");
			script.Append("\r\nvar oN=document.getElementById('supertab');");
			script.Append("\r\noN.innerHTML=html;");
			script.Append("\r\n}");
			script.Append("\r\n</script>");
			return(script.ToString());
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page : 
		///		Flash d'attente
		///		Initialisation des connections � la BD
		///		Redirection en cas changement d'un crit�re de s�lection
		///		Traduction du site
		///		Extraction du code HTML r�pondant � la s�lection utilisateur
		///		MAJ dans la session de la derni�re page de r�sultats atteinte lors de la navigation
		/// </summary>		
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		private void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				#region Gestion du flash d'attente
				if(Page.Request.Form.GetValues("__EVENTTARGET")!=null)
				{
					string nomInput=Page.Request.Form.GetValues("__EVENTTARGET")[0];
					
				}
				else
				{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
				}
				#endregion

				#region Url Suivante
				
				if(_nextUrl.Length!=0)
				{
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+idsession);
				}
				#endregion

				#region Validation du menu
				if(Page.Request.QueryString.Get("validation")=="ok")
				{
					_webSession.Save();				
				}
				#endregion

				#region Textes et Langage du site
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
				Moduletitlewebcontrol2.CustomerWebSession=_webSession;
				ModuleBridgeWebControl1.CustomerWebSession=_webSession;
				InformationWebControl1.Language = _webSession.SiteLanguage;
				//				ExportWebControl1.CustomerWebSession=_webSession;			
				#endregion
			
				#region D�finition de la page d'aide
				//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"CompetitorAlertResultsHelp.aspx";
				#endregion

				#region S�lection du vehicle
				string vehicleSelection=_webSession.GetSelection(_webSession.SelectionUniversMedia,Right.type.vehicleAccess);
				DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
				if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.CompetitorRulesException("La s�lection de m�dias est incorrecte"));
				#endregion
                
				displayMediaAgencyList=MediaAgencyYearWebControl1.DisplayListMediaAgency();

				#region choix du type d'encarts
						
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName)
				{
					#region Affichage encarts en fonction du module
					switch(_webSession.CurrentModule)
					{
						case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
						case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
						case WebConstantes.Module.Name.ALERTE_POTENTIELS:	
						case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
							ResultsOptionsWebControl1.InsertOption=true;	
							break;
						default :
							ResultsOptionsWebControl1.InsertOption=false;																																	
							break;
					}
					#endregion					
				}			
				else ResultsOptionsWebControl1.InsertOption=false;
			
				#endregion

				if(_webSession.CurrentTab == TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert.SYNTHESIS)
				{
					DetailProductLevelWebControl2.Visible = false;
					detailProductAdExpressText.Visible=false;
					ResultsOptionsWebControl1.Percentage=false;
					_webSession.PDM=false;					
				}
				else 
				{
					DetailProductLevelWebControl2.Visible = true;
					detailProductAdExpressText.Visible=true;
				}


				#region R�sultat
				//Code html des r�sultats
				result="";			
				// Initialisation de preformatedProductDetail				
				//				if(_webSession.PreformatedProductDetail==TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserProduct
				//					|| !_webSession.ReachedModule
				//					) {			
				//					_webSession.PreformatedProductDetail=TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;	 
				//				}

				if(_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess) == DBClassificationConstantes.Vehicles.names.press.GetHashCode().ToString()
					&& !_webSession.ReachedModule
					)
				{
					_webSession.Unit=WebConstantes.CustomerSessions.Unit.pages;
				}
				
				// }
				// Calcul du r�sultat
//				result=WebBF.Results.CompetitorSystem.GetHtml(this.Page,_webSession);
				webControlResultTable.Data=TNS.AdExpress.Web.Rules.Results.CompetitorRules.GetGenericData(_webSession);
				webControlResultTable.IdSession = this._webSession.IdSession;
				
//				if(!this.Page.ClientScript.IsClientScriptBlockRegistered("Pagination"))this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"Pagination",Pagination(result));
			
				#endregion

				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.ReachedModule=true;
				_webSession.Save();
				#endregion
			}
			catch(System.Exception exc)
			{
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
				{
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page:
		///		Fermeture des connections BD
		/// </summary>		
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e)
		{			
		}
		#endregion
		
		#region DeterminePostBackMode
		/// <summary>
		/// Evaluation de l'�v�nement PostBack:
		///		base.DeterminePostBackMode();
		///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() 
		{
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			ResultsOptionsWebControl1.CustomerWebSession = _webSession;
			//recallWebControl.CustomerWebSession=_webSession;
			DetailProductLevelWebControl2.WebSession=_webSession;
			InitializeProductWebControl1.CustomerWebSession=_webSession;
			MediaAgencyYearWebControl1.WebSession=_webSession;
			
			return tmp;
		}
		#endregion
	
		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output)
		{
			try
			{				
				int i;
				if(_webSession.CompetitorUniversMedia.Count==1)
				{
					//Supprime l'affichage des onglets communs,absents,exclusifs,synth�se s'il n'ya pas d'univers concurrents
					//					for(i=1;i<=3;i++){
					for(i=1;i<=4;i++)
					{
						ResultsOptionsWebControl1.resultsPages.Items.Remove(ResultsOptionsWebControl1.resultsPages.Items.FindByValue(i.ToString()));
					}					
					
				}
			}
			catch(System.Exception exc)
			{
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
				{
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
			base.Render(output);
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e) 
		{
			//
			// CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent() 
		{    
			this.Unload += new System.EventHandler(this.Page_UnLoad);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
		
		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() 
		{
			return "";
		}
		#endregion

	}
}
