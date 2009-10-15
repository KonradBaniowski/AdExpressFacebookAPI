#region Information
/*
 * Author : G Ragneau
 * Created on 13/08/2008
 * Modifications :
 *      Ahtour - Date - Description
 * 
 * 
 * */
#endregion

#region Using
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;

using TNS.AdExpressI.PresentAbsentDAL.Exceptions;

using TNS.FrameWork.DB;

using CstDBDesc = TNS.AdExpress.Domain.DataBaseDescription;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctWeb = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

#endregion


namespace TNS.AdExpressI.PresentAbsent.DAL.Russia {
	public class PresentAbsentDAL : PresentAbsent.DAL.PresentAbsentDAL {
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="session">User Session</param>
		public PresentAbsentDAL(WebSession session)
			: base(session) {
		}
		#endregion

		
	}
}
