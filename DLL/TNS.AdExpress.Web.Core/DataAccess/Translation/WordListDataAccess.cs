#region Informations
// Author: G. Facon
// Creation Date:
// Modification Date:
//	11/08/2005	G. Facon	variable names
#endregion

using System;
using System.Data;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Exceptions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;


namespace TNS.AdExpress.Web.Core.DataAccess.Translation{
	/// <summary>
	/// Class used to load all the words of a language
	/// </summary>
	public class WordListDataAccess{

		#region Variables
		/// <summary>
		/// DataBase connection
		/// </summary>
		private IDataSource _source;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
        /// <param name="source">Data source</param>
        protected WordListDataAccess(IDataSource source) {
            if(source==null) throw (new ArgumentNullException("Invalid source parameter"));
            _source=source;
		}
		#endregion


		/// <summary>
		/// Load the texts
		/// </summary>
		/// <param name="SiteLanguage">Language Id</param>
		/// <returns>Texts list</returns>
		protected  string[] GetList(int SiteLanguage){
            DataSet ds,dsMax;
            int max=0;

            #region request Max
            string sqlMax="SELECT max(id_web_text)as max from "+DBConstantes.Schema.APPLICATION_SCHEMA+".web_text";
            try {
                dsMax=_source.Fill(sqlMax);
                max=int.Parse(dsMax.Tables[0].Rows[0]["max"].ToString()); 
            }
            catch(System.Exception err) {
                throw(new WordListDBException("Impossible to get max word id",err));
            }
            #endregion

            string[] list=new string[max+1];

            #region word request
            string sql="SELECT id_web_text,web_text from "+DBConstantes.Schema.APPLICATION_SCHEMA+".web_text where id_language="+SiteLanguage.ToString()+" order by id_web_text";
            #endregion
            try {
                ds=_source.Fill(sql);
                if(ds.Tables!=null && ds.Tables.Count>0 && ds.Tables[0].Rows!=null) {
                    foreach(DataRow currentRow in ds.Tables[0].Rows) {
                        list[Int64.Parse(currentRow[0].ToString())]=currentRow[1].ToString();
                    }
                }
            }
            catch(System.Exception err) {
                throw (new WordListDBException("Impossible to get words",err));
            }
			return(list);
		}
	}
}
