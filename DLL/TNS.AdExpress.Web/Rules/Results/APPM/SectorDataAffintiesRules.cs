#region Informations
// Author: Y. R'kaina 
// Date of creation: 29/01/2007
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using DataAcces = TNS.AdExpress.Web.DataAccess;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.WebResultUI;
using APPMConstantes=TNS.AdExpress.Constantes.FrameWork.Results.APPM;

namespace TNS.AdExpress.Web.Rules.Results.APPM{
	/// <summary>
	/// Formats the table for affinities
	/// </summary>
	public class SectorDataAffintiesRules{

		/// <summary>
		/// Formats the table for affinities 
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User Session</param>
		/// <param name="dateBegin">Period beginning</param>
		/// <param name="dateEnd">Period end</param>
		/// <param name="idBaseTarget">Default target</param>
		/// <param name="idWave">Study wave</param>
		/// <returns>DataSet ready to be displayed</returns>
		public static ResultTable GetData(WebSession webSession,IDataSource dataSource, int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idWave){
		
			#region variables
			double grpBaseTarget=0;
			double cgrpBaseTarget=0;
			double cgrp=0;
			Int64 euros=0;			
			DataTable data=null;
			ResultTable resultTable=null;
			bool baseTargetNotFound=true;
			string baseTarget=string.Empty;
			#endregion
		
			#region Création des headers
			Headers headers=new Headers();
			headers.Root.Add(new Header(GestionWeb.GetWebWord(1757,webSession.SiteLanguage),APPMConstantes.FIRST_COLUMN_INDEX-1));			
			headers.Root.Add(new Header(GestionWeb.GetWebWord(1679,webSession.SiteLanguage),APPMConstantes.GRP_COLUMN_INDEX-1));
			headers.Root.Add(new Header(GestionWeb.GetWebWord(1686,webSession.SiteLanguage),APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX-1));
			headers.Root.Add(new Header(GestionWeb.GetWebWord(1685,webSession.SiteLanguage),APPMConstantes.CGRP_COLUMN_INDEX-1));
			headers.Root.Add(new Header(GestionWeb.GetWebWord(1686,webSession.SiteLanguage),APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX-1));
			#endregion

			try{

				#region get data
				data = DataAcces.Results.APPM.AffinitiesDataAccess.GetData(webSession,dataSource,dateBegin, dateEnd, idBaseTarget, idWave).Tables[0];
				#endregion

				if(data!=null && data.Rows.Count>0){

					#region Crétaion de table resultTable
					long nbLines=data.Rows.Count;
					long nbCol;
					long lineIndex=0;
					resultTable = new ResultTable(nbLines,headers);
					nbCol = resultTable.DataColumnsNumber;
					#endregion

					#region getting base target values for calculating affinities
					//we need to find the values of base target to calculate the affinities and to add it in the first row of the dtAffinities table
					//for loop is used to find the base target but instead of traversing the whole table as soon as the values are found
					//we exit the loop through baseTargetNotFound boolean
					for(int i=0;i<data.Rows.Count && baseTargetNotFound;i++){
						if(Convert.ToInt64(data.Rows[i]["id_target"])==idBaseTarget){
							baseTarget=data.Rows[i]["target"].ToString();
							grpBaseTarget+=Convert.ToDouble(data.Rows[i]["GRP"]);
							euros+=Convert.ToInt64(data.Rows[i]["euros"]);
							baseTargetNotFound=false;
						}
					}
					if(grpBaseTarget!=0)
						cgrpBaseTarget=Math.Round(euros/grpBaseTarget,3);
					euros=0;
					#endregion

					#region filling resultTable
					//the new table is being filled with the required values to be displayed
					//Base Target Values
					lineIndex = resultTable.AddNewLine(LineType.total);
					resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(baseTarget);
					resultTable[lineIndex,APPMConstantes.GRP_COLUMN_INDEX]=new CellGRP(Math.Round(grpBaseTarget,3));
					if(grpBaseTarget!=0)
						resultTable[lineIndex,APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX]=new CellAffinity(Math.Round((grpBaseTarget/grpBaseTarget)*100,3));
					else
						resultTable[lineIndex,APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX]=new CellAffinity(0);
					
					resultTable[lineIndex,APPMConstantes.CGRP_COLUMN_INDEX]=new CellCGRP(Math.Round(cgrpBaseTarget,3));
					if(cgrpBaseTarget!=0)
						resultTable[lineIndex,APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX]=new CellAffinity(Math.Round((cgrpBaseTarget/cgrpBaseTarget)*100,3));
					else
						resultTable[lineIndex,APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX]=new CellAffinity(0);

					//Additional targets
					foreach(DataRow row in data.Rows){
						if(Convert.ToInt64(row["id_target"])==idBaseTarget)
							continue;
						else{
							lineIndex = resultTable.AddNewLine(LineType.level1);
							resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(row["target"].ToString());
							resultTable[lineIndex,APPMConstantes.GRP_COLUMN_INDEX]=new CellGRP(Math.Round(Convert.ToDouble(row["GRP"]),3));
							if(grpBaseTarget!=0)
								resultTable[lineIndex,APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX]=new CellAffinity(Math.Round((Convert.ToDouble(row["GRP"])/grpBaseTarget)*100,3));
							else
								resultTable[lineIndex,APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX]=new CellAffinity(0);
					
							if(Convert.ToDouble(row["GRP"])!=0){
								resultTable[lineIndex,APPMConstantes.CGRP_COLUMN_INDEX]=new CellCGRP(Math.Round(Convert.ToDouble(row["euros"])/Convert.ToDouble(row["GRP"]),3));
								cgrp=Math.Round(Convert.ToDouble(row["euros"])/Convert.ToDouble(row["GRP"]),3);
							}
							else{
								resultTable[lineIndex,APPMConstantes.CGRP_COLUMN_INDEX]=new CellCGRP(0);
								cgrp=0;
							}

							if(cgrpBaseTarget!=0)
								resultTable[lineIndex,APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX]=new CellAffinity(Math.Round((cgrp/cgrpBaseTarget)*100,3));
							else
								resultTable[lineIndex,APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX]=new CellAffinity(0);
						}
					}
					#endregion
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.SectorDataAffinitiesRulesExcpetion("Error while formatting the data for Sector Data Affinties ",err));
			}

			return resultTable;
		}
	}
}
