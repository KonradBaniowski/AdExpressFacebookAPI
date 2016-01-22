#region Information
//Auteur A.Obermeyer
//date de création : 08/12/04
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
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;

using Portofolio = TNS.AdExpressI.Portofolio;
using Domain = TNS.AdExpress.Domain.Web.Navigation;
using System.Reflection;
#endregion

namespace AdExpress.Private.Results.Excel{

	/// <summary>
	/// Affiche les résultats du portefeuille d'un support au format Excel
	/// </summary>
	public partial class PortofolioResults : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur : Chargement de la session
		/// </summary>
		public PortofolioResults():base(){			
		}
		#endregion

		#region DeterminePostBackMode
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			_resultWebControl.CustomerWebSession = _webSession;
			return base.DeterminePostBackMode ();
		}
		#endregion

        #region On PreInit
        /// <summary>
        /// On preinit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {
            try{
            base.OnPreInit(e);
            switch (_webSession.CurrentTab) {
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                    _resultWebControl.SkinID = "portofolioExcelSynthesisResultTable";
                    break;
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    _resultWebControl.SkinID = "portofolioExcelResultTable";
                    break;
            }
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

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">page</param>
		/// <param name="e">arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

                Response.ContentType = "application/vnd.ms-excel";

				Domain.Module module = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
				if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
				object[] parameters = new object[1];
				parameters[0] = _webSession;
				Portofolio.IPortofolioResults portofolioResult = (Portofolio.IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);
				
				#region Resultat
				switch(_webSession.CurrentTab) {
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
						_resultWebControl.Visible = true;
						break;	
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
						_resultWebControl.Visible = false;
						result = portofolioResult.GetDetailMediaHtml(true);
						break;
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY:
					case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
						_resultWebControl.Visible = false;
						//result=TNS.AdExpress.Web.BusinessFacade.Results.PortofolioSystem.GetExcel(Page,_webSession);
						result = portofolioResult.GetStructureHtml(true);
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
		private void InitializeComponent()
		{
           
		}
		#endregion

	}
}
