#region Informations
// Auteur: Y. R'kaina
// Date de création: 06/12/2006
// Date de modification:
#endregion

using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Windows.Forms;
using TNS.AdExpress.Web.DataAccess.Selections.Programs;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.DataAccess;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Selections {
	/// <summary>
	/// Composant affichant la liste des annonceurs
	/// Il est utilisé dans la sélection de l'annonceur ainsi que dans la sous sélection
	/// au niveau des pages de résultats.
	/// Suivant la valeur de l'entier buttonTarget des actions sont réalisées.
	/// 
	/// </summary>
	
	[ToolboxData("<{0}:SponsorshipFormSelectionWebControl runat=server></{0}:SponsorshipFormSelectionWebControl>")]
	public class SponsorshipFormSelectionWebControl: System.Web.UI.WebControls.CheckBoxList {

		#region Variables
	     
		/// <summary>
		/// Dataset contenant la liste des formes de parrainage
		/// </summary>
		protected DataSet dsListSponsorshipForm;
		/// <summary>
		/// Session
		/// </summary>
		protected WebSession webSession;
		/// <summary>
		/// Text saisie pour le choix de la recherhce
		/// </summary>
		protected string sponsorshipFormText;
		/// <summary>
		/// Tableau avec la liste des formes de parrainage en accès
		/// </summary>
		protected string[] tabSponsorshipFormAccess;
		/// <summary>
		/// Tableau avec la liste des formes de parrainage en Exception
		/// </summary>
		protected string[] tabSponsorshipFormException;
		/// <summary>
		/// Entier qui détermine qu'elle boutton 
		/// a été cliquer 1 pour valider
		/// 2 pour ok
		/// </summary>
		int eventButton;
		/// <summary>
		/// Message d'erreur
		/// </summary>
		string ErrorMessage="";
		/// <summary>
		/// si nbElement=2 un liste contient plus de 1000 éléments
		/// si nbElement=3 aucun élément n'a été séléctionnée
		/// </summary>
		int nbElement=1;
		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public SponsorshipFormSelectionWebControl():base() {

			this.EnableViewState=true;
		}
		#endregion

		#region Accesseurs
		
		/// <summary>
		/// Obtient le dataset avec la liste des formes de parrainage 
		/// </summary>
		public DataSet DsListSponsorshipForm {
			get{return dsListSponsorshipForm;}
		}

		/// <summary>
		/// Obtient ou définit la webSession 
		/// </summary>
		public WebSession WebSession 
		{
			get{return webSession;}
			set{webSession=value;}
		}

		/// <summary>
		/// Obtient ou définit le text saisie au niveau du forme de parrainage
		/// </summary>
		public string SponsorshipFormText {
			get{return sponsorshipFormText;}
			set{sponsorshipFormText=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur de la case à cocher groupe
		/// </summary>
		public int NbElement {
			get{return nbElement;}
			set{nbElement=value;}
		}

		/// <summary>
		/// Obtient ou définit la valeur du boutton qui a été
		/// cliqué
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
			string listSponsorshipForm="";
			string listSponsorshipFormAccess="";
			#endregion

			if(eventButton==-1) {
				//if(eventButton==constEvent.eventSelection.INITIALIZE_EVENT) {
				Items.Clear();
			}

			#region !IsPostBack
			if((!Page.IsPostBack) || (eventButton==-1)) {
				//if((!Page.IsPostBack) || (eventButton==constEvent.eventSelection.INITIALIZE_EVENT)) {

				dsListSponsorshipForm = SponsorshipFormListDataAccess.GetSponsorshipFormListDataAccess(webSession);
			}
			#endregion

			#region Bouton Ok
			if(eventButton==2 || eventButton==3) {	
				//if(eventButton==constEvent.eventSelection.OK_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT) {	
				if(Page.IsPostBack) {

					#region PostBack

					#region Création des listes
					foreach(System.Web.UI.WebControls.ListItem currentItem in this.Items) {

						if(currentItem.Selected) {
							listSponsorshipFormAccess+=currentItem.Value+",";
						}
					}
					#endregion
				
					#region Initialisation des tableaux
					tabSponsorshipFormAccess=null;
					#endregion

					#region Création des listes et des tableaux
					//Liste formes de parrainage
					if(listSponsorshipFormAccess.Length!=0) {
						listSponsorshipForm+=listSponsorshipFormAccess;
						tabSponsorshipFormAccess=listSponsorshipFormAccess.Split(',');
					}

					//Suppression de la dernière
					if(listSponsorshipForm.Length>0) {
						listSponsorshipForm=listSponsorshipForm.Substring(0,listSponsorshipForm.Length-1);
					}
					
					#endregion

					// Evènment sauvegarder
					if(eventButton==3) {
						//if(eventButton==constEvent.eventSelection.SAVE_EVENT) {
						webSession.SelectionUniversSponsorshipForm=this.createTreeNode();
						webSession.Save();
					}
					
					//if(eventButton==constEvent.eventSelection.OK_EVENT) {
					if(eventButton==2) {

						if(sponsorshipFormText.Length <2 ||  !TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(sponsorshipFormText)) {
							sponsorshipFormText="";
						}

						// Vérifie que les listes soient inférieur à 1000 éléments
						if(listSponsorshipForm.Split(',').Length<=TNS.AdExpress.Constantes.Web.Advertiser.MAX_NUMBER_ADVERTISER) {
							// Requête pour récupérer la liste des éléments
							if((sponsorshipFormText.Length >=2 && TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(sponsorshipFormText)) 
								|| listSponsorshipForm.Length!=0) {
							
								dsListSponsorshipForm = GenericClassificationSearchDataAccess.GetSearchResultListDataAccess(webSession,sponsorshipFormText,listSponsorshipForm,DetailLevelItemInformation.Levels.sponsorshipForm);
							}
						}
							// Message d'erreur : Vous devez sélectionner moins de 1000 éléments
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
				webSession.SelectionUniversSponsorshipForm=this.createTreeNode();
				webSession.Save();
			}

			if(eventButton==-2) {
				this.Items.Clear();
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

			#region Ajout des items à partir de dsListSponsorshipForm
			
			if((eventButton==1)||(eventButton==2)||(eventButton==-1)||(eventButton==0)) {

				// Suppression des checkbox contenues dans la liste
				this.Items.Clear();

				#region 1 niveaux
				// Ajout des checkboxs dans la liste
			
				if(DsListSponsorshipForm!=null) {	
					foreach(DataRow currentRow in DsListSponsorshipForm.Tables[0].Rows) {
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow[1].ToString(),currentRow[0].ToString()));					
					}
				}
			
				#endregion

			}
			#endregion

			#region Ajout des items à partir de l'arbre SelectionUniversProgramType
			// Chargement de l'univers
			// Ajout des éléments dans listItem
			// Exécuter lors des évènmements suivant: 4 : bouton charger
			// 6 : bouton ok de la popup 8 : Retour à la sélection initiale
			// 9 : lors d'un chargement dans la page de résultat
			if(eventButton==4 || eventButton==3 || eventButton==8 || eventButton==9) {
				//if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT || eventButton==8 || eventButton==9) {
				
				// Suppression des checkbox contenues dans la liste
				this.Items.Clear();

				foreach(System.Windows.Forms.TreeNode currentNode in webSession.SelectionUniversSponsorshipForm.Nodes) {					
					System.Web.UI.WebControls.ListItem checkBox1 =new System.Web.UI.WebControls.ListItem();
					checkBox1.Text=((LevelInformation)currentNode.Tag).Text;
					checkBox1.Value=((LevelInformation)currentNode.Tag).ID.ToString();
					this.Items.Add(checkBox1);
				}
			}
			#endregion

		}
		#endregion 
		
		#region CreateItems
		/// <summary>
		/// Méthode pour créer la liste d'item à partir de l'arbre CurrentUniversAdvertiser
		/// </summary>
		protected void createItems() {
		}
		#endregion 

		#region Render
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output"></param>
		protected override void Render(HtmlTextWriter output) {
			// Numéro du checkBox
			int i=0;
			int length=50000;
			int nbLinesTest=0;
			string checkBox="";
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

			if(dsListSponsorshipForm!=null)length=dsListSponsorshipForm.Tables[0].Rows.Count*500;
			System.Text.StringBuilder t=new System.Text.StringBuilder(length);
			
			#region Affichache des Annonceurs à partir du dsListSponsorshipForm
			if((eventButton==2)||(eventButton==-1)||!(Page.IsPostBack)) {
				//if((eventButton==constEvent.eventSelection.OK_EVENT)||(eventButton==constEvent.eventSelection.INITIALIZE_EVENT)||!(Page.IsPostBack)) {
			
				#region Affichage
		
				if(dsListSponsorshipForm!=null) {

					if(dsListSponsorshipForm.Tables[0].Rows.Count>0) {	
						t.Append("<tr class=\"txtGris11Bold\">");
                        t.Append("<td class=\"backGroundWhite\">");
						t.Append("&nbsp;"+GestionWeb.GetWebWord(812,webSession.SiteLanguage)+"</td></tr>");
					}
				}
                t.Append("<tr height=5px class=\"backGroundWhite\"><td></td></tr>");
				//t.Append("<tr><td class=\"backGroundWhite\"><img src=\"images/pixel.gif\" width=\"1\" height=\"5\"></td></tr>");
                t.Append("<tr><td class=\"backGroundWhite\"><img src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\" width=\"1\" height=\"20\"></td></tr> ");

				#endregion
			
				//Tableau global
				t.Append("<tr><td class=\"backGroundWhite\" valign=\"top\"><table width=\"100%\">");		
	            t.Append("<tr><td vAlign=\"top\">");

				#region 1 niveaux
				
				if(dsListSponsorshipForm!=null) {
					if(dsListSponsorshipForm.Tables[0].Rows.Count!=0) {
                        t.Append("<table class=\"violetBorderWithoutBottom backGroundWhite\" width=\"100%\">");
						t.Append("<tr><td class=\"txtViolet11Bold\" align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
						t.Append("<label for=\"Formes de parrainages\">" + GestionWeb.GetWebWord(2052,webSession.SiteLanguage) + "");
						t.Append("</label>");
						t.Append("</td></tr></table>");
                        t.Append("<table class=\"violetBorder paleVioletBackGround\" width=\"100%\">");
						t.Append("<tr><td colspan=\"3\"><a href=# class=\"roll04\" onclick=\"allSelectionRef()\" style=\"TEXT-DECORATION: none\"  ID=\"tab1\">");
						t.Append(GestionWeb.GetWebWord(2056,webSession.SiteLanguage));
						t.Append("</a></td></tr>");	
					
						foreach(DataRow currentRow in dsListSponsorshipForm.Tables[0].Rows) {
					
							#region foreach

							checkBox=""; 
							
							if(tabSponsorshipFormAccess!=null) {
								foreach(string item1  in tabSponsorshipFormAccess) {
									if(item1==currentRow[0].ToString()) {
										checkBox="checked";
										break;
									}
								}
							}

							#region Affichage des fils
							if(nbLinesTest==2) {
								t.Append("<td class=\"txtViolet10\" width=215>");
								t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\" name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[1].ToString()+"<br></label>");
								t.Append("</td>");
								nbLinesTest=1;
								i++;
				
							}
							else if(nbLinesTest==1) {
								t.Append("<td class=\"txtViolet10\" width=215>");								
								t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\"  name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[1].ToString()+"<br></label>");
								t.Append("</td>");								
								t.Append("</tr>");
								nbLinesTest=0;
								i++;	
				
							}
							else {
								t.Append("<tr>");
								t.Append("<td class=\"txtViolet10\" width=215>");
								t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\" name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[1].ToString()+"<br></label>");
								t.Append("</td>");								
								nbLinesTest=2;
								i++;
							}
							#endregion

							#endregion

						}
				
						if(nbLinesTest!=0) {
							t.Append("</tr>");
							nbLinesTest=0;
						}
						else {
							nbLinesTest=0;
						}
						t.Append("</table>");
					}
				}
				
				#endregion

				if(sponsorshipFormText.Length<2 && eventButton==2) {							
					//if(programTypeText.Length<2 && eventButton==constEvent.eventSelection.OK_EVENT) {
					// Saississez 2 caractères au minimum
                    t.Append("<tr><td class=\"txtGris11Bold backGroundWhite\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(1473,webSession.SiteLanguage)+" "+sponsorshipFormText+".</p> ");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}	
				else if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(sponsorshipFormText) && eventButton==2) {
					//else if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(programTypeText) && eventButton==constEvent.eventSelection.OK_EVENT) {
                    t.Append("<tr><td class=\"txtGris11Bold backGroundWhite\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(820,webSession.SiteLanguage)+" "+sponsorshipFormText+".</p> ");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}	
				else if(dsListSponsorshipForm!=null) {
					if(dsListSponsorshipForm.Tables[0].Rows.Count==0) {
                        t.Append("<tr><td class=\"txtGris11Bold backGroundWhite\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
						t.Append(" "+GestionWeb.GetWebWord(819,webSession.SiteLanguage)+" "+sponsorshipFormText+".</p> ");
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
                t.Append("<tr><td class=\"backGroundWhite\"><IMG height=\"20\" src=\"App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"1\"></td></tr>");
				t.Append("<tr><td></td></tr>");	

				output.Write(t.ToString());		
			}	
			#endregion
	
			#region Affichache des Annonceurs à partir d'un arbre
			if(eventButton==4  || eventButton==7 || eventButton==3 || eventButton==8 || eventButton==9) 
			{
				//if(eventButton==constEvent.eventSelection.LOAD_EVENT  || eventButton==7 || eventButton==constEvent.eventSelection.SAVE_EVENT || eventButton==8 || eventButton==9) {
				if(eventButton==4 || eventButton==3) {
					//if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT) {
					t.Append("<tr class=\"txtGris11Bold\">");
                    t.Append("<td class=\"backGroundWhite\">");
					t.Append(""+GestionWeb.GetWebWord(812,webSession.SiteLanguage)+"</td></tr>");
				}
				
				t.Append("<tr height=5px class=\"backGroundWhite\"><td></td></tr>");				
				t.Append("<tr><td class=\"backGroundWhite\" vAlign=\"top\">");
				t.Append("<table class=\"backGroundWhite\" width=\"100%\">");				
				t.Append("<tr> ");
				t.Append("</td></tr><tr><td>");	

				// Affichage des formes de parrainages à partir de SelectionUniversSponsorshipForm
				if(eventButton==4 || eventButton==3 || eventButton==8) {
					//if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT || eventButton==8) {
					
					#region 1 niveaux
                    t.Append("<table class=\"backGroundWhite violetBorderWithoutBottom\" width=\"100%\">");
					t.Append("<tr><td class=\"txtViolet11Bold\" align=\"left\" height=\"10\" valign=\"middle\" nowrap>");
					t.Append("<label for=\"Formes de parrainages\">" + GestionWeb.GetWebWord(2052,webSession.SiteLanguage) + "");
					t.Append("</label>");
					t.Append("</td></tr></table>");
                    t.Append("<table class=\"violetBorder paleVioletBackGround\" width=\"100%\">");
					t.Append("<tr><td colspan=\"3\"><a href=# class=\"roll04\" onclick=\"allSelectionRef()\" style=\"TEXT-DECORATION: none\"  ID=\"tab1\">");
					t.Append(GestionWeb.GetWebWord(817,webSession.SiteLanguage));
					t.Append("</a></td></tr>");	
				
					foreach(System.Windows.Forms.TreeNode currentNode in webSession.SelectionUniversSponsorshipForm.Nodes){
					
						if(currentNode.Checked)
							checkBox="checked";

						if(nbLinesTest==2) {
							t.Append("<td class=\"txtViolet10\" width=215>");
							t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\" name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+((LevelInformation)currentNode.Tag).Text+"<br></label>");
							t.Append("</td>");
							nbLinesTest=1;
							i++;
		
						}
						else if(nbLinesTest==1) {
							t.Append("<td class=\"txtViolet10\" width=215>");								
							t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\"  name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+((LevelInformation)currentNode.Tag).Text+"<br></label>");
							t.Append("</td>");								
							t.Append("</tr>");
							nbLinesTest=0;
							i++;	
		
						}
						else {
							t.Append("<tr>");
							t.Append("<td class=\"txtViolet10\" width=215>");
							t.Append("<input type=\"checkbox\" "+checkBox+" ID=\"AdvertiserSelectionWebControl1_"+i+"\" value=\"child\" name=\"AdvertiserSelectionWebControl1$"+i+"\"><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+((LevelInformation)currentNode.Tag).Text+"<br></label>");
							t.Append("</td>");								
							nbLinesTest=2;
							i++;
						}
					}
		
					if(nbLinesTest!=0) {
						t.Append("</tr>");
						nbLinesTest=0;
					}
					else {
						nbLinesTest=0;
					}
					t.Append("</table>");
				
					#endregion
				}
				t.Append("</td></tr></table></td></tr>");				
				output.Write(t.ToString());	
			}
			#endregion

		}
		#endregion
		
		#region création de l'arbre
		/// <summary>
		/// Création de l'arbre à partir de la liste des Items
		/// </summary>
		/// <returns></returns>
        protected System.Windows.Forms.TreeNode createTreeNode()
        {
			System.Windows.Forms.TreeNode sponsorshipFormTree=new System.Windows.Forms.TreeNode("sponsorshipform");					
		
			// forme de parrainages sélectionnés		
			sponsorshipFormTree = ((System.Windows.Forms.TreeNode)webSession.SelectionUniversSponsorshipForm);					
			int compteurChild=0;
			
			if(webSession.SelectionUniversSponsorshipForm.FirstNode!=null) {
				webSession.SelectionUniversSponsorshipForm.Nodes.Clear();
			}																	
				
			#region foreach
			System.Windows.Forms.TreeNode tmp;
			foreach(ListItem item in this.Items) {
				if (item.Selected) {
					tmp = new System.Windows.Forms.TreeNode(item.Text);
					tmp.Tag = new LevelInformation(CstWebCustomer.Right.type.sponsorshipFormAccess,Int64.Parse(item.Value),item.Text);
					tmp.Checked = true;
					sponsorshipFormTree.Nodes.Add(tmp);
					compteurChild++;				
				}
			}													
			#endregion												
					
			// Fournit une valeur à nbElement pour vérifier la validité du nombre 
			// d'éléments sélectionné
			if (compteurChild==0) {
				nbElement=3;  
			}

			return sponsorshipFormTree;
		}
		#endregion

	}
}
