#region Informations
// Auteur: G. Facon
// Date de création: 16/11/2006
// Date de modification:
#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Core.Result{
	/// <summary>
	/// Cellule contenant un lien vers un plan media
	/// </summary>
	public class CellMediaScheduleLink:CellImageLink{

		#region Variables
		/// <summary>
		/// Session du client
		/// </summary>
		protected WebSession _webSession;
		/// <summary>
		/// Identifiant de la nomenclature sur lequel le plan media doit s'executer
		/// </summary>
		protected Int64 _classificationId;
		/// <summary>
		/// Niveau
		/// </summary>
		protected int _level;
		/// <summary>
		/// Chemin de l'image servant au lien
		/// </summary>
		protected string _imagePath="";
		/// <summary>
		/// Adresse d'appelle du plan media
		/// </summary>
		//protected string _link="javascript:OpenMediaPlanAlert('{0}','{1}','{2}');";
        protected string _link = "javascript:OpenGenericMediaSchedule('{0}','{1}','{2}');";
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellLevel">Niveau de détail</param>
		/// <param name="webSession">Session du client</param>
		public CellMediaScheduleLink(CellLevel cellLevel,WebSession webSession){
			if(webSession==null)throw(new ArgumentNullException("L'objet WebSession est null"));
			if(cellLevel==null)throw(new ArgumentNullException("L'objet cellLevel est null"));
			_classificationId=cellLevel.Id;
			_level=cellLevel.Level;
			_webSession=webSession;
            _imagePath = "/App_Themes/" + WebApplicationParameters.Themes[_webSession.SiteLanguage].Name + "/Images/Common/picto_plus.gif";
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
			if(_level>0){
				DetailLevelItemInformation.Levels detailLevelItemInformation=_webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(_level);
				if(detailLevelItemInformation==DetailLevelItemInformation.Levels.advertiser||
					detailLevelItemInformation==DetailLevelItemInformation.Levels.product||
					detailLevelItemInformation==DetailLevelItemInformation.Levels.brand||
					detailLevelItemInformation==DetailLevelItemInformation.Levels.holdingCompany||
					detailLevelItemInformation==DetailLevelItemInformation.Levels.sector||
					detailLevelItemInformation==DetailLevelItemInformation.Levels.subSector||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.group ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.segment ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.subBrand ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.program ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.programTypology ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.programGenre ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.spotSubType ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.adSlogan ||
                    detailLevelItemInformation == DetailLevelItemInformation.Levels.PurchasingAgency)
                {

                    //return(_link.Replace("{0}",_webSession.IdSession).Replace("{1}",_classificationId.ToString()).Replace("{2}",_level.ToString()));
                    return string.Format("id={0}&level={1}",  _classificationId, _level.ToString());
                }
			}
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

