#region Informations
// Auteur:
// Date de création:
// Date de modification:
//      24/10/2005 D. V. Mussuma Ajout des fonctions de conversion des unités
#endregion

using System;
using System.Collections;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DateFrameWork=TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core.Sessions;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.Functions{

	/// <summary>
	/// Fonctions de travail sur les unités
	/// </summary>
	public class Units : FctUtilities.Units{
		
		#region Unités disponibles en fonction des vehicule
		/// <summary>
		/// Renvoie les unités disponibles en fonction des vehicule d'étude
		/// </summary>
		/// <param name="vehicleSelection">Liste des vehicule séprés par un virgule</param>
		/// <returns>Liste des unités disponibles</returns>
		public new static ArrayList getUnitsFromVehicleSelection(string vehicleSelection){

            return new ArrayList(FctUtilities.Units.getUnitsFromVehicleSelection(vehicleSelection));

		}
		
		/// <summary>
		/// Renvoie les unités disponibles de la press pour le module Appm
		/// </summary>
		/// <returns>Liste des unités Appm disponibles</returns>
		public new static ArrayList getUnitsFromAppmPress() {

            return new ArrayList(FctUtilities.Units.getUnitsFromAppmPress());

        }

		/// <summary>
		/// Renvoie la liste des encarts 
		/// </summary>
		/// <returns>liste des encarts</returns>
		public new static ArrayList getInserts(){

            return new ArrayList(FctUtilities.Units.getInserts());

        }
		#endregion

	}
}
