#region Informations
// Auteur: D. V. Mussuma
// Date de Cr�ation: 31/01/2006
// Date de Modification:
#endregion

using System;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Information sp�cifiques � un menu d'une ent�te d'un d�tail m�dia
	/// </summary>
	public class HeaderMediaDetailMenuItem : HeaderMenuItem{

		#region Variables
		/// <summary>
		/// Identifiants des niveaux de d�tail m�dia
		/// </summary>
		protected string _ids;
		/// <summary>
		/// P�riode �tudi�e en zoom
		/// </summary>
		protected string _zoomDate;
		/// <summary>
		/// Nombre g�n�r�e � partir de la date
		/// </summary>
		protected string _param;		
		/// <summary>
		/// Identifiant du m�dia 
		/// </summary>
		protected string _idVehicle;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient Identifiants des niveaux de d�tail m�dia
		/// </summary>
		public string Ids{
			get{return(_ids);}
			set{_ids=value;}
		}

		/// <summary>
		/// Obtient la p�riode �tudi�e en zoom
		/// </summary>
		public string ZoomDate{
			get{return _zoomDate;}
			set{_zoomDate=value;}
		}

		/// <summary>
		/// Nombre g�n�r�e � partir de la date
		/// </summary>
		public string Param{
			get{return(_param);}
			set{_param=value;}
		}
		
		/// <summary>
		/// Identifiant du m�dia 
		/// </summary>
		public string IdVehicle{
			get{return(_idVehicle);}
			set{_idVehicle=value;}
		}

		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="idMenu">Identifiant du menu</param>
		/// <param name="targetUrl">Url cible du menu</param>
		/// <param name="giveLanguage">Indique si la langue doit �tre indiqu�e dans l'url</param> 
		/// <param name="giveSession">Indique si l'identifiant de la session doit �tre indiqu�e dans l'url</param>
		/// <param name="target">Indique la fen�tre cible du lien courant</param>
		/// <param name="ids">Identifiants des niveaux de d�tail m�dia</param>		
		/// <param name="zoomDate">p�riode �tudi�e en zoom</param>
		/// <param name="idVehicle">Identifiant du m�dia</param>
		/// <param name="param"> Nombre g�n�r�e � partir de la date</param>
		public HeaderMediaDetailMenuItem(Int64 idMenu, string targetUrl,bool giveLanguage,bool giveSession,string target,string ids,string zoomDate,string param,string idVehicle):base(idMenu,targetUrl,giveLanguage,giveSession,target,false){
			_ids = ids;
			_zoomDate = zoomDate;
			_param = param;
			_idVehicle = idVehicle;
		}
	
		#endregion
	}
}
