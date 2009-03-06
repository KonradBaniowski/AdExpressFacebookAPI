#region Informations
// Auteur: G. Facon 
// Date de création: 10/05/2004 
// Date de modification: 14/05/2004 
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Affiche la liste des vehicles que peut sélectionner un client en fonction de ses droits.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:VehicleSelectionWebControl runat=server></{0}:VehicleSelectionWebControl>")]
	public class VehicleSelectionWebControl : System.Web.UI.WebControls.CheckBoxList{

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession webSession=null; 
 
		#endregion
	
		#region Accesseurs

		/// <summary>
		/// Définit la session à utiliser
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set{webSession = value;}
			}

		#endregion

		#region Evènements
		/// <summary>
		/// Méthode lancée avant le rendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if(webSession!=null){
				VehicleListDataAccess vl=new VehicleListDataAccess(webSession);
				this.DataSource=vl.List;
				this.DataTextField="vehicle";
				this.DataValueField="idVehicle";
				this.DataBind();
			}
			else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas définie"));
			//Edition du javascript permettant la selection de tous les vehicules
			if (!Page.ClientScript.IsClientScriptBlockRegistered("selectAllVehicles")){
				string script = "\n<script language=\"JavaScript\">";
				script += "\n\tfunction selectAllVehicles(){";
				string tmp2="";
				Object tmp = this.Parent;
				while (tmp!= null && tmp.GetType() != typeof(System.Web.UI.HtmlControls.HtmlForm))
					tmp = ((System.Web.UI.Control) tmp).Parent;
				if (tmp != null)
					tmp2 =((System.Web.UI.HtmlControls.HtmlForm) tmp).ID;
				else tmp2 = "Form2";
				script += "\n\tm=";

                string[] idMedias = Lists.GetIdList(GroupList.ID.media, GroupList.Type.mediaInSelectAll).Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0; i<this.Items.Count; i++){
                    if (idMedias == null || idMedias.Length == 0
                        || Array.Exists<string>(idMedias, delegate(string s) { return s == this.Items[i].Value; }))
                    {
                        script += " document." + tmp2 + "." + this.ID + "_" + i + ".checked &&";
                    }
				}
				script = script.Substring(0, script.Length-3)+";";
				for(int i=0; i<this.Items.Count; i++){
                    script += string.Format("\n\t document.{0}.{1}_{2}.checked = !m && {3};"
                        , tmp2, this.ID, i
                        , (idMedias == null || idMedias.Length == 0 || Array.Exists<string>(idMedias, delegate(string s) { return s == this.Items[i].Value; })).GetHashCode());
				}
				script += "\n\t}\n</script>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"selectAllVehicles",script);
			}
			base.OnLoad (e);
		}
		#endregion
	}
}
