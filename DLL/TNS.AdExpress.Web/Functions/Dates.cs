#region Information
/*
 * auteur : 
 * créé le :
 * modifié le : 22/07/2004
 * par : Guillaume Ragneau
 * */
#endregion

using System;
using System.Collections;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Exceptions;

using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using Cst = TNS.AdExpress.Constantes;
using FrequencyCst = TNS.AdExpress.Constantes.Customer.DB.Frequency;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Ensemble de fonctions de traitement de période
	/// </summary>
	public class Dates{


		#region Variables
		
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public Dates(){		
		}
		#endregion

		#region MAJ des dates de la session
		/// <summary>
		/// Fonction qui les a jour les dates de debut et de fin de periodes dans la session en fonction du type de période sélectionné et de la longueur de la période
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>
		/// <returns>Date de début et de fin de périodes</returns>
		public static ArrayList updateWebSessionDates(WebSession webSession){
		AtomicPeriodWeek tmp;
		ArrayList alDate=new ArrayList();
			switch(webSession.PeriodType){
				case CstCustomerSession.Period.Type.nLastMonth:
					webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-(webSession.PeriodLength-1)).ToString("yyyyMM");
					webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
					break;
				case CstCustomerSession.Period.Type.nLastYear:
					webSession.PeriodBeginningDate = DateTime.Now.AddYears(-(webSession.PeriodLength-1)).ToString("yyyyMM");
					webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
					break;
				case CstCustomerSession.Period.Type.previousMonth:
					webSession.PeriodEndDate = webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
					break;
				case CstCustomerSession.Period.Type.previousYear:
					webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy")+"12";
					webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy")+"01";
					break;
				case CstCustomerSession.Period.Type.dateToDateMonth:
					webSession.PeriodBeginningDate=webSession.PeriodBeginningDate;
					webSession.PeriodEndDate=webSession.PeriodEndDate;
					break;
				case CstCustomerSession.Period.Type.dateToDateWeek:
					webSession.PeriodBeginningDate=webSession.PeriodBeginningDate;
					webSession.PeriodEndDate=webSession.PeriodEndDate;
					break;
				case CstCustomerSession.Period.Type.nLastWeek:
					tmp = new AtomicPeriodWeek(DateTime.Now);
					if (tmp.Week < 10){
						webSession.PeriodEndDate = tmp.Year.ToString()+"0"+tmp.Week.ToString();
					}
					else{
						webSession.PeriodEndDate = tmp.Year.ToString()+tmp.Week.ToString();
					}
					tmp.SubWeek(webSession.PeriodLength-1);
					if(tmp.Week<10){
						webSession.PeriodBeginningDate = tmp.Year.ToString()+"0"+tmp.Week.ToString();
					}else {
						webSession.PeriodBeginningDate = tmp.Year.ToString()+tmp.Week.ToString();
					}
					break;
				case CstCustomerSession.Period.Type.previousWeek:
					tmp = new AtomicPeriodWeek(DateTime.Now);
					tmp.SubWeek(1);
					webSession.PeriodEndDate = webSession.PeriodBeginningDate = tmp.Week.ToString();
					break;
			}
				alDate.Add(webSession.PeriodBeginningDate);
				alDate.Add(webSession.PeriodEndDate);
				return alDate;
				

		}
		#endregion

		#region YYYYMM => "Month, Year" ou YYYYSS => "Week nn, Year"
		/// <summary>
		/// Transforme une periode  sous forme YYYYMM ou YYYYSS en "Month, Yeaar" ou "Week nn, Year"
		/// </summary>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="period">Période à "traduire"</param>
		/// <returns>"Month, Year" ou "Week nn, Year"</returns>
		public static string getPeriodTxt(WebSession webSession, string period){

			StringBuilder txt = new StringBuilder(20);

			// Texte de période
			if (webSession.DetailPeriod == CstWeb.CustomerSessions.Period.DisplayLevel.weekly){
				txt.Append(GestionWeb.GetWebWord(848, webSession.SiteLanguage) + " " + period.Substring(4,2) + " (" + period.Substring(0,4)+")");
			}
			else{
				switch(period.Substring(4,2)){
					case "01":
						txt.Append(GestionWeb.GetWebWord(945, webSession.SiteLanguage));
						break;
					case "02":
						txt.Append(GestionWeb.GetWebWord(946, webSession.SiteLanguage));
						break;
					case "03":
						txt.Append(GestionWeb.GetWebWord(947, webSession.SiteLanguage));
						break;
					case "04":
						txt.Append(GestionWeb.GetWebWord(948, webSession.SiteLanguage));
						break;
					case "05":
						txt.Append(GestionWeb.GetWebWord(949, webSession.SiteLanguage));
						break;
					case "06":
						txt.Append(GestionWeb.GetWebWord(950, webSession.SiteLanguage));
						break;
					case "07":
						txt.Append(GestionWeb.GetWebWord(951, webSession.SiteLanguage));
						break;
					case "08":
						txt.Append(GestionWeb.GetWebWord(952, webSession.SiteLanguage));
						break;
					case "09":
						txt.Append(GestionWeb.GetWebWord(953, webSession.SiteLanguage));
						break;
					case "10":
						txt.Append(GestionWeb.GetWebWord(954, webSession.SiteLanguage));
						break;
					case "11":
						txt.Append(GestionWeb.GetWebWord(955, webSession.SiteLanguage));
						break;
					case "12":
						txt.Append(GestionWeb.GetWebWord(956, webSession.SiteLanguage));
						break;
				}
				txt.Append(" " + period.Substring(0,4));
			}
			return txt.ToString();
		}
		#endregion

		#region Retourne la première lettre du jour de la semaine
		/// <summary>
		/// Retourne la première lettre du jour de la semaine dans la langue en cours 
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="dayOfWeek">jour de la semaine</param>
		/// <returns>première lettre du jour de la semaine</returns>
		public static string getDayOfWeek(WebSession webSession, string dayOfWeek){
			string txt="";
			switch(dayOfWeek){
				case "Monday":
					txt=GestionWeb.GetWebWord(541,webSession.SiteLanguage);
					break;
				case "Tuesday":
					txt=GestionWeb.GetWebWord(542, webSession.SiteLanguage);
					break;
				case "Wednesday":
					txt=GestionWeb.GetWebWord(618, webSession.SiteLanguage);
					break;
				case "Thursday":
					txt=GestionWeb.GetWebWord(543, webSession.SiteLanguage);
					break;
				case "Friday":
					txt=GestionWeb.GetWebWord(544, webSession.SiteLanguage);
					break;
				case "Saturday":
					txt=GestionWeb.GetWebWord(545, webSession.SiteLanguage);
					break;
				case "Sunday":
					txt=GestionWeb.GetWebWord(546, webSession.SiteLanguage);
					break;						
			}
			return txt;
		}
		#endregion

		#region Retourne le jour de la semaine
		/// <summary>
		/// Retourne le jour de la semaine dans la langue en cours 
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="dayOfWeek">jour de la semaine en anglais</param>
		/// <returns>jour de la semaine dans la langue en cours </returns>
		public static string getDay(WebSession webSession, string dayOfWeek){
			string txt="";
			switch(dayOfWeek){
				case "Monday":
					txt=GestionWeb.GetWebWord(654,webSession.SiteLanguage);
					break;
				case "Tuesday":
					txt=GestionWeb.GetWebWord(655, webSession.SiteLanguage);
					break;
				case "Wednesdays":
					txt=GestionWeb.GetWebWord(656, webSession.SiteLanguage);
					break;
				case "Thursday":
					txt=GestionWeb.GetWebWord(657, webSession.SiteLanguage);
					break;
				case "Friday":
					txt=GestionWeb.GetWebWord(658, webSession.SiteLanguage);
					break;
				case "Saturday":
					txt=GestionWeb.GetWebWord(659, webSession.SiteLanguage);
					break;
				case "Sunday":
					txt=GestionWeb.GetWebWord(660, webSession.SiteLanguage);
					break;						
			}
			return txt;
		}
		#endregion

		#region DateTime => dd/MM/YYYY ou MM/dd/YYYY suivant la langue
		/// <summary>
		/// Fonction qui formate une date en chaine de caractère en fonction d'un langage
		/// </summary>
		/// <param name="date">Date à formater</param>
		/// <param name="language">Langage de traduction</param>
		/// <returns>dd/MM/YYYY ou MM/dd/YYYY suivant la langue</returns>
		public static string dateToString(DateTime date, int language){
			switch(language){
				case Cst.DB.Language.ENGLISH:
					return date.ToString("MM/dd/yyyy");
				default:
					return date.ToString("dd/MM/yyyy");
			}
		}
		#endregion

		#region Date de début d'une période en fonction d'un type de période
		/// <summary>
		/// Foction qui extrait à partir d'une période et d'un type de période la date de début de cette période
		/// </summary>
		/// <param name="period">Période dont on veut la date de début</param>
		/// <param name="periodType">Type de période considérée</param>
		/// <returns>Date de début de period</returns>
		/// <remarks>
		/// Utilise la classe:
		///		public TNS.FrameWork.Date.AtomicPeriodWeek
		/// </remarks>
		public static DateTime getPeriodBeginningDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType){
			AtomicPeriodWeek tmpWeek;
			switch(periodType){
				case CstCustomerSession.Period.Type.dateToDateWeek:
				case CstCustomerSession.Period.Type.nLastWeek:
				case CstCustomerSession.Period.Type.previousWeek:
				case CstCustomerSession.Period.Type.LastLoadedWeek:
                    if (period.Length == 6) {
                        tmpWeek = new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)));
                        return tmpWeek.FirstDay;
                    }
                    else {
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
                    }
				case CstCustomerSession.Period.Type.dateToDateMonth:
				case CstCustomerSession.Period.Type.nLastMonth:
				case CstCustomerSession.Period.Type.LastLoadedMonth:
				case CstCustomerSession.Period.Type.nLastYear:
				case CstCustomerSession.Period.Type.previousMonth:
				case CstCustomerSession.Period.Type.previousYear:
				case CstCustomerSession.Period.Type.nextToLastYear:
				case CstCustomerSession.Period.Type.currentYear:
                    if(period.Length==6)
					    return new DateTime(int.Parse(period.Substring(0,4)),int.Parse(period.Substring(4,2)), 1);
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
				default:
				case CstCustomerSession.Period.Type.previousDay:
				case CstCustomerSession.Period.Type.nLastDays:
					return new DateTime(int.Parse(period.Substring(0,4)),int.Parse(period.Substring(4,2)),int.Parse(period.Substring(6,2)));
			}
		}
		#endregion

		#region Date de fin d'une période en fonction d'un type de période
		/// <summary>
		/// Fonction qui extrait à partir d'une période et d'un type de période la date de fin de cette période
		/// </summary>
		/// <param name="period">Période dont on veut la date de fin</param>
		/// <param name="periodType">Type de période considérée</param>
		/// <returns>Date de fin de period</returns>
		/// <remarks>
		/// Utilise la classe:
		///		public TNS.FrameWork.Date.AtomicPeriodWeek
		/// </remarks>
		public static DateTime getPeriodEndDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType){
			switch(periodType){
				case CstCustomerSession.Period.Type.dateToDateWeek:
				case CstCustomerSession.Period.Type.nLastWeek:
				case CstCustomerSession.Period.Type.previousWeek:
				case CstCustomerSession.Period.Type.LastLoadedWeek:
                    if(period.Length == 6)
                        return (new AtomicPeriodWeek(int.Parse(period.Substring(0,4)),int.Parse(period.Substring(4,2)))).LastDay;
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
				case CstCustomerSession.Period.Type.dateToDateMonth:
				case CstCustomerSession.Period.Type.LastLoadedMonth:
				case CstCustomerSession.Period.Type.nLastMonth:
				case CstCustomerSession.Period.Type.nLastYear:
				case CstCustomerSession.Period.Type.previousMonth:
				case CstCustomerSession.Period.Type.previousYear:
				case CstCustomerSession.Period.Type.nextToLastYear:
				case CstCustomerSession.Period.Type.currentYear:
                    if(period.Length == 6)
                        return (new DateTime(int.Parse(period.Substring(0,4)),int.Parse(period.Substring(4,2)), 1)).AddMonths(1).AddDays(-1);			
                    else
                        return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
				default:
				case CstCustomerSession.Period.Type.nLastDays:
				case CstCustomerSession.Period.Type.previousDay:
                    return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), int.Parse(period.Substring(6, 2)));
			}
		}
		#endregion

		#region Vérification de la validité de la fin de la période en fonction de la fréquence de livraison des données dans le module
		/// <summary>
		/// Vérification de la validité de la fin de la période en fonction de la fréquence de livraison des données dans le module
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="EndPeriod">Période de fin</param>
		/// <returns>Période de fin</returns>
		public static string CheckPeriodValidity( WebSession webSession, string EndPeriod){
												
			//traitement de la notion de fréquence
			Int64 frequency = webSession.CustomerLogin.GetIdFrequency(webSession.CurrentModule);
			switch (frequency){
				case FrequencyCst.ANNUAL:
					//si l'année d'etude n'est pas chargée dans son intégralité (année d'étude = année courante
					//ou decembre non chargé)
					if (int.Parse(EndPeriod.Substring(0,4)) >= int.Parse(webSession.LastAvailableRecapMonth.Substring(0,4)))
						throw new NoDataException();
					break;
				case FrequencyCst.MONTHLY:
					return GetAbsoluteEndPeriod(webSession,EndPeriod,1);
				case FrequencyCst.TWO_MONTHLY:
					return GetAbsoluteEndPeriod(webSession,EndPeriod,2);
				case FrequencyCst.QUATERLY:					
					return GetAbsoluteEndPeriod(webSession,EndPeriod,3);
				case FrequencyCst.SEMI_ANNUAL:
					return GetAbsoluteEndPeriod(webSession,EndPeriod,6);
				case FrequencyCst.DAILY:
					throw new DeliveryFrequencyException("La fréquence de livraison des données d'analyse sectorielle n'est pas valide pour ce module (" + FrequencyCst.DAILY.ToString() + ")");
				case FrequencyCst.SEMI_MONTHLY:
					throw new DeliveryFrequencyException("La fréquence de livraison des données d'analyse sectorielle n'est pas valide pour ce module (" + FrequencyCst.SEMI_MONTHLY.ToString() + ")");
				case FrequencyCst.WEEKLY:
					throw new DeliveryFrequencyException("La fréquence de livraison des données d'analyse sectorielle n'est pas valide pour ce module (" + FrequencyCst.WEEKLY.ToString() + ")");
			}

			return EndPeriod;
		}
		#endregion

		#region Affichage de la période
		/// <summary>
		///  Affichage de la période dans les tableaux dynamiques en fonction de l'année choisie
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="period">period</param>
		/// <returns>Période dans les tableaux dynamiques en fonction de l'année choisie</returns>
		public static string getPeriodLabel(WebSession webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type period){
			string beginPeriod="";
			string endPeriod="";
			string year="";
			

			switch(period){
				case CstWeb.CustomerSessions.Period.Type.currentYear :
					beginPeriod=webSession.PeriodBeginningDate;
					endPeriod=WebFunctions.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
					
					break;
				case CstWeb.CustomerSessions.Period.Type.previousYear :
					year=(int.Parse(webSession.PeriodBeginningDate.Substring(0,4))-1).ToString();
					beginPeriod=year+webSession.PeriodBeginningDate.Substring(4);
					//endPeriod= year+webSession.PeriodEndDate.Substring(4);
					endPeriod= year+WebFunctions.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate).Substring(4);
                    						
					break;
				default :
					throw new Exception("getPeriodLabel(WebSession webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.Period period)-->Impossible de déterminer le type de période.");
			}

			return Convertion.ToHtmlString(switchPeriod(webSession,beginPeriod,endPeriod));
		}
		
		/// <summary>
		/// Affichage de la période dans les tableaux dynamiques
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="beginPeriod">Début de la période</param>
		/// <param name="endPeriod">Fin de la période</param>
		/// <returns>Retourne la période sélectionnée</returns>
		public static string switchPeriod(WebSession webSession,string beginPeriod,string endPeriod){
				
			string periodText;
			switch(webSession.PeriodType){

				case CstCustomerSession.Period.Type.nLastMonth:
				case CstCustomerSession.Period.Type.dateToDateMonth:
				case CstCustomerSession.Period.Type.previousMonth:
				case CstCustomerSession.Period.Type.currentYear:
				case CstCustomerSession.Period.Type.previousYear:
				case CstCustomerSession.Period.Type.nextToLastYear:			

					if(beginPeriod!=endPeriod)
						periodText=MonthString.Get(int.Parse(beginPeriod.Substring(4,2)),webSession.SiteLanguage,0)+"-"+MonthString.Get(int.Parse(endPeriod.Substring(4,2)),webSession.SiteLanguage,0)+" "+beginPeriod.Substring(0,4);				
					else
						periodText=MonthString.Get(int.Parse(beginPeriod.Substring(4,2)),webSession.SiteLanguage,0)+" "+beginPeriod.Substring(0,4);				
					break;
				default:
					throw new Exception("switchPeriod(WebSession webSession,string beginPeriod,string endPeriod)-->Impossible de déterminer le type de période.");
			
			}

			return periodText;
		}
		#endregion

		#region Dates de chargement des données

		#region Période chargement des données
		/// <summary>
		/// Dates de chargement des données
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="PeriodBeginningDate">date de début</param>		
		/// <param name="PeriodEndDate">date de fin</param>
		/// <param name="periodType">type de période</param>
		public static void DownloadDates(WebSession webSession,ref string  PeriodBeginningDate,ref string  PeriodEndDate,CstPeriodType periodType){			
			double currentWeek = ((double)DateTime.Now.Day/(double)7);
			currentWeek=Math.Ceiling(currentWeek);
			AtomicPeriodWeek week=new AtomicPeriodWeek(DateTime.Now);	

			
				if(DateTime.Now.Month==1){
					if(CstPeriodType.previousYear == periodType){
						if(IsPeriodActive(currentWeek,week,periodType)){
							PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
							PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
						}else{ 
							PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
							PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy11");
						}
					}else
					throw new NoDataException(GestionWeb.GetWebWord(1612,webSession.SiteLanguage));
				}else {	
					if(CstPeriodType.currentYear == periodType){
						if(IsPeriodActive(currentWeek,week,periodType)){
							PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
							PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						}
						else {
							if( (( currentWeek==1 || currentWeek==2 ) && week.Week<=2 && DateTime.Now.Year==week.FirstDay.Year)
								|| (DateTime.Now.Month==2 && !IsPeriodActive(currentWeek,week,periodType))							  
								)
								throw new NoDataException(GestionWeb.GetWebWord(1612,webSession.SiteLanguage));														
							else {
								PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
								PeriodEndDate = DateTime.Now.AddMonths(-2).ToString("yyyyMM");
							}
						}
					}else {						
							PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
							PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");						
					}
				}							
		}
		#endregion

		#region Période chargement des données sauvegardées
		/// <summary>
		/// Dates de chargement des données sauvegardées
		/// </summary>
		/// <param name="webSessionSave">session du client sauvegardée</param>
		/// <param name="PeriodBeginningDate">date de début </param>
		/// <param name="PeriodEndDate">date de fin</param>
		public static void WebSessionSaveDownloadDates(WebSession webSessionSave,ref string  PeriodBeginningDate,ref string  PeriodEndDate){
			switch(webSessionSave.PeriodType){
				case CstCustomerSession.Period.Type.LastLoadedWeek :
					 LastLoadedWeek(ref PeriodBeginningDate,ref PeriodEndDate);
					break;
				case CstCustomerSession.Period.Type.LastLoadedMonth :
					 LastLoadedMonth(ref PeriodBeginningDate,ref PeriodEndDate,webSessionSave.PeriodType);
					break;				
				case CstCustomerSession.Period.Type.currentYear:
				case CstCustomerSession.Period.Type.previousYear:
					DownloadDates(webSessionSave,ref PeriodBeginningDate,ref PeriodEndDate,webSessionSave.PeriodType);
					break;
				case CstCustomerSession.Period.Type.nLastYear:
					PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy01");
					PeriodEndDate = DateTime.Now.ToString("yyyyMM");
					break;				
				case CstCustomerSession.Period.Type.dateToDateMonth:
					PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
					PeriodEndDate=webSessionSave.PeriodEndDate;
					break;
				case CstCustomerSession.Period.Type.dateToDateWeek:
					PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
					PeriodEndDate=webSessionSave.PeriodEndDate;
					break;
				case CstCustomerSession.Period.Type.nextToLastYear:
					PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy01");
					PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy12");
					break;
				case CstCustomerSession.Period.Type.dateToDate:
					PeriodBeginningDate=webSessionSave.PeriodBeginningDate;
					PeriodEndDate=webSessionSave.PeriodEndDate;
					break;
			}
		}
		#endregion
		
		#region Période active
		/// <summary>
		/// Vérifie si une période peut être traitée
		/// </summary>
		/// <param name="currentWeek">semaine courante</param>
		/// <param name="week">semaine </param>
		/// <param name="periodType">type de période</param>
		/// <returns>vrai si période active</returns>
		private static bool IsPeriodActive(double currentWeek,AtomicPeriodWeek week,CstPeriodType periodType){
			bool enabled =false;
			AtomicPeriodWeek previousWeek;	
			if(currentWeek==1 && week.FirstDay.Month==week.LastDay.Month
				&& (int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())>=5 
				|| int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())==0)){
				enabled = true;
			}
				//2ème semaine du mois en cours
			else if( currentWeek==2 ){
				if(((int) DateTime.Now.DayOfWeek>=5 || (int) DateTime.Now.DayOfWeek==0)){
					enabled = true;
				}else {
					previousWeek=new AtomicPeriodWeek(DateTime.Now.AddDays(-7));
					if(previousWeek.FirstDay.Month==previousWeek.LastDay.Month)
						enabled = true;
				}
				//Plus de 2 semaines du  mois en cours
			}else if(currentWeek>2 && !((CstPeriodType.previousYear == periodType || (CstPeriodType.LastLoadedMonth== periodType && week.FirstDay.Month!=12) ||CstPeriodType.LastLoadedWeek == periodType)  && ( week.Week==52 || week.Week==53))){
				enabled = true;
			}
			return enabled;
		}
		#endregion

		#region  Dernier mois chargé
		/// <summary>
		/// Dernier mois chargé
		/// </summary>
		/// <param name="PeriodBeginningDate">date de début</param>		
		/// <param name="PeriodEndDate">date de fin</param>	
		///  <param name="periodType">type de période</param>
		public static void LastLoadedMonth(ref string  PeriodBeginningDate,ref string  PeriodEndDate,CstPeriodType periodType){			
			double currentWeek = ((double)DateTime.Now.Day/(double)7);
			currentWeek=Math.Ceiling(currentWeek);
			AtomicPeriodWeek week=new AtomicPeriodWeek(DateTime.Now);	
			
				if(DateTime.Now.Month>2){
					if(IsPeriodActive(currentWeek,week,periodType)){
						PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
					}else{
						PeriodBeginningDate = DateTime.Now.AddMonths(-2).ToString("yyyyMM");
						PeriodEndDate = DateTime.Now.AddMonths(-2).ToString("yyyyMM");
					}
				}
				else {
					if(DateTime.Now.Month==2 && IsPeriodActive(currentWeek,week,periodType)){
						PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						PeriodEndDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
					}									
					else {
						if(DateTime.Now.Month==2 || IsPeriodActive(currentWeek,week,periodType)){
							PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
							PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
						}else{
							PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy11");
							PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy11");
						}
					}
				}					
		}
		
		#endregion

		#region Derniere semaine chargée
		/// <summary>
		/// Derniere semaine chargée
		/// </summary>
		/// <param name="PeriodBeginningDate">date de début</param>		
		/// <param name="PeriodEndDate">date de fin</param>			
		public static void LastLoadedWeek(ref string  PeriodBeginningDate,ref string  PeriodEndDate){			
			AtomicPeriodWeek currentWeek=new AtomicPeriodWeek(DateTime.Now);
			int numberWeek=currentWeek.Week;
//			int LoadedWeek=0;		
//			AtomicPeriodWeek previousYearWeek=new AtomicPeriodWeek(DateTime.Now.AddYears(-1));
			
			AtomicPeriodWeek previousWeek=new AtomicPeriodWeek(DateTime.Now.AddDays(-7));
			int days=0;

			#region ancienne version
//			//Plus de 2 semaines année en cours
//			if(numberWeek>2 && currentWeek.Year!=DateTime.Now.AddYears(-1).Year){				
//				if(currentWeek.FirstDay.Month==currentWeek.LastDay.Month &&
//					(int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())>=5 
//					|| int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())== 0)){
//					LoadedWeek = currentWeek.Week-1;					
//				}else LoadedWeek = currentWeek.Week-2;
//				PeriodBeginningDate = currentWeek.Year.ToString()+((LoadedWeek>9)? "": "0" ) + LoadedWeek.ToString();
//				PeriodEndDate = currentWeek.Year.ToString()+((LoadedWeek>9)? "": "0" ) + LoadedWeek.ToString();
//			//2 semaines année en cours
//			}else if(numberWeek==2){
//				if( (int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())>=5 
//					|| int.Parse(DateTime.Now.DayOfWeek.GetHashCode().ToString())== 0)){
//					LoadedWeek = currentWeek.Week-1;
//					PeriodBeginningDate = currentWeek.Year.ToString()+"0"+ LoadedWeek.ToString();
//					PeriodEndDate = currentWeek.Year.ToString()+"0"+ LoadedWeek.ToString();
//				}else {
//					PeriodBeginningDate = previousYearWeek.Year.ToString()+previousYearWeek.NumberWeekInYear.ToString();
//					PeriodEndDate = previousYearWeek.Year.ToString()+previousYearWeek.NumberWeekInYear.ToString();
//				}
//			//1 ere semaine ou 52 ou 53 semaines année précédente
//			}else{
//				LoadedWeek=previousYearWeek.NumberWeekInYear-1;
//				PeriodBeginningDate = previousYearWeek.Year.ToString()+LoadedWeek.ToString();
//				PeriodEndDate = previousYearWeek.Year.ToString()+LoadedWeek.ToString();
//			
//			}
			#endregion
			days = DateTime.Now.Subtract(previousWeek.LastDay).Days;
			if(days<5){			
				previousWeek = new AtomicPeriodWeek(DateTime.Now.AddDays(-14));			
			}
			PeriodBeginningDate = previousWeek.Year.ToString()+((previousWeek.Week>9)? "": "0" ) + previousWeek.Week.ToString();
			PeriodEndDate = previousWeek.Year.ToString()+((previousWeek.Week>9)? "": "0" ) + previousWeek.Week.ToString();
		}
		#endregion

        #region GetFirstDayNotEnable
        /// <summary>
        /// Renvoie le premier jour du calendrier à partir duquel les données ne sont pas encore chargées
        /// </summary>
        /// <returns>Le premier jour du calendrier à partir duquel les données ne sont pas encore chargées</returns>
		public static DateTime GetFirstDayNotEnabled(WebSession webSession, long selectedVehicle, int startYear, IDataSource dataSource) {
            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);
            DateTime firstDayOfWeek = week.FirstDay;
            DateTime publicationDate;
            string lastDate = string.Empty;

            switch ((DBClassificationConstantes.Vehicles.names)selectedVehicle) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
					lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(webSession, selectedVehicle, dataSource);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(1);
                    return firstDayOfWeek;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    return (firstDayOfWeek);
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    if (!((int)DateTime.Now.DayOfWeek >= 5) && !((int)DateTime.Now.DayOfWeek == 0)) {
                        firstDayOfWeek = firstDayOfWeek.AddDays(-7);
                    }
                    return (firstDayOfWeek);
                case DBClassificationConstantes.Vehicles.names.directMarketing:
					lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(webSession, selectedVehicle, dataSource);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(7);
                    return firstDayOfWeek;
                case DBClassificationConstantes.Vehicles.names.internet:
                    lastDate = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(webSession, selectedVehicle,dataSource);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    publicationDate = publicationDate.AddMonths(1);
                    firstDayOfWeek = new DateTime(publicationDate.Year, publicationDate.Month, 1);
                    return firstDayOfWeek;
            }

            return firstDayOfWeek;

        }
        #endregion

		#endregion

		#region Dates année précédente
		/// <summary>
		/// Obtient la date de l'année précédente  sous forme YYYYMM ou YYYYWW
		/// </summary>
		/// <param name="period">date de l'année en cours sous forme YYYYMM ou YYYYWW</param>
		/// <param name="detailPeriod">niveau de détail de la période (ex. mois ou semaine)</param>
		/// <returns>date de l'année précédente  sous forme YYYYMM ou YYYYWW</returns>
		public static string GetPreviousYearDate(string period,CstCustomerSession.Period.DisplayLevel detailPeriod){
			try{
				int year;
				AtomicPeriodWeek tmpWeek;

				switch(detailPeriod){
					//recupère date au format hebdomadaire : YYYYWW
					case CstCustomerSession.Period.DisplayLevel.weekly :
						year = int.Parse(period.Substring(0,4));
						tmpWeek = new AtomicPeriodWeek(year,int.Parse(period.Substring(4,2)));
						tmpWeek.SubWeek(52);
						return tmpWeek.FirstDay.Year.ToString()+(tmpWeek.Week.ToString().Length==1? "0"+tmpWeek.Week.ToString() : tmpWeek.Week.ToString());						
					
					//recupère date au format mensuel : YYYYMM
					case CstCustomerSession.Period.DisplayLevel.monthly :
						year = int.Parse(period.Substring(0,4))- 1;
						return year.ToString()+period.Substring(4,2);
				
					default : 
						throw new FunctionsException(" Impossible d'identifier le niveau de détail de la période.");
				}
			}catch(Exception ex){
				throw new FunctionsException(" Impossible de récupérer la date de l'année précédente.",ex);
			}
		}
		#endregion

		#region Tranches Horaires
		
		/// <summary>
		/// This function returns the time slice 
		/// </summary>
		/// <param name="webSession">session of the client</param>
		/// <param name="timeSlice">time slice</param>
		/// <returns>string time slice</returns>
		public static string GetTimeSlice(WebSession webSession,TNS.AdExpress.Constantes.Web.Repartition.timeInterval timeSlice)
		{
			switch(timeSlice)
			{
				case CstWeb.Repartition.timeInterval.Total:
					return (GestionWeb.GetWebWord(1401, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_7h_12h:
					return (GestionWeb.GetWebWord(1564, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_12h_14h:
					return (GestionWeb.GetWebWord(1566, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_14h_17h:
					return (GestionWeb.GetWebWord(1568, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_17h_19h:
					return (GestionWeb.GetWebWord(1569, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_19h_22h:
					return (GestionWeb.GetWebWord(1570, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_22h_24h:
					return (GestionWeb.GetWebWord(1572, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_24h_7h:
					return (GestionWeb.GetWebWord(1573, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_5h_6h59:
					return (GestionWeb.GetWebWord(1562, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_7h_8h59:
					return (GestionWeb.GetWebWord(1563, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_9h_12h59:
					return (GestionWeb.GetWebWord(1565, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_13h_18h59:
					return (GestionWeb.GetWebWord(1567, webSession.SiteLanguage));
				case CstWeb.Repartition.timeInterval.Slice_19h_24h:
					return (GestionWeb.GetWebWord(1571, webSession.SiteLanguage));
				default:
					return "";
				
			}
		}
		#endregion

		#region Format Spots
		
		/// <summary>
		/// This function returns the format of the spot
		/// </summary>
		/// <param name="webSession">session of the client</param>
		/// <param name="format">format spot</param>
		/// <returns>string format</returns>
		public static string GetFormat(WebSession webSession,TNS.AdExpress.Constantes.Web.Repartition.Format format)
		{
			switch(format)
			{
				case CstWeb.Repartition.Format.Total:
					return (GestionWeb.GetWebWord(1401, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_1_9:
					return (GestionWeb.GetWebWord(1545, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_10:
					return (GestionWeb.GetWebWord(1546, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_11_19:
					return (GestionWeb.GetWebWord(1547, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_20:
					return (GestionWeb.GetWebWord(1548, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_21_29:
					return (GestionWeb.GetWebWord(1549, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_30:
					return (GestionWeb.GetWebWord(1550, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_31_45:
					return (GestionWeb.GetWebWord(1551, webSession.SiteLanguage));
				case CstWeb.Repartition.Format.Spot_45:
					return (GestionWeb.GetWebWord(1552, webSession.SiteLanguage));
				default:
					return "";
				
			}
		}
		#endregion

		#region dernière année à étudier
		/// <summary>
		/// Convertit si nécessaire l'année de la date en année avant année précédente
		/// </summary>
		/// <param name="date">date</param>
		/// <returns>date</returns>
		public static string ConvertToNextToLastYear(string date){

			int nextToLastYear = DateTime.Now.AddYears(-2).Year;
			if(date!=null && date.Length>3 && int.Parse(date.Substring(0,4))< nextToLastYear){				
				date = nextToLastYear.ToString()+date.Substring(4,date.Length-4);
			}

			return date;
		}
		
		#endregion

		#region identifiant de l'année : 0==N , 1==N-1,2==N-2
		/// <summary>
		/// Obtient l'identifiant de l'année sélectionnée  : 0==N , 1==N-1,2==N-2
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="YearSelected">année sélectionné</param>
		/// <param name="year">identifiant année sélectionné</param>
		/// <param name="PeriodBeginningDate">date de début</param>
		public static void GetYearSelected(WebSession webSession, ref string YearSelected,ref int year,DateTime PeriodBeginningDate){
			if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-1)) {
				if(DateTime.Now.Year>webSession.DownLoadDate){
					YearSelected="";
					year=0;
				}				
				else{
					YearSelected="1";
					year=1;
				}
			}
			if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-2)) {
				if(DateTime.Now.Year>webSession.DownLoadDate){
					YearSelected="1";
					year=1;				
				}
				else{				
					YearSelected="2";
					year=2;
				}
			}
			if(PeriodBeginningDate.Year.Equals(System.DateTime.Now.Year-3)) {
				if(DateTime.Now.Year>webSession.DownLoadDate){
					YearSelected="2";
					year=2;				
				}				
			}	
		}

		/// <summary>
		/// Determine l'identifiant de l'année de l'étude : 0==N , 1==N-1,2==N-2
		/// </summary>
		/// <param name="PeriodDate">DateTime</param>		
		///<param name="webSession">Session client</param>
		/// <returns>entier identifiant</returns>
		public static int yearID(DateTime PeriodDate,WebSession webSession){
			int year=0;
			int downLoadDate=webSession.DownLoadDate;
			if(DateTime.Now.Year>downLoadDate){
				if(PeriodDate.Year==DateTime.Now.Year-1)year=0;
				else if(PeriodDate.Year==DateTime.Now.Year-2)year=1; 
				else if(PeriodDate.Year==DateTime.Now.Year-3)year=2;
			}else{
				if(PeriodDate.Year==DateTime.Now.Year-1)year=1;
				else if(PeriodDate.Year==DateTime.Now.Year-2)year=2; 
			}
			return year;
		}
		#endregion
		
		#region Identifie le mois actif 
		/// <summary>
		/// Identifie le mois actif 
		/// </summary>
		/// <param name="PeriodEndDate">fin de la période</param>
		///<param name="webSession">Session client</param>
		/// <returns>mois </returns>
		public static string CurrentActiveMonth(DateTime PeriodEndDate,WebSession webSession){
			string CurrentMonth="";
			// mois actif
			if(PeriodEndDate.Month==DateTime.Now.Month)
				CurrentMonth =GetMonthAlias(PeriodEndDate.Month-1,yearID(PeriodEndDate.AddMonths(-1).Date,webSession),3,webSession);
			else CurrentMonth = GetMonthAlias(PeriodEndDate.Month,yearID(PeriodEndDate,webSession),3,webSession);
			
			return CurrentMonth;
		}
		#endregion

		#region Obtient les alias (lettres) d'un mois
		/// <summary>
		/// Obtient les alias (lettres) d'un mois
		/// </summary>
		/// <param name="monthNumber">Identifiant du mois</param>
		/// <param name="YearSelected">année sélectionnée</param>
		/// <param name="numberOfChar">Nombre de caractères</param>
		/// <returns>Première lettres d'un mois</returns>
		/// <param name="webSession">Session client</param>
		public static string GetMonthAlias(int monthNumber,int YearSelected,int numberOfChar,WebSession webSession){
			string month="";
			string year="";
			if(DateTime.Now.Year>webSession.DownLoadDate){
				YearSelected++;
			}

			try{					
				switch(monthNumber){
					case 1:
						month="January";
						break;
					case 2:
						month="February";
						break;
					case 3:
						month="March";
						break;
					case 4:
						month="April";
						break;
					case 5:
						month="May";
						break;
					case 6:
						month="June";
						break;
					case 7:
						month="July";
						break;
					case 8:
						month="August";
						break;
					case 9:
						month="September";
						break;
					case 10:
						month="October";
						break;
					case 11:
						month="November";
						break;
					case 12:
						month="December";
						break;
					
					default:
						throw(new SQLGeneratorException("Le mois sélectionnée n'est pas correcte: "));
				}

				try{
					switch(YearSelected){
						case 0 :
							year=(System.DateTime.Now.Year.ToString()).Substring(2,2);	
							break;
						case 1 :
							year=((System.DateTime.Now.Year-1).ToString()).Substring(2,2);
							break;
						case 2 :
							year=((System.DateTime.Now.Year-2).ToString()).Substring(2,2);
							break;
						case 3 :
							year=((System.DateTime.Now.Year-3).ToString()).Substring(2,2);
							break;
						default:
							throw(new SQLGeneratorException("L'année sélectionnée n'est pas correcte: "));
					}
				}
				catch(System.Exception ex){
					throw(new SQLGeneratorException("Impossible d'obtenir l'anné sélectionnée :"+ex.Message));
				}
				if(numberOfChar<=month.Length && numberOfChar>0)return(month.Substring(0,numberOfChar)+year);
				return(month);
			}
			catch(System.Exception e){
				throw(new SQLGeneratorException("Impossible d'obtenir les premières lettres d'une date:"+e.Message));
				
			}
		}
		#endregion

		#region Donne la date en fonction de l'alias MMMYY
		/// <summary>
		/// Donne la date en fonction de l'alias MMMYY
		/// </summary>
		/// <param name="DateAlias">date MMMYY</param>		
		/// <returns>retourne la date </returns>
		public static DateTime GetDateFromAlias(string DateAlias){
			string month="";
			int intMonth=0;
			string year="";
			try{									
				switch(DateAlias.Length){
					case 5 :
						month = DateAlias.ToString().Substring(0,3);
						year = DateAlias.ToString().Substring(3,2);
						year = DateTime.Now.Year.ToString().Substring(0,2)+year;
						try{
							switch(month){
								case "Jan":
									intMonth=1;
									break;
								case "Feb":
									intMonth=2;
									break;
								case "Mar":
									intMonth=3;
									break;
								case "Apr":
									intMonth=4;
									break;
								case "May":
									intMonth=5;
									break;
								case "Jun":
									intMonth=6;
									break;
								case "Jul":
									intMonth=7;
									break;
								case "Aug":
									intMonth=8;
									break;
								case "Sep":
									intMonth=9;
									break;
								case "Oct":
									intMonth=10;
									break;
								case "Nov":
									intMonth=11;
									break;
								case "Dec":
									intMonth=12;
									break;
								default :
									throw(new SQLGeneratorException("La date en entrée n'est pas valide"));
							}	
							return  new DateTime(int.Parse(year),int.Parse(intMonth.ToString()),1);
							
						}catch(Exception er){
							throw (new SQLGeneratorException("Impossible de créer la date :"+er.Message));
						}
					default:
						throw(new SQLGeneratorException("La date en entrée n'est pas valide"));
				}					
			}
			catch(System.Exception e){
				throw(new SQLGeneratorException("Impossible d'obtenir les éléments d'une date:"+e.Message));				
			}
		}
		#endregion

        #region Compare two dates and return the greater
        /// <summary>
        /// Compare two dates and return the greater
        /// </summary>
        /// <param name="date1">First Date Param</param>
        /// <param name="date2">Second Date Parm</param>
        /// <returns>Greater Date</returns>
        public static DateTime Max(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) >= 0)
            {
                return date1;
            }
            return date2;
        }
        #endregion

        #region Compare two dates and return the smaller
        /// <summary>
        /// Compare two dates and return the smalelr
        /// </summary>
        /// <param name="date1">First Date Param</param>
        /// <param name="date2">Second Date Parm</param>
        /// <returns>Smaller Date</returns>
        public static DateTime Min(DateTime date1, DateTime date2)
        {
            if (date1.CompareTo(date2) >= 0)
            {
                return date2;
            }
            return date1;
        }
        #endregion

        #region Méthodes internes
        /// <summary>
		/// Obtient la fin de la période en fonction de la fréquence de chargement des données
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="EndPeriod">fin de la période</param>
		/// <param name="divisor">diviseur</param>
		/// <returns>Fin de la période</returns>
		private static string GetAbsoluteEndPeriod(WebSession webSession,string EndPeriod,int divisor){
			
			string absoluteEndPeriod="0";

			//Si l'année sélectionnée est inférieure ou égale à l'année de chargement des données alors cherche à
			//récupérer le dernier mois du dernier trimestre complet
			if(webSession.LastAvailableRecapMonth!=null && webSession.LastAvailableRecapMonth.Length>=6 
				&& int.Parse(EndPeriod.Substring(0,4))<=int.Parse(webSession.LastAvailableRecapMonth.Substring(0,4))						
				){	
				//Si l'année sélectionnée est idem à l'année de chargement des données on récupère le dernier mois du dernier trimestre complet.
				//Sinon si elle est inférieure on récupère le dernier mois sélectionné de l'année précédente.
				if(int.Parse(EndPeriod.Substring(0,4))==int.Parse(webSession.LastAvailableRecapMonth.Substring(0,4))){
					absoluteEndPeriod = webSession.LastAvailableRecapMonth.Substring(0,4) + (int.Parse(webSession.LastAvailableRecapMonth.Substring(4,2)) - int.Parse(webSession.LastAvailableRecapMonth.Substring(4,2)) % divisor).ToString("00");
					//si le mois d'étude est superieur strictment au dernier mois du trimestre chargé, on le ramène au dernier mois du trimestre chargé
					if (int.Parse(EndPeriod) > int.Parse(absoluteEndPeriod)){
						EndPeriod = absoluteEndPeriod;
					}
				}										
			}else{
				EndPeriod = EndPeriod.Substring(0,4)+"00";				
			}
		
			return EndPeriod;
		}
		#endregion

        #region Date de début d'un zoom en fonction d'un type de période
        /// <summary>
        /// Foction qui extrait à partir d'un zoom et d'un type de période la date de début de cette période zoom
        /// </summary>
        /// <param name="period">Zoom dont on veut la date de début</param>
        /// <param name="periodType">Type de période considérée</param>
        /// <returns>Date de début de zoom</returns>
        /// <remarks>
        /// Utilise la classe:
        ///		public TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getZoomBeginningDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
        {
            AtomicPeriodWeek tmpWeek;
            switch (periodType)
            {
                case CstCustomerSession.Period.Type.dateToDateWeek:
                case CstCustomerSession.Period.Type.nLastWeek:
                case CstCustomerSession.Period.Type.previousWeek:
                case CstCustomerSession.Period.Type.LastLoadedWeek:
                    tmpWeek = new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)));
                    return tmpWeek.FirstDay;
                case CstCustomerSession.Period.Type.dateToDateMonth:
                case CstCustomerSession.Period.Type.nLastMonth:
                case CstCustomerSession.Period.Type.LastLoadedMonth:
                case CstCustomerSession.Period.Type.nLastYear:
                case CstCustomerSession.Period.Type.previousMonth:
                case CstCustomerSession.Period.Type.previousYear:
                case CstCustomerSession.Period.Type.nextToLastYear:
                case CstCustomerSession.Period.Type.currentYear:
                case CstCustomerSession.Period.Type.previousDay:
                case CstCustomerSession.Period.Type.nLastDays:
                default:
                    return new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), 1);
            }
        }
        #endregion

        #region Date de fin d'un zoom en fonction d'un type de période
        /// <summary>
        /// Fonction qui extrait à partir d'un zoom et d'un type de période la date de fin de cette période
        /// </summary>
        /// <param name="period">Zoom dont on veut la date de fin</param>
        /// <param name="periodType">Type de période considérée</param>
        /// <returns>Date de fin de zoom</returns>
        /// <remarks>
        /// Utilise la classe:
        ///		public TNS.FrameWork.Date.AtomicPeriodWeek
        /// </remarks>
        public static DateTime getZoomEndDate(string period, TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType)
        {
            switch (periodType)
            {
                case CstCustomerSession.Period.Type.dateToDateWeek:
                case CstCustomerSession.Period.Type.nLastWeek:
                case CstCustomerSession.Period.Type.previousWeek:
                case CstCustomerSession.Period.Type.LastLoadedWeek:
                    return (new AtomicPeriodWeek(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)))).LastDay;
                case CstCustomerSession.Period.Type.dateToDateMonth:
                case CstCustomerSession.Period.Type.LastLoadedMonth:
                case CstCustomerSession.Period.Type.nLastMonth:
                case CstCustomerSession.Period.Type.nLastYear:
                case CstCustomerSession.Period.Type.previousMonth:
                case CstCustomerSession.Period.Type.previousYear:
                case CstCustomerSession.Period.Type.nextToLastYear:
                case CstCustomerSession.Period.Type.currentYear:
                default:
                case CstCustomerSession.Period.Type.nLastDays:
                case CstCustomerSession.Period.Type.previousDay:
                    return (new DateTime(int.Parse(period.Substring(0, 4)), int.Parse(period.Substring(4, 2)), 1)).AddMonths(1).AddDays(-1);
            }
        }
        #endregion

    }
}
