using System;
using System.IO;
using System.Collections;
using System.Globalization;
using TNS.AdExpress.Web.Core.Sessions;

using FrameWorkDate = TNS.FrameWork.Date;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.AdExpressI.InfoNews;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Web;
namespace TNS.AdExpressI.InfosNews.France {	
		public class InfoNewsResult : TNS.AdExpressI.InfoNews.InfoNewsResult {

			#region Constructor
			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="session">User session</param>
			/// <param name="themeName">theme Name</param>
			public InfoNewsResult(WebSession session, string themeName)
				: base(session, themeName) {
			}
			#endregion

			#region Méthode interne
			/// <summary>
			/// Fonction qui teste la présence d'un caractère spécifié dans une chaine
			/// </summary>
			/// <param name="name">Chaine à tester</param>
			protected virtual bool VerifCharacter(string name) {
				if (name == null) return (false);
				if (name.Length < 1) return (false);
				if (name.IndexOf("_") < 0) return (false);
				return (true);
			}


			/// <summary>
			/// Formatage du nom du fichier en libellé MOIS ANNEE
			/// </summary>
			/// <param name="name">Nom de la plaquette</param>
			/// <param name="fileName">Nom du fichier</param>
			/// <param name="webSession">Session</param>
			/// <returns>Libellé</returns>
			protected  override string FormatFileName(WebCst.ModuleInfosNews.Directories idDirectory , string fileName) {

				#region Variables
				int idMonth = 0;
				string fileNameTemp = "";
				string monthName = "";
				string service = "";
				CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
				#endregion

				#region Formatage du nom du fichier
				idMonth = int.Parse(fileName.Substring(fileName.LastIndexOf(@"_") + 5, 2));
				monthName = FrameWorkDate.MonthString.GetCharacters(idMonth, cultureInfo, 0);
				fileNameTemp = monthName + " " + fileName.Substring(fileName.LastIndexOf(@"_") + 1, 4);

				if (idDirectory == WebCst.ModuleInfosNews.Directories.novelties) {
					//Formatage du nom du fichier pour nouveautés
					service = fileName.Substring(0, fileName.LastIndexOf(@"_"));
					fileNameTemp = service + " (" + fileNameTemp + ")";
				}
				#endregion

				return (fileNameTemp);
			}
			#endregion
		}	
}
