#region Info
/*
 * Author : Y. Rkaina
 * Creation : 24/08/2007
 * Modification :
 *		Author - Date - description
 *		G Ragneau - 30/04/2008 - Déplacer de TNS/.AdExpres.Web vers TNS.AdExpress.Domain
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Domain.Results {

    /// <summary>
    /// Objet version pour un export de la publicité extérieur
    /// </summary>
    public class ExportOutdoorVersionItem : VersionItem {

        #region Variables
		/// <summary>
        /// Nombre de supports
        /// </summary>
		private Int64 _nbMedia;
		/// <summary>
        /// Budget
        /// </summary>
		private Double _expenditureEuro;
		/// <summary>
        /// Nombre de panneaux
        /// </summary>
        private Int64 _nbBoards;
        /// <summary>
        /// Nombre de visuels
        /// </summary>
        private Int64 _nbVisuel;
		#endregion

		#region Accessors
		///<summary>
        ///Get / Set Nombre de supports
        ///</summary>
		public Int64 NbMedia {
			get {return (_nbMedia);}
			set {_nbMedia = value;}
		}
		///<summary>
        ///Get / Set Budget
        ///</summary>
		public Double ExpenditureEuro {
			get {return (_expenditureEuro);}
			set {_expenditureEuro = value;}
		}
		///<summary>
        ///Get / Set Nombre de panneaux
        ///</summary>
        public Int64 NbBoards {
			get {return (_nbBoards);}
            set { _nbBoards = value; }
		}
        ///<summary>
        ///Get / Set Nombre de visuels
        ///</summary>
        public Int64 NbVisuel {
            get { return (_nbVisuel); }
            set { _nbVisuel = value; }
        }
		#endregion

		#region Constructors
		///<summary>
        ///Constructor
        ///</summary>
        public ExportOutdoorVersionItem(Int64 id, string cssClass): base(id, cssClass) {
		}
		#endregion

    }
}
