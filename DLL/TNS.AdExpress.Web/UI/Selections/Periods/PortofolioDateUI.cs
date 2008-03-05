#region Informations
// Auteur: A. Obermeyer
// Date de création: 29/11/2004
// Date de modification:
// Par: 
#endregion

using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Selections.Periods;
using TNS.AdExpress.Domain.Translation;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpress.Web.UI.Selections.Periods{

	/// <summary>
	/// Classe utilisée dans l'affichage des dates
	/// pour les modules portefeuille
	/// </summary>
	public class PortofolioDateUI{

		#region Méthodes
		/// <summary>
		/// Affiche la liste des numéros médias
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>Code HTML pour la liste des numéros médias</returns>
		public static string ListMedia(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
			
			#region Variables
			int width=545;			
			string parent="";
			string oldParent="-1";
			string day;
			int start=-1;
			int compteur=0;
			string date="";
			int monthDate=0;
			string couverture="";
			#endregion
			
			DataSet ds=null;
			StringBuilder t=new StringBuilder(500);
	
			ds=PortofolioDateDataAccess.GetListDate(webSession,idVehicle,idMedia,dateBegin,dateEnd,false);

			DataTable dt=ds.Tables[0];
			if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
				|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){
				width=500;
			}

			if(dt.Rows.Count>0){
				monthDate=int.Parse(dt.Rows[0][0].ToString().Substring(4,2));
				date= TNS.FrameWork.Date.MonthString.GetHTMLCharacters(monthDate,webSession.SiteLanguage,20);
				date+=" "+dt.Rows[0][0].ToString().Substring(0,4);
                string pathWeb;
                if ((int)idVehicle == DBClassificationConstantes.Vehicles.names.press.GetHashCode()
                    || (int)idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode())
                {
                    pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + idMedia.ToString() + "/" + dt.Rows[0]["date_cover_num"].ToString() + "/Imagette/" + CstWeb.CreationServerPathes.COUVERTURE + "";
                }
                else
                {
                    pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + idMedia.ToString() + "/" + dt.Rows[0]["date_media_num"].ToString() + "/Imagette/" + CstWeb.CreationServerPathes.COUVERTURE + "";
                }
				if(dt.Rows.Count>0){
					if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode()
						|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){
						if(dt.Rows[0][1]!= System.DBNull.Value && int.Parse(dt.Rows[0][1].ToString())<10){
							couverture="onmouseover=\"graph.src = '/Images/"+webSession.SiteLanguage+"/Others/no_visuel.gif';\" onmouseout=\"graph.src = '/Images/Common/vide.gif';\"";
						}
						else{
							couverture="onmouseover=\"graph.src = '"+pathWeb+"';\" onmouseout=\"graph.src = '/Images/Common/vide.gif';\"";
						}
					}
				}

				if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
					|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){
					t.Append("<table><tr valign=top><td>");
				}

				t.Append("<table  cellpadding=0 cellspacing=0   width="+width+">");			
				t.Append("<tr><td width=50%>");
			
				// Dans le cas de la presse
				if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
					|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){

					t.Append("<a href=\"javascript:__doPostBack('link','"+dt.Rows[0][0].ToString()+"');\"  "+couverture+" class=\"txtGroupViolet11Bold\">"+GestionWeb.GetWebWord(1374,webSession.SiteLanguage)+" : "+dt.Rows[0][0].ToString().Substring(6,2)+" "+date+"</a>");

					//href=\"/Private/Results/PortofolioResults.aspx?idSession="+webSession.IdSession+"&date="+dt.Rows[0][0].ToString()+"\"
				}
					// Autres cas
				else{
					t.Append("<a href=\"javascript:__doPostBack('link','"+dt.Rows[0][0].ToString()+"');\"  class=\"txtGroupViolet11Bold\">"+GestionWeb.GetWebWord(1374,webSession.SiteLanguage)+" : "+dt.Rows[0][0].ToString().Substring(6,2)+" "+date+"</a>");
				}
				t.Append("</td></tr>");
				t.Append("</table>");

				t.Append("<table  cellpadding=0 cellspacing=0   width="+width+">");
				t.Append("<tr><td class=\"txtViolet11Bold\"  width=50%>");
				t.Append("&nbsp;");
				t.Append("</td></tr>");
				t.Append("</table>");
				
				#region Parcours du tableau
				if(dt.Rows.Count>1){
					for(int i=1;i<dt.Rows.Count;i++) {
                        if ((int)idVehicle == DBClassificationConstantes.Vehicles.names.press.GetHashCode()
                            || (int)idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode())
                        {
                            pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + idMedia.ToString() + "/" + dt.Rows[i]["date_cover_num"].ToString() + "/Imagette/" + CstWeb.CreationServerPathes.COUVERTURE + "";
                        }
                        else
                        {
                            pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + idMedia.ToString() + "/" + dt.Rows[i]["date_media_num"].ToString() + "/Imagette/" + CstWeb.CreationServerPathes.COUVERTURE + "";
                        }
						DateTime dayDT=new DateTime(int.Parse(dt.Rows[i][0].ToString().Substring(0,4)),int.Parse(dt.Rows[i][0].ToString().Substring(4,2)),int.Parse(dt.Rows[i][0].ToString().Substring(6,2)));
						parent=dt.Rows[i][0].ToString().Substring(4,2);
						monthDate=int.Parse(dt.Rows[i][0].ToString().Substring(4,2));
				
						date= TNS.FrameWork.Date.MonthString.GetHTMLCharacters(monthDate,webSession.SiteLanguage,20);
						date+=" "+dt.Rows[i][0].ToString().Substring(0,4);
				
						day=GetDayOfWeek(webSession,dayDT.DayOfWeek.ToString())+" "+dt.Rows[i][0].ToString().Substring(6,2);
						//Premier
						if(parent!=oldParent && start!=0){

                            t.Append("<table class=\"listMediaBorder txtViolet11Bold\" cellpadding=0 cellspacing=0 width=" + width + ">");
							t.Append("<tr onClick=\"showHideContent1('"+parent+"');\" class=\"cursorHand\">");
                            t.Append("<td align=\"left\" height=\"10\" width=\"100%\" valign=\"middle\" class=\"arrowBackGround\">");					
							t.Append("<label ID=\""+dt.Rows[i][0]+"\" class=\"txtNowrap\">&nbsp;");
							t.Append(""+date+"");
							t.Append("</label>");
							t.Append("</td>");
							//t.Append("<td  ><IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");	
							t.Append("</tr>");
							t.Append("</table>");
                            t.Append("<div id=\"" + parent + "Content1\" class=\"listMediaHeaderBorderNone\" style=\"display: none; width: 100%;\">");
                            t.Append("<table class=\"listMediaHeaderBorder paleVioletBackGround\" width=" + width + ">");
							oldParent=parent;
					
							start=0;
							compteur=0;
					
						}
						else if (parent!=oldParent){				
							t.Append("</table>");
							t.Append("</div>");
                            t.Append("<table class=\"listMediaHeaderBorder txtViolet11Bold\" cellpadding=0 cellspacing=0 width=" + width + ">");
							t.Append("<tr onClick=\"showHideContent1('"+parent+"');\" class=\"cursorHand\">");
                            t.Append("<td align=\"left\" height=\"10\" width=\"100%\" valign=\"middle\" class=\"arrowBackGround\">");					
							t.Append("<label ID=\""+dt.Rows[i][0]+"\" class=\"txtNowrap\">&nbsp;");
							t.Append(""+date+"");
							t.Append("</label>");
							t.Append("</td>");
							//t.Append("<td ><IMG height=\"15\" src=\"/images/Common/button/bt_arrow_down.gif\" width=\"15\"></td>");		
							t.Append("</tr>");
							t.Append("</table>");
							t.Append("<div id=\""+parent+"Content1\"  class=\"listMediaHeaderBorderNone\" style=\"display: none; width: 100%;\">");
							t.Append("<table class=\"listMediaHeaderBorder paleVioletBackGround\" width="+width+">");

							oldParent=parent;
							compteur=0;
						}				
				
						if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
							|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){
                                if (dt.Rows[0][1] != System.DBNull.Value && int.Parse(dt.Rows[i][1].ToString()) < 10)
                                {
								couverture="onmouseover=\"graph.src = '/Images/"+webSession.SiteLanguage+"/Others/no_visuel.gif';\" onmouseout=\"graph.src = '/Images/Common/vide.gif';\"";
							}
							else{
								couverture="onmouseover=\"graph.src = '"+pathWeb+"';\" onmouseout=\"graph.src = '/Images/Common/vide.gif';\"";
							}
						}
						if(compteur==0){	
							t.Append("<tr><td width=33%>");						
							t.Append("<a href=\"javascript:__doPostBack('link','"+dt.Rows[i][0].ToString()+"');\"  "+couverture+" class=\"txtGroupViolet11Bold\">"+day+"</a>");
							t.Append("</td>");
							compteur=1;
						}else if(compteur==1){
							t.Append("<td width=33%>");						
							t.Append("<a href=\"javascript:__doPostBack('link','"+dt.Rows[i][0].ToString()+"');\"   "+couverture+" class=\"txtGroupViolet11Bold\">"+day+"</a>");
							t.Append("</td>");
							compteur=2;
						}
						else{
							t.Append("<td  width=33%>");
							t.Append("<a href=\"javascript:__doPostBack('link','"+dt.Rows[i][0].ToString()+"');\"  "+couverture+" class=\"txtGroupViolet11Bold\">"+day+"</a>");
							t.Append("</td></tr>");
							compteur=0;
						}			
				
					}
					if(compteur!=0){
						t.Append("</tr>");
					}
					t.Append("</table>");
					t.Append("</div>");

				}
				#endregion	
			
				// Cas de la presse : affichage de la couverture
				if((int)idVehicle==DBClassificationConstantes.Vehicles.names.press.GetHashCode() 
					|| (int)idVehicle==DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode() ){
					t.Append("</td>");
					t.Append("<td>");
					t.Append("<img src='/Images/Common/vide.gif' id=\"graph\" width=180 height=220>");
					t.Append("</td></tr></table>");
				}
			}
			else{
				t.Append("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");	
			}
			return (t.ToString());
		}
		#endregion

		/// <summary>
		/// Donne le jour de la semaine
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="dayOfWeek">Jour de la semaine en anglais</param>
		/// <returns>Donne le jour de la semaine</returns>
		public static string GetDayOfWeek(WebSession webSession, string dayOfWeek){
			string txt="";
			switch(dayOfWeek){
				case "Monday":
					txt=GestionWeb.GetWebWord(654,webSession.SiteLanguage);
					break;
				case "Tuesday":
					txt=GestionWeb.GetWebWord(655, webSession.SiteLanguage);
					break;
				case "Wednesday":
					txt=GestionWeb.GetWebWord(656, webSession.SiteLanguage);
					break;
				case "Thursday":
					txt=GestionWeb.GetWebWord(657, webSession.SiteLanguage);
					break;
				case "Friday":
					txt=GestionWeb.GetWebWord(658, webSession.SiteLanguage);
					break;
				case "Saturday":
					txt=GestionWeb.GetWebWord(659, webSession.SiteLanguage);
					break;
				case "Sunday":
					txt=GestionWeb.GetWebWord(660, webSession.SiteLanguage);
					break;						
			}
			return txt;
		}

	}
}
