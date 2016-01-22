#region Informations
// Auteur: g. Facon
// Date de création:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables
#endregion

using System;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Classe contenant divers information sur une page de resultat comme son url ou son code de traduction
	/// </summary>
	public abstract class PageInformation{

		#region Variables
		/// <summary>
		/// Identifiant de la page
		/// </summary>
		protected int _id;
		/// <summary>
		/// Url de la page de résultat
		/// </summary>
		protected string _url="";
		///<summary>
		/// Url de la page d'aide
		/// </summary>
		///  <author>B. Masson</author>
		///  <since>25/07/2006</since>
		protected string _helpUrl="";
		/// <summary>
		/// Code traduction de la page
		/// </summary>
		protected Int64 _idWebText;
		///<author>G. Facon</author>
		///  <since>01/08/2006</since>
		///<summary>
		/// Identifiant du texte dans le menu correspondant à la page
		///</summary>
		protected Int64 _menuTextId=0;
		///<author>G. Ragneau</author>
		///  <since>03/08/2006</since>
		///<summary>
		///Specify whereas the page is allowed to use recall action or not
		///</summary>
		protected bool _allowRecall = true;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'objet</param>
		/// <param name="url">Url de la page</param>
		/// <param name="idWebText">Identifiant du texte</param>
		/// <param name="helpUrl">Url de la page d'aide</param>
		/// <param name="menuTextId">Identifiant du texte dans le menu correspondant à la page</param>
		protected PageInformation(int id, string url, Int64 idWebText,string helpUrl,Int64 menuTextId) {
			_url=url;
			_helpUrl=helpUrl;
			_idWebText=idWebText;
			_id=id;
			_menuTextId=menuTextId;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient l'identifiant de la page
		/// </summary>
		public int Id{
			get{return(_id);}
		}

		/// <summary>
		/// Obtient l'adresse de la page
		/// </summary>
		public string Url{
			get{return _url;}
		}

		///<summary>
		/// Obtient l'url de la page d'aide
		/// </summary>
		///  <author>B. Masson</author>
		///  <since>25/07/2006</since>
		public string HelpUrl {
			get {return (_helpUrl);}
		}

		/// <summary>
		/// Obtient le code de traduction de la page
		/// </summary>
		public Int64 IdWebText{
			get{return _idWebText;}
		}
		
		///<author>G. Facon</author>
		///  <since>01/08/2006</since>
		///<summary>
		/// obtient l'dentifiant du texte dans le menu correspondant à la page
		///</summary>
		public Int64 MenuTextId {
			get {return(_menuTextId);}
		}
		///<author>G. Ragneau</author>
		///  <since>03/08/2006</since>
		///<summary>
		///Specify whereas the page is allowed to use recall action or not
		///</summary>
		public bool AllowRecall{
			get{return _allowRecall;}
		}
		#endregion




	}
}

