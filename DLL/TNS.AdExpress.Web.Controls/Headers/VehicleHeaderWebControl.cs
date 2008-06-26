#region Information
/*
 * auteur : D. V. Mussuma
 * créé le :
 * modifié le : 31/01/2006
 * par : 
 * */
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using System.Collections;
using System.Collections.Specialized;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Contrôle qui affiche les onglets de sélections d'un média (vehicle)
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:VehicleHeaderWebControl runat=server></{0}:VehicleHeaderWebControl>")]
	public class VehicleHeaderWebControl : System.Web.UI.WebControls.WebControl{

		#region variables
		/// <summary>
		/// Session du client 
		/// </summary>
		protected WebSession _customerWebSession = null;
		/// <summary>
		/// Indique le menu qui est actif
		/// </summary>
		protected int _activeMenu=-1;
		/// <summary>
		/// en-têtes
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.Navigation.HeaderMediaDetailMenuItem</associates>
		protected System.Collections.Specialized.ListDictionary _headers=null;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Session du client
		/// </summary>
		public WebSession CustomerWebSession{
			get{return _customerWebSession;}
			set{_customerWebSession=value;}
		}

		/// <summary>
		/// Obtient ou défnit le menu qui est actif
		/// </summary>
		[Description("Spécifie l'index du menu actif(0...)"),
		DefaultValue(0)]
		public int ActiveMenu{
			get{return _activeMenu;}
			set{_activeMenu = value;}
		}
		
		/// <summary>
		/// En-têtes
		/// </summary>
		public ListDictionary Headers{
			get{return _headers;}
			set{_headers = value;}
		}
		#endregion
		
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			string menus="";
			string href="";
			string look="";
			string classBgColor="";
			string image="";
			string languageString,idSessionString,idsString,zoomDateString,paramString,idVehicleString; 
			bool firstParameter;
            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[_customerWebSession.SiteLanguage].Name;

			WebHeaderMediaDetailMenuItem currentHeaderMenuItem=null;
            output.Write("\n<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" class=\"lightPurple\">");		
			output.Write("\n<tr>");

			if(Headers!=null && Headers.Count>0){
				IDictionaryEnumerator myEnumerator = Headers.GetEnumerator();				
				while(myEnumerator.MoveNext()){
					languageString="";
					idSessionString="";	
					idsString="";
					zoomDateString="";
					paramString="";
					idVehicleString="-1";
					firstParameter=true;
					currentHeaderMenuItem = (WebHeaderMediaDetailMenuItem)myEnumerator.Value;
					//Options dans l url suivant le menuItem
					//Differenciation et inactivation du menu actif si nécessaire
					if(ActiveMenu==int.Parse(currentHeaderMenuItem.IdVehicle)){
						href="#";
						look="roll03";
                        classBgColor = "whiteBackGround";
                        image = "/App_Themes/"+themeName+"/Images/Common/ongletCoinBlanc.gif";
					}
					else{
						look="roll01";
                        classBgColor = "violetBackGround";
                        image = "/App_Themes/" + themeName + "/Images/Common/ongletCoinViolet.gif";

						#region Gestion des Paramètres

						#region Paramètre de la langue
						if(currentHeaderMenuItem.GiveLanguage){
							languageString="siteLanguage="+CustomerWebSession.SiteLanguage.ToString();
							if(firstParameter){
								languageString="?"+languageString;
								firstParameter=false;
							}
							else{
								languageString="&"+languageString;
							}
						}
						#endregion

						#region Paramètre de l'identifiant de la session
						if(currentHeaderMenuItem.GiveSession){
							idSessionString="idSession="+CustomerWebSession.IdSession.ToString();
							if(firstParameter){
								idSessionString="?"+idSessionString;
								firstParameter=false;
							}
							else{
								idSessionString="&"+idSessionString;
							}
						}
						#endregion

						#region Paramètre des identitifiants de détail média
						if(currentHeaderMenuItem.Ids!=null && currentHeaderMenuItem.Ids.Length>0){
							idsString="ids="+currentHeaderMenuItem.Ids;
							if(firstParameter){
								idsString="?"+idsString;
								firstParameter=false;
							}
							else{
								idsString="&"+idsString;
							}
						}
						#endregion

						#region Paramètre de zoom date
						if(currentHeaderMenuItem.ZoomDate!=null && currentHeaderMenuItem.ZoomDate.Length>0){
							zoomDateString="zoomDate="+currentHeaderMenuItem.ZoomDate;
							if(firstParameter){
								zoomDateString="?"+zoomDateString;
								firstParameter=false;
							}
							else{
								zoomDateString="&"+zoomDateString;
							}
						}
						#endregion

						#region Paramètre de cache 
						if(currentHeaderMenuItem.Param!=null && currentHeaderMenuItem.Param.Length>0){
							paramString="param="+currentHeaderMenuItem.Param;
							if(firstParameter){
								paramString="?"+paramString;
								firstParameter=false;
							}
							else{
								paramString="&"+paramString;
							}
						}
						#endregion

						#region Paramètre identifiant média
						if(currentHeaderMenuItem.IdVehicle!=null && currentHeaderMenuItem.IdVehicle.Length>0){
							idVehicleString="idVehicleFromTab="+currentHeaderMenuItem.IdVehicle;
							if(firstParameter){
								idVehicleString="?"+idVehicleString;
								firstParameter=false;
							}
							else{
								idVehicleString="&"+idVehicleString;
							}
						}
						#endregion

						#endregion

						href = currentHeaderMenuItem.TargetUrl + languageString + idSessionString+idsString+zoomDateString+paramString+idVehicleString;
						if(currentHeaderMenuItem.Target.Length>0)href+="\" target=\""+currentHeaderMenuItem.Target+"\"";
					}
					//menus += "\n<td rowspan=2 nowrap bgcolor="+bgcolor+" style=\"border-left-width:1px;border-left-color:#FFFFFF;border-left-style:solid;border-top-width:1px;border-top-color:#FFFFFF;border-top-style:solid;\">&nbsp;<A style =\" font-size:13px \"  class=\""+look+"\" href=\""+href+"\">"+GestionWeb.GetWebWord((int)currentHeaderMenuItem.IdMenu, CustomerWebSession.SiteLanguage)+"</A>&nbsp;</td>";
					menus += "\n<td rowspan=2 nowrap class="+classBgColor+" >&nbsp;<A style =\" font-size:13px \"  class=\""+look+"\" href=\""+href+"\">"+GestionWeb.GetWebWord((int)currentHeaderMenuItem.IdMenu, CustomerWebSession.SiteLanguage)+"</A>&nbsp;</td>";
					menus += "\n<td valign=top width=8px><img src=\""+image+"\" border=0 height=20px></td>";
				}

				output.Write(menus);
				//output.Write("\n<td width=100% valign=top align=left><img src=\"/Images/Common/ongletFin.gif\" height=20px border=0></td>");
				output.Write("\n<td width=100%>&nbsp;</td>");
				output.Write("\n</tr>");

				// 2ème TR pour affichage correcte avec Firefox (rowspan écrit plus haut dans le 1er TD du 1er TR)
				output.Write("\n<tr>");
				currentHeaderMenuItem = null;
				IDictionaryEnumerator myEnumerator2 = Headers.GetEnumerator();
				while(myEnumerator2.MoveNext()){
					currentHeaderMenuItem = (WebHeaderMediaDetailMenuItem)myEnumerator2.Value;
					if(ActiveMenu==int.Parse(currentHeaderMenuItem.IdVehicle)){
                        classBgColor = "whiteBackGround";
					}
					else{
                        classBgColor = "violetBackGround";
					}
					output.Write("\n<td class="+classBgColor+"></td>");
				}
				//output.Write("\n<td style=\"background-repeat: repeat-y; background-image:url(/Images/Common/pixelBlanc.gif);\"></td>");
				output.Write("\n<td></td>");
				output.Write("\n</tr>");

				output.Write("\n</table>");
			}
		}
	}
}
