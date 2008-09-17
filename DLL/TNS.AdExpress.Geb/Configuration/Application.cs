#region Informations
// Auteur : B.Masson
// Date de cr�ation : 12/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Geb.Configuration{
	/// <summary>
	/// Classe de configuration de Geb server
	/// </summary>
	public class Application{

		#region Variables
		/// <summary>
		/// Fr�quence
		/// </summary>
		private int _refresh = 0;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="refresh">Intervalle de temps</param>
		public Application(int refresh){
			// La valeur pass� en param�tre est en seconde (d�finie dans le fichier XML)
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
