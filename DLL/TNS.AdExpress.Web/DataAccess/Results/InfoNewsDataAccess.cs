#region Informations
// Auteur: B. Masson
// Date de création: 17/11/2004 
// Date de modification: 17/11/2004 
#endregion

using System;
using System.Data;
using System.Collections;
using System.IO;

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Classe qui récupère les informations d'infos/news
	/// </summary>
	public class InfoNewsDataAccess{
		
		/// <summary>
		/// Obtient la liste des fichiers PDF d'une plaquette
		/// </summary>
		/// <param name="name">Nom de la plaquette</param>
		/// <param name="pathDirectory">Chemin du répertoire</param>
		/// <returns>Liste des fichiers</returns>
		internal static string[] GetData(string name, string pathDirectory){
			string[] filesList = Directory.GetFiles(pathDirectory,"*.pdf");
			return (filesList);
		}

	}
}
