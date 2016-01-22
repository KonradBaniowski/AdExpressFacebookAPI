#region Informations
// Auteur: G. Facon
// Cr�ation: 09/05/2006
// Modification:
#endregion

using System;
using System.Collections;
using TNS.AdExpress.Domain.Level;

namespace TNS.AdExpress.Web.Core.Sessions{
	/// <summary>
	/// Description d'un niveau de d�tail g�n�rique d'un client sauvegard�
	/// </summary>
	public class GenericDetailLevelSaved:GenericDetailLevel{

		#region Variables
		/// <summary>
		/// Identifiant du niveau sauvegard�
		/// </summary>
		private Int64 _id;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">Identifiant du niveau sauvegard�</param>
		/// <param name="levelIds">Liste des identifiant des �l�ments duniveau de d�tail</param>
		/// <remarks>levelIds doit contenir des int</remarks>
		public GenericDetailLevelSaved(Int64 id,ArrayList levelIds):base(levelIds){
			_id=id;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Identifiant du niveau sauvegard�
		/// </summary>
		public Int64 Id{
			get{return(_id);}
		}
		#endregion
	}
}
