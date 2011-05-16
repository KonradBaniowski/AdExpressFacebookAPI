#region Information
// Auteur : G. Facon
// Créé le : 25/10/2005
// Modifié le : 
//
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpressWebRules=TNS.AdExpress.Web.Rules;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.UI;
using ProductClassification=TNS.AdExpress.DataAccess.Classification.ProductBranch;
using MediaClassification=TNS.AdExpress.DataAccess.Classification.MediaBranch;
using AdExpressException=TNS.AdExpress.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;

using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress;
using TNS.AdExpressI.Visual;
using System.Reflection;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;

namespace AdExpress.Private{
	/// <summary>
	/// Point d'entrer du site AdExpress Pour TNS Creative Explorer
	/// </summary>
    public partial class CreativeView : TNS.AdExpress.Web.UI.WebPage {
    
        #region Evènements

        #region Chargement de la Page
        /// <summary>
		/// Chargement de la Page
		/// </summary>
		/// <param name="sender">Objet Source</param>
		/// <param name="e">Paramètres</param>
		protected void Page_Load(object sender, System.EventArgs e){
            try {

                #region On récupère les paramètres de TNS Creative Explorer
                string path = HttpContext.Current.Request.QueryString.Get("path");
                Int64 vehicleId = Int64.Parse(HttpContext.Current.Request.QueryString.Get("id_vehicle"));
                bool isBlur = bool.Parse(HttpContext.Current.Request.QueryString.Get("is_blur"));
                #endregion

                object[] parameters = new object[2];
                parameters[0] = vehicleId;
                parameters[1] = path;

                IVisual visual = (IVisual)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

                Page.Response.ContentType = "image/jpeg";
                Page.Response.BinaryWrite(visual.GetBinaries(isBlur));

            }
            catch(Exception er) {
            }
		
		}
		#endregion

		#endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Paramètres</param>
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
	}
}
