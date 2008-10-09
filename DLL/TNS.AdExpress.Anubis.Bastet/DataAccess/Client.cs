#region Informations
///////////////////////////////////////////////////////////
//  Client.cs
//  Implementation of the Class BastetDataAccess
//  Generated by Enterprise Architect
//  Created on:      17-nov.-2005 16:51:11
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
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Anubis.Bastet.DataAccess {
	/// <summary>
	/// Obtient les donn�es pour la g�n�ration du fichier excel
	/// </summary>
	public class Client {
				
				
		#region Top des clients qui se connectent		

		/// <summary>
		/// Top  des clients qui se connectent le plus
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Donn�es Top  des clients qui se connectent le plus</returns>
		public static DataTable  TopConnected(BastetCommon.Parameters parameters){
			try{
				#region Requ�te
				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table connectionByLoginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingConnectionByLogin);
				
				//select				
				sql.Append(" select sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER,sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_8_12) as CONNECTION_NUMBER_8_12");
				sql.Append(",sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_12_16) as CONNECTION_NUMBER_12_16,sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_16_20) as CONNECTION_NUMBER_16_20");
				sql.Append(",sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_20_24) as CONNECTION_NUMBER_20_24,sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_24_8) as CONNECTION_NUMBER_24_8");
				sql.Append(","+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+connectionByLoginTable.Prefix+".id_login,"+loginTable.Prefix+".login ");
				//From
				sql.Append(" from " + connectionByLoginTable.SqlWithPrefix);
				sql.Append(" ," + companyTable.SqlWithPrefix + "," + contactTable.SqlWithPrefix
					+","+addressTable.SqlWithPrefix+","+loginTable.SqlWithPrefix);
				//Where
				sql.Append(" where "+connectionByLoginTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+connectionByLoginTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+connectionByLoginTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".id_address = "+addressTable.Prefix+".id_address ");
				sql.Append(" and "+addressTable.Prefix+".id_company="+companyTable.Prefix+".id_company ");
				//Gourp by
				sql.Append(" group by  "+companyTable.Prefix+".id_company,"+companyTable.Prefix+".company,"+connectionByLoginTable.Prefix+".id_login,"+loginTable.Prefix+".login ");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc,"+loginTable.Prefix+".login ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopConnected : Impossible to get the list of most connected clients ", err));
			}
			
	
		}
		#endregion

		#region Top connections par type de client	

		/// <summary>
		/// Top  des connections par type de client 
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Donn�es Top  connections par type de client </returns>
		public static DataTable  TopTypeConnected(BastetCommon.Parameters parameters){
			try{
				#region Requ�te
				StringBuilder sql = new StringBuilder(3000);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table connectionByLoginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingConnectionByLogin);
				Table groupContactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContactGroup);

				//select
				sql.Append(" select sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER) as CONNECTION_NUMBER,sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_8_12) as CONNECTION_NUMBER_8_12");
				sql.Append(",sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_12_16) as CONNECTION_NUMBER_12_16,sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_16_20) as CONNECTION_NUMBER_16_20");
				sql.Append(",sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_20_24) as CONNECTION_NUMBER_20_24,sum("+connectionByLoginTable.Prefix+".CONNECTION_NUMBER_24_8) as CONNECTION_NUMBER_24_8");
				sql.Append(",group_contact ");
				//From
				sql.Append(" from " + connectionByLoginTable.SqlWithPrefix);
				sql.Append(" ,"+loginTable.SqlWithPrefix+","+contactTable.SqlWithPrefix
					+ "," + groupContactTable.SqlWithPrefix);
					
				//Where
				sql.Append(" where "+connectionByLoginTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+connectionByLoginTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+connectionByLoginTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+contactTable.Prefix+".id_group_contact(+)="+groupContactTable.Prefix+".id_group_contact ");
				sql.Append(" and "+groupContactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);			
				//Gourp by
				sql.Append(" group by group_contact");
				//Order by
				sql.Append(" order by  CONNECTION_NUMBER  desc,group_contact");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopConnected : Impossible to get most connected clients list ", err));
			}
			
	
		}
		#endregion

		#region Top connections par mois	

		/// <summary>
		/// Top connections clients par mois
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Top connections clients par mois </returns>
		public static DataTable  TopConnectedByMonth(BastetCommon.Parameters parameters){
			try{
				#region Requ�te

				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table connectionByLoginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingConnectionByLogin);
				
				//select	
				sql.Append(" select id_company,company,id_login ,login,DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER from ( ");

				sql.Append(" select "+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+connectionByLoginTable.Prefix+".id_login ,"+loginTable.Prefix+".login");
				sql.Append(",TO_NUMBER(SUBSTR(TO_CHAR(DATE_CONNECTION),1,6)) AS DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER");
				
				//From
				sql.Append(" from "+connectionByLoginTable.SqlWithPrefix);
				sql.Append(" ,"+loginTable.SqlWithPrefix+","+contactTable.SqlWithPrefix
				+","+addressTable.SqlWithPrefix+","+companyTable.SqlWithPrefix);
				//Where
				sql.Append(" where "+connectionByLoginTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+connectionByLoginTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+connectionByLoginTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+contactTable.Prefix+".id_address = "+addressTable.Prefix+".id_address ");
				sql.Append(" and "+addressTable.Prefix+".id_company="+companyTable.Prefix+".id_company ");
							
				//Gourp by
				sql.Append(" group by "+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+connectionByLoginTable.Prefix+".id_login,login,date_connection,connection_number");
				//Order by
				sql.Append(" order by  "+connectionByLoginTable.Prefix+".id_login,login,date_connection");

				sql.Append(" ) group by date_connection,id_company,company,id_login,login ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopConnectedByMonth : Impossible to get top connected client by month list ", err));
			}
			
	
		}
		#endregion

		#region Top connections par mois et par type de client	

		/// <summary>
		/// Top connections par type de clients et par mois 
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Top connections clients par mois </returns>
		public static DataTable  TopTypeConnectedByMonth(BastetCommon.Parameters parameters){
			try{
				#region Requ�te

				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table connectionByLoginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingConnectionByLogin);
				Table groupContactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContactGroup);

				//select	
				sql.Append(" select id_group_contact,group_contact,DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER from ( ");

				sql.Append(" select "+contactTable.Prefix+".id_group_contact,group_contact");
				sql.Append(",TO_NUMBER(SUBSTR(TO_CHAR(DATE_CONNECTION),1,6)) AS DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER");
				
				//From
				sql.Append(" from "+connectionByLoginTable.SqlWithPrefix);
				sql.Append(" ,"+loginTable.SqlWithPrefix+","+contactTable.SqlWithPrefix
					+","+groupContactTable.SqlWithPrefix);
					
				//Where
				sql.Append(" where "+connectionByLoginTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+connectionByLoginTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+connectionByLoginTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+contactTable.Prefix+".id_group_contact(+)="+groupContactTable.Prefix+".id_group_contact ");
				sql.Append(" and "+groupContactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
							
				//Gourp by
				sql.Append(" group by "+contactTable.Prefix+".id_group_contact,group_contact,date_connection,connection_number");
				//Order by
				sql.Append(" order by  "+contactTable.Prefix+".id_group_contact,group_contact,date_connection");

				sql.Append(" ) group by date_connection,id_group_contact,group_contact ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopTypeConnectedByMonth : Impossible d'obtenir la liste des Top connections type clients par mois ", err));
			}
			
	
		}
		#endregion

		#region Top connections par jour nomm�	

		/// <summary>
		/// Top connections clients par jour nomm�	
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Top connections clients par jour nomm�	 </returns>
		public static DataTable  TopConnectedByDay(BastetCommon.Parameters parameters){
			try{
				#region Requ�te

				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table connectionByLoginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingConnectionByLogin);
				
				//select	
				sql.Append(" select id_company,company,id_login ,login,DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER from ( ");

				sql.Append(" select "+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+connectionByLoginTable.Prefix+".id_login ,"+loginTable.Prefix+".login");
				sql.Append(",TO_NUMBER(TO_CHAR(TO_DATE(DATE_CONNECTION,'YYYY-MM-DD'),'D')) AS DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER");
				
				//From
				//From
				sql.Append(" from " + connectionByLoginTable.SqlWithPrefix);
				sql.Append(" ," + loginTable.SqlWithPrefix + "," + contactTable.SqlWithPrefix
				+ "," + addressTable.SqlWithPrefix + "," + companyTable.SqlWithPrefix);
				//Where
				sql.Append(" where "+connectionByLoginTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+connectionByLoginTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+connectionByLoginTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+contactTable.Prefix+".id_address = "+addressTable.Prefix+".id_address ");
				sql.Append(" and "+addressTable.Prefix+".id_company="+companyTable.Prefix+".id_company ");
							
				//Gourp by
				sql.Append(" group by "+companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+connectionByLoginTable.Prefix+".id_login,login,date_connection,connection_number");
				//Order by
				sql.Append(" order by  "+connectionByLoginTable.Prefix+".id_login,login,date_connection");

				sql.Append(" ) group by date_connection,id_company,company,id_login,login ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopConnectedByMonth : Impossible d'obtenir la liste des Top connections clients par mois ", err));
			}
			
	
		}
		#endregion

		#region Top connections par jour nomm� et par type de client	

		/// <summary>
		/// Top connections par type de clients et par jour nomm� 
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Top connections type de clients et par jour nomm� </returns>
		public static DataTable  TopTypeConnectedByDay(BastetCommon.Parameters parameters){
			try{
				#region Requ�te

				StringBuilder sql = new StringBuilder(3000);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table connectionByLoginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingConnectionByLogin);
				Table groupContactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContactGroup);

				//select	
				sql.Append(" select id_group_contact,group_contact,DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER from ( ");

				sql.Append(" select "+contactTable.Prefix+".id_group_contact,group_contact");
				sql.Append(",TO_NUMBER(TO_CHAR(TO_DATE(DATE_CONNECTION,'YYYY-MM-DD'),'D')) AS DATE_CONNECTION,sum(CONNECTION_NUMBER) as CONNECTION_NUMBER");
				
				//From
				//From
				sql.Append(" from " + connectionByLoginTable.SqlWithPrefix);
				sql.Append(" ," + loginTable.SqlWithPrefix + "," + contactTable.SqlWithPrefix
				+ "," + groupContactTable.SqlWithPrefix );
				
				//Where
				sql.Append(" where "+connectionByLoginTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+connectionByLoginTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+connectionByLoginTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+contactTable.Prefix+".id_group_contact(+)="+groupContactTable.Prefix+".id_group_contact ");
				sql.Append(" and "+groupContactTable.Prefix+".activation<"+DBConstantes.ActivationValues.UNACTIVATED);
							
				//Gourp by
				sql.Append(" group by "+contactTable.Prefix+".id_group_contact,group_contact,date_connection,connection_number");
				//Order by
				sql.Append(" order by  "+contactTable.Prefix+".id_group_contact,group_contact,date_connection");

				sql.Append(" ) group by date_connection,id_group_contact,group_contact ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" TopTypeConnectedByDay : Impossible to get the list of Top connected clients type  named day. ", err));
			}
			
	
		}
		#endregion

		#region IP par client	

		/// <summary>
		/// IP par client
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>Donn�es IP par client</returns>
		public static DataTable  IPAddress(BastetCommon.Parameters parameters){
			try{
				#region Requ�te
				StringBuilder sql = new StringBuilder(3000);
				Table companyTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightCompany);
				Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
				Table addressTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightAddress);
				Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				Table ipLoginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.trackingLoginIp);
				
				//select
				sql.Append(" select ");				
				sql.Append(companyTable.Prefix+".id_company ,"+companyTable.Prefix+".company,"+ipLoginTable.Prefix+".id_login,"+loginTable.Prefix+".login,IP_ADDRESS ");
				//From
				sql.Append(" from " + ipLoginTable.SqlWithPrefix);
				sql.Append(" ,"+loginTable.SqlWithPrefix+","+contactTable.SqlWithPrefix
					+","+addressTable.SqlWithPrefix+","+companyTable.SqlWithPrefix);
				//Where
				sql.Append(" where "+ipLoginTable.Prefix+".date_connection  between "+parameters.PeriodBeginningDate+" and "+parameters.PeriodEndDate);
				if(parameters!=null && parameters.Logins.Length>0)
					sql.Append(" and "+ipLoginTable.Prefix+".id_login in ("+parameters.Logins+") ");
				sql.Append(" and "+loginTable.Prefix+".id_login="+ipLoginTable.Prefix+".id_login ");
				sql.Append(" and "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");
				sql.Append(" and "+contactTable.Prefix+".id_address = "+addressTable.Prefix+".id_address ");
				sql.Append(" and "+addressTable.Prefix+".id_company="+companyTable.Prefix+".id_company ");
				//Gourp by
				sql.Append(" group by  "+companyTable.Prefix+".id_company,"+companyTable.Prefix+".company,"+ipLoginTable.Prefix+".id_login,"+loginTable.Prefix+".login,IP_ADDRESS");
				//Order by
				sql.Append(" order by  "+companyTable.Prefix+".company,"+loginTable.Prefix+".login ");
				#endregion
				
				#region Execution
		
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
				#endregion
			}
			catch(System.Exception err){
				throw (new AnubisBastet.Exceptions.BastetDataAccessException(" IPAddress : Impossible to get clients IP list. ", err));
			}
			
	
		}
		#endregion

		#region Nom des clients		

		/// <summary>
		/// Noms clients
		/// </summary>
		/// <param name="parameters">parametres</param>
		/// <returns>nom clients</returns>
		public static DataTable  Name(BastetCommon.Parameters parameters,string idlogins){

			#region Requ�te
			StringBuilder sql = new StringBuilder(3000);
			Table contactTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightContact);
			Table loginTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.rightLogin);
				
			//select
			sql.Append(" select "+loginTable.Prefix+".id_login,"+loginTable.Prefix+".login,"+contactTable.Prefix+".FIRST_NAME ,"+contactTable.Prefix+".NAME ");			
			//From
			sql.Append(" from ");
			sql.Append(" "+loginTable.SqlWithPrefix+","+contactTable.SqlWithPrefix);
			//Where
			sql.Append(" where ");
			if(idlogins.Length>0){
				sql.Append("  "+loginTable.Prefix+".id_login in ("+idlogins+") ");
				sql.Append("  and ");
			}
			sql.Append("  "+loginTable.Prefix+".id_contact="+contactTable.Prefix+".id_contact ");			
			//Gourp by
			sql.Append(" group by  "+loginTable.Prefix+".id_login,"+loginTable.Prefix+".login,"+contactTable.Prefix+".FIRST_NAME,"+contactTable.Prefix+".NAME");
			

			#endregion
				
			#region Execution
			try{
				return(parameters.Source.Fill(sql.ToString()).Tables[0]);
			}
			catch(System.Exception err){
				throw (new Exception(" TopConnected : Impossible to get names of most connected clients "+ err));
			}
			#endregion
	
		}
		#endregion

		

	}//end BastetDataAccess

}//end namespace DataAccess