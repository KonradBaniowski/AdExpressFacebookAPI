#region Informations
// Auteur: G.Facon 
// Date de création: 07/02/2005 
// Date de modification
#endregion

using System;
using System.Data;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.DB.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Functions;

namespace TNS.AdExpress.Web.DataAccess.Results{


	///<summary>
	/// Description résumée de SearchLevelDataAcess.
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class SearchLevelDataAccess {

		/// <summary>
		/// Compte le nombre d'éléments contenant le terme contenu dans wordToSearch dans le niveau de nomenclature table
		/// </summary>
		/// <param name="table">Table de la nomenclature</param>
		/// <param name="wordToSearch">Terme recherché</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Le nombre d'élément de la nomenclature trouvé</returns>
		public static long CountItems(TNS.AdExpress.Constantes.Classification.DB.Table.name table,string wordToSearch,WebSession webSession){

			#region Construction de la requête
			string sql="select count(distinct wp.id_"+table.ToString()+") as items ";
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".product_dimension_"+webSession.DataLanguage.ToString()+" wp";
			sql+=" where wp."+table.ToString()+" like '%"+wordToSearch.ToUpper().Replace("'"," ")+"%'";

			#region Application des droits produits
			sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
			#endregion

			#endregion

			#region Excution de la requête
			DataSet ds=null;
			try{
				ds=webSession.Source.Fill(sql);
			}
			catch(System.Exception){
			}
			#endregion

			if(ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
				return(long.Parse((ds.Tables[0].Rows[0][0]).ToString()));
			}
			return(0);


		}
	}
}

