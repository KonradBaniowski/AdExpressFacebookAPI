#region Informations
// Auteur: D.V Mussuma 
// Date de création: 14/09/2004 
// Date de modification: 01/10/2004
//		03/11/2004 B. Masson ajout espace devant les média sans checkbox 
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
using VhCstes=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using TableName=TNS.AdExpress.Constantes.Classification.DB.Table.name;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Show media list to select.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:SectorAnalysisVehicleSelectionWebControl runat=server></{0}:SectorAnalysisVehicleSelectionWebControl>")]
	public class SectorAnalysisVehicleSelectionWebControl :  System.Web.UI.WebControls.CheckBoxList
	{
		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession webSession=null; 		
		/// <summary>
		/// Current vehicle list
		/// </summary>		
		protected List<long>  _currentVehicleList=null;
		/// <summary>
		/// Indique si la liste doit être rechargée ou non.
		/// </summary>
		protected bool _reload = false;
		/// <summary>
		/// liste des médias(vehicle) dont le clients à accès
		/// </summary>
		protected DataTable dtVehicle;
		#endregion

		#region Accesseurs

		/// <summary>
		/// Définit la session à utiliser
		/// </summary>
		public virtual WebSession CustomerWebSession 
		{
			set{webSession = value;}
		}

		/// <summary>
		/// Définit siu la liste des médias doit être rechargée
		/// </summary>
		public virtual bool Reload{
			set{_reload=value;}
		}

		#endregion

		#region Evènements
		
		#region Onload
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e) 
		{	
			#region Creation du dataSet
			if(!Page.IsPostBack){
				if( webSession != null) {
					//Data loading
					VehicleListDataAccess vl = new VehicleListDataAccess(webSession);
					dtVehicle = vl.List;
				}
				else throw (new WebControlInitializationException("Impossible to init component, web session is not define"));
			}
			#endregion
		}
		#endregion

		#region PreRender
		/// <summary>
		/// Bulid checkbox list
		/// </summary>
		/// <param name="e">event arguments</param>
		protected override void OnPreRender(EventArgs e) 
		{
			if (!Page.IsPostBack)
			{	
				#region Variables locales
				//Variables media
				Int64 IdVehicle = 0;
				Int64 oldIdVehicle = 0;
				
				//Variables categorie
				Int64 IdCategory = 0;
				Int64 oldCategory = 0;				
				#endregion

				//Clear items list
				this.Items.Clear();
				
				//Build checkbox list
				_currentVehicleList = new List<long>();
				VehicleInformation vehicleInfo = VehiclesInformation.Get(VhCstes.plurimedia);
				if (vehicleInfo != null) {
					//Remark : It's always possible to select plurimedia vehicle
					this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(210, webSession.SiteLanguage), "vh_" + vehicleInfo.DatabaseId.ToString()));
					_currentVehicleList.Add(vehicleInfo.DatabaseId);
				}
				
				foreach(DataRow currentRow in dtVehicle.Rows) 
				{					
					if ( (IdVehicle = Int64.Parse(currentRow["id_vehicle"].ToString())) != oldIdVehicle )
					{							
						oldIdVehicle = IdVehicle;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["vehicle"].ToString(),"vh_"+IdVehicle));
					}
					if ( ((IdCategory  = (Int64)currentRow["id_category"]) != oldCategory ) && showCategory(Int64.Parse(currentRow["id_vehicle"].ToString())) )
					{	oldCategory = IdCategory ;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["category"].ToString(),"ct_"+(Int64)currentRow["id_category"]));
					}
					if (showMedia(Int64.Parse(currentRow["id_vehicle"].ToString())))
					{						
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["media"].ToString(),"md_"+(Int64)currentRow["id_media"]));
					}
					if (!_currentVehicleList.Contains(Int64.Parse(currentRow["id_vehicle"].ToString()))) _currentVehicleList.Add(Int64.Parse(currentRow["id_vehicle"].ToString()));
				}	
						
			}
		}

		#endregion

		#region Render 
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output)
		{
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			
			if(_reload){
				if( webSession != null) {
					//Data loading
					VehicleListDataAccess vl = new VehicleListDataAccess(webSession);
					dtVehicle = vl.List;
				}
				else throw (new WebControlInitializationException("Impossible to load data, webSession is not define"));
			}


			if (dtVehicle != null && dtVehicle.Rows.Count > 0)
			{
				#region variables locales
				//variables du niveau  Media
				Int64 idVehicleOld=-1;
				Int64 idVehicle = -2;										
				int startVehicle=-1;	
				string vhSeparator="";
				string VehicleIds="";
				string[] VehicleIdsArr=null;
				string 	vhlist="";								
				//variables du niveau Category
				Int64 idCategoryOld=-1;
				Int64 idCategory = -2;										
				int startCategory=-1;										
				//variables du niveau Support
				Int64 i = 0;				
				int numColumn = 0;			
				string checkBox="";						
				#endregion				

				if (_currentVehicleList != null && _currentVehicleList.Count>0) {
					
					for (int v = 0; v < _currentVehicleList.Count; v++) {
						if (v > 0) vhSeparator = "-";
						VehicleIds += vhSeparator + "vh_" + _currentVehicleList[v].ToString();
					}					
				}

				//Global table 
                t.Append("\n<tr vAlign=\"top\" height=\"1%\">\n<td class=\"backGroundWhite\" >\n");	
				t.Append("<a href=\"javascript: ExpandColapseAllDivs('"+vhlist+"')\" ");
				t.Append("\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;"+GestionWeb.GetWebWord(1117,webSession.SiteLanguage)+"</a>");	
				
				#region  PluriMedia
				VehicleInformation vehicleInfo = VehiclesInformation.Get(VhCstes.plurimedia);
				if (vehicleInfo != null) {
					//Top table border
					t.Append("\n<tr><td><div style=\"MARGIN-LEFT: 10px\" id=\"vh_" + vehicleInfo.DatabaseId.ToString() + "\" >");
					if (idVehicleOld == -1) t.Append("\n<table class=\"backGroundWhite violetBorderWithoutBottom txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n<tr>");
					else t.Append("\n<table class=\"backGroundWhite violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n<tr>");
					t.Append("\n<td align=\"left\" height=\"10\" valign=\"top\" width=100%>");
					t.Append("\n<input type=checkbox name=\"SectorAnalysisVehicleSelectionWebControl1$" + i + "\" name=\"SectorAnalysisVehicleSelectionWebControl1_" + i + "\" onClick=\"CheckAllChilds('vh_" + vehicleInfo.DatabaseId.ToString() + "','" + VehicleIds + "','vh_" + vehicleInfo.DatabaseId.ToString() + "')\" value=\"vh_" + vehicleInfo.DatabaseId.ToString() + "\">" + GestionWeb.GetWebWord(210, webSession.SiteLanguage) + "");
					t.Append("\n</td>\n</tr>");
					t.Append("\n</table></div></td></tr>");
					i++;
				}
				#endregion

				#region For each  vehicle

				foreach (DataRow currentRow in dtVehicle.Rows) {
					//Initialisation of parents Id
					idVehicle = (Int64)currentRow["id_vehicle"];
					idCategory = (Int64)currentRow["id_category"];

					#region contruction tableau principal
					//Close Category				
					if (idCategory != idCategoryOld && startCategory == 0) {	//Fermeture support
						if (showMedia(idVehicle)) {
							if (numColumn != 0) t.Append("</tr>");
						}
						t.Append("</table>");
						t.Append("</td></tr></table></div>");
						t.Append("</DIV></td></tr>");

					}

					//Close Vehicle				
					if (idVehicle != idVehicleOld && startVehicle == 0) {
						startCategory = -1;
						if (OpenVehicleDiv(idVehicle)) {
							t.Append("\n</TD></TR>");
						}
						t.Append("\n</TABLE></Div>\n</td></tr>");

						t.Append("\n</table></div>");
						t.Append("\n</td></tr>");
					}

					#region New Vehicle
					//New Vehicle 
					if (idVehicle != idVehicleOld) {
						//Border top table						
						t.Append("\n<tr><td><div style=\"MARGIN-LEFT: 10px\" id=\"vh_" + idVehicle + "\" >");
						if (idVehicleOld == -1) t.Append("\n<table class=\"backGroundWhite violetBorder txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n");
						else t.Append("\n<table class=\"backGroundWhite violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n");

						//Cursor on  line
						if (OpenVehicleDiv(idVehicle))
							t.Append("\n<tr  style=\"cursor : hand\">");
						else t.Append("\n<tr>");

						idVehicleOld = idVehicle;
						startVehicle = 0;

						//checkbox vehicle 
						t.Append("\n<td align=\"left\" height=\"10\" valign=\"top\" >");
						t.Append("\n<input type=checkbox name=\"SectorAnalysisVehicleSelectionWebControl1$" + i + "\" name=\"SectorAnalysisVehicleSelectionWebControl1_" + i + "\" onClick=\"CheckAllChilds('vh_" + idVehicle + "','" + VehicleIds + "','vh_" + idVehicle + "')\" value=vh_" + idVehicle + "></td>");

						//End label vehicle 						
						if (OpenVehicleDiv(idVehicle)) {
							t.Append("<td width=100% align=\"left\" onClick=\"javascript : DivDisplayer('vhDiv" + idVehicle + "');\" >" + currentRow["vehicle"].ToString() + "");
							t.Append("</td><td align=\"right\" onClick=\"javascript : DivDisplayer('vhDiv" + idVehicle + "');\">");
							t.Append("<IMG src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\">");
						}
						else {
							t.Append("<td width=100% align=\"left\">" + currentRow["vehicle"].ToString() + "");
						}
						t.Append("</td></tr>");

						t.Append("<tr><td colspan=\"3\" >");
						t.Append("<Div  style=\"display:none;\" id=\"vhDiv" + idVehicle + "\">");
						t.Append("<TABLE cellpadding=0 cellspacing=0 width=100% class=\"violetBackGroundV3 violetBorderTop txtViolet11Bold\">");

						if (OpenVehicleDiv(idVehicle)) {
							t.Append("<tr><td class=\"roll04\"><a href=\"javascript: SelectExclusiveAllChilds('vh_" + idVehicle + "','" + VehicleIds + "','vh_" + idVehicle + "','vh_','ct_')\" title=\"" + GestionWeb.GetWebWord(1151, webSession.SiteLanguage) + "\" class=\"roll04\">&nbsp;&nbsp;" + GestionWeb.GetWebWord(1151, webSession.SiteLanguage) + "</td></tr>");
							t.Append("<TR><TD>");
						}
						i++;
					}
					#endregion

					#region New category
					if (idCategory != idCategoryOld && showMediaBranchItem(TableName.category, idVehicle)) {
						numColumn = 0;
						if (startCategory == -1) t.Append("<tr><td ><div style=\"MARGIN-LEFT: 5px; width=100%;\" class=\"violetBackGroundV3\"  id=\"ct_" + idCategory + "\"><table class=\"violetBackGroundV3 txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
						else t.Append("<tr><td><div style=\"MARGIN-LEFT: 5px; width=100%;\" class=\"violetBackGroundV3\"  id=\"ct_" + idCategory + "\"><table class=\"violetBackGroundV3 violetBorderTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");

						//Cursor on  line
						if (showMediaBranchItem(TableName.media, idVehicle)) t.Append("\n<tr  style=\"cursor : pointer\">");
						else t.Append("\n<tr>");

						idCategoryOld = idCategory;
						startCategory = 0;
						t.Append("<td  align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");

						// checkbox category 
						t.Append("<input type=checkbox name=\"SectorAnalysisVehicleSelectionWebControl1$" + i + "\" id=\"SectorAnalysisVehicleSelectionWebControl1_" + i + "\" onClick=\"CheckAllChilds('ct_" + idCategory + "','" + VehicleIds + "','vh_" + idVehicle + "')\" value=\"ct_" + idCategory + "\">");
						t.Append("\n</td>\n");
						//Label category
						t.Append("<td width=100% valign=\"middle\" height=\"10\" onClick=\"javascript : DivDisplayer('" + idCategory + "');\" align=\"left\"  ");
						if (showMediaBranchItem(TableName.media, idVehicle)) {
							t.Append(">&nbsp;" + currentRow["category"].ToString() + "</td>");
							t.Append("<td valign=\"baseline\"  onClick=\"javascript : DivDisplayer('" + idCategory + "');\"   ");
							t.Append(">&nbsp;<IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\" align=\"right\"></td>");
						}
						else t.Append(" width=\"15\" colspan=\"2\" >&nbsp;" + currentRow["category"].ToString() + "</td>");

						t.Append("</tr><tr><td colspan=\"3\"><DIV id=\"" + idCategory + "\" ><table cellpadding=0 cellspacing=0 border=\"0\" width=100%>");
						if (showMediaBranchItem(TableName.media, idVehicle)) {
							t.Append("<table cellpadding=0 cellspacing=0 border=\"0\" width=100% class=\"mediumPurple1 whiteTopBorder txtViolet10\">");
							t.Append("<tr><td colspan=\"3\" class=\"roll04\" ><a href=\"javascript: SelectExclusiveAllChilds('ct_" + idCategory + "','" + VehicleIds + "','vh_" + idVehicle + "','vh_','ct_')\" title=\"" + GestionWeb.GetWebWord(1066, webSession.SiteLanguage) + "\" class=\"roll04\">&nbsp;" + GestionWeb.GetWebWord(1066, webSession.SiteLanguage) + "</a></td></tr>");
						}
						i++;
					}
					#endregion

					#region Show media checkbox
					if (showMediaBranchItem(TableName.media, idVehicle)) {
						if (numColumn == 0) {
							t.Append("<tr >");
							t.Append("<td  width=\"33%\">");
							t.Append("<input type=\"checkbox\" " + checkBox + " name=\"SectorAnalysisVehicleSelectionWebControl1$" + i + "\" id=\"SectorAnalysisVehicleSelectionWebControl1_" + i + "\" value=\"md_" + currentRow["media"] + "\" onClick=\"CheckAllChilds('md_" + currentRow["id_media"] + "','" + VehicleIds + "','vh_" + idVehicle + "')\">" + currentRow["media"].ToString());
							t.Append("</td>");
							numColumn++;
							i++;
						}
						else if (numColumn == 1) {
							t.Append("<td  width=\"33%\">");
							t.Append("<input type=\"checkbox\" " + checkBox + " name=\"SectorAnalysisVehicleSelectionWebControl1$" + i + "\"  id=\"SectorAnalysisVehicleSelectionWebControl1_" + i + "\" value=\"md_" + currentRow["media"] + "\" onClick=\"CheckAllChilds('md_" + currentRow["id_media"] + "','" + VehicleIds + "','vh_" + idVehicle + "')\">" + currentRow["media"].ToString());
							t.Append("</td>");
							numColumn++;
							i++;
						}
						else {
							t.Append("<td  width=\"33%\">");
							t.Append("<input type=\"checkbox\" name=\"SectorAnalysisVehicleSelectionWebControl1$" + i + "\" onClick=\"CheckAllChilds('md_" + currentRow["id_media"] + "','" + VehicleIds + "','vh_" + idVehicle + "')\"  value=\"md_" + currentRow["media"] + "\" id=\"SectorAnalysisVehicleSelectionWebControl1_" + i + "\" >" + currentRow["media"].ToString());
							t.Append("</td></tr>");
							numColumn = 0;
							i++;
						}
					}
					#endregion

					#endregion

				}

				#endregion

				//Fermeture Tableau global	
				//Close Category				
				if ( startCategory == 0 && showMediaBranchItem(TableName.category, idVehicle)) {	//Fermeture support
					if (showMedia(idVehicle)) {
						if (numColumn != 0) t.Append("</tr>");
					}
					t.Append("</table>");
					t.Append("</td></tr></table></div>");
					t.Append("</DIV></td></tr>");
				}
				//Close Vehicle				
				if ( startVehicle == 0) {
					if (OpenVehicleDiv(idVehicle)) {
						t.Append("\n</TD></TR>");
					}
					t.Append("\n</TABLE></Div>\n</td></tr>");

					t.Append("\n</table></div>");
					t.Append("\n</td></tr>");

				}				
				t.Append(" \n\n</td>\n</tr>\n ");
			}
			else
			{
				t.Append("\n<tr>\n<td bgcolor=\"#ffffff\" class=\"txtGris11Bold\">\n<p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" "+GestionWeb.GetWebWord(1091,webSession.SiteLanguage)+"\n</p> ");
				t.Append(" \n</td> ");
				t.Append(" \n</tr> ");
			}
			output.Write(t.ToString());

		}
		#endregion

		#endregion

		#region Internal methods

		/// <summary>
		/// Dermine if media level can be shown for the current vehicle 
		/// </summary>
		/// <param name="idVehicle">Id Vehicle</param>
		/// <param name="mediaBranchItem">Media branch</param>
		/// <returns>True if category level can be shown </returns>
		private bool showMediaBranchItem(DBConstantesClassification.Table.name mediaBranchItem, Int64 idVehicle) {
			VehicleInformation vehicleInfo = VehiclesInformation.Get(idVehicle);
			switch(mediaBranchItem)
			{	
				case DBConstantesClassification.Table.name.vehicle:
					return(true);
				case DBConstantesClassification.Table.name.category:
					return(showCategory(idVehicle));
				case DBConstantesClassification.Table.name.media:
					return(showMedia(idVehicle));
			}
			return(false);
		}

		/// <summary>
		/// Dermine if category level can be shown 
		/// </summary>
		/// <param name="idVehicle">Id Vehicle</param>
		/// <returns>True if category level can be shown </returns>
		private bool showCategory(Int64 idVehicle)
		{
			VehicleInformation vehicleInfo = VehiclesInformation.Get(idVehicle);
			return (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category));
			#region A supprimer
			//switch (vehicleInfo.Id)
			//{
			//    case DBConstantesClassification.Vehicles.names.cinema:
			//        return(false);
			//    default:
			//        return(true);
			//}
			#endregion
		}

		/// <summary>
		/// Dermine if media level can be shown 
		/// </summary>
		/// <param name="idVehicle">Id Vehicle</param>
		/// <returns>True if media level can be shown </returns>
		private bool showMedia(Int64 idVehicle)
		{
			VehicleInformation vehicleInfo = VehiclesInformation.Get(idVehicle);
			return (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media));
			#region A supprimer
			//switch (vehicleInfo.Id)
			//{
			//    case DBConstantesClassification.Vehicles.names.internet:
			//    case DBConstantesClassification.Vehicles.names.press:
			//    case DBConstantesClassification.Vehicles.names.internationalPress:
			//    case DBConstantesClassification.Vehicles.names.cinema:
			//    case DBConstantesClassification.Vehicles.names.mobileTelephony:
			//    case DBConstantesClassification.Vehicles.names.emailing:
			//        return(false);
			//    default:
			//        return(true);
			//}
			#endregion
		}

		/// <summary>
		/// Indique si le  calque contenant média(vehicle) peut etre ouvert ou fermé
		/// par un clique sur l'image associée 
		/// </summary>
		/// <param name="idVehicle">Vehicle à traiter</param>
		/// <returns>True s'il doit être montrer, false sinon</returns>
		private bool OpenVehicleDiv(Int64 idVehicle)
		{
			VehicleInformation vehicleInfo = VehiclesInformation.Get(idVehicle);
			return (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category));
			#region A supprimer
			//switch (vehicleInfo.Id)
			//{
			//    case DBConstantesClassification.Vehicles.names.cinema:
			//        return(false);
			//    default:
			//        return(true);
			//}
			#endregion
		}
		#endregion

		
	}
}
