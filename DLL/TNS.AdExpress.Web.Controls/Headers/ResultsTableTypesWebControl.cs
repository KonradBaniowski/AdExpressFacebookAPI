#region Information
/*
 * Author : D. Mussuma
 * Creation : 4/12/2006
 * Modifications:
 * */
#endregion
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Controls.Results;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	///  Composant Choix du type de tableau
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:ResultsTableTypesWebControl runat=server></{0}:ResultsTableTypesWebControl>")]
	public class ResultsTableTypesWebControl : System.Web.UI.WebControls.WebControl
	{
		#region Variables
		/// <summary>
		/// Choix du type de tableau afficher
		/// </summary>
		protected ImageDropDownListWebControl tblChoice;

		/// <summary>
		/// Code de hache du premier Index de parrainage
		/// </summary>
		int _sponsorshipListIndex = 24;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Session du client 
		/// </summary>
		protected WebSession customerWebSession = null;
		/// <summary>Session du client</summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}
				

		#region Propriétés de TblChoice
		/// <summary>
		/// hauteur de l'image
		/// </summary>
		[Bindable(true),Category("Appearance")]
		private double imageHeight = 15.0 ;
		/// <summary></summary>
		public double ImageHeight{
			get{return imageHeight;}
			set{imageHeight = value;}
		}

		/// <summary>
		/// largeur de l'image
		/// </summary>
		[Bindable(true),Category("Appearance")]
		private double imageWidth = 15.0 ;
		/// <summary></summary>
		public double ImageWidth{
			get{return imageWidth;}
			set{imageWidth = value;}
		}

		/// <summary>
		/// largeur des bordures
		/// </summary>
		[Bindable(true),Category("Appearance"), DefaultValue(1.0)]
		private  double borderWidth=1.0;
		/// <summary></summary>
		public new double BorderWidth{
			get{return borderWidth;}
			set{borderWidth = Math.Max(0, value);}
		}

		/// <summary>
		/// option afficher images
		/// </summary>
		[Bindable(true),DefaultValue(true)]
		protected bool pictShow=true;
		/// <summary></summary>
		public bool ShowPictures{
			get{return pictShow;}
			set{pictShow = value;}
		}
		/// <summary>
		///feuille de style 
		/// </summary>
		[Bindable(true),DefaultValue("ddlOut")]
		protected string outCssClass="ddlOut";
		/// <summary></summary>
		public string OutCssClass{
			get{return outCssClass;}
			set{outCssClass = value;}
		}

		/// <summary>
		///feuille de style pour l'élément en roll-over 
		/// </summary>
		[Bindable(true),DefaultValue("ddlOver")]
		protected string overCssClass="ddlOver";
		/// <summary></summary>
		public string OverCssClass{
			get{return overCssClass;}
			set{overCssClass = value;}
		}

		/// <summary>
		/// liste des éléments
		/// </summary>
		[Bindable(true),DefaultValue("")]
		private string texts = "";
		/// <summary></summary>
		public string List{
			get{return texts;}
			set{texts = value;}
		}
	


		/// <summary>
		///noms des images 
		/// </summary>
		[Bindable(true),DefaultValue("")]
		private string images = "";
		/// <summary></summary>
		public string Images{
			get{return images;}
			set{images = value;}
		}

		/// <summary>
		/// index liste
		/// </summary>
		[Bindable(true),DefaultValue(0)]
		private int index;
		/// <summary></summary>
		public int ListIndex{
			get{return index;}
			set{index = Math.Min(value, texts.Split('|').Length - 1);}
		}

        /// <summary>
        /// Images button Arrow
        /// </summary>
        [Bindable(true), DefaultValue("")]
        private string _imageButtonArrow;
        /// <summary></summary>
        public string ImageButtonArrow {
            get { return _imageButtonArrow; }
            set { _imageButtonArrow = value; }
        }

		#endregion

		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ResultsTableTypesWebControl():base(){
			this.EnableViewState = true;
			this.PreRender += new EventHandler(Custom_PreRender);
		}

		#endregion

		#region Init
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnInit(EventArgs e) {
			
			if (Page.IsPostBack){
				Int64 formatTable = _sponsorshipListIndex + Int64.Parse(Page.Request.Form.GetValues("DDL"+this.ID)[0]);
				customerWebSession.PreformatedTable = (CustomerSessions.PreformatedDetails.PreformatedTables) formatTable;
			}
		}
		#endregion

		#region Custom PreRender

		/// <summary>
		///custom prerender 
		/// </summary>
		/// <param name="sender">object qui lance l'évènement</param>
		/// <param name="e">arguments</param>
		private void Custom_PreRender(object sender, System.EventArgs e) {

		        string themeName =  WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;

				tblChoice = new ImageDropDownListWebControl();
				tblChoice.BackColor = this.BackColor;
				tblChoice.BorderColor = this.BorderColor;
				tblChoice.BorderWidth = new System.Web.UI.WebControls.Unit(this.borderWidth);
                tblChoice.ImageButtonArrow = this._imageButtonArrow;

				if (this.List!="") tblChoice.List = this.List;
				else{					
						tblChoice.List = "&nbsp;|&nbsp;|&nbsp;";
				
				}
				if (this.images!="") tblChoice.Images = this.images;
				else{
                    tblChoice.Images = "/App_Themes/" + themeName + "/Images/Culture/Tables/Parrainage_type1.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/Parrainage_type2.gif" +
                            "|/App_Themes/" + themeName + "/Images/Culture/Tables/Parrainage_type3.gif";							
					
				}
				if(customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
					|| customerWebSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES) 
					tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode()- _sponsorshipListIndex;
				else tblChoice.ListIndex = customerWebSession.PreformatedTable.GetHashCode();
				tblChoice.ImageHeight = this.imageHeight;
				tblChoice.ImageWidth = this.imageWidth;
				tblChoice.ID = "DDL"+this.ID;
				tblChoice.OutCssClass = this.outCssClass;
				tblChoice.OverCssClass = this.overCssClass;
				tblChoice.Width = new System.Web.UI.WebControls.Unit("121");
				tblChoice.ShowPictures = this.pictShow;
				Controls.Add(tblChoice);
							
		
		}
		#endregion


		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			//Debut du tableau
            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"whiteBackGround\">");

			//format de tableau
			
			output.Write("\n<tr>");
			output.Write("\n<td class=\"txtGris11Bold\">");
			output.Write(GestionWeb.GetWebWord(1140,customerWebSession.SiteLanguage));
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td>");
			tblChoice.RenderControl(output);
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			
			//fin tableau
			output.Write("\n</table>");
			
		}
		#endregion
	}
}
