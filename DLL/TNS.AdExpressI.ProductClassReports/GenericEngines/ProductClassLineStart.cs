#region Info
/*
 * Author : G Ragneau
 * Created on : 03/02/2009
 * Modification:
 *      date - author - description
 * 
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{

    /// <summary>
    /// Specific line start to manage reference, competitor... elements
    /// </summary>
    public class ProductClassLineStart : LineStart
    {

        #region Attributes
        /// <summary>
        /// Indicates if the type of univers have been altered or not
        /// </summary>
        protected bool _universModified = false;
        /// <summary>
        /// Indicates if the personalisation must be displayed or not
        /// </summary>
        protected bool _displayPerso = false;
        /// <summary>
        /// Describge the tye of line regarding to reference and competitors universes
        /// </summary>
        protected UniversType _universType = UniversType.neutral;
        /// <summary>
        /// Css Styles to use if tle line is neutral, reference, competitor, both
        /// </summary>
        protected string[] _cssStyles = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
        /// <summary>
        /// background color to use if tle line is neutral, reference, competitor, both
        /// </summary>
        protected string[] _backgroundColors = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty };
        #endregion

        #region Accesseurs
        /// <summary>
        /// Get / Set flag which specify if the perso criterion must be displayed or not
        /// </summary>
        public bool DisplayPerso
        {
            get { return _displayPerso; }
            set { _displayPerso = value; }

        }
        /// <summary>
        /// Get / Set CSS to use for the line
        /// </summary>
        public override string CssClass
        {
            get { return (_displayPerso) ? _cssStyles[_universType.GetHashCode()] : _cssStyles[0]; }
            set { 
                string[] values = value.Split(',');
                if (values != null && values.Length > 0)
                {
                    string defaultValue = (values[0] != null)?values[0]:string.Empty;

                    for (int i = 0; i < _cssStyles.Length; i++)
                    {
                        _cssStyles[i] = (values != null && values.Length > i && values[i] != null && values[i].Length > 0) ? values[i] : defaultValue;
                    }
                }
            }
        }
        /// <summary>
        /// Get / Set Background color to use for the line
        /// </summary>
        public override string BackgroundColor
        {
            get { return (_displayPerso) ? _backgroundColors[_universType.GetHashCode()] : _backgroundColors[0]; }
            set
            {
                string[] values = value.Split(',');
                if (values != null && values.Length > 0)
                {
                    string defaultValue = (values[0] != null) ? values[0] : string.Empty;

                    for (int i = 0; i <  _backgroundColors.Length; i++)
                    {
                        _backgroundColors[i] = (values != null && values.Length > i && values[i] != null && values[i].Length > 0) ? values[i] : defaultValue;
                    }
                }
            }
        }
        /// <summary>
        /// Get / Set level of line
        /// </summary>
        public virtual LineType LineType
        {
            get { return (_lineType); }
        }
        /// <summary>
        /// Get line univers
        /// </summary>
        public UniversType LineUnivers
        {
            get { return (_universType); }
        }
        #endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="lineType">Type of line</param>
        public ProductClassLineStart(LineType lineType):base(lineType){
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lineType">Type of line</param>
        /// <param name="type">Type of univers</param>
        public ProductClassLineStart(LineType lineType, UniversType type): base(lineType)
        {
            SetUniversType(type);
        }
        #endregion


        #region Reference / Competitor management
        /// <summary>
        /// Set Univers Type
        /// </summary>
        /// <example>
        /// Rules:
        /// Univers has not been set already ==> belongs only at "type"
        /// Univers has already been set and new type is the same  ==> belongs only at "type"
        /// Univers has already been set and new type is different from current ==> belongs only at two types and set to mixed
        /// </example>
        /// <param name="type">Type of univers of the current line</param>
        public void SetUniversType(UniversType type)
        {

            if (!_universModified){
                _universType = type;
            }
            else{
                if(_universType != type){
                    _universType = UniversType.mixed;
                }
            }
            _universModified = true;

        }
        #endregion

        #region Implémentation de ICell
        /// <summary>
        /// Test cells equality
        /// </summary>
        /// <param name="cell">Cell to compare</param>
        /// <returns>True if cells are equal, false either</returns>
        public override bool Equals(ICell cell)
        {
            if (cell == null)
                return false;

            if (cell.GetType() != this.GetType())
                return false;

            ProductClassLineStart lsCell = (ProductClassLineStart)cell;
            return this.LineType.GetHashCode().Equals(lsCell.LineType.GetHashCode())
                && lsCell.CssClass.Equals(this._cssClass)
                && lsCell.BackgroundColor.Equals(this.BackgroundColor)
                && lsCell.HtmlCode.Equals(this._htmlCode)
                && lsCell._universType.GetHashCode().Equals(this._universType.GetHashCode());
        }

        /// <summary>
        /// Html code for rendering
        /// </summary>
        /// <returns>Html code</returns>
        public override string Render()
        {
            return Render(CssClass);
        }
        /// <summary>
        /// Html code for rendering
        /// </summary>
        /// <param name="cssClass">Style to use</param>
        /// <returns>Html code</returns>
        public override string Render(string cssClass)
        {
            return "<tr " + ((cssClass.Length > 0) ? " class=\"" + cssClass + "\" " : "") +
                ((BackgroundColor.Length > 0) ? " bgcolor=\"" + BackgroundColor + "\" " : "") +
                ((HighlightBackgroundColor.Length > 0 && BackgroundColor.Length > 0) ? " onmouseover=\"this.style.backgroundColor='" + HighlightBackgroundColor + "';\" onmouseout=\"this.style.backgroundColor='" + BackgroundColor + "';\"" : "") +
                HtmlCode + ">";
        }
        /// <summary>
        /// Html code for excel rendering
        /// </summary>
        /// <param name="cssClass">useless</param>
        /// <returns>Excel html code</returns>
        public override string RenderExcel(string cssClass)
        {
            return "<tr>";
        }
        /// <summary>
        /// Html code for excel rendering
        /// </summary>
        /// <returns>Excel html code</returns>
        public override string RenderRowExcel()
        {
            return RenderExcel(string.Empty);
        }
        /// <summary>
        /// Get object hashcode
        /// </summary>
        /// <returns>Object hashcode</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }

    #region UniversType
    /// <summary>
    /// Describe the type of univers in which the data is included
    /// </summary>
    public enum UniversType
    {
        /// <summary>
        /// Belongs to neutral univers
        /// </summary>
        neutral=0,
        /// <summary>
        /// Belongs to reference univers only
        /// </summary>
        reference=1,
        /// <summary>
        /// Belongs to concurrent univers only
        /// </summary>
        concurrent = 2,
        /// <summary>
        /// Belongs to two univers at least (neutral and reference, reference and concurrent, reference and concurrent and neutral...)
        /// </summary>
        mixed = 3
    }
    #endregion

}