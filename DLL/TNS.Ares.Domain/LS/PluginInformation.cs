using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Ares.Domain.LS {

    #region Enum PluginType
    /// <summary>
    /// Plugin Type
    /// </summary>
    public enum PluginType
    { 
        /// <summary>
        /// Appm Plugin
        /// </summary>
        Appm = 1,
        /// <summary>
        /// Bastet Plugin
        /// </summary>
        Bastet = 2,
        /// <summary>
        /// Sobek Plugin
        /// </summary>
        Sobek = 4,
        /// <summary>
        /// Satet Plugin
        /// </summary>
        Satet = 5,
        /// <summary>
        /// Hotep Plugin
        /// </summary>
        Hotep = 6,
        /// <summary>
        /// Miysis Plugin
        /// </summary>
        Miysis = 7,
        /// <summary>
        /// Mnevis Plugin
        /// </summary>
        Mnevis = 8,
        /// <summary>
        /// Shou Plugin
        /// </summary>
        Shou = 9,
        /// <summary>
        /// Amset Plugin
        /// </summary>
        Amset = 10,
        /// <summary>
        /// Aton Plugin
        /// </summary>
        Aton = 11,
        /// <summary>
        /// Alertes Plugin
        /// </summary>
        Alertes = 20,
        /// <summary>
        /// Tefnout Plugin
        /// </summary>
        Tefnout = 21,
        /// <summary>
        /// Selket Plugin
        /// </summary>
        Selket = 22,
        /// <summary>
        /// Thoueris Plugin
        /// </summary>
        Thoueris = 23,
        /// <summary>
        /// Dedoum Plugin
        /// </summary>
        Dedoum = 24,
        /// <summary>
        /// Pachet plugin
        /// </summary>
        Pachet = 25,
        /// <summary>
        /// Apis plugin
        /// </summary>
        Apis = 26,
        /// <summary>
        /// Amon plugin
        /// </summary>
        Amon=27,
        /// <summary>
        /// Ptah plugin
        /// </summary>
        Ptah =28,
        /// <summary>
        /// Blur Press Creative
        /// </summary>
        BlurPressCreative = 29,
        /// <summary>
        /// easyMusicExcel Plugin
        /// </summary>
        easyMusicExcel = 30,
        /// <summary>
        /// easyMusicPdf Plugin
        /// </summary>
        easyMusicPdf = 31,
        /// <summary>
        /// wcbExcel Plugin
        /// </summary>
        wcbExcel = 40,
        /// <summary>
        /// wcbPdf Plugin
        /// </summary>
        wcbPdf = 41,

      
    }
    #endregion

    public class PluginInformation {

        #region Variables
        /// <summary>
        /// Use execution date or not
        /// </summary>
        private bool _useExec = false;
        /// <summary>
        /// If Delete Row When success or not
        /// </summary>
        private bool _deleteRowSuccess = false;
        /// <summary>
        /// List of execution date to exchange
        /// </summary>
        private Dictionary<DayOfWeek, PluginExec> _pluginExecList = null;
        /// <summary>
        /// Path File
        /// </summary>
        private string _filePath;
        /// <summary>
        /// Longevity
        /// </summary>
        private int _longevity;
        /// <summary>
        /// Delete Expirate
        /// </summary>
        private bool _deleteExpired;
        /// <summary>
        /// Type of Result
        /// </summary>
        private int _resultType;
        /// <summary>
        /// Name
        /// </summary>
        private string _name;
        /// <summary>
        /// Theme Path
        /// </summary>
        private string _themePath;
        /// <summary>
        /// File Extension
        /// </summary>
        private string _extension;
        /// <summary>
        /// Plugin Type
        /// </summary>
        private PluginType _typeOfPlugin;
        /// <summary>
        /// Virtual Path
        /// </summary>
        private string _virtualPath;
        /// <summary>
        /// Path File Configuration
        /// </summary>
        private string _pathFileConfiguration = string.Empty;
        /// <summary>
        /// Family Id (LS family)
        /// </summary>
        private Int32 _familyId;
        /// <summary>
        /// Assembly class
        /// </summary>
        private string _class;
        /// <summary>
        /// Assembly name
        /// </summary>
        private string _assemblyName;       
        #endregion

        #region Assessor
        /// <summary>
        /// Get Use execution date or not
        /// </summary>
        public bool UseExec {
            get { return (this._useExec); }
        }
        /// <summary>
        /// Get If Delete Row When success or not
        /// </summary>
        public bool DeleteRowSuccess {
            get { return (this._deleteRowSuccess); }
        }
        /// <summary>
        /// Get List of execution date to exchange
        /// </summary>
        public Dictionary<DayOfWeek, PluginExec> PluginExecList {
            get { return (this._pluginExecList); }
        }
        /// <summary>
        /// Get Path File
        /// </summary>
        public string FilePath {
            get { return (this._filePath); }
        }
        /// <summary>
        /// Get Longevity
        /// </summary>
        public int Longevity {
            get { return (this._longevity); }
        }
        /// <summary>
        /// Get Delete Expirate
        /// </summary>
        public bool DeleteExpired {
            get { return (this._deleteExpired); }
        }
        /// <summary>
        /// Type of Result
        /// </summary>
        public int ResultType {
            get { return (this._resultType); }
        }
        /// <summary>
        /// Get Name
        /// </summary>
        public string Name {
            get { return (this._name); }
        }
        /// <summary>
        /// Get Theme Path
        /// </summary>
        public string ThemePath {
            get { return (this._themePath); }
        }
        /// <summary>
        /// Get File Extension
        /// </summary>
        public string Extension {
            get { return (this._extension); }
        }
        /// <summary>
        /// Get Plugin Type
        /// </summary>
        public PluginType TypeOfPlugin {
            get { return (this._typeOfPlugin); }
        }
        /// <summary>
        /// Get Virtual Path
        /// </summary>
        public string VirtualPath {
            get { return (this._virtualPath); }
        }
        /// <summary>
        /// Get Path File Configuration
        /// </summary>
        public string PathFileConfiguration {
            get { return (this._pathFileConfiguration); }
        }
        /// <summary>
        /// Get Family Id (LS family)
        /// </summary>
        public int FamilyId {
            get { return (this._familyId); }
        }
        /// <summary>
        /// Assembly name
        /// </summary>
        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        /// <summary>
        /// Assembly class
        /// </summary>
        public string Class_
        {
            get { return _class; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">Path File</param>
        /// <param name="virtualPath">Virtual Path</param>
        /// <param name="longevity">Longevity</param>
        /// <param name="resultType">Type of Result</param>
        /// <param name="themePath">Theme Path</param>
        /// <param name="typeOfPlugin">Plugin Type</param>
        /// <param name="familyId">Family Id (LS family)</param>
        /// <param name="pluginExec">Specify execution for decal launch</param>
        /// <param name="useExec">Use field execution or not (use for decal launch)</param>
        /// <param name="deleteExpired">Delete Expirate</param>
        /// <param name="name">Name</param>
        /// <param name="extension">Extension</param>
        /// <param name="pathFileConfiguration">Path File Configuration</param>
        /// <param name="deleteRowSuccess">If Delete Row When success or not</param>
        /// <param name="class_">assembly default Class</param>
        /// <param name="assemblyName">Assembly Name</param>
        public PluginInformation(string filePath, string virtualPath, int longevity, int resultType, string themePath, PluginType typeOfPlugin, int familyId, bool useExec, Dictionary<DayOfWeek, PluginExec> pluginExec, bool deleteRowSuccess, bool deleteExpired, string name, string extension, string pathFileConfiguration, string class_,string assemblyName) {
            this._filePath = filePath;
            this._longevity = longevity;
            this._deleteExpired = deleteExpired;
            this._resultType = resultType;
            this._themePath = themePath;
            this._name = name;
            this._extension = extension;
            this._typeOfPlugin = typeOfPlugin;
            this._virtualPath = virtualPath;
            this._pathFileConfiguration = pathFileConfiguration;
            this._familyId = familyId;
            this._useExec = useExec;
            this._deleteRowSuccess = deleteRowSuccess;
            if (useExec && pluginExec != null)
                this._pluginExecList = pluginExec;
            else
                this._pluginExecList = new Dictionary<DayOfWeek,PluginExec>();
            _class = class_;
            _assemblyName = assemblyName;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">Path File</param>
        /// <param name="virtualPath">Virtual Path</param>
        /// <param name="longevity">Longevity</param>
        /// <param name="resultType">Type of Result</param>
        /// <param name="themePath">Theme Path</param>
        /// <param name="typeOfPlugin">Plugin Type</param>
        /// <param name="familyId">Family Id (LS family)</param>
        /// <param name="pluginExec">Specify execution for decal launch</param>
        /// <param name="useExec">Use field execution or not (use for decal launch)</param>
        /// <param name="deleteExpired">Delete Expirate</param>
        /// <param name="name">Name</param>
        /// <param name="extension">Extension</param>
        /// <param name="deleteRowSuccess">If Delete Row When success or not</param>
        public PluginInformation(string filePath, string virtualPath, int longevity, int resultType, string themePath, PluginType typeOfPlugin, int familyId, bool useExec, Dictionary<DayOfWeek, PluginExec> pluginExec, bool deleteRowSuccess, bool deleteExpired, string name, string extension)
            : this(filePath, virtualPath, longevity, resultType, themePath, typeOfPlugin, familyId, useExec, pluginExec, deleteRowSuccess, deleteExpired, name, extension, string.Empty, string.Empty, string.Empty)
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filePath">Path File</param>
        /// <param name="virtualPath">Virtual Path</param>
        /// <param name="longevity">Longevity</param>
        /// <param name="resultType">Type of Result</param>
        /// <param name="themePath">Theme Path</param>
        /// <param name="typeOfPlugin">Plugin Type</param>
        /// <param name="familyId">Family Id (LS family)</param>
        /// <param name="pluginExec">Specify execution for decal launch</param>
        /// <param name="useExec">Use field execution or not (use for decal launch)</param>
        /// <param name="deleteRowSuccess">If Delete Row When success or not</param>
        public PluginInformation(string filePath, string virtualPath, int longevity, int resultType, string themePath, PluginType typeOfPlugin, int familyId, bool useExec, Dictionary<DayOfWeek, PluginExec> pluginExec, bool deleteRowSuccess)
            : this(filePath, virtualPath, longevity, resultType, themePath, typeOfPlugin, familyId, useExec, pluginExec, deleteRowSuccess, false, "Unknown", ".pdf") { }
        #endregion
    }
}
