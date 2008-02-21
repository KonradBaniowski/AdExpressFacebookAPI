#region Informations
// Auteur: Y. R'kaina
// Date de cr�ation: 12/12/2006
// Date de modification:
#endregion

using System;

namespace TNS.AdExpress.Web.Core.Navigation{
	/// <summary>
	/// Description r�sum�e de SubSelectionPageInformation.
	/// </summary>
	public class SubSelectionPageInformation:OptionalPageInformation{

		#region Variables
		///<author>G. Facon</author>
		///  <since>31/07/2006</since>
		///<summary>
		///Nom de la m�thode qui indique si les �l�ments de la page de s�lection ont �t� s�lectionn�s
		///</summary>
		protected string _validationMethod;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">Identifiant de la page</param>
		/// <param name="showLink">Doit on montrer la page dans le composant recall</param>
		/// <param name="url">Url de la page</param>
		/// <param name="idWebText">Identifiant du texte</param>
		/// <param name="iconeName">Nom de l'ic�ne</param>
		/// <param name="helpUrlValue">Url de la page d'aide</param>
		/// <param name="validationMethod">Name of the method witch specify if the selection on the page is valid or not</param>
		/// <param name="menuTextId">Identifiant du texte dans le menu correspondant � la page</param>
		/// <param name="allowRecall">Specify whereas the page is allowed to use recall actions</param>
		public SubSelectionPageInformation(int id,bool showLink,string url, Int64 idWebText,string iconeName,string helpUrlValue,string validationMethod,Int64 menuTextId, bool allowRecall):base(id,showLink,url,idWebText,iconeName,helpUrlValue,menuTextId){
			_validationMethod = validationMethod;
			_allowRecall = allowRecall;
		}
		#endregion

		#region Accesseur

		///  <since>31/07/2006</since>
		///<summary>
		///Obtient la m�thode qui indique si les �l�ments de la page de s�lection ont �t� s�lectionn�s
		///</summary>
		public string ValidationMethod{
			get {return(_validationMethod);}
		}
		#endregion
	}
}
