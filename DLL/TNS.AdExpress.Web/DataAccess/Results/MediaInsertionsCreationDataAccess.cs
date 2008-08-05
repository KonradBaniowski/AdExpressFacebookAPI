#region Informations
// Auteur: G. Facon
// Date de cr�ation: 14/06/2004
// Date de modification:
//	22/07/2004	G. Ragneau
//	06/07/2005	K.Shehzad		Addition of Agglomeration colunm for Outdoor creations 
//	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
//	12/12/2005	D. V. Mussuma	Gestion du niveau de d�tail m�dia
//	19/12/2005	D. V. Mussuma	Modification totale de la requ�te pour int�gration des d�tails r�gies et accroches.
#endregion

#region Using
using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.FrameWork.DB.Common;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebDataAccess=TNS.AdExpress.Web.DataAccess;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using ConstantesCustomer=TNS.AdExpress.Constantes.Customer;
using WebFunctions = TNS.AdExpress.Web.Functions;
using  ConstantesFrameWork=TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Extrait le d�tail des insertions publicitaires dans un support, une cat�gorie, un m�dia correspondant
	/// � la s�lection utilisateur (nomenclatures produits et m�dia, p�riode) pr�sente dans une session
	/// </summary>
	public class MediaCreationDataAccess{

		#region M�thodes publiques

		#region D�tail insertion sans colonne g�n�riques

        #region GetAdNetTrackData
        /// <summary>
		/// Extrait le d�tail des insertions publicitaires dans un m�dia de niveau 1, 2,3,4 correspondant
		/// � la s�lection utilisateur (nomenclatures produits et m�dia, p�riode) pr�sente dans une session:
		///		Extraction de la table attaqu�e et des champs de s�lection � partir du vehicle
		///		Construction et ex�cution de la requ�te
		///		
		/// </summary>
		/// <param name="webSession">Session du client</param>		
		/// <param name="dateBegin">Date de d�but de l'�tude</param>
		/// <param name="dateEnd">Date de fin de l'�tude</param>
		/// <param name="mediaList">list des d�tails m�dias </param>
		/// <param name="idVehicle">Identifiant du m�dia (Vehicle) s�lectionn� par le client</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lanc�e quand on ne sait pas quelle table attaquer, quels champs s�lectionner ou quand une erreur Oracle s'est produite
		/// </exception>
		/// <returns>DataSet contenant le r�sultat de la requ�te</returns>
		/// <remarks>
		/// Utilise les m�thodes:
		///		- <code>private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail)</code> : obtient les champs de la requ�te.
		///		- <code>private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)</code> : obtient les tables de donn�es pour la requ�te
		///		- <code>private static void GetJoinConditions(WebSession webSession,StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd)</code> : obtient les jointures de la requ�te
		///		- <code> public static string TNS.AdExpress.Web.DataAccess.GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable)</code> : obtient liodre de tri des r�sultats
		/// </remarks>
		public static DataSet GetAdNetTrackData(WebSession webSession,ListDictionary mediaList,int dateBegin,int dateEnd,Int64 idVehicle){
			
			#region Variables
			StringBuilder sql=new StringBuilder(5000);
//			string currentIdMedia="";
			#endregion

			sql = GetRequest(webSession, mediaList, dateBegin, dateEnd, idVehicle);

			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException ("Impossible de charger pour les insertions: "+sql,err));
			}
			#endregion
        }
        #endregion

        #region GetMDData
        /// <summary>
        /// Extrait les versions du media marketing direct d�taill�es par cat�gorie/support/semaine
        ///		Extraction de la table attaqu�e et des champs de s�lection � partir du vehicle
        ///		Construction et ex�cution de la requ�te
        ///		
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="dateBegin">Date de d�but de l'�tude</param>
        /// <param name="dateEnd">Date de fin de l'�tude</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
        /// <param name="idVehicle">Identifiant du m�dia (Vehicle) s�lectionn� par le client</param>
        /// <param name="export">True s'il s'agit d'un export</param>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Lanc�e quand on ne sait pas quelle table attaquer, quels champs s�lectionner ou quand une erreur Oracle s'est produite
        /// </exception>
        /// <returns>DataSet contenant le r�sultat de la requ�te</returns>
        /// <remarks>
        /// Utilise les m�thodes:
        ///		- <code>private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail)</code> : obtient les champs de la requ�te.
        ///		- <code>private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)</code> : obtient les tables de donn�es pour la requ�te
        ///		- <code>private static void GetJoinConditions(WebSession webSession,StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd)</code> : obtient les jointures de la requ�te
        ///		- <code> public static string TNS.AdExpress.Web.DataAccess.GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable)</code> : obtient liodre de tri des r�sultats
        /// </remarks>
        public static DataSet GetMDData(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle, bool export) {

            #region Variables
            StringBuilder sql = new StringBuilder(5000);
            #endregion

            sql = GetRequest(webSession, mediaList, dateBegin, dateEnd, idVehicle, export);

            #region Execution de la requ�te
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new MediaCreationDataAccessException("Impossible de charger pour les insertions: " + sql, err));
            }
            #endregion
        }
        #endregion

        #region GetRequest
        /// <summary>
        /// Construit la requ�te sql pour la m�thode GetAdNetTrackData
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="dateBegin">Date de d�but de l'�tude</param>
        /// <param name="dateEnd">Date de fin de l'�tude</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
        /// <param name="idVehicle">Identifiant du m�dia (Vehicle) s�lectionn� par le client</param>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Lanc�e quand on ne sait pas quelle table attaquer, quels champs s�lectionner ou quand une erreur Oracle s'est produite
        /// </exception>
        /// <returns>string contenant la requ�te</returns>
        public static StringBuilder GetRequest(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle) { 
            return (GetRequest(webSession, mediaList, dateBegin, dateEnd, idVehicle, false));        
        }


        /// <summary>
        /// Construit la requ�te sql pour la m�thode GetAdNetTrackData
        /// </summary>
        /// <param name="webSession">Session du client</param>		
        /// <param name="dateBegin">Date de d�but de l'�tude</param>
        /// <param name="dateEnd">Date de fin de l'�tude</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
        /// <param name="idVehicle">Identifiant du m�dia (Vehicle) s�lectionn� par le client</param>
        /// <param name="export">True si la requ�te renvoie les donn�es pour un export</param>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
        /// Lanc�e quand on ne sait pas quelle table attaquer, quels champs s�lectionner ou quand une erreur Oracle s'est produite
        /// </exception>
        /// <returns>DataSet contenant le r�sultat de la requ�te</returns>
        /// <remarks>
        /// Utilise les m�thodes:
        ///		- <code>private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail)</code> : obtient les champs de la requ�te.
        ///		- <code>private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)</code> : obtient les tables de donn�es pour la requ�te
        ///		- <code>private static void GetJoinConditions(WebSession webSession,StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd)</code> : obtient les jointures de la requ�te
        ///		- <code> public static string TNS.AdExpress.Web.DataAccess.GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable)</code> : obtient liodre de tri des r�sultats
        /// </remarks>
        public static StringBuilder GetRequest(WebSession webSession, ListDictionary mediaList, int dateBegin, int dateEnd, Int64 idVehicle, bool export) {

            #region Variables
            string fields = "";
            string list = "";
            bool premier = true;
            StringBuilder sql = new StringBuilder(5000);
            #endregion

            try{

                #region Construction de la requ�te
                fields = GetFields((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE);

                // S�lection de la nomenclature Support
                sql.Append("select " + fields);

                // Tables // TODO ADNETTRACK ??
                sql.Append(" from ");
                sql.Append(GetTables((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), mediaList, webSession.PreformatedMediaDetail, webSession));

                // Conditions de jointure
                sql.Append(" Where ");
                GetJoinConditions(webSession, sql, (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), mediaList, DbTables.WEB_PLAN_PREFIXE, false);

                // P�riode
                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num>=" + dateBegin);
                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".date_media_num<=" + dateEnd);

                //Affiner univers version
                if (webSession.CurrentModule == WebConstantes.Module.Name.ALERTE_PLAN_MEDIA && webSession.SloganIdList != null && webSession.SloganIdList.Length > 0) {
                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_slogan in ( " + webSession.SloganIdList + " )  ");
                }

                // Gestion des s�lections et des droits
                #region Nomenclature Produit (droits)
                premier = true;
                //Droits en acc�s
                sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));

                #endregion

                // Niveau de produit
                sql.Append(SQLGenerator.getLevelProduct(webSession, DbTables.WEB_PLAN_PREFIXE, true));
                // Produit � exclure en radio
                sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DbTables.WEB_PLAN_PREFIXE, true, false));

                #region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection)

				// S�lection de Produits
				if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
					sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));


                #endregion

                #region Nomenclature Media (droits et s�lection)

                #region Droits
                // On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
                if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.adnettrack)
                    sql.Append(SQLGenerator.GetAdNetTrackMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
                else
                    sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, DbTables.WEB_PLAN_PREFIXE, true));
                
                //Droit detail spot � spot TNT
                if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.tv
                    && !webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_category != " + DBConstantes.Category.ID_DIGITAL_TV + "  ");

                #endregion

                #region S�lection m�dia client

                //Obtient la s�lection des m�dia en fonction du niveau de d�tail m�dia

                if (mediaList != null && mediaList.Count > 0) {

                    IEnumerator myEnumerator = mediaList.GetEnumerator();
                    foreach (DictionaryEntry de in mediaList) {

                        if (de.Value != null && de.Key != null && long.Parse(de.Value.ToString()) > -1) {

							if (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
                                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + " is null  "); //accroche ==0
                            else if (de.Key.ToString().Equals(DBConstantes.Fields.ID_VEHICLE) && de.Value.ToString().Equals(DBClassificationConstantes.Vehicles.names.internet.GetHashCode().ToString())
                                )
                            {
                                //Remplace identifiant internet par celui adnettrack
                                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode().ToString() + "  ");
                            }
                            else {
                                if (!de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN)
									|| (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && !de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)))
                                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + de.Value.ToString() + "  ");
                            }

                        }
                    }
                }


                #endregion

                #endregion

                // Clause marketing Direct
                if (int.Parse(idVehicle.ToString()) == (int)DBClassificationConstantes.Vehicles.names.directMarketing) {
                    sql.Insert(0, getMDSpecificField((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE, false, export));
                    sql.Append(GetMDGroupByFields((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE, true));
                    sql.Append(GetMDOrderByFields((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE));
                    sql.Append(getMDSpecificGroupBy((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), mediaList, webSession, DbTables.WEB_PLAN_PREFIXE, false, export));
                }
                else
                    // Ordre
                    sql.Append(" order by advertiser,product ");
                #endregion

            }
            catch (System.Exception err) {
                throw (new MediaCreationDataAccessException("Impossible de construire la requ�te", err));
            }

            return sql;
        }
        #endregion

        #endregion

        #region D�tail insertion avec colonnes g�n�riques
        /// <summary>
		/// Extrait le d�tail des insertions publicitaires dans un m�dia de niveau 1, 2,3,4 correspondant
		/// � la s�lection utilisateur (nomenclatures produits et m�dia, p�riode) pr�sente dans une session:
		///		Extraction de la table attaqu�e et des champs de s�lection � partir du vehicle
		///		Construction et ex�cution de la requ�te
		///		
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="mediaList">Liste des m�dias</param>
		/// <param name="dateBegin">Date de d�but</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="idVehicle">Identifiant du m�dia</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lanc�e quand on ne sait pas quelle table attaquer, quels champs s�lectionner ou quand une erreur Oracle s'est produite
		/// </exception>
		/// <returns>DataSet contenant la liste des insertions par m�dia</returns>		
		public static DataSet GetData(WebSession webSession,ListDictionary mediaList,int dateBegin,int dateEnd,Int64 idVehicle){
			
			#region Variables
			string list="";
			bool premier = true;
			StringBuilder sql=new StringBuilder(5000);
			ArrayList detailLevelList = new ArrayList();
			bool hasDetailLevelSelect=false;
			bool hasDetailLevelGroupBy=false;
			bool hasDetailLevelOrder=false;
			string selectedMediaList="";
			#endregion
			
			try {

				//Select
				sql.Append(" select ");
				GetSqlFields((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), sql, webSession, ref hasDetailLevelSelect,detailLevelList);			

				//Tables
				GetSqlTables((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), webSession, sql, detailLevelList);
			
				// Conditions de jointure
				sql.Append(" Where ");

				// P�riode
				sql.Append("  "+DbTables.WEB_PLAN_PREFIXE+".date_media_num>="+dateBegin);
				sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".date_media_num<="+dateEnd);

				//Affiner univers version
				if(webSession.CurrentModule == WebConstantes.Module.Name.ALERTE_PLAN_MEDIA && webSession.SloganIdList!=null && webSession.SloganIdList.Length>0){
					sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".id_slogan in ( "+webSession.SloganIdList+" )  ");

				}
				if(webSession.GenericInsertionColumns.GetSqlJoins(webSession.DataLanguage,DbTables.WEB_PLAN_PREFIXE,detailLevelList).Length>0)
				sql.Append("  "+webSession.GenericInsertionColumns.GetSqlJoins(webSession.DataLanguage,DbTables.WEB_PLAN_PREFIXE,detailLevelList));
				if(webSession.DetailLevel!=null)
					sql.Append("  "+webSession.DetailLevel.GetSqlJoins(webSession.DataLanguage,DbTables.WEB_PLAN_PREFIXE));							
				sql.Append("  "+webSession.GenericInsertionColumns.GetSqlContraintJoins());
				

			
				// Gestion des s�lections et des droits
				#region Nomenclature Produit (droits)
				premier=true;
				//Droits en acc�s
				sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));

				#endregion
			
				// Niveau de produit
				sql.Append(SQLGenerator.getLevelProduct(webSession,DbTables.WEB_PLAN_PREFIXE,true));
				// Produit � exclure en radio
				if(WebFunctions.Modules.IsSponsorShipTVModule(webSession))
					sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,DbTables.WEB_PLAN_PREFIXE,DbTables.WEB_PLAN_PREFIXE,DbTables.WEB_PLAN_PREFIXE,true));
				else
				sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DbTables.WEB_PLAN_PREFIXE,true,false));


				#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection) 

				// S�lection de Produits
			    if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                    sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));
				#endregion

				#region Nomenclature Media (droits et s�lection)

				#region Droits
				sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));
				//Droit detail spot � spot TNT
				if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.tv
					&& !webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
					sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + ".id_category != " + DBConstantes.Category.ID_DIGITAL_TV + "  ");
                #endregion

				#region S�lection m�dia client
				//obtient la s�lection des m�dia en fonction du niveau de d�tail m�dia
                if (mediaList != null && mediaList.Count > 0)
                {
                    IEnumerator myEnumerator = mediaList.GetEnumerator();
                    foreach (DictionaryEntry de in mediaList)
                    {
                        if (de.Value != null && de.Key != null && long.Parse(de.Value.ToString()) > -1)
                        {

							if (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
                                sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + " is null  "); //accroche ==0
                            else
                            {
                                if (!de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN)
									|| (de.Key.ToString().Equals(DBConstantes.Fields.ID_SLOGAN) && !de.Value.ToString().Equals("0") && webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)))
                                    sql.Append(" and " + DbTables.WEB_PLAN_PREFIXE + "." + de.Key.ToString() + "=" + de.Value.ToString() + "  ");
                            }

                        }
                    }
                }
				sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".id_category<>35  ");// Pas d'affichage de TV NAT th�matiques
				sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".id_vehicle="+idVehicle.ToString());
                if (webSession.SloganIdZoom > -1)
                {
                    sql.AppendFormat(" and wp.id_slogan={0}", webSession.SloganIdZoom);
                }
				//univers supports s�l�ctionn�	pour Parrainage
				if(webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES
					|| webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS){
					
					if(webSession.CurrentUniversMedia!=null && webSession.CurrentUniversMedia.Nodes.Count>0){
						selectedMediaList+=webSession.GetSelection(webSession.CurrentUniversMedia,CustomerRightConstante.type.mediaAccess)+",";				
					}
					if (selectedMediaList.Length>0)sql.Append(" and  "+DbTables.WEB_PLAN_PREFIXE+".id_media in ("+selectedMediaList.Substring(0,selectedMediaList.Length-1)+") ");
				}
				#endregion

				#endregion

				#region Nomenclature Emission
				if(webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES
					|| webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS){
						//S�lection des �missions
						sql.Append(WebFunctions.SQLGenerator.GetCustomerProgramSelection(webSession,DbTables.WEB_PLAN_PREFIXE,DbTables.WEB_PLAN_PREFIXE,true));
								
						//s�lection des formes de parrainages
						sql.Append(WebFunctions.SQLGenerator.GetCustomerSponsorshipFormSelection(webSession,DbTables.WEB_PLAN_PREFIXE,true));
				}
				#endregion

				//Group by
				GetSqlGroupByFields(webSession, sql, (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), ref hasDetailLevelGroupBy, detailLevelList);
				
				//Order by 
				GetSqlOrderFields(webSession, sql, (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), ref hasDetailLevelOrder, detailLevelList);
				
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException("Impossible de construire la requ�te",err));
			}


			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException ("Impossible de charger pour les insertions: "+sql,err));
			}
			#endregion
		}
		#endregion

		#region identification des m�dia (vehicles) � traiter
		
		/// <summary>
		/// Obtient le m�dia (vehicle) correspondant � la cat�gorie et/ou support s�lectionn�.
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="idCategory">Identifiant cat�gorie</param>
		/// <param name="idMedia">Identifiant support</param>
		/// <returns>M�dia (vehicle)</returns>
		internal static DataSet GetIdsVehicle(WebSession webSession,Int64 idCategory,Int64 idMedia){

			StringBuilder sql=new StringBuilder(500);			
			
			
			#region Construction de la requ�te
			
			sql.Append(" select distinct "+DbTables.VEHICLE_PREFIXE+".id_vehicle,vehicle from ");
			sql.Append(" "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.VEHICLE+" "+DbTables.VEHICLE_PREFIXE);			
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.CATEGORY+" "+DbTables.CATEGORY_PREFIXE);
			
			if(idMedia>-1){			
				sql.Append(" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DbTables.BASIC_MEDIA_PREFIXE);
				sql.Append(" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DbTables.MEDIA_PREFIXE);
			}
			
			sql.Append(" Where ");	
		
			//Langages
			sql.Append("  "+DbTables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and  "+DbTables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			if(idMedia>-1){
				sql.Append(" and "+DbTables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());			
				sql.Append(" and "+DbTables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			}

			//Jointures
			sql.Append("  and "+DbTables.VEHICLE_PREFIXE+".id_vehicle ="+DbTables.CATEGORY_PREFIXE+".id_vehicle");
			if(idMedia>-1){
				sql.Append("  and "+DbTables.BASIC_MEDIA_PREFIXE+".id_basic_media ="+DbTables.MEDIA_PREFIXE+".id_basic_media");
				sql.Append("  and "+DbTables.BASIC_MEDIA_PREFIXE+".id_category ="+DbTables.CATEGORY_PREFIXE+".id_category");
				sql.Append("  and "+DbTables.MEDIA_PREFIXE+".id_media= "+idMedia);
			}
			if(idCategory>-1)sql.Append(" and "+DbTables.CATEGORY_PREFIXE+".id_category ="+idCategory);
			
			
			//Activation
			sql.Append(" and "+DbTables.VEHICLE_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);			
			sql.Append(" and "+DbTables.CATEGORY_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);			
			if(idMedia>-1){
				sql.Append(" and "+DbTables.MEDIA_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);
				sql.Append(" and "+DbTables.BASIC_MEDIA_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);
			}

			sql.Append(" order by vehicle");
			#endregion

			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException ("Impossible d'identifier le m�dia : "+sql,err));
			}
			#endregion

		}
				

		/// <summary>
		/// Obtient le m�dia (vehicle) correspondant aux accroches.
		/// </summary>
		/// <param name="webSession">session du client</param>
		///<param name="idSlogan">identifiant accroche</param>	
		///<param name="idVehicle">Identifiant du m�dia</param>
		///<param name="tableName">Table de donn�es</param>
		///<param name="dateBegin">date de d�but</param>
		///<param name="dateEnd">date de fin</param>
		/// <returns>Identifiant M�dia (vehicle)</returns>
		internal static DataSet GetIdsVehicle(WebSession webSession,Int64 idSlogan,string tableName,string dateBegin,string dateEnd,string idVehicle){
			StringBuilder sql=new StringBuilder(500);	
			string list="";
			bool premier = true;
//			int positionUnivers=1;
//			string mediaList="";

			#region Construction de la requ�te
			//select
			sql.Append(" select count(ROWNUM) from ");
							
			sql.Append(" "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" "+DbTables.WEB_PLAN_PREFIXE);
			
			
			sql.Append(" Where ");	

			
			// P�riode
			sql.Append("  "+DbTables.WEB_PLAN_PREFIXE+".date_media_num>="+dateBegin);
			sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".date_media_num<="+dateEnd);

			sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".id_category<>35  ");// Pas d'affichage de TV NAT th�matiques
			sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".id_vehicle="+idVehicle+"  ");

			#region S�lection m�dia client	
			if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)) {							
				if(idSlogan==0){
					sql.Append(" and  "+DbTables.WEB_PLAN_PREFIXE+".id_slogan is null ");
				
				}else sql.Append(" and  "+DbTables.WEB_PLAN_PREFIXE+".id_slogan="+idSlogan);
			}	
			
			#endregion
		
			#region Nomenclature Produit (droits)
			premier=true;
			//Droits en acc�s
			sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));

			#endregion
			
			// Niveau de produit
			sql.Append(SQLGenerator.getLevelProduct(webSession,DbTables.WEB_PLAN_PREFIXE,true));

			
			//Cat�gorie exclusive du parrainage TV
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession))
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,DbTables.WEB_PLAN_PREFIXE,DbTables.WEB_PLAN_PREFIXE,DbTables.WEB_PLAN_PREFIXE,true));			
			else{
				// Produit � exclure en radio
				sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DbTables.WEB_PLAN_PREFIXE,true,false));
			}

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection) 

			// S�lection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));

			#region Ancienne version S�lection
			//// S�lection en acc�s
			//premier=true;
			//// HoldingCompany
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
			//if(list.Length>0){
			//    sql.Append(" and ((wp.id_holding_company in ("+list+") ");
			//    premier=false;
			//}
			//// Advertiser
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_advertiser in ("+list+") ");
			//    premier=false;
			//}
			//// Marque
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
			//if(list.Length>0) {
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_brand in ("+list+") ");
			//    premier=false;
			//}

			//// Product
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_product in ("+list+") ");
			//    premier=false;
			//}

			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorAccess);
			//if(list.Length>0){
			//    sql.Append(" and ((wp.id_sector in ("+list+") ");
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_subsector in ("+list+") ");
			//    premier=false;
			//}
			//// group
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_group_ in ("+list+") ");
			//    premier=false;
			//}

			//if(!premier) sql.Append(" )");
			
			//// S�lection en Exception
			//// HoldingCompany
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_holding_company not in ("+list+") ");
			//    premier=false;
			//}
			//// Advertiser
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_advertiser not in ("+list+") ");
			//    premier=false;
			//}
			//// brand
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandException);
			//if(list.Length>0) {
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_brand not in ("+list+") ");
			//    premier=false;
			//}
			//// Product
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_product not in ("+list+") ");
			//    premier=false;
			//}
			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_sector not in ("+list+") ");
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_subsector not in ("+list+") ");
			//    premier=false;
			//}
			//// Group
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_group_ not in ("+list+") ");
			//    premier=false;
			//}
			//if(!premier) sql.Append(" )");
			#endregion

			#endregion

			#region Nomenclature Media (droits et s�lection)

			#region Droits
			// On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.adnettrack)
				sql.Append(SQLGenerator.GetAdNetTrackMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));
			else
				sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));
			#endregion
		
			#endregion

			
			
			#endregion
			
			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException ("Impossible d'identifier le m�dia : "+sql,err));
			}
			#endregion


		}
	

		/// <summary>
		/// Obtient le m�dia (vehicle) correspondant au niveau de d�tail s�lectionn�
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="mediaImpactedList">Liste des m�dias impact�s</param>		
		///<param name="idVehicle">identifiant du m�dia</param>
		///<param name="tableName">Table de donn�es</param>
		///<param name="dateBegin">date de d�but</param>
		///<param name="dateEnd">date de fin</param>
		/// <returns>Identifiant M�dia (vehicle)</returns>
		internal static DataSet GetIdsVehicle(WebSession webSession,ListDictionary mediaImpactedList,string tableName,string dateBegin,string dateEnd,string idVehicle){
			StringBuilder sql=new StringBuilder(500);	
			string list="";
			bool premier = true;

			#region Construction de la requ�te
			//select
			sql.Append(" select count(ROWNUM) from ");
							
			sql.Append(" "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" "+DbTables.WEB_PLAN_PREFIXE);
			
			
			sql.Append(" Where ");	

			// P�riode
			sql.Append("  "+DbTables.WEB_PLAN_PREFIXE+".date_media_num>="+dateBegin);
			sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".date_media_num<="+dateEnd);

			#region S�lection m�dia client
			if(mediaImpactedList!=null && mediaImpactedList.Count>0){
				//obtient la s�lection des m�dia en fonction du niveau de d�tail m�dia
				IEnumerator myEnumerator = mediaImpactedList.GetEnumerator();				
				foreach (DictionaryEntry de in mediaImpactedList ){
					if(de.Key!=null && de.Value !=null && long.Parse(de.Value.ToString())>-1 && de.Key!=null && de.Key.ToString().Length>0){																
						if (de.Key.ToString().Equals(DBConstantes.Fields.ID_VEHICLE) && de.Value.ToString().Equals(DBClassificationConstantes.Vehicles.names.internet.GetHashCode().ToString()) 
							){
							//Remplace identifiant internet par celui adnettrack
							sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+"."+de.Key.ToString()+"="+DBClassificationConstantes.Vehicles.names.adnettrack.GetHashCode().ToString()+"  ");
						}
						else
							sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+"."+de.Key.ToString()+"="+de.Value.ToString()+"  ");						
					
					}				
				}
			}
			sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".id_category<>35  ");// Pas d'affichage de TV NAT th�matiques
			sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".id_vehicle="+idVehicle.ToString()); 

			#endregion
		
			#region Nomenclature Produit (droits)
			premier=true;
			//Droits en acc�s
			sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));

			#endregion
			
			// Niveau de produit
			sql.Append(SQLGenerator.getLevelProduct(webSession,DbTables.WEB_PLAN_PREFIXE,true));
			
			//Cat�gorie exclusive du parrainage TV
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession))
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,DbTables.WEB_PLAN_PREFIXE,DbTables.WEB_PLAN_PREFIXE,DbTables.WEB_PLAN_PREFIXE,true));
			else{
				// Produit � exclure en radio
				sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DbTables.WEB_PLAN_PREFIXE,true,false));
			}

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection) 

			// S�lection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.WEB_PLAN_PREFIXE, true));

			#region S�lection
			//// S�lection en acc�s
			//premier=true;
			//// HoldingCompany
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
			//if(list.Length>0){
			//    sql.Append(" and ((wp.id_holding_company in ("+list+") ");
			//    premier=false;
			//}
			//// Advertiser
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_advertiser in ("+list+") ");
			//    premier=false;
			//}
			//// Marque
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
			//if(list.Length>0) {
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_brand in ("+list+") ");
			//    premier=false;
			//}

			//// Product
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_product in ("+list+") ");
			//    premier=false;
			//}

			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorAccess);
			//if(list.Length>0){
			//    sql.Append(" and ((wp.id_sector in ("+list+") ");
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_subsector in ("+list+") ");
			//    premier=false;
			//}
			//// group
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_group_ in ("+list+") ");
			//    premier=false;
			//}

			//if(!premier) sql.Append(" )");
			
			//// S�lection en Exception
			//// HoldingCompany
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_holding_company not in ("+list+") ");
			//    premier=false;
			//}
			//// Advertiser
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_advertiser not in ("+list+") ");
			//    premier=false;
			//}
			//// brand
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandException);
			//if(list.Length>0) {
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_brand not in ("+list+") ");
			//    premier=false;
			//}
			//// Product
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_product not in ("+list+") ");
			//    premier=false;
			//}
			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.sectorException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_sector not in ("+list+") ");
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.subSectorException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_subsector not in ("+list+") ");
			//    premier=false;
			//}
			//// Group
			//list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.groupException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_group_ not in ("+list+") ");
			//    premier=false;
			//}
			//if(!premier) sql.Append(" )");
			#endregion

			#endregion

			#region Nomenclature Media (droits et s�lection)

			#region Droits
			// On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.adnettrack)
				sql.Append(SQLGenerator.GetAdNetTrackMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));
			else
			sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true));
			#endregion
			
			
			#endregion

			
			#endregion
			
			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException ("Impossible d'identifier le m�dia : "+sql,err));
			}
			#endregion


		}

		/// <summary>
		/// Obtient le m�dia (vehicle) correspondant aux accroches.
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idSlogan">Identifiant accroche</param>	
		/// <returns>Identifiant M�dia (vehicle)</returns>
		internal static DataSet GetIdsVehicle(WebSession webSession,Int64 idSlogan){
			StringBuilder sql=new StringBuilder(500);				

			#region Construction de la requ�te
			
			sql.Append(" select "+DbTables.SLOGAN_PREFIXE+".id_vehicle,vehicle from ");	
			sql.Append(" "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.VEHICLE+" "+DbTables.VEHICLE_PREFIXE);			
			sql.Append(","+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DbTables.SLOGAN+" "+DbTables.SLOGAN_PREFIXE);						
			sql.Append(" Where ");	
			sql.Append("  "+DbTables.SLOGAN_PREFIXE+".id_slogan= "+idSlogan);
			sql.Append("  and "+DbTables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append("  and "+DbTables.SLOGAN_PREFIXE+".id_language= "+webSession.DataLanguage.ToString());
			sql.Append("  and "+DbTables.VEHICLE_PREFIXE+".id_vehicle ="+DbTables.SLOGAN_PREFIXE+".id_vehicle");
			sql.Append(" and "+DbTables.VEHICLE_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);			
			sql.Append("  and  "+DbTables.SLOGAN_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);	
			sql.Append(" order by vehicle ");
			#endregion
			
			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaCreationDataAccessException ("Impossible d'identifier le m�dia : "+sql,err));
			}
			#endregion
		}
		
		#endregion

		#endregion		

		#region M�thodes priv�es
				
		#region Get Fields

		/// <summary>
		/// Obetient les champs de la requ�te
		/// </summary>
		/// <param name="webSession"></param>
		/// <returns></returns>
		private static string GetSqlFields(WebSession webSession) {
			string sql = "";
			IList tempDetailLevelSqlFields = null, tempInsertionColumnsSqlFields = null;

			if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlFields().Length > 0) {
				tempDetailLevelSqlFields = webSession.DetailLevel.GetSqlFields().Split(',');
				sql = "  " + webSession.DetailLevel.GetSqlFields();
			}
			if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlFields().Length > 0) {

				if (tempDetailLevelSqlFields != null && tempDetailLevelSqlFields.Count > 0) {
					tempInsertionColumnsSqlFields = webSession.GenericInsertionColumns.GetSqlFields().Split(',');
					for (int i = 0; i < tempInsertionColumnsSqlFields.Count; i++) {

						if (!tempDetailLevelSqlFields.Contains(tempInsertionColumnsSqlFields[i].ToString()))
							sql += " ," + tempInsertionColumnsSqlFields[i].ToString();
					}
				}
				else sql = webSession.GenericInsertionColumns.GetSqlFields();
			}

			return sql;
		}

		/// <summary>
		/// Donne les champs � traiter pour le d�tail des insertions.
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
		/// <param name="webSesssion">Session  du client</param>
		/// <param name="prefixeMediaPlanTable">prfixe table m�dia (vehicle)</param>
		/// <returns>Chaine contenant les champs � traiter</returns>
        private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable){
			string sql="";
					
			
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					
					
					sql="  "+prefixeMediaPlanTable+".date_media_Num"
						+", "+prefixeMediaPlanTable+".media_paging"
						+", group_"
						+", advertiser"
						+", product"
						+", format"
						+", "+prefixeMediaPlanTable+".area_page"
						+", color"
						+", "+prefixeMediaPlanTable+".expenditure_euro"
						+", location"
						+", "+prefixeMediaPlanTable+".visual "
						+", "+prefixeMediaPlanTable+".id_advertisement"
						+", "+DbTables.APPLICATION_MEDIA_PREFIXE+".disponibility_visual"
						+", "+DbTables.APPLICATION_MEDIA_PREFIXE+".activation";
//						+", "+prefixeMediaPlanTable+".id_slogan";
						sql+=" ,"+GetMediaFields(webSesssion,webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
                        sql += ", " + prefixeMediaPlanTable + ".date_cover_Num";
					return sql;

				case DBClassificationConstantes.Vehicles.names.radio:
					
					
						sql=" "+prefixeMediaPlanTable+".date_media_num"
						+", "+prefixeMediaPlanTable+".id_top_diffusion"
						+", "+prefixeMediaPlanTable+".associated_file"
						+", advertiser"
						+", product"
						+", group_"
						+", "+prefixeMediaPlanTable+".duration"
						+", "+prefixeMediaPlanTable+".rank"
						+", "+prefixeMediaPlanTable+".duration_commercial_break"
						+", "+prefixeMediaPlanTable+".number_spot_com_break"
						+", "+prefixeMediaPlanTable+".rank_wap"
						+", "+prefixeMediaPlanTable+".duration_com_break_wap"
						+", "+prefixeMediaPlanTable+".number_spot_com_break_wap"
						+", "+prefixeMediaPlanTable+".expenditure_euro"
						+", "+prefixeMediaPlanTable+".id_cobranding_advertiser";
					sql+=" ,"+GetMediaFields(webSesssion,webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
					return sql;

				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					
					
						sql=" "+prefixeMediaPlanTable+".date_media_num"
						+", "+prefixeMediaPlanTable+".top_diffusion"
						+", "+prefixeMediaPlanTable+".associated_file"
						+", advertiser"
						+", product"
						+", group_"
						+", "+prefixeMediaPlanTable+".duration"
						+", "+prefixeMediaPlanTable+".id_rank"
						+", "+prefixeMediaPlanTable+".duration_commercial_break"
						+", "+prefixeMediaPlanTable+".number_message_commercial_brea"
						+", "+prefixeMediaPlanTable+".expenditure_euro"
						+", "+prefixeMediaPlanTable+".id_commercial_break";
					sql+=" ,"+GetMediaFields(webSesssion,webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
					return sql;

				case DBClassificationConstantes.Vehicles.names.outdoor:					
					
						sql=" "+prefixeMediaPlanTable+".date_media_num"						
						+", advertiser"
						+", product"
						+", group_"
						+", "+prefixeMediaPlanTable+".number_board"
						+", "+prefixeMediaPlanTable+".type_board"
						+", "+prefixeMediaPlanTable+".type_sale"
						+", "+prefixeMediaPlanTable+".poster_network"
						+", "+DbTables.AGGLOMERATION_PREFIXE+".agglomeration"																	
						+", "+prefixeMediaPlanTable+".expenditure_euro";
					sql+=" ,"+GetMediaFields(webSesssion,webSesssion.PreformatedMediaDetail,prefixeMediaPlanTable);
					return sql;
				case DBClassificationConstantes.Vehicles.names.adnettrack:
					sql=" distinct "+prefixeMediaPlanTable+".hashcode,"
					  +" "+prefixeMediaPlanTable+".ASSOCIATED_FILE,"
					+" "+prefixeMediaPlanTable+".dimension,"
					+" "+prefixeMediaPlanTable+".format,"					
					+" "+prefixeMediaPlanTable+".url,product,"+prefixeMediaPlanTable+".id_product,advertiser,"+prefixeMediaPlanTable+".id_advertiser";
				
				
					return sql;

                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    sql = "  " + DbTables.CATEGORY_PREFIXE + ".id_category, category"
                       + ", " + DbTables.MEDIA_PREFIXE + ".id_media, media"
                       + ", " + prefixeMediaPlanTable + ".date_media_num"
                       + ", " + DbTables.ADVERTISER_PREFIXE + ".id_advertiser, advertiser"
                       + ", " + DbTables.PRODUCT_PREFIXE + ".id_product, product"
                       + ", " + DbTables.GROUP_PREFIXE + ".id_group_, group_"
                       + ", " + prefixeMediaPlanTable + ".weight"
                       + ", " + prefixeMediaPlanTable + ".associated_file"
                       + ", sum(" + prefixeMediaPlanTable + ".expenditure_euro) as expenditure_euro"
                       + ", sum(" + prefixeMediaPlanTable + ".volume) as volume"
                       + ", " + prefixeMediaPlanTable + ".id_slogan";
                       
                    sql += GetMDFields(idVehicle, mediaList, webSesssion, prefixeMediaPlanTable,true); 
                    return sql;

				default:
					throw new Exceptions.MediaCreationDataAccessException("GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion) :: Le cas de ce m�dia n'est pas g�rer. Pas de table correspondante.");
			}
		}

        /// <summary>
		/// Donne les champs � traiter pour les cr�ations du Marketing Direct.
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
		/// <param name="webSesssion">Session  du client</param>
		/// <param name="prefixeMediaPlanTable">prfixe table m�dia (vehicle)</param>
        /// <param name="withPrefix">Ajouter des pr�fixes ou non</param>
		/// <returns>Chaine contenant les champs � traiter</returns>
        private static string GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix){
        
            string sql = "";

            if (mediaList["id_media"] != null && mediaList["id_media"].ToString()!= "-1"){

                switch (mediaList["id_media"].ToString()) {

                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql = ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + ".mail_format as " : "") + "wp_mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default :
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas � un support du MD.");
                }

            }
            else if (mediaList["id_category"] != null && mediaList["id_category"].ToString()!="-1"){

                switch (mediaList["id_category"].ToString()){

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                        sql = ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                        sql = ", " + (withPrefix ? prefixeMediaPlanTable + ".mail_format as " : "") + "wp_mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette cat�gorie ne correspond pas � une cat�gorie du MD.");
                }

            }
            else { 
            
                sql = ",  format, "
                    + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                    + "mail_type,"
                    + (withPrefix ? prefixeMediaPlanTable + ".mail_format as " : "") + "wp_mail_format,"
                    + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                    + " mailing_rapidity" + (withPrefix ? "," : "")
                    + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                    + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
            
            }

            return sql;

        }

		/// <summary>
		/// Donne les champs � traiter pour le d�tail des insertions.
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia</param>
		/// <param name="webSesssion">Session  du client</param>
		/// <param name="hasDetailLevelSelect">Indique si des niveaux de d�taill�s ont �t� s�lectionn�s</param>
		/// <returns>Chaine contenant les champs � traiter</returns>
		private static void GetSqlFields(DBClassificationConstantes.Vehicles.names idVehicle, StringBuilder sql, WebSession webSession, ref bool hasDetailLevelSelect, ArrayList detailLevelList) {
			

			if (webSession.DetailLevel != null && webSession.DetailLevel.Levels != null && webSession.DetailLevel.Levels.Count > 0) {
				//detailLevelList = new ArrayList();
				foreach (DetailLevelItemInformation detailLevelItemInformation in webSession.DetailLevel.Levels) {
					detailLevelList.Add(detailLevelItemInformation.Id.GetHashCode());
				}
				if (webSession.DetailLevel.GetSqlFields().Length > 0) {
					sql.Append(" " + webSession.DetailLevel.GetSqlFields());
					hasDetailLevelSelect = true;
				}

			}

			//Ajoute des champs sp�cifiques � la Presse 
			if (( idVehicle == DBClassificationConstantes.Vehicles.names.press
				|| idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress
				)
				&& AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE).Length > 0) {
				sql.Append("," + AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE));
			}


			if (webSession.GenericInsertionColumns.GetSqlFields(detailLevelList).Length > 0) {
				if (hasDetailLevelSelect) sql.Append(" , ");
				sql.Append(" " + webSession.GenericInsertionColumns.GetSqlFields(detailLevelList));
			}

			if (webSession.GenericInsertionColumns.GetSqlConstraintFields().Length > 0)
				sql.Append(" , " + webSession.GenericInsertionColumns.GetSqlConstraintFields());//Champs pour la gestion des contraintes m�tiers

			AddSloganField(webSession, sql, idVehicle);

            if (webSession.GenericInsertionColumns.GetSqlFields(detailLevelList).Length > 0 && (idVehicle == DBClassificationConstantes.Vehicles.names.tv) && !webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.category) && !webSession.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.category))
            {
                sql.Append(" , " + DbTables.WEB_PLAN_PREFIXE + ".id_category");
            }
        }

		/// <summary>
		/// Obtient les champs correspondants au d�tail media demand� par le client.
		/// Les champs demand�es corespondent aux libell�s des niveaux de m�dia s�lectionn�s
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="preformatedMediaDetail">Niveau du d�tail media demand�</param>
		/// <param name="prefixeMediaPlanTable">prefixe table plan m�dia</param>
		/// <returns>Cha�ne contenant les champs</returns>
		public static string GetMediaFields(WebSession webSession,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail,string prefixeMediaPlanTable){


			if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG))
					return(" vehicle,"+prefixeMediaPlanTable+".id_category,category,"+prefixeMediaPlanTable+".id_media,media ,interest_center,media_seller,"+prefixeMediaPlanTable+".id_slogan");	
					else return (" vehicle,"+prefixeMediaPlanTable+".id_category,category,"+prefixeMediaPlanTable+".id_media,media,interest_center,media_seller ");	
		
		}

		#endregion
		
		#region GetTables
		/// <summary>
		///Obtient les tables correspondants au d�tail media demand�e par le client. 
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="sql">Chaine sql</param>
		/// <param name="detailLevelList">Liste des niveaux de d�tail</param>
		private static void GetSqlTables(DBClassificationConstantes.Vehicles.names idVehicle, WebSession webSession, StringBuilder sql, ArrayList detailLevelList) {
			
			string tableName = "";
			Module currentModuleDescription = ModulesList.GetModule(webSession.CurrentModule);
			if (WebFunctions.Modules.IsSponsorShipTVModule(webSession))
				tableName = DBConstantes.Tables.DATA_SPONSORSHIP;
			else tableName = SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle, currentModuleDescription.ModuleType);

			sql.Append(" from ");
			sql.Append(" " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " " + DbTables.WEB_PLAN_PREFIXE);
			if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0)
				sql.Append(" , " + webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA));
			if (webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA, detailLevelList).Length > 0) {
				sql.Append(" ," + webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA, detailLevelList));
			}
			if (webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0)
				sql.Append(" , " + webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA));//Tables pour la gestion des contraintes m�tiers
			
		}
		/// <summary>
		///Obtient les tables correspondants au d�tail media demand�e par le client. 
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia (vehicle)</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
		/// <param name="preformatedMediaDetail">Niveau de d�tail m�dia</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Cha�ne contenant les tables</returns>
        private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail, WebSession webSession){
			string sql="";
			string tableName="";
			Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
			tableName = SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle,currentModuleDescription.ModuleType);
			
			
			sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".advertiser   "+DbTables.ADVERTISER_PREFIXE;
			sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product   "+DbTables.PRODUCT_PREFIXE;		

			sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" "+DbTables.WEB_PLAN_PREFIXE;
			
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.others:
				case DBClassificationConstantes.Vehicles.names.tv:
					sql+=GetMediaTables(preformatedMediaDetail);
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GROUP_+ "  "+DbTables.GROUP_PREFIXE;
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
					sql+=GetMediaTables(preformatedMediaDetail);
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GROUP_+ "  "+DbTables.GROUP_PREFIXE;
					break;
				case DBClassificationConstantes.Vehicles.names.outdoor:
					sql+=GetMediaTables(preformatedMediaDetail);
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GROUP_+ "  "+DbTables.GROUP_PREFIXE;
					sql+="," +DBConstantes.Schema.ADEXPRESS_SCHEMA+".agglomeration  "+DbTables.AGGLOMERATION_PREFIXE;
					break;
				case DBClassificationConstantes.Vehicles.names.internationalPress:
				case DBClassificationConstantes.Vehicles.names.press:
					sql+=GetMediaTables(preformatedMediaDetail);
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GROUP_+ "  "+DbTables.GROUP_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.COLOR+" "+DbTables.COLOR_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.LOCATION+" "+DbTables.LOCATION_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_LOCATION+" "+DbTables.DATA_LOCATION_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.FORMAT+" "+DbTables.FORMAT_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.APPLICATION_MEDIA+" "+DbTables.APPLICATION_MEDIA_PREFIXE;
					break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + "  " + DbTables.GROUP_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.CATEGORY + "  " + DbTables.CATEGORY_PREFIXE;
                    sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MEDIA + "  " + DbTables.MEDIA_PREFIXE;
                    sql += GetMDTables(idVehicle, mediaList, preformatedMediaDetail, webSession);
                    break;
			}
						
			return sql;
		}

        		/// <summary>
		///Obtient les tables correspondants au d�tail media demand�e par le client. 
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia (vehicle)</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
		/// <param name="preformatedMediaDetail">Niveau de d�tail m�dia</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Cha�ne contenant les tables</returns>
        private static string GetMDTables(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail, WebSession webSession){

            string sql = "", cond="";

            if ((mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1") || (mediaList["id_category"] != null && mediaList["id_category"].ToString()!="-1")){

                if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
                    cond = mediaList["id_media"].ToString();
                else
                    cond = mediaList["id_category"].ToString();

                switch (cond){

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + "  " + DbTables.FORMAT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_FORMAT + "  " + DbTables.MAIL_FORMAT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_TYPE + "  " + DbTables.MAIL_TYPE_PREFIXE;
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAILING_RAPIDITY + "  " + DbTables.MAILING_RAPIDITY_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_CONTENT + "  " + DbTables.MAIL_CONTENT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_MAIL_CONTENT + "  " + DbTables.DATA_MAIL_CONTENT_PREFIXE;
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_CONTENT + "  " + DbTables.MAIL_CONTENT_PREFIXE;
                        sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_MAIL_CONTENT + "  " + DbTables.DATA_MAIL_CONTENT_PREFIXE;
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Le support ou la cat�gorie ne correspondent pas � un support ou une cat�gorie du MD.");
                }

            }
            else{

                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + "  " + DbTables.FORMAT_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_FORMAT + "  " + DbTables.MAIL_FORMAT_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_TYPE + "  " + DbTables.MAIL_TYPE_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAILING_RAPIDITY + "  " + DbTables.MAILING_RAPIDITY_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MAIL_CONTENT + "  " + DbTables.MAIL_CONTENT_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_MAIL_CONTENT + "  " + DbTables.DATA_MAIL_CONTENT_PREFIXE;
            }

            return sql;

        }
        	
		/// <summary>
		/// Obtient les tables m�dias correspondants au d�tail media demand�e par le client. 
		/// </summary>
		/// <param name="preformatedMediaDetail">Niveau du d�tail media demand�</param>
		/// <returns>Cha�ne contenant les tables m�dias</returns>
		public static string GetMediaTables(WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail){
			string sql="";
			
			//Vehicles							
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DbTables.VEHICLE_PREFIXE;								
			
			//Categories
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DbTables.CATEGORY_PREFIXE;					
					

			// Media							
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DbTables.MEDIA_PREFIXE;												

			// Interest center
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".interest_center "+DbTables.INTEREST_CENTER_PREFIXE;					


			// R�gie
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media_seller "+DbTables.MEDIA_SELLER_PREFIXE;										

	
			return(sql);
		}

		/// <summary>
		/// Obetient les tables de la requ�te
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>Tables</returns>
		private static string GetSqlTables(WebSession webSession) {
			string sql = "";
			IList tempDetailLevelSqlTables = null, tempInsertionColumnsSqlTables = null;

			if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0) {
				tempDetailLevelSqlTables = webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Split(',');
				sql = "  " + webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
			}
			if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length > 0) {

				if (tempDetailLevelSqlTables != null && tempDetailLevelSqlTables.Count > 0) {
					tempInsertionColumnsSqlTables = webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Split(',');
					for (int i = 0; i < tempInsertionColumnsSqlTables.Count; i++) {

						if (!tempDetailLevelSqlTables.Contains(tempInsertionColumnsSqlTables[i].ToString()))
							sql += " ," + tempInsertionColumnsSqlTables[i].ToString();
					}
				}
				else sql = webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
			}

			return sql;
		}
		#endregion		

		#region Jointures

		/// <summary>
		/// Obtient les jointures � utiliser lors d'un d�tail media
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="sql">requete sql</param>
		/// <param name="idVehicle">Identifiant m�dia (vehicle)</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
		/// <param name="dataTablePrefixe">prefixe table m�dia</param>
		/// <param name="beginByAnd">Vrai si la condition doit commenc�e par And</param>
		/// <returns>requete sql</returns>
        private static void GetJoinConditions(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, string dataTablePrefixe, bool beginByAnd){			

			if(beginByAnd) sql.Append(" and ");
			//produit
			sql.Append(" "+DbTables.PRODUCT_PREFIXE+".id_product="+dataTablePrefixe+".id_product ");
			sql.Append(" and "+DbTables.PRODUCT_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DbTables.PRODUCT_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);							

			// Annonceur
			sql.Append(" and "+DbTables.ADVERTISER_PREFIXE+".id_advertiser="+dataTablePrefixe+".id_advertiser ");
			sql.Append(" and "+DbTables.ADVERTISER_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DbTables.ADVERTISER_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);

			switch(idVehicle){

				case DBClassificationConstantes.Vehicles.names.others:
				case DBClassificationConstantes.Vehicles.names.tv:
					// Groupe
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_group_="+dataTablePrefixe+".id_group_ ");
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);

					sql.Append(GetMediaJoinConditions(webSession,dataTablePrefixe,true));
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
					sql.Append(GetMediaJoinConditions(webSession,dataTablePrefixe,true));
					// Groupe
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_group_="+dataTablePrefixe+".id_group_ ");
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);
					break;
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					sql.Append(GetMediaJoinConditions(webSession,dataTablePrefixe,true));
					// Groupe
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_group_="+dataTablePrefixe+".id_group_ ");
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);

					sql.Append(" and ("+DbTables.APPLICATION_MEDIA_PREFIXE+".id_media(+) = "+dataTablePrefixe+".id_media ");
                    sql.Append(" and " + DbTables.APPLICATION_MEDIA_PREFIXE + ".date_debut(+) = " + dataTablePrefixe + ".date_media_num ");
					sql.Append(" and "+DbTables.APPLICATION_MEDIA_PREFIXE+".id_project(+) = "+ CstProject.ADEXPRESS_ID +") ");
			
					sql.Append(" and "+DbTables.LOCATION_PREFIXE+".id_location (+)="+DbTables.DATA_LOCATION_PREFIXE+".id_location ");
					sql.Append(" and "+DbTables.LOCATION_PREFIXE+".id_language (+)="+webSession.DataLanguage.ToString());
				
					sql.Append(" and "+DbTables.DATA_LOCATION_PREFIXE+".id_advertisement (+)="+dataTablePrefixe+".id_advertisement ");
					sql.Append(" and "+DbTables.DATA_LOCATION_PREFIXE+".id_media (+)="+dataTablePrefixe+".id_media ");
                    sql.Append(" and " + DbTables.DATA_LOCATION_PREFIXE + ".date_media_num (+)=" + dataTablePrefixe + ".date_media_num ");
				

					sql.Append(" and "+DbTables.COLOR_PREFIXE+".id_color (+)="+dataTablePrefixe+".id_color ");
					sql.Append(" and "+DbTables.COLOR_PREFIXE+".id_language (+)="+webSession.DataLanguage.ToString());
			
					sql.Append(" and "+DbTables.FORMAT_PREFIXE+".id_format (+)="+dataTablePrefixe+".id_format ");
					sql.Append(" and "+DbTables.FORMAT_PREFIXE+".id_language (+)="+webSession.DataLanguage.ToString());
					break;
				case DBClassificationConstantes.Vehicles.names.outdoor:
					sql.Append(GetMediaJoinConditions(webSession,dataTablePrefixe,true));
					// Groupe
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_group_="+dataTablePrefixe+".id_group_ ");
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
					sql.Append(" and "+DbTables.GROUP_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);

					sql.Append(" and "+DbTables.AGGLOMERATION_PREFIXE+".id_agglomeration (+)="+dataTablePrefixe+".id_agglomeration ");				
					sql.Append(" and "+DbTables.AGGLOMERATION_PREFIXE+".id_language (+)="+webSession.DataLanguage.ToString());
					sql.Append(" and "+DbTables.AGGLOMERATION_PREFIXE+".activation (+)< "+ DBConstantes.ActivationValues.UNACTIVATED);
					break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    // Groupe
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_group_=" + dataTablePrefixe + ".id_group_ ");
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.GROUP_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                    // category
                    sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".id_category=" + dataTablePrefixe + ".id_category ");
                    sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.CATEGORY_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                    // Media
                    sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".id_media=" + dataTablePrefixe + ".id_media ");
                    sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".id_language=" + webSession.DataLanguage.ToString());
                    sql.Append(" and " + DbTables.MEDIA_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
                    GetMDJoinConditions(webSession, mediaList, sql, idVehicle, dataTablePrefixe, beginByAnd);
                    break;

			}
		}

        /// <summary>
		/// Obtient les jointures � utiliser lors d'un d�tail media
		/// </summary>
		/// <param name="webSession">Session client</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
		/// <param name="sql">requete sql</param>
		/// <param name="idVehicle">Identifiant m�dia (vehicle)</param>
		/// <param name="dataTablePrefixe">prefixe table m�dia</param>
		/// <param name="beginByAnd">Vrai si la condition doit commenc�e par And</param>
		/// <returns>requete sql</returns>
        private static void GetMDJoinConditions(WebSession webSession, ListDictionary mediaList, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, string dataTablePrefixe, bool beginByAnd){

            string cond ="";

            if ((mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1") || (mediaList["id_category"] != null && mediaList["id_category"].ToString()!="-1")){

                if (mediaList["id_media"] != null && mediaList["id_media"].ToString()!="-1")
                    cond = mediaList["id_media"].ToString();
                else
                    cond = mediaList["id_category"].ToString();

                switch (cond){

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_format (+)=" + dataTablePrefixe + ".id_format ");
                        sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_mail_format (+)=" + dataTablePrefixe + ".id_mail_format ");
                        sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_mail_type (+)=" + dataTablePrefixe + ".id_mail_type ");
                        sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_mailing_rapidity (+)=" + dataTablePrefixe + ".id_mailing_rapidity ");
                        sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        sql.Append(" and " + dataTablePrefixe + ".id_media =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_media (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".date_media_num =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".date_media_num (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_cobranding_advertiser =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_cobranding_advertiser (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_data_marketing_direct_panel =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_data_marketing_direct_panel (+) ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_mail_content (+) =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql.Append(" and " + dataTablePrefixe + ".id_media =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_media (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".date_media_num =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".date_media_num (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_cobranding_advertiser =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_cobranding_advertiser (+) ");
                        sql.Append(" and " + dataTablePrefixe + ".id_data_marketing_direct_panel =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_data_marketing_direct_panel (+) ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_mail_content (+) =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content ");
                        sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDJoinConditions(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, string dataTablePrefixe, bool beginByAnd) : Le support ou la cat�gorie ne correspondent pas � un support ou une cat�gorie du MD.");
                }

            }
            else{

                sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_format (+)=" + dataTablePrefixe + ".id_format ");
                sql.Append(" and " + DbTables.FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_mail_format (+)=" + dataTablePrefixe + ".id_mail_format ");
                sql.Append(" and " + DbTables.MAIL_FORMAT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_mail_type (+)=" + dataTablePrefixe + ".id_mail_type ");
                sql.Append(" and " + DbTables.MAIL_TYPE_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_mailing_rapidity (+)=" + dataTablePrefixe + ".id_mailing_rapidity ");
                sql.Append(" and " + DbTables.MAILING_RAPIDITY_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
                sql.Append(" and " + dataTablePrefixe + ".id_media =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_media (+) ");
                sql.Append(" and " + dataTablePrefixe + ".date_media_num =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".date_media_num (+) ");
                sql.Append(" and " + dataTablePrefixe + ".id_cobranding_advertiser =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_cobranding_advertiser (+) ");
                sql.Append(" and " + dataTablePrefixe + ".id_data_marketing_direct_panel =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_data_marketing_direct_panel (+) ");
                sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_mail_content (+) =" + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content ");
                sql.Append(" and " + DbTables.MAIL_CONTENT_PREFIXE + ".id_language (+)=" + webSession.DataLanguage.ToString());
            }


        }		
        
		/// <summary>
		/// Obtient les jointures m�dia � utiliser lors d'un d�tail media
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="dataTablePrefixe">Prefixe de la table de r�sultat</param>
		/// <param name="beginByAnd">Vrai si la condition doit commenc�e par And</param>
		/// <returns>Cha�ne contenant les tables</returns>
		private static string GetMediaJoinConditions(WebSession webSession,string dataTablePrefixe,bool beginByAnd){
			string tmp="";

			//Vehicles			
			if(beginByAnd)tmp+=" and ";
			tmp+="  "+DbTables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage;
			tmp+=" and "+DbTables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_vehicle="+dataTablePrefixe+".id_vehicle ";
					
			
			//Categories
			tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage;
			tmp+=" and "+DbTables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_category="+dataTablePrefixe+".id_category ";
			
			// Media			
			tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage;
			tmp+=" and "+DbTables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_media="+dataTablePrefixe+".id_media ";
				
			
			// Interest center

			tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.DataLanguage;
			tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center="+dataTablePrefixe+".id_interest_center ";


			// R�gie
			tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".id_language="+webSession.DataLanguage;
			tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller="+dataTablePrefixe+".id_media_seller ";



			//			if(tmp.Length==0)throw(new Exceptions.SQLGeneratorException("Le d�tail support demand� n'est pas valide"));
			//			if(!beginByAnd)tmp=tmp.Substring(4,tmp.Length-4);
			return(tmp);
		}
		#endregion		

		#region Champ sp�cifique � rajouter 
		/// <summary>
		/// Obtient un champ de donn�es sp�cifique pour la presse
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="wpPrefixe">prefixe table</param>
		/// <returns>Champ</returns>
        private static string AddPressSpecificField(WebSession webSession, string wpPrefixe)
        {

            string sql = "";
            ArrayList detailLevelList = new ArrayList();
            bool hasIdMedia = false;
            bool hasDateMediaNum = false;
            bool hasLocation = false;

            if (webSession.DetailLevel != null)
            {
                foreach (DetailLevelItemInformation detailLevelItemInformation in webSession.DetailLevel.Levels)
                {
                    if (detailLevelItemInformation.DataBaseIdField != null && detailLevelItemInformation.DataBaseIdField.Trim().Equals("id_media"))
                        hasIdMedia = true;
                    if (detailLevelItemInformation.DataBaseField != null && detailLevelItemInformation.DataBaseField.Trim().Equals("date_media_num"))
                        hasDateMediaNum = true;
                }
            }



            if (!hasIdMedia || !hasDateMediaNum || !hasLocation && webSession.GenericInsertionColumns != null)
            {
                foreach (GenericColumnItemInformation genericColumnItemInformation in webSession.GenericInsertionColumns.Columns)
                {
                    if (genericColumnItemInformation.DataBaseIdField != null && genericColumnItemInformation.DataBaseIdField.Trim().Equals("id_media"))
                        hasIdMedia = true;
                    if (genericColumnItemInformation.DataBaseField != null && genericColumnItemInformation.DataBaseField.Trim().Equals("date_media_num"))
                        hasDateMediaNum = true;
                    if ((GenericColumnItemInformation.Columns)genericColumnItemInformation.Id == GenericColumnItemInformation.Columns.location)
                        hasLocation = true;
                }

            }
            if ((webSession.DetailLevel != null && webSession.DetailLevel.Levels != null && webSession.DetailLevel.Levels.Count > 0)
                || (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.Columns != null && webSession.GenericInsertionColumns.Columns.Count > 0)

                )
            {
                if (!hasIdMedia)
                    sql = wpPrefixe + ".id_media";
                if (!hasDateMediaNum)
                {
                    if (sql.Length > 0) sql += ",";
                    sql += wpPrefixe + ".date_media_num";
                }
                if (!hasLocation)
                {
                }

            }
            if (sql.Length > 0) sql += ",";
            sql += wpPrefixe + ".date_cover_num";

            return sql;


        }

		/// <summary>
		/// Ajoute le champ slogan si n�cessaire
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="sql">Chaine de caract�res</param>
		/// <param name="idVehicle">Identifiant du m�dia</param>
		private static void AddSloganField(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle) {
			//Ajoute l'identifiant (uniquement pour la radio) de la version qui sera n�cessaire pour construire le chemin du fichier audio de la radio
			if (webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) && idVehicle == DBClassificationConstantes.Vehicles.names.radio &&
				!webSession.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.slogan) && !webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.slogan)
				) sql.Append(" ," + DbTables.WEB_PLAN_PREFIXE + ".id_slogan ");
		}

        /// <summary>
        /// Donne les champs � traiter pour les cr�ations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table m�dia (vehicle)</param>
        /// <returns>Chaine contenant les champs � traiter</returns>
        private static string getMDSpecificField(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix, bool export) {

            string sql = string.Empty;
            string cond = string.Empty;

            sql = "  select id_category, category"
                + ", id_media, media"
                + (!export ? ", date_media_num" : ", min(date_media_num) as date_media_num")
                + ", id_advertiser, advertiser"
                + ", id_product, product"
                + ", id_group_, group_"
                + ", weight"
                + ", associated_file"
                + (!export ? ", expenditure_euro" : ", sum(expenditure_euro) as expenditure_euro")
                + (!export ? ", volume" : ", sum(volume) as volume")
                + ", id_slogan";
            sql += GetMDFields(idVehicle, mediaList, webSesssion, prefixeMediaPlanTable, withPrefix);

            if ((mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1") || (mediaList["id_category"] != null && mediaList["id_category"].ToString() != "-1")){

                if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1")
                    cond = mediaList["id_media"].ToString();
                else
                    cond = mediaList["id_category"].ToString();
            }

            if(cond != DBConstantes.Media.PUBLICITE_NON_ADRESSEE && cond != DBConstantes.Category.PUBLICITE_NON_ADRESSEE){

                sql += ",max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.LETTRE_ACCOMP_PERSONALIS + "',MAIL_CONTENT)) as item1,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ENV_RETOUR_PRE_IMPRIMEE + "',MAIL_CONTENT)) as item2,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ENV_RETOUR_A_TIMBRER + "',MAIL_CONTENT)) as item3,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.COUPON_DE_REDUCTION + "',MAIL_CONTENT)) as item4,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ECHANTILLON + "',MAIL_CONTENT)) as item5,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.BON_DE_COMMANDE + "',MAIL_CONTENT)) as item6,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.JEUX_CONCOUR + "',MAIL_CONTENT)) as item7,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.CATALOGUE_BROCHURE + "',MAIL_CONTENT)) as item8,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.CADEAU + "',MAIL_CONTENT)) as item9,"
                 + "max(decode(MAIL_CONTENT,'" + DBConstantes.MailContent.ACCELERATEUR_REPONSE + "',MAIL_CONTENT)) as item10";
            }
            sql += " from (";
            return sql;

        }

        /// <summary>
        /// Donne les champs � traiter pour les cr�ations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table m�dia (vehicle)</param>
        /// <returns>Chaine contenant les champs � traiter</returns>
        private static string getMDSpecificGroupBy(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix, bool export){

            string sql = string.Empty;

            sql = " ) Group by id_category, category"
                + ", id_media, media"
                + (!export ? ", date_media_num" : "")
                + ", id_advertiser, advertiser"
                + ", id_product, product"
                + ", id_group_, group_"
                + ", weight"
                + ", associated_file"
                + (!export ? ", expenditure_euro" : "")
                + (!export ? ", volume" : "")
                + ", id_slogan";
            sql += GetMDGroupByFields(idVehicle, mediaList, webSesssion, prefixeMediaPlanTable, withPrefix);
            return sql;

        }
		#endregion

		#region Get Group By
		/// <summary>
		/// Regroupe les donn�es de la requ�te
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="sql">Chaine sql</param>
		/// <param name="idVehicle">Identifiant du m�dia</param>
		/// <param name="hasDetailLevelGroupBy">Indique s'il faut regrouper les niveaux de d�tail</param>
		/// <param name="detailLevelList">Liste des niveaux de d�tail</param>
		private static void GetSqlGroupByFields(WebSession webSession, StringBuilder sql, DBClassificationConstantes.Vehicles.names idVehicle, ref bool hasDetailLevelGroupBy, ArrayList detailLevelList) {
			
			sql.Append("  group by");
			if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlGroupByFields().Length > 0) {
				sql.Append("  " + webSession.DetailLevel.GetSqlGroupByFields());
				hasDetailLevelGroupBy = true;
			}
			if (webSession.GenericInsertionColumns.GetSqlGroupByFields(detailLevelList).Length > 0) {
				if (hasDetailLevelGroupBy) sql.Append(",");
				sql.Append("  " + webSession.GenericInsertionColumns.GetSqlGroupByFields(detailLevelList));
			}
			if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields() != null && webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields().Length > 0) {
				if (webSession.GenericInsertionColumns.GetSqlGroupByFields().Length > 0) sql.Append("  , ");
				sql.Append("  " + webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields());//contraintre regroupement des donn�es
			}

			if ((idVehicle == DBClassificationConstantes.Vehicles.names.press
				|| idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress
				) && AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE).Length > 0) {
				sql.Append("," + AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE));
			}

			AddSloganField(webSession, sql, idVehicle);

            if (webSession.GenericInsertionColumns.GetSqlFields(detailLevelList).Length > 0 && (idVehicle == DBClassificationConstantes.Vehicles.names.tv) && !webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.category) && !webSession.DetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.category))
            {
                sql.Append(" , " + DbTables.WEB_PLAN_PREFIXE + ".id_category");
            }

        }

        /// <summary>
        /// Donne les champs � traiter pour les cr�ations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table m�dia (vehicle)</param>
        /// <returns>Chaine contenant les champs � traiter</returns>
        private static string GetMDGroupByFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable, bool withPrefix){

            string sql = (withPrefix ? "group by " + DbTables.CATEGORY_PREFIXE + ".id_category, category, " + DbTables.MEDIA_PREFIXE + ".id_media, media, " + prefixeMediaPlanTable + ".date_media_num, " + DbTables.ADVERTISER_PREFIXE + ".id_advertiser, advertiser, " + DbTables.PRODUCT_PREFIXE + ".id_product, product, " + DbTables.GROUP_PREFIXE + ".id_group_, group_, " + prefixeMediaPlanTable + ".weight, " + prefixeMediaPlanTable + ".associated_file," + prefixeMediaPlanTable + ".id_slogan" : "");

            if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1"){

                switch (mediaList["id_media"].ToString()){

                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql += ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + "." : "wp_") + "mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + ".object_count," : "object_count")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas � un support du MD.");
                }

            }
            else if (mediaList["id_category"] != null && mediaList["id_category"].ToString()!="-1"){

                switch (mediaList["id_category"].ToString()){

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                        sql += ",  format, "
                            + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                        sql += ", " + (withPrefix ? prefixeMediaPlanTable + "." : "wp_") + "mail_format,"
                            + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                            + " mailing_rapidity" + (withPrefix ? "," : "")
                            + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                            + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette cat�gorie ne correspond pas � une cat�gorie du MD.");
                }

            }
            else{

                sql += ",  format, "
                    + (withPrefix ? DbTables.MAIL_FORMAT_PREFIXE + "." : "") + "mail_format, "
                    + "mail_type,"
                    + (withPrefix ? prefixeMediaPlanTable + "." : "wp_") + "mail_format,"
                    + " " + (withPrefix ? prefixeMediaPlanTable + "." : "") + "object_count,"
                    + " mailing_rapidity" + (withPrefix ? "," : "")
                    + " " + (withPrefix ? DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content," : "")
                    + " " + (withPrefix ? DbTables.MAIL_CONTENT_PREFIXE + ".mail_content" : "");

            }

            return sql;

        }
		#endregion

		#region Get Sql order fields

		/// <summary>
		/// Obtient les chaines pour ordonner les donn�es
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="sql">Chaine sql</param>
		/// <param name="idVehicle">Idnetifiant du m�dia</param>
		/// <param name="hasDetailLevelOrder">Indique s'il faut ordonner les donn�es</param>
		/// <param name="detailLevelList">Liste des niveaux de d�tail</param>
		private static void GetSqlOrderFields(WebSession webSession, StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle, ref bool hasDetailLevelOrder, ArrayList detailLevelList) {
			sql.Append("  order by ");
			if (webSession.DetailLevel != null && webSession.DetailLevel.GetSqlOrderFields().Length > 0) {
				sql.Append("  " + webSession.DetailLevel.GetSqlOrderFields());
				hasDetailLevelOrder = true;
			}
			if (webSession.GenericInsertionColumns.GetSqlOrderFields(detailLevelList).Length > 0) {
				if (hasDetailLevelOrder) sql.Append(",");
				sql.Append("  " + webSession.GenericInsertionColumns.GetSqlOrderFields(detailLevelList));
			}
			if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.GetSqlConstraintOrderFields() != null && webSession.GenericInsertionColumns.GetSqlConstraintOrderFields().Length > 0) {
				if (webSession.GenericInsertionColumns.GetSqlOrderFields().Length > 0) sql.Append("  , ");
				sql.Append("  " + webSession.GenericInsertionColumns.GetSqlConstraintOrderFields());//contraintre ordre des donn�es
			}

			if ((idVehicle == DBClassificationConstantes.Vehicles.names.press
				|| idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress
				) && AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE).Length > 0) {
				sql.Append("," + AddPressSpecificField(webSession, DbTables.WEB_PLAN_PREFIXE));
			}

			AddSloganField(webSession, sql, idVehicle);
		}

        /// <summary>
        /// Donne les champs � traiter pour les cr�ations du Marketing Direct.
        /// </summary>
        /// <param name="idVehicle">Identifiant du m�dia</param>
        /// <param name="mediaList">list des d�tails m�dias </param>
        /// <param name="webSesssion">Session  du client</param>
        /// <param name="prefixeMediaPlanTable">prfixe table m�dia (vehicle)</param>
        /// <returns>Chaine contenant les champs � traiter</returns>
        private static string GetMDOrderByFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable){

            string sql = " Order by " + DbTables.CATEGORY_PREFIXE + ".id_category, category, " + DbTables.MEDIA_PREFIXE + ".id_media, media, " + prefixeMediaPlanTable + ".date_media_num, " + DbTables.ADVERTISER_PREFIXE + ".id_advertiser, advertiser, " + DbTables.PRODUCT_PREFIXE + ".id_product, product, " + DbTables.GROUP_PREFIXE + ".id_group_, group_," + prefixeMediaPlanTable + ".associated_file," + prefixeMediaPlanTable + ".id_slogan";

            if (mediaList["id_media"] != null && mediaList["id_media"].ToString() != "-1"){

                switch (mediaList["id_media"].ToString()){

                    case DBConstantes.Media.PUBLICITE_NON_ADRESSEE:
                        sql += ",  format, "
                            + DbTables.MAIL_FORMAT_PREFIXE + ".mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GENERAL:
                        sql += ", " + prefixeMediaPlanTable + ".mail_format,"
                            + " " + prefixeMediaPlanTable + ".object_count,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_PRESSE:
                        sql += ", " + prefixeMediaPlanTable + ".object_count,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    case DBConstantes.Media.COURRIER_ADRESSE_GESTION:
                        sql += ", " + prefixeMediaPlanTable + ".object_count,"
                            + " mailing_rapidity,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas � un support du MD.");
                }

            }
            else if (mediaList["id_category"] != null && mediaList["id_category"].ToString()!="-1"){

                switch (mediaList["id_category"].ToString()){

                    case DBConstantes.Category.PUBLICITE_NON_ADRESSEE:
                        sql = ",  format, "
                            + DbTables.MAIL_FORMAT_PREFIXE + ".mail_format, "
                            + "mail_type";
                        break;
                    case DBConstantes.Category.COURRIER_ADRESSE:
                        sql += ", " + prefixeMediaPlanTable + ".mail_format,"
                            + " " + prefixeMediaPlanTable + ".object_count,"
                            + " mailing_rapidity,"
                            + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                            + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";
                        break;
                    default:
                        throw new Exceptions.MediaCreationDataAccessException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette cat�gorie ne correspond pas � une cat�gorie du MD.");
                }

            }
            else{

                sql += ",  format, "
                    + DbTables.MAIL_FORMAT_PREFIXE + ".mail_format, "
                    + "mail_type,"
                    + prefixeMediaPlanTable + ".mail_format,"
                    + " " + prefixeMediaPlanTable + ".object_count,"
                    + " mailing_rapidity,"
                    + " " + DbTables.DATA_MAIL_CONTENT_PREFIXE + ".id_mail_content,"
                    + " " + DbTables.MAIL_CONTENT_PREFIXE + ".mail_content";

            }

            return sql;

        }
		#endregion

		#endregion

	}
}
