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
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Rules.Customer;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;


namespace AdExpress.Private.Universe{

	/// <summary>
	/// Attention : ne plus l'utiliser, utiliser saveUniverse 
	/// Sauvegarde d'un univers 
	/// </summary>
	public partial class UniverseSavePopUp : System.Web.UI.Page{

		#region Variables
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
		/// Session du client
		/// </summary>
		protected WebSession _webSession;
		/// <summary>
		/// Liste des groupes d'univers
		/// </summary>
		/// <summary>
		/// Nom de l'univers
		/// </summary>
		/// <summary>
		/// Bouton OK
		/// </summary>
		/// <summary>
		/// Bouton Valider
		/// </summary>
		/// <summary>
		/// Langue du site
		/// </summary>
		protected int _siteLanguage;
		/// <summary>
		/// Identifiant de la session
		/// </summary>
		public Int64 idSession=0;

		/// <summary>
		/// Classification concernée par l'univers a sauvegardé
		/// </summary>
		protected TNS.AdExpress.Constantes.Classification.Branch.type branchType;

		/// <summary>
		/// Classification concernée par l'univers a sauvegardé sous forme de chaine
		/// </summary>
		protected string stringBranch="";
		/// <summary>
		/// Url Suivante
		/// </summary>
		public string url;
		/// <summary>
		/// Identifiant permettant de connaitrele niveau de la nomenclature sauvegardé
		/// dans la nomenclature
		/// </summary>
		protected  Int64 idUniverseClientDescription;

		#endregion 

		#region Chargement de la page
		protected void Page_Load(object sender, System.EventArgs e)
		{

			
			_webSession = (WebSession)WebSession.Load(Page.Request.QueryString.Get("idSession"));
			stringBranch= Page.Request.QueryString.Get("brancheType");

			if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.media.ToString()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.media;
			}else if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.advertiser.ToString()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.advertiser;
			}
			else if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.brand.ToString())
			{
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.brand;
			}
			else if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.product.ToString()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.product;
			}else if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress.ToString()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress;
			}else if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio.ToString()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio;
			}else if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv.ToString()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv;
			}else if(stringBranch==TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternet.ToString()){
				branchType=TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternet;
			}


			_siteLanguage=_webSession.SiteLanguage;
			
			//Connaitre la page annonceur (annonceur, concurentiel)
			Module currentModuleDescription=TNS.AdExpress.Web.Core.Navigation.ModulesList.GetModule(_webSession.CurrentModule);
			if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.advertiser
				|| branchType==TNS.AdExpress.Constantes.Classification.Branch.type.brand){
				url=((ResultPageInformation) currentModuleDescription.SelectionsPages[0]).Url;
			} 
			
			if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.media  
			||	branchType==TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress
			||	branchType==TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio
			||	branchType==TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv
			&& _webSession.CurrentModule==277
				){
				url=((ResultPageInformation) currentModuleDescription.SelectionsPages[1]).Url;
			}

			if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.product
				&& _webSession.CurrentModule==277
				){
				url=((ResultPageInformation) currentModuleDescription.SelectionsPages[3]).Url;
			}

			// id session utilisée dans la page aspx
			idSession=Int64.Parse(_webSession.IdSession);

			//Modification de la langue pour les Textes AdExpress
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);
						
			//Langage du site
			_siteLanguage = _webSession.SiteLanguage;

			// RollOver
			cancelImageButtonRollOverWebControl.ImageUrl="/Images/"+_webSession.SiteLanguage+"/button/annuler_up.gif";
			cancelImageButtonRollOverWebControl.RollOverImageUrl="/Images/"+_webSession.SiteLanguage+"/button/annuler_down.gif";

			if (!IsPostBack){
				//Liste des groupes d'univers
				DataSet ds= TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetGroupUniverses(_webSession);		
		
				directoryDropDownList.DataSource=ds.Tables[0];
				directoryDropDownList.DataTextField=ds.Tables[0].Columns[1].ToString();
				directoryDropDownList.DataValueField=ds.Tables[0].Columns[0].ToString();
				directoryDropDownList.DataBind();				
			}

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

		}
		#endregion

		#region Bouton OK
		/// <summary>
		/// Gestion du bouton OK
		/// </summary>
		/// <param name="sender">Objet qui envoie l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void ImageButtonRollOverWebControl1_Click(object sender, System.EventArgs e) {

			if (directoryDropDownList.Items.Count!=0){
				string universeName = this.universeTextBox.Text;
				universeName = CheckedText.CheckedAccentText(universeName);
				if (universeName.Length!=0 && universeName.Length<TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT){				
					if (!UniversListDataAccess.IsUniverseExist(_webSession, universeName)){

						ArrayList alTreeNodeUniverse=new ArrayList();
						#region Ancienne version
//						if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.advertiser){
//							alTreeNodeUniverse.Add(_webSession.SelectionUniversAdvertiser);
//						}
//						else if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.brand)
//						{
//							alTreeNodeUniverse.Add(_webSession.CurrentUniversProduct);
//						}
//						else if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.product){
//							alTreeNodeUniverse.Add(_webSession.CurrentUniversProduct);
//						}
//						else if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.media){
//							alTreeNodeUniverse.Add(_webSession.CurrentUniversMedia);
//						}else if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress){
//							alTreeNodeUniverse.Add(_webSession.CurrentUniversMedia);
//						}else if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio){
//							alTreeNodeUniverse.Add(_webSession.CurrentUniversMedia);
//						}else if(branchType==TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv){
//							alTreeNodeUniverse.Add(_webSession.CurrentUniversMedia);
//						}	
						#endregion
						switch(branchType){
							case TNS.AdExpress.Constantes.Classification.Branch.type.advertiser :
								alTreeNodeUniverse.Add(_webSession.SelectionUniversAdvertiser);
								break;
							case TNS.AdExpress.Constantes.Classification.Branch.type.product :							
							case TNS.AdExpress.Constantes.Classification.Branch.type.brand :
								alTreeNodeUniverse.Add(_webSession.CurrentUniversProduct);
								break;
							case TNS.AdExpress.Constantes.Classification.Branch.type.media :
							case TNS.AdExpress.Constantes.Classification.Branch.type.mediaPress :
							case TNS.AdExpress.Constantes.Classification.Branch.type.mediaTv :	
							case TNS.AdExpress.Constantes.Classification.Branch.type.mediaRadio :												
							case TNS.AdExpress.Constantes.Classification.Branch.type.mediaInternet :
								alTreeNodeUniverse.Add(_webSession.CurrentUniversMedia);
								break;							
							default :break;
						}

						alTreeNodeUniverse.Add(_webSession.TemporaryTreenode);

						if(UniversListDataAccess.SaveUniverse(Int64.Parse(directoryDropDownList.SelectedItem.Value),universeName,alTreeNodeUniverse,branchType,idUniverseClientDescription,_webSession)){
							// Validation : confirmation d'enregistrement de l'univers
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(921,_webSession.SiteLanguage)+"\");");
							Response.Write("	window.close();");						
							Response.Write("</script>");
						}
						else{
							// Erreur : Echec de l'enregistrement de l'univers
							Response.Write("<script language=javascript>");
							Response.Write("	alert(\""+GestionWeb.GetWebWord(922,_webSession.SiteLanguage)+"\");");
							Response.Write("</script>");
						}
					}
					else{
						// Erreur : univers déjà existant
						Response.Write("<script language=javascript>");
						Response.Write("	alert(\""+GestionWeb.GetWebWord(923,_webSession.SiteLanguage)+"\");");
						Response.Write("</script>");
					}
				}
				else if(universeName.Length==0){
					// Erreur : Le champs est vide
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(837,_webSession.SiteLanguage)+"\");");
					Response.Write("</script>");
				}
				else{
					// Erreur : suppérieur à 50 caractères
					Response.Write("<script language=javascript>");
					Response.Write("	alert(\""+GestionWeb.GetWebWord(823,_webSession.SiteLanguage)+"\");");
					Response.Write("</script>");
				}
			}
			else{
				// Erreur : Impossible de sauvegarder, pas de groupe d'univers créé
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(925,_webSession.SiteLanguage)+"\");");
				Response.Write("</script>");
			}
		}
		#endregion

		#region Annuler
		protected void cancelImageButtonRollOverWebControl_Click(object sender, System.EventArgs e) {
			DBFunctions.closeDataBase(_webSession);
			Response.Write("<script language=javascript>");
			Response.Write("	window.close();");
			Response.Write("</script>");
		}
		#endregion
		

	}
}
