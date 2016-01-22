#region Information
// Auteur: K.Shehzad
// Date de création: 21/04/2005
// 29/11/2005 Par B.Masson > Mise en place de IDataSource pour Execution de la requête
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using RightCst = TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationCst = TNS.AdExpress.Constantes.Classification.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// This class is used to get a list of sectors with the help of groups that are being selected.
	/// </summary>
	public class SectorsSelectedDataAccess{
	
		#region GetData
		/// <summary>
		/// Method that is being called by the SectoresSelectedBusinessFacade class to get the dataset.
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <returns>Dataset that carries the sectors that are being selected</returns>
		public static DataSet getData(WebSession webSession){
			
			#region Variables
			StringBuilder sql = new StringBuilder(2000);
			DataSet ds=new DataSet();
			string groupList ="";
			string segmentList ="";
			List<NomenclatureElementsGroup> groups = webSession.PrincipalProductUniverses[0].GetIncludes();
			if (groups != null && groups.Count > 0) {
				groupList = groups[0].GetAsString(TNSClassificationLevels.GROUP_);
				if (groupList == null) groupList = ""; 
				segmentList = groups[0].GetAsString(TNSClassificationLevels.SEGMENT);
				if (segmentList == null) segmentList = "";
			}
			//string groupList = webSession.GetSelection(webSession.SelectionUniversProduct, RightCst.type.groupAccess);
			//string segmentList = webSession.GetSelection(webSession.SelectionUniversProduct, RightCst.type.segmentAccess);
			if (groupList.Length < 1 && segmentList.Length < 1) {
				ds=null;
				return ds;
			}
			#endregion

			#region Construction de la requête
			try{
				QueryConstructor(webSession, sql, groupList,segmentList);
			}
			catch(System.Exception e){
				throw(new WebExceptions.SectorsSelectedDataAccessException("Impossible de consruit un query "+ e.Message));
			}
			#endregion

			#region Execution de la requête
			try{
                IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].NlsSort); 
                ds = source.Fill(sql.ToString());
				return (ds);
			}
			catch(System.Exception err){
				throw (new WebExceptions.SectorsSelectedDataAccessException("Impossible de charger une liste de Famille pour le rappel de la sélection",err));
			}
			#endregion

			#region Ancien code
//
//			#region Execution de la requête
//			//OracleConnection connection=webSession.CustomerLogin.Connection;
//			OracleConnection connection = new OracleConnection(DBCst.Connection.RECAP_CONNECTION_STRING);
//			OracleCommand sqlCommand=null;
//			OracleDataAdapter sqlAdapter=null;
//
//			#region Ouverture de la base de données
//			bool DBToClosed=false;
//			// On teste si la base est déjà ouverte
//			if (connection.State==System.Data.ConnectionState.Closed) 
//			{
//				DBToClosed=true;
//				try 
//				{
//					connection.Open();
//				}
//				catch(System.Exception et) 
//				{
//					throw(new WebExceptions.SectorsSelectedDataAccessException("Impossible d'ouvrir la base de données:"+et.Message));
//				}
//			}
//			#endregion
//
//			#region Execution
//			try 
//			{
//				sqlCommand=new OracleCommand(sql.ToString(),connection);
//				sqlAdapter=new OracleDataAdapter(sqlCommand);
//				sqlAdapter.Fill(ds);
//			}
//				#endregion
//
//			#region Traitement d'erreur du chargement des données
//			catch(System.Exception ex) {
//				try {
//					// Fermeture de la base de données
//					if (sqlAdapter!=null) {
//						sqlAdapter.Dispose();
//					}
//					if(sqlCommand!=null) sqlCommand.Dispose();
//					if (DBToClosed) connection.Close();
//				}
//				catch(System.Exception et){
//					throw(new WebExceptions.SectorsSelectedDataAccessException("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
//				}
//				throw(new WebExceptions.SectorsSelectedDataAccessException("Impossible de charger les données:"+sql+" "+ex.Message));
//			}
//			#endregion
//
//			#region Fermeture de la base de données
//			try 
//			{
//				// Fermeture de la base de données
//				if (sqlAdapter!=null) 
//				{
//					sqlAdapter.Dispose();
//				}
//				if(sqlCommand!=null)sqlCommand.Dispose();
//				if (DBToClosed) connection.Close();
//			}
//			catch(System.Exception et) 
//			{
//				throw(new WebExceptions.SectorsSelectedDataAccessException("Impossible de fermer la base de données :"+et.Message));
//			}
//			#endregion
//
//			#endregion	
//
//			return ds;

			#endregion

		}
		#endregion

		#region Méthodes Privées
			
		#region QueryConstructor
		/// <summary>
		/// to make the query that gets sectors using groups
		/// </summary>
		/// <param name="webSession">Session of the client</param>
		/// <param name="sql">The query string which carries the query</param>
		/// <param name="gList">String that carries the list of groups selected</param>
		/// <param name="sList">String that carries the list of segments selected</param>
		private static void QueryConstructor(WebSession webSession, StringBuilder sql, string gList,string sList){
			// The query that is being used to collect sectors from the database using selected groups.
			sql.Append("Select distinct ");

			sql.Append(DBCst.Tables.SECTOR_PREFIXE+".ID_SECTOR, "+DBCst.Tables.SECTOR_PREFIXE+".SECTOR ");

			sql.Append("from "+DBClassificationCst.Table.name.sector+" "+ DBCst.Tables.SECTOR_PREFIXE+", "+ DBClassificationCst.Table.name.subsector+" "+ DBCst.Tables.SUBSECTOR_PREFIXE+", "+ DBClassificationCst.Table.name.group_+" "+ DBCst.Tables.GROUP_PREFIXE+", "+ DBClassificationCst.Table.name.segment+" "+ DBCst.Tables.SEGMENT_PREFIXE);

			sql.Append(" where "+DBCst.Tables.SECTOR_PREFIXE+".ID_SECTOR="+DBCst.Tables.SUBSECTOR_PREFIXE+".ID_SECTOR");
			sql.Append(" and "+DBCst.Tables.SECTOR_PREFIXE+".ID_LANGUAGE= "+webSession.DataLanguage);
			sql.Append(" and "+DBCst.Tables.SUBSECTOR_PREFIXE+".ID_SUBSECTOR="+DBCst.Tables.GROUP_PREFIXE+".ID_SUBSECTOR");
			sql.Append(" and "+DBCst.Tables.GROUP_PREFIXE+".ID_LANGUAGE= "+webSession.DataLanguage);
			sql.Append(" and "+DBCst.Tables.SEGMENT_PREFIXE+".id_group_="+DBCst.Tables.GROUP_PREFIXE+".id_group_");
			sql.Append(" and "+DBCst.Tables.SEGMENT_PREFIXE+".ID_LANGUAGE= "+webSession.DataLanguage);
			string htmlTemp="";
			if(gList.Length>0){
				sql.Append(" and ("+DBCst.Tables.GROUP_PREFIXE+".ID_GROUP_ "+ " IN(" + gList + ")" );
				htmlTemp=" ) ";
			}
			if(sList.Length>0){
				if(gList.Length>0)
					sql.Append(" or ");
				else
					sql.Append(" and ");
				sql.Append(DBCst.Tables.SEGMENT_PREFIXE+".id_segment "+ " IN(" + sList + ") " );
			}
			sql.Append(htmlTemp);
		}
		#endregion

		#endregion

	}
}
