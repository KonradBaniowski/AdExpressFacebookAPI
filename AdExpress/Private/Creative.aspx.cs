#region Information
// Auteur : G. Facon
// Créé le : 25/10/2005
// Modifié le : 
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpressWebRules=TNS.AdExpress.Web.Rules;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.UI;
using ProductClassification=TNS.AdExpress.Classification.DataAccess.ProductBranch;
using MediaClassification=TNS.AdExpress.Classification.DataAccess.MediaBranch;
using AdExpressException=TNS.AdExpress.Exceptions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;

using TNS.FrameWork.Date;

namespace AdExpress.Private{
	/// <summary>
	/// Point d'entrer du site AdExpress Pour TNS Creative Explorer
	/// </summary>
	public partial class Creative : System.Web.UI.Page{
    
		#region Constantes
		/// <summary>
		/// Login de TNS Creative Explorer
		/// </summary>
		private const string LOGIN="CREATIVE3";
		/// <summary>
		/// Password de TNS Creative Explorer
		/// </summary>
		private const string PASSWORD="EXPLOV3";
		#endregion

        #region Variables
        /// <summary>
        /// Langue de la page
        /// </summary>
        private int _siteLanguage;
        #endregion

        #region Evènements

        #region Chargement de la Page
        /// <summary>
		/// Chargement de la Page
		/// </summary>
		/// <param name="sender">Objet Source</param>
		/// <param name="e">Paramètres</param>
		protected void Page_Load(object sender, System.EventArgs e){

			System.Windows.Forms.TreeNode tmpNode;
			Int64 id=0;
            NomenclatureElementsGroup nomenclatureElementsGroup = null;
            AdExpressUniverse adExpressUniverse = null;
            Dictionary<int, AdExpressUniverse> universeDictionary = null;
            List<long> products = null;
            WebSession webSession = null;
			try{

				#region On récupère les paramètres de TNS Creative Explorer
				string dateBegin=HttpContext.Current.Request.QueryString.Get("date_debut");
				string dateEnd=HttpContext.Current.Request.QueryString.Get("date_fin");
				string vehicleListIdString=HttpContext.Current.Request.QueryString.Get("idVehicle");
                string vehicleId = string.Empty;
                string productListIdString = HttpContext.Current.Request.QueryString.Get("listeProduct");
                string[] versionListId;
                string versionListIdString = HttpContext.Current.Request.QueryString.Get("listeVersion");
				_siteLanguage=int.Parse(HttpContext.Current.Request.QueryString.Get("langueSite"));
				#endregion

				#region On test les paramètres
				if(dateBegin.Length!=8 || dateEnd.Length!=8) throw (new AdExpressException.AdExpressCustomerException("Paramètres dates invalides"));
				if(vehicleListIdString.Length<1) throw (new AdExpressException.AdExpressCustomerException("Paramètre media invalide"));
                if(productListIdString.Length<1) throw (new AdExpressException.AdExpressCustomerException("Paramètre produit invalide"));
				//Verification de la page d'appel
				//Page.Request.UrlReferrer								 
				#endregion

                string[] vehicleListId = vehicleListIdString.Split(',');
                string[] productListId = productListIdString.Split(',');

				#region On crée la Session (authentification)
			
				//WebSession webSession=null;
				//Creer l'objet Right
                TNS.FrameWork.DB.Common.IDataSource source = new TNS.FrameWork.DB.Common.OracleDataSource("User Id=" + LOGIN + "; Password=" + PASSWORD + " " + TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING);
				TNS.AdExpress.Web.Core.WebRight loginRight=new TNS.AdExpress.Web.Core.WebRight(LOGIN,PASSWORD,source);
				//Vérifier le droit d'accès au site
				if (loginRight.CanAccessToAdExpress(source)){
					// Regarde à partir de quel tables charger les droits clients
					// (template ou droits propres au login)
					loginRight.htRightUserBD();				
					if(true){
						//Creer une session
						if(webSession==null) webSession = new WebSession(loginRight);
						// On Met à jour la session avec les paramètres
						webSession.CurrentModule=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
						webSession.Insert=TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;
						webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
						webSession.LastReachedResultUrl="";

                        #region Media
                        try {
                            webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
                            MediaClassification.PartialVehicleListDataAccess vehicleLabels = new MediaClassification.PartialVehicleListDataAccess(vehicleListIdString, _siteLanguage, loginRight.Connection);
                            foreach (string currentVehicleId in vehicleListId) {
                                id = Int64.Parse(currentVehicleId);
                                tmpNode = new System.Windows.Forms.TreeNode(vehicleLabels[id]);
                                tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, Int64.Parse(currentVehicleId), vehicleLabels[id]);
                                webSession.CurrentUniversMedia.Nodes.Add(tmpNode);
                            }
                            webSession.SelectionUniversMedia = (System.Windows.Forms.TreeNode)webSession.CurrentUniversMedia.Clone();
                        }
                        catch (System.Exception err) {
                            throw (new AdExpressException.AdExpressCustomerException("Impossible de construire l'univers media : " + err.Message));
                        }
                        #endregion

                        #region Produit
                        try {
                            nomenclatureElementsGroup = new NomenclatureElementsGroup(0, AccessType.includes);

                            products = new List<long>();
                            foreach (string currentProductId in productListId) {
                                products.Add(Int64.Parse(currentProductId));
                            }

                            nomenclatureElementsGroup.AddItems(TNSClassificationLevels.PRODUCT, products);
                            adExpressUniverse = new AdExpressUniverse(Dimension.product);
                            adExpressUniverse.AddGroup(0, nomenclatureElementsGroup);
                            universeDictionary = new Dictionary<int, AdExpressUniverse>();
                            universeDictionary.Add(0, adExpressUniverse);
                            webSession.PrincipalProductUniverses = universeDictionary;
                        }
                        catch (System.Exception err) {
                            throw (new AdExpressException.AdExpressCustomerException("Impossible de construire l'univers produit : " + err.Message));
                        }
                        #endregion

                        #region Dates
                        webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.dayly;
                        webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDate;
                        webSession.PeriodBeginningDate = dateBegin;
                        webSession.PeriodEndDate = dateEnd;
                        #endregion

                        #region Versions
                        try {
                            if (versionListIdString != null && versionListIdString.Length > 0) {
                                versionListId = versionListIdString.Split(',');
                                webSession.IdSlogans = new ArrayList();
                                if(vehicleListId.Length>1)
                                    throw (new AdExpressException.AdExpressCustomerException("On ne peut pas avoir un plan media pluri media si on passe un numéro de version en paramètre"));
                                foreach (string idVersion in versionListId) {
                                    vehicleId = idVersion.Substring(0, 1);
                                    if(!vehicleId.Equals(vehicleListIdString))
                                        throw (new AdExpressException.AdExpressCustomerException("La version " + idVersion + " n'appartient pas au media : " + vehicleListIdString));
                                    webSession.IdSlogans.Add(Int64.Parse(idVersion.Substring(1)));
                                }
                            }
                        }
                        catch (System.Exception err) {
                            throw (new AdExpressException.AdExpressCustomerException("Impossible de construire la liste des versions : " + err.Message));
                        }
                        #endregion

                        #region Niveau de détail générique
                        ArrayList levels = new ArrayList();
                        levels.Add(1);
                        levels.Add(2);
                        levels.Add(3);
                        webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                        #endregion

                        webSession.SiteLanguage=_siteLanguage;
						//Sauvegarder la session
						webSession.Save();
						Response.Redirect("/Private/Results/CreativeMediaPlanResults.aspx?idSession="+webSession.IdSession);
						Response.Flush();
					}
				}
				throw (new AdExpressException.AdExpressCustomerException("Connection impossible"));
			}
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
                    WebFunctions.CreativeErrorTreatment.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, webSession),Page,_siteLanguage);
                }
            }
            #endregion

            // On affiche le résultat
		
		}
		#endregion

		#endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Paramètres</param>
		override protected void OnInit(EventArgs e){
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
	}
}
