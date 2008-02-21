#region Information
// Auteur : D. V. Mussuma
// Créé le : 13/12/2004
// Date de modification : 
//	30/12/2004  D. Mussuma Intégration de WebPage  
//	19/05/2005  K. Shehzad   
//	12/01/2006	B.Masson > Ajout icone pour export excel des unités (Plan média)
#endregion

#region Namespace
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
using TNS.AdExpress.Web.Rules.Results;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.UI.Results;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using DataAccessFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using System.Windows.Forms;
using TNS.AdExpress.Web.Exceptions;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using WebControls=TNS.AdExpress.Web.Controls;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Selection;
#endregion

namespace AdExpress.Private.Results{
	/// <summary>
	/// Pop up qui affiche l'alerte plan média
	/// </summary>
	public partial class MediaPlanAlertPopUpResults :  TNS.AdExpress.Web.UI.PrivateWebPage{
		
		#region Variables	
		/// <summary>
		/// Code html de l'header
		/// </summary>
		public string header = "";	
		/// <summary>
		/// Code html du résultat
		/// </summary>
		public string result = "";		
		/// <summary>
		/// Code html de fermeture du flash d'attente
		/// </summary>
		public string divClose=LoadingSystem.GetHtmlCloseDiv();
		/// <summary>
		/// Séléection du niveau de détail
		/// </summary>
		/// <summary>
		/// Boutton de validation
		/// </summary>
		/// <summary>
		/// Web Control to display a media plan in alert
		/// </summary>
		/// <summary>
		/// Contextual Menu
		/// </summary>
		/// <summary>
		/// Niveau de la nomenclature produit
		/// </summary>
		string Level="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MediaPlanAlertPopUpResults():base(){
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{
	
				#region Flash d'attente
				Page.Response.Write(LoadingSystem.GetHtmlDiv(_webSession.SiteLanguage,Page));
				Page.Response.Flush();
				#endregion	

				#region Textes et Langage du site
				//Bouton valider dans la sous sélection
				okImageButton.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_up.gif";
				okImageButton.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/valider_down.gif";
				InformationWebControl1.Language = _webSession.SiteLanguage;
				#endregion

				_webSession.Save();			

			}		
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion
		
		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Evènement d'initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent(){
          
		}	

		#endregion

		#region DeterminePostBack
		/// <summary>
		/// Détermine la valeur de PostBack
		/// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();

			GenericMediaLevelDetailSelectionWebControl1.CustomerWebSession=_webSession;
			AlertMediaPlanResultWebControl1.CustomerWebSession = _webSession;
			
			MenuWebControl2.CustomerWebSession = _webSession;

			string id="";
			string idVehicle=_webSession.GetSelection(_webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
			if( Page.Request.QueryString.Get("id")!=null)id = Page.Request.QueryString.Get("id").ToString();
			if(Page.Request.QueryString.Get("Level")!=null)Level =  Page.Request.QueryString.Get("Level").ToString();			

			SetProduct(int.Parse(id),int.Parse(Level));
			// On force l'initialisation du composant avec les valeurs du plan media
			switch(_webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ALERTE_POTENTIELS:
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
					GenericMediaLevelDetailSelectionWebControl1.ForceModuleId=WebConstantes.Module.Name.ALERTE_PLAN_MEDIA;
					break;
				case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
				case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
					GenericMediaLevelDetailSelectionWebControl1.ForceModuleId=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
					break;
			}
			return tmp;
		}
		#endregion

		#region Prérender
		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);

			#region Variables
			System.Text.StringBuilder HtmlTxt = new System.Text.StringBuilder(3000);
			#endregion

            try
            {
                #region Résultat
                // Gestion PostBack
                Module currentModuleDescription = TNS.AdExpress.Web.Core.Navigation.ModulesList.GetModule(_webSession.CurrentModule);

                MenuWebControl2.ForbidHelpPages = true;

                WebConstantes.CustomerSessions.Period.Type periodType = _webSession.PeriodType;
                WebConstantes.CustomerSessions.Period.DisplayLevel periodDisplayLevel = _webSession.DetailPeriod;
                string periodBegSave = _webSession.PeriodBeginningDate;
                string periodEndSave = _webSession.PeriodEndDate;

                try
                {
                    if (currentModuleDescription.ModuleType == TNS.AdExpress.Constantes.Web.Module.Type.analysis)
                    {
                        AlertMediaPlanResultWebControl1.Visible = false;

                        if (_webSession.PeriodType == WebConstantes.CustomerSessions.Period.Type.dateToDate)
                        {
                            _webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.monthly;
                        }
                        MediaSchedulePeriod period = new MediaSchedulePeriod(
                            WebFunctions.Dates.getPeriodBeginningDate(periodBegSave, _webSession.PeriodType),
                            WebFunctions.Dates.getPeriodEndDate(periodEndSave, _webSession.PeriodType), 
                            _webSession.DetailPeriod);
                            
                        result = GenericMediaScheduleUI.GetHtml(GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1), _webSession, period, string.Empty).HTMLCode;
                        MenuWebControl2.ForcePrint = "/Private/Results/Excel/MediaPlanResults.aspx?idSession=" + _webSession.IdSession;
                        MenuWebControl2.ForceExcelUnit = "/Private/Results/ValueExcel/MediaPlanResults.aspx?idSession=" + _webSession.IdSession;
                    }
                    else if (currentModuleDescription.ModuleType == TNS.AdExpress.Constantes.Web.Module.Type.alert)
                    {
                        MenuWebControl2.ForcePrint = "/Private/Results/Excel/MediaPlanAlertResults.aspx?idSession=" + _webSession.IdSession;
                        MenuWebControl2.ForceExcelUnit = "/Private/Results/ValueExcel/MediaPlanAlertResults.aspx?idSession=" + _webSession.IdSession;
                    }

                    #region Libellé du module et période
                    HtmlTxt.Append("<TABLE>");
                    //libéllé alerte plan média et période
                    HtmlTxt.Append("<tr height=\"10\" vAlign=\"center\"><td class=\"txtViolet14Bold\">");
                    HtmlTxt.Append(GestionWeb.GetWebWord(751, _webSession.SiteLanguage)
                        + " ( " + GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType), _webSession.SiteLanguage)
                        + GestionWeb.GetWebWord(897, _webSession.SiteLanguage) + WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType), _webSession.SiteLanguage) + " )");
                    HtmlTxt.Append("");
                    HtmlTxt.Append("</td></tr></table>");
                    #endregion
                }
                finally
                {
                    _webSession.DetailPeriod = periodDisplayLevel;
                    _webSession.PeriodType = periodType;
                    _webSession.PeriodBeginningDate = periodBegSave;
                    _webSession.PeriodEndDate = periodEndSave;
                }

                header = HtmlTxt.ToString();


                #endregion

                #region MAJ _webSession
                //				_webSession.LastReachedResultUrl=Page.Request.Url.AbsolutePath;
                _webSession.Save();
                #endregion
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
		}
		#endregion

		#region Méthodes internes

		#region Identifiant des éléments de la nomenclature produit
		private void SetProduct(int id,int level){
			System.Windows.Forms.TreeNode tree=new System.Windows.Forms.TreeNode();
			switch((DetailLevelItemInformation.Levels)_webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(level)){
				case DetailLevelItemInformation.Levels.sector:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialSectorLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.sector,tree);
					break;
				case DetailLevelItemInformation.Levels.subSector:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialSubSectorLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.subsector,tree);
					break;
				case DetailLevelItemInformation.Levels.group:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialGroupLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.group,tree);
					break;
				case DetailLevelItemInformation.Levels.segment:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialSegmentLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.segment,tree);
					break;
				case DetailLevelItemInformation.Levels.product:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialProductLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.product,tree);
					break;
				case DetailLevelItemInformation.Levels.advertiser:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialAdvertiserLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.advertiser,tree);
					break;
				case DetailLevelItemInformation.Levels.brand:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialBrandLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.brand,tree);
					break;
				case DetailLevelItemInformation.Levels.holdingCompany:
					tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess,id,(new TNS.AdExpress.Classification.DataAccess.ProductBranch.PartialHoldingCompanyLevelListDataAccess(id.ToString(),_webSession.SiteLanguage,(OracleConnection)_webSession.Source.GetSource()))[id].ToString());
					tree.Checked=true;
					_webSession.ProductDetailLevel=new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.holding_company,tree);
					break;
			}
		}
		#endregion

		#endregion

	}
}

