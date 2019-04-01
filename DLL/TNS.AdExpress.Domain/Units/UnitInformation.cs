#region Information
//  Author : G. Facon
//  Creation  date: 05/08/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using System.Globalization;

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
        /// Text Id for unit sign (€, $, £ ...)
        /// </summary>
        private Int64 _webTextSignId;
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
        /// Field name in aggregated data
        /// </summary>
        private string _databaseTrendsField="";
        /// <summary>
        /// Full type name of cell
        /// </summary>
        private string _cellType="";
        /// <summary>
        /// String format name
        /// </summary>
        private string _strFormat = "N";
        private  string _assembly = "";
        /// <summary>
        /// Group Id used in select control
        /// </summary>
        private int _groupId = 0;
        /// <summary>
        /// Group Selection Type
        /// </summary>
        private GroupSelection.GroupType _groupType = GroupSelection.GroupType.mono;
        /// <summary>
        /// Group Text Id
        /// </summary>
        private Int64 _groupTextId;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Unit Id</param>
        /// <param name="format">String format for output</param>
        /// <param name="webTextId">Text Id</param>
        /// <param name="baseId">Parent Id</param>
        /// <param name="databaseField">Field name in occurencies data</param>
        /// <param name="databaseMultimediaField">Field name in aggregated data</param>
        /// <param name="assembly">assembly</param>
        public UnitInformation(string id, string format, Int64 webTextId, Int64 webTextSignId ,string baseId, string cellType, string databaseField, string databaseMultimediaField, string databaseTrendsField, string assembly, int groupId, GroupSelection.GroupType groupType, Int64 groupTextId)
        {
            if(id==null || id.Length==0) throw (new ArgumentException("Invalid paramter unit id"));
            if (cellType != null || cellType.Length > 0) _cellType = cellType;
            if(databaseField!=null || databaseField.Length>0) _databaseField=databaseField;
            if(databaseMultimediaField!=null || databaseMultimediaField.Length>0) _databaseMultimediaField=databaseMultimediaField;
            if(databaseTrendsField!=null || databaseTrendsField.Length>0) _databaseTrendsField=databaseTrendsField;
            if (format != null && format.Length > 0)
            {
                _strFormat = format;
            }
            _webTextId=webTextId;
            _webTextSignId = webTextSignId;
            _groupId = groupId;
            _groupType = groupType;
            _groupTextId = groupTextId;
            if (_assembly != null || _assembly.Length > 0) _assembly = assembly;
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
        /// Get Web Text Id for unit sign
        /// </summary>
        public Int64 WebTextSignId
        {
            get { return (_webTextSignId); }
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
        /// Get Database Multimedia Field
        /// </summary>
        public string DatabaseTrendsField {
            get { return (_databaseTrendsField); }
        }

        /// <summary>
        /// Get Full type name of cell
        /// </summary>
        public string CellType {
            get { return(_cellType); }
        }
        /// <summary>
        /// Get assembly name of cell
        /// </summary>
        public string Assembly
        {
            get { return (_assembly); }
        }
        /// <summary>
        /// Get Unit format when displayed
        /// </summary>
        public string Format
        {
            get { return _strFormat; }
        }
        /// <summary>
        /// Get Unit format ready for ToString methods ==> "{0:Format}"
        /// </summary>
        public string StringFormat {
            get { return "{0:"+_strFormat+"}"; }
        }
        /// <summary>
        /// Get Group Id
        /// </summary>
        public int GroupId
        {
            get { return _groupId; }
        }
        /// <summary>
        /// Get Group Type
        /// </summary>
        public GroupSelection.GroupType GroupType
        {
            get { return _groupType; }
        }
        /// <summary>
        /// Get Group Text Id
        /// </summary>
        public Int64 GroupTextId
        {
            get { return _groupTextId; }
        }
        #endregion

        #region Public methods

        #region SQL
        /// <summary>
        /// Get SQL database field sum query with Alias
        /// </summary>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLDetailledSum() {
            return (GetSQLDetailledSum(string.Empty));
        }

        /// <summary>
        /// Get SQL database field multimedia sum query with Alias
        /// </summary>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLSum() {
            return (GetSQLSum(string.Empty));
        }

        /// <summary>
        /// Get SQL database id sum query with Alias
        /// </summary>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLUnionSum() {
            return (" sum(" + _id.ToString() + ") as " + _id.ToString() + " ");
        }

        /// <summary>
        /// Get SQL database field sum query with Alias
        /// </summary>
        /// <param name="prefixe">prefixe of table</param>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLDetailledSum(string prefixe) {
            if (_databaseField == null || _databaseField.Length == 0) throw (new NullReferenceException("DatabaseField is not defined"));
            if (prefixe != null && prefixe.Length > 0) prefixe += ".";
            else prefixe = "";
            return (" sum(" + prefixe + _databaseField + ") as " + _id.ToString() + " ");
        }

        /// <summary>
        /// Get SQL database field multimedia sum query with Alias
        /// </summary>
        /// <param name="prefixe">prefixe of table</param>
        /// <returns>SQL sum Query with Alias</returns>
        public string GetSQLSum(string prefixe) {
            if (_databaseMultimediaField == null || _databaseMultimediaField.Length == 0) throw (new NullReferenceException("DatabaseField is not defined"));
            if (prefixe != null && prefixe.Length > 0) prefixe += ".";
            else prefixe = "";
            return (" sum(" + prefixe + _databaseMultimediaField + ") as " + _id.ToString() + " ");
        }

        #endregion

        #region Web Texts
        /// <summary>
        /// Get unit web text
        /// </summary>
        /// <param name="siteLanguage">Site language</param>
        /// <returns>Web text</returns>
        public string GetUnitWebText(int siteLanguage)
        {
            return GestionWeb.GetWebWord(_webTextId, siteLanguage);
        }
        /// <summary>
        /// Get unit capital letter web text
        /// </summary>
        /// <param name="siteLanguage">Site language</param>
        /// <returns>Web text</returns>
        public string GetUnitCapitalLetterWebText(int siteLanguage)
        {
            string unitWebText = GetUnitWebText(siteLanguage);
            return WebApplicationParameters.AllowedLanguages[siteLanguage].CultureInfo.TextInfo.ToTitleCase(unitWebText);
        }
        /// <summary>
        /// Get unit sign web text
        /// </summary>
        /// <param name="siteLanguage">Site language</param>
        /// <returns>Web text</returns>
        public string GetUnitSignWebText(int siteLanguage)
        {
            return (_webTextSignId > 0) ? GestionWeb.GetWebWord(_webTextSignId, siteLanguage) : GestionWeb.GetWebWord(_webTextId, siteLanguage);
        }
        #endregion

        #endregion

    }
}
