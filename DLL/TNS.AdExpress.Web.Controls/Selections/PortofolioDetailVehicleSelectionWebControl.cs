#region Informations
// Auteur: A. OBERMEYER 
// Date de création: 25/11/2004 
// Date de modification:  
// Date de modification: 07/06/2005 
// K. Shehzad:option open/close all

#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
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

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Affiche la liste des categories/media d'un vehicle que peut sélectionner un client en fonction de ses droits.
	/// </summary>
	public class PortofolioDetailVehicleSelectionWebControl{
	
		#region propriétés
		/// <summary>
		/// boolean si listeMedia est vide
		/// </summary>
		private static bool isEmptyListPortofolio=true;

		/// <summary>
		/// Obtient ListeMedia est vide
		/// </summary>
		[Bindable(true),
		Description("bool de pour la liste vide")]
		public static bool IsEmptyListPortofolio{
			get{return isEmptyListPortofolio;}
			set{isEmptyListPortofolio=value;}
		}
		#endregion

		#region Liste des médias
		/// <summary>
		/// Génère la liste des supports
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="keyWord">Mot clé</param>
		/// <returns></returns>	
		public static string listMedia(WebSession webSession,string keyWord) {
			
			DataSet dsListMedia=null;
			int insertIndex = 0;
			string 	vhList="";		 	
			int eventButton=-1;	
			string textOpenclose="";
			bool loadIsPossible=true;
			
			if(webSession.PreformatedMediaDetail==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia) {
				dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.keyWordInterestCenterListDataAccess(webSession,keyWord,"");

			}else if(webSession.PreformatedMediaDetail==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia){
				dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.keyWordMediaSellerListDataAccess(webSession,keyWord,"");
			}
			else if(webSession.PreformatedMediaDetail==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleTitleMedia){
				dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.TitleListDataAccess.keyWordTitleListDataAccess(webSession,keyWord,"");
			}
			else if(webSession.PreformatedMediaDetail==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCountryMedia){
				dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.CountryListDataAccess.keyWordCountryListDataAccess(webSession,keyWord,"");
			}
			
			//dsListMedia=TNS.AdExpress.Web.DataAccess.Selections.Medias.VehicleListDataAccess.keyWordInterestCenterListDataAccess(webSession,keyWord,"");
			
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			
			#region Affichage à partir du dataset		
				
			#region Variables
			Int64 idParentOld=-1;
			Int64 idParent;				
			string textParent;
			string textParentOld;
			int nbColumn=0;
			int start=-1;
			int i =0;
			string insertLabel=string.Empty;
			int labelIndex=0;
			#endregion
	
			if (dsListMedia.Tables[0].Rows.Count>0){
				isEmptyListPortofolio=false;
			}
			else
				isEmptyListPortofolio=true;

			//Tableau global
			if(webSession.PreformatedMediaDetail ==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia) 	
				textOpenclose=GestionWeb.GetWebWord(1617,webSession.SiteLanguage);
			else if(webSession.PreformatedMediaDetail ==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia)
				textOpenclose=GestionWeb.GetWebWord(1616,webSession.SiteLanguage);
			else if(webSession.PreformatedMediaDetail ==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleTitleMedia)
				textOpenclose=GestionWeb.GetWebWord(1977,webSession.SiteLanguage);
			else if(webSession.PreformatedMediaDetail ==CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCountryMedia)
				textOpenclose=GestionWeb.GetWebWord(2094,webSession.SiteLanguage);
					
			t.Append("<a href=\"javascript: ExpandColapseAllDivs('");
			insertIndex = t.Length;
			t.Append("')\" class=\"roll04\" >&nbsp;&nbsp;&nbsp;");
			labelIndex=t.Length;
			t.Append("</a>");	
			//			
			t.Append("<tr class=\"whiteBackGround\"><td  vAlign=\"top\"><table id=\"AdvertiserSelectionWebControl1\">");			
			t.Append("<tr><td vAlign=\"top\" class=\"whiteBackGround\">");
			if(dsListMedia!=null){
				
				foreach(DataRow currentRow in dsListMedia.Tables[0].Rows){								
										
					#region Foreach 		
					
					idParent=(Int64)currentRow[0];
					textParent=currentRow[1].ToString();		
					
					if(idParentOld!=idParent && start==0){																	
						if(nbColumn!=0){
							t.Append("</tr>");
							nbColumn=0;
						}
						t.Append("</table>");
						t.Append("</div>");
                        t.Append("<table class=\"listMediaHeaderBorder txtViolet11Bold\" cellpadding=0 cellspacing=0 width=645>");
						t.Append("<tr onClick=\"DivDisplayer('"+idParent+"Ct"+"');\" class=\"cursorHand\">");
                        t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"arrowBackGround\">");
						//t.Append("<input type=\"checkbox\"  onclick=\"integration('"+idParent+"',"+i+")\" ID=\"AdvertiserSelectionWebControl1_"+i+"\" name=\"AdvertiserSelectionWebControl1:"+i+"\">");
                        t.Append("<label class=\"txtNowrap\">  ");
						t.Append("&nbsp;&nbsp;"+currentRow[1].ToString()+"");
						t.Append("</label>");
						t.Append("</td>");
                        //t.Append("<td height=\"15\" width=\"15\" class=\"arrowBackGround\"></td>");		
						t.Append("</tr>");
						t.Append("</table>");
						t.Append("<div id=\""+idParent+"Ct\" class=\"listMediaHeaderBorderNone\" style=\"display: none; width: 100%;\">");
                        t.Append("<table id=\"AdvertiserSelectionWebControl1\" class=\"listMediaHeaderBorder paleVioletBackGround\" width=645>");
						vhList=vhList+(Int64)currentRow[0]+"Ct"+",";
						idParentOld=idParent;
						textParentOld=textParent;
					
					}
					//Premier	
					if(idParentOld!=idParent && start!=0){

                        t.Append("<table class=\"listMediaBorder txtViolet11Bold\" cellpadding=0 cellspacing=0   width=645>");
						t.Append("<tr onClick=\"DivDisplayer('"+idParent+"Ct"+"');\" class=\"cursorHand\" >");
                        t.Append("<td align=\"left\" height=\"10\" valign=\"middle\" class=\"arrowBackGround\">");
						//		t.Append("<input type=\"checkbox\"  onclick=\"integration('"+idParent+"',"+i+")\" ID=\"AdvertiserSelectionWebControl1_"+i+"\" name=\"AdvertiserSelectionWebControl1:"+i+"\">");
                        t.Append("<label class=\"txtNowrap\">");
						t.Append("&nbsp;&nbsp;"+currentRow[1].ToString()+"");
						t.Append("</label>");
						t.Append("</td>");
                        //t.Append("<td><IMG height=\"15\" width=\"15\" src=\"/App_Themes/DefaultAdExpressFr/images/Common/button/bt_arrow_down.gif\"></td>");	
						t.Append("</tr>");
						t.Append("</table>");
                        t.Append("<div id=\"" + idParent + "Ct\" class=\"listMediaHeaderBorderNone\" style=\"display: none; width: 100%;\">");
                        t.Append("<table id=\"AdvertiserSelectionWebControl1\" class=\"listMediaHeaderBorder paleVioletBackGround\" width=645>");
					
						vhList=vhList+(Int64)currentRow[0]+"Ct"+",";
						idParentOld=idParent;
						textParentOld=textParent;
						start=0;
				
					}
					
					#region Affichage des fils							
					// Milieu
					if(nbColumn==2){				
						t.Append("<td class=\"txtViolet10\" width=215>");
						//		t.Append("<input ID=\"AdvertiserSelectionWebControl1_"+i+"\" type=\"radio\"  name=\"AdvertiserSelectionWebControl1\"  value=\""+idParent+"\" /><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[3].ToString()+"<br></label>");
						t.Append("<input ID=\"AdvertiserSelectionWebControl1_"+i+"\" type=\"radio\"  name=\"AdvertiserSelectionWebControl1\"  value=\""+currentRow[3].ToString()+"\" onClick=\"insertValueInHidden('"+currentRow[2]+"','"+currentRow[3]+"');\" />"+currentRow[3].ToString()+"<br>");
						t.Append("</td>");
						nbColumn=1;
						i++;
					
					}
						// Dernier
					else if(nbColumn==1 ){
						t.Append("<td class=\"txtRed\" width=215>");								
						//	t.Append("<input ID=\"AdvertiserSelectionWebControl1_"+i+"\" type=\"radio\"  name=\"AdvertiserSelectionWebControl1\"  value=\""+idParent+"\" /><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[3].ToString()+"<br></label>");
						t.Append("<input ID=\"AdvertiserSelectionWebControl1_"+i+"\" type=\"radio\"  name=\"AdvertiserSelectionWebControl1\"  value=\""+currentRow[3].ToString()+"\" onClick=\"insertValueInHidden('"+currentRow[2]+"','"+currentRow[3]+"');\" value=\""+currentRow[2]+"\" />"+currentRow[3].ToString()+"<br>");
						t.Append("</td>");								
						t.Append("</tr>");
						nbColumn=0;
						i++;	
					
					}
						// Premier
					else {
						t.Append("<tr>");
						t.Append("<td class=\"txtViolet10\" width=215>");
						//	t.Append("<input ID=\"AdvertiserSelectionWebControl1_"+i+"\" type=\"radio\"  name=\"AdvertiserSelectionWebControl1\"  value=\""+idParent+"\" /><label for=\"AdvertiserSelectionWebControl1_"+i+"\">"+currentRow[3].ToString()+"<br></label>");
						t.Append("<input ID=\"AdvertiserSelectionWebControl1_"+i+"\" type=\"radio\"  name=\"AdvertiserSelectionWebControl1\"  value=\""+currentRow[3].ToString()+"\" onClick=\"insertValueInHidden('"+currentRow[2]+"','"+currentRow[3]+"');\" />"+currentRow[3].ToString()+"<br>");
						t.Append("</td>");								
						nbColumn=2;
						i++;
					}
					#endregion

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

				#region Message d'erreurs
				// Message d'erreur : veuillez saisir 3 caractères minimums
				if(keyWord.Length<3 && keyWord.Length>0 && eventButton==constEvent.eventSelection.OK_EVENT){
                    t.Append("<tr class=\"whiteBackGround\"><td class=\"whiteBackGround txtGris11Bold\" ><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(352,webSession.SiteLanguage)+" "+keyWord+".</p> ");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}
					// Message d'erreur : mot incorrect
				else if(!TNS.AdExpress.Web.Functions.CheckedText.CheckedProductText(keyWord) && keyWord.Length>0 && eventButton==constEvent.eventSelection.OK_EVENT){
                    t.Append("<tr><td class=\"whiteBackGround txtGris11Bold\"><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
					t.Append(" "+GestionWeb.GetWebWord(1088,webSession.SiteLanguage)+" "+keyWord+".</p> ");
					t.Append(" </td> ");
					t.Append(" </tr> ");
				}
					// Message d'erreur : aucun résultat avec le mot clé
				else if(dsListMedia!=null){
					if(dsListMedia.Tables[0].Rows.Count==0){
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
				t.Insert(labelIndex+listLenght,insertLabel);
					

			}
			#endregion	

			// Message d'erreur : Chargement de l'univers impossible
			if(!loadIsPossible){
                t.Append("<tr class=\"whiteBackGround\"><td class=\"whiteBackGround txtGris11Bold\" ><p style=\"PADDING-RIGHT:20px;PADDING-LEFT:80px\">");
				t.Append(" load impossible </p> ");
				t.Append(" </td> ");
				t.Append(" </tr> ");
				
			}
			return(t.ToString());
		}
		#endregion
		
	} 
}