#region Informations
// Auteur: Y. R'kaina
// Date de création: 09/04/2010
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result{
    /// <summary>
    /// Cellule contenant des KRubles ou KUSD (Pour AdExpress Russia)
    /// </summary> 
    [System.Serializable]
    public class CellRussiaKCurrency : CellKEuro{

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public CellRussiaKCurrency() : base() { }
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="unitValue">Valeur de la cellule</param>
        public CellRussiaKCurrency(double unitValue){
            _nullableValue = unitValue;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="unitValue">Valeur de la cellule (Elle peut être null)</param>
        public CellRussiaKCurrency(double? unitValue){
            _nullableValue = unitValue;
        }
		#endregion

        #region GetInstance
        /// <summary>
        /// Obtient un Objet par défaut à partir du type
        /// </summary>
        /// <returns>Cell</returns>
        new public static Cell GetInstance(){
            return (new CellRussiaKCurrency());
        }
        #endregion

        #region Surcharge de CellUnit
        /// <summary>
        /// Création d'une nouvelle instance de l'objet, initialisée par le paramètre
        /// </summary>
        /// <param name="unitValue">Valeur d'initialisation</param>
        /// <returns>Nouvelle instance</returns>
        public override CellUnit Clone(double unitValue){
            CellRussiaKCurrency c = new CellRussiaKCurrency(unitValue);
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
            CellRussiaKCurrency c = new CellRussiaKCurrency(unitValue);
            c._cssClass = this._cssClass;
            c._displayContent = this._displayContent;
            c._strFormat = this._strFormat;
            return (c);
        }
        #endregion

        #region Value Render
        /// <summary>
        /// Value Render : nbsp; or Value
        /// </summary>
        /// <returns>Value to show</returns>
        protected override string GetValueRender(){

            return this.ToString();
        }
        #endregion

    }
}
