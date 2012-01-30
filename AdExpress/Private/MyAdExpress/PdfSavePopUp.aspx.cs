#region Informations
// Auteur: D. Mussuma
// Date de création: 02/02/2007
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
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Result;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.DataAccess.Classification;

using Oracle.DataAccess.Client;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.DataAccess.Classification.ProductBranch;

namespace AdExpress.Private.MyAdExpress
{
	/// <summary>
	/// Page de demande d'export de résultat au format PDF
	/// </summary>
	public partial class PdfSavePopUp : TNS.AdExpress.Web.UI.PrivateWebPage { 

		#region Variables

		#region variables fiche justificatives
		/// <summary>
		/// Identifiant du media
		/// </summary>
		private string _idMedia = null;
		/// <summary>
		/// Identifiant du produit
		/// </summary>
		private string _idProduct = null;
		/// <summary>
		/// Date faciale
		/// </summary>
		private string _dateCover = null;
		/// <summary>
		/// Date parution
		/// </summary>
		private string _dateParution = null;
		/// <summary>
		/// Page
		/// </summary>
		private string _pageNumber = null;
		#endregion

        #region Variables Plan média
        /// <summary>
        /// Zoom parameters in Media Plan
        /// </summary>
        string zoomDate = string.Empty;
        #endregion

        private string _idDataPromotion = null;
        private string _resultType = null;
        #endregion

        #region Varaibles MMI
        /// <summary>
		/// Libellé du nom de fichier
		/// </summary>
		/// <summary>
		/// Libellé du mail
		/// </summary>
		/// <summary>
		/// Nom du fichier
		/// </summary>
		/// <summary>
		/// Mail
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		/// <summary>
		/// Titre de la sauvegarde
		/// </summary>

		/// <summary>
		/// Case à cocher mémoriser adresse email 
		/// </summary>
		/// <summary>
		/// Bouton fermer
		/// </summary>
		

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : chargement de la session
		/// </summary>
		public PdfSavePopUp():base() {			
	}
		#endregion

		#region Evènements

		#region Chargement de la page
		/// <summary>
		/// Evènement de chargement de la page
		/// </summary>		
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			try{
				

				#region Paramètres pour les fiches justificatives
				//Récupération des paramètres de l'url
				if(_webSession.CurrentModule == Module.Name.JUSTIFICATIFS_PRESSE ){
					_idMedia	= Page.Request.QueryString.Get("idmedia");
					_idProduct	= Page.Request.QueryString.Get("idproduct");
					_dateCover = Page.Request.QueryString.Get("dateCover");
					_dateParution = Page.Request.QueryString.Get("dateParution");
					_pageNumber		= Page.Request.QueryString.Get("page");
				}
				#endregion

                #region Paramètres Plans média
                zoomDate = Page.Request.QueryString.Get("zoomDate");
                #endregion
                _idDataPromotion = Page.Request.QueryString.Get("idDataPromotion");
                _resultType = Page.Request.QueryString.Get("resultType");

				#region Gestion des cookies

				#region Cookies enregistrement des préférences
				
				//Vérifie si le navigateur accepte les cookies
				if(Request.Browser.Cookies){
					cbxRegisterMail.Text = GestionWeb.GetWebWord(2117,_webSession.SiteLanguage);
					cbxRegisterMail.CssClass = "txtViolet11Bold";
					
					HttpCookie isRegisterEmailForRemotingExport = null, savedEmailForRemotingExport = null ;
					cbxRegisterMail.Visible = true; //RegisterMailLabel.Visible = true;

					if(!Page.IsPostBack){
						WebFunctions.Cookies.LoadSavedEmailForRemotingExport(Page,isRegisterEmailForRemotingExport, savedEmailForRemotingExport,cbxRegisterMail,tbxMail);
					}
				}else cbxRegisterMail.Visible = false; // = RegisterMailLabel.Visible = false;

				#endregion

				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		
		}
		#endregion

		/// <summary>
		/// Femeture de la fenêtre
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void closeRollOverWebControl_Click(object sender, System.EventArgs e) {
			this.ClientScript.RegisterClientScriptBlock(this.GetType(),"closeScript",WebFunctions.Script.CloseScript());
		}

		/// <summary>
		/// Lancer une génération
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Arguments</param>
		protected void validateRollOverWebControl_Click(object sender, System.EventArgs e) {
			string fileName=tbxFileName.Text;
			string mail=tbxMail.Text;
            try
            {
				if(fileName==null || mail==null || fileName.Length==0 || mail.Length==0) {
					this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(1748,_siteLanguage)));
				}
				else if (!WebFunctions.CheckedText.CheckedMailText(mail)) {
					this.ClientScript.RegisterClientScriptBlock(this.GetType(),"alert",WebFunctions.Script.Alert(GestionWeb.GetWebWord(2041,_siteLanguage)));
				} 
				else {

					#region Gestion des cookies
					
					#region Cookies enregistrement des préférences
				
					//Vérifie si le navigateur accepte les cookies
					if(Request.Browser.Cookies){						
						 WebFunctions.Cookies.SaveEmailForRemotingExport(Page,mail,cbxRegisterMail);						
					}
					#endregion

					#endregion

					_webSession.ExportedPDFFileName = fileName;
					string[] mails=new string[1];
					mails[0]=mail;
					_webSession.EmailRecipient=mails;
					Int64 idStaticNavSession = 0;

					switch(_webSession.CurrentModule){

						case Module.Name.BILAN_CAMPAGNE :
							idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.mnevis);
							break;
                        //Plan média
						case Module.Name.ANALYSE_CONCURENTIELLE :
                        case Module.Name.ANALYSE_DYNAMIQUE:
                        case Module.Name.ANALYSE_PLAN_MEDIA:
                        case Module.Name.ANALYSE_PORTEFEUILLE:
                        case Module.Name.ANALYSE_DES_DISPOSITIFS:
                        case Module.Name.ANALYSE_DES_PROGRAMMES:
                        case Module.Name.ALERTE_PORTEFEUILLE:

                            #region Classification Filter Init
                            string id = "";
                            string Level = "";
                            if (Page.Request.QueryString.Get("id") != null) id = Page.Request.QueryString.Get("id").ToString();
                            if (Page.Request.QueryString.Get("Level") != null) Level = Page.Request.QueryString.Get("Level").ToString();

                            if (id.Length > 0 && Level.Length > 0)
                            {
                                SetProduct(int.Parse(id), int.Parse(Level));
                            }
                            #endregion

                            #region Period Detail
                            DateTime begin;
                            DateTime end;
                            if (zoomDate != null && zoomDate != string.Empty)
                            {
                                if (_webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.weekly)
                                {
                                    begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
                                    end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateWeek);
                                }
                                else
                                {
                                    begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
                                    end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantesPeriod.Type.dateToDateMonth);
                                }
                                begin = WebFunctions.Dates.Max(begin,
                                    WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType));
                                end = WebFunctions.Dates.Min(end,
                                    WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType));

                                _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
                            }
                            else
                            {
                                begin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                                end = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
                                if (_webSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                                {
                                    _webSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                                }
                            }
                            _webSession.PeriodBeginningDate = begin.ToString("yyyyMMdd");
                            _webSession.PeriodEndDate = end.ToString("yyyyMMdd");
                            switch (_webSession.PeriodType)
                            {
                                case ConstantesPeriod.Type.currentYear:
                                case ConstantesPeriod.Type.dateToDateMonth:
                                case ConstantesPeriod.Type.dateToDateWeek:
                                case ConstantesPeriod.Type.LastLoadedMonth:
                                case ConstantesPeriod.Type.LastLoadedWeek:
                                case ConstantesPeriod.Type.nextToLastYear:
                                case ConstantesPeriod.Type.nLastMonth:
                                case ConstantesPeriod.Type.nLastWeek:
                                case ConstantesPeriod.Type.nLastYear:
                                case ConstantesPeriod.Type.previousWeek:
                                case ConstantesPeriod.Type.previousYear:
                                    _webSession.PeriodType = ConstantesPeriod.Type.dateToDate;
                                    break;
                            }
                            _webSession.CustomerPeriodSelected = new CustomerPeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate);
                            #endregion
 
                            idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.miysis);
                            //idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.unknown);
							break;
						case Module.Name.INDICATEUR :
							idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.hotep);
							break;
						case Module.Name.JUSTIFICATIFS_PRESSE :
							ProofDetail pDetail = new ProofDetail(_webSession, _idMedia, _idProduct, _dateCover, _dateParution, _pageNumber);
							idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(pDetail,TNS.AdExpress.Anubis.Constantes.Result.type.shou,_webSession.ExportedPDFFileName);
							break; 
						case Module.Name.DONNEES_DE_CADRAGE:
							idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession,TNS.AdExpress.Anubis.Constantes.Result.type.aton);
							break;
                        case Module.Name.VP:
                            if (!string.IsNullOrEmpty(_resultType))
                            {
                                if (Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.selket.GetHashCode())
                                {
                                    if (!string.IsNullOrEmpty( _idDataPromotion)) _webSession.IdPromotion = long.Parse(_idDataPromotion);
                                    idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.selket);
                                }
                                else if (Convert.ToInt32(_resultType) == TNS.AdExpress.Anubis.Constantes.Result.type.thoueris.GetHashCode())
                                {
                                    idStaticNavSession = TNS.AdExpress.Anubis.BusinessFacade.Result.ParameterSystem.Save(_webSession, TNS.AdExpress.Anubis.Constantes.Result.type.thoueris);
                                }
                            }
                            break;
						//default :
							//throw new AdExpress. Exceptions.PdfSavePopUpException(" Impossssile d'identifier le module.");
							
					}
				 
						
					closeRollOverWebControl_Click(this,null);
				}
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
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
		override protected void OnInit(EventArgs e) {
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
		private void InitializeComponent() {
           
		}
		#endregion

        #region Identifiant des éléments de la nomenclature produit
        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        private void SetProduct(int id, int level)
        {
            WebFunctions.ProductDetailLevel.SetProductLevel(_webSession, id, level);           
        }
        #endregion

    }
}

