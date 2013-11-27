﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Date;
using KMI.AdExpress.Aphrodite.Domain;
using KMI.AdExpress.Aphrodite.Constantes;

namespace KMI.AdExpress.Aphrodite {
    /// <summary>
    /// 
    /// </summary>
    public class Period {

        #region Chargement de la liste des périodes
        /// <summary>
        /// Chargement de la liste des périodes
        /// </summary>
        /// <param name="vehicle">vehicle à traité</param>
        public static List<string> DownloadListPeriod(DateTime currentDay, Application.TreatmentType treatmentType) {
            if (treatmentType == Application.TreatmentType.month) {
                return DownLoadListMonthPeriod(currentDay);
            }
            else {
                return DownLoadListWeekPeriod(currentDay);
            }
        }
        #endregion

        #region Chargement des périodes mensuelles
        /// <summary>
        /// Chargement des périodes pour le cas des périodes mensuelles
        /// </summary>
        public static List<string> DownLoadListMonthPeriod(DateTime currentDay) {
            List<string> monthList=new List<string>();
            int i;
            DateTime dt = currentDay.AddMonths(-1);
            string mois_string;
            int mois=dt.Month;
            //int mois=2;
            string year_cur=dt.Year.ToString();
            string year_prev=(dt.Year-1).ToString();
            string date_lancement=mois.ToString();
            if(date_lancement.Length<2) date_lancement="0"+date_lancement;
            date_lancement=year_cur+date_lancement;


            for(i=1;i<=mois;i++) {
                mois_string=i.ToString();
                if(mois_string.Length<2) mois_string="0"+mois_string;
                monthList.Add(year_cur+mois_string);
            }

            for(i=mois;i<=12;i++) {
                mois_string=i.ToString();
                if(mois_string.Length<2) mois_string="0"+mois_string;
                monthList.Add(year_prev+mois_string);
            }
            return (monthList);
        }
        #endregion

        #region Chargement des périodes hebdommadaires
        /// <summary>
        /// Chargement des périodes pour le cas des périodes hebdommadaires
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        public static List<string> DownLoadListWeekPeriod(DateTime currentDay) {

            //Recherche de la semaine actuelle
            DateTime dt = DateTime.Now;
            List<string> weekList = new List<string>();

            TNS.FrameWork.Date.AtomicPeriodWeek week = new AtomicPeriodWeek(dt);
            TNS.FrameWork.Date.AtomicPeriodWeek weekPrev = new AtomicPeriodWeek(dt.AddYears(-1));
            week.SubWeek(1);
            weekPrev.SubWeek(1);

            int iSemaine = week.Week;
            string year = week.FirstDay.Year.ToString();
            string yearPrev = (week.FirstDay.Year - 1).ToString();
            string date_lancement = year + iSemaine.ToString();

            int i;
            for (i = 1; i <= iSemaine; i++) {
                if (i.ToString().Length < 2) {
                    weekList.Add(year + "0" + i.ToString());
                }
                else {
                    weekList.Add(year + i.ToString());
                }
            }
            // On complète avec les semaines de l'année précédente

            if (iSemaine == 1) {
                iSemaine = 2;
            }

            for (i = iSemaine - 1; i <= weekPrev.NumberWeekInYear; i++) {
                if (i.ToString().Length < 2) {
                    weekList.Add(yearPrev + "0" + i.ToString());
                }
                else {
                    weekList.Add(yearPrev + i.ToString());
                }
            }

            return weekList;
        }
        #endregion

    }
}
