#region Informations
// Auteur: G. Facon
// Date de cr�ation: 30/03/2004
// Date de modification: 30/03/2004
#endregion

using System;
using System.Data;
using TNS.AdExpress.Classification.DataAccess;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Classification.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Classification.DataAccess.ProductBranch{
	/// <summary>
	/// Chargement d'une liste d'Advertiser
	/// Attention cette classe ne charge pas tous les �l�ments.
	/// A chaque demande d'un libell�, elle fait une requ�te � la base de donn�es pour obtenir le texte
	/// </summary>
	public class AllAdvertiserLevelListDataAccess{

		#region Variables
		/// <summary>
		/// Connexion � la base de donn�es
		/// </summary>
		private IDataSource source;

		#endregion
		
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Connexion � la base de donn�es</param>
		public AllAdvertiserLevelListDataAccess(IDataSource source){
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
                string table=TNS.AdExpress.Constantes.Classification.DB.Table.name.advertiser.ToString();
				string sql="select id_"+table+", "+table+" from adexpressfr01."+table+" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" and id"+table+"="+id.ToString();
                #endregion

                #region Resquest execution
                try {
                    ds=source.Fill(sql);
                    text=ds.Tables[0].Rows[0][1].ToString();
				}
				catch(System.Exception e){
					throw(new ClassificationDataDBException("Impossible de s�lectionner les �l�ments : "+e.Message));
                }
                #endregion

                return (text);
			}
		}
		#endregion
	}
}
