using System;
using System.Collections.Generic;

namespace KMI.AdExpress.Aphrodite.Domain.Layers {
    /// <summary>
    /// Base class for layers
    /// </summary>
    public abstract class LayerBase{

        #region Variables
        /// <summary>
        /// Layer Name
        /// </summary>
        protected string _name;
        /// <summary>
        /// assembly Name
        /// </summary>
        protected string _assemblyName;
        /// <summary>
        /// Class to be used
        /// </summary>
        protected string _class;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Layer Name</param>
        /// <param name="assemblyName">Assembly name</param>
        /// <param name="className">Class name</param>
        protected LayerBase(string name,string assemblyName,string className) {
            if(name==null ||name.Length<1) throw (new ArgumentException("Layer name is invalide"));
            if(assemblyName==null || assemblyName.Length<1) throw (new ArgumentException("Layer assembly name is invalide"));
            if(className==null || className.Length<1) throw (new ArgumentException("Layer class name is invalide"));
            _name=name;
            _assemblyName=assemblyName;
            _class=className;
        }
        public LayerBase() { }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Layer Name
        /// </summary>
        public string Name {
            get { return (_name); }
            internal set { _name = value; }
        }

        /// <summary>
        /// Get assembly Name
        /// </summary>
        public string AssemblyName {
            get { return (_assemblyName); }
            internal set { _assemblyName = value; }
        }

        /// <summary>
        /// Get Class name
        /// </summary>
        public string Class {
            get { return (_class); }
            internal set { _class = value; }
        }
        #endregion

    }
}
