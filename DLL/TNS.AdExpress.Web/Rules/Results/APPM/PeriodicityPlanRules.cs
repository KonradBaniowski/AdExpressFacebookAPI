#region Informations
// Auteur: A.DADOUCH
// Date de création: 15/07/2005 
// Modified by: K.Shehzad
// Date of Modification: 12/08/2005  (changing the Exception usage)
// Modified by: Y.R'kaina
// Date of Modification: 01/02/2007  (Ajouter GetPeriodicityResultTable)
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using APPMConstantes=TNS.AdExpress.Constantes.FrameWork.Results.APPM;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Rules.Results.APPM
{
	/// <summary>
	/// Description résumée de PeriodicityPlanRules.
	/// </summary>
	public class PeriodicityPlanRules{

		/// <summary>
		/// Périodicité Plan APPM
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		///<param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>	
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <returns>HTML</returns>
		public static DataTable PeriodicityPlan(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget){
			
			#region variable
			DataTable periodicityPlanTable=null;
			DataTable periodicityData=new DataTable();
			string targets=string.Empty;
			string products=string.Empty;
			string periodicity=string.Empty;
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
				periodicityPlanTable=TNS.AdExpress.Web.DataAccess.Results.APPM.PeriodicityPlanDataAccess.GetPeriodicityData(webSession,dataSource,idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,products).Tables[0];
				#endregion
				
				#region Construction du tableau de données
				//	periodicityData.Columns.Clear();
				if(periodicityPlanTable!=null && periodicityPlanTable.Rows.Count>0){
					#region Creation des colonnes de la table
					periodicityData.Columns.Add("periodicity");
					//periodicityData.Columns.Add("idBaseTarget");
					//periodicityData.Columns.Add("idAdditionalTarget");
					periodicityData.Columns.Add("baseTarget");
					periodicityData.Columns.Add("additionalTarget");
					periodicityData.Columns.Add("unitBase");
					periodicityData.Columns.Add("unitSelected");	
					periodicityData.Columns.Add("cgrpBase");
					periodicityData.Columns.Add("cgrpSelected");
					periodicityData.Columns.Add("distributionBase");
					periodicityData.Columns.Add("distributionSelected");
					periodicityData.Columns.Add("cgrpDistributionBase");
					periodicityData.Columns.Add("cgrpDistributionSelected");
					periodicityData.Columns.Add("totalBaseTargetUnit");
					periodicityData.Columns.Add("totalAdditionalTargetUnit");
					#endregion

					//Calcul des total grp pour la cible de base et la cible additionnée
					foreach(DataRow current in periodicityPlanTable.Rows){
						if(Convert.ToInt32(current["id_target"])==idBaseTarget){
							totalBaseTargetUnit+=Convert.ToDouble(current["unit"].ToString());
							//totalBaseCgrp+=Convert.ToDouble(current["cgrp"].ToString());
						}else totalAdditionalTargetUnit+=Convert.ToDouble(current["unit"].ToString());
							//totalAdditionalCgrp+=Convert.ToDouble(current["cgrp"].ToString());
					}
						periodicityData.Rows.Add(periodicityData.NewRow());
						periodicityData.Rows[0]["totalBaseTargetUnit"]=Convert.ToString(totalBaseTargetUnit);
						periodicityData.Rows[0]["totalAdditionalTargetUnit"]=Convert.ToString(totalAdditionalTargetUnit);
					int i=1;
					periodicityData.Rows.Add(periodicityData.NewRow());
					foreach(DataRow dt in periodicityPlanTable.Rows){					
						periodicityData.Rows[i]["periodicity"]=dt["periodicity"].ToString();
						if(Convert.ToInt32(dt["id_target"])==idBaseTarget){
							periodicityData.Rows[i]["unitBase"]=dt["unit"].ToString();
							periodicityData.Rows[i]["distributionBase"]=Math.Round((Convert.ToDouble(dt["unit"])*100/totalBaseTargetUnit),2).ToString() ;
							if(Convert.ToDouble(dt["totalgrp"])!=0){
								periodicityData.Rows[i]["cgrpBase"]=Math.Round(Convert.ToDouble(dt["euros"])/Convert.ToDouble(dt["totalgrp"]),0).ToString(); 				
							}
							else
								periodicityData.Rows[i]["cgrpBase"]=0;
							periodicityData.Rows[0]["baseTarget"]=dt["target"].ToString();

						}else{
							periodicityData.Rows[i]["unitSelected"]=dt["unit"].ToString();
							periodicityData.Rows[i]["distributionSelected"]=Math.Round((Convert.ToDouble(dt["unit"])*100/totalAdditionalTargetUnit),2).ToString() ;
							if (Convert.ToDouble(dt["totalgrp"])!=0){
								periodicityData.Rows[i]["cgrpSelected"]=Math.Round(Convert.ToDouble(dt["euros"])/Convert.ToDouble(dt["totalgrp"]),0).ToString();
							}else
								periodicityData.Rows[i]["cgrpSelected"]=0;
							periodicityData.Rows[0]["additionalTarget"]=dt["target"].ToString();
							
							i++;
							periodicityData.Rows.Add(periodicityData.NewRow());
						}
					}
				}
			}
				#endregion   

			catch(System.Exception err){				
				throw(new WebExceptions.PeriodicityPlanRulesException("Erreur dans la récupération des données des périodivités du plan ",err));
			}
			return periodicityData;
		}  

		#region GetPeriodicityResultTable
		/// <summary>
		/// Périodicité Plan
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="dataSource">dataSource pour la creation de Dataset </param>
		///<param name="idWave">Identifiant de la vague</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>	
		/// <param name="idBaseTarget">Identifiant de la cible de base</param>
		/// <param name="idAdditionalTarget">Identifiant de la cible sélectionnée</param>
		/// <returns>ResultTable</returns>
		public static ResultTable GetPeriodicityResultTable(WebSession webSession,IDataSource dataSource,Int64 idWave,int dateBegin,int dateEnd,Int64 idBaseTarget,Int64 idAdditionalTarget){

			#region variable
			DataTable periodicityPlanTable=null;
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
				periodicityPlanTable=TNS.AdExpress.Web.DataAccess.Results.APPM.PeriodicityPlanDataAccess.GetPeriodicityData(webSession,dataSource,idWave,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,products).Tables[0];
				#endregion

				#region Construction du tableau de données
				if(periodicityPlanTable!=null && periodicityPlanTable.Rows.Count>0){
				
					#region Calcul des total grp pour la cible de base et la cible additionnée
					foreach(DataRow current in periodicityPlanTable.Rows){

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
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1774,webSession.SiteLanguage),APPMConstantes.TYPE_COLUMN_INDEX));	
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
						headers.Root.Add(new Header(GestionWeb.GetWebWord(1774,webSession.SiteLanguage),APPMConstantes.TYPE_COLUMN_INDEX));	
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
					long nbLines=(periodicityPlanTable.Rows.Count/2)+1;
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

					foreach(DataRow dt in periodicityPlanTable.Rows){
					
						currentUnit=Convert.ToDouble(dt["unit"].ToString());

						if(Convert.ToInt32(dt["id_target"])==idBaseTarget){
							
							lineIndex = resultTable.AddNewLine(LineType.level1);
							resultTable[lineIndex,APPMConstantes.TYPE_COLUMN_INDEX]=new CellLabel(dt["periodicity"].ToString());
						
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
				throw(new WebExceptions.PeriodicityPlanRulesException("Erreur dans la récupération des données des périodivités du plan ",err));
			}
			return resultTable;
		}
		#endregion
	}
}
