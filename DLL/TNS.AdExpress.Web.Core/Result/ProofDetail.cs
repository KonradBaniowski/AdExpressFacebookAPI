#region Informations
//Auteur: D. Mussuma
//Date de création: 05/02/2007
//Date de modification:
#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Core.Result
{
	/// <summary>
	/// Contient les informations d'une fiche justificative
	/// </summary>
	[System.Serializable]
	public class ProofDetail
	{
		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _webSession;
		
		/// <summary>
		/// Identifiant du support
		/// </summary>
		protected string _idMedia = null;

		/// <summary>
		/// Identifiant du poduit
		/// </summary>
		protected string _idProduct = null;

		/// <summary>
		/// Numero de la page
		/// </summary>
		protected string _mediaPaging = null;

		/// <summary>
		/// Date Faciale
		/// </summary>
		protected string _dateCover = null;

		/// <summary>
		/// Date de parution
		/// </summary>
		protected string _dateParution = null;

		#endregion

		#region Accesseurs

		/// <summary>
		/// Obtient la session client
		/// </summary>
		public WebSession CustomerWebSession{
			get{return(_webSession);}
		}

		/// <summary>
		/// Obtient l'identifiant du support
		/// </summary>
		public string IdMedia{
			get{return(_idMedia);}
		}
		
		/// <summary>
		/// Obtient l'identifiant du produit
		/// </summary>
		public string IdProduct{
			get{return(_idProduct);}
		}

		/// <summary>
		/// Obtient le numero de la page
		/// </summary>
		public string MediaPaging{
			get{return(_mediaPaging);}
		}

		/// <summary>
		/// Obtient la date faciale
		/// </summary>
		public string DateCover {
			get { return (_dateCover); }
		}

		/// <summary>
		/// Obtient la date de parution
		/// </summary>
		public string DateParution{
			get{return(_dateParution);}
		}


		#endregion

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="mediaPaging">Numero de page</param>
		/// <param name="dateCover">Date faciale</param>	
		/// <param name="dateParution">Date de parution</param>	
		/// <param name="webSession">Session du client</param>
		public ProofDetail(WebSession webSession, string idMedia,string idProduct,string dateCover, string dateParution,string mediaPaging)
		{
			if(webSession==null)throw(new ArgumentNullException("L'objet WebSession est null"));
			if(idMedia==null)throw(new ArgumentNullException("Le parametre idMedia est null"));
			if (idProduct == null) throw (new ArgumentNullException("Le parametre idProduct est null"));
			if (mediaPaging == null) throw (new ArgumentNullException("Le parametre mediaPaging est null"));
			if (dateCover == null) throw (new ArgumentNullException("Le parametre dateCover est null"));
			if (dateParution == null) throw (new ArgumentNullException("Le parametre dateParution est null"));
			_idMedia = idMedia;
			_idProduct = idProduct;
			_webSession = webSession;
			_mediaPaging = mediaPaging;
			_dateCover = dateCover;
			_dateParution = dateParution;
		}
	}
}
