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

namespace TNS.AdExpressI.Classification.DAL.SqlServerRussia {
	public class ClassificationLevelListDALFactory : TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory {



		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="language"></param>
		public ClassificationLevelListDALFactory(IDataSource source, int language)
			: base(source, language) {

		}
		#endregion

		

	}
}
