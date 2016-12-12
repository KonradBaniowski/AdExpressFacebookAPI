using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Classification.Universe {

	[System.Serializable]
	public class NomenclatureElementsGroup {

		#region variables
		/// <summary>
		/// Group identifier
		/// </summary>
		private Int64 _id = 0;

		/// <summary>
		/// Label
		/// </summary>
		private string _label = string.Empty;

		/// <summary>
		/// Flag element list access type
		/// </summary>
		TNS.Classification.Universe.AccessType _accessType;

		/// <summary>
		/// Dictionary of nomenclature elements list.
		/// The key is the elements level in nomenclature and value the 
		/// list of nomeclature elements (of same level). 
		/// </summary>
		System.Collections.Generic.Dictionary<Int64, List<Int64>> _elementsGroup = new Dictionary<Int64, List<Int64>>();

		#endregion

		#region Properties

		/// <summary>
		/// Flag element list access type
		/// </summary>
		public TNS.Classification.Universe.AccessType AccessType {
			get { return _accessType; }
			set { _accessType = value; }
		}

		/// <summary>
		/// Label
		/// </summary>
		public string Label {
			get { return _label; }
			set { _label = value; }
		}

		/// <summary>
		/// Group identifier
		/// </summary>
		public Int64 ID {
			get { return _id; }
			set { _id = value; }
		}

		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="accessType">Flag element list access type</param>
		/// <param name="id">Identifier</param>
		/// <param name="label">Label</param>
		public NomenclatureElementsGroup(string label, Int64 id ,TNS.Classification.Universe.AccessType accessType) {			
			
			_accessType = accessType;

			_id = id;

			_label = label;


		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="accessType">Flag element list access type</param>
		/// <param name="id">Identifier</param>
		public NomenclatureElementsGroup(Int64 id, TNS.Classification.Universe.AccessType accessType) {

			_accessType = accessType;

			_id = id;

		}
		#endregion

		#region Public Méthods

        #region Add
        /// <summary>
        /// Add nomenclature element list into dictionary
        /// </summary>
        /// <param name="level">corresponding </param>
        /// <param name="item">Item to add</param>
        public void AddItem(Int64 levelId,Int64 item) {
            if(!_elementsGroup.ContainsKey(levelId)) {
                _elementsGroup[levelId]=new List<Int64>();
            }
            _elementsGroup[levelId].Add(item);
        }

        /// <summary>
        /// Add nomenclature element list into dictionary
        /// </summary>
        /// <param name="level">corresponding </param>
        /// <param name="items">Items list to add</param>
        public void AddItems(Int64 levelId,List<Int64> items) {
            if(!_elementsGroup.ContainsKey(levelId)) {
                _elementsGroup[levelId]=new List<Int64>();
            }
            _elementsGroup[levelId].AddRange(items);
        }

        /// <summary>
        /// Add nomenclature element list into dictionary
        /// </summary>
        /// <remarks>Séparator is ,</remarks>
        /// <param name="level">corresponding </param>
        /// <param name="items">Items list to add</param>
        public void AddItems(Int64 levelId,string items) {
            if(!_elementsGroup.ContainsKey(levelId)) {
                _elementsGroup[levelId]=new List<Int64>();
            }
            _elementsGroup[levelId].AddRange(ConvertToListInt64(items));
        }

        /// <summary>
        /// Add nomenclature element list into dictionary
        /// </summary>
        /// <param name="level">corresponding </param>
        /// <param name="items">Items list to add</param>
        public void AddLevel(Int64 levelId,List<Int64> items) {
            _elementsGroup.Add(levelId,items);
        }

        /// <summary>
        /// Add nomenclature element list into dictionary
        /// </summary>
        /// <remarks>Séparator is ,</remarks>
        /// <param name="level">corresponding </param>
        /// <param name="items">Items list to add</param>
        public void AddLevel(Int64 levelId,string items) {
            _elementsGroup.Add(levelId,ConvertToListInt64(items));
        }
        #endregion

        /// <summary>
		/// Remove nomenclature element list into dictionary
		/// </summary>
		/// <param name="level">nomenclature level  </param>
		public void Remove(Int64 levelId) {
			_elementsGroup.Remove(levelId);
		}

		/// <summary>
		/// Determine if dictionary contains element list of specific level. 
		/// </summary>
		/// <param name="level">nomenclature level  </param>
		/// <returns></returns>
		public bool Contains(Int64 levelId) {
			return _elementsGroup.ContainsKey(levelId);
		}

		/// <summary>
		/// Get the number of list in dictionary
		/// </summary>
		/// <returns></returns>
		public int Count() {
			return _elementsGroup.Count;			
			
		}

		/// <summary>
		/// Get a specific level item list
		/// </summary>
		/// <param name="level">nomenclature level</param>
		/// <returns></returns>
		public List<Int64> Get(Int64 levelId) {
			return (_elementsGroup.ContainsKey(levelId)) ? _elementsGroup[levelId] : null;
		}

        /// <summary>
        /// Get a specific level item list
        /// </summary>
        /// <param name="level">nomenclature level</param>
        /// <returns>id list of the level</returns>
        public string GetAsString(Int64 levelId) {
            if(!_elementsGroup.ContainsKey(levelId)) return (null);
            string list=ConvertToString(_elementsGroup[levelId]);
            if(list.Length==0) return (null);
            return (list);
        }

        /// <summary>
        /// Get Level Ids
        /// </summary>
        /// <returns>Level Ids list</returns>
        public List<Int64> GetLevelIdsList() {
            List<Int64> list=new List<Int64>(_elementsGroup.Count);
            foreach(Int64 currentLevelId in _elementsGroup.Keys) {
                list.Add(currentLevelId);
            }
            return (list);
        }
		#endregion

        #region Private Methods
        /// <summary>
        /// Convert a string to List<Int64>
        /// </summary>
        /// <param name="itemsList">Items List</param>
        /// <returns>List converted</returns>
        private List<Int64> ConvertToListInt64(string itemsList) {
            try {
                string[] list=itemsList.Split(',');
                List<Int64> myItems=new List<long>(list.Length);
                foreach(string currentId in list) {
                    myItems.Add(Int64.Parse(currentId));
                }
                return (myItems);
            }
            catch(System.Exception err) { 
                throw (new ApplicationException("Impossible to convert string to List<Int64>",err)); 
            }
        
        }

        /// <summary>
        /// Convert a List<Int64> to a string
        /// </summary>
        /// <param name="list">List to convert</param>
        /// <returns>Converted list</returns>
        private string ConvertToString(List<Int64> list) {
            try {
                string listString="";
                foreach(Int64 currentId in list) {
                    listString+=currentId.ToString()+",";
                }
                if(listString.Length>0) listString=listString.Substring(0,listString.Length-1);
                return (listString);
            }
            catch(System.Exception err) {
                throw (new ApplicationException("Impossible to convert List<Int64> to string",err));
            }
        }


        #endregion
    }
}
