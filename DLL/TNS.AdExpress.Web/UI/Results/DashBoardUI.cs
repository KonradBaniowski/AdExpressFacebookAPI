#region Informations
// Auteur: A. DADOUCH	
// Date de création: 21/04/2005
//	12/08/2005	G. Facon Gestion des exceptions
// 12/08/2005 D. V. Mussuma : renommage de DashBoardRules.getDataTable(webSession) en DashBoardRules.GetDataTable(webSession);
#endregion

using System;
using System.Web.UI;
using System.Windows.Forms;
using System.IO;
using System.Data; 
using System.Collections;
using System.Text;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.UI.Selections.Periods;
using TNS.AdExpress.Web.DataAccess.Selections.Periods;
using TNS.AdExpress.Domain.Translation;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using ClassificationCst=TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Domain.Translation;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ResultConstantes= TNS.AdExpress.Constantes.FrameWork.Results.DashBoard;
using CstDB = TNS.AdExpress.Constantes.DB; 
using TNS.AdExpress.Web.Rules.Results;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using DateFrameWork=TNS.FrameWork.Date;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;


namespace TNS.AdExpress.Web.UI.Results {
	/// <summary>
	/// Description résumée de DashBoardUI.
	/// </summary>
	public class DashBoardUI { 
		
		#region Html
		/// <summary>
		/// Génère l'export Excel pour Tableau de Bord
		/// </summary>
		///<param name="webSession">Session du client</param>
		/// <returns>tableau formatté pour affichage sous excel</returns>			
		internal static string  GetHtml(WebSession webSession){
			try{
				return(GetHtmlSource(webSession,false));
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardUIException("Impossible de construire le code HTML",err));
			}
		}


		#endregion
		 
		#region Excel1
		/// <summary>
		/// Génère l'export Excel pour Tableau de Bord
		/// </summary>
		///<param name="webSession">Session du client</param>
		/// <returns>tableau formatté pour affichage sous excel</returns>			
		internal static string  GetExcel(WebSession webSession){
						
			#region Variables 
			StringBuilder t=new StringBuilder(5000);
			string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(Int64.Parse(Vehicle));
			#endregion
			string module=""; 
			try{
				switch (webSession.CurrentModule){
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PRESSE:
						module=GestionWeb.GetWebWord(1613,webSession.SiteLanguage);
						break;
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_RADIO:
						module=GestionWeb.GetWebWord(1614,webSession.SiteLanguage);
						break;
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_TELEVISION:
						module=GestionWeb.GetWebWord(1615,webSession.SiteLanguage);
						break;
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PAN_EURO:
						module=GestionWeb.GetWebWord(1892,webSession.SiteLanguage);
						break;
					default:
						break;

				}
                t.Append(ExcelFunction.GetLogo(webSession));
				t.Append(ExcelFunction.GetExcelHeader(webSession,true,true,false,true,module));		//TODO : Adapter pour univers generique		
				t.Append(GetHtmlSource(webSession,true));
				t.Append(ExcelFunction.GetFooter(webSession));					
				return Convertion.ToHtmlString(t.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardUIException("Impossible de construire le code Excel",err));
			}
		}
	
		#endregion 
		
		#region Excel2
		/// <summary>
		/// Lance l'affichage correspondant au tableau demandé
		/// </summary>
		///<param name="webSession">Session du client</param>
		/// <returns>UI du tableau</returns>	
		public static string  GetExcelBis(WebSession webSession){
			

			#region variable
			//les labels des 3 niveaux
			bool label1=false;
			bool label2=false;
			bool label3=false;
			
			//les labels des a sélectionnés
			bool currentPeriod=false;
			bool precedingPeriod=false;
			bool currentPdm=false;
			bool precedingPdm=false;
			bool currentPdv=false;
			bool precedingPdv=false;
			bool evolution=false;
			//Ligne vide
			bool epmty=true;

			string space2 ="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
			string space3 ="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
			
			string tempo1="";
			string tempo2="";

			int nbCol=0;
			//Nombre des données non null dans une Ligne vide
			int epmtyLine=0;
			int tempoLabel1=0;
			int tempoLabel2=0;
			//Nombre des libellés sélectionnées et affichés
			int nbLabel=0;
			int tempId=-1;
			
			string P2="p2";
			string classCss="";
			#endregion

			object[,] tab=null;
			StringBuilder html=new StringBuilder(5000);
			string module="";

			try{
				switch (webSession.CurrentModule) {
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PRESSE:
						module=GestionWeb.GetWebWord(1613,webSession.SiteLanguage);
						break;
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_RADIO:
						module=GestionWeb.GetWebWord(1614,webSession.SiteLanguage);
						break;
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_TELEVISION:
						module=GestionWeb.GetWebWord(1615,webSession.SiteLanguage);
						break;
					case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PAN_EURO:
						module=GestionWeb.GetWebWord(1892,webSession.SiteLanguage);
						break;
					default:
						break;

				}
                html.Append(ExcelFunction.GetLogo(webSession));
				html.Append(ExcelFunction.GetExcelHeader(webSession,true,true,false,true,module)); //TODO : Adapter pour univers generique		

				try{
					tab = DashBoardRules.GetDataTable(webSession);
				}
				catch(System.Exception err){
					throw(new WebExceptions.DashBoardUIException("Impossible d'obtenir le tableau de résultat",err));
				}
				if(tab==null || tab.Length==0)
					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");

					#region 1er ligne du tableau
					//1ere ligne du tableau
					html.Append("<table justify=center border=0 cellpadding=0 cellspacing=0 width=400 >");
					html.Append("\r\n\t<tr height=\"30px\">");
					html.Append("<td>&nbsp;</td>");
				
					//Nombre de Colonnes resultat
			
					if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
						nbCol=tab.GetLength(1)-ResultConstantes.TOTAL_COLUMN_INDEX;
					}else
						nbCol=tab.GetLength(1)-(ResultConstantes.TOTAL_COLUMN_INDEX+1);
					//parcour du tableau
					for(int i=1;i<tab.GetLength(0);i++){
						if(!currentPeriod){
							if (tab[i,ResultConstantes.PERIOD_N_COLUMN_INDEX]!=null){
								html.Append("<td  align= \"center\" class=\""+P2+"\" nowrap colspan="+nbCol+">"+(tab[i,ResultConstantes.PERIOD_N_COLUMN_INDEX].ToString())+"</td>");
								nbLabel++;							
								currentPeriod=true;
							}
						}
				
						if(!precedingPeriod){
							if (tab[i,ResultConstantes.PERIOD_N1_COLUMN_INDEX]!=null){
								html.Append("<td  align= \"center\" class=\""+P2+"\" nowrap colspan="+nbCol+">"+(tab[i,ResultConstantes.PERIOD_N1_COLUMN_INDEX].ToString())+"</td>");					
								nbLabel++;
								precedingPeriod=true;
							}
						}
						if(!currentPdm){
							if (tab[i,ResultConstantes.PDM_COLUMN_INDEX]!=null){
								html.Append("<td  align= \"center\" class=\""+P2+"\" nowrap colspan="+nbCol+">"+(tab[i,ResultConstantes.PDM_COLUMN_INDEX].ToString())+"</td>");
								nbLabel++;
								currentPdm=true;	
								tempo1=tab[i,ResultConstantes.PDM_COLUMN_INDEX].ToString();
							}
						}
	
						if(!precedingPdm) {
							if ((tab[i,ResultConstantes.PDM_COLUMN_INDEX]!=null)&&(tempo1!=tab[i,ResultConstantes.PDM_COLUMN_INDEX].ToString())){
								html.Append("<td  align= \"center\" class=\""+P2+"\" nowrap colspan="+nbCol+">"+(tab[i,ResultConstantes.PDM_COLUMN_INDEX].ToString())+"</td>");
								nbLabel++;
								precedingPdm=true;
							}
						}

						if(!currentPdv){
							if (tab[i,ResultConstantes.PDV_COLUMN_INDEX]!=null){
								html.Append("<td  align= \"center\" class=\""+P2+"\" nowrap colspan="+nbCol+">"+(tab[i,ResultConstantes.PDV_COLUMN_INDEX].ToString())+"</td>");
								nbLabel++;
								currentPdv=true;
								tempo2=tab[i,ResultConstantes.PDV_COLUMN_INDEX].ToString();
							}
						}
						if(!precedingPdv){
							if ((tab[i,ResultConstantes.PDV_COLUMN_INDEX]!=null)&&(tempo2!=tab[i,ResultConstantes.PDV_COLUMN_INDEX].ToString())){
								html.Append("<td  align= \"center\" class=\""+P2+"\" nowrap colspan="+nbCol+">"+(tab[i,ResultConstantes.PDV_COLUMN_INDEX].ToString())+"</td>");
								nbLabel++;
								precedingPdv=true;
							}
						}
						if(!evolution){
							if (tab[i,ResultConstantes.EVOL_COLUMN_INDEX]!=null){
								html.Append("<td  align= \"center\" class=\""+P2+"\" nowrap colspan="+nbCol+">"+(tab[i,ResultConstantes.EVOL_COLUMN_INDEX].ToString())+"</td>");					;
								nbLabel++;
								evolution=true;
							}
						} 
					}
					tempoLabel1 =nbLabel;
	
					html.Append("</tr>");
					#endregion

					html.Append("\r\n\t<tr height=\"20px\">");
					html.Append("<td>&nbsp;</td>");
					//lignes des sous_libellés
					while(nbLabel!=0){
						for(int j=ResultConstantes.TOTAL_COLUMN_INDEX;j<tab.GetLength(1);j++){
							if(tab[0,j]!= null){
								html.Append("<td  align=\"center\" height=\"20px\" class=\""+P2+"\" nowrap>"+(tab[0,j].ToString())+"</td>");
							}	
						}
						if(nbLabel!=1){
						}
						nbLabel--;
					}
					html.Append("</tr>");
						
				for(int i=1;i<tab.GetLength(0);i++){

					#region ligne vide
					epmtyLine=0;
					epmty=true;
					for(int k=0;k<tab.GetLength(1);k++){
						if (tab[i,k]!=null)
							epmtyLine++;
					}
					if(epmtyLine!=0)
						epmty=false;
					
					#endregion
						
					if(!epmty){

						#region niveau1
						if(tab[i,ResultConstantes.ID_ELMT_L1_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX]==null){
							classCss="acl11";
							if(!label1){
                                html.Append("\r\n\t<tr align=\"right\"  class=\"whiteBackGround\" height=\"20px\" >\r\n\t\t<td align=\"left\" class=\"" + classCss + "\" nowrap >" + (tab[i, ResultConstantes.LABEL_ELMT_L1_COLUMN_INDEX].ToString()) + "</td>");
								label1=true;	
								tempoLabel2= tempoLabel1;
							}
						}
							#endregion
						
							#region Niveau2
						else if(tab[i,ResultConstantes.ID_ELMT_L1_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX]==null){
							
							if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia){		
								classCss="acl31";
							}else
								classCss="acl21";
						
							if(tempId==int.Parse(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString())){
								label2=true;
							}else label2=false;
							if(!label2){
								html.Append("</tr>");
								tempoLabel2= tempoLabel1;
                                html.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");
                                html.Append("\r\n\t<tr align=\"right\"  class=\"whiteBackGround\" height=\"20px\" >\r\n\t\t<td align=\"left\" class=\"" + classCss + "\" nowrap>" + space2 + "" + (tab[i, ResultConstantes.LABEL_ELMT_L2_COLUMN_INDEX].ToString()) + "</td>");
								tempId=int.Parse(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString());
							}
						}
							#endregion

							#region niveau3
						else if(tab[i,ResultConstantes.ID_ELMT_L1_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX]!=null){
							classCss="acl31";

							if(tempId==int.Parse(tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX].ToString())){
								label3=true;
							}else label3=false;
							if(!label3){
								html.Append("</tr>");
								tempoLabel2= tempoLabel1;
                                html.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");
                                html.Append("\r\n\t<tr align=\"right\"  class=\"whiteBackGround\" height=\"20px\" >\r\n\t\t<td align=\"left\" class=\"" + classCss + "\" nowrap>" + space3 + "" + (tab[i, ResultConstantes.LABEL_ELMT_L3_COLUMN_INDEX].ToString()) + "</td>");
								tempId=int.Parse(tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX].ToString());
							}
						}
						#endregion
	
						#region Affichage de données
						
						if( tempoLabel2!=0){

							if (tab[i,ResultConstantes.PERIOD_N_COLUMN_INDEX]!=null){
								CurrentPeriode(tab,webSession,html,i,classCss);

							}else if (webSession.ComparativeStudy && tab[i,ResultConstantes.PERIOD_N1_COLUMN_INDEX]!=null){
								PrecedingPeriode(tab,webSession,html,i,classCss);
				
							}else if (webSession.PDM && tab[i,ResultConstantes.PDM_COLUMN_INDEX]!=null){
								PDM(tab,webSession,html,i,classCss);
						
							}else if (webSession.PDV && tab[i,ResultConstantes.PDV_COLUMN_INDEX]!=null){
								PDV(tab,webSession,html,i,classCss);
					
							}else if (webSession.Evolution && tab[i,ResultConstantes.EVOL_COLUMN_INDEX]!=null){
								EvolutionPeriode(tab,webSession,html,i,classCss,true);
							
							}
							else
								for (int j=1;j<=nbCol;j++){
									html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
								}
							tempoLabel2--;
						}	
					}
					#endregion	
				}
				html.Append(ExcelFunction.GetFooter(webSession));
				return Convertion.ToHtmlString(html.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardUIException("Impossible de construire le code Excel",err));
			}
		}
		#endregion

		#region méthodes internes

		#region Formate le tableau pour l'HTML et le premier Excel
		/// <summary>
		/// Lance l'affichage correspondant au tableau demandé
		/// </summary>
		///<param name="webSession">Session du client</param>
		///<param name="excel">Excel</param>
		/// <returns>UI du tableau</returns>
		private static string  GetHtmlSource(WebSession webSession,bool excel){
			#region variable
			bool label1=false;
			bool label2=false;
			bool label3=false;
			int nbCol=0;
			int tempId=-1;

			string space2 ="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
			string space3 ="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
			string P2="p2";
					
			string classCss="";
			string classCss2="";
			string classCss3="";
			//couleurs de sorties 
			//string colorClasses="";
			string colorClasses2="";
			string colorClasses3="";
		
			#endregion
		
			#region UI
		
			object[,] tab=null;
			StringBuilder html=new StringBuilder(5000);
		
			try{
				tab = DashBoardRules.GetDataTable(webSession);
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardUIException("Impossible d'obtenir le tableau de résultat",err));
			}
			if(tab==null || tab.GetLength(0)==0 )return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)+"</div>");
						
			#region 1er ligne du tableau						
			html.Append("<table justify=center border=0 cellpadding=0 cellspacing=0 width=400 >");
			html.Append("\r\n\t<tr height=\"30px\">");
						
			#region Titre du tableau
			switch(webSession.PreformatedTable){
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_TimeSlice :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Sector :
					html.Append("<td class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(1141,webSession.SiteLanguage)+"</td>");
					break;
	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice :
					html.Append("<td class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(454,webSession.SiteLanguage)+"</td>");
					break;
	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Format :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_Mensual :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_NamedDay :
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.sector_X_TimeSlice :
					html.Append("<td align=\"center\" class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
					break;
			}
			#endregion
					
			for(int j=ResultConstantes.TOTAL_COLUMN_INDEX;j<tab.GetLength(1);j++){
				if(tab[0,j]!= null){
					html.Append("<td align=\"center\" class=\""+P2+"\" nowrap>"+tab[0,j].ToString()+"</td>");
				}	
			}
			html.Append("</tr>");
			#endregion

			if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
				nbCol=tab.GetLength(1)-ResultConstantes.TOTAL_COLUMN_INDEX;
			}else
				nbCol=tab.GetLength(1)-(ResultConstantes.TOTAL_COLUMN_INDEX+1);
						
			#region Parcour du Tableau
			//tableau			
			for (int i=1;i<tab.GetLength(0);i++){
	
				#region niveau1
				if(tab[i,ResultConstantes.ID_ELMT_L1_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX]==null){
					if (!excel){
						classCss="acl1";
					}else classCss="acl1";
								
					if(!label1){
                        html.Append("\r\n\t<tr align=\"right\"  class=\"whiteBackGround\" height=\"20px\" >\r\n\t\t<td align=\"left\"  class=\"" + classCss + "\" nowrap >" + tab[i, ResultConstantes.LABEL_ELMT_L1_COLUMN_INDEX].ToString() + "</td>");
						html.Append("<td class=\""+classCss+"\" nowrap colspan="+nbCol+" >&nbsp;</td>");
						html.Append("</tr>");
						label1=true;
					}
				}
				#endregion

				#region niveau2
				if(tab[i,ResultConstantes.ID_ELMT_L1_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX]==null){
					if(webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia){		
						if (!excel){
							classCss="asl5b";
							classCss2="asl5";
							colorClasses2="#E1E0DA";
						}else{
							classCss="acl11";
							classCss2="p142xls";
						}
					}
					else if(!excel){ 
						classCss="pmcategoryb";
						classCss2="insertionHeader";
						colorClasses2="#B1A3C1";
					}else {
						classCss="acl31";
						classCss2="acl21";
					}
	
					if(tempId==int.Parse(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString())){
						label2=true;
					}else label2=false;
							
					if(!label2){
						//html.Append("\r\n\t<tr align=\"right\"  height=\"20px\" >\r\n\t\t<td align=\"left\"  onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='"+colorClasses2+"';\"  bgcolor="+colorClasses2+"  class=\""+classCss2+"\" nowrap>"+space2+""+tab[i,ResultConstantes.LABEL_ELMT_L2_COLUMN_INDEX].ToString()+"</td>");
						//html.Append("<td onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='"+colorClasses2+"';\"  bgcolor="+colorClasses2+"  class=\""+classCss2+"\" nowrap colspan="+nbCol+" >&nbsp;</td>");
						//html.Append("</tr>");
						html.Append("\r\n\t<tr align=\"right\"  height=\"20px\" >\r\n\t\t<td align=\"left\"   class=\"" + classCss2 + "\" nowrap>" + space2 + "" + tab[i, ResultConstantes.LABEL_ELMT_L2_COLUMN_INDEX].ToString() + "</td>");
						html.Append("<td   class=\"" + classCss2 + "\" nowrap colspan=" + nbCol + " >&nbsp;</td>");
						html.Append("</tr>");
						tempId=int.Parse(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString());
					}
				}
				#endregion
	
				#region niveau3
				if(tab[i,ResultConstantes.ID_ELMT_L1_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]==null && tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX]!=null){
					if (!excel){
						classCss="asl5b";
						classCss3="asl5";
						colorClasses3="#E1E0DA";
					}else if (excel){
						classCss="acl11";
						classCss3="p142xls";
					}
					if(tempId==int.Parse(tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX].ToString())){
						label3=true;
					}else label3=false;
	
					if(!label3){
						//html.Append("\r\n\t<tr align=\"right\"  bgcolor=#ffffff height=\"20px\" >\r\n\t\t<td align=\"left\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='"+colorClasses3+"';\"  bgcolor="+colorClasses3+" class=\""+classCss3+"\" nowrap>"+space3+""+tab[i,ResultConstantes.LABEL_ELMT_L3_COLUMN_INDEX].ToString()+"</td>");
						//html.Append("<td onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='"+colorClasses3+"';\"  bgcolor="+colorClasses3+" class=\""+classCss3+"\" nowrap colspan="+nbCol+" >&nbsp;</td>");
						//html.Append("</tr>");
						html.Append("\r\n\t<tr align=\"right\"   height=\"20px\" >\r\n\t\t<td align=\"left\"  class=\"" + classCss3 + "\" nowrap>" + space3 + "" + tab[i, ResultConstantes.LABEL_ELMT_L3_COLUMN_INDEX].ToString() + "</td>");
						html.Append("<td  class=\"" + classCss3 + "\" nowrap colspan=" + nbCol + " >&nbsp;</td>");
						html.Append("</tr>");
						tempId=int.Parse(tab[i,ResultConstantes.ID_ELMT_L3_COLUMN_INDEX].ToString());
					}
				}
				#endregion	
	
				#endregion
	
			#region Eguillage
				if (tab[i,ResultConstantes.PERIOD_N_COLUMN_INDEX]!=null){
					html.Append("\r\n\t<tr height=\"20px\">");
					html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+tab[i,ResultConstantes.PERIOD_N_COLUMN_INDEX].ToString()+"</td>");
					CurrentPeriode(tab,webSession,html,i,classCss);
					html.Append("</tr>");
				}
				if (webSession.ComparativeStudy && tab[i,ResultConstantes.PERIOD_N1_COLUMN_INDEX]!=null){
					html.Append("\r\n\t<tr height=\"20px\">");
					html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+tab[i,ResultConstantes.PERIOD_N1_COLUMN_INDEX].ToString()+"</td>");	
					PrecedingPeriode(tab,webSession,html,i,classCss);
					html.Append("</tr>");
				}
				if (webSession.Evolution && tab[i,ResultConstantes.EVOL_COLUMN_INDEX]!=null){
					html.Append("\r\n\t<tr height=\"20px\">");
					html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+tab[i,ResultConstantes.EVOL_COLUMN_INDEX].ToString()+"</td>");	
					EvolutionPeriode(tab,webSession,html,i,classCss,excel);
					html.Append("</tr>");
				}										
				if (webSession.PDM && tab[i,ResultConstantes.PDM_COLUMN_INDEX]!=null){
					html.Append("\r\n\t<tr height=\"20px\">");
					html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+tab[i,ResultConstantes.PDM_COLUMN_INDEX].ToString()+"</td>");
					PDM(tab,webSession,html,i,classCss);
					html.Append("</tr>");
				}	
				if (webSession.PDV && tab[i,ResultConstantes.PDV_COLUMN_INDEX]!=null){
					html.Append("\r\n\t<tr height=\"20px\">");
					html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+tab[i,ResultConstantes.PDV_COLUMN_INDEX].ToString()+"</td>");
					PDV(tab,webSession,html,i,classCss);
					html.Append("</tr>");	
				}		
				#endregion 		
			}
			html.Append("</table>");	
			return html.ToString();
			#endregion
		}	
		#endregion

		#region periode courant		
		/// <summary>
		/// Méthode qui affiche la ligne de tableau periode de l'année courante 
		/// </summary>
		/// <param name="webSession">Session de l'utilisateur</param>
		///<param name="tab">Tableau d'objet</param>
		///<param name="html">Html</param>
		///<param name="i">Ligne du Tableau</param>
		///<param name="classCss">ClasCss</param>
		/// <returns>UI du tableau</returns>	
		private static string CurrentPeriode(object[,] tab,WebSession webSession, StringBuilder html ,int i,string classCss){	
			
			string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(Int64.Parse(Vehicle));
			
			#region  colonne Totale
			if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
				if(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX]!=null){					
					if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null)
						html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ##0.##")+" %</td>");
					else 
						//unité duration
					
						if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format|| 
						webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay|| 
						webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice){
						if(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString()=="2"){
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString(),CstWeb.CustomerSessions.Unit.duration,false)+"</td>");
						}else 
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
					}else 
						html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
				}else
					html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");				
			}			
			#endregion

			for(int j=ResultConstantes.TOTAL_COLUMN_INDEX+1;j<tab.GetLength(1);j++){
				
				#region TV et RADIO
				//si les medias sélectionnés sont different de TV ou RADIO
				if(vehicleType!=ClassificationCst.DB.Vehicles.names.press){
					
					#region tableau n°1 
					//cas de tableau n°1:media/unite
					if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
						if(tab[i,j]!= null){
							//afficher la durée en h/m/s pour le tableau n°1:media/unite
							if (j==ResultConstantes.TOTAL_COLUMN_INDEX+2){
								if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null){
									html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
								}
								else	html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),CstWeb.CustomerSessions.Unit.duration)+"</td>");
							}
							else 
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");							
						}	else
							html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");						
					}
						#endregion

						#region tableaux de 2 à 11
					else 
						if(tab[i,j]!= null){						
						if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null)
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
						else 
							//unité duration
							if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format|| 
							webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay|| 
							webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice){
							if(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString()=="2"){
//								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+GetDurationFormatHH_MM_SS(int.Parse(tab[i,j].ToString()))+"</td>");
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),CstWeb.CustomerSessions.Unit.duration,false)+"</td>");
							}else 
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");
						}else 
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");
					}else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
					#endregion
					#endregion									

					#region Presse
					// si le media sélectionné est PRESSE
				}else if(vehicleType==ClassificationCst.DB.Vehicles.names.press){

					#region tableau n°1
					if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
						if(tab[i,j]!= null){

							#region Colonne Page
							if (j==ResultConstantes.TOTAL_COLUMN_INDEX+3){
								if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null){
									html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
								}
								else	html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),CstWeb.CustomerSessions.Unit.pages)+"</td>");
							}
							else 
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");							
						}else
							html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
					}
						#endregion
						#endregion

						#region tableau 2-11
					else 
						if(tab[i,j]!= null){						
						if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null)
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
						else 
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");
					}else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
				}
				#endregion

				#endregion
			}
			return html.ToString();
		}
		#endregion

		#region Periode precedent
		/// <summary>
		/// Méthode qui affiche la lignes de tableau période de l'année dernière
		/// </summary>
		///<param name="tab">Tableau d'objet</param>
		///<param name="webSession">Websession</param>
		///<param name="html">Html</param>
		///<param name="i">Ligne du Tableau</param>
		///<param name="classCss">ClasCss</param>
		/// <returns>UI du tableau</returns>		
		private static string PrecedingPeriode(object[,] tab,WebSession webSession, StringBuilder html ,int i,string classCss){
	
			string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
			ClassificationCst.DB.Vehicles.names vehicleType = VehiclesInformation.DatabaseIdToEnum(Int64.Parse(Vehicle));
			
			#region  colonne Totale
			if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
				if(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX]!=null){					
					if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null)
						html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ##0.##")+" %</td>");
					else 
						//unité duration
					
						if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format|| 
						webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay|| 
						webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice){
						if(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString()=="2"){
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString(),CstWeb.CustomerSessions.Unit.duration,false)+"</td>");
						}else 
						html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
						
					}else 
						html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit)+"</td>");
				}else
					html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");				
			}			
			#endregion

			for(int j=ResultConstantes.TOTAL_COLUMN_INDEX+1;j<tab.GetLength(1);j++){
				
				#region TV et RADIO
				//si les medias sélectionnés sont different de TV ou RADIO
				if(vehicleType!=ClassificationCst.DB.Vehicles.names.press){
					
					#region tableau n°1 
					//cas de tableau n°1:media/unite
					if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
						if(tab[i,j]!= null){
							//afficher la durée en h/m/s pour le tableau n°1:media/unite
							if (j==ResultConstantes.TOTAL_COLUMN_INDEX+2){
								if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null){
									html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
								}
								else	html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),CstWeb.CustomerSessions.Unit.duration)+"</td>");
							}
							else 
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");							
						}	else
							html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");						
					}
						#endregion

						#region tableaux de 2 à 11
					else 
						if(tab[i,j]!= null){						
						if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null)
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
						else 
							//unité duration
							if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_Format|| 
							webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_NamedDay|| 
							webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.units_X_TimeSlice){
							if(tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX]!=null && tab[i,ResultConstantes.ID_ELMT_L2_COLUMN_INDEX].ToString()=="2"){
//								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+GetDurationFormatHH_MM_SS(int.Parse(tab[i,j].ToString()))+"</td>");
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),CstWeb.CustomerSessions.Unit.duration,false)+"</td>");
							}else 
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");
						
						}else 
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");
					}else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
					#endregion
					#endregion									

				#region Presse
					// si le media sélectionné est PRESSE
				}else if(vehicleType==ClassificationCst.DB.Vehicles.names.press){

					#region tableau n°1
					if(webSession.PreformatedTable==CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
						if(tab[i,j]!= null){

							#region Colonne Page
							if (j==ResultConstantes.TOTAL_COLUMN_INDEX+3){
								if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null){
									html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
								}
								else	html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),CstWeb.CustomerSessions.Unit.pages)+"</td>");
							}
							else 
								html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");							
						}else
							html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
					}
						#endregion
						#endregion

						#region tableau 2-11
					else 
						if(tab[i,j]!= null){						
						if(tab[i,ResultConstantes.REPARTITION_PERCENT]!=null)
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
						else 
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j].ToString(),webSession.Unit)+"</td>");
					}else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
				}
				#endregion

				#endregion
			}
			return html.ToString();
		}
		#endregion

		#region Evolution
		/// <summary>
		/// Méthode d'affichage de la ligne de tableau évolution
		///</summary>
		///<param name="tab">Tableau d'objet</param>
		///<param name="webSession">webSession</param>
		///<param name="html">Html</param>
		///<param name="i">Ligne du Tableau</param>
		///<param name="classCss">ClasCss</param>
		///<param name="excel">Excel</param>
		/// <returns>UI du tableau</returns>
		private static string EvolutionPeriode(object[,] tab,WebSession webSession, StringBuilder html ,int i,string classCss,bool excel ){
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			// affichage de la colonne total pour tous les types de tableaux sauf Media/Unité
			#region Evolution pour Html
			if(!excel){
				if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
					if(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX]!=null){

						if (double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString())>0) //hausse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()))) ? double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/g.gif></td>");		
						else if (double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString())<0) //baisse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()))) ? double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/r.gif></td>");						
						else if (!Double.IsNaN(double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()))) // 0 exactement
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap> 0 %<img src=/I/o.gif></td>");
					}else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
				}

				for(int j=ResultConstantes.TOTAL_COLUMN_INDEX+1;j<tab.GetLength(1);j++){
					if(tab[i,j]!= null){
						if (double.Parse(tab[i,j].ToString())>0) //hausse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,j].ToString()))) ? double.Parse(tab[i,j].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/g.gif></td>");		
						else if (double.Parse(tab[i,j].ToString())<0) //baisse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,j].ToString()))) ? double.Parse(tab[i,j].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "<img src=/I/r.gif></td>");						
						else if (!Double.IsNaN(double.Parse(tab[i,j].ToString()))) // 0 exactement
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap> 0 %<img src=/I/o.gif></td>");
					}
					else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
				}
			}
				#endregion

			#region Evolution pour Excel
			else if(excel){
				if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
					if(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX]!=null){
						
						if (double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString())>0) //hausse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()))) ? double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "</td>");
						else if(double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString())<0) //baisse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()))) ? double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "</td>");				
						else if (!Double.IsNaN(double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()))) // 0 exactement
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap> 0 %</td>");
					}else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
				}

				for(int j=ResultConstantes.TOTAL_COLUMN_INDEX+1;j<tab.GetLength(1);j++){
					if(tab[i,j]!= null){
						if (double.Parse(tab[i,j].ToString())>0) //hausse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,j].ToString()))) ? double.Parse(tab[i,j].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "</td>");		
						else if (double.Parse(tab[i,j].ToString())<0) //baisse
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>" + ( (!Double.IsInfinity(double.Parse(tab[i,j].ToString()))) ? double.Parse(tab[i,j].ToString()).ToString("# ### ##0.##")+" %" : "" ) + "</td>");						
						else if (!Double.IsNaN(double.Parse(tab[i,j].ToString()))) // 0 exactement
							html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap> 0 %</td>");
					}
					else
						html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
				}
			}
			#endregion

			return html.ToString();
		}
		#endregion
		
		#region PDM
		/// <summary>
		/// Méthode d'affichage de la ligne de tableau PDM
		/// </summary>
		///<param name="tab">Tableau d'objet</param>
		///<param name="webSession">webSession</param>
		///<param name="html">Html</param>
		///<param name="i">Ligne du Tableau</param>
		///<param name="classCss">ClasCss</param>
		/// <returns>UI du tableau</returns>
		private static string PDM(object[,] tab,WebSession webSession, StringBuilder html , int i,string classCss){	

			// affichage de la colonne total pour tous les types de tableaux sauf Media/Unité
			if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
				if(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX]!=null){
				//	html.Append("<td align=\"right\" class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ##0.##")+"%</td>");
						html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString((tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()),webSession.Unit,true)+" %</td>");	
				}else
					html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
			}
			for(int j=ResultConstantes.TOTAL_COLUMN_INDEX+1;j<tab.GetLength(1);j++){
				if(tab[i,j]!= null){
				//	html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");	
			html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString((tab[i,j].ToString()),webSession.Unit,true)+" %</td>");	
					
				}else
					html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
			}
			return html.ToString();
		}
		
		#endregion

		#region PDV
		/// <summary>
		/// Méthode d'affichage de la ligne de tableau PDV
		/// </summary>
		///<param name="tab">Tableau d'objet</param>
		///<param name="webSession">webSession</param>
		///<param name="html">Html</param>
		///<param name="i">Ligne du Tableau</param>
		///<param name="classCss">ClasCss</param>
		/// <returns>UI du tableau</returns>
		private static string PDV(object[,] tab,WebSession webSession, StringBuilder html , int i,string classCss){
			
			// affichage de la colonne total pour tous les types de tableaux sauf Media/Unité
			if (webSession.PreformatedTable!=CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units){
				if(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX]!=null){
					html.Append("<td align=\"right\" class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+double.Parse(tab[i,ResultConstantes.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ##0.##")+"%</td>");
				}else
					html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
			}
			for(int j=ResultConstantes.TOTAL_COLUMN_INDEX+1;j<tab.GetLength(1);j++){
				if(tab[i,j]!= null){
					html.Append("<td align=\"right\" class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ### ##0.##")+" %</td>");
				}
				else
					html.Append("<td align=\"center\" class=\""+classCss+"\" nowrap>  </td>");
			}
			return html.ToString();	
		}
		#endregion		

		#region Fréquence de livraison des données invalide
		/// <summary>
		/// Fréquence de livraison des données invalide
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML</returns>
		private static string UnvalidFrequencyDelivery(WebSession webSession){
			return "<div align=\"center\" class=\"txtViolet11Bold\"><br><br>"+GestionWeb.GetWebWord(1234,webSession.SiteLanguage) + "<br><br><br></div>";
		}
		#endregion

		#endregion
        
	}
}

		