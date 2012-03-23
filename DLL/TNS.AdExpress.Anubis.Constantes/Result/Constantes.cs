#region Informations
// Auteur: G. Facon
// Date de création: 20/05/2005
// Date de modification: 20/05/2005
#endregion


using System;

namespace TNS.AdExpress.Anubis.Constantes{
	/// <summary>
	/// Description résumée de Constantes.
	/// </summary>
	public class Result{
		/// <summary>
		/// Lien pour récupérer les css
		/// </summary>
        //public const string CSS_LINK="www.tnsadexpress.com";
        //public const string CSS_LINK = "http://localhost:3525/App_Themes";
		/// <summary>
		/// indique le type de résultat à traiter
		/// </summary>
		public enum type{
			/// <summary>
			/// Type de résultat inconnu
			/// </summary>
			unknown=0,
			/// <summary>
			/// Type APPM
			/// </summary>
			appm=1,
			/// <summary>
			/// Type Bastet
			/// </summary>
			bastet=2,
			/// <summary>
			/// Type alerte Geb (alerte portefeuille)
			/// </summary>
			gebAlert=3,
			/// <summary>
			/// Type détail insertion APPM
			/// </summary>
			appmInsertionDetail=4,
			/// <summary>
			/// Type APPM excel (plug-in Satet)
			/// </summary>
			appmExcel=5, 
			/// <summary>
			/// Type Indicateurs PDF (plug-in Hotep)
			/// </summary>
			hotep=6,
			/// <summary>
			/// Type Alerte Plan Média PDF (plug-in Miysis)
			/// </summary>
			miysis=7,
			/// <summary>
			/// Type APPM Plan Media PDF (plug-in Mnevis)
			/// </summary>
			mnevis=8,
			/// <summary>
			/// Type Justificatifs Presse PDF (plug-in Shou)
			/// </summary>
			shou = 9,
			/// <summary>
			/// Type Données de Cadrage Excel (Plug-in Amset)
			/// </summary>
			amset= 10,
			/// <summary>
			/// Type Données de Cadrage PDF (Plug-in Aton)
			/// </summary>
			aton= 11,
			/// <summary>
			/// Type Indicateurs Press (Hermes)
			/// </summary>
			hermesPress= 12,
			/// <summary>
			/// Type Indicateurs Radio TV (Hermes)
			/// </summary>
			hermesRadioTV= 13,
            /// <summary>
            /// Alerte AdExpress
            /// </summary>
            alertAdExpress = 20,
            /// <summary>
            /// Export Excel veille promo
            /// </summary>
            tefnout = 21,
            /// <summary>
            /// Export pdf veille promo file
            /// </summary>
            selket = 22,
            /// <summary>
            /// Export pdf veille promo calendar schedule
            /// </summary>
            thoueris = 23,
             /// <summary>
            /// Export des visuels Evaliant (Plug-in Dedoum)
            /// </summary>
            dedoum = 24,
            /// Export texte parrainage TV
            /// </summary>
            pachet = 25

            
		}

		/// <summary>
		/// Statut de la génération du résultat
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
			/// traité
			/// </summary>
			done=2,
			/// <summary>
			/// envoyé
			/// </summary>
			sent=3,
			/// <summary>
			/// Une erreur est survenue
			/// </summary>
			error=4
		}
	}
}
