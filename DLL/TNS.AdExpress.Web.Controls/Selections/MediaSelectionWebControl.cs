#region Informations
// Auteur: D. V. Mussuma 
// Date de cr�ation: 01/03/2005 
// Date de modification: 01 /03/2005 
#endregion

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Web.Core.Translation;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
using VhCstes=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using TableName=TNS.AdExpress.Constantes.Classification.DB.Table.name;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;

namespace TNS.AdExpress.Web.Controls.Selections
{
	/// <summary>
	/// Affiche la liste des m�dia/centres d'int�r�ts/supports que peut s�lectionner un client en fonction de ses droits.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:MediaSelectionWebControl runat=server></{0}:MediaSelectionWebControl>")]
	public class MediaSelectionWebControl : System.Web.UI.WebControls.CheckBoxList{
	
		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession webSession=null; 
		/// <summary>
		/// Dataset contenant la liste des m�dias
		/// </summary>
		protected DataSet dsListMedia;
		/// <summary>
		/// liste des m�dias(vehicle) dont le clients � acc�s
		/// </summary>		
		protected string  _vhlist;
		/// <summary>
		/// Ev�nement qui a �t� lanc�
		/// </summary>
		protected int eventButton;
		/// <summary>
		/// valeur qui permet de savoir si des �l�ments ont
		/// �t� s�lectionn�s
		/// </summary>
		protected int nbElement=-1;
		/// <summary>
		/// Indique si on charge l'arbre Selection des M�dias pr�sent dans la session ou non
		/// </summary>
		public bool LoadSession = false;
		
		/// <summary>
		/// Indique la pr�sence d'enregistrements m�dia
		/// </summary>
		public bool hasRecords=true;

		#endregion

		#region Accesseurs

		/// <summary>
		/// D�finit la session � utiliser
		/// </summary>
		public virtual WebSession CustomerWebSession {
			set{webSession = value;}
		}
		
		/// <summary>
		/// D�finit l'�v�nement qui a �t� lancer
		/// </summary>
		public virtual int EventButton {
			set{eventButton = value;}
		}
		
		/// <summary>
		/// Obtient ou d�finit la valeur qui permet de savoir si des �l�ments ont
		/// �t� s�lectionn�s
		/// </summary>
		public int NbElement{
			get{return nbElement;}
			set{nbElement=value;}
		}
		
		#endregion

		#region Ev�nements

		#region Chargement 
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e">argument</param>
		protected override void OnLoad(EventArgs e) {			
			
			#region Creation du dataSet
			if(webSession != null) {
				//Chargement du DataSet des M�dias
				if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
					dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleInterestCenterMediaListDataAccess(webSession,GetIdVehicle(webSession).ToString(),webSession.GetSelection(webSession.CurrentUniversMedia,RightConstantes.type.interestCenterAccess),webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.mediaAccess));		
				}else
				dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleInterestCenterMediaListDataAccess(webSession,GetIdVehicle(webSession).ToString());		
			}
			else throw (new WebControlInitializationException("Impossible d'initialiser le composant, la session n'est pas d�finie."));				
			#endregion
			
			if(dsListMedia.Equals(System.DBNull.Value) || dsListMedia.Tables[0]==null || dsListMedia.Tables[0].Rows.Count==0)
			hasRecords=false;

			#region script 			
			// Cochage/Decochage des checkbox p�res, fils et concurrents
			if (!Page.ClientScript.IsClientScriptBlockRegistered("CheckAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"CheckAllChilds",TNS.AdExpress.Web.Functions.Script.CheckAllChilds());
			}
			// Ouverture/fermeture des fen�tres p�res
			if (!Page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
			}
			// fermer/ouvrir tous les calques
			if (!Page.ClientScript.IsClientScriptBlockRegistered("ExpandColapseAllDivs")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ExpandColapseAllDivs",TNS.AdExpress.Web.Functions.Script.ExpandColapseAllDivs());
			}	
			// S�lection de tous les fils
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SelectAllChilds",TNS.AdExpress.Web.Functions.Script.SelectAllChilds());
			}	
			// S�lection de tous les fils
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectExclusiveAllChilds")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"SelectExclusiveAllChilds",TNS.AdExpress.Web.Functions.Script.SelectExclusiveAllChilds());
			}					
			#endregion
		}
		#endregion

		#region Prerender
		/// <summary>
		/// Contruction des �l�ments du cheboxlist
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnPreRender(EventArgs e){		
				
			#region Variables locales
				//Variables media
				Int64 IdVehicle = 0;
				Int64 oldIdVehicle = 0;
				
				//Variables centre d'int�r�ts
				Int64 IdInterestCenter = 0;
				Int64 oldIdInterestCenter = 0;				
				#endregion
				
			//Initialisation de la liste d'items
			this.Items.Clear();
				
			//Construction de la liste de checkbox				
			foreach(DataRow currentRow in dsListMedia.Tables[0].Rows) {	
				if(showMedia((Int64)currentRow["id_vehicle"])){
					//Ajout d'un m�dia
					if ((IdVehicle = (Int64)currentRow["id_vehicle"]) != oldIdVehicle){							
						oldIdVehicle = IdVehicle;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["vehicle"].ToString(),"vh_"+IdVehicle));
					}
					//Ajout d'un centre d'intr�ts
					if ( ((IdInterestCenter  = (Int64)currentRow["id_interest_center"]) != oldIdInterestCenter )) {
						oldIdInterestCenter = IdInterestCenter ;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["interest_center"].ToString(),"ic_"+(Int64)currentRow["id_interest_center"]));
					}
					//Ajout d'un support
					this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["media"].ToString(),"md_"+(Int64)currentRow["id_media"]));							
				}
			}
												
		}
		#endregion

		#region Render 
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output) {
			System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
			
			#region Traitement des sauvegardes et chargement des m�dia
			IList savedVehicle=null;
			IList savedInterestCenter=null;
			IList savedMedia=null;
			string delimStr = ",";
			char [] delimiter = delimStr.ToCharArray();
			string displayVh="";
			string displayIc="none";
			int insertIndex = 0;
			string icList="";		
						
			if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
				//M�dia sauvegard�s 
				savedVehicle = new ArrayList();
				try{
					savedVehicle  = webSession.GetSelection(webSession.CurrentUniversMedia,RightConstantes.type.vehicleAccess).Split(delimiter);		
				}catch(Exception){} 
				//Centres d'int�r�ts sauvegard�s
				 savedInterestCenter = new ArrayList();
				try{
					savedInterestCenter  = webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.interestCenterAccess).Split(delimiter);		
				}catch(Exception){} 
				//supports sauvegard�s
				savedMedia = new ArrayList();
				try{
					savedMedia  = webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.mediaAccess).Split(delimiter);		
				}catch(Exception){}
			}		
			
			#endregion

			#region Affichage � partir du groupe de donn�es (DataSet)			
			if(dsListMedia != null && dsListMedia.Tables[0].Rows.Count >0) {
				
				#region variables locales				
				//variables du niveau  Media
				Int64 idVehicleOld=-1;
				Int64 idVehicle;										
				int startVehicle=-1;	
				string vhSeparator="";
				string VehicleIds="";			
				string 	vhlist="";	
				//Chargement de la liste des m�dia(vehicle) dans l'arbre SelectionUniversMedia si n�cessaire (post-enregistrement) et de leur index dans l'arbre
				Hashtable vehicles = null;
				if (LoadSession){
					vehicles = LoadChilds(webSession.SelectionUniversMedia);
				}
				int indexVh = -1;
				//variables du niveau centre d'int�r�ts
				Int64 idInterestCenterOld=-1;
				Int64 idInterestCenter;										
				int startInterestCenter=-1;	
				Hashtable interestCenters = null;
				int indexIc	=-1;
					
				//variables du niveau Support
				Int64 i = 0;				
				int numColumn = 0;
				Hashtable medias = null;	
				//Variables �tat des checkbox	
				string disabled="none";
				string checkBox="";
				string checkVh="";	
				string checkIc="";		
				#endregion

				//R�cup�ration de la liste des m�dia autoris�s pour l'utilisateur courant
				getListIdVehicle(ref vhlist, ref VehicleIds, vhSeparator);				
				//Tableau global 
				t.Append("\n<tr vAlign=\"top\" height=\"1%\">\n<td bgColor=\"#ffffff\">\n");	
				t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
				insertIndex = t.Length;
				t.Append("')\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;"+GestionWeb.GetWebWord(1617,webSession.SiteLanguage)+"</a>");									

				#region Foreach  Dataset des m�dias

				foreach(DataRow currentRow in dsListMedia.Tables[0].Rows) {	
					//Initialisation des identifiants parents
					idVehicle=(Int64)currentRow["id_vehicle"];
					idInterestCenter=(Int64)currentRow["id_interest_center"];
					
					if(showMedia(idVehicle)){ //&& showSavedVehicle(savedVehicle,parentVehicle,idVehicle.ToString(),eventButton)){
						#region contruction tableau principal
						//Fermeture centre d'int�r�t			
						if (idInterestCenter!= idInterestCenterOld && startInterestCenter==0 ) {
							//Fermeture support							
							if (numColumn!=0)t.Append("</tr>");						
							t.Append("</table>");							
							t.Append("</td></tr></table></div></DIV></td></tr>");										
						}
						//Fermeture Media				
						if (idVehicle!= idVehicleOld && startVehicle==0 ) {
							startInterestCenter=-1;
							t.Append("\n</TD></TR></TABLE></Div>");
							t.Append("\n</td></tr></table></div></td></tr>");
						}
											
						#region Nouveau Media
						//Nouveau Media
						if (idVehicle!= idVehicleOld) {							
							//bordure du haut de tableau						
							t.Append("\n<tr><td><div style=\"MARGIN-LEFT: 10px;  \" id=\"vh_"+idVehicle+"\" >");
							if (idVehicleOld == -1)t.Append("\n<table bgColor=\"#ffffff\" style=\"border-top :#644883 1px solid; border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n");
							else t.Append("\n<table bgColor=\"#ffffff\" style=\"border-bottom :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=\"650\">\n");

							//Curseur sur toute la ligne
							t.Append("\n<tr  style=\"cursor : hand\">");
																		
							#region Etat de cochage du m�dia(vehicle)	
							try{
								checkVh = checkBox = "";
								disabled="";																
								//V�rification de l'�tat du m�dia (s�lectionner ou non)
								if(webSession.SelectionUniversMedia.Nodes[(int)vehicles[idVehicle]].Checked){
									checkVh = checkBox = " Checked ";
								}
									//Gestion �l�ments sauvegard�s
								else if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedVehicle!=null && savedVehicle.Contains(idVehicle.ToString()))
										checkVh = checkBox = " Checked ";
								}
								//Construction de la liste de ses fils present dans l 'arbre
								indexVh = (int)vehicles[idVehicle];
								interestCenters = LoadChilds(webSession.SelectionUniversMedia.Nodes[(int)vehicles[idVehicle]]);
								
							}
								//Exceptions si le centre d'int�r�t n'est pas trouver dans l'arbre
							catch(System.NullReferenceException){
								interestCenters  = null;
								//Gestion �l�ments sauvegard�s
								if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedVehicle!=null && savedVehicle.Contains(idVehicle.ToString()))
										checkVh = checkBox = " Checked ";
								}
							}
							#endregion						
							idVehicleOld=idVehicle;
							startVehicle=0;
					
							//checkbox m�dia 
							t.Append("\n<td align=\"left\" height=\"10\" valign=\"top\" >");
						
							t.Append("\n<input type=checkbox name=\"MediaSelectionWebControl1$"+i+"\" id=\"MediaSelectionWebControl1_"+i+"\" "+checkBox+"  onClick=\"CheckAllChilds('vh_"+idVehicle+"','"+VehicleIds+"','vh_"+idVehicle+"')\" value=vh_"+idVehicle+"></td>");																											

							//fin libell� m�dia (nouvelle version)														
							t.Append("<td width=100% align=\"left\" onClick=\"javascript : DivDisplayer('"+"vh"+idVehicle+"');\" >"+currentRow["vehicle"].ToString()+"");
							t.Append("</td><td align=\"right\" onClick=\"javascript : DivDisplayer('"+"vh"+idVehicle+"');\">");
							t.Append("<IMG src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\">");
							t.Append("</td></tr>");							
							t.Append("<tr><td colspan=\"3\" >");
							if(eventButton==constEvent.eventSelection.LOAD_EVENT)
								displayVh="";
							t.Append("<Div  style=\"display ='"+displayVh+"'\" id=\"vh"+idVehicle+"\">");							
							t.Append("<TABLE bgcolor=\"#B1A3C1\" cellpadding=0 cellspacing=0 width=100% style=\"border-top :#644883 1px solid; border-bottom :#644883 0px solid; border-left :#644883 0px solid; \" class=\"txtViolet11Bold\">");							
							t.Append("<tr><td class=\"roll04\"><a href=\"javascript: SelectExclusiveAllChilds('vh_"+idVehicle+"','"+VehicleIds+"','vh_"+idVehicle+"','vh_','ic_')\" title=\""+GestionWeb.GetWebWord(1540,webSession.SiteLanguage)+"\" class=\"roll04\">&nbsp;&nbsp;"+GestionWeb.GetWebWord(1540,webSession.SiteLanguage)+"</td></tr>");							
							t.Append("<TR><TD>");							
							i++;	
						}
						#endregion

						#region Nouveau centre d'int�r�ts
						if (idInterestCenter!=idInterestCenterOld ) {	
							numColumn=0;															
							if(startInterestCenter== -1)t.Append("<tr><td ><div style=\" display ='"+displayIc+"' MARGIN-LEFT: 5px; width=100%; background-color:#B1A3C1;\"  id=\"ic_"+idInterestCenter+"\"><table style=\"background-color:#B1A3C1; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
							else t.Append("<tr><td><div style=\"display ='"+displayIc+"' MARGIN-LEFT: 5px; width=100%; background-color:#B1A3C1;\"  id=\"ic_"+idInterestCenter+"\"><table style=\"background-color:#B1A3C1; border-top :#644883 1px solid;\" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");

							//Curseur sur toute la ligne
							t.Append("\n<tr  style=\"cursor : hand\">");					

							idInterestCenterOld=idInterestCenter;
							startInterestCenter=0;
							t.Append("<td  align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");
							
							#region Gestion de l'etat de cochage du centre d'int�r�ts
							//si le centre d'int�r�ts est dans la liste de centre d'int�r�ts de l'arbre m�dia
							try{
								checkBox =checkIc="";
								disabled="";
								if (checkVh.Length>0){
									checkBox = checkIc = " Checked ";
									disabled="disabled";
								}else{
									//V�rification de l'�tat du centre d'int�r�ts (s�lectionner ou non)
									if(webSession.SelectionUniversMedia.Nodes[indexVh].Nodes[(int)interestCenters[(Int64)currentRow["id_interest_center"]]].Checked){
										checkBox = checkIc = " Checked ";
										if (checkVh.Length>0)disabled="disabled";
									}//Gestion �l�ments sauvegard�s
									else if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
										if(savedInterestCenter!=null && savedInterestCenter.Contains(currentRow["id_interest_center"].ToString())){
											checkBox = checkIc = " Checked ";	
											//ouverture du calque
											if(checkIc.Equals(" Checked "))
												displayIc="";
										}
										if (checkVh.Length>0)disabled="disabled";
										
									}
									//Construction de la liste de ses fils present dans l 'arbre
									indexIc = (int)interestCenters[idInterestCenter];
									medias = LoadChilds(webSession.SelectionUniversMedia.Nodes[indexVh].Nodes[(int)interestCenters[(Int64)currentRow["id_interest_center"]]]);
								}
							}
								//Exceptions si le centre d'int�r�ts  n'est pas trouver dans l'arbre
							catch(System.ArgumentOutOfRangeException) {
								if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedInterestCenter!=null && savedInterestCenter.Contains(currentRow["id_interest_center"].ToString())){
										checkBox = checkIc = " Checked ";	
										//ouverture du calque
										if(checkIc.Equals(" Checked "))displayIc="";
									}
									if (checkVh.Length>0)disabled="disabled";								
									
								}
								if (checkVh.Length>0){
									checkBox=" Checked ";
									disabled="disabled";
								}
							}
							catch(System.NullReferenceException){
								if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedInterestCenter!=null && savedInterestCenter.Contains(currentRow["id_interest_center"].ToString())){
										checkBox = checkIc = " Checked ";
										//ouverture du calque
										if(checkIc.Equals(" Checked "))displayIc="";
									}
									if (checkVh.Length>0)disabled="disabled";
									
								}
								if (checkVh.Length>0){
									checkBox=" Checked ";
									disabled="disabled";
								}
								medias =null;
							}
							#endregion

							// checkbox choix centre d'int�r�ts
							t.Append("<input type=checkbox name=\"MediaSelectionWebControl1$"+i+"\" id=\"MediaSelectionWebControl1_"+i+"\" "+checkBox+" "+disabled+" onClick=\"CheckAllChilds('ic_"+idInterestCenter+"','"+VehicleIds+"','vh_"+idVehicle+"')\" value=\"ic_"+idInterestCenter+"\">");
							t.Append("\n</td>\n");
							//Libell� centre d'int�r�ts
							t.Append("<td width=100% valign=\"middle\" height=\"10\" onClick=\"javascript : DivDisplayer('"+"ic"+idInterestCenter+"');\" align=\"left\"  ");
							t.Append(">&nbsp;"+currentRow["interest_center"].ToString()+"</td>");
							t.Append("<td valign=\"baseline\"  onClick=\"javascript : DivDisplayer('"+"ic"+idInterestCenter+"');\"   ");
							t.Append(">&nbsp;<IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\" align=\"right\"></td>");							
							t.Append("</tr><tr><td colspan=\"3\"><DIV style=\" display ='"+displayIc+"' \" id=\"ic"+idInterestCenter+"\" ><table cellpadding=0 cellspacing=0 border=\"0\" width=100%>");							
							t.Append("<table cellpadding=0 cellspacing=0 border=\"0\" width=100% style=\"background-color:#D0C8DA; border-top :#ffffff 1px solid; \"  class=\"txtViolet10\">");	
							t.Append("<tr><td colspan=\"3\" class=\"roll04\" ><a href=\"javascript: SelectExclusiveAllChilds('ic_"+idInterestCenter+"','"+VehicleIds+"','vh_"+idVehicle+"','vh_','ic_')\" title=\""+GestionWeb.GetWebWord(1066,webSession.SiteLanguage)+"\" class=\"roll04\">&nbsp;"+GestionWeb.GetWebWord(1066,webSession.SiteLanguage)+"</a></td></tr>");							
							i++;
							icList=icList+(icList.Length>0?",":"")+"ic"+idInterestCenter ;
							displayIc="none";
						}							
						#endregion

						#region Affichage des supports

						#region Gestion de l'etat de cochage du support
						//si le  support est dans la liste de support de l'arbre m�dia
						try{
							checkBox = "";
							disabled="";
							//V�rification de l'�tat du support (s�lectionner ou non)
							if(checkVh.Length>0 || checkIc.Length>0){
								checkBox = checkIc = " Checked ";
								disabled="disabled";
							}else{
								if(webSession.SelectionUniversMedia.Nodes[indexVh].Nodes[(int)interestCenters[(Int64)currentRow["id_interest_center"]]].Nodes[(int)medias[(Int64)currentRow["id_media"]]].Checked){
									checkBox = " Checked ";
									if(checkVh.Length>0 || checkIc.Length>0)disabled="disabled";
								}//Gestion �l�ments sauvegard�s
								else if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedMedia!=null && savedMedia.Contains(currentRow["id_media"].ToString()))
										checkBox = " Checked ";
									if (checkVh.Length>0 || checkIc.Length>0)disabled="disabled";
								}
							}
						}
							//Exceptions si le groupe n'est pas trouver dans l'arbre
						catch(System.ArgumentOutOfRangeException) {
							if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
								if(savedMedia!=null && savedMedia.Contains(currentRow["id_media"].ToString()))
									checkBox = " Checked ";
								if (checkVh.Length>0 || checkIc.Length>0)disabled="disabled";
							}
							if (checkVh.Length>0 || checkIc.Length>0)checkBox=" Checked ";
						}
						catch(System.NullReferenceException){
							if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
								if(savedMedia!=null && savedMedia.Contains(currentRow["id_media"].ToString()))
									checkBox = " Checked ";
								if (checkVh.Length>0 || checkIc.Length>0)disabled="disabled";
							}
							if (checkVh.Length>0 || checkIc.Length>0)checkBox=" Checked ";
						}
						#endregion

						if(numColumn==0) {								
							t.Append("<tr >");
							t.Append("<td  width=\"33%\">");								
							t.Append("<input type=\"checkbox\" "+checkBox+" "+disabled+" name=\"MediaSelectionWebControl1$"+i+"\" id=\"MediaSelectionWebControl1_"+i+"\" value=\"md_"+currentRow["media"]+"\"   onClick=\"CheckAllChilds('md_"+currentRow["id_media"]+"','"+VehicleIds+"','vh_"+idVehicle+"')\">"+currentRow["media"].ToString());
							t.Append("</td>");								
							numColumn++;
							i++;					
						}
						else if(numColumn==1 ) {
							t.Append("<td  width=\"33%\">");								
							t.Append("<input type=\"checkbox\" "+checkBox+" "+disabled+" name=\"MediaSelectionWebControl1$"+i+"\"  id=\"MediaSelectionWebControl1_"+i+"\" value=\"md_"+currentRow["media"]+"\"   onClick=\"CheckAllChilds('md_"+currentRow["id_media"]+"','"+VehicleIds+"','vh_"+idVehicle+"')\">"+currentRow["media"].ToString());
							t.Append("</td>");								
							numColumn++;
							i++;
						}
						else {
							t.Append("<td  width=\"33%\">");								
							t.Append("<input type=\"checkbox\" name=\"MediaSelectionWebControl1$"+i+"\" "+checkBox+" "+disabled+" onClick=\"CheckAllChilds('md_"+currentRow["id_media"]+"','"+VehicleIds+"','vh_"+idVehicle+"')\"  value=\"md_"+currentRow["media"]+"\" id=\"MediaSelectionWebControl1_"+i+"\" >"+currentRow["media"].ToString());
							t.Append("</td></tr>");
							numColumn=0;
							i++;
						}					
						#endregion
					
						#endregion
					}					
				}
				#endregion

				//Fermeture Tableau global												
				t.Append(" \n\n<tr> \n\n</table></td></tr></table></DIV></div></td></tr>");				
				t.Append(" \n\n</td></tr></table></div></td></tr>");
				t.Append(" </TD></TR></TABLE></Div> \n\n</td>\n</tr>\n ");
			}
			else {				
				output.Write("<div align=\"center\" class=\"txtGris11Bold\">"+GestionWeb.GetWebWord(1091,webSession.SiteLanguage)
					+"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">"
					+"<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a>"
					+"</div><br>");
			}		
			t.Insert(insertIndex,icList);

			output.Write(t.ToString());
			#endregion
		}
		#endregion

		#endregion
		
		#region Methodes internes
		
		#region Construction d'une liste de noeuds enfants 
		/// <summary>
		/// Construction d'une liste de noeuds enfants 
		/// </summary>
		/// <param name="tree">arbre</param>
		/// <returns>liste des fils d'un noeud</returns>
        private Hashtable LoadChilds(System.Windows.Forms.TreeNode tree)
        {
			Hashtable t = new Hashtable();
			foreach(System.Windows.Forms.TreeNode node in tree.Nodes){
				t.Add(((LevelInformation)node.Tag).ID,node.Index);
			}
			return t;
		}
		#endregion

		/// <summary>
		/// Indique si le niveau m�dia(support) doit �tre montrer pour le vehicle
		/// </summary>
		/// <param name="idVehicle">M�dia � traiter</param>
		/// <returns>True s'il doit �tre montrer, false sinon</returns>
		private bool showMedia(Int64 idVehicle) {
			DBConstantesClassification.Vehicles.names vehicletype=(DBConstantesClassification.Vehicles.names)int.Parse(idVehicle.ToString());
			switch(vehicletype) {
				case DBConstantesClassification.Vehicles.names.radio:
				case DBConstantesClassification.Vehicles.names.press:
				case DBConstantesClassification.Vehicles.names.internationalPress:
				case DBConstantesClassification.Vehicles.names.tv:	
				case DBConstantesClassification.Vehicles.names.others:	
					return(true);
				default:
					return(false);
			}
		}
		
		/// <summary>
		/// Indique si le niveau m�dia(vehcile) doit �tre montrer pour le vehicle.		
		/// </summary>
		/// <param name="savedVehicle">m�dia sauvegard�</param>		
		/// <param name="idVehicle">m�dia</param>
		/// <param name="parentVehicle">vehicle parent</param>
		/// <param name="eventButton">evenement</param>
		/// <returns>Vrai si le vehicle est charg� depuis l'univers sauvegard�</returns>
		private static bool showSavedVehicle(IList savedVehicle,IList parentVehicle, string idVehicle,int eventButton){
			bool isShow=false;
			if(eventButton==constEvent.eventSelection.LOAD_EVENT){
				if(savedVehicle!=null && savedVehicle[0].ToString().Length>0){
					if(savedVehicle.Contains(idVehicle))isShow=true;
					else isShow=false;
				}
				else if(parentVehicle!=null && parentVehicle[0].ToString().Length>0){
					if(parentVehicle.Contains(idVehicle))isShow=true;
					else isShow=false;
				}
				else isShow=false;
			}else isShow=true;
			return isShow;
		}

		/// <summary>
		/// R�cup�re la liste des m�dias (vehicle) autoris�s pour l'utilisateur courant
		/// </summary>
		/// <param name="vhlist">liste des m�dias (vehicle)</param>
		/// <param name="VehicleIds">Identifiants des m�dias </param>
		/// <param name="vhSeparator">separateur</param>
		private void getListIdVehicle(ref string vhlist, ref string  VehicleIds,string vhSeparator){
			//R�cup�ration de la liste des m�dia autoris�s pour l'utilisateur courant
			if(webSession.CustomerLogin.getListIdVehicle().Length>0){				
				int c=0;
				string[] VehicleIdsArr = webSession.CustomerLogin.getListIdVehicle().Split(',');
				for(int v=0;v<VehicleIdsArr.Length;v++){
					if(v>0)vhSeparator="-";
					VehicleIds+=vhSeparator+"vh_"+VehicleIdsArr[v].ToString();
					if(c==0)vhlist+="vh"+VehicleIdsArr[v].ToString();
					else vhlist+=",vh"+VehicleIdsArr[v].ToString();
					c++;
				}				
			}
			else  throw (new WebControlInitializationException("Impossible d'initialiser le composant, aucun m�dia n'est accessible."));				

		}
		
		/// <summary>
		/// Donne le media (vehicle) � afficher
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <returns></returns>
		private Int64 GetIdVehicle(WebSession webSession){
			switch(webSession.CurrentModule){
				case Module.Name.TABLEAU_DE_BORD_PRESSE :
					return Int64.Parse(DBConstantesClassification.Vehicles.names.press.GetHashCode().ToString());
				case Module.Name.TABLEAU_DE_BORD_RADIO :
					return Int64.Parse(DBConstantesClassification.Vehicles.names.radio.GetHashCode().ToString());
				case Module.Name.TABLEAU_DE_BORD_TELEVISION :
					return Int64.Parse(DBConstantesClassification.Vehicles.names.tv.GetHashCode().ToString());
				case Module.Name.TABLEAU_DE_BORD_PAN_EURO :
					return Int64.Parse(DBConstantesClassification.Vehicles.names.others.GetHashCode().ToString());
				default :
					throw (new MediaSelectionWebControlException(" GetIdVehicle(WebSession webSession) : Impossible d'identifier le module s�lectionn�."));				
			}			
		}
		
		#endregion
	}
}
