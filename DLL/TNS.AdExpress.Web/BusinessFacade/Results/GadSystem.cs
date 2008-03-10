#region Informations
// Auteur: B. Masson
// Date de cr�ation: 27/09/2004
// Date de modification: 27/09/2004
//	G. Facon	11/08/2005	New Exception Management
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpressDataAccess=TNS.AdExpress.Web.DataAccess;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using DBCst=TNS.AdExpress.Constantes.Customer.DB;

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Acc�s aux donn�es du GAD
	/// </summary>
	public class GadSystem{

		#region Variables
		/// <summary>
		/// Nom de la soci�t�
		/// </summary>
		private string _company;
		/// <summary>
		/// Adresse de la soci�t�
		/// </summary>
		private string _street;
		/// <summary>
		/// Adresse 2 de la soci�t�
		/// </summary>
		private string _street2;
		/// <summary>
		/// Code postal de la soci�t�
		/// </summary>
		private string _codePostal;
		/// <summary>
		/// Ville de la soci�t�
		/// </summary>
		private string _town;
		/// <summary>
		/// T�l�phone de la soci�t�
		/// </summary>
		private string _phone;
		/// <summary>
		/// Fax de la soci�t�
		/// </summary>
		private string _fax;
		/// <summary>
		/// Email de la soci�t�
		/// </summary>
		private string _email;
		//G Ragneau - int�gration Doc Marketing - 22/01
		/// <summary>
		/// Identifiant de l'annonceur pour le line vers le site du Doc Marketing
		/// </summary>
		private string _docMarketingId = string.Empty;
		/// <summary>
		/// Cl� � utiliser pour d�bloquer l'acc�s vers le site du Doc Marketing
		/// </summary>
		private string _docMarketingKey = string.Empty;
		//fin modif
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idAddress">Identifiant de l'adresse</param>
		public GadSystem(WebSession webSession, string idAddress){
			if (idAddress!=null && idAddress.Length>0){
				DataTable result=null;
				try{
					//charge les donn�es du DataSet dans un DataTable
					result=AdExpressDataAccess.Results.GadDataAccess.GetData(webSession,idAddress).Tables[0];
				}
				catch(System.Exception err){
					throw(new WebExceptions.GadSystemException("Impossible de charger les donn�es du GAD � partir de la base de donn�es, erreur sur l'identifiant de l'adresse : "+idAddress,err));
				}
			
				//rempli les champs
				foreach(DataRow myRow in result.Rows){
					_company			= myRow["company"].ToString();
					_street				= myRow["street"].ToString();
					_street2			= myRow["street2"].ToString();
					_codePostal			= myRow["code_postal"].ToString();
					_town				= myRow["town"].ToString();
					_phone				= myRow["telephone"].ToString();
					_fax				= myRow["fax"].ToString();
					_email				= myRow["email"].ToString();
					if (webSession.CustomerLogin.GetFlag((long)DBCst.Flag.id.gad.GetHashCode()).Length>0){
						_docMarketingId		= myRow["id_gad"].ToString();
						_docMarketingKey	= myRow["docKey"].ToString();
					}
				}			
			}
			else{
				//Exception
				throw(new WebExceptions.GadSystemException("Impossible de charger les donn�es du GAD, erreur sur l'identifiant de l'adresse : "+idAddress));
			}
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le nom de la soci�t�
		/// </summary>
		public string Company{
			get{return _company;}
		}

		/// <summary>
		/// Obtient l'adresse de la soci�t�
		/// </summary>
		public string Street{
			get{return _street;}
		}

		/// <summary>
		/// Obtient l'adresse 2 de la soci�t�
		/// </summary>
		public string Street2{
			get{return _street2;}
		}

		/// <summary>
		/// Obtient le nom de la soci�t�
		/// </summary>
		public string CodePostal{
			get{return _codePostal;}
		}

		/// <summary>
		/// Obtient la ville de la soci�t�
		/// </summary>
		public string Town{
			get{return _town;}
		}

		/// <summary>
		/// Obtient le t�l�phone de la soci�t�
		/// </summary>
		public string Phone{
			get{return _phone;}
		}

		/// <summary>
		/// Obtient le fax de la soci�t�
		/// </summary>
		public string Fax{
			get{return _fax;}
		}

		/// <summary>
		/// Obtient l'email de la soci�t�
		/// </summary>
		public string Email{
			get{return _email;}
		}
		//G Ragneau - int�gration Doc Marketing - 22/01
		/// <summary>
		/// Identifiant de l'annonceur pour le lien vers le site du Doc Marketing
		/// </summary>
		public string DocMarketingId {
			get{return _docMarketingId;}
		}
		/// <summary>
		/// Cl� � utiliser pour d�bloquer l'acc�s vers le site du Doc Marketing
		/// </summary>
		public string DocMarketingKey {
			get{return _docMarketingKey;}
		}
		//fin modif
		#endregion
	}
}
