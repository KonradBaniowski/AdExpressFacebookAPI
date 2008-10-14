#region Informations
// Auteur:
// Création:
// Modification:
//	D. V. Mussuma	01/10/2004
//	G. Facon		12/08/2005  Nom et commentaire de fonction
//	B. Masson		12/01/2006	Ajout des méthodes pour export excel pop up des unités (Plan média)
#endregion

using System;
using System.Text;
using TNS.AdExpress.Web.UI;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Fonctions javascript
	/// </summary>
	public class Script{

		#region Selection Detail
		/// <summary>
		/// Register name for GetHelpJS javascript
		/// </summary>
		public static string GET_SELECTION_JS_REGISTER = "ScriptRecallpopup";
		/// <summary>
		/// Get Java script for help popup
		/// </summary>
		/// <returns>Javascript Code</returns>
		public static string GetSelectionDetailJS(){
			string script="<script language=\"JavaScript\"> ";
			script+="function popupRecallOpen(page,width,height){";
			script+="	window.open(page,'','width='+width+',height='+height+',toolbar=no,scrollbars=yes,resizable=no');";
			script+="}";
			script+="</script>";
			return script;
		}

		#endregion

		#region Evènements
		/// <summary>
		/// Ajout un Evènement JavaScript
		/// </summary>
		/// <remarks>Nom du script AddJsEvent</remarks>
		/// <returns>Script</returns>
		public static string AddJsEvent(){
			StringBuilder js=new StringBuilder(200);
			js.Append("<script>");

			js.Append("\r\n\taddJsEvent = function(o, e, f, s){");
			js.Append("\r\n\t\tvar r = o[r = \"_\" + (e = \"on\" + e)] = o[r] || (o[e] ? [[o[e], o]] : []), a, c, d;");
			js.Append("\r\n\t\tr[r.length] = [f, s || o], o[e] = function(e){");
			js.Append("\r\n\t\t\ttry{");
			js.Append("\r\n\t\t\t(e = e || event).preventDefault || (e.preventDefault = function(){e.returnValue = false;});");
			js.Append("\r\n\t\t\t\te.stopPropagation || (e.stopPropagation = function(){e.cancelBubble = true;});");
			js.Append("\r\n\t\t\t\te.target || (e.target = e.srcElement || null);");
			js.Append("\r\n\t\t\t\te.key = (e.which + 1 || e.keyCode + 1) - 1 || 0;");
			js.Append("\r\n\t\t\t}catch(f){}");
			js.Append("\r\n\t\t\tfor(d = 1, f = r.length; f; r[--f] && (a = r[f][0], o = r[f][1], a.call ? c = a.call(o, e) : (o._ = a, c = o._(e), o._ = null), d &= c !== false));");
			js.Append("\r\n\t\t\treturn e = null, !!d;");
			js.Append("\r\n\t\t}");
			js.Append("\r\n\t};");

			js.Append("\r\n</script>");	
			return(js.ToString());
		}
		#endregion

		#region Gestion des generic detail levels
		/// <summary>
		/// Permet d'ajouter un élément de la liste des niveaux de détail enregistrés
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom SaveGenericDetailLevelFromList
		/// </remarks>
		/// <param name="listName">Nom de la liste</param>
		/// <param name="optionText">Texte à ajouter</param>
		/// <param name="optionValue">Valeur à ajouter</param>
		/// <returns>JavaScript</returns>
		public static string SaveGenericDetailLevelFromList(string listName,string optionText,string optionValue){
			StringBuilder script=new StringBuilder(2000);
			script.Append("<script language=\"JavaScript\">");
			script.Append("\r\n\t var oN=opener.document.all.item('"+listName+"');");
			script.Append("\r\n\t oN.options[oN.length-1].text  = '"+optionText.Replace(@"\",@"\\")+"';");
			script.Append("\r\n\t oN.options[oN.length-1].value  = '"+optionValue+"';");
			//script.Append("\r\n\t oN.selectedIndex=oN.length-1;");			
			script.Append("\r\n\t setTimeout('window.close();',2000);");
			script.Append("\r\n</script>");	
			return(script.ToString());
		}


		/// <summary>
		/// Permet de supprimer un élément de la liste des niveaux de détail enregistrés
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom RemoveGenericDetailLevelFromList
		/// </remarks>
		/// <param name="listName">Nom de la liste</param>
		/// <param name="indexToRemove">Index de la liste à supprimer</param>
		/// <returns>JavaScript</returns>
		public static string RemoveGenericDetailLevelFromList(string listName,string indexToRemove){
			StringBuilder script=new StringBuilder(2000);
			script.Append("<script language=\"JavaScript\">");
			//script.Append("\r\nfunction remove(){");
			script.Append("\r\n\t var oN=opener.document.all.item('"+listName+"');");
			script.Append("\r\n\t oN.options["+indexToRemove+"]  = null;");
			script.Append("\r\n\t oN.selectedindex=0;");
			script.Append("\r\n\t setTimeout('window.close();',2000);");
			//script.Append("\r\n\t opener.document.forms[0]."+listName+".options["+indexToRemove+"]  = null;");
			//script.Append("\r\n\t opener.document.forms[0]."+listName+".selectedindex=1;");
			//script.Append("\r\n}");
			script.Append("\r\n</script>");	
			return(script.ToString());
		}

		#endregion

		#region Fonction popup
		/// <summary>
		/// JavaScript d'ouverture d'une popup dondt l'url, la largeur et la hauteur sont à préciser à l'appel du script
		/// </summary>
		/// <returns>Code JavaScript</returns>
		public static string Popup(){
			string script="<script language=\"JavaScript\"> ";
			script+="function popupOpen(page,width,height){";
			script+="	window.open(page,'','top=(screen.height-'+height+')/2, left=(screen.width-'+width+')/2,width='+width+',height='+height+',toolbar=no,scrollbars=no,resizable=no');";
			script+="}";
			script+="</script>";
			return script;
		}

		/// <summary>
		/// Key register for popupOpenBis
		/// </summary>
		public static string RESIZABLE_POPUP_REGISTER ="ScriptpopupReSize";
		/// <summary>
		/// PopUp Opening Javascript, height an width to set up
		/// </summary>
		/// <returns>JavaScript Code</returns>
		public static string ReSizablePopUp(){
			string script="<script language=\"JavaScript\"> ";
			script+="function popupOpenBis(page,width,height,resizable){";
			script+="	window.open(page,'','width='+width+',height='+height+',toolbar=no,scrollbars='+resizable+',resizable='+resizable+'');";
			script+="}";
			script+="</script>";
			return script;
		}
		/// <summary>
		/// Key register for OpenNewWindow
		/// </summary>
		public static string NEW_WINDOW_REGISTER = "OpenNewWindow";
		/// <summary>
		/// New windows javascript
		/// </summary>
		/// <returns>Javascript Code</returns>
		public static string OpenNewWindow(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction OpenNewWindow(link){");
			script.Append("\n\t\twindow.open(link"
				+",''"
				+",'toolbar=no, directories=no, status=yes, menubar=yes, width=1024, height=600, scrollbars=yes, location=no, resizable=yes');");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion
	
		#region Fonctions DynamicMediaPlan
		/// <summary>
		/// JavaScript qui permet de rendre dynamique les calendriers d'action
		/// Fermeture des catégories
		/// Zoom sur un support
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom DynamicMediaPlan
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string DynamicMediaPlan(){
			System.Text.StringBuilder t=new System.Text.StringBuilder(2010);
			t.Append("\n<!-- Ouverture fermeture du calendrier d'action -->\n");
			t.Append("<SCRIPT language=JavaScript>\n");
			t.Append("<!--\n");
			t.Append("var selectionned=0; \n");
			t.Append("var ancien;\n");
			t.Append("var ancien3;\n");
			t.Append("var ouvert3=false;\n");
			t.Append("var i;\n");
			t.Append("var zoomMedia=false;\n");
			t.Append("var tab=new Array();\n");
			t.Append("var nbElements;\n");
			//showHideContent3(id)
			t.Append("function showHideContent3(id) {\n");
			t.Append("\tvar oContent3 = document.all.item('calendartable');\n");
			t.Append("\tvar oContentP3= oContent3.all;\n");
			t.Append("\tvar taille=oContentP3.length;\n");
			t.Append("\tfor(i=0;i<taille;i++){\n");
			t.Append("\t\tif(oContentP3[i].id.indexOf(id+\"Content3\")>0){\n");
			t.Append("\t\t\tif(oContentP3[i].style.display==\"none\"){\n");
			t.Append("\t\t\t\toContentP3[i].style.display=\"inline\";\n");
			t.Append("\t\t\t}\n");
			t.Append("\t\t\telse{\n");
			t.Append("\t\t\t\toContentP3[i].style.display=\"none\";\n");
			t.Append("\t\t\t}\n");
			t.Append("\t\t}\n");
			t.Append("\t}\n");
			t.Append("}\n");
			//showAllContent3()
			t.Append("function showAllContent3() {\n");
			t.Append("\tvar oContent3 = document.all.item('calendartable');\n");
			t.Append("\tvar oContentP3= oContent3.all;\n");
			t.Append("\tvar taille=oContentP3.length;\n");
			t.Append("\tfor(i=0;i<taille;i++){\n");
			t.Append("\t\tif(oContentP3[i].id.indexOf(\"Content3\")>0){\n");
			t.Append("\t\t\toContentP3[i].style.display=\"inline\";\n");
			t.Append("\t\t}\n");
			t.Append("\t}\n");
			t.Append("}\n");
			//hideAllContent3()
			t.Append("function hideAllContent3() {\n");
			t.Append("\tvar oContent3 = document.all.item('calendartable');\n");
			t.Append("\tvar oContentP3= oContent3.all;\n");
			t.Append("\tvar taille=oContentP3.length;\n");
			t.Append("\tfor(i=0;i<taille;i++){\n");
			t.Append("\t\tif(oContentP3[i].id.indexOf(\"Content3\")>0){\n");
			t.Append("\t\t\toContentP3[i].style.display=\"none\";\n");
			t.Append("\t\t}\n");
			t.Append("\t}\n");
			t.Append("}\n");
			//computeTable()
			t.Append("function computeTable(){\n");
			t.Append("\tnbElements=0;\n");
			t.Append("\ttab=new Array();\n");
			t.Append("\tvar oContent3 = document.all.item('calendartable');\n");
			t.Append("\tvar oContentP3= oContent3.all;\n");
			t.Append("\tvar taille=oContentP3.length;\n");
			t.Append("\tfor(i=0;i<taille;i++){\n");
			t.Append("\t\tif(oContentP3[i].id.indexOf(\"Content\")>0){\n");
			t.Append("\t\t\ttab[nbElements]=new Array(oContentP3[i],oContentP3[i].style.display);\n");
			t.Append("\t\t\tnbElements++;\n");
			t.Append("\t\t}\n");
			t.Append("\t}\n");
			t.Append("}\n");
			// getTable()
			t.Append("function getTable(){\n");
			t.Append("\tvar oContent3;\n");
			t.Append("\tfor(i=0;i<nbElements;i++){\n");
			t.Append("\t\ttab[i][0].style.display=tab[i][1];\n");
			t.Append("\t}\n");
			t.Append("}\n");
			// showHideAllContent3(name,id)
			t.Append("function showHideAllContent3(name,id) {\n");
			t.Append("\tvar display=\"\";\n");
			t.Append("\tvar oContent3 = document.all.item('calendartable');\n");
			t.Append("\tvar oContentP3= oContent3.all;\n");
			t.Append("\tvar taille=oContentP3.length;\n");
			t.Append("\tvar currentContentP3= document.all.item(name+\"Content3\");\n");
			t.Append("\tif(zoomMedia){\n");
			t.Append("\t\tgetTable();\n");
			t.Append("\t\tdisplay=\"inline\";\n");
			t.Append("\t\tdocument.all.item('pictofermer').style.display=\"inline\";\n");
			t.Append("\t\tdocument.all.item('pictoouvrir').style.display=\"inline\";\n");
			t.Append("\t\tzoomMedia=false;\n");
			t.Append("\t}\n");
			t.Append("\telse{\n");
			t.Append("\t\tcomputeTable();\n");
			t.Append("\t\tdocument.all.item('pictofermer').style.display=\"none\";");
			t.Append("\t\tdocument.all.item('pictoouvrir').style.display=\"none\";");
			t.Append("\t\tdisplay=\"none\";\n");
			t.Append("\t\tfor(i=0;i<taille;i++){\n");
			t.Append("\t\t\tif(oContentP3[i].id.indexOf(\"Content\")>0){\n");
			t.Append("\t\t\t\toContentP3[i].style.display=display;\n");
			t.Append("\t\t\t}\n");
			t.Append("\t\t}\n");
			t.Append("\tcurrentContentP3.style.display=\"inline\";\n");
			t.Append("\tzoomMedia=true;\n");
			t.Append("\t}\n");
			t.Append("}\n");
			t.Append("-->\n");
			t.Append("</SCRIPT>\n");
			return(t.ToString());
		}
		#endregion
		
		#region myAdExpress
		/// <summary>
		/// Affiche le détail d'une sélection dans mon AdExpress
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom myAdExpress
		/// </remarks>
		/// <param name="idSession">identifiant de la session Mon AdExpress</param>
		/// <param name="webSession">Session Client</param>
		/// <returns>Code JavaScript</returns>
		public static string MyAdExpress(string idSession,WebSession webSession){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append(" <script language=\"JavaScript\">");			
			t.Append("function myAdExpress(){ ");
			t.Append(" if(document.Form2.idPopup.name!=\"namePopup\"){ ");
			t.Append(" window.open('/Private/MyAdExpress/MySessionDetailPopUp.aspx?idMySession='+document.Form2.idPopup.name+'&idSession="+idSession+"','','width=660,height=700,toolbar=no,scrollbars=yes,resizable=no'); ");
			t.Append(" } ");
			t.Append(" else{ ");
			t.Append(" alert(\""+GestionWeb.GetWebWord(831,webSession.SiteLanguage)+"\"); ");
			t.Append(" } ");
			t.Append(" } ");
			t.Append("</script>");
			return t.ToString();
		}
		#endregion

		#region insertHidden
		/// <summary>
		/// Insert l'identifiant de la session dans les champs hidden
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom insertHidden
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string InsertHidden(){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append(" <script language=\"JavaScript\">");	
			t.Append(" function insertIdMySession4(idSession,idRepertory){ "); 			
			t.Append(" document.Form2.idMySession.name=\"CKB_\"+idSession+\"_\"+idRepertory; ");
			t.Append(" document.Form2.idPopup.name=idSession; ");
			t.Append(" } ");
			t.Append("</script>");
			return t.ToString();
		}
		#endregion

		#region ShowHideContent
        /// <summary>
        /// Javascript permettant d'ouvrir/Fermer des blocs div.
        /// </summary>
        /// <remarks>
        /// Le script est à enregistrer sous le nom ShowHideContent
        /// </remarks>
        /// <returns>Code JavaScript</returns>
        public static string ShowHideCalendar() {
            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            t.Append(" <script language=\"JavaScript\">");
            t.Append(" function ShowHideCalendar(id)");
            t.Append(" { ");
            t.Append(" var oContent = document.all.item(id+\"Content\"); ");
            t.Append(" if (ancien!=null){ ");
            t.Append("	if (id+\"Content\"==ancien && ouvert==true){");
            t.Append(" var oAncien=document.all.item(ancien); ");
            t.Append(" oAncien.style.display=\"none\"; ");
            t.Append(" ouvert=false; ");
            t.Append(" return; ");
            t.Append(" } ");
            t.Append(" var oAncien=document.all.item(ancien); ");
            t.Append(" oAncien.style.display=\"none\"; ");
            t.Append(" } ");
            t.Append(" ancien=id+\"Content\"; ");
            t.Append(" oContent.style.display = \"\"; ");
            t.Append(" ouvert=true; ");
            t.Append(" } ");
            t.Append("</script>");
            return (t.ToString());
        }

		/// <summary>
		/// Javascript permettant d'ouvrir/Fermer des blocs div.
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom ShowHideContent
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string ShowHideContent(){ 
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append(" <script language=\"JavaScript\">");			
			t.Append(" var ancien5; ");
			t.Append(" var ancien4; ");
			t.Append(" var ancien6; ");
			t.Append(" var ancien; ");
			t.Append(" var ancien7; ");
			
			t.Append(" function showHideContent(id)");
			t.Append(" { ");
			t.Append(" var oContent = document.all.item(id+\"Content\"); ");
			t.Append(" if (ancien!=null){ ");
			t.Append("	if (id+\"Content\"==ancien && ouvert==true){");
			t.Append(" var oAncien=document.all.item(ancien); ");
			t.Append(" oAncien.style.display=\"none\"; ");
			t.Append(" ouvert=false; ");
			t.Append(" return; ");
			t.Append(" } ");
			t.Append(" var oAncien=document.all.item(ancien); ");
			t.Append(" oAncien.style.display=\"none\"; ");
			t.Append(" } ");
			t.Append(" ancien=id+\"Content\"; ");
			t.Append(" oContent.style.display = \"\"; ");
			t.Append(" ouvert=true; ");
			t.Append(" } ");

			t.Append(" function showHideContent5(id)");
			t.Append(" { ");
			t.Append(" var oContent5 = document.all.item(id+\"Content5\"); ");
			t.Append(" if (ancien5!=null){ ");
			t.Append("	if (id+\"Content5\"==ancien5 && ouvert5==true){");
			t.Append(" var oAncien5=document.all.item(ancien5); ");
			t.Append(" oAncien5.style.display=\"none\"; ");
			t.Append(" ouvert5=false; ");
			t.Append(" return; ");
			t.Append(" } ");
			t.Append(" var oAncien5=document.all.item(ancien5); ");
			t.Append(" oAncien5.style.display=\"none\"; ");
			t.Append(" } ");
			t.Append(" ancien5=id+\"Content5\"; ");
			t.Append(" oContent5.style.display = \"\"; ");
			t.Append(" ouvert5=true; ");
			t.Append(" } ");
			
			t.Append(" function showHideContent4(id)");
			t.Append(" { ");
			t.Append(" var oContent4 = document.all.item(id+\"Content4\"); ");
			t.Append(" if (ancien4!=null){ ");
			t.Append("	if (id+\"Content4\"==ancien4 && ouvert4==true){");
			t.Append(" var oAncien4=document.all.item(ancien4); ");
			t.Append(" oAncien4.style.display=\"none\"; ");
			t.Append(" ouvert4=false; ");
			t.Append(" return; ");
			t.Append(" } ");
			t.Append(" var oAncien4=document.all.item(ancien4); ");
			t.Append(" oAncien4.style.display=\"none\"; ");
			t.Append(" } ");
			t.Append(" ancien4=id+\"Content4\"; ");
			t.Append(" oContent4.style.display = \"\"; ");
			t.Append(" ouvert4=true; ");
			t.Append(" } ");

			t.Append(" function showHideContent6(id)");
			t.Append(" { ");
			t.Append(" var oContent6 = document.all.item(id+\"Content6\"); ");
			t.Append(" if (ancien6!=null){ ");
			t.Append("	if (id+\"Content6\"==ancien6 && ouvert6==true){");
			t.Append(" var oAncien6=document.all.item(ancien6); ");
			t.Append(" oAncien6.style.display=\"none\"; ");
			t.Append(" ouvert6=false; ");
			t.Append(" return; ");
			t.Append(" } ");
			t.Append(" var oAncien6=document.all.item(ancien6); ");
			t.Append(" oAncien6.style.display=\"none\"; ");
			t.Append(" } ");
			t.Append(" ancien6=id+\"Content6\"; ");
			t.Append(" oContent6.style.display = \"\"; ");
			t.Append(" ouvert6=true; ");
			t.Append(" } ");

			

			t.Append(" function showHideContent7(id)");
			t.Append(" { ");
			t.Append(" var oContent7 = document.all.item(id+\"Content7\"); ");
			t.Append(" if (ancien7!=null){ ");
			t.Append("	if (id+\"Content7\"==ancien7 && ouvert7==true){");
			t.Append(" var oAncien7=document.all.item(ancien7); ");
			t.Append(" oAncien7.style.display=\"none\"; ");
			t.Append(" ouvert7=false; ");
			t.Append(" return; ");
			t.Append(" } ");
			t.Append(" var oAncien7=document.all.item(ancien7); ");
			t.Append(" oAncien7.style.display=\"none\"; ");
			t.Append(" } ");
			t.Append(" ancien7=id+\"Content7\"; ");
			t.Append(" oContent7.style.display = \"\"; ");
			t.Append(" ouvert7=true; ");
			t.Append(" } ");

			t.Append("</script>");

			return t.ToString();
		}
		#endregion

		#region showHideContent1
		/// <summary>
		/// Javascript permettant d'ouvrir/Fermer des blocs div.
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom showHideContent1 
		/// </remarks>
		/// <param name="i">Identifiant</param>
		/// <returns>Code JavaScript</returns>
		public static string ShowHideContent1(int i){
			
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append(" <script language=\"JavaScript\">");	
			t.Append(" var ancien"+i+"; ");
			
			t.Append(" function showHideContent"+i+"(id)");
			t.Append(" { ");
			t.Append(" var oContent"+i+" = document.all.item(id+\"Content"+i+"\"); ");
			t.Append(" if (ancien"+i+"!=null){ ");
			t.Append("	if (id+\"Content"+i+"\"==ancien"+i+" && ouvert"+i+"==true){");
			t.Append(" var oAncien"+i+"=document.all.item(ancien"+i+"); ");
			t.Append(" oAncien"+i+".style.display=\"none\"; ");
			t.Append(" ouvert"+i+"=false; ");
			t.Append(" return; ");
			t.Append(" } ");
			t.Append(" var oAncien"+i+"=document.all.item(ancien"+i+"); ");
			t.Append(" oAncien"+i+".style.display=\"none\"; ");
			t.Append(" } ");
			t.Append(" ancien"+i+"=id+\"Content"+i+"\"; ");
			t.Append(" oContent"+i+".style.display = \"\"; ");
			t.Append(" ouvert"+i+"=true; ");
			t.Append(" } ");
			t.Append("</script>");

			return t.ToString();
		}
		#endregion
		
		#region Script d'ouverture d'un création Press
		/// <summary>
		/// Construit le code HTML de la fonction JavaScript qui permet d'ouvrir une création Press
		/// </summary>
		/// <remarks>
		/// Enregistrer sous openPressCreation
		/// </remarks>
		/// <returns>Code HTML du script</returns>
		public static string OpenPressCreation(){
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openPressCreation(file){");
			script.Append("\n\t\twindow.open('/Private/Results/ZoomCreationPopUp.aspx?creation='+file,"
				+"'',"
				+"\"left=\"+((screen.width-530)/2)+\", top=\"+((screen.height-700)/2)+\",toolbar=0, directories=0, status=0, menubar=0,width=530 , height=700, scrollbars=1, location=0, resizable=1\");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Script d'ouverture de la PopUp de téléchargement d'une vidéo ou d'un fichier son
		/// <summary>
		/// Ouvre la popUp qui permet de lire ou de télécharger une création radio ou tv
		/// </summary>
		/// <remarks>
		/// A enregistrer sous openDownload
		/// </remarks>
		/// <returns>Code javascript généré</returns>
		public static string OpenDownload(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openDownload(file,idSession, idVehicle){");
//			script.Append("\n\t\twindow.open(\"/Private/Results/AccessDownloadCreationsPopUp.aspx?idSession=\"+idSession+\"&idVehicle=\"+idVehicle+\"&creation=\"+file, '', \"toolbar=0, directories=0, status=0, menubar=0, width=440, height=300, scrollbars=0, location=0, resizable=0\");");
			script.Append("\n\t\twindow.open(\"/Private/Results/DownloadCreationsPopUp.aspx?idSession=\"+idSession+\"&idVehicle=\"+idVehicle+\"&creation=\"+file, '', \"top=\"+(screen.height-420)/2+\", left=\"+(screen.width-830)/2+\",toolbar=0, directories=0, status=0, menubar=0, width=830, height=420, scrollbars=0, location=0, resizable=0\");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}


		/// <summary>
		/// Ouvre la popUp qui permet de lire ou de télécharger une création radio ou tv
		/// </summary>
		/// <remarks>
		/// A enregistrer sous openAlertsDownload
		/// </remarks>
		/// <returns>Code javascript généré</returns>
		public static string OpenAlertsDownload(string pagePath){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openAlertsDownload(file,parameters, idVehicle,idSlogan){");
			script.Append("\n\t\twindow.open(\""+pagePath+"?parameters=\"+parameters+\"&idVehicle=\"+idVehicle+\"&creation=\"+file+\"&idSlogan=\"+idSlogan, '', \"top=\"+(screen.height-420)/2+\", left=\"+(screen.width-830)/2+\",toolbar=0, directories=0, status=0, menubar=0, width=830, height=420, scrollbars=0, location=0, resizable=0\");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region script d'ouverture d'une popup de détail des insertions
		/// <summary>
		/// Script d'ouverture de la popUp de détail des insertions.
		/// </summary>
		/// <remarks>
		/// Script à enregistrer sous le nom openCreation
		/// </remarks>
		/// <returns>Code du scriptopenCreation</returns>
		public static string OpenCreation(){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\n\tfunction openCreation(idSession,ids,zoomDate){");
			t.Append("\n\t\twindow.open('/Private/Results/MediaInsertionsCreationsResults.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&param='+"+GenerateNumber()+", '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=540, scrollbars=1, location=0, resizable=1\");");
            //t.Append("\n\t\twindow.open('/Private/Results/Creatives.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&param='+" + GenerateNumber() + ", '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=540, scrollbars=1, location=0, resizable=1\");");
			t.Append("\n\t}");
			t.Append("\n</script>");
			return t.ToString();
		}

        /// <summary>
        /// Script d'ouverture de la popUp de détail des insertions.
        /// </summary>
        /// <remarks>
        /// Script à enregistrer sous le nom OpenInsertions
        /// </remarks>
        /// <returns>Code du scriptopenCreation</returns>
        public static string OpenInsertions() {
            System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
            t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
            t.Append("\n\tfunction OpenInsertions(idSession,ids,zoomDate){");
            t.Append("\n\t\twindow.open('/Private/Results/MediaInsertionsCreationsResults.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&param='+"+GenerateNumber()+", '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=540, scrollbars=1, location=0, resizable=1\");");
            t.Append("\n\t}");
            t.Append("\n</script>");
            return t.ToString();
        }

        /// <summary>
        /// Script d'ouverture de la popUp de détail des versions
        /// </summary>
        /// <remarks>
        /// Script à enregistrer sous le nom OpenCreatives
        /// </remarks>
        /// <returns>Code du scriptopenCreation</returns>
        public static string OpenCreatives() {
            System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
            t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
            t.Append("\n\tfunction OpenCreatives2(idSession,ids,zoomDate,universId,moduleId){");
            t.Append("\n\t\twindow.open('/Private/Results/Creatives.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&idUnivers='+universId+'&moduleId='+moduleId+'&param='+" + GenerateNumber() + ", '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=540, scrollbars=1, location=0, resizable=1\");");
            t.Append("\n\t}");
            t.Append("\n</script>");
            return t.ToString();
        }
        /// <summary>
        /// Script d'ouverture de la popUp de détail des insertions (nouvelle page)
        /// </summary>
        /// <remarks>
        /// Script à enregistrer sous le nom OpenInsertion
        /// </remarks>
        /// <returns>Code du scriptopenCreation</returns>
        public static string OpenInsertion() {
            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
            t.Append("\n\tfunction OpenInsertion(idSession,ids,zoomDate,universId,moduleId){");
            t.Append("\n\t\twindow.open('/Private/Results/Insertions.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&idUnivers='+universId+'&moduleId='+moduleId+'&param='+" + GenerateNumber() + ", '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=540, scrollbars=1, location=0, resizable=1\");");
            t.Append("\n\t}");
            t.Append("\n</script>");
            return t.ToString();
        }

		#endregion

		#region script d'ouverture d'une popup de détail des insertions par Parution
		/// <summary>
		/// Script d'ouverture de la popUp de détail des insertions.
		/// </summary>
		/// <remarks>
		/// Script à enregistrer sous le nom openCreationByParution
		/// </remarks>
		/// <returns>Code du scriptopenCreation</returns>
		public static string OpenCreationByParution(){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\n\tfunction openCreationByParution(idSession,ids,zoomDate){");
			t.Append("\n\t\twindow.open('/Private/Results/MediaCreationsByParutionResults.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate+'&param='+"+GenerateNumber()+", '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1\");");
			t.Append("\n\t}");
			t.Append("\n</script>");
			return t.ToString();
		}

		#endregion

		#region Script d'ouverture d'une popup de détail des insertions pour l'alerte concurrentielle
		/// <summary>
		/// Script d'ouverture d'une popup de détail des insertions pour l'alerte concurrentielle
		/// </summary>
		/// <remarks>
		/// Script à enregistrer sous le nom OpenCreationCompetitorAlert
		/// </remarks>
		/// <returns>Code du script</returns>
		public static string OpenCreationCompetitorAlert(){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\n\tfunction OpenCreationCompetitorAlert(idSession,ids,zoomDate){");
			t.Append("\n\t\twindow.open('/Private/Results/CompetitorAlertCreationsResults.aspx?idSession='+idSession+'&ids='+ids+'&zoomDate='+zoomDate, '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1\");");
			t.Append("\n\t}");
			t.Append("\n</script>");
			return t.ToString();

		}

		#endregion

		#region Script d'ouverture d'une popup de plan média
		/// <summary>
		/// Script d'ouverture d'une popup de plan média alerte
		/// </summary>
		/// <remarks>
		/// Script à enregistrer sous le nom OpenMediaPlan
		/// </remarks>
		/// <returns>Code du script</returns>
		public static string OpenMediaPlanAlert(){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\n\tfunction OpenMediaPlanAlert(idSession,id,Level){");
            t.Append("\n\t\twindow.open('/Private/Results/MediaSchedulePopUp.aspx?idSession='+idSession+'&id='+id+'&Level='+Level, '', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\", toolbar=0, directories=0, status=0, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1\");");
			t.Append("\n\t}");
			t.Append("\n</script>");
			return t.ToString();

		}

		#endregion

		#region Alerte d'erreur et fermeture de la page responsable
		/// <summary>
		/// Script qui permet à un code serveur d'envoyer une erreur au client et de fermer la page responsable de l'erreur
		/// </summary>
		/// <param name="msg">Message d'erreur</param>
		/// <returns>Code Javascript</returns>
		public static string ErrorCloseScript(string msg){
			return "\n<script language=\"JavaScript\">\nalert(\""+msg.Replace("\n"," ")+"\");\nwindow.close();\n</script>";
		}

		/// <summary>
		/// Script qui permet fermer la fenêtre
		/// </summary>
		/// <remarks>Nom; closeSript</remarks>
		/// <returns>Code Javascript</returns>
		public static string CloseScript(){
			return "\n<script language=\"JavaScript\">\nwindow.close();\n</script>";
		}
		#endregion

		#region Alerte d'erreur
		/// <summary>
		/// Script affichant un message d'erreur
		/// </summary>
		/// <param name="msg">Message d'erreur</param>
		/// <returns>Code JavaScript</returns>
		public static string Alert(string msg){
			return"<script language=\"JavaScript\" type=\"text/JavaScript\">alert(\""+msg+"\");</script>";
		}

		/// <summary>
		/// Script affichant un message d'erreur
		/// </summary>
		/// <param name="msg">Message d'erreur</param>
		/// <returns>Code JavaScript</returns>
		public static string AlertWithWindowClose(string msg) {
			return "<script language=\"JavaScript\" type=\"text/JavaScript\">alert(\"" + msg + "\");window.close();</script>";
		}
		#endregion

		#region Ouverture d'une PopUp donnant le détail des insertions dans Excel
		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcel
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenExcel(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openExcel(idSession, period, ids){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Excel/MediaInsertionsCreations.aspx?idSession=\"+idSession"
				+"+\"&zoomDate=\"+period"
				+"+\"&ids=\"+ids"
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}

		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel
		/// </summary>
		/// <remarks>
		/// Enregistré sous openMediaDetailExcel
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenMediaDetailExcel(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openMediaDetailExcel(idSession, period, ids,idVehicleFromTab){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Excel/MediaInsertionsCreations.aspx?idSession=\"+idSession"
				+"+\"&zoomDate=\"+period"
				+"+\"&ids=\"+ids"
				+"+\"&idVehicleFromTab=\"+idVehicleFromTab"
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion
		 
		#region Ouverture d'une PopUp donnant le détail des insertions par parution dans Excel
			 /// <summary>
			 /// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel
			 /// </summary>
			 /// <remarks>
			 /// Enregistré sous openExcel
			 /// </remarks>
			 /// <returns>Code Javascript</returns>
			 public static string OpenExcelByParution(){
				 StringBuilder script = new StringBuilder(1000);
				 script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
				 script.Append("\n\tfunction openExcel(idSession, period, ids){");
				 script.Append("\n\t\twindow.open("+
					 "\"/Private/Results/Excel/MediaCreationsByParution.aspx?idSession=\"+idSession"
					 +"+\"&zoomDate=\"+period"
					 +"+\"&ids=\"+ids"
					 +",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
					 +");");
				 script.Append("\n\t}");
				 script.Append("\n</script>");
				 return script.ToString();
			 }
		#endregion

		#region Ouverture d'une popup pour le GAD
		/// <summary>
		/// Construit un javascript pur le GAD
		/// </summary>
		/// <remarks>
		/// Enregistré sous openGad
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string OpenGad(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openGad(idSession, advertiser, idAddress){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Gad.aspx?idSession=\"+idSession"
				+"+\"&advertiser=\"+advertiser"
				+"+\"&idAddress=\"+idAddress"
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=0, status=0, menubar=0, width=500, height=300, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}

		#endregion

		#region Ouverture d'une PopUp donnant le détail des insertions dans Excel pour une alerte concurrentielle
		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel pour une alerte concurrentiel
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcelCompetitorAlert
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenExcelCompetitorAlert(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openExcelCompetitorAlert(idSession,ids){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Excel/CompetitorAlertCreations.aspx?idSession=\"+idSession"
				+"+\"&ids=\"+ids"
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Ouverture d'une PopUp donnant dans Excel pour une alerte plan média
		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel pour une alerte plan média
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcelCompetitorAlert
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenExcelMediaPlanAlert(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openExcelMediaPlanAlert(idSession){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Excel/MediaPlanAlertResults.aspx?idSession=\"+idSession"			
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}

		/// <summary>
		/// Construit un JavaScript ouvrant le détail des unités dans une PopUp Excel pour une alerte plan média
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcelCompetitorAlert
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenValueExcelMediaPlanAlert(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openValueExcelMediaPlanAlert(idSession){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/ValueExcel/MediaPlanAlertResults.aspx?idSession=\"+idSession"			
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Ouverture d'une PopUp donnant dans Excel pour un plan média
		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel pour une alerte plan média
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcelCompetitorAnalysis
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenExcelMediaPlanAnalysis(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openExcelMediaPlanAnalysis(idSession){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Excel/MediaPlanResults.aspx?idSession=\"+idSession"			
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}

		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel pour une alerte plan média
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcelCompetitorAnalysis
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenValueExcelMediaPlanAnalysis(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openValueExcelMediaPlanAnalysis(idSession){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/ValueExcel/MediaPlanResults.aspx?idSession=\"+idSession"			
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Ouverture d'une PopUp donnant dans Excel pour une alerte plan média
		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel pour une alerte plan média
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcelZoomMediaPlanAlert
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenExcelZoomMediaPlanAlert(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openExcelZoomMediaPlanAlert(idSession,zoomDate){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Excel/ZoomMediaPlanAnalysisResults.aspx?idSession=\"+idSession"	
				+"+\"&zoomDate=\"+zoomDate"
			//	+"&zoomDate=\"+zoomDate"
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}

		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel pour une alerte plan média
		/// </summary>
		/// <remarks>
		/// Enregistré sous openExcelZoomMediaPlanAlert
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenValueExcelZoomMediaPlanAlert(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openValueExcelZoomMediaPlanAlert(idSession,zoomDate){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/ValueExcel/ZoomMediaPlanAnalysisResults.aspx?idSession=\"+idSession"	
				+"+\"&zoomDate=\"+zoomDate"
				//	+"&zoomDate=\"+zoomDate"
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Gestion de l'évènements "enter"
		/// <summary>
		/// JavaScript permettant d'intercepter l'évènement "enter" et de faire un appel serveur imputé à un contrôle quelconque passé en paramètre.
		/// </summary>
		/// <remarks>
		/// Enregistré sous le nom trapEnter
		/// </remarks>
		/// <param name="validatedComponent">Composant qui sera vu comme le responsable du postBack</param>
		/// <returns>Code JavaScript</returns>
		public static string TrapEnter(string validatedComponent){
			string scriptTrapEnter="";
			scriptTrapEnter += "\n<script language=\"JavaScript\">";
			scriptTrapEnter += "\nfunction trapEnter(){";
			scriptTrapEnter += "\nif((event.which && event.which == 13)||(event.keyCode && event.keyCode == 13)){";
			scriptTrapEnter += "\n__doPostBack('"+validatedComponent+"','');}";
			scriptTrapEnter += "\n}\n</script>";
			return scriptTrapEnter;
		}
		#endregion		

		#region Sélection de tous les annonceurs
		/// <summary>
		/// Sélectionne toutes les cases d'un père
		/// </summary>
		/// <remarks>
		/// Enregistrer sous allSelection
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string AllSelection(){
			
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("function allSelection(idParent,debut){");
			script.Append("var aInput=document.Form2;");
			script.Append("var longueur=aInput.elements.length;");
			script.Append("var selectionned=0;");
			script.Append("var other=0;");		
			script.Append("for(i=debut;i<longueur;i++){	");
            script.Append("if(aInput.elements[i].value==idParent && aInput.elements[i].checked==false && aInput.elements[i].disabled==false ){");
			script.Append("selectionned=1;");
			script.Append("break;");						
			script.Append("	}else{selectionned=0;}");				
			script.Append("}");
			script.Append("for(i=debut;i<longueur;i++){");
			script.Append(" if(selectionned==1){ ");
            script.Append("	if(aInput.elements[i].value==idParent && aInput.elements[i].disabled==false){ ");
            script.Append("	aInput.elements[i].checked=true; ");
			script.Append("	other=1; ");
			script.Append("} ");
            script.Append("	if(other==1 && aInput.elements[i].value!=idParent && aInput.elements[i].disabled==false){ ");
			script.Append("	other=0; ");
			script.Append("	break; ");
			script.Append("	} ");
			script.Append("	} ");					
			script.Append("	if(selectionned==0){ ");
            script.Append("	if(aInput.elements[i].value==idParent && aInput.elements[i].disabled==false){ ");
            script.Append("	aInput.elements[i].checked=false; ");
			script.Append("	other=1; ");
			script.Append("	} ");
            script.Append("	if(other==1 && aInput.elements[i].value!=idParent && aInput.elements[i].disabled==false){ ");
			script.Append("	other=0; ");
			script.Append("	break; ");
			script.Append("	} ");
			script.Append(" } ");
			script.Append("	} ");				
			script.Append(" } ");
			script.Append("\n</script>");
			 
			return script.ToString();
		}
		



		#endregion

		#region integration automatique

		/// <summary>
		/// Coche toutes les cases fils lors de la sélection 
		/// d'un case intégration automatique
		/// </summary>
		/// <remarks>
		/// Enregistrer sous integration
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string Integration(){
		
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("\n<!--");
            script.Append("\nfunction integration(idParent,debut,e){");
            script.Append("\n\tvar aInput=document.Form2; ");
            script.Append("\n\tvar longueur=aInput.elements.length; ");
            script.Append("\n\tvar target; ");
            script.Append("\n\tif (\"activeElement\" in document){ /* Si IE*/");
            script.Append("\n\t\ttarget = document.activeElement;");
            script.Append("\n\t} ");
            script.Append("\n\telse{ ");
            script.Append("\n\t\ttarget = e ? e.explicitOriginalTarget : null;  /* Si Firefox*/");
            script.Append("\n\t}");
            script.Append("\n\tvar other=0;");
            script.Append("\n\tif(target.checked==true){ ");
            script.Append("\n\t\tfor(i=debut;i<longueur;i++){ ");
            script.Append("\n\t\t\tif(aInput.elements[i].value==idParent){ ");
            script.Append("\n\t\t\t\taInput.elements[i].checked=true;");
            script.Append("\n\t\t\t\tother=1; ");
            script.Append("\n\t\t\t} ");
            script.Append("\n\t\t\tif(other==1 && aInput.elements[i].value!=idParent){ ");
            script.Append("\n\t\t\t\tother=0; ");
            script.Append("\n\t\t\t\t\tbreak; ");
            script.Append("\n\t\t\t} ");
            script.Append("\n\t\t}");
            script.Append("\n\t} ");
            script.Append("\n\tif(target.checked==false){ ");
            script.Append("\n\t\tfor(i=debut;i<longueur;i++){ ");
            script.Append("\n\t\t\tif(aInput.elements[i].value==idParent){ ");
            script.Append("\n\t\t\t\taInput.elements[i].checked=false;");
            script.Append("\n\t\t\t\tother=1; ");
            script.Append("\n\t\t\t} ");
            script.Append("\n\t\t\tif(other==1 && aInput.elements[i].value!=idParent){ ");
            script.Append("\n\t\t\t\tother=0; ");
            script.Append("\n\t\t\t\tbreak; ");
            script.Append("\n\t\t\t} ");
            script.Append("\n\t\t}");
            script.Append("\n\t} ");
            script.Append("\n} ");
            script.Append("\n-->");
			script.Append("\n</script>");

			return script.ToString();
		}
		#endregion

		#region Sélection de toutes éléments d'un niveau (les références, groupes...)

		/// <summary>
		/// Script permettant de sélectionner toutes les éléments d'un niveau dans la sélection
		/// </summary>
		/// <remarks>
		/// Enregistrer sous allSelectionRef
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string AllLevelSelection(){ 

			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("");
			script.Append(" function allSelectionRef(){ ");	
			script.Append(" var aInput=document.Form2; ");
			script.Append(" var longueur=aInput.elements.length; ");
			script.Append(" var selectionned=0; ");
			script.Append(" var other=0;  ");
			script.Append(" for(i=0;i<longueur;i++){ ");
            script.Append("	if(aInput.elements[i].value==\"child\" && aInput.elements[i].checked==false){ ");
			script.Append(" selectionned=1; ");
			script.Append(" break; ");						 
			script.Append(" }else{selectionned=0;}	");				
			script.Append(" } ");
			script.Append(" for(i=0;i<longueur;i++){ ");
			script.Append("	if(selectionned==1){ ");
            script.Append("	if(aInput.elements[i].value==\"child\"){ ");
            script.Append("	aInput.elements[i].checked=true; ");
			script.Append("	other=1; ");
			script.Append("	} ");
            script.Append("	if(other==1 && aInput.elements[i].value!=\"child\"){ ");
			script.Append(" other=0; "); 
			script.Append("	break; ");
			script.Append("	} ");
			script.Append(" } ");
			script.Append("	if(selectionned==0){ ");
            script.Append("	if(aInput.elements[i].value==\"child\"){ ");
            script.Append(" aInput.elements[i].checked=false; ");
			script.Append("	other=1; ");
			script.Append("	} ");
            script.Append("	if(other==1 && aInput.elements[i].value!=\"child\"){ ");
			script.Append("	other=0; ");
			script.Append("	break; ");
			script.Append("	} ");
			script.Append("	} ");
			script.Append("	} ");
		
			script.Append("	} ");
            script.Append("");
			script.Append("\n</script>");

			return script.ToString();

		}

		#endregion

		#region DivDisplayer
		/// <summary>
		/// Javascript permettant d'ouvrir/Fermer un div 
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom DivDisplayer
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string DivDisplayer(){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append(" <script language=\"JavaScript\">");	
			t.Append(" function DivDisplayer(id)");
			t.Append(" { ");
			t.Append(" var oContent = document.all.item(id); ");
			t.Append("if (oContent.style.display =='') {"); 
			t.Append("oContent.style.display = 'none'; ");
			t.Append("}"); 
			t.Append("else { ");
			t.Append("oContent.style.display=''; ");			
			t.Append(" } ");
			t.Append(" } ");
			t.Append("</script>");

			return t.ToString();
		}	
	
		/// <summary>
		/// Javascript permettant d'ouvrir/Fermer un div 
		/// </summary>
        /// <param name="themeName">Theme name</param>
		/// <remarks>
		/// Le script est à enregistrer sous le nom DivDisplayer
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string DivDisplayerOption(string themeName){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append(" <script language=\"JavaScript\">");	
			t.Append(" function DivDisplayerOption(id)");
			t.Append(" { ");
			t.Append(" var oContent = document.all.item(id); ");
			t.Append("if (oContent.style.display =='') {"); 
			t.Append("oContent.style.display = 'none'; ");

			t.Append("document.getElementById('ImgOpenOption').src = \"/App_Themes/"+themeName+"/Images/Culture/Others/OpenOptions.gif\"; ");

			t.Append("}"); 
			t.Append("else { ");
			t.Append("oContent.style.display=''; ");
            t.Append("document.getElementById('ImgOpenOption').src = \"/App_Themes/"+themeName+"/Images/Culture/Others/CloseOptions.gif\"; ");
			t.Append(" } ");
			t.Append(" } ");
			t.Append("</script>");

			return t.ToString();
		}	
		#endregion

		#region Ouvrir / Fermer un tableau de div
		/// <summary>
		/// Javascript permettant d'ouvrir/Fermer un tableau de div
		/// </summary>
		/// <remarks>
		/// Le script est à enregistrer sous le nom ExpandColapseAllDivs
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string ExpandColapseAllDivs() {
			System.Text.StringBuilder t=new System.Text.StringBuilder(500);
			t.Append("\n <script language=\"JavaScript\">");
			t.Append("\n <!-- ");
			t.Append("\n var allDivOpen = false; ");

			t.Append("\n function ExpandColapseAllDivs(ids)");
			t.Append("\n { ");
			t.Append("\n var divs = ids.split(','); ");
//			t.Append("\n var IsDivOpen = 0; ");
			
			 
			
			//Verifie si un calque est ouvert
//				t.Append("\n for(i=0; i<divs.length;i++) {"); 
//					t.Append("if (document.all.item(divs[i])!=null && document.all.item(divs[i]).style.display =='') {"); 
//
//					//Fermer tous les calques si 1 est  ouvert 
//						t.Append("\n for(j=0; j<divs.length;j++) {"); 
//							t.Append("if(document.all.item(divs[j])!=null)document.all.item(divs[j]).style.display ='none'; ");
//						t.Append("}");
//						t.Append("IsDivOpen = 1; "); 	
//					t.Append("\n\t\t break; ");
//					t.Append("\n}"); 
//				t.Append("}"); 

			//Ouvrir tous les calques
				
//				t.Append("if (IsDivOpen == 0) {"); 
				t.Append("if (!allDivOpen) {"); 
					t.Append("\n for(h=0; h<divs.length;h++) {"); 
						t.Append("if(document.all.item(divs[h])!=null)document.all.item(divs[h]).style.display =''; ");
					t.Append("}");			
				t.Append("}");
 			
//				t.Append("if (IsDivOpen == 1) {"); 
				t.Append("if (allDivOpen) {"); 			
					t.Append("\n for(h=0; h<divs.length;h++) {"); 
						t.Append("if(document.all.item(divs[h])!=null)document.all.item(divs[h]).style.display ='none'; ");
					t.Append("}");			
				t.Append("}");

				t.Append("\n  allDivOpen = !allDivOpen; ");
	 				
			t.Append(" } ");
			t.Append("\n //--> ");
			t.Append("</script>");

			return t.ToString();			
		}	

		
		
		#endregion

		#region selection toutes les variétés d'un groupe
			
		/// <summary>
		/// Script permettant de sélectionner toutes les variétés dans sélection
		/// de groupe
		/// </summary>
		/// <remarks>
		///  Enregistrer sous SelectAllSegments
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string SelectAllChilds()
		{ 
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n function SelectAllChilds(chkName){ ");
			script.Append("\n var nbElement = document.getElementById(chkName).getElementsByTagName(\'input\').length; ");
			script.Append("\n var sel=0; ");

			// verifie si un element est coché
			script.Append("\n for(i=0;i<nbElement; i++){");
			script.Append(" if(document.getElementById(chkName).getElementsByTagName(\'input\')[i].checked==0)");
			script.Append("\n\t	{ ");
			script.Append("\n\t\t sel=1; ");
			script.Append("\n\t\t break; ");
			script.Append("\n\t	} ");			
			script.Append("\n }");

			// coche ou décoche tous les éléménts fils 
			script.Append("\n for(i=0;i<nbElement; i++){");
			script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[i].checked = sel;");									
			script.Append("	} "); 				
			script.Append(" } ");
			script.Append("\n</script>");
			return script.ToString();
		}

		/// <summary>
		/// Script permettant de gérer le cochage des cases pour l integration automatique
		/// de groupe
		/// </summary>
		/// <remarks>
		/// Enregistrer sous AllGroupIntegration et fait appel à la méthode GroupIntegration(chkName)
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string AllGroupIntegration(){ 
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n function AllGroupIntegration(ids){ ");
			script.Append("\n var divs = ids.split(','); ");
				script.Append("\n for(i=0; i<divs.length;i++) {"); 
					script.Append("if (document.all.item(divs[i])!=null) {");
						script.Append(" SelectAllChilds(divs[i]);");
					script.Append(" } ");
				script.Append(" } ");
			script.Append(" } ");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region selection toutes les variétés d'un groupe
			
		/// <summary>
		/// Script permettant de gérer le cochage des cases pour l integration automatique
		/// de groupe
		/// </summary>
		/// <remarks>
		/// Enregistrer sous GroupIntegration
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string GroupIntegration(){ 
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n function GroupIntegration(chkName){ ");
			script.Append("\n var nbElement = document.getElementById(chkName).getElementsByTagName(\'input\').length; ");
			script.Append("\n var sel = 0; ");
			script.Append("\n if(document.activeElement.checked==true){");
			script.Append("\n sel = 1;\n}");
            
			// coche ou décoche tous les éléménts fils 
			script.Append("\n for(i=0;i<nbElement; i++){");
			script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[i].checked = sel;");									
			script.Append("	} "); 				
			script.Append(" } ");
			script.Append("\n</script>");
			return script.ToString();
		}
		
		
		#endregion

		#region Ouverture de la popup de sauvegarde d'un univers
		/// <summary>
		/// Script permettant d'ouvrir la fenetre modale d'enregistrement d'un univers
		/// Elle est ouverte par la fenetre appelante mais se ferme independemment de cette derniere
		/// Le script est destinée à être placé dans un lien ou un onClick, onLoad...
		/// Pas d'enregistrement
		/// </summary>
		/// <returns>Code JavaScript</returns>
		public static string SaveUniverseOpen(string idSession, TNS.AdExpress.Constantes.Classification.Branch.type branch,Int64 idUniverseClientDescription){
			return "window.showModalDialog('/Private/Universe/SaveUniverse.aspx?idSession="+idSession+"&brancheType="+branch
				+"&idUniverseClientDescription="+idUniverseClientDescription+"&atd="+DateTime.Now.ToString("yyyyMMddhhmmss")+" ',null, 'dialogHeight:300px;dialogWidth:450px;help:no;resizable:no;scroll:no;status:no;');";
		}
		#endregion

		#region Gestion de la sélection d'un univers à charger
		/// <summary>
		/// Génération du code html d'un script de
		/// Gestion de la sélection d'un radioButton dans le chargement des univers
		/// </summary>
		/// <remarks>
		/// A enregistrer sous insertIdMySession4
		/// </remarks>
		/// <returns>Code html DU SCRIPT</returns>
		public static string InsertIdMySession4(){
			StringBuilder t = new StringBuilder(300);
			t.Append("\n <!—gestion de l’univers sélectionné -->\n");
			t.Append("\n <SCRIPT language=JavaScript>\n");
			t.Append("\t function insertIdMySession4(idSession,idRepertory){\n");
			t.Append("\t\t document.Form2.idMySession.name='UNIVERSE_'+idSession+'_'+idRepertory;\n");
			t.Append("\t }\n");
			t.Append("</script>\n");
			return t.ToString();
		}
		#endregion

		#region cochage/décochage des checkbox dans choix media analyse sectorielle

		
		/// <summary>
		/// Script permettant de gérer le cochage des cases pour le choix des médias
		/// Le choix étant exclusif (un media ou la case plurimedia)
		/// </summary>
		/// <remarks>
		/// Enregistrer sous CheckAllChilds
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string CheckAllChilds(){ 
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n function CheckAllChilds(chkName,ids,highParent){ ");
			script.Append("\n var nbElement = 0; ");
			script.Append(" if(document.getElementById(chkName)!=null){");
			script.Append("\n nbElement = document.getElementById(chkName).getElementsByTagName(\'input\').length; ");
			script.Append("\n } ");	
			script.Append("\n var sel=0; ");			
			script.Append("\n var divs = ids.split(\'-\');");
			script.Append("\n var nbChilds = 0; ");
			//Coche les checkbox fils si le checkbox parent est coché
			script.Append(" if(document.activeElement.checked==true){");			
			script.Append("\n\t\t sel=1; }");	
			script.Append("\n for(i=0;i<nbElement; i++){");
			script.Append("\n\t document.getElementById(chkName).getElementsByTagName(\'input\')[i].checked = sel;  ");	
			script.Append(" if(i>0)");
			script.Append("\n\t document.getElementById(chkName).getElementsByTagName(\'input\')[i].disabled = sel; } ");	
			//Decoche tous les checkbox des parents concurrents et leurs fils
			script.Append("\n\t	if(document.activeElement.checked==true){ ");
				script.Append("\n if(divs.length >0){ ");
					script.Append("\n for(j=0;j<divs.length; j++){");
						script.Append("\n if(highParent != divs[j]){");
							script.Append(" if(document.getElementById(divs[j])!=null){");	
								script.Append("nbChilds=document.getElementById(divs[j]).getElementsByTagName(\'input\').length; ");																			
									
									script.Append("\n for(k=0;k<nbChilds; k++){");
									script.Append(" if(document.getElementById(divs[j]).getElementsByTagName('input')[k]!=null){");
									script.Append("\n document.getElementById(divs[j]).getElementsByTagName('input')[k].checked = 0;");	
									script.Append("\n document.getElementById(divs[j]).getElementsByTagName('input')[k].disabled = 0;");																																		
									script.Append("\n } ");
								script.Append("\n }");
							script.Append("\n }");
							script.Append("\n nbChilds =0;  ");									
						script.Append(" } ");
					script.Append(" } ");
				script.Append(" } ");
			script.Append(" } ");
			script.Append(" } ");
			script.Append("\n</script>");
			return script.ToString();
		}
		
		#endregion

		#region Gestion de l'ImageDropDownList
		/// <summary>
		/// Gestion de l'ImageDropDownList
		/// </summary>
		/// <remarks>
		/// CommonImageDropDownListScripts
		/// </remarks>	
		/// <param name="pictShow">Image à montrer</param>
		/// <returns>Code JavaScript</returns>
		public static string ImageDropDownListScripts(bool pictShow){

			StringBuilder script = new StringBuilder(500);

			//Fonction permettant de fermer le menu si ce on clique ailleurs
			script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\t var openMenu = null;");
			script.Append("\n\t function openMenuTest(){");
			script.Append("\n\t\t if ( openMenu!=null){");
			script.Append("\n\t\t\t if(window.event.srcElement.className != document.getElementById(openMenu).className){");
			script.Append("\n\t\t\t\t HideScrollerImageDDL(openMenu);");
			script.Append("\n\t\t\t }");
			script.Append("\n\t\t }");
			script.Append("\n\t }");

			//Fonction affichant le scroller
			script.Append("\n function ShowScrollerImageDDL(idControl) {");
			script.Append("\n\t var scroller = document.all['scroller_' + idControl ];");
			script.Append("\n\t openMenu = idControl ;");
			script.Append("\n\t if (scroller.style.display==\"none\") {");
			script.Append("\n\t\t scroller.style.display=\"\";");
			script.Append("\n\t } else {");
			script.Append("\n\t\t HideScrollerImageDDL(idControl);");
			script.Append("\n\t }");
			script.Append("\n }");

			//Fonction pour fermer le scroller
			script.Append("\n function HideScrollerImageDDL(idControl) {");
			script.Append("\n\t openMenu = null;");
			script.Append("\n\t var scroller = document.all['scroller_' + idControl];");
			script.Append("\n\t scroller.style.display=\"none\";");
			script.Append("\n }");

			//Fonction changeant la class d'un item, until pour la gestion des classes enrollover
			script.Append("\n function ChangeItemClassDDL(obj, cssClass) {");
			script.Append("\n\t obj.className = cssClass;");
			script.Append("\n }");

			//Selection d'un élément;
			script.Append("\n function ItemClickDDL(idControl, obj, ItemId, ItemText, ItemImg) {");
			script.Append("\n\t	onMenu = null;");
			script.Append("\n\t	HideScrollerImageDDL(idControl);");
			if (pictShow){
				script.Append("\n\n\t var ComboImg = document.all['ComboImg_' + idControl ];");
				script.Append("\n\t ComboImg.src = ItemImg;");
			}
			script.Append("\n\n\t	var ComboText = document.all['ComBoText_' + idControl];");
			//script.Append("\n\n alert(ComboText);");
			script.Append("\n\t var ComboIndex = document.all[idControl];");
			script.Append("\n\t ComboText.innerHTML = '&nbsp;' + ItemText;");
			script.Append("\n\t ComboIndex.value = ItemId;");
			script.Append("\n }");

            //Selection d'un élément dans LanguageSelectionWebControl
            script.Append("\n function ItemClickLS(idControl, obj, ItemId, ItemText, ItemImg, path) {");
            script.Append("\n\t	onMenu = null;");
            script.Append("\n\t	HideScrollerImageDDL(idControl);");
            if (pictShow) {
                script.Append("\n\n\t var ComboImg = document.all['ComboImg_' + idControl ];");
                script.Append("\n\t ComboImg.src = ItemImg;");
            }
            script.Append("\n\n\t	var ComboText = document.all['ComBoText_' + idControl];");
            //script.Append("\n\n alert(ComboText);");
            script.Append("\n\t var ComboIndex = document.all[idControl];");
            script.Append("\n\t ComboText.innerHTML = '&nbsp;' + ItemText;");
            script.Append("\n\t ComboIndex.value = ItemId;");
            script.Append("\n\t window.location.href = path;");
            script.Append("\n }");

			script.Append("\n </script>");
			
			return script.ToString();
		}

		#endregion

        #region Gestion de LanguageSelection
        /// <summary>
        /// Gestion de LanguageSelection
        /// </summary>
        /// <param name="pictShow">Image à montrer</param>
        /// <returns>Code JavaScript</returns>
        public static string LanguageSelectionScripts(bool pictShow) {

            StringBuilder script = new StringBuilder(500);

            //Fonction permettant de fermer le menu si ce on clique ailleurs
            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("\n\t var openMenuLS = null;");
            script.Append("\n\t function openMenuTestLS(){");
            script.Append("\n\t\t if ( openMenuLS!=null){");
            script.Append("\n\t\t\t if(window.event.srcElement.className != document.getElementById(openMenuLS).className){");
            script.Append("\n\t\t\t\t HideScrollerImageLS(openMenuLS);");
            script.Append("\n\t\t\t }");
            script.Append("\n\t\t }");
            script.Append("\n\t }");

            //Fonction affichant le scroller
            script.Append("\n function ShowScrollerImageLS(idControl) {");
            script.Append("\n\t var scroller = document.all['scroller_' + idControl ];");
            script.Append("\n\t openMenuLS = idControl ;");
            script.Append("\n\t if (scroller.style.display==\"none\") {");
            script.Append("\n\t\t scroller.style.display=\"\";");
            script.Append("\n\t } else {");
            script.Append("\n\t\t HideScrollerImageLS(idControl);");
            script.Append("\n\t }");
            script.Append("\n }");

            //Fonction pour fermer le scroller
            script.Append("\n function HideScrollerImageLS(idControl) {");
            script.Append("\n\t openMenuLS = null;");
            script.Append("\n\t var scroller = document.all['scroller_' + idControl];");
            script.Append("\n\t scroller.style.display=\"none\";");
            script.Append("\n }");

            //Fonction changeant la class d'un item, until pour la gestion des classes enrollover
            script.Append("\n function ChangeItemClassLS(obj, cssClass) {");
            script.Append("\n\t obj.className = cssClass;");
            script.Append("\n }");

            //Selection d'un élément dans LanguageSelectionWebControl
            script.Append("\n function ItemClickLS(idControl, obj, ItemId, ItemText, ItemImg, path) {");
            script.Append("\n\t	openMenuLS = null;");
            script.Append("\n\t	HideScrollerImageLS(idControl);");
            script.Append("\n\t window.location.href = path;");
            script.Append("\n }");

            script.Append("\n </script>");

            return script.ToString();
        }

        #endregion

        #region Cookies
        /// <summary>
        /// Cookies javascript functions
        /// </summary>
        /// <returns>Javascript code</returns>
        public static string CookiesJScript() {
            StringBuilder script = new StringBuilder(500);

            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            
            script.Append("\n function GetCookie(name) {");
	        script.Append("\n\t if (document.cookie){");
		    script.Append("\n\t var startIndex = document.cookie.indexOf(name);");
		    script.Append("\n\t if (startIndex != -1) {");
			script.Append("\n\t var endIndex = document.cookie.indexOf(';', startIndex);");
			script.Append("\n\t if (endIndex == -1) endIndex = document.cookie.length;");
			script.Append("\n\t return unescape(document.cookie.substring(startIndex+name.length+1, endIndex));");
		    script.Append("\n }");
	        script.Append("\n }");
	        script.Append("\n\t return null;");
            script.Append("\n }");

            script.Append("\n function setPermanentCookie(name,value) {");
            script.Append("\n\t var expire = new Date ();");
            script.Append("\n\t var path = '/';");
	        script.Append("\n\t expire.setTime (expire.getTime() + 24 * 60 * 60 * 1000 * 365);");
            script.Append("\n\t document.cookie = name + '=' + escape(value) + '; expires=' + expire.toGMTString() + '; path='+path ;");
            script.Append("\n }");

            script.Append("\n function setCookie(name,value,days) {");
	        script.Append("\n\t var expire = new Date ();");
            script.Append("\n\t var path = '/';");
	        script.Append("\n\t expire.setTime (expire.getTime() + (24 * 60 * 60 * 1000) * days);");
            script.Append("\n\t document.cookie = name + '=' + escape(value) + '; expires=' + expire.toGMTString() + '; path='+path ;");
            script.Append("\n }");

            script.Append("\r\n function DeleteCookie(name) {");
            script.Append("\r\n\t  var expire = new Date ();");
            script.Append("\r\n\t  expire.setTime (expire.getTime() - (24 * 60 * 60 * 1000));");
            script.Append("\r\n\t  document.cookie = name + \"=; expires=\" + expire.toGMTString();");
            script.Append("\r\n  }");

            script.Append("\n</script>");

            return script.ToString();
        }
        /// <summary>
        /// Cookies
        /// </summary>
        /// <param name="siteLanguage">Site Language</param>
        /// <param name="link">Link</param>
        /// <returns>Code JavaScript</returns>
        public static string CookieScripts(int siteLanguage, string link) {

            StringBuilder script = new StringBuilder(500);

            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n var language = "+siteLanguage+";");
            script.Append("\n var cook = GetCookie('" + WebApplicationParameters.WebSiteName + WebCst.Cookies.LANGUAGE+ "');");
			script.Append("\n if (cook != null){");
			script.Append("\n if (language != cook){");
			script.Append("\n document.location=\""+link+"\"+cook;");
			script.Append("\n }");
			script.Append("\n }");
            script.Append("\n</script>");

            return script.ToString();
        
        }
        /// <summary>
        /// Set cookie script
        /// </summary>
        /// <param name="siteLanguage">Site language</param>
        /// <returns>Code JavaScript</returns>
        public static string SetCookieScript(int siteLanguage) {

            StringBuilder script = new StringBuilder(500);
            script.Append("\n <script language=\"JavaScript\" type=\"text/JavaScript\">");
            script.Append("\n setPermanentCookie('" + WebApplicationParameters.WebSiteName + WebCst.Cookies.LANGUAGE + "','" + siteLanguage + "');");
            script.Append("\n</script>");

            return script.ToString();
        }
        #endregion

        #region selection toutes les variétés d'un groupe

        /// <summary>
		/// Script permettant de sélectionner exclusivement un ensemble de checkbox		
		/// </summary>
		/// <remarks>
		/// Enregistrer sous SelectExclusiveAllChilds
		/// </remarks>
		/// <returns>CodeJavaScript</returns>
		public static string SelectExclusiveAllChilds() { 
			StringBuilder script = new StringBuilder(500);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n function SelectExclusiveAllChilds(chkName,ids,highParent,prefixeVehicle,prefixeCategory){ ");
			script.Append("\n var nbElement = document.getElementById(chkName).getElementsByTagName(\'input\').length; ");
			script.Append("\n var sel=0; ");
			script.Append("\n var Id=chkName; ");
			script.Append("\n var HighParent=highParent; ");
			script.Append("\n for(i=0;i<nbElement; i++){");
			// Verifie si un élément est coché ou décoché dans le groupe des checkbox traités
			script.Append("\n if(i>0){");
				script.Append(" if(document.getElementById(chkName).getElementsByTagName(\'input\')[i].checked==0)");
				script.Append("\n\t	{ ");
				script.Append("\n\t\t sel=1; ");			
				script.Append("\n\t\t break; ");
				script.Append("\n\t	} ");		
				script.Append("\n }");
			script.Append("\n }");
			//On décoche et désactive le parent	
			script.Append(" if(document.getElementById(chkName).getElementsByTagName(\'input\')[0]!=null && Id.indexOf(prefixeVehicle)>-1 ){ "); //&& document.getElementById(HighParent).getElementsByTagName(\'input\')[0].checked==0
			script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[0].checked=0; ");
			script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[0].disabled=0; ");
			script.Append("	} \n");
			script.Append(" else { \n"); 
				script.Append(" if(document.getElementById(HighParent).getElementsByTagName(\'input\')[0].checked==0){ "); 
					script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[0].checked=0; ");
					script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[0].disabled=0; ");
				script.Append("	} ");
			script.Append("	} ");
			// coche ou décoche tous les éléménts fils 
			script.Append("\n for(i=0;i<nbElement; i++){");
				script.Append("\n if(i>0){");
				script.Append("if(document.getElementById(HighParent).getElementsByTagName(\'input\')[0].checked==0){");															
					script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[i].checked = sel;");
				script.Append("	}\n ");
				script.Append("if(Id.indexOf(prefixeVehicle)>-1 ){");
					script.Append("if(document.getElementById(chkName).getElementsByTagName(\'input\')[i].value.indexOf(prefixeCategory)>-1){");															
						script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[i].disabled = 0;");							
					script.Append("	} ");
					script.Append("else{");
						script.Append("if(document.getElementById(HighParent).getElementsByTagName(\'input\')[0].checked==0){");																					
							script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[i].disabled = sel;");															
						script.Append("	}\n ");
					script.Append("	}\n ");
				script.Append("	}\n ");
				script.Append(" else { ");
					script.Append("if(document.getElementById(HighParent).getElementsByTagName(\'input\')[0].checked==0){");															
						script.Append(" document.getElementById(chkName).getElementsByTagName(\'input\')[i].disabled = 0;}");															
					script.Append("	} ");					
				script.Append("	} ");
			script.Append("	} ");
 
			#region désactivation des supports,catégories,médias concurrents (selection exclusive)
			//SELECTION EXCLUSIVE
			script.Append("\n\t\t sel=0; ");			
			script.Append("\n var divs = ids.split(\'-\');");
			script.Append("\n var nbChilds = 0; ");
			//Decoche tous les checkbox des parents concurrents et leurs fils			
			script.Append("\n if(divs.length >0){ ");
			script.Append("\n for(j=0;j<divs.length; j++){");
			script.Append("\n if(highParent != divs[j]){");
			script.Append(" if(document.getElementById(divs[j])!=null){");	
			script.Append("nbChilds=document.getElementById(divs[j]).getElementsByTagName(\'input\').length; ");																			
									
			script.Append("\n for(k=0;k<nbChilds; k++){");
			script.Append(" if(document.getElementById(divs[j]).getElementsByTagName('input')[k]!=null){");
			script.Append("\n document.getElementById(divs[j]).getElementsByTagName('input')[k].checked = 0;");
			script.Append("\n document.getElementById(divs[j]).getElementsByTagName('input')[k].disabled = 0;");																							
			script.Append("\n } ");
			script.Append("\n }");
			script.Append("\n }");
			script.Append("\n nbChilds =0;  ");									
			script.Append(" } ");
			script.Append(" } ");
			script.Append(" } ");
			#endregion

			script.Append(" } ");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Ouverture d'une popup pour le chemin de fer
		/// <summary>	
		/// Ouverture d'une popup pour le chemin de fer	
		/// </summary>
		/// <remarks>
		/// Enregistré sous portofolioCreation
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string PortofolioCreation(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction portofolioCreation(idSession, idMedia, date,parution,nameMedia,nbrePages){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/PortofolioCreationMediaPopUp.aspx?idSession=\"+idSession"
				+"+\"&idMedia=\"+idMedia"
				+"+\"&date=\"+date"
                + "+\"&parution=\"+parution"
				+"+\"&nameMedia=\"+nameMedia"
				+"+\"&nbrePages=\"+nbrePages"
				+",'', \"top=10, left=10,toolbar=0, directories=0, status=0, menubar=0, width=980, height=650, scrollbars=1, location=0, resizable=0\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}

		/// <summary>	
		/// Ouverture d'une popup pour le chemin de fer	avec positionnement de la publicité dans son contexte
		/// </summary>
		/// <remarks>
		/// Enregistré sous portofolioCreation
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string PortofolioCreationWithAnchor(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction portofolioCreationWithAnchor(idSession, idMedia, date,parution, nameMedia, nbrePages, pageAnchor){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/PortofolioCreationMediaPopUp.aspx?idSession=\"+idSession"
				+"+\"&idMedia=\"+idMedia"
				+"+\"&date=\"+date"
				+"+\"&parution=\"+parution"
				+"+\"&nameMedia=\"+nameMedia"
				+"+\"&nbrePages=\"+nbrePages"
				+"+\"&pageAnchor=\"+pageAnchor"
				+",'', \"top=10, left=10,toolbar=0, directories=0, status=0, menubar=0, width=980, height=650, scrollbars=1, location=0, resizable=0\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Ouverture d'une popup pour une image d'un support
		/// <summary>		 
		/// Ouverture d'une popup pour une image d'un support
		/// </summary>
		/// <remarks>
		/// Enregistré sous portofolioOneCreation
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string PortofolioOneCreation(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction portofolioOneCreation(idMedia, date,fileName1,fileName2){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/PortofolioCreationOneMediaPopUp.aspx?idMedia=\"+idMedia"
				+"+\"&date=\"+date"
				+"+\"&fileName1=\"+fileName1"
				+"+\"&fileName2=\"+fileName2"
				+",'', \"top=10, left=10,toolbar=0, directories=0, status=0, menubar=0, width=985, height=700, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		
		#endregion

		#region Ouverture d'une popup pour le detail portefeuille d'un support		
		/// <summary>		 
		/// Ouverture d'une popup pour le detail portefeuille d'un support
		/// </summary>
		/// <remarks>
		/// Enregistré sous portofolioDetailMedia
		/// </remarks>
		/// <returns>Code JavaScript</returns>
		public static string PortofolioDetailMedia(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction portofolioDetailMedia(idSession,idMedia,dayOfWeek,ecran){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/PortofolioDetailMediaPopUp.aspx?idSession=\"+idSession"
				+"+\"&idMedia=\"+idMedia"
				+"+\"&dayOfWeek=\"+dayOfWeek"
				+"+\"&ecran=\"+ecran"
				+",'', \"top=10, left=10,toolbar=0, directories=0, status=0, menubar=0, width=985, height=700, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		
		#endregion

		#region Ouverture d'une PopUp donnant le détail des insertions dans Excel pour le portefeuille
		/// <summary>
		/// Construit un JavaScript ouvrant le détail des insertions dans une PopUp Excel
		/// </summary>
		/// <remarks>
		/// Enregistré sous OpenPortofolioDetailMediaPopUpExcel		
		/// </remarks>
		/// <returns>Code Javascript</returns>
		public static string OpenPortofolioDetailMediaPopUpExcel(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction OpenPortofolioDetailMediaPopUpExcel(idSession, ecran, dayOfWeek){");
			script.Append("\n\t\twindow.open("+
				"\"/Private/Results/Excel/PortofolioDetailMediaPopUp.aspx?idSession=\"+idSession"
				+"+\"&ecran=\"+ecran"
				+"+\"&dayOfWeek=\"+dayOfWeek"
				+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion
		
		#region Script de redirection vers la page d'erreur du site
		/// <summary>
		/// Script de redirection vers la page d'erreur du site
		/// </summary>
		/// <returns>Code du script</returns>
		public static string RedirectError(string siteLanguage){
			System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
			t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append(" window.document.location.href='/Public/Message.aspx?msgCode=5&siteLanguage="+siteLanguage+"';");
			t.Append("\n</script>");
			return t.ToString();

		}

		#endregion

        #region Script de redirection vers la page d'erreur du plan media version quali
        /// <summary>
        /// Script de redirection vers la page d'erreur du plan media version quali
        /// </summary>
        /// <returns>Code du script</returns>
        public static string RedirectCreativeError(string siteLanguage) {
            System.Text.StringBuilder t = new System.Text.StringBuilder(1000);
            t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
            t.Append(" window.document.location.href='/Public/CreativeError.aspx?msgCode=1&siteLanguage=" + siteLanguage + "';");
            t.Append("\n</script>");
            return t.ToString();

        }

        #endregion

		#region script de sélection d'une date hebdomadiare ou mensuelle
		/// <summary>
		/// Active la période (cumulée,mensuelle,hebdomadaire) sélectionnée par l'utilisateur
		/// et désactive lez concurrents.
		/// </summary>
		/// <param name="monthID">ID du Contrôle des périodes mensuelles </param>
		/// <param name="weekID">ID du Contrôle des périodes hebdomadaires</param>
		/// <param name="cumulID">ID du Contrôle des périodes cumulée</param>
		/// <returns>script choix d'une période</returns>
		public static string SelectPeriod(string monthID,string weekID,string cumulID){
			System.Text.StringBuilder script=new System.Text.StringBuilder(1000);

			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\t function SelectPeriod(controlID){");
			script.Append("\n\t\t  switch(controlID)");
				script.Append("\n\t\t\t {");
					//Période mensuelle
					script.Append("\n\t\t\t\t case '"+monthID+"' :");
					script.Append("\n\t\t\t\t document.forms[0]."+weekID+".options[0].selected=true;");										
					script.Append("\n\t\t\t\t document.forms[0]."+cumulID+".checked=false;");
//			script.Append("\n\t\t\t\t document.forms[0]."+cumulID+".removeAttribute('disabled');");
//			script.Append("\n\t\t\t\t document.forms[0]."+cumulID+".parentElement.removeAttribute('disabled');");
					script.Append("\n\t\t\t\t break;");
					//Période hebdomadaire
					script.Append("\n\t\t\t\t case '"+weekID+"' :");
					script.Append("\n\t\t\t\t document.forms[0]."+monthID+".options[0].selected=true;");														
					script.Append("\n\t\t\t\t document.forms[0]."+cumulID+".checked=false;");
//			script.Append("\n\t\t\t\t document.forms[0]."+cumulID+".removeAttribute('disabled');");
//			script.Append("\n\t\t\t\t document.forms[0]."+cumulID+".parentElement.removeAttribute('disabled');");
					script.Append("\n\t\t\t\t break;");
					//Période cumulée
					script.Append("\n\t\t\t\t case '"+cumulID+"' :");
					script.Append("\n\t\t\t\t document.forms[0]."+monthID+".options[0].selected=true;");
					script.Append("\n\t\t\t\t document.forms[0]."+weekID+".options[0].selected=true;");
//					script.Append("\n\t\t\t\t document.forms[0]."+cumulID+".disabled=true;");
//					script.Append("\n\t\t\t\t if(document.getElementById("+cumulID+").getElementsByTagName(\'input\')[0].checked==1){document.getElementById("+cumulID+").getElementsByTagName(\'input\')[0].checked=0;}");
//					script.Append("\n\t\t\t\t else {document.getElementById("+cumulID+").getElementsByTagName(\'input\')[0].checked==1;}");
					script.Append("\n\t\t\t\t break;");
//					script.Append("\n\t\t\t\t case 'Form2' :");
//					script.Append("\n\t\t\t\t default : ");	
//					//script.Append("\n\t\t\t\t if(document.forms[0]."+monthID+".options[0].selected && !document.forms[0]."+cumulID+".checked && document.forms[0]."+weekID+".options[0].selected){ ");
//					script.Append("\n\t\t\t\t alert('Il est nécessaire de sélectionner une période pour valider!'); ");
//					script.Append("\n\t\t\t\t return false; ");		
//					//script.Append("\n\t\t\t\t } ");
//					script.Append("\n\t\t\t\t break;");
				script.Append("\n\t\t\t }");					
			script.Append("\n\t\t }");
			script.Append("\n</script>");

			return script.ToString();
		}
		#endregion

		#region Méthodes interne
		/// <summary>
		/// Génère un nombre à partir de la date
		/// </summary>
		/// <returns>Nombre généré</returns>
		public static string GenerateNumber(){
			DateTime dt=DateTime.Now;
			return(dt.Year.ToString()+dt.Month.ToString()+dt.Day.ToString()+dt.Hour.ToString()+dt.Minute.ToString()+dt.Second.ToString()+dt.Millisecond.ToString()+new Random().Next(1000));
		}
		#endregion

		#region APPM Insertion PopUp
		/// <summary>
		/// Generate the code so as to open a popup
		/// </summary>
		/// <param name="excel">Specify if the webpage is on HTML or Excel format</param>
		/// <returns>JavaScript Code</returns>
		public static string PopUpInsertion(bool excel){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction PopUpInsertion(idSession, idMedia){");
			script.Append("\n\t\twindow.open(");
			if (!excel){
				script.Append("\"/Private/Results/APPMInsertion.aspx?idSession=\"+idSession");
				script.Append("+\"&idMed=\"+idMedia"
					+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=0, status=1, menubar=0, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
					+");");
			}
			else{
				script.Append("\"/Private/Results/Excel/APPMInsertion.aspx?idSession=\"+idSession");
				script.Append("+\"&idMed=\"+idMedia"
					+",'', \"top=\"+(screen.height-600)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=1, directories=1, status=1, menubar=1, width=1024, height=600, scrollbars=1, location=0, resizable=1\""
					+");");
			}
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region APPM Justificatif

		#region PopUp
		/// <summary>
		/// Ouvre une pop up pour les justificatifs
		/// </summary>
		/// <returns>JavaScript Code</returns>
		public static string OpenPopUpJustificatif(){
			StringBuilder script = new StringBuilder(1000);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\n\tfunction openPopUpJustificatif(idSession, idMedia, idProduct, page, date, dateParution){");
			script.Append("\n\t\twindow.open(");
			script.Append("\"/Private/Results/APPMProof.aspx?idSession=\"+idSession");
			script.Append("+\"&idMedia=\"+idMedia+\"&idProduct=\"+idProduct+\"&date=\"+date+\"&dateParution=\"+dateParution+\"&page=\"+page"
				+",'AdExpress', \"top=\"+(screen.height-700)/2+\", left=\"+(screen.width-1024)/2+\",toolbar=0, directories=0, status=1, menubar=0, width=1024, height=700, scrollbars=1, location=0, resizable=1\""
				+");");
			script.Append("\n\t}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Affichage d'une image (zoom)
		/// <summary>
		/// Affiche une image, par rapport à un évèment onMouseOver venant d'une imagette
		/// </summary>
		/// <returns>JavaScript Code</returns>
		public static string ViewAdZoom(){
			StringBuilder script = new StringBuilder(250);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\nfunction viewAdZoom(name){");
			script.Append("\n\timgOn=eval(name+\".src\");");
			script.Append("\n\tdocument['displayImg'].src=imgOn;");
			script.Append("\n}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#endregion

		#region Préchargement d'images
		/// <summary>
		/// Précharge des images
		/// </summary>
		/// <param name="fileList">Liste des nom de fichier image</param>
		/// <param name="path">Chemin</param>
		/// <returns>Javascript code</returns>
		/// <remarks>Le nom des images commence par ImgN, où N étant le numéro</remarks>
		public static string PreloadImages(string[] fileList, string path){
			StringBuilder script = new StringBuilder();
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			for(int j=0; j < fileList.Length; j++){
				script.Append("\nImg"+ j +" = new Image();");
				script.Append("\nImg"+ j +".src = '"+ path+fileList.GetValue(j) +"';");
			}
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Ancre pour le positionnement de l'image dans son contexte (chemin de fer)
		/// <summary>
		/// Positionnement de l'image dans son contexte
		/// </summary>
		/// <returns>Javascript code</returns>
		public static string GoToAnchorImage(){
			StringBuilder script = new StringBuilder(300);
			script.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			script.Append("\nvar stoploop=0;");
			script.Append("\nfunction goToAnchorImage(name){");
			script.Append("\n\tvar j=1;");
			
			script.Append("\n\tfor(i = 1; i < document.images.length; i++){"); // Parcours le nombre d'image du document
			script.Append("\n\t\tif(document.images[i].complete){"); // Vérifie si le chargement de chaque image est effectué
			script.Append("\n\t\t\tj++;");
			script.Append("\n\t\t}");
			script.Append("\n\t}");
			
			script.Append("\n\tif(j==document.images.length){"); // Si compteur J des images chargées est égale au nombre d'images du document alors Anhcor
			script.Append("\n\t\tdocument.location ='#'+name;");
			script.Append("\n\t}");
			script.Append("\n\telse{"); // Sinon on boucle une nouvelle fois
			script.Append("\n\t\tif(stoploop<2){");
			script.Append("\n\t\t\tgoToAnchorImage(name);");
			script.Append("\n\t\t\tstoploop++;");
			script.Append("\n\t\t}");
			script.Append("\n\t}");

			script.Append("\n}");
			script.Append("\n</script>");
			return script.ToString();
		}
		#endregion

		#region Script d'ouverture de fichier media
		/// <summary>
		/// Script d'ouveture d'un fichier Media Payer
		/// </summary>
		/// <returns>Script</returns>
		public static string GetObjectWindowsMediaPlayerRender(int siteLanguage){

			StringBuilder res = new StringBuilder(2000);
			res.Append("<script language=\"JavaScript\" type=\"text/javascript\">"); 			
			res.Append(" function GetObjectWindowsMediaPlayerRender(filepath){");
			res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" bgColor=\"#ffffff\"><TBODY><TR><TD>');");
			//Lecture par Media player
			res.Append(" document.write('<object id=\"video1\"  classid=\"CLSID:22D6F312-B0F6-11D0-94AB-0080C74C7E95\" height=\"288\" width=\"352\" align=\"middle\"  codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,5,715\"  standby=\""+GestionWeb.GetWebWord(1911,siteLanguage)+"\" type=\"application/x-oleobject\">');"); 			
			res.Append(" document.write('<param name=\"FileName\" value='+filepath+' >');");
			res.Append(" document.write('<param name=\"AutoStart\" value=\"true\">');");
			res.Append(" document.write('<embed type=\"application/x-mplayer2\" pluginspage=\"http://www.microsoft.com/Windows/MediaPlayer/\"  src='+filepath+' name=\"video1\" height=\"288\" width=\"352\" AutoStart=true>'); ");		
			res.Append(" document.write('</embed>');");
			res.Append(" document.write('</object>');");
			res.Append(" document.write('</TD></TR></TBODY></TABLE></TD>');");
			res.Append(" }");			
			res.Append("</script>");
			return res.ToString();
		}


		/// <summary>
		/// Script d'ouveture d'un fichier Real Payer
		/// </summary>
		/// <returns>Script</returns>
		public static string GetObjectRealPlayer(){
			StringBuilder res = new StringBuilder(2000);
			res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");		
			res.Append(" function GetObjectRealPlayer(filepath){");
			res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" bgColor=\"#ffffff\"><TBODY><TR><TD>');");
			//Lecture par Real player
			res.Append(" document.write('<object classid=\"clsid:CFCDAA03-8BE4-11cf-B84B-0020AFBBCCFA\"  ID=\"Realaudio1\" height=\"288\" width=\"352\">');");
			res.Append(" document.write('<param name=\"console\" value=\"video\">');");
			res.Append(" document.write('<param name=\"controls\" value=\"all\">');");
			res.Append(" document.write('<param name=\"autostart\" value=\"true\">');");
			res.Append(" document.write('<param name=\"src\" value='+filepath+' >');");
			res.Append(" document.write('<embed  src='+filepath+' height=\"288\" width=\"352\" type=\"audio/x-pn-realaudio-plugin\"  controls=\"all\" pluginspage=\"http://www.real.com\" console=\"video\" autostart=\"true\">');");
			res.Append(" document.write('</embed>');");
			res.Append(" document.write('</object>');");
			res.Append(" document.write('</TD></TR></TBODY></TABLE></TD>');");
			res.Append(" }");			
			res.Append("</script>");
			return res.ToString();
		}
		#endregion

        #region PostBack
        /// <summary>
        /// Vérifie s'il faut afficher le choix de sélectyion comparative ou non
        /// </summary>
        /// <param name="language">Langue du site</param>
        /// <param name="IsDynamicModule">Indique si on est dans le module dynamique</param>
        /// <param name="calendarButtonId">Le boutton valider du calendrier</param>
        /// <param name="ListButtonId">Le boutton valider des listes</param>
        /// <param name="comparativeLinkId">Le boutton qui affiche la sélection comparative</param>
        /// <param name="weekListId">La liste des semaines</param>
        /// <param name="dateSelectedItemId">Le hidden input qui contient les informations sur le calendrier</param>
        /// <param name="dayListId">La liste de jours</param>
        /// <param name="monthListId">la liste des mois</param>
        /// <param name="previousWeekCheckBox">Dernière semaine</param>
        /// <param name="previousDayCheckBox">Dernier jour</param>
        /// <param name="currentYearCheckBox">Année courante</param>
        /// <param name="previousMonthCheckBox">Mois précédent</param>
        /// <param name="previousYearCheckBox">Année précédente</param>
        /// <returns></returns>
        public static string PostBack(int language, bool IsDynamicModule, string calendarButtonId, string ListButtonId, string comparativeLinkId, string monthListId, string weekListId, string dayListId, string previousWeekCheckBox, string previousDayCheckBox, string currentYearCheckBox, string previousYearCheckBox, string previousMonthCheckBox, string dateSelectedItemId) { 
			StringBuilder res = new StringBuilder(2000);
            string caption = "";
            string label = "";

            res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");
            
            res.Append(" function PostBack(buttonClicked){");
            if (IsDynamicModule) {
                res.Append("\n var on = document.getElementById('" + comparativeLinkId + "');");
                res.Append("\n var comparativeON = document.getElementById('comparativeDiv');");
                res.Append("\n var disponibilityON = document.getElementById('disponibilityDiv');");
                res.Append("\n var espDivON1 = document.getElementById('espaceDiv1');");
                res.Append("\n var espDivON2 = document.getElementById('espaceDiv2');");
                res.Append("\n var selectedType;");
                res.Append("\n var GECKO = (navigator.product == (\"Gecko\"));");
                res.Append("\n espDivON1.style.display=\"\"; ");
                res.Append("\n espDivON2.style.display=\"none\"; ");
                res.Append("\n disponibilityON.style.display=\"\"; ");
                res.Append("\n comparativeON.style.display=\"\"; ");
                res.Append("\n on.title = \"" + GestionWeb.GetWebWord(2370, language) + "\"; ");
                caption = GestionWeb.GetWebWord(2371, language);
                label = GestionWeb.GetWebWord(2372, language);
                res.Append("\n on.alt = \"#TB_inline?height=200&width=400&inlineId=myOnPageContent&caption="+caption+"&label="+label+"\"; ");
                res.Append("\n buttonName = buttonClicked;");
                res.Append("\n if((buttonClicked != '" +ListButtonId+"') && (buttonClicked != '" +calendarButtonId+"'))");
                res.Append("\n buttonNameValue = '9999';");
                res.Append("\n else");
                res.Append("\n buttonNameValue = '';");
                res.Append("\n if(buttonClicked == '" + ListButtonId + "'){");
                res.Append("\n if(document.forms[0]." + monthListId + ".selectedIndex!=0){");
                res.Append("\n comparativeON.style.display=\"none\"; ");
                res.Append("\n espDivON1.style.display=\"none\"; ");
                res.Append("\n espDivON2.style.display=\"\"; ");
                res.Append("\n document.forms[0].selectionType.value = 'dateToDate';");
                res.Append("\n on.alt = \"#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption="+caption+"&label="+label+"\"; ");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");

                res.Append("\n }");
                res.Append("\n else if((document.forms[0]." + weekListId + ".selectedIndex!=0)||(document.forms[0]." + dayListId + ".selectedIndex!=0)){");
                res.Append("\n document.forms[0].selectionType.value = 'dateWeekComparative';");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=200&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");
                
                res.Append("\n }");
                res.Append("\n else if((document.forms[0]." + previousWeekCheckBox + ".checked==true)||(document.forms[0]." + previousDayCheckBox + ".checked==true)){");
                res.Append("\n disponibilityON.style.display=\"none\";");
                res.Append("\n document.forms[0].selectionType.value = 'dateWeekComparative';");
                res.Append("\n on.alt = \"#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption="+caption+"&label="+label+"\"; ");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");

                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n document.forms[0].selectionType.value = 'dateToDate';");
                res.Append("\n __doPostBack('" + ListButtonId + "','');");
                res.Append("\n }");
                res.Append("\n }");
                res.Append("\n else if(buttonClicked == '" + calendarButtonId + "'){");
                res.Append("\n selectedType = document.forms[0]." + dateSelectedItemId + ".value;");
                res.Append("\n selectedType = selectedType.split(\",\");");
                res.Append("\n if(selectedType[0]==1 || selectedType[0]==2){");
                res.Append("\n disponibilityON.style.display=\"none\"; ");
                res.Append("\n document.forms[0].selectionType.value = 'dateWeekComparative';");
                res.Append("\n on.alt = \"#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption="+caption+"&label="+label+"\"; ");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");

                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n document.forms[0].selectionType.value = 'dateToDate';");
                res.Append("\n __doPostBack(''+buttonClicked+'','');");
                res.Append("\n }");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n selectedType = document.forms[0]." + dateSelectedItemId + ".value;");
                res.Append("\n selectedType = selectedType.split(\",\");");
                res.Append("\n if(document.forms[0]." + monthListId + ".selectedIndex!=0){");
                res.Append("\n comparativeON.style.display=\"none\"; ");
                res.Append("\n espDivON1.style.display=\"none\"; ");
                res.Append("\n espDivON2.style.display=\"\"; ");
                res.Append("\n document.forms[0].selectionType.value = 'dateToDate';");
                res.Append("\n on.alt = \"#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption="+caption+"&label="+label+"\"; ");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");

                res.Append("\n }");
                res.Append("\n else if((document.forms[0]." + weekListId + ".selectedIndex!=0)||(document.forms[0]." + dayListId + ".selectedIndex!=0)){");
                res.Append("\n document.forms[0].selectionType.value = 'dateWeekComparative';");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=200&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");

                res.Append("\n }");
                res.Append("\n else if((document.forms[0]." + previousWeekCheckBox + ".checked==true)||(document.forms[0]." + previousDayCheckBox + ".checked==true)){");
                res.Append("\n disponibilityON.style.display=\"none\";");
                res.Append("\n document.forms[0].selectionType.value = 'dateWeekComparative';");
                res.Append("\n on.alt = \"#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption="+caption+"&label="+label+"\"; ");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");

                res.Append("\n }");
                res.Append("\n else if((selectedType[0]==1 || selectedType[0]==2) && (document.forms[0]." + previousMonthCheckBox + ".checked!=true) && (document.forms[0]." + previousYearCheckBox + ".checked!=true) && (document.forms[0]." + currentYearCheckBox + ".checked!=true)){");
                res.Append("\n disponibilityON.style.display=\"none\"; ");
                res.Append("\n document.forms[0].selectionType.value = 'dateWeekComparative';");
                res.Append("\n on.alt = \"#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption="+caption+"&label="+label+"\"; ");

                res.Append("\n if (GECKO) {");
                res.Append("\n tb_show('" + GestionWeb.GetWebWord(2370, language) + "', '#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption=" + caption + "&label=" + label + "+', ''); ");
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n on.click();");
                res.Append("\n }");
                
                res.Append("\n }");
                res.Append("\n else {");
                res.Append("\n document.forms[0].selectionType.value = 'dateToDate';");
                res.Append("\n __doPostBack(''+buttonClicked+'','9999');");
                res.Append("\n }");
                res.Append("\n }");
            }
            else
                res.Append(" __doPostBack(''+buttonClicked+'','');");
            res.Append("}");
            res.Append("</script>");
            return res.ToString();

        }
        #endregion


    }
}
