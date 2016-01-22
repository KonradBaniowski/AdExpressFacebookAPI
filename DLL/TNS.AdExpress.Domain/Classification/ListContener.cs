#region Informations
// Author: G. Facon
// Creation Date: 27/08/2008 
// Modification Date: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using DomainException=TNS.AdExpress.Domain.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;

namespace TNS.AdExpress.Domain.Classification {
    /// <summary>
    /// Classification list Generic class
    /// </summary>
    /// <typeparam name="T">Type of the list</typeparam>
    public class ListContener<T> where T:IBaalItemsList {

        #region Variables
        /// <summary>
        /// Media classification universe list
        /// </summary>
        protected static Dictionary<int,T> _list=null;
        /// <summary>
        /// Baal Xml Node name
        /// </summary>
        protected static string _baalXmlNodeName="";
        #endregion

        #region Constructor
        /// <summary>
        /// Initialization of the lists
        /// </summary>
        static ListContener() {
            try {
                _list=new Dictionary<int,T>();
            }
            catch(System.Exception ex) {
                throw (new DomainException.ListContenerException("Impossible to initialize the classification list contener",ex));
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Baal Xml Node Name
        /// </summary>
        public static string BaalXmlNodeName {
            get{return(_baalXmlNodeName);}
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load Baal Lists from XML file
        /// </summary>
        /// <param name="source">DataSource</param>
        protected static void LoadBaalLists(IDataSource source) {
            BaalListXL<T>.Load(source);
        }


        /// <summary>
        /// Add A media list
        /// </summary>
        /// <param name="idMediaItemsList">List id</param>
        /// <param name="mediaItemsList">List value</param>
        public static void Add(int idList,T itemsList) {
            try {
                _list.Add(idList,itemsList);
            }
            catch(System.Exception err) {
                throw (new DomainException.ListContenerException("Imposssible to add the list "+idList,err));
            }
        }

        /// <summary>
        /// Contains a specific list
        /// </summary>
        /// <param name="idMediaItemsList">List Id</param>
        /// <returns>true if the list contains the specific list</returns>
        public static bool Contains(int idList) {
            return (_list.ContainsKey(idList));
        }


        /// <summary>
        /// Get a media list
        /// </summary>
        /// <param name="idMediaItemsList">List ID</param>
        /// <returns>List object which contains the medialist</returns>
        public static T GetItemsList(int idList) {
            try {
                return (_list[idList]);
            }
            catch(System.Exception) {
                throw (new DomainException.ListContenerException("the list doesn't exists for Id: "+idList));
            }
        }

        #endregion
    }
}
