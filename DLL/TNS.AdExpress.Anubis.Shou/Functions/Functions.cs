using System;
using System.Collections.Generic;
using System.Text;

using TNS.AdExpress.Anubis.Shou.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using System.IO;

namespace TNS.AdExpress.Anubis.Shou.Functions
{
    /// <summary>
    /// Description résumée de Functions.
    /// </summary>
    public class Functions
    {

        #region Html File Management
        /// <summary>
        /// Create a html file with a complete header, ready to receive html code after body
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>link so as to write text in the file</returns>
        public static StreamWriter GetHtmlFile(string path, WebSession webSession, string serverName)
        {
            try
            {
                string charSet = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Charset;
                string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

                if (!File.Exists(path))
                {
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
                else
                {
                    throw new UnauthorizedAccessException("The file " + path + " already exists.");
                }
            }
            catch (System.Exception e)
            {
                throw (new ShouPdfException("Unable to create html file.", e));
            }
        }

        /// <summary>
        /// Append HTML terminaison tags (/form>/body>/HTML>) and close the file
        /// </summary>
        /// <param name="sw">StreamWriter on a html file</param>
        public static void CloseHtmlFile(StreamWriter sw)
        {
            try
            {
                sw.WriteLine("</form></body></HTML>");
                sw.Close();
            }
            catch (System.Exception e)
            {
                throw (new ShouPdfException("Unable to close the html file stream.", e));
            }
        }
        #endregion

    }
}
