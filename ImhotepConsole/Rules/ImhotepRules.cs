using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ImhotepConsole.DAL;

using TNS.AdExpress.Web.Core.Sessions;
using ImhotepConsole.Core.Sessions;
using System.Windows.Forms;
using System.Collections;
using TNS.AdExpress.Classification;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;

namespace ImhotepConsole.Rules
{
    public class ImhotepRules
    {
        #region Conversion des WebSession vers NewWebSession
        /// <summary>
        /// Convertit les anciennes sessions (WebSession) en NewWebSession (nouvelle version)
        /// : WebSession --> NewWebSession
        /// </summary>
        public static bool ConvertAllWebSessionToNewWebSession()
        {

            #region Variables
            WebSession webSession = null;
            NewWebSession newbSession = null;
            bool success = false;
            #endregion

            #region Chargement de WebSession et conversion en NewWebSession

            //Chargement des Ids Session
            DataSet Ids = ImhotepDAL.LoadSessionIds();

            //Chargement de WebSession et conversion en NewWebSession
            if (Ids.Tables[0].Rows.Count > 0)
            {
                try
                {
                    Console.WriteLine("DEBUT CONVERSION des WebSession en NewWebSession.");
                    foreach (DataRow dr in Ids.Tables[0].Rows)
                    {
                        try
                        {
                            webSession = (WebSession)ImhotepDAL.LoadMySession(dr["id_my_session"].ToString().Trim());
                            //création de NewWebsession
                            newbSession = ConvertWebSessionToNewWebSession(webSession);
                            //Enregistrement de NewWebsession en base
                            ImhotepDAL.UpdateSession(dr["id_my_session"].ToString().Trim(), newbSession);
                            Console.WriteLine("CONVERSION de la WebSession " + dr["id_my_session"].ToString() + " - " + dr["my_session"].ToString() + " en NewWebSession.");
                        }
                        catch (System.Exception e)
                        {

                            Console.WriteLine("ERROR : Impossible de convertir la session " + dr["id_my_session"].ToString() + " NewWebsession=>WebSession : " + e.Message + "\n" + e.StackTrace);
                        }

                    }
                    Console.WriteLine("FIN CONVERSION des WebSession en NewWebSession.");
                }
                catch (System.Exception e)
                {

                    Console.WriteLine("ERROR : Impossible de convertir Websession=>NewWebSession : " + e.Message + "\n" + e.StackTrace);
                }
            }
            #endregion
            Console.ReadLine();
            #region Local Test
            //try
            //{
            //    webSession = (WebSession)ImhotepDAL.LoadMySession("821");
            //    ////création de NewWebsession
            //    newbSession = ConvertWebSessionToNewWebSession(webSession);
            //    ////Enregistrement de NewWebsession en base
            //    ImhotepDAL.UpdateSession("821", newbSession);
            //    Console.WriteLine("Conversion de la WebSession " + "821" + " - Test Dédé à supprimer en NewWebSession.");
            //}
            //catch (System.Exception e)
            //{
            //    Console.WriteLine("ERROR : Impossible de conversion NewWebsession=>WebSession : " + e.Message + "\n" + e.StackTrace);
            //}
            #endregion


            success = true;
            return success;


        }
        #endregion

        #region Conversion des Alerts WebSession vers NewWebSession
        /// <summary>
        /// Convertit les anciennes sessions (WebSession) en NewWebSession (nouvelle version)
        /// : WebSession --> NewWebSession
        /// </summary>
        public static bool ConvertAllAlertWebSessionToNewWebSession()
        {

            #region Variables
            WebSession webSession = null;
            NewWebSession newbSession = null;
            bool success = false;
            #endregion

            #region Chargement de WebSession et conversion en NewWebSession

            //Chargement des Ids Session
            DataSet Ids = ImhotepDAL.LoadSessionAlertIds();

            //Chargement de WebSession et conversion en NewWebSession
            if (Ids.Tables[0].Rows.Count > 0)
            {
                try
                {
                    Console.WriteLine("DEBUT CONVERSION des WebSession en NewWebSession.");
                    foreach (DataRow dr in Ids.Tables[0].Rows)
                    {
                        try
                        {
                            webSession = (WebSession)ImhotepDAL.LoadMySessionAlert(dr["id_alert"].ToString().Trim());
                            //création de NewWebsession
                            newbSession = ConvertWebSessionToNewWebSession(webSession);
                            //Enregistrement de NewWebsession en base
                            ImhotepDAL.UpdateSessionAlert(dr["id_alert"].ToString().Trim(), newbSession);
                            Console.WriteLine("CONVERSION de la WebSession " + dr["id_alert"].ToString() + " - " + dr["alert"].ToString() + " en NewWebSession.");
                        }
                        catch (System.Exception e)
                        {

                            Console.WriteLine("ERROR : Impossible de convertir la session " + dr["id_alert"].ToString() + " NewWebsession=>WebSession : " + e.Message + "\n" + e.StackTrace);
                        }

                    }
                    Console.WriteLine("FIN CONVERSION des WebSession en NewWebSession.");
                }
                catch (System.Exception e)
                {

                    Console.WriteLine("ERROR : Impossible de convertir Websession=>NewWebSession : " + e.Message + "\n" + e.StackTrace);
                }
                Console.ReadLine();
            }
            #endregion

            #region Local Test
            //try
            //{
            //    webSession = (WebSession)ImhotepDAL.LoadMySessionAlert("618");
            //    ////création de NewWebsession
            //    newbSession = ConvertWebSessionToNewWebSession(webSession);
            //    ////Enregistrement de NewWebsession en base
            //    //ImhotepDAL.UpdateSessionAlert("618", newbSession);
            //    Console.WriteLine("Conversion de la WebSession " + "618" + " - Test Dédé à supprimer en NewWebSession.");
            //}
            //catch (System.Exception e)
            //{
            //    Console.WriteLine("ERROR : Impossible de conversion NewWebsession=>WebSession : " + e.Message + "\n" + e.StackTrace);
            //}
            #endregion


            success = true;
            return success;


        }
        #endregion

        #region Conversion des NewWebSession vers WebSession
        /// <summary>
        /// Convertit les  sessions nouvellement crées (NewWebSession) en ancienne session modifiées (WebSession)
        /// : NewWebSession --> WebSession
        /// </summary>
        public static bool ConvertAllNewWebSessionToWebSession()
        {
            #region Variables
            WebSession webSession = null;
            NewWebSession newWebSession = null;
            bool success = false;
            #endregion

            //Chargement des Ids Session
            DataSet Ids = ImhotepDAL.LoadSessionIds();
            if (Ids.Tables[0].Rows.Count > 0)
            {
                Console.WriteLine("DEBUT CONVERSION des NewWebSession en WebSession.");
                try
                {
                    //Chargement de NewWebSession et conversion en WebSession
                    foreach (DataRow dr in Ids.Tables[0].Rows)
                    {
                        try
                        {
                            newWebSession = (NewWebSession)ImhotepDAL.LoadMySession(dr["id_my_session"].ToString().Trim());
                            //création de NewWebsession
                            webSession = ConvertNewWebSessionToWebSession(newWebSession);
                            //Enregistrement de Websession en base
                            ImhotepDAL.UpdateSession(dr["id_my_session"].ToString().Trim(), webSession);
                            Console.WriteLine("CONVERSION de la NewWebSession " + dr["id_my_session"].ToString() + "-" + dr["my_session"].ToString() + " en WebSession.");
                        }
                        catch (System.Exception e)
                        {

                            Console.WriteLine("ERROR : Impossible de convertir la NewWebsession " + dr["id_my_session"].ToString() + " en Websession : " + e.Message + "\n" + e.StackTrace);
                        }
                    }
                }
                catch (System.Exception e)
                {

                    Console.WriteLine("ERROR : Impossible de convertir NewWebsession=>WebSession : " + e.Message + "\n" + e.StackTrace);
                }
                Console.WriteLine("FIN CONVERSION des NewWebSession en WebSession.");
            }

            Console.ReadLine();
            #region Local Test
            //newWebSession = (NewWebSession)ImhotepDAL.LoadMySession("821");
            //webSession = ConvertNewWebSessionToWebSession(newWebSession);
            //ImhotepDAL.UpdateSession("821", webSession);
            //webSession = (WebSession)ImhotepDAL.LoadMySession("821");
           
            #endregion
            success = true;
            return success;
        }

        #endregion

        #region Conversion des Alert NewWebSession vers WebSession
        /// <summary>
        /// Convertit les  sessions alerts nouvellement crées (NewWebSession) en ancienne session modifiées (WebSession)
        /// : NewWebSession --> WebSession
        /// </summary>
        public static bool ConvertAllAlertNewWebSessionToWebSession()
        {
            #region Variables
            WebSession webSession = null;
            NewWebSession newWebSession = null;
            bool success = false;
            #endregion

            //Chargement des Ids Session
            DataSet Ids = ImhotepDAL.LoadSessionAlertIds();
            if (Ids.Tables[0].Rows.Count > 0)
            {
                Console.WriteLine("DEBUT CONVERSION des NewWebSession en WebSession.");
                try
                {
                    //Chargement de NewWebSession et conversion en WebSession
                    foreach (DataRow dr in Ids.Tables[0].Rows)
                    {
                        try
                        {
                            newWebSession = (NewWebSession)ImhotepDAL.LoadMySessionAlert(dr["id_alert"].ToString().Trim());
                            //création de NewWebsession
                            webSession = ConvertNewWebSessionToWebSession(newWebSession);
                            //Enregistrement de Websession en base
                            ImhotepDAL.UpdateSessionAlert(dr["id_alert"].ToString().Trim(), webSession);
                            Console.WriteLine("CONVERSION de la NewWebSession " + dr["id_alert"].ToString() + "-" + dr["alert"].ToString() + " en WebSession.");
                        }
                        catch (System.Exception e)
                        {

                            Console.WriteLine("ERROR : Impossible de convertir la NewWebsession " + dr["id_alert"].ToString() + " en Websession : " + e.Message + "\n" + e.StackTrace);
                        }
                    }
                }
                catch (System.Exception e)
                {

                    Console.WriteLine("ERROR : Impossible de convertir NewWebsession=>WebSession : " + e.Message + "\n" + e.StackTrace);
                }
                Console.WriteLine("FIN CONVERSION des NewWebSession en WebSession.");
            }

            Console.ReadLine();
            #region Local Test
            //newWebSession = (NewWebSession)ImhotepDAL.LoadMySessionAlert("821");
            //webSession = ConvertNewWebSessionToWebSession(newWebSession);
            //ImhotepDAL.UpdateSessionAlert("821", webSession);
            //webSession = (WebSession)ImhotepDAL.LoadMySessionAlert("821");

            #endregion
            success = true;
            return success;
        }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Crée la nouvelle version de session à partir des valeurs de l'ancienne
        /// </summary>
        /// <param name="webSession">ancienne version de la session client</param>
        /// <returns>nouvelle version de la session client</returns>
        //TODO : décommenter pour la conversion des sessions
        //TODO : commenter pour la conversion des univers
        public static NewWebSession ConvertWebSessionToNewWebSession(WebSession webSession)
        {
            NewWebSession newWebSession = null;
            if (webSession != null)
            {
                //newWebSession = new NewWebSession(webSession.CustomerLogin);
                newWebSession = new NewWebSession();
                newWebSession = SetNewWebSessionValues(newWebSession, webSession);
            }
            return newWebSession;
        }

        /// <summary>
        /// Crée la nouvelle version de session à partir des valeurs de l'ancienne
        /// </summary>
        /// <param name="newWebSession">ancienne version de la session client</param>
        /// <returns>nouvelle version de la session client</returns>
        public static WebSession ConvertNewWebSessionToWebSession(NewWebSession newWebSession)
        {
            WebSession webSession = null;
            if (newWebSession != null)
            {
               webSession = new TNS.AdExpress.Web.Core.Sessions.WebSession();
                webSession = Functions.SetCoreWebSessionValues(newWebSession, webSession);
            }
            return webSession;
        }

        #region Copies des valeurs de WebSession vers NewWebSession
        /// <summary>
        /// Copie les valeurs de WebSession (ancienne version de session)
        /// vers NewWebSession (nouvelle version)
        /// </summary>
        /// <param name="newWebSession">nouvelle session</param>
        /// <param name="webSession">ancienne session</param>
        /// <returns>nouvelle session</returns>
        //TODO : décommenter pour la conversion des sessions
        //TODO : commenter pour la conversion des univers
        private static NewWebSession SetNewWebSessionValues(NewWebSession newWebSession, WebSession webSession)
        {

            #region Initialisation des données
            //module en cours
            try
            {
                newWebSession.CurrentModule = (Int64)webSession.CurrentModule;
            }
            catch (Exception) { newWebSession.CurrentModule = 0; }
            //onglet en cours
            try
            {
                newWebSession.CurrentTab = webSession.CurrentTab;
            }
            catch (Exception) { newWebSession.CurrentTab = 0; }
            //Code de traduction du module
            try
            {
                newWebSession.ModuleTraductionCode = (int)webSession.ModuleTraductionCode;
            }
            catch (Exception) { newWebSession.ModuleTraductionCode = 0; }
            //Module cible
            try
            {
                newWebSession.ReachedModule = webSession.ReachedModule;
            }
            catch (Exception) { newWebSession.ReachedModule = false; }

            #region Univers et sélection des éléments de la nomenclature
            //Univers Média courant
            try
            {
                //newWebSession.CurrentUniversMedia = (TreeNode)ConvertOldTreeToTmp(webSession.CurrentUniversMedia);
                newWebSession.CurrentUniversMedia = (TreeNode)webSession.CurrentUniversMedia;
            }
            catch (Exception) { newWebSession.CurrentUniversMedia = new TreeNode("media"); }
            //Univers annonceur courant
            try
            {
                //newWebSession.CurrentUniversAdvertiser = (TreeNode)ConvertOldTreeToTmp(webSession.CurrentUniversAdvertiser);
                newWebSession.CurrentUniversAdvertiser = (TreeNode)webSession.CurrentUniversAdvertiser;
            }
            catch (Exception) { newWebSession.CurrentUniversAdvertiser = new TreeNode("advertiser"); }
            //Univers produit courant
            try
            {
                //newWebSession.CurrentUniversProduct = (TreeNode)ConvertOldTreeToTmp(webSession.CurrentUniversProduct);
                newWebSession.CurrentUniversProduct = (TreeNode)webSession.CurrentUniversProduct;
            }
            catch (Exception) { newWebSession.CurrentUniversProduct = new TreeNode("produit"); }
            //Annonceurs sélectionnés
            try
            {
                //newWebSession.SelectionUniversAdvertiser = (TreeNode)ConvertOldTreeToTmp(webSession.SelectionUniversAdvertiser);
                newWebSession.SelectionUniversAdvertiser = (TreeNode)webSession.SelectionUniversAdvertiser;
            }
            catch (Exception) { newWebSession.SelectionUniversAdvertiser = new TreeNode("advertiser"); }
            //Média Sélectionné
            try
            {
                //newWebSession.SelectionUniversMedia = (TreeNode)ConvertOldTreeToTmp(webSession.SelectionUniversMedia);
                newWebSession.SelectionUniversMedia = (TreeNode)webSession.SelectionUniversMedia;
            }
            catch (Exception) { newWebSession.SelectionUniversMedia = new TreeNode("media"); }
            //Produits sélectionnés
            try
            {
                //newWebSession.SelectionUniversProduct = (TreeNode)ConvertOldTreeToTmp(webSession.SelectionUniversProduct);
                newWebSession.SelectionUniversProduct = (TreeNode)webSession.SelectionUniversProduct;
            }
            catch (Exception) { newWebSession.SelectionUniversProduct = new TreeNode("produit"); }
            //Annonceurs de références
            try
            {
                //newWebSession.ReferenceUniversAdvertiser = (TreeNode)ConvertOldTreeToTmp(webSession.ReferenceUniversAdvertiser);
                newWebSession.ReferenceUniversAdvertiser = (TreeNode)webSession.ReferenceUniversAdvertiser;
            }
            catch (Exception) { newWebSession.ReferenceUniversAdvertiser = new TreeNode("advertiser"); }
            //Média de références
            try
            {
                //newWebSession.ReferenceUniversMedia = (TreeNode)ConvertOldTreeToTmp(webSession.ReferenceUniversMedia);
                newWebSession.ReferenceUniversMedia = (TreeNode)webSession.ReferenceUniversMedia;
            }
            catch (Exception) { newWebSession.ReferenceUniversMedia = new TreeNode("media"); }
            //Produits de références
            try
            {
                //newWebSession.ReferenceUniversProduct = (TreeNode)ConvertOldTreeToTmp(webSession.ReferenceUniversProduct);
                newWebSession.ReferenceUniversProduct = (TreeNode)webSession.ReferenceUniversProduct;
            }
            catch (Exception) { newWebSession.ReferenceUniversProduct = new TreeNode("produit"); }



            //Annonceurs concurrents
            try
            {
                //newWebSession.CompetitorUniversAdvertiser = new Hashtable(5);
                //if (webSession.CompetitorUniversAdvertiser.Count > 0)
                //{
                    //foreach (int key in webSession.CompetitorUniversAdvertiser.Keys)
                    //{
                    //    if (webSession.CompetitorUniversAdvertiser[key].GetType() == typeof(TreeNode))
                    //    {
                    //        newWebSession.CompetitorUniversAdvertiser.Add(key, ConvertOldTreeToTmp((TreeNode)webSession.CompetitorUniversAdvertiser[key]));
                    //    }
                    //    else
                    //    {
                    //        TNS.AdExpress.Web.Common.Univers.CompetitorAdvertiser cOld = (TNS.AdExpress.Web.Common.Univers.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[key];
                    //        newWebSession.CompetitorUniversAdvertiser.Add(key, new TNS.AdExpress.Imhotep.Core.Sessions.CompetitorAdvertiser(cOld.NameCompetitorAdvertiser, ConvertOldTreeToTmp(cOld.TreeCompetitorAdvertiser)));
                    //    }
                    //}
                    newWebSession.CompetitorUniversAdvertiser = webSession.CompetitorUniversAdvertiser;
                //}
            }
            catch (Exception) { newWebSession.CompetitorUniversAdvertiser = new Hashtable(5); }
            //Média concurrents
            try
            {
                //newWebSession.CompetitorUniversMedia = new Hashtable(5);
                //if (webSession.CompetitorUniversMedia.Count > 0)
                //{
                    //foreach (int key in webSession.CompetitorUniversMedia.Keys)
                    //{
                    //    newWebSession.CompetitorUniversMedia.Add(key, ConvertOldTreeToTmp((TreeNode)webSession.CompetitorUniversMedia[key]));
                    //}
                    newWebSession.CompetitorUniversMedia = webSession.CompetitorUniversMedia;
                //}
            }
            catch (Exception) { newWebSession.CompetitorUniversMedia = new Hashtable(5); }
            //Produits concurrents
            try
            {
                newWebSession.CompetitorUniversProduct = webSession.CompetitorUniversProduct;
            }
            catch (Exception) { newWebSession.CompetitorUniversProduct = new Hashtable(5); }
            #endregion

            //Arbre temporaire
            try
            {
                //newWebSession.TemporaryTreenode = ConvertOldTreeToTmp(webSession.TemporaryTreenode);
                newWebSession.TemporaryTreenode = webSession.TemporaryTreenode;
            }
            catch (Exception) { newWebSession.TemporaryTreenode = null; }

            #region Période
            //Durée de la période
            try
            {
                newWebSession.PeriodLength = webSession.PeriodLength;
            }
            catch (Exception) { newWebSession.PeriodLength = 0; }
            //Date de début
            try
            {
                newWebSession.PeriodBeginningDate = webSession.PeriodBeginningDate;
            }
            catch (Exception) { newWebSession.PeriodBeginningDate = ""; }

            //Date de fin 
            try
            {
                newWebSession.PeriodEndDate = webSession.PeriodEndDate;
            }
            catch (Exception) { newWebSession.PeriodEndDate = ""; }

            //Début de la période
            try
            {
                newWebSession.BeginningDate = webSession.BeginningDate;
            }
            catch (Exception) { newWebSession.BeginningDate = DateTime.Now; }
            //Date de chargement des données
            try
            {
                newWebSession.DownLoadDate = webSession.DownLoadDate;
            }
            catch (Exception) { newWebSession.DownLoadDate = 0; }
            //Type de la période
            try
            {
                newWebSession.PeriodType = webSession.PeriodType;
            }
            catch (Exception) { newWebSession.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth; }
            //Détail la période
            try
            {
                newWebSession.DetailPeriod = webSession.DetailPeriod;
            }
            catch (Exception) { newWebSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.monthly; }
            //Date de modification	
            try
            {
                newWebSession.ModificationDate = webSession.ModificationDate;
            }
            catch (Exception) { newWebSession.ModificationDate = DateTime.Now; }
            //Détail Période préformaté
            try
            {
                newWebSession.PreformatedPeriodDetail = webSession.PreformatedPeriodDetail;
            }
            catch (Exception) { newWebSession.PreformatedPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedPeriodDetails.monthly_And_Total; }
            #endregion

            //Encarts
            try
            {
                newWebSession.Insert = (TNS.AdExpress.Constantes.Web.CustomerSessions.Insert)webSession.Insert;
            }
            catch (Exception) { newWebSession.Insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total; }
            //Affiche graphique
            try
            {
                newWebSession.Graphics = webSession.Graphics;
            }
            catch (Exception) { newWebSession.Graphics = true; }
            //Unités
            try
            {
                newWebSession.Unit = (CstWeb.CustomerSessions.Unit)webSession.Unit;
            }
            catch (Exception) { newWebSession.Unit = CstWeb.CustomerSessions.Unit.euro; }

            #region élément préformatés
            //Détail produit préformaté
            try
            {
                newWebSession.PreformatedProductDetail = (CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails)webSession.PreformatedProductDetail;
            }
            catch (Exception) { newWebSession.PreformatedProductDetail = CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group; }
            //Détail Média préformaté
            try
            {
                newWebSession.PreformatedMediaDetail = webSession.PreformatedMediaDetail;
            }
            catch (Exception) { newWebSession.PreformatedMediaDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle; }
            //Tableau préformaté
            try
            {
                newWebSession.PreformatedTable = webSession.PreformatedTable;
            }
            catch (Exception) { newWebSession.PreformatedTable = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedTables.media_X_Year; }
            #endregion

            //Dernière page visitée
            try
            {
                newWebSession.LastReachedResultUrl = webSession.LastReachedResultUrl;
            }
            catch (Exception) { newWebSession.LastReachedResultUrl = ""; }
            //Pourcentage
            try
            {
                newWebSession.Percentage = webSession.Percentage;
            }
            catch (Exception) { newWebSession.Percentage = false; }
            //Niveau de détail produit
            try
            {
                //newWebSession.ProductDetailLevel = new ProductDetailLevel(webSession.ProductDetailLevel.LevelProduct, ConvertOldTreeToTmp(webSession.ProductDetailLevel.ListElement));
                newWebSession.ProductDetailLevel = webSession.ProductDetailLevel;
            }
            catch (Exception) { newWebSession.ProductDetailLevel = null; }
            //Niveau de détail média
            try
            {
                //newWebSession.MediaDetailLevel = new MediaDetailLevel(webSession.MediaDetailLevel.LevelMedia, ConvertOldTreeToTmp(webSession.MediaDetailLevel.ListElement));
                newWebSession.MediaDetailLevel = webSession.MediaDetailLevel;
            }
            catch (Exception) { newWebSession.MediaDetailLevel = null; }
            //Critère de comparaison
            try
            {
                newWebSession.ComparaisonCriterion = webSession.ComparaisonCriterion;
            }
            catch (Exception) { newWebSession.ComparaisonCriterion = TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal; }
            //Etudes comparatives
            try
            {
                newWebSession.ComparativeStudy = webSession.ComparativeStudy;
            }
            catch (Exception) { newWebSession.ComparativeStudy = false; }
            //login
            try
            {
                newWebSession.CustomerLogin = webSession.CustomerLogin;
            }
            catch (Exception) { }
            //personnalisation des éléménts de référence
            try
            {
                newWebSession.CustomizedReferenceComcurrentElements = webSession.CustomizedReferenceComcurrentElements;
            }
            catch (Exception) { newWebSession.CustomizedReferenceComcurrentElements = false; }
            //Evolution
            try
            {
                newWebSession.Evolution = webSession.Evolution;
            }
            catch (Exception) { newWebSession.Evolution = true; }
            //ID Session	
            try
            {
                newWebSession.IdSession = webSession.IdSession;
            }
            catch (Exception) { }
            //Dernier mois disponible pour les recaps		
            try
            {
                newWebSession.LastAvailableRecapMonth = webSession.LastAvailableRecapMonth;
            }
            catch (Exception) { newWebSession.LastAvailableRecapMonth = ""; }
            //Nouveau produit	
            try
            {
                newWebSession.NewProduct = webSession.NewProduct;
            }
            catch (Exception) { newWebSession.NewProduct = TNS.AdExpress.Constantes.Web.CustomerSessions.NewProduct.support; }
            //PDM	
            try
            {
                newWebSession.PDM = webSession.PDM;
            }
            catch (Exception) { newWebSession.PDM = false; }
            //PDV	
            try
            {
                newWebSession.PDV = webSession.PDV;
            }
            catch (Exception) { newWebSession.PDV = false; }
            //Personnalisé élément
            try
            {
                newWebSession.PersonalizedElementsOnly = webSession.PersonalizedElementsOnly;
            }
            catch (Exception) { newWebSession.PersonalizedElementsOnly = false; }
            // Utilisation de l'auto-promo Evaliant	
            try
            {
                newWebSession.AutopromoEvaliant = webSession.AutopromoEvaliant;
            }
            catch (Exception) { newWebSession.AutopromoEvaliant = false; }

            //language du site
            try
            {
                newWebSession.SiteLanguage = webSession.SiteLanguage;
            }
            catch (Exception) { newWebSession.SiteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH; }



            #region Tri
            ////Tri
            //try
            //{
            //    newWebSession.Sort = webSession.Sort;
            //}
            //catch (Exception) { newWebSession.Sort = 0; }
            ////Order de Tri
            //try
            //{
            //    newWebSession.SortOrder = webSession.SortOrder;
            //}
            //catch (Exception) { newWebSession.SortOrder = TNS.AdExpress.Constantes.Web.CustomerSessions.SortOrder.non; }
            #endregion

            #region éléments rajoutés pour migration 03/05/2005

            #region Général
            // Nom du fichier PDF exporté
            try
            {
                newWebSession.ExportedPDFFileName = webSession.ExportedPDFFileName;
            }
            catch (Exception)
            {
                newWebSession.ExportedPDFFileName = "";
            }


            // Email des destinataires du fichier PDF à exporter
            try
            {
                newWebSession.EmailRecipient = webSession.EmailRecipient;
            }
            catch (Exception)
            {
                newWebSession.EmailRecipient = null;
            }
            #endregion

            #region AdExpress pricnicapl and secondary universes
            // Principal dictionary of  universe product selection           
            try
            {
                newWebSession.PrincipalProductUniverses = webSession.PrincipalProductUniverses;
            }
            catch (Exception)
            {
                newWebSession.PrincipalProductUniverses = new Dictionary<int, AdExpressUniverse>();
            }
           
            // Secondary dictionary of universe product selection           
            try
            {
                newWebSession.SecondaryProductUniverses = webSession.SecondaryProductUniverses;
            }
            catch (Exception)
            {
                newWebSession.SecondaryProductUniverses = new Dictionary<int, AdExpressUniverse>();
            }
            // Principal dictionary of  universe media selection
            try
            {
                newWebSession.PrincipalMediaUniverses = webSession.PrincipalMediaUniverses;
            }
            catch (Exception)
            {
                newWebSession.PrincipalMediaUniverses = new Dictionary<int, AdExpressUniverse>();
            }

            // Secondary dictionary of universe media selection
            try
            {
                newWebSession.SecondaryMediaUniverses = webSession.SecondaryMediaUniverses;
            }
            catch (Exception)
            {
                newWebSession.SecondaryMediaUniverses = new Dictionary<int, AdExpressUniverse>();
            }
            #endregion

            #region période
            //Date de début du détail de la période
            try
            {
                newWebSession.DetailPeriodBeginningDate = webSession.DetailPeriodBeginningDate;
            }
            catch (Exception) { newWebSession.DetailPeriodBeginningDate = ""; }
            //Date de fin du détail de la période
            try
            {
                newWebSession.DetailPeriodEndDate = webSession.DetailPeriodEndDate;
            }
            catch (Exception) { newWebSession.DetailPeriodEndDate = ""; }

            // Date de début de publication
            try
            {
                newWebSession.PublicationBeginningDate = webSession.PublicationBeginningDate;
            }
            catch (Exception) { newWebSession.PublicationBeginningDate = ""; }
            // Date de fin de publication
            try
            {
                newWebSession.PublicationEndDate = webSession.PublicationEndDate;
            }
            catch (Exception) { newWebSession.PublicationEndDate = ""; }
            // Année du fichier d'agence média
            try
            {
                newWebSession.MediaAgencyFileYear = webSession.MediaAgencyFileYear;
            }
            catch (Exception) { newWebSession.MediaAgencyFileYear = ""; }
            // Identifiant de type de date de parution
            try
            {
                newWebSession.PublicationDateType = webSession.PublicationDateType;
            }
            catch (Exception) { newWebSession.PublicationDateType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.publicationType.staticDate; }

            #endregion

            #region univers étude bilan de campagne (APPM)
            //Ecart
            try
            {
                newWebSession.Ecart = webSession.Ecart;
            }
            catch (Exception)
            {
                newWebSession.Ecart = false;
            }
            //Sélection courante des cibles AEPM de l'univers courant
            try
            {
                //newWebSession.CurrentUniversAEPMTarget = (TreeNode)ConvertOldTreeToTmp(webSession.CurrentUniversAEPMTarget);
                newWebSession.CurrentUniversAEPMTarget = (TreeNode)webSession.CurrentUniversAEPMTarget;
            }
            catch (Exception) { newWebSession.CurrentUniversAEPMTarget = new TreeNode("target"); }

            //Sélection originale des cibles AEPM de l'univers courant
            try
            {
                //newWebSession.SelectionUniversAEPMTarget = (TreeNode)ConvertOldTreeToTmp(webSession.SelectionUniversAEPMTarget);
                newWebSession.SelectionUniversAEPMTarget = (TreeNode)webSession.SelectionUniversAEPMTarget;
            }
            catch (Exception) { newWebSession.SelectionUniversAEPMTarget = new TreeNode("target"); }

            //  Sélection originale des vagues AEPM de l'univers courant
            try
            {
                //newWebSession.SelectionUniversAEPMWave = (TreeNode)ConvertOldTreeToTmp(webSession.SelectionUniversAEPMWave);
                newWebSession.SelectionUniversAEPMWave = (TreeNode)webSession.SelectionUniversAEPMWave;
            }
            catch (Exception) { newWebSession.SelectionUniversAEPMWave = new TreeNode("wave"); }

            // Sélection courante des vagues AEPM de l'univers courant
            try
            {
                //newWebSession.CurrentUniversAEPMWave = (TreeNode)ConvertOldTreeToTmp(webSession.CurrentUniversAEPMWave);
                newWebSession.CurrentUniversAEPMWave = (TreeNode)webSession.CurrentUniversAEPMWave;
            }
            catch (Exception) { newWebSession.CurrentUniversAEPMWave = new TreeNode("wave"); }

            // Sélection courante des vagues OJD de l'univers courant
            try
            {
                newWebSession.CurrentUniversOJDWave = (TreeNode)webSession.CurrentUniversOJDWave;
            }
            catch (Exception) { newWebSession.CurrentUniversOJDWave = new TreeNode("wave"); }

            // Sélection originale des vagues OJD de l'univers courant
            try
            {
                newWebSession.SelectionUniversOJDWave = (TreeNode)webSession.SelectionUniversOJDWave;
            }
            catch (Exception) { newWebSession.SelectionUniversOJDWave = new TreeNode("wave"); }
            #endregion

            #region variables de Répartition
            //  format
            try
            {
                newWebSession.Format = webSession.Format;
            }
            catch (Exception) { newWebSession.Format = CstWeb.Repartition.Format.Total; }

            // Tranche horaire
            try
            {
                newWebSession.TimeInterval = webSession.TimeInterval;
            }
            catch (Exception) { newWebSession.TimeInterval = CstWeb.Repartition.timeInterval.Total; }

            //  Jour nommé
            try
            {
                newWebSession.NamedDay = webSession.NamedDay;
            }
            catch (Exception) { newWebSession.NamedDay = CstWeb.Repartition.namedDay.Total; }

            #endregion


            #endregion

            #region éléments rajoutés pour migration 29/12/2005
            // Source de données du client pour accéder aux données
            //try
            //{
            //    newWebSession.Source = webSession.Source;
            //}
            //catch (Exception) { newWebSession.Source = webSession.CustomerLogin.Source; }

            //Liste des visuels
            try
            {
                newWebSession.Visuals = webSession.Visuals;
            }
            catch (Exception) { newWebSession.Visuals = null; }

            //Contient les nouvelles variables sessions
            try
            {
                newWebSession.UserParameters = webSession.UserParameters;
            }
            catch (Exception) { newWebSession.UserParameters = new Hashtable(); }

            //Liste destinée à contenir les identifiants accroches de personnalisation dans les plans médias
            try
            {
                newWebSession.IdSlogans = webSession.IdSlogans;
            }
            catch (Exception) { newWebSession.IdSlogans = null; }

            //Hashtable contenant une liste d'accroches en clés et des couleurs associées en valeurs
            try
            {
                newWebSession.SloganColors = webSession.SloganColors;
            }
            catch (Exception) { newWebSession.SloganColors = null; }

            #endregion

            #endregion

            #region variables temporaires
            //ID login du client
            try
            {
                newWebSession.userIdLogin = webSession.CustomerLogin.IdLogin;
            }
            catch (Exception)
            {
                newWebSession.userIdLogin = 0;
            }

            //login du client
            try
            {
                newWebSession.userLogin = webSession.CustomerLogin.Login;
            }
            catch (Exception)
            {
                newWebSession.userLogin = "";
            }

            //login du client
            try
            {
                newWebSession.userPassWord = webSession.CustomerLogin.PassWord;
            }
            catch (Exception)
            {
                newWebSession.userPassWord = "";
            }

            #endregion



            return newWebSession;
        }
        #endregion

        #endregion


    }
}
