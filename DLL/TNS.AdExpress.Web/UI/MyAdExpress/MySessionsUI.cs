#region Information
//auteur : G Facon
//créé le :
//modifié le :
//	22/07/2004		Guillaume Ragneau
//	12/08/2004		Guillaume Facon		Nom de variables

#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;

namespace TNS.AdExpress.Web.UI.MyAdExpress{

	/// <summary>
	/// Classe utilisée dans l'affichage des répertoires dans Mon Adexpress
	/// ou Mes Univers
	/// </summary>
	public class MySessionsUI{
		
		#region Variables
		/// <summary>
		/// Script
		/// </summary>
		private string _script;
		/// <summary>
		/// Type de la requête
		/// </summary>
		private MySessionsUI.type _request;
		/// <summary>
		/// Branche
		/// </summary>
		private string _branch="";
		/// <summary>
		/// Session du client
		/// </summary>
		private WebSession _webSession;
		/// <summary>
		/// Largeur
		/// </summary>
		private int _width;

		/// <summary>
		/// Allowed universe levels
		/// </summary>
		protected List<Int64> _allowedLevels = new List<long>();

		#endregion

		#region Enumérateur
		/// <summary>
		/// Enumérateur type de requête
		/// </summary>
		public enum type{
			/// <summary>
			/// Session dans Mon AdExpress
			/// </summary>
			mySession,
			/// <summary>
			/// Univers dans Mes Univers 
			/// </summary>
			universe
		}
		#endregion

		#region Constructeur
	
		/// <summary>
		/// Constructeur de base 
		/// </summary>
		protected MySessionsUI(){
			#region script
			_script=" <script language=\"JavaScript\">";			
			_script+=" var ancien4; ";
			_script+=" var ancien3; ";
			_script+=" function showHideContent4(id)";
			_script+=" { ";
			_script+=" var oContent4 = document.all.item(id+\"Content4\"); ";
			_script+=" if (ancien4!=null){ ";
			_script+="	if (id+\"Content4\"==ancien4 && ouvert4==true){";
			_script+=" var oAncien4=document.all.item(ancien4); ";
			_script+=" oAncien4.style.display=\"none\"; ";
			_script+=" ouvert4=false; ";
			_script+=" return; ";
			_script+=" } ";
			_script+=" var oAncien4=document.all.item(ancien4); ";
			_script+=" oAncien4.style.display=\"none\"; ";
			_script+=" } ";
			_script+=" ancien4=id+\"Content4\"; ";
			_script+=" oContent4.style.display = \"\"; ";
			_script+=" ouvert4=true; ";
			_script+=" } ";

			_script+=" function showHideContent3(id)";
			_script+=" { ";
			_script+=" var oContent3 = document.all.item(id+\"Content3\"); ";
			_script+=" if (ancien3!=null){ ";
			_script+="	if (id+\"Content3\"==ancien3 && ouvert3==true){";
			_script+=" var oAncien3=document.all.item(ancien3); ";
			_script+=" oAncien3.style.display=\"none\"; ";
			_script+=" ouvert3=false; ";
			_script+=" return; ";
			_script+=" } ";
			_script+=" var oAncien3=document.all.item(ancien3); ";
			_script+=" oAncien3.style.display=\"none\"; ";
			_script+=" } ";
			_script+=" ancien3=id+\"Content3\"; ";
			_script+=" oContent3.style.display = \"\"; ";
			_script+=" ouvert3=true; ";
			_script+=" } ";

			_script+="</script>";
			#endregion
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="request">Type de requête (session ou univers)</param>
		/// <param name="width">Taille de la table</param>
		public MySessionsUI(WebSession webSession,type request,int width):this(){
			_webSession=webSession;
			_request=request;
			_width=width;
			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">webSession</param>
		/// <param name="branch">advertiser, média, product, nothing</param>
		/// <param name="width">Taille de la table</param>
		public MySessionsUI(WebSession webSession,string branch,int width ):this(){
			_request=MySessionsUI.type.universe;
			_branch=branch;
			_webSession=webSession;
			_width=width;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">web Session</param>
		/// <param name="branch">advertiser, média, product, nothing</param>
		/// <param name="width">Table width</param>
		/// <param name="allowedLevels">Allowed universe levels</param>
		public MySessionsUI(WebSession webSession, string branch, int width,List<Int64> allowedLevels )
			: this() {
			_request = MySessionsUI.type.universe;
			_branch = branch;
			_webSession = webSession;
			_width = width;
			_allowedLevels = allowedLevels;
		}

		#endregion

		#region Méthodes

		/// <summary>
		/// Méthode utilisée pour l'affichage d'un tableau contenant un type de répertoire 
		/// (Mon AdExpress, Mes Univers...)
		/// </summary>
		/// <param name="valueTable">valeur utilisée pour choisir le bon script</param>
		/// <param name="ListUniverseClientDescription">ID_UNIVERSE_CLIENT_DESCRIPTION</param>
		/// <returns>Retourne le tableau correspondant au choix fait au niveau du constructeur</returns>
		public string GetSelectionTableHtmlUI(int valueTable,string ListUniverseClientDescription){
			DataSet dsListRepertory=null;
			if(_request==type.mySession){
				dsListRepertory= MySessionsDataAccess.GetData(_webSession);
			}
			else if(_request==type.universe){
				dsListRepertory= TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetData(_webSession,_branch,ListUniverseClientDescription);
			}
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			Int64 idParent;			
			Int64 idParentOld=-1;
			string textParent;
			string textParentOld;
			int start=-1;
			int compteur=0;

			if(dsListRepertory.Tables[0].Rows.Count!=0){
				foreach(DataRow currentRow in dsListRepertory.Tables[0].Rows) {
					idParent=(Int64)currentRow[0];
					textParent=currentRow[1].ToString();
					//Premier
					if(idParent!=idParentOld && start!=0){

                        t.Append("<table class=\"violetBorder txtViolet11Bold\" cellpadding=0 cellspacing=0   width=" + _width + ">");
					
						t.Append("<tr onClick=\"showHideContent"+valueTable+"('"+idParent+"');\" class=\"cursorHand\">");
						t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");					
						t.Append("<label ID=\""+currentRow[0]+valueTable+"\">&nbsp;");
						t.Append(""+textParent+"");
						t.Append("</label>");
						t.Append("</td>");
                        t.Append("<td class=\"arrowBackGround\"></td>");	
						t.Append("</tr>");
						t.Append("</table>");
						t.Append("<div id=\""+idParent+"Content"+valueTable+"\" style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\" >");
                        t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + _width + ">");
					//	t.Append("<tr><td>");
					//	t.Append("<label style=\"cursor : hand\" onclick=\"allSelection2('"+idParent+textParent+"')\" ID=\""+currentRow[0]+"\">");
					//	t.Append("</td></tr>");			


						idParentOld=idParent;
						textParentOld=textParent;
						start=0;
						compteur=0;
					
					}
					else if (idParent!=idParentOld){				
						t.Append("</table>");
						t.Append("</div>");
                        t.Append("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=" + _width + ">");
						t.Append("<tr onClick=\"showHideContent"+valueTable+"('"+idParent+"');\" class=\"cursorHand\">");
						t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");					
						t.Append("<label ID=\""+currentRow[0]+valueTable+"\">&nbsp;");
						t.Append(""+textParent+"");
						t.Append("</label>");
						t.Append("</td>");
                        t.Append("<td class=\"arrowBackGround\"></td>");		
						t.Append("</tr>");
						t.Append("</table>");
						t.Append("<div id=\""+idParent+"Content"+valueTable+"\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
                        t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + _width + ">");
					//	t.Append("<tr><td>");
					//	t.Append("<label style=\"cursor : hand\" onclick=\"allSelection2('"+idParent+textParent+"')\" ID=\""+currentRow[0]+"\">");
					//	t.Append("</td></tr>");	


						idParentOld=idParent;
						textParentOld=textParent;
						compteur=0;
					}
				
					if(currentRow[2]!=System.DBNull.Value){
						if(compteur==0){	
							t.Append("<tr><td class=\"txtViolet10\" width=50%>");
							//	t.Append("<input type=\"radio\" ID=\""+currentRow[2]+currentRow[3]+"\"  value=\""+currentRow[2]+"_"+currentRow[3].ToString()+"\" name=\"CKB_"+currentRow[0]+"_"+currentRow[2]+"\">"+currentRow[3].ToString()+"<br>");
							t.Append("<input type=\"radio\" ID=\""+currentRow[2]+currentRow[3]+valueTable+"\" onClick=\"insertIdMySession"+valueTable+"('"+currentRow[2]+"','"+currentRow[0]+"');\" value=\""+currentRow[2]+"\" name=\"Session\">"+currentRow[3].ToString()+"<br>");
							t.Append("</td>");
							compteur=1;
						}
						else{
							t.Append("<td class=\"txtViolet10\" width=50%>");
							t.Append("<input type=\"radio\" ID=\""+currentRow[2]+currentRow[3]+valueTable+"\" onClick=\"insertIdMySession"+valueTable+"('"+currentRow[2]+"','"+currentRow[0]+"');\" value=\""+currentRow[2]+"\" name=\"Session\">"+currentRow[3].ToString()+"<br>");
							t.Append("</td></tr>");
							compteur=0;
						}
					}
					else{					
						t.Append("<tr><td class=\"txtViolet10\" width=50%>");
						t.Append(GestionWeb.GetWebWord(285,_webSession.SiteLanguage));
						t.Append("</td></tr>");
						
					}
				}
				if(compteur!=0){
					t.Append("</tr>");
				}
				t.Append("</table>");
				t.Append("</div>");

				return(t.ToString());
			}
			return("");
		}


		/// <summary>
		/// Méthode utilisée pour l'affichage d'un tableau contenant un type de répertoire 
		/// (Mon AdExpress, Mes Univers...)
		/// </summary>
		/// <param name="valueTable">valeur utilisée pour choisir le bon script</param>
		/// <param name="ListUniverseClientDescription">ID_UNIVERSE_CLIENT_DESCRIPTION</param>
		/// <param name="allowedLevels">Allowed levels</param>
		/// <returns>Retourne le tableau correspondant au choix fait au niveau du constructeur</returns>
		public string GetSelectionTableHtmlUI(int valueTable, string ListUniverseClientDescription, List<Int64> allowedLevels) {
			DataSet dsListRepertory = null;
			DataTable dt = null;
			if (_request == type.mySession) {
				dsListRepertory = MySessionsDataAccess.GetData(_webSession);
				if(dsListRepertory.Tables[0].Rows.Count != 0)dt =dsListRepertory.Tables[0];
			}
			else if (_request == type.universe) {
				dt = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetData(_webSession, _branch, ListUniverseClientDescription,allowedLevels);
			}
			System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
			Int64 idParent;
			Int64 idParentOld = -1;
			string textParent;
			string textParentOld;
			int start = -1;
			int compteur = 0;

			if (dt != null && dt.Rows.Count > 0) {
				foreach (DataRow currentRow in dt.Rows) {
					idParent = (Int64)currentRow[0];
					textParent = currentRow[1].ToString();
					//Premier
					if (idParent != idParentOld && start != 0) {

                        t.Append("<table class=\"violetBorder txtViolet11Bold\" cellpadding=0 cellspacing=0   width=" + _width + ">");

                        t.Append("<tr onClick=\"showHideContent" + valueTable + "('" + idParent + "');\" class=\"cursorHand\">");
						t.Append("<td align=\"left\" height=\"10\"  valign=\"middle\">");
						t.Append("<label ID=\"" + currentRow[0] + valueTable + "\">&nbsp;");
						t.Append("" + textParent + "");
						t.Append("</label>");
						t.Append("</td>");
                        t.Append("<td class=\"arrowBackGround\"></td>");
						t.Append("</tr>");
						t.Append("</table>");
						t.Append("<div id=\"" + idParent + "Content" + valueTable + "\" style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\" >");
                        t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + _width + ">");						

						idParentOld = idParent;
						textParentOld = textParent;
						start = 0;
						compteur = 0;

					}
					else if (idParent != idParentOld) {
						t.Append("</table>");
						t.Append("</div>");
                        t.Append("<table class=\"violetBorderWithoutTop txtViolet11Bold\"  cellpadding=0 cellspacing=0 width=" + _width + ">");
                        t.Append("<tr onClick=\"showHideContent" + valueTable + "('" + idParent + "');\" class=\"cursorHand\">");
						t.Append("<td align=\"left\" height=\"10\" valign=\"middle\">");
						t.Append("<label ID=\"" + currentRow[0] + valueTable + "\">&nbsp;");
						t.Append("" + textParent + "");
						t.Append("</label>");
						t.Append("</td>");
                        t.Append("<td class=\"arrowBackGround\"></td>");
						t.Append("</tr>");
						t.Append("</table>");
						t.Append("<div id=\"" + idParent + "Content" + valueTable + "\"  style=\"BORDER-BOTTOM: #ffffff 0px solid; BORDER-LEFT: #ffffff 0px solid; BORDER-RIGHT: #ffffff 0px solid; DISPLAY: none; WIDTH: 100%\">");
                        t.Append("<table class=\"violetBorderWithoutTop paleVioletBackGround\" width=" + _width + ">");						


						idParentOld = idParent;
						textParentOld = textParent;
						compteur = 0;
					}

					if (currentRow[2] != System.DBNull.Value) {
						if (compteur == 0) {
							t.Append("<tr><td class=\"txtViolet10\" width=50%>");
							//	t.Append("<input type=\"radio\" ID=\""+currentRow[2]+currentRow[3]+"\"  value=\""+currentRow[2]+"_"+currentRow[3].ToString()+"\" name=\"CKB_"+currentRow[0]+"_"+currentRow[2]+"\">"+currentRow[3].ToString()+"<br>");
							t.Append("<input type=\"radio\" ID=\"" + currentRow[2] + currentRow[3] + valueTable + "\" onClick=\"insertIdMySession" + valueTable + "('" + currentRow[2] + "','" + currentRow[0] + "');\" value=\"" + currentRow[2] + "\" name=\"Session\">" + currentRow[3].ToString() + "<br>");
							t.Append("</td>");
							compteur = 1;
						}
						else {
							t.Append("<td class=\"txtViolet10\" width=50%>");
							t.Append("<input type=\"radio\" ID=\"" + currentRow[2] + currentRow[3] + valueTable + "\" onClick=\"insertIdMySession" + valueTable + "('" + currentRow[2] + "','" + currentRow[0] + "');\" value=\"" + currentRow[2] + "\" name=\"Session\">" + currentRow[3].ToString() + "<br>");
							t.Append("</td></tr>");
							compteur = 0;
						}
					}
					else {
						t.Append("<tr><td class=\"txtViolet10\" width=50%>");
						t.Append(GestionWeb.GetWebWord(285, _webSession.SiteLanguage));
						t.Append("</td></tr>");

					}
				}
				if (compteur != 0) {
					t.Append("</tr>");
				}
				t.Append("</table>");
				t.Append("</div>");

				return (t.ToString());
			}
			return ("");
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le script javaScript
		/// </summary>
		public string Script{
			get{return _script;}
		}
		#endregion

	}
}
