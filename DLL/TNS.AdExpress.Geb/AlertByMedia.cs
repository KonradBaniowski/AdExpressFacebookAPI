#region Informations
// Auteur : B.Masson
// Date de cr�ation : 14/04/2006
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
		/// <remarks>ds2 � virer</remarks>
		public AlertByMedia(IDataSource source, DataSet ds2){
			try{
				// R�cup�ration de la liste des alertes
				DataSet ds = AlertByMediaDataAccess.GetData(source,ds2);

				if(ds!=null && ds.Tables.Count>0 && ds.Tables[0]!=null && ds.Tables[0].Rows.Count>0){
					// Cr�ation d'une hashtable contenant les alertes par support :
					// Cl� : Identifiant du support
					// Objet : Arraylist contenant les alertes
					foreach(DataRow currentRow in ds.Tables[0].Rows){
						try{
							_mediaList.Add(Int64.Parse(currentRow["id_media"].ToString()),new ArrayList());
						}
						catch(System.ArgumentException){
							// La cl� existe d�j�, ce n'est pas une erreur
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
		/// Obtient la liste des alertes pour un ID support donn�
		/// </summary>
		public ArrayList this[Int64 index]{
			get{
				return((ArrayList)_mediaList[index]);
			}
		}
		/// <summary>
		/// Obtient le nombre de supports impact�s par les alertes
		/// </summary>
		public int Count{
			get{return(_mediaList.Count);}
		}
		#endregion

		#region M�thode interne
		/// <summary>
		/// Indique si oui ou non un identifiant support est pr�sent dans la hashtable
		/// </summary>
		/// <param name="mediaId">Identifiant du support</param>
		/// <returns>True si l'identifiant du support donn� est dans la hashtable</returns>
		internal bool Contains(Int64 mediaId){
			return(_mediaList.ContainsKey(mediaId));
		}
		#endregion

	}
}
