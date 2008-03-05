#region Informations
// Auteur: A. Obermeyer
// Date de création: 30/09/2004
// Date de modification: 30/09/2004
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;

using System.ComponentModel;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using CoreSelection = TNS.AdExpress.Web.Core.Selection;
using TNS.Classification.WebControls;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Controls.Selections{
 
	/// <summary>
	/// Contrôle affichant la liste des univers pouvant 
	/// être chargée dans un module
	/// </summary>


	[DefaultProperty("Text"), 
	ToolboxData("<{0}:LoadableUniversWebControl runat=server></{0}:LoadableUniversWebControl>")]
	public class LoadableUniversWebControl: System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession webSession = null;
		/// <summary>
		/// Liste des branches de la nomenclature pouvant être enregistrer
		/// </summary>
		protected string listBranchType="";
		/// <summary>
		/// bouton RollOver
		/// </summary>
		protected TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl buttonRollOver; 
		/// <summary>
		/// Indicate if component is used for generics universe
		/// </summary>
		protected bool _forGenericUniverse = false;

		/// <summary>
		/// Allowed universe levels
		/// </summary>
		protected List<Int64> _allowedLevelsId = new List<long>();

		/// <summary>
		/// Universe dimension
		/// </summary>
		protected Dimension _dimension = Dimension.product;

		/// <summary>
		/// allowed branches identifiers
		/// </summary>
		protected List<int> _allowedBranchesIds = new List<int>();
		#endregion

		#region Accesseurs
		/// <summary>
		/// Session du client
		/// </summary>
		public WebSession CustomerWebSession{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// Indicate if component is used for generics universe
		/// </summary>
		public bool ForGenericUniverse {
			get { return _forGenericUniverse; }
			set { _forGenericUniverse = value; }
		}

		/// <summary>
		/// Session du client
		/// </summary>
		public string ListBranchType {
			get { return listBranchType; }
			set { listBranchType = value; }
		}

		/// <summary>
		/// Get /Set Universe dimension (product, media...)
		/// </summary>
		[Bindable(true), Category("Type"), Description("Universe dimension (product, media...)")]
		public Dimension Dimension_ {
			get {
				return _dimension;
			}
			set {
				_dimension = value;
			}
		}

		#endregion

		/// <summary>
		/// Page de sélection
		/// </summary>
		[Bindable(false),
		Description("Option choix de l'unité")]
		protected bool selectionPage = true;
		/// <summary>
		/// Page sélectionnée
		/// </summary>
		public bool SelectionPage{
			get{return selectionPage;}
			set{selectionPage=value;}
		}
			

		#region Evènements

		/// <summary>
		/// Rendu de la page
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			buttonRollOver=new TNS.AdExpress.Web.Controls.Buttons.ImageButtonRollOverWebControl();
			buttonRollOver.ImageUrl="/Images/"+webSession.SiteLanguage+"/button/charger_up.gif";
			buttonRollOver.RollOverImageUrl="/Images/"+webSession.SiteLanguage+"/button/charger_down.gif";
			buttonRollOver.ID="loadImageButtonRollOverWebControl";
		
		
		
			string imageLoadScriptKey = "rolloverImageBuildingCode" + buttonRollOver.ClientID;
			string script = 
				@"<script language=""JavaScript"">
					<!--
						" + buttonRollOver.ClientID + "_img_out = new Image(); " +
				 buttonRollOver.ClientID + @"_img_out.src = """ + buttonRollOver.ImageUrl + @""";
						" + buttonRollOver.ClientID + "_img_over = new Image(); " +
				 buttonRollOver.ClientID + @"_img_over.src = """ + buttonRollOver.RollOverImageUrl + @""";
					// -->
				  </script>";
			Page.ClientScript.RegisterClientScriptBlock(this.GetType(),imageLoadScriptKey, script);

			buttonRollOver.Attributes["onmouseover"] = "rolloverServerControl_display('" + buttonRollOver.ClientID + "_img'," + buttonRollOver.ClientID + "_img_over);";
			buttonRollOver.Attributes["onmouseout"] = "rolloverServerControl_display('" + buttonRollOver.ClientID + "_img'," + buttonRollOver.ClientID + "_img_out);";

			const string displayScriptKey = "rolloverImageLoadScript";
			if (!Page.ClientScript.IsClientScriptBlockRegistered(displayScriptKey)) {
				script = @"<script language=""JavaScript"">
				<!--
					function rolloverServerControl_display(imgName, imgUrl) {
						if (document.images && typeof imgUrl != 'undefined')
							document[imgName].src = imgUrl.src;
					}
				// -->
				</script>";

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),displayScriptKey, script);
			}	

			base.OnPreRender (e);
		
		}

		/// <summary>
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output">Le writer HTML vers lequel écrire</param>
		protected override void Render(HtmlTextWriter output){
		
		string listUniverses="";
		bool existUnivers=true;
		string listUniverseClientDescription="";
		CoreSelection.ILevelsRules levelsRules = null;
		List<int> tempBranchIds = null;
		List<UniverseLevel> tempLevels = null;

		Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
			if(selectionPage){
				foreach(SelectionPageInformation currentPage in currentModuleDescription.SelectionsPages){
					if (currentPage.Url.Equals(this.Page.Request.Url.AbsolutePath)) {
						listUniverseClientDescription += currentPage.LoadableUniversString;
						
						//Apply rigth Rules for getting levels and branches
						levelsRules = new CoreSelection.AdExpressLevelsRules(webSession, currentPage.AllowedBranchesIds, UniverseLevels.GetList(currentPage.AllowedLevelsIds), _dimension);
						tempBranchIds = levelsRules.GetAuthorizedBranches();
						tempLevels = levelsRules.GetAuthorizedLevels();
						if (tempBranchIds != null && tempBranchIds.Count > 0)
							_allowedBranchesIds = tempBranchIds;
						if (tempLevels != null && tempLevels.Count > 0) {
							for (int i = 0; i < tempLevels.Count; i++) {
								_allowedLevelsId.Add(tempLevels[i].ID);
							}
						}
					}
				
				}
			}else{
				foreach(OptionalPageInformation currentPage in currentModuleDescription.OptionalsPages){
					if (currentPage.Url.Equals(this.Page.Request.Url.AbsolutePath)) {
						listUniverseClientDescription += currentPage.LoadableUniversString;

						//Apply rigth Rules for getting levels and branches
						if (currentPage.AllowedLevelsIds != null && currentPage.AllowedLevelsIds.Count > 0) {
							tempLevels = UniverseLevels.GetList(currentPage.AllowedLevelsIds);
							if (tempLevels != null && tempLevels.Count > 0) {
								for (int i = 0; i < tempLevels.Count; i++) {
									_allowedLevelsId.Add(tempLevels[i].ID);
								}
							}

						}
						if (currentPage.AllowedBranchesIds != null && currentPage.AllowedBranchesIds.Count > 0)
							_allowedBranchesIds = currentPage.AllowedBranchesIds;
					}
				
				}
			}
			


		output.Write("<table style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" cellSpacing=\"0\" cellPadding=\"0\" width=\"200\" border=\"0\">");
		output.Write("<tr style=\"CURSOR: hand\" onclick=\"showHideContent6('listAdvertiser');\">");
		output.Write("<td class=\"txtViolet11Bold\" >&nbsp;"+GestionWeb.GetWebWord(893,webSession.SiteLanguage)+"&nbsp;</td>");
		output.Write("<td align=\"right\"><IMG  src=\"/Images/Common/button/bt_arrow_down.gif\" align=\"absMiddle\">");
		output.Write("</td>");
		output.Write("</tr>");
		output.Write("</table>");



		output.Write("<div id=\"listAdvertiserContent6\" style=\"BORDER-RIGHT: #644883 1px solid; DISPLAY: none; BORDER-LEFT: #644883 1px solid; WIDTH: 620px; BORDER-BOTTOM: #644883 1px solid\">");
		output.Write("<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" align=\"center\" bgColor=\"#ffffff\" border=\"0\">");
		output.Write("<tr>");
		output.Write("<td width=\"199\"><IMG height=\"1\" src=\"/images/Common/pixel.gif\"></td>");
		output.Write("<td style=\"BORDER-TOP: #644883 1px solid\" width=\"421\"><IMG height=\"1\" src=\"/images/Common/pixel.gif\"></td>");
		output.Write("</tr>");
		output.Write("<tr>");
		output.Write("<td class=\"txtGris11Bold\" style=\"PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 2px; PADDING-TOP: 0px\" colSpan=\"2\">&nbsp;</td>");
		output.Write("</tr>");
		output.Write("<tr>");
		output.Write("<td style=\"PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 2px; PADDING-TOP: 0px\" colSpan=\"2\">");
		//<!--Tableau des univers-->
		//<%=listUniverses%>

			TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI listUnivers=  new TNS.AdExpress.Web.UI.MyAdExpress.MySessionsUI(webSession,listBranchType,600);
			if (_forGenericUniverse) {
				listUniverseClientDescription = TNS.AdExpress.Constantes.Web.LoadableUnivers.GENERIC_UNIVERSE.ToString();
				listUniverses = listUnivers.GetSelectionTableHtmlUI(4, listUniverseClientDescription, _allowedLevelsId);
			}else
			listUniverses = listUnivers.GetSelectionTableHtmlUI(4, listUniverseClientDescription);
			if(listUniverses.Length==0) {
				existUnivers=false;
				listUniverses="<tr><td class=\"txtViolet12Bold\" colspan=3 align=middle>";
				listUniverses+="&nbsp&nbsp&nbsp"+GestionWeb.GetWebWord(930,webSession.SiteLanguage);
				listUniverses+="</td></tr>";
			}

		output.Write(listUniverses);
		output.Write("<input id=\"idMySession\" type=\"hidden\" name=\"nameMySession\">");
		output.Write("</td>");
		output.Write("</tr>");
		output.Write("<tr>");
		
		if(existUnivers)output.Write("<td style=\"PADDING-RIGHT: 10px; PADDING-LEFT: 10px; PADDING-BOTTOM: 5px; PADDING-TOP: 2px; TEXT-ALIGN: right\" colSpan=\"2\"><a id=\"loadImageButtonRollOverWebControl\" onmouseover=\"rolloverServerControl_display('loadImageButtonRollOverWebControl_img',loadImageButtonRollOverWebControl_img_over);\" onmouseout=\"rolloverServerControl_display('loadImageButtonRollOverWebControl_img',loadImageButtonRollOverWebControl_img_out);\" href=\"javascript:__doPostBack('loadImageButtonRollOverWebControl','')\"><img name=\"loadImageButtonRollOverWebControl_img\" src=\"/Images/"+webSession.SiteLanguage+"/button/charger_up.gif\" border=\"0\" /></a>");
			
		//	<cc2:imagebuttonrolloverwebcontrol id=\"loadImageButtonRollOverWebControl\" runat=\"server\"></cc2:imagebuttonrolloverwebcontrol>
		output.Write("</td>");
		output.Write("</tr>");
		output.Write("</table>");
		output.Write("</div>");

		
		}
		#endregion


	}
}
