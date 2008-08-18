#region Information
//  Author : G. Facon
//  Creation  date: 05/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.Units {
    /// <summary>
    /// Unit description
    /// </summary>
    public class UnitInformation {

        #region Variables
        /// <summary>
        /// Unit Id
        /// </summary>
        private CustomerSessions.Unit _id;
        /// <summary>
        /// Text Id
        /// </summary>
        private Int64 _webTextId;
        /// <summary>
        /// Parent Id
        /// </summary>
        /// <example>For radio spot correspond to insertion where multimedia vehicles are selected</example>
        private CustomerSessions.Unit _baseId=CustomerSessions.Unit.none;
        /// <summary>
        /// Field name in occurencies data
        /// </summary>
        private string _databaseField="";
        /// <summary>
        /// Field name in aggregated data
        /// </summary>
        private string _databaseMultimediaField="";
        /// <summary>
        /// Full type name of cell
        /// </summary>
        private string _cellType="";
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Unit Id</param>
        /// <param name="webTextId">Text Id</param>
        /// <param name="baseId">Parent Id</param>
        /// <param name="databaseField">Field name in occurencies data</param>
        /// <param name="databaseMultimediaField">Field name in aggregated data</param>
        public UnitInformation(string id,Int64 webTextId,string baseId,string cellType,string databaseField,string databaseMultimediaField) {
            if(id==null || id.Length==0) throw (new ArgumentException("Invalid paramter unit id"));
            if (cellType != null || cellType.Length > 0) _cellType = cellType;
            if(databaseField!=null || databaseField.Length>0) _databaseField=databaseField;
            if(databaseMultimediaField!=null || databaseMultimediaField.Length>0) _databaseMultimediaField=databaseMultimediaField; 
            _webTextId=webTextId;
            try {
                _id=(CustomerSessions.Unit)Enum.Parse(typeof(CustomerSessions.Unit),id,true);
                if(baseId!=null && baseId.Length>0) _baseId=(CustomerSessions.Unit)Enum.Parse(typeof(CustomerSessions.Unit),baseId,true);
            }
            catch(System.Exception err) {
                throw (new ArgumentException("Invalid parameter",err));
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get unit Id
        /// </summary>
        public CustomerSessions.Unit Id {
            get{return(_id);}
        }

        /// <summary>
        /// Get Web Text Id
        /// </summary>
        public Int64 WebTextId {
            get { return (_webTextId); }
        }

        /// <summary>
        /// Get base unit Id
        /// </summary>
        public CustomerSessions.Unit BaseId {
            get { return (_baseId); }
        }

        /// <summary>
        /// Get Database Field
        /// </summary>
        public string DatabaseField {
            get { return (_databaseField); }
        }

        /// <summary>
        /// Get Database Multimedia Field
        /// </summary>
        public string DatabaseMultimediaField {
            get { return (_databaseMultimediaField); }
        }

        /// <summary>
        /// Get Full type name of cell
        /// </summary>
        public string CellType {
            get { return(_cellType); }
        }

        #endregion

        #region Public methods

        #region SQL
        /// <summary>
        /// Get SQL database field sum query with Alias
        /// </summary>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLDetailledSum() {
            if (_databaseField == null || _databaseField.Length == 0) throw (new NullReferenceException("DatabaseField is not defined"));
            return (" sum("+_databaseField+") as "+_id.ToString()+" ");
        }

        /// <summary>
        /// Get SQL database field multimedia sum query with Alias
        /// </summary>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLSum() {
            if (_databaseMultimediaField == null || _databaseMultimediaField.Length == 0) throw (new NullReferenceException("DatabaseField is not defined"));
            return (" sum(" + _databaseMultimediaField + ") as " + _id.ToString() + " ");
        }

        /// <summary>
        /// Get SQL database id sum query with Alias
        /// </summary>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLUnionSum() {
            return (" sum(" + _id.ToString() + ") as " + _id.ToString() + " ");
        }

        #endregion
        #endregion
    }
}
