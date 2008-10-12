#region Informations
// Auteur:D. V. Mussuma
// Cr�ation: 12/12/2005
// Modification:
#endregion

using System;

using TNS.AdExpress.Web.Core.Sessions;
using WebModule=TNS.AdExpress.Constantes.Web.Module;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Translation;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CstPeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;

using DateDll = TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.Web.Functions
{
	/// <summary>
	/// Ensemble de fonctions sur les Modules du site AdExpress.
	/// </summary>
	public class Modules
	{
		#region Tableaux de bord
		
		/// <summary>
		/// V�rifie si le module appartient aux tableaux de bord
		/// </summary>
		/// <param name="webSession">sessioin du client </param>
		/// <returns>vrai c'est un module tableaux de bord</returns>
		public static bool IsDashBoardModule(WebSession webSession){
			switch(webSession.CurrentModule){
				case WebModule.Name.TABLEAU_DE_BORD_PRESSE :
				case WebModule.Name.TABLEAU_DE_BORD_RADIO :
				case WebModule.Name.TABLEAU_DE_BORD_TELEVISION :
				case WebModule.Name.TABLEAU_DE_BORD_PAN_EURO :
                case WebModule.Name.TABLEAU_DE_BORD_EVALIANT:
					return true;
				default : return false;
			}
		}
		
		#endregion

		#region Analyse Dynamique
		/// <summary>
		/// Charge les dates d'�tudes sauvegard�es de l'analyse dynamique
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="webSessionSave">Session Client sauvegard�e</param>
		public static void LoadModuleStudyPeriodDates(WebSession webSession,WebSession webSessionSave){
			

			string downloadBeginningDate="";
			string downloadEndDate="";
			long selectedVehicle = -1;
			DateDll.AtomicPeriodWeek week;
			DateTime monthPeriod;
			string lastCompleteMonth = null;



			if(webSessionSave.SelectionUniversMedia!=null && webSessionSave.SelectionUniversMedia.FirstNode !=null)
			 selectedVehicle = ((LevelInformation)webSessionSave.SelectionUniversMedia.FirstNode.Tag).ID;
				
			if(selectedVehicle == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.internet))
			 lastCompleteMonth = TNS.AdExpress.Web.DataAccess.Selections.Medias.MediaPublicationDatesDataAccess.GetLatestPublication(webSessionSave,WebConstantes.CustomerSessions.Period.DisplayLevel.monthly,WebConstantes.Module.Type.analysis,selectedVehicle);

		
			switch(webSessionSave.PeriodType){
				
					//Mois pr�c�dent
				case CstCustomerSession.Period.Type.previousMonth : 
					//Dates de chargement des donn�es pour Internet
					if(selectedVehicle == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.internet)){						
						if(lastCompleteMonth !=null && lastCompleteMonth.Length>0 && int.Parse(lastCompleteMonth) >= int.Parse(DateTime.Now.AddMonths(-1).ToString("yyyyMM")))
							webSession.PeriodEndDate = webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
						else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,webSessionSave.SiteLanguage));
					}else{
						//Dates de chargement des donn�es pour les autres m�dias
						ActivePreviousAtomicPeriod(CstPeriodType.previousMonth,webSessionSave);											
						webSession.PeriodEndDate= webSession.PeriodBeginningDate=DateTime.Now.AddMonths(-1).ToString("yyyyMM");
					}
					webSession.PeriodType = CstPeriodType.previousMonth;	
					webSession.PeriodLength = 1;
					webSession.DetailPeriod = CstPeriodDetail.monthly;
					break;

					//Semaine pr�c�dente
				case CstCustomerSession.Period.Type.previousWeek:
					ActivePreviousAtomicPeriod(CstPeriodType.previousWeek,webSessionSave);
					webSession.PeriodType=CstPeriodType.previousWeek;
					webSession.PeriodLength=1;
					week = new DateDll.AtomicPeriodWeek(DateTime.Now.AddDays(-7));
					webSession.PeriodBeginningDate=webSession.PeriodEndDate=week.Year.ToString() + ((week.Week>9)?"":"0")+week.Week.ToString();
					webSession.DetailPeriod = CstPeriodDetail.weekly;
					break;

				// N derniers mois
				case CstCustomerSession.Period.Type.nLastMonth:	

					webSession.PeriodType = CstPeriodType.nLastMonth;

					//Dates de chargement des donn�es pour Internet
					if (selectedVehicle == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.internet)) {
						if(lastCompleteMonth !=null && lastCompleteMonth.Length>0){
							webSession.PeriodEndDate = lastCompleteMonth;
							monthPeriod = new DateTime(int.Parse(lastCompleteMonth.Substring(0,4)),int.Parse(lastCompleteMonth.Substring(4,2)),01);						
							webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - webSession.PeriodLength).ToString("yyyyMM");	
						}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,webSessionSave.SiteLanguage));
					}else{
						//Dates de chargement des donn�es pour les autres m�dias
												
						WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.nLastMonth);								
						monthPeriod = new DateTime(int.Parse(downloadBeginningDate.Substring(0,4)),int.Parse(downloadBeginningDate.Substring(4,2)),01);						
						webSession.PeriodBeginningDate = monthPeriod.AddMonths(1 - webSession.PeriodLength).ToString("yyyyMM");							
						webSession.PeriodEndDate = downloadEndDate;						
					}
					webSession.DetailPeriod = CstPeriodDetail.monthly;	
					break;

				//Ann�e courante
				case CstCustomerSession.Period.Type.currentYear:
					webSession.PeriodLength=1;		
			
					//Dates de chargement des donn�es pour Internet
					if(selectedVehicle == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.internet)){
						if( lastCompleteMonth !=null && lastCompleteMonth.Length>0 && int.Parse(lastCompleteMonth.Substring(0,4))==DateTime.Now.Year){
							webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");						
							webSession.PeriodEndDate = lastCompleteMonth;
						}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,webSessionSave.SiteLanguage));
					}else{
						//Dates de chargement des donn�es pour les autres m�dias
						if(DateTime.Now.Month==1){
							throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(1612,webSessionSave.SiteLanguage));														
						}
						else {
							WebFunctions.Dates.DownloadDates(webSessionSave,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.currentYear);								
							webSession.PeriodBeginningDate = downloadBeginningDate;						
							webSession.PeriodEndDate = downloadEndDate;
						}	
					}
					webSession.DetailPeriod = CstPeriodDetail.monthly;
					break;

				//N derni�res semaines
				case CstCustomerSession.Period.Type.nLastWeek:
					
						//obtient derni�re semaine charg�e
						WebFunctions.Dates.LastLoadedWeek(ref downloadBeginningDate,ref downloadEndDate);
						webSession.PeriodType=CstPeriodType.nLastWeek;
						week = new DateDll.AtomicPeriodWeek(int.Parse(downloadEndDate.Substring(0,4)),int.Parse(downloadEndDate.Substring(4,2)));
						webSession.PeriodEndDate = downloadEndDate;
						week.SubWeek(webSession.PeriodLength-1);
						webSession.PeriodBeginningDate = week.Year.ToString() + ((week.Week>9)?"":"0") + week.Week.ToString();
						webSession.DetailPeriod = CstPeriodDetail.weekly;
					
					break;

				//Ann�e pr�c�dente
				case CstCustomerSession.Period.Type.previousYear :

					webSession.PeriodType = CstPeriodType.previousYear;
					webSession.PeriodLength = 1;

					//Dates de chargement des donn�es pour Internet
					if(selectedVehicle == VehiclesInformation.EnumToDatabaseId(DBClassificationConstantes.Vehicles.names.internet)){						
						if(lastCompleteMonth !=null && lastCompleteMonth.Length>0 && int.Parse(lastCompleteMonth.Substring(0,4))>DateTime.Now.AddYears(-2).Year){
							webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
							webSession.PeriodEndDate = (int.Parse(lastCompleteMonth.Substring(0,4))== DateTime.Now.Year)? DateTime.Now.AddYears(-1).ToString("yyyy12") : lastCompleteMonth;
						}else throw new WebExceptions.NoDataException(GestionWeb.GetWebWord(2157,webSessionSave.SiteLanguage));
					}else{
						//Dates de chargement des donn�es pour les autres m�dias										
						WebFunctions.Dates.DownloadDates(webSessionSave,ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.previousYear);								
						webSession.PeriodBeginningDate = downloadBeginningDate;						
						webSession.PeriodEndDate = downloadEndDate;
					}
					webSession.DetailPeriod = CstPeriodDetail.monthly;		
					break;
	
				default :
					webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
					webSession.PeriodEndDate = webSessionSave.PeriodEndDate;

#if DEBUG
//					webSession.PeriodBeginningDate = "200501";
//					webSession.PeriodEndDate = "200506";
#endif
					break;
			}
		}

		#region Gestion activation choix mois ou semaine pr�c�dente
		/// <summary>
		/// G�re la s�lection de la semaine ou mois pr�c�dente
		/// </summary>
		/// <param name="previousPeriod">type de p�riode</param>
		/// <param name="webSession">Session du client</param>
		public static void ActivePreviousAtomicPeriod(CstPeriodType previousPeriod,WebSession webSession){
			string downloadBeginningDate="";
			string downloadEndDate="";
			switch(previousPeriod){
				case CstPeriodType.previousWeek :
					//activation checkbox semaine  pr�c�dente
					WebFunctions.Dates.LastLoadedWeek(ref downloadBeginningDate,ref downloadEndDate);				
					DateDll.AtomicPeriodWeek week = new DateDll.AtomicPeriodWeek(int.Parse(downloadEndDate.Substring(0,4)),int.Parse(downloadEndDate.Substring(4,2)));
					DateDll.AtomicPeriodWeek  currentWeek = new DateDll.AtomicPeriodWeek(DateTime.Now);
					currentWeek.SubWeek(1);
					if(week.LastDay.Subtract(currentWeek.FirstDay).Days<0)throw(new WebExceptions.NoDataException(GestionWeb.GetWebWord(1787,webSession.SiteLanguage)));
					break;
				case CstPeriodType.previousMonth :
					//activation checkbox mois  pr�c�dente
					WebFunctions.Dates.LastLoadedMonth(ref downloadBeginningDate,ref downloadEndDate,CstPeriodType.nLastMonth);								
					DateTime monthPeriod = new DateTime(int.Parse(downloadEndDate.Substring(0,4)),int.Parse(downloadEndDate.Substring(4,2)),01);										
					if( !monthPeriod.Month.ToString().Equals(DateTime.Now.AddMonths(-1).Month.ToString())
						|| !monthPeriod.Year.ToString().Equals(DateTime.Now.AddMonths(-1).Year.ToString()))
						throw(new WebExceptions.NoDataException(GestionWeb.GetWebWord(1787,webSession.SiteLanguage)));
					break;
				default:
					throw(new WebExceptions.NoDataException(" Impossible d'identifier le d�tail de la p�riode"));
			}
		}
		#endregion



		#endregion

		#region Prrainage TV
		/// <summary>
		/// Obtient si le module appartient au groupe PARRAINAGE TV
		/// </summary>
		/// <param name="webSession"></param>
		/// <returns></returns>
		public static bool IsSponsorShipTVModule(WebSession webSession){
			Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
			if(currentModuleDescription.ModuleType==WebModule.Type.tvSponsorship)
				return true;
			return false;
		}
		#endregion

		#region  AnalyseSectorielle
		/// <summary>
		/// V�rifie si le module appartient aux Recap
		/// </summary>
		/// <param name="webSession">sessioin du client </param>
		/// <returns>vrai si c'est un module des Recap</returns>
		public static bool IsRecapModule(WebSession webSession) {
			switch (webSession.CurrentModule) {
				case WebModule.Name.INDICATEUR:
				case WebModule.Name.TABLEAU_DYNAMIQUE:			
					return true;
				default: return false;
			}
		}
		#endregion
	}
}
