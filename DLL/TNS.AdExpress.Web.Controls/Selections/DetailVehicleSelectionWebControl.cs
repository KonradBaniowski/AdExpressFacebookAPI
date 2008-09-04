#region Informations
// Auteur: A. OBERMEYER 
// Date de création: 10/09/2004 
// Date de modification: 15 /09/2004 
// Date de modification: 07/06/2005 
// K. Shehzad:option open/close all
// 29/11/2005 Par B.Masson > Ligne 225 Correction Bug pour la recherche par mot clef
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Classification;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Affiche la liste des categories/media d'un vehicle que peut sélectionner un client en fonction de ses droits.
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:DetailVehicleSelectionWebControl runat=server></{0}:DetailVehicleSelectionWebControl>")]
	public class DetailVehicleSelectionWebControl : System.Web.UI.WebControls.CheckBoxList{

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession webSession=null; 
		/// <summary>
		/// Dataset contenant la liste des médias
		/// </summary>
		protected DataSet dsListMedia;
		/// <summary>
		/// Evènement qui a été lancé
		/// </summary>
		protected int eventButton;
		/// <summary>
		/// Mot clé
		/// </summary>
		protected string keyWord="";
		/// <summary>
		/// Liste des médias sélectionnés
		/// </summary>
		protected string[] tabListMedia=null;
//		protected string[] tabListMedia={"1"};
		/// <summary>
		/// Liste des médias déjà sélectionnés
		/// </summary>
		protected string[] tabListMediaCompetitor=null;
//		protected string[] tabListMediaCompetitor={"2"};
		/// <summary>
		/// valeur qui permet de savoir si des éléments ont
		/// été sélectionnés
		/// </summary>
		protected int nbElement=-1;
		/// <summary>
		///  Vérifie si le chargement est possible
		/// </summary>
		protected bool loadIsPossible=true;
		/// <summary>
		/// Vérifie si les supports n'ont pas déjà été sélectionner
		/// </summary>
		protected bool saveValue=true;
//		/// <summary>
//		/// Vérifie si on est sur la page next
//		/// </summary>
		//protected bool nextLabel=false;		
		#endregion

		#region Cosntantes
		const int MAX_SELECTABLE_ELEMENTS = 200;
		#endregion

		#region constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public DetailVehicleSelectionWebControl():base(){
			this.EnableViewState=true;
		}
		#endregion
	
		#region Accesseurs
		/// <summary>
		/// Définit la session à utiliser
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set{webSession = value;}
		}
		
		/// <summary>
		/// Définit l'évènement qui a été lancer
		/// </summary>
		public virtual int EventButton {
			set{eventButton = value;}
		}
		
		/// <summary>
		/// Obtient ou définit la valeur qui permet de savoir si des éléments ont
		/// été sélectionnés
		/// </summary>
		public int NbElement{
			get{return nbElement;}
			set{nbElement=value;}
		}

		/// <summary>
		/// Obtient ou définit le text saisie au niveau de l'annonceur
		/// </summary>
		public string KeyWord{
			get{return keyWord;}
			set{keyWord=value;}
		}
		#endregion

		#region propriétés
		/// <summary>
		/// boolean si listeMedia est vide
		/// </summary>
		private static bool isEmptyList=false;
		/// <summary>
		/// boolean si les onpeu chargé un univer 
		/// </summary>
		private static bool loadData=true;

		/// <summary> 
		/// Obtient ListeMedia est vide
		/// </summary>
		[Bindable(true),
		Description("bool de pour la liste vide")]
		public static bool IsEmptyList{
			get{return isEmptyList;}
			set{isEmptyList=value;}
		}

		/// <summary> 
		/// obtient si on peux ou non chargé un univer
		/// </summary>
		[Bindable(true),
		Description("bool de pour l'univer")]
		public static bool LoadData{
			get{return loadData;}
		}
		
		#endregion

		#region Evènements
		
		#region Chargement 
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e">Evènement</param>
		protected override void OnLoad(EventArgs e) {
		
			#region Variables
			string listAccessMedia="";
			string listAccessMediaDisable="";
//			bool nextLabel=true;			
			#endregion

			#region Création des listes
			foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items){
				string[] tabParent=currentItem.Text.Split('_') ;
				if(tabParent[0]=="Children"){
					if(currentItem.Selected){					
						listAccessMedia+=currentItem.Value+",";						
					}	
				}					
			}
			

			int j=1;
			while(j<=webSession.CompetitorUniversMedia.Count){
                System.Windows.Forms.TreeNode competitorMedia = (System.Windows.Forms.TreeNode)webSession.CompetitorUniversMedia[j];
                foreach (System.Windows.Forms.TreeNode item2 in competitorMedia.Nodes)
                {
					listAccessMediaDisable+=((LevelInformation)item2.Tag).ID.ToString()+",";
				}			
				j++;
			}
			


			//Média sélectionnées			
			if(listAccessMedia.Length>0){
				tabListMedia=listAccessMedia.Split(',');	

				if(listAccessMediaDisable.Length>0){					
					tabListMediaCompetitor=listAccessMediaDisable.Split(',');				
				}
			}

			
		

			// Vérifie que les éléments n'ont pas déjà été sélectionner
			if(tabListMedia!=null && tabListMedia.Length>0){
				foreach(string item1 in tabListMedia){
					if(tabListMediaCompetitor!=null && tabListMediaCompetitor.Length>0){
						foreach(string item2 in tabListMediaCompetitor){
							if(item1==item2 && item1.Length>0 && item2.Length>0){
								saveValue=false;
								break;
							}
						}
					}
					if(!saveValue)break;
				}
			}

			if(!saveValue){
				nbElement=constEvent.error.MEDIA_SELECTED_ALREADY;
			}

			if(saveValue){
				listAccessMedia+=listAccessMediaDisable;
			}

			if(listAccessMedia.Length>0){
				listAccessMedia=listAccessMedia.Substring(0,listAccessMedia.Length-1);//suppression de la dernière virgule
			}
			
			#endregion
			
			// Suppression des espaces
			keyWord=keyWord.Trim();		

			// listAccessMedia
			IList listMediaSelected=null;
			listMediaSelected=listAccessMedia.Split(',');
			
			
						
			#region Creation du dataSet	
		
			if (listMediaSelected.Count >= MAX_SELECTABLE_ELEMENTS && eventButton == constEvent.eventSelection.OK_EVENT)
				nbElement=constEvent.error.MAX_ELEMENTS;

			dsListMedia = GetData(eventButton,keyWord,listAccessMedia);
			if(Page.IsPostBack && eventButton!=constEvent.eventSelection.INITIALIZE_EVENT &&  eventButton!=constEvent.eventSelection.ALL_INITIALIZE_EVENT){
				if(eventButton==constEvent.eventSelection.OK_OPTION_MEDIA_EVENT 
					|| (eventButton==constEvent.eventSelection.OK_OPTION_MEDIA_EVENT && (keyWord.Length >=2 || keyWord.Length ==0 ||  TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(keyWord)))
					)
					loadData=true;
			}
			#endregion

			#region chargement d'un univers
			if(eventButton==constEvent.eventSelection.LOAD_EVENT){
				// Vérifie si l'on peut charger l'univers
				loadIsPossible=this.verifLoadIsOk();
				if(!loadIsPossible){
					nbElement=constEvent.error.LOAD_NOT_POSSIBLE;
					loadData=false;
				}
			}			
			#endregion

			#region Création de l'arbre
			if(saveValue){
				if(eventButton==constEvent.eventSelection.NEXT_EVENT || eventButton==constEvent.eventSelection.VALID_EVENT || 
					eventButton==constEvent.eventSelection.SAVE_EVENT){
					webSession.CurrentUniversMedia=this.createTreeNode();
					webSession.Save();
				}
			}
			#endregion
		 
			// fermer/ouvrir tous les calques
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs",TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
			}
			// Ouverture/fermeture des fenêtres pères
			if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
			}
			// Initialisation des items
			if(eventButton==constEvent.eventSelection.INITIALIZE_EVENT || eventButton==constEvent.eventSelection.ALL_INITIALIZE_EVENT){
				tabListMedia=null;
				if(eventButton==constEvent.eventSelection.ALL_INITIALIZE_EVENT) {
					webSession.CompetitorUniversMedia.Clear();
					webSession.Save();
					loadData=true;
				}
			}

//			// listAccessMedia
//			IList listMediaSelected=null;
//			listMediaSelected=listAccessMedia.Split(',');			
//			

			#region Ajout des items à partir de dsListAdvertiser
			// Bouton OK
			if(eventButton==constEvent.eventSelection.OK_EVENT || !Page.IsPostBack || eventButton==constEvent.eventSelection.INITIALIZE_EVENT ||
				eventButton==constEvent.eventSelection.ALL_INITIALIZE_EVENT   || eventButton==constEvent.eventSelection.NEXT_EVENT ||
				eventButton==constEvent.eventSelection.OK_OPTION_MEDIA_EVENT || eventButton==constEvent.eventSelection.LOAD_EVENT){
				// Suppression des checkbox contenues dans la liste
				this.Items.Clear();				

				// Ajout des checkboxs dans la liste					
				int start=0;
				if(dsListMedia!=null){	
					foreach(DataRow currentRow in dsListMedia.Tables[0].Rows) {
						this.Items.Add(new System.Web.UI.WebControls.ListItem("Children_" + currentRow[3].ToString(), currentRow[2].ToString()));
						if(start==0 && !listMediaSelected.Contains(currentRow[2].ToString())){
//							nextLabel=true;
							start=-1;
						}
//						else if(start==0){
//							nextLabel=false;
//						}
					}
					
				}
			}				
				//if(eventButton==constEvent.eventSelection.OK_EVENT || eventButton==constEvent.eventSelection.VALID_EVENT ){
				#region Next Label
			
				if ( (dsListMedia!=null && dsListMedia.Tables[0].Rows.Count>0) || (webSession.CompetitorUniversMedia!=null && webSession.CompetitorUniversMedia.Count>0)){						
					isEmptyList=false;	
				}
				else
					isEmptyList=true;
		
		
				#endregion

			#endregion

			#region Ajout des items à partir de l'arbre
			//if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.OK_POP_UP_EVENT){	
			if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
				this.Items.Clear();
                foreach (System.Windows.Forms.TreeNode currentNode in webSession.CurrentUniversMedia.Nodes)
                {					
					System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
					checkBox1.Text="Children_"+((LevelInformation)currentNode.Tag).Text;
					checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
					this.Items.Add(checkBox1);
					isEmptyList=false;
				}
			}
			
			#endregion

		}

		#endregion

	
		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output) {
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			
			#region Affichage à partir du dataset
			if(eventButton==constEvent.eventSelection.OK_EVENT || (!Page.IsPostBack && eventButton!=constEvent.eventSelection.OK_POP_UP_EVENT ) 
				|| eventButton==constEvent.eventSelection.NEXT_EVENT || eventButton==constEvent.eventSelection.OK_OPTION_MEDIA_EVENT || eventButton==constEvent.eventSelection.INITIALIZE_EVENT || eventButton==constEvent.eventSelection.ALL_INITIALIZE_EVENT){
				
				#region Variables
				Int64 idParentOld=-1;
				Int64 idParent;				
				string textParent;
				string textParentOld;
				int nbColumn=0;
				int start=-1;
				int i =0;
				int insertIndex = 0;
				string checkBox="";
				string disabled="";
				string 	vhList="";	
				string textOpenclose="";
				string displayDiv="None";
				int displayIndex=0;
				Int64 idParentDiv=0;
				int counter=0;
				string insertLabel=string.Empty;
				int labelIndex=0;
                string cssTextItem = "txtViolet10";
				#endregion

				if(!isEmptyList){
                    textOpenclose = GestionWeb.GetWebWord(2461, webSession.SiteLanguage);
				}
			
				if(IsMediaFound(keyWord,eventButton,isEmptyList)){
					t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
					insertIndex = t.Length;
					t.Append("')\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;"+textOpenclose+"</a>");	
					//t.Append("<td><tr>");
				}

                t.Append("<tr class=\"whiteBackGround\"><td  vAlign=\"top\"><table >");
                t.Append("<tr><td vAlign=\"top\" class=\"whiteBackGround\">");

				if(dsListMedia!=null && !isEmptyList && IsMediaFound(keyWord,eventButton,isEmptyList)){
                    foreach(DataRow currentRow in dsListMedia.Tables[0].Rows) {

                        #region Foreach
                        counter += 1;
                        idParent = (Int64)currentRow[0];
                        textParent = currentRow[1].ToString();

                        if(start != 0) {
                            idParentDiv = idParent;
                        }
                        //to maintain the state of the list 
                        if(idParentDiv != idParent) {
                            t.Insert(displayIndex, displayDiv);
                            displayDiv = "None";
                            idParentDiv = idParent;
                        }

                        if(idParentOld != idParent && start == 0) {
                            if(nbColumn != 0) {
                                t.Append("</tr>");
                                nbColumn = 0;
                            }
                            t.Append("</table>");
                            t.Append("</div>");
                            t.Append("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=645>");
                            t.Append("<tr onClick=\"DivDisplayer('" + idParent + "Ct" + "');\" class=\"cursorHand\">");
                            t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");
                            //t.Append("<input type=\"checkbox\"  onclick=\"integration('"+idParent+"',"+i+")\" ID=\"AdvertiserSelectionWebControl1_"+i+"\" name=\"AdvertiserSelectionWebControl1:"+i+"\">");
                            t.Append("<label>  ");
                            t.Append("&nbsp;&nbsp;" + currentRow[1].ToString() + "");
                            t.Append("</label>");
                            t.Append("</td>");
                            t.Append("<td class=\"arrowBackGround\"></td>");
                            t.Append("</tr>");
                            t.Append("</table>");
                            t.Append("<div id=\"" + idParent + "Ct\" class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY:");
                            displayIndex = t.Length;
                            t.Append("; WIDTH: 100%\">");
                            t.Append("<table id=" + idParent + " class=\"violetBorderWithoutTop paleVioletBackGround\" width=645>");
                            t.Append("<tr><td colspan=\"3\"><a href=# style=\"TEXT-DECORATION: none\" class=\"roll04\" onclick=\"allSelection('" + idParent + "'," + i + ")\" ID=\"" + currentRow[0] + "\">");
                            t.Append(GestionWeb.GetWebWord(1066, webSession.SiteLanguage));
                            t.Append("</a></td></tr>");
                            vhList = vhList + (Int64)currentRow[0] + "Ct" + ",";
                            idParentOld = idParent;
                            textParentOld = textParent;
                        }

                        //Premier	
                        if(idParentOld != idParent && start != 0) {
                            t.Append("<table class=\"violetBorder txtViolet11Bold\" cellpadding=0 cellspacing=0   width=645>");
                            t.Append("<tr onClick=\"DivDisplayer('" + idParent + "Ct" + "');\" class=\"cursorHand\" >");
                            //t.Append("<tr onClick=\"showHideContent('"+idParent+"');\" style=\"cursor : hand\" >");
                            t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");
                            //t.Append("<input type=\"checkbox\"  onclick=\"integration('"+idParent+"',"+i+")\" ID=\"AdvertiserSelectionWebControl1_"+i+"\" name=\"AdvertiserSelectionWebControl1:"+i+"\">");
                            t.Append("<label>");
                            t.Append("&nbsp;&nbsp;" + currentRow[1].ToString() + "");
                            t.Append("</label>");
                            t.Append("</td>");
                            t.Append("<td class=\"arrowBackGround\"></td>");
                            t.Append("</tr>");
                            t.Append("</table>");
                            t.Append("<div id=\"" + idParent + "Ct\" class=\"BlancBorderColorWithoutTop\" style=\"DISPLAY: ");
                            displayIndex = t.Length;
                            t.Append("; WIDTH: 100%\" >");
                            t.Append("<table id=" + idParent + " class =\"violetBorderWithoutTop paleVioletBackGround\" width=645>");
                            t.Append("<tr><td colspan=\"3\"><a href=# class=\"roll04\" style=\"TEXT-DECORATION: none\" onclick=\"allSelection('" + idParent + "'," + i + ")\" ID=\"" + currentRow[0] + "\">");
                            t.Append(GestionWeb.GetWebWord(1066, webSession.SiteLanguage));
                            t.Append("</a></td></tr>");
                            idParentOld = idParent;
                            textParentOld = textParent;
                            vhList = vhList + (Int64)currentRow[0] + "Ct" + ",";
                            start = 0;
                        }

                        #region Affichage des fils
                        checkBox = "";
                        disabled = "";
                        if(tabListMedia != null) {
                            foreach(string item1 in tabListMedia) {
                                if(item1 == currentRow[2].ToString()) {
                                    checkBox = "checked";
                                    break;
                                }
                            }
                        }
                        int j = 1;
                        while(j <= webSession.CompetitorUniversMedia.Count) {

                            System.Windows.Forms.TreeNode competitorMedia = (System.Windows.Forms.TreeNode)webSession.CompetitorUniversMedia[j];
                            foreach(System.Windows.Forms.TreeNode item2 in competitorMedia.Nodes) {
                                if(((LevelInformation)item2.Tag).ID == (long)currentRow[2]) {
                                    disabled = "disabled";
                                    checkBox = "checked";
                                    displayDiv = "''";
                                    break;
                                }
                            }
                            j++;
                        }

                        // Milieu
                        if(nbColumn == 2) {
                            cssTextItem = int.Parse(currentRow[4].ToString()) != DBConstantes.ActivationValues.DEAD ? "txtViolet10" : "txtOrange10";

                            t.Append("<td class=\"" + cssTextItem + "\" width=215>");
                            t.Append("<input type=\"checkbox\" " + checkBox + " " + disabled + " ID=\"AdvertiserSelectionWebControl1_" + i + "\" value=" + idParent + " name=\"AdvertiserSelectionWebControl1$" + i + "\"><label for=\"AdvertiserSelectionWebControl1_" + i + "\">" + currentRow[3].ToString() + "<br></label>");
                            t.Append("</td>");
                            nbColumn = 1;
                            i++;

                        }
                        // Dernier
                        else if(nbColumn == 1) {
                            cssTextItem = int.Parse(currentRow[4].ToString()) != DBConstantes.ActivationValues.DEAD ? "txtViolet10" : "txtOrange10";

                            t.Append("<td class=\"" + cssTextItem + "\" width=215>");
                            t.Append("<input type=\"checkbox\" " + checkBox + " " + disabled + " ID=\"AdvertiserSelectionWebControl1_" + i + "\"  value=" + idParent + "  name=\"AdvertiserSelectionWebControl1$" + i + "\"><label for=\"AdvertiserSelectionWebControl1_" + i + "\">" + currentRow[3].ToString() + "<br></label>");
                            t.Append("</td>");
                            t.Append("</tr>");
                            nbColumn = 0;
                            i++;

                        }
                        // Premier
                        else {
                            cssTextItem = int.Parse(currentRow[4].ToString()) != DBConstantes.ActivationValues.DEAD ? "txtViolet10" : "txtOrange10";

                            t.Append("<tr>");
                            t.Append("<td class=\"" + cssTextItem + "\" width=215>");
                            t.Append("<input type=\"checkbox\" " + checkBox + " " + disabled + " ID=\"AdvertiserSelectionWebControl1_" + i + "\" value=" + idParent + " name=\"AdvertiserSelectionWebControl1$" + i + "\"><label for=\"AdvertiserSelectionWebControl1_" + i + "\">" + currentRow[3].ToString() + "<br></label>");
                            t.Append("</td>");
                            nbColumn = 2;
                            i++;
                        }
                        #endregion

                        //To maintain the state of the list when its the last div as it wont be handeled by the first check
                        if(idParentDiv == idParent && dsListMedia.Tables[0].Rows.Count == counter) {
                            t.Insert(displayIndex, displayDiv);
                            displayDiv = "None";
                            idParentDiv = idParent;
                        }

                        #endregion

                    }
				
					if(dsListMedia.Tables[0].Rows.Count!=0){
						if(nbColumn!=0){
							t.Append("</tr>"); 
							nbColumn=0;
						}
						else{
							nbColumn=0;
						}
						t.Append("</table></div>");
					}
				}

				#region  Message d'erreurs
				// Message d'erreur : veuillez saisir 2 caractères minimums
				if(keyWord.Length<2 && keyWord.Length>0 && eventButton==constEvent.eventSelection.OK_EVENT){
                    t.Append("<tr class=\"whiteBackGround\" ><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(1473,webSession.SiteLanguage)+" "+keyWord+".</p> ");
					t.Append("</table>");
					t.Append("</td>");
					t.Append("</tr>");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}
					// Message d'erreur : mot incorrect
				else if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(keyWord) && keyWord.Length>0 && eventButton==constEvent.eventSelection.OK_EVENT && isEmptyList){
                    t.Append("<tr><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(1088,webSession.SiteLanguage)+" "+keyWord+".</p> ");
					t.Append("</table>");
					t.Append("</td>");
					t.Append("</tr>");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}
					// Message d'erreur : aucun résultat avec le mot clé
				else if(dsListMedia!=null){
					if(dsListMedia.Tables[0].Rows.Count==0 || isEmptyList){
                        t.Append("<tr><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
						t.Append(" "+GestionWeb.GetWebWord(819,webSession.SiteLanguage)+" "+keyWord+".</p> ");
						t.Append("</table>");
						t.Append("</td>");
						t.Append("</tr>");
						t.Append(" </td> ");
						t.Append(" </tr> ");
					}
				}
				#endregion

				int listLenght=0;
				if (vhList!=""){
					t.Append("</td></tr></table></td></tr>");			
					vhList = vhList.Remove(vhList.Length-1, 1);
					t.Insert(insertIndex, vhList);
					listLenght=vhList.Length;
					insertLabel=textOpenclose;
				}
				else
					t.Insert(labelIndex+listLenght,insertLabel);
				output.Write(t.ToString());
			}
			#endregion

			#region Affichage à partir de l'arbre
			//if( (eventButton==constEvent.eventSelection.LOAD_EVENT && loadIsPossible) || eventButton==constEvent.eventSelection.OK_POP_UP_EVENT ){
			if(!isEmptyList){
				if( (eventButton==constEvent.eventSelection.LOAD_EVENT && loadIsPossible) || eventButton==constEvent.eventSelection.SAVE_EVENT ){
				
					t.Append("<tr class=\"txtGris11Bold\">");
                    t.Append("<td class=\"whiteBackGround\"  >");
					t.Append(""+GestionWeb.GetWebWord(812,webSession.SiteLanguage)+"</td></tr>");


                    t.Append("<tr height=5px class=\"whiteBackGround\"><td></td></tr>");
                    t.Append("<tr><td class=\"whiteBackGround\" vAlign=\"top\">");
                    t.Append("<table class=\"whiteBackGround\">");				
					t.Append("<tr><td>");	
					// Affichage des annonceur à partir de SelectionUniversAdvertiser où CurrentUniversAdvertiser
							
					t.Append(TNS.AdExpress.Web.Functions.DisplayTreeNode.ToHtml(webSession.CurrentUniversMedia,true,true,true,645,true,true,webSession.SiteLanguage,3,1,true));
				
					t.Append("</td></tr></table></td></tr>");
					output.Write(t.ToString());	
				}
			
			}
			#endregion

			// Message d'erreur : Chargement de l'univers impossible
			if(!loadIsPossible){
                t.Append("<tr class=\"whiteBackGround\" ><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" "+GestionWeb.GetWebWord(1086,webSession.SiteLanguage)+"</p> ");			
				t.Append(" </td> ");
				t.Append(" </tr> ");
				output.Write(t.ToString());
			}
		}
		#endregion

		#region Création de l'arbre
		/// <summary>
		/// Création de l'arbre à partir de la liste des Items
		/// </summary>
		/// <returns></returns>
        protected System.Windows.Forms.TreeNode createTreeNode(){
			System.Windows.Forms.TreeNode mediaTree=new System.Windows.Forms.TreeNode();
			System.Windows.Forms.TreeNode tmpNode=null;
			// Nbre de cases cochés
			int compteurChild=0;		
				
			foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items){
				#region foreach
				string[] tabParent=currentItem.Text.Split('_') ;
				if(tabParent[0]=="Children"  && currentItem.Selected){
					tmpNode=new System.Windows.Forms.TreeNode(tabParent[1]);						
					tmpNode.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,Int64.Parse(currentItem.Value),tabParent[1]);
					tmpNode.Checked=true;
					mediaTree.Nodes.Add(tmpNode);	
					compteurChild++;
				}	
				#endregion
			}	
			
			// Fournit une valeur à nbElement pour vérifier la validité du nombre 
			// d'éléments sélectionné
			if (compteurChild==0){
				nbElement=constEvent.error.CHECKBOX_NULL;  
			}
			else if (compteurChild >= MAX_SELECTABLE_ELEMENTS) {
				nbElement=constEvent.error.MAX_ELEMENTS;
			}

			return mediaTree;

		}
		#endregion
		
		#region vérifie si l'on peut faire le chargement d'un univers
		/// <summary>
		/// Vérifie si l'on peut charger l'univers
		/// </summary>
		/// <returns></returns>
		protected bool verifLoadIsOk(){
			string listAccessMediaDisable="";
			string[] tabListMediaDisable;
			bool loadOk=true;
			int j=1;
			// Récupère l'ensemble des éléments déjà sélectionnés
			while(j<=webSession.CompetitorUniversMedia.Count){
                System.Windows.Forms.TreeNode competitorMedia = (System.Windows.Forms.TreeNode)webSession.CompetitorUniversMedia[j];
                foreach (System.Windows.Forms.TreeNode item2 in competitorMedia.Nodes)
                {
					listAccessMediaDisable+=((LevelInformation)item2.Tag).ID.ToString()+",";
				}			
				j++;
			}
			if(listAccessMediaDisable.Length>0){
				listAccessMediaDisable=listAccessMediaDisable.Substring(0,listAccessMediaDisable.Length-1);
			}
			tabListMediaDisable=listAccessMediaDisable.Split(',');

			// Regarde s'il existe déjà un élément sélectionné se trouvant dans l'univers sauvegardé
            foreach (System.Windows.Forms.TreeNode item2 in webSession.CurrentUniversMedia.Nodes)
            {
				if(tabListMediaDisable!=null && tabListMediaDisable.Length>0){
					foreach(string item1 in tabListMediaDisable){
						if(item1==((LevelInformation)item2.Tag).ID.ToString()){
							loadOk=false;
							break;
						}
					}
				}
				if(!loadOk)break;
			}
			return loadOk;
		}
		#endregion

		#region Obtention des supports
		
		/// <summary>
		/// Obtient la liste des supports à afficher
		/// </summary>
		/// <param name="eventButton">evenement</param>
		/// <param name="keyWord">Mot clé recherché</param>
		/// <param name="listAccessMedia">liste des medias en accès</param>
		/// <returns>Liste des supports à aficher</returns>
		private DataSet GetData(int eventButton,string keyWord,string listAccessMedia){

            if (eventButton == constEvent.eventSelection.OK_EVENT && nbElement != constEvent.error.MAX_ELEMENTS)
                return Core.DataAccess.DetailMediaDataAccess.keyWordDetailMediaListDataAccess(webSession, keyWord, listAccessMedia, DetailLevelItemsInformation.Get(webSession.MediaSelectionParent.GetHashCode()));
            else return Core.DataAccess.DetailMediaDataAccess.DetailMediaListDataAccess(webSession, DetailLevelItemsInformation.Get(webSession.MediaSelectionParent.GetHashCode()));

		}
		
		/// <summary>
		/// Vérifie si des médias ont été trouvés 
		/// </summary>
		/// <param name="keyWord">mot clé</param>
		/// <param name="eventButton">évènement</param>
		/// <param name="isEmptyList">vrai si vide</param>
		/// <returns>vrai si média trpouvé</returns>
		private bool IsMediaFound(string keyWord,int eventButton,bool isEmptyList){
			if((keyWord.Length<2 && keyWord.Length>0 && eventButton==constEvent.eventSelection.OK_EVENT)
			|| (!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(keyWord) && keyWord.Length>0 && eventButton==constEvent.eventSelection.OK_EVENT && isEmptyList)
			|| (dsListMedia==null || (dsListMedia!=null && dsListMedia.Tables[0].Rows.Count==0) || isEmptyList)
				)
				return false;

			return true;
		}
		#endregion

		#endregion
		
	}
}
