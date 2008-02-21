#region Informations
/*
Auteur : A.Dadouch
Cr�ation : 12/05/2005
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
	/// Acc�s au r�sultat de l'analyse dynamique
	/// </summary>
	public class DashBoardSystem{

		#region Html
		/// <summary>
		/// Acc�s � la construction du tableau de Tableau de Bord en HTML
		/// </summary>
		/// <param name="webSession">WebSession</param>
		/// <returns>Code HTML du tableau de Tableau de Bord</returns>
		public static string GetHtml(WebSession webSession){
			try{
				return(WebUI.Results.DashBoardUI.GetHtml(webSession));
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardSystemException("Impossible de calculer le r�sultat HTML",err));
			}
		}
		#endregion

		#region Excel
		/// <summary>
		/// Acc�s � la construction du tableau de Tableau de Bord
		/// </summary>
		/// <param name="page">Page</param>		
		/// <param name="webSession">WebSession</param>
		/// <returns>Code HTML du tableau de Tableau de Bord</returns>
		public static string GetExcel(Page page,WebSession webSession){
			//Construction du tableau de donn�es en fonction du r�sultat demand�
			try{
				int ExcelType=DashBoardQuery(page); 
				switch(ExcelType){
					case 1 :
						return(WebUI.Results.DashBoardUI.GetExcel(webSession));
					case 2 :
						return(WebUI.Results.DashBoardUI.GetExcelBis(webSession));
					default:
						throw(new WebExceptions.DashBoardSystemException("Impossible de d�terminer le type d'Excel"));
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.DashBoardSystemException("Impossible de calculer le r�sultat Excel",err));
			}
		}

		#endregion
		
		#region Query
		/// <summary>
		/// Renvoi la m�thode d'export excel a utilis� pour Tableau de Board
		/// </summary>
		/// <param name="page">Page</param>
		/// <returns>Entier de la m�thode d'export excel de Tableau de Bord</returns>
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
