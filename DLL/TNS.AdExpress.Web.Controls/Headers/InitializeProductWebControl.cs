#region Informations
// Auteur : A.Obermeyer
// Date de création : 29/12/2004
#endregion

using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Windows.Forms;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Headers{

	/// <summary>
	/// Contrôle pour l'initialisation des produits de références
	/// </summary>
	[ToolboxData("<{0}:InitializeProductWebControl runat=server></{0}:InitializeProductWebControl>")]
	public class InitializeProductWebControl: System.Web.UI.WebControls.WebControl {

		#region Variables
		/// <summary>
		/// CheckBox pour l'initialisation des produits de références
		/// </summary>
		public System.Web.UI.WebControls.CheckBox initializeProductCheckBox=new System.Web.UI.WebControls.CheckBox();
		/// <summary>
		/// CheckBox pour l'initialisation des annonceurs de références et concurrents
		/// </summary>
		public System.Web.UI.WebControls.CheckBox initializeAdvertiserCheckBox=new System.Web.UI.WebControls.CheckBox();

		/// <summary>
		/// Checkbox dédiée à l'annulation des versions affinées
		/// </summary>
        public System.Web.UI.WebControls.CheckBox initializeSlogansUniverseCheckBox;

        /// <summary>
        /// CheckBox Initialization of AdvertisementType
        /// </summary>
        public System.Web.UI.WebControls.CheckBox initializeAdvertisementTypeCheckBox = new System.Web.UI.WebControls.CheckBox();

		#endregion

		#region Propriétés
		/// <summary>
		/// Initialisation des produits de référence
		/// </summary>
		[Bindable(true),DefaultValue(false),
		Description("Option initialisation des produits de références")]
		protected bool initializeProduct = false;
		/// <summary></summary>
		public bool InitializeProduct{
			get{return initializeProduct;}
			set{initializeProduct=value;}
		}

		/// <summary>
		/// Initialisation des annonceurs de références et concurrents
		/// </summary>
		[Bindable(true),DefaultValue(false),
		Description("Option initialisation des annonceurs de références et concurrents")]
		protected bool initializeAdvertiser = false;
		/// <summary></summary>
		public bool InitializeAdvertiser{
			get{return initializeAdvertiser;}
			set{initializeAdvertiser=value;}
		}
		
		/// <summary>
		/// Option affiner univers versions
		/// </summary>
		[Bindable(true),
		Description("Option :Option affiner univers versions")]
		protected bool initializeSlogans = false;
		/// <summary></summary>
		public bool InitializeSlogans{
			get{return initializeSlogans;}
			set{initializeSlogans=value;}
		}

		/// <summary>
		/// Session du client (utile pour la langue)
		/// </summary>
		protected WebSession customerWebSession = null;
		/// <summary></summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}

		/// <summary>
		/// CssClass générale 
		/// </summary>
		[Bindable(true),DefaultValue("txtNoir11Bold"),
		Description("Option choix de l'unité")]
		protected string cssClass = "txtNoir11Bold";
		/// <summary></summary>
		public string CommonCssClass{
			get{return cssClass;}
			set{cssClass=value;}
		}

		/// <summary>
		/// Autopostback Vrai par défaut
		/// </summary>
		[Bindable(true),
		Description("autoPostBack")]
		protected bool autoPostBackOption = true;
		/// <summary></summary>
		public bool AutoPostBackOption{
			get{return autoPostBackOption;}
			set{autoPostBackOption=value;}
		}

        /// <summary>
        /// Initialization of initializeAdvertisementType
        /// </summary>
        [Bindable(true), DefaultValue(false),
        Description("Option Initialization of initializeAdvertisementType")]
        protected bool _initializeAdvertisementType = false;
        /// <summary></summary>
        public bool InitializeAdvertisementType
        {
            get { return _initializeAdvertisementType; }
            set { _initializeAdvertisementType = value; }
        }
		#endregion

		#region Constructeur
		/// <summary>
		/// Contructeur
		/// </summary>
		public InitializeProductWebControl():base(){			
			this.EnableViewState = true;
			this.PreRender += new EventHandler(Custom_PreRender);
		}
		#endregion

	
		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">argument</param>
		protected override void OnInit(EventArgs e) {
            bool canSaveSession = false;
			if(initializeProduct){
				try{
					if(Page.Request.Form.GetValues("_initializeProduct")[0]!=null){
						customerWebSession.PrincipalProductUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
						customerWebSession.SecondaryProductUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
						
						customerWebSession.CurrentUniversProduct= new System.Windows.Forms.TreeNode("produit");
						customerWebSession.CurrentUniversAdvertiser= new System.Windows.Forms.TreeNode("produit");
						customerWebSession.SelectionUniversProduct=new System.Windows.Forms.TreeNode("produit");
						customerWebSession.SelectionUniversAdvertiser=new System.Windows.Forms.TreeNode("produit");
						customerWebSession.ReferenceUniversProduct=new System.Windows.Forms.TreeNode("produit");
						customerWebSession.ReferenceUniversAdvertiser=new System.Windows.Forms.TreeNode("produit");						
                        canSaveSession = true;
					}
				}catch(Exception){			
				}				
			}

			if(initializeAdvertiser){
				try{
					if(Page.Request.Form.GetValues("_initializeAdvertiser")[0]!=null){
						customerWebSession.SecondaryProductUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

						customerWebSession.ReferenceUniversAdvertiser=new System.Windows.Forms.TreeNode("advertiser");
						customerWebSession.CompetitorUniversAdvertiser=new Hashtable(5);						
                        canSaveSession = true;
					}
				}catch(Exception){			
				}				
			}

            if (_initializeAdvertisementType)
            {
                try
                {
                    if (Page.Request.Form.GetValues("_initializeAdvertisementType")[0] != null)
                    {
                        customerWebSession.AdvertisementTypeUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();                        
                        canSaveSession = true;
                    }
                }
                catch (Exception)
                {
                }
            }
            if (canSaveSession) customerWebSession.Save();
			base.OnInit (e);
		}

		#endregion

		#region Load
		/// <summary>
		/// launched when the control is loaded
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e){
			#region annuler sélection accroche
			if(initializeSlogans){
				initializeSlogansUniverseCheckBox=new System.Web.UI.WebControls.CheckBox();
				initializeSlogansUniverseCheckBox.ID=this.ID + "_initializeSlogans";
				initializeSlogansUniverseCheckBox.EnableViewState=true;
				initializeSlogansUniverseCheckBox.ToolTip = GestionWeb.GetWebWord(1946,customerWebSession.SiteLanguage);
				initializeSlogansUniverseCheckBox.CssClass=cssClass;
				initializeSlogansUniverseCheckBox.AutoPostBack=autoPostBackOption;
				initializeSlogansUniverseCheckBox.Width = new System.Web.UI.WebControls.Unit("100%");
				initializeSlogansUniverseCheckBox.Text=GestionWeb.GetWebWord(1946,customerWebSession.SiteLanguage);	
							
				Controls.Add(initializeSlogansUniverseCheckBox);


                if (Page.IsPostBack && Page.Request.Form.GetValues(this.ID + "_initializeSlogans") != null
                    && Page.Request.Form.GetValues(this.ID + "_initializeSlogans")[0] != null)
                {
                    customerWebSession.IdSlogans = new ArrayList();
                    customerWebSession.Save();
                }
                
				if(customerWebSession.IdSlogans!=null && customerWebSession.IdSlogans.Count>0){
					initializeSlogansUniverseCheckBox.Enabled=true;
				}else{
					initializeSlogansUniverseCheckBox.Enabled=false;					
				}
			
			}
			#endregion

			base.OnLoad (e);

		}
		#endregion

		#region Custom PreRender
		/// <summary>
		/// Custom PreRender
		/// </summary>
		/// <param name="sender">sender</param>
		/// <param name="e">argument</param>
		private void Custom_PreRender(object sender, System.EventArgs e){
			// Partie Produits		
			if(initializeProduct){
				//initializeProductCheckBox=new CheckBox();
				initializeProductCheckBox.ID="_initializeProduct";
				initializeProductCheckBox.CssClass=cssClass;
				initializeProductCheckBox.AutoPostBack=autoPostBackOption;
				initializeProductCheckBox.Text=GestionWeb.GetWebWord(1131,customerWebSession.SiteLanguage);
				//if(customerWebSession.CurrentUniversProduct.Nodes.Count>0){
				if (customerWebSession.PrincipalProductUniverses.Count > 0) {
					initializeProductCheckBox.Enabled=true;
				}else{
					initializeProductCheckBox.Enabled=false;
				}
				Controls.Add(initializeProductCheckBox);
			
			}

			// Partie Annonceurs		
			if(InitializeAdvertiser){
				//initializeProductCheckBox=new CheckBox();
				initializeAdvertiserCheckBox.ID="_initializeAdvertiser";
				initializeAdvertiserCheckBox.CssClass=cssClass;
				initializeAdvertiserCheckBox.AutoPostBack=autoPostBackOption;
				initializeAdvertiserCheckBox.Text=GestionWeb.GetWebWord(1455,customerWebSession.SiteLanguage);
				if(customerWebSession.isReferenceAdvertisersSelected() || customerWebSession.isCompetitorAdvertiserSelected()){
					initializeAdvertiserCheckBox.Enabled=true;
				}else{
					initializeAdvertiserCheckBox.Enabled=false;
				}
				Controls.Add(initializeAdvertiserCheckBox);
			
			}

            // Advertisement Type	
            if (_initializeAdvertisementType)
            {
                initializeAdvertisementTypeCheckBox.ID = "_initializeAdvertisementType";
                initializeAdvertisementTypeCheckBox.CssClass = cssClass;
                initializeAdvertisementTypeCheckBox.AutoPostBack = autoPostBackOption;
                initializeAdvertisementTypeCheckBox.Text = GestionWeb.GetWebWord(2674, customerWebSession.SiteLanguage);
                if (customerWebSession.AdvertisementTypeUniverses != null && customerWebSession.AdvertisementTypeUniverses.Count > 0)
                {
                    initializeAdvertisementTypeCheckBox.Enabled = true;
                }
                else
                {
                    initializeAdvertisementTypeCheckBox.Enabled = false;
                }
                Controls.Add(initializeAdvertisementTypeCheckBox);

            }
		
		
		}

		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
		
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			// Initialisation des produits de références
			if(initializeProduct){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				initializeProductCheckBox.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");			
			}

			if(InitializeAdvertiser){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				initializeAdvertiserCheckBox.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");			
			}
            // Advertisement Type	
            if (_initializeAdvertisementType)
            {
                output.Write("\n<tr>");
                output.Write("\n<td>");
                initializeAdvertisementTypeCheckBox.RenderControl(output);
                output.Write("\n</td>");
                output.Write("\n</tr>");
                output.Write("\n<TR>");
                output.Write("\n<TD height=\"5\"></TD>");
                output.Write("\n</TR>");
            }
			//options de affiner versions
			if (initializeSlogans){
				output.Write("\n<tr>");
				output.Write("\n<td>");					
				initializeSlogansUniverseCheckBox.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\"></TD>");
				output.Write("\n</TR>");
			}

			output.Write("\n</table>");
		}
		#endregion
	}
}
