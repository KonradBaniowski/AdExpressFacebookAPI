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
using WebFunctions=TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Supprime un univers sauvegardé
	/// </summary>
	public partial class GenericDetailLevelRemove: TNS.AdExpress.Web.UI.WebPage{
	
		#region Variables

		#endregion

		#region Variables MMI
		/// <summary>
		/// Texte
		/// </summary>
		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeurs
		/// </summary>
		public GenericDetailLevelRemove():base(){
		}
		#endregion

		#region Evènements
		/// <summary>
		/// Chargement de la page
		/// </summary>
		/// <param name="sender">Objet source</param>
		/// <param name="e">Paramètres</param>
		protected void Page_Load(object sender, System.EventArgs e){
			
			#region Textes et Langage du site
			TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[0].Controls,_webSession.SiteLanguage);			
			#endregion

			try{
				string genericDetailLevelSavedIdString=HttpContext.Current.Request.QueryString.Get("genericDetailLevelSavedId");
				string genericDetailControlNameString=HttpContext.Current.Request.QueryString.Get("genericDetailControlName");
				string genericDetailLevelSavedIndexString=HttpContext.Current.Request.QueryString.Get("genericDetailLevelSavedIndex");
				if(genericDetailLevelSavedIdString.Length>0 && int.Parse(genericDetailLevelSavedIdString)!=-1){
					TNS.AdExpress.Web.Core.DataAccess.Session.GenericDetailLevelDataAccess.Remove(_webSession,Int64.Parse(genericDetailLevelSavedIdString));
					if(genericDetailControlNameString.Length>0 && genericDetailLevelSavedIndexString.Length>0 && !Page.ClientScript.IsClientScriptBlockRegistered("RemoveGenericDetailLevelFromList")){
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"RemoveGenericDetailLevelFromList",WebFunctions.Script.RemoveGenericDetailLevelFromList(genericDetailControlNameString,genericDetailLevelSavedIndexString));
					}
				}
				else{
					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"CloseRemoveGenericDetailLevel","\r\n\t <script language=\"JavaScript\">setTimeout('window.close();',1);\r\n</script>");
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguements</param>
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
		private void InitializeComponent()
		{
           
		}
		#endregion
	}
}
