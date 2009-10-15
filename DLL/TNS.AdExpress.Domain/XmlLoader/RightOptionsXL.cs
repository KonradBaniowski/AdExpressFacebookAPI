#region Informations
// Author: D. Mussuma
// Creation Date: 21/02/2008
// Modifications: 
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Core;
using TNS.AdExpress.Domain.Exceptions;


namespace TNS.AdExpress.Domain.XmlLoader {
	/// <summary>
	/// Load all the configuration parameter for right management
	/// </summary>
	public class RightOptionsXL {
		/// <summary>
		/// Load result options
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <summary>
		public static void Load(IDataSource dataSource) {

			#region Variables
			XmlTextReader reader = null;
			#endregion

			try {
				reader = (XmlTextReader)dataSource.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {

						switch (reader.LocalName) {
							case "rightOption":
								if (reader.GetAttribute("useDefaultConnection") != null && reader.GetAttribute("useDefaultConnection").Length > 0) {
									WebApplicationParameters.UseRightDefaultConnection = Convert.ToBoolean(reader.GetAttribute("useDefaultConnection"));
								}
								break;
						}
					}
				}
			}

			#region Error Management
			catch (System.Exception err) {

				#region Close the file
				if (dataSource.GetSource() != null) dataSource.Close();
				#endregion

				throw (new XmlException(" Error while loading RightOptions.xml : ", err));
			}
			#endregion

			dataSource.Close();

		}
	}
}
