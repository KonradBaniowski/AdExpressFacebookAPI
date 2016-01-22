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
using TNS.AdExpress.Domain.Translation;
using DataAcces = TNS.AdExpress.Web.DataAccess;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.WebResultUI;
using APPMConstantes=TNS.AdExpress.Constantes.FrameWork.Results.APPM;
using TNS.AdExpress.Domain.Units;
using WebCste = TNS.AdExpress.Constantes.Web;

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
            string numberFormat = "{0:max0}", affinityFormat = "{0:max0}", cGrpFormat = "{0:max0}", percentFormat = "{0:percentage}";

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
                    int nbLines = data.Rows.Count;
                    int nbCol;
                    int lineIndex = 0;
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
                            grpBaseTarget += Convert.ToDouble(data.Rows[i][UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]);
							euros+=Convert.ToInt64(data.Rows[i][UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]);
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
					CellGRP cGrp = new CellGRP(Math.Round(grpBaseTarget, 3));
					cGrp.StringFormat = UnitsInformation.Get(WebCste.CustomerSessions.Unit.grp).StringFormat;
					resultTable[lineIndex, APPMConstantes.GRP_COLUMN_INDEX] = cGrp;
					if (grpBaseTarget != 0) {
						CellAffinity cAf = new CellAffinity(Math.Round((grpBaseTarget / grpBaseTarget) * 100, 3));
						cAf.StringFormat = affinityFormat;
						resultTable[lineIndex, APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX] = cAf;
					}
					else {
						CellAffinity cAf1 = new CellAffinity(0);
						cAf1.StringFormat = affinityFormat;
						resultTable[lineIndex, APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX] = cAf1;
					}
					CellCGRP cCgrp0 = new CellCGRP(Math.Round(cgrpBaseTarget, 3));
					cCgrp0.StringFormat = cGrpFormat;
					resultTable[lineIndex, APPMConstantes.CGRP_COLUMN_INDEX] = cCgrp0;
					if (cgrpBaseTarget != 0) {
						CellAffinity cAf2 = new CellAffinity(Math.Round((cgrpBaseTarget / cgrpBaseTarget) * 100, 3)); 
						cAf2.StringFormat = affinityFormat;
						resultTable[lineIndex, APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX] = cAf2;
					}
					else {
						CellAffinity cAf3 = new CellAffinity(0);
						cAf3.StringFormat = affinityFormat;
						resultTable[lineIndex, APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX] = cAf3;
					}

					//Additional targets
					foreach(DataRow row in data.Rows){
						if(Convert.ToInt64(row["id_target"])==idBaseTarget)
							continue;
						else{
							lineIndex = resultTable.AddNewLine(LineType.level1);
							resultTable[lineIndex,APPMConstantes.FIRST_COLUMN_INDEX]=new CellLabel(row["target"].ToString());
							CellGRP cGrp1 = new CellGRP(Math.Round(Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]), 3));
							cGrp1.StringFormat = UnitsInformation.Get(WebCste.CustomerSessions.Unit.grp).StringFormat;
							resultTable[lineIndex, APPMConstantes.GRP_COLUMN_INDEX] = cGrp1;
							if (grpBaseTarget != 0) {
								CellAffinity cAf4 = new CellAffinity(Math.Round((Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]) / grpBaseTarget) * 100, 3));
								cAf4.StringFormat = affinityFormat;
								resultTable[lineIndex, APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX] = cAf4;
							}
							else {
								CellAffinity cAf5 = new CellAffinity(0);
								cAf5.StringFormat = affinityFormat;
								resultTable[lineIndex, APPMConstantes.AFFINITIES_GRP_COLUMN_INDEX] = cAf5;
							}

                            if (Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]) != 0) {
								CellCGRP cCgrp = new CellCGRP(Math.Round(Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]) / Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]), 3));
								cCgrp.StringFormat = cGrpFormat;
								resultTable[lineIndex, APPMConstantes.CGRP_COLUMN_INDEX] = cCgrp;
                                cgrp = Math.Round(Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.euro].Id.ToString()]) / Convert.ToDouble(row[UnitsInformation.List[WebCste.CustomerSessions.Unit.grp].DatabaseField]), 3);
							}
							else{
								CellCGRP cCgrp1 = new CellCGRP(0);
								cCgrp1.StringFormat = cGrpFormat;
								resultTable[lineIndex, APPMConstantes.CGRP_COLUMN_INDEX] = cCgrp1;
								cgrp=0;
							}

							if (cgrpBaseTarget != 0) {
								CellAffinity cAf6 = new CellAffinity(Math.Round((cgrp / cgrpBaseTarget) * 100, 3));
								cAf6.StringFormat = affinityFormat;
								resultTable[lineIndex, APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX] = cAf6;
							}
							else {
								CellAffinity cAf7 = new CellAffinity(0);
								cAf7.StringFormat = affinityFormat;
								resultTable[lineIndex, APPMConstantes.AFFINITIES_CGRP_COLUMN_INDEX] = cAf7;
							}
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
