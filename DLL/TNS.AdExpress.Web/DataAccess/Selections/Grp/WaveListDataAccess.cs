#region Informations
// Auteur: D. V. Mussuma 
// Date de création: 24/06/2005 
// Date de modification:24/06/2005 
#endregion

using System;
using System.Data;
using FwkSelection=TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using DBCst = TNS.AdExpress.Constantes.DB;
using Oracle.DataAccess.Client;
using WebDataAccess=TNS.AdExpress.Web.DataAccess;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.DB.Common;
using DateFrameWork=TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions=TNS.AdExpress.Web.Functions;

namespace TNS.AdExpress.Web.DataAccess.Selections.Grp
{
	/// <summary>
	/// Charge les vagues AEPM  et OJD 
	/// </summary>
	public class WaveListDataAccess
	{	
		/// <summary>
		/// Liste des vagues  à afficher 
		/// </summary>
		/// <param name="waveType">typde de vague</param>
		/// <param name="dataSource">source de données</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="typeDateBegin">type de date de début</param>
		/// <returns>liste des vagues</returns>
		public static DataSet GetWaveListDataAccess(FwkSelection.Wave.Type waveType,IDataSource dataSource,DateTime dateBegin,APPM.TypeDateBegin typeDateBegin)
		{
			switch(waveType){
				case FwkSelection.Wave.Type.AEPM :
					return GetAEPMWaveListDataAccess(dataSource,dateBegin,typeDateBegin);
				case FwkSelection.Wave.Type.OJD :
					return null;
				default :
					 throw new WaveListDataAccessException(" GetWaveListDataAccess(FwkSelection.Wave.Type waveType ) : impossible d'identifier le type de vague à traiter.");
			}
			
		}
		
		/// <summary>
		/// Liste des vagues AEPM
		/// </summary>
		/// <param name="dataSource">source de données</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="typeDateBegin">type de date de début</param>
		/// <returns>liste des vagues AEPM</returns>
		public static DataSet GetAEPMWaveListDataAccess(IDataSource dataSource,DateTime dateBegin,APPM.TypeDateBegin typeDateBegin){
			string sql="";

			sql= " select "+DBCst.Tables.WAVE_PREFIXE+".id_wave,"+DBCst.Tables.WAVE_PREFIXE+".wave";
			sql+=","+DBCst.Tables.WAVE_PREFIXE+".date_beginning,"+DBCst.Tables.WAVE_PREFIXE+".date_end,"+DBCst.Tables.WAVE_PREFIXE+".date_creation  ";
			sql+=" from "+DBCst.Schema.APPM_SCHEMA+"."+DBCst.Tables.WAVE+" "+DBCst.Tables.WAVE_PREFIXE;							
			sql+=" Where  "+DBCst.Tables.WAVE_PREFIXE+".activation="+DBCst.ActivationValues.ACTIVATED;
			
			#region conditions		
			sql+=DateSelection(typeDateBegin,dateBegin);			
			#endregion

			sql+=" order by "+DBCst.Tables.WAVE_PREFIXE+".date_beginning asc,"+DBCst.Tables.WAVE_PREFIXE+".date_creation asc";
			
			#region Execution de la requête
			try{
				return(dataSource.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw(new WebExceptions.WaveListDataAccessException("GetAEPMWaveListDataAccess(IDataSource dataSource):: Erreur durant le chargement des vagues AEPM. ",err));
			}		
			#endregion
		}
	

		#region Méthodes internes
		/// <summary>
		/// Période sélectionnée
		/// </summary>
		/// <param name="typeDateBegin">type de date de début</param>
		/// <param name="dateBegin">date de début</param>
		/// <returns>période sélectionnée</returns>
		private static string DateSelection(APPM.TypeDateBegin typeDateBegin,DateTime dateBegin){
			string sql="";
			switch(typeDateBegin){
				case APPM.TypeDateBegin.inSideWave : 
				sql+=" and date_beginning<=To_date('"+DateFrameWork.DateString.dateTimeToYYYYMMDD_HH24_MI_SS(dateBegin)+"','YYYYMMDD:HH24:MI:SS') ";
				sql+=" and date_end>=To_date('"+DateFrameWork.DateString.dateTimeToYYYYMMDD_HH24_MI_SS(dateBegin)+"','YYYYMMDD:HH24:MI:SS') ";
					break;
				case APPM.TypeDateBegin.outSideWave : 
				case APPM.TypeDateBegin.none : 
					
				default: 
					break;
			}

			return sql;
		}
		#endregion

	}
}
