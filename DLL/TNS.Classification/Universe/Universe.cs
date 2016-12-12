using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Classification.Universe {
	[System.Serializable]
	public class Universe {

		#region Variables

		/// <summary>
		///  universe ID
		/// </summary>
		private Int64 _id = 0;
		/// <summary>
		/// universe label
		/// </summary>
		private string _label = null;
		/// <summary>
		/// Dictionnary of groups element list 
		/// </summary>
		private Dictionary<Int64, NomenclatureElementsGroup> _elementsGroupDictionary = new Dictionary<Int64, NomenclatureElementsGroup>();
		/// <summary>
		/// Universe dimension
		/// </summary>
		TNS.Classification.Universe.Dimension _dimension;
        /// <summary>
        /// Define Security check
        /// </summary>
        Security _security=Security.none;

		#endregion

		#region Properties

		/// <summary>
		///  universe ID
		/// </summary>
		public Int64 ID {
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// universe label
		/// </summary>
		public string Label {
			get { return _label; }
			set { _label = value; }
		}

		/// <summary>
		/// Get\Set universe dimension
		/// </summary>
		public TNS.Classification.Universe.Dimension UniverseDimension {
			get { return _dimension; }
			set { _dimension = value; }
		}
		/// <summary>
		/// Get\Set Dictionnary of groups element list 
		/// </summary>
		public Dictionary<Int64, NomenclatureElementsGroup> ElementsGroupDictionary {
			get { return _elementsGroupDictionary; }
			set { _elementsGroupDictionary = value; }
		}
		/// <summary>
		/// Get\Set Security check
		/// </summary>
		public Security Security {
			get { return _security; }
			set { _security = value; }
		}
		#endregion

		#region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimension">Universe dimension (product or media)</param>
        /// <param name="security">Set Security check</param>
        public Universe(TNS.Classification.Universe.Dimension dimension,Security security) {
            _label="";
            _dimension=dimension;
            _security=security;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="label">Label</param>
        /// <param name="dimension">Universe dimension (product or media)</param>
        /// <param name="security">Set Security check</param>
        public Universe(string label,TNS.Classification.Universe.Dimension dimension,Security security)
            : this(dimension,security) {
            _label=label;
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">Identifier</param>
		/// <param name="label">Label</param>
        /// <param name="dimension">Universe dimension (product or media)</param>
        public Universe(Int64 id,string label,TNS.Classification.Universe.Dimension dimension,Security security)
            : this(label,dimension,security) {
			_id = id;
		}
		#endregion

		#region Public Methods

        #region Add
        /// <summary>
		/// Add nomenclature elements group into universe
		/// </summary>
		/// <param name="groupKey">group key</param>
		/// <param name="elementsGroup">elements group</param>
		public void AddGroup(Int64 groupKey,NomenclatureElementsGroup elementsGroup) {
            try{
            if(_security==Security.full) {
                List<Int64> levelIdsList=elementsGroup.GetLevelIdsList();
                foreach(Int64 currentLevel in levelIdsList) {
                    if(this.ContainsLevel(currentLevel,elementsGroup.AccessType)) throw (new SecurityException("Secutity status is full. A level id has already been set"));
                }
            }
			_elementsGroupDictionary.Add(groupKey, elementsGroup);
            }
            catch(System.Exception err) {
                throw(new System.Exception("Impossible to add a group to the universe",err));
            }
        }

        public void AddLevel(Int64 groupKey,Int64 levelId,List<Int64> itemsList){
            try {
                if(!_elementsGroupDictionary.ContainsKey(groupKey)) throw (new ArgumentException("The group \""+groupKey.ToString()+"\" does not exist"));
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to add the level to the universe",err));
            }
        }

        public void AddLevel(Int64 groupKey,Int64 levelId,string itemsList) {
            try {
                if(!_elementsGroupDictionary.ContainsKey(groupKey)) throw (new ArgumentException("The group \""+groupKey.ToString()+"\" does not exist"));
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to add the level to the universe",err));
            }
        }

        #region AddItems
        /// <summary>
        /// Add items of a level to a group
        /// </summary>
        /// <param name="groupKey">Group Id</param>
        /// <param name="levelId">Level Id</param>
        /// <param name="itemsList">items list</param>
        public void AddItems(Int64 groupKey,Int64 levelId,List<Int64> itemsList) {
            try {
                if(!_elementsGroupDictionary.ContainsKey(groupKey)) throw (new ArgumentException("The group \""+groupKey.ToString()+"\" does not exist"));
                if(_security==Security.full) {
                    AccessType accessType=_elementsGroupDictionary[groupKey].AccessType;
                    // Can we add this level or some items of this level to the group
                    // Yes if any groups have this level or the only one is the current group
                    if(FindGroup(levelId,accessType)!=null&&FindGroup(levelId,accessType).ID!=groupKey) {
                        throw (new SecurityException("Secutity status is full. Impossible to add items of this level, because another group has already it"));
                    }
                }
                _elementsGroupDictionary[groupKey].AddItems(levelId,itemsList);
            }
            catch(System.Exception err) {
                throw(new System.Exception("Impossible to add items to the universe",err));
            }
        }

        /// <summary>
        /// Add items of a level to a group
        /// </summary>
        /// <param name="groupKey">Group Id</param>
        /// <param name="levelId">Level Id</param>
        /// <param name="itemsList">items list</param>
        public void AddItems(Int64 groupKey,Int64 levelId,string itemsList) {
            try {
                if(!_elementsGroupDictionary.ContainsKey(groupKey)) throw (new ArgumentException("The group \""+groupKey.ToString()+"\" does not exist"));
                if(_security==Security.full) {
                    AccessType accessType=_elementsGroupDictionary[groupKey].AccessType;
                    // Can we add this level or some items of this level to the group
                    // Yes if any groups have this level or the only one is the current group
                    if(FindGroup(levelId,accessType)!=null&&FindGroup(levelId,accessType).ID!=groupKey) {
                        throw (new SecurityException("Secutity status is full. Impossible to add items of this level, because another group has already it"));
                    }
                }

                _elementsGroupDictionary[groupKey].AddItems(levelId,itemsList);
            }
            catch(System.Exception err) {
                throw(new System.Exception("Impossible to add items to the universe",err));
            }
        }

        
        /// <summary>
        /// Add items to the universe
        /// </summary>
        /// <remarks>Can be used only if Security not none</remarks>
        /// <param name="levelId">Level Id</param>
        /// <param name="accessType">Access type</param>
        /// <param name="itemsList">Items list</param>
        public void AddItems(Int64 levelId,AccessType accessType, List<Int64> itemsList) {
            try {
                if(_security==Security.none) throw (new SecurityException("Secutity status is non. Impossible to add items without groupKey"));
                FindGroup(levelId,accessType).AddItems(levelId,itemsList);
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to add items to the universe",err));
            }

        }

        /// <summary>
        /// Add items to the universe
        /// </summary>
        /// <remarks>Can be used only if Security not none</remarks>
        /// <param name="levelId">Level Id</param>
        /// <param name="accessType">Access type</param>
        /// <param name="itemsList">Items list</param>
        public void AddItems(Int64 levelId,AccessType accessType,string itemsList) {
            try {
                if(_security==Security.none) throw (new SecurityException("Secutity status is non. Impossible to add items without groupKey"));
                FindGroup(levelId,accessType).AddItems(levelId,itemsList);
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to add items to the universe",err));
            }
        }
        #endregion

        #endregion

        /// <summary>
		/// Remove nomenclature elements group into universe
		/// </summary>
		/// <param name="groupKey"></param>
		public void Remove(Int64 groupKey) {
			_elementsGroupDictionary.Remove(groupKey);
		}

		/// <summary>
		/// Determine if universe contains a group
		/// </summary>
		/// <param name="groupKey">group key</param>
		/// <returns>True if universe contains a group</returns>
		public bool Contains(Int64 groupKey) {
            try {
                return _elementsGroupDictionary.ContainsKey(groupKey);
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to check if the universe contains this group",err));
            }
		}

        /// <summary>
        /// Determine if universe contains a levelId
        /// </summary>
        /// <param name="levelId">Level Id</param>
        /// <param name="accesType">Access type to check</param>
        /// <returns>True if universe contains a group</returns>
        public bool ContainsLevel(Int64 levelId,AccessType accesType) {
            try {
                List<NomenclatureElementsGroup> list=GetNomenclatureItemsGroups(accesType);
                if(list==null) return (false);
                foreach(NomenclatureElementsGroup currentGroup in list) {
                    if(currentGroup.Contains(levelId)) return (true);
                }
                return (false);
            }
            catch(System.Exception err) {
                throw(new System.Exception("Impossible to check if a universe contains a level",err));
            }
        } 

		/// <summary>
		/// Get number of groups
		/// </summary>
		/// <returns></returns>
		public int Count() {
            try {
                return _elementsGroupDictionary.Count;
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to count the number of groups",err));
            }
		}

		/// <summary>
		/// Get a  specific group
		/// </summary>
		/// <param name="groupKey">group key</param>
		/// <returns>spcecific group</returns>
        public NomenclatureElementsGroup GetGroup(Int64 groupKey) {
            try {
                return (_elementsGroupDictionary.ContainsKey(groupKey))?_elementsGroupDictionary[groupKey]:null;
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to get this groups",err));
            }
        }

        /// <summary>
		/// Get a  specific level
		/// </summary>
        /// <remarks>Securitu must be set at full</remarks>
		/// <param name="groupKey">Level Id</param>
        /// <param name="accessType">Access type</param>
		/// <returns>level List</returns>
        public string GetLevel(Int64 levelId,AccessType accessType) {
            if(_security!=Security.full) throw (new SecurityException("Security is not set at full"));
            NomenclatureElementsGroup neg=FindGroup(levelId,accessType);
            if(neg==null) return (null);
            string list=neg.GetAsString(levelId);
            return (list);

        }

        /// <summary>
        /// Get a  specific level
        /// </summary>
        /// <remarks>Securitu must be set at full</remarks>
        /// <param name="groupKey">Level Id</param>
        /// <param name="accessType">Access type</param>
        /// <returns>level List</returns>
        public List<long> GetLevelValue(Int64 levelId, AccessType accessType)
        {
            if (_security != Security.full) throw (new SecurityException("Security is not set at full"));
            NomenclatureElementsGroup neg = FindGroup(levelId, accessType);
            if (neg == null) return (null);
            var list = neg.Get(levelId);
            return (list);

        }

		/// <summary>
		/// Get universe level list Id
		/// </summary>
		/// <remarks>Each Id is unique in the list</remarks>		
		/// <returns>level List Id</returns>
		public List<Int64> GetLevelListId() {
			List<Int64> currentListId = null, list=new List<Int64>();
			try {
				if (_elementsGroupDictionary.Count > 0) {
					foreach (KeyValuePair<Int64, NomenclatureElementsGroup> group in _elementsGroupDictionary) {
						if (group.Value.Count() > 0) {
							currentListId = group.Value.GetLevelIdsList();
							for (int i = 0; i < currentListId.Count; i++) {
								if (!list.Contains(currentListId[i])) list.Add(currentListId[i]);
							}
							
						}
					}				
				}
				return list;
			}
			catch (System.Exception err) {
				throw (new System.Exception("Impossible to get universe level list Id", err));
			}
		}

        /// <summary>
        /// Get all includes elements groups
        /// </summary>
        /// <returns>List of NomenclatureElementsGroup</returns>
		public List<NomenclatureElementsGroup> GetIncludes() {
            try {
                return GetNomenclatureItemsGroups(TNS.Classification.Universe.AccessType.includes);
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to get the included groups",err));
            }
		}	

        /// <summary>
        /// Get all exludes elements groups
        /// </summary>
        /// <returns>List of NomenclatureElementsGroup</returns>
		public List<NomenclatureElementsGroup> GetExludes() {
            try {
                return GetNomenclatureItemsGroups(TNS.Classification.Universe.AccessType.excludes);
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to get the excluded groups",err));
            }
		}

		
		#endregion

		#region Private Methods
		/// <summary>
		///	Get elements groups according to access type
		/// </summary>
		/// <param name="accessType">access type</param>
		/// <returns>list of nomenclature elements group</returns>
		private List<NomenclatureElementsGroup> GetNomenclatureItemsGroups(TNS.Classification.Universe.AccessType accessType) {
            try {
                if(_elementsGroupDictionary.Count>0) {
                    List<NomenclatureElementsGroup> list=new List<NomenclatureElementsGroup>();
                    foreach(KeyValuePair<Int64,NomenclatureElementsGroup> kvp in _elementsGroupDictionary) {
                        if(kvp.Value.AccessType==accessType)
                            list.Add(kvp.Value);
                    }
                    return list;
                }
                return null;
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to get items groups according to access type",err));
            }

		}

        /// <summary>
        /// Find the group which contains the level Id
        /// </summary>
        /// <param name="levelId">Level Id</param>
        /// <returns>Nomenclature Items Group, null if no groups have this level id</returns>
        private NomenclatureElementsGroup FindGroup(Int64 levelId,AccessType accessType) {
            try {
                if(_security==Security.none) throw (new SecurityException("Secutity status is non. Impossible use the method FindGroup"));
                List<NomenclatureElementsGroup> list=GetNomenclatureItemsGroups(accessType);
                if(list==null) return (null);
                foreach(NomenclatureElementsGroup currentGroup in list) {
                    if(currentGroup.Contains(levelId)) return (currentGroup);
                }
                return (null);
            }
            catch(System.Exception err) {
                throw (new System.Exception("Impossible to find the group which contains the level Id",err));
            }

        }


        
		#endregion

	}
}
