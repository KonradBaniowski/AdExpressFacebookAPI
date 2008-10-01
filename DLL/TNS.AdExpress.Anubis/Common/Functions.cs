#region Information
/*
 * Author : G. RAGNEAU
 * Creation : 19/05/2005
 * Modifications :
 *		
 * */
#endregion


using System;
using System.IO;
using System.Text;

using TNS.AdExpress.Anubis.Exceptions;
using TNSAnubisConstantes = TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Common
{
	/// <summary>
	/// Usefull tools
	/// </summary>
	public class Functions {
		private static Random r = new Random(unchecked((int)DateTime.Now.Ticks));

		private static char[] characters = new char[47]{'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7','8','9','-','_','[',']','(',')','{','}','+','=','!'};

		#region Random String
		/// <summary>
		/// Generate a string with random size and random caracters authorized in a file name
		/// </summary>
		/// <param name="maxL">Minimal length of the string</param>
		/// <param name="minL">Maximal Length of the string (0 or less for no limit)</param>
		/// <returns>Random String</returns>
		public static string GetRandomString(int minL, int maxL){
			int max;
			r = new Random(unchecked((int)DateTime.Now.Ticks));
			if(maxL<=0)
				max = GetRandomIntegerAbove(minL);
			else
				max = GetRandomInteger(minL, maxL);
			StringBuilder str = new StringBuilder(max);
			for(int i = 0; i < max; i++){
				str.Append(characters[GetRandomInteger(0,characters.Length-1)]);
			}
			return str.ToString();
		}

		/// <summary>
		/// Determine a random integer between i and j included
		/// </summary>
		/// <param name="min">Minimal value</param>
		/// <param name="max">Maximal value (0 or less for no limit)</param>
		/// <returns>Random integer</returns>
		public static int GetRandomInteger(int min, int max){
			return r.Next(Math.Min(min, max), Math.Max(min, max));
		}

		/// <summary>
		/// Determine a random integer equal or superior to min
		/// </summary>
		/// <param name="min">Minimal value</param>
		/// <returns>Random number</returns>
		public static int GetRandomIntegerAbove(int min){
			return (r.Next()+ Math.Max(0,min));
		}

		/// <summary>
		/// Determine a random integer equal or inferior to max
		/// </summary>
		/// <param name="max">Maximal value</param>
		/// <returns>Random number</returns>
		public static int GetRandomIntegerUnder(int max){
			return (r.Next(Math.Max(0,max)));
		}
		#endregion

		#region Html File Management
		/// <summary>
		/// Create a html file with a complete header, ready to receive html code after body
		/// </summary>
		/// <param name="path">File path</param>
        /// <param name="webSession">Web Session</param>
		/// <returns>link so as to write text in the file</returns>
        public static StreamWriter GetHtmlFile(string path, WebSession webSession, string serverName) {
			try{
                string charSet = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Charset;
                string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
                themeName = "FinlandAdExpressUk";

				if (!File.Exists(path)){
					StreamWriter sw = File.CreateText(path);
					sw.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" >");
					sw.WriteLine("<HTML>");
					sw.WriteLine("<HEAD>");
                    sw.WriteLine("<META http-equiv=\"Content-Type\" content=\"text/html; charset=" + charSet + "\">");
					sw.WriteLine("<meta content=\"Microsoft Visual Studio .NET 7.1\" name=\"GENERATOR\">");
					sw.WriteLine("<meta content=\"C#\" name=\"CODE_LANGUAGE\">");
					sw.WriteLine("<meta content=\"JavaScript\" name=\"vs_defaultClientScript\">");
					sw.WriteLine("<meta content=\"http://schemas.microsoft.com/intellisense/ie5\" name=\"vs_targetSchema\">");
                    sw.WriteLine("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/AdExpress.css\" type=\"text/css\" rel=\"stylesheet\">");
                    sw.WriteLine("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/GenericUI.css\" type=\"text/css\" rel=\"stylesheet\">");
                    sw.WriteLine("<LINK href=\"" + serverName + "/App_Themes" + "/" + themeName + "/Css/MediaSchedule.css\" type=\"text/css\" rel=\"stylesheet\">");
					sw.WriteLine("<meta http-equiv=\"expires\" content=\"Wed, 23 Feb 1999 10:49:02 GMT\">");
					sw.WriteLine("<meta http-equiv=\"expires\" content=\"0\">");
					sw.WriteLine("<meta http-equiv=\"pragma\" content=\"no-cache\">");
					sw.WriteLine("<meta name=\"Cache-control\" content=\"no-cache\">");
					sw.WriteLine("</HEAD>");
					sw.WriteLine("<body>");
					sw.WriteLine("<form>");
					return sw;
				}
				else{
					throw new UnauthorizedAccessException("The file " + path + " already exists.");
				}
			}
			catch(System.Exception e){
				throw(new FunctionsException("Unable to create html file.",e));
			}
		}

		/// <summary>
		/// Append HTML terminaison tags (/form>/body>/HTML>) and close the file
		/// </summary>
		/// <param name="sw">StreamWriter on a html file</param>
		public static void CloseHtmlFile(StreamWriter sw){
			try{
				sw.WriteLine("</form></body></HTML>");
				sw.Close();
			}
			catch(System.Exception e){
				throw(new FunctionsException("Unable to close the html file stream.",e));
			}
		}
		#endregion

		#region CleanWorkDirectory
		/// <summary>
		/// Suppress a directory and its content
		/// </summary>
		/// <param name="path">Directory path to delete</param>
		public static void CleanWorkDirectory(string path){
			try{
				Directory.Delete(path,true);
			}
			catch(System.Exception e){
				throw(new FunctionsException("Unable to clean the working directory " + path + ".", e));
			}
		}
		#endregion

	}
}
