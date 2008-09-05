#region Informations
// Auteur:
// Création:
// Modification:
//	12/08/2005	Par G.Facon > Nom des méthodes
//	09/02/2006	Par B.Masson & G.Facon > Refonte de l'affichage sous Excel
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Classe pour l'affichage d'un arbre
	/// </summary>
	public class DisplayTreeNode{

		#region Affichage d'un arbre

		#region Cas 1 avec witdhTable en int
		/// <summary>
		///  Affichage d'un arbre au format HTML
		/// </summary>
		/// <param name="root">Arbre à afficher</param>
		/// <param name="write">Arbre en lecture où en écriture</param>
		/// <param name="displayArrow">Affichage de la flêche</param>
		/// <param name="displayCheckbox">Affichage de la checkbox</param>
		/// <param name="witdhTable">Largeur de la table</param>
		/// <param name="displayBorderTable">Affichage de la bordure</param>
		/// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
		/// <param name="SiteLanguage">Langue</param>
		/// <param name="showHideContent">Index ShowHideCOntent ?</param>
		/// <param name="typetree">Type d'arbre ?</param>
		/// <param name="div">Afficher les div true si c'est le cas</param>
		/// <returns>tableau correspondant à l'arbre</returns>
		public static string ToHtml(TreeNode root,bool write,bool displayArrow,bool displayCheckbox,int witdhTable,bool displayBorderTable,bool allSelection,int SiteLanguage,int typetree,int showHideContent,bool div){
			return(ToHtml(root,write,displayArrow,displayCheckbox,witdhTable,displayBorderTable,allSelection,SiteLanguage,typetree,showHideContent,div,false));
		}
		#endregion

		#region Cas 2 avec witdhTable en pourcenatge
		/// <summary>
		///  Affichage d'un arbre au format HTML
		/// </summary>
		/// <param name="root">Arbre à afficher</param>
		/// <param name="write">Arbre en lecture où en écriture</param>
		/// <param name="displayArrow">Affichage de la flêche</param>
		/// <param name="displayCheckbox">Affichage de la checkbox</param>
		/// <param name="witdhTable">Largeur de la table (en pourcentage)</param>
		/// <param name="displayBorderTable">Affichage de la bordure</param>
		/// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
		/// <param name="SiteLanguage">Langue</param>
		/// <param name="showHideContent">Index ShowHideCOntent ?</param>
		/// <param name="typetree">Type d'arbre ?</param>
		/// <param name="div">Afficher les div true si c'est le cas</param>
		/// <returns>tableau correspondant à l'arbre</returns>
		public static string ToHtml(TreeNode root,bool write,bool displayArrow,bool displayCheckbox,string witdhTable,bool displayBorderTable,bool allSelection,int SiteLanguage,int typetree,int showHideContent,bool div){
			return(ToHtml(root,write,displayArrow,displayCheckbox,int.Parse(witdhTable),displayBorderTable,allSelection,SiteLanguage,typetree,showHideContent,div,true));
		}
		#endregion

		#endregion

		

		#region Affichage d'un arbre pour Excel (Nouvelle version)
		/// <summary>
		/// Affichage d'un arbre pour l'export Excel
		/// </summary>
		/// <param name="root">Arbre</param>
        /// <param name="siteLanguage">Site language</param>
		/// <returns>Code HTML</returns>
		public static string ToExcel(TreeNode root, int siteLanguage){
			int maxLevel=0;
			GetNbLevels(root,1,ref maxLevel);
			StringBuilder html = new StringBuilder();
			int nbTD=1;
			TNS.AdExpress.Web.Functions.DisplayTreeNode.ToExcel(root,ref html,0,maxLevel-1,ref nbTD, siteLanguage);
			return(html.ToString());
		}

		/// <summary>
		/// Donne le nombre de niveau d'un arbre
		/// </summary>
		/// <param name="root">Arbre</param>
		/// <param name="level">Niveau de l'arbre</param>
		/// <param name="max">Nombre maximum de niveau</param>
		private static void GetNbLevels(TreeNode root,int level,ref int max){
			if(max<level)max=level;
			foreach(TreeNode currentNode in root.Nodes){
				GetNbLevels(currentNode,level+1,ref max);
			}
		}

		/// <summary>
		/// Donne le nom du style CSS selon le niveau de l'arbre
		/// </summary>
		/// <param name="level">Niveau de l'arbre</param>
        /// <param name="cssNameStyle">Css name style</param>
		/// <returns>Nom du style CSS</returns>
		private static string GetLevelCss(int level,string cssNameStyle){
			switch(level){
				case 1:
                    return ("Level" + cssNameStyle + "1");
				case 2:
                    return ("Level" + cssNameStyle + "2");
				case 3:
                    return ("Level" + cssNameStyle + "3");
				default:
                    return ("Level" + cssNameStyle + "1");
			}
		}

		/// <summary>
		/// Affichage d'un arbre pour l'export Excel
		/// </summary>
		/// <param name="root">Arbre</param>
		/// <param name="html">Code html</param>
		/// <param name="level">Niveau de l'arbre</param>
		/// <param name="maxLevel">Nombre maximum de niveaud e l'arbre</param>
		/// <param name="nbTD">Nombre de cellule TD</param>
        /// <param name="siteLanguage">Site langauge</param>
		/// <returns>True si le nombre maximum de TD a été atteint, sinon false</returns>
		/// <remarks>
		/// - Actuellement, la méthode gère 3 niveaux d'affichage mais elle est générique.
		/// Par conséquent, 3 styles sont définis. Il est possible de rajouter des niveaux de style CSS dans le 'switch case' correspondant
		/// dans la méthode ci-après et ajouter les niveaux dans la méthode GetLevelCss(int level)
		/// - Affichage sur 3 colonnes dans le dernier niveau
		/// </remarks>
		private static bool ToExcel(TreeNode root, ref StringBuilder html, int level, int maxLevel, ref int nbTD, int siteLanguage){

			#region Variables
			string img="";
            string themeName = WebApplicationParameters.Themes[siteLanguage].Name;
			#endregion

			#region Checkbox
			// Non cocher
			if(!root.Checked) img="<img src=/App_Themes/"+themeName+"/Images/Common/checkbox_not_checked.GIF>";
			// Cocher
            //else if(root.Checked) img="<img src=/Images/Common/checkbox.GIF>";
            else if (root.Checked) img = "<img src=/App_Themes/"+themeName+"/Images/Common/checkbox.GIF>";
			#endregion

			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
				html.Append("<td class="+GetLevelCss(level,"")+" >"+img+"&nbsp;&nbsp;&nbsp;&nbsp;"+((LevelInformation)root.Tag).Text+"</td>");	
			}
			else{
				// Ajout d'une cellule TD, valable pour n'importe quel niveau de l'arbre (affichage du noeud)
				if(level!=0)html.Append("<tr class=\"BorderLevel\"><td colspan=4 class="+GetLevelCss(level,"")+" >"+img+"&nbsp;&nbsp;&nbsp;&nbsp;"+((LevelInformation)root.Tag).Text+"</td></tr>");
				// On prépare l'affichage du dernier niveau de l'arbre (nouvelle ligne) si on affiche le père d'une feuille
				if(level==maxLevel-1 && root.Nodes.Count>0)
					html.Append("<tr>");
			}
			// Boucle sur chaque noeud de l'arbre
			foreach(TreeNode currentNode in root.Nodes){
				// Si le niveau inférieur indique qu'il faut changer de ligne et que la demande n'a pas été faite par le dernier fils
				if(ToExcel(currentNode,ref html,level+1,maxLevel,ref nbTD,siteLanguage) && currentNode!=root.LastNode){
					html.Append("</tr><tr>");
				}
			}
			//On est dans le niveau père des feuilles et il a des fils on fait les bordures
			if(level==maxLevel-1 && root.Nodes.Count>0){
				if(nbTD!=1){
					// On ajoute des cellules vides (td avec colspan)pour avoir le bon nombre de cellules (TD)
					html.Append("<td class="+GetLevelCss(level+1,"")+" colspan="+(((int)(4-nbTD)).ToString())+">&nbsp;</td>");
					// Bordure de droite
                    html.Append("<td class=\"" + GetLevelCss(level + 1, "RightBorder") + "\">&nbsp;</td></tr>");
				}
				// Bordure en bas et bordure à droite
                html.Append("<tr><td colspan=4 class=\"" + GetLevelCss(level + 1, "RightBottomBorder") + "\" >&nbsp;</td></tr>");
				nbTD=1;
			}
			// Si on est dans le dernier niveau de l'arbre
			if(level==maxLevel){ 
				// On test si on est dans la dernière colonne, 
				// On affiche une cellule vide pour faire la bordure de droite
				// On prépare le changement de ligne commander au niveau supperieur par le return true
				if(nbTD==3){
					nbTD=1;
                    html.Append("<td class=\"" + GetLevelCss(level, "RightBorder") + "\" >&nbsp;</td></tr>");
					return(true);
				}
				nbTD++;
			}
			// On indique au niveau suppérieur que l'on ne doit pas changer de ligne
			return(false);
		}
		#endregion

		#region Affichage d'un arbre pour Excel (Ancienne version)
		/// <summary>
		/// Affichage d'un arbre pour l'export Excel
		/// </summary>
		/// <param name="root">Arbre à afficher</param>		
		/// <param name="SiteLanguage">Langue</param>	
		/// <param name="displayCheckbox">True si l'on souhaite afficher la checkbox</param>
		/// <returns>tableau correspondant à l'arbre</returns>
		public	static string ToExcel(TreeNode root,int SiteLanguage,bool displayCheckbox){

			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
            string themeName = WebApplicationParameters.Themes[SiteLanguage].Name;
			//int nbElement=0;
			string treeNode="";
			int i=0;
			int start=0;
			int colonne=0;			
			string tmp="";	
			string classCss="";
			int nbrColumn=0;
			const string violet="txtViolet11Bold";
			classCss=violet;

			foreach(TreeNode currentNode in root.Nodes){				
				if(start==0){
                    t.Append("<table class=\"detailSelectionH\" cellpadding=0 cellspacing=0 >");
					start=1;
				}
				else{
                    t.Append("<table class=\"detailSelectionH\" cellpadding=0 cellspacing=0 >");
				}
				t.Append("<tr>");
				t.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");		
				
				//Non cocher
				if(displayCheckbox && !currentNode.Checked){
                    t.Append("<img src=/App_Themes/" + themeName + "/Images/Common/checkbox_not_checked.GIF>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
				}
				//cocher
				else if(displayCheckbox && currentNode.Checked){
                    t.Append("<img src=/App_Themes/" + themeName + "/Images/Common/checkbox.GIF>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
				}
				t.Append(""+((LevelInformation)currentNode.Tag).Text+"");
				t.Append("</td>");
				
				if(currentNode.Nodes.Count>0){
					if(currentNode.Nodes.Count==2){
						nbrColumn=1;
						t.Append("<td colspan=\""+nbrColumn+"\">&nbsp;</td>");
					}
					else if (currentNode.Nodes.Count>2){
						nbrColumn=2;
						t.Append("<td colspan=\""+nbrColumn+"\">&nbsp;</td>");
					}
					t.Append("</tr>");
					t.Append("</table>");

                    t.Append("<table class=\"detailSelection\" width=100%>");
				}
				
				colonne=0;
				i=0;
				while(i<currentNode.Nodes.Count){
					//Non cocher
					if(!currentNode.Nodes[i].Checked){
                        tmp = "<img src=/App_Themes/" + themeName + "/Images/Common/checkbox_not_checked.GIF>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br>";
					}
					//En lecture et cocher
					else if(currentNode.Nodes[i].Checked){
                        tmp = "<img src=/App_Themes/" + themeName + "/Images/Common/checkbox.GIF>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br>";
							
					}else {
						tmp="";
					}
					if(colonne==2){
						t.Append("<td class=\""+classCss+"\">");
						t.Append(tmp);
						t.Append("</td>");
						colonne=1;
					}
					else if(colonne==1){
						t.Append("<td class=\""+classCss+"\">");
						t.Append(tmp);
						t.Append("</td>");
						t.Append("</tr>");
						colonne=0;
					}
					else{
						t.Append("<tr>");
						t.Append("<td class=\""+classCss+"\">");
						t.Append(tmp);
						t.Append("</td>");
						colonne=2;
					}
					i++;
				}
				t.Append("</table>");
			}
			treeNode=t.ToString();
			return treeNode;
		}
		#endregion

		#region Script
		/// <summary>
		/// Script
		/// </summary>
		/// <returns>Code JavaScript</returns>
		public	static string AddScript(){
			string script;	

			script=" <script language=\"JavaScript\">";
			script+=" var selectionned=0; "; 
			script+=" var ancien; ";
			script+=" var ancien1; ";
			script+=" var ancien2;";
			
			script+=" function allSelection2(tt){ ";			
			script+=" var aInput=document.Form2.all;	";		
			script+="var nbrBox; "; 
			script+=" if(aInput.item(\"\"+tt+\"\").length!=null){  ";
			script+= "   nbrBox=aInput.item(\"\"+tt+\"\").length;} ";
			script+=" else{ nbrBox=1;}";
			script+=" for(i=0 ;i<nbrBox;i++){  ";			
			script+= " if(aInput.item(\"\"+tt+\"\",i).checked==false){";				
			script+=" selectionned=1;	";			
			script+=" break; }";
			script+=" else{selectionned=0; ";
			script+=" } ";
			script+="	} ";	
			script+=" if(selectionned==1 ){ ";
			script+=" for(i=0 ;i<nbrBox;i++){ ";				
			script+=" aInput.item(\"\"+tt+\"\",i).checked=true;	";
			script+="	} ";
			script+=" selectionned=0; ";	
			script+=" } ";
			script+=" else{ ";
			script+=" for(i=0 ;i<nbrBox;i++){ ";				
			script+=" aInput.item(\"\"+tt+\"\",i).checked=false; ";							
			script+=" } ";
			script+=" selectionned=1; ";		
			script+=" } ";
			script+=" } ";




			script+=" function integration2(tt){";
			script+=" var aInput=document.Form2.all;	";		
			script+="var nbrBox; "; 
			script+=" if(aInput.item(\"\"+tt+\"\").length!=null){  ";
			script+= "   nbrBox=aInput.item(\"\"+tt+\"\").length;} ";
			script+=" else{ nbrBox=1;}";
			script+=" if(document.activeElement.checked==true){ ";			
			script+=" for(i=0 ;i<nbrBox;i++){ ";
			
			script+=" aInput.item(\"\"+tt+\"\",i).checked=true; ";
			script+="} ";
			script+="} ";			
			script+="}";


			script+=" function showHideContent(id)";
			script+=" { ";
			script+=" var oContent1 = document.all.item(id+\"Content1\"); ";
			script+=" if (ancien1!=null){ ";
			script+="	if (id+\"Content1\"==ancien1 && ouvert1==true){";
			script+=" var oAncien1=document.all.item(ancien1); ";
			script+=" oAncien1.style.display=\"none\"; ";
			script+=" ouvert1=false; ";
			script+=" return; ";
			script+=" } ";
			script+=" var oAncien1=document.all.item(ancien1); ";
			script+=" oAncien1.style.display=\"none\"; ";
			script+=" } ";
			script+=" ancien1=id+\"Content1\"; ";
			script+=" oContent1.style.display = \"\"; ";
			script+=" ouvert1=true; ";
			script+=" } ";

				
			
			script+=" function showHideContent2(id)";
			script+=" { ";
			script+=" var oContent1 = document.all.item(id+\"Content2\"); ";
			script+=" if (ancien2!=null){ ";
			script+="	if (id+\"Content2\"==ancien2 && ouvert1==true){";
			script+=" var oAncien1=document.all.item(ancien2); ";
			script+=" oAncien1.style.display=\"none\"; ";
			script+=" ouvert1=false; ";
			script+=" return; ";
			script+=" } ";
			script+=" var oAncien1=document.all.item(ancien2); ";
			script+=" oAncien1.style.display=\"none\"; ";
			script+=" } ";
			script+=" ancien2=id+\"Content2\"; ";
			script+=" oContent1.style.display = \"\"; ";
			script+=" ouvert1=true; ";
			script+=" } ";
			
			
			script+=" </script> ";
			return script;
		}
		#endregion

		#region Méthode interne

		#region Affichage d'un arbre
		/// <summary>
		///  Affichage d'un arbre au format HTML
		/// </summary>
		/// <param name="root">Arbre à afficher</param>
		/// <param name="write">Arbre en lecture où en écriture</param>
		/// <param name="displayArrow">Affichage de la flêche</param>
		/// <param name="displayCheckbox">Affichage de la checkbox</param>
		/// <param name="witdhTable">Largeur de la table</param>
		/// <param name="displayBorderTable">Affichage de la bordure</param>
		/// <param name="allSelection">Affichage du lien "tout sélectionner"</param>
		/// <param name="SiteLanguage">Langue</param>
		/// <param name="showHideContent">Index ShowHideCOntent ?</param>
		/// <param name="typetree">Type d'arbre ?</param>
		/// <param name="div">Afficher les div true si c'est le cas</param>
		/// <param name="percentage">Pour indiquer si la valeur de witdhTable est en pourcentage ou pas</param>
		/// <returns>tableau correspondant à l'arbre</returns>
		private static string ToHtml(TreeNode root,bool write,bool displayArrow,bool displayCheckbox,int witdhTable,bool displayBorderTable,bool allSelection,int SiteLanguage,int typetree,int showHideContent,bool div,bool percentage){
		
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
            string themeName = WebApplicationParameters.Themes[SiteLanguage].Name;
			//int nbElement=0;
			string treeNode="", percentageSymbol="";
			int i=0;
			int start=0;
			int colonne=0;
			string buttonAutomaticChecked="";
			string disabled="";
			string tmp="";
			int j=0;

			if(percentage)
				percentageSymbol="%";

			foreach(TreeNode currentNode in root.Nodes){
				if(start==0){
					if(displayBorderTable){
                        t.Append("<table class=\"txtViolet11Bold violetBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + "  >");
						start=1;
					}
					else{
                        t.Append("<table class=\"txtViolet11Bold BlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
					}

				}
				else{
					if(displayBorderTable){
                        t.Append("<table class=\"txtViolet11Bold violetBorderWithoutTop\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
					}
					else{
                        t.Append("<table class=\"txtViolet11Bold BlancBorder\"  cellpadding=0 cellspacing=0 width=" + witdhTable + percentageSymbol + ">");
					}
				}
				t.Append("<tr>");
				t.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");		
				if(displayCheckbox){
					//En lecture et Non cocher
					if(!write && !currentNode.Checked){
						disabled="disabled";
						buttonAutomaticChecked="";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){ 
							t.Append("<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1$"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">");
							j++;
						}
						else{
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
							j++;
						}
					}
						//En lecture et cocher
					else if(!write && currentNode.Checked){
						disabled="disabled";
						buttonAutomaticChecked="checked";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
							j++;
						}
						else{
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
							j++;
						}
					}
						//En Ecriture et Non cocher
					else if(write && !currentNode.Checked){
						disabled="";
						buttonAutomaticChecked="";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){
							disabled="disabled";
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
							j++;
						}
						else{
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
							j++;
						}
					}
						//En Ecriture et cocher
					else{
						disabled="";
						buttonAutomaticChecked="checked";
						if(typetree==2){
							t.Append("<input type=\"checkbox\"  "+disabled+" "+buttonAutomaticChecked+"  ID=\""+((LevelInformation)currentNode.Tag).ID+"\" value=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\" name=\"AUTOMATIC_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">");
						}
						else if(typetree==3){
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
							j++;
						}
						else{
                            t.Append("<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " onclick=\"integration2('" + ((LevelInformation)currentNode.Tag).ID + "'," + j + ")\" ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">");
							j++;
						}
					}
				    t.Append("</label>");
				}
				else{t.Append("&nbsp;");}
				t.Append(""+((LevelInformation)currentNode.Tag).Text+"");
				t.Append("</td>");
				if(displayArrow && currentNode.Nodes.Count>0){
					if(showHideContent==1){
						t.Append("<td width=\"100%\" align=right onClick=\"showHideContent1('"+((LevelInformation)currentNode.Tag).ID+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
					}
					else if(showHideContent==2){
						t.Append("<td width=\"100%\" align=right onClick=\"showHideContent2('"+((LevelInformation)currentNode.Tag).ID+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
					}
					else if(showHideContent==3){
						t.Append("<td width=\"100%\" align=right onClick=\"showHideContent3('"+((LevelInformation)currentNode.Tag).ID+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
					}
					else if(showHideContent==4){
						t.Append("<td width=\"100%\" align=right onClick=\"showHideContent4('"+((LevelInformation)currentNode.Tag).ID+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
					}
					else if(showHideContent==5){
						t.Append("<td width=\"100%\" align=right onClick=\"showHideContent5('"+((LevelInformation)currentNode.Tag).ID+"');\" style=\"cursor : hand\"><IMG height=\"15\" src=\"/App_Themes/" + themeName + "/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
					}
				}
				else{
					t.Append("<td width=\"15\"></td>");
				}
				t.Append("</tr>");
				t.Append("</table>");
				if(currentNode.Nodes.Count>0){
					if(displayBorderTable){
						if(div){
                            t.Append("<div id=\"" + ((LevelInformation)currentNode.Tag).ID + "Content" + showHideContent + "\" class=\"BlancBorderColorWithoutTop\"  style=\"DISPLAY: none; WIDTH: 100%\">");
						}
                        t.Append("<table class=\"violetBorderWithoutTop lightPurple\" width=" + witdhTable + percentageSymbol + ">");
					}
					else{
						if(div){
                            t.Append("<div class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
						}
                        t.Append("<table class=\"BlancBorder lightPurple\" width=" + witdhTable + percentageSymbol + ">");
					}
				}
				//Cas où l'on veut mettre le lien tout sélectionner
				if(allSelection){
					int code=817;
					bool parentChecked=true;
					// cas advertiser
					if(((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException || ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess){
						code=817;		
					}
					// cas Marque
					if(((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandException || ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.brandAccess){
						code=817;		
					}
						// cas holding company
					else if(((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException || ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess){
						code=816;
					}
						// cas Sector
					else if(((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException || ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess){
						code=968;
					}
					else if(((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException || ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess){
						code=969;
					}

					if(typetree==1){
						t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('"+((LevelInformation)currentNode.Tag).ID+"',"+j+")\" ID=\""+((LevelInformation)currentNode.Tag).ID+"\">"+GestionWeb.GetWebWord(code,SiteLanguage)+"</a></td></tr>");
					}
					else if(typetree==2){
						t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection2('"+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"')\" ID=\""+((LevelInformation)currentNode.Tag).ID+"\">"+GestionWeb.GetWebWord(code,SiteLanguage)+"</a></td></tr>");
					}
				
					if(((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException
						|| ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.advertiserException
						|| ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.sectorException
						|| ((LevelInformation)currentNode.Tag).Type==TNS.AdExpress.Constantes.Customer.Right.type.subSectorException
						){
						parentChecked=false;
					}


					if(typetree==3 && !parentChecked){
						t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('"+((LevelInformation)currentNode.Tag).ID+"',"+j+")\" ID=\""+((LevelInformation)currentNode.Tag).ID+"\">"+GestionWeb.GetWebWord(code,SiteLanguage)+"</a></td></tr>");
					}
			
				}
				colonne=0;
				i=0;
				while(i<currentNode.Nodes.Count){
					
					if(displayArrow){
						//En lecture et Non cocher
						if(!write && !currentNode.Nodes[i].Checked){
							disabled="disabled";
							buttonAutomaticChecked="";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else{
                                tmp = "<input type=\"checkbox\" " + disabled + " " + buttonAutomaticChecked + " value=" + ((LevelInformation)currentNode.Tag).ID + " ID=\"AdvertiserSelectionWebControl1_" + j + "\" name=\"AdvertiserSelectionWebControl1$" + j + "\"><label for=\"AdvertiserSelectionWebControl1_" + j + "\">" + ((LevelInformation)currentNode.Nodes[i].Tag).Text + "<br></label>";
								j++;
							}
						}
							//En lecture et cocher
						else if(!write && currentNode.Nodes[i].Checked){
							disabled="disabled";
							buttonAutomaticChecked="checked";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else{
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1$"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
						
						}
							//En Ecriture et Non cocher
						else if(write && !currentNode.Nodes[i].Checked){
							disabled="";
							buttonAutomaticChecked="";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else if(typetree==3){
								disabled="disabled";
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1$"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
							else{
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1$"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
						
						}
						else{
							disabled="";
							buttonAutomaticChecked="checked";
							if(typetree==2){
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" ID=\""+((LevelInformation)currentNode.Tag).ID+((LevelInformation)currentNode.Tag).Text+"\"  value=\""+((LevelInformation)currentNode.Nodes[i].Tag).ID+"_"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"\" name=\"CKB_"+((LevelInformation)currentNode.Tag).ID+"_"+((LevelInformation)currentNode.Tag).Text+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br>";
							}
							else{
								tmp="<input type=\"checkbox\" "+disabled+" "+buttonAutomaticChecked+" value="+((LevelInformation)currentNode.Tag).ID+" ID=\"AdvertiserSelectionWebControl1_"+j+"\" name=\"AdvertiserSelectionWebControl1$"+j+"\"><label for=\"AdvertiserSelectionWebControl1_"+j+"\">"+((LevelInformation)currentNode.Nodes[i].Tag).Text+"<br></label>";
								j++;
							}
						}
					}
					else{t.Append("&nbsp;");}

					if(colonne==2){
								
						t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");						
						t.Append(tmp);						
						t.Append("</td>");
						colonne=1;
					}
					else if(colonne==1){
						t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");								
						t.Append(tmp);
						t.Append("</td>");
						t.Append("</tr>");
						colonne=0;
					}
					else{
						t.Append("<tr>");
						t.Append("<td style=\"white-space: nowrap\" class=\"txtViolet10\" width=33%>");
						t.Append(tmp);
						t.Append("</td>");								
						colonne=2;
					}

					//	t.Append(((LevelInformation)currentNode.Nodes[i].Tag).Text);
					i++;
				}	
				if(currentNode.Nodes.Count>0){
					if(colonne!=0){
						if(colonne==2){
							t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
							t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
						}
						else if(colonne==1){
							t.Append("<td class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
						}
						t.Append("</tr>");
					}
					t.Append("</table>");
					if(div){
						t.Append("</div>");				
					}
				}

			}

			treeNode=t.ToString();
			return treeNode;
		}
		#endregion

		#endregion


	}
}
