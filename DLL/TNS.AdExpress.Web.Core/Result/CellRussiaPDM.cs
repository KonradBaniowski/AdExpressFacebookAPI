using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result{
    /// <summary>
    /// Cellule contenant des pdm (Pour AdExpress Russia)
    /// </summary>
    [System.Serializable]
    public class CellRussiaPDM : CellPDM{

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public CellRussiaPDM() : base() { }
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="unitValue">Valeur de la cellule</param>
        public CellRussiaPDM(double unitValue){
            _nullableValue = unitValue;
        }
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="unitValue">Valeur de la cellule</param>
		/// <param name="reference">Cellule de référence</param>
        public CellRussiaPDM(double unitValue, CellUnit reference)
            : this(unitValue){
            if (reference == null) _isTotal = true;
            _reference = reference;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="unitValue">Valeur de la cellule (Elle peut être null)</param>
        public CellRussiaPDM(double? unitValue){
            _nullableValue = unitValue;
        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="unitValue">Valeur de la cellule (Elle peut être null)</param>
        /// <param name="reference">Cellule de référence</param>
        public CellRussiaPDM(double? unitValue, CellUnit reference)
            : this(unitValue){
            if (reference == null) _isTotal = true;
            _reference = reference;
        }
		#endregion

        #region GetInstance
        /// <summary>
        /// Obtient un Objet par défaut à partir du type
        /// </summary>
        /// <returns>Cell</returns>
        new public static Cell GetInstance(){
            return (new CellRussiaPDM());
        }
        #endregion

        #region Surcharge de CellUnit
        /// <summary>
        /// Création d'une nouvelle instance de l'objet, initialisée par le paramètre
        /// </summary>
        /// <param name="unitValue">Valeur d'initialisation</param>
        /// <returns>Nouvelle instance</returns>
        public override CellUnit Clone(double unitValue){
            CellRussiaPDM c = new CellRussiaPDM(unitValue);
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
            CellRussiaPDM c = new CellRussiaPDM(unitValue);
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
