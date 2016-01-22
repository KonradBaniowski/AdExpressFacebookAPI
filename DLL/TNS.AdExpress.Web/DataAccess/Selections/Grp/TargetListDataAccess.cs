#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 29/06/2005 
// Date de modification: 29/06/2005 
#endregion

using System;
using System.Data;

using Oracle.DataAccess.Client;

using DBCst = TNS.AdExpress.Constantes.DB;
using FwkSelection=TNS.AdExpress.Constantes.FrameWork.Selection;

using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using WebDataAccess=TNS.AdExpress.Web.DataAccess;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.DataAccess.Selections.Grp;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.DataAccess.Selections.Grp
{
	/// <summary>
	/// Traite les cibles AEPM  
	/// </summary>
	public class TargetListDataAccess
	{
		/// <summary>
		/// Liste des cibles AEPM
		/// </summary>
		/// <param name="idWave">identifiant(s) de(s) vague(s)</param>
		/// <param name="connectionString">Chaîne de connexion</param>
		/// <returns>liste des cibles AEPM</returns>
		public static DataSet GetAEPMTargetListDataAccess(string idWave,IDataSource source){
			string sql="";

			sql= " select "+DBCst.Tables.TARGET_PREFIXE+".id_target,"+DBCst.Tables.TARGET_PREFIXE+".target";
			sql+=" from "+DBCst.Schema.APPM_SCHEMA+"."+DBCst.Tables.TARGET+" "+DBCst.Tables.TARGET_PREFIXE;			
			sql+=" Where  "+DBCst.Tables.TARGET_PREFIXE+".activation="+DBCst.ActivationValues.ACTIVATED;
			if(WebFunctions.CheckedText.IsStringEmpty(idWave))
				sql+="  and "+DBCst.Tables.TARGET_PREFIXE+".id_wave in ("+idWave+")";
			sql+=" order by "+DBCst.Tables.TARGET_PREFIXE+".target asc";
			return source.Fill(sql);
		}

		/// <summary>
		/// Libellé de cibles AEPM
		/// </summary>
		/// <param name="idWave">identifiant(s) de(s) vague(s)</param>
		/// <param name="targets">Libellés cibles (séparés par une virgule)</param>
		/// <param name="connectionString">Chaîne de connection</param>
		/// <returns>Libellé des cibles AEPM</returns>
		public static DataSet GetAEPMTargetListDataAccess(string idWave,string targets,IDataSource source){

			string sql="";

			sql= " select "+DBCst.Tables.TARGET_PREFIXE+".id_target,"+DBCst.Tables.TARGET_PREFIXE+".target";
			sql+=" from "+DBCst.Schema.APPM_SCHEMA+"."+DBCst.Tables.TARGET+" "+DBCst.Tables.TARGET_PREFIXE;			
			sql+=" Where  "+DBCst.Tables.TARGET_PREFIXE+".activation="+DBCst.ActivationValues.ACTIVATED;
			if(WebFunctions.CheckedText.IsStringEmpty(idWave))
				sql+="  and "+DBCst.Tables.TARGET_PREFIXE+".id_wave in ("+idWave+")";
			if(WebFunctions.CheckedText.IsStringEmpty(targets))
				sql+= " and "+DBCst.Tables.TARGET_PREFIXE+".target in ('"+targets+"')";
			sql+=" order by "+DBCst.Tables.TARGET_PREFIXE+".target asc";

			try{
				return (source.Fill(sql));
			}
			catch(System.Exception e){
				throw(e);
			}

		}

		/// <summary>
		/// Libellé de cibles AEPM
		/// </summary>
		/// <param name="idWave">identifiant(s) de(s) vague(s)</param>
		/// <param name="targets">identetifiants cibles (séparés par une virgule)</param>
		/// <param name="connectionString">Chaîne de connection</param>
		/// <returns>Libellé des cibles AEPM</returns>
		public static DataSet GetAEPMTargetListFromIDSDataAccess(string idWave,string targets,IDataSource source){

			string sql="";

			sql= " select "+DBCst.Tables.TARGET_PREFIXE+".id_target,"+DBCst.Tables.TARGET_PREFIXE+".target";
			sql+=" from "+DBCst.Schema.APPM_SCHEMA+"."+DBCst.Tables.TARGET+" "+DBCst.Tables.TARGET_PREFIXE;			
			sql+=" Where  "+DBCst.Tables.TARGET_PREFIXE+".activation="+DBCst.ActivationValues.ACTIVATED;
			if(WebFunctions.CheckedText.IsStringEmpty(idWave))
				sql+="  and "+DBCst.Tables.TARGET_PREFIXE+".id_wave in ("+idWave+")";
			if(WebFunctions.CheckedText.IsStringEmpty(targets))
				sql+= " and "+DBCst.Tables.TARGET_PREFIXE+".id_target in ("+targets+")";
			sql+=" order by "+DBCst.Tables.TARGET_PREFIXE+".id_target asc";

			try{
				return source.Fill(sql);
			}
			catch(System.Exception e){
				throw(e);
			}

		}

	}
}
