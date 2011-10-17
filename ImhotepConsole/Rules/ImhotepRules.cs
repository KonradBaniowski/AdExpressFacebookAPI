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
using TNS.AdExpress.Domain.Web;
using TNS.Classification.Universe;

namespace ImhotepConsole.Rules
{
    public class ImhotepRules
    {

        const long ID_PRESS = 2;
        const long ID_MAGAZINE = 98;
        const long ID_NEWSPAPERS = 99;
        const long ID_CATEGORY_NEWSPAPERS = 8;
        const long ID_CATEGORY_FREE_DISTRIBUTION_NEWSPAPERS = 4;
        const long ID_CATEGORY_MAGAZINES = 2;
        const long ID_IC_ECONOMY_BUSINESS = 43;
        const long MAGAZINE_BRANCH_TYPE = 30;
        const long NEWSPAPER_BRANCH_TYPE = 31;

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

        #region Conversion  Splitt All WebSession Mediatype
        /// <summary>
        /// Splitt All WebSession Media type
        /// </summary>
        public static bool SplittAllWebSessionMediatype()
        {

            #region Variables
            WebSession webSession = null;
            WebSession newWebSession = null;
            bool success = false;
            TreeNode tnd = null;
            #endregion

            #region Splitt All WebSession Media type
            //Chargement des Ids Session
            DataSet Ids = ImhotepDAL.LoadSessionIds();

            //Chargement de WebSession et conversion en NewWebSession
            if (Ids.Tables[0].Rows.Count > 0)
            {
                try
                {
                    Console.WriteLine("DEBUT SPLIT des MEDIAS de WebSession.");
                    foreach (DataRow dr in Ids.Tables[0].Rows)
                    {
                        try
                        {
                            webSession = (WebSession)ImhotepDAL.LoadMySession(dr["id_my_session"].ToString().Trim());
                            #region SPLIT MEDIA

                            //split des médias

                            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                            || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                            || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE)
                            {
                                GetCompetitorUniverse((Hashtable)webSession.CompetitorUniversMedia, webSession, long.Parse(dr["id_my_session"].ToString()), "CompetitorUniversMedia");
                            }
                            else
                            {
                                //CurrentUniversMedia
                                //webSession.CurrentUniversMedia = ConvertOldTreeToTmp(webSession.CurrentUniversMedia, webSession, long.Parse(dr["id_my_session"].ToString()), "CurrentUniversMedia");
                                webSession.CurrentUniversMedia = SplitPressTreenode(webSession.CurrentUniversMedia, webSession, long.Parse(dr["id_my_session"].ToString()), "CurrentUniversMedia");

                                //SelectionUniversMedia
                                webSession.SelectionUniversMedia = SplitPressTreenode(webSession.SelectionUniversMedia, webSession, long.Parse(dr["id_my_session"].ToString()), "SelectionUniversMedia");

                                if (webSession.CompetitorUniversMedia.Count > 0)
                                {
                                    string Pb = "Hoooooo!";
                                }
                            }

                            //ReferenceUniversMedia
                            webSession.ReferenceUniversMedia = SplitPressTreenode(webSession.ReferenceUniversMedia, webSession, long.Parse(dr["id_my_session"].ToString()), "ReferenceUniversMedia");

                            //Dictionary<int, AdExpressUniverse> SecondaryMediaUniverses 
                            newWebSession = SplitMediaUniverse(webSession.SecondaryMediaUniverses, webSession, long.Parse(dr["id_my_session"].ToString()), "SecondaryMediaUniverses");

                            //Dictionary<int, AdExpressUniverse> PrincipalMediaUniverses 
                            newWebSession = SplitMediaUniverse(webSession.PrincipalMediaUniverses, webSession, long.Parse(dr["id_my_session"].ToString()), "PrincipalMediaUniverses");
                            #endregion


                            //Enregistrement de NewWebsession en base
                             ImhotepDAL.UpdateSession(dr["id_my_session"].ToString().Trim(), webSession);
                            Console.WriteLine("SPLIT des MEDIAS  de la WebSession " + dr["id_my_session"].ToString() + " - " + dr["my_session"].ToString() + " .");
                        }
                        catch (System.Exception e)
                        {

                            Console.WriteLine("ERROR : Impossible de SPLIT des MEDIAS de la session " + dr["id_my_session"].ToString() + "  : " + e.Message + "\n" + e.StackTrace);
                        }

                    }
                    Console.WriteLine("FIN SPLIT des MEDIAS de WebSession..");
                }
                catch (System.Exception e)
                {

                    Console.WriteLine("ERROR : Impossible de SPLIT des MEDIAS DE WebSession. : " + e.Message + "\n" + e.StackTrace);
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

        private static WebSession SplitMediaUniverse(Dictionary<int, AdExpressUniverse> dictionary, WebSession webSession, long mySessionId, string propertyName)
        {
            List<NomenclatureElementsGroup> listGroup = null;
            List<long> listIds = null;
            if (dictionary != null && dictionary.Count > 0)
            {
                for (int k = 0; k < dictionary.Count; k++)
                {
                    if (dictionary[k].Count() > 0)
                    {
                        //Get excludes items
                        listGroup = dictionary[k].GetExludes();
                        if (listGroup != null && listGroup.Count > 0)
                        {
                            for (int i = 0; i < listGroup.Count; i++)
                            {
                                if (listGroup[i] != null && listGroup[i].Count() > 0)
                                {
                                    //Media TYPE
                                    listIds = listGroup[i].Get(TNSClassificationLevels.VEHICLE);
                                    if (listIds != null && listIds.Contains(ID_PRESS))
                                        Console.WriteLine(" Media TYPE ! La session " + mySessionId + "  contient le media PRESS en exclusion pour le module " + GetModuleName(webSession.CurrentModule) + " pour propriété " + propertyName);
                                }
                            }
                        }
                        //Get includes items
                        listGroup = dictionary[k].GetIncludes();
                        if (listGroup != null && listGroup.Count > 0)
                        {
                            for (int i = 0; i < listGroup.Count; i++)
                            {
                                if (listGroup[i] != null && listGroup[i].Count() > 0)
                                {
                                    //Media TYPE
                                    listIds = listGroup[i].Get(TNSClassificationLevels.VEHICLE);
                                    if (listIds != null && listIds.Contains(ID_PRESS))
                                        Console.WriteLine(" Media TYPE ! La session " + mySessionId + "  contient le media PRESS en accès pour le module " + GetModuleName(webSession.CurrentModule) + " pour propriété " + propertyName);
                                }
                            }
                        }
                    }
                }
            }

            return webSession;
        }
        #endregion

        #region Convert All Universes
        public static bool ConvertAllUniverses()
        {
            bool success = false;

            string universeListId= ImhotepDAL.GetUniversList("8","5");
            string[] uvivArr = universeListId.Split(',');
            DataSet ds = DAL.ImhotepDAL.GetVehicleInterectcenterMedia("98,99", 44);
            DataTable dt = ds.Tables[0];
            TreeNode t2 = null, tMag = null, tNews;
            TNS.AdExpress.Web.Core.Sessions.LevelInformation li2 = null;           
            DataRow[] drs = null;

            for (int i = 0; i < uvivArr.Length; i++)
            {
                ArrayList univ =  (ArrayList)ImhotepDAL.GetTreeNodeUniverse(long.Parse(uvivArr[i]));
               
                TreeNode t = (TreeNode)univ[0];

                tMag = new TreeNode();
                tNews = new TreeNode();
                foreach (TreeNode n2 in t.Nodes)
                {
                    
                    li2 = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n2.Tag;

                    drs = dt.Select(" id_media=" + li2.ID.ToString());

                    t2 = new TreeNode(n2.Text);
                    t2.Checked = n2.Checked;
                    t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li2.Type, li2.ID, li2.Text);
                    if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_MAGAZINE)
                    {
                        
                        tMag.Nodes.Add(t2);
                    }
                    else if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_NEWSPAPERS)
                    {                      
                        tNews.Nodes.Add(t2);
                    }
                    else throw new Exception(" Impossible to identify MEDIA node ");                  

                }

                //Add Treenode magazine  

                if (tMag.Nodes.Count > 0)
                {
                    univ[0] = tMag;
                    ImhotepDAL.UpDateUniverse(long.Parse(uvivArr[i]), MAGAZINE_BRANCH_TYPE, univ);
                }
                //Add Treenode newspaper
                else if (tNews.Nodes.Count > 0)
                {

                    univ[0] = tNews;
                    ImhotepDAL.UpDateUniverse(long.Parse(uvivArr[i]), NEWSPAPER_BRANCH_TYPE, univ);
                }
                else
                {
                    throw new Exception(" Impossible to create MEDIA node ");

                }
                   

                if (univ[1] != null)
                {
                    throw new Exception(" Impossible to identify MEDIA node ");            
                }
            }
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

        #region Methods Internes

        #region SplitPressTreenode
        public static TreeNode SplitPressTreenode(TreeNode oldTree, WebSession webSession, long mySessionId, string propertyName)
        {


            return SplitPressTreenode(oldTree, webSession, mySessionId, propertyName, null, null);
        }

        public static TreeNode SplitPressTreenode(TreeNode oldTree, WebSession webSession, long mySessionId, string propertyName, List<TreeNode> treeListMag, List<TreeNode> treeListNews)
        {
            TreeNode newTree = null;
            DataSet ds = null;


            if (oldTree != null
                )
            {
                newTree = new TreeNode(oldTree.Text);
                newTree.Checked = oldTree.Checked;

                if (oldTree.Tag != null)
                {
                    newTree.Tag = oldTree.Tag;
                }
                ConvertSplitPressTag(oldTree, newTree, webSession, mySessionId, propertyName, treeListMag, treeListNews);

            }

            return newTree;
        }


        public static void ConvertSplitPressTag(TreeNode from, TreeNode to, WebSession webSession, long mySessionId, string propertyName, List<TreeNode> treeListMag, List<TreeNode> treeListNews)
        {
            DataSet ds = null;
            TreeNode t2 = null, t2Mag = null, t2News, t3 = null;
            TNS.AdExpress.Web.Core.Sessions.LevelInformation li = null, li2 = null, liTemp = null, li3 = null;
            int nbCategoryType = 0;
            bool isChecked = false;
            TreeNode toClone = ("CompetitorUniversMedia" == propertyName) ? (TreeNode)to.Clone() : null;

            foreach (TreeNode n in from.Nodes)
            {
                TreeNode t = new TreeNode(n.Text);
                t.Checked = n.Checked;
                if (n.Tag != null)
                {
                    li = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n.Tag;

                    if (IsPressMediaType(li.Type, webSession, mySessionId, li.ID, propertyName)
                        || ("CompetitorUniversMedia" == propertyName))
                    {
                        switch (webSession.CurrentModule)
                        {

                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA:
                                #region ANALYSE_PLAN_MEDIA
                                ds = DAL.ImhotepDAL.GetVehicles("", ID_MAGAZINE.ToString(), webSession.SiteLanguage);
                                string vehicleLabel = ds.Tables[0].Rows[0]["vehicle"].ToString();
                                t = new TreeNode(vehicleLabel);
                                t.Checked = n.Checked;
                                t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_MAGAZINE, vehicleLabel);
                                to.Nodes.Add(t);
                                break;
                                #endregion

                            case TNS.AdExpress.Constantes.Web.Module.Name.TENDACES:
                                #region TENDACES
                                ds = DAL.ImhotepDAL.GetVehicles("", ID_MAGAZINE.ToString(), webSession.SiteLanguage);
                                vehicleLabel = ds.Tables[0].Rows[0]["vehicle"].ToString();
                                t = new TreeNode(vehicleLabel);
                                t.Checked = n.Checked;
                                t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_MAGAZINE, vehicleLabel);
                                to.Nodes.Add(t);
                                break;
                                #endregion

                            case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
                            case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:

                                #region TABLEAU_DYNAMIQUE & INDICATEUR
                                List<TreeNode> newsPaperCategories = new List<TreeNode>();
                                List<TreeNode> magazinesCategories = new List<TreeNode>();

                                foreach (TreeNode n2 in n.Nodes)
                                {
                                    li2 = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n2.Tag;
                                    if (li2.ID == ID_CATEGORY_FREE_DISTRIBUTION_NEWSPAPERS || li2.ID == ID_CATEGORY_NEWSPAPERS)
                                    {
                                        newsPaperCategories.Add(n2);
                                    }
                                    else if (li2.ID == ID_CATEGORY_MAGAZINES)
                                    {
                                        magazinesCategories.Add(n2);
                                    }
                                    else throw new Exception(" Impossible to identify CATEGORY node ");

                                }

                                if (newsPaperCategories.Count > 0 && magazinesCategories.Count > 0)
                                    nbCategoryType = 2;
                                else if (newsPaperCategories.Count > 0 || magazinesCategories.Count > 0)
                                    nbCategoryType = 1;

                                if (magazinesCategories.Count > 0 || n.Checked)
                                {

                                    ds = DAL.ImhotepDAL.GetVehicles("", ID_MAGAZINE.ToString(), webSession.SiteLanguage);
                                    vehicleLabel = ds.Tables[0].Rows[0]["vehicle"].ToString();
                                    t = new TreeNode(vehicleLabel);
                                    t.Checked = n.Checked;
                                    isChecked = n.Checked;
                                    t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_MAGAZINE, vehicleLabel);
                                    to.Nodes.Add(t);

                                    if (magazinesCategories.Count > 0)
                                    {
                                        for (int h = 0; h < magazinesCategories.Count; h++)
                                        {
                                            t2 = new TreeNode(magazinesCategories[h].Text);
                                            t2.Checked = magazinesCategories[h].Checked;
                                            if (!isChecked) isChecked = t2.Checked;
                                            liTemp = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)magazinesCategories[h].Tag;
                                            t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(liTemp.Type, liTemp.ID, liTemp.Text);
                                            t.Nodes.Add(t2);
                                        }
                                    }
                                }


                                if (nbCategoryType == 1 && (newsPaperCategories.Count > 0 || n.Checked))
                                {
                                    ds = DAL.ImhotepDAL.GetVehicles("", ID_NEWSPAPERS.ToString(), webSession.SiteLanguage);
                                    vehicleLabel = ds.Tables[0].Rows[0]["vehicle"].ToString();
                                    t = new TreeNode(vehicleLabel);
                                    t.Checked = (isChecked) ? false : n.Checked; //On décoche cette media si le premier est coché ?
                                    if (t.Checked) t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_NEWSPAPERS, vehicleLabel);
                                    else t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(GetTypeException(li.Type), ID_NEWSPAPERS, vehicleLabel);
                                    t.Text = vehicleLabel;
                                    to.Nodes.Add(t);

                                    if (newsPaperCategories.Count > 0)
                                    {
                                        for (int h = 0; h < newsPaperCategories.Count; h++)
                                        {
                                            t2 = new TreeNode(newsPaperCategories[h].Text);
                                            t2.Checked = (isChecked) ? false : newsPaperCategories[h].Checked;//ON décoche cette catégorie si le premier est coché ?
                                            liTemp = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)newsPaperCategories[h].Tag;
                                            if (t2.Checked) t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(liTemp.Type, liTemp.ID, liTemp.Text);
                                            else t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(GetTypeException(liTemp.Type), liTemp.ID, liTemp.Text);
                                            t.Nodes.Add(t2);
                                        }
                                    }
                                }

                                break;
                                #endregion
                            case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PRESSE:

                                #region TABLEAU_DE_BORD_PRESSE
                                //all media
                                ds = DAL.ImhotepDAL.GetVehicleInterectcenterMedia("98,99", webSession.SiteLanguage);
                                DataTable dt = ds.Tables[0];

                                // MAGAZINE NODE                               
                                DataSet ds2 = DAL.ImhotepDAL.GetVehicles("", ID_MAGAZINE.ToString(), webSession.SiteLanguage);
                                vehicleLabel = ds2.Tables[0].Rows[0]["vehicle"].ToString();
                                TreeNode tMag = new TreeNode(vehicleLabel);
                                tMag.Checked = n.Checked;
                                isChecked = n.Checked;
                                tMag.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_MAGAZINE, vehicleLabel);

                                ds2 = DAL.ImhotepDAL.GetVehicles("", ID_NEWSPAPERS.ToString(), webSession.SiteLanguage);
                                vehicleLabel = ds2.Tables[0].Rows[0]["vehicle"].ToString();
                                TreeNode tNews = new TreeNode(vehicleLabel);
                                tNews.Checked = n.Checked;
                                isChecked = n.Checked;
                                tNews.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_NEWSPAPERS, vehicleLabel);

                                DataRow[] drs = null;
                                foreach (TreeNode n2 in n.Nodes)
                                {
                                    li2 = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n2.Tag;
                                    if (li2.ID == ID_IC_ECONOMY_BUSINESS)
                                    {
                                        drs = dt.Select(" id_interest_center=" + li2.ID.ToString());

                                        //Interest center Mag
                                        t2Mag = new TreeNode(n2.Text);
                                        t2Mag.Checked = n2.Checked;
                                        isChecked = n2.Checked;

                                        //Interest center News
                                        t2News = new TreeNode(n2.Text);
                                        t2News.Checked = n2.Checked;
                                        isChecked = n2.Checked;

                                        foreach (TreeNode n3 in n2.Nodes)
                                        {
                                            li3 = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n3.Tag;
                                            t3 = new TreeNode(n3.Text);
                                            t3.Checked = n3.Checked;
                                            t3.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li3.Type, li3.ID, li3.Text);

                                            drs = dt.Select(" id_interest_center=" + li2.ID.ToString() + "  and id_media =" + li3.ID);
                                            if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_MAGAZINE)
                                            {
                                                t2Mag.Nodes.Add(t3);
                                            }
                                            else if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_NEWSPAPERS)
                                            {
                                                t2News.Nodes.Add(t3);
                                            }
                                            else throw new Exception(" Impossible to identify INTERERST CENTER node ");

                                        }

                                        if (t2Mag != null && t2Mag.Nodes.Count > 0)
                                        {
                                            tMag.Nodes.Add(t2Mag);
                                        }

                                        if (t2News != null && t2News.Nodes.Count > 0)
                                        {
                                            tNews.Nodes.Add(t2News);
                                        }


                                    }
                                    else
                                    {
                                        drs = dt.Select(" id_interest_center=" + li2.ID.ToString());
                                        t2 = new TreeNode(n2.Text);
                                        t2.Checked = n2.Checked;
                                        isChecked = n2.Checked;
                                        if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_MAGAZINE)
                                        {
                                            t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li2.Type, li2.ID, li2.Text);
                                            tMag.Nodes.Add(t2);
                                        }
                                        else if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_NEWSPAPERS)
                                        {
                                            t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li2.Type, li2.ID, li2.Text);
                                            tNews.Nodes.Add(t2);
                                        }
                                        else throw new Exception(" Impossible to identify INTERERST CENTER node ");

                                        foreach (TreeNode n3 in n2.Nodes)
                                        {
                                            li3 = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n3.Tag;
                                            t3 = new TreeNode(n3.Text);
                                            t3.Checked = n3.Checked;
                                            t3.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li3.Type, li3.ID, li3.Text);
                                            t2.Nodes.Add(t3);
                                        }

                                    }

                                }

                                //Add Treenode magazine
                                if (tMag.Nodes.Count > 0)
                                    to.Nodes.Add(tMag);

                                //Add Treenode newspaper
                                if (tMag.Nodes.Count == 0 && tNews.Nodes.Count > 0)
                                    to.Nodes.Add(tNews);


                                break;
                                #endregion
                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE:
                            case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:

                                #region ANALYSE_DYNAMIQUE &  ANALYSE_CONCURENTIELLE & ANALYSE_PORTEFEUILLE

                                //all media
                                ds = DAL.ImhotepDAL.GetVehicleInterectcenterMedia("98,99", webSession.SiteLanguage);
                                dt = ds.Tables[0];


                                    if ("CompetitorUniversMedia" == propertyName)
                                {
                                    drs = dt.Select(" id_media=" + li.ID.ToString());

                                    tMag = null; tNews = null;
                                    if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_MAGAZINE)
                                    {
                                        tMag = new TreeNode(n.Text);
                                        tMag.Checked = n.Checked;
                                        tMag.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
                                        to.Nodes.Add(tMag);
                                    }
                                    else if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_NEWSPAPERS)
                                    {

                                        tNews = new TreeNode(n.Text);
                                        tNews.Checked = n.Checked;
                                        tNews.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
                                        toClone.Nodes.Add(tNews);
                                    }
                                    else throw new Exception(" Impossible to identify MEDIA node ");
                                }
                                else
                                {
                                    // MAGAZINE NODE                               
                                    ds2 = DAL.ImhotepDAL.GetVehicles("", ID_MAGAZINE.ToString(), webSession.SiteLanguage);
                                    vehicleLabel = ds2.Tables[0].Rows[0]["vehicle"].ToString();
                                    tMag = new TreeNode(vehicleLabel);
                                    tMag.Checked = n.Checked;
                                    isChecked = n.Checked;
                                    tMag.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_MAGAZINE, vehicleLabel);

                                    ds2 = DAL.ImhotepDAL.GetVehicles("", ID_NEWSPAPERS.ToString(), webSession.SiteLanguage);
                                    vehicleLabel = ds2.Tables[0].Rows[0]["vehicle"].ToString();
                                    tNews = new TreeNode(vehicleLabel);
                                    tNews.Checked = n.Checked;
                                    isChecked = n.Checked;
                                    tNews.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, ID_NEWSPAPERS, vehicleLabel);

                                    drs = null;
                                    foreach (TreeNode n2 in n.Nodes)
                                    {
                                        li2 = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n2.Tag;

                                        drs = dt.Select(" id_media=" + li2.ID.ToString());

                                        t2 = new TreeNode(n2.Text);
                                        t2.Checked = n2.Checked;
                                        isChecked = n2.Checked;
                                        if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_MAGAZINE)
                                        {
                                            t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li2.Type, li2.ID, li2.Text);
                                            tMag.Nodes.Add(t2);
                                        }
                                        else if (long.Parse(drs[0]["id_vehicle"].ToString()) == ID_NEWSPAPERS)
                                        {
                                            t2.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li2.Type, li2.ID, li2.Text);
                                            tNews.Nodes.Add(t2);
                                        }
                                        else throw new Exception(" Impossible to identify MEDIA node ");

                                    }
                                    //Add Treenode magazine  
                                        
                                    if (tMag.Nodes.Count>0 || tNews.Nodes.Count==0)
                                        to.Nodes.Add(tMag);

                                    //Add Treenode newspaper
                                    else 
                                        toClone.Nodes.Add(tNews);
                                }

                                break;
                                #endregion
                            default:
                                string defaultmodule = " MODULE UNKNOWN";
                                break;

                        }
                    }
                    else
                    {
                        to.Nodes.Add(n);
                    }
                }
                //to.Nodes.Add(t);
                // ConvertSplitPressTag(n, t, webSession, mySessionId, propertyName);
            }

            switch (webSession.CurrentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    if ("CompetitorUniversMedia" == propertyName)
                    {
                        //Tree Mag
                        if (to.Nodes.Count > 0) treeListMag.Add(to);

                        //Tree News
                        if (toClone.Nodes.Count > 0) treeListNews.Add(toClone);
                    }
                    break;
            }
        }

        protected static void GetCompetitorUniverse(Hashtable CompetitorUniverse, WebSession webSession, long mySessionId, string propertyName)
        {
            Hashtable ht = new Hashtable(5);
            List<TreeNode> treeListMag = new List<TreeNode>(), treeListNews = new List<TreeNode>();
            if (webSession.CompetitorUniversMedia.Count > 0)
            {
                for (int i = 1; i <= webSession.CompetitorUniversMedia.Count; i++)
                {
                    TreeNode tree = (TreeNode)webSession.CompetitorUniversMedia[i];
                    tree = SplitPressTreenode(tree, webSession, mySessionId, "CompetitorUniversMedia", treeListMag, treeListNews);                  
                }

                DataSet ds = null;
                TreeNode t = null;
                TreeNode newTree = new TreeNode("media");
                newTree.Checked = false;

                if (treeListMag.Count > 0)
                {

                    ds = DAL.ImhotepDAL.GetVehicles("", ID_MAGAZINE.ToString(), webSession.SiteLanguage);
                    string vehicleLabel = ds.Tables[0].Rows[0]["vehicle"].ToString();
                    t = new TreeNode(vehicleLabel);
                    t.Checked = true;
                    t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, ID_MAGAZINE, vehicleLabel);
                    newTree.Nodes.Add(t);
                    webSession.SelectionUniversMedia = newTree;


                    //MAG Universe
                    for (int j = 0; j < treeListMag.Count; j++)
                    {
                        if (j == 0) webSession.CurrentUniversMedia = treeListMag[j];
                        ht.Add(j+1, treeListMag[j]);
                    }
                    webSession.CompetitorUniversMedia = ht;
                }
                else if (treeListNews.Count > 0)
                {
                    ds = DAL.ImhotepDAL.GetVehicles("", ID_NEWSPAPERS.ToString(), webSession.SiteLanguage);
                    string vehicleLabel = ds.Tables[0].Rows[0]["vehicle"].ToString();
                    t = new TreeNode(vehicleLabel);
                    t.Checked = true;
                    t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, ID_NEWSPAPERS, vehicleLabel);
                    newTree.Nodes.Add(t);
                    webSession.SelectionUniversMedia = newTree;

                    //News Universe
                    for (int j = 0; j < treeListNews.Count; j++)
                    {
                        if (j == 0) webSession.CurrentUniversMedia = treeListNews[j];
                        ht.Add(j+1, treeListNews[j]);
                    }
                    webSession.CompetitorUniversMedia = ht;
                }
            }
            else
            {
                webSession.CurrentUniversMedia = SplitPressTreenode(webSession.CurrentUniversMedia, webSession, mySessionId, "CurrentUniversMedia");
                webSession.SelectionUniversMedia = SplitPressTreenode(webSession.SelectionUniversMedia, webSession, mySessionId, "SelectionUniversMedia");
            }
        }
        private static bool IsPressMediaType(TNS.AdExpress.Constantes.Customer.Right.type type, WebSession webSession, long mySessionId, long id, string propertyName)
        {
            switch (type)
            {

                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess:
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleException:
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap:
                    return (ID_PRESS == id);
                default: return false;
            }
        }
        #endregion

        #region Conversion d'un TreeNode des anciennes sessions vers un treenode temporaire
        public static TreeNode ConvertOldTreeToTmp(TreeNode oldTree, WebSession webSession, long mySessionId, string propertyName)
        {
            TreeNode newTree = null;
            DataSet ds = null;

            if (oldTree != null)
            {
                //if (TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR == webSession.CurrentModule
                //    || TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE == webSession.CurrentModule)
                //{
                //    string catList = webSession.GetSelection(oldTree, TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess);
                //    if (string.IsNullOrEmpty(catList)) catList = webSession.GetSelection(oldTree, TNS.AdExpress.Constantes.Customer.Right.type.categoryException);
                //    if (!string.IsNullOrEmpty(catList))
                //        ds = DAL.ImhotepDAL.GetVehicles(catList, webSession.SiteLanguage);
                //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 1)
                //    {
                //        Console.WriteLine(" ERROR ! Les catégories appartiennent à plus de 1 Media pour la  session " + mySessionId + "  contient le media PRESS pour le module " + GetModuleName(webSession.CurrentModule) + " pour propriété " + propertyName);
                //    }
                //}
                newTree = new TreeNode(oldTree.Text);
                newTree.Checked = oldTree.Checked;
                if (oldTree.Tag != null)
                {
                    TNS.AdExpress.Web.Core.Sessions.LevelInformation li = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)oldTree.Tag;
                    //newTree.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
                    SplitMediaType(li.Type, webSession, mySessionId, li.ID, propertyName);
                }
                ConvertTagToTmp(oldTree, newTree, webSession, mySessionId, propertyName);
            }
            return newTree;
        }
        public static void ConvertTagToTmp(TreeNode from, TreeNode to, WebSession webSession, long mySessionId, string propertyName)
        {
            foreach (TreeNode n in from.Nodes)
            {
                TreeNode t = new TreeNode(n.Text);
                t.Checked = n.Checked;
                if (n.Tag != null)
                {
                    TNS.AdExpress.Web.Core.Sessions.LevelInformation li = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n.Tag;
                    // t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
                    SplitMediaType(li.Type, webSession, mySessionId, li.ID, propertyName);
                }
                to.Nodes.Add(t);
                ConvertTagToTmp(n, t, webSession, mySessionId, propertyName);
            }
        }


        #endregion

        #region Conversion dun TreeNode temporaire vers un TreeNode des nouvelles sessions
        //public static TreeNode ConvertTmpTreeToNew(TreeNode tmpTree){
        //    TreeNode newTree = null;
        //    if (tmpTree != null){
        //        newTree = new TreeNode(tmpTree.Text);
        //        newTree.Checked = tmpTree.Checked;
        //        if (tmpTree.Tag != null){
        //            TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation li = (TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation)tmpTree.Tag;
        //            tmpTree.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
        //        }
        //        ConvertTagToNew(tmpTree, newTree);
        //    }
        //    return newTree;
        //}
        //public static void ConvertTagToNew(TreeNode from, TreeNode to){
        //    foreach(TreeNode n in from.Nodes){
        //        TreeNode t = new TreeNode(n.Text);
        //        t.Checked = n.Checked;
        //        if (n.Tag != null){
        //            TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation li = (TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation)n.Tag;
        //            t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
        //        }
        //        to.Nodes.Add(t);
        //        ConvertTagToNew(n,t);
        //    }
        //}
        #endregion

        #region GetTypeException
        private static TNS.AdExpress.Constantes.Customer.Right.type GetTypeException(TNS.AdExpress.Constantes.Customer.Right.type type)
        {
            switch (type)
            {
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess:
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleException:
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap:
                    return TNS.AdExpress.Constantes.Customer.Right.type.vehicleException;

                case TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess:
                case TNS.AdExpress.Constantes.Customer.Right.type.mediaException:
                    return TNS.AdExpress.Constantes.Customer.Right.type.mediaException;


                case TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess:
                case TNS.AdExpress.Constantes.Customer.Right.type.categoryException:
                    return TNS.AdExpress.Constantes.Customer.Right.type.categoryException;
                default: throw new Exception(" GetTypeException : impossible to identify level type ");
            }
        }
        #endregion

        #region SplitMediaType
        private static void SplitMediaType(TNS.AdExpress.Constantes.Customer.Right.type type, WebSession webSession, long mySessionId, long id, string propertyName)
        {
            long pressDataBaseId = 2;
            switch (type)
            {
                case TNS.AdExpress.Constantes.Customer.Right.type.nothing:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.productAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.productException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.sectorException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.subSectorException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.groupAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.groupException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.segmentException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.advertiserException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess:
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleException:
                case TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap:
                    if (pressDataBaseId == id)
                    {
                        Console.WriteLine(" ATTENTION ! La session " + mySessionId + "  contient le media PRESS pour le module " + GetModuleName(webSession.CurrentModule) + " pour propriété " + propertyName);
                    }
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.categoryException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess:
                case TNS.AdExpress.Constantes.Customer.Right.type.mediaException:

                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.frequency:

                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.brandAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.brandException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.interestCenterAccess:
                case TNS.AdExpress.Constantes.Customer.Right.type.interestCenterException:

                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.aepmWaveAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.aepmWaveException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.aepmTargetAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.aepmTargetException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.aepmBaseTargetAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.aepmBaseTargetException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.programTypeAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.programTypeException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.programAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.programException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.sponsorshipFormAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.sponsorshipFormException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.basicMediaAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.basicMediaException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.regionException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.regionAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.subBrandAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.subBrandException:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.advertisementTypeAccess:
                    break;
                case TNS.AdExpress.Constantes.Customer.Right.type.advertisementTypeException:
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region GetModuleName
        private static string GetModuleName(long p)
        {
            switch (p)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA:
                    return " MEDIA SCHEDULE";
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE:
                    return " PRESENT ABSENT";
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE:
                    return " LOST WON ABSENT";
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE:
                    return " VEHICLE PORTFOLIO";
                case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE:
                    return " PRODUCT CLASS REPORTS";
                case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR:
                    return " PRODUCT CLASS GRAPHICS KEY";
                case TNS.AdExpress.Constantes.Web.Module.Name.TENDACES:
                    return " TRENDS REPORTS";
                case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PRESSE:
                    return " PRESS REPORTS";
                default:
                    return " MODULE UNKNOWN";

            }
        }

        #endregion

        #endregion

        #region Conversion d'un TreeNode des anciennes sessions vers un treenode temporaire
        //public static TreeNode ConvertOldTreeToTmp(TreeNode oldTree)
        //{
        //    TreeNode newTree = null;
        //    if (oldTree != null)
        //    {
        //        newTree = new TreeNode(oldTree.Text);
        //        newTree.Checked = oldTree.Checked;
        //        if (oldTree.Tag != null)
        //        {
        //            TNS.AdExpress.Web.Core.Sessions.LevelInformation li = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)oldTree.Tag;
        //            newTree.Tag = new TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
        //        }
        //        ConvertTagToTmp(oldTree, newTree);
        //    }
        //    return newTree;
        //}
        //public static void ConvertTagToTmp(TreeNode from, TreeNode to)
        //{
        //    foreach (TreeNode n in from.Nodes)
        //    {
        //        TreeNode t = new TreeNode(n.Text);
        //        t.Checked = n.Checked;
        //        if (n.Tag != null)
        //        {
        //            TNS.AdExpress.Web.Core.Sessions.LevelInformation li = (TNS.AdExpress.Web.Core.Sessions.LevelInformation)n.Tag;
        //            t.Tag = new TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
        //        }
        //        to.Nodes.Add(t);
        //        ConvertTagToTmp(n, t);
        //    }
        //}
        #endregion

        #region Conversion dun TreeNode temporaire vers un TreeNode des nouvelles sessions
        //public static TreeNode ConvertTmpTreeToNew(TreeNode tmpTree)
        //{
        //    TreeNode newTree = null;
        //    if (tmpTree != null)
        //    {
        //        newTree = new TreeNode(tmpTree.Text);
        //        newTree.Checked = tmpTree.Checked;
        //        if (tmpTree.Tag != null)
        //        {
        //            TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation li = (TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation)tmpTree.Tag;
        //            tmpTree.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
        //        }
        //        ConvertTagToNew(tmpTree, newTree);
        //    }
        //    return newTree;
        //}
        //public static void ConvertTagToNew(TreeNode from, TreeNode to)
        //{
        //    foreach (TreeNode n in from.Nodes)
        //    {
        //        TreeNode t = new TreeNode(n.Text);
        //        t.Checked = n.Checked;
        //        if (n.Tag != null)
        //        {
        //            TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation li = (TNS.AdExpress.Imhotep.Core.Sessions.LevelInformation)n.Tag;
        //            t.Tag = new TNS.AdExpress.Web.Core.Sessions.LevelInformation(li.Type, li.ID, li.Text);
        //        }
        //        to.Nodes.Add(t);
        //        ConvertTagToNew(n, t);
        //    }
        //}
        #endregion


    }
}
