#region Informations
///////////////////////////////////////////////////////////
//  Client.cs
//  Implementation of the Class BastetDataAccess
//  Generated by Enterprise Architect
//  Created on:      01-mar.-2006 11:51:11
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
using DBConstantes=TNS.AdExpress.Constantes.DB;
namespace TNS.AdExpress.Anubis.Bastet.DataAccess
{
	/// <summary>
	/// Description r�sum�e de Company.
	/// </summary>
	public class Company
	{
		#region Top des soci�t�s qui se connectent		

		/// <summary>
		/// Top  des soci�t�s qui se connectent le plus
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Donn�es Top  des soci�t�s qui se connectent le plus</returns>
		public static DataTable  TopConnected(BastetCommon.Parameters parameters){
			try{
				#region Requ�te
				StringBuilder sql = new StringBuilder(3000);
				//select
				sql.Append(" select sum("+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".CONNECTION_NUMBER) as CONNECTION_NUMBER,sum("+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".CONNECTION_NUMBER_8_12) as CONNECTION_NUMBER_8_12");
				sql.Append(",sum("+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".CONNECTION_NUMBER_12_16) as CONNECTION_NUMBER_12_16,sum("+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".CONNECTION_NUMBER_16_20) as CONNECTION_NUMBER_16_20");
				sql.Append(",sum("+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".CONNECTION_NUMBER_20_24) as CONNECTION_NUMBER_20_24,sum("+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".CONNECTION_NUMBER_24_8) as CONNECTION_NUMBER_24_8");
				sql.Append(","+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,group_contact");
				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".CONNECTION_BY_LOGIN "+DBTables.CONNECTION_BY_LOGIN_PREFIXE);
				sql.Append(" ,"+DBSchema.LOGIN_SCHEMA+".LOGIN "+DBTables.LOGIN_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".CONTACT "+DBTables.CONTACT_PREFIXE
					+","+DBSchema.LOGIN_SCHEMA+".GROUP_CONTACT "+DBTables.GROUP_CONTACT_PREFIXE
					+","+DBSchema.LOGIN_SCHEMA+".ADDRESS "+DBTables.ADDRESS_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".COMPANY "+DBTables.COMPANY_PREFIXE);
				//Where
				sql.Append(" where "+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_login="+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".id_login ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_contact="+DBTables.CONTACT_PREFIXE+".id_contact ");
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".id_group_contact(+)="+DBTables.GROUP_CONTACT_PREFIXE+".id_group_contact ");
				sql.Append(" and "+DBTables.GROUP_CONTACT_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".id_address = "+DBTables.ADDRESS_PREFIXE+".id_address ");
				sql.Append(" and "+DBTables.ADDRESS_PREFIXE+".id_company="+DBTables.COMPANY_PREFIXE+".id_company ");
				//Gourp by
				sql.Append(" group by  "+DBTables.COMPANY_PREFIXE+".id_company,"+DBTables.COMPANY_PREFIXE+".company,group_contact");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc,company,group_contact");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopConnected : Impossible d'obtenir la liste des soci�t�s les plus connect�s ", err));
			}
			
	
		}
		#endregion

		#region Top connections par mois	

		/// <summary>
		/// Top connections soci�t�s par mois
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Top connections clients par mois </returns>
		public static DataTable  TopConnectedByMonth(BastetCommon.Parameters parameters){
			try{
				#region Requ�te

				StringBuilder sql = new StringBuilder(3000);

				//select	
				sql.Append(" select id_company,company,group_contact,DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER from ( ");

				sql.Append(" select "+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,group_contact ");
				sql.Append(",TO_NUMBER(SUBSTR(TO_CHAR(DATE_CONNECTION),1,6)) AS DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER");
				
				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".CONNECTION_BY_LOGIN "+DBTables.CONNECTION_BY_LOGIN_PREFIXE);
				sql.Append(" ,"+DBSchema.LOGIN_SCHEMA+".LOGIN "+DBTables.LOGIN_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".CONTACT "+DBTables.CONTACT_PREFIXE
					+","+DBSchema.LOGIN_SCHEMA+".ADDRESS "+DBTables.ADDRESS_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".COMPANY "+DBTables.COMPANY_PREFIXE
				+","+DBSchema.LOGIN_SCHEMA+".GROUP_CONTACT "+DBTables.GROUP_CONTACT_PREFIXE);
				//Where
				sql.Append(" where "+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_login="+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".id_login ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_contact="+DBTables.CONTACT_PREFIXE+".id_contact ");
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".id_group_contact(+)="+DBTables.GROUP_CONTACT_PREFIXE+".id_group_contact ");
				sql.Append(" and "+DBTables.GROUP_CONTACT_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".id_address = "+DBTables.ADDRESS_PREFIXE+".id_address ");
				sql.Append(" and "+DBTables.ADDRESS_PREFIXE+".id_company="+DBTables.COMPANY_PREFIXE+".id_company ");
							
				//Gourp by
				sql.Append(" group by "+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,group_contact,date_connection,connection_number");
				//Order by
				sql.Append(" order by  "+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,date_connection");

				sql.Append(" ) group by date_connection,id_company,company,group_contact ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopConnectedByMonth : Impossible d'obtenir la liste des Top connections soci�t�s par mois ", err));
			}
			
	
		}
		#endregion

		#region Top connections par jour nomm�	

		/// <summary>
		/// Top connections soci�t�s par jour nomm�	
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Top connections clients par jour nomm�	 </returns>
		public static DataTable  TopConnectedByDay(BastetCommon.Parameters parameters){
			try{
				#region Requ�te

				StringBuilder sql = new StringBuilder(3000);

				//select	
				sql.Append(" select id_company,company,group_contact,DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER from ( ");

				sql.Append(" select "+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,group_contact ");
				sql.Append(",TO_NUMBER(TO_CHAR(TO_DATE(DATE_CONNECTION,'YYYY-MM-DD'),'D')) AS DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER");
				
				//From
				sql.Append(" from "+DBSchema.UNIVERS_SCHEMA+".CONNECTION_BY_LOGIN "+DBTables.CONNECTION_BY_LOGIN_PREFIXE);
				sql.Append(" ,"+DBSchema.LOGIN_SCHEMA+".LOGIN "+DBTables.LOGIN_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".CONTACT "+DBTables.CONTACT_PREFIXE
					+","+DBSchema.LOGIN_SCHEMA+".ADDRESS "+DBTables.ADDRESS_PREFIXE+","+DBSchema.LOGIN_SCHEMA+".COMPANY "+DBTables.COMPANY_PREFIXE
					+","+DBSchema.LOGIN_SCHEMA+".GROUP_CONTACT "+DBTables.GROUP_CONTACT_PREFIXE);
				//Where
				sql.Append(" where "+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_login="+DBTables.CONNECTION_BY_LOGIN_PREFIXE+".id_login ");
				sql.Append(" and "+DBTables.LOGIN_PREFIXE+".id_contact="+DBTables.CONTACT_PREFIXE+".id_contact ");
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".id_group_contact(+)="+DBTables.GROUP_CONTACT_PREFIXE+".id_group_contact ");
				sql.Append(" and "+DBTables.GROUP_CONTACT_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+DBTables.CONTACT_PREFIXE+".id_address = "+DBTables.ADDRESS_PREFIXE+".id_address ");
				sql.Append(" and "+DBTables.ADDRESS_PREFIXE+".id_company="+DBTables.COMPANY_PREFIXE+".id_company ");
							
				//Gourp by
				sql.Append(" group by "+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,group_contact,date_connection,connection_number");
				//Order by
				sql.Append(" order by  "+DBTables.COMPANY_PREFIXE+".id_company ,"+DBTables.COMPANY_PREFIXE+".company,date_connection");

				sql.Append(" ) group by date_connection,id_company,company,group_contact ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopConnectedByDay : Impossible d'obtenir la liste des Top connections soci�t�s par jour nomm� ", err));
			}
			
	
		}
		#endregion
	}
}