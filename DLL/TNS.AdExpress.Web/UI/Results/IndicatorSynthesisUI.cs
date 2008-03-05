#region Information
// Auteur: Dédé Mussuma
// Créé le: 18/04/2006
// Modifiée le: 
#endregion
using System;
using System.Text;
using TNS.AdExpress.Web;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebRules=TNS.AdExpress.Web.Rules;
using TNS.AdExpress.Web.Core.Translation;
using FrameWorkConstantes = TNS.AdExpress.Constantes.FrameWork;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.UI.Results
{
	/// <summary>
	/// Classe d'affichage des données de la synthèse des indicateurs sectorielles.
	/// </summary>
	public class IndicatorSynthesisUI
	{
		#region Synthèse sortie HTML
		/// <summary>
		/// Présente au format HTML la synthèse des indicateurs sectorielles.
		/// </summary>
		/// <param name="webSession">Sesssion du client</param>
		/// <param name="excel">Vrai si sortie au format excel</param>
		/// <param name="pdf">Vrai si sortie au format Pdf</param>
		/// <returns></returns>
		public static string GetIndicatorSynthesisHtmlUI(WebSession webSession,bool excel,bool pdf) {
			StringBuilder t = new StringBuilder(2000);
			const string P2="p2";
			string L1="acl1";
			string L2="acl2";
//			string L3="acl3";		
			string unit="";
			string classCss=L1;
			if(excel)classCss="acl31";
			bool percentage=false;
//			string symbol="";
			string image="";
			bool evol=false;
			double tmpData=0;

			#region style excel
			if(excel){
				L1="acl11";
				L2="acl21";
//				L3="acl31";
			}
			#endregion
		
			//Obtention du tableau de résulats
			object[,] tab = WebRules.Results.IndicatorSynthesisRules.GetFormattedTable(webSession);
			if(tab!=null){

				#region Affichage du tableau de résultats
				t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 align=center>");
					
				#region En-tête du tableau
				t.Append("\r\n\t<tr height=\"20px\">");
				//Libellé période N
				t.Append("<td  bgcolor=#ffffff nowrap  valign=\"middle\">&nbsp;</td>");
				t.Append("<td class=\""+P2+"\"  nowrap >"+tab[0,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX].ToString()+"</td>");				
				if(webSession.ComparativeStudy){
					//Libellé période N-1
					t.Append("<td class=\""+P2+"\"  nowrap>"+tab[0,FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX].ToString()+"</td>");				
					//Evolution
					t.Append("<td  class=\""+P2+"\" nowrap  >"+tab[0,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX].ToString()+"</td>");
					//Ecart
					t.Append("<td  class=\""+P2+"\" nowrap >"+tab[0,FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX].ToString()+"</td>");
				}				
				t.Append("</tr>");
				#endregion

				#region Lignes du tableau
				for(int i=1;i<tab.GetLength(0);i++){
					
					t.Append("\r\n\t<tr align=\"right\"  bgcolor=#B1A3C1 height=\"20px\" >");

					for(int j=0;j<tab.GetLength(1);j++){							
						unit ="";
						// libellé des univers à calculer
						if(j==FrameWorkConstantes.Results.SynthesisRecap.LABEL_COLUMN_INDEX ){
							if(tab[i,j]!=null)t.Append("\r\n\t\t<td align=\"left\" class=\""+L1+"\" nowrap>"+tab[i,j].ToString()+"</td>");
							else t.Append("\r\n\t\t<td align=\"left\" class=\""+L1+"\" nowrap>&nbsp;</td>");
						}
						if(j==FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX || 
							(j>FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX && webSession.ComparativeStudy)){ //Affiche données année N ou N-1 avec étude comparartive
							//Nombre de produits ou d'annonceurs actifs						
							if(IsNumberValue(i,j)){
								if(tab[i,j]!=null)t.Append("\r\n\t<td bgcolor=#D0C8DA class=\""+L2+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ##0")+"</td>");											 
								else t.Append("\r\n\t\t<td align=\"left\" bgcolor=#D0C8DA class=\""+L2+"\" nowrap>&nbsp;</td>");
							}else{
								//Valeur Keuro ou PDV ou Evolution
								percentage = IsPercentageValue(i,j);
								if(percentage && tab[i,j]!=null){
					
//									if( !excel ){ //&& tab[i,FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX]!=null && ){
										evol = (j==FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX )? true : false;
										//evol
										tmpData = double.Parse(tab[i,j].ToString());											
										if (tmpData>0) {//hausse
											if(evol && !excel && !pdf)image = "<img src=/I/g.gif>";
											unit =  ( (!Double.IsInfinity(tmpData)) ? Double.Parse(tmpData.ToString()).ToString("### ### ### ### ##0.##")+" %" : "" ) + image;
										}
										else if(tmpData<0){ //baisse
											if(evol && !excel && !pdf)image = "<img src=/I/r.gif>";
											unit = ( (!Double.IsInfinity(tmpData)) ? Double.Parse(tmpData.ToString()).ToString("### ### ### ### ##0.##")+" %" : "" ) + image;
										}
										else if (!Double.IsNaN(tmpData)){ // 0 exactement
											if(evol ){
												if(!excel && !pdf)image = "<img src=/I/o.gif>";
//												unit =" 0 %<img src=/I/o.gif></td>";
												unit =" 0 % "+image;
											}else
											unit = ( (!Double.IsNaN(tmpData)) ? Double.Parse(tmpData.ToString()).ToString("### ### ### ### ##0.##")+" %" : "" );
										}
										else
											unit =""; 																	
//									}	
									image="";	
								}else if(IsUnitValue(i,j) && tab[i,j]!=null){
									unit = WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),WebConstantes.CustomerSessions.Unit.kEuro,percentage);						
								}
								if(unit.Trim().Length==0)unit="";							
								t.Append("\r\n\t<td bgcolor=#D0C8DA class=\""+L2+"\" nowrap>"+unit+"</td>");
								unit="";	
							}
						}
								
					}
					t.Append("</tr>");
					if(i==FrameWorkConstantes.Results.SynthesisRecap.PDV_UNIV_TOTAL_MARKET_LINE_INDEX
						|| i==FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX)
							t.Append(AddSeparator(excel,webSession.ComparativeStudy));
					
				}

				#endregion
				t.Append("</table>");
				#endregion
	
				return t.ToString();
			}else{
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}
			
		}
		#endregion

		#region Synthèse sortie Excel
		/// <summary>
		/// Présente au format Excel la synthèse des indicateurs sectorielles.
		/// </summary>
		/// <param name="webSession">Sesssion du client</param>
		/// <param name="excel">Vrai si sortie au format excel</param>
		/// <returns></returns>
		public static string GetIndicatorSynthesisExcelUI(WebSession webSession,bool excel) {
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

			#region Rappel des paramètres
            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,GestionWeb.GetWebWord(1254,webSession.SiteLanguage)));						
			#endregion

			t.Append(GetIndicatorSynthesisHtmlUI(webSession,true,false));
			t.Append(ExcelFunction.GetFooter(webSession));
			return Convertion.ToHtmlString(t.ToString());
		
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Vérifie si une valeur est de type unité (KEuro,euro,...)
		/// </summary>
		/// <param name="lineIndex">index ligne du tableau</param>
		/// <param name="columnIndex">index colonne du tableau</param>
		/// <returns>Vrai si type unité.</returns>
		private static bool IsUnitValue(int lineIndex, int columnIndex){
			switch(lineIndex){
				case FrameWorkConstantes.Results.SynthesisRecap.TOTAL_UNIV_INVEST_LINE_INDEX :
				case FrameWorkConstantes.Results.SynthesisRecap.TOTAL_SECTOR_INVEST_LINE_INDEX :
				case FrameWorkConstantes.Results.SynthesisRecap.TOTAL_MARKET_INVEST_LINE_INDEX :
				case FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_ADVERTISER_LINE_INDEX :
				case FrameWorkConstantes.Results.SynthesisRecap.AVERAGE_INVEST_BY_PRODUCT_LINE_INDEX :
				switch(columnIndex){
					case FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX:
					case FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX :					
					case FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX :
						return true;
					default : return false;
				}				
				default : return false;
			}
		}

		/// <summary>
		/// Vérifie si une valeur est en % (PDV,Evolution,...)
		/// </summary>
		/// <param name="lineIndex">index ligne du tableau</param>
		/// <param name="columnIndex">index colonne du tableau</param>
		/// <returns>Vrai si pourcentage.</returns>
		private static bool IsPercentageValue(int lineIndex, int columnIndex){
			switch(lineIndex){
				case FrameWorkConstantes.Results.SynthesisRecap.PDV_UNIV_TOTAL_MARKET_LINE_INDEX :
				case FrameWorkConstantes.Results.SynthesisRecap.PDV_UNIV_TOTAL_SECTOR_LINE_INDEX :
					return true;								
				default :
				switch(columnIndex){
					case FrameWorkConstantes.Results.SynthesisRecap.EVOLUTION_COLUMN_INDEX:					
						return true;
					default : return false;
				}	
			}			
		}
		
		/// <summary>
		/// Vérifie si une valeur est un nombre (KEuro,euro,...)
		/// </summary>
		/// <param name="lineIndex">index ligne du tableau</param>
		/// <param name="columnIndex">index colonne du tableau</param>
		/// <returns>Vrai si nombre.</returns>
		private static bool IsNumberValue(int lineIndex, int columnIndex){
			switch(lineIndex){
				case FrameWorkConstantes.Results.SynthesisRecap.NB_ADVERTISER_LINE_INDEX :
				case FrameWorkConstantes.Results.SynthesisRecap.NB_PRODUCT_LINE_INDEX :				
				switch(columnIndex){
					case FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N_COLUMN_INDEX:
					case FrameWorkConstantes.Results.SynthesisRecap.TOTAL_N1_COLUMN_INDEX :					
					case FrameWorkConstantes.Results.SynthesisRecap.ECART_COLUMN_INDEX :
						return true;
					default : return false;
				}				
				default : return false;
			}
		}
		
		/// <summary>
		/// Separateur de lignes du tableau HTML.
		/// </summary>	
		/// <param name="excel">Vrai si format excel</param>
		/// <param name="comparativeStudy">Vrai s iétude comparative</param>
		/// <returns></returns>
		private static string AddSeparator(bool excel,bool comparativeStudy){
			string separator="";
			string colspan="3";
			if(comparativeStudy)colspan="5"; 
			if(!excel)separator="<tr><td bgcolor=\"#ffffff\" style=\"HEIGHT: 5px; BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\" colspan="+colspan+"></td></tr>";	
			//if(!excel)separator="<tr><td bgcolor=\"#ffffff\" style=\"BORDER-TOP: white 0px solid;BORDER-BOTTOM: white 1px solid\" colspan="+colspan+"><img width=3px></td></tr>";	// bgcolor=\"#644883\"	
			return separator;
		}

		#endregion
	}
}
