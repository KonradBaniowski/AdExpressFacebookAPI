#region Informations
/*
Auteur : A.Dadouch
Création : 12/05/2005
	G. Facon,	11/08/2005,	New Exception Management
*/
#endregion

using System;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebRules=TNS.AdExpress.Web.Rules;
using WebUI=TNS.AdExpress.Web.UI;
using WebExceptions=TNS.AdExpress.Web.Exceptions;

namespace TNS.AdExpress.Web.BusinessFacade.Results{
	/// <summary>
	/// Accès au résultat de l'analyse dynamique
	/// </summary>
	public class DashBoardSystem{

		#region Html
		/// <summary>
		/// Accès à la construction du tableau de Tableau de Bord en HTML
		/// </summary>
		/// <param name="webSession">WebSession</param>
		/// <returns>Code HTML du tableau de Tableau de Bord</returns>
		public static string GetHtml(WebSession webSession){
			try{
				return(WebUI.Results.DashBoardUI.GetHtml(webSession));
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardSystemException("Impossible de calculer le résultat HTML",err));
			}
		}
		#endregion

		#region Excel
		/// <summary>
		/// Accès à la construction du tableau de Tableau de Bord
		/// </summary>
		/// <param name="page">Page</param>		
		/// <param name="webSession">WebSession</param>
		/// <returns>Code HTML du tableau de Tableau de Bord</returns>
		public static string GetExcel(Page page,WebSession webSession){
			//Construction du tableau de données en fonction du résultat demandé
			try{
				int ExcelType=DashBoardQuery(page); 
				switch(ExcelType){
					case 1 :
						return(WebUI.Results.DashBoardUI.GetExcel(webSession));
					case 2 :
						return(WebUI.Results.DashBoardUI.GetExcelBis(webSession));
					default:
						throw(new WebExceptions.DashBoardSystemException("Impossible de déterminer le type d'Excel"));
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardSystemException("Impossible de calculer le résultat Excel",err));
			}
		}

		#endregion
		
		#region Query
		/// <summary>
		/// Renvoi la méthode d'export excel a utilisé pour Tableau de Board
		/// </summary>
		/// <param name="page">Page</param>
		/// <returns>Entier de la méthode d'export excel de Tableau de Bord</returns>
		private static int DashBoardQuery(Page page){
			int temp=0;
				if (page.Request.QueryString.Get("type")==null){
					temp=1;
				}else if (page.Request.QueryString.Get("type")=="2")
					temp= 2;
			return temp;			
		}
		#endregion

	}
}
