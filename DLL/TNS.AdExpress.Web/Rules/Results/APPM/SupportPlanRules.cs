#region Infomration
/*
Author ; G. RAGNEAU
Creation : 27/07/2005
Last Modification : 
*/
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;

using TNS.FrameWork.DB.Common;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;

using DataAccesFct = TNS.AdExpress.Web.DataAccess;
using TNS.AdExpress.Domain.Units;
using WebConstante = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Rules.Results.APPM {

	class TargetColumns{
		/// <summary>
		/// GRP Column index for a target
		/// </summary>
		public int _grpIndex = -1;
		/// <summary>
		/// PDM Column index for a target
		/// </summary>
		public int _pdmIndex = -1;
		/// <summary>
		/// CGRP Column index for a target
		/// </summary>
		public int _cgrpIndex = -1;

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="grp">grp</param>
		/// <param name="pdm">pdm</param>
		/// <param name="cgrp">cout grp</param>
		public TargetColumns(int grp, int pdm, int cgrp){
			_grpIndex = grp;
			_pdmIndex = pdm;
			_cgrpIndex = cgrp;
		}
		#endregion
	}

	/// <summary>
	/// Process data for the APPM module, tab "Valorisation and efficiency by media"
	/// </summary>
	public class SupportPlanRules {

		/// <summary>
		/// Process data from database so as to get the desired information
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User Session</param>
		/// <param name="dateBegin">Period beginning</param>
		/// <param name="dateEnd">Period end</param>
		/// <param name="idBaseTarget">Default target</param>
		/// <param name="idAdditionaleTarget">Additionnale target</param>
		/// <param name="idWave">Study wave</param>
		/// <returns>DataSet ready to be displayed</returns>
		public static DataTable GetData(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idAdditionaleTarget, Int64 idWave){

			#region Variables
			Int64 idOldVeh = -1;
			DataRow oldVh = null;
			Int64 idOldCat = -1;
			DataRow oldCt = null;
			Int64 idOldMedia = -1;
			DataRow oldMedia = null;
			Decimal tmp = 0;
			#endregion

			DataTable data = DataAccesFct.Results.APPM.SupportPlanDataAccess.GetData(dataSource, webSession , dateBegin, dateEnd, idBaseTarget, idAdditionaleTarget, idWave).Tables[0];

			DataTable dtResult = new DataTable();
			dtResult.Columns.Add("rowType", System.Type.GetType("System.String"));
			dtResult.Columns.Add("idMedia", System.Type.GetType("System.Int64"));
			dtResult.Columns.Add("label", System.Type.GetType("System.String"));
			dtResult.Columns.Add("budget", System.Type.GetType("System.Decimal")).Caption = GestionWeb.GetWebWord(1682, webSession.SiteLanguage);
			dtResult.Columns.Add("PDM", System.Type.GetType("System.Decimal")).Caption = GestionWeb.GetWebWord(1682, webSession.SiteLanguage);
			
			DataTable dtTargets = DataAccesFct.Functions.SelectDistinct("targets",data,new string[2]{"id_target","target"});
			
			Hashtable targetsCol = new Hashtable(2);

			int i = 5;
			int j = 1;
			foreach(DataRow row in dtTargets.Rows){
				dtResult.Columns.Add("GRP" + j,System.Type.GetType("System.Decimal")).Caption = row[1].ToString();
				dtResult.Columns.Add("PDM" + j,System.Type.GetType("System.Decimal")).Caption = row[1].ToString();
				dtResult.Columns.Add("C/GRP" + j,System.Type.GetType("System.Decimal")).Caption = row[1].ToString();
				targetsCol.Add(row[0], new TargetColumns(i,i+1,i+2));
				i += 3;
				j++;
			}


			TargetColumns t = null;

			foreach(DataRow row in data.Rows){

				//new vehicle ?
				if(int.Parse(row["id_vehicle"].ToString()) != idOldVeh){
					//Calcul GRP, budget, 
					idOldVeh = int.Parse(row["id_vehicle"].ToString());
					oldVh = dtResult.NewRow();
					dtResult.Rows.Add(oldVh);
					oldVh["rowType"] = CustomerCst.Right.type.vehicleAccess.ToString();
					oldVh["label"] = "Total";//row["vehicle"].ToString();
                    oldVh["budget"] = Decimal.Parse(data.Compute("sum(" + UnitsInformation.List[WebConstante.CustomerSessions.Unit.euro].Id.ToString() + ")", "id_vehicle=" + idOldVeh + " and id_target=" + dtTargets.Rows[0][0].ToString()).ToString());
					foreach(object obj in targetsCol.Keys){
						t = (TargetColumns)targetsCol[obj];
						tmp = Decimal.Parse(data.Compute("sum(totalgrp)","id_vehicle=" + idOldVeh + " and id_target=" + obj.ToString()).ToString());
						oldVh[t._grpIndex] = tmp;
						if (tmp != 0){
							oldVh[t._cgrpIndex] = Decimal.Parse(oldVh["budget"].ToString()) / tmp;
						}
						else{
							oldVh[t._cgrpIndex] = 0;
						}
						oldVh[t._pdmIndex] = 100;
					}
					oldVh["pdm"] = 100;
					idOldCat = -1;
					idOldMedia = -1;
				}

				//new category ?
				if(int.Parse(row["id_category"].ToString()) != idOldCat){
					//Calcul GRP, budget, 
					idOldCat = int.Parse(row["id_category"].ToString());
					oldCt = dtResult.NewRow();
					dtResult.Rows.Add(oldCt);
                    oldCt["rowType"] = CustomerCst.Right.type.categoryAccess.ToString();
					oldCt["label"] = row["category"].ToString();
                    oldCt["budget"] = int.Parse(data.Compute("sum(" + UnitsInformation.List[WebConstante.CustomerSessions.Unit.euro].Id.ToString() + ")", "id_vehicle=" + idOldVeh + " and id_category=" + idOldCat + " and id_target=" + dtTargets.Rows[0][0].ToString()).ToString());
					foreach(object obj in targetsCol.Keys){
						t = (TargetColumns)targetsCol[obj];
						tmp = Decimal.Parse(data.Compute("sum(totalgrp)","id_vehicle=" + idOldVeh + " and id_category=" + idOldCat + " and id_target=" + obj.ToString()).ToString());
						oldCt[t._grpIndex] = tmp;
						if (tmp != 0){
							oldCt[t._cgrpIndex] = Decimal.Parse(oldCt["budget"].ToString()) / tmp;
						}
						else{
							oldCt[t._cgrpIndex] = 0;
						}
						if(Decimal.Parse(oldVh[t._grpIndex].ToString())>0)
						oldCt[t._pdmIndex] = 100 * Decimal.Parse(oldCt[t._grpIndex].ToString()) / Decimal.Parse(oldVh[t._grpIndex].ToString());
						else oldCt[t._cgrpIndex] = 0;
					}
					if(Decimal.Parse(oldVh["budget"].ToString())>0)
					oldCt["pdm"] = 100 * Decimal.Parse(oldCt["budget"].ToString()) / Decimal.Parse(oldVh["budget"].ToString());
					else oldCt["pdm"] =  0;
					idOldMedia = -1;
				}
				
				//new media ?
				if(int.Parse(row["id_media"].ToString()) != idOldMedia){
					//Calcul budget, PDM
					idOldMedia = int.Parse(row["id_media"].ToString());
					oldMedia = dtResult.NewRow();
					dtResult.Rows.Add(oldMedia);
                    oldMedia["rowType"] = CustomerCst.Right.type.mediaAccess.ToString();
					oldMedia["idMedia"] = idOldMedia;
					oldMedia["label"] = row["media"].ToString();
                    oldMedia["budget"] = int.Parse(row[UnitsInformation.List[WebConstante.CustomerSessions.Unit.euro].Id.ToString()].ToString());
					if(Decimal.Parse(oldCt["budget"].ToString())>0)
					oldMedia["pdm"] = 100 * Decimal.Parse(oldMedia["budget"].ToString()) / Decimal.Parse(oldCt["budget"].ToString());
					else oldMedia["pdm"] = 0;
				}

				//target management
				t = (TargetColumns)targetsCol[Int64.Parse(row["id_target"].ToString())];
				oldMedia[t._grpIndex] = Decimal.Parse(row["totalgrp"].ToString());
				oldMedia[t._cgrpIndex] = ((tmp = Decimal.Parse(row["cgrp"].ToString()))!=0)?tmp:0;
				if(Decimal.Parse(oldCt[t._grpIndex].ToString())>0)
				oldMedia[t._pdmIndex] = 100 * Decimal.Parse(oldMedia[t._grpIndex].ToString()) / Decimal.Parse(oldCt[t._grpIndex].ToString());
				else oldMedia[t._pdmIndex] = 0;

			}

			return dtResult;

		}

	}
}
