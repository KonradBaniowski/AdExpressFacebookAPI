#region Informations
// Auteur: D. Mussuma 
// Date de création: 28/07/2006
// Date de modification:
#endregion

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebSystem=TNS.AdExpress.Web.BusinessFacade;

using Dundas.Charting.WebControl;

using AjaxPro;

using Oracle.DataAccess.Client;

namespace TNS.AdExpress.Web.Controls.Results.Appm
{
	/// <summary>
	/// Conteneur des composants destinés à l'APPM.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:AppmContainerWebControl runat=server></{0}:AppmContainerWebControl>")]
	public class AppmContainerWebControl :   System.Web.UI.WebControls.WebControl{
	
		
		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _customerWebSession=null;
		
		/// <summary>
		/// Contrôle qui affiche les résultats de l'APPM sous forme graphique
		/// </summary>
		protected BaseAppmChartWebControl appmChartWebControl = null;

		/// <summary>
		/// Source de données
		/// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource = null;

		/// <summary>
		/// Obtient ou définit le type de l'image
		/// </summary>		
		protected ChartImageType _imageType = ChartImageType.Jpeg;

		/// <summary>
		/// Contrôle qui affiche les résultats de l'APPM sous forme de tableau html
		/// </summary>
		protected AppmWebControl appmHtmlWebControl = null;
        /// <summary>
        /// nom du skin id du controle graphique de l'analyse par famille de presse
        /// </summary>
        private string _analyseFamilyInterestPlanSkinID = string.Empty;
        /// <summary>
        /// nom du skin id du controle graphique des parts de voix
        /// </summary>
        private string _pdvPlanSkinID = string.Empty;
        /// <summary>
        /// nom du skin id du controle graphique de la periodicité
        /// </summary>
        private string _periodicityPlanSkinID = string.Empty;
		#endregion

		#region Accesseurs
	
		/// <summary>
		/// Obtient ou définit la source de données
		/// </summary>
        public TNS.FrameWork.DB.Common.IDataSource Source
        {
			set{_dataSource = value;}
			get{return _dataSource;}
		}
		
		/// <summary>
		/// Obtient ou définit la Session du client
		/// </summary>
		public WebSession CustomerWebSession{
			set{_customerWebSession = value;}
			get{return _customerWebSession;}
		}

		/// <summary>
		/// Obtient ou définit le type de l'image pour le résultat sous forme graphique de l'APPM
		/// </summary>
		public ChartImageType AppmImageType{
			set{_imageType = value;}
			get{return _imageType;}
		}

        /// <summary>
        /// Obtient ou définit le nom du skin id du controle graphique de l'analyse par famille de presse
        /// </summary>
        public string AnalyseFamilyInterestPlanSkinID {
            set { _analyseFamilyInterestPlanSkinID = value; }
            get { return _analyseFamilyInterestPlanSkinID; }
        }
        /// <summary>
        /// Obtient ou définit le nom du skin id du controle graphique des parts de voix
        /// </summary>
        public string PdvPlanSkinID {
            set { _pdvPlanSkinID = value; }
            get { return _pdvPlanSkinID; }
        }
        /// <summary>
        /// Obtient ou définit le nom du skin id du controle graphique de la periodicité
        /// </summary>
        public string PeriodicityPlanSkinID {
            set { _periodicityPlanSkinID = value; }
            get { return _periodicityPlanSkinID; }
        }
		#endregion
		
		#region Evènements

		#region Initialisation
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e){
			base.OnInit(e);
		}
		#endregion

		#region Load
		/// <summary>
		/// Chargement des composants
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e){
			Controls.Clear();
			#region Chargement du Contrôle qui affiche les résultats de l'APPM sous forme graphique
			
			//Resultats sous forme de Graphiques
			if(_customerWebSession!=null && _customerWebSession.Graphics && _customerWebSession.CurrentTab!=APPM.synthesis){
				switch(_customerWebSession.CurrentTab){

					case APPM.interestFamily :
                        appmChartWebControl = new AnalyseFamilyInterestPlanAppmChartWebControl(_customerWebSession, _dataSource, this._imageType,_analyseFamilyInterestPlanSkinID);
						break;
					case APPM.PDVPlan :
						appmChartWebControl = new PdvPlanAppmChartWebControl(_customerWebSession,_dataSource,this._imageType,_pdvPlanSkinID);
						break;
					case APPM.periodicityPlan:
						appmChartWebControl = new PeriodicityPlanAppmChartWebControl(_customerWebSession,_dataSource,this._imageType,_periodicityPlanSkinID);
						break;
				}
                if (appmChartWebControl != null) {
                    //appmChartWebControl.SetDesignMode();
                    appmChartWebControl.EnableViewState = false;
                    appmChartWebControl.ID = "appmChartWebControl_" + this.ID;
                    Controls.Add(appmChartWebControl);
                }
			}

			//Resultats sous forme de Tableaux
			else if(_customerWebSession!=null && !_customerWebSession.Graphics){
					
					appmHtmlWebControl = new AppmWebControl();				
					appmHtmlWebControl.CustomerWebSession = _customerWebSession;
					appmHtmlWebControl.ID="appmHTmlWebControl_"+this.ID;		
					appmHtmlWebControl.EnableViewState=false;
					Controls.Add(appmHtmlWebControl);
			}

			#endregion
			
			base.OnLoad (e);
		}
		
		#endregion
		
		#region PréRender
		/// <summary>
		/// Prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if(_customerWebSession!=null && _customerWebSession.CurrentTab==APPM.supportPlan){
				if (!Page.ClientScript.IsClientScriptBlockRegistered("PopUpInsertion")){
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"PopUpInsertion",WebFunctions.Script.PopUpInsertion(false));
				}
			}
			base.OnPreRender (e);

			
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			base.Render(output);
		}
		#endregion

		#endregion

		
	}
}
