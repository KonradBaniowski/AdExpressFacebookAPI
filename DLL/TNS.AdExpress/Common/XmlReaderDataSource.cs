#region Informations
// Auteur: G. Facon
// Date de création: 29/06/2005
// Date de modification:
#endregion

using System;
using System.Data;
using System.Xml;
using System.IO;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Exceptions;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Common{
	/// <summary>
	/// Description résumée de OracleDataSource.
	/// </summary>
	public class _XmlReaderDataSource:_IDataSource{

		#region Variables
		/// <summary>
		/// Source de données
		/// </summary>
		protected XmlTextReader _source;
		/// <summary>
		/// Type de la source de données
		/// </summary>
		protected DataSource.Type _sourceType=DataSource.Type.xml;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Source de données</param>
		public _XmlReaderDataSource(XmlTextReader source){
			if(source==null)throw(new NullReferenceException("La source de données est NULL"));
			_source=source;
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="filePath">Chemin de la source de données</param>
		public _XmlReaderDataSource(string filePath){
			if(filePath==null)throw(new NullReferenceException("Le chemin de la source de données est NULL"));
			if(filePath.Length==0)throw(new ArgumentException("Le chemin de la source de données est vide"));
			if(!File.Exists(filePath))throw(new FileNotFoundException("La source de donnes n'existe pas"));
			_source=new XmlTextReader(filePath);
		}

		#endregion

		#region méthodes Externes
		/// <summary>
		/// Obtient la source de données
		/// </summary>
		/// <returns>Source de données</returns>
		public object GetSource(){
			if(_source==null)throw(new NullReferenceException("La source de données est NULL"));
			return(_source);
		}

		/// <summary>
		/// Obtient le type de la source de données
		/// </summary>
		/// <returns>Type de la source de données</returns>
		public DataSource.Type DataSourceType(){
			return(_sourceType);
		}

		/// <summary>
		/// Ouvre la source de données
		/// </summary>
		public void Open(){
			throw(new NotImplementedException("Non implémentée: la source est forcement ouverte"));
		}

		/// <summary>
		/// Ferme la source de données
		/// </summary>
		public void Close(){
			try{
				_source.Close();
				_source=null;
			}
			catch(System.Exception err){
				throw(new XmlReaderDataSourceException ("Impossible de fermer la source de données",err));
			}
		}

		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		/// <returns>Resultat</returns>
		public DataSet Fill(string command){
			throw(new NotImplementedException ("Non implémentée"));
		}

		/// <summary>
		/// Modifie un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Update(string command){
			throw(new NotImplementedException("Not implemented"));
		}
		/// <summary>
		/// Ajoute un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Insert(string command){
			throw(new NotImplementedException("Not implemented"));
		}
		/// <summary>
		/// Supprime un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Delete(string command){
			throw(new NotImplementedException("Not implemented"));
		}
		#endregion
	}
}
