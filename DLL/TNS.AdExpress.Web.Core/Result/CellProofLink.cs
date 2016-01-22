#region Informations
// Auteur: D. Mussuma
// Date de création: 29/01/2007
// Date de modification:
#endregion

using System;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork;
using TNS.FrameWork.WebResultUI;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core.Result{


	/// <summary>
	/// Cellule représentant un lien vers une fiche justifivcative.
	/// </summary>
	public class  CellProofLink : CellImageLink {

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _webSession;

		/// <summary>
		/// Chemin de l'image servant au lien
		/// </summary>
		protected string _imagePath="";
		/// <summary>
		/// Adresse d'appelle du justificatif
		/// </summary>
		protected string _link="javascript:OpenProof('{0}','{1}','{2}','{3}','{4}','{5}');";

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
		/// Date de couverture
		/// </summary>
		protected string _dateFacial = null;

		/// <summary>
		/// Date de parution
		/// </summary>
		protected string _dateParution = null;

		#endregion

		#region Constructeurs


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="mediaPaging">Numero de page</param>
		/// <param name="date">Date</param>	
		/// <param name="dateParution">Date de parution</param>	
		/// <param name="webSession">Session du client</param>
		public CellProofLink(WebSession webSession,string idMedia, string idProduct, string mediaPaging, string dateFacial, string dateParution ) {
			if(webSession==null)throw(new ArgumentNullException("L'objet WebSession est null"));
			if(idMedia==null)throw(new ArgumentNullException("L'objet idMedia est null"));
			if(idProduct==null)throw(new ArgumentNullException("L'objet idProduct est null"));
			if(mediaPaging==null)throw(new ArgumentNullException("L'objet mediaPaging est null"));
			if(dateFacial==null)throw(new ArgumentNullException("L'objet dateFacial est null"));
			if(dateParution==null)throw(new ArgumentNullException("L'objet dateParution est null"));
			_idMedia = idMedia;
			_idProduct = idProduct;
			_webSession = webSession;
			_mediaPaging = mediaPaging;
			_dateFacial = dateFacial;
			_dateParution = dateParution;
            _imagePath = "/App_Themes/"+WebApplicationParameters.Themes[_webSession.SiteLanguage].Name+"/Images/Common/picto_plus.gif";
		}
		#endregion

		/// <summary>
		/// Obtient le chemin de l'image contenant le lien
		/// </summary>
		/// <returns>Chemin de l'image contenant le lien</returns>
		public override string GetImagePath(){
			return(_imagePath);
		}
		/// <summary>
		/// Obtient l'adresse du lien
		/// </summary>
		/// <returns>Adresse du lien</returns>
		public override string GetLink(){
				StringBuilder html = new StringBuilder(100); 
				if(_idMedia.Length>0 && _idProduct.Length>0 && _dateFacial.Length>0 && _dateParution.Length>0  && _mediaPaging.Length>0)
				return(string.Format(_link,_webSession.IdSession,_idProduct,_idMedia,_dateFacial,_dateParution,_mediaPaging));
			return("");
		}

		/// <summary>
		/// Retourne une représentation string de l'objet
		/// </summary>
		/// <returns>Chaine de caractère</returns>
		public override string ToString(){
			throw(new NotImplementedException());
		}
		/// <summary>
		/// Comparaison les valeurs de deux cellules.
		/// </summary>
		/// <param name="cell">Cellule à comparer</param>
		/// <remarks>Au cas où cell n'est pas comparable, on utilise ToString pour comparer les objets.</remarks>
		/// <returns>1 si objet courant supérieur à cell, -1 si objet courant inférieur à cell et 0 si les deux objets sont égaux</returns>
		public override int CompareTo(object cell){
			throw(new NotImplementedException());
		}
		/// <summary>
		/// Teste l'égalité de deux cellules.
		/// </summary>
		/// <param name="cell">Cellule à comparer</param>
		/// <returns>vrai si les deux cellules sont égales, faux sinon</returns>
		public override bool Equals(object cell){
			throw(new NotImplementedException());
		}
		/// <summary>
		/// Rendu de code HTML pour Excel
		/// </summary>
		/// <returns>Code HTML pour Excel</returns>
		public override string RenderExcel(string cssClass){
			return("");
		}
				
	}
}
