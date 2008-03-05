#region Informations
// Auteur: D. V. Mussuma
// Date de Création: 31/01/2006
// Date de Modification:
#endregion

using System;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Information spécifiques à un menu d'une entête d'un détail média
	/// </summary>
	public class HeaderMediaDetailMenuItem : HeaderMenuItem{

		#region Variables
		/// <summary>
		/// Identifiants des niveaux de détail média
		/// </summary>
		protected string _ids;
		/// <summary>
		/// Période étudiée en zoom
		/// </summary>
		protected string _zoomDate;
		/// <summary>
		/// Nombre générée à partir de la date
		/// </summary>
		protected string _param;		
		/// <summary>
		/// Identifiant du média 
		/// </summary>
		protected string _idVehicle;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient Identifiants des niveaux de détail média
		/// </summary>
		public string Ids{
			get{return(_ids);}
			set{_ids=value;}
		}

		/// <summary>
		/// Obtient la période étudiée en zoom
		/// </summary>
		public string ZoomDate{
			get{return _zoomDate;}
			set{_zoomDate=value;}
		}

		/// <summary>
		/// Nombre générée à partir de la date
		/// </summary>
		public string Param{
			get{return(_param);}
			set{_param=value;}
		}
		
		/// <summary>
		/// Identifiant du média 
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
		/// <param name="giveLanguage">Indique si la langue doit être indiquée dans l'url</param> 
		/// <param name="giveSession">Indique si l'identifiant de la session doit être indiquée dans l'url</param>
		/// <param name="target">Indique la fenêtre cible du lien courant</param>
		/// <param name="ids">Identifiants des niveaux de détail média</param>		
		/// <param name="zoomDate">période étudiée en zoom</param>
		/// <param name="idVehicle">Identifiant du média</param>
		/// <param name="param"> Nombre générée à partir de la date</param>
		public HeaderMediaDetailMenuItem(Int64 idMenu, string targetUrl,bool giveLanguage,bool giveSession,string target,string ids,string zoomDate,string param,string idVehicle):base(idMenu,targetUrl,giveLanguage,giveSession,target,false){
			_ids = ids;
			_zoomDate = zoomDate;
			_param = param;
			_idVehicle = idVehicle;
		}
	
		#endregion
	}
}
