#region Informations
// Auteur: A.DADOUCH
// Date de création: 15/07/2005 
// Modified by: K.Shehzad
// Date of Modification: 12/08/2005  (changing the Exception usage)
// Modified by: Y.R'kaina
// Date of Modification: 01/02/2007  (Ajouter GetFamilyInterestResultTable)
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Translation;
using APPMConstantes=TNS.AdExpress.Constantes.FrameWork.Results.APPM;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Rules.Results.APPM {
		/// <summary>
		/// Description résumée de AnalyseFamilyInterestPlanRules.
		/// </summary>
	public class AnalyseFamilyInterestPlanRules{

		/// <summary>
		/// familles d'intérêts Plan APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		///<param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>	
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <returns>HTML</returns>
		public static DataTable InterestFamilyPlan(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget){
			
			#region variable
			DataTable InterestFamilyPlanTable=null;
			DataTable InterestFamilyData=new DataTable();
			string targets=string.Empty;
			string products=string.Empty;
			string InterestFamily=string.Empty;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;						
			double totalBaseTargetUnit=0;
			double totalAdditionalTargetUnit=0;
			//double totalBaseCgrp=0;
			//double totalAdditionalCgrp=0;

			#endregion

			try{
				//Produit sélectionné
				//products=webSession.GetSelection(webSession.SelectionUniversAdvertiser,CustomerRightConstante.type.productAccess);			
				#region Données
				InterestFamilyPlanTable=TNS.AdExpress.Web.DataAccess.Results.APPM.AnalyseFamilyInterestPlanDataAccess.GetAnalyseFamilyInterestPlan(webSession,dataSource,idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,products).Tables[0];
				#endregion
				
				#region Construction du tableau de données
				//	InterestFamilyData.Columns.Clear();
				if(InterestFamilyPlanTable!=null && InterestFamilyPlanTable.Rows.Count>0){
					#region Creation des colonnes de la table
					InterestFamilyData.Columns.Add("InterestFamily");
					//periodicityData.Columns.Add("idBaseTarget");
					//periodicityData.Columns.Add("idAdditionalTarget");
					InterestFamilyData.Columns.Add("baseTarget");
					InterestFamilyData.Columns.Add("additionalTarget");
					InterestFamilyData.Columns.Add("unitBase");
					InterestFamilyData.Columns.Add("unitSelected");	
					InterestFamilyData.Columns.Add("cgrpBase");
					InterestFamilyData.Columns.Add("cgrpSelected");
					InterestFamilyData.Columns.Add("distributionBase");
					InterestFamilyData.Columns.Add("distributionSelected");
					InterestFamilyData.Columns.Add("cgrpDistributionBase");
					InterestFamilyData.Columns.Add("cgrpDistributionSelected");
					InterestFamilyData.Columns.Add("totalBaseTargetUnit");
					InterestFamilyData.Columns.Add("totalAdditionalTargetUnit");
					#endregion

					//Calcul des total grp pour la cible de base et la cible additionnée
					foreach(DataRow current in InterestFamilyPlanTable.Rows){
						if(Convert.ToInt32(current["id_target"])==idBaseTarget){
							totalBaseTargetUnit+=Convert.ToDouble(current["unit"].ToString());
						//	totalBaseCgrp+=Convert.ToDouble(current["cgrp"].ToString());
						}else 
							totalAdditionalTargetUnit+=Convert.ToDouble(current["unit"].ToString());
						//totalAdditionalCgrp+=Convert.ToDouble(current["cgrp"].ToString());
					}
					InterestFamilyData.Rows.Add(InterestFamilyData.NewRow());
					InterestFamilyData.Rows[0]["totalBaseTargetUnit"]=Convert.ToString(totalBaseTargetUnit);
					InterestFamilyData.Rows[0]["totalAdditionalTargetUnit"]=Convert.ToString(totalAdditionalTargetUnit);
					int i=1;
					InterestFamilyData.Rows.Add(InterestFamilyData.NewRow());
					foreach(DataRow dt in InterestFamilyPlanTable.Rows){					
						InterestFamilyData.Rows[i]["InterestFamily"]=dt["Interest_Center"].ToString();
						if(Convert.ToInt32(dt["id_target"])==idBaseTarget){
							InterestFamilyData.Rows[i]["unitBase"]=dt["unit"].ToString();
							InterestFamilyData.Rows[i]["distributionBase"]=Math.Round((Convert.ToDouble(dt["unit"])*100/totalBaseTargetUnit),2).ToString() ;
							if (Convert.ToDouble(dt["totalgrp"])!=0){
								InterestFamilyData.Rows[i]["cgrpBase"]=Math.Round(Convert.ToDouble(dt["euros"])/Convert.ToDouble(dt["totalgrp"]),0).ToString(); 
							}else
								InterestFamilyData.Rows[i]["cgrpBase"]=0;
							InterestFamilyData.Rows[0]["baseTarget"]=dt["target"].ToString();

						}else{
							InterestFamilyData.Rows[i]["unitSelected"]=dt["unit"].ToString();
							InterestFamilyData.Rows[i]["distributionSelected"]=Math.Round((Convert.ToDouble(dt["unit"])*100/totalAdditionalTargetUnit),2).ToString() ;
							if (Convert.ToDouble(dt["totalgrp"])!=0){
								InterestFamilyData.Rows[i]["cgrpSelected"]=Math.Round(Convert.ToDouble(dt["euros"])/Convert.ToDouble(dt["totalgrp"]),0).ToString(); 
							}else
								InterestFamilyData.Rows[i]["cgrpSelected"]=0;
							InterestFamilyData.Rows[0]["additionalTarget"]=dt["target"].ToString();
							
							i++;
							InterestFamilyData.Rows.Add(InterestFamilyData.NewRow());
						}
					}
				}
			}
				#endregion   

			catch(System.Exception err){				
				throw(new WebExceptions.AnalyseFamilyInterestPlanRulesException("Erreur dans la récupération des données des AnalyseFamilyInterestPlan ",err));
			}
			return InterestFamilyData;
		}  

		#region GetFamilyInterestResultTable
		/// <summary>
		/// familles d'intérêts Plan
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		///<param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>	
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <returns>ResultTable</returns>
		public static ResultTable GetFamilyInterestResultTable(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget){
		
			#region variable
			DataTable InterestFamilyPlanTable=null;
			ResultTable resultTable=null;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;						
			double totalBaseTargetUnit=0;
			double totalAdditionalTargetUnit=0;
			string products=string.Empty;
			double currentUnit=0;
			string period=string.Empty;
			#endregion
			
			try{

				#region Données
				InterestFamilyPlanTable=TNS.AdExpress.Web.DataAccess.Results.APPM.AnalyseFamilyInterestPlanDataAccess.GetAnalyseFamilyInterestPlan(webSession,dataSource,idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,products).Tables[0];
				#endregion

				#region Construction du tableau de données
				if(InterestFamilyPlanTable!=null && InterestFamilyPlanTable.Rows.Count>0){
				
					#region Calcul des total grp pour la cible de base et la cible additionnée
					foreach(DataRow current in InterestFamilyPlanTable.Rows){
						currentUnit=Convert.ToDouble(current["unit"].ToString());
						
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
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1777,webSession.SiteLanguage),APPMConstantes.TYPE_COLUMN_INDEX));	
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
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1777,webSession.SiteLanguage),APPMConstantes.TYPE_COLUMN_INDEX));	
						switch(webSession.Unit){
							case WebConstantes.CustomerSessions.Unit.euro:
								headers.Root.Add(new Header(GestionWeb.GetWebWord(1669,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));break;
							case WebConstantes.CustomerSessions.Unit.kEuro:
								headers.Root.Add(new Header(GestionWeb.GetWebWord(1790,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));break;
							case WebConstantes.CustomerSessions.Unit.pages:
								headers.Root.Add(new Header(GestionWeb.GetWebWord(943,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));break;
							case WebConstantes.CustomerSessions.Unit.insertion:
								headers.Root.Add(new Header(GestionWeb.GetWebWord(940,webSession.SiteLanguage),APPMConstantes.UNIT_COLUMN_INDEX));break;
						}
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1743,webSession.SiteLanguage),APPMConstantes.DISTRIBUTION_COLUMN_INDEX));
					}
					#endregion

					#region Crétaion de table resultTable
					long nbLines=(InterestFamilyPlanTable.Rows.Count/2)+1;
					long nbCol;
					long lineIndex=0;
					resultTable = new ResultTable(nbLines,headers);
					nbCol = resultTable.DataColumnsNumber;
					#endregion

					#region filling resultTable
					Int64 baseValueColumn=0, baseDistributionColumn=0, additionalValueColumn=0, additionalDistributionColumn=0;

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
					resultTable[lineIndex,baseDistributionColumn]=new CellPercent(100);
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						resultTable[lineIndex,additionalValueColumn]=cellUnitFactory.Get(totalAdditionalTargetUnit);
						resultTable[lineIndex,additionalDistributionColumn]=new CellPercent(100);
					}

					foreach(DataRow dt in InterestFamilyPlanTable.Rows){
					
						currentUnit=Convert.ToDouble(dt["unit"].ToString());

						if(Convert.ToInt32(dt["id_target"])==idBaseTarget){
							
							lineIndex = resultTable.AddNewLine(LineType.level1);
							resultTable[lineIndex,APPMConstantes.TYPE_COLUMN_INDEX]=new CellLabel(dt["Interest_Center"].ToString());
						
							resultTable[lineIndex,baseValueColumn]=cellUnitFactory.Get(currentUnit);
							resultTable[lineIndex,baseDistributionColumn]=new CellPercent(Math.Round((currentUnit*100/totalBaseTargetUnit),2));
						}
						else if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){

							resultTable[lineIndex,additionalValueColumn]=cellUnitFactory.Get(currentUnit);
							resultTable[lineIndex,additionalDistributionColumn]=new CellPercent(Math.Round((currentUnit*100/totalAdditionalTargetUnit),2));
						}
					}

					#endregion

				}
				#endregion


			}
			catch(System.Exception err){				
				throw(new WebExceptions.AnalyseFamilyInterestPlanRulesException("Erreur dans la récupération des données des AnalyseFamilyInterestPlan ",err));
			}

			return resultTable;
		}
		#endregion

	}
}

