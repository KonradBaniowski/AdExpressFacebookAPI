#region Information
//Auteur A.Obermeyer
//date de création : 19/10/04
//date de modification : 30/12/2004  D. Mussuma Intégration de WebPage
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
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using ExcelFunction = TNS.AdExpress.Web.UI.ExcelWebPage;
#endregion

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Description résumée de IndicatorSeasonalityResults.
	/// </summary>
	public partial class IndicatorSeasonalityResults : TNS.AdExpress.Web.UI.PrivateWebPage{		
		#region variables
		/// <summary>
		/// Code HTML des résultats
		/// </summary>
		public string result=""; 
		/// <summary>
		/// Type de l'élément à trier
		/// </summary>
		string itemType="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public IndicatorSeasonalityResults():base(){			
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{		
				#region Textes et langage du site
				//Modification de la langue pour les Textes AdExpress
				TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);			
				#endregion

				#region Calcul du résultat
			
				switch(_webSession.CurrentTab){
					case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES:
						result=TNS.AdExpress.Web.UI.Results.IndicatorPalmaresUI.GetAllPalmaresIndicatorExcelUI(_webSession,itemType);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.EVOLUTION:
						result=TNS.AdExpress.Web.UI.Results.IndicatorEvolutionUI.GetAllEvolutionIndicatorExcelUI(_webSession);
						break;
					case 	
					TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.NOVELTY:
						result = ExcelFunction.GetLogo();
						result += ExcelFunction.GetExcelHeader(_webSession, false, true, false, GestionWeb.GetWebWord(1310, _webSession.SiteLanguage));
						result += TNS.AdExpress.Web.UI.Results.IndicatorNoveltyUI.GetIndicatorNoveltyExcelUI(this,IndicatorNoveltyRules.GetFormattedTable(_webSession,TNS.AdExpress.Constantes.FrameWork.Results.Novelty.ElementType.product),_webSession,TNS.AdExpress.Constantes.FrameWork.Results.Novelty.ElementType.product);
						result += "<br>"+TNS.AdExpress.Web.UI.Results.IndicatorNoveltyUI.GetIndicatorNoveltyExcelUI(this,IndicatorNoveltyRules.GetFormattedTable(_webSession,TNS.AdExpress.Constantes.FrameWork.Results.Novelty.ElementType.advertiser),_webSession,TNS.AdExpress.Constantes.FrameWork.Results.Novelty.ElementType.advertiser);
						result += ExcelFunction.GetFooter(_webSession);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY:
						result=TNS.AdExpress.Web.UI.Results.IndicatorSeasonalityUI.GetIndicatorSeasonalityExcelUI(this,IndicatorSeasonalityRules.GetFormattedTable(_webSession),_webSession);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.MEDIA_STRATEGY:
						result=TNS.AdExpress.Web.UI.Results.IndicatorMediaStrategyUI.GetIndicatorMediaStrategyExcelUI(this,IndicatorMediaStrategyRules.GetFormattedTable(_webSession,_webSession.ComparaisonCriterion),_webSession,true);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS:						
						result=TNS.AdExpress.Web.UI.Results.IndicatorSynthesisUI.GetIndicatorSynthesisExcelUI(_webSession,true);
						break;
					default:
						break;					
				}		
			
				#endregion
			}			
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e){
			DBFunctions.closeDataBase(_webSession);
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
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
		private void InitializeComponent()
		{
            
		}
		#endregion
	}
}
