#region Informations
// Auteur:
// Date de cr�ation:
// Date de modification:
//      24/10/2005 D. V. Mussuma Ajout des fonctions de conversion des unit�s
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
	/// Fonctions de travail sur les unit�s
	/// </summary>
	public class Units : FctUtilities.Units{
		
		#region Unit�s disponibles en fonction des vehicule
		/// <summary>
		/// Renvoie les unit�s disponibles en fonction des vehicule d'�tude
		/// </summary>
		/// <param name="vehicleSelection">Liste des vehicule s�pr�s par un virgule</param>
		/// <returns>Liste des unit�s disponibles</returns>
		public new static ArrayList getUnitsFromVehicleSelection(string vehicleSelection){

            return new ArrayList(FctUtilities.Units.getUnitsFromVehicleSelection(vehicleSelection));

		}
		
		/// <summary>
		/// Renvoie les unit�s disponibles de la press pour le module Appm
		/// </summary>
		/// <returns>Liste des unit�s Appm disponibles</returns>
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
