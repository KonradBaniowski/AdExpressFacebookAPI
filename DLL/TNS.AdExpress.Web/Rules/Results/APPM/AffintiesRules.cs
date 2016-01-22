#region Informations
// Author: K. Shehzad 
// Date of creation: 27/07/2005 
#endregion
using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using DataAcces = TNS.AdExpress.Web.DataAccess;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Units;
using WebCste = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Rules.Results.APPM
{
	/// <summary>
	/// Formats the table for affinities
	/// </summary>
	public class AffintiesRules
	{
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
		public static DataTable GetData(WebSession webSession,IDataSource dataSource, int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idWave)
		{
			#region variables			
			double grpBaseTarget=0;
			double cgrpBaseTarget=0;
			Int64 euros=0;			
			DataTable data=null;
			DataTable dtAffinities=null;
			bool baseTargetNotFound=true;
			DataRow resultRow=null;
			string baseTarget=string.Empty;
			#endregion
			
			try{
                #region get data
				data = DataAcces.Results.APPM.AffinitiesDataAccess.GetData(webSession,dataSource,dateBegin, dateEnd, idBaseTarget, idWave).Tables[0];
				#endregion

				#region formatting
				if(data!=null && data.Rows.Count>0){
					#region creating resultant table
					//The resultant table which will have the formatted data
					dtAffinities = new DataTable();
					dtAffinities.Columns.Add("id_target",System.Type.GetType("System.String"));
					dtAffinities.Columns.Add("target",System.Type.GetType("System.String"));
					dtAffinities.Columns.Add("totalGRP",System.Type.GetType("System.Double"));
					dtAffinities.Columns.Add("GRPAffinities",System.Type.GetType("System.Double"));
					dtAffinities.Columns.Add("cgrp",System.Type.GetType("System.Double"));
					dtAffinities.Columns.Add("cgrpAffinities",System.Type.GetType("System.Double"));				
					#endregion

                    #region getting base target values for calculating affinities
					//we need to find the values of base target to calculate the affinities and to add it in the first row of the dtAffinities table
					//for loop is used to find the base target but instead of traversing the whole table as soon as the values are found
					//we exit the loop through baseTargetNotFound boolean
					for(int i=0;i<data.Rows.Count && baseTargetNotFound;i++){
						if(Convert.ToInt64(data.Rows[i]["id_target"])==idBaseTarget){
							baseTarget=data.Rows[i]["target"].ToString();
							grpBaseTarget+=Convert.ToDouble(data.Rows[i][UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]);
                            euros += Convert.ToInt64(data.Rows[i][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]);
							baseTargetNotFound=false;
						}
					}
					if(grpBaseTarget!=0)
						cgrpBaseTarget=Math.Round(euros/grpBaseTarget,3);
					euros=0;
					#endregion

					#region filling the new table
					//the new table is being filled with the required values to be displayed
					//Base Target Values
					resultRow=dtAffinities.NewRow();						
					resultRow["id_target"]=idBaseTarget;
					resultRow["target"]=baseTarget;
					resultRow["totalGRP"]=Math.Round(grpBaseTarget,3);
					if(grpBaseTarget!=0)
						resultRow["GRPAffinities"]=Math.Round((grpBaseTarget/grpBaseTarget)*100,3);
					else
						resultRow["GRPAffinities"]=0;
					resultRow["cgrp"]=Math.Round(cgrpBaseTarget,3);
					if(cgrpBaseTarget!=0)
						resultRow["cgrpAffinities"]=Math.Round((cgrpBaseTarget/cgrpBaseTarget)*100,3);
					else
						resultRow["cgrpAffinities"]=0;
					dtAffinities.Rows.Add(resultRow);	
					//Additional targets
					foreach(DataRow row in data.Rows){
						if(Convert.ToInt64(row["id_target"])==idBaseTarget)
							continue;
						else{
							resultRow=dtAffinities.NewRow();						
							resultRow["id_target"]=row["id_Target"];
							resultRow["target"]=row["target"];
                            resultRow["totalGRP"] = Math.Round(Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]), 3);
							if(grpBaseTarget!=0)
                                resultRow["GRPAffinities"] = Math.Round((Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]) / grpBaseTarget) * 100, 3);
							else
								resultRow["GRPAffinities"]=0;
                            if (Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]) != 0)
                                resultRow["cgrp"] = Math.Round(Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]) / Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]), 3);
							else
								resultRow["cgrp"]=0;
							if(cgrpBaseTarget!=0)
								resultRow["cgrpAffinities"]=Math.Round((Convert.ToDouble(resultRow["cgrp"])/cgrpBaseTarget)*100,3);
							else
								resultRow["cgrpAffinities"]=0;
							dtAffinities.Rows.Add(resultRow);			
						}
							
					}
					#endregion

					#region old code
//					foreach(DataRow row in data.Rows)
//					{
//						if(Convert.ToInt64(row["id_target"])==idTarget)
//						{
//							totalGRP+=Convert.ToDouble(row["GRP"]);
//							euros+=Convert.ToInt64(row["euros"]);
//						}
//						//another "if" is used instead of "else" to take into account the last target calculated else it will be skipped
//						//and wont be added in the affinties table, so when the id target changes or we reache to the end of the data table the
//						//calculated values are stored in the affinities table.
//						if(Convert.ToInt64(row["id_target"])!=idTarget || data.Rows.Count==rowsTreated+1)
//						{
//							dtAffinities.Rows.Add(dtAffinities.NewRow());
//							dtAffinities.Rows[i]["id_target"]=idTarget;
//							dtAffinities.Rows[i]["target"]=target;
//							dtAffinities.Rows[i]["totalGRP"]=Math.Round(totalGRP,3);
//							dtAffinities.Rows[i]["cgrp"]=Math.Round(euros/totalGRP,3);
//							idTarget=Convert.ToInt64(row["id_target"]);
//							target=row["target"].ToString();
//							totalGRP=Convert.ToDouble(row["GRP"]);
//							euros=Convert.ToInt64(row["euros"]);
//							i++;
//						}
//						rowsTreated++;
//					}
					#endregion
				}
				#endregion
			}
			catch(System.Exception err){
				throw(new WebExceptions.AffinitiesRulesExcpetion("Error while formatting the data for APPM Affinties ",err));
			}
					
			return dtAffinities;
		}
	}
}
