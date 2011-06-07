#region Information
// Auteur : G Facon
// Créé le : 24/01/2005
// Modifié le : 24/01/2005
//		12/08/2005	G. Facon	Nom de variables
//		30/11/2005	D. Mussuma	Gestion des tableaux de bord
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExeptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.UI{
	/// <summary>
	/// Page mère des page Web de résultats
	/// </summary>
	public class BaseResultWebPage: PrivateWebPage{

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public BaseResultWebPage():base(){
			base.Load +=new EventHandler(ResultWebPage_Load);
			try{
				SetSelectionError();
			}
			catch(System.Exception){}
		}
		#endregion

		#region Méthode internes
		/// <summary>
		/// On vérifie que toutes les variables ont été sélectionnées
		/// </summary>
		/// <returns>0 si tous est sélectionné, sinon l'identifiant de l'élément manquant</returns>
		private WebConstantes.ErrorManager.selectedUnivers CanShowResult(){
			switch(_webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
					//Vehicle sélectionné
					if(!_webSession.isVehicleSelected())return(WebConstantes.ErrorManager.selectedUnivers.vehicle);
					// Media
					if(!_webSession.isCompetitorMediaSelected())return(WebConstantes.ErrorManager.selectedUnivers.media);
					//Date
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					return(WebConstantes.ErrorManager.selectedUnivers.none);
				case WebConstantes.Module.Name.ALERTE_POTENTIELS:
				case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
					//Vehicle sélectionné
					if(!_webSession.isVehicleSelected())return(WebConstantes.ErrorManager.selectedUnivers.vehicle);
					// Media
					if(!_webSession.isCompetitorMediaSelected())return(WebConstantes.ErrorManager.selectedUnivers.media);
					// Au moins 2 univers media
					if(_webSession.mediaUniversNumber()<2)return(WebConstantes.ErrorManager.selectedUnivers.mediaNumber);
					//Date
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					return(WebConstantes.ErrorManager.selectedUnivers.none);

				case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA:
				case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
					//produit
					if(!_webSession.isCurrentAdvertisersSelected())return(WebConstantes.ErrorManager.selectedUnivers.product);
					
					// Media
					if(!_webSession.isMediaSelected())return(WebConstantes.ErrorManager.selectedUnivers.media);					
					//Date
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					return(WebConstantes.ErrorManager.selectedUnivers.none);
				
				case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
				case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
					// Produit référence et concurrent
					if(_webSession.advertiserUniversNumber()<2)return(WebConstantes.ErrorManager.selectedUnivers.product);
					// Media
					if(!_webSession.isMediaSelected())return(WebConstantes.ErrorManager.selectedUnivers.media);	
					//Date
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					return(WebConstantes.ErrorManager.selectedUnivers.none);

				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
					// vehicle
					if(!_webSession.isVehicleSelected())return(WebConstantes.ErrorManager.selectedUnivers.vehicle);
					// Media
					if(!_webSession.isReferenceMediaSelected())return(WebConstantes.ErrorManager.selectedUnivers.media);					
					//Date
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					return(WebConstantes.ErrorManager.selectedUnivers.none);
					
				case WebConstantes.Module.Name.INDICATEUR:
				case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
					//produit
					if(!_webSession.isSelectionProductSelected())return(WebConstantes.ErrorManager.selectedUnivers.product);
					// Media
					if(!_webSession.isMediaSelected())return(WebConstantes.ErrorManager.selectedUnivers.media);
					//Date
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					return(WebConstantes.ErrorManager.selectedUnivers.none);

				case WebConstantes.Module.Name.BILAN_CAMPAGNE:
					//produit
					if(_webSession.advertiserUniversNumber()<1)return(WebConstantes.ErrorManager.selectedUnivers.product);
					//period
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					//target
					if(!_webSession.IsTargetSelected())return(WebConstantes.ErrorManager.selectedUnivers.target);
					return (WebConstantes.ErrorManager.selectedUnivers.none);

				case WebConstantes.Module.Name.TABLEAU_DE_BORD_PRESSE :
				case WebConstantes.Module.Name.TABLEAU_DE_BORD_RADIO :
				case WebConstantes.Module.Name.TABLEAU_DE_BORD_TELEVISION :
				case WebConstantes.Module.Name.TABLEAU_DE_BORD_PAN_EURO :
					//Familles de produits
					//if(_webSession.CurrentUniversProduct==null || _webSession.CurrentUniversProduct.Nodes==null || _webSession.CurrentUniversProduct.Nodes.Count==0)
					//return(WebConstantes.ErrorManager.selectedUnivers.product);
					if (!_webSession.isCurrentAdvertisersSelected()) return (WebConstantes.ErrorManager.selectedUnivers.product);
					// Media
					if(!_webSession.isMediaSelected())return(WebConstantes.ErrorManager.selectedUnivers.media);
					//Date
					if(!_webSession.isDatesSelected())return(WebConstantes.ErrorManager.selectedUnivers.period);
					return(WebConstantes.ErrorManager.selectedUnivers.none);


				default:
					throw(new System.Exception("Le module sélectionné n'est pas défini"));
			}
		
		}
		#endregion

		#region Evènement
        /// <summary>
        /// OnInitComplete
        /// </summary>
        /// <param name="e">Event Argument</param>       
        protected override void OnInitComplete(EventArgs e) {

            try {

                _webSession.IsRetailerDisplay = WebApplicationParameters.UseRetailer && ModulesList.GetModule(_webSession.CurrentModule).GetDisplayRetailerOption(_webSession.CurrentTab);
                _webSession.Save();
            }
            catch (Exception ex) {
                if (ex.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, ex, _webSession));
                }
            }

            base.OnInitComplete(e);
        }

		/// <summary>
		/// Page Loading
		/// </summary>
		/// <param name="sender">Source Object</param>
		/// <param name="e">Arguments</param>
		private void ResultWebPage_Load(object sender, EventArgs e) {
			_nextUrl=GetNextUrlFromMenu();			
		}
        #endregion

		#region Méthodes 
		/// <summary>
		/// Get next URL from contextual menu
		/// </summary>
		/// <returns>Next URL</returns>
		protected virtual string GetNextUrlFromMenu(){
			throw(new NotImplementedException("Doit être implémenté dans l'objet enfant"));
		}
        /// <summary>
        /// Set Selection Error variable
        /// Check if the study univers are selected for the result page (if not we redirect the user to the error page)
        /// </summary>
        protected virtual void SetSelectionError(){
            _selectionError=CanShowResult();
        }
		#endregion

	}
}
