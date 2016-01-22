#region Informations
// Auteur: 
// Date de création: 
//date de modification : 09/11/2005  D. Mussuma Intégration des statistiques
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;


namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Liste avec les différents choix préformatés de niveaux de détail produits
	/// </summary>
	[DefaultProperty("Text"),ToolboxData("<{0}:DetailProductLevelWebControl runat=server></{0}:DetailProductLevelWebControl>")]
	public class DetailProductLevelWebControl : System.Web.UI.WebControls.DropDownList{

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
		#endregion

		#region Evènements

		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
		
			if(Page.IsPostBack){

				#region Choix du niveaux de détails produits			
				if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")!=null && Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]!=webSession.PreformatedProductDetail.ToString()){
					if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector;
					}					
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct;
					}
					//additions
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct.ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct;
					}
					else if(Page.Request.Form.GetValues("DetailProductLevelWebControl2")[0]==WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser .ToString()){
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser;
					}
					else {
						webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup;
					}		
				}
				#endregion
								
			}		
		}

		/// <summary>
		/// PreRender 
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			
			//if(!Page.IsPostBack || this.Items.Count==0){
				this.Items.Clear();
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1103,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1532,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1104,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1491,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1492,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct.ToString()));				
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1105,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct.ToString()));
				
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(858,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product.ToString()));

				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1106,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1108,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct.ToString()));	
				//Right verification for Brand
				if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
				{
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1107,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand.ToString()));	
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1109,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct.ToString()));
				}				
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1110,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1112,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1145,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser.ToString()));
				//Right verification for Brand
				if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
				{
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1111,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand.ToString()));
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1113,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct.ToString()));
						
				}
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1577,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser.ToString()));
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1578,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct.ToString()));				
				//Right verification for Brand
				if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
				{
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1579,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand.ToString()));
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1603,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand.ToString()));
					
				}
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1602,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct.ToString()));
								
				//Rights for Holding company				
				if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY))
				{
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1589,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany.ToString()));
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1590,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser.ToString()));
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1592,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct.ToString()));
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1893,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser.ToString()));
					if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
						this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1591,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand.ToString()));
				}
				// Agence Media
				if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY))
				{
						this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1115,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser.ToString()));
						this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1116,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct.ToString()));
						this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1114,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency.ToString()));
						this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1642,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser.ToString()));
						this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(1643,webSession.SiteLanguage),WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct.ToString()));
						
					//	this.Items.FindByValue(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup.ToString()).Selected=true;
				}

		//	}
			
			this.Items.FindByValue(webSession.PreformatedProductDetail.ToString()).Selected=true;
			//this.AutoPostBack=true;
			this.CssClass="txtNoir11Bold";

			base.OnLoad (e);
		}

		#endregion
		
	}
}
