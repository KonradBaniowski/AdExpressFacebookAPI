#region Informations
// Auteur: Y. R'kaina
// Date de création: 25/01/2007
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Translation;
using WebDataAccess=TNS.AdExpress.Web.DataAccess;
using APPMConstantes=TNS.AdExpress.Constantes.FrameWork.Results.APPM;
using TNS.FrameWork.WebResultUI;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Rules.Results.APPM{
	/// <summary>
	/// Description résumée de SectorDataSeasonalityRules.
	/// </summary>
	public class SectorDataSeasonalityRules{

		#region GetSeasonalityPreformatedData
		/// <summary>
		/// Seasonality Plan Sector Rules
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		///<param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>	
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <returns>DataTable</returns>
		public static DataTable GetSeasonalityPreformatedData(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget){
			
			#region variable
			DataTable SeasonalityPlanTable=null;
			DataTable SeasonalityData=new DataTable();
			string targetSelected=string.Empty;
			string targetBase=string.Empty;						
			double totalBaseTargetUnit=0;
			double totalAdditionalTargetUnit=0;
			double currentUnit=0;
			#endregion

			try{
			
				#region Données
				SeasonalityPlanTable=TNS.AdExpress.Web.DataAccess.Results.APPM.SectorDataSeasonalityDataAccess.GetSeasonalityData(webSession,dataSource,idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget).Tables[0];
				#endregion
				
				#region Construction du tableau de données
				if(SeasonalityPlanTable!=null && SeasonalityPlanTable.Rows.Count>0){

					#region Creation des colonnes de la table
					SeasonalityData.Columns.Add("seasonality");
					SeasonalityData.Columns.Add("baseTarget");
					SeasonalityData.Columns.Add("additionalTarget");
					SeasonalityData.Columns.Add("unitBase");
					SeasonalityData.Columns.Add("unitSelected");	
					SeasonalityData.Columns.Add("distributionBase");
					SeasonalityData.Columns.Add("distributionSelected");
					SeasonalityData.Columns.Add("totalBaseTargetUnit");
					SeasonalityData.Columns.Add("totalAdditionalTargetUnit");
					#endregion

					#region date field name
					string dateField= WebDataAccess.Functions.GetDateFieldWebPlanTable(webSession);
					#endregion

					//Calcul des total grp pour la cible de base et la cible additionnée
					foreach(DataRow current in SeasonalityPlanTable.Rows){
						currentUnit=Convert.ToDouble(current[WebFunctions.SQLGenerator.GetUnitAlias(webSession)].ToString());

						#region Traitement en fonction de l'unité seléctionner
						switch(webSession.Unit){
							case WebConstantes.CustomerSessions.Unit.kEuro:
							case WebConstantes.CustomerSessions.Unit.pages:
								currentUnit = currentUnit/1000;break;
						}
						#endregion

						if(Convert.ToInt32(current["id_target"])==idBaseTarget){
							totalBaseTargetUnit+=currentUnit;
						}
						else 
							totalAdditionalTargetUnit+=currentUnit;
					}
					int i=0;
					currentUnit=0;
					SeasonalityData.Rows.Add(SeasonalityData.NewRow());
					foreach(DataRow dt in SeasonalityPlanTable.Rows){	
						SeasonalityData.Rows[i]["seasonality"]=dt[""+dateField+""].ToString();
						currentUnit=Convert.ToDouble(dt[WebFunctions.SQLGenerator.GetUnitAlias(webSession)].ToString());

						#region Traitement en fonction de l'unité seléctionner
						switch(webSession.Unit){
							case WebConstantes.CustomerSessions.Unit.kEuro:
							case WebConstantes.CustomerSessions.Unit.pages:
								currentUnit = currentUnit/1000;break;
						}
						#endregion

						if(Convert.ToInt32(dt["id_target"])==idBaseTarget){
							SeasonalityData.Rows[i]["unitBase"]=currentUnit.ToString();
							SeasonalityData.Rows[i]["distributionBase"]=Math.Round((currentUnit*100/totalBaseTargetUnit),2).ToString() ;
							SeasonalityData.Rows[i]["baseTarget"]=dt["target"].ToString();
							SeasonalityData.Rows[i]["totalBaseTargetUnit"]=Convert.ToString(totalBaseTargetUnit);
							SeasonalityData.Rows[i]["totalAdditionalTargetUnit"]=Convert.ToString(totalAdditionalTargetUnit);
						}
						else{
							SeasonalityData.Rows[i]["unitSelected"]=currentUnit.ToString();
							SeasonalityData.Rows[i]["distributionSelected"]=Math.Round((currentUnit*100/totalAdditionalTargetUnit),2).ToString() ;
							SeasonalityData.Rows[i]["additionalTarget"]=dt["target"].ToString();
							SeasonalityData.Rows[i]["totalBaseTargetUnit"]=Convert.ToString(totalBaseTargetUnit);
							SeasonalityData.Rows[i]["totalAdditionalTargetUnit"]=Convert.ToString(totalAdditionalTargetUnit);
							i++;
							SeasonalityData.Rows.Add(SeasonalityData.NewRow());
						}
					}
					SeasonalityData.Rows.RemoveAt(SeasonalityData.Rows.Count-1);
				}
			}
				#endregion   

			catch(System.Exception err){				
				throw(new WebExceptions.SectorDataSeasonalityRulesException("Erreur dans le formatage des données de sector data seasonality ",err));
			}
			return SeasonalityData;
		} 
		#endregion

		#region GetSeasonalityResultTable
		/// <summary>
		/// Seasonality Plan Sector Rules
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		///<param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>	
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <returns>ResultTable</returns>
		public static ResultTable GetSeasonalityResultTable(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget){
			
			#region variable
			DataTable SeasonalityPlanTable=null;
			ResultTable resultTable=null;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;						
			double totalBaseTargetUnit=0;
			double totalAdditionalTargetUnit=0;
			double currentUnit=0;
			string period=string.Empty;
			string percentFormat = "{0:percentage}";
			#endregion

			try{
			
				#region Données
				SeasonalityPlanTable=TNS.AdExpress.Web.DataAccess.Results.APPM.SectorDataSeasonalityDataAccess.GetSeasonalityData(webSession,dataSource,idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget).Tables[0];
				#endregion
				
				#region Construction du tableau de données
				if(SeasonalityPlanTable!=null && SeasonalityPlanTable.Rows.Count>0){

					#region date field name
					string dateField= WebDataAccess.Functions.GetDateFieldWebPlanTable(webSession);
					#endregion

					#region Calcul des total grp pour la cible de base et la cible additionnée
					foreach(DataRow current in SeasonalityPlanTable.Rows){
						currentUnit=Convert.ToDouble(current[WebFunctions.SQLGenerator.GetUnitAlias(webSession)].ToString());
						
						if(Convert.ToInt32(current["id_target"])==idBaseTarget){
							totalBaseTargetUnit+=currentUnit;
							targetBase=current["target"].ToString();
						}
						else{ 
							totalAdditionalTargetUnit+=currentUnit;
							targetSelected=current["target"].ToString();
						}
					}
					currentUnit=0;
					#endregion

					#region Création des headers
					Headers headers=new Headers();
					
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1139,webSession.SiteLanguage),APPMConstantes.TYPE_COLUMN_INDEX));	
						HeaderGroup baseTarget=new HeaderGroup(GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+" ("+targetBase+")",APPMConstantes.GRP_UNIT_COLUMN_INDEX);
						baseTarget.Add(new Header(GestionWeb.GetWebWord(573,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));
						baseTarget.Add(new Header(GestionWeb.GetWebWord(1743,webSession.SiteLanguage),APPMConstantes.DISTRIBUTION_COLUMN_INDEX));
						HeaderGroup additionalTarget=new HeaderGroup(GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+" ("+targetSelected+")",APPMConstantes.GRP_DISTRIBUTION_COLUMN_INDEX);
						additionalTarget.Add(new Header(GestionWeb.GetWebWord(573,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));
						additionalTarget.Add(new Header(GestionWeb.GetWebWord(1743,webSession.SiteLanguage),APPMConstantes.DISTRIBUTION_COLUMN_INDEX));
						headers.Root.Add(baseTarget);
						headers.Root.Add(additionalTarget);
					}
					else{
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1139,webSession.SiteLanguage),APPMConstantes.TYPE_COLUMN_INDEX));	
						switch(webSession.Unit){
							case WebConstantes.CustomerSessions.Unit.euro:
								headers.Root.Add(new Header(GestionWeb.GetWebWord(1669,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));break;
							case WebConstantes.CustomerSessions.Unit.kEuro:
								headers.Root.Add(new Header(GestionWeb.GetWebWord(1790,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));break;
                            case WebConstantes.CustomerSessions.Unit.pages:
                                headers.Root.Add(new Header(Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].WebTextId, webSession.SiteLanguage)), APPMConstantes.UNIT_COLUMN_INDEX)); break;
                            case WebConstantes.CustomerSessions.Unit.insertion:
                                headers.Root.Add(new Header(Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].WebTextId, webSession.SiteLanguage)), APPMConstantes.UNIT_COLUMN_INDEX)); break;
						}
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1743,webSession.SiteLanguage),APPMConstantes.DISTRIBUTION_COLUMN_INDEX));
					}
					#endregion

					#region Crétaion de table resultTable
                    int nbLines = (SeasonalityPlanTable.Rows.Count / 2) + 1;
                    int nbCol;
                    int lineIndex = 0;
					resultTable = new ResultTable(nbLines,headers);
					nbCol = resultTable.DataColumnsNumber;
					#endregion

					#region filling resultTable
                    int baseValueColumn = 0, baseDistributionColumn = 0, additionalValueColumn = 0, additionalDistributionColumn = 0;

					#region Sélection de l'unité
					CellUnitFactory cellUnitFactory=webSession.GetCellUnitFactory();
					#endregion

					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						baseValueColumn = resultTable.GetHeadersIndexInResultTable(APPMConstantes.GRP_UNIT_COLUMN_INDEX+"-"+APPMConstantes.UNIT_COLUMN_INDEX);
						baseDistributionColumn = resultTable.GetHeadersIndexInResultTable(APPMConstantes.GRP_UNIT_COLUMN_INDEX+"-"+APPMConstantes.DISTRIBUTION_COLUMN_INDEX);
						additionalValueColumn = resultTable.GetHeadersIndexInResultTable(APPMConstantes.GRP_DISTRIBUTION_COLUMN_INDEX+"-"+APPMConstantes.UNIT_COLUMN_INDEX);
						additionalDistributionColumn = resultTable.GetHeadersIndexInResultTable(APPMConstantes.GRP_DISTRIBUTION_COLUMN_INDEX+"-"+APPMConstantes.DISTRIBUTION_COLUMN_INDEX);
					}
					else{
						baseValueColumn = APPMConstantes.UNIT_COLUMN_INDEX;
						baseDistributionColumn = APPMConstantes.DISTRIBUTION_COLUMN_INDEX;
						additionalValueColumn = APPMConstantes.GRP_UNIT_COLUMN_INDEX;
						additionalDistributionColumn = APPMConstantes.GRP_DISTRIBUTION_COLUMN_INDEX;
					}

					lineIndex = resultTable.AddNewLine(LineType.total);
					resultTable[lineIndex,APPMConstantes.TYPE_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1401,webSession.SiteLanguage));
					resultTable[lineIndex,baseValueColumn]=cellUnitFactory.Get(totalBaseTargetUnit);
					CellPercent cPer = new CellPercent(100);
					cPer.StringFormat = percentFormat;
					resultTable[lineIndex, baseDistributionColumn] = cPer;
					
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						resultTable[lineIndex,additionalValueColumn]=cellUnitFactory.Get(totalAdditionalTargetUnit);
						CellPercent cPer1 = new CellPercent(100);
						cPer1.StringFormat = percentFormat;
						resultTable[lineIndex, additionalDistributionColumn] = cPer1;
					}

					foreach(DataRow dt in SeasonalityPlanTable.Rows){
					
						currentUnit=Convert.ToDouble(dt[WebFunctions.SQLGenerator.GetUnitAlias(webSession)].ToString());

						if(Convert.ToInt32(dt["id_target"])==idBaseTarget){
							
							period=WebFunctions.Dates.getPeriodTxt(webSession,dt[""+dateField+""].ToString());

							lineIndex = resultTable.AddNewLine(LineType.level1);
							resultTable[lineIndex,APPMConstantes.TYPE_COLUMN_INDEX]=new CellLabel(period);
						
							resultTable[lineIndex,baseValueColumn]=cellUnitFactory.Get(currentUnit);
							CellPercent cPer2 = new CellPercent(Math.Round((currentUnit * 100 / totalBaseTargetUnit), 2));
							cPer2.StringFormat = percentFormat;
							resultTable[lineIndex, baseDistributionColumn] = cPer2;
						}
						else if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){

							resultTable[lineIndex,additionalValueColumn]=cellUnitFactory.Get(currentUnit);

							CellPercent cPer3 = new CellPercent(Math.Round((currentUnit * 100 / totalAdditionalTargetUnit), 2));
							cPer3.StringFormat = percentFormat;
							resultTable[lineIndex, additionalDistributionColumn] = cPer3;
						}
					}

					#endregion

				}
				#endregion
			}
			catch(System.Exception err){				
				throw(new WebExceptions.SectorDataSeasonalityRulesException("Erreur dans le formatage des données de sector data seasonality ",err));
			}
			return resultTable;
		} 
		#endregion

	}
}
