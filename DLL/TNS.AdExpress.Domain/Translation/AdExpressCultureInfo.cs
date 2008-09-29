#region Info
/*
 * Author : G Ragneau
 * Created on : 05/09/2008
 * Modification : 
 *      Date - Autor - Description
 * 
 * 
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TNS.AdExpress.Domain.Translation
{
    /// <summary>
    /// Extend Culture Info to support more culture specific format
    /// </summary>
    public class AdExpressCultureInfo : CultureInfo, ICustomFormatter
    {

        #region Variables
        /// <summary>
        /// Supported patterns
        /// </summary>
        Dictionary<string, string> _patterns = new Dictionary<string, string>();
        #endregion

        #region Properties
        /// <summary>
        /// Add a specific pattern
        /// </summary>
        /// <param name="name">Name of the pattern</param>
        /// <param name="format">Pattern</param>
        /// <exception cref=""
        /// <returns></returns>
        public void AddPattern(string name, string format)
        {
            if (_patterns.ContainsKey(name))
                throw new ArgumentException(string.Format("Duplicated key {0}", name));
            _patterns.Add(name.ToLower(), format);
        }
		/// <summary>
		/// Get a specific short date format
		/// </summary>
		/// <param name="name">Name of the pattern</param>	
		/// <returns>Short date format</returns>
		public string GetFormatPattern(string name) {
			if (!_patterns.ContainsKey(name.ToLower()))
				throw new ArgumentException(string.Format("The pattern {0} is not defined ", name));
			return _patterns[name.ToLower()];
		}
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="name"></param>
        public AdExpressCultureInfo(string name): base(name)
        {
        }
        #endregion

        #region IFormatProvider Implementations
        /// <summary>
        /// Get Type of format
        /// </summary>
        /// <param name="formatType">Format required</param>
        /// <returns>Format Provider</returns>
        public override object GetFormat(Type formatType)
        {
			
            // Return ourselves or null.
            if (typeof(ICustomFormatter).Equals(formatType)) return this;
            if (typeof(NumberFormatInfo).Equals(formatType)) return this.NumberFormat;
            if (typeof(DateTimeFormatInfo).Equals(formatType)) return this.DateTimeFormat;
            return null;
        }
        #endregion

        #region ICustomFormatter Implementation
        /// <summary>
        /// Format string
        /// </summary>
        /// <param name="format">Format to apply</param>
        /// <param name="arg">value</param>
        /// <param name="formatProvider">Format provider</param>
        /// <returns>value as a string</returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Make sure the object to format isn't null.
            if (arg == null) throw new ArgumentNullException("arg");

            // Make sure there is a valid format specifier.
            if (format != null)
            {
                string f = format.ToLower();
                if (_patterns.ContainsKey(format.ToLower()))
                {
                    return ((IFormattable)arg).ToString(_patterns[f], this);
                }
            }

            // Default to the format provided by the argument to format.
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, this);
            else return arg.ToString();
        }
        #endregion

		#region Methods
		/// <summary>
		/// Get format pattern from string Format like "{0:Format}"
		/// </summary>
		/// <param name="stringFormat">string Format like "{0:Format}"</param>
		/// <returns>Format pattern like "# ##0"</returns>
		public string GetFormatPatternFromStringFormat(string stringFormat) {
			if (stringFormat == null || stringFormat.Length == 0 || stringFormat.Length<5 ) throw new ArgumentNullException(" StringFormat is null");
			string formatKey = stringFormat.Substring(3, stringFormat.Length - 4);
			if (!_patterns.ContainsKey(formatKey.ToLower()))throw new ArgumentException(string.Format("The pattern {0} is not defined ", formatKey));
			return _patterns[formatKey.ToLower()];
		}
		#endregion

	}
}
