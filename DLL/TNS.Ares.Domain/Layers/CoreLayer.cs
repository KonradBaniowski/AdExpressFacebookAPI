#region Information
//  Author : Y. Rkaina && D. Mussuma
//  Creation  date: 15/07/2009
//  Modifications:
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Ares.Domain.Layers {
	
	/// <summary>
	/// Core Layer
	/// </summary>
	public class CoreLayer : LayerBase {

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Layer Name</param>
		/// <param name="assemblyName">Assembly name</param>
		/// <param name="className">Class name</param>
		public CoreLayer(string name, string assemblyName, string className)
			: base(name, assemblyName, className) {
		}
		#endregion
	}
}
