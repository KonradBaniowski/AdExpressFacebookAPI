#region Informations
// Auteur: Y. R'kaina
// Date de cr�ation: 28/11/2006
// Date de modification:
#endregion

using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Windows.Forms;
using TNS.AdExpress.Web.DataAccess.Selections.Programs;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.DataAccess;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Composant affichant la liste des annonceurs
	/// Il est utilis� dans la s�lection de l'annonceur ainsi que dans la sous s�lection
	/// au niveau des pages de r�sultats.
	/// Suivant la valeur de l'entier buttonTarget des actions sont r�alis�es.
	/// 
	/// </summary>
	
	[ToolboxData("<{0}:ProgramTypeWebControl runat=server></{0}:ProgramTypeWebControl>")]
	public class ProgramTypeWebControl: System.Web.UI.WebControls.CheckBoxList {

		#region Variables
	     
		/// <summary>
		/// Dataset contenant la liste des genres d'�missions
		/// </summary>
		protected DataSet dsListProgramType;
		/// <summary>
		/// Session
		/// </summary>
		protected WebSession webSession;
		/// <summary>
		/// Text saisie pour le choix de la recherhce
		/// </summary>
		protected string programTypeText;
		/// <summary>
		/// Tableau avec la liste des genres d'�missions en acc�s
		/// </summary>
		protected string[] tabProgramTypeAccess;
		/// <summary>
		/// Tableau avec la liste des genres d'�missions en Exception
		/// </summary>
		protected string[] tabProgramTypeException;
		/// <summary>
		/// Tableau avec la liste des �missions en acc�s
		/// </summary>
		protected string[] tabProgramAccess;
		/// <summary>
		/// Tableau avec la liste des �missions en Exception
		/// </summary>
		protected string[] tabProgramException;
		/// <summary>
		/// Entier qui d�termine qu'elle boutton 
		/// a �t� cliquer 1 pour valider
		/// 2 pour ok
		/// </summary>
		int eventButton;
		/// <summary>
		/// Message d'erreur
		/// </summary>
		string ErrorMessage="";
		/// <summary>
		/// si nbElement=2 un liste contient plus de 1000 �l�ments
		/// si nbElement=3 aucun �l�ment n'a �t� s�l�ctionn�e
		/// </summary>
		int nbElement=1;
		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public ProgramTypeWebControl():base() {

			this.EnableViewState=true;
		}
		#endregion

		#region Accesseurs
		
		/// <summary>
		/// Obtient le dataset avec la liste des genres d'�missions 
		/// </summary>
		public DataSet DsListProgramType {
			get{return dsListProgramType;}
		}

		/// <summary>
		/// Obtient ou d�finit la webSession 
		/// </summary>
		public WebSession WebSession {
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// Obtient ou d�finit le text saisie au niveau du genre d'�mission
		/// </summary>
		public string ProgramTypeText {
			get{return programTypeText;}
			set{programTypeText=value;}
		}

		/// <summary>
		/// Obtient ou d�finit la valeur de la case � cocher groupe
		/// </summary>
		public int NbElement {
			get{return nbElement;}
			set{nbElement=value;}
		}

		/// <summary>
		/// Obtient ou d�finit la valeur du boutton qui a �t�
		/// cliqu�
		/// </summary>
		public int EventButton {
			get{return eventButton;}
			set{eventButton=value;}
		}
		
		#endregion

		#region Chargement de la page
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e) {
		
			#region variables
			string listProgramType="";
			string listProgramTypeAccess="";
			string listProgramTypeException="";
			string listProgramTypeAutomatic="";

			string listProgram="";
			string listProgramAccess="";
			string listProgramException="";
			string listProgramAutomatic="";	
				
			bool parentBool=false;
			#endregion

			if(eventButton==-1) {
			//if(eventButton==constEvent.eventSelection.INITIALIZE_EVENT) {
				Items.Clear();
			}
	
			#region !IsPostBack
			if((!Page.IsPostBack) || (eventButton==-1)) {
			//if((!Page.IsPostBack) || (eventButton==constEvent.eventSelection.INITIALIZE_EVENT)) {

				dsListProgramType= ProgramTypeListDataAccess.GetProgramTypeListDataAccess(webSession);
			}
			#endregion

			#region Bouton Ok
			if(eventButton==2 || eventButton==3) {	
			//if(eventButton==constEvent.eventSelection.OK_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT) {	
				if(Page.IsPostBack) {

					#region PostBack

					#region Cr�ation des listes
					foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items) {

						string[] tabParent=currentItem.Text.Split('_') ;

						if(tabParent[0]=="Parent" && currentItem.Selected) {

							parentBool=true;
							listProgramTypeAccess+=currentItem.Value+",";
							listProgramTypeAutomatic+=currentItem.Value+",";
							
						}	
						else if(tabParent[0]=="Parent" && !currentItem.Selected) {
							parentBool=false;
						}

						else if(tabParent[0]=="Children" && parentBool) {

							if(!currentItem.Selected) {
								listProgramException+=currentItem.Value+",";
							}
						}
						else if(tabParent[0]=="Children" && !parentBool) {

							if(currentItem.Selected) {
								listProgramAccess+=currentItem.Value+",";
							}
						}
					}
					#endregion
				
					#region Initialisation des tableaux
					tabProgramTypeAccess=null;
					tabProgramTypeException=null;
					tabProgramAccess=null;	
					tabProgramException=null;
					#endregion

					#region Cr�ation des listes et des tableaux
					//Liste Program Type
					if(listProgramTypeAccess.Length!=0 && listProgramTypeAutomatic.Length!=0) {
						listProgramType+=listProgramTypeAccess;	
						tabProgramTypeAccess=listProgramTypeAccess.Split(',');
					}
					else if(listProgramTypeAccess.Length!=0) {
						listProgramType+=listProgramTypeAccess;
						tabProgramTypeAccess=listProgramTypeAccess.Split(',');
					}
			 			
					if(listProgramTypeException.Length!=0) {
						listProgramType+=listProgramTypeAutomatic;
						tabProgramTypeException=listProgramTypeException.Split(',');
				
					}

					//Liste Program
					if(listProgramAccess.Length!=0 && listProgramAutomatic.Length!=0) {
						listProgram+=listProgramAccess;
						tabProgramAccess=listProgramAccess.Split(',');
					}
					else if(listProgramAccess.Length!=0) {
						listProgram+=listProgramAccess;
						tabProgramAccess=listProgramAccess.Split(',');
					}

					if(listProgramException.Length!=0) {
						listProgram+=listProgramAutomatic;	
						tabProgramException=listProgramException.Split(',');
					}

					//Suppression de la derni�re ,
					if(listProgramType.Length>0) {
						listProgramType=listProgramType.Substring(0,listProgramType.Length-1);
					}
					if(listProgram.Length>0) {
						listProgram=listProgram.Substring(0,listProgram.Length-1);
					}
					
					#endregion

					// Ev�nment sauvegarder
					if(eventButton==3) {
					//if(eventButton==constEvent.eventSelection.SAVE_EVENT) {
						webSession.SelectionUniversProgramType=this.createTreeNode();
						webSession.Save();
					}
					
					//if(eventButton==constEvent.eventSelection.OK_EVENT) {
					if(eventButton==2) {

						if(programTypeText.Length <2 ||  !TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(programTypeText)) {
							programTypeText="";
						}

						// V�rifie que les listes soient inf�rieur � 1000 �l�ments
						if( listProgramType.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER 
							&& listProgram.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER) {
							// Requ�te pour r�cup�rer la liste des �l�ments
							if((programTypeText.Length >=2 && TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(programTypeText)) 
								|| (listProgramType.Length!=0 || listProgram.Length!=0 )) {
							
								dsListProgramType = GenericClassificationSearchDataAccess.GetSearchResultListDataAccess(webSession,programTypeText,listProgramType,listProgram,DetailLevelItemInformation.Levels.program,DetailLevelItemInformation.Levels.programType);
							}
						}
							// Message d'erreur : Vous devez s�lectionner moins de 1000 �l�ments
						else {
                            ErrorMessage = GestionWeb.GetWebWord(2265, webSession.SiteLanguage);
						}
					}
					#endregion
				}
			}
			#endregion

			#region Bouton Valider ou Recall
			if(eventButton==1 || eventButton==5 || eventButton==-2 || eventButton==6) {
			//if(eventButton==constEvent.eventSelection.VALID_EVENT || eventButton==5 || eventButton==-2 || eventButton==6) {
				webSession.SelectionUniversProgramType=this.createTreeNode();
				webSession.Save();
			}

			if(eventButton==-2) {
				this.Items.Clear();
			}

			#endregion

			#region Bouton Valider
			// Bouton valider dans la sousSelection
			if(eventButton==7) {
				webSession.CurrentUniversAdvertiser=this.createTreeNode();
				if(webSession.CurrentUniversAdvertiser.Nodes.Count>0)
					webSession.Save();
				this.createItems();
			}
			#endregion

			#region Script
            // fermer/ouvrir tous les calques  
           
            if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs", TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
            }
            // Ouverture/fermeture des fen�tres p�res
            if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.DivDisplayer());
            }                     

            #endregion

            base.OnLoad (e);
		}
		#endregion

		#region PreRender
		/// <summary>
		/// PreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e) {
			
			#region Ajout des items � partir de dsListProgramType
			
			Int64 idParentOld=-1;
			Int64 idParent;
			// Suppression des checkbox contenues dans la liste
			this.Items.Clear();

			#region 2 niveaux
			// Ajout des checkboxs dans la liste
			
			if(dsListProgramType!=null) {	
				foreach(DataRow currentRow in dsListProgramType.Tables[0].Rows) {
	
					idParent=(Int64)currentRow[0];
					if(idParentOld!=idParent) {
						this.Items.Add(new System.Web.UI.WebControls.ListItem("Parent_"+currentRow[1].ToString(),currentRow[0].ToString()));
						idParentOld=idParent;
					}					
					this.Items.Add(new System.Web.UI.WebControls.ListItem("Children_"+currentRow[3].ToString(),currentRow[2].ToString()));					
				}
			}
			
			#endregion

			#endregion

			#region Ajout des items � partir de l'arbre SelectionUniversProgramType
			// Chargement de l'univers
			// Ajout des �l�ments dans listItem
			// Ex�cuter lors des �v�nmements suivant: 4 : bouton charger
			// 6 : bouton ok de la popup 8 : Retour � la s�lection initiale
			// 9 : lors d'un chargement dans la page de r�sultat
			if(eventButton==4 || eventButton==3 || eventButton==8 || eventButton==9) {
			//if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT || eventButton==8 || eventButton==9) {
				//	if(buttonTarget==4){
				// Suppression des checkbox contenues dans la liste
				this.Items.Clear();
				int i=0;

                foreach (System.Windows.Forms.TreeNode currentNode in webSession.SelectionUniversProgramType.Nodes)
                {					
					//System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem("Parent_"+((LevelInformation)currentNode.Tag).Text,((LevelInformation)currentNode.Tag).ID.ToString());
					System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
					checkBox1.Text="Parent_"+((LevelInformation)currentNode.Tag).Text;
					checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
					this.Items.Add(checkBox1);
					i=0;
					while(i<currentNode.Nodes.Count) {
						System.Web.UI.WebControls.ListItem checkBox2 =new System.Web.UI.WebControls.ListItem();
						checkBox2.Text="Children_"+((LevelInformation)currentNode.Nodes[i].Tag).Text;
						checkBox2.Value=((LevelInformation)currentNode.Nodes[i].Tag).ID.ToString();
						this.Items.Add(checkBox2);
						i++;
					}
				}
			}
			#endregion
		}
		#endregion
		
		#region CreateItems
		/// <summary>
		/// M�thode pour cr�er la liste d'item � partir de l'arbre CurrentUniversAdvertiser
		/// </summary>
		protected void createItems() {
		
			this.Items.Clear();
			int i=0;

            foreach (System.Windows.Forms.TreeNode currentNode in webSession.CurrentUniversAdvertiser.Nodes)
            {					
				//System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem("Parent_"+((LevelInformation)currentNode.Tag).Text,((LevelInformation)currentNode.Tag).ID.ToString());
				System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
				checkBox1.Text="Parent_"+((LevelInformation)currentNode.Tag).Text;
				checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
				this.Items.Add(checkBox1);
				i=0;
				while(i<currentNode.Nodes.Count) {
					System.Web.UI.WebControls.ListItem checkBox2 =new System.Web.UI.WebControls.ListItem();
					checkBox2.Text="Children_"+((LevelInformation)currentNode.Nodes[i].Tag).Text;
					checkBox2.Value=((LevelInformation)currentNode.Nodes[i].Tag).ID.ToString();
					this.Items.Add(checkBox2);
					i++;
				}
			}
		}
		#endregion

		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output) {
			// Num�ro du checkBox
			int i=0;
			int length=50000;
			int insertIndex = 0;
			string 	vhList="";
			string textOpenclose="";
			if(dsListProgramType!=null)length=dsListProgramType.Tables[0].Rows.Count*500;
			System.Text.StringBuilder t=new System.Text.StringBuilder(length);
			
			if((eventButton==2)||(eventButton==-1)||!(Page.IsPostBack)) {
			//if((eventButton==constEvent.eventSelection.OK_EVENT)||(eventButton==constEvent.eventSelection.INITIALIZE_EVENT)||!(Page.IsPostBack)) {
			
				#region Affichage

                string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
		
				if(dsListProgramType!=null) {

					if(dsListProgramType.Tables[0].Rows.Count>0) {	
						t.Append("<tr class=\"txtGris11Bold\">");
                        t.Append("<td class=\"backGroundWhite\"  >");
						t.Append("&nbsp;"+GestionWeb.GetWebWord(812,webSession.SiteLanguage)+"</td></tr>");
					}
				}
                t.Append("<tr><td class=\"backGroundWhite\"><img src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"1\" height=\"20\"></td></tr> ");

				textOpenclose = GestionWeb.GetWebWord(2054,webSession.SiteLanguage);

				if(dsListProgramType!=null) {
					if(dsListProgramType.Tables[0].Rows.Count>0) {

                        t.Append("<tr class=\"backGroundWhite\"> ");
                        t.Append("<td class=\"backGroundWhite\"> ");
						t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
						insertIndex = t.Length;
						t.Append("')\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;"+textOpenclose+"</a>");	
						t.Append("</td> ");
						t.Append("</tr> ");
					}
				}

				#endregion

				int nbLinesTest=0;
			
				//Tableau global
                t.Append("<tr><td class=\"backGroundWhite\" vAlign=\"top\"><table width=\"100%\" >");			
				t.Append("<tr><td vAlign=\"top\">");

				Int64 idParentOld=-1;
				Int64 idParent;				
				string textParent;
				string textParentOld;
				int automatic=0;
		
				int start=-1;
				string checkBox="";	

				#region  2 niveaux
			
				if(dsListProgramType!=null)	{

					foreach(DataRow currentRow in dsListProgramType.Tables[0].Rows) {								
							
						#region Foreach
		
						idParent=(Int64)currentRow[0];
						textParent=currentRow[1].ToString();
						checkBox="";

						#region Parcours des tableaux
						// Parcours du tableau pour v�rifier si l'�l�ment doit �tre cocher
						if(tabProgramTypeAccess!=null) {
							foreach(string item1  in tabProgramTypeAccess) {
								if(item1==currentRow[0].ToString()) {
									checkBox="checked";
									break;
								}
							}
						}
						#endregion
			
						if(idParentOld!=idParent && start==0) {																	
							if(nbLinesTest!=0) {
								if (nbLinesTest==2){
									t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
									t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								}
								else if(nbLinesTest==1){
									t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								}
								t.Append("</tr>");
								nbLinesTest=0;
							}
			
							if(checkBox=="checked") {
								automatic=1;
							}
							else {
								automatic=0;
							}
		
							t.Append("</table>");
							t.Append("</div>");
                            t.Append("<table class=\"txtViolet11Bold violetBorderWithoutTop\"  cellpadding=0 cellspacing=0 width=100%>");
							t.Append("<tr>");
							t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
							t.Append("<input type=\"checkbox\" "+checkBox+" onclick=\"integration('"+idParent+"',"+i+")\" ID=\"AdvertiserSelectionWebControl1_"+i+"\" name=\"AdvertiserSelectionWebControl1$"+i+"\">");
							t.Append("<label for=\"AdvertiserSelectionWebControl1_"+i+"\">");
							t.Append(""+currentRow[1].ToString()+"");
							t.Append("</label>");
							t.Append("</td>");
                            t.Append("<td width=\"100%\" align=right onClick=\"DivDisplayer('" + idParent + "Ct');\" style=\"cursor : hand\"><img height=\"15\" src=\"/App_Themes/" + themeName + "/Images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
							t.Append("</tr>");
							t.Append("</table>");
                            t.Append("<div id=\"" + idParent + "Ct\" class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\">");
                            t.Append("<table id=" + idParent + " class=\"violetBorderWithoutTop lightPurple\" width=100%>");
							t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('"+idParent+"',"+i+")\" ID=\""+currentRow[0]+"\">");
						
							t.Append(GestionWeb.GetWebWord(2055,webSession.SiteLanguage));
						
			
							t.Append("</a></td></tr>");
							vhList=vhList+(Int64)currentRow[0]+"Ct"+",";
							idParentOld=idParent;
							textParentOld=textParent;
							i++;
						}
						//Premier						
		
						if(idParentOld!=idParent && start!=0) {

							if(checkBox=="checked") {
								automatic=1;
							}
							else {
								automatic=0;
							}
							t.Append("<table cellpadding=0 cellspacing=0   width=670px>");
							t.Append("<tr><td></td></tr>");
							t.Append("</table>");
                            t.Append("<table class=\"txtViolet11Bold violetBorder\" cellpadding=0 cellspacing=0   width=100%>");
							t.Append("<tr>");
							t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
							t.Append("<input type=\"checkbox\" "+checkBox+" onclick=\"integration('"+idParent+"',"+i+")\" ID=\"AdvertiserSelectionWebControl1_"+i+"\" name=\"AdvertiserSelectionWebControl1$"+i+"\">");
							t.Append("<label for=\"AdvertiserSelectionWebControl1_"+i+"\">");
							t.Append(""+currentRow[1].ToString()+"");
							t.Append("</label>");
							t.Append("</td>");
                            t.Append("<td width=\"100%\" align=right onClick=\"DivDisplayer('" + idParent + "Ct');\" style=\"cursor : hand\"><img height=\"15\" src=\"/App_Themes/" + themeName + "/Images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");	
							t.Append("</tr>");
							t.Append("</table>");
							t.Append("<div id=\"" + idParent + "Ct\" class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: none; WIDTH: 100%\" >");
                            t.Append("<table id=" + idParent + " class=\"violetBorderWithoutTop paleVioletBackGround\" width=100%>");
							t.Append("<tr><td colspan=\"3\"><a href=# class=\"roll04\" style=\"TEXT-DECORATION: none\" onclick=\"allSelection('"+idParent+"',"+i+")\" ID=\""+currentRow[0]+"\">");
							t.Append(GestionWeb.GetWebWord(2055,webSession.SiteLanguage));
							t.Append("</a></td></tr>");	

							vhList=vhList+(Int64)currentRow[0]+"Ct"+",";
							idParentOld=idParent;
							textParentOld=textParent;
							start=0;
							i++;
						}

						#region Parcours des tableaux
						checkBox="";
				
						if(automatic==0) {	
							if(tabProgramAccess!=null) {
								foreach(string item1  in tabProgramAccess) {
									if(item1==currentRow[2].ToString()) {
										checkBox="checked";
										break;
									}
								}
							}
						}

						if(automatic==1) {
							checkBox="checked";	 

							if(tabProgramException!=null) {
								foreach(string item1  in tabProgramException) {
									if(item1==currentRow[2].ToString()) {
										checkBox="";
										break;
									}
								}
							}
						}
						#endregion
		
						#region Affichage des fils
						if(nbLinesTest==2) {								

							t.Append("<td class=\"txtViolet10\" width=\"33%\">");
							t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value="+idParent+" name=\"AdvertiserSelectionWebControl1$"+i+"\"><label style=\"white-space: nowrap\" for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[3].ToString()+"<br></label>");
							t.Append("</td>");
							nbLinesTest=1;
							i++;
		
						}
						else if(nbLinesTest==1) {
							t.Append("<td class=\"txtViolet10\" width=\"33%\">");								
							t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\"  value="+idParent+"  name=\"AdvertiserSelectionWebControl1$"+i+"\"><label style=\"white-space: nowrap\" for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[3].ToString()+"<br></label>");
							t.Append("</td>");								
							t.Append("</tr>");
							nbLinesTest=0;
							i++;	
		
						}
						else {
							t.Append("<tr>");
							t.Append("<td class=\"txtViolet10\" width=\"33%\">");
							t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value="+idParent+" name=\"AdvertiserSelectionWebControl1$"+i+"\"><label style=\"white-space: nowrap\" for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[3].ToString()+"<br></label>");
							t.Append("</td>");								
							nbLinesTest=2;
							i++;
						}
						#endregion

						#endregion
					}
		
					if(dsListProgramType.Tables[0].Rows.Count!=0) {
						if(nbLinesTest!=0) {
							
							if (nbLinesTest==2) {
								t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
								t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
							}
							else if(nbLinesTest==1) {
								t.Append("<td align=\"center\" class=\"txtViolet10\" width=\"33%\">&nbsp;</td>");
							}

							t.Append("</tr>");
							nbLinesTest=0;
						}
						else {

							nbLinesTest=0;
						}
						t.Append("</table></div>");
					}
				}
			
				#endregion				

				if(programTypeText.Length<2 && eventButton==2) {							
				//if(programTypeText.Length<2 && eventButton==constEvent.eventSelection.OK_EVENT) {
					// Saississez 2 caract�res au minimum
                    t.Append("<tr><td class=\"txtGris11Bold backGroundWhite\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(1473,webSession.SiteLanguage)+" "+programTypeText+".</p> ");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}	
				else if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(programTypeText) && eventButton==2) {
				//else if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(programTypeText) && eventButton==constEvent.eventSelection.OK_EVENT) {
                    t.Append("<tr><td class=\"txtGris11Bold backGroundWhite\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(820,webSession.SiteLanguage)+" "+programTypeText+".</p> ");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}	
				else if(dsListProgramType!=null) {
					if(dsListProgramType.Tables[0].Rows.Count==0) {
                        t.Append("<tr><td class=\"txtGris11Bold backGroundWhite\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
						t.Append(" "+GestionWeb.GetWebWord(819,webSession.SiteLanguage)+" "+programTypeText+".</p> ");
						t.Append(" </td> ");
						t.Append(" </tr> ");
					}
				}
				else if(ErrorMessage.Length>0) {
                    t.Append("<tr><td class=\"txtGris11Bold backGroundWhite\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(ErrorMessage);
					t.Append("</td></tr>");
				}			
				//Fin du tableau global
				t.Append("</td></tr></table></td></tr>");
                t.Append("<tr><td class=\"backGroundWhite\"><IMG height=\"20\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"1\"></td></tr>");
				t.Append("<tr><td></td></tr>");	

				if (vhList!="") {

					vhList = vhList.Remove(vhList.Length-1, 1);
					t.Insert(insertIndex, vhList);
				}
				output.Write(t.ToString());		
			}		
								
			#region Affichache des Annonceurs � partir d'un arbre
			if(eventButton==4  || eventButton==7 || eventButton==3 || eventButton==8 || eventButton==9) {
			//if(eventButton==constEvent.eventSelection.LOAD_EVENT  || eventButton==7 || eventButton==constEvent.eventSelection.SAVE_EVENT || eventButton==8 || eventButton==9) {
				if(eventButton==4 || eventButton==3) {
				//if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT) {
					t.Append("<tr class=\"txtGris11Bold\">");
                    t.Append("<td class=\"backGroundWhite\"  >");
					t.Append(""+GestionWeb.GetWebWord(812,webSession.SiteLanguage)+"</td></tr>");
				}

                t.Append("<tr height=5px class=\"backGroundWhite\"><td></td></tr>");
                t.Append("<tr><td class=\"backGroundWhite\" valign=\"top\">");
                t.Append("<table width=\"100%\" class=\"backGroundWhite\">");				
				t.Append("<tr> ");
				
				t.Append("</td></tr><tr><td>");	
				// Affichage des genres d'�missions � partir de SelectionUniversProgramType
				if(eventButton==4 || eventButton==3 || eventButton==8) {
				//if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT || eventButton==8) {
                    t.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSession.SelectionUniversProgramType, true, true, true, "100", true, true, webSession.SiteLanguage, 3, 1, true, webSession.DataLanguage, webSession.CustomerDataFilters.DataSource));
				}
				t.Append("</td></tr></table></td></tr>");				
				output.Write(t.ToString());	
			}
			#endregion

		}
		#endregion
		
		#region cr�ation de l'arbre
		/// <summary>
		/// Cr�ation de l'arbre � partir de la liste des Items
		/// </summary>
		/// <returns></returns>
        protected System.Windows.Forms.TreeNode createTreeNode()
        {

			System.Windows.Forms.TreeNode programTypeTree=new System.Windows.Forms.TreeNode();
			
			System.Windows.Forms.TreeNode tmpNode=null;
			System.Windows.Forms.TreeNode tmpParentNode=null;
			// Cas diff�rent du bouton valider de la sous s�lection
			if(eventButton!=7) {
				programTypeTree= ((System.Windows.Forms.TreeNode)webSession.SelectionUniversProgramType);
			}

			else {
				//programTypeTree= ((System.Windows.Forms.TreeNode)webSession.CurrentUniversAdvertiser);
			}
			int compteurParent=0;
			int compteurChild=0;
			if(eventButton!=7) {
				if(webSession.SelectionUniversProgramType.FirstNode!=null) {
					webSession.SelectionUniversProgramType.Nodes.Clear();
				}
			}
			else {
				//				if(webSession.CurrentUniversAdvertiser.FirstNode!=null)
				//				{
				//					webSession.CurrentUniversAdvertiser.Nodes.Clear();
				//				}
			}

			#region 2 niveaux
			int integration=0;
			int nbrChild=1;
			foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items) {

				#region foreach
			
				string[] tabParent=currentItem.Text.Split('_') ;
			
				// cas Int�gration automatique
				if(tabParent[0]=="Parent" && currentItem.Selected) {
				
					if(nbrChild==0) {
						programTypeTree.LastNode.Remove();
					}
					nbrChild=1;
					tmpParentNode=new System.Windows.Forms.TreeNode(tabParent[1]);
					tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.programTypeAccess,Int64.Parse(currentItem.Value),tabParent[1]);
					tmpParentNode.Checked=true;
					integration=1;
					programTypeTree.Nodes.Add(tmpParentNode);
					compteurParent++;

				}
				// Cas Non int�gration automatique
				if(tabParent[0]=="Parent" &&  !currentItem.Selected) {

					if(nbrChild==0) {
						programTypeTree.LastNode.Remove();
					}
					nbrChild=0;
					tmpParentNode=new System.Windows.Forms.TreeNode(tabParent[1]);
					tmpParentNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.programTypeException,Int64.Parse(currentItem.Value),tabParent[1]);
					tmpParentNode.Checked=false;
					integration=2;
				
					programTypeTree.Nodes.Add(tmpParentNode);
					//compteurParent++;
				}
		
				if(tabParent[0]=="Children" && integration==1 && !currentItem.Selected) {

					tmpNode=new System.Windows.Forms.TreeNode(tabParent[1]);
					tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.programException,Int64.Parse(currentItem.Value),tabParent[1]);
					tmpNode.Checked=false;
					nbrChild++;
					tmpParentNode.Nodes.Add(tmpNode);
					compteurChild++;
			
				}

				if(tabParent[0]=="Children" &&  integration ==2 && currentItem.Selected) {

					tmpNode=new System.Windows.Forms.TreeNode(tabParent[1]);
					tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.programAccess,Int64.Parse(currentItem.Value),tabParent[1]);
					tmpNode.Checked=true;
					nbrChild++;
					tmpParentNode.Nodes.Add(tmpNode);
					compteurChild++;
				}

				#endregion
			}

			if(programTypeTree.LastNode!=null) {	
				if(programTypeTree.LastNode.FirstNode==null && integration!=1) {
					programTypeTree.LastNode.Remove();
				}
			}
			#endregion

			
			// Fournit une valeur � nbElement pour v�rifier la validit� du nombre 
			// d'�l�ments s�lectionn�
			if(compteurChild > TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER || compteurParent>TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER) {
				nbElement=2;
			}
			else if (compteurChild==0 && compteurParent==0) {
				nbElement=3;  
			}

			return programTypeTree;
		}
		#endregion

	}
}
