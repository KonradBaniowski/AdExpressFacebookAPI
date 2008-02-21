#region Informations
// Auteur: G. Facon
// Date de création: 29/06/2005
// Date de modification:
#endregion

using System;
using System.Data;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Common{
	/// <summary>
	/// Source de données
	/// </summary>
	public interface _IDataSource{
		/// <summary>
		/// Obtient la source de données
		/// </summary>
		/// <returns>Source de données</returns>
		object GetSource();
		/// <summary>
		/// Obtient le type de la source de données
		/// </summary>
		/// <returns>Type de la source de données</returns>
		DataSource.Type DataSourceType();
		/// <summary>
		/// Ouvre la source de données
		/// </summary>
		void Open();
		/// <summary>
		/// Ferme la source de données
		/// </summary>
		void Close();
		/// <summary>
		/// Execute
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		/// <returns>Resultat</returns>
		DataSet Fill(string command);
		/// <summary>
		/// modifie un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		void Update(string command);
		/// <summary>
		/// Insérer un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		void Insert(string command);
		/// <summary>
		/// Supprimer un élément
		/// </summary>
		/// <param name="command">Ligne de commande</param>
		void Delete(string command);

	}
}
