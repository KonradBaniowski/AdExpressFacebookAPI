using KM.AdExpress.AudioVideoConverter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AudioVideoConverter.Tools
{
  

        public class ReportGenerator
        {

           


            public DateTime DateCreationBegin { get; set; }
            public DateTime DateCreationEnd { get; set; }

          
            public string Media { get; set; }

            public int NbAllFiles { get; set; }

          

            public int NbFilesNotConverted { get; set; }
            public int NbFilesSourceNotExisting { get; set; }
            public int NbFileConverted { get; set; }
            public int NbExistingDestinationFiles { get; set; }
            public int NbExistingMp4SourceFiles { get; set; }

            #region Constructeur
            /// <summary>
            /// Constructeur
            /// </summary>
            /// <param name="dateCreation"></param>
            /// <param name="dateMediaBegin"></param>
            /// <param name="dateMediaEnd"></param>
            /// <param name="media"></param>       
            public ReportGenerator(DateTime dateCreationBegin, DateTime dateCreationEnd,  string media)
            {
                DateCreationBegin = dateCreationBegin;
                DateCreationEnd = dateCreationEnd;               
                Media = media;

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
                t.AppendFormat("<td nowrap><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">La conversion des fichiers du média <strong>{0}</strong> est terminée.</font>&nbsp;&nbsp;</td>", Media);
                t.Append("</tr>");
                t.Append("<tr>");
                t.AppendFormat("<td nowrap><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">La conversion concerne les fichiers créés à partir du <strong>{0:dd/MM/yyyy}</strong> au <strong>{1:dd/MM/yyyy}</strong></font>&nbsp;&nbsp;<br/></td>", DateCreationBegin, DateCreationEnd);
                t.Append("</tr>");
               
                t.Append("<tr>");
                t.AppendFormat("<td nowrap><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Nombre de fichiers Initials : <strong>{0}</strong></font>&nbsp;&nbsp;<br/></td>", NbAllFiles);
                t.Append("</tr>");
                t.Append("<tr>");
                t.AppendFormat("<td nowrap><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Nombre de fichiers convertis : <strong>{0}</strong></font>&nbsp;&nbsp;<br/></td>", NbFileConverted);
                t.Append("</tr>");
                if (NbFilesNotConverted > 0)
                {
                    t.AppendFormat("<tr><td nowrap><font color=\"#FF0000\" size=\"2\" face=\"Arial\"><strong>{0}</strong> fichiers sources non pas pu être convertis. Veuillez  consulter le fichier de log qui repertorie les fichiers non converti.</font>&nbsp;&nbsp;<br/>", NbFilesNotConverted);                 
                    t.Append("</td>");
                    t.Append("</tr>");
                }
                if (NbFilesSourceNotExisting>0 )
                {
                    t.AppendFormat("<tr><td nowrap><font color=\"#FF0000\" size=\"2\" face=\"Arial\"> <strong>{0}</strong> fichiers sources  n'existent pas. Veuillez  consulter le fichier de log qui repertorie les fichiers sources manquants.</font>&nbsp;&nbsp;<br/>"
                        , NbFilesSourceNotExisting);                                 
                    t.Append("</td>");
                    t.Append("</tr>");
                }
                if (NbExistingDestinationFiles > 0)
                {
                    t.AppendFormat("<tr><td nowrap><font color=\"#008000\" size=\"2\" face=\"Arial\"> <strong>{0}</strong> fichiers existent déjà dans le dossier de destination. Veuillez  consulter le fichier de log qui repertorie les fichiers existants.</font>&nbsp;&nbsp;<br/>"
                        , NbExistingDestinationFiles);
                    t.Append("</td>");
                    t.Append("</tr>");
                }

                if (NbExistingMp4SourceFiles > 0)
                {
                    t.AppendFormat("<tr><td nowrap><font color=\"#008000\" size=\"2\" face=\"Arial\"> <strong>{0}</strong> fichiers MP4 existent déjà dans le dossier source. </font>&nbsp;&nbsp;<br/>"
                        , NbExistingMp4SourceFiles);
                    t.Append("</td>");
                    t.Append("</tr>");
                }
                
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

