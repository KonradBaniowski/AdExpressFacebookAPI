#region Informations
// Auteur: G. Facon
// Création: 05/08/2008
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.XmlLoader;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Bastet.Periods {

    /// <summary>
    /// Units descriptions
    /// </summary>
    public class PeriodsInformation {

        #region variables       
        ///<summary>
        /// Periods description list
        /// </summary>
        private static Dictionary<Int64, PeriodInformation> _listDataBaseId = new Dictionary<Int64, PeriodInformation>();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
        static PeriodsInformation() {
		}
		#endregion

		#region Méthodes publiques
        /// <summary>
        /// Get Period informations
        /// </summary>
        public static PeriodInformation Get(Int64 dataBaseId) {
            try {
                return (_listDataBaseId[dataBaseId]);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested unit", err));
            }
        }

		/// <summary>
		/// Initialisation de la liste à partir du fichier XML
		/// </summary>
		/// <param name="source">Source de données</param>
		public static void Init(IDataSource source){
            _listDataBaseId.Clear();
            List<PeriodInformation> units = PeriodsDescriptionXL.Load(source);
            try{
                foreach (PeriodInformation currentUnit in units) {
                    _listDataBaseId.Add(currentUnit.DataBaseId, currentUnit); 
                }
            }
            catch(System.Exception err){
                throw(new UnitException("Impossible the unit list",err));
            }
		}
		#endregion
    }
}
