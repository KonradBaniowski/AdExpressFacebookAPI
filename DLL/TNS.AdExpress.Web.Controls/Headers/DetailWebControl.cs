#region Informations
// Auteur: Y.R'kaina
// Date de création: 18/01/2007  
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Controls.Results;

using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using CustomerCst = TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Description résumée de DetailWebControl.
	/// </summary>
	[ToolboxData("<{0}:DetailWebControl runat=server></{0}:DetailWebControl>")]
	public class DetailWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Choix du niveau de détail produit
		/// </summary>
		public RadioButtonList productDetail;
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession customerWebSession = null;
		#endregion		

		#region Propriétés
		/// <summary>
		/// Websession
		///</summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}

		/// <summary>
		/// Option filtre par Annonceur, marque ou produit
		/// </summary>
		[Bindable(true),
		Description("Option choix d'annoceur,marque ou produit")]
		protected bool advertiserBrandProductOption = false;
		/// <summary></summary>
		public bool AdvertiserBrandProductOption{
			get{return advertiserBrandProductOption;}
			set{advertiserBrandProductOption=value;}
		}

		/// <summary>
		/// Autopostback Vrai par défaut
		/// </summary>
		[Bindable(true),
		Description("autoPostBack")]
		protected bool autoPostBackOption = false;
		/// <summary></summary>
		public bool AutoPostBackOption{
			get{return autoPostBackOption;}
			set{autoPostBackOption=value;}
		}
		/// <summary>
		/// True if show product level
		/// </summary>
		[Bindable(true),
		Description("autoPostBack")]
		protected bool _showProduct = false;
		/// <summary></summary>
		public bool ShowProduct {
			get { return _showProduct; }
			set { _showProduct = value; }
		}

		/// <summary>
		/// CssClass générale 
		/// </summary>
		[Bindable(true),DefaultValue("txtNoir11Bold"),
		Description("Style CSS")]
		protected string cssClass = "txtNoir11Bold";
		/// <summary></summary>
		public string CommonCssClass{
			get{return cssClass;}
			set{cssClass=value;}
		}
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DetailWebControl():base(){
			this.EnableViewState = true;
			this.PreRender += new EventHandler(Custom_PreRender);
		}	
		#endregion

		#region Evenements

		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e){
		}
		#endregion

		#region Custom PreRender

		/// <summary>
		///custom prerender 
		/// </summary>
		/// <param name="sender">object qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		private void Custom_PreRender(object sender, System.EventArgs e){

			#region régie ou centred'intêret ou Titre
			//Option détail Produit
			if(advertiserBrandProductOption){
				productDetail = new RadioButtonList();
				productDetail.Width = new System.Web.UI.WebControls.Unit("100%");
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1106, customerWebSession.SiteLanguage),"AdvertiserBrandProduct_"+SessionCst.PreformatedDetails.PreformatedProductDetails.advertiser.GetHashCode().ToString()));
				productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1889, customerWebSession.SiteLanguage),"AdvertiserBrandProduct_"+SessionCst.PreformatedDetails.PreformatedProductDetails.brand.GetHashCode().ToString()));
				if(_showProduct)productDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(858, customerWebSession.SiteLanguage),"AdvertiserBrandProduct_"+SessionCst.PreformatedDetails.PreformatedProductDetails.product.GetHashCode().ToString()));
				productDetail.ID = "productDetail_"+this.ID;
				productDetail.AutoPostBack=AutoPostBackOption;
				productDetail.CssClass=CommonCssClass;
				try{
					productDetail.Items.FindByValue("AdvertiserBrandProduct_"+customerWebSession.PreformatedProductDetail.GetHashCode().ToString()).Selected = true;
				}
				catch(System.Exception){
					productDetail.Items[0].Selected = true;
				}
				
				Controls.Add(productDetail);
			}
			#endregion
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" bgcolor=\"#FFFFFF\">");
			//Descriptif
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");
			output.Write("\n<td class=\"txtGris11Bold\">");
			output.Write(GestionWeb.GetWebWord(2082,customerWebSession.SiteLanguage));
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");

			//détail produit
			if (advertiserBrandProductOption){
				output.Write("\n<tr>");
				output.Write("\n<td class=\"txtGris11Bold\">");
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td>");
				productDetail.RenderControl(output);				
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}

			output.Write("\n</table>");
		}
		#endregion

		#endregion

	}
}
