#region Informations
// Auteur : B.Masson
// Date de création : 14/04/2006
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Data;
using TNS.AdExpress.Geb.DataAccess;
using GebExceptions=TNS.AdExpress.Geb.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Geb{
	/// <summary>
	/// Classe des alertes par support
	/// </summary>
	public class AlertByMedia{

		#region Variables
		/// <summary>
		/// Liste des supports
		/// </summary>
		private Hashtable _mediaList = new Hashtable();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <remarks>ds2 à virer</remarks>
		public AlertByMedia(IDataSource source, DataSet ds2){
			try{
				// Récupération de la liste des alertes
				DataSet ds = AlertByMediaDataAccess.GetData(source,ds2);

				if(ds!=null && ds.Tables.Count>0 && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
					// Création d'une hashtable contenant les alertes par support :
					// Clé : Identifiant du support
					// Objet : Arraylist contenant les alertes
					foreach(DataRow currentRow in ds.Tables[0].Rows){
						try{
							_mediaList.Add(Int64.Parse(currentRow["id_media"].ToString()),new ArrayList());
						}
						catch(System.ArgumentException){
							// La clé existe déjà, ce n'est pas une erreur
						}
						((ArrayList)_mediaList[Int64.Parse(currentRow["id_media"].ToString())]).Add((Int64)currentRow["id_alert"]);
					}
				}
			}
			catch(System.Exception err){
				throw(new GebExceptions.AlertByMediaException("Impossible de construire la liste des alertes par support",err));
			}
		}
		#endregion

		#region Accesseur
		/// <summary>
		/// Obtient la liste des alertes pour un ID support donné
		/// </summary>
		public ArrayList this[Int64 index]{
			get{
				return((ArrayList)_mediaList[index]);
			}
		}
		/// <summary>
		/// Obtient le nombre de supports impactés par les alertes
		/// </summary>
		public int Count{
			get{return(_mediaList.Count);}
		}
		#endregion

		#region Méthode interne
		/// <summary>
		/// Indique si oui ou non un identifiant support est présent dans la hashtable
		/// </summary>
		/// <param name="mediaId">Identifiant du support</param>
		/// <returns>True si l'identifiant du support donné est dans la hashtable</returns>
		internal bool Contains(Int64 mediaId){
			return(_mediaList.ContainsKey(mediaId));
		}
		#endregion

	}
}
