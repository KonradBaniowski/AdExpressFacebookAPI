#region Informations
// Auteur: G. Facon
// Date de cr�ation: 29/06/2005
// Date de modification:
#endregion

using System;
using System.Data;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Common{
	/// <summary>
	/// Source de donn�es
	/// </summary>
	public interface _IDataSource{
		/// <summary>
		/// Obtient la source de donn�es
		/// </summary>
		/// <returns>Source de donn�es</returns>
		object GetSource();
		/// <summary>
		/// Obtient le type de la source de donn�es
		/// </summary>
		/// <returns>Type de la source de donn�es</returns>
		DataSource.Type DataSourceType();
		/// <summary>
		/// Ouvre la source de donn�es
		/// </summary>
		void Open();
		/// <summary>
		/// Ferme la source de donn�es
		/// </summary>
		void Close();
		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		/// <returns>Resultat</returns>
		DataSet Fill(string command);
		/// <summary>
		/// modifie un �l�ment
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		void Update(string command);
		/// <summary>
		/// Ins�rer un �l�ment
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		void Insert(string command);
		/// <summary>
		/// Supprimer un �l�ment
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		void Delete(string command);

	}
}
