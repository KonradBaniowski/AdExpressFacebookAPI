#region Information
//  Author : G. Facon
//  Creation  date: 20/03/2008
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Ares.Domain.Layers {
    /// <summary>
    /// Base class for layers
    /// </summary>
    public abstract class LayerBase {


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
            if(name.Length<1) throw (new ArgumentException("Layer name is invalide"));
            if(assemblyName.Length<1) throw (new ArgumentException("Layer assembly name is invalide"));
            if(className.Length<1) throw (new ArgumentException("Layer class name is invalide"));
            _name=name;
            _assemblyName=assemblyName;
            _class=className;
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Layer Name
        /// </summary>
        public string Name {
            get { return (_name); }
        }

        /// <summary>
        /// Get assembly Name
        /// </summary>
        public string AssemblyName {
            get { return (_assemblyName); }
        }

        /// <summary>
        /// Get Class name
        /// </summary>
        public string Class {
            get { return (_class); }
        }
        #endregion
    }
}
