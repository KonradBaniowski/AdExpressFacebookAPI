#region Informations
// Auteur:
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
//		03/04/2006 D. Mussuma Sauvegarde d'univers de type pan euro

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Text;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.Classification.Universe;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace AdExpress.Private.Universe{

	/// <summary>
	/// Sauvegarde d'un univers
	/// </summary>
	public partial class SaveUniverse : TNS.AdExpress.Web.UI.PrivateWebPage{

		#region Variables MMI
		/// <summary>
		/// Enregistrer l'univers
		/// </summary>
		/// <summary>
		/// Sélectionner un groupe d'univers
		/// </summary>
		/// <summary>
		/// Enregistrer votre univers
		/// </summary>
		/// <summary>
		/// Liste des groupes d'univers
		/// </summary>
		/// <summary>
		/// Nom de l'univers
		/// </summary>
		/// <summary>
		/// Bouton valider
		/// </summary>
		#endregion

		#region Variables
		/// <summary>
		/// Identifiant permettant de connaitrele niveau de la nomenclature sauvegardé
		/// dans la nomenclature
		/// </summary>
		protected  Int64 idUniverseClientDescription = 0;

		string stringBranch = String.Empty;
		#endregion 

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public SaveUniverse():base(){
		}
		#endregion

		#region Evènements
		
		#region Chargement de la page
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e) {
			
			try{
				idUniverseClientDescription = Int64.Parse(Page.Request.QueryString.Get("idUniverseClientDescription"));
				stringBranch = Page.Request.QueryString.Get("brancheType");

				#region Langage
				//Modification de la langue pour les Textes AdExpress
                //for (int i = 0; i < this.Controls.Count; i++) {
                //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
                //}
				//okButton.ImageUrl="/Images/"+_siteLanguage+"/button/valider_up.gif";
				//okButton.RollOverImageUrl="/Images/"+_siteLanguage+"/button/valider_down.gif";
				#endregion

				#region Création de la liste des groupes d'univers disponibles
				
				//Liste des groupes d'univers
				DataSet ds = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetGroupUniverses(_webSession);

				directoryDropDownList.DataSource = ds.Tables[0];
				directoryDropDownList.DataTextField = ds.Tables[0].Columns[1].ToString();
				directoryDropDownList.DataValueField = ds.Tables[0].Columns[0].ToString();
				directoryDropDownList.DataBind();

				//Liste des univers sauvegardés
				DataSet dsUniverses = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetData(_webSession, string.Empty, string.Empty);
			
				#endregion

				directoryDropDownList.EnableViewState = false;
				universeDropDownList.EnableViewState = false;


				if (!this.Page.ClientScript.IsClientScriptBlockRegistered("UniverseJavaScriptFunctions"))
					this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "UniverseJavaScriptFunctions", UniverseJavaScriptFunctions(dsUniverses, ds));

				directoryDropDownList.Attributes.Add("onChange", "fillUniverses(this.options[this.selectedIndex].value);");
				universeDropDownList.Attributes.Add("onChange", "selectUniverse();");
				universeTextBox.Attributes.Add("onKeypress", "deleteSelectedUniverse();");
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
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion	

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
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
		private void InitializeComponent(){    
			this.Unload += new System.EventHandler(this.Page_UnLoad);
	
		}
		#endregion

		#region Bouton OK
		/// <summary>
		/// Gestion du bouton OK
		/// </summary>
		/// <param name="sender">Objet qui envoie l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void okButton_Click(object sender, System.EventArgs e) {
			ArrayList alTreeNodeUniverse = new ArrayList();
			try{	
				
				if (directoryDropDownList.Items.Count!=0){
				
					string idSelectedUniverse = Request.Form.GetValues("universeDropDownList")[0];
					string idSelectedDirectory = Request.Form.GetValues("directoryDropDownList")[0];
					Module currentModuleDescription = ModulesList.GetModule(_webSession.CurrentModule);

					//Identification de la branche de l'univers					
					TNS.AdExpress.Constantes.Classification.Branch.type branchType =  GetBrancheType(stringBranch);
										
					#region Sauvegarde de l'univers
					string universeName = CheckedText.CheckedAccentText(this.universeTextBox.Text);
					
					if (universeName.Length == 0 && !idSelectedUniverse.Equals("0")) {
						SetTreeNodeUniverse(branchType, alTreeNodeUniverse);
						if ((branchType!= TNS.AdExpress.Constantes.Classification.Branch.type.product && UniversListDataAccess.UpdateUniverse(Int64.Parse(idSelectedUniverse), _webSession, idUniverseClientDescription, branchType.GetHashCode(), alTreeNodeUniverse))
							|| ((WebFunctions.Modules.IsDashBoardModule(_webSession) || WebFunctions.Modules.IsRecapModule(_webSession)) && _webSession.PrincipalProductUniverses.Count > 0 && UniversListDataAccess.UpdateUniverse(Int64.Parse(idSelectedUniverse), _webSession, idUniverseClientDescription, branchType.GetHashCode(), _webSession.PrincipalProductUniverses))
							) {

						    // Validation : confirmation d'enregistrement de la requête
						    _webSession.Source.Close();
						    Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(921, _webSession.SiteLanguage)));							
						}
						else {
						    // Erreur : Echec de l'enregistrement de la requête	
						    _webSession.Source.Close();
						    Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(922, _webSession.SiteLanguage)));
						}
					}
					else if (universeName.Length!=0 && universeName.Length<TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT){				
						if (!UniversListDataAccess.IsUniverseExist(_webSession, universeName)) {
							
							SetTreeNodeUniverse(branchType, alTreeNodeUniverse);

							if ((branchType != TNS.AdExpress.Constantes.Classification.Branch.type.product && idSelectedDirectory != null && idSelectedDirectory.Length > 0 && UniversListDataAccess.SaveUniverse(Int64.Parse(idSelectedDirectory), universeName, alTreeNodeUniverse, branchType, idUniverseClientDescription, _webSession))
								|| ((WebFunctions.Modules.IsDashBoardModule(_webSession) || WebFunctions.Modules.IsRecapModule(_webSession)) && idSelectedDirectory != null && idSelectedDirectory.Length > 0 && _webSession.PrincipalProductUniverses.Count > 0 && UniversListDataAccess.SaveUniverse(Int64.Parse(idSelectedDirectory), universeName, _webSession.PrincipalProductUniverses, branchType, idUniverseClientDescription, _webSession))
								){
								// Validation : confirmation d'enregistrement de l'univers
								_webSession.Source.Close();
								Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(921, _webSession.SiteLanguage)));							
							}
							else{
								// Erreur : Echec de l'enregistrement de l'univers
								_webSession.Source.Close();
								Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(922, _webSession.SiteLanguage)));							
							}
						}
						else{
							// Erreur : univers déjà existant
							_webSession.Source.Close();
							Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(923, _webSession.SiteLanguage)));														
						}
					}
					else if(universeName.Length==0){
						// Erreur : Le champs est vide
						_webSession.Source.Close();
						Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(837, _webSession.SiteLanguage)));																				
					}
					else{
						// Erreur : suppérieur à 50 caractères
						_webSession.Source.Close();
						Response.Write(WebFunctions.Script.Alert(GestionWeb.GetWebWord(823, _webSession.SiteLanguage)));																										
					}
				}
					#endregion

				else{					
					// Erreur : Impossible de sauvegarder, pas de groupe d'univers créé
					Response.Write(WebFunctions.Script.AlertWithWindowClose(GestionWeb.GetWebWord(925, _webSession.SiteLanguage)));												
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#endregion

		#region Javascript
		/// <summary>
		/// Génére les javascripts utilisés pour la sauvegarde des univers
		/// </summary>
		/// <param name="ds">Liste des univers sauvegardés</param>
		/// <param name="dsDir">Liste des répertoires</param>
		/// <returns>Code Javascript</returns>
		private string UniverseJavaScriptFunctions(DataSet ds, DataSet dsDir) {


			StringBuilder script = new StringBuilder(2000);
			int i = 0, k = 0;

			script.Append("<script language=\"JavaScript\">");
			script.Append("\r\n  var directories = new Array(); ");
			universeDropDownList.Items.Clear();
			universeDropDownList.Items.Insert(k, new ListItem("------------------", "0"));
			k++;
			foreach (DataRow dr in ds.Tables[0].Rows) {

				if (dr["ID_UNIVERSE_CLIENT"] != System.DBNull.Value) {
					script.Append("\r\n directories[" + i + "] = new Array();");
					script.Append("\r\n directories[" + i + "][\"ID_GROUP_UNIVERSE_CLIENT\"] = \"" + dr["ID_GROUP_UNIVERSE_CLIENT"].ToString() + "\"; ");
					script.Append("\r\n directories[" + i + "][\"ID_UNIVERSE_CLIENT\"] = \"" + dr["ID_UNIVERSE_CLIENT"].ToString() + "\"; ");
					script.Append("\r\n directories[" + i + "][\"UNIVERSE_CLIENT\"] = \"" + dr["UNIVERSE_CLIENT"].ToString() + "\"; ");
					i++;
				}
				if (dsDir.Tables[0].Rows.Count > 0 && Int64.Parse(dsDir.Tables[0].Rows[0]["ID_GROUP_UNIVERSE_CLIENT"].ToString()) == Int64.Parse(dr["ID_GROUP_UNIVERSE_CLIENT"].ToString())
					&& dr["ID_UNIVERSE_CLIENT"] != System.DBNull.Value) {
					universeDropDownList.Items.Insert(k, new ListItem(dr["UNIVERSE_CLIENT"].ToString(), dr["ID_UNIVERSE_CLIENT"].ToString()));
					k++;
				}

			}

			script.Append("\r\n function verif()");
			script.Append("\r\n {");
			script.Append("\r\n\t if (document.layers)");
			script.Append("\r\n\t {");
			script.Append("\r\n\t theForm = document.forms.Form1;");
			script.Append("\r\n\t }");
			script.Append("\r\n\t else");
			script.Append("\r\n\t {");
			script.Append("\r\n\t theForm = document.Form1;");
			script.Append("\r\n\t }");
			script.Append("\r\n}");

			script.Append("\r\n function selectUniverse()");
			script.Append("\r\n {");
			script.Append("\r\n\t theForm.universeTextBox.value=\"\";");
			script.Append("\r\n }");

			script.Append("\r\n function deleteSelectedUniverse()");
			script.Append("\r\n {");
			script.Append("\r\n\t theForm.universeDropDownList.options.selectedIndex = 0;");
			script.Append("\r\n }");

			script.Append("\r\n function fillUniverses(codeDirectory)");
			script.Append("\r\n {");
			script.Append("\r\n\t verif();");
			script.Append("\r\n\t if(codeDirectory>0)");
			script.Append("\r\n\t {");
			script.Append("\r\n\t\t theForm.universeDropDownList.options.length = 0;");
			script.Append("\r\n\t\t\t\t theForm.universeDropDownList.options[theForm.universeDropDownList.options.length] = new Option(\"------------------\",\"0\");");
			script.Append("\r\n\t\t for (var i=0;i<directories.length;i++)");
			script.Append("\r\n\t\t {");
			script.Append("\r\n\t\t\t if(i==0)");
			script.Append("\r\n\t\t\t {");
			script.Append("\r\n\t\t\t }");
			script.Append("\r\n\t\t\t if(parseInt(directories[i][\"ID_GROUP_UNIVERSE_CLIENT\"]) == parseInt(codeDirectory))");
			script.Append("\r\n\t\t\t {");
			script.Append("\r\n\t\t\t\t theForm.universeDropDownList.options[theForm.universeDropDownList.options.length] = new Option(directories[i][\"UNIVERSE_CLIENT\"],directories[i][\"ID_UNIVERSE_CLIENT\"]);");
			script.Append("\r\n\t\t\t }");
			script.Append("\r\n\t\t }");
			script.Append("\r\n\t\t theForm.universeDropDownList.options.selectedIndex = 0;");
			script.Append("\r\n\t }");
			script.Append("\r\n\t else");
			script.Append("\r\n\t {");
			script.Append("\r\n\t\t theForm.universeDropDownList.options[theForm.universeDropDownList.options.length] = new Option(\"------------------\",\"0\");");
			script.Append("\r\n\t }");

			script.Append("\r\n }");
			script.Append("\r\n </script>");

			return (script.ToString());
		}
		#endregion

		/// <summary>
		/// Obtient la branche de l'univers
		/// </summary>
		/// <param name="stringBranch">chaine de caractère representant une branche</param>
		/// <returns>branche de l'univers</returns>
		private TNS.AdExpress.Constantes.Classification.Branch.type GetBrancheType(string stringBranch) {

			if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.media.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.media;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.brand.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.brand;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.advertiser.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.advertiser;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.product.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.product;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv;
			}
            else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaAdnettrack.ToString()) {
                return TNS.AdExpress.Constantes.Classification.Branch.type.mediaAdnettrack;
            }
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaTvSponsorship.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaTvSponsorship;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaOutdoor.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaOutdoor;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaOthers.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaOthers;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternationalPress.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternationalPress;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.programType.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.programType;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.sponsorshipForm.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.sponsorshipForm;
			}
			else if (stringBranch == TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternet.ToString()) {
				return TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternet;
			}
			else 
			return 0;

			 
		}

		/// <summary>
		/// Defnit l'univers 
		/// </summary>
		/// <param name="branchType">Type de branche</param>
		/// <param name="alTreeNodeUniverse">arbre univers</param>
		private void SetTreeNodeUniverse(TNS.AdExpress.Constantes.Classification.Branch.type branchType, ArrayList alTreeNodeUniverse) {
			switch (branchType) {
				case TNS.AdExpress.Constantes.Classification.Branch.type.advertiser:
					alTreeNodeUniverse.Add(_webSession.SelectionUniversAdvertiser);
					break;
				case TNS.AdExpress.Constantes.Classification.Branch.type.product:
					if (_webSession.CurrentUniversProduct.FirstNode != null) {
						alTreeNodeUniverse.Add(_webSession.CurrentUniversProduct);
					}
					else {
						alTreeNodeUniverse.Add(_webSession.SelectionUniversAdvertiser);
					}
					break;
				case TNS.AdExpress.Constantes.Classification.Branch.type.brand:
					alTreeNodeUniverse.Add(_webSession.SelectionUniversAdvertiser);
					break;
				case TNS.AdExpress.Constantes.Classification.Branch.type.media:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaTvSponsorship:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaOthers:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaOutdoor:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternationalPress:
				case TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternet:
                case TNS.AdExpress.Constantes.Classification.Branch.type.mediaAdnettrack:
					alTreeNodeUniverse.Add(_webSession.CurrentUniversMedia);
					break;
				case TNS.AdExpress.Constantes.Classification.Branch.type.programType:
					alTreeNodeUniverse.Add(_webSession.SelectionUniversProgramType);
					break;
				case TNS.AdExpress.Constantes.Classification.Branch.type.sponsorshipForm:
					alTreeNodeUniverse.Add(_webSession.SelectionUniversSponsorshipForm);
					break;
				default: break;
			}

			alTreeNodeUniverse.Add(_webSession.TemporaryTreenode);

		}

	}
}
