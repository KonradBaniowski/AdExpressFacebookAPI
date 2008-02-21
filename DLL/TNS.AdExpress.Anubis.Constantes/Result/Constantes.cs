#region Informations
// Auteur: G. Facon
// Date de cr�ation: 20/05/2005
// Date de modification: 20/05/2005
#endregion


using System;

namespace TNS.AdExpress.Anubis.Constantes{
	/// <summary>
	/// Description r�sum�e de Constantes.
	/// </summary>
	public class Result{
		/// <summary>
		/// Lien pour r�cup�rer les css
		/// </summary>
		public const string CSS_LINK="www.tnsadexpress.com";
		/// <summary>
		/// indique le type de r�sultat � traiter
		/// </summary>
		public enum type{
			/// <summary>
			/// Type de r�sultat inconnu
			/// </summary>
			unknown=0,
			/// <summary>
			/// Type APPM
			/// </summary>
			appm=101,
			/// <summary>
			/// Type Bastet
			/// </summary>
			bastet=102,
			/// <summary>
			/// Type alerte Geb (alerte portefeuille)
			/// </summary>
			gebAlert=103,
			/// <summary>
			/// Type d�tail insertion APPM
			/// </summary>
			appmInsertionDetail=104,
			/// <summary>
			/// Type APPM excel (plug-in Satet)
			/// </summary>
			appmExcel=105, 
			/// <summary>
			/// Type Indicateurs PDF (plug-in Hotep)
			/// </summary>
			hotep=106,
			/// <summary>
			/// Type Alerte Plan M�dia PDF (plug-in Miysis)
			/// </summary>
			miysis=107,
			/// <summary>
			/// Type APPM Plan Media PDF (plug-in Mnevis)
			/// </summary>
			mnevis=108,
			/// <summary>
			/// Type Justificatifs Presse PDF (plug-in Shou)
			/// </summary>
			shou = 109,
			/// <summary>
			/// Type Donn�es de Cadrage Excel (Plug-in Amset)
			/// </summary>
			amset=110,
			/// <summary>
			/// Type Donn�es de Cadrage PDF (Plug-in Aton)
			/// </summary>
			aton=111,
			/// <summary>
			/// Type Indicateurs Press (Hermes)
			/// </summary>
			hermesPress=112,
			/// <summary>
			/// Type Indicateurs Radio TV (Hermes)
			/// </summary>
			hermesRadioTV=113
		}

		/// <summary>
		/// Statut de la g�n�ration du r�sultat
		/// </summary>
		public enum status{
			/// <summary>
			/// Nouvelle demande
			/// </summary>
			newOne=0,
			/// <summary>
			/// En cours
			/// </summary>
			processing=1,
			/// <summary>
			/// trait�
			/// </summary>
			done=2,
			/// <summary>
			/// envoy�
			/// </summary>
			sent=3,
			/// <summary>
			/// Une erreur est survenue
			/// </summary>
			error=4
		}
	}
}
