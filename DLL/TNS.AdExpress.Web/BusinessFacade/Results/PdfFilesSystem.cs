#region Informations
// Auteur: A.DADOUCH
// Date de création: 23/08/2005 
#endregion

using System;
using System.Web.UI;
using TNS.FrameWork.DB.Common;
using System.Collections;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebRules = TNS.AdExpress.Web.Rules;
using WebCommon = TNS.AdExpress.Web.Common;
using WebUI = TNS.AdExpress.Web.UI.Results;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Accès aux données Fichiers resultats
	/// </summary>
	public class PdfFilesSystem{
		
		#region Variables
		/// <summary>
		/// Liste de tous les éléments
		/// </summary>
		protected ArrayList _list=new ArrayList();
		#endregion
		
		#region Constructeur
//		/// <summary>
//		/// Constructeur
//		/// </summary>
//		/// <param name="page">Page en cours</param>
//		/// <param name="webSession">Session</param>
//		/// <param name="dataSource">DataSource pour la creation de DataSet</param>
//		public PdfFilesSystem(Page page, WebSession webSession,IDataSource dataSource){
//			try{
//				_list.Add(new WebCommon.Results.FilesItem(WebConstantes.FilesResults.PDF,WebRules.Results.FilesItemRules.GetData(WebConstantes.FilesResults.Type.pdf,webSession,dataSource)));
//			}
//			catch(System.Exception){
//			}
//		}
		#endregion

		#region Sortie HTML
		/// <summary>
		/// Accès à la construction du tableau déroulant Fichiers resultats
		/// </summary>
		/// <param name="page">Page en cours</param>
		/// <param name="webSession">Session</param>
		/// <param name="dataSource">Source de données</param>
		/// <returns>Code HTML du tableau déroulant Fichiers resultats</returns>
        public static string GetHtml(Page page, WebSession webSession, TNS.FrameWork.DB.Common.IDataSource dataSource)
        {
			//Liste des fichiers résultats
			ArrayList list=new ArrayList();
			//liste des types de résultat
			ArrayList typeList = new ArrayList();
			
			#region Construction de la liste des types
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.appm.GetHashCode());
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.appmInsertionDetail.GetHashCode());
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.appmExcel.GetHashCode()); 
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.hotep.GetHashCode()); 
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.miysis.GetHashCode());
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.mnevis.GetHashCode());
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.shou.GetHashCode());
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.amset.GetHashCode());
			typeList.Add(TNS.AdExpress.Anubis.Constantes.Result.type.aton.GetHashCode());
			#endregion
			
			#region Construction de la liste
			list=WebRules.Results.FilesItemRules.GetData(typeList,webSession,dataSource);
			#endregion

			return (WebUI.FilesItemUI.GetHtml(page,list));
		}
		#endregion
		
	}
}
