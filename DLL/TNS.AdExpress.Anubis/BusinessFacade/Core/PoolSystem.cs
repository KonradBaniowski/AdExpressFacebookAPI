#region Informations
// Auteur: G. Facon
// Date de création: 20/05/2005
// Date de modification: 20/05/2005
#endregion


using System;
using System.Collections;
using TNSAnubisExceptions=TNS.AdExpress.Anubis.Exceptions;
using TNS.AdExpress.Anubis.Common.Core;

namespace TNS.AdExpress.Anubis.BusinessFacade.Core{
	/// <summary>
	/// Liste des demandes de résultats
	/// </summary>
	public class PoolSystem{

		#region Variables
		/// <summary>
		/// Liste des demandes de résultat
		/// </summary>
		private static ArrayList _list; 
		#endregion

		#region Evènement
		/// <summary>
		/// Le nombre de connection change
		/// </summary>
		public delegate void ConnectionNumberChangedHandler(int itemsNumber);
		/// <summary>
		/// Erreur
		/// </summary>
		public delegate void ErrorHandler(System.Exception err);
		/// <summary>
		/// Evènement indiquant que le nombre d'éléments dans la liste à changé
		/// </summary>
		public static event ConnectionNumberChangedHandler ConnectionNumberChanged;
		/// <summary>
		/// Evènement indiquant qu'il y a une erreur
		/// </summary>
		public static event ErrorHandler Error;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		static PoolSystem(){
			_list=new ArrayList();
		}
		#endregion

		#region Méthodes externes

		/// <summary>
		/// Initialise l'objet
		/// </summary>
		public static void Init(){
		}

		/// <summary>
		/// Obtient une demande
		/// </summary>
		/// <returns>Demande</returns>
		public static PoolItem Get(){
			PoolItem currentPoolItem;
			lock(_list){
				if(_list.Count<1) throw(new TNSAnubisExceptions.PoolEmptyException());
				currentPoolItem=(PoolItem)_list[0];
				try{
					_list.RemoveAt(0);
				}
				catch(System.Exception err){
					Error(new TNSAnubisExceptions.PoolFatalException("Impossible de supprimer dans liste des demandes",err));
				}
				ConnectionNumberChanged(_list.Count);
				return(currentPoolItem);
			}
		}

		/// <summary>
		/// Ajoute une demande
		/// </summary>
		/// <param name="poolItem">Demande à ajouter</param>
		public static void Put(PoolItem poolItem){
			if(poolItem==null)throw(new ArgumentNullException("La demande est null"));
			lock(_list){
				try{
					_list.Add(poolItem);
					ConnectionNumberChanged(_list.Count);
				}
				catch(System.Exception err){
					Error(new TNSAnubisExceptions.PoolException("Impossible d'insérer la demande dans liste des demandes: "+err.Message+" - "+err.StackTrace));
				}
			}
		}

		/// <summary>
		/// Obtient le nombre d'éléments dans la liste
		/// </summary>
		/// <returns>nombre d'éléments dans la liste</returns>
		public static int Count(){
			lock(_list){
				return(_list.Count);
			}
		}

		#endregion
	}
}
