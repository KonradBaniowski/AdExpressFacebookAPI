#region Informations
// Auteur : B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Hermes.ScheduleServer.Configuration{
	/// <summary>
	/// Classe de configuration de Hermes Schedule Server
	/// </summary>
	public class Application{
		
		#region Variables
		/// <summary>
		/// Fréquence
		/// </summary>
		private int _refresh = 0;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="refresh">Intervalle de temps</param>
		public Application(int refresh){
			// La valeur passé en paramètre est en seconde (définie dans le fichier XML)
			// On multiplie la valeur par 1000 pour transformer la valeur en milliseconde
			_refresh = refresh * 1000;
		}
		#endregion

		#region Accesseur
		/// <summary>
		/// Obtient l'intervalle de temps en 2 refresh
		/// </summary>
		public int Refresh{
			get{return _refresh;}
		}
		#endregion

	}
}
