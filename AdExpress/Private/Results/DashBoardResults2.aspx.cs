#region Informations
// Auteur:  D. V. Mussuma
// Date de cr�ation : 24/02/2005
// Date de modification :
// 
#endregion

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
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Constantes.Customer;
//using AdExpressException=AdExpress.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Date;
using FunctionsScript = TNS.AdExpress.Web.Functions.Script;
using ControlsHeaders=TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Web.Core.Sessions;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using WebModule=TNS.AdExpress.Constantes.Web.Module;
using WebSystem=TNS.AdExpress.Web.BusinessFacade;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Web;


namespace AdExpress.Private.Results
{
	/// <summary>
	/// Page de r�sultat du module Tableau de bord
	/// </summary>
    public class DashBoardResults2 : TNS.AdExpress.Web.UI.ResultWebPage
	{

		#region Variables MMI
		/// <summary>
		/// titre du module
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleTitleWebControl Moduletitlewebcontrol2;
		/// <summary>
		/// Contr�le qui affiche les r�sultats des tableaux de bord
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ResultsDashBoardOptionsWebControl ResultsDashBoardOptionsWebControl1;
		/// <summary>
		/// Bouton de validation des options d'analyse
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl okImageButton;
		/// <summary>
		/// Contr�le de changement de module
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.ModuleBridgeWebControl ModuleBridgeWebControl1;
		/// <summary>
		/// Contr�le d'ent�te
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Headers.HeaderWebControl HeaderWebControl1;
		/// <summary>
		/// Controle tableau r�sultat
		/// </summary>
		protected TNS.FrameWork.WebResultUI.WebControlResultTable webControlResultTable;
		#endregion
		
		#region Variables
		/// <summary>
		/// R�sultat HTML
		///</summary>
		public string result="";										
		/// <summary>
		/// Script de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// JavaScripts � ins�rer
		/// </summary>
		public string scripts="";
		/// <summary>
		/// JavaScripts bodyOnclick
		/// </summary>
		public string scriptBody="";
		/// <summary>
		///identification du M�dia  s�lectionn�
		/// </summary>
		protected string Vehicle="";
		protected TNS.AdExpress.Web.Controls.Headers.InformationWebControl InformationWebControl1;
		/// <summary>
		///Type de M�dia s�lectionn�
		/// </summary>
		private DBClassificationConstantes.Vehicles.names vehicleType;
		//		/// <summary>
		//		/// Date de d�but de chargemebt des donn�es
		//		/// </summary>
		//		private string downloadBeginningDate="";		
		//		/// <summary>
		//		/// Date de fin de chargement des donn�es
		//		/// </summary>
		//		private string downloadEndDate="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur, chargement de la session
		/// </summary>
		public DashBoardResults2():base()
		{	
			//identification du M�dia  s�lectionn�
			Vehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			vehicleType = (DBClassificationConstantes.Vehicles.names)int.Parse(Vehicle);
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
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
//					if(nomInput!=MenuWebControl2.ID)
//					{
//						Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
//						Page.Response.Flush();
//					}
				}
				else
				{
					Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
					Page.Response.Flush();
				}
				#endregion

				#region Url Suivante
				//				_nextUrl=this.recallWebControl.NextUrl;
				if(_nextUrl.Length!=0)
				{
					_webSession.Source.Close();
					Response.Redirect(_nextUrl+"?idSession="+_webSession.IdSession);
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
			
				#region scripts
				scripts = FunctionsScript.ImageDropDownListScripts(ResultsDashBoardOptionsWebControl1.ShowPictures);
				scriptBody = "javascript:openMenuTest();";
				#endregion		

				#region R�sultat
				result = WebSystem.Results.DashBoardSystem.GetHtml(_webSession);
//				webControlResultTable.Data=TNS.AdExpress.Web.Rules.Results.DashBoardRules.GetDataTableForGenericUI(_webSession);
//				webControlResultTable.IdSession = this._webSession.IdSession;
				#endregion

				#region MAJ _webSession
				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
				_webSession.ReachedModule=true;	
				_webSession.Save();
				#endregion			

				#region D�finition de la page d'aide
				//				helpWebControl.Url=WebConstantes.Links.HELP_FILE_PATH+"DashBoardResultsHelp.aspx";
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
			DashBoardOptionsWebControl();						
			//recallWebControl.CustomerWebSession=_webSession;
//			MenuWebControl2.CustomerWebSession = _webSession;
			return tmp;
		}
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation des composants
		/// </summary>
		/// <param name="e">arguments</param>
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		#region M�thodes priv�es
		/// <summary>
		/// Options des Contr�les fils � afficher dans le contr�le du choix des opotions d'analyse
		/// </summary>
		private void DashBoardOptionsWebControl()
		{
			TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables PreformatedTable =TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units;
			Int64 indexPreformatedTable;
			Int64 DashBoardEnumIndex=11;
			int numberImagesForPress=4;
			int numberImagesForOthersMedia=13;
			if(!_webSession.ComparativeStudy)_webSession.Evolution=false;
			
			bool fromSearchSession=false;
			if(Request.UrlReferrer!=null && Request.UrlReferrer.AbsolutePath.IndexOf("SearchSession")>0
				&& _webSession.PeriodType!=CstCustomerSession.Period.Type.LastLoadedWeek && _webSession.PeriodType!=CstCustomerSession.Period.Type.LastLoadedMonth
				)
				fromSearchSession = true;

			//Options encarts
			if(_webSession.CurrentModule==WebModule.Name.TABLEAU_DE_BORD_PRESSE)
			{
				ResultsDashBoardOptionsWebControl1.InsertOption =true;
                if (!Page.IsPostBack && !fromSearchSession && WebApplicationParameters.AllowInsetOption) _webSession.Insert = WebConstantes.CustomerSessions.Insert.withOutInsert;
			}
			else ResultsDashBoardOptionsWebControl1.InsertOption =false;

			//Bouton valider
			if(Request.Form.Get("__EVENTTARGET")=="okImageButton")
			{
				//Tableau demand�
				indexPreformatedTable = Int64.Parse(Page.Request.Form.GetValues("DDLResultsDashBoardOptionsWebControl1")[0]);
				if(vehicleType==DBClassificationConstantes.Vehicles.names.press && indexPreformatedTable>2 )
					PreformatedTable = (TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables) (indexPreformatedTable+DashBoardEnumIndex+(numberImagesForOthersMedia-numberImagesForPress)) ;
				else 
					PreformatedTable = (TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables) (indexPreformatedTable+DashBoardEnumIndex) ;
				_webSession.PreformatedTable = PreformatedTable;
			}
			_webSession.PreformatedProductDetail =  TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector;
			ResultsDashBoardOptionsWebControl1.CustomerWebSession = _webSession;				
			switch(_webSession.PreformatedTable)
			{
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
					if(!fromSearchSession)
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					setRepartitionOption(_webSession);					
					setPeriodOption(true);
					ResultsDashBoardOptionsWebControl1.PdvOption = false;
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.PDV = false;
						_webSession.PDM =true;
						_webSession.Percentage = false;
					}
					if((!Page.IsPostBack && _webSession.LastReachedResultUrl.Length==0) || (!fromSearchSession))
						_webSession.TimeInterval = WebConstantes.Repartition.timeInterval.Total;
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
					ResultsDashBoardOptionsWebControl1.PdmOption = true;
					ResultsDashBoardOptionsWebControl1.UnitOption = false;
					ResultsDashBoardOptionsWebControl1.Percentage = false;
					ResultsDashBoardOptionsWebControl1.InterestCenterListOption = false;
					ResultsDashBoardOptionsWebControl1.SectorListOption = true;										
					break;	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
					ResultsDashBoardOptionsWebControl1.UnitOption=true;				
					setPeriodOption(false);
					setRepartitionOption(_webSession);
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.PDM = true;
						_webSession.PDV = false;
						if(!Page.IsPostBack)_webSession.TimeInterval = WebConstantes.Repartition.timeInterval.Total;
						_webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
						_webSession.Percentage = false;
					}
					ResultsDashBoardOptionsWebControl1.PdvOption = false;
					ResultsDashBoardOptionsWebControl1.PdmOption = true;
					ResultsDashBoardOptionsWebControl1.Percentage = true;
					ResultsDashBoardOptionsWebControl1.MonthlyDateOption=false;
					ResultsDashBoardOptionsWebControl1.WeeklyDateOption=false;
					ResultsDashBoardOptionsWebControl1.InterestCenterListOption = false;
					ResultsDashBoardOptionsWebControl1.SectorListOption = true;
					if(!fromSearchSession)					
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					break;	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :	
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
					if(!fromSearchSession)
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					setPeriodOption(false);				
					ResultsDashBoardOptionsWebControl1.MediaDetailOption=false;
					setRepartitionOption(_webSession);		
					ResultsDashBoardOptionsWebControl1.UnitOption=true;
					ResultsDashBoardOptionsWebControl1.PdvOption = true;
					ResultsDashBoardOptionsWebControl1.PdmOption = false;
					ResultsDashBoardOptionsWebControl1.SectorListOption = false;
					ResultsDashBoardOptionsWebControl1.InterestCenterListOption = true;															
					ResultsDashBoardOptionsWebControl1.Percentage = true;
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.SelectionUniversProduct=new System.Windows.Forms.TreeNode("produit");			
						_webSession.PDM = false;
						_webSession.PDV = true;
						_webSession.Percentage = false;				
						_webSession.Evolution = false;
						if(!Page.IsPostBack)_webSession.TimeInterval = WebConstantes.Repartition.timeInterval.Total;
					}													
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :			
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
					ResultsDashBoardOptionsWebControl1.MediaDetailOption=true;
					ResultsDashBoardOptionsWebControl1.UnitOption=true;
					setPeriodOption(true);	
					setRepartitionOption(_webSession);
					ResultsDashBoardOptionsWebControl1.PdvOption = false;
					ResultsDashBoardOptionsWebControl1.PdmOption = true;
					ResultsDashBoardOptionsWebControl1.InterestCenterListOption = false;
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.PDM = true;
						_webSession.PDV = false;
						_webSession.Percentage = false;
						_webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
					}
					if(!fromSearchSession)
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :												
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
					ResultsDashBoardOptionsWebControl1.MediaDetailOption=false;
					ResultsDashBoardOptionsWebControl1.UnitOption=false;
					ResultsDashBoardOptionsWebControl1.PdvOption = false;
					ResultsDashBoardOptionsWebControl1.PdmOption = false;
					ResultsDashBoardOptionsWebControl1.Percentage = true;
					setPeriodOption(true);
					setRepartitionOption(_webSession);
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.PDM = false;
						_webSession.PDV = false;
						_webSession.Percentage = false;
					}
					if(Page.Request.QueryString.Get("p")==null)
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :				
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:					
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
					ResultsDashBoardOptionsWebControl1.MediaDetailOption=false;
					ResultsDashBoardOptionsWebControl1.UnitOption=true;
					setPeriodOption(true);
					setRepartitionOption(_webSession);
					ResultsDashBoardOptionsWebControl1.PdmOption = false;
					ResultsDashBoardOptionsWebControl1.PdvOption = true;
					ResultsDashBoardOptionsWebControl1.SectorListOption = false;
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.SelectionUniversProduct=new System.Windows.Forms.TreeNode("produit");
						_webSession.PDM = false;
						_webSession.PDV = true;
						_webSession.Percentage = false;
					}
					if(!fromSearchSession)
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					break;	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
					ResultsDashBoardOptionsWebControl1.UnitOption=true;				
					setPeriodOption(true);
					setRepartitionOption(_webSession);
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.PDM = true;
						_webSession.PDV = false;
						if(!Page.IsPostBack)_webSession.TimeInterval = WebConstantes.Repartition.timeInterval.Total;
						_webSession.SelectionUniversProduct=new System.Windows.Forms.TreeNode("produit");
						_webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
						_webSession.Percentage = false;
					}
					ResultsDashBoardOptionsWebControl1.PdvOption = false;
					ResultsDashBoardOptionsWebControl1.PdmOption = true;
					ResultsDashBoardOptionsWebControl1.Percentage = true;					
					ResultsDashBoardOptionsWebControl1.InterestCenterListOption = false;
					ResultsDashBoardOptionsWebControl1.SectorListOption = false;
					if(!fromSearchSession)					
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					break;																																																										
				default : 
					throw new Exception(" DashBoardOptionsWebControl() --> Impossible d'identifier le tableau de bord � traiter.");
			}	

			//Pas de r�partition pour la presse
			if(DBClassificationConstantes.Vehicles.names.press==vehicleType)
			{
				ResultsDashBoardOptionsWebControl1.FormatOption = false;
				ResultsDashBoardOptionsWebControl1.TimeIntervalOption = false;
				ResultsDashBoardOptionsWebControl1.NamedDayOption = false;
				if(IsRepatitionDashBoard(_webSession))
				{
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
						_webSession.PreformatedTable =  WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units;
					if(Page.Request.QueryString.Get("p")==null)
						_webSession.DetailPeriodBeginningDate = _webSession.DetailPeriodEndDate = "";
					setRepartitionOption(_webSession);					
					setPeriodOption(true);
					ResultsDashBoardOptionsWebControl1.PdvOption = false;
					if(_webSession.LastReachedResultUrl.Length==0 || (!fromSearchSession))
					{
						_webSession.PDV = false;
						_webSession.PDM =true;
						_webSession.Percentage = false;
						_webSession.Unit =  WebConstantes.CustomerSessions.Unit.euro;
						_webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
					}
					ResultsDashBoardOptionsWebControl1.PdmOption = true;
					ResultsDashBoardOptionsWebControl1.UnitOption = false;
					ResultsDashBoardOptionsWebControl1.Percentage = false;
					ResultsDashBoardOptionsWebControl1.InterestCenterListOption = false;
				}
			}

			_webSession.Save();
		}

		/// <summary>
		/// Condition pour afficher les repartitions pour TV et Radio seulement
		/// </summary>
		/// <returns>false s'il doit �tre montrer, true sinon</returns>
		private bool showRepartition() 
		{
			Int64 idVehicle = ((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID;
			DBClassificationConstantes.Vehicles.names vehicletype=(DBClassificationConstantes.Vehicles.names)idVehicle;
			switch(vehicletype) 
			{
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.radio:
				case DBClassificationConstantes.Vehicles.names.others:
					return(true);
				default:
					return(false);
			}
		}
		/// <summary>
		/// methode d'affichage des options de repartition
		/// </summary>
		private void setRepartitionOption(WebSession _webSession)
		{
			if(!showRepartition())
			{
				ResultsDashBoardOptionsWebControl1.FormatOption =false;
				ResultsDashBoardOptionsWebControl1.NamedDayOption = false;
				ResultsDashBoardOptionsWebControl1.TimeIntervalOption = false;
			}
				
			switch(_webSession.PreformatedTable)
			{
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual:
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					if(showRepartition())
					{
						ResultsDashBoardOptionsWebControl1.FormatOption = true;
						ResultsDashBoardOptionsWebControl1.NamedDayOption = true;
						ResultsDashBoardOptionsWebControl1.TimeIntervalOption = true;
					}
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :				
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :	
					ResultsDashBoardOptionsWebControl1.FormatOption =false;
					ResultsDashBoardOptionsWebControl1.NamedDayOption =true;
					ResultsDashBoardOptionsWebControl1.TimeIntervalOption =true;
					_webSession.Format =  WebConstantes.Repartition.Format.Total;
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
					ResultsDashBoardOptionsWebControl1.FormatOption =true;
					ResultsDashBoardOptionsWebControl1.NamedDayOption =false;
					ResultsDashBoardOptionsWebControl1.TimeIntervalOption =true;
					_webSession.NamedDay =  WebConstantes.Repartition.namedDay.Total;
					break;
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					ResultsDashBoardOptionsWebControl1.FormatOption =true;
					ResultsDashBoardOptionsWebControl1.NamedDayOption =true;
					ResultsDashBoardOptionsWebControl1.TimeIntervalOption =false;
					_webSession.TimeInterval =  WebConstantes.Repartition.timeInterval.Total;
					break;
				
			}
		}
		/// <summary>
		/// Indique le controle p�riode � afficher	
		/// </summary>
		private void setPeriodOption(bool isDetailPeriod)
		{
			if(isDetailPeriod)
			{
				if(_webSession.DetailPeriod==CstPeriodDetail.monthly)
				{
					ResultsDashBoardOptionsWebControl1.WeeklyDateOption =false;
					ResultsDashBoardOptionsWebControl1.MonthlyDateOption =true;
				}
				else if(_webSession.DetailPeriod==CstPeriodDetail.weekly)
				{
					ResultsDashBoardOptionsWebControl1.MonthlyDateOption =false;
					ResultsDashBoardOptionsWebControl1.WeeklyDateOption =true;
				}
			}
			else
			{
				ResultsDashBoardOptionsWebControl1.MonthlyDateOption =false;
				ResultsDashBoardOptionsWebControl1.WeeklyDateOption = false;
			}
		}
		
		/// <summary>
		/// V�rifie si une tableau de bord de r�partition est s�lectionn�
		/// </summary>
		/// <param name="_webSession">session du client</param>
		/// <returns>vrai si tableau de bord de r�partition est s�lectionn�</returns>
		private static bool IsRepatitionDashBoard(WebSession _webSession)
		{
			switch(_webSession.PreformatedTable)
			{
				
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :				
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :						
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :	
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice:					
				case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					return true;
				default : return false;
				
			}
		}
		

		#endregion

		#region Abstract Methods
		/// <summary>
		/// Retrieve next Url from contextual menu
		/// </summary>
		/// <returns></returns>
		protected override string GetNextUrlFromMenu() 
		{
//			return MenuWebControl2.NextUrl;
			return "";
		}
		#endregion
	}
}
