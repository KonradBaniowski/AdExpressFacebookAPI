#region Informations
// Auteur: K. Shehzad 
// Date de création: 12/07/2005 
#endregion

using System;
using System.Data;
using System.Text;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Translation;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.UI.Results.APPM{
	/// <summary>
	/// This class claculates the HTML table for the APPM synthesis.
	/// </summary>
	public class SynthesisUI{

		#region HTML

		#region Synthsis HTML
		/// <summary>
		/// This method generates the HTML for the APPM Synthesis Tab
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget)
		{
			
			#region variables
			StringBuilder t=null;
			string styleTitle="portofolio2";
			string styleValue="portofolio22";
			bool mediaAgencyAccess=false;
			string idProductString = "";
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);

			#endregion

			#region Media Agency rights
			//To check if the user has a right to view the media agency or not
			//mediaAgencyAccess flag is used in the rest of the classes which indicates whethere the user has access 
			//to media agency or not
            string listStr = webSession.GetSelection(webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (webSession.CustomerLogin.CustomerMediaAgencyFlagAccess(Int64.Parse(listStr)))
				mediaAgencyAccess=true;

			#endregion

			#region IdProduct
			//this is the id of the product selected from the products dropdownlist. 0 id refers to the whole univers i.e. if no prodcut is
			//selected its by default the whole univers and is represeted by product id 0.
			Int64 idProduct=0;
			if (showProduct) {
				//string idProductString = webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.productAccess);
				if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0))
					idProductString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.PRODUCT);
				if (WebFunctions.CheckedText.IsStringEmpty(idProductString)) {
					idProduct = Int64.Parse(idProductString);
				}
			}
			#endregion

			#region get Data
			Hashtable synthesisData=TNS.AdExpress.Web.Rules.Results.APPM.SynthesisRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,idProduct,mediaAgencyAccess);
			#endregion
			
			#region HTML

			try{
				if(synthesisData!=null&&synthesisData.Count>0){
					t=new StringBuilder(3500);
					t.Append("<table  border=0 cellpadding=0 cellspacing=0 width=600 style=\"text-align:left\">");
					//titre
					//t.Append("\r\n\t<tr height=\"30px\"><td colspan=2 class=\"p2\" align=\"center\" style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid;font-size: 16px\">"+GestionWeb.GetWebWord(1666,webSession.SiteLanguage)+"</td></tr>");
                    t.Append("\r\n\t<tr height=\"30px\"><td colspan=2 class=\"portofolioSynthesisBorderHeader\" align=\"center\">" + GestionWeb.GetWebWord(1666, webSession.SiteLanguage) + "</td></tr>");
					if (idProduct != 0 && showProduct) {
						//Nom du Produit
						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1418,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["product"]+"</td></tr>");	
						//Nom de l'announceur
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1667,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["advertiser"]+"</td></tr>");	
						if(mediaAgencyAccess && synthesisData["agency"].ToString().Length>0){
							//Nom de l'agence Media
							styleTitle=InvertStyle(styleTitle);
							styleValue=InvertStyle(styleValue);
							t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(731,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["agency"]+"</td></tr>");	
						}

					}
					
					//Périod d'analyse
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(381,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["dateBegin"]+" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+" "+synthesisData["dateEnd"]+"</td></tr>");	
					//Budget brut (euros)
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1669,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["budget"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre d'insertions
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["insertions"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre des pages
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["pages"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Nombre de supports utilisés
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1670,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["supports"]).ToString("# ### ##0.##")+"</td></tr>");	
					//Secteur de référence
					//if the competitor univers is not selected we print the groups of the products selected
					//if(webSession.CompetitorUniversAdvertiser.Count<2){
					if(webSession.PrincipalProductUniverses.Count<2){
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						string[] groups=synthesisData["group"].ToString().Split(',');
						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50% valign=top>"+GestionWeb.GetWebWord(1668,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>");
						Array.Sort(groups);
						foreach(string gr in groups){
							t.Append("&nbsp;&nbsp;&nbsp;&nbsp;"+gr+"<br>");	
						}
						t.Append("</td></tr>");
					}
					if(synthesisData["PDV"].ToString()!=""){
						//Part de voix de la campagne
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1671,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["PDV"]).ToString("# ### ##0.##")+"%</td></tr>");	
					}
					//cible selectionnée
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1672,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["targetSelected"]+"</td></tr>");	
					// nombre de GRP
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPNumber"]).ToString("# ### ##0.##")+"</td></tr>");	
					// nombre de GRP 15 et +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPNumberBase"]).ToString("# ### ##0.##")+"</td></tr>");
					//Indice GRP vs cible 15 ans à +																				   
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);					
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1674,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["IndiceGRP"].ToString().Length > 0)? Convert.ToDouble(synthesisData["IndiceGRP"]).ToString("# ### ##0.##"):"&nbsp;")+"</td></tr>");	
					// Coût GRP(cible selectionnée)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["GRPCost"].ToString().Length>0)?Convert.ToDouble(synthesisData["GRPCost"]).ToString("# ### ##0"):"&nbsp;")+"</td></tr>");	
					// Coût GRP(cible 15 ans et +)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["GRPCostBase"].ToString().Length > 0)?Convert.ToDouble(synthesisData["GRPCostBase"]).ToString("# ### ##0"):"&nbsp;")+"</td></tr>");	
					//Indice coût GRP vs cible 15 ans à +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1676,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((synthesisData["IndiceGRPCost"].ToString().Length>0)?Convert.ToDouble(synthesisData["IndiceGRPCost"]).ToString("# ### ##0.##"):"&nbsp;")+"</td></tr>");									

					t.Append("</table>");	
				}				
			}
			catch(System.Exception e){
				throw(new WebExceptions.SynthesisUIException("Error while constructing the HTML for the Synthesis Table ",e));
			}
			#endregion			

			#region no data
			if(t==null||t.Length<=0)
					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
				
			#endregion

			return t.ToString();
		}
		#endregion

		#region Synthesis by version HTML
		/// <summary>
		/// This method generates the HTML for the APPM Synthesis by version Tab
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="idVersion">ID Version</param>
		/// <param name="firstInsertionDate">First insertion date</param>
		/// <param name="excel">Excel parameter for display the right clic information</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetHTML(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,string idVersion,string firstInsertionDate,bool excel){
		
			#region variables
			StringBuilder t=null;
			string styleTitle="portofolio2";
			string styleValue="portofolio22";
			bool mediaAgencyAccess=false;
			string idProductString = "";
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
			#endregion

			#region Media Agency rights
			//To check if the user has a right to view the media agency or not
			//mediaAgencyAccess flag is used in the rest of the classes which indicates whethere the user has access 
			//to media agency or not

            string listStr = webSession.GetSelection(webSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
			if(webSession.CustomerLogin.CustomerMediaAgencyFlagAccess(Int64.Parse(listStr)))
				mediaAgencyAccess=true;

			#endregion

			#region IdProduct
			//this is the id of the product selected from the products dropdownlist. 0 id refers to the whole univers i.e. if no prodcut is
			//selected its by default the whole univers and is represeted by product id 0.
			Int64 idProduct=0;
			if (showProduct) {
				//string idProductString = webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.productAccess);
				if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0))
					idProductString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.PRODUCT);

				if (WebFunctions.CheckedText.IsStringEmpty(idProductString)) {
					idProduct = Int64.Parse(idProductString);
				}
			}
			#endregion

			#region get Data
			Hashtable synthesisData=TNS.AdExpress.Web.Rules.Results.APPM.SynthesisRules.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,mediaAgencyAccess,idVersion,firstInsertionDate);
			#endregion
			
			#region HTML

			try{
				if(synthesisData!=null&&synthesisData.Count>0){
                    string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

					t=new StringBuilder(3500);
					t.Append("<table  border=0 cellpadding=0 cellspacing=0 width=600 >");			

					#region Info sur le clic droit de la souris
					if(!excel){
						t.Append("<tr><td colspan=2>");
                        t.Append("\n<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"headerTabBackGround\">");
						t.Append("\n<tr><td>");
						t.Append("\n<script language=\"javascript\" type=\"text/javascript\">");
						t.Append("\nif(hasRightFlashVersion==true){");
						t.Append("\ndocument.writeln('<object id=\"infoOptionFlash\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\" width=\"400\" height=\"20\" VIEWASTEXT>');");
                        t.Append("\ndocument.writeln('<param name=\"movie\" value=\"/App_Themes/" + themeName + "/Flash/Culture/infoOptionsOneLine.swf\">');");
						t.Append("\ndocument.writeln('<param name=\"quality\" value=\"high\">');");
						t.Append("\ndocument.writeln('<param name=\"menu\" value=\"false\">');");
						t.Append("\ndocument.writeln('<param name=\"wmode\" value=\"transparent\">');");
                        t.Append("\ndocument.writeln('<embed src=\"/App_Themes/" + themeName + "/Flash/Culture/infoOptionsOneLine.swf\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"400\" height=\"20\"></embed>');");
						t.Append("\ndocument.writeln('</object></td>');");
						t.Append("\n}\nelse{");
                        t.Append("\ndocument.writeln('<img src=\"/App_Themes/" + themeName + "/Images/Culture/FlashReplacement/infoOptionsOneLine.gif\"></td>');");
						t.Append("\n}");
						t.Append("\n</script>");
						t.Append("\n</tr>");
						t.Append("\n</table>");
						t.Append("</td></tr>");
					}
					#endregion

					//titre
                    t.Append("\r\n\t<tr height=\"30px\"><td colspan=2 align=\"center\" class=\"synthesisUiTitle\">" + GestionWeb.GetWebWord(2003, webSession.SiteLanguage) + "</td></tr>");	
						
						//Numéro de version
						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2005,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["version"]+"</td></tr>");

						if (showProduct) {
							//Nom du Produit
							styleTitle = InvertStyle(styleTitle);
							styleValue = InvertStyle(styleValue);
							t.Append("\r\n\t<tr ><td class=\"" + styleTitle + "\" width=50%>" + GestionWeb.GetWebWord(1418, webSession.SiteLanguage) + " : " + "</td><td class=\"" + styleValue + "\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;" + synthesisData["product"] + "</td></tr>");
						}
						//Nom de la marque
						if(webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE)){
							styleTitle=InvertStyle(styleTitle);
							styleValue=InvertStyle(styleValue);
							t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2001,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["brand"]+"</td></tr>");	
						}

						//Nom de l'annonceur
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1667,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["advertiser"]+"</td></tr>");	
						if(mediaAgencyAccess && synthesisData["agency"].ToString().Length>0){
							//Nom de l'agence Media
							styleTitle=InvertStyle(styleTitle);
							styleValue=InvertStyle(styleValue);
							t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(731,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["agency"]+"</td></tr>");	
						}

					//Périod d'analyse
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(381,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["dateBegin"]+" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+" "+synthesisData["dateEnd"]+"</td></tr>");
					
					//Date de la 1ere insertion de la version
					if(synthesisData.Contains("firstInsertionDate")){
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2000,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["firstInsertionDate"]+" </td></tr>");	
					}
					
					//Budget brut (euros)
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1669,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["budget"]).ToString("# ### ##0.##")+"</td></tr>");	
					
					//Nombre d'insertions
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["insertions"]).ToString("# ### ##0.##")+"</td></tr>");	
					
					//Poids de la version vs total du produit
					if(synthesisData["versionWeight"]!=null && synthesisData["versionWeight"].ToString()!=""){						
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(2002,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["versionWeight"]).ToString("# ### ##0.##")+"%</td></tr>");	
					}

					//Nombre des pages
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["pages"]).ToString("# ### ##0.##")+"</td></tr>");	

					//Nombre de supports utilisés
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1670,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["supports"]).ToString("# ### ##0.##")+"</td></tr>");	

					//Secteur de référence
					//if the competitor univers is not selected we print the groups of the products selected
					//if(webSession.CompetitorUniversAdvertiser.Count<2){
					if(webSession.PrincipalProductUniverses.Count<2){
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						string[] groups=synthesisData["group"].ToString().Split(',');
						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50% valign=top>"+GestionWeb.GetWebWord(1668,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>");
						Array.Sort(groups);
						foreach(string gr in groups){
							t.Append("&nbsp;&nbsp;&nbsp;&nbsp;"+gr+"<br>");	
						}
						t.Append("</td></tr>");
					}

					if(synthesisData["PDV"].ToString()!=""){
						//Part de voix de la campagne
						styleTitle=InvertStyle(styleTitle);
						styleValue=InvertStyle(styleValue);
						t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1671,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["PDV"]).ToString("# ### ##0.##")+"%</td></tr>");	
					}

					//cible selectionnée
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1672,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+synthesisData["targetSelected"]+"</td></tr>");	
					
					// nombre de GRP
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPNumber"]).ToString("# ### ##0.##")+"</td></tr>");	
					
					// nombre de GRP 15 et +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1673,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPNumberBase"]).ToString("# ### ##0.##")+"</td></tr>");
					
					//Indice GRP vs cible 15 ans à +																				   
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1674,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["IndiceGRP"]).ToString("# ### ##0.##")+"</td></tr>");	
					// Coût GRP(cible selectionnée)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPCost"]).ToString("# ### ##0")+"</td></tr>");	
					
					// Coût GRP(cible 15 ans et +)					
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1675,webSession.SiteLanguage) + " " +  synthesisData["baseTarget"] + " : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["GRPCostBase"]).ToString("# ### ##0")+"</td></tr>");	
					
					//Indice coût GRP vs cible 15 ans à +
					styleTitle=InvertStyle(styleTitle);
					styleValue=InvertStyle(styleValue);
					t.Append("\r\n\t<tr ><td class=\""+styleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1676,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" : "+"</td><td class=\""+styleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+Convert.ToDouble(synthesisData["IndiceGRPCost"]).ToString("# ### ##0.##")+"</td></tr>");									

					t.Append("</table>");	
				}				
			}
			catch(System.Exception e){
				throw(new WebExceptions.SynthesisUIException("Error while constructing the HTML for the Synthesis by Version Table ",e));
			}
			#endregion			

			#region no data
			if(t==null||t.Length<=0)
                return ("<div align=\"center\"><table class=\"txtViolet11Bold whiteBackGround\"><tr><td>" + GestionWeb.GetWebWord(177, webSession.SiteLanguage) + "</td></tr></table></div>");
				
			#endregion

			return t.ToString();
		}
		#endregion

		#endregion

		#region Excel
		/// <summary>
		/// Excel de Synthèse APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible supplementaire</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetExcel(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			#region Variables
			StringBuilder t = new StringBuilder(20000);
			#endregion

			#region Rappel des paramètres
			// Paramètres du tableau
            t.Append(ExcelFunction.GetAppmLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession, GestionWeb.GetWebWord(1666, webSession.SiteLanguage)));
			#endregion

			t.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.UI.Results.APPM.SynthesisUI.GetHTML(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget)));

			t.Append(ExcelFunction.GetFooter(webSession));
			return t.ToString();
		}

		/// <summary>
		/// Excel de Synthèse APPM par version
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible supplementaire</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="idVersion">ID Version</param>
		/// <param name="firstInsertionDate">First insertion date</param>
		/// <param name="excel">Excel parameter for display the right clic information</param>
		/// <returns>HTML string for the synthesis table</returns>
		public static string GetExcel(WebSession webSession,IDataSource dataSource, int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,string idVersion,string firstInsertionDate,bool excel) {
			#region Variables
			StringBuilder t = new StringBuilder(20000);
			#endregion

			#region Rappel des paramètres
			// Paramètres du tableau
            t.Append(ExcelFunction.GetAppmLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession, GestionWeb.GetWebWord(1666, webSession.SiteLanguage)));
			#endregion

			t.Append(Convertion.ToHtmlString(TNS.AdExpress.Web.UI.Results.APPM.SynthesisUI.GetHTML(webSession, dataSource, dateBegin, dateEnd, baseTarget, additionalTarget, idVersion, firstInsertionDate, excel)));
			t.Append(ExcelFunction.GetFooter(webSession));
			return t.ToString();
		}
		#endregion

		#region private methods
		/// <summary>
		/// inverts the style for alternating rows
		/// </summary>
		/// <param name="style">the style string to be inverted</param>
		/// <returns>the inverted style</returns>
		private static string InvertStyle(string style)
		{
			string invertedStyle=string.Empty;
			//inverting the title style
			if(style=="portofolio2")
				invertedStyle="portofolio1";
			else if(style=="portofolio1")
				invertedStyle="portofolio2";
			//inverting the value style
			else if(style=="portofolio22")
				invertedStyle="portofolio11";
			else if(style=="portofolio11")
				invertedStyle="portofolio22";
			
			return invertedStyle;
		}

		#endregion

	}
}
