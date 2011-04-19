using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace KMI.P3.Domain.Translation
{
    public class P3CultureInfo : CultureInfo, ICustomFormatter
    {

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="name"></param>
        public P3CultureInfo(string name)
            : base(name)
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
           

            // Default to the format provided by the argument to format.
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, this);
            else return arg.ToString();
        }
        #endregion
    }
}
