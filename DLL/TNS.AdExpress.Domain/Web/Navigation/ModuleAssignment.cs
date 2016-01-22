#region Informations
// Auteur: g. Facon
// Date de création:
// Date de modification:
//	G. Facon    11/08/2005      Nom de variables
//  G. Facon    27/08/2008      Add Allowed Media universe
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Classification;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	///  Classe utilisée dans l'affichage des textes des modules
	/// </summary>
	public class ModuleAssignment{
		
		#region Variable
        /// <summary>
        /// moduleId
        /// </summary>
        protected Int64 _moduleId;
        /// <summary>
        /// Date Begin
        /// </summary>
        protected DateTime _dateBegin;
        /// <summary>
        /// Date End
        /// </summary>
        protected DateTime _dateEnd;
        /// <summary>
        /// Frequency
        /// </summary>
        protected Int64 _frequency;
        /// <summary>
        /// Nb Alert
        /// </summary>
        protected Int64 _nbAlert;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="moduleId">Module ID</param>
		/// <param name="dateBegin">Date begin</param>
		/// <param name="dateEnd">Date End</param>
		/// <param name="frequency">Frequency</param>
        public ModuleAssignment(Int64 moduleId, DateTime dateBegin, DateTime dateEnd, Int64 frequency, Int64 nbAlert) {
            _moduleId = moduleId;
			_dateBegin=dateBegin;
            _dateEnd = dateEnd;
            _frequency = frequency;
            _nbAlert = nbAlert;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// get Module Id
		/// </summary>
        public Int64 ModuleId
        {
            get { return _moduleId; }
        }

        /// <summary>
        /// get Date Begin
        /// </summary>
        public DateTime DateBegin {
            get { return _dateBegin; }
        }

        /// <summary>
        /// get Date End
        /// </summary>
        public DateTime DateEnd {
            get { return _dateEnd; }
        }

        /// <summary>
        /// Get Frequency
        /// </summary>
        public Int64 Frequency {
            get { return _frequency; }
        }

        /// <summary>
        /// Get Nb Alert
        /// </summary>
        public Int64 NbAlert {
            get { return _nbAlert; }
        }
		#endregion

	}
}
