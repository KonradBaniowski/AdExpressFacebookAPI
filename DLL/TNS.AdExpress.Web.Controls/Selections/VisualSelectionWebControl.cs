#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 28/09/2005 
// Date de modification: 28/09/2005 
#endregion

#region NameSpace
using System;
using System.IO;
using System.Data;
using System.Web.UI;
using System.Windows.Forms;
using System.Collections;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections;
using TNS.AdExpress.Web.DataAccess.Selections.Products;
using CstWeb = TNS.AdExpress.Constantes.Web;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CustomerCst=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Web.Controls.Results;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.DataAccess.Results.APPM;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Composant affichant la liste des visuels à sélectionner.
	/// </summary>
	[ToolboxData("<{0}:VisualSelectionWebControl runat=server></{0}:VisualSelectionWebControl>")]
	public class VisualSelectionWebControl : System.Web.UI.WebControls.WebControl
	{
		#region variables
		/// <summary>
		/// List des supports selectionnée
		/// </summary>
		public DropDownList drpMedias;

		/// <summary>
		/// List des supports selectionnée
		/// </summary>
		public RadioButtonList rdbListVisual;
	
		#endregion

		#region Propriétés

		/// <summary>
		/// Option supports
		/// </summary>	
		[Bindable(true),
		Description("Option liste des supports")]
		protected bool _mediasOption = false;
		/// <summary>liste supports actifs Appm</summary>
		public bool MediasOption {
			get{return _mediasOption;}
			set{_mediasOption=value;}
		}

		/// <summary>
		/// Option visuels
		/// </summary>	
		[Bindable(true),
		Description("Option liste des visuels")]
		protected bool _visualsOption = false;
		/// <summary>liste visuels actifs Appm</summary>
		public bool VisualsOption {
			get{return _visualsOption;}
			set{_visualsOption=value;}
		}

		/// <summary>
		/// Autopostback Vrai par défaut
		/// </summary>
		[Bindable(true),
		Description("autoPostBack")]
		protected bool _autoPostBackOption = true;
		/// <summary>Autopostback Vrai par défaut</summary>
		public bool AutoPostBackOption{
			get{return _autoPostBackOption;}
			set{_autoPostBackOption=value;}
		}
		
		/// <summary>
		/// Source de données
		/// </summary>
        protected TNS.FrameWork.DB.Common.IDataSource _dataSource = null;
		/// <summary>Source de données</summary>
        public TNS.FrameWork.DB.Common.IDataSource DataSource
        {
			get{return _dataSource;}
			set{_dataSource=value;}
		}

		/// <summary>
		/// Session du client 
		/// </summary>
		protected WebSession _customerWebSession = null;
		/// <summary> Session du client </summary>
		public WebSession CustomerWebSession{
			get{return _customerWebSession;}
			set{_customerWebSession=value;}
		}
		
		/// <summary>
		/// Cible supplémentaire
		/// </summary>
		protected long _idAdditionalTarget = 0;
		/// <summary>
		///Cible supplémentaire 
		/// </summary>
		public long IdAdditionalTarget{
			set{_idAdditionalTarget=value;}
			get{return _idAdditionalTarget;}
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

		#endregion

		#region Load
		/// <summary>
		/// Evènement lancé lors du chargement du control
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e) {
			#region initialisation des controls
			int mediaID =0;

			#region supports APPM
			drpMedias = new DropDownList();
			drpMedias.EnableViewState=true;
			drpMedias.ID = "_medias";
			Controls.Add(drpMedias);
			#endregion
		
			#region visuels APPM
			rdbListVisual = new RadioButtonList();
			rdbListVisual.EnableViewState=false;
			rdbListVisual.ID = "_visuals";
			Controls.Add(rdbListVisual);
			#endregion

			#endregion

			#region Premier chargement des controls

			
			//Création de la liste des supports appm
			if(_mediasOption) {
				if(!Page.IsPostBack || drpMedias.Items.Count<=0) {
					
					drpMedias.CssClass =  cssClass;
					drpMedias.AutoPostBack = _autoPostBackOption;
					DataTable mediasTable= VisualChoiceDataAccess.GetMediaListData(_customerWebSession,_dataSource,int.Parse(_customerWebSession.PeriodBeginningDate),int.Parse(_customerWebSession.PeriodEndDate),_idAdditionalTarget).Tables[0];


					if(mediasTable.Rows.Count>0) {	
						drpMedias.Items.Add(new ListItem("----------------------------------------","0"));
						foreach(DataRow dr in mediasTable.Rows) {
							drpMedias.Items.Add(new ListItem(dr["media"].ToString(),dr["id_media"].ToString()));
						}
					}
					
				}
					try{
						drpMedias.Items.FindByValue("0").Selected = true;				
					}
					catch(System.Exception){
					}
				
			}											
						
			//sauvegarde le support sélectionné dans dans l'univers media courant lorsque la page est publiée.
			if (Page.IsPostBack) {
			
				//supports
				if (_mediasOption){
					try{	
						
						mediaID=Convert.ToInt32(Page.Request.Form.GetValues("_medias")[0]);					
						//string product=products.SelectedItem.Text;
						_customerWebSession.CurrentUniversProduct.Nodes.Clear();
                        System.Windows.Forms.TreeNode tmp = new System.Windows.Forms.TreeNode("media");
						tmp.Tag = new LevelInformation(CustomerCst.type.mediaAccess,mediaID,"");
						tmp.Checked = true;
						_customerWebSession.CurrentUniversProduct = tmp;		
						
					}
					catch(SystemException){
					}
				}
				//visuels
				 if(drpMedias!=null && drpMedias.Items.Count>0)_visualsOption=true;
				if(_visualsOption){
					try{	
						rdbListVisual.CssClass =  cssClass;
						ListItem rdbItem;
						string[] fileList = null;
						string visual="";
						string pathCouv="";						
						int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType).ToString("yyyyMMdd"));
						int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(_customerWebSession.PeriodEndDate, _customerWebSession.PeriodType).ToString("yyyyMMdd"));
						DataTable visualsTable= VisualChoiceDataAccess.GetVisualListData(_dataSource,_customerWebSession,mediaID,dateBegin,dateEnd,_idAdditionalTarget).Tables[0];
						if(visualsTable.Rows.Count>0){
							
							foreach(DataRow dr in visualsTable.Rows) {
								//Affichage des visuels à sélectionner								
								if(dr["visual"].ToString().Length > 0){
									fileList = dr["visual"].ToString().Split(',');
								}
								
								if(fileList!=null){
									pathCouv = CstWeb.CreationServerPathes.IMAGES + "/" + dr["id_media"].ToString() + "/" + dr["date_cover_num"].ToString() + "/imagette/" + fileList.GetValue(0).ToString();
									visual="<img src='"+pathCouv+"' border=\"0\" width=\"100\" height=\"141\">";
									rdbItem = new ListItem(dr["visual"].ToString(), dr["visual"].ToString() + "-" + dr["id_media"].ToString() + "-" + dr["date_cover_num"].ToString());
									rdbItem.Text = visual;	
							
									rdbListVisual.Items.Add(rdbItem);
								
								}
								else{									
									pathCouv="/Images/"+_customerWebSession.DataLanguage+"/Others/no_visuel.gif";
									visual="<img src='"+pathCouv+"' border=\"0\" width=\"100\" height=\"141\">";
									visual="<img src='"+pathCouv+"' border=\"0\" width=\"100\" height=\"141\">";
									rdbItem = new ListItem(dr["visual"].ToString(), "no_visuel.gif-" + dr["id_media"].ToString() + "-" + dr["date_cover_num"].ToString());
									rdbItem.Text = visual;
							
									rdbListVisual.Items.Add(rdbItem);
								}
								
								fileList=null;
								pathCouv="";
							}
							rdbListVisual.RepeatDirection = RepeatDirection.Vertical;
							rdbListVisual.RepeatColumns = 3;
							rdbListVisual.CellPadding = 15;
							rdbListVisual.CellSpacing = 15;
							rdbListVisual.RepeatLayout = RepeatLayout.Table;														
							rdbListVisual.BorderColor = System.Drawing.Color.FromName("#644883");
							rdbListVisual.TextAlign = TextAlign.Left;																				
						}
												
						rdbListVisual.RepeatDirection = RepeatDirection.Vertical;				
						ArrayList tmp = new ArrayList();
						string key="";
						if(Page.Request.Form.GetValues("_visuals")!=null && !Page.Request.Form.GetValues("_visuals")[0].ToString().Equals("0")){
							key=Page.Request.Form.GetValues("_visuals")[0].ToString();	
							tmp.Add(key);											
							_customerWebSession.Visuals = tmp;
						}else _customerWebSession.Visuals = null;
						_customerWebSession.Save();

									
					}
					catch(SystemException){						
					}

				}
				
			}

			// Ouverture/fermeture des fenêtres pères
			if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
			}

			base.OnLoad (e);

			#endregion
		}


		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {	
			string display="none";
			if(Page.IsPostBack)display="";
            string themeName = WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;

            output.Write("\n<table class=\"txtViolet11Bold GenericMediaLevelDetailSelectionHeader\"  cellpadding=0 cellspacing=0 width=\"100%\">\n");
			output.Write("\n<tr style=\"cursor : hand\">");
			output.Write("\n<td align=\"left\" onClick=\"javascript : DivDisplayer('md_0');\" >");
			output.Write("&nbsp;"+GestionWeb.GetWebWord(1782,_customerWebSession.SiteLanguage));
			output.Write("</td>");
            output.Write("<td align=\"right\" onClick=\"javascript : DivDisplayer('md_0');\"><img src=\"../../App_Themes/" + themeName + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\">");
			output.Write("\n</td>");
			output.Write("\n</tr>");
			
			//Debut affichage visuels et supports
			output.Write("\n<tr>");
			output.Write("\n<td width=\"100%\" colspan=2 >");
			output.Write("\n<div  id=\"md_0\" style=\" display : "+display+"; \">");
            output.Write("\n<Table class=\"txtViolet11Bold paleVioletBackGroundV2\" cellpadding=5 cellspacing=0 width=\"100%\">\n");
		
			//option supports
			if (_mediasOption){
				output.Write("\n<tr>");
				output.Write("\n<td  class=\"txtRouge11\" colspan=2 >");
				output.Write(GestionWeb.GetWebWord(1087,_customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td colspan=2 >");
				drpMedias.RenderControl(output);
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\" colspan=2 ></TD>");
				output.Write("\n</TR>");
			}
			
			//option visuels
			if (_visualsOption){
				output.Write("\n<tr>");
				output.Write("\n<td  class=\"txtRouge11\"   colspan=2 >");
				output.Write(GestionWeb.GetWebWord(1781,_customerWebSession.SiteLanguage));
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<tr>");
				output.Write("\n<td colspan=2 >");
				if(rdbListVisual.Items.Count>0){
					rdbListVisual.RenderControl(output);
				}else{
					try{
						if(drpMedias!=null && drpMedias.Items!=null && drpMedias.Items.FindByValue("0").Selected != true) 
							output.Write(GestionWeb.GetWebWord(1783,_customerWebSession.SiteLanguage));
								
					}
					catch(SystemException){
					}
				}
				output.Write("\n</td>");
				output.Write("\n</tr>");
				output.Write("\n<TR>");
				output.Write("\n<TD height=\"5\" colspan=2 ></TD>");
				output.Write("\n</TR>");
			}
			//fin affichage visuels et supports
		
			output.Write("\n</table>\n");
			output.Write("\n</div>");
			output.Write("\n</td>");
			output.Write("\n</tr>");

			//fin tableau
			output.Write("\n</table>");
			
			
		}
		#endregion
	}
}
