#region Informations
// Auteur: D. Mussuma 
// Date de création: 07/09/2004 
// Date de modification: 07/09/2004 
// 29/11/2005 Par B.Masson > Mise en place de IDataSource pour Execution de la requête
#endregion

using System;
using System.Collections;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Rules.Customer;
using TablesDBConstantes=TNS.AdExpress.Constantes.DB.Tables;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.DataBaseDescription;
using TNS.AdExpress.Web.Core;

namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// Description résumée de RecapProductClassificationListDataAccess.
	/// </summary>
	public class RecapProductClassificationListDataAccess{

		/// <summary>
		/// Donne une liste de Famille, classe, groupe,variété en fonction des droits du client.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Liste d'éléments de la nomenclature</returns>
		public static DataSet RecapGetDataForSectorAnalysis(WebSession webSession){
			
			#region Construction de la requête
			string sql="";
			sql+=" select "+TablesDBConstantes.SECTOR_PREFIXE+".id_sector,"+TablesDBConstantes.SECTOR_PREFIXE+".sector,"+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_subsector,"+TablesDBConstantes.SUBSECTOR_PREFIXE+".subsector,"+TablesDBConstantes.GROUP_PREFIXE+".id_group_,"+TablesDBConstantes.GROUP_PREFIXE+".group_,"+TablesDBConstantes.SEGMENT_PREFIXE+".id_segment,"+TablesDBConstantes.SEGMENT_PREFIXE+".segment";
			sql+=" from sector "+TablesDBConstantes.SECTOR_PREFIXE+", subsector "+TablesDBConstantes.SUBSECTOR_PREFIXE+",group_ "+TablesDBConstantes.GROUP_PREFIXE+",segment "+TablesDBConstantes.SEGMENT_PREFIXE+"";
			sql+=" Where";
			// Langue
			sql+=" "+TablesDBConstantes.SECTOR_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+TablesDBConstantes.SEGMENT_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			// Activation
			sql+=" and "+TablesDBConstantes.SECTOR_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and "+TablesDBConstantes.SEGMENT_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			
			// jointure
			sql+=" and "+TablesDBConstantes.SECTOR_PREFIXE+".id_sector="+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_sector";
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_subsector="+TablesDBConstantes.GROUP_PREFIXE+".id_subsector";
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".id_group_="+TablesDBConstantes.SEGMENT_PREFIXE+".id_group_";

			#region Nomenclature Produit (droits)
			//Droits en accès
			sql+=SQLGenerator.getClassificationCustomerProductRight(webSession,true);
			#endregion
			
			// Tri
			sql+=" order by "+TablesDBConstantes.SECTOR_PREFIXE+".sector,"+TablesDBConstantes.SUBSECTOR_PREFIXE+".subsector,"+TablesDBConstantes.GROUP_PREFIXE+".group_,"+TablesDBConstantes.SEGMENT_PREFIXE+".segment";

			#endregion

			#region Execution de la requête
			try{
                IDataSource source=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw (new RecapProductClassificationListDataAccessException("Impossible de charger une liste de Famille, classe, groupe,variété en fonction des droits du client",err));
			}
			#endregion

			#region Ancien code
//
//			#region Execution de la requête
//			DataSet ds=new DataSet();
//			//OracleConnection connection=webSession.CustomerLogin.Connection;
//			OracleConnection connection = new OracleConnection(TNS.AdExpress.Constantes.DB.Connection.RECAP_CONNECTION_STRING);
//			OracleCommand sqlCommand=null;
//
//			OracleDataAdapter sqlAdapter=null;
//
//			#region Ouverture de la base de données
//			bool DBToClosed=false;
//			// On teste si la base est déjà ouverte
//			if (connection.State==System.Data.ConnectionState.Closed){
//				DBToClosed=true;
//				try
//				{
//					connection.Open();
//				}
//				catch(System.Exception et)
//				{
//					throw(new RecapProductClassificationListDataAccessException("Impossible d'ouvrir la base de données:"+et.Message));
//				}
//			}
//			#endregion
//
//			try{
//				sqlCommand=new OracleCommand(sql,connection);
//				sqlAdapter=new OracleDataAdapter(sqlCommand);
//				sqlAdapter.Fill(ds);
//			}
//			#region Traitement d'erreur du chargement des données
//			catch(System.Exception ex){
//				try{
//					// Fermeture de la base de données
//					if (sqlAdapter!=null){
//						sqlAdapter.Dispose();
//					}
//					if(sqlCommand!=null) sqlCommand.Dispose();
//					if (DBToClosed) connection.Close();
//				}
//				catch(System.Exception et){
//					throw(new RecapProductClassificationListDataAccessException ("Impossible de fermer la base de données, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
//				}
//				throw(new RecapProductClassificationListDataAccessException ("Impossible de charger les données:"+sql+" "+ex.Message));
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
//				throw(new RecapProductClassificationListDataAccessException ("Impossible de fermer la base de données :"+et.Message));
//			}
//			#endregion
//
//			#endregion	
//
//			return(ds);
//
			#endregion

		}
	}
}
