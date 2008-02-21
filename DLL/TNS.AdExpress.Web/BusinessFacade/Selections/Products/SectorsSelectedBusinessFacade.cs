#region Informations
// Auteur: K. Shehzad 
// Date de création: 22/04/2005 
//	G. Facon	11/08/2005	New Exception Management 
//  Y. Rkaina   26/07/2006 Surcharge de la méthode GetSectorsSelected
#endregion

using System;
using System.Data;
using System.Text;
using DBClassificationCst = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Translation;

using SSDataAccess= TNS.AdExpress.Web.DataAccess.Selections.Products;

namespace TNS.AdExpress.Web.BusinessFacade.Selections.Products{
	/// <summary>
	/// This class is used to generate HTML and EXCEL strings which are used in the DetailSelection.aspx page to show the
	/// sectors which are being selected. Class SectorsSelectedDataAccess.cs is used to get the dataset.
	/// </summary>
	public class SectorsSelectedBusinessFacade{
		
		#region HTML
		/// <summary>
		/// Accès à la construction du tableau des familles selectionnés
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du famille qui sont selectionées</returns>
		public static String GetSectorsSelected(WebSession webSession){
			try{
				//calling the DataAccess class to get the DataSet
				DataSet ds=SSDataAccess.SectorsSelectedDataAccess.getData(webSession);
				//checking that Dataset is null or not if null we just return an empty string 
				if(ds==null)return "";
				StringBuilder html = new StringBuilder(2000);
				
				DataTable sectors = ds.Tables[0];
				Boolean flag=false;
				html.Append("<br>");
				html.Append("<div class=\"txtViolet11Bold\" align=\"left\" >"+"&nbsp;&nbsp;"+GestionWeb.GetWebWord(1601,webSession.SiteLanguage)+ "</div>");
				
				//creation of the table
				
				html.Append("<table style=\"border-bottom :#644883 1px solid; border-top :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width="+600+"  >");
				foreach(DataRow dr in sectors.Rows){
					if(flag){
						html.Append("<tr>");
						html.Append("<td bgcolor=\"#644883\" height=\"1px\" >");	
						html.Append("</td>");
						html.Append("</tr>");
					}
					flag=true;
					html.Append("<tr>");
					html.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");	
					html.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+dr["sector"].ToString());
					html.Append("</td>");
					html.Append("</tr>");				
			
				}
				html.Append("</table>");
				return html.ToString();
			}
			catch (System.Exception err){
				throw(new WebExceptions.SectorsSelectedBusinessFacadeException("Impossible de construire le tableau de résultat pour famille",err));
			}
		}

		#endregion

		#region HTML
		/// <summary>
		/// Accès à la construction du tableau des familles selectionnés
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="withTitle">Boolean pour indiquer l'ajout ou non du titre</param>
		/// <returns>Code HTML du famille qui sont selectionées</returns>
		public static String GetSectorsSelected(WebSession webSession,bool withTitle){
			try{
				//calling the DataAccess class to get the DataSet
				DataSet ds=SSDataAccess.SectorsSelectedDataAccess.getData(webSession);
				//checking that Dataset is null or not if null we just return an empty string 
				if(ds==null)return "";
				StringBuilder html = new StringBuilder(2000);
				
				DataTable sectors = ds.Tables[0];
				Boolean flag=false;
				if(withTitle){
					html.Append("<br>");
					html.Append("<div class=\"txtViolet11Bold\" align=\"left\" >"+"&nbsp;&nbsp;"+GestionWeb.GetWebWord(1601,webSession.SiteLanguage)+ "</div>");
				}
				
				//creation of the table
				
				html.Append("<table align=\"center\" style=\"border-bottom :#644883 1px solid; border-top :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 width="+600+"  >");
				foreach(DataRow dr in sectors.Rows){
					if(flag){
						html.Append("<tr>");
						html.Append("<td bgcolor=\"#644883\" height=\"1px\" >");	
						html.Append("</td>");
						html.Append("</tr>");
					}
					flag=true;
					html.Append("<tr>");
					html.Append("<td align=\"left\" height=\"10\"  valign=\"middle\" nowrap>");	
					html.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+dr["sector"].ToString());
					html.Append("</td>");
					html.Append("</tr>");				
			
				}
				html.Append("</table>");
				return html.ToString();
			}
			catch (System.Exception err){
				throw(new WebExceptions.SectorsSelectedBusinessFacadeException("Impossible de construire le tableau de résultat pour famille",err));
			}
		}

		#endregion

		#region Excel
		/// <summary>
		/// Accès à la construction du tableau des familles selectionées
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Code HTML du tableau des familles selectionnées</returns>
		public static String GetExcelSectorsSelected(WebSession webSession)
		{
			try{

				DataSet ds=SSDataAccess.SectorsSelectedDataAccess.getData(webSession);
				if(ds==null)return "";
				StringBuilder html = new StringBuilder(2000);
				DataTable sectors = ds.Tables[0];
				Boolean flag=false;
				html.Append("<br>");
				html.Append("<div class=\"txtViolet11Bold\" align=\"left\" >"+"&nbsp;&nbsp;"+GestionWeb.GetWebWord(1601,webSession.SiteLanguage)+ "</div>");
				html.Append("<table style=\"border-bottom :#644883 1px solid; border-top :#644883 1px solid; border-left :#644883 1px solid; border-right :#644883 1px solid; \" class=\"txtViolet11Bold\"  cellpadding=0 cellspacing=0 >");
				foreach(DataRow dr in sectors.Rows){
					html.Append("<tr>");
					if(flag){
						html.Append("<td style=\"border-top :#644883 1px solid;\" align=\"left\" valign=\"middle\" height=\"10\" nowrap>");	
					}
					else
						html.Append("<td align=\"left\" valign=\"middle\" height=\"10\" nowrap>");	
					
					flag=true;								
					html.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+dr["sector"].ToString());
					html.Append("</td>");
					html.Append("</tr>");				
			
				}
				html.Append("</table>");
				return html.ToString();
			}
			catch (System.Exception err){
				throw(new WebExceptions.SectorsSelectedBusinessFacadeException("Impossible de construire le tableau de résultat pour famille",err));
			}
		}

		
		#endregion

	}
}
