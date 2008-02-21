#region Informations
// Auteurs: D. Mussuma & G. Ragneau
// Date de création: 09/09/2004
// Date de modification: 14/09/2004
#endregion

using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.Collections;

using System.Collections.Generic;
using TNS.AdExpress.Web.DataAccess.Selections.Products;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Composant affichant la liste des variétés
	/// Il est utilisé dans la sélection de groupes de variétés	
	/// en fonction des droits client.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:SegmentSelectionWebControl runat=server></{0}:SegmentSelectionWebControl>")]
	public class RecapProductSelectionWebControl : System.Web.UI.WebControls.CheckBoxList
	{
		#region Variables		     
		/// <summary>
		/// Dataset contenant la liste des variétés
		/// </summary>
		protected DataSet dsListSegment;
		/// <summary>
		/// Session
		/// </summary>
		protected WebSession webSession;										
		/// <summary>
		/// Indique si on charge l'arbre SelectionUniversProduct présent dans la session ou non
		/// </summary>
		public bool LoadSession = false;
		#endregion
	
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public RecapProductSelectionWebControl():base()
		{
			this.EnableViewState=true;
		}
		#endregion

		#region Accesseurs
		
		/// <summary>
		/// Obtient le dataset avec la liste des variétés 
		/// </summary>
		public DataSet DsListSegment
		{
			get{return dsListSegment;}
		}

		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		public virtual WebSession WebSession
		{
			get{return webSession;}
			set{webSession=value;}
		}						
				
		#endregion

		#region Evènements

		#region OnLoad
		/// <summary>
		/// Extraction des données de la base
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e) {
			dsListSegment=TNS.AdExpress.Web.DataAccess.Selections.Products.RecapProductClassificationListDataAccess.RecapGetDataForSectorAnalysis(webSession);
		}

		#endregion
		
		#region PreRender
		/// <summary>
		/// Construction de la liste de checkbox
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {
			if (!Page.IsPostBack){
				//Variables locales
				Int64 idGroup = 0;
				Int64 oldIdGroup = 0;

				//Initialisation de la liste d'items
				this.Items.Clear();

				//Construction de la liste de checkbox
				foreach(DataRow currentRow in dsListSegment.Tables[0].Rows) {
					if ( (idGroup = (Int64)currentRow["id_group_"]) != oldIdGroup ){
						oldIdGroup = idGroup;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["group_"].ToString(),"gp_"+idGroup));
					}
					this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["segment"].ToString(),"sg_"+(Int64)currentRow["id_segment"]));
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

			#region Construction du code HTML
			if(dsListSegment != null && dsListSegment.Tables[0].Rows.Count != 0)
			{
				#region variables locales
				//Hastable de variétés
				//Hashtable segments = null;
				List<long> includeSegments = null,excludeSegments=null;
				Int64 i = 0;
				//int indexGp = -1;

				//variables du niveau  famille
				Int64 idSectorOld=-1;
				Int64 idSector;										
				int startSector=-1;		
				string SectorIds = "";	
				int insertIndex = 0;
			
				//variables du niveau classe
				Int64 idSubSectorOld=-1;
				Int64 idSubSector;										
				int startSubSector=-1;	
			
				//variables du niveau groupe
				Int64 idGroupOld=-1;
				Int64 idGroup;										
				int startGroup=-1;	
				//Chargement de la liste des groupes dans l'arbre SelectionUniversProduct si nécessaire (post-enregistrement) et de leur index dans l'arbre
				//Hashtable groups = null;
				List<long> includeGroups = null;
				if (LoadSession){
					List<NomenclatureElementsGroup> nElmtGr = null;
					//groups = LoadChilds(webSession.SelectionUniversProduct);
					if (webSession.PrincipalProductUniverses.Count > 0 ) {
						nElmtGr = webSession.PrincipalProductUniverses[0].GetIncludes();
						if (nElmtGr != null && nElmtGr.Count > 0) {
							includeGroups = nElmtGr[0].Get(TNSClassificationLevels.GROUP_);
							includeSegments = nElmtGr[0].Get(TNSClassificationLevels.SEGMENT);
						}
						nElmtGr = webSession.PrincipalProductUniverses[0].GetExludes();
						if (nElmtGr != null && nElmtGr.Count > 0) {
							excludeSegments = nElmtGr[0].Get(TNSClassificationLevels.SEGMENT);
						}
					}					
				}

				//variables du niveau variétés
				int numColumn = 0;
				string checkBox="";
				string checkGp="";
				#endregion

				#region Debut Tableau global 
				t.Append("<tr vAlign=\"top\" height=\"100%\" align=\"center\"><td bgColor=\"#ffffff\"><table  vAlign=\"top\">");		
				t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
				insertIndex = t.Length;
				t.Append("')\" class=\"roll04\" >"+GestionWeb.GetWebWord(1084,webSession.SiteLanguage)+"</a>");
				t.Append("<tr><td vAlign=\"top\">");
				#endregion

				#region Foreach  Dataset des variétés
				foreach(DataRow currentRow in dsListSegment.Tables[0].Rows)
				{	
					//Initialisation des identifiants parents
					idSector=(Int64)currentRow["id_sector"];
					idSubSector=(Int64)currentRow["id_subsector"];
					idGroup=(Int64)currentRow["id_group_"];
								
					#region Fermeture Groupe
					if (idGroup!= idGroupOld && startGroup==0)
					{			
						if (numColumn!=0)
						{
							t.Append("</tr>");
						}
						t.Append("</table><tr height=\"5\"><td></td></tr></div></td></tr></table></td></tr>");
					}
					#endregion

					#region Fermeture Classe
					if (idSubSector!= idSubSectorOld && startSubSector==0)
					{
						startGroup=-1;
						t.Append("</table></div></td></tr></table></td></tr>");
					}
					#endregion 
					
					#region Fermeture famille
					if (idSector!= idSectorOld && startSector==0)
					{
						startSubSector=-1;
						t.Append("</table></div></td></tr></table>");
					}
					#endregion

					#region Nouvelle famille
					if (idSector!= idSectorOld)
					{
						//bordure du haut de tableau
						if (idSectorOld == -1)t.Append("<table style=\"border-top :#644883 1px solid; border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\"><tr onClick=\"javascript : DivDisplayer('sc_"+idSector+"');\" style=\"cursor : hand\">");
						else t.Append("<table style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\"><tr onClick=\"javascript : DivDisplayer('sc_"+idSector+"');\" style=\"cursor : hand\">");
						idSectorOld=idSector;
						startSector=0;
						t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">&nbsp;&nbsp;&nbsp;"+currentRow["sector"].ToString());
						t.Append("</td>");
						t.Append("<td align=\"right\"><IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
						t.Append("</tr><tr><td colspan=\"2\"><div style=\"MARGIN-LEFT: 0px; background-color:#B1A3C1;display ='none';\" id=\"sc_"+idSector+"\"><table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
						//lien tous selectionner
						t.Append("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllChilds('sc_"+idSector+"')\" title=\""+GestionWeb.GetWebWord(1082,webSession.SiteLanguage)+"\" class=\"roll04\">"+GestionWeb.GetWebWord(1082,webSession.SiteLanguage)+"</a></td></tr>");
						SectorIds+="sc_"+idSector+",";
					}
					#endregion

					#region Nouvelle Classe
					if (idSubSector!= idSubSectorOld)
					{
						//bordure du haut de tableau#
						if (startSubSector == -1)t.Append("<tr><td ><table style=\"background-color:#B1A3C1; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('sb_"+idSubSector+"');\" style=\"cursor : hand\">");
						else t.Append("<tr><td ><table style=\"background-color:#B1A3C1; border-top :#644883 1px solid;\" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\"><tr onClick=\"javascript : DivDisplayer('sb_"+idSubSector+"');\" style=\"cursor : hand\">");
						idSubSectorOld=idSubSector;
						startSubSector=0;
						t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");
						t.Append("&nbsp;&nbsp;"+currentRow["SubSector"].ToString()+"</td>");
						t.Append("<td align=\"right\"><IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
						t.Append("</tr><tr><td colspan=\"2\"><DIV style=\"background-color:#D0C8DA; MARGIN-LEFT: 5px\" id=\"sb_"+idSubSector+"\"><table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
						//lien tous selectionner
						t.Append("<tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllChilds('sb_"+idSubSector+"')\" title=\""+GestionWeb.GetWebWord(1083,webSession.SiteLanguage)+"\" class=\"roll04\">"+GestionWeb.GetWebWord(1083,webSession.SiteLanguage)+"</a></td></tr>");
					}
					#endregion

					#region Nouveau Group
					if (idGroup!= idGroupOld)
					{
						numColumn = 0;
						//bordure du haut de tableau#
						t.Append("<tr><td ><table style=\"background-color:#D0C8DA; border-top :#ffffff 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"643\"><tr width=100%>");
						idGroupOld=idGroup;
						startGroup=0;
						t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
						t.Append("<input id=\"recapProductSelectionWebControl_"+i+"\" name=\"recapProductSelectionWebControl$"+i+"\"");
			
						#region Etat de cochage du groupe	
						try{
							#region Ancienne version
							//checkGp = checkBox = "";
							////Vérification de l'état du groupe (sélectionner ou non)
							//if(webSession.SelectionUniversProduct.Nodes[(int)groups[idGroup]].Checked){
							//    checkGp = checkBox = " Checked ";
							//}
							////Construction de la liste de ses fils present dans l arbre
							//indexGp = (int)groups[idGroup];
							//segments = LoadChilds(webSession.SelectionUniversProduct.Nodes[(int)groups[idGroup]]);
							#endregion

							//Vérification de l'état du groupe (sélectionner ou non)
							checkGp = checkBox = "";
							if (includeGroups != null && includeGroups.Contains(idGroup)) {
								checkGp = checkBox = " Checked ";
							}
						}
						//Exceptions si le groupe n'est pas trouver 
						catch(System.NullReferenceException){
							//segments = null;
						}
						#endregion

						t.Append(" onclick=\"javascript:GroupIntegration('gp_"+idGroup+"')\" type=\"checkbox\" "+checkBox+"   value=\"gp_"+idGroup+"\">"+currentRow["Group_"].ToString()+"</td>");
						t.Append("<td align=\"right\" width=100% onClick=\"javascript : DivDisplayer('gp_"+idGroup+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");
						t.Append("</tr><tr><td colspan=\"2\"><DIV style=\"MARGIN-LEFT: 0px\" id=\"gp_"+idGroup+"\"><table cellpadding=0 cellspacing=0 border=\"0\" bgColor=\"#E1E0DA\" width=\"100%\"><tr><td colspan=\"2\">&nbsp;<a href=\"javascript: SelectAllChilds('gp_"+idGroup+"')\" class=\"roll04\"  >"+GestionWeb.GetWebWord(1067,webSession.SiteLanguage)+"</a></td></tr>");
						i++;
					}
					#endregion

					#region Variété (fils de group)
					
					#region Gestion de l'etat de cochage de la variété
					//si le groupe est dans la liste de groupe 
					try{
						checkBox = "";
						#region Ancienne version
						////Vérification de l'état de la variété (sélectionner ou non)
						//if(webSession.SelectionUniversProduct.Nodes[indexGp].Nodes[(int)segments[(Int64)currentRow["id_segment"]]].Checked){
						//    checkBox = " Checked ";
						//}
						#endregion

						//Vérification de l'état de la variété (sélectionner ou non)
						if ((includeSegments != null && includeSegments.Contains(long.Parse(currentRow["id_segment"].ToString())))
							|| (includeGroups != null && includeGroups.Contains(idGroup) && (excludeSegments == null || !excludeSegments.Contains(long.Parse(currentRow["id_segment"].ToString()))))) {
							checkBox = " Checked ";
						}
					}
					//Exceptions si le groupe n'est pas trouver dans l'arbre
					catch(System.ArgumentOutOfRangeException) {if (checkGp.Length>0)checkBox=" Checked ";}
					catch(System.NullReferenceException){if (checkGp.Length>0)checkBox=" Checked ";}
					#endregion
					
					if(numColumn==0)
					{								
						t.Append("<tr>");
						t.Append("<td class=\"txtViolet10\" width=\"33%\">");								
						t.Append("<input type=\"checkbox\" "+checkBox+" name=\"recapProductSelectionWebControl$"+i+"\" id=\"recapProductSelectionWebControl_"+i+"\" value=sg_"+currentRow["id_segment"]+">"+currentRow["segment"].ToString());
						t.Append("</td>");								
						numColumn++;
						i++;
					
					}
					else if(numColumn==1 )
					{
						t.Append("<td class=\"txtViolet10\" width=\"33%\">");								
						t.Append("<input type=\"checkbox\" "+checkBox+" name=\"recapProductSelectionWebControl$"+i+"\" id=\"recapProductSelectionWebControl_"+i+"\" value=sg_"+currentRow["id_segment"]+">"+currentRow["segment"].ToString());
						t.Append("</td>");								
						numColumn++;
						i++;
					}
					else 
					{
						t.Append("<td class=\"txtViolet10\" width=\"33%\">");								
						t.Append("<input type=\"checkbox\" "+checkBox+" name=\"recapProductSelectionWebControl$"+i+"\" id=\"recapProductSelectionWebControl_"+i+"\" value=sg_"+currentRow["id_segment"]+">"+currentRow["segment"].ToString());
						t.Append("</td></tr>");
						numColumn=0;
						i++;
					}
					#endregion

				}
				#endregion

				#region Fermeture Tableau global
				if (numColumn!=0)
				{
					t.Append("</tr>");
				}
				t.Append("</table></div></td></tr></table></td></tr>      </table></div></td></tr></table></td></tr>         </table></div></td></tr></table></td></tr></table></td></tr>");			
				SectorIds = SectorIds.Remove(SectorIds.Length-1, 1);
				t.Insert(insertIndex, SectorIds);
				#endregion
			}
			else
			{
				t.Append("<tr><td bgcolor=\"#ffffff\" class=\"txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" "+GestionWeb.GetWebWord(1081,webSession.SiteLanguage)+"</p> ");
				t.Append(" </td> ");
				t.Append(" </tr> ");
			}
			#endregion

			output.Write(t.ToString());

		}
		#endregion

		#region Construction d'une liste des enfants d'un noeuds
		//private Hashtable LoadChilds(System.Windows.Forms.TreeNode tree)
		//{
		//    Hashtable t = new Hashtable();
		//    foreach(System.Windows.Forms.TreeNode node in tree.Nodes){
		//        t.Add(((LevelInformation)node.Tag).ID,node.Index);
		//    }
		//    return t;
		//}
		#endregion

		#endregion

		
		
	}
}
