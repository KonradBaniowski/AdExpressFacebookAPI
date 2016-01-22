using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Ares.Constantes {
    public class Constantes {

        #region Result
        public class Result {
            /// <summary>
            /// Lien pour récupérer les css
            /// </summary>
            //public const string CSS_LINK="www.tnsadexpress.com";
            //public const string CSS_LINK = "http://localhost:3525/App_Themes";
            /// <summary>
            /// indique le type de résultat à traiter
            /// </summary>
            public enum type {
                /// <summary>
                /// Type de résultat inconnu
                /// </summary>
                unknown = 0,
                /// <summary>
                /// Type APPM
                /// </summary>
                appm = 1,
                /// <summary>
                /// Type Bastet
                /// </summary>
                bastet = 2,
                /// <summary>
                /// Type alerte Geb (alerte portefeuille)
                /// </summary>
                gebAlert = 3,
                /// <summary>
                /// Type détail insertion APPM
                /// </summary>
                appmInsertionDetail = 4,
                /// <summary>
                /// Type APPM excel (plug-in Satet)
                /// </summary>
                appmExcel = 5,
                /// <summary>
                /// Type Indicateurs PDF (plug-in Hotep)
                /// </summary>
                hotep = 6,
                /// <summary>
                /// Type Alerte Plan Média PDF (plug-in Miysis)
                /// </summary>
                miysis = 7,
                /// <summary>
                /// Type APPM Plan Media PDF (plug-in Mnevis)
                /// </summary>
                mnevis = 8,
                /// <summary>
                /// Type Justificatifs Presse PDF (plug-in Shou)
                /// </summary>
                shou = 9,
                /// <summary>
                /// Type Données de Cadrage Excel (Plug-in Amset)
                /// </summary>
                amset = 10,
                /// <summary>
                /// Type Données de Cadrage PDF (Plug-in Aton)
                /// </summary>
                aton = 11,
                /// <summary>
                /// Type Indicateurs Press (Hermes)
                /// </summary>
                hermesPress = 12,
                /// <summary>
                /// Type Indicateurs Radio TV (Hermes)
                /// </summary>
                hermesRadioTV = 13,
                /// <summary>
                /// Alerte AdExpress
                /// </summary>
                alertAdExpress = 20,
                /// <summary>
                /// EasyMusic export Excel
                /// </summary>
                easyMusicExcel = 30,
                /// <summary>
                /// EasyMusic export Pdf
                /// </summary>
                easyMusicPdf = 31,
            }


            /// <summary>
            /// Statut de la génération du résultat
            /// </summary>
            public enum status {
                /// <summary>
                /// Nouvelle demande
                /// </summary>
                newOne = 0,
                /// <summary>
                /// En cours
                /// </summary>
                processing = 1,
                /// <summary>
                /// traité
                /// </summary>
                done = 2,
                /// <summary>
                /// envoyé
                /// </summary>
                sent = 3,
                /// <summary>
                /// Une erreur est survenue
                /// </summary>
                error = 4
            }
        #endregion
        }

        #region Classe des statuts et types des alertes

        public class Alerts {
            /// <summary>
            /// Defines the type of an alert
            /// </summary>
            public enum AlertType {
                Portfolio = 1,
                AdExpressAlert = 2
            }

            /// <summary>
            /// Defines the periodicity of an alert
            /// </summary>
            public enum AlertPeriodicity {
                Daily = 10,
                Weekly = 20,
                Monthly = 30
            }

            /// <summary>
            /// Defines the status of an alert
            /// </summary>
            public enum AlertStatuses {
                Activated = 0,
                New = 10,
                ToDelete = 50
            }
        }

        #endregion

    }
}
