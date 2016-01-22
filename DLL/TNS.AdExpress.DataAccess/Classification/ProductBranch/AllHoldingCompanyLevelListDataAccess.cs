#region Informations
// Auteur: G. Facon
// Date de création: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using System.Data;
using TNS.AdExpress.DataAccess.Classification;
using TNS.AdExpress.Classification.Exceptions;
using TNS.FrameWork.DB.Common;


namespace TNS.AdExpress.DataAccess.Classification.ProductBranch {
	/// <summary>
	/// Chargement partiel d'une liste de HoldingCompanies
	/// Attention cette classe ne charge pas tous les éléments.
	/// A chaque demande d'un libellé, elle fait une requête à la base de données pour obtenir le texte
	/// </summary>
	public class AllHoldingCompanyLevelListDataAccess{

		#region Variables
		/// <summary>
		/// Connexion à la base de données
		/// </summary>
		private IDataSource source;

		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Connexion à la base de données</param>
		public AllHoldingCompanyLevelListDataAccess(IDataSource source){
			this.source=source;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Retourne la valeur dont l'dentifiant est ID
		/// </summary>
		public string this [Int64 id]{
			get{
				string text;
                DataSet ds;

                #region Request
                string table=TNS.AdExpress.Constantes.Classification.DB.Table.name.holding_company.ToString();
				// Construction de la requête
				string sql="select id_"+table+", "+table+" from adexpressfr01."+table+" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" and id"+table+"="+id.ToString();
                #endregion

                #region Resquest execution
                try {
                    ds=source.Fill(sql);
                    text=ds.Tables[0].Rows[0][1].ToString();
                }
                catch(System.Exception e) {
                    throw (new ClassificationDALException("Impossible de sélectionner les éléments : "+e.Message));
                }
                #endregion

                return (text);
			}
		}
		#endregion
	}
}


