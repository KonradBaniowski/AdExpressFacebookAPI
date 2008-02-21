#region Informations
// Auteur: G. Facon
// Date de cr�ation: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Classification.Exceptions;


namespace TNS.AdExpress.Classification.DataAccess.ProductBranch{
	/// <summary>
	/// Chargement partiel d'une liste de HoldingCompanies
	/// Attention cette classe ne charge pas tous les �l�ments.
	/// A chaque demande d'un libell�, elle fait une requ�te � la base de donn�es pour obtenir le texte
	/// </summary>
	public class AllHoldingCompanyLevelListDataAccess{

		#region Variables
		/// <summary>
		/// Connexion � la base de donn�es
		/// </summary>
		private OracleConnection connection;

		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="connection">Connexion � la base de donn�es</param>
		public AllHoldingCompanyLevelListDataAccess(OracleConnection connection){
			this.connection=connection;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Retourne la valeur dont l'dentifiant est ID
		/// </summary>
		public string this [Int64 id]{
			get{
				string text;
				string table=TNS.AdExpress.Constantes.Classification.DB.Table.name.holding_company.ToString();
				// Construction de la requ�te
				string sql="select id_"+table+", "+table+" from adexpressfr01."+table+" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" and id"+table+"="+id.ToString();
				// Ouverture de la base de donn�es
				bool DBToClosed=false;
				if (connection.State==System.Data.ConnectionState.Closed){
					DBToClosed=true;
					try{
						connection.Open();
					}
					catch(System.Exception e){
						throw(e);
					}
				}
				OracleCommand oracleCommand;
				OracleDataReader oracleDataReader=null;
				try{
					oracleCommand=new OracleCommand(sql,connection);
					oracleDataReader=oracleCommand.ExecuteReader();
					if(oracleDataReader.Read()) text=oracleDataReader.GetValue(1).ToString();
					else throw(new Classification.Exceptions.ClassificationDataDBException("Aucun Libell� ne correspond � l'identifiant "+id));
				}
				catch(System.Exception e){
					try{
						if(oracleDataReader!=null) oracleDataReader.Close();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception ex){
						throw(new ClassificationDataDBException("Impossible de fermer la base de donn�es: "+ex.Message));
					}
					throw(new ClassificationDataDBException("Impossible de s�lectionner les �l�ments : "+e.Message));
				}
				// Fermeture de la base de donn�es
				try{
					oracleDataReader.Close();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception e){
					throw(new ClassificationDataDBException("Impossible de fermer la base de donn�es : "+e.Message));
				}
				return(text);
			}
		}
		#endregion
	}
}

