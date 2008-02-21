using System;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Exceptions;
using TNS.AdExpress.Anubis.DataAccess.Configuration;

namespace TNS.AdExpress.Anubis.BusinessFacade.Configuration{
	/// <summary>
	/// Description résumée de PluginsSystem.
	/// </summary>
	public class PluginsSystem{

		/// <summary>
		/// Chargement de la liste des plug-ins à lancer
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <returns>Liste des plug-ins</returns>
		public static Hashtable GetPluginsToLoad(IDataSource dataSource){
			try{
				return(PluginsDataAccess.GetPlugins(dataSource));
			}
			catch(System.Exception err){
				throw(err);
			}
			
		}
	}
}
