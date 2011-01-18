using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;

using System.IO;
using System.Data;
using ImhotepConsole.DAL;
using ImhotepConsole.Core.Sessions;
using ImhotepConsole.Rules;
//using TNS.AdExpress.Web.Rules.Global.Customer;
using System.Windows.Forms;
using System.Collections;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
//using TNS.AdExpress.Web.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Classification;

namespace ImhotepConsole
{
    public class Functions
    {

        #region Copies des valeurs de NewWebSession vers WebSession de l'assembly TNS.WEB.SESSION.CORE
        /// <summary>
        /// Copie les valeurs de NewWebSession (ancienne version de session)
        /// vers WebSession (nouvelle version)situé dans l'asembly TNS.WEB.SESSION.CORE
        /// </summary>
        /// <param name="newWebSession">nouvelle session</param>
        /// <param name="webSession">session définitive</param>
        /// <returns>session définitive</returns>
        public static WebSession SetCoreWebSessionValues(NewWebSession newWebSession, WebSession webSession)
        {
            #region Initialisation des données
            //module en cours
            try
            {
                webSession.CurrentModule = (Int64)newWebSession.CurrentModule;
            }
            catch (Exception) { webSession.CurrentModule = 0; }
            //onglet en cours
            try
            {
                webSession.CurrentTab = newWebSession.CurrentTab;
            }
            catch (Exception) { webSession.CurrentTab = 0; }
            //Code de traduction du module
            try
            {
                webSession.ModuleTraductionCode = (int)newWebSession.ModuleTraductionCode;
            }
            catch (Exception) { webSession.ModuleTraductionCode = 0; }
            //Module cible
            try
            {
                webSession.ReachedModule = newWebSession.ReachedModule;
            }
            catch (Exception) { webSession.ReachedModule = false; }

            #region Univers et sélection des éléments de la nomenclature
            //Univers Média courant
            try
            {
                //webSession.CurrentUniversMedia = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.CurrentUniversMedia);
                webSession.CurrentUniversMedia = newWebSession.CurrentUniversMedia;
            }
            catch (Exception) { webSession.CurrentUniversMedia = new TreeNode("media"); }
            //Univers annonceur courant
            try
            {
                //webSession.CurrentUniversAdvertiser = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.CurrentUniversAdvertiser);
                webSession.CurrentUniversAdvertiser = newWebSession.CurrentUniversAdvertiser;
            }
            catch (Exception) { webSession.CurrentUniversAdvertiser = new TreeNode("advertiser"); }
            //Univers produit courant
            try
            {
                //webSession.CurrentUniversProduct = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.CurrentUniversProduct);
                webSession.CurrentUniversProduct = newWebSession.CurrentUniversProduct;
            }
            catch (Exception) { webSession.CurrentUniversProduct = new TreeNode("produit"); }
            //Annonceurs sélectionnés
            try
            {
                //webSession.SelectionUniversAdvertiser = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.SelectionUniversAdvertiser);
                webSession.SelectionUniversAdvertiser = newWebSession.SelectionUniversAdvertiser;
            }
            catch (Exception) { webSession.SelectionUniversAdvertiser = new TreeNode("advertiser"); }
            //Média Sélectionné
            try
            {
                //webSession.SelectionUniversMedia = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.SelectionUniversMedia);
                webSession.SelectionUniversMedia = newWebSession.SelectionUniversMedia;
            }
            catch (Exception) { webSession.SelectionUniversMedia = new TreeNode("media"); }
            //Produits sélectionnés
            try
            {
                //webSession.SelectionUniversProduct = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.SelectionUniversProduct);
                webSession.SelectionUniversProduct = newWebSession.SelectionUniversProduct;
            }
            catch (Exception) { webSession.SelectionUniversProduct = new TreeNode("produit"); }
            //Annonceurs de références
            try
            {
                //webSession.ReferenceUniversAdvertiser = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.ReferenceUniversAdvertiser);
                webSession.ReferenceUniversAdvertiser = newWebSession.ReferenceUniversAdvertiser;
            }
            catch (Exception) { webSession.ReferenceUniversAdvertiser = new TreeNode("advertiser"); }
            //Média de références
            try
            {
                //webSession.ReferenceUniversMedia = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.ReferenceUniversMedia);
                webSession.ReferenceUniversMedia = newWebSession.ReferenceUniversMedia;
            }
            catch (Exception) { webSession.ReferenceUniversMedia = new TreeNode("media"); }
            //Produits de références
            try
            {
                //webSession.ReferenceUniversProduct = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.ReferenceUniversProduct);
                webSession.ReferenceUniversProduct = newWebSession.ReferenceUniversProduct;
            }
            catch (Exception) { webSession.ReferenceUniversProduct = new TreeNode("produit"); }





            //Annonceurs concurrents
            try
            {
                webSession.CompetitorUniversAdvertiser = newWebSession.CompetitorUniversAdvertiser;               
            }
            catch (Exception) { webSession.CompetitorUniversAdvertiser = new Hashtable(5); }
            //Média concurrents
            try
            {
                webSession.CompetitorUniversMedia = newWebSession.CompetitorUniversMedia;
               
            }
            catch (Exception) { webSession.CompetitorUniversMedia = new Hashtable(5); }
            //Produits concurrents
            try
            {
                webSession.CompetitorUniversProduct = newWebSession.CompetitorUniversProduct;
            }
            catch (Exception) { webSession.CompetitorUniversProduct = new Hashtable(5); }
            #endregion






            //Arbre temporaire
            try
            {
                //webSession.TemporaryTreenode = ImhotepRules.ConvertTmpTreeToNew(newWebSession.TemporaryTreenode);
                webSession.TemporaryTreenode = newWebSession.TemporaryTreenode;
            }
            catch (Exception) { webSession.TemporaryTreenode = null; }

            #region Période
            //Durée de la période
            try
            {
                webSession.PeriodLength = newWebSession.PeriodLength;
            }
            catch (Exception) { webSession.PeriodLength = 0; }
            //Date de début
            try
            {
                webSession.PeriodBeginningDate = newWebSession.PeriodBeginningDate;
            }
            catch (Exception) { webSession.PeriodBeginningDate = ""; }

            //Date de fin 
            try
            {
                webSession.PeriodEndDate = newWebSession.PeriodEndDate;
            }
            catch (Exception) { webSession.PeriodEndDate = ""; }

            //Début de la période
            try
            {
                webSession.BeginningDate = newWebSession.BeginningDate;
            }
            catch (Exception) { webSession.BeginningDate = DateTime.Now; }
            //Date de chargement des données
            try
            {
                webSession.DownLoadDate = newWebSession.DownLoadDate;
            }
            catch (Exception) { webSession.DownLoadDate = 0; }
            //Type de la période
            try
            {
                webSession.PeriodType = newWebSession.PeriodType;
            }
            catch (Exception) { webSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth; }
            //Détail la période
            try
            {
                webSession.DetailPeriod = newWebSession.DetailPeriod;
            }
            catch (Exception) { webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly; }
            //Date de modification	
            try
            {
                webSession.ModificationDate = newWebSession.ModificationDate;
            }
            catch (Exception) { webSession.ModificationDate = DateTime.Now; }
            //Détail Période préformaté
            try
            {
                webSession.PreformatedPeriodDetail = newWebSession.PreformatedPeriodDetail;
            }
            catch (Exception) { webSession.PreformatedPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedPeriodDetails.monthly_And_Total; }
            #endregion

            //Encarts
            try
            {
                webSession.Insert = (TNS.AdExpress.Constantes.Web.CustomerSessions.Insert)newWebSession.Insert;
            }
            catch (Exception) { webSession.Insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total; }
            //Affiche graphique
            try
            {
                webSession.Graphics = newWebSession.Graphics;
            }
            catch (Exception) { webSession.Graphics = true; }
            //Unités
            try
            {
                webSession.Unit = (CstWeb.CustomerSessions.Unit)newWebSession.Unit;
            }
            catch (Exception) { webSession.Unit = CstWeb.CustomerSessions.Unit.euro; }

            #region élément préformatés
            //Détail produit préformaté
            try
            {
                webSession.PreformatedProductDetail = (CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails)newWebSession.PreformatedProductDetail;
            }
            catch (Exception) { webSession.PreformatedProductDetail = CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup; }
            //Détail Média préformaté
            try
            {
                webSession.PreformatedMediaDetail = newWebSession.PreformatedMediaDetail;
            }
            catch (Exception) { webSession.PreformatedMediaDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle; }
            //Tableau préformaté
            try
            {
                webSession.PreformatedTable = newWebSession.PreformatedTable;
            }
            catch (Exception) { webSession.PreformatedTable = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.media_X_Year; }
            #endregion

            //Dernière page visitée
            try
            {
                webSession.LastReachedResultUrl = newWebSession.LastReachedResultUrl;
            }
            catch (Exception) { webSession.LastReachedResultUrl = ""; }
            //Pourcentage
            try
            {
                webSession.Percentage = newWebSession.Percentage;
            }
            catch (Exception) { webSession.Percentage = false; }

            //Niveau de détail produit
            try
            {
                //webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(newWebSession.ProductDetailLevel.LevelProduct, ImhotepRules.ConvertTmpTreeToNew(newWebSession.ProductDetailLevel.ListElement));
                webSession.ProductDetailLevel = newWebSession.ProductDetailLevel;
            }
            catch (Exception) { webSession.ProductDetailLevel = null; }
            //Niveau de détail média
            try
            {
                //webSession.MediaDetailLevel = new TNS.AdExpress.Web.Core.Sessions.MediaLevelSelection(newWebSession.MediaDetailLevel.LevelMedia, ImhotepRules.ConvertTmpTreeToNew(newWebSession.MediaDetailLevel.ListElement));
                webSession.MediaDetailLevel = newWebSession.MediaDetailLevel;
            }
            catch (Exception) { webSession.MediaDetailLevel = null; }

            //Critère de comparaison
            try
            {
                webSession.ComparaisonCriterion = newWebSession.ComparaisonCriterion;
            }
            catch (Exception) { webSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal; }
            //Etudes comparatives
            try
            {
                webSession.ComparativeStudy = newWebSession.ComparativeStudy;
            }
            catch (Exception) { webSession.ComparativeStudy = false; }
            //login
            try
            {
                webSession.CustomerLogin = newWebSession.CustomerLogin;
            }
            catch (Exception) { }
            //personnalisation des éléménts de référence
            try
            {
                webSession.CustomizedReferenceComcurrentElements = newWebSession.CustomizedReferenceComcurrentElements;
            }
            catch (Exception) { webSession.CustomizedReferenceComcurrentElements = false; }
            //Evolution
            try
            {
                webSession.Evolution = newWebSession.Evolution;
            }
            catch (Exception) { webSession.Evolution = true; }
            //ID Session	
            try
            {
                webSession.IdSession = newWebSession.IdSession;
            }
            catch (Exception) { }
            //Dernier mois disponible pour les recaps		
            try
            {
                webSession.LastAvailableRecapMonth = newWebSession.LastAvailableRecapMonth;
            }
            catch (Exception) { webSession.LastAvailableRecapMonth = ""; }
            //Nouveau produit	
            try
            {
                webSession.NewProduct = newWebSession.NewProduct;
            }
            catch (Exception) { webSession.NewProduct = TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support; }
            //PDM	
            try
            {
                webSession.PDM = newWebSession.PDM;
            }
            catch (Exception) { webSession.PDM = false; }
            //PDV	
            try
            {
                webSession.PDV = newWebSession.PDV;
            }
            catch (Exception) { webSession.PDV = false; }
            //Personnalisé élément
            try
            {
                webSession.PersonalizedElementsOnly = newWebSession.PersonalizedElementsOnly;
            }
            catch (Exception) { webSession.PersonalizedElementsOnly = false; }
            //Utilisation de l'auto-promo Evaliant
            try
            {
                webSession.AutopromoEvaliant = newWebSession.AutopromoEvaliant;
            }
            catch (Exception) { webSession.AutopromoEvaliant = false; }


            //language du site
            try
            {
                webSession.SiteLanguage = newWebSession.SiteLanguage;
            }
            catch (Exception) { webSession.SiteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH; }

           

            #region éléments rajoutés pour migration 03/05/2005

            #region Général
            // Nom du fichier PDF exporté
            try
            {
                webSession.ExportedPDFFileName = newWebSession.ExportedPDFFileName;
            }
            catch (Exception)
            {
                webSession.ExportedPDFFileName = "";
            }


            // Email des destinataires du fichier PDF à exporter
            try
            {
                webSession.EmailRecipient = newWebSession.EmailRecipient;
            }
            catch (Exception)
            {
                webSession.EmailRecipient = null;
            }
            #endregion

            #region période
            //Date de début du détail de la période
            try
            {
                webSession.DetailPeriodBeginningDate = newWebSession.DetailPeriodBeginningDate;
            }
            catch (Exception) { webSession.DetailPeriodBeginningDate = ""; }
            //Date de fin du détail de la période
            try
            {
                webSession.DetailPeriodEndDate = newWebSession.DetailPeriodEndDate;
            }
            catch (Exception) { webSession.DetailPeriodEndDate = ""; }

            // Date de début de publication
            try
            {
                webSession.PublicationBeginningDate = newWebSession.PublicationBeginningDate;
            }
            catch (Exception) { webSession.PublicationBeginningDate = ""; }
            // Date de fin de publication
            try
            {
                webSession.PublicationEndDate = newWebSession.PublicationEndDate;
            }
            catch (Exception) { webSession.PublicationEndDate = ""; }
            // Année du fichier d'agence média
            try
            {
                webSession.MediaAgencyFileYear = newWebSession.MediaAgencyFileYear;
            }
            catch (Exception) { webSession.MediaAgencyFileYear = ""; }
            // Identifiant de type de date de parution
            try
            {
                webSession.PublicationDateType = newWebSession.PublicationDateType;
            }
            catch (Exception) { webSession.PublicationDateType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.publicationType.staticDate; }

            #endregion

            #region univers étude bilan de campagne (APPM)
            //Ecart
            try
            {
                webSession.Ecart = newWebSession.Ecart;
            }
            catch (Exception)
            {
                webSession.Ecart = false;
            }

            //Sélection courante des cibles AEPM de l'univers courant
            try
            {
                //webSession.CurrentUniversAEPMTarget = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.CurrentUniversAEPMTarget);
                webSession.CurrentUniversAEPMTarget = newWebSession.CurrentUniversAEPMTarget;
            }
            catch (Exception) { webSession.CurrentUniversAEPMTarget = new TreeNode("target"); }

            //Sélection originale des cibles AEPM de l'univers courant
            try
            {
                //webSession.SelectionUniversAEPMTarget = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.SelectionUniversAEPMTarget);
                webSession.SelectionUniversAEPMTarget = newWebSession.SelectionUniversAEPMTarget;
            }
            catch (Exception) { webSession.SelectionUniversAEPMTarget = new TreeNode("target"); }

            //  Sélection originale des vagues AEPM de l'univers courant
            try
            {
                //webSession.SelectionUniversAEPMWave = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.SelectionUniversAEPMWave);
                webSession.SelectionUniversAEPMWave = newWebSession.SelectionUniversAEPMWave;
            }
            catch (Exception) { webSession.SelectionUniversAEPMWave = new TreeNode("wave"); }

            // Sélection courante des vagues AEPM de l'univers courant
            try
            {
                //webSession.CurrentUniversAEPMWave = (TreeNode)ImhotepRules.ConvertTmpTreeToNew(newWebSession.CurrentUniversAEPMWave);
                webSession.SelectionUniversAEPMWave = newWebSession.CurrentUniversAEPMWave;
            }
            catch (Exception) { webSession.CurrentUniversAEPMWave = new TreeNode("wave"); }

            // Sélection courante des vagues OJD de l'univers courant
            try
            {
                webSession.CurrentUniversOJDWave = newWebSession.CurrentUniversOJDWave;
            }
            catch (Exception) { webSession.CurrentUniversOJDWave = new TreeNode("wave"); }

            // Sélection originale des vagues OJD de l'univers courant
            try
            {
                webSession.SelectionUniversOJDWave = newWebSession.SelectionUniversOJDWave;
            }
            catch (Exception) { webSession.SelectionUniversOJDWave = new TreeNode("wave"); }
            #endregion

            #region AdExpress pricnicapl and secondary universes
            // Principal dictionary of  universe product selection           
            try
            {
                webSession.PrincipalProductUniverses = newWebSession.PrincipalProductUniverses;
            }
            catch (Exception)
            {
                webSession.PrincipalProductUniverses = new Dictionary<int, AdExpressUniverse>();
            }

            // Secondary dictionary of universe product selection           
            try
            {
                webSession.SecondaryProductUniverses = newWebSession.SecondaryProductUniverses;
            }
            catch (Exception)
            {
                webSession.SecondaryProductUniverses = new Dictionary<int, AdExpressUniverse>();
            }
            // Principal dictionary of  universe media selection
            try
            {
                webSession.PrincipalMediaUniverses = newWebSession.PrincipalMediaUniverses;
            }
            catch (Exception)
            {
                webSession.PrincipalMediaUniverses = new Dictionary<int, AdExpressUniverse>();
            }

            // Secondary dictionary of universe media selection
            try
            {
                webSession.SecondaryMediaUniverses = newWebSession.SecondaryMediaUniverses;
            }
            catch (Exception)
            {
                webSession.SecondaryMediaUniverses = new Dictionary<int, AdExpressUniverse>();
            }
            #endregion

            #region variables de Répartition
            //  format
            try
            {
                webSession.Format = newWebSession.Format;
            }
            catch (Exception) { webSession.Format = CstWeb.Repartition.Format.Total; }

            // Tranche horaire
            try
            {
                webSession.TimeInterval = newWebSession.TimeInterval;
            }
            catch (Exception) { webSession.TimeInterval = CstWeb.Repartition.timeInterval.Total; }

            //  Jour nommé
            try
            {
                webSession.NamedDay = newWebSession.NamedDay;
            }
            catch (Exception) { webSession.NamedDay = CstWeb.Repartition.namedDay.Total; }

            #endregion


            #endregion

            #region éléments rajoutés pour migration 29/12/2005

            //Liste des visuels
            try
            {
                webSession.Visuals = newWebSession.Visuals;
            }
            catch (Exception) { webSession.Visuals = null; }

            //Contient les nouvelles variables sessions
            try
            {
                webSession.UserParameters = (Hashtable)newWebSession.UserParameters;
            }
            catch (Exception) { webSession.UserParameters = new Hashtable(); }

            //Liste destinée à contenir les identifiants accroches de personnalisation dans les plans médias
            try
            {
                webSession.IdSlogans = newWebSession.IdSlogans;
            }
            catch (Exception) { webSession.IdSlogans = null; }

            //Hashtable contenant une liste d'accroches en clés et des couleurs associées en valeurs
            try
            {
                webSession.SloganColors = newWebSession.SloganColors;
            }
            catch (Exception) { webSession.SloganColors = null; }

            ////Agences media
           webSession.PrincipalAdvertisingAgnecyUniverses = new Dictionary<int, AdExpressUniverse>();
            webSession.SecondaryAdvertisingAgnecyUniverses = new Dictionary<int, AdExpressUniverse>();
            #endregion
            #endregion

            return webSession;
        }
        #endregion
    }
}
