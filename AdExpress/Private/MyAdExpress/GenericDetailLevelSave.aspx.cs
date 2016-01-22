#region Information
 // Auteur : Guillaume Facon
 // Création : 30/04/2006
 // Modification :
#endregion

using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using TNS.AdExpress.Domain.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.AdExpress.Web.Core.Sessions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.DataAccess.Session;
using TNS.AdExpress.Domain.Level;

namespace AdExpress.Private.MyAdExpress{
	/// <summary>
	/// Sauvegarde d'un niveau de détail
	/// </summary>
	public partial class GenericDetailLevelSave : TNS.AdExpress.Web.UI.PrivateWebPage{

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
		public GenericDetailLevelSave():base(){
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
            //for (int i = 0; i < this.Controls.Count; i++) {
            //    TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[i].Controls, _webSession.SiteLanguage);
            //}
			#endregion

			string genericDetailControlNameString="";
			try{
				genericDetailControlNameString=HttpContext.Current.Request.QueryString.Get("genericDetailControlName");
				string levels=HttpContext.Current.Request.QueryString.Get("detailLevel");
				string detailLevelTypeString=HttpContext.Current.Request.QueryString.Get("detailLevelType");
				
				

				if (levels.Length>0 && detailLevelTypeString.Length>0 && genericDetailControlNameString.Length>0){
					WebConstantes.GenericDetailLevel.Type detailLevelType=(WebConstantes.GenericDetailLevel.Type)int.Parse(detailLevelTypeString);
					int levelId;
					string[] levelList=levels.Split(',');
					ArrayList levelIds=new ArrayList();
					foreach(string currentLevel in levelList){
						if(currentLevel.Length>0){
							levelId=int.Parse(currentLevel);
							if(levelId>0)levelIds.Add(levelId);
						}
					}
					 
					if (levelIds.Count>0){
						GenericDetailLevel genericDetailLevel=new GenericDetailLevel(levelIds,TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.savedLevels,detailLevelType);
						if (IsDetailLevelsAlreadySaved(genericDetailLevel)) {//Tests if detail levels are already saved
							if (genericDetailControlNameString.Length > 0) {								
								AlertMessage(2256, genericDetailControlNameString);
							}
						}
						else {
							Int64 listId = TNS.AdExpress.Web.Core.DataAccess.Session.GenericDetailLevelDataAccess.Save(_webSession, genericDetailLevel);
							if (!Page.ClientScript.IsClientScriptBlockRegistered("SaveGenericDetailLevelFromList")) {
								Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SaveGenericDetailLevelFromList", WebFunctions.Script.SaveGenericDetailLevelFromList(genericDetailControlNameString, genericDetailLevel.GetLabel(_webSession.SiteLanguage), listId.ToString()));
							}
						}
					}
					else{
						if(genericDetailControlNameString.Length>0){							
							AlertMessage(1945, genericDetailControlNameString);
						}
					}
				}
			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
					if(genericDetailControlNameString.Length>0){						
						AlertMessage(1945, genericDetailControlNameString);
					}
					this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this,exc,_webSession));
				}
			}
		}

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page:
		///		Fermeture des connections BD
		/// </summary>
		/// <remarks>
		/// Utilise les méthodes:
		///		public static void TNS.AdExpress.Web.DataAccess.Functions.closeDataBase(WebSession _webSession)
		/// </remarks>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#region DeterminePostBackMode
		/// <summary>
		/// DeterminePostBackMode
		/// </summary>
		/// <returns>DeterminePostBackMode</returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
			return(tmp);
		}
		#endregion


		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
		override protected void OnInit(EventArgs e){
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

		#region Private methods 
		/// <summary>
		/// Tests if detail levels are already saved
		/// </summary>
		/// <param name="toTest">detail levels to test</param>
		/// <returns>True if detail levels are already saved</returns>
		private bool IsDetailLevelsAlreadySaved(GenericDetailLevel toTest) {

			DataSet ds;
			int currentLevelId;			
			Int64 currentId = -1;
			int currentIndex = 0;
			ArrayList genericDetailLevelsSaved = new ArrayList();
			 
			try{
				ds = GenericDetailLevelDataAccess.Load(_webSession);
				if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {
					foreach (DataRow currentRow in ds.Tables[0].Rows) {

						if (currentId != Int64.Parse(currentRow["id_list"].ToString())) {
							if (currentId>-1 && ((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).EqualLevelItems(toTest)) return true;
							currentId = Int64.Parse(currentRow["id_list"].ToString());
							currentIndex = genericDetailLevelsSaved.Add(new GenericDetailLevelSaved(currentId, new ArrayList()));							
						}
						currentLevelId = int.Parse(currentRow["id_type_level"].ToString());
						((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).AddLevel(currentLevelId);
					}
					if (currentId > -1 && ((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).EqualLevelItems(toTest)) return true;
				}
			}
			catch (System.Exception err) {
				this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, err, _webSession));
			}

			return false;
		}

		/// <summary>
		/// Set message alert
		/// </summary>
		/// <param name="code">word code word</param>
		/// <param name="genericDetailControlNameString">generic Detail Control Name String</param>
		private void AlertMessage(long code, string genericDetailControlNameString) {
			StringBuilder script = new StringBuilder(2000);
			script.Append("<script language=\"JavaScript\">");
			script.Append("\r\n\t var oN=opener.document.all.item('" + genericDetailControlNameString + "');");
			script.Append("\r\n\t oN.options[oN.length-1]  = null;");
			script.Append("\r\n\t alert(\"" + GestionWeb.GetWebWord(code, this._siteLanguage) + "\");");
			script.Append("\r\n\t setTimeout('window.close();',1);");
			script.Append("\r\n</script>");
			Page.Response.Write(script.ToString());
			Page.Response.Flush();
		}
		#endregion
	}
}
