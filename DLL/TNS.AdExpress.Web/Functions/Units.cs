#region Informations
// Auteur:
// Date de création:
// Date de modification:
//24/10/2005 D. V. Mussuma Ajout des fonctions de conversion des unités
#endregion

using System;
using System.Collections;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DateFrameWork=TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork;

namespace TNS.AdExpress.Web.Functions{

	/// <summary>
	/// Fonctions de travail sur les unités
	/// </summary>
	public class Units{
		
		#region Conversion des unités
		/// <summary>
		/// Convertit la valeur de l'unité dans le format correspondant à l'unité
		/// </summary>
		/// <param name="unitValue">Valeur de l'unité</param>
		/// <param name="unit">Unité</param>
		/// <returns>Valeur convertie</returns>
		public static string ConvertUnitValueToString(string unitValue,WebConstantes.CustomerSessions.Unit unit){
			switch(unit){
				//unité page
				case WebConstantes.CustomerSessions.Unit.pages:
					//if(unitValue=="0")return(double.Parse(unitValue).ToString("### ### ### ###.###"));
					//return(double.Parse(unitValue).ToString("### ### ### ##0.###"));
					return(ConvertToPages(unitValue,3));
				//unité durée
				case WebConstantes.CustomerSessions.Unit.duration:
					return(DateFrameWork.DateString.SecondToHH_MM_SS(int.Parse(unitValue)));
				//unité kEuro
				case WebConstantes.CustomerSessions.Unit.kEuro: 
					return(ConvertToKEuro(unitValue,0));
				//unité euro
				case WebConstantes.CustomerSessions.Unit.euro:
                    if(unitValue==null || unitValue.Length==0)return("");
					return(Int64.Parse(unitValue).ToString("### ### ### ### ###"));
				//unité Mm/Col
				case WebConstantes.CustomerSessions.Unit.mmPerCol:
					return(Int64.Parse(unitValue).ToString("### ### ### ### ###"));
				case WebConstantes.CustomerSessions.Unit.spot:
				case WebConstantes.CustomerSessions.Unit.insertion:
					return(Int64.Parse(unitValue).ToString("### ### ### ### ###"));
				case WebConstantes.CustomerSessions.Unit.grp:
					return(double.Parse(unitValue).ToString("### ### ### ### ###.##"));
                //unité volume
                case WebConstantes.CustomerSessions.Unit.volume:
                    if (unitValue == null || unitValue.Length == 0) return ("");
                    double volume =  Math.Round(double.Parse(unitValue));
                    return (volume.ToString("### ### ### ### ###"));
				default:
					return(Int64.Parse(unitValue).ToString("### ### ### ### ###"));
			}
		}
		
		/// <summary>
		/// Convertit la valeur de l'unité dans le format correspondant à l'unité
		/// </summary>
		/// <param name="unitValue">Valeur de l'unité</param>
		/// <param name="unit">Unité</param>
		/// <param name="kEuroDecimals">nombre de chiffres après la virgule pour unité kilo euro</param>
		/// <param name="pagesDecimals">nombre de chiffres après la virgule pour unité pages</param>
		/// <param name="euroDecimals">nombre de chiffres après la virgule pour unité euro</param>
		/// <param name="grpDecimals">nombre de chiffres après la virgule pour unité grp</param>
		/// <param name="insertionDecimals">nombre de chiffres après la virgule pour unité insertion</param>
		/// <param name="mmcDecimals">nombre de chiffres après la virgule pour unité mm/col</param>
		/// <returns>Valeur convertie</returns>
		public static string ConvertUnitValueToString(string unitValue,WebConstantes.CustomerSessions.Unit unit,int kEuroDecimals,int euroDecimals,int pagesDecimals,int grpDecimals,int insertionDecimals, int mmcDecimals){
			string decimalsString="";
			
			switch(unit){
				//unité page
				case WebConstantes.CustomerSessions.Unit.pages:
					for(int i=0;i<pagesDecimals;i++){
						if(i==0)decimalsString=".";
						decimalsString+="#";
					}					
					//if(unitValue=="0")return(double.Parse(unitValue).ToString("### ### ### ##0"+decimalsString));					
					//return(double.Parse(unitValue).ToString("### ### ### ###"+decimalsString));
					return(ConvertToPages(unitValue,pagesDecimals));
				//unité durée
				case WebConstantes.CustomerSessions.Unit.duration:
					return(DateFrameWork.DateString.SecondToHH_MM_SS(long.Parse(unitValue)));
				//unité kEuro
				case WebConstantes.CustomerSessions.Unit.kEuro: 
					return(ConvertToKEuro(unitValue,kEuroDecimals));
				//unité euro
				case WebConstantes.CustomerSessions.Unit.euro:
					for(int i=0;i<pagesDecimals;i++){
						if(i==0)decimalsString=".";
						decimalsString+="#";
					}	
					return(double.Parse(unitValue).ToString("### ### ### ### ###"+decimalsString));
				//unité grp
				case WebConstantes.CustomerSessions.Unit.grp:
					for(int i=0;i<grpDecimals;i++){
						if(i==0)decimalsString=".";
						decimalsString+="#";
					}	
					return(double.Parse(unitValue).ToString("### ### ### ### ###"+decimalsString));
				//unité insertion
				case WebConstantes.CustomerSessions.Unit.spot:
				case WebConstantes.CustomerSessions.Unit.insertion:
					for(int i=0;i<insertionDecimals;i++){
						if(i==0)decimalsString=".";
						decimalsString+="#";
					}	
					return(double.Parse(unitValue).ToString("### ### ### ### ###"+decimalsString));
					//unité mm/col
				case WebConstantes.CustomerSessions.Unit.mmPerCol:
					for(int i=0;i<mmcDecimals;i++){
						if(i==0)decimalsString=".";
						decimalsString+="#";
					}	
					return(double.Parse(unitValue).ToString("### ### ### ### ###"+decimalsString));

				default:
					return(Int64.Parse(unitValue).ToString("### ### ### ### ###"));
			}
		}

		/// <summary>
		/// Convertit la valeur de l'unité dans le format correspondant à l'unité ainsi que les PDM
		/// </summary>
		/// <param name="unitValue">Valeur de l'unité</param>
		/// <param name="unit">Unité</param>
		/// <param name="pdm">pdm</param>
		/// <returns>Valeur convertie</returns>
		public static string ConvertUnitValueAndPdmToString(string unitValue,WebConstantes.CustomerSessions.Unit unit,bool pdm){
			if(!pdm) return ConvertUnitValueToString(unitValue,unit);
			else return double.Parse(unitValue).ToString("0.00");	// ToString("# ### ##0.##")
		}

		/// <summary>
		/// Convertit la valeur de l'unité dans le format correspondant à l'unité ainsi que les PDM
		/// </summary>
		/// <param name="unitValue">Valeur de l'unité</param>
		/// <param name="unit">Unité</param>
		/// <param name="pdm">pdm</param>
		/// <param name="decimals">nombre de chiffres après la virgule pour unité KEuro</param>
		/// <returns>Valeur convertie</returns>
		public static string ConvertUnitValueAndPdmToString(string unitValue,WebConstantes.CustomerSessions.Unit unit,bool pdm,int decimals){
			if(!pdm) return ConvertUnitValueToString(unitValue,unit,decimals,0,3,2,2,0);
			else return double.Parse(unitValue).ToString("0.00");	// ToString("# ### ##0.##")
		}

		/// <summary>
		/// Converti une valeur Euro en KEuro
		/// </summary>
		/// <param name="unitValue">Valeur à traiter</param>
		/// <returns>Valeur converti</returns>
		/// <remarks>Division de la valeur par 1000</remarks>
		private static string ConvertToKEuro(string unitValue){			
			return ConvertToKEuro(unitValue,0);
		}

		/// <summary>
		/// Converti une valeur Euro en KEuro
		/// </summary>
		/// <param name="unitValue">Valeur à traiter</param>
		/// <param name="decimals">Nombre de chiffres après la virgule</param>
		/// <returns>Valeur converti</returns>
		/// <remarks>Division de la valeur par 1000</remarks>
		private static string ConvertToKEuro(string unitValue,int decimals){
			double valueToConvert = double.Parse(unitValue) / 1000;
			if(valueToConvert==0)return("");
			if(decimals==0) return(valueToConvert.ToString("### ### ### ##0"));

			string decimalsString="";
			for(int i=0;i<decimals;i++){
				decimalsString+="#";
			}
			return(valueToConvert.ToString("### ### ### ##0."+decimalsString));
			
		}

		/// <summary>
		/// Converti une valeur en Pages
		/// </summary>
		/// <param name="unitValue">Valeur à traiter</param>
		/// <returns>Valeur converti</returns>
		/// <remarks>Division de la valeur par 1000</remarks>
		private static string ConvertToPages(string unitValue){			
			return ConvertToPages(unitValue,0);
		}

		/// <summary>
		/// Converti une valeur en Pages
		/// </summary>
		/// <param name="unitValue">Valeur à traiter</param>
		/// <param name="decimals">Nombre de chiffres après la virgule</param>
		/// <returns>Valeur converti</returns>
		/// <remarks>Division de la valeur par 1000</remarks>
		private static string ConvertToPages(string unitValue,int decimals){
			double valueToConvert = double.Parse(unitValue) / 1000;
			if(valueToConvert==0)return("");
			if(decimals==0) return(valueToConvert.ToString("### ### ### ##0"));

			string decimalsString="";
			for(int i=0;i<decimals;i++){
				decimalsString+="#";
			}
			return(valueToConvert.ToString("### ### ### ##0."+decimalsString));
		}
		#endregion

		#region Unités disponibles en fonction des vehicule
		/// <summary>
		/// Renvoie les unités disponibles en fonction des vehicule d'étude
		/// </summary>
		/// <param name="vehicleSelection">Liste des vehicule séprés par un virgule</param>
		/// <returns>Liste des unités disponibles</returns>
		public static ArrayList getUnitsFromVehicleSelection(string vehicleSelection){
			ArrayList units=new ArrayList();
			if(vehicleSelection.IndexOf("7")>=0 || vehicleSelection.IndexOf("9")>=0){
				//vehicule cinéma ou internet présent dans la liste
				units.Add(WebConstantes.CustomerSessions.Unit.euro);
			}
			else{
				if (vehicleSelection.Split(',').Length==2 && vehicleSelection.IndexOf("2")>=0 && vehicleSelection.IndexOf("3")>=0){
					//vehicule radio et télé uniquement
					units.Add(WebConstantes.CustomerSessions.Unit.kEuro); 
					units.Add(WebConstantes.CustomerSessions.Unit.euro);
					units.Add(WebConstantes.CustomerSessions.Unit.duration);
					units.Add(WebConstantes.CustomerSessions.Unit.spot);
				}
				else if (vehicleSelection.Split(',').Length>=2){
					//plus d'un vehicule dans la sélection (sélection différents de "radio, tv"
					units.Add(WebConstantes.CustomerSessions.Unit.kEuro); 
					units.Add(WebConstantes.CustomerSessions.Unit.euro);
                    if(!(vehicleSelection.IndexOf("10")>=0))
					    units.Add(WebConstantes.CustomerSessions.Unit.insertion);
				}
				else{
					switch(vehicleSelection){
						case "1":
						case "11":
							//presse
							units.Add(WebConstantes.CustomerSessions.Unit.kEuro); 
							units.Add(WebConstantes.CustomerSessions.Unit.euro);
							units.Add(WebConstantes.CustomerSessions.Unit.pages);
							units.Add(WebConstantes.CustomerSessions.Unit.mmPerCol);							
							units.Add(WebConstantes.CustomerSessions.Unit.insertion);
							break;
						case "2":
							//radio
						case "3":
						case "5":
							//tv
							units.Add(WebConstantes.CustomerSessions.Unit.kEuro); 
							units.Add(WebConstantes.CustomerSessions.Unit.euro);
							units.Add(WebConstantes.CustomerSessions.Unit.duration);
							units.Add(WebConstantes.CustomerSessions.Unit.spot);
							break;
						case "8":
							//affichage
							units.Add(WebConstantes.CustomerSessions.Unit.kEuro); 
							units.Add(WebConstantes.CustomerSessions.Unit.euro);
							units.Add(WebConstantes.CustomerSessions.Unit.numberBoard);
							//units.Add(WebConstantes.CustomerSessions.Unit.insertion);
							break;
                        case "10":
                            //MarketingDirect
                            units.Add(WebConstantes.CustomerSessions.Unit.kEuro);
                            units.Add(WebConstantes.CustomerSessions.Unit.euro);
                            units.Add(WebConstantes.CustomerSessions.Unit.volume);
                            break;
						default:
							units.Add(WebConstantes.CustomerSessions.Unit.kEuro); 
							units.Add(WebConstantes.CustomerSessions.Unit.euro);
							break;
					}
				}
			}
			return units;
		}
		
		/// <summary>
		/// Renvoie les unités disponibles de la press pour le module Appm
		/// </summary>
		/// <returns>Liste des unités Appm disponibles</returns>
		public static ArrayList getUnitsFromAppmPress() {
			ArrayList unitsAppm=new ArrayList();
			//vehicule press
			unitsAppm.Add(WebConstantes.CustomerSessions.Unit.kEuro); // Nouveau
			unitsAppm.Add(WebConstantes.CustomerSessions.Unit.grp);
			unitsAppm.Add(WebConstantes.CustomerSessions.Unit.euro);
			unitsAppm.Add(WebConstantes.CustomerSessions.Unit.pages);
			unitsAppm.Add(WebConstantes.CustomerSessions.Unit.insertion);
			return unitsAppm;
		}

		/// <summary>
		/// Renvoie la liste des encarts 
		/// </summary>
		/// <returns>liste des encarts</returns>
		public static ArrayList getInserts(){
			ArrayList inserts=new ArrayList();
			inserts.Add(WebConstantes.CustomerSessions.Insert.total);
			inserts.Add(WebConstantes.CustomerSessions.Insert.insert);
			inserts.Add(WebConstantes.CustomerSessions.Insert.withOutInsert);
			return inserts;
		}
		#endregion

		#region libellés
//		/// <summary>
//		/// Libellé unité
//		/// </summary>
//		/// <param name="webSession">session  du client</param>
//		/// <param name="excel">vrai si sortie au format excel</param>
//		/// <returns> Libellé unité</returns>
//		public static string UnitLabel(WebSession webSession,bool excel){
//			switch(webSession.CurrentModule){
//				case WebConstantes.Module.Name.INDICATEUR :
//				case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE :
//					if(excel)
//						return Convertion.ToHtmlString(GestionWeb.GetWebWord(1789,webSession.SiteLanguage));	
//					else
//						return GestionWeb.GetWebWord(1789,webSession.SiteLanguage);					
//				default :
//					if(excel)
//						return Convertion.ToHtmlString(GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage));	
//					else
//						return GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage);	
//			}
//		}
		#endregion

	}
}
