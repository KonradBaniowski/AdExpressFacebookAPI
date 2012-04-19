using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Results;

namespace TNS.AdExpressI.Celebrities
{
    ///<summary>
    /// Data for Celebrities report
    /// </summary>
    public class CelebritiesData: ResultData
    {

        #region Variables       
        ///<summary>
        /// Celebrities Header (usefull for pagination)
        /// </summary>
        protected string _headers = string.Empty;
        ///<summary>
        /// Number of periods (usefull for pagination)
        /// </summary>
        protected Int64 _periodNb = -1;
        #endregion

        #region Constructors
		///<summary>
		/// Constructor
		/// </summary>
        public CelebritiesData()
            : base()
        {
		}
		#endregion

        #region Accesseurs
        
        ///<summary>
        /// Celebrities Header (usefull for pagination)
        /// </summary>
        public string Headers
        {
            get { return (_headers); }
            set { _headers = value; }
        }
        ///<summary>
        /// Number of periods (usefull for pagination)
        /// </summary>
        public Int64 PeriodNb
        {
            get { return (_periodNb); }
            set { _periodNb = value; }
        }
        #endregion
    }
}
