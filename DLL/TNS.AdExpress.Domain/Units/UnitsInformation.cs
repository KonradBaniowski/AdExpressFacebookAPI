#region Informations
// Auteur: G. Facon
// Cr�ation: 05/08/2008
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.Units {

    /// <summary>
    /// Units descriptions
    /// </summary>
    public class UnitsInformation {

        #region variables
		///<summary>
		/// Units description list
		/// </summary>
        private static Dictionary<CustomerSessions.Unit,UnitInformation> _list=new Dictionary<CustomerSessions.Unit,UnitInformation>();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		static UnitsInformation(){
		}
		#endregion

		#region Accesseurs
        /// <summary>
        /// Get units description list
        /// </summary>
        public static Dictionary<CustomerSessions.Unit, UnitInformation> List {
            get { return _list; }
        }
		#endregion

		#region M�thodes publiques
		/// <summary>
		/// Get Unit informations
		/// </summary>
		public static UnitInformation Get(CustomerSessions.Unit id){
			try{
				return(_list[id]);
			}
			catch(System.Exception err){
				throw(new ArgumentException("impossible to reteive the requested unit",err));
			}
		}

        /// <summary>
        /// Get Parent unit id 
        /// </summary>
        /// <param name="id">Unit id</param>
        /// <returns>Parent unit id </returns>
        public static CustomerSessions.Unit GetParentUnitId(CustomerSessions.Unit id) {
            CustomerSessions.Unit currentParentUnit = Get(id).BaseId;
            CustomerSessions.Unit currentUnit = id;
            if (currentParentUnit != CustomerSessions.Unit.none && currentParentUnit != id)
                currentUnit = GetParentUnitId(currentParentUnit);

            return currentUnit;
        }

		/// <summary>
		/// Initialisation de la liste � partir du fichier XML
		/// </summary>
		/// <param name="source">Source de donn�es</param>
		public static void Init(IDataSource source){
            _list.Clear();
			List<UnitInformation> units=UnitsDescriptionXL.Load(source);
            try{
                foreach(UnitInformation currentUnit in units){
                    _list.Add(currentUnit.Id,currentUnit);   
                }
            }
            catch(System.Exception err){
                throw(new UnitException("Impossible the unit list",err));
            }
		}
		#endregion
    }
}
