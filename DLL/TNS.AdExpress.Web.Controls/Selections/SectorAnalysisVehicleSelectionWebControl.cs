#region Informations
// Auteur: D.V Mussuma 
// Date de création: 14/09/2004 
// Date de modification: 01/10/2004
//		03/11/2004 B. Masson ajout espace devant les média sans checkbox 
#endregion

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain.Translation;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
using VhCstes=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using TableName=TNS.AdExpress.Constantes.Classification.DB.Table.name;



namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Affiche la liste des médias, catégories et support que le client peut sélectionner en fonction de ses droits.
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
		/// Dataset contenant la liste des médias
		/// </summary>
		protected DataSet dsListVehicle;
		/// <summary>
		/// liste des médias(vehicle) dont le clients à accès
		/// </summary>		
		protected string  _vhlist;
		/// <summary>
		/// Indique si la liste doit être rechargée ou non.
		/// </summary>
		protected bool _reload = false;
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
					//Chargement du DataSet des Médias
					dsListVehicle=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleCatMediaListDataAccess(webSession);		
				}
				else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas définie"));
			}
			#endregion
		}
		#endregion

		#region PreRender
		/// <summary>
		/// Construction de la liste de checkbox
		/// </summary>
		/// <param name="e">arguments</param>
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
				
				//Initialisation de la liste d'items
				this.Items.Clear();
				
				//Construction de la liste de checkbox
				this.Items.Add(new System.Web.UI.WebControls.ListItem(GestionWeb.GetWebWord(210,webSession.SiteLanguage),"vh_"+VhCstes.plurimedia.GetHashCode().ToString()));
				foreach(DataRow currentRow in dsListVehicle.Tables[0].Rows) 
				{					
					if ( (IdVehicle = (Int64)currentRow["id_vehicle"]) != oldIdVehicle )
					{							
						oldIdVehicle = IdVehicle;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["vehicle"].ToString(),"vh_"+IdVehicle));
					}
					if ( ((IdCategory  = (Int64)currentRow["id_category"]) != oldCategory ) && (Int64)VhCstes.cinema != (Int64)currentRow["id_vehicle"])
					{	oldCategory = IdCategory ;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["category"].ToString(),"ct_"+(Int64)currentRow["id_category"]));
					}
					if ( (Int64)VhCstes.cinema != (Int64)currentRow["id_vehicle"] && (Int64)VhCstes.internet != (Int64)currentRow["id_vehicle"] && (Int64)VhCstes.press != (Int64)currentRow["id_vehicle"])
					{						
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["media"].ToString(),"md_"+(Int64)currentRow["id_media"]));
					}					
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
			
			if(_reload){
				if( webSession != null) {
					//Chargement du DataSet des Médias
					dsListVehicle=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleCatMediaListDataAccess(webSession);		
				}
				else throw (new WebControlInitializationException("Impossible de recharger le composant, la session n'est pas définie"));
			}


			if(dsListVehicle != null && dsListVehicle.Tables[0].Rows.Count != 0)
			{
				#region variables locales				
				//variables du niveau  Media
				Int64 idVehicleOld=-1;
				Int64 idVehicle;										
				int startVehicle=-1;	
				string vhSeparator="";
				string VehicleIds="";
				string[] VehicleIdsArr=null;
				string 	vhlist="";								
				//variables du niveau Category
				Int64 idCategoryOld=-1;
				Int64 idCategory;										
				int startCategory=-1;										
				//variables du niveau Support
				Int64 i = 0;				
				int numColumn = 0;			
				string checkBox="";						
				#endregion

				//Récupération de la liste des média autorisés pour l'utilisateur courant
				if(webSession.CustomerLogin.getListIdVehicle().Length>0){				
					vhlist=webSession.CustomerLogin.getListIdVehicle();
					VehicleIdsArr = vhlist.Split(',');
					for(int v=0;v<VehicleIdsArr.Length;v++){
						if(v>0)vhSeparator="-";
						VehicleIds+=vhSeparator+"vh_"+VehicleIdsArr[v].ToString();
					}
					if(VehicleIds.Length>0)VehicleIds+="-vh_"+VhCstes.plurimedia.GetHashCode().ToString();
				}
				else  throw (new WebControlInitializationException("Impossible d'initialiser le composant, aucun média n'est accessible."));				

				//Tableau global 
				t.Append("\n<tr vAlign=\"top\" height=\"1%\">\n<td bgColor=\"#ffffff\">\n");	
				t.Append("<a href=\"javascript: ExpandColapseAllDivs('"+vhlist+"')\" ");
				//insertIndex = t.Length;
				t.Append("\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;"+GestionWeb.GetWebWord(1117,webSession.SiteLanguage)+"</a>");	
				
				#region  PluriMedia
				//bordure du haut de tableau
				t.Append("\n<tr><td><div style=\"MARGIN-LEFT: 10px\" id=\"vh_"+VhCstes.plurimedia.GetHashCode().ToString()+"\" >");
				if (idVehicleOld == -1)t.Append("\n<table bgColor=\"#ffffff\" style=\"border-top :#644883 1px solid;  border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n<tr>");
				else t.Append("\n<table bgColor=\"#ffffff\" style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n<tr>");							
				t.Append("\n<td align=\"left\" height=\"10\" valign=\"top\" width=100%>");
				t.Append("\n<input type=checkbox name=\"SectorAnalysisVehicleSelectionWebControl1$"+i+"\" name=\"SectorAnalysisVehicleSelectionWebControl1_"+i+"\" onClick=\"CheckAllChilds('vh_50','"+VehicleIds+"','vh_50')\" value=\"vh_50\">"+GestionWeb.GetWebWord(210,webSession.SiteLanguage)+"");							
				t.Append("\n</td>\n</tr>");		
				t.Append("\n</table></div></td></tr>");				
				i++;
				#endregion

				#region Foreach  Dataset des médias

				foreach(DataRow currentRow in dsListVehicle.Tables[0].Rows)
				{	
					//Initialisation des identifiants parents
					idVehicle=(Int64)currentRow["id_vehicle"];
					idCategory=(Int64)currentRow["id_category"];
					
					#region contruction tableau principal
						//Fermeture Category				
						if (idCategory!= idCategoryOld && startCategory==0 && idVehicle!=9)
						{	//Fermeture support
							if(idVehicle!=7 && idVehicle!=1 && idVehicle!=9)
							{
								if (numColumn!=0)t.Append("</tr>");
							}
							t.Append("</table>");							
							t.Append("</td></tr></table></div></DIV></td></tr>");										
						}

						//Fermeture Media				
						if (idVehicle!= idVehicleOld && startVehicle==0 )
						{
							startCategory=-1;
							t.Append("\n</TD></TR></TABLE></Div>");
							t.Append("\n</td></tr></table></div></td></tr>");
						}
											
						#region Nouveau Media
						//Nouveau Media
						if (idVehicle!= idVehicleOld)
						{							
							//bordure du haut de tableau						
							t.Append("\n<tr><td><div style=\"MARGIN-LEFT: 10px\" id=\"vh_"+idVehicle+"\" >");
							if (idVehicleOld == -1)t.Append("\n<table bgColor=\"#ffffff\" style=\"border-top :#644883 1px solid; border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n");
							else t.Append("\n<table bgColor=\"#ffffff\" style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n");

								//Curseur sur toute la ligne
								if(OpenVehicleDiv((Int64)idVehicle))t.Append("\n<tr  style=\"cursor : hand\">");
								else t.Append("\n<tr>");						
														
							idVehicleOld=idVehicle;
							startVehicle=0;							

							//checkbox média (nouvelle version)
							t.Append("\n<td align=\"left\" height=\"10\" valign=\"top\" >");
							t.Append("\n<input type=checkbox name=\"SectorAnalysisVehicleSelectionWebControl1$"+i+"\" name=\"SectorAnalysisVehicleSelectionWebControl1_"+i+"\" onClick=\"CheckAllChilds('vh_"+idVehicle+"','"+VehicleIds+"','vh_"+idVehicle+"')\" value=vh_"+idVehicle+"></td>");																											

							//fin libellé média (nouvelle version)							
							if(OpenVehicleDiv((Int64)idVehicle)){
								t.Append("<td width=100% align=\"left\" onClick=\"javascript : DivDisplayer('"+idVehicle+"');\" >"+currentRow["vehicle"].ToString()+"");
								t.Append("</td><td align=\"right\" onClick=\"javascript : DivDisplayer('"+idVehicle+"');\">");
								t.Append("<IMG src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\">");
							}else t.Append("<td width=100% align=\"left\" >"+currentRow["vehicle"].ToString()+"");														
							t.Append("</td></tr>");							
							t.Append("<tr><td colspan=\"3\" >");
							t.Append("<Div  style=\"display ='none'\" id=\""+idVehicle+"\">");							
							t.Append("<TABLE bgcolor=\"#B1A3C1\" cellpadding=0 cellspacing=0 width=100% style=\"border-top :#644883 1px solid; border-bottom :#644883 0px solid; border-left :#644883 0px solid; \" class=\"txtViolet11Bold\">");
							if(OpenVehicleDiv((Int64)idVehicle)){	
							t.Append("<tr><td class=\"roll04\"><a href=\"javascript: SelectExclusiveAllChilds('vh_"+idVehicle+"','"+VehicleIds+"','vh_"+idVehicle+"','vh_','ct_')\" title=\""+GestionWeb.GetWebWord(1151,webSession.SiteLanguage)+"\" class=\"roll04\">&nbsp;&nbsp;"+GestionWeb.GetWebWord(1151,webSession.SiteLanguage)+"</td></tr>");
								t.Append("<TR><TD>");	
							}
							i++;	
						}
						#endregion

						#region Nouvelle Categorie
						if (idCategory!=idCategoryOld && showMediaBranchItem(TableName.category,(Int64)idVehicle))
						{	
							numColumn=0;															
							if(startCategory== -1)t.Append("<tr><td ><div style=\"MARGIN-LEFT: 5px; width=100%; background-color:#B1A3C1;\"  id=\"ct_"+idCategory+"\"><table style=\"background-color:#B1A3C1; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							else t.Append("<tr><td><div style=\"MARGIN-LEFT: 5px; width=100%; background-color:#B1A3C1;\"  id=\"ct_"+idCategory+"\"><table style=\"background-color:#B1A3C1; border-top :#644883 1px solid;\" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");

							//Curseur sur toute la ligne
							if(showMediaBranchItem(TableName.media,(Int64)idVehicle)) t.Append("\n<tr  style=\"cursor : hand\">");
							else t.Append("\n<tr>");

							idCategoryOld=idCategory;
							startCategory=0;
							t.Append("<td  align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");
							
							// checkbox choix catégorie (nouvelle version)
							t.Append("<input type=checkbox name=\"SectorAnalysisVehicleSelectionWebControl1$"+i+"\" id=\"SectorAnalysisVehicleSelectionWebControl1_"+i+"\" onClick=\"CheckAllChilds('ct_"+idCategory+"','"+VehicleIds+"','vh_"+idVehicle+"')\" value=\"ct_"+idCategory+"\">");
							t.Append("\n</td>\n");
							//Libellé catégorie
							t.Append("<td width=100% valign=\"middle\" height=\"10\" onClick=\"javascript : DivDisplayer('"+idCategory+"');\" align=\"left\"  ");
							if(showMediaBranchItem(TableName.media,(Int64)idVehicle)){
								t.Append(">&nbsp;"+currentRow["category"].ToString()+"</td>");
								t.Append("<td valign=\"baseline\"  onClick=\"javascript : DivDisplayer('"+idCategory+"');\"   ");
								t.Append(">&nbsp;<IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\" align=\"right\"></td>");
							}
							else t.Append(" width=\"15\" colspan=\"2\" >&nbsp;"+currentRow["category"].ToString()+"</td>");

							t.Append("</tr><tr><td colspan=\"3\"><DIV id=\""+idCategory+"\" ><table cellpadding=0 cellspacing=0 border=\"0\" width=100%>");							
							if(showMediaBranchItem(TableName.media,(Int64)idVehicle))
							{
								t.Append("<table cellpadding=0 cellspacing=0 border=\"0\" width=100% style=\"background-color:#D0C8DA; border-top :#ffffff 1px solid; \"  class=\"txtViolet10\">");	
								t.Append("<tr><td colspan=\"3\" class=\"roll04\" ><a href=\"javascript: SelectExclusiveAllChilds('ct_"+idCategory+"','"+VehicleIds+"','vh_"+idVehicle+"','vh_','ct_')\" title=\""+GestionWeb.GetWebWord(1066,webSession.SiteLanguage)+"\" class=\"roll04\">&nbsp;"+GestionWeb.GetWebWord(1066,webSession.SiteLanguage)+"</a></td></tr>");
							}
							i++;
						}							
						#endregion

						#region Affichage des supports
						if(showMediaBranchItem(TableName.media,(Int64)idVehicle))
						{
							if(numColumn==0)
							{								
								t.Append("<tr >");
								t.Append("<td  width=\"33%\">");								
								t.Append("<input type=\"checkbox\" "+checkBox+" name=\"SectorAnalysisVehicleSelectionWebControl1$"+i+"\" id=\"SectorAnalysisVehicleSelectionWebControl1_"+i+"\" value=\"md_"+currentRow["media"]+"\" onClick=\"CheckAllChilds('md_"+currentRow["id_media"]+"','"+VehicleIds+"','vh_"+idVehicle+"')\">"+currentRow["media"].ToString());
								t.Append("</td>");								
								numColumn++;
								i++;					
							}
							else if(numColumn==1 )
							{
								t.Append("<td  width=\"33%\">");								
								t.Append("<input type=\"checkbox\" "+checkBox+" name=\"SectorAnalysisVehicleSelectionWebControl1$"+i+"\"  id=\"SectorAnalysisVehicleSelectionWebControl1_"+i+"\" value=\"md_"+currentRow["media"]+"\" onClick=\"CheckAllChilds('md_"+currentRow["id_media"]+"','"+VehicleIds+"','vh_"+idVehicle+"')\">"+currentRow["media"].ToString());
								t.Append("</td>");								
								numColumn++;
								i++;
							}
							else 
							{
								t.Append("<td  width=\"33%\">");								
								t.Append("<input type=\"checkbox\" name=\"SectorAnalysisVehicleSelectionWebControl1$"+i+"\" onClick=\"CheckAllChilds('md_"+currentRow["id_media"]+"','"+VehicleIds+"','vh_"+idVehicle+"')\"  value=\"md_"+currentRow["media"]+"\" id=\"SectorAnalysisVehicleSelectionWebControl1_"+i+"\" >"+currentRow["media"].ToString());
								t.Append("</td></tr>");
								numColumn=0;
								i++;
							}
						}
						#endregion
					
					#endregion
					
				}

				#endregion

				//Fermeture Tableau global	
											
				t.Append(" \n\n<tr> \n\n</table></td></tr></table></DIV></div></td></tr>");				
				t.Append(" \n\n</td></tr></table></div></td></tr>");
				t.Append(" </TD></TR></TABLE></Div> \n\n</td>\n</tr>\n ");
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

		#region Methodes internes
		
		/// <summary>
		/// Indique si le niveau catégorie doit être montrer pour le vehicle
		/// </summary>
		/// <param name="mediaBranchItem">item branche média</param>
		/// <param name="idVehicle">identifiant média</param>
		/// <returns>vrai si le média doit être affiché</returns>
		private bool showMediaBranchItem(DBConstantesClassification.Table.name mediaBranchItem, Int64 idVehicle)
		{
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
		/// Indique si le niveau catégorie doit être montrer pour le vehicle
		/// </summary>
		/// <param name="idVehicle">Vehicle à traiter</param>
		/// <returns>True s'il doit être montrer, false sinon</returns>
		private bool showCategory(Int64 idVehicle)
		{
			DBConstantesClassification.Vehicles.names vehicletype=(DBConstantesClassification.Vehicles.names)int.Parse(idVehicle.ToString());
			switch(vehicletype)
			{
				case DBConstantesClassification.Vehicles.names.cinema:
					return(false);
				default:
					return(true);
			}
		}

		/// <summary>
		/// Indique si le niveau média(support) doit être montrer pour le vehicle
		/// </summary>
		/// <param name="idVehicle">Vehicle à traiter</param>
		/// <returns>True s'il doit être montrer, false sinon</returns>
		private bool showMedia(Int64 idVehicle)
		{
			DBConstantesClassification.Vehicles.names vehicletype=(DBConstantesClassification.Vehicles.names)int.Parse(idVehicle.ToString());
			switch(vehicletype)
			{
				case DBConstantesClassification.Vehicles.names.internet:
				case DBConstantesClassification.Vehicles.names.press:
				case DBConstantesClassification.Vehicles.names.internationalPress:
				case DBConstantesClassification.Vehicles.names.cinema:
					return(false);
				default:
					return(true);
			}
		}

		/// <summary>
		/// Indique si le  calque contenant média(vehicle) peut etre ouvert ou fermé
		/// par un clique sur l'image associée 
		/// </summary>
		/// <param name="idVehicle">Vehicle à traiter</param>
		/// <returns>True s'il doit être montrer, false sinon</returns>
		private bool OpenVehicleDiv(Int64 idVehicle)
		{
			DBConstantesClassification.Vehicles.names vehicletype=(DBConstantesClassification.Vehicles.names)int.Parse(idVehicle.ToString());
			switch(vehicletype)
			{
				case DBConstantesClassification.Vehicles.names.cinema:
					return(false);
				default:
					return(true);
			}
		}
		#endregion

		
	}
}
