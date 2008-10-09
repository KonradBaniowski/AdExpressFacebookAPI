#region Informations
///////////////////////////////////////////////////////////
//  File.cs
//  Created on:      25-nov.-2005 16:51:11
//  Original author: D.V. Mussuma
///////////////////////////////////////////////////////////
#endregion

using System;
using System.Data;
using System.Text;

using BastetCommon=TNS.AdExpress.Bastet.Common;
using DBSchema=TNS.AdExpress.Constantes.DB.Schema;
using DBTables=TNS.AdExpress.Constantes.DB.Tables;
using AnubisBastet=TNS.AdExpress.Anubis.Bastet;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Obtient top utilisations des fichiers GAD et AGM
	/// </summary>
	public class File
	{
		#region Top GAD
		/// <summary>
		///  Obtient les données du top d'utilisation du fichier GAD
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>données du top d'utilisation du fichier GAD</returns>
		public static DataTable TopGAD(BastetCommon.Parameters parameters)
		{
			#region Requête
			StringBuilder sql = new StringBuilder(3000);
			Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
			Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
			Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
			Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
			Table topGadTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopGad);

			//select
			sql.Append(" select sum("+topGadTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER ");			
			sql.Append(","+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+topGadTable.Prefix+".id_login,"+loginTable.Prefix+".login ");
			//From
			sql.Append(" from " + topGadTable.SqlWithPrefix);
			sql.Append(" ," + loginTable.SqlWithPrefix + "," + contactTable.SqlWithPrefix
				+ "," + addressTable.SqlWithPrefix + "," + companyTable.SqlWithPrefix);
			//Where
			sql.Append(" where "+topGadTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
			if(parameters!=null && parameters.Logins.Length>0)
				sql.Append(" and "+topGadTable.Prefix+".id_login in ("+parameters.Logins+") ");
			sql.Append(" and "+loginTable.Prefix+".id_login="+topGadTable.Prefix+".id_login ");
			sql.Append(" and " + loginTable.Prefix + ".id_contact=" + contactTable.Prefix + ".id_contact ");
			sql.Append(" and " + contactTable.Prefix + ".id_address = " + addressTable.Prefix + ".id_address ");
			sql.Append(" and " + addressTable.Prefix + ".id_company=" + companyTable.Prefix + ".id_company ");
			//Gourp by
			sql.Append(" group by  "+companyTable.Prefix+".id_company,cpn.company,"+topGadTable.Prefix+".id_login,"+loginTable.Prefix+".login ");
			//Order by
			sql.Append(" order by  CONNECTION_NUMBER  desc,"+loginTable.Prefix+".login ");
			#endregion

			#region Execution
			try{
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopGAD : Impossible to get clients most using GAD file ", err));
			}
			#endregion
		}
		#endregion

		#region Top AGM
		/// <summary>
		///  Obtient les données du top d'utilisation du fichier AGENCES MEDIA
		/// </summary>
		/// <param name="parameters">parametres des statistiques</param>
		/// <returns>données du top d'utilisation du fichier GAD</returns>
		public static DataTable TopAGM(BastetCommon.Parameters parameters) {
			try{
				#region Requête
				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table topMediaAgencyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingTopMediaAgency);

				//select
				sql.Append(" select sum("+topMediaAgencyTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER ");			
				sql.Append(","+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+topMediaAgencyTable.Prefix+".id_login,"+loginTable.Prefix+".login ");
				//From
				sql.Append(" from " + topMediaAgencyTable.SqlWithPrefix);
				sql.Append(" ," + loginTable.SqlWithPrefix + "," + contactTable.SqlWithPrefix
					+ "," + addressTable.SqlWithPrefix + "," + companyTable.SqlWithPrefix);
				//Where
				sql.Append(" where "+topMediaAgencyTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+topMediaAgencyTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+topMediaAgencyTable.Prefix+".id_login ");
				sql.Append(" and " + loginTable.Prefix + ".id_contact=" + contactTable.Prefix + ".id_contact ");
				sql.Append(" and " + contactTable.Prefix + ".id_address = " + addressTable.Prefix + ".id_address ");
				sql.Append(" and " + addressTable.Prefix + ".id_company=" + companyTable.Prefix + ".id_company ");
				//Gourp by
				sql.Append(" group by  "+companyTable.Prefix+".id_company,cpn.company,"+topMediaAgencyTable.Prefix+".id_login,"+loginTable.Prefix+".login ");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc,"+loginTable.Prefix+".login ");
			
				#endregion

				#region Execution
			
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopAGM : Impossible to get cutomser using most media agency file ", err));
			}
			
		}
		#endregion
	}
}
