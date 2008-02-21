#region Informations
// Author: G. Facon
// Creation Date:
// Modification Date:
//	11/08/2005	G. Facon	variable names
#endregion

using System;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;


namespace TNS.AdExpress.Web.Core.DataAccess.Translation{
	/// <summary>
	/// Class used to load all the words of a language
	/// </summary>
	public class WordListDataAccess{

		#region Variables

		/// <summary>
		/// DataBase connection
		/// </summary>
		private OracleConnection _connection;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connectionString">DataBase connection</param>
		protected WordListDataAccess(string connectionString){
			try{
				_connection=new OracleConnection(connectionString);
			}
			catch(System.Exception e){
				throw(new WordListDBException("Impossible to connect to the dataBase",e));
			}
		}

		#endregion


		/// <summary>
		/// Load the texts
		/// </summary>
		/// <param name="SiteLanguage">Language Id</param>
		/// <returns>Texts list</returns>
		protected string[] GetList(int SiteLanguage){
			long max=0;
			string[] list=null;

			#region Open the DataBase
			bool DBToClosed=false;
			if (_connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					_connection.Open();
				}
				catch(System.Exception e){
					throw(new WordListDBException("Impossible to open the database:",e));
				}
			}
			#endregion

			OracleCommand cmd=null;
			OracleDataReader myReader=null;
			try{
				//Seach the max id
				cmd= new OracleCommand("SELECT max(id_web_text) from "+DBConstantes.Schema.APPLICATION_SCHEMA+".web_text" ,_connection);
				myReader=cmd.ExecuteReader();
				if (myReader.Read()){
					max=int.Parse(myReader.GetValue(0).ToString());
				}
				myReader.Close();
				if (max>0){
					// some words have been found
					list=new String[max+1];
					// Load all words from table web_text
					cmd.CommandText="SELECT id_web_text,web_text from "+DBConstantes.Schema.APPLICATION_SCHEMA+".web_text where id_language="+SiteLanguage.ToString()+" order by id_web_text";
					myReader=cmd.ExecuteReader();
					while(myReader.Read()){
						// Insert word in the list
						list[Int64.Parse(myReader.GetValue(0).ToString())]=myReader.GetValue(1).ToString();
					}
				}
			}
			catch(System.Exception e){
				try{
					// Close Fermeture reader and database
					if(cmd!=null) cmd.Dispose();
					if(myReader!=null){
						myReader.Close();
						myReader.Dispose();
					}
					if (DBToClosed) _connection.Close();
				}
				catch(System.Exception et){
					throw(new WordListDBException("Impossible to close the database",et));
				}
				throw(new WordListDBException("Impossible build the words list "+_connection,e));
			}

			#region Close Fermeture reader and database
			try{
				if(cmd!=null) cmd.Dispose();
				if(myReader!=null){
					myReader.Close();
					myReader.Dispose();
				}
				if (DBToClosed) _connection.Close();
			}
			catch(System.Exception e){
				throw(new WordListDBException("Impossible close de dataBase",e));
			}
			#endregion

			return(list);
		}
	}
}
