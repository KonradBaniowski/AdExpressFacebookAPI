#region Informations
// Auteur: A. Obermeyer 
// Date de création: 09/02/2005 
// Date de modification
// Modified By: K.Shehzad 10/05/2005(chagement d'en tête Excel)
// 25/10/2005 B.Masson mise en place de KEuros
#endregion

using System;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Date;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.DataAccess.Results;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Génère le code HTML pour le module Tendance
	/// </summary>
	public class TendenciesUI{

        /// <summary>
        /// Current session
        /// </summary>
        private static WebSession _webSession = null;
		
		#region Ancien code HTML
		/// <summary>
		/// Fournit le code HTML pour le module tendances
		/// </summary>
		/// <param name="tab">tableau de données</param>
		/// <param name="webSession">Session Client</param>
		/// <param name="vehicleName">Nom du vehicle</param>
		/// <param name="excel">true si export excel</param>
		/// <returns>code HTML</returns>
		public static string GetHTMLTendenciesUITemp(object[,] tab,WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName,bool excel){
		
			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
			string totalUnit="";
			string evolPicture="";			
			#endregion

			#region Constantes
			const string P2="p2";
			const string L1="acl1";
			const string L2="acl2";			
			#endregion	

            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo;

			t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 >");
			
			#region Première Ligne
			//Première ligne
			t.Append("\r\n\t<tr height=\"30px\" >");
			t.Append("<td>&nbsp;</td>");
			//Investissement
			t.Append("<td class=\""+P2+"\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>"+GestionWeb.GetWebWord(1206,webSession.SiteLanguage)+"</td>");

			switch(vehicleName){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					//Surface
					t.Append("<td class=\""+P2+"\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>"+GestionWeb.GetWebWord(943,webSession.SiteLanguage)+"</td>");
					// Nombre d'insertion
					t.Append("<td class=\""+P2+"\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+"</td>");
					break;
				case DBClassificationConstantes.Vehicles.names.radio:					
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:	
					//duree
					t.Append("<td class=\""+P2+"\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>"+GestionWeb.GetWebWord(861,webSession.SiteLanguage)+"</td>");
					// Nombre de spot
					t.Append("<td class=\""+P2+"\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>"+GestionWeb.GetWebWord(571,webSession.SiteLanguage)+"</td>");
					break;
				default:
					break;
			}			
			t.Append("</tr>");
			#endregion

			#region Deuxième Ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			switch(vehicleName){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					//Titres
					t.Append("<td class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(1504,webSession.SiteLanguage)+"</td>");
					break;
				case DBClassificationConstantes.Vehicles.names.radio:					
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:	
					//Supports
					t.Append("<td class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
					break;
				default:
					break;
			}	
		
			
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+TendenciesDataAccess.GetPeriodN1(webSession.PeriodBeginningDate).Substring(0,4)+"</td>");
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+webSession.PeriodBeginningDate.Substring(0,4)+"</td>");
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");
			
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+TendenciesDataAccess.GetPeriodN1(webSession.PeriodBeginningDate).Substring(0,4)+"</td>");
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+webSession.PeriodBeginningDate.Substring(0,4)+"</td>");
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");
			
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+TendenciesDataAccess.GetPeriodN1(webSession.PeriodBeginningDate).Substring(0,4)+"</td>");
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+webSession.PeriodBeginningDate.Substring(0,4)+"</td>");
			t.Append("<td class=\""+P2+"\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");

			t.Append("</tr>");
			#endregion

			#region Ligne Total 
		
			if(!webSession.PDM){
				t.Append("\r\n\t<tr height=\"20px\" >");
				t.Append("<td align=\"left\" class=\""+L1+"\" nowrap>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
			
				// Investissement
				totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N1], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				totalUnit=ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				// Evolution
				evolPicture=Evol(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_INVEST],tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INVEST_N],excel);
				totalUnit=ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_INVEST],WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");

				switch(vehicleName){
					case DBClassificationConstantes.Vehicles.names.press:
					case DBClassificationConstantes.Vehicles.names.internationalPress:
						totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N1],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
						totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
						// Evolution
						evolPicture=Evol(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_SURFACE],tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.SURFACE_N],excel);
						totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_SURFACE],WebConstantes.CustomerSessions.Unit.pages,true, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
						break;
					case DBClassificationConstantes.Vehicles.names.radio:					
					case DBClassificationConstantes.Vehicles.names.tv:
					case DBClassificationConstantes.Vehicles.names.others:	
						totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N1],WebConstantes.CustomerSessions.Unit.duration,webSession.PDM, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
						totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N],WebConstantes.CustomerSessions.Unit.duration,webSession.PDM, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
						// Evolution
						evolPicture=Evol(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_DURATION],tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.DURATION_N],excel);
						totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_DURATION],WebConstantes.CustomerSessions.Unit.duration,true, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
						break;
					default:
						break;
				}

				totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N1],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				// Evolution
				evolPicture=Evol(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_INSERTION],tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.INSERTION_N],excel);
				totalUnit= ConvertUnitValueAndPdmToString(tab[FrameWorkConstantes.Tendencies.TOTAL_LINE,FrameWorkConstantes.Tendencies.EVOL_INSERTION],WebConstantes.CustomerSessions.Unit.insertion,true, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
				t.Append("</tr>");
			}
			#endregion

			#region Parcours du tableau
			
			for(int i=FrameWorkConstantes.Tendencies.TOTAL_LINE+1;i<tab.GetLength(0);i++){

				#region Categorie

				if(tab[i,FrameWorkConstantes.Tendencies.CATEGORY_INDEX]!=null){
                    t.Append("\r\n\t<tr align=\"right\" class=\"violetBackGroundV3\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\" height=\"20px\" >");
					
					t.Append("<td align=\"left\" class=\""+L1+"\" nowrap>"+tab[i,FrameWorkConstantes.Tendencies.CATEGORY_INDEX]+"</td>");
			
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INVEST_N1], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INVEST_N], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");					
					// Evolution
					evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_INVEST],tab[i,FrameWorkConstantes.Tendencies.INVEST_N],excel);
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_INVEST],WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");

					switch(vehicleName){
						case DBClassificationConstantes.Vehicles.names.press:
						case DBClassificationConstantes.Vehicles.names.internationalPress:
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.SURFACE_N],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
							// Evolution
							evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_INVEST],tab[i,FrameWorkConstantes.Tendencies.SURFACE_N],excel);
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_INVEST],WebConstantes.CustomerSessions.Unit.pages,true, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
							break;
						case DBClassificationConstantes.Vehicles.names.radio:					
						case DBClassificationConstantes.Vehicles.names.tv:
						case DBClassificationConstantes.Vehicles.names.others:	
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.DURATION_N1],WebConstantes.CustomerSessions.Unit.duration,webSession.PDM, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.DURATION_N],WebConstantes.CustomerSessions.Unit.duration,webSession.PDM, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
							// Evolution
							evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_DURATION],tab[i,FrameWorkConstantes.Tendencies.DURATION_N],excel);
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_DURATION],WebConstantes.CustomerSessions.Unit.duration,true, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
							break;
						default:
							break;
					}
					

					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INSERTION_N],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					// Evolution
					evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_INSERTION],tab[i,FrameWorkConstantes.Tendencies.INSERTION_N],excel);
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_INSERTION],WebConstantes.CustomerSessions.Unit.insertion,true, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
					
					t.Append("</tr>");
				
				}
				#endregion

				#region Media
				if(tab[i,FrameWorkConstantes.Tendencies.MEDIA_INDEX]!=null){
                    t.Append("\r\n\t<tr align=\"right\" class=\"violetBackGroundV2\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" height=\"20px\" >");
					
					t.Append("<td align=\"left\" class=\""+L2+"\" nowrap>&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkConstantes.Tendencies.MEDIA_INDEX]+"</td>");
			
					//totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INVEST_N1],WebConstantes.CustomerSessions.Unit.euro,webSession.PDM);
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INVEST_N1], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					//totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INVEST_N],WebConstantes.CustomerSessions.Unit.euro,webSession.PDM);
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INVEST_N], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					// Evolution
					evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_INVEST],tab[i,FrameWorkConstantes.Tendencies.INVEST_N],excel);
					//totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_INVEST],WebConstantes.CustomerSessions.Unit.euro,true);
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_INVEST],WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");

					switch(vehicleName){
						case DBClassificationConstantes.Vehicles.names.press:
						case DBClassificationConstantes.Vehicles.names.internationalPress:
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.SURFACE_N1],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
							t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.SURFACE_N],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
							t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
							// Evolution
							evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_SURFACE],tab[i,FrameWorkConstantes.Tendencies.SURFACE_N],excel);
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_SURFACE],WebConstantes.CustomerSessions.Unit.pages,true, fp);
							t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");
							break;
						case DBClassificationConstantes.Vehicles.names.radio:					
						case DBClassificationConstantes.Vehicles.names.tv:
						case DBClassificationConstantes.Vehicles.names.others:	
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.DURATION_N1],WebConstantes.CustomerSessions.Unit.duration,webSession.PDM, fp);
							t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
							totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.DURATION_N],WebConstantes.CustomerSessions.Unit.duration,webSession.PDM, fp);
							t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
							// Evolution
							evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_DURATION],tab[i,FrameWorkConstantes.Tendencies.DURATION_N],excel);
                            totalUnit = ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Tendencies.EVOL_DURATION], WebConstantes.CustomerSessions.Unit.duration, true, fp);
							t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");
							break;
						default:
							break;
					}
								

					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INSERTION_N1],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
					t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					totalUnit= ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.INSERTION_N],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
					t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					// Evolution
					evolPicture=Evol(tab[i,FrameWorkConstantes.Tendencies.EVOL_INSERTION],tab[i,FrameWorkConstantes.Tendencies.INSERTION_N],excel);
					totalUnit=ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Tendencies.EVOL_INSERTION],WebConstantes.CustomerSessions.Unit.insertion,true, fp);
					t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");
					t.Append("</tr>");

					#endregion
				
				}			
			}
			#endregion

			t.Append("</table>");

			return t.ToString();

		}
		#endregion

		
		#region Html
		/// <summary>
		/// Génère le code Html pour les tendances
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="vehicleName">Nom du vehicle</param>
		/// <param name="excel">format excel</param>
		/// <returns></returns>
		public static string GetHTMLTendenciesUI(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName,bool excel){

            _webSession = webSession;

			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
			int idOldCategory=-1;
			int idCategory=-1;
			string totalUnit="";
			string evolPicture="";
			string yearCur="";
			string yearPrev="";
			#endregion

			#region Constantes
			const string P2="p2";
			const string L1="acl1";
			const string L2="acl2";
			#endregion	

            IFormatProvider fp = (excel) ? WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfoExcel : WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo;

			DataTable dt = TNS.AdExpress.Web.DataAccess.Results.TendenciesDataAccess.GetDataTendencies(webSession,vehicleName).Tables[0];
			DataTable dtTotal = TNS.AdExpress.Web.DataAccess.Results.TendenciesDataAccess.GetTotalTendencies(webSession,vehicleName).Tables[0];
	
			#region Pas de données à afficher
			if(dt.Rows.Count==0){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}			
			#endregion

            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");
			
			#region Première Ligne
			//Première ligne
			t.Append("\r\n\t<tr height=\"30px\" >");
			t.Append("<td>&nbsp;</td>");
			//Investissement
            t.Append("<td class=\"" + P2 + "\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>" + GestionWeb.GetWebWord(1206, webSession.SiteLanguage) + "</td>");

			switch(vehicleName){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					//Surface
                    t.Append("<td class=\"" + P2 + "\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>" + GestionWeb.GetWebWord(943, webSession.SiteLanguage) + "</td>");
					// Nombre d'insertion
                    t.Append("<td class=\"" + P2 + "\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>" + GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + "</td>");
					break;
				case DBClassificationConstantes.Vehicles.names.radio:					
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:	
					//duree
                    t.Append("<td class=\"" + P2 + "\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>" + GestionWeb.GetWebWord(861, webSession.SiteLanguage) + "</td>");
					// Nombre de spot
                    t.Append("<td class=\"" + P2 + "\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>" + GestionWeb.GetWebWord(571, webSession.SiteLanguage) + "</td>");
					break;
				case DBClassificationConstantes.Vehicles.names.outdoor:	
					// Nombre de panneaux
                    t.Append("<td class=\"" + P2 + "\" colspan=\"3\" align=\"center\" valign=\"middle\" nowrap>" + GestionWeb.GetWebWord(1604, webSession.SiteLanguage) + "</td>");
					break;
				
				default:
					break;
			}			
			t.Append("</tr>");
			#endregion
			
			#region Deuxième Ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			switch(vehicleName){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					//Titres
                    t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(1504, webSession.SiteLanguage) + "</td>");
					break;
				case DBClassificationConstantes.Vehicles.names.radio:					
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:	
				case DBClassificationConstantes.Vehicles.names.outdoor:	
					//Supports
                    t.Append("<td class=\"" + P2 + "\" nowrap>" + GestionWeb.GetWebWord(804, webSession.SiteLanguage) + "</td>");
					break;
				default:
					break;
			}
			
			if(webSession.PeriodType==WebConstantes.CustomerSessions.Period.Type.cumlDate){
//				TNS.FrameWork.Date.AtomicPeriodWeek weekCurrent=new AtomicPeriodWeek(DateTime.Now);
//				TNS.FrameWork.Date.AtomicPeriodWeek weekCurrentPrec=new AtomicPeriodWeek(DateTime.Now.AddYears(-1));
//				weekCurrent.SubWeek(1);
//				weekCurrentPrec.SubWeek(1);

				string period=dt.Rows[0]["date_period"].ToString();
				TNS.FrameWork.Date.AtomicPeriodWeek weekCurrent=new AtomicPeriodWeek(int.Parse(period.Substring(0,4)),int.Parse(period.Substring(4,2)));

				yearCur=weekCurrent.FirstDay.Year.ToString();
				yearPrev=(int.Parse(yearCur)-1).ToString();
				webSession.PeriodEndDate=weekCurrent.LastDay.ToString("yyyyMMdd");
				webSession.PeriodBeginningDate=yearCur+"0101";
				webSession.Save();

                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + yearCur + "</td>");
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + yearPrev + "</td>");	
		

			}
			else{
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + TendenciesDataAccess.GetPeriodN1(webSession.PeriodBeginningDate).Substring(0, 4) + "</td>");	
			}

            t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + GestionWeb.GetWebWord(1212, webSession.SiteLanguage) + "</td>");			

			if(webSession.PeriodType==WebConstantes.CustomerSessions.Period.Type.cumlDate){
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + yearCur + "</td>");
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + yearPrev + "</td>");	
			}
			else{
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + TendenciesDataAccess.GetPeriodN1(webSession.PeriodBeginningDate).Substring(0, 4) + "</td>");
			}


            t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + GestionWeb.GetWebWord(1212, webSession.SiteLanguage) + "</td>");
			if(webSession.PeriodType==WebConstantes.CustomerSessions.Period.Type.cumlDate){
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + yearCur + "</td>");
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + yearPrev + "</td>");	
			}
			else{
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + webSession.PeriodBeginningDate.Substring(0, 4) + "</td>");
                t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + TendenciesDataAccess.GetPeriodN1(webSession.PeriodBeginningDate).Substring(0, 4) + "</td>");
			}
            t.Append("<td class=\"" + P2 + "\" align=\"center\" nowrap>" + GestionWeb.GetWebWord(1212, webSession.SiteLanguage) + "</td>");

			t.Append("</tr>");
			#endregion

			#region Ligne Total

			if(!webSession.PDM){
				t.Append("\r\n\t<tr height=\"20px\" >");
				t.Append("<td align=\"left\" class=\""+L1+"\" nowrap>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
			
				// Investissement
				totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["expenditure_cur"], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				totalUnit=ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["expenditure_prev"], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				// Evolution
				evolPicture=Evol(dtTotal.Rows[0]["expenditure_evol"],dtTotal.Rows[0]["expenditure_cur"],excel);
				totalUnit=ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["expenditure_evol"],WebConstantes.CustomerSessions.Unit.kEuro,true, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");

				switch(vehicleName){
					case DBClassificationConstantes.Vehicles.names.press:
					case DBClassificationConstantes.Vehicles.names.internationalPress:
						totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["page_cur"],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
						totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["page_prev"],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
						// Evolution
						evolPicture=Evol(dtTotal.Rows[0]["page_evol"],dtTotal.Rows[0]["page_cur"],excel);
						totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["page_evol"],WebConstantes.CustomerSessions.Unit.pages,true, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
						break;
					case DBClassificationConstantes.Vehicles.names.radio:					
					case DBClassificationConstantes.Vehicles.names.tv:
					case DBClassificationConstantes.Vehicles.names.others:
                        if (!excel || webSession.PDM)
                        {
                            totalUnit = ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["duration_cur"], WebConstantes.CustomerSessions.Unit.duration, webSession.PDM, fp);
                        }
                        else
                        {
                            totalUnit = string.Format(fp, UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.duration).StringFormat, Convert.ToDouble(dtTotal.Rows[0]["duration_cur"]));
                        }
                        t.Append("<td align=\"right\" class=\"" + L1 + "\" nowrap>" + totalUnit + "</td>");
                        if (!excel || webSession.PDM)
                        {
                            totalUnit = ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["duration_prev"], WebConstantes.CustomerSessions.Unit.duration, webSession.PDM, fp);
                        }
                        else
                        {
                            totalUnit = string.Format(fp, UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.duration).StringFormat, Convert.ToDouble(dtTotal.Rows[0]["duration_prev"]));
                        }
                        t.Append("<td align=\"right\" class=\"" + L1 + "\" nowrap>" + totalUnit + "</td>");
						// Evolution
						evolPicture=Evol(dtTotal.Rows[0]["duration_evol"],dtTotal.Rows[0]["duration_cur"],excel);
						totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["duration_evol"],WebConstantes.CustomerSessions.Unit.duration,true, fp);
						t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
						break;
					default:
						break;
				}

				totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["insertion_cur"],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["insertion_prev"],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
				// Evolution
				evolPicture=Evol(dtTotal.Rows[0]["insertion_evol"],dtTotal.Rows[0]["insertion_cur"],excel);
				totalUnit= ConvertUnitValueAndPdmToString(dtTotal.Rows[0]["insertion_evol"],WebConstantes.CustomerSessions.Unit.insertion,true, fp);
				t.Append("<td align=\"right\" class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
				t.Append("</tr>");
			}

			#endregion

			#region Parcours du tableau
			foreach(DataRow currentRow in dt.Rows){
				idCategory = int.Parse(currentRow["ID_CATEGORY"].ToString());
				if (idOldCategory != idCategory){
                    t.Append("\r\n\t<tr align=\"right\" class=\"violetBackGroundV3\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\" height=\"20px\" >");
					t.Append("<td align=\"left\" class=\""+L1+"\" nowrap>"+currentRow["category"]+"</td>");
					
					//totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_expenditure_cur"],WebConstantes.CustomerSessions.Unit.euro,webSession.PDM);
					totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_expenditure_cur"], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					
					//totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_expenditure_prev"],WebConstantes.CustomerSessions.Unit.euro,webSession.PDM);
					totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_expenditure_prev"], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					
					//totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_expenditure_evol"],WebConstantes.CustomerSessions.Unit.euro,true);
					totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_expenditure_evol"],WebConstantes.CustomerSessions.Unit.kEuro,true, fp);
					evolPicture=Evol(currentRow["sub_expenditure_evol"],currentRow["sub_expenditure_cur"],excel);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
					
					switch(vehicleName){
						case DBClassificationConstantes.Vehicles.names.press:
						case DBClassificationConstantes.Vehicles.names.internationalPress:
							totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_page_cur"],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					
							totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_page_prev"],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					
							totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_page_evol"],WebConstantes.CustomerSessions.Unit.pages,true, fp);
							evolPicture=Evol(currentRow["sub_page_evol"],currentRow["sub_page_cur"],excel);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
							break;
						case DBClassificationConstantes.Vehicles.names.radio:
						case DBClassificationConstantes.Vehicles.names.tv:
						case DBClassificationConstantes.Vehicles.names.others:
                            if (!excel || webSession.PDM)
                            {
                                totalUnit = ConvertUnitValueAndPdmToString(currentRow["sub_duration_cur"], WebConstantes.CustomerSessions.Unit.duration, webSession.PDM, fp);
                            }
                            else
                            {
                                totalUnit = string.Format(fp, UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.duration).StringFormat, Convert.ToDouble(currentRow["sub_duration_cur"]));
                            }
                            t.Append("<td class=\"" + L1 + "\" nowrap>" + totalUnit + "</td>");
                            if (!excel || webSession.PDM)
                            {
                                totalUnit = ConvertUnitValueAndPdmToString(currentRow["sub_duration_prev"], WebConstantes.CustomerSessions.Unit.duration, webSession.PDM, fp);
                            }
                            else
                            {
                                totalUnit = string.Format(fp, UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.duration).StringFormat, Convert.ToDouble(currentRow["sub_duration_prev"]));
                            }
                            t.Append("<td class=\"" + L1 + "\" nowrap>" + totalUnit + "</td>");
					
							totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_duration_evol"],WebConstantes.CustomerSessions.Unit.duration,true, fp);
							evolPicture=Evol(currentRow["sub_duration_evol"],currentRow["sub_duration_cur"],excel);
							t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
							break;
						default:
							break;
					}
					
					totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_insertion_cur"],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					
					totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_insertion_prev"],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit+"</td>");
					
					totalUnit= ConvertUnitValueAndPdmToString(currentRow["sub_insertion_evol"],WebConstantes.CustomerSessions.Unit.insertion,true, fp);
					evolPicture=Evol(currentRow["sub_insertion_evol"],currentRow["sub_insertion_cur"],excel);
					t.Append("<td class=\""+L1+"\" nowrap>"+totalUnit + evolPicture+"</td>");
					idOldCategory = idCategory;
				}
                t.Append("\r\n\t<tr align=\"right\" class=\"violetBackGroundV2\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" height=\"20px\" >");
				t.Append("<td align=\"left\" class=\""+L2+"\" nowrap>&nbsp;&nbsp;"+currentRow["media"]+"</td>");
				
				//totalUnit= ConvertUnitValueAndPdmToString(currentRow["expenditure_cur"],WebConstantes.CustomerSessions.Unit.euro,webSession.PDM);
				totalUnit= ConvertUnitValueAndPdmToString(currentRow["expenditure_cur"], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
				t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					
				//totalUnit= ConvertUnitValueAndPdmToString(currentRow["expenditure_prev"],WebConstantes.CustomerSessions.Unit.euro,webSession.PDM);
				totalUnit= ConvertUnitValueAndPdmToString(currentRow["expenditure_prev"], WebConstantes.CustomerSessions.Unit.kEuro,webSession.PDM, fp);
				t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					
				//totalUnit= ConvertUnitValueAndPdmToString(currentRow["expenditure_evol"],WebConstantes.CustomerSessions.Unit.euro,true);
				totalUnit= ConvertUnitValueAndPdmToString(currentRow["expenditure_evol"],WebConstantes.CustomerSessions.Unit.kEuro,true, fp);
				evolPicture=Evol(currentRow["expenditure_evol"],currentRow["expenditure_cur"],excel);
				t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");
				
				switch(vehicleName){
					case DBClassificationConstantes.Vehicles.names.press:
					case DBClassificationConstantes.Vehicles.names.internationalPress:
						totalUnit= ConvertUnitValueAndPdmToString(currentRow["page_cur"],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
						t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					
						totalUnit= ConvertUnitValueAndPdmToString(currentRow["page_prev"],WebConstantes.CustomerSessions.Unit.pages,webSession.PDM, fp);
						t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					
						totalUnit= ConvertUnitValueAndPdmToString(currentRow["page_evol"],WebConstantes.CustomerSessions.Unit.pages,true, fp);
						evolPicture=Evol(currentRow["page_evol"],currentRow["page_cur"],excel);
						t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");
						break;
					case DBClassificationConstantes.Vehicles.names.radio:
					case DBClassificationConstantes.Vehicles.names.tv:
					case DBClassificationConstantes.Vehicles.names.others:
                        if (!excel || webSession.PDM)
                        {
                            totalUnit = ConvertUnitValueAndPdmToString(currentRow["duration_cur"], WebConstantes.CustomerSessions.Unit.duration, webSession.PDM, fp);
                        }
                        else
                        {
                            totalUnit = string.Format(fp, UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.duration).StringFormat, Convert.ToDouble(currentRow["duration_cur"]));
                        }
                        t.Append("<td class=\"" + L2 + "\" nowrap>" + totalUnit + "</td>");
                        if (!excel || webSession.PDM)
                        {
                            totalUnit = ConvertUnitValueAndPdmToString(currentRow["duration_prev"], WebConstantes.CustomerSessions.Unit.duration, webSession.PDM, fp);
                        }
                        else
                        {
                            totalUnit = string.Format(fp, UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.duration).StringFormat, Convert.ToDouble(currentRow["duration_prev"]));
                        }
                        t.Append("<td class=\"" + L2 + "\" nowrap>" + totalUnit + "</td>");
					
						totalUnit= ConvertUnitValueAndPdmToString(currentRow["duration_evol"],WebConstantes.CustomerSessions.Unit.duration,true, fp);
						evolPicture=Evol(currentRow["duration_evol"],currentRow["duration_cur"],excel);
						t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");
						break;
					default:
						break;
				}
					
				totalUnit= ConvertUnitValueAndPdmToString(currentRow["insertion_cur"],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
				t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					
				totalUnit= ConvertUnitValueAndPdmToString(currentRow["insertion_prev"],WebConstantes.CustomerSessions.Unit.insertion,webSession.PDM, fp);
				t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit+"</td>");
					
				totalUnit= ConvertUnitValueAndPdmToString(currentRow["insertion_evol"],WebConstantes.CustomerSessions.Unit.insertion,true, fp);
				evolPicture=Evol(currentRow["insertion_evol"],currentRow["insertion_cur"],excel);
				t.Append("<td class=\""+L2+"\" nowrap>"+totalUnit + evolPicture+"</td>");
			}


			#endregion

			t.Append("</table>");

			return t.ToString();
		}

		#endregion

		#region Excel
		/// <summary>
		/// Export Excel
		/// </summary>		
		/// <param name="webSession">Session Client</param>
		/// <param name="vehicleName">Nom du vehicle</param>
		/// <param name="excel">True si Export Excel</param>
		/// <returns>Export Excel</returns>
		public static string GetExcelTendenciesUI(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName,bool excel){
			
			#region variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
			#endregion

			#region Rappel des paramètres
			// Paramètres du tableau
            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1500,webSession.SiteLanguage)));
			
			#endregion

			t.Append(GetHTMLTendenciesUI(webSession,vehicleName,excel));
			t.Append(ExcelFunction.GetFooter(webSession));
			return t.ToString();

		}
		#endregion

		#region Methodes Internes
		/// <summary>
		/// Fournit la fleche correspondante à l'évolution
		/// </summary>
		/// <param name="data">Evolution</param>
		/// <param name="N">Année N</param>
		/// <returns>image correspondante</returns>
		/// <param name="excel">True si excel</param>
		internal static string Evol(object data,object N,bool excel){	
		    string themeName = WebApplicationParameters.Themes[_webSession.SiteLanguage].Name;
			string picture="";
			if(excel){
				return "";
			}

			if (data==null){
				if(N==null || decimal.Parse(N.ToString())==0){
                    picture = "&nbsp;<img src=/I/r.gif>"; ;
				}
				else{
					picture="&nbsp;<img src=/I/g.gif>";	
				}

			}			
			else if(decimal.Parse(data.ToString())>0){			
				picture="&nbsp;<img src=/I/g.gif>";			
			}
			else if(decimal.Parse(data.ToString())<0){
				picture="&nbsp;<img src=/I/r.gif>";
			}			
			else{
				picture="&nbsp;<img src=/I/o.gif>";
			}
			return picture;
		}

		/// <summary>
		/// Fournit le bon format pour une unité
		/// </summary>
		/// <param name="unitValue">valeur de l'unité</param>
		/// <param name="unit">unité</param>
		/// <param name="pdm">pdm</param>
		/// <returns>format</returns>
		private static string ConvertUnitValueAndPdmToString(object unitValue,WebConstantes.CustomerSessions.Unit unit,bool pdm, IFormatProvider fp){

            if(unitValue==null){
				return string.Empty;
			}
			return WebFunctions.Units.ConvertUnitValueAndPdmToString(unitValue, unit, pdm, fp);

		}
		#endregion
	}
}
