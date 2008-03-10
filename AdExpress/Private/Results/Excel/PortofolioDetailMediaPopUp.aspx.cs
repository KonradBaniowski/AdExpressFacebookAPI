#region Information
//Auteur A.Obermeyer
//date de création : 17/12/04
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
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Results.Excel{

	/// <summary>
	/// Export Excel pour les popup de la planche détail support
	/// dans le module portefeuille
	/// </summary>
    public partial class PortofolioDetailMediaPopUp : TNS.AdExpress.Web.UI.PrivateWebPage {

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _webSession;
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Langue du site
		/// </summary>
		public int _siteLanguage;
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string _idsession="";
		/// <summary>
		/// Code ecran
		/// </summary>
		public string _codeEcran="";
		/// <summary>
		/// Jour de la semaine
		/// </summary>
		public string _dayOfWeek;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public PortofolioDetailMediaPopUp():base(){
			// Chargement de la Session
			_webSession=(WebSession)WebSession.Load(HttpContext.Current.Request.QueryString.Get("idSession"));
			_idsession=HttpContext.Current.Request.QueryString.Get("idSession");
			_codeEcran=HttpContext.Current.Request.QueryString.Get("ecran");
			_dayOfWeek=HttpContext.Current.Request.QueryString.Get("dayOfWeek");
		}
		#endregion

		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Page</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){

            Response.ContentType = "application/vnd.ms-excel";

			#region Textes et langage du site
			//Langue du site
			_siteLanguage=_webSession.SiteLanguage;
			#endregion

            //DataSet dsTable=null;

//			DataSet dsTable=TNS.AdExpress.Web.DataAccess.Results.PortofolioDataAccess.GetDetailMedia(_webSession,((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID,((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID,_webSession.PeriodBeginningDate,_webSession.PeriodEndDate,_codeEcran);
			
            //if(WebFunctions.CheckedText.IsStringEmpty(_codeEcran) || WebFunctions.CheckedText.IsStringEmpty(_dayOfWeek)){
            //    dsTable=TNS.AdExpress.Web.DataAccess.Results.PortofolioDetailMediaDataAccess.GetDetailMedia(_webSession,((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID,((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID,_webSession.PeriodBeginningDate,_webSession.PeriodEndDate,_codeEcran,false);
            //    result=TNS.AdExpress.Web.UI.Results.PortofolioUI.GetExcelDetailMediaPopUpUI(Page,_webSession,dsTable.Tables[0],((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID,((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID,_dayOfWeek,true,false,_codeEcran);
            //}
            //else{
            //    dsTable=TNS.AdExpress.Web.DataAccess.Results.PortofolioDetailMediaDataAccess.GetDetailMedia(_webSession,((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID,((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID,_webSession.PeriodBeginningDate,_webSession.PeriodEndDate,_codeEcran,true);
            //    result=TNS.AdExpress.Web.UI.Results.PortofolioUI.GetExcelDetailMediaPopUpUI(Page,_webSession,dsTable.Tables[0],((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID,((LevelInformation)_webSession.ReferenceUniversMedia.FirstNode.Tag).ID,_dayOfWeek,true,true,"");
            //}
		}
		#endregion

        #region Determine PostBack
        /// <summary>
        /// Determine Post Back
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
            System.Collections.Specialized.NameValueCollection ret = base.DeterminePostBackMode();

            ArrayList columnItemList = new ArrayList();

            PortofolioDetailMediaResultWebControl1.MediaId = HttpContext.Current.Request.QueryString.Get("idMedia");
            PortofolioDetailMediaResultWebControl1.DayOfWeek = HttpContext.Current.Request.QueryString.Get("dayOfWeek");
            PortofolioDetailMediaResultWebControl1.AdBreak = HttpContext.Current.Request.QueryString.Get("ecran");

            PortofolioDetailMediaResultWebControl1.CustomerWebSession = _webSession;

            #region Niveau de colonnes (Generic)
            columnItemList = PortofolioDetailMediaColumnsInformation.GetDefaultMediaDetailColumns(((LevelInformation)_webSession.SelectionUniversMedia.FirstNode.Tag).ID);

            ArrayList columnIdList = new ArrayList();
            foreach (GenericColumnItemInformation Column in columnItemList)
                columnIdList.Add((int)Column.Id);

            PortofolioDetailMediaResultWebControl1.CustomerWebSession.GenericInsertionColumns = new GenericColumns(columnIdList);
            PortofolioDetailMediaResultWebControl1.CustomerWebSession.Save();
            #endregion

            return ret;
        }
        #endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e){
            _webSession.Source.Close();
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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
