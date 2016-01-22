#region Information
// Auteur : A.Obermeyer
// Date de création : 18/04/2005
// Date de modification
#endregion

using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.Controls.Results{

	/// <summary>
	///Affiche la liste des années disponibles pour les agences médias
	/// </summary>
	[DefaultProperty("Text"),ToolboxData("<{0}:MediaAgencyYearWebControl runat=server></{0}:MediaAgencyYearWebControl>")]
	public class MediaAgencyYearWebControl : System.Web.UI.WebControls.DropDownList{

		#region Variables
		/// <summary>
		/// Objet session
		/// </summary>
		protected WebSession webSession=null;
		#endregion

		#region Propriétés
		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		public WebSession WebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// CssClass générale 
		/// </summary>
		[Bindable(true),DefaultValue("txtNoir11Bold"),
		Description("Option choix de l'unité")]
		protected string cssClass = "txtNoir11Bold";
		/// <summary>CSS</summary>
		public string CommonCssClass{
			get{return cssClass;}
			set{cssClass=value;}
		}
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public MediaAgencyYearWebControl(){
		}
		#endregion

		#region Initialisation
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e){
			#region ancienne version
//			// Première page
//			if(!Page.IsPostBack && !webSession.ReachedModule){
//				webSession.MediaAgencyFileYear=DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+webSession.PeriodEndDate.Substring(0,4);					
//			}
//			// Cas MonAdEpress
//			else if(!Page.IsPostBack && webSession.ReachedModule){
//				
//			}
//			else{
//				if(Page.Request.Form.GetValues("MediaAgencyYearWebControl1")!=null){
//					webSession.MediaAgencyFileYear=Page.Request.Form.GetValues("MediaAgencyYearWebControl1")[0];
//				}
//				else{
//					webSession.MediaAgencyFileYear=DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+webSession.PeriodEndDate.Substring(0,4);
//				}
//			}
			#endregion 

			#region Nouvelle version

			// Première page
			if(!Page.IsPostBack && !webSession.ReachedModule){
				webSession.MediaAgencyFileYear=DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+webSession.PeriodEndDate.Substring(0,4);					
			}			
			else{
				if(!(!Page.IsPostBack && webSession.ReachedModule)){//Recupère l'année sélectionnée
					if(Page.Request.Form.GetValues("MediaAgencyYearWebControl1") != null){
						webSession.MediaAgencyFileYear = Page.Request.Form.GetValues("MediaAgencyYearWebControl1")[0];
					}
					else{
						webSession.MediaAgencyFileYear=DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+webSession.PeriodEndDate.Substring(0,4);
					}
				}
			}
					
			#endregion

			webSession.CustomerLogin.ClearModulesList();
			webSession.Save();
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e">Arguements</param>
		protected override void OnPreRender(EventArgs e){
		
			this.Items.Clear();
			
			//Années agences médias disponible pour le média courant
			DataTable dt = TNS.AdExpress.Web.DataAccess.Results.MediaAgencyDataAccess.GetListYear(webSession).Tables[0];

			#region ancienne version					 
//			for(int i=0;i<dt.Rows.Count;i++){
//				this.Items.Add(new System.Web.UI.WebControls.ListItem(dt.Rows[i]["year"].ToString(),DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+dt.Rows[i]["year"].ToString()));			
//			}
//			this.CssClass="txtNoir11Bold";			
			
			
//			if(this.Items.FindByValue(webSession.MediaAgencyFileYear)!=null)
//				this.Items.FindByValue(webSession.MediaAgencyFileYear).Selected=true;
//			else{
//				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1581,webSession.SiteLanguage),DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+DateTime.Today.AddYears(-1).Year));	
//				this.Items.FindByValue(DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+DateTime.Today.AddYears(-1).Year).Selected=true;
//			}	
			#endregion

			#region Nouvelle version
			this.CssClass=cssClass;	
			if(dt!=null && dt.Rows.Count>0){
				for(int i=0;i<dt.Rows.Count;i++){
					this.Items.Add(new System.Web.UI.WebControls.ListItem(dt.Rows[i]["year"].ToString(),DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY+dt.Rows[i]["year"].ToString()));			
				}			
				if(this.Items.FindByValue(webSession.MediaAgencyFileYear)!=null){
					this.Items.FindByValue(webSession.MediaAgencyFileYear).Selected=true;					
				}else{					
					this.Items[0].Selected =true;
					webSession.MediaAgencyFileYear = this.Items[0].Value; 					
					webSession.Save();
				}	
			}else this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1581,webSession.SiteLanguage),webSession.MediaAgencyFileYear));	

			#endregion

			
			base.OnLoad (e);
		}
		#endregion

		#region Affichage de la liste ??
		/// <summary>
		/// Indique si l'on doit afficher les agences  médias
		/// </summary>
		/// <returns></returns>
		public bool DisplayListMediaAgency(){
			if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY)){
				return true;
			}
			else return false;
		}
		#endregion

	}
}
