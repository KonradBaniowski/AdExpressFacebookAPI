#region Informations
// Auteur: D. V. Mussuma 
// Date de cr�ation: 01/03/2005 
// Date de modification: 01 /03/2005 
#endregion

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

using Oracle.DataAccess.Client;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Controls.Exceptions;
using TNS.AdExpress.Web.DataAccess.Selections.Medias;
using TNS.AdExpress.Web.Core.Sessions;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Domain.Translation;
using DBConstantesClassification=TNS.AdExpress.Constantes.Classification.DB;
using VhCstes=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using TableName=TNS.AdExpress.Constantes.Classification.DB.Table.name;
using constEvent=TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;

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
					//dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleInterestCenterMediaListDataAccess(webSession,GetIdVehicle(webSession).ToString(),webSession.GetSelection(webSession.CurrentUniversMedia,RightConstantes.type.interestCenterAccess),webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.mediaAccess));
					dsListMedia = TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleInterestCenterMediaListDataAccess(webSession, webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.interestCenterAccess), webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.mediaAccess));		
				}else
				//dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleInterestCenterMediaListDataAccess(webSession,GetIdVehicle(webSession).ToString());	
					dsListMedia = TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.VehicleInterestCenterMediaListDataAccess(webSession);		
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
		/// Bulid cheboxlist box items
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnPreRender(EventArgs e){		
				
			#region Variables 
				//Variables 
				Int64 IdVehicle = 0;
				Int64 oldIdVehicle = 0;								
				Int64 IdInterestCenter = 0;
				Int64 oldIdInterestCenter = 0;				
				#endregion
				
			//Item list initialization 
			this.Items.Clear();
				
			//Inset items into check box list 				
			foreach(DataRow currentRow in dsListMedia.Tables[0].Rows) {	
				if(showMedia((Int64)currentRow["id_vehicle"])){
					//Add vehcile
					if ((IdVehicle = (Int64)currentRow["id_vehicle"]) != oldIdVehicle){							
						oldIdVehicle = IdVehicle;
						this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["vehicle"].ToString(),"vh_"+IdVehicle));
					}
					//Add interest center
					if ( ((IdInterestCenter  = (Int64)currentRow["id_interest_center"]) != oldIdInterestCenter )) {
						oldIdInterestCenter = IdInterestCenter ;
                        this.Items.Add(new System.Web.UI.WebControls.ListItem(currentRow["interest_center"].ToString(), "" + IdVehicle + "ic_" + (Int64)currentRow["id_interest_center"]));
					}
					//Add media
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
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			
			#region Traitement des sauvegardes et chargement des m�dia
			List<Int64> savedVehicle = null, savedInterestCenter = null, savedMedia = null;			
			string delimStr = ",";
			char [] delimiter = delimStr.ToCharArray();
			string displayVh="";
			string displayIc="none";
			int insertIndex = 0;
			string icList="";		
						
			if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
				//Vehicle saved 
				savedVehicle = new List<Int64>();
				try{
					savedVehicle = new List<Int64>(Array.ConvertAll<string, Int64>(webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.vehicleAccess).Split(delimiter), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
				}catch(Exception){} 
				//Interest center saved
				savedInterestCenter = new List<Int64>();
				try{
					savedInterestCenter = new List<Int64>(Array.ConvertAll<string, Int64>(webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.interestCenterAccess).Split(delimiter), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));					
				}catch(Exception){} 
				//Media saved
				savedMedia = new List<Int64>();
				try{
					savedMedia = new List<Int64>(Array.ConvertAll<string, Int64>(webSession.GetSelection(webSession.CurrentUniversMedia, RightConstantes.type.mediaAccess).Split(delimiter), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));					
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
				Dictionary<long, int> vehicles = null;
				if (LoadSession){
					vehicles = LoadChilds(webSession.SelectionUniversMedia);
				}
				int indexVh = -1;
				//variables du niveau centre d'int�r�ts
				Int64 idInterestCenterOld=-1;
				Int64 idInterestCenter;										
				int startInterestCenter=-1;
				Dictionary<long, int> interestCenters = null;
				int indexIc	=-1;
                string classBorder = string.Empty;
					
				//variables du niveau Support
				Int64 i = 0;				
				int numColumn = 0;
				Dictionary<long, int> medias = null;	
				//Variables �tat des checkbox	
				string disabled="none";
				string checkBox="";
				string checkVh="";	
				string checkIc="";		
				#endregion

				//R�cup�ration de la liste des m�dia autoris�s pour l'utilisateur courant
				getListIdVehicle(ref vhlist, ref VehicleIds, vhSeparator);				
				//Tableau global 
                t.Append("\n<tr vAlign=\"top\" height=\"1%\">\n<td>\n");	
				t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
				insertIndex = t.Length;
				t.Append("')\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;"+GestionWeb.GetWebWord(1617,webSession.SiteLanguage)+"</a>");									

				#region Foreach  Dataset des m�dias

				foreach(DataRow currentRow in dsListMedia.Tables[0].Rows) {	
					//Initialisation des identifiants parents
					idVehicle=(Int64)currentRow["id_vehicle"];
					idInterestCenter=(Int64)currentRow["id_interest_center"];
					
					if(showMedia(idVehicle)){ 
						#region contruction tableau principal
						//Fermeture centre d'int�r�t			
						if (idInterestCenter!= idInterestCenterOld && startInterestCenter==0 ) {
							//Close media							
							if (numColumn!=0)t.Append("</tr>");						
							t.Append("</table>");							
							t.Append("</td></tr></table></div></DIV></td></tr>");										
						}
						//Close vehicle				
						if (idVehicle!= idVehicleOld && startVehicle==0 ) {
							startInterestCenter=-1;
							t.Append("\n</TD></TR></TABLE></Div>");
							t.Append("\n</td></tr></table></div></td></tr>");
						}
											
						#region New vehicle
						//New vehicle
						if (idVehicle!= idVehicleOld) {							
							//bordure du haut de tableau						
							t.Append("\n<tr><td><div style=\"MARGIN-LEFT: 10px;  \" id=\"vh_"+idVehicle+"\" >");

                            if (idVehicleOld == -1) classBorder = "violetBorder";
                            else classBorder = "violetBorderWithoutTop";

                            t.Append("\n<table class=\"txtViolet11Bold backGroundWhite\"  cellpadding=0 cellspacing=0 width=\"650\">\n");

							//Curseur sur toute la ligne
							t.Append("\n<tr  style=\"cursor : hand\"><td>");
																		
							#region Check box vehicle state	
							try{
								checkVh = checkBox = "";
								disabled="";																
								//V�rification de l'�tat du m�dia (s�lectionner ou non)
								if(webSession.SelectionUniversMedia.Nodes[vehicles[idVehicle]].Checked){
									checkVh = checkBox = " Checked ";
								}
									//Gestion �l�ments sauvegard�s
								else if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedVehicle!=null && savedVehicle.Contains(idVehicle))
										checkVh = checkBox = " Checked ";
								}
								//Construction de la liste de ses fils present dans l 'arbre
								indexVh = vehicles[idVehicle];
								interestCenters = LoadChilds(webSession.SelectionUniversMedia.Nodes[vehicles[idVehicle]]);
								
							}
								//Exceptions si le centre d'int�r�t n'est pas trouver dans l'arbre
                            catch (System.NullReferenceException){
								interestCenters  = null;
								//Gestion �l�ments sauvegard�s
								if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedVehicle!=null && savedVehicle.Contains(idVehicle))
										checkVh = checkBox = " Checked ";
								}
							}
                            catch (System.Collections.Generic.KeyNotFoundException) {
                                interestCenters = null;
                                //Gestion �l�ments sauvegard�s
                                if (eventButton == constEvent.eventSelection.LOAD_EVENT || eventButton == constEvent.eventSelection.SAVE_EVENT) {
                                    if (savedVehicle != null && savedVehicle.Contains(idVehicle))
                                        checkVh = checkBox = " Checked ";
                                }
                            }
							#endregion						
							idVehicleOld=idVehicle;
							startVehicle=0;

                            t.Append("<Div class=\"" + classBorder + "\" style=\"width:100%;\">");
                            t.Append("\n<table class=\"txtViolet11Bold backGroundWhite\"  cellpadding=0 cellspacing=0 width=\"100%\">\n");
                            t.Append("<tr><td>");
					
							//checkbox m�dia 
                            t.Append("\n<td align=\"left\" height=\"10\" valign=\"top\">");
						
							t.Append("\n<input type=checkbox name=\"MediaSelectionWebControl1$"+i+"\" id=\"MediaSelectionWebControl1_"+i+"\" "+checkBox+"  onClick=\"CheckAllChilds('vh_"+idVehicle+"','"+VehicleIds+"','vh_"+idVehicle+"')\" value=vh_"+idVehicle+"></td>");																											

							//fin libell� m�dia (nouvelle version)														
                            t.Append("<td width=100% align=\"left\" onClick=\"javascript : DivDisplayer('" + "vh" + idVehicle + "');\" >" + currentRow["vehicle"].ToString() + "");
                            t.Append("</td><td align=\"right\" onClick=\"javascript : DivDisplayer('" + "vh" + idVehicle + "');\">");
                            t.Append("<img src=\"/App_Themes/" + themeName + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\">");

                            t.Append("</td></tr></table></div>");

							t.Append("</td></tr>");			
				


							t.Append("<tr><td>");
							if(eventButton==constEvent.eventSelection.LOAD_EVENT)
								displayVh="";
                            t.Append("<Div class=\"violetBorderWithoutTop\" style=\"width:100%;display ='" + displayVh + "'\" id=\"vh" + idVehicle + "\">");
                            t.Append("<TABLE class=\"violetBackGroundV3\" cellpadding=0 cellspacing=0 width=100% class=\"txtViolet11Bold violetBorderTop\">");							
							t.Append("<tr><td class=\"roll04\"><a href=\"javascript: SelectExclusiveAllChilds('vh_"+idVehicle+"','"+VehicleIds+"','vh_"+idVehicle+"','vh_','ic_')\" title=\""+GestionWeb.GetWebWord(1540,webSession.SiteLanguage)+"\" class=\"roll04\">&nbsp;&nbsp;"+GestionWeb.GetWebWord(1540,webSession.SiteLanguage)+"</td></tr>");							
							t.Append("<TR><TD>");							
							i++;	
						}
						#endregion

						#region New interest center
						if (idInterestCenter!=idInterestCenterOld ) {	
							numColumn=0;
                            if (startInterestCenter == -1) t.Append("<tr><td ><div style=\" display ='" + displayIc + "' MARGIN-LEFT: 5px; width=100%;\" class=\"violetBackGroundV3\"  id=\""+idVehicle+"ic_" + idInterestCenter + "\"><table class=\"txtViolet11Bold violetBackGroundV3\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");
                            else t.Append("<tr><td><div style=\"display ='" + displayIc + "' MARGIN-LEFT: 5px; width=100%;\" class=\"violetBackGroundV3\"  id=\"" + idVehicle + "ic_" + idInterestCenter + "\"><table class=\"txtViolet11Bold violetBackGroundV3 violetBorderTop\"  cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\">");

							//Curseur sur toute la ligne
							t.Append("\n<tr  style=\"cursor : hand\">");					

							idInterestCenterOld=idInterestCenter;
							startInterestCenter=0;
							t.Append("<td  align=\"left\" height=\"10\" valign=\"middle\" class=\"txtGroupViolet11Bold\">");

							#region Check box interest center state
							//si le centre d'int�r�ts est dans la liste de centre d'int�r�ts de l'arbre m�dia
							try{
								checkBox =checkIc="";
								disabled="";
								if (checkVh.Length>0){
									checkBox = checkIc = " Checked ";
									disabled="disabled";
								}else{
									//V�rification de l'�tat du centre d'int�r�ts (s�lectionner ou non)
									if(webSession.SelectionUniversMedia.Nodes[indexVh].Nodes[interestCenters[Int64.Parse(currentRow["id_interest_center"].ToString())]].Checked){
										checkBox = checkIc = " Checked ";
										if (checkVh.Length>0)disabled="disabled";
									}//Gestion �l�ments sauvegard�s
									else if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
										if (savedInterestCenter != null && savedInterestCenter.Contains(Int64.Parse(currentRow["id_interest_center"].ToString()))) {
											checkBox = checkIc = " Checked ";	
											//ouverture du calque
											if(checkIc.Equals(" Checked "))
												displayIc="";
										}
										if (checkVh.Length>0)disabled="disabled";
										
									}
									//Construction de la liste de ses fils present dans l 'arbre
									indexIc = interestCenters[idInterestCenter];
									medias = LoadChilds(webSession.SelectionUniversMedia.Nodes[indexVh].Nodes[interestCenters[Int64.Parse(currentRow["id_interest_center"].ToString())]]);
								}
							}
								//Exceptions si le centre d'int�r�ts  n'est pas trouver dans l'arbre
							catch(System.ArgumentOutOfRangeException) {
								if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedInterestCenter!=null && savedInterestCenter.Contains(Int64.Parse(currentRow["id_interest_center"].ToString()))){
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
                            catch (System.Collections.Generic.KeyNotFoundException) {
                                if (eventButton == constEvent.eventSelection.LOAD_EVENT || eventButton == constEvent.eventSelection.SAVE_EVENT) {
                                    if (savedInterestCenter != null && savedInterestCenter.Contains(Int64.Parse(currentRow["id_interest_center"].ToString()))) {
                                        checkBox = checkIc = " Checked ";
                                        //ouverture du calque
                                        if (checkIc.Equals(" Checked ")) displayIc = "";
                                    }
                                    if (checkVh.Length > 0) disabled = "disabled";

                                }
                                if (checkVh.Length > 0) {
                                    checkBox = " Checked ";
                                    disabled = "disabled";
                                }
                                medias = null;
                            }
							catch(System.NullReferenceException){
								if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedInterestCenter!=null && savedInterestCenter.Contains(Int64.Parse(currentRow["id_interest_center"].ToString()))){
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
                            t.Append("<input type=checkbox name=\"MediaSelectionWebControl1$" + i + "\" id=\"MediaSelectionWebControl1_" + i + "\" " + checkBox + " " + disabled + " onClick=\"CheckAllChilds('" + idVehicle + "ic_" + idInterestCenter + "','" + VehicleIds + "','vh_" + idVehicle + "')\" value=\"" + idVehicle + "ic_" + idInterestCenter + "\">");
							t.Append("\n</td>\n");
							//Libell� centre d'int�r�ts
							t.Append("<td width=100% valign=\"middle\" height=\"10\" onClick=\"javascript : DivDisplayer('" + idVehicle + "ic"+idInterestCenter+"');\" align=\"left\"  ");
							t.Append(">&nbsp;"+currentRow["interest_center"].ToString()+"</td>");
                            t.Append("<td align=\"right\" style=\"VERTICAL-ALIGN: bottom;\" onClick=\"javascript : DivDisplayer('" + idVehicle + "ic" + idInterestCenter + "');\"   ");
                            t.Append(">&nbsp;<IMG style=\"VERTICAL-ALIGN: bottom;\" height=\"15\" src=\"/App_Themes/" + themeName + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\"></td>");
                            t.Append("</tr><tr><td colspan=\"3\"><DIV style=\" display:" + ((displayIc.Length < 1) ? "block" : displayIc) + "; \" id=\"" + idVehicle + "ic" + idInterestCenter + "\" ><table cellpadding=0 cellspacing=0 border=\"0\" width=100%>");
                            t.Append("<table cellpadding=0 cellspacing=0 border=\"0\" width=\"100%\" class=\"txtViolet10 mediumPurple1 BlancTopBorder\">");
                            t.Append("<tr><td colspan=\"3\" class=\"roll04\" ><a href=\"javascript: SelectExclusiveAllChilds('" + idVehicle + "ic_" + idInterestCenter + "','" + VehicleIds + "','vh_" + idVehicle + "','vh_','ic_')\" title=\"" + GestionWeb.GetWebWord(1066, webSession.SiteLanguage) + "\" class=\"roll04\">&nbsp;" + GestionWeb.GetWebWord(1066, webSession.SiteLanguage) + "</a></td></tr>");							
							i++;
                            icList = icList + (icList.Length > 0 ? "," : "") + idVehicle + "ic" + idInterestCenter;
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
								if(webSession.SelectionUniversMedia.Nodes[indexVh].Nodes[interestCenters[Int64.Parse(currentRow["id_interest_center"].ToString())]].Nodes[medias[Int64.Parse(currentRow["id_media"].ToString())]].Checked){
									checkBox = " Checked ";
									if(checkVh.Length>0 || checkIc.Length>0)disabled="disabled";
								}//Gestion �l�ments sauvegard�s
								else if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
									if(savedMedia!=null && savedMedia.Contains(Int64.Parse(currentRow["id_media"].ToString())))
										checkBox = " Checked ";
									if (checkVh.Length>0 || checkIc.Length>0)disabled="disabled";
								}
							}
						}
							//Exceptions si le groupe n'est pas trouver dans l'arbre
						catch(System.ArgumentOutOfRangeException) {
							if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
								if(savedMedia!=null && savedMedia.Contains(Int64.Parse(currentRow["id_media"].ToString())))
									checkBox = " Checked ";
								if (checkVh.Length>0 || checkIc.Length>0)disabled="disabled";
							}
							if (checkVh.Length>0 || checkIc.Length>0)checkBox=" Checked ";
						}
                        catch (System.Collections.Generic.KeyNotFoundException) {
                            if (eventButton == constEvent.eventSelection.LOAD_EVENT || eventButton == constEvent.eventSelection.SAVE_EVENT) {
                                if (savedMedia != null && savedMedia.Contains(Int64.Parse(currentRow["id_media"].ToString())))
                                    checkBox = " Checked ";
                                if (checkVh.Length > 0 || checkIc.Length > 0) disabled = "disabled";
                            }
                            if (checkVh.Length > 0 || checkIc.Length > 0) checkBox = " Checked ";
                        }
						catch(System.NullReferenceException){
							if(eventButton==constEvent.eventSelection.LOAD_EVENT || eventButton==constEvent.eventSelection.SAVE_EVENT){
								if(savedMedia!=null && savedMedia.Contains(Int64.Parse(currentRow["id_media"].ToString())))
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
                    + "<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/App_Themes/" + themeName + "/Images/Common/Button/back_down.gif';\" onmouseout=\"bouton.src = '/App_Themes/" + themeName + "/Images/Common/Button/back_up.gif';\">"
                    + "<img src=\"/App_Themes/" + themeName + "/Images/Common/Button/back_up.gif\" border=0 name=bouton></a>"
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
        private Dictionary<long,int> LoadChilds(System.Windows.Forms.TreeNode tree)
        {
			Dictionary<long, int> t = new Dictionary<long, int>();
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
			VehicleInformation vehicleInformation = VehiclesInformation.Get(idVehicle);
			switch (vehicleInformation.Id) {
				case DBConstantesClassification.Vehicles.names.radio:
                case DBConstantesClassification.Vehicles.names.radioGeneral:
                case DBConstantesClassification.Vehicles.names.radioSponsorship:
                case DBConstantesClassification.Vehicles.names.radioMusic:
				case DBConstantesClassification.Vehicles.names.press:
                case DBConstantesClassification.Vehicles.names.newspaper:
                case DBConstantesClassification.Vehicles.names.magazine:
				case DBConstantesClassification.Vehicles.names.internationalPress:
				case DBConstantesClassification.Vehicles.names.tv:
                case DBConstantesClassification.Vehicles.names.tvGeneral:
                case DBConstantesClassification.Vehicles.names.tvSponsorship:
                case DBConstantesClassification.Vehicles.names.tvAnnounces:
                case DBConstantesClassification.Vehicles.names.tvNonTerrestrials:	
				case DBConstantesClassification.Vehicles.names.others:
                case DBConstantesClassification.Vehicles.names.adnettrack:
					return(true);
				default:
					return(false);
			}
		}
		

		/// <summary>
		/// R�cup�re la liste des m�dias (vehicle) autoris�s pour l'utilisateur courant
		/// </summary>
		/// <param name="vhlist">liste des m�dias (vehicle)</param>
		/// <param name="VehicleIds">Identifiants des m�dias </param>
		/// <param name="vhSeparator">separateur</param>
		private void getListIdVehicle(ref string vhlist, ref string  VehicleIds,string vhSeparator){
			//R�cup�ration de la liste des m�dia autoris�s pour l'utilisateur courant
			if(webSession.CustomerLogin[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap].Length>0){				
				int c=0;
				string[] VehicleIdsArr = webSession.CustomerLogin[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap].Split(',');
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
		
		
		
		#endregion
	}
}
