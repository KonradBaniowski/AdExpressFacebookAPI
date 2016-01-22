#region Informations
// Auteur: D. Mussuma
// Date de création: 17/08/2009
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpressI.Classification.DAL.Exceptions;

namespace TNS.AdExpressI.Classification.DAL.Default {
	/// <summary>
	/// Provides all methods to create items list of classification level.
	/// </summary>
	/// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.ClassificationDALException">
	/// Unknow Detail level information Identifier 
	/// </exception>
	public class ClassificationLevelListDALFactory : TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory {		

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="source">Data source</param>
		/// <param name="language">languague Identifier</param>
		public ClassificationLevelListDALFactory( IDataSource source, int language)
		:base(source,language) {
			
		}
		#endregion
		

	}
}
