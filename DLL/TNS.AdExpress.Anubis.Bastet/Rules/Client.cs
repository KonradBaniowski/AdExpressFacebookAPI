#region Informations
///////////////////////////////////////////////////////////
//  Client.cs
//  Implementation of the Class BastetDataAccess
//  Generated by Enterprise Architect
//  Created on:      03/03/2006
//  Original author: D.V. Mussuma
///////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;
using BastetDataAccess=TNS.AdExpress.Anubis.Bastet.DataAccess;
using BastetCommon=TNS.AdExpress.Bastet.Common;

namespace TNS.AdExpress.Anubis.Bastet.Rules
{
	/// <summary>
	/// Traite les donn�es clients pour la g�n�ration du fichier excel
	/// </summary>
	public class Client
	{

		#region Top des clients par mois
		/// <summary>
		/// Obtient le top des clients par mois.
		/// </summary>
		/// <param name="parameters">param�tres client.</param>
		/// <returns>donn�es top connections clients par mois.</returns>
        public static DataTable TopConnectedByMonth(BastetCommon.Parameters parameters, DataAccess.Client dataAccessClient) {
			
			DataTable dt=null, dtResult=null;
			Int64 oldDateConnection=0;
			ArrayList idLoginsArr = null;
			DataRow resultRow=null;
            dt = dataAccessClient.TopConnectedByMonth();
			object tempValue=null;
			string columnName="";
			if(dt!=null && dt.Rows.Count>0){				

				//Table de r�sultats
				
				dtResult = new DataTable();

				dtResult.Columns.Add("company",System.Type.GetType("System.String"));
				dtResult.Columns.Add("login",System.Type.GetType("System.String"));
				dtResult.Columns.Add("connection_number",System.Type.GetType("System.Double"));		
				foreach(DataRow dr in dt.Rows){
					if(oldDateConnection!=Int64.Parse(dr["date_connection"].ToString())){
						columnName = "month_"+dr["date_connection"].ToString();
						dtResult.Columns.Add(columnName,System.Type.GetType("System.Double"));						
					}
					
					oldDateConnection=Int64.Parse(dr["date_connection"].ToString());
				}				

				idLoginsArr = new ArrayList();
				columnName="";
				foreach(DataRow row in dt.Rows){
					if(!idLoginsArr.Contains(row["id_login"].ToString())){
						resultRow=dtResult.NewRow();							
						resultRow["company"] = row["company"].ToString();
						resultRow["login"] = row["login"].ToString();
						resultRow["connection_number"] = double.Parse(dt.Compute("sum(connection_number)","id_login="+row["id_login"].ToString()).ToString()); 
						for(int i=3;i<dtResult.Columns.Count;i++){	
							columnName=dtResult.Columns[i].ColumnName;
							columnName = columnName.Substring(6,6);
							tempValue = dt.Compute("sum(connection_number)","id_login="+row["id_login"].ToString()+" and date_connection="+columnName); 						
							if(tempValue!=System.DBNull.Value && !tempValue.Equals(""))
								resultRow[i] = double.Parse(tempValue.ToString());
							else resultRow[i] =(double)0;
						}
						dtResult.Rows.Add(resultRow);	
						idLoginsArr.Add(row["id_login"].ToString());
					}
				}
				
				//Trie tableau
				DataSet ds2=new DataSet();				
				ds2.Merge(dtResult.Select("","connection_number DESC"));
				dtResult = ds2.Tables[0];
				
			}
			
			return dtResult;
		}
		#endregion

		#region Top par type de client et par mois
		/// <summary>
		/// Obtient le top des types de clients par mois.
		/// </summary>
		/// <param name="parameters">param�tres client.</param>
		/// <returns>donn�es top connections types de clients par mois.</returns>
        public static DataTable TopTypeConnectedByMonth(BastetCommon.Parameters parameters, DataAccess.Client dataAccessClient) {
			
			DataTable dt=null, dtResult=null;
			Int64 oldDateConnection=0;
			ArrayList idGroupContactsArr = null;
			DataRow resultRow=null;
            dt = dataAccessClient.TopTypeConnectedByMonth();
			object tempValue=null;
			string columnName="";
			if(dt!=null && dt.Rows.Count>0){				

				//Table de r�sultats
				
				dtResult = new DataTable();

				dtResult.Columns.Add("id_group_contact",System.Type.GetType("System.String"));
				dtResult.Columns.Add("group_contact",System.Type.GetType("System.String"));
				dtResult.Columns.Add("connection_number",System.Type.GetType("System.Double"));		
				foreach(DataRow dr in dt.Rows){
					if(oldDateConnection!=Int64.Parse(dr["date_connection"].ToString())){
						columnName = "month_"+dr["date_connection"].ToString();
						dtResult.Columns.Add(columnName,System.Type.GetType("System.Double"));						
					}
					
					oldDateConnection=Int64.Parse(dr["date_connection"].ToString());
				}
				

				idGroupContactsArr = new ArrayList();
				columnName="";
				foreach(DataRow row in dt.Rows){
					if(!idGroupContactsArr.Contains(row["id_group_contact"].ToString())){
						resultRow=dtResult.NewRow();							
						resultRow["id_group_contact"] = row["id_group_contact"].ToString();
						resultRow["group_contact"] = row["group_contact"].ToString();
						resultRow["connection_number"] = double.Parse(dt.Compute("sum(connection_number)","id_group_contact="+row["id_group_contact"].ToString()).ToString()); 
						for(int i=3;i<dtResult.Columns.Count;i++){	
							columnName=dtResult.Columns[i].ColumnName;
							columnName = columnName.Substring(6,6);
							tempValue = dt.Compute("sum(connection_number)","id_group_contact="+row["id_group_contact"].ToString()+" and date_connection="+columnName); 						
							if(tempValue!=System.DBNull.Value && !tempValue.Equals(""))
								resultRow[i] = double.Parse(tempValue.ToString());
							else resultRow[i] =(double)0;
						}
						dtResult.Rows.Add(resultRow);	
						idGroupContactsArr.Add(row["id_group_contact"].ToString());
					}
				}
				
				//Trie tableau
				DataSet ds2=new DataSet();				
				ds2.Merge(dtResult.Select("","connection_number DESC"));
				dtResult = ds2.Tables[0];
				
			}
			
			return dtResult;
		}
		#endregion

		#region Top des clients par jour nomm�
		/// <summary>
		/// Obtient le top des clients par jour nomm�.
		/// </summary>
		/// <param name="parameters">param�tres client.</param>
		/// <returns>donn�es top connections clients par jour nomm�.</returns>
        public static DataTable TopConnectedByDay(BastetCommon.Parameters parameters, DataAccess.Client dataAccessClient) {
			
			DataTable dt=null, dtResult=null;
			Int64 oldDateConnection=0;
			ArrayList idLoginsArr = null;
			DataRow resultRow=null;
            dt = dataAccessClient.TopConnectedByDay();
			object tempValue=null;
			string columnName="";
			if(dt!=null && dt.Rows.Count>0){				

				//Table de r�sultats
				
				dtResult = new DataTable();

				dtResult.Columns.Add("company",System.Type.GetType("System.String"));
				dtResult.Columns.Add("login",System.Type.GetType("System.String"));
				dtResult.Columns.Add("connection_number",System.Type.GetType("System.Double"));		
				foreach(DataRow dr in dt.Rows){
					if(oldDateConnection!=Int64.Parse(dr["date_connection"].ToString())){
						columnName = "day_"+dr["date_connection"].ToString();
						dtResult.Columns.Add(columnName,System.Type.GetType("System.Double"));						
					}
					
					oldDateConnection=Int64.Parse(dr["date_connection"].ToString());
				}
				

				idLoginsArr = new ArrayList();
				columnName="";
				foreach(DataRow row in dt.Rows){
					if(!idLoginsArr.Contains(row["id_login"].ToString())){
						resultRow=dtResult.NewRow();							
						resultRow["company"] = row["company"].ToString();
						resultRow["login"] = row["login"].ToString();
						resultRow["connection_number"] = double.Parse(dt.Compute("sum(connection_number)","id_login="+row["id_login"].ToString()).ToString()); 
						for(int i=3;i<dtResult.Columns.Count;i++){	
							columnName=dtResult.Columns[i].ColumnName;
							columnName = columnName.Substring(4,1);
							tempValue = dt.Compute("sum(connection_number)","id_login="+row["id_login"].ToString()+" and date_connection="+columnName); 						
							if(tempValue!=System.DBNull.Value && !tempValue.Equals(""))
								resultRow[i] = double.Parse(tempValue.ToString());
							else resultRow[i] =(double)0;
						}
						dtResult.Rows.Add(resultRow);	
						idLoginsArr.Add(row["id_login"].ToString());
					}
				}
				
				//Trie tableau
				DataSet ds2=new DataSet();				
				ds2.Merge(dtResult.Select("","connection_number DESC"));
				dtResult = ds2.Tables[0];
				
			}
			
			return dtResult;
		}
		#endregion

		#region Top par type de client et par jour nomm�
		/// <summary>
		/// Obtient le top des types de clients et par jour nomm�
		/// </summary>
		/// <param name="parameters">param�tres client.</param>
		/// <returns>donn�es top connections types de clients et par jour nomm�.</returns>
        public static DataTable TopTypeConnectedByDay(BastetCommon.Parameters parameters, DataAccess.Client dataAccessClient) {
			
			DataTable dt=null, dtResult=null;
			Int64 oldDateConnection=0;
			ArrayList idGroupContactsArr = null;
			DataRow resultRow=null;
            dt = dataAccessClient.TopTypeConnectedByDay();
			object tempValue=null;
			string columnName="";
			if(dt!=null && dt.Rows.Count>0){				

				//Table de r�sultats
				
				dtResult = new DataTable();

				dtResult.Columns.Add("id_group_contact",System.Type.GetType("System.String"));
				dtResult.Columns.Add("group_contact",System.Type.GetType("System.String"));
				dtResult.Columns.Add("connection_number",System.Type.GetType("System.Double"));		
				foreach(DataRow dr in dt.Rows){
					if(oldDateConnection!=Int64.Parse(dr["date_connection"].ToString())){
						columnName = "day_"+dr["date_connection"].ToString();
						dtResult.Columns.Add(columnName,System.Type.GetType("System.Double"));						
					}
					
					oldDateConnection=Int64.Parse(dr["date_connection"].ToString());
				}
				

				idGroupContactsArr = new ArrayList();
				columnName="";
				foreach(DataRow row in dt.Rows){
					if(!idGroupContactsArr.Contains(row["id_group_contact"].ToString())){
						resultRow=dtResult.NewRow();							
						resultRow["id_group_contact"] = row["id_group_contact"].ToString();
						resultRow["group_contact"] = row["group_contact"].ToString();
						resultRow["connection_number"] = double.Parse(dt.Compute("sum(connection_number)","id_group_contact="+row["id_group_contact"].ToString()).ToString()); 
						for(int i=3;i<dtResult.Columns.Count;i++){	
							columnName=dtResult.Columns[i].ColumnName;
							columnName = columnName.Substring(4,1);
							tempValue = dt.Compute("sum(connection_number)","id_group_contact="+row["id_group_contact"].ToString()+" and date_connection="+columnName); 						
							if(tempValue!=System.DBNull.Value && !tempValue.Equals(""))
								resultRow[i] = double.Parse(tempValue.ToString());
							else resultRow[i] =(double)0;
						}
						dtResult.Rows.Add(resultRow);	
						idGroupContactsArr.Add(row["id_group_contact"].ToString());
					}
				}
				
				//Trie tableau
				DataSet ds2=new DataSet();				
				ds2.Merge(dtResult.Select("","connection_number DESC"));
				dtResult = ds2.Tables[0];
				
			}
			
			return dtResult;
		}
		#endregion
	}
}
