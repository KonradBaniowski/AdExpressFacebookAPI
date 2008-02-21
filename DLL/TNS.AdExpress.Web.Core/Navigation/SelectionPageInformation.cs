#region Informations
// Auteur: g. Facon
// Date de cr�ation:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
//  G. Ragneau - 01/08/2006 - Add param "validationMethod to the constructor 
//  Y. R'kaina 13/12/2006 _htSubSelectionPageInformation pour le stockage des pages secondaires (optionnelles)
#endregion

using System;
using System.Collections;

namespace TNS.AdExpress.Web.Core.Navigation{
	/// <summary>
	/// Description r�sum�e de SelectionPageInformation.
	/// </summary>
	public class SelectionPageInformation:OptionalPageInformation{

		#region Variables
		///<author>G. Facon</author>
		///  <since>31/07/2006</since>
		///<summary>
		///Nom de la m�thode qui indique si les �l�ments de la page de s�lection ont �t� s�lectionn�s
		///</summary>
		protected string _validationMethod;
		/// <summary>
		/// Permet de r�cup�rer les pages secondaires(pages optionnelles)
		/// </summary>
		protected Hashtable _htSubSelectionPageInformation;
        /// <summary>
        /// Le nom de la fonction qu'il faut appeler dans le menuItem R�sultat de MenuWebControl
        /// </summary>
        protected string _functionName = "";
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
		public SelectionPageInformation(int id,bool showLink,string url, Int64 idWebText,string iconeName,string helpUrlValue,string validationMethod,Int64 menuTextId, bool allowRecall,string functionName):base(id,showLink,url,idWebText,iconeName,helpUrlValue,menuTextId){
			_validationMethod = validationMethod;
			_allowRecall = allowRecall;
            _functionName = functionName;
			_htSubSelectionPageInformation = new Hashtable();
		}
		#endregion

		#region Accesseur

		///<author>G. Facon</author>
		///  <since>31/07/2006</since>
		///<summary>
		///Obtient la m�thode qui indique si les �l�ments de la page de s�lection ont �t� s�lectionn�s
		///</summary>
		public string ValidationMethod {
			get {return(_validationMethod);}
		}

		/// <summary>
		/// Obtient le Hashtable qui permet de r�cup�rer les pages secondaires(pages optionnelles)
		/// </summary>
		public Hashtable HtSubSelectionPageInformation { 
			get {
				return(_htSubSelectionPageInformation);
			}
			set {
				_htSubSelectionPageInformation = value;
			}
		}

        /// <summary>
        /// Obtient le nom de la fonction script � appeler dans le menuItem R�sultat
        /// </summary>
        public string FunctionName {
            get { return _functionName; }
        }
		#endregion

	}
}
