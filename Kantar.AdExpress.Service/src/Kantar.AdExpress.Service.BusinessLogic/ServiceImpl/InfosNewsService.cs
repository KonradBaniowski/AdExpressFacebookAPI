﻿using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using DomainResults = TNS.AdExpress.Domain.Results;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class InfosNewsService : IInfosNewsService
    {
        private WebSession _customerSession { get; set; }

        public List<Documents> GetInfosNews(string idWebSession)
        {
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            DomainResults.InfoNews infoNewsInformations = WebApplicationParameters.InfoNewsInformations;
            List<DomainResults.InfoNewsItem> sortedInfoNewsItems = (infoNewsInformations != null) ? infoNewsInformations.GetSortedInfoNewsItems() : null;
            string[] files = null;
            List<Documents> documents = new List<Documents>();
            int nbFilesAdded = 0;

            if (sortedInfoNewsItems != null && sortedInfoNewsItems.Count > 0)
            {
                //For each directory
                for (int i = 0; i < sortedInfoNewsItems.Count; i++)
                {
                    files = GetDirectoryFiles(sortedInfoNewsItems[i].PhysicalPath);
                    if (files != null && files.Length > 0)
                    {
                        //Siort files by alpabetical label
                        Array.Sort(files, Comparer.Default);

                        List<InfosNews> infosNews = new List<InfosNews>();

                        //Show all directoy's files						
                        for (int j = files.Length - 1; j >= 0; j--)
                        {
                            if (Path.GetExtension(files[j].ToString()).ToUpper().Equals(".DB")) continue; ;
                            //Limitation of Nb files to show							
                            if (sortedInfoNewsItems[i].NbMaxItemsToShow > -1 && nbFilesAdded == sortedInfoNewsItems[i].NbMaxItemsToShow) break;

                            infosNews.Add(new InfosNews { Label = FormatFileName(sortedInfoNewsItems[i].Id, Path.GetFileNameWithoutExtension(files[j].ToString())), Url = sortedInfoNewsItems[i].VirtualPath + Path.GetFileName(files[j].ToString()) });

                            nbFilesAdded++;
                        }
                        nbFilesAdded = 0;
                        documents.Add(new Documents { Id = sortedInfoNewsItems[i].Id.GetHashCode(), Label = GestionWeb.GetWebWord(sortedInfoNewsItems[i].WebTextId, _customerSession.SiteLanguage), InfosNews = infosNews });
                    }

                }
            }

            return documents;
        }

        private string[] GetDirectoryFiles(string pathDirectory)
        {
            string[] filesList = Directory.GetFiles(pathDirectory);
            return (filesList);
        }

        private string FormatFileName(ModuleInfosNews.Directories idDirectory, string fileName)
        {

            #region Variables
            int idMonth = 0;
            string fileNameTemp = "";
            string monthName = "";
            string service = "";
            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_customerSession.SiteLanguage].Localization);
            #endregion

            #region Formatage du nom du fichier
            idMonth = int.Parse(fileName.Substring(fileName.LastIndexOf(@"_") + 5, 2));
            monthName = TNS.FrameWork.Date.MonthString.GetCharacters(idMonth, cultureInfo, 0);
            fileNameTemp = monthName + " " + fileName.Substring(fileName.LastIndexOf(@"_") + 1, 4);

            if (idDirectory == ModuleInfosNews.Directories.novelties)
            {
                //Formatage du nom du fichier pour nouveautés
                service = fileName.Substring(0, fileName.LastIndexOf(@"_"));
                fileNameTemp = service + " (" + fileNameTemp + ")";
            }
            #endregion

            return (fileNameTemp);
        }
    }
}