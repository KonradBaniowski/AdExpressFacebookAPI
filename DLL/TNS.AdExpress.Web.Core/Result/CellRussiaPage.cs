using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result
{
    /// <summary>
    /// Cellule contenant des pages russes
    /// </summary> 
    [System.Serializable]
    public class CellRussiaPage : CellUnit{

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public CellRussiaPage() : base() { }
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="unitValue">Valeur de la cellule</param>
        public CellRussiaPage(double unitValue){
            base.Value = unitValue;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="unitValue">Valeur de la cellule (Elle peut être null)</param>
        public CellRussiaPage(double? unitValue){
            _nullableValue = unitValue;
        }
		#endregion

        #region GetInstance
        /// <summary>
        /// Obtient un Objet par défaut à partir du type
        /// </summary>
        /// <returns>Cell</returns>
        public static Cell GetInstance() {
            return (new CellRussiaPage());
        }
        #endregion
			
		#region Surcharge de CellUnit
		/// <summary>
		/// Création d'une nouvelle instance de l'objet, initialisée par le paramètre
		/// </summary>
		/// <param name="unitValue">Valeur d'initialisation</param>
		/// <returns>Nouvelle instance</returns>
        public override CellUnit Clone(double unitValue){
            CellRussiaPage c = new CellRussiaPage(unitValue);
            c._cssClass = this._cssClass;
            c._displayContent = this._displayContent;
            c._strFormat = this._strFormat;
            return (c);
		}
        /// <summary>
        /// Création d'une nouvelle instance de l'objet, initialisée par le paramètre
        /// </summary>
        /// <param name="unitValue">Valeur d'initialisation (Elle peut être null)</param>
        /// <returns>Nouvelle instance</returns>
        public override CellUnit Clone(double? unitValue){
            CellRussiaPage c = new CellRussiaPage(unitValue);
            c._cssClass = this._cssClass;
            c._displayContent = this._displayContent;
            c._strFormat = this._strFormat;
            return (c);
        }
		#endregion
    }
}
