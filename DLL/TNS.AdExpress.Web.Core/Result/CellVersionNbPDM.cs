#region Informations
/*
 * Author : G. Ragneau
 * Created on : 10/09/2008
 * Updates:
 *      Date - Author - Description
 * 
 * 
 * 
 * */
#endregion

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Collections;
using System.Collections.Generic;
using System;

namespace TNS.AdExpress.Web.Core.Result{
	/// <summary>
	/// Cell for version numbers PDM
	/// </summary> 
	[System.Serializable]
	public class CellVersionNbPDM : CellIdsNumber{

		#region Variables
        ///<summary>
		/// Calcul base
		/// </summary>
		protected CellIdsNumber _reference;
		/// <summary>
		/// Specify if cell is total ==> 100%
		/// </summary>
		protected bool _isTotal=false;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="reference">Cellule de référence</param>
        public CellVersionNbPDM(CellIdsNumber reference)
            : base()
        {
			if(reference==null)_isTotal=true;
			_reference=reference;
		}
		/// <summary>
		/// Default constructor
		/// </summary>
        public CellVersionNbPDM(string[] ids, CellIdsNumber reference)
            : base(ids)
        {
            if (reference == null) _isTotal = true;
            _reference = reference;
        }
		/// <summary>
		/// Default constructor
		/// </summary>
        public CellVersionNbPDM(Int64[] ids, CellIdsNumber reference)
            : base(ids)
        {
            if (reference == null) _isTotal = true;
            _reference = reference;
        }
		/// <summary>
		/// Default constructor
		/// </summary>
        public CellVersionNbPDM(List<Int64> ids, CellIdsNumber reference)
            : base(ids)
        {
            if (reference == null) _isTotal = true;
            _reference = reference;
        }		

		#endregion
		
		#region Accesseurs
		/// <summary>
		/// Obtient ou définit la valeur de la cellule
		/// </summary>
		public override double Value{
			get{
				if(_isTotal)return(100);
                if (_reference != null && _reference.List.length != 0) return ((base.Value / _reference.List.length) * 100.0);
                if (base.Value != 0)
                    return (Double.PositiveInfinity);
                else
                    return 0;
			}
		}
		#endregion

        #region ToString()
        public override string ToString(string format, IFormatProvider fp)
        {
            if (format != null && format != string.Empty)
                return string.Format(fp, format, this.Value);
            else
                return this.ToString("{0:N}", fp);
        }
        #endregion

        #region GetInstance
        /// <summary>
        /// Obtient un Objet par défaut à partir du type
        /// </summary>
        /// <returns>Cell</returns>
        public static Cell GetInstance() {
            return (new CellVersionNbPDM(null));
        }
        #endregion
		
		#region Surcharge de CellUnit
		/// <summary>
		/// Création d'une nouvelle instance de l'objet, initialisée par le paramètre
		/// </summary>
        /// <param name="value">Valeur d'initialisation</param>
		/// <returns>Nouvelle instance</returns>
        public override CellUnit Clone(double value)
        {
            CellVersionNbPDM c = new CellVersionNbPDM(null);
            if (value > 0)
            {
                c.Add(value);
            }
            return c;
		}
		#endregion


        #region Render
        /// <summary>
        /// Rendu de code HTML
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render()
        {
            return Render(_cssClass);
        }
        /// <summary>
        /// Rendu de code HTML avec un style css spécifique
        /// </summary>
        /// <returns>Code HTML</returns>
        public override string Render(string cssClass)
        {
            string str = "";
            if (_newGroup)
                str += RenderSeparator();
            double value = this.Value;
            return str + ((value != 0)
                ? "<td " + ((cssClass.Length > 0) ? " class=\"" + cssClass + "\"" : "") + ">" + this.ToString() + "</td>"
                : "<td " + ((cssClass.Length > 0) ? " class=\"" + cssClass + "\"" : "") + ">&nbsp;</td>");
        }
        /// <summary>
        /// Rendu de code HTML pour Excel
        /// </summary>
        /// <returns>Code HTML pour Excel</returns>
        public override string RenderExcel(string cssClass)
        {
            double value = this.Value;
            return ((value != 0)
                ? "<td " + ((cssClass.Length > 0) ? " class=\"" + cssClass + "\"" : "") + ">" + this.ToString() + "</td>"
                : "<td " + ((cssClass.Length > 0) ? " class=\"" + cssClass + "\"" : "") + ">&nbsp;</td>");
        }

        /// <summary>
        /// Rendu de code HTML pour Excel brut
        /// </summary>
        /// <returns>Code HTML pour Excel brut</returns>
        public override string RenderRowExcel()
        {
            double value = this.Value;
            return ((value != 0)
                ? "<td>" + this.ToString() + "</td>"
                : "<td>&nbsp;</td>");
        }
        #endregion

    }
	
}
