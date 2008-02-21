#region Informations
// Auteur: G. Facon
// Date de cr�ation: 29/06/2005
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
	/// Description r�sum�e de OracleDataSource.
	/// </summary>
	public class _XmlReaderDataSource:_IDataSource{

		#region Variables
		/// <summary>
		/// Source de donn�es
		/// </summary>
		protected XmlTextReader _source;
		/// <summary>
		/// Type de la source de donn�es
		/// </summary>
		protected DataSource.Type _sourceType=DataSource.Type.xml;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		public _XmlReaderDataSource(XmlTextReader source){
			if(source==null)throw(new NullReferenceException("La source de donn�es est NULL"));
			_source=source;
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="filePath">Chemin de la source de donn�es</param>
		public _XmlReaderDataSource(string filePath){
			if(filePath==null)throw(new NullReferenceException("Le chemin de la source de donn�es est NULL"));
			if(filePath.Length==0)throw(new ArgumentException("Le chemin de la source de donn�es est vide"));
			if(!File.Exists(filePath))throw(new FileNotFoundException("La source de donnes n'existe pas"));
			_source=new XmlTextReader(filePath);
		}

		#endregion

		#region m�thodes Externes
		/// <summary>
		/// Obtient la source de donn�es
		/// </summary>
		/// <returns>Source de donn�es</returns>
		public object GetSource(){
			if(_source==null)throw(new NullReferenceException("La source de donn�es est NULL"));
			return(_source);
		}

		/// <summary>
		/// Obtient le type de la source de donn�es
		/// </summary>
		/// <returns>Type de la source de donn�es</returns>
		public DataSource.Type DataSourceType(){
			return(_sourceType);
		}

		/// <summary>
		/// Ouvre la source de donn�es
		/// </summary>
		public void Open(){
			throw(new NotImplementedException("Non impl�ment�e: la source est forcement ouverte"));
		}

		/// <summary>
		/// Ferme la source de donn�es
		/// </summary>
		public void Close(){
			try{
				_source.Close();
				_source=null;
			}
			catch(System.Exception err){
				throw(new XmlReaderDataSourceException ("Impossible de fermer la source de donn�es",err));
			}
		}

		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		/// <returns>Resultat</returns>
		public DataSet Fill(string command){
			throw(new NotImplementedException ("Non impl�ment�e"));
		}

		/// <summary>
		/// Modifie un �l�ment
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Update(string command){
			throw(new NotImplementedException("Not implemented"));
		}
		/// <summary>
		/// Ajoute un �l�ment
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Insert(string command){
			throw(new NotImplementedException("Not implemented"));
		}
		/// <summary>
		/// Supprime un �l�ment
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		public void Delete(string command){
			throw(new NotImplementedException("Not implemented"));
		}
		#endregion
	}
}
