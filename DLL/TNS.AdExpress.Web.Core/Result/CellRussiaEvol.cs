using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result
{
    ///<summary>
    /// Cell evolution for Russia
    /// </summary>
    ///  <author>D. Mussuma</author>
    ///  <since>13/04/2011</since>
    [System.Serializable]
    public class CellRussiaEvol : CellEvol
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public CellRussiaEvol() : base() { }
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="unitValue">Valeur de la cellule</param>
        public CellRussiaEvol(double unitValue):base(unitValue){            
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="unitValue">Valeur de la cellule (Elle peut être null)</param>
        public CellRussiaEvol(double? unitValue):base(unitValue){           
        }
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="cellValue">Cellule de la valeur servant à calculer l'évolution</param>
		/// <param name="cellReference">Cellule de référence servant à calculer l'évolution</param>
        public CellRussiaEvol(ICell cellValue, ICell cellReference):base(cellValue,cellReference)
        {
        }
		#endregion

       

        /// <summary>
        /// Manage Cell Reference if is zero 
        /// </summary>
        /// <returns>value</returns>
        protected override void TreatZeroCellReference()
        {
            _nullableValue = (_cellValue.Value == 0.0) ? 0.0 : 100.0; 
        }
    
        
    }
}
