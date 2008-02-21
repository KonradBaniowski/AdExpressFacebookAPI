#region Information
// Auteur : G. Facon
// Cr�� le : 25/10/2005
// Modifi� le : 
//
#endregion

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
using System.Windows.Forms;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpressWebRules=TNS.AdExpress.Web.Rules;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using ProductClassification=TNS.AdExpress.Classification.DataAccess.ProductBranch;
using MediaClassification=TNS.AdExpress.Classification.DataAccess.MediaBranch;
using AdExpressException=TNS.AdExpress.Exceptions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork.DB.Common;

using TNS.FrameWork.Date;

namespace AdExpress.Private{
	/// <summary>
	/// Point d'entrer du site AdExpress Pour TNS Creative Explorer
	/// </summary>
	public partial class Creative : System.Web.UI.Page{

		#region Constantes
		/// <summary>
		/// Login de TNS Creative Explorer
		/// </summary>
		private const string LOGIN="CREATIVE3";
		/// <summary>
		/// Password de TNS Creative Explorer
		/// </summary>
		private const string PASSWORD="EXPLOV3";
		#endregion

		#region Ev�nements

		#region Chargement de la Page
		/// <summary>
		/// Chargement de la Page
		/// </summary>
		/// <param name="sender">Objet Source</param>
		/// <param name="e">Param�tres</param>
		protected void Page_Load(object sender, System.EventArgs e){

			System.Windows.Forms.TreeNode tmpNode;
			Int64 id=0;
			try{
				#region On r�cup�re les param�tres de TNS Creative Explorer
				string dateBegin=HttpContext.Current.Request.QueryString.Get("date_debut");
				string dateEnd=HttpContext.Current.Request.QueryString.Get("date_fin");
				string[] vehicleListId=HttpContext.Current.Request.QueryString.Get("idVehicle").Split(',');
				string vehicleListIdString=HttpContext.Current.Request.QueryString.Get("idVehicle");
				Int64 productList=Int64.Parse(HttpContext.Current.Request.QueryString.Get("listeProduct"));
				int siteLanguage=int.Parse(HttpContext.Current.Request.QueryString.Get("langueSite"));
				#endregion

				#region On test les param�tres
				if(dateBegin.Length!=8 || dateEnd.Length!=8) throw (new AdExpressException.AdExpressCustomerException("Param�tres invalides"));
				if(vehicleListIdString.Length<1) throw (new AdExpressException.AdExpressCustomerException("Param�tres invalides"));
				//Verification de la page d'appel
				//Page.Request.UrlReferrer								 
				#endregion

				#region On cr�e la Session (authentification)
			
				WebSession webSession=null;
				//Creer l'objet Right
                TNS.FrameWork.DB.Common.IDataSource source = new TNS.FrameWork.DB.Common.OracleDataSource("User Id=" + LOGIN + "; Password=" + PASSWORD + " " + TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING);
				TNS.AdExpress.Web.Core.WebRight loginRight=new TNS.AdExpress.Web.Core.WebRight(LOGIN,PASSWORD,source);
				//V�rifier le droit d'acc�s au site
				if (loginRight.CanAccessToAdExpress(source)){
					// Regarde � partir de quel tables charger les droits clients
					// (template ou droits propres au login)
					loginRight.htRightUserBD();				
					if(true){
						//Creer une session
						if(webSession==null) webSession = new WebSession(loginRight);
						// On Met � jour la session avec les param�tres
						webSession.CurrentModule=WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
						webSession.Insert=TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;
						webSession.Unit=WebConstantes.CustomerSessions.Unit.euro;
						webSession.LastReachedResultUrl="";

						webSession.CurrentUniversMedia=new System.Windows.Forms.TreeNode("media");
						MediaClassification.PartialVehicleListDataAccess vehicleLabels=new MediaClassification.PartialVehicleListDataAccess(vehicleListIdString,siteLanguage,loginRight.Connection);
						foreach (string currentVehicleId in vehicleListId){
							id=Int64.Parse(currentVehicleId);
							tmpNode=new System.Windows.Forms.TreeNode(vehicleLabels[id]);
							tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,Int64.Parse(currentVehicleId),vehicleLabels[id]);
							webSession.CurrentUniversMedia.Nodes.Add(tmpNode);
						}
						webSession.SelectionUniversMedia=(System.Windows.Forms.TreeNode)webSession.CurrentUniversMedia.Clone();
						
						webSession.CurrentUniversProduct =  new System.Windows.Forms.TreeNode("produit");
						ProductClassification.PartialProductLevelListDataAccess productLabels=new ProductClassification.PartialProductLevelListDataAccess(productList.ToString(),siteLanguage,loginRight.Connection);
						tmpNode=new System.Windows.Forms.TreeNode(productLabels[productList]);
						tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess,productList,productLabels[productList]);
						webSession.CurrentUniversProduct.Nodes.Add(tmpNode);
						webSession.SelectionUniversAdvertiser=(System.Windows.Forms.TreeNode) webSession.CurrentUniversProduct.Clone();
						webSession.CurrentUniversAdvertiser=(System.Windows.Forms.TreeNode) webSession.CurrentUniversProduct.Clone();

						webSession.DetailPeriod=WebConstantes.CustomerSessions.Period.DisplayLevel.weekly;
						webSession.PeriodType=WebConstantes.CustomerSessions.Period.Type.dateToDateWeek;
						webSession.PeriodBeginningDate = DateString.AtomicPeriodWeekToYYYYWW(new AtomicPeriodWeek(new DateTime(int.Parse(dateBegin.Substring(0,4)),int.Parse(dateBegin.Substring(4,2)),int.Parse(dateBegin.Substring(6,2)))));
						webSession.PeriodEndDate = DateString.AtomicPeriodWeekToYYYYWW(new AtomicPeriodWeek(new DateTime(int.Parse(dateEnd.Substring(0,4)),int.Parse(dateEnd.Substring(4,2)),int.Parse(dateEnd.Substring(6,2)))));	


						webSession.SiteLanguage=siteLanguage;
						//Sauvegarder la session
						webSession.Save();
						Response.Redirect("/Private/Results/CreativeMediaPlanResults.aspx?idSession="+webSession.IdSession);
						Response.Flush();
					}
				}
				throw (new AdExpressException.AdExpressCustomerException("Connection impossible"));
			}
			catch(TNS.AdExpress.Exceptions.AdExpressCustomerException){
				Response.Write("<script language=javascript>");
				Response.Write("	alert(\""+GestionWeb.GetWebWord(880,33)+"\");");
				Response.Write("</script>");
				Response.Flush();
			}
			#endregion

			// On affiche le r�sultat
		
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Param�tres</param>
		override protected void OnInit(EventArgs e){
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette m�thode avec l'�diteur de code.
		/// </summary>
		private void InitializeComponent(){    
		}
		#endregion
	}
}
