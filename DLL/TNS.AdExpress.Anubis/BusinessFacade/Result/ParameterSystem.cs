#region Informations
// Auteur: G. Facon
// Date de création: 02/06/2005
// Date de modification: 02/06/2005
//		17/08/2005 : G. RAGNEAU - GetRequest
//		17/08/2005 : D. Mussuma - surchage méthode Save
//		19/03/2007 : Y. R'kaina - Add LoadObject

#endregion

using System;
using System.Data;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Anubis.Exceptions;
using TNS.AdExpress.Anubis.DataAccess.Result;
using TNS.AdExpress.Web.Core.Result;
using AnubisConstantes=TNS.AdExpress.Anubis.Constantes;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.BusinessFacade.Result{
	/// <summary>
	/// Gestion des paramètres de résultats
	/// </summary>
	public class ParameterSystem{

		#region WSession Load and Save

		/// <summary>
		/// Sauvegarde l'état de la session client du site Web pour générer un résultat PDF
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="resultType">Type du résultat</param>
		/// <returns>Identifiant de la sauvegarde</returns>
		public static Int64  Save(WebSession webSession,TNS.AdExpress.Anubis.Constantes.Result.type resultType)
		{
			return Save(webSession,resultType,webSession.ExportedPDFFileName);
		}
		/// <summary>
		/// Sauvegarde l'état de la session client du site Web pour générer un résultat PDF (ou texte,...)
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="resultType">Type du résultat</param>
		/// <param name="fileName">Nom du fichier</param>
		/// <returns>Identifiant de la sauvegarde</returns>
		public static Int64  Save(WebSession webSession,TNS.AdExpress.Anubis.Constantes.Result.type resultType,string fileName){
			if(webSession==null)throw(new ArgumentNullException("La session du client est null"));
			//if(resultType==TNS.AdExpress.Anubis.Constantes.Result.type.unknown)throw(new ArgumentException("Le type de résultat n'est pas valide"));
			return(ParameterDataAccess.Save(webSession,resultType,fileName));
		}

		/// <summary>
		/// Sauvegarde les paramètres nécessaires au résultats d'une fiche justificative PDF
		/// </summary>
		/// <param name="pDetail">Paramètres d'une fiche justificatives du client</param>
		/// <param name="resultType">Type du résultat</param>
		/// <param name="fileName">Nom du fichier</param>
		/// <returns>Identifiant de la sauvegarde</returns>
		public static Int64  Save(ProofDetail pDetail,TNS.AdExpress.Anubis.Constantes.Result.type resultType,string fileName){
			if(pDetail==null)throw(new ArgumentNullException("Les Paramètres da la fiche justificative est null"));
			if(resultType==TNS.AdExpress.Anubis.Constantes.Result.type.unknown)throw(new ArgumentException("Le type de résultat n'est pas valide"));
			return(ParameterDataAccess.Save(pDetail,resultType,fileName));
		}

		/// <summary>
		/// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des static_nav_sessions
		/// </summary>
		/// <param name="idStaticNavSession">Identifiant</param>
		/// <returns>Paramètres pour générer un résultat</returns>
		public static WebSession Load(Int64 idStaticNavSession){
			if(idStaticNavSession<0)throw(new ArgumentException("L'identifiant des paramètres pour générer le résultat n'est pas valide"));
			try{
				return((WebSession)ParameterDataAccess.Load(idStaticNavSession));
			}
			catch(System.Exception err){
				throw(new ParameterSystemException("Impossible de charger les paramètres pour générer le résultat",err));
			}
		}

		/// <summary>
		/// Méthode pour la récupération et la "deserialization" d'un objet ProofDetail à partir du champ BLOB de la table des static_nav_sessions
		/// </summary>
		/// <param name="idStaticNavSession">Identifiant</param>
		/// <returns>Paramètres pour générer un résultat</returns>
		public static ProofDetail LoadProofDetail(Int64 idStaticNavSession){
			if(idStaticNavSession<0)throw(new ArgumentException("L'identifiant des paramètres pour générer le résultat n'est pas valide"));
			try{
				return((ProofDetail)ParameterDataAccess.Load(idStaticNavSession));
			}
			catch(System.Exception err){
				throw(new ParameterSystemException("Impossible de charger les paramètres pour générer le résultat",err));
			}
		} 

		/// <summary>
		/// Méthode pour la récupération et la "deserialization" de n'importe quel objet à partir du champ BLOB de la table des static_nav_sessions
		/// </summary>
		/// <param name="idStaticNavSession">Identifiant</param>
		/// <returns>Un objet</returns>
		public static object LoadObject(Int64 idStaticNavSession){
			if(idStaticNavSession<0)throw(new ArgumentException("L'identifiant des paramètres pour générer le résultat n'est pas valide"));
			try{
				return(ParameterDataAccess.Load(idStaticNavSession));
			}
			catch(System.Exception err){
				throw(new ParameterSystemException("Impossible de charger les paramètres pour générer le résultat",err));
			}
		}
		#endregion

		#region ChangeStatus
		/// <summary>
		/// Changement 
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="idStaticNavSession">Demande à mettre à jour</param>
		/// <param name="pdfStatus">Nouveau Statut</param>
		public static void ChangeStatus(IDataSource dataSource,Int64 idStaticNavSession,AnubisConstantes.Result.status pdfStatus){
			if(idStaticNavSession<0)throw(new ArgumentException("L'identifiant des paramètres pour générer le résultat n'est pas valide"));
			try{
				ParameterDataAccess.ChangeStatus(dataSource,idStaticNavSession,pdfStatus);
			}
			catch(System.Exception err){
				throw(err);
			}
			
		}
		#endregion

		#region RegisterFile
		/// <summary>
		/// Register the physical file name associated to a session and update the staut to done
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="idStaticNavSession">User session</param>
		/// <param name="fileName"></param>
		public static void RegisterFile(IDataSource dataSource, Int64 idStaticNavSession,string fileName){
			if(idStaticNavSession<0)throw(new ArgumentException("L'identifiant des paramètres pour générer le résultat n'est pas valide"));
			if(fileName == null)throw(new ArgumentNullException("The file name can not be null."));
			if(fileName == string.Empty)throw(new ArgumentException("Unvalid file name : length = 0."));
			try{
				ParameterDataAccess.RegisterFile(dataSource, idStaticNavSession, fileName);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

		#region GetRequestDetails
		/// <summary>
		/// Get all details of the request idStaticNavSession apart from the blob field
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="idStaticNavSession">Session ID to get</param>
		/// <returns>DataSet containing all fields of the request except from the blob filed</returns>
		public static DataSet GetRequestDetails(IDataSource dataSource, Int64 idStaticNavSession){
			if(idStaticNavSession<0)throw(new ArgumentException("L'identifiant des paramètres pour générer le résultat n'est pas valide"));
			try{
				return ParameterDataAccess.GetRequestDetails(dataSource,idStaticNavSession);
			}
			catch(System.Exception err){
				throw(err);
			}
		}
		#endregion

		#region Suppression d'une demande
		/// <summary>
		/// Delete a request 
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		/// <param name="idStaticNavSession">Session ID to delete</param>
		public static void DeleteRequest(IDataSource dataSource, Int64 idStaticNavSession){
			if(idStaticNavSession<0)throw(new ArgumentException("L'identifiant des paramètres pour générer le résultat n'est pas valide"));
			try{
				 ParameterDataAccess.DeleteRequest(dataSource,idStaticNavSession);
			}
			catch(System.Exception err){
				throw(err);
			}
		}
		#endregion


	}
}
