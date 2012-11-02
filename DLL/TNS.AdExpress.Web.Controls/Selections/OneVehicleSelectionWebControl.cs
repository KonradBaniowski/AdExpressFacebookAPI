#region Informations
// Auteur: A. OBERMEYER
// Date de création: 7/09/2004 
// Date de modification: 7/09/2004 
#endregion

using System;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Affiche la liste des vehicles que peut sélectionner un client en fonction de ses droits.
	/// Le client ne peut sélectionner qu'un média dans la liste (classe dérivant de RadioButtonList)
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:OneVehicleSelectionWebControl runat=server></{0}:OneVehicleSelectionWebControl>")]
	public class OneVehicleSelectionWebControl : System.Web.UI.WebControls.RadioButtonList{

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
				//VehicleListDataAccess vl=new VehicleListDataAccess(webSession);
				//this.DataSource=vl.List;
				CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
				if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
				object[]  param = new object[1];
				param[0] = webSession;
				IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);				
				this.DataSource = classficationDAL.GetMediaType().Tables[0];
                this.DataTextField = "mediaType";
                this.DataValueField = "idMediaType";
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
				for(int i=0; i<this.Items.Count; i++){
					script += " document." + tmp2 + "." + this.ID + "_" + i + ".checked &&";
				}
				script = script.Substring(0, script.Length-3)+";";
				for(int i=0; i<this.Items.Count; i++){
					script += "\n\t document." + tmp2 + "." + this.ID + "_" + i + ".checked = !m;";
				}
				script += "\n\t}\n</script>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"selectAllVehicles",script);
			}
			base.OnLoad (e);
		}
		#endregion
	}
}
