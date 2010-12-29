#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification: 
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
using TNS.AdExpress.Bastet.BusinessFacade;
using TNS.FrameWork.DB.Common;
using IsisCommon=TNS.Isis.Right.Common;
using TNS.AdExpress.Bastet.Web;

namespace BastetWeb.Indicators{
	/// <summary>
	/// Page de résultat des indicateurs
	/// </summary>
	public partial class IndicatorsResult : PrivateWebPage{

		#region Variables
		/// <summary>
		/// Résultat html
		/// </summary>
		public string _result = String.Empty;
		#endregion

		#region Variables MMI
		#endregion
	
		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try{

				#region Vérification des éléments en session
				// Ecriture des dates dans la session pour tester
				//Session.Add("DateBegin","20070201");
				//Session.Add("DateEnd","20070302");
                Session.Add(TNS.AdExpress.Bastet.Constantes.Web.WebSession.VEHICLE_LIST, "1,2,3");

                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN] == null) throw (new SystemException("Aucun login en session"));
                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_BEGIN] == null) throw (new SystemException("Aucune date de début en session"));
                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_END] == null) throw (new SystemException("Aucune date de fin en session"));
                if (Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.VEHICLE_LIST] == null) throw (new SystemException("Aucune média en session"));
				#endregion

                HeaderWebControl1.LanguageId = _siteLanguage;
                HeaderWebControl1.Type_de_page = TNS.AdExpress.Bastet.WebControls.PageType.generic;

				#region Résultat HTML
                _result = IndicatorsBusinessFacade.GetIndicators(((IsisCommon.Login)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.LOGIN]).Source, Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.VEHICLE_LIST].ToString(), (DateTime)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_BEGIN], (DateTime)Session[TNS.AdExpress.Bastet.Constantes.Web.WebSession.DATE_END], _siteLanguage, WebApplicationParameters.AllowedLanguages[_siteLanguage].ClassificationLanguageId);
				#endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new ErrorEventArgs(this, exc));
				}
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
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
