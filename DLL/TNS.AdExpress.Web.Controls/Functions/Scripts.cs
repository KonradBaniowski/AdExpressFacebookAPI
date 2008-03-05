#region Information
// Auteur : D. Mussuma
// Créé le : 26/03/2007
// Modifié le :
#endregion
using System;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Functions
{
	/// <summary>
	/// Fonctions de Scripts pour les contrôles web.
	/// </summary>
	public class Scripts
	{
		#region Cookies

		/// <summary>
		/// Script de definition d'un cookie
		/// </summary>
		/// <returns>Javascript Code</returns>
		public static string SetCookie(){
			StringBuilder js = new StringBuilder(1000);
			js.Append("\r\n function setCookie(name,value,days) {");
			js.Append("\r\n\t  var expire = new Date ();");	
			js.Append("\r\n\t  expire.setTime (expire.getTime() + (24 * 60 * 60 * 1000) * days);");
			js.Append("\r\n\t document.cookie = name + \"=\" + escape(value) + \"; expires=\" +expire.toGMTString();");						
			js.Append("\r\n  }");
			return js.ToString();
		}
		
		/// <summary>
		/// Script d'obtention d'un cookie
		/// </summary>
		/// <returns>Javascript Code</returns>
		public static string GetCookie(){
			StringBuilder js = new StringBuilder(1000);
			js.Append("\r\n function GetCookie(name) {");
			js.Append("\r\n\t var startIndex = document.cookie.indexOf(name);");
			js.Append("\r\n\t  if (startIndex != -1) {");
			js.Append("\r\n\t var endIndex = document.cookie.indexOf(\";\", startIndex);");
			js.Append("\r\n\t   if (endIndex == -1) endIndex = document.cookie.length;");
			js.Append("\r\n\t   return unescape(document.cookie.substring(startIndex+name.length+1, endIndex));");
			js.Append("\r\n\t  }");
			js.Append("\r\n\t  else {");
			js.Append("\r\n\t  return null;");
			js.Append("\r\n\t  }");
			js.Append("\r\n  }");
			return js.ToString();
		}

		/// <summary>
		/// Script de suppresion d'un cookie
		/// </summary>
		/// <returns></returns>
		public static string DeleteCookie(){
		StringBuilder js = new StringBuilder(1000);
		js.Append("\r\n function DeleteCookie(name) {");
		js.Append("\r\n\t  var expire = new Date ();");		
		js.Append("\r\n\t  expire.setTime (expire.getTime() - (24 * 60 * 60 * 1000));");		
		js.Append("\r\n\t  document.cookie = name + \"=; expires=\" + expire.toGMTString();");		
		js.Append("\r\n  }");
			return js.ToString();
		}
		#endregion

		#region Images
		/// <summary>
		/// Script de prechargement d'images 
		/// </summary>
		/// <param name="webControlID">Id du contrôle web appelant</param>
		/// <param name="js">Flux de caractères</param>
		/// <returns>Javascript Code</returns>
		public static void PreLoadImages(string webControlID,StringBuilder js ){
	
			js.AppendFormat("\r\n\t {0}_img_last_out = new Image(); {0}_img_last_out.src ='{1}';\r\n{0}_img_last_in = new Image(); {0}_img_last_in.src ='{2}';\r\n{0}_img_first_out = new Image(); {0}_img_first_out.src ='{3}';\r\n{0}_img_first_in = new Image(); {0}_img_first_in.src ='{4}';\r\n"
				, webControlID
				, "/Images/Common/Result/bt_last_up.gif"
				, "/Images/Common/Result/bt_last_down.gif"
				, "/Images/Common/Result/bt_first_up.gif"
				, "/Images/Common/Result/bt_first_down.gif"
				);	
			js.AppendFormat("\r\n\t {0}_img_next_out = new Image(); {0}_img_next_out.src ='{1}';\r\n{0}_img_next_in = new Image(); {0}_img_next_in.src ='{2}';\r\n{0}_img_previous_out = new Image(); {0}_img_previous_out.src ='{3}';\r\n{0}_img_previous_in = new Image(); {0}_img_previous_in.src ='{4}';\r\n"
				, webControlID
				, "/Images/Common/Result/bt_next_up.gif"
				, "/Images/Common/Result/bt_next_down.gif"
				, "/Images/Common/Result/bt_previous_up.gif"
				, "/Images/Common/Result/bt_previous_down.gif"
				);
			js.AppendFormat("\r\n\t {0}_img_detail_out = new Image(); {0}_img_detail_out.src ='{1}';\r\n{0}_img_detail_in = new Image(); {0}_img_detail_in.src ='{2}';\r\n"
				, webControlID
				, "/Images/Common/Result/bt_detail_up.gif"
				, "/Images/Common/Result/bt_detail_down.gif"
				);		
		}
		#endregion

		#region Fonction Page de résultats
		/// <summary>
		/// Script fontcion d'affichage d'une page de résultats 
		/// </summary>		
		/// <returns>Javascript Code</returns>
		public static string GetResultPage(){
			StringBuilder js = new StringBuilder(1000);
			js.Append("\r\n\nfunction GetResultPage(obj){");			
			
			js.Append("\r\n\t var htmlNavigationBarUp=''; ");
			js.Append("\r\n\t var htmlNavigationBarDown=''; ");
			js.Append("\r\n\t\t var sb = new StringBuilder();");	
			js.Append("\r\n var i;");	
		
			js.Append("\r\n\t htmlNavigationBarUp=GetNavigationBar('isUp'); ");
			
			js.Append("\r\n\t\t sb.append('<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\"><tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/Images/Common/Result/header.gif\">');");				
			js.Append("\r\n\t\t sb.append(htmlNavigationBarUp);");	
			js.Append("\r\n\t\t sb.append('</td></tr>');");	
			

			js.Append("\r\n\t\t sb.append('<tr> <td align=\"center\" style=\"padding:10px;border-left-color:#644883; border-left-width:1px; border-left-style:solid;border-right-color:#644883; border-right-width:1px; border-right-style:solid;\">');");	
			js.Append("\r\n\t\t sb.append('<table width=\"100%\" bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0>');");	
			js.Append("\r\n\t if(withHeader) ");
			js.Append("\r\n\t\t sb.append( tab[0]);");	
			js.Append("\r\n\t if(currentPageIndex==1) ");
			js.Append("\r\n\t\t i = (withHeader)?(currentPageIndex*pageSize - pageSize ) + 1 : (currentPageIndex*pageSize - pageSize ); ");
			js.Append("\r\n\t else ");
			js.Append("\r\n\t\t i = (currentPageIndex*pageSize - pageSize ); ");
			
			#region Répétion en-tête parent
			//Repete parent si page courante commance par niveau le plus bas	
			js.Append("\r\n\t if(isShowParenHeader  && tabIndex!=null && pageCount>1){");
			js.Append("\r\n\t GetParentLines(i,sb);");
			js.Append("\r\n\t }");
			#endregion
		
			js.Append("\r\n\t for( i ; i< (currentPageIndex*pageSize) && i <tab.length ; i++){ ");					
			js.Append("\r\n\t\t sb.append(tab[i]);");		
			js.Append("\r\n\t }");								
			js.Append("\r\n\t\t sb.append('</table></td></tr>');");
											
			js.Append("\r\n\t htmlNavigationBarDown=GetNavigationBar('isDown'); ");
			js.Append("\r\n\t\t sb.append('<tr><td class=\"nav\" height=\"27\" align=\"left\" background=\"/Images/Common/Result/footer.gif\">');");	
			js.Append("\r\n\t\t sb.append(htmlNavigationBarDown);");
			js.Append("\r\n\t\t sb.append('</td></tr></table>');");
			js.Append("\r\n\t obj.innerHTML=sb.toString();");	
			js.Append("\r\n\t\t sb=null;");

			js.Append("\r\n }");
			return js.ToString();
		}
		#endregion

		#region Fonction pagination
		/// <summary>
		/// Script fonction de pagination 
		/// </summary>		
		/// <returns>Javascript Code</returns>
		public static string Paginate(){
			StringBuilder js = new StringBuilder(1000);
			js.Append("\r\n\nfunction paginate(pageIndex){");
			js.Append("\r\n\t currentPageIndex = pageIndex;");

			//page de résultat
			js.Append("\r\n\t oN.innerHTML='<img src=\"/Images/Common/waitAjax.gif\">';");
			js.Append("\r\n\t setTimeout(\"GetResultPage(oN)\",0);");
						
			js.Append("\r\n}");
			return js.ToString();
		}
		#endregion

		#region Fonction Barre de navigation
		/// <summary>
		/// Script fonction  Barre de navigation
		/// </summary>	
		/// <param name="webControlID">Id du contrôle web appelant</param>
		/// <param name="webSession">Session du client</param>	
		/// <param name="withSelectionCallBack">Indique s'il faut afficher le bouton de rappel de sélection</param>
		/// <returns>Javascript Code</returns>
		public static string GetNavigationBar(string webControlID,WebSession webSession,bool withSelectionCallBack){
			StringBuilder js = new StringBuilder(2000);
			js.Append("\r\n\nfunction GetNavigationBar(up){");
			js.Append("\r\n\t var htmlNavigationBar = ''; ");
			js.Append("\r\n\t var nbIndexPage = numberIndexPage - 1; ");
			js.Append("\r\n\t if( pageCount <= nbIndexPage ) { ");
			js.Append("\r\n\t  nbIndexPage = pageCount - 1; ");
			js.Append("\r\n\t\t }");
			
			js.Append("\r\n\t if( pageCount > 0 ) { ");
			
			js.Append("\r\n\t\t if( pageCount > 1 ) { ");								
			js.Append("\r\n\t\t htmlNavigationBar += ' <font color=\"#FF0099\">'+currentPageIndex+'</font> ';");
			js.Append("\r\n\t\t leftPageIndex = rightPageIndex = currentPageIndex;");
			js.Append("\r\n\t\t while(nbIndexPage>0 && pageCount>1){ ");
			js.Append("\r\n\t\t\t if( currentPageIndex!=1 && leftPageIndex>1) {");
			js.Append("\r\n\t\t\t\t leftPageIndex--; ");
			js.Append("\r\n\t\t\t\t nbIndexPage--; ");						
			js.Append("\r\n\t\t\t\t\t htmlNavigationBar =' <a class=\"navlink\" href=\"javascript:paginate('+leftPageIndex+');\">'+leftPageIndex+'</a> '+htmlNavigationBar ;");						
			js.Append("\r\n\t\t\t }");
			js.Append("\r\n\t\t\t if( currentPageIndex < pageCount && rightPageIndex < pageCount) {");
			js.Append("\r\n\t\t\t\t rightPageIndex++; ");
			js.Append("\r\n\t\t\t\t nbIndexPage--; ");													
			js.Append("\r\n\t\t\t\t\t htmlNavigationBar = htmlNavigationBar+' <a class=\"navlink\" href=\"javascript:paginate('+rightPageIndex+');\">'+rightPageIndex+'</a> ' ;");						
			js.Append("\r\n\t\t\t }");
			js.Append("\r\n\t\t }");

			//Page précédente
			js.Append("\r\n\t\t var pres='';");
			js.Append("\r\n\t\t\t tempIndex = currentPageIndex-1;");
			js.Append("\r\n\t\t\t if(currentPageIndex > 1) {");
			js.Append("\r\n\t\t\t\t  pres = pres+ '<a ';");
			js.Append("\r\n\t\t\t\t  pres = pres +' onmouseover=\"'+up+'_previous_img.src=" + webControlID + "_img_previous_in.src;\"';");
			js.Append("\r\n\t\t\t\t  pres = pres +' onmouseout=\"'+up+'_previous_img.src=" + webControlID + "_img_previous_out.src;\"' ;");
			js.Append("\r\n\t\t\t\t  pres = pres +' href=\"javascript:paginate('+tempIndex+');\" ';");
			js.Append("\r\n\t\t\t\t  pres = pres +'><IMG border=0 alt=\"\" name=\"'+up+'_previous_img\" src=\"/Images/Common/Result/bt_previous_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
			js.Append("\r\n\t\t\t\t  pres = pres +'<IMG border=0 alt=\"\" name=\"'+up+'_previous_img\" src=\"/Images/Common/Result/bt_previous_up.gif\">';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  htmlNavigationBar=pres+htmlNavigationBar;");

			//Première Page 
			js.Append("\r\n\t\t var firstpage='';");
			js.Append("\r\n\t\t\t tempIndex = 1;");
			js.Append("\r\n\t\t\t if(currentPageIndex > 1) {");
			js.Append("\r\n\t\t\t  firstpage = firstpage+ '<a ';");
			js.Append("\r\n\t\t\t  firstpage = firstpage +' onmouseover=\"'+up+'_first_img.src=" + webControlID + "_img_first_in.src;\"';");
			js.Append("\r\n\t\t\t  firstpage = firstpage +' onmouseout=\"'+up+'_first_img.src=" + webControlID + "_img_first_out.src;\"' ;");
			js.Append("\r\n\t\t\t  firstpage = firstpage +' href=\"javascript:paginate('+tempIndex+');\" ';");
			js.Append("\r\n\t\t\t  firstpage = firstpage +'><IMG border=0 alt=\"\" name=\"'+up+'_first_img\" src=\"/Images/Common/Result/bt_first_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
			js.Append("\r\n\t\t\t  firstpage = firstpage +'<IMG border=0 alt=\"\" name=\"'+up+'_first_img\" src=\"/Images/Common/Result/bt_first_up.gif\">';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  htmlNavigationBar= '&nbsp;&nbsp;'+firstpage+htmlNavigationBar;");

			//Page suivante
			js.Append("\r\n\t\t\t  tempIndex = currentPageIndex +1;");
			js.Append("\r\n\t\t\t if(currentPageIndex < pageCount) {");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +'<a ';");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +' onmouseover=\"'+up+'_next_img.src=" + webControlID + "_img_next_in.src;\"';");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +' onmouseout=\"'+up+'_next_img.src=" + webControlID + "_img_next_out.src;\"' ;");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +' href=\"javascript:paginate('+tempIndex+');\" ';");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +'><IMG border=0 alt=\"\" name=\"'+up+'_next_img\" src=\"/Images/Common/Result/bt_next_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
			js.Append("\r\n\t\t\t\t  htmlNavigationBar = htmlNavigationBar +'<IMG border=0 alt=\"\" name=\"'+up+'_next_img\" src=\"/Images/Common/Result/bt_next_up.gif\">';");
			js.Append("\r\n\t\t\t  }");

			//Dernière Page 			
			js.Append("\r\n\t\t var lastpage='';");
			js.Append("\r\n\t\t\t tempIndex = pageCount;");
			js.Append("\r\n\t\t\t if(currentPageIndex < pageCount) {");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage+ '<a ';");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +' onmouseover=\"'+up+'_last_img.src=" + webControlID + "_img_last_in.src;\"';");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +' onmouseout=\"'+up+'_last_img.src=" + webControlID + "_img_last_out.src;\"' ;");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +' href=\"javascript:paginate('+tempIndex+');\" ';");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +'><IMG border=0 alt=\"\" name=\"'+up+'_last_img\" src=\"/Images/Common/Result/bt_last_up.gif\"></a>';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  else {");
			js.Append("\r\n\t\t\t\t  lastpage = lastpage +'<IMG border=0 alt=\"\" name=\"'+up+'_last_img\" src=\"/Images/Common/Result/bt_last_up.gif\">';");
			js.Append("\r\n\t\t\t  }");
			js.Append("\r\n\t\t\t  htmlNavigationBar=htmlNavigationBar+lastpage;");
			js.Append("\r\n\t\t } ");		

			//Option ajout répétion en-tête élément parent 
			js.Append("\r\n\t\t\t if(tabIndex!=null && pageCount>1) {");
			js.Append("\r\n\t\t htmlNavigationBar=HeaderParentOption(htmlNavigationBar,up); ");
			js.Append("\r\n\t\t }");

			//Sélection options taille page
			js.Append("\r\n\t\t htmlNavigationBar=PageSizeOptions(pageSizeOptionsList,htmlNavigationBar,up); }");
					
			//Appel calque rappel sélection (Rappel de sélection : 1989)
			if(withSelectionCallBack)js.Append("\r\n htmlNavigationBar ='&nbsp;<a href=\"javascript:afficher(divName);\"     onmouseover=\"'+up+'_detail_img.src=" + webControlID + "_img_detail_in.src;\" onmouseout=\"'+up+'_detail_img.src=" + webControlID + "_img_detail_out.src;\"     ><img src=\"/Images/Common/Result/bt_detail_up.gif\" border=0 title=\""+GestionWeb.GetWebWord(1989,webSession.SiteLanguage)+"\" align=\"absmiddle\" name=\"'+up+'_detail_img\"></a>&nbsp;'+htmlNavigationBar;");
			js.Append("\r\n\t return(htmlNavigationBar); ");
			js.Append("\r\n }");
			return js.ToString();
		}
		#endregion

		#region Fonction options nombre de lignes par page
		/// <summary>
		/// Script fonction options nombre de lignes par page
		/// </summary>	
		/// <param name="cookieID">Id cookie</param>
		/// <param name="webSession">Session du client</param>	
		/// <returns>Javascript Code</returns>
		public static string PageSizeOptions(string cookieID,WebSession webSession){
		//Fonction options nombre de lignes par page
		StringBuilder js = new StringBuilder(2000);
		js.Append("\r\n\n function PageSizeOptions(pagesizeList,htmlNavigationBar,up){");			
		js.Append("\r\n\t var list = pagesizeList.split(\",\");");
		js.Append("\r\n\t var isSelected = '';");
		js.Append("\r\n\t var tempSelect;");
		js.Append("\r\n\t var n;");
		js.Append("\r\n\t tempSelect='<span >'; ");			
		js.Append("\r\n\t tempSelect= tempSelect +' "+ GestionWeb.GetWebWord(2045,webSession.SiteLanguage)+" <select name=\"pageSizeOptions\" id=\"'+up+'pageSizeOptions\" onChange=\"ChangePageSize(this.value)\"  class=\"txtNoir11\">'; ");
		js.Append("\r\n\t for( n=0; n<list.length; n++){ ");
		js.Append("\r\n\t\t if(pageSize==list[n]){ ");
		js.Append("\r\n\t\t isSelected='selected';");
		js.Append("\r\n\t\t }");
		js.Append("\r\n\t\t if(n==0){ ");
		js.Append("\r\n\t\t minPageSize=list[n];");
		js.Append("\r\n\t\t }");
		js.Append("\r\n\t\t tempSelect = tempSelect + '<option value=\"'+list[n]+'\" '+isSelected+'>'+list[n]+'</option>';");
		js.Append("\r\n\t\t isSelected='';");
		js.Append("\r\n\t }");
		js.Append("\r\n\t tempSelect= tempSelect +'</select></span>&nbsp;'; ");
		js.Append("\r\n\t if(tab!=null && minPageSize>=tab.length){");
		js.Append("\r\n\t\t tempSelect= '';");
		js.Append("\r\n\t }");
			
		js.Append("\r\n\t htmlNavigationBar = tempSelect + htmlNavigationBar;");						
		js.Append("\r\n return(htmlNavigationBar);}");
						
		js.Append("\r\n\n function ChangePageSize(pagesizeIndex){");			
		js.Append("\r\n\t pageSize = pagesizeIndex;  ");

		js.Append("\r\n\t setCookie(\""+cookieID+"\",pagesizeIndex,365); ");//

		js.Append("\r\n\t currentPageIndex  = 1, leftPageIndex  = 0, rightPageIndex  = 0;");
		js.Append("\r\n\t if(tab!=null && tab.length>0){");//Total pages
		js.Append("\r\n\t\t pageCount = Math.ceil((tab.length - 1)/pageSize);");
		js.Append("\r\n\t }");
			
		//page de résultat	
		js.Append("\r\n\t oN.innerHTML='<img src=\"/Images/Common/waitAjax.gif\">';");
		js.Append("\r\n\t setTimeout(\"GetResultPage(oN)\",0);");
		js.Append("\r\n }");
			return js.ToString();
		}
		#endregion

		#region Fonction Option ajout en-tête parent
		/// <summary>
		/// Script fonction Option ajout en-tête parent
		/// </summary>	
		/// <param name="cookieID">Id cookie</param>
		/// <param name="webSession">Session du client</param>	
		/// <returns>Javascript Code</returns>
		public static string HeaderParentOption(string cookieID,WebSession webSession){
			
		StringBuilder js = new StringBuilder(2000);
		js.Append("\r\n\n function HeaderParentOption(htmlNavigationBar,up){");
		js.Append("\r\n\t var tempCheckBox,temp;");
		js.Append("\r\n\t var cook = GetCookie(\""+cookieID+"\"); ");
		js.Append("\r\n\t if(cook != null){");
		js.Append("\r\n\t if(cook==\"false\")temp = false; ");
		js.Append("\r\n\t else temp = true;");
		js.Append("\r\n\t isShowParenHeader =  temp;");
		js.Append("\r\n\t }");
		js.Append("\r\n\t tempCheckBox='<input type=checkbox style=\"WIDTH: 15px; HEIGHT: 15px\" id=\"'+up+'_headerParentOption\" '; ");
		js.Append("	if(isShowParenHeader){ ");
		js.Append("	tempCheckBox += ' checked '; ");
		js.Append("\r\n }");			
		js.Append("\r\n	tempCheckBox += ' onClick=\"SetHeaderParentOption(this.checked)\">'; ");
		js.Append("\r\n\t htmlNavigationBar = '&nbsp;&nbsp;|&nbsp;&nbsp;"+ GestionWeb.GetWebWord(2046,webSession.SiteLanguage)+"&nbsp;' + tempCheckBox + htmlNavigationBar;");
		js.Append("\r\n return(htmlNavigationBar);}");

		js.Append("\r\n\n function SetHeaderParentOption(showparent){");			
		js.Append("\r\n\t isShowParenHeader = showparent;");

		js.Append("\r\n\t setCookie(\""+cookieID+"\",isShowParenHeader,365); ");//Cookie rappel en-tête


		//page de résultat

		js.Append("\r\n\t oN.innerHTML='<img src=\"/Images/Common/waitAjax.gif\">';");
		js.Append("\r\n\t setTimeout(\"GetResultPage(oN)\",0);");
		js.Append("\r\n }");
			return js.ToString();
		}
		#endregion

		#region Fonction ajout de(s) ligne(s) parent
		/// <summary>
		/// Script Fonction ajout de(s) ligne(s) parent
		/// </summary>	
		/// <returns>Javascript Code</returns>
		public static string GetParentLines(){
			
			StringBuilder js = new StringBuilder(1000);
			js.Append("\r\n\n function GetParentLines(lineIndex,sBuilder){");
			js.Append("\r\n\t if( tabIndex[lineIndex]!=null && tabIndex[lineIndex]!=0 ){");
			js.Append("\r\n\t\t GetParentLines(tabIndex[lineIndex],sBuilder);");
			js.Append("\r\n\t\t sBuilder.append(tab[tabIndex[lineIndex]]);");
			js.Append("\r\n\t}");
			js.Append("\r\n }");
			return js.ToString();
		}
		#endregion

		#region Fonction StringBuilder
		/// <summary>
		/// Script Fonction Construtction chaine de caractères
		/// </summary>			
		/// <returns>Javascript Code</returns>
		public static string StringBuilder(){
			
			StringBuilder js = new StringBuilder(1000);
		js.Append("\r\n function StringBuilder(value){");
		js.Append("\r\n\t this.strings = new Array(\"\");");	
		js.Append("\r\n\t this.append(value);");
		js.Append("\r\n }");

		js.Append("\r\n StringBuilder.prototype.append = function (value){");
		js.Append("\r\n\t if (value){");	
		js.Append("\r\n\t this.strings.push(value);");
		js.Append("\r\n\t }");
		js.Append("\r\n  }");

		js.Append("\r\n StringBuilder.prototype.clear = function (){");
		js.Append("\r\n\t this.strings.length = 1;");				
		js.Append("\r\n  }");

		js.Append("\r\n StringBuilder.prototype.toString = function (){");
		js.Append("\r\n\t return this.strings.join(\"\");");				
		js.Append("\r\n  }");
			return js.ToString();
		}
		#endregion



    }
}
