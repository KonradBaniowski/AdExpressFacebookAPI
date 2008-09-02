#region Informations
// Auteur: K. Shehzad 
// Date de création: 13/07/2005 
#endregion
using System;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Date;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using SQLFunctions=TNS.AdExpress.Web.DataAccess;
using WebFnc = TNS.AdExpress.Web.Functions;
using WebCste = TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Units;
namespace TNS.AdExpress.Web.Rules.Results.APPM{
	/// <summary>
	/// Provides formatting rules for APPM Synthesis
	/// </summary>
	public class SynthesisRules{

		#region APPM synthesis 
		/// <summary>
		/// Formats the date for APPM synthesis 
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="idProduct">ID of the product selected from the products dropdownList </param>
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		/// <returns>HTML string for the synthesis table</returns>		
		public static Hashtable GetData(WebSession webSession,IDataSource dataSource,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,Int64 idProduct,bool mediaAgencyAccess){
			
			#region variables
			Hashtable synthesisData=null;
			DataTable synthesisTable=null;
			DataTable synthesisPDV=null;
			string startDate=string.Empty;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;
			string endDate=string.Empty;
			string oldGroup=string.Empty;
			string groups=string.Empty;
			string groupIds=string.Empty;
			Int64 budget=0;
			Int64 universgroupInvestment=0;
			int insertions=0;
			int numberOfMedias=0;
			double pdv=0;
			double totalGRP=0;
			double baseTargetGRP=0;
			double additionalTargetGRP=0;
			double additionalTargetCost=0;
			double pages=0;
			#endregion			

			try{

				#region get Data
			 	
				synthesisTable=TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,idProduct,mediaAgencyAccess).Tables[0];
				#endregion

				if(synthesisTable!=null && synthesisTable.Rows.Count>0){
					
					#region date formatting
					//Getting the date in the format yyyyMMdd
					int dateBeginning = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
					int dateEnding = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
					//Formatting the date in the format e.g 12/12/2005
					if(dateBeginning.ToString().Length>0)
						startDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateBeginning.ToString().Substring(0,4)),int.Parse(dateBeginning.ToString().Substring(4,2)),int.Parse(dateBeginning.ToString().Substring(6,2))),webSession.SiteLanguage);						
//						startDate=dateBeginning.ToString().Substring(6,2)+"/"+dateBeginning.ToString().Substring(4,2)+"/"+dateBeginning.ToString().Substring(0,4);
					if(dateEnding.ToString().Length>0)
						endDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateEnding.ToString().Substring(0,4)),int.Parse(dateEnding.ToString().Substring(4,2)),int.Parse(dateEnding.ToString().Substring(6,2))),webSession.SiteLanguage);												
//						endDate=dateEnding.ToString().Substring(6,2)+"/"+dateEnding.ToString().Substring(4,2)+"/"+dateEnding.ToString().Substring(0,4);
					#endregion															
									
					#region construction of synthesis hashtable
					synthesisData = new Hashtable();
					//idProduct 0 refers to the whole univers so if idProduct is not 0 so we will show the following lines
					//for a specific product
					if(idProduct!=0){
						//Nom du produit
						synthesisData.Add("product", synthesisTable.Rows[0]["product"]);
						//Nom de l'annonceur
						synthesisData.Add("advertiser", synthesisTable.Rows[0]["advertiser"]);
						//Nom d'Agence Média
						if(mediaAgencyAccess)
							synthesisData.Add("agency", synthesisTable.Rows[0]["advertising_agency"]);						
					}
					// Période d'analyse
					synthesisData.Add("dateBegin",startDate);
					synthesisData.Add("dateEnd",endDate);	
				
					#region traversing the table
					foreach(DataRow dr in synthesisTable.Rows){
						//Values for the base target
						if(Convert.ToInt64(dr["id_target"])==baseTarget){
							baseTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
							budget+=Convert.ToInt64(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString());
							targetBase=dr["target"].ToString();
                            insertions += Convert.ToInt32(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
							//numberOfMedias+=Convert.ToInt32(dr["medias"].ToString());
                            pages += Convert.ToDouble(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.pages].Id.ToString()]);
						}
						//values for the supplementary target
						else{
							additionalTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
							targetSelected=dr["target"].ToString();	
						}
						totalGRP+=Convert.ToDouble(dr["totalgrp"].ToString());					
                        
					}					
					#endregion
					
					#region getting group ids and groups
					//to select distinct groupd ids and groups using the SelectDistinct function
					string[] groupArray={"id_group_","group_"};
					DataTable groupsTable=SQLFunctions.Functions.SelectDistinct("groups",synthesisTable,groupArray);
					foreach(DataRow dr in groupsTable.Rows){
						groupIds+=dr["id_group_"].ToString()+",";
						groups+=dr["group_"].ToString()+",";
					}
					#endregion

					#region getting number of media
					DataTable mediaTable=SQLFunctions.Functions.SelectDistinct("media",synthesisTable,"id_media");
					if(mediaTable!=null)
						numberOfMedias=mediaTable.Rows.Count;
					#endregion

					//Secteur de référence 
					synthesisData.Add("group", groups.Remove(groups.Length-1,1));					
					//budget brut (euros)
					synthesisData.Add("budget",budget.ToString());
                    //nombre d'insertions
					synthesisData.Add("insertions",insertions.ToString());
					//nombre de pages utilisés
					pages=Math.Round(pages/1000,3);
					synthesisData.Add("pages",pages.ToString());
					//nombre de supports utilisés
					synthesisData.Add("supports",numberOfMedias.ToString());

					//Part de voix de la campagne
					#region group investment for PDV
					//UniversGroupInvestment method in the SynthesisDataAccess class will return the budegt of the groups of the products selected
					//or of the competitor univers if it was selected. 
					synthesisPDV=TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.UniversGroupInvestment(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,groupIds.Remove(groupIds.Length-1,1)).Tables[0];
                    if (synthesisPDV != null && synthesisPDV.Rows.Count > 0 && !synthesisPDV.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString().Equals("")) {
                        universgroupInvestment = Convert.ToInt64(synthesisPDV.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]);					
						//if the competitor univers was selected we will add its bugdet to the budget of the reference univers
						//to calculate the PDV
						//if(webSession.CompetitorUniversAdvertiser.Count>1){
						if(webSession.PrincipalProductUniverses.Count>1){
							universgroupInvestment=universgroupInvestment+budget;
						}
					}
					#endregion
					if(universgroupInvestment!=0){
						pdv=Math.Round(Convert.ToDouble(budget)/Convert.ToDouble(universgroupInvestment)*100,2);
						synthesisData.Add("PDV",pdv.ToString());
					}
					else{
						synthesisData.Add("PDV","");
					}
					// cible selectionnée
					//string targetSelected =((LevelInformation)webSession.SelectionUniversAEPMTarget.FirstNode.Tag).ID.ToString();
					synthesisData.Add("targetSelected",targetSelected);
                    //cible de base
					synthesisData.Add("baseTarget",targetBase);
					//nombre de GRP(cible selectionnée)
					additionalTargetGRP=Math.Round(additionalTargetGRP,3);
					synthesisData.Add("GRPNumber",additionalTargetGRP.ToString());
					//nombre de GRP(cible 15 ans et +)
					synthesisData.Add("GRPNumberBase",Math.Round(baseTargetGRP, 3).ToString());
					//Indice GRP vs cible 15 ans à +
					if(baseTargetGRP>0){
						synthesisData.Add("IndiceGRP",Math.Round((additionalTargetGRP/baseTargetGRP)*100,3).ToString());
					}
					else{
						synthesisData.Add("IndiceGRP","");
					}
					// Coût GRP(cible selectionnée)
					//additionalTargetCost=Math.Round(additionalTargetCost,3);
					if(additionalTargetGRP>0){
						additionalTargetCost=Math.Round(budget/additionalTargetGRP,3);
						synthesisData.Add("GRPCost",additionalTargetCost.ToString());
					}
					else{
						synthesisData.Add("GRPCost","");
					}
					// Coût GRP(cible 15 et +)
					if(baseTargetGRP>0){
						synthesisData.Add("GRPCostBase",Math.Round(budget/baseTargetGRP,3).ToString());
					}
					else{
						synthesisData.Add("GRPCostBase","");
					}
					//Indice coût GRP vs cible 15 ans à +
					if(additionalTargetGRP>0 && baseTargetGRP>0){
						synthesisData.Add("IndiceGRPCost",Math.Round(((budget/additionalTargetGRP)/(budget/baseTargetGRP))*100,3).ToString());
					}
					else{
						synthesisData.Add("IndiceGRPCost","");
					}
					//synthesisData.Add("IndiceGRPCost",Math.Round((additionalTargetCost/baseTargetCost)*100,3).ToString());
					#endregion
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.SynthesisRulesException("Error while formatting the data for APPM Synthesis ",err));
			}

			#region Recharging the univers
			//charging the reference univers
			//webSession.CurrentUniversAdvertiser=(TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;				
			#endregion

			return synthesisData;
		}
		#endregion

		#region APPM synthesis bY Version
		/// <summary>
		///	Get APPM synthesis data result for one Version
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>		
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		///<param name="idVersion">ID Version</param>
		///<param name="firstInsertionDate">First insertion date</param>
		/// <returns>HTML string for the synthesis table</returns>		
		public static Hashtable GetData(WebSession webSession,IDataSource dataSource,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,bool mediaAgencyAccess,string idVersion,string firstInsertionDate){
			#region variables
			Hashtable synthesisData=null;
			DataTable synthesisTable=null;
			DataTable synthesisPDV=null,investProduct=null;
			
			string startDate=string.Empty;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;
			string endDate=string.Empty;
			string oldGroup=string.Empty;
			string groups=string.Empty;
			string groupIds=string.Empty;
			Int64 budget=0;
			Int64 universgroupInvestment=0;
			Int64 tempInvestProduct=0;
			int insertions=0;
			int numberOfMedias=0;
			double pdv=0;
			double totalGRP=0;
			double baseTargetGRP=0;
			double additionalTargetGRP=0;
			double additionalTargetCost=0;
			double pages=0;
			#endregion			

			if(webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)){//Droits versions
				try{
					
					#region date formatting
					//Getting the date in the format yyyyMMdd
					int dateBeginning = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
					int dateEnding = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
					//Formatting the date in the format e.g 12/12/2005
					if(dateBeginning.ToString().Length>0)
						startDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateBeginning.ToString().Substring(0,4)),int.Parse(dateBeginning.ToString().Substring(4,2)),int.Parse(dateBeginning.ToString().Substring(6,2))),webSession.SiteLanguage);						
					if(dateEnding.ToString().Length>0)
						endDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateEnding.ToString().Substring(0,4)),int.Parse(dateEnding.ToString().Substring(4,2)),int.Parse(dateEnding.ToString().Substring(6,2))),webSession.SiteLanguage);												
					#endregion	

					#region get Data
					if(idVersion!=null && idVersion.Length>0)
						synthesisTable=TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.GetData(webSession,dataSource,dateBeginning,dateEnding,baseTarget,additionalTarget,mediaAgencyAccess,idVersion).Tables[0];
					#endregion

					if(synthesisTable!=null && synthesisTable.Rows.Count>0){
																		
									
						#region construction of synthesis hashtable
						synthesisData = new Hashtable();

						//Version selectionné
						synthesisData.Add("version",synthesisTable.Rows[0]["id_slogan"]);

						//idProduct 0 refers to the whole univers so if idProduct is not 0 so we will show the following lines
						//for a specific product
						//					if(idProduct!=0){

						//Product Label
						synthesisData.Add("product", synthesisTable.Rows[0]["product"]);

						//Brand Label
						//Rights verification for Brand
						if(webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE))
							synthesisData.Add("brand", synthesisTable.Rows[0]["brand"]);

						//Advertiser Label
						synthesisData.Add("advertiser", synthesisTable.Rows[0]["advertiser"]);

						//Agency media Label
						if(mediaAgencyAccess )
							synthesisData.Add("agency", synthesisTable.Rows[0]["advertising_agency"]);						
						//					}

						// Période d'analyse
						synthesisData.Add("dateBegin",startDate);
						synthesisData.Add("dateEnd",endDate);
	
						//Date de 1ere insertion de la version
						if(firstInsertionDate!=null && firstInsertionDate.Length>0)
							synthesisData.Add("firstInsertionDate",DateString.YYYYMMDDToDD_MM_YYYY(firstInsertionDate,webSession.SiteLanguage));
				
						#region traversing the table
						foreach(DataRow dr in synthesisTable.Rows){
							//Values for the base target
							if(Convert.ToInt64(dr["id_target"])==baseTarget){
								baseTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
                                budget += Convert.ToInt64(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString());
								targetBase=dr["target"].ToString();
                                insertions += Convert.ToInt32(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
								//numberOfMedias+=Convert.ToInt32(dr["medias"].ToString());
                                pages += Convert.ToDouble(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.pages].Id.ToString()]);
							}
								//values for the supplementary target
							else{
								additionalTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
								targetSelected=dr["target"].ToString();	
							}
							totalGRP+=Convert.ToDouble(dr["totalgrp"].ToString());					
                        
						}					
						#endregion
					
						#region getting group ids and groups
						//to select distinct group ids and groups using the SelectDistinct function
						string[] groupArray={"id_group_","group_"};
						DataTable groupsTable=SQLFunctions.Functions.SelectDistinct("groups",synthesisTable,groupArray);
						foreach(DataRow dr in groupsTable.Rows){
							groupIds+=dr["id_group_"].ToString()+",";
							groups+=dr["group_"].ToString()+",";
						}
						#endregion

						#region getting number of media
						DataTable mediaTable=SQLFunctions.Functions.SelectDistinct("media",synthesisTable,"id_media");
						if(mediaTable!=null)
							numberOfMedias=mediaTable.Rows.Count;
						#endregion

						//Secteur de référence 
						synthesisData.Add("group", groups.Remove(groups.Length-1,1));
					
						//budget brut (euros)
						synthesisData.Add("budget",budget.ToString());

						//nombre d'insertions
						synthesisData.Add("insertions",insertions.ToString());

						//nombre de pages utilisés
						pages=Math.Round(pages/1000,3);
						synthesisData.Add("pages",pages.ToString());

						//nombre de supports utilisés
						synthesisData.Add("supports",numberOfMedias.ToString());

						//Part de voix de la campagne
						#region group investment for PDV
						//UniversGroupInvestment method in the SynthesisDataAccess class will return the budegt of the groups of the products selected
						//or of the competitor univers if it was selected. 
						synthesisPDV=TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.UniversGroupInvestment(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,groupIds.Remove(groupIds.Length-1,1)).Tables[0];
                        if (synthesisPDV != null && synthesisPDV.Rows.Count > 0 && !synthesisPDV.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString().Equals("")) {
                            universgroupInvestment = Convert.ToInt64(synthesisPDV.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]);					
							//if the competitor univers was selected we will add its bugdet to the budget of the reference univers
							//to calculate the PDV
							//if(webSession.CompetitorUniversAdvertiser.Count>1){
							if(webSession.PrincipalProductUniverses.Count>1){
								universgroupInvestment=universgroupInvestment+budget;
							}
						}
						#endregion
							
						if(universgroupInvestment!=0){
							pdv=Math.Round(Convert.ToDouble(budget)/Convert.ToDouble(universgroupInvestment)*100,2);
							synthesisData.Add("PDV",pdv.ToString());
						}
						else{
							synthesisData.Add("PDV","");
						}

						//Poids de la version vs produit correspondant
						if( synthesisTable.Rows[0]["id_product"]!=System.DBNull.Value){
							investProduct = TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.GetProductInvestment(webSession,dataSource,dateBegin,dateEnd,baseTarget,synthesisTable.Rows[0]["id_product"].ToString()).Tables[0];
                            if (investProduct != null && investProduct.Rows.Count > 0 && !investProduct.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString().Equals("")) {
                                tempInvestProduct = Convert.ToInt64(investProduct.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]);	
								if(tempInvestProduct>0){
									double versionWeight=Math.Round(Convert.ToDouble(budget)/Convert.ToDouble(tempInvestProduct)*100,2);
									synthesisData.Add("versionWeight",versionWeight.ToString());
								}
								else{
									synthesisData.Add("versionWeight","");
								}

							}
						}
				
						// cible selectionnée
						//string targetSelected =((LevelInformation)webSession.SelectionUniversAEPMTarget.FirstNode.Tag).ID.ToString();
						synthesisData.Add("targetSelected",targetSelected);

						//cible de base
						synthesisData.Add("baseTarget",targetBase);

						//nombre de GRP(cible selectionnée)
						additionalTargetGRP=Math.Round(additionalTargetGRP,3);
						synthesisData.Add("GRPNumber",additionalTargetGRP.ToString());

						//nombre de GRP(cible 15 ans et +)
						synthesisData.Add("GRPNumberBase",Math.Round(baseTargetGRP, 3).ToString());

						//Indice GRP vs cible 15 ans à +
						if(baseTargetGRP>0){
							synthesisData.Add("IndiceGRP",Math.Round((additionalTargetGRP/baseTargetGRP)*100,3).ToString());
						}
						else{
							synthesisData.Add("IndiceGRP","");
						}
						// Coût GRP(cible selectionnée)
						//additionalTargetCost=Math.Round(additionalTargetCost,3);
						if(additionalTargetGRP>0){
							additionalTargetCost=Math.Round(budget/additionalTargetGRP,3);
							synthesisData.Add("GRPCost",additionalTargetCost.ToString());
						}
						else{
							synthesisData.Add("GRPCost","");
						}

						// Coût GRP(cible 15 et +)
						if(baseTargetGRP>0){
							synthesisData.Add("GRPCostBase",Math.Round(budget/baseTargetGRP,3).ToString());
						}
						else{
							synthesisData.Add("GRPCostBase","");
						}

						//Indice coût GRP vs cible 15 ans à +
						if(additionalTargetGRP>0 && baseTargetGRP>0){
							synthesisData.Add("IndiceGRPCost",Math.Round(((budget/additionalTargetGRP)/(budget/baseTargetGRP))*100,3).ToString());
						}
						else{
							synthesisData.Add("IndiceGRPCost","");
						}

						//synthesisData.Add("IndiceGRPCost",Math.Round((additionalTargetCost/baseTargetCost)*100,3).ToString());
						#endregion
					}
				}
				catch(System.Exception err){
					throw(new WebExceptions.SynthesisRulesException("Error while formatting the data for APPM Synthesis by Version ",err));
				}

				#region Recharging the univers
				//charging the reference univers
				//webSession.CurrentUniversAdvertiser=(TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;				
				#endregion
			}

			return synthesisData;
		}
		#endregion 

		#region APPM synthesis By List of Versions
		/// <summary>
		///	Get APPM synthesis data result for a List of Versions
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>		
		/// <param name="mediaAgencyAccess">A flag that indicates whether the client has access to Media Agency or no</param>
		///<param name="versions">List of Versions ID</param>
		/// <returns>HTML string for the synthesis table</returns>		
		public static Hashtable[] GetData(WebSession webSession,IDataSource dataSource,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,bool mediaAgencyAccess,ICollection versions) {

			#region variables
			//Hashtable synthesisData=null;
			Hashtable[] synthesisData = new Hashtable[versions.Count];
			int k=0;
			DataTable synthesisTable=null;
			DataTable synthesisTableTmp = null; 
			DataTable synthesisPDV=null,investProduct=null;
			
			string startDate=string.Empty;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;
			string endDate=string.Empty;
			string oldGroup=string.Empty;
			string groups=string.Empty;
			string groupIds=string.Empty;
			Int64 budget=0;
			Int64 universgroupInvestment=0;
			Int64 tempInvestProduct=0;
			int insertions=0;
			int numberOfMedias=0;
			double pdv=0;
			double totalGRP=0;
			double baseTargetGRP=0;
			double additionalTargetGRP=0;
			double additionalTargetCost=0;
			double pages=0;
			#endregion			

			if(webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)) {
				//Droits versions
				try {
					
					#region date formatting
					//Getting the date in the format yyyyMMdd
					int dateBeginning = int.Parse(WebFnc.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
					int dateEnding = int.Parse(WebFnc.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
					//Formatting the date in the format e.g 12/12/2005
					if(dateBeginning.ToString().Length>0)
						startDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateBeginning.ToString().Substring(0,4)),int.Parse(dateBeginning.ToString().Substring(4,2)),int.Parse(dateBeginning.ToString().Substring(6,2))),webSession.SiteLanguage);						
					if(dateEnding.ToString().Length>0)
						endDate = WebFnc.Dates.dateToString(new DateTime(int.Parse(dateEnding.ToString().Substring(0,4)),int.Parse(dateEnding.ToString().Substring(4,2)),int.Parse(dateEnding.ToString().Substring(6,2))),webSession.SiteLanguage);												
					#endregion	

					#region get Data
					if(versions!=null && versions.Count>0)
						synthesisTable=TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.GetData(webSession,dataSource,dateBeginning,dateEnding,baseTarget,additionalTarget,mediaAgencyAccess,versions).Tables[0];
					#endregion

					#region get Synthèse pour chaque version
								
					foreach(Object version in versions) {

						#region Initialisation des variables
						synthesisPDV=null;
						investProduct=null;
						startDate=string.Empty;
						targetSelected=string.Empty;
						targetBase=string.Empty;
						endDate=string.Empty;
						oldGroup=string.Empty;
						groups=string.Empty;
						groupIds=string.Empty;
						budget=0;
						universgroupInvestment=0;
						tempInvestProduct=0;
						insertions=0;
						numberOfMedias=0;
						pdv=0;
						totalGRP=0;
						baseTargetGRP=0;
						additionalTargetGRP=0;
						additionalTargetCost=0;
						pages=0;
						#endregion
						
						synthesisTableTmp = new DataTable();

						for(int i=0; i<synthesisTable.Columns.Count; i++)
							synthesisTableTmp.Columns.Add(synthesisTable.Columns[i].Caption, synthesisTable.Columns[i].DataType);
						
						DataRow[] foundRows = synthesisTable.Select("id_slogan = "+version.ToString());

						foreach(DataRow row in foundRows) {
							
							DataRow nRow= synthesisTableTmp.NewRow();
							
							for(int j=0; j<synthesisTable.Columns.Count; j++)
								nRow[synthesisTable.Columns[j].Caption] = row[synthesisTable.Columns[j].Caption];

							synthesisTableTmp.Rows.Add(nRow);
						}
					
						if(synthesisTableTmp!=null && synthesisTableTmp.Rows.Count>0) {
									
							#region construction of synthesis hashtable
							synthesisData[k] = new Hashtable();

							//Version selectionné
							synthesisData[k].Add("version",synthesisTableTmp.Rows[0]["id_slogan"]);

							//idProduct 0 refers to the whole univers so if idProduct is not 0 so we will show the following lines
							//for a specific product
							//					if(idProduct!=0){

							//Product Label
							synthesisData[k].Add("product", synthesisTableTmp.Rows[0]["product"]);

							//Brand Label
							//Rights verification for Brand
							if(webSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE))
								synthesisData[k].Add("brand", synthesisTableTmp.Rows[0]["brand"]);

							//Advertiser Label
							synthesisData[k].Add("advertiser", synthesisTableTmp.Rows[0]["advertiser"]);

							//Agency media Label
							if(mediaAgencyAccess )
								synthesisData[k].Add("agency", synthesisTableTmp.Rows[0]["advertising_agency"]);						
							//					}

							// Période d'analyse
							synthesisData[k].Add("dateBegin",startDate);
							synthesisData[k].Add("dateEnd",endDate);
	
							//Date de 1ere insertion de la version
							//						if(firstInsertionDate!=null && firstInsertionDate.Length>0)
							//							synthesisData.Add("firstInsertionDate",DateString.YYYYMMDDToDD_MM_YYYY(firstInsertionDate,webSession.SiteLanguage));
				
							#region traversing the table
							foreach(DataRow dr in synthesisTableTmp.Rows) 
							{
								//Values for the base target
								if(Convert.ToInt64(dr["id_target"])==baseTarget) 
								{
									baseTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
                                    budget += Convert.ToInt64(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString());
									targetBase=dr["target"].ToString();
                                    insertions += Convert.ToInt32(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
									//numberOfMedias+=Convert.ToInt32(dr["medias"].ToString());
                                    pages += Convert.ToDouble(dr[UnitsInformation.List[WebCste.CustomerSessions.Unit.pages].Id.ToString()]);
								}
									//values for the supplementary target
								else 
								{
									additionalTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
									targetSelected=dr["target"].ToString();	
								}
								totalGRP+=Convert.ToDouble(dr["totalgrp"].ToString());					
                        
							}					
							#endregion
					
							#region getting group ids and groups
							//to select distinct group ids and groups using the SelectDistinct function
							string[] groupArray={"id_group_","group_"};
							DataTable groupsTable=SQLFunctions.Functions.SelectDistinct("groups",synthesisTableTmp,groupArray);
							foreach(DataRow dr in groupsTable.Rows) 
							{
								groupIds+=dr["id_group_"].ToString()+",";
								groups+=dr["group_"].ToString()+",";
							}
							#endregion

							#region getting number of media
							DataTable mediaTable=SQLFunctions.Functions.SelectDistinct("media",synthesisTableTmp,"id_media");
							if(mediaTable!=null)
								numberOfMedias=mediaTable.Rows.Count;
							#endregion

							//Secteur de référence 
							synthesisData[k].Add("group", groups.Remove(groups.Length-1,1));
					
							//budget brut (euros)
							synthesisData[k].Add("budget",budget.ToString());

							//nombre d'insertions
							synthesisData[k].Add("insertions",insertions.ToString());

							//nombre de pages utilisés
							pages=Math.Round(pages/1000,3);
							synthesisData[k].Add("pages",pages.ToString());

							//nombre de supports utilisés
							synthesisData[k].Add("supports",numberOfMedias.ToString());

							//Part de voix de la campagne
							#region group investment for PDV
							//UniversGroupInvestment method in the SynthesisDataAccess class will return the budegt of the groups of the products selected
							//or of the competitor univers if it was selected. 
							synthesisPDV=TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.UniversGroupInvestment(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,groupIds.Remove(groupIds.Length-1,1)).Tables[0];
                            if (synthesisPDV != null && synthesisPDV.Rows.Count > 0 && !synthesisPDV.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString().Equals("")) 
							{
                                universgroupInvestment = Convert.ToInt64(synthesisPDV.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]);					
								//if the competitor univers was selected we will add its bugdet to the budget of the reference univers
								//to calculate the PDV
								//if(webSession.CompetitorUniversAdvertiser.Count>1)
								if(webSession.PrincipalProductUniverses.Count>1)
								{
									universgroupInvestment=universgroupInvestment+budget;
								}
							}
							#endregion

							if(universgroupInvestment!=0) 
							{
								pdv=Math.Round(Convert.ToDouble(budget)/Convert.ToDouble(universgroupInvestment)*100,2);
								synthesisData[k].Add("PDV",pdv.ToString());
							}
							else 
							{
								synthesisData[k].Add("PDV","");
							}

							//Poids de la version vs produit correspondant
							if( synthesisTableTmp.Rows[0]["id_product"]!=System.DBNull.Value) 
							{
								investProduct = TNS.AdExpress.Web.DataAccess.Results.APPM.SynthesisDataAccess.GetProductInvestment(webSession,dataSource,dateBegin,dateEnd,baseTarget,synthesisTableTmp.Rows[0]["id_product"].ToString()).Tables[0];			
								if(investProduct!=null && investProduct.Rows.Count>0 && !investProduct.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()].ToString().Equals("") ) 
								{
                                    tempInvestProduct = Convert.ToInt64(investProduct.Rows[0][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]);	
									if(tempInvestProduct>0) 
									{
										double versionWeight=Math.Round(Convert.ToDouble(budget)/Convert.ToDouble(tempInvestProduct)*100,2);
										synthesisData[k].Add("versionWeight",versionWeight.ToString());
									}
									else 
									{
										synthesisData[k].Add("versionWeight","");
									}

								}
							}
				
							// cible selectionnée
							//string targetSelected =((LevelInformation)webSession.SelectionUniversAEPMTarget.FirstNode.Tag).ID.ToString();
							synthesisData[k].Add("targetSelected",targetSelected);

							//cible de base
							synthesisData[k].Add("baseTarget",targetBase);

							//nombre de GRP(cible selectionnée)
							additionalTargetGRP=Math.Round(additionalTargetGRP,3);
							synthesisData[k].Add("GRPNumber",additionalTargetGRP.ToString());

							//nombre de GRP(cible 15 ans et +)
							synthesisData[k].Add("GRPNumberBase",Math.Round(baseTargetGRP, 3).ToString());

							//Indice GRP vs cible 15 ans à +
							if(baseTargetGRP>0) 
							{
								synthesisData[k].Add("IndiceGRP",Math.Round((additionalTargetGRP/baseTargetGRP)*100,3).ToString());
							}
							else 
							{
								synthesisData[k].Add("IndiceGRP","");
							}
							// Coût GRP(cible selectionnée)
							//additionalTargetCost=Math.Round(additionalTargetCost,3);
							if(additionalTargetGRP>0) 
							{
								additionalTargetCost=Math.Round(budget/additionalTargetGRP,3);
								synthesisData[k].Add("GRPCost",additionalTargetCost.ToString());
							}
							else 
							{
								synthesisData[k].Add("GRPCost","");
							}

							// Coût GRP(cible 15 et +)
							if(baseTargetGRP>0) 
							{
								synthesisData[k].Add("GRPCostBase",Math.Round(budget/baseTargetGRP,3).ToString());
							}
							else 
							{
								synthesisData[k].Add("GRPCostBase","");
							}

							//Indice coût GRP vs cible 15 ans à +
							if(additionalTargetGRP>0 && baseTargetGRP>0) 
							{
								synthesisData[k].Add("IndiceGRPCost",Math.Round(((budget/additionalTargetGRP)/(budget/baseTargetGRP))*100,3).ToString());
							}
							else 
							{
								synthesisData[k].Add("IndiceGRPCost","");
							}

							//synthesisData.Add("IndiceGRPCost",Math.Round((additionalTargetCost/baseTargetCost)*100,3).ToString());

							k++;

							#endregion

							
						}
					}
					#endregion

				}
				catch(System.Exception err) {
					throw(new WebExceptions.SynthesisRulesException("Error while formatting the data for APPM Synthesis by Version ",err));
				}

				#region Recharging the univers
				//charging the reference univers
				//webSession.CurrentUniversAdvertiser=(TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;				
				#endregion
			}

			return synthesisData;
		}
		#endregion 

	}
}
