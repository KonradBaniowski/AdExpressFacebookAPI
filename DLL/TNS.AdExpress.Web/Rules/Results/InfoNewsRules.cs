#region Informations
// Auteur: B. Masson
// Date de création: 17/11/2004 
// Date de modification: 17/11/2004 
//	12/08/2005	A.Dadouch	Nom de fonctions	
#endregion

using System;
using System.Collections;
using System.Globalization;
using TNS.AdExpress.Web.DataAccess.Results;
using FrameWorkDate=TNS.FrameWork.Date;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpressWebRules=TNS.AdExpress.Web.Rules;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Rules.Results{
	/// <summary>
	/// Traitement des listes de fichiers pour infos/news 
	/// </summary>
	public class InfoNewsRules{
		
		#region Traitement
		/// <summary>
		/// Obtient le tableau de résultat d'infos/news
		/// </summary>
		/// <param name="name">Nom de la plaquette</param>
		/// <param name="webSession">Session</param>
		/// <returns>Tableau de résultat</returns>
		internal static string[,] GetData(string name, WebSession webSession){

			#region Variables
			int i=0;
			int nbItemsInTabData=0;
			int nbItems=0;
//			int end;
			string[] files=null;
			string[,] tabData=null;
			string pathDirectory="";
			string linkFile="";
			string fileName="";
			string linkTemp="";
			bool haveInserItem=false;
			#endregion

			try{

				#region Configuration
				switch(name){
					case WebConstantes.ModuleInfosNews.ADEXNEWS:
						//pathDirectory = AppDomain.CurrentDomain.BaseDirectory+@name;
						pathDirectory = WebConstantes.ModuleInfosNews.LOCAL_PATH_ADEXNEWS;
						nbItems = WebConstantes.ModuleInfosNews.NB_ELEMENTS_ADEXNEWS;
						linkFile = WebConstantes.ModuleInfosNews.LINK_ADEXNEWS;
						break;
					case WebConstantes.ModuleInfosNews.ADEXREPORT:
						//pathDirectory = AppDomain.CurrentDomain.BaseDirectory+@name;
						pathDirectory = WebConstantes.ModuleInfosNews.LOCAL_PATH_ADEXREPORT;
						nbItems = WebConstantes.ModuleInfosNews.NB_ELEMENTS_ADEXREPORT;
						linkFile = WebConstantes.ModuleInfosNews.LINK_ADEXREPORT;
						break;
					case WebConstantes.ModuleInfosNews.ADEXREVIEW:
						//pathDirectory = AppDomain.CurrentDomain.BaseDirectory+@name;
						pathDirectory = WebConstantes.ModuleInfosNews.LOCAL_PATH_ADEXREVIEW;
						nbItems = WebConstantes.ModuleInfosNews.NB_ELEMENTS_ADEXREVIEW;
						linkFile = WebConstantes.ModuleInfosNews.LINK_ADEXREVIEW;
						break;
					case WebConstantes.ModuleInfosNews.NOUVEAUTES:
						//pathDirectory = AppDomain.CurrentDomain.BaseDirectory+@name;
						pathDirectory = WebConstantes.ModuleInfosNews.LOCAL_PATH_NOUVEAUTES;
						nbItems = WebConstantes.ModuleInfosNews.NB_ELEMENTS_NOUVEAUTES;
						linkFile = WebConstantes.ModuleInfosNews.LINK_NOUVEAUTES;
						break;
				}
				#endregion

				#region Liste des fichiers
				files=InfoNewsDataAccess.GetData(name,pathDirectory);
				if(files.Length<1) throw (new WebExceptions.InfoNewsRulesException("Aucuns éléments dans le répertoire"));
				//Tri des fichiers par ordre alphabétique
				Array.Sort(files,Comparer.Default);
				#endregion
			
				#region Remplissage du tableau tabData
				if(nbItems.ToString()==null || nbItems==0 || files.Length<nbItems) nbItems = files.Length;
				i=files.Length-1;
//				end=
//
//				if(files.Length<nbItems)
//				else i=nbItems-1;
				

				tabData=new string[2,nbItems];
				while(nbItemsInTabData<nbItems && i>=0){
					fileName = files.GetValue(i).ToString();
					fileName = fileName.Substring(fileName.LastIndexOf(@"\")+1, (fileName.Length-fileName.LastIndexOf(@"\"))-1);
					linkTemp = linkFile+fileName;

					if(VerifCharacter(fileName)){

						//Colonne 0 : Nom du fichier - Colonne 1 : Chemin du fichier
						tabData.SetValue(FormatFileName(name,fileName,webSession),0,nbItemsInTabData);
						tabData.SetValue(linkTemp,1,nbItemsInTabData);
						haveInserItem=true;
						nbItemsInTabData++;
					}
					i--;
				}
				#endregion

			}
			catch(System.Exception e){
				throw (new WebExceptions.InfoNewsRulesException("Erreur dans la récupération des données",e));
			}
			if(!haveInserItem) throw (new WebExceptions.InfoNewsRulesException("Aucuns éléments correspondent aux critères"));
			return (tabData);
		}
		#endregion

		#region Méthode interne
		/// <summary>
		/// Fonction qui teste la présence d'un caractère spécifié dans une chaine
		/// </summary>
		/// <param name="name">Chaine à tester</param>
		private static bool VerifCharacter(string name){
			if(name==null)return(false);
			if(name.Length<1)return(false);
			if(name.IndexOf("_")<0)return(false);
			return(true);
		}


		/// <summary>
		/// Formatage du nom du fichier en libellé MOIS ANNEE
		/// </summary>
		/// <param name="name">Nom de la plaquette</param>
		/// <param name="fileName">Nom du fichier</param>
		/// <param name="webSession">Session</param>
		/// <returns>Libellé</returns>
		private static string FormatFileName(string name, string fileName, WebSession webSession){

			#region Variables
			int idMonth=0;
			string fileNameTemp="";
			string monthName="";
			string service="";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
			#endregion

			#region Formatage du nom du fichier
			idMonth		= int.Parse(fileName.Substring(fileName.LastIndexOf(@"_")+5,2));
			monthName	= FrameWorkDate.MonthString.GetCharacters(idMonth, cultureInfo, 0);
			fileNameTemp= monthName +" "+ fileName.Substring(fileName.LastIndexOf(@"_")+1,4);

			if(name==WebConstantes.ModuleInfosNews.NOUVEAUTES){
				//Formatage du nom du fichier pour nouveautés
				service = fileName.Substring(0,fileName.LastIndexOf(@"_"));
				fileNameTemp = service +" ("+ fileNameTemp +")";
			}
			#endregion

			return (fileNameTemp);
		}
		#endregion

	}
}
