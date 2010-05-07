#region Informations
// Auteur: G. Facon
// Cr�ation: 05/08/2008
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.XmlLoader;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Bastet.Units {

    /// <summary>
    /// Units descriptions
    /// </summary>
    public class UnitsInformation {

        #region variables
		///<summary>
		/// Units description list
		/// </summary>
        private static Dictionary<CustomerSessions.Unit,UnitInformation> _list=new Dictionary<CustomerSessions.Unit,UnitInformation>();
        ///<summary>
        /// Units description list
        /// </summary>
        private static Dictionary<Int64, UnitInformation> _listDataBaseId = new Dictionary<Int64, UnitInformation>();
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
        /// Get Unit informations
        /// </summary>
        public static UnitInformation Get(Int64 dataBaseId) {
            try {
                return (_listDataBaseId[dataBaseId]);
            }
            catch (System.Exception err) {
                throw (new ArgumentException("impossible to reteive the requested unit", err));
            }
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