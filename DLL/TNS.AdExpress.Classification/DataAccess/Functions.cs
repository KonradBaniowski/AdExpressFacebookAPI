#region Informations
// Auteur: B. Masson G. Facon
// Date de création: 09/03/2004
// Date de modification: 25/10/2004
#endregion

using System;
using System.Data;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Classification{
	/// <summary>
	/// Fonctions traitant des données des nomenclatures
	/// </summary>
	public class Functions{

		#region Recherche d'élements dans les nomenclatures support et produit

		/// <summary>
		/// Recherche les éléments dans tous les niveaux d’une branche de la nomenclature définie par le type
		/// </summary>
		/// <param name="type">Branche</param>
		/// <param name="wordToFind">Mot clef</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <returns>Element recherché dans les niveaux du branche de la nomenclature</returns>
		public static DataTable findClassificationsItemsByWord(Branch.type type,string wordToFind,OracleConnection connection){
			if (wordToFind.Length==0) throw(new System.Exception("Le champs est vide"));
			string newWordToFind=wordToFind.Trim(' ');

			DataTable dtParent = new DataTable();
			dtParent.Columns.Add("Type",typeof(int));
			dtParent.Columns.Add("Id",typeof(Int64));
			dtParent.Columns.Add("Text",typeof(string));

			switch(type){

				#region Nomenclature Produit
				case Branch.type.product:
					// Recherche dans la table Sector
					resultsDataTable(Table.name.sector,findClassificationsItemsByWord(Table.name.sector,wordToFind,connection),dtParent);
					// Recherche dans la table Subsector
					resultsDataTable(Table.name.subsector,findClassificationsItemsByWord(Table.name.subsector,wordToFind,connection),dtParent);
					// Recherche dans la table Group_
					resultsDataTable(Table.name.group_,findClassificationsItemsByWord(Table.name.group_,wordToFind,connection),dtParent);
					// Recherche dans la table Segment
					resultsDataTable(Table.name.segment,findClassificationsItemsByWord(Table.name.segment,wordToFind,connection),dtParent);
					// Recherche dans la table Advertiser
					//resultsDataTable(Table.name.advertiser,findClassificationsItemsByWord(Table.name.advertiser,wordToFind,connection),dtParent);
					// Recherche dans la table Holding_company
					//resultsDataTable(Table.name.holding_company,findClassificationsItemsByWord(Table.name.holding_company,wordToFind,connection),dtParent);
					break;
				#endregion

				#region Nomenclature media
				 case Branch.type.media:
					// Recherche dans la table Vehicle (Média)
					resultsDataTable(Table.name.vehicle,findClassificationsItemsByWord(Table.name.vehicle,newWordToFind,connection),dtParent);
					// Recherche dans la table Category (Catégorie)
					resultsDataTable(Table.name.category,findClassificationsItemsByWord(Table.name.category,newWordToFind,connection),dtParent);
					// Recherche dans la table Media (Support)
					resultsDataTable(Table.name.media,findClassificationsItemsByWordWithoutActivation(Table.name.media,newWordToFind,connection),dtParent);
					// Recherche dans la table Title (Titre)
					 resultsDataTable(Table.name.title,findClassificationsItemsByWordWithoutActivation(Table.name.title,newWordToFind,connection),dtParent); 
					// Recherche dans la table Media Seller (Régie)
					 resultsDataTable(Table.name.media_seller,findClassificationsItemsByWordWithoutActivation(Table.name.media_seller,newWordToFind,connection),dtParent);
					break;
				#endregion

				default:
					throw (new System.Exception("Le type de branche de nomenclature n'existe pas"));
			}
			return(dtParent);
		}

		/// <summary>
		/// Remplit le DataTable unique
		/// </summary>
		/// <param name="tableName">Nom de la table</param>
		/// <param name="dtResult">Resultat du DataTable par nom de table</param>
		/// <param name="dtParent">Résultat parent</param>
		/// <returns>Tous les résultats contenu dans le DataTable unique</returns>
		public static void resultsDataTable(Table.name tableName,DataTable dtResult,DataTable dtParent){
			DataRow drParent;
			drParent = dtParent.NewRow();
			foreach(DataRow current in dtResult.Rows){
				drParent = dtParent.NewRow();
				drParent["Type"]	= (int)tableName;
				drParent["Id"]		= current["Id"];
				drParent["Text"]	= current["Text"];
				dtParent.Rows.Add(drParent);
			}
		}

		#endregion

		#region Fonctions de recherches avec code d'activation

		/// <summary>
		/// Recherche d’un mot dans une branche de nomenclature
		/// </summary>
		/// <param name="tableName">Nom de la table</param>
		/// <param name="wordToFind">Mot clef</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <returns>Element recherché dans les niveaux du branche de la nomenclature</returns>
		public static DataTable findClassificationsItemsByWord(Table.name tableName,string wordToFind,OracleConnection connection){
			
			#region Construction de la requête
			string sql="select id_"+tableName.ToString()+","+tableName.ToString() ;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName.ToString();
			sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and UPPER("+tableName.ToString()+") LIKE('%"+wordToFind.ToUpper()+"%')";
			sql+=" order by "+tableName.ToString() ;
			#endregion

			DataTable dt = new DataTable();
			dt.Columns.Add("Id",typeof(int));
			dt.Columns.Add("Text",typeof(string));
			DataRow dr;

			#region Ouverture de la base de données
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
			#endregion

			OracleDataReader sqlReader=null;
			try{
				// Execution de la requête
				OracleCommand sqlCommand=new OracleCommand(sql,connection);
				sqlReader=sqlCommand.ExecuteReader();
				// Lecture des résultats
				while(sqlReader.Read()){
					dr = dt.NewRow();
					dr["Id"]=(Int64) sqlReader.GetValue(0);
					dr["Text"]=sqlReader.GetValue(1).ToString();
					dt.Rows.Add(dr);
				}
				try{
					// Fermeture du reader et de la base de données
					sqlReader.Close();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception e){
					//throw(new DbOperationException("Impossible de fermer la base de données :"+e.Message));
					throw(e);
				}
			}
			catch(System.Exception e){
				//throw (new DbOperationException("Impossible de trouver l'élément: "+e.Message));
				throw(e);
			}
			return dt;
		}

		/// <summary>
		/// Recherche un élement par rapport à un code donné dans la table spécifiée par l’argument tableName
		/// </summary>
		/// <param name="tableName">Nom de la table</param>
		/// <param name="code">Code de l'élément</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <returns>Element recherché</returns>
		public static DataTable findClassificationsItemsByCode(Table.name tableName,int code,OracleConnection connection){
			if (code.ToString().Length==0) throw(new System.Exception("La champs est vide"));
			if (code.GetType() != typeof(System.Int32)) throw(new System.Exception("Code invalide"));

			#region Construction de la requête
			string sql="select id_"+tableName.ToString()+","+tableName.ToString() ;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName.ToString();
			sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and id_"+tableName.ToString()+"="+code.ToString();
			sql+=" order by "+tableName.ToString() ;
			#endregion

			DataTable dt = new DataTable();
			dt.Columns.Add("Type",typeof(int));
			dt.Columns.Add("Id",typeof(Int64));
			dt.Columns.Add("Text",typeof(string));
			DataRow dr;
		
			#region Ouverture de la base de données
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
			#endregion

			OracleDataReader sqlReader=null;
			try{
				// Execution de la requête
				OracleCommand sqlCommand=new OracleCommand(sql,connection);
				sqlReader=sqlCommand.ExecuteReader();
			}
			catch(System.Exception e){
				//throw (new DbOperationException("Impossible de trouver l'élément: "+e.Message));
				throw(e);
			}
			// Lecture des résultats, dans ce cas qu'une seule valeur doit être retournée
			if(sqlReader.Read()){
				dr = dt.NewRow();
				dr["Type"]=(int) tableName;
				dr["Id"]=(Int64) sqlReader.GetValue(0);
				dr["Text"]=sqlReader.GetValue(1).ToString();
				dt.Rows.Add(dr);
			}
				// Il n'y a pas de valeur (ERREUR)
			else{
				try{
					// Fermeture du reader et de la base de données
					sqlReader.Close();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception e){
					//throw(new DbOperationException("Impossible de fermer la base de données :"+e.Message));
					throw(e);
				}
				//throw (new DbOperationException("Impossible de trouver l'élément"));
				throw (new System.Exception("Impossible de trouver l'élément"));
			}
			return dt;
		}

		/// <summary>
		/// Recherche les élements par rapport à une liste de codes donnée dans la table spécifiée par l’argument tableName
		/// </summary>
		/// <param name="tableName">Nom de la table</param>
		/// <param name="codeList">Liste des codes</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <returns>Element recherché</returns>
		public static DataTable findClassificationsItemsByCode(Table.name tableName,string codeList,OracleConnection connection){
			if (codeList.Length==0) throw(new System.Exception("La champs est vide"));
			
			// Vérification de la liste
			verifCharacter(codeList);

			#region Construction de la requête
			string sql="select id_"+tableName.ToString()+","+tableName.ToString() ;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName.ToString();
			sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and id_"+tableName.ToString()+" in ("+codeList+")";
			sql+=" order by "+tableName.ToString() ;
			#endregion

			DataTable dt = new DataTable();
			dt.Columns.Add("Type",typeof(int));
			dt.Columns.Add("Id",typeof(Int64));
			dt.Columns.Add("Text",typeof(string));
			DataRow dr;

			#region Ouverture de la base de données
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
			#endregion

			OracleDataReader sqlReader=null;
			try{
				// Execution de la requête
				OracleCommand sqlCommand=new OracleCommand(sql,connection);
				sqlReader=sqlCommand.ExecuteReader();
				// Lecture des résultats
				while(sqlReader.Read()){
					dr = dt.NewRow();
					dr["Type"]=(int) tableName;
					dr["Id"]=(Int64) sqlReader.GetValue(0);
					dr["Text"]=sqlReader.GetValue(1).ToString();
					dt.Rows.Add(dr);
				}
				try{
					// Fermeture du reader et de la base de données
					sqlReader.Close();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception e){
					//throw(new DbOperationException("Impossible de fermer la base de données :"+e.Message));
					throw (e);
				}
			}
			catch(System.Exception e){
				//throw (new DbOperationException("Impossible de trouver l'élément: "+e.Message));
				throw (e);
			}
			return dt;
		}

		#endregion

		#region Fonctions de recherches sans code d'activation

		/// <summary>
		/// Recherche d’un mot dans une branche de nomenclature
		/// </summary>
		/// <param name="tableName">Nom de la table</param>
		/// <param name="wordToFind">Mot clef</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <returns>Element recherché dans les niveaux du branche de la nomenclature</returns>
		public static DataTable findClassificationsItemsByWordWithoutActivation(Table.name tableName,string wordToFind,OracleConnection connection){
			
			#region Construction de la requête
			string sql="select id_"+tableName.ToString()+","+tableName.ToString() ;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName.ToString();
			sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and UPPER("+tableName.ToString()+") LIKE('%"+wordToFind.ToUpper()+"%')";
			sql+=" order by "+tableName.ToString() ;
			#endregion

			DataTable dt = new DataTable();
			dt.Columns.Add("Id",typeof(int));
			dt.Columns.Add("Text",typeof(string));
			DataRow dr;

			#region Ouverture de la base de données
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
			#endregion

			OracleDataReader sqlReader=null;
			try{
				// Execution de la requête
				OracleCommand sqlCommand=new OracleCommand(sql,connection);
				sqlReader=sqlCommand.ExecuteReader();
				// Lecture des résultats
				while(sqlReader.Read()){
					dr = dt.NewRow();
					dr["Id"]=(Int64) sqlReader.GetValue(0);
					dr["Text"]=sqlReader.GetValue(1).ToString();
					dt.Rows.Add(dr);
				}
				try{
					// Fermeture du reader et de la base de données
					sqlReader.Close();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception e){
					//throw(new DbOperationException("Impossible de fermer la base de données :"+e.Message));
					throw(e);
				}
			}
			catch(System.Exception e){
				//throw (new DbOperationException("Impossible de trouver l'élément: "+e.Message));
				throw(e);
			}
			return dt;
		}

		/// <summary>
		/// Recherche un élement par rapport à un code donné dans la table spécifiée par l’argument tableName
		/// </summary>
		/// <param name="tableName">Nom de la table</param>
		/// <param name="code">Code de l'élément</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <returns>Element recherché</returns>
		public static DataTable findClassificationsItemsByCodeWithoutActivation(Table.name tableName,int code,OracleConnection connection){
			if (code.ToString().Length==0) throw(new System.Exception("La champs est vide"));
			if (code.GetType() != typeof(System.Int32)) throw(new System.Exception("Code invalide"));

			#region Construction de la requête
			string sql="select id_"+tableName.ToString()+","+tableName.ToString() ;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName.ToString();
			sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and id_"+tableName.ToString()+"="+code.ToString();
			sql+=" order by "+tableName.ToString() ;
			#endregion

			DataTable dt = new DataTable();
			dt.Columns.Add("Type",typeof(int));
			dt.Columns.Add("Id",typeof(Int64));
			dt.Columns.Add("Text",typeof(string));
			DataRow dr;
		
			#region Ouverture de la base de données
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
			#endregion

			OracleDataReader sqlReader=null;
			try{
				// Execution de la requête
				OracleCommand sqlCommand=new OracleCommand(sql,connection);
				sqlReader=sqlCommand.ExecuteReader();
			}
			catch(System.Exception e){
				//throw (new DbOperationException("Impossible de trouver l'élément: "+e.Message));
				throw(e);
			}
			// Lecture des résultats, dans ce cas qu'une seule valeur doit être retournée
			if(sqlReader.Read()){
				dr = dt.NewRow();
				dr["Type"]=(int) tableName;
				dr["Id"]=(Int64) sqlReader.GetValue(0);
				dr["Text"]=sqlReader.GetValue(1).ToString();
				dt.Rows.Add(dr);
			}
				// Il n'y a pas de valeur (ERREUR)
			else{
				try{
					// Fermeture du reader et de la base de données
					sqlReader.Close();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception e){
					//throw(new DbOperationException("Impossible de fermer la base de données :"+e.Message));
					throw(e);
				}
				//throw (new DbOperationException("Impossible de trouver l'élément"));
				throw (new System.Exception("Impossible de trouver l'élément"));
			}
			return dt;
		}

		/// <summary>
		/// Recherche les élements par rapport à une liste de codes donnée dans la table spécifiée par l’argument tableName
		/// </summary>
		/// <param name="tableName">Nom de la table</param>
		/// <param name="codeList">Liste des codes</param>
		/// <param name="connection">Connexion à la base de données</param>
		/// <returns>Element recherché</returns>
		public static DataTable findClassificationsItemsByCodeWithoutActivation(Table.name tableName,string codeList,OracleConnection connection){
			if (codeList.Length==0) throw(new System.Exception("La champs est vide"));
			
			// Vérification de la liste
			verifCharacter(codeList);

			#region Construction de la requête
			string sql="select id_"+tableName.ToString()+","+tableName.ToString() ;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName.ToString();
			sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and id_"+tableName.ToString()+" in ("+codeList+")";
			sql+=" order by "+tableName.ToString() ;
			#endregion

			DataTable dt = new DataTable();
			dt.Columns.Add("Type",typeof(int));
			dt.Columns.Add("Id",typeof(Int64));
			dt.Columns.Add("Text",typeof(string));
			DataRow dr;

			#region Ouverture de la base de données
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
			#endregion

			OracleDataReader sqlReader=null;
			try{
				// Execution de la requête
				OracleCommand sqlCommand=new OracleCommand(sql,connection);
				sqlReader=sqlCommand.ExecuteReader();
				// Lecture des résultats
				while(sqlReader.Read()){
					dr = dt.NewRow();
					dr["Type"]=(int) tableName;
					dr["Id"]=(Int64) sqlReader.GetValue(0);
					dr["Text"]=sqlReader.GetValue(1).ToString();
					dt.Rows.Add(dr);
				}
				try{
					// Fermeture du reader et de la base de données
					sqlReader.Close();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception e){
					//throw(new DbOperationException("Impossible de fermer la base de données :"+e.Message));
					throw (e);
				}
			}
			catch(System.Exception e){
				//throw (new DbOperationException("Impossible de trouver l'élément: "+e.Message));
				throw (e);
			}
			return dt;
		}

		#endregion

		#region Fonctions
		/// <summary>
		/// Vérifie la liste des codes
		/// </summary>	
		/// <param name="codeList">Liste des codes</param>
		public static void verifCharacter(string codeList){
			string trimCodeList = codeList.Trim();
			string [] verifCodeList = trimCodeList.Split(new Char [] {','});
			long tmp;
			foreach(string current in verifCodeList){
				//if (current.ToString()=="") throw(new Exception("Chaque code doit être séparé par une virgule"));
				try{
					tmp = long.Parse(current.ToString());
				}
				catch(System.Exception e){
					throw(new System.Exception("Les codes doivent être des entiers séparés par des virgules.\n"+e.Message));
				}
			}
		}
		#endregion
	}
}
