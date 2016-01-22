using System;
using System.Text;

namespace KM.AdExpress.AlertPreRoll
{
    public class ReportGenerator
    {

        #region Variables
        /// <summary>
        ///Day
        /// </summary>
        private DateTime _day;
        /// <summary>
        /// file name
        /// </summary>
        private string _fileName;
      
        #endregion

        

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="day">Day</param>
        /// <param name="filename">File name</param>
        public ReportGenerator( DateTime day,string filename)
        {
            _day = day;
            _fileName = filename;
        }
        #endregion

        #region Méthodes public
        public string GetHtml()
        {

            StringBuilder t = new StringBuilder();

            t.Append("<table id=\"TableReport\" cellSpacing=\"1\" cellPadding=\"1\" border=\"0\">");
            t.Append("<tr>");
            t.Append("<td><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Bonjour,<br/></font></td>");
            t.Append("</tr>");
            t.Append("<tr>");
            t.Append("<td>&nbsp;</td>");
            t.Append("</tr>");
            t.Append("<tr>");
            t.AppendFormat("<td nowrap><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Veuillez trouver sur votre espace FTP le fichier  des Pré-Rolls Evaliant <strong>{0}</strong>.</font>&nbsp;&nbsp;</td>", _fileName);          
            t.Append("</tr>");
            t.Append("<tr>");
            t.AppendFormat("<td nowrap><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Ce fichier correspond à la journée du <strong>{0:dd/MM/yyyy}</strong></font>&nbsp;&nbsp;<br/></td>", _day);          
            t.Append("</tr>");
            t.Append("<tr>");
            t.Append("<td>&nbsp;</td>");
            t.Append("</tr>");
            t.Append("<tr>");
            t.Append("<td><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Cordialement,</font></td>");
            t.Append("</tr>");
            t.Append("<tr>");
            t.Append("<td nowrap><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\"><strong>Kantar Media, Ad Intelligence</strong></font>&nbsp;&nbsp;</td>");           
            t.Append("</tr>");
           
           
            t.Append("</table>");

            return t.ToString();
        }
        #endregion

    }
}
