//#region Information
//// Auteur: G. Ragneau
//// Date de création: 24/09/2004 
//// Date de modification: 24/09/2004 
//// 18/02/2005 A.Obermeyer > rajout Marque en personnalisation
//// 04/04/2005 A.Obermeyer > correction pdm, pdv tableau type 5B
//// 12/08/2005 A.Dadouch > Nom de fonctions
//// 24/10/2005 B.Masson > Suppression du /1000 (Division gérée désormais dans l'UI par l'appel des méthodes de la classe Units)
//// 16/03/2006 G. Ragneau > Ajout des niveaux de détails "vehicle/media" et "média"
//#endregion

//using System;
//using System.Data;
//using System.Collections;
//using System.Text;
//using System.Windows.Forms;

//using TNS.AdExpress.Domain.Translation;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using TNS.AdExpress.Web.Exceptions;
//using TNS.AdExpress.Web.Common.Results;

//using ClassifCst = TNS.AdExpress.Constantes.Classification;
//using FormatCst = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
//using DBClassificationCst = TNS.AdExpress.Constantes.Classification.DB;
//using RightCst = TNS.AdExpress.Constantes.Customer.Right;
//using WebCst = TNS.AdExpress.Constantes.Web;
//using TNS.Classification.Universe;
//using TNS.AdExpress.Classification;
//using TNS.AdExpress.Domain.Exceptions;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

//namespace TNS.AdExpress.Web.Rules.Results{
//    /// <summary>
//    /// Classe métier du module tableau dynamique (analyse sectorielle)
//    /// </summary>
//    public class DynamicTablesRules{

//        #region getDataTable
//        /// <summary>
//        /// Redirige les données vers une méthode chargée de les structuré spéciofiquement en fonction du tableau 
//        /// préformaté sélectionné
//        /// </summary>
//        /// <param name="webSession">Session utilisateur</param>
//        /// <returns>Tableau d'objet contenant les données à livrer structurées suivant le tableau préformaté
//        /// sélectionné</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		TNS.AdExpress.Web.DataAccess.Results.DynamicTablesDataAccess.getData(WebSession)
//        /// </remarks>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.NoDataException()">Remontée si l'exception est lancée dans le dataAccess</exception>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.DeliveryFrequencyException()">Remontée si l'exception est lancée dans le dataAccess</exception>
//        /// <exception cref="System.Exception()">Remontée si l'exception est lancée dans le dataAccess</exception>
//        /// <exception cref="TNS.AdExpress.Web.Exceptions.DynamicTableRulesException()">Lancée si aucun tableau préformatée n'est valide pour celui présent en session utilisateur</exception>
//        public static object[,] GetDataTable(WebSession webSession){

//            DataSet dsData;
//            try{
//                dsData = DynamicTablesDataAccess.GetData(webSession);
//            }
//            catch(TNS.AdExpress.Domain.Exceptions.NoDataException){ return null;}
//            catch(DeliveryFrequencyException e2){ throw e2;}
//            catch(System.Exception e3){ throw e3;}

//            switch(webSession.PreformatedTable){
//                case FormatCst.PreformatedTables.media_X_Year:
//                case FormatCst.PreformatedTables.product_X_Year:
//                    return BuildDataTableType_1_2(dsData, webSession);
//                case FormatCst.PreformatedTables.productMedia_X_Year:
//                case FormatCst.PreformatedTables.mediaProduct_X_Year:
//                    return BuildDataTableType_3_4_10_11(dsData, webSession, false);				
//                case FormatCst.PreformatedTables.productMedia_X_YearMensual:
//                case FormatCst.PreformatedTables.mediaProduct_X_YearMensual:
//                    return BuildDataTableType_3_4_10_11(dsData, webSession, true);				
//                case FormatCst.PreformatedTables.productYear_X_Media:
//                    return BuildDataTableType_5(dsData, webSession);					
//                case FormatCst.PreformatedTables.productYear_X_Mensual:
//                case FormatCst.PreformatedTables.productYear_X_Cumul:
//                case FormatCst.PreformatedTables.mediaYear_X_Mensual:
//                case FormatCst.PreformatedTables.mediaYear_X_Cumul:
//                    return BuildDataTableType_6_7_8_9(dsData, webSession);					
//                default:
//                    throw new DynamicTableRulesException("Le cas du tableau " + webSession.PreformatedTable.ToString() + " n'est pas géré.");
//            }

//        }
//        #endregion

//        #region Type de tableau 1 et 2
//        /// <summary>
//        /// Construit un tableau structuré de type 1 ou 2 en fonction des détails sélectionnés.
//        /// Type 1 : (media) X (N [PDM, N-1, EVOL])
//        /// Type 2 : (produit) X (N [PDV, N-1, EVOL])
//        /// Etapes:
//        ///		- Vérification de la présence de données dans dsData
//        ///		- Construction des constantes nécessaires au traitement des données:
//        ///			- personnalisation des éléments référents et concurrents
//        ///			- index de la première colonne à contenir des données quantitatives
//        ///			- tableau des index de nomenclature contenaznt pour chaque niveau de nomenclature des triplets
//        ///			(index colonne de la nom dans dsData, ligne du niveau dans le tableau de resultat, Identifiant du niveau)
//        /// </summary>
//        /// <param name="dsData">DataSet issue de la couche BDD. Il contient déjà les données nécessaires à 
//        /// l'édition du tableau en contion des niveaux de détails préformatés et du tableau considéré</param>
//        /// <param name="webSession">Session utilisateur</param>
//        /// <returns>Un tableau structurant les données de dsData pour un tableau de type 1 ou 2</returns>
//        private static object[,] BuildDataTableType_1_2(DataSet dsData, WebSession webSession){

//            DataTable dtData = dsData.Tables[0];

			

//            if (dtData.Rows.Count<=0) return null;

//            object[,] data;

//            int i,j;
//            int nbLine;
//            int currentLine;

//            #region Constantes

//            //Notion de personnalisation? dernière colonne = advertiser ==> perso_column=dernière colonne
//            int PERSO_COLUMN = 0;
//            string[] referenceSelection = null;
//            string[] concurrentSelection = null;
//            if(dtData.Columns.Contains("id_advertiser")){
//                PERSO_COLUMN=1;
//                string tempString = "";
//                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) referenceSelection = tempString.Split(',');
//                }
//                //referenceSelection = webSession.GetSelection(webSession.ReferenceUniversAdvertiser, RightCst.type.advertiserAccess).Split(',');
//                //concurrentSelection= (webSession.CompetitorUniversAdvertiser[0]!=null)?webSession.GetSelection( ((TreeNode)webSession.CompetitorUniversAdvertiser[0]), RightCst.type.advertiserAccess).Split(','):null;
//                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1) && webSession.SecondaryProductUniverses[1].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) concurrentSelection = tempString.Split(',');
//                }
//            }

//            //Calcul de l'index de la première colonne a contenir des données quantitatives
//            for(i=0; i<dtData.Columns.Count-PERSO_COLUMN; i=i+2){
//                if (dtData.Columns[i].ColumnName.IndexOf("ID_M")<0 && dtData.Columns[i].ColumnName.IndexOf("ID_P")<0){
//                    break;
//                }
//            }
//            int FIRST_DATA_INDEX=i;
//            // Supprime les lignes inutiles
//            dtData=DynamicDataTable(dsData,FIRST_DATA_INDEX,PERSO_COLUMN);

//            //Edition de la liste des colonnes descriptives (media, produit, groupe, category...) et initialisation
//            //de la partie "nbLevel" et "oldLevelID"
//            int[,] CLASSIF_INDEXES = new int[FIRST_DATA_INDEX/2, 3];
//            for(i = 0; i<(FIRST_DATA_INDEX); i+=2){
//                CLASSIF_INDEXES[i/2, 0]=i;
//                CLASSIF_INDEXES[i/2, 1]=0;
//                CLASSIF_INDEXES[i/2, 2]=-1;
//            }

//            //nombre d'options
//            int NB_OPTION = 0;
//            bool percentage = false;
//            bool evolution = false;
//            if (webSession.PDM && webSession.PreformatedTable == FormatCst.PreformatedTables.media_X_Year){
//                NB_OPTION++;
//                percentage = true;
//            }

//            if (webSession.PDV && webSession.PreformatedTable == FormatCst.PreformatedTables.product_X_Year){
//                NB_OPTION++;
//                percentage = true;
//            }
		

//            if (webSession.ComparativeStudy){
//                if (webSession.Evolution)
//                {
//                    NB_OPTION++;
//                    evolution = true;
//                }
//                if (percentage)
//                    NB_OPTION++;
//            }

//            #endregion

//            #region Calcul du nombre de lignes au maximum et initialisation de data
//            //si tableau prodtuiXannée ou plurtimedia ==>ligne total
//            if (webSession.PreformatedTable == FormatCst.PreformatedTables.product_X_Year
//                || ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()) {
//                nbLine = 1;
//                currentLine = 0;
//            }
//            else{
//                nbLine = 0;
//                currentLine = -1;
//            }

//            foreach(DataRow currentRow in dtData.Rows){
//                for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
//                    if(CLASSIF_INDEXES[i,2]!=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString())){
//                        nbLine++;
//                        CLASSIF_INDEXES[i,2]=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString());
//                        for(j=i+1; j<=CLASSIF_INDEXES.GetUpperBound(0); j++){
//                            CLASSIF_INDEXES[j,2]=-1;
//                        }
//                    }
//                }
//            }
//            data = new object[nbLine,dtData.Columns.Count-CLASSIF_INDEXES.GetLength(0) + NB_OPTION];
//            #endregion

//            #region Construction du tableau
//            //Réinitialisation des "oldIdLevel"
//            for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
//                CLASSIF_INDEXES[i,2]=-1;
//            }

//            //Ligne de total si on est en plurimedia ou en tableau de type 2
//            if (webSession.PreformatedTable == FormatCst.PreformatedTables.product_X_Year
//                || ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode())
//            {
//                data[0,0] = "TOTAL";
//                for(i=0; i < (dtData.Columns.Count+NB_OPTION-FIRST_DATA_INDEX)-PERSO_COLUMN; i++){
//                    data[0, CLASSIF_INDEXES.GetLength(0)+i] = 0.0;
//                }
//            }

//            int offset = (percentage) ? 2 : 1;

//            foreach(DataRow currentRow in dtData.Rows){

//                for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
//                //Pour chaque niveau de nomenclature (produit ou media indifféremment)
//                    if(CLASSIF_INDEXES[i,2]!=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString())){
						
//                        //niveau différent du précédent ==> nouvelle ligne dans le tableau
//                        currentLine++;
						
//                        //Mettre a null toutes les colonnes correspondant aux niveaux supérieurs
//                        for(j=0; j<i; j++) data[currentLine, j]=null;
//                        //Initialisation des totaux
//                        for(j=0; j < (dtData.Columns.Count+NB_OPTION-FIRST_DATA_INDEX-PERSO_COLUMN); j++)data[currentLine, CLASSIF_INDEXES.GetLength(0)+j] = 0.0;
//                        //Mettre a null les niveaux inférieurs;
//                        for(j=i+1; j<=(CLASSIF_INDEXES.GetUpperBound(0)-2); j++) data[currentLine, j]=null;
//                        if (PERSO_COLUMN>0) data[currentLine, data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.none;
//                        //Renseigner la ligne courante
//                        data[currentLine, i] = currentRow[CLASSIF_INDEXES[i,0]+1].ToString();

//                        //Annonceur normal, reference ou concurrent
//                        //si on visualise les elmt perso et
//                        //		si on considère un niveau de détail avec advertiser en niveau pere
//                        //		ou si on est en detail gp/Annon ou Gp/Ref et que c le niveau le plus bas
//                        //			si l'annonceur considéré est reference ou concurrents...
//                        if(PERSO_COLUMN>0 && 
//                            ( webSession.PreformatedProductDetail.ToString().StartsWith(FormatCst.PreformatedProductDetails.advertiser.ToString())
//                            || (( webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupAdvertiser
//                            ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupProduct
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.brand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentAdvertiser
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentProduct
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.product
//                            )
//                            && i == CLASSIF_INDEXES.GetUpperBound(0)))
//                            ){
//                            if (Contains(referenceSelection,currentRow["id_advertiser"].ToString()))
//                                data[currentLine,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.reference;
//                            else if (Contains(concurrentSelection,currentRow["id_advertiser"].ToString()))
//                                data[currentLine,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                            else
//                                data[currentLine,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.none;
//                        }
						
//                        //Sauvegarde du niveau courant
//                        CLASSIF_INDEXES[i,2]=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString());
//                        CLASSIF_INDEXES[i,1]=currentLine;
						
//                        //Réinitialisation des niveaux inférieurs
//                        for(j=i+1; j<=CLASSIF_INDEXES.GetUpperBound(0); j++)CLASSIF_INDEXES[j,2]=-1;
//                    }
//                }
//                //Données quantitatives de la ligne courante
//                for(i=0; i < (dtData.Columns.Count-FIRST_DATA_INDEX-PERSO_COLUMN); i++){
//                    data[currentLine, CLASSIF_INDEXES.GetLength(0)+offset*i] = double.Parse(currentRow[FIRST_DATA_INDEX+i].ToString());
//                }
//                //Calcul des totaux
//                for(i=CLASSIF_INDEXES.GetUpperBound(0)-1; i>=0; i--){
//                    for(j=0; j < (dtData.Columns.Count-FIRST_DATA_INDEX-PERSO_COLUMN); j++){
//                        data[CLASSIF_INDEXES[i,1], CLASSIF_INDEXES.GetLength(0)+offset*j] = (double.Parse(data[CLASSIF_INDEXES[i,1], CLASSIF_INDEXES.GetLength(0)+offset*j].ToString()) + double.Parse(data[currentLine, CLASSIF_INDEXES.GetLength(0)+offset*j].ToString()));
//                    }				
//                }
//                if (data[0,0].ToString() == "TOTAL"){
//                    for(j=0; j < (dtData.Columns.Count-FIRST_DATA_INDEX-PERSO_COLUMN); j++){
//                        data[0, CLASSIF_INDEXES.GetLength(0)+offset*j] = (double.Parse(data[0, CLASSIF_INDEXES.GetLength(0)+offset*j].ToString()) + double.Parse(data[currentLine, CLASSIF_INDEXES.GetLength(0)+offset*j].ToString()));
//                    }
//                }
//            }
//            #endregion

//            #region Calculs

//            if(evolution ||percentage){

//                #region Initialisation des "totaux" d'es niveaus intermédiaires (cad =/ du pus bas bniveau qui ne sera pas une reference total)
//                int FIRST_RESULT_COLUMN = CLASSIF_INDEXES.GetLength(0);
//                //Product_Index contiendra sur une ligne i les totaux du niveau de nomenclature supérieur au niveau i
//                //ligne 0 : totaux généraux pour le calcul des PDM/PDV du niveau 0
//                double[] TOTAL_INDEXES_N = null;
//                double[] TOTAL_INDEXES_N_1 = null;
//                if(percentage){
//                    TOTAL_INDEXES_N = new double[CLASSIF_INDEXES.GetLength(0)];
//                    TOTAL_INDEXES_N_1 = new double[CLASSIF_INDEXES.GetLength(0)];
//                    for (i=0; i < TOTAL_INDEXES_N.GetLength(0); i++)
//                    {
//                        TOTAL_INDEXES_N[i] = TOTAL_INDEXES_N_1[i] = 0.0;
//                    }
//                }
//                #endregion

//                #region Parcours du tableau
//                bool monoMedia = false;
//                //pluri et présentation nomenclature media
//                if (webSession.PreformatedTable == FormatCst.PreformatedTables.media_X_Year
//                    &&((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID != DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
//                    monoMedia = true;
//                }

//                for(i=0; i<data.GetLength(0); i++){

//                    //Extraction de l'information niveau
//                    for(j=0; j<FIRST_RESULT_COLUMN; j++){
//                        if (data[i,j]!=null) break;
//                    }
 
//                    if(percentage){
//                        //si ligne courante = total, affectation dans j cad 0
//                        //sinon, si ligne different du total et niveau différent du plus bas, affectation dans j+1 cad niveau+1
//                        if(i==0 && !monoMedia)
//                        {
//                            TOTAL_INDEXES_N[j] = double.Parse(data[i,FIRST_RESULT_COLUMN].ToString());
//                            if (webSession.ComparativeStudy)
//                                TOTAL_INDEXES_N_1[j] = double.Parse(data[i,FIRST_RESULT_COLUMN+2].ToString());
//                        }
//                        else if(j<FIRST_RESULT_COLUMN-1)
//                        {
//                            TOTAL_INDEXES_N[j+1] = double.Parse(data[i,FIRST_RESULT_COLUMN].ToString());
//                            if (webSession.ComparativeStudy)
//                                TOTAL_INDEXES_N_1[j+1] = double.Parse(data[i,FIRST_RESULT_COLUMN+2].ToString());
//                        }
//                        //calcul pdv ou pdm
//                        if(i!=0)
//                        {
//                            if(double.Parse(TOTAL_INDEXES_N[j].ToString())!=0)
//                                data[i,FIRST_RESULT_COLUMN+1] = 100 * double.Parse(data[i,FIRST_RESULT_COLUMN].ToString()) 
//                                    / TOTAL_INDEXES_N[j];
//                            else
//                                data[i,FIRST_RESULT_COLUMN+1] = null;
//                            if (webSession.ComparativeStudy)
//                                if(double.Parse(TOTAL_INDEXES_N_1[j].ToString())!=0)
//                                    data[i,FIRST_RESULT_COLUMN+3] = 100 * double.Parse(data[i,FIRST_RESULT_COLUMN+2].ToString()) 
//                                        / TOTAL_INDEXES_N_1[j];
//                                else
//                                    data[i,FIRST_RESULT_COLUMN+3] = null;
//                        }
//                        else{
//                            data[i,FIRST_RESULT_COLUMN+1] = 100;
//                            if (webSession.ComparativeStudy)
//                                data[i,FIRST_RESULT_COLUMN+3] = 100;
//                        }					
//                    }
//                    //calcul evolution
//                    if (evolution){
//                        data[i,FIRST_RESULT_COLUMN+1+NB_OPTION] = 100 * ( double.Parse(data[i,FIRST_RESULT_COLUMN].ToString())
//                            - double.Parse(data[i,FIRST_RESULT_COLUMN+((percentage)?2:1)].ToString()))
//                            / double.Parse(data[i,FIRST_RESULT_COLUMN+((percentage)?2:1)].ToString());
//                        currentLine++;
//                    }

//                }

//                #endregion

//            }
//            #endregion

//            return data;
//        }
//        #endregion

//        #region Type de tableau 3 et 4, 10 et 11 
//        private static object[,] BuildDataTableType_3_4_10_11(DataSet dsData, WebSession webSession, bool extendedDynamicTable)
//        {
			
//            #region Variables
//            object[,] data = null;

//            DataTable dtData = dsData.Tables[0];

//            //Compteurs
//            int i,j,k;
			

//            #region Indexes relatifs aux nomenclatures

//            //Première colonne de données numériques dans dtdata et 
//            //détermination des index de début de chaque nomenclature
//            int DATA_FIRST_PRODUCT_INDEX = -1;
//            int DATA_FIRST_MEDIA_INDEX = -1;
//            for(i=0; i<dtData.Columns.Count; i=i+2){
//                if (dtData.Columns[i].ColumnName.IndexOf("ID_M")>=0 || dtData.Columns[i].ColumnName.IndexOf("ID_P")>=0){
//                    if (dtData.Columns[i].ColumnName.IndexOf("ID_M")>=0 && DATA_FIRST_MEDIA_INDEX<0){
//                        DATA_FIRST_MEDIA_INDEX = i;
//                    }
//                    else if (dtData.Columns[i].ColumnName.IndexOf("ID_P")>=0 && DATA_FIRST_PRODUCT_INDEX<0){
//                        DATA_FIRST_PRODUCT_INDEX = i;
//                    }
//                }
//                else
//                    break;
//            }
//            int DATA_FIRST_DATA_COLUMN = i;

//            // Supprime les lignes inutiles
//            int PERSO_COLUMN = 0;			
//            if(dtData.Columns.Contains("id_advertiser")){
//                PERSO_COLUMN=1;
//            }
//            dtData=DynamicDataTable(dsData,DATA_FIRST_DATA_COLUMN,PERSO_COLUMN);
			

//            //identification de la hiérarchie des nomenclatures.
//            int DATA_MAIN_CLASSIF_INDEX;
//            int DATA_SECOND_CLASSIF_INDEX;
//            ClassifCst.Branch.type MAIN_CLASSIF_TYPE;
//            ClassifCst.Branch.type SECOND_CLASSIF_TYPE;
//            if (DATA_FIRST_MEDIA_INDEX < DATA_FIRST_PRODUCT_INDEX){
//                //nomenclature principale = media
//                DATA_MAIN_CLASSIF_INDEX = DATA_FIRST_MEDIA_INDEX;
//                MAIN_CLASSIF_TYPE = ClassifCst.Branch.type.media;
//                //nomenclature secondaire = produit
//                DATA_SECOND_CLASSIF_INDEX = DATA_FIRST_PRODUCT_INDEX;
//                SECOND_CLASSIF_TYPE = ClassifCst.Branch.type.product;
//            }
//            else{
//                //nomenclature principale = produit
//                DATA_MAIN_CLASSIF_INDEX = DATA_FIRST_PRODUCT_INDEX;
//                MAIN_CLASSIF_TYPE = ClassifCst.Branch.type.product;
//                //nomenclature secondaire = media
//                DATA_SECOND_CLASSIF_INDEX = DATA_FIRST_MEDIA_INDEX;
//                SECOND_CLASSIF_TYPE = ClassifCst.Branch.type.media;
//            }

//            //indexes et totaux de la nomeclature principale
//            int[,] MAIN_CLASSIF_INDEXES = new int[DATA_SECOND_CLASSIF_INDEX/2,3];
//            //totaux année N
//            double[] MAIN_CLASSIF_TOTALS_N = new double[DATA_SECOND_CLASSIF_INDEX/2];
//            //totaux année N-1
//            double[] MAIN_CLASSIF_TOTALS_N_1 = new double[DATA_SECOND_CLASSIF_INDEX/2];
//            //totaux généraux de la seconde nomenclature pour chaque niveau de la nomenclature principale
//            //(exemple : totaux Gp > annonceurs pour chaque niveau support si tableau de type 4)
//            ExtendedHashTable[] SCD_TOTALS_N = new ExtendedHashTable[MAIN_CLASSIF_INDEXES.GetLength(0)];
//            ExtendedHashTable[] SCD_TOTALS_N_1 = new ExtendedHashTable[MAIN_CLASSIF_INDEXES.GetLength(0)];
//            for(i = 0; i < MAIN_CLASSIF_INDEXES.GetLength(0); i++)
//            {
//                //index dans la table de données issues de dataAccess
//                MAIN_CLASSIF_INDEXES[i,0] = 2*i;
//                //clé de l'index courant
//                MAIN_CLASSIF_INDEXES[i,1] = -1;
//                //champ bonus
//                MAIN_CLASSIF_INDEXES[i,2] = 0;
//                //total année N
//                MAIN_CLASSIF_TOTALS_N[i] = 0.0;
//                //total année N-1
//                MAIN_CLASSIF_TOTALS_N_1[i] = 0.0;
//            }

//            //indexes et totaux de la nomenclatures secondaire
//            int[,] SECONDARY_CLASSIF_INDEXES = new int[(DATA_FIRST_DATA_COLUMN-DATA_SECOND_CLASSIF_INDEX)/2,3];
//            //totaux année N
//            double[] SECONDARY_CLASSIF_TOTALS_N = new double[(DATA_FIRST_DATA_COLUMN-DATA_SECOND_CLASSIF_INDEX)/2];
//            //totaux année N-1
//            double[] SECONDARY_CLASSIF_TOTALS_N_1 = new double[(DATA_FIRST_DATA_COLUMN-DATA_SECOND_CLASSIF_INDEX)/2];
//            for(i = 0; i < SECONDARY_CLASSIF_INDEXES.GetLength(0); i++)
//            {
//                //Index dans la table de données
//                SECONDARY_CLASSIF_INDEXES[i,0] = DATA_SECOND_CLASSIF_INDEX + 2*i;
//                //champ bonus
//                SECONDARY_CLASSIF_INDEXES[i,2] = 0;
//                //total
//                SECONDARY_CLASSIF_TOTALS_N[i] = 0.0;
//                SECONDARY_CLASSIF_TOTALS_N_1[i] = 0.0;
//            }

//            #endregion

//            #region Indexes des colonnes de données numériques dans le tableau de résultat

//            //indexes année N, PDM, PDV, année N-1, Evolution, Données mensuels (extendedDynamicTable), Type de nomenclature et type d'annonceur dans le tableau de résultat
//            //année N
//            int RESULT_N_INDEX = MAIN_CLASSIF_INDEXES.GetLength(0) + SECONDARY_CLASSIF_INDEXES.GetLength(0) - 1;
			
//            //PDV
//            int RESULT_PDV_INDEX = -10;
//            if (webSession.PDV) RESULT_PDV_INDEX = MAIN_CLASSIF_INDEXES.GetLength(0)+SECONDARY_CLASSIF_INDEXES.GetLength(0);
//            //PDM
//            int RESULT_PDM_INDEX = -10;
//            if(webSession.PDM){
//                if (RESULT_PDV_INDEX>0)
//                    RESULT_PDM_INDEX = RESULT_PDV_INDEX + 1;
//                else
//                    RESULT_PDM_INDEX = MAIN_CLASSIF_INDEXES.GetLength(0)+SECONDARY_CLASSIF_INDEXES.GetLength(0);
//            }
//            //année N-1
//            int RESULT_N_1_INDEX = -10;
//            int NB_YEAR = 1;
//            if (webSession.ComparativeStudy){
//                RESULT_N_1_INDEX = Math.Max(RESULT_PDM_INDEX, Math.Max(RESULT_PDV_INDEX,RESULT_N_INDEX)) + 1;
//                NB_YEAR = 2;
//            }
//            //PDV N-1
//            int RESULT_PDV_N_1_INDEX = -10;
//            if (webSession.ComparativeStudy && webSession.PDV)
//            {
//                RESULT_PDV_N_1_INDEX = RESULT_N_1_INDEX + 1;
//            }
			
//            //PDM N-1
//            int RESULT_PDM_N_1_INDEX = -10;
//            if (webSession.ComparativeStudy && webSession.PDM)
//            {
//                RESULT_PDM_N_1_INDEX = Math.Max(RESULT_N_1_INDEX, RESULT_PDV_N_1_INDEX) + 1;
//            }

//            //Evol
//            int RESULT_EVOL_INDEX = -10;
//            if (webSession.Evolution && webSession.ComparativeStudy){
//                RESULT_EVOL_INDEX = Math.Max(RESULT_PDM_N_1_INDEX,
//                    Math.Max(RESULT_N_1_INDEX, RESULT_PDV_N_1_INDEX)) + 1;
//            }
			
//            //Mois
//            int RESULT_MONTHS = -10;
//            int RESULT_LAST_MONTHS = -10;
//            string absolutePeriodEnd = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
//            if (extendedDynamicTable) 
//            {
//                RESULT_MONTHS = Math.Max(RESULT_PDV_N_1_INDEX, 
//                    Math.Max(RESULT_PDM_N_1_INDEX, 
//                    Math.Max(RESULT_PDM_INDEX, 
//                    Math.Max(RESULT_PDV_INDEX,
//                    Math.Max(RESULT_N_INDEX, 
//                    Math.Max(RESULT_EVOL_INDEX, RESULT_N_1_INDEX)
//                    ))))) + 1;
//                RESULT_LAST_MONTHS = RESULT_MONTHS + (int.Parse(absolutePeriodEnd.Substring(4,2))-int.Parse(webSession.PeriodBeginningDate.Substring(4,2)));
//            }


//            //Type de nomenclature
//            int RESULT_CLASSIF_TYPE_INDEX = Math.Max(RESULT_LAST_MONTHS, 
//                Math.Max(RESULT_PDV_N_1_INDEX, 
//                Math.Max(RESULT_PDM_N_1_INDEX, 
//                Math.Max(RESULT_PDM_INDEX, 
//                Math.Max(RESULT_PDV_INDEX,
//                Math.Max(RESULT_N_INDEX, 
//                Math.Max(RESULT_EVOL_INDEX, RESULT_N_1_INDEX)
//                )))))) + 1;
//            //Type d'annonceur
//            int RESULT_ADVERTISER_INDEX = RESULT_CLASSIF_TYPE_INDEX + 1;
//            //string[] referenceSelection = webSession.GetSelection(webSession.ReferenceUniversAdvertiser, RightCst.type.advertiserAccess).Split(',');
//            //string[] concurrentSelection = (webSession.CompetitorUniversAdvertiser[0]!=null)?webSession.GetSelection( ((TreeNode)webSession.CompetitorUniversAdvertiser[0]), RightCst.type.advertiserAccess).Split(','):null;
//            string tempString = "";
//            string[] referenceSelection = null;
//            string[] concurrentSelection = null;
//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0)) {
//                tempString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                if (tempString != null && tempString.Length > 0) referenceSelection = tempString.Split(',');
//            }
//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1) && webSession.SecondaryProductUniverses[1].Contains(0)) {
//                tempString = webSession.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                if (tempString != null && tempString.Length > 0) concurrentSelection = tempString.Split(',');
//            }
//            #endregion
			
			


//            #region Initialisation du tableau résultant
//            int NB_LINE = 0;

//            foreach(DataRow currentRow in dtData.Rows){
				
//                //pour chaque niveau de la nomenclature principale
//                for ( i = 0; i < MAIN_CLASSIF_INDEXES.GetLength(0); i++){

//                    if(MAIN_CLASSIF_INDEXES[i,1] != int.Parse(currentRow[MAIN_CLASSIF_INDEXES[i,0]].ToString())){
//                        //réinitialisation des index de niveau inférieur
//                        for(int b=i+1; b < MAIN_CLASSIF_INDEXES.GetLength(0); b++)
//                        {
//                            MAIN_CLASSIF_INDEXES[b,1] = -1;
//                        }
//                        //réinitialisation des index de la nomenclature secondaire
//                        for(int b=0; b < SECONDARY_CLASSIF_INDEXES.GetLength(0); b++)
//                        {
//                            SECONDARY_CLASSIF_INDEXES[b,1] = -1;
//                        }
//                        MAIN_CLASSIF_INDEXES[i,1] = int.Parse(currentRow[MAIN_CLASSIF_INDEXES[i,0]].ToString());
//                        NB_LINE++;

//                        for ( j = i+1; j<MAIN_CLASSIF_INDEXES.GetLength(1); j++){
//                            MAIN_CLASSIF_INDEXES[i,1] = -1 ;
//                        }

//                        //si on est sur le niveau le plus bas:
//                        if (i == MAIN_CLASSIF_INDEXES.GetUpperBound(0)){
							
//                            for (j = 0; j < SECONDARY_CLASSIF_INDEXES.GetLength(0); j++){
//                                //pour chaque niveau de la nomenclature secondaire
//                                if (SECONDARY_CLASSIF_INDEXES[j,1] != int.Parse(currentRow[SECONDARY_CLASSIF_INDEXES[j,0]].ToString())){
//                                    //réinitialisation des index de niveau inférieur
//                                    for(int b=j+1; b < SECONDARY_CLASSIF_INDEXES.GetLength(0); b++)
//                                    {
//                                        SECONDARY_CLASSIF_INDEXES[b,1] = -1;
//                                    }

//                                    for( k = 0; k < MAIN_CLASSIF_INDEXES.GetLength(0); k++){
//                                        //chaquer niveau de la classification secondaire sera present sur un niveau
//                                        // de la classification principale
//                                        NB_LINE++;
//                                    }
//                                }
//                            }

//                        }
//                    }
				
//                }

//            }
//            data = new object[NB_LINE+1, RESULT_ADVERTISER_INDEX+1];

//            #endregion

//            #endregion

//            DataSet workDataSet;
//            string conditions = "";
//            string scndConditions = "";
//            string sort = "";
//            int currentLine = 0;
//            int constComparativeStudy=0;
//            string[] Keys;

//            #region Construction du tableau

//            foreach(DataRow currentRow in dtData.Rows){
//                //pour chaque ligne issue de la DataAccess

//                for (i = 0; i < MAIN_CLASSIF_INDEXES.GetLength(0); i++){
//                    //Pour chaque niveau de nomenclature

//                    if (MAIN_CLASSIF_INDEXES[i,1] != int.Parse(currentRow[MAIN_CLASSIF_INDEXES[i,0]].ToString())){
//                        //si le niveau courant est différent du niveau précédent

//                        //identifiant de l'enregistrement
//                        MAIN_CLASSIF_INDEXES[i,1]=int.Parse(currentRow[MAIN_CLASSIF_INDEXES[i,0]].ToString());

//                        #region Données qualitatives de la nomenclature principale
//                        //insertion de l'enregistrement courant
//                        //libellé
//                        data[currentLine,i] = currentRow[MAIN_CLASSIF_INDEXES[i,0]+1].ToString();
//                        //nomenclature
//                        data[currentLine, RESULT_CLASSIF_TYPE_INDEX] = MAIN_CLASSIF_TYPE;
//                        //annonceur
//                        if (MAIN_CLASSIF_TYPE==ClassifCst.Branch.type.product && dtData.Columns.Contains("id_advertiser")
//                            && ( webSession.PreformatedProductDetail.ToString().StartsWith(FormatCst.PreformatedProductDetails.advertiser.ToString())
//                            || (( webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupAdvertiser
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupProduct
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.brand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentAdvertiser
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.product
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentProduct
//                            )
//                            && i == MAIN_CLASSIF_INDEXES.GetUpperBound(0)))						
//                            ){
//                            if(Contains(referenceSelection, currentRow["id_advertiser"].ToString()))
//                                data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.reference;
//                            else if(Contains(concurrentSelection, currentRow["id_advertiser"].ToString()))
//                                data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                            else
//                                data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.none;
//                        }
//                        else
//                            data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.none;
//                        #endregion

//                        workDataSet = new DataSet();
//                        //construction de la clause de conditions et de la clause de tri de manière à récupérer les
//                        //lignes qui correspondent aux identifiants du niveau courant et de ses niveaux supérieurs
//                        conditions = "";
//                        for (j = 0; j <= i ; j++){
//                            conditions += ((j>0)?" AND ":"") + dtData.Columns[MAIN_CLASSIF_INDEXES[j,0]].ColumnName
//                                + "=" + MAIN_CLASSIF_INDEXES[j,1];
//                        }

//                        //le tri porte sur les niveaux de la nomenclature secondaire
//                        sort = "";
//                        for (j = 0; j < SECONDARY_CLASSIF_INDEXES.GetLength(0) ; j++){
//                            sort += ((j>0)?" ,":"") + dtData.Columns[SECONDARY_CLASSIF_INDEXES[j,0]+1].ColumnName;
//                            sort += "," + dtData.Columns[SECONDARY_CLASSIF_INDEXES[j,0]].ColumnName;
//                        }


//                        //récupération des lignes qui correspondent aux conditions
//                        workDataSet.Merge(dtData.Select(conditions, sort));

//                        #region Données numériques de la nomenclature principal
//                        //insertion des données numériques de la ligne courante dans le tableau de résultat
//                        //données N
//                        data[currentLine, RESULT_N_INDEX] = MAIN_CLASSIF_TOTALS_N[i] = double.Parse(
//                            dtData.Compute( "sum(" + dtData.Columns[DATA_FIRST_DATA_COLUMN].ColumnName 
//                            + ")",conditions).ToString());


//                        //données N-1
//                        if(RESULT_N_1_INDEX>0)
//                            data[currentLine, RESULT_N_1_INDEX] = MAIN_CLASSIF_TOTALS_N_1[i] = double.Parse(
//                                dtData.Compute( "sum(" + dtData.Columns[DATA_FIRST_DATA_COLUMN+1].ColumnName 
//                                + ")",conditions).ToString());

//                        //PDM année N
//                        if (RESULT_PDM_INDEX>-1){
//                            if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media){
//                                data[currentLine, RESULT_PDM_INDEX] = (i!=0)?
//                                    100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString()) 
//                                    / MAIN_CLASSIF_TOTALS_N[i-1]
//                                    :
//                                    100.0;
//                                }
//                            else{
//                                data[currentLine, RESULT_PDM_INDEX] = 100.0;
//                            }
//                        }
//                        //PDM année N-1
//                        if (RESULT_PDM_N_1_INDEX>-1)
//                        {
//                            if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media)
//                            {
//                                data[currentLine, RESULT_PDM_N_1_INDEX] = (i!=0)?
//                                    100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString()) 
//                                    / MAIN_CLASSIF_TOTALS_N_1[i-1]
//                                    :
//                                    100.0;
//                            }
//                            else
//                            {
//                                data[currentLine, RESULT_PDM_N_1_INDEX] = 100.0;
//                            }
//                        }

//                        //PDV
//                        if (RESULT_PDV_INDEX>-1)
//                        {
//                            if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media){
//                                data[currentLine, RESULT_PDV_INDEX] = 100.0;
//                            }
//                            else{
//                                data[currentLine, RESULT_PDV_INDEX] = (i!=0)?
//                                    100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString()) 
//                                    / MAIN_CLASSIF_TOTALS_N[i-1]
//                                    :
//                                    100.0;
//                            }
//                        }
//                        //PDV année N-1
//                        if (RESULT_PDV_N_1_INDEX>-1)
//                        {
//                            if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media)
//                            {
//                                data[currentLine, RESULT_PDV_N_1_INDEX] = 100.0;
//                            }
//                            else
//                            {
//                                data[currentLine, RESULT_PDV_N_1_INDEX] = (i!=0)?
//                                    100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString()) 
//                                    / MAIN_CLASSIF_TOTALS_N_1[i-1]
//                                    :
//                                    100.0;
//                            }
//                        }
//                        //Evolution
//                        if (RESULT_EVOL_INDEX>0)
//                            data[currentLine, RESULT_EVOL_INDEX] = 100 * ( double.Parse(data[currentLine, RESULT_N_INDEX].ToString()) 
//                                - double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString()) )
//                                / double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString());

//                        //Mois de N
//                        if (extendedDynamicTable)
//                        {
//                            for (int w = 0; w <= (RESULT_LAST_MONTHS-RESULT_MONTHS); w++)
//                            {
//                                data[currentLine,RESULT_MONTHS+w] = double.Parse(
//                                    dtData.Compute( "sum(" + dtData.Columns[DATA_FIRST_DATA_COLUMN+NB_YEAR+w].ColumnName 
//                                    + ")",conditions).ToString());
//                            }
//                        }


//                        //incrémentation de la ligne dans le tableau de résultat
//                        currentLine++;

//                        #endregion

//                        #region Nomenclature secondaire
//                        //initialisation des identifiants de la seconde nomenclature
//                        for(j = 0; j < SECONDARY_CLASSIF_INDEXES.GetLength(0); j++){
//                            SECONDARY_CLASSIF_INDEXES[j,1] = -1;
//                        }


//                        SCD_TOTALS_N[i] = new ExtendedHashTable();
//                        if (RESULT_N_1_INDEX>0)
//                            SCD_TOTALS_N_1[i] = new ExtendedHashTable();


//                        foreach(DataRow secondaryRow in workDataSet.Tables[0].Rows)
//                        {
//                            //pour toutes les lignes qui correspondent à l'identifiant courant et aus identifiant sup

//                            scndConditions = conditions;

//                            for( j = 0 ; j < SECONDARY_CLASSIF_INDEXES.GetLength(0); j++)
//                            {
//                                //pour tous les niveaux de la nomenclature secondaire

//                                if (SECONDARY_CLASSIF_INDEXES[j,1] != int.Parse(secondaryRow[SECONDARY_CLASSIF_INDEXES[j,0]].ToString())){
//                                    //si l'identifiant du niveau courant est different du precedent

//                                    for(int b=j+1; b < SECONDARY_CLASSIF_INDEXES.GetLength(0); b++)
//                                    {
//                                        SECONDARY_CLASSIF_INDEXES[b,1] = -1;
//                                    }

//                                    SECONDARY_CLASSIF_INDEXES[j,1] = int.Parse(secondaryRow[SECONDARY_CLASSIF_INDEXES[j,0]].ToString());

//                                    for(k=0; k <= j; k++)
//                                    {
//                                        scndConditions += " AND " + dtData.Columns[SECONDARY_CLASSIF_INDEXES[k,0]].ColumnName
//                                            + "=" + SECONDARY_CLASSIF_INDEXES[k,1].ToString();
//                                    }
									
//                                    //insertion dans le tableau résultat
//                                    //libellé
//                                    data[currentLine,i+j] = secondaryRow[SECONDARY_CLASSIF_INDEXES[j,0]+1].ToString();
									
//                                    //nomenclature
//                                    data[currentLine, RESULT_CLASSIF_TYPE_INDEX] = SECOND_CLASSIF_TYPE;
									
//                                    //données N
//                                    data[currentLine, RESULT_N_INDEX] = double.Parse(
//                                        workDataSet.Tables[0].Compute( "sum(" + dtData.Columns[DATA_FIRST_DATA_COLUMN].ColumnName 
//                                        + ")",scndConditions).ToString());
									
//                                    //total courant N
//                                    SECONDARY_CLASSIF_TOTALS_N[j] = double.Parse(data[currentLine, RESULT_N_INDEX].ToString());

//                                    //totaux dans la nomenclature principale
//                                    Keys = new string[j+1];
//                                    for( k = 0; k <= j; k++){
//                                        Keys[k] = secondaryRow[SECONDARY_CLASSIF_INDEXES[k,0]].ToString();
//                                    }
//                                    SCD_TOTALS_N[i].Add(double.Parse(data[currentLine, RESULT_N_INDEX].ToString()),Keys);
									
//                                    //données N-1
//                                    if(RESULT_N_1_INDEX>0)
//                                    {
//                                        data[currentLine, RESULT_N_1_INDEX] = double.Parse(workDataSet.Tables[0].Compute( "sum(" + dtData.Columns[DATA_FIRST_DATA_COLUMN+1].ColumnName 
//                                            + ")",scndConditions).ToString());

//                                        //total courant N-1
//                                        SECONDARY_CLASSIF_TOTALS_N_1[j] = double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString());
//                                        //total dans la nomenclature principale
//                                        SCD_TOTALS_N_1[i].Add(double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString()),Keys);
//                                    }

//                                    //PDM année N
//                                    if (RESULT_PDM_INDEX > -1){
//                                        if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media){
//                                            data[currentLine, RESULT_PDM_INDEX] = (i!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString())
//                                                / SCD_TOTALS_N[i-1].GetValue(Keys)
//                                                :
//                                                100.0;
//                                        }
//                                        else{
//                                            data[currentLine, RESULT_PDM_INDEX] = (j!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString())
//                                                / SECONDARY_CLASSIF_TOTALS_N[j-1]
//                                                :
//                                                100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString())
//                                                / MAIN_CLASSIF_TOTALS_N[i]
//                                                ;
//                                        }
//                                    }
//                                    //PDM année N-1
//                                    if (RESULT_PDM_N_1_INDEX > -1)
//                                    {
//                                        if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media)
//                                        {
//                                            data[currentLine, RESULT_PDM_N_1_INDEX] = (i!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString())
//                                                / SCD_TOTALS_N_1[i-1].GetValue(Keys)
//                                                :
//                                                100.0;
//                                        }
//                                        else
//                                        {
//                                            data[currentLine, RESULT_PDM_N_1_INDEX] = (j!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString())
//                                                / SECONDARY_CLASSIF_TOTALS_N_1[j-1]
//                                                :
//                                                100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString())
//                                                / MAIN_CLASSIF_TOTALS_N_1[i]
//                                                ;
//                                        }
//                                    }


//                                    //PDV
//                                    if (RESULT_PDV_INDEX > -1){
//                                        if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media){
//                                            data[currentLine, RESULT_PDV_INDEX] = (j!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString())
//                                                / SECONDARY_CLASSIF_TOTALS_N[j-1]
//                                                :
//                                                100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString())
//                                                / MAIN_CLASSIF_TOTALS_N[i]
//                                                ;
//                                        }
//                                        else{
//                                            data[currentLine, RESULT_PDV_INDEX] = (i!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_INDEX].ToString())
//                                                / SCD_TOTALS_N[i-1].GetValue(Keys)
//                                                :
//                                                100.0;
//                                        }
//                                    }
//                                    //PDV N-1
//                                    if (RESULT_PDV_N_1_INDEX > -1)
//                                    {
//                                        if (MAIN_CLASSIF_TYPE == ClassifCst.Branch.type.media)
//                                        {
//                                            data[currentLine, RESULT_PDV_N_1_INDEX] = (j!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString())
//                                                / SECONDARY_CLASSIF_TOTALS_N_1[j-1]
//                                                :
//                                                100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString())
//                                                / MAIN_CLASSIF_TOTALS_N_1[i]
//                                                ;
//                                        }
//                                        else
//                                        {
//                                            data[currentLine, RESULT_PDV_N_1_INDEX] = (i!=0)?
//                                                100 * double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString())
//                                                / SCD_TOTALS_N_1[i-1].GetValue(Keys)
//                                                :
//                                                100.0;
//                                        }
//                                    }
									
//                                    //Evolution
//                                    if (RESULT_EVOL_INDEX>0)
//                                        data[currentLine, RESULT_EVOL_INDEX] = 100 * ( double.Parse(data[currentLine, RESULT_N_INDEX].ToString()) 
//                                            - double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString()) )
//                                            / double.Parse(data[currentLine, RESULT_N_1_INDEX].ToString());
									
//                                    //Mois de N
//                                    if (extendedDynamicTable)
//                                    {										
//                                        for (int w = 0; w <= (RESULT_LAST_MONTHS-RESULT_MONTHS); w++)
//                                        {
//                                            //!!!!!!!!!!!
//                                            if(webSession.ComparativeStudy)
//                                                constComparativeStudy=2;
//                                            else constComparativeStudy=1;

////											data[currentLine, RESULT_MONTHS+w] = double.Parse(workDataSet.Tables[0].Compute( "sum(" + dtData.Columns[DATA_FIRST_DATA_COLUMN+constComparativeStudy+w].ColumnName 
////												+ ")",scndConditions).ToString())/1000;
//                                            data[currentLine, RESULT_MONTHS+w] = double.Parse(workDataSet.Tables[0].Compute( "sum(" + dtData.Columns[DATA_FIRST_DATA_COLUMN+constComparativeStudy+w].ColumnName 
//                                                + ")",scndConditions).ToString());

//                                        }
//                                    }
//                                    //annonceur
//                                    if (SECOND_CLASSIF_TYPE==ClassifCst.Branch.type.product && workDataSet.Tables[0].Columns.Contains("id_advertiser")
//                                        && ( webSession.PreformatedProductDetail.ToString().StartsWith(FormatCst.PreformatedProductDetails.advertiser.ToString())
//                                        || (( webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupAdvertiser
//                                        ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupProduct
//                                        ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.brand
//                                        ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupBrand
//                                        || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentAdvertiser
//                                        || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentBrand
//                                        || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentProduct
//                                        || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.product
//                                        )
//                                        && j == SECONDARY_CLASSIF_INDEXES.GetUpperBound(0)))										
//                                        ){
//                                        if(Contains(referenceSelection, secondaryRow["id_advertiser"].ToString()))
//                                            data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.reference;
//                                        else if(Contains(concurrentSelection, secondaryRow["id_advertiser"].ToString()))
//                                            data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                                        else
//                                            data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.none;
//                                    }
//                                    else
//                                        data[currentLine, RESULT_ADVERTISER_INDEX] = WebCst.AdvertiserPersonalisation.Type.none;

//                                    //incrémentation de la ligne dans le tableau de résultat
//                                    currentLine++;
//                                }

//                            }

//                        }

//                        #endregion

//                    }

//                }
//            }
//            data[currentLine,0] = new TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd();
//            #endregion

//            return data;

//        }
//        #endregion

//        #region Tableau de type 5
//        private static object[,] BuildDataTableType_5(DataSet dsData, WebSession webSession){

//            DataTable dtData = dsData.Tables[0];

//            if (dtData.Rows.Count<=0) return null;

//            #region Variables
//            object[,] data = null;
//            int i,j,k,l,currentLine;
//            string yearN ,yearN1;

//            //Notion de personnalisation? dernière colonne = advertiser ==> perso_column=dernière colonne
//            int PERSO_COLUMN = 0;
//            string[] referenceSelection = null;
//            string[] concurrentSelection = null;
//            if(dtData.Columns.Contains("id_advertiser")){
//                PERSO_COLUMN=1;
//                //referenceSelection = webSession.GetSelection(webSession.ReferenceUniversAdvertiser, RightCst.type.advertiserAccess).Split(',');
//                //concurrentSelection= (webSession.CompetitorUniversAdvertiser[0]!=null)?webSession.GetSelection( ((TreeNode)webSession.CompetitorUniversAdvertiser[0]), RightCst.type.advertiserAccess).Split(','):null;
//                string tempString = "";				
//                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) referenceSelection = tempString.Split(',');
//                }
//                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1) && webSession.SecondaryProductUniverses[1].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) concurrentSelection = tempString.Split(',');
//                }
//            }

//            int NB_LINE = 0;
//            int NB_OPTION = 0;
//            int nbYearData;
//            int FIRST_DATA_INDEX;
//            int FIRST_MEDIA_DATA_INDEX;
//            int FIRST_RESULT_LINE_INDEX = 0;

//            int[,] PRODUCT_DATA_INDEXES;
//            int[,] MEDIA_DATA_INDEXES;

//            int FIRST_MEDIA_RESULT_INDEX;
//            int FIRST_PRODUCT_RESULT_INDEX = 0;

//            string TOTAL = "PLURIMEDIA";

//            string[] mainHeaders;
//            StringBuilder headers = new StringBuilder(750);
//            #endregion

//            #region Calcul de l'index de la première colonne a contenir des données quantitatives
//            for(i=0; i<dtData.Columns.Count; i=i+2){
//                if (dtData.Columns[i].ColumnName.IndexOf("ID_M")<0 && dtData.Columns[i].ColumnName.IndexOf("ID_P")<0){
//                    break;
//                }
//            }
//            FIRST_DATA_INDEX=i;
//            // Supprime les lignes inutiles
//            dtData=DynamicDataTable(dsData,FIRST_DATA_INDEX,PERSO_COLUMN);
//            #endregion

//            #region Constante indexes de la nomenclature produit
//            //Edition de la liste des colonnes descriptives (nomenclature produit uniquement <== gestion des lignes) et initialisation
//            //de la partie "nbLevel" et "oldLevelID"
			
//            if (webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.brand ||
//                webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.group ||
//                webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.advertiser)
//            {
//                PRODUCT_DATA_INDEXES = new int[1, 3]{{0,0,-1}};
//            }
//            else{
//                PRODUCT_DATA_INDEXES = new int[2, 3]{{0,0,-1},{2,0,-1}};
//            }
//            #endregion

//            #region Calcul du nombre de lignes au maximum
//            if (!webSession.ComparativeStudy) NB_LINE = 2;
//            else{
//                NB_LINE = 3;
//                if (webSession.Evolution){
//                    NB_OPTION++;
//                    NB_LINE++;
//                }
//            }
//            if (webSession.PDM){
//                NB_OPTION += (webSession.ComparativeStudy)?2:1;
//                NB_LINE += (webSession.ComparativeStudy)?2:1;
//            }
//            if (webSession.PDV){
//                NB_OPTION += (webSession.ComparativeStudy)?2:1;
//                NB_LINE += (webSession.ComparativeStudy)?2:1;
//            }
//            foreach(DataRow currentRow in dtData.Rows){
//                for(i=0; i<=PRODUCT_DATA_INDEXES.GetUpperBound(0); i++){
//                    if(PRODUCT_DATA_INDEXES[i,2]!=int.Parse(currentRow[PRODUCT_DATA_INDEXES[i,0]].ToString())){
//                        if (!webSession.ComparativeStudy) NB_LINE+=2+NB_OPTION;
//                        else NB_LINE+=3+NB_OPTION;
//                        PRODUCT_DATA_INDEXES[i,2]=int.Parse(currentRow[PRODUCT_DATA_INDEXES[i,0]].ToString());
//                        for(j=i+1; j<=PRODUCT_DATA_INDEXES.GetUpperBound(0); j++){
//                            PRODUCT_DATA_INDEXES[j,2]=-1;
//                        }
//                    }
//                }
//            }
//            #endregion

//            #region Gestion de la nomenclature media

//            #region Triage des données suivant la nomenclature media
//            string sortStr = "";
//            switch(webSession.PreformatedMediaDetail){
//                case FormatCst.PreformatedMediaDetails.vehicle:
//                    sortStr = "ID_M1";
//                    break;
//                case FormatCst.PreformatedMediaDetails.vehicleCategory:
//                case FormatCst.PreformatedMediaDetails.vehicleMedia:
//                    sortStr = "ID_M1,ID_M2";
//                    break;
//                case FormatCst.PreformatedMediaDetails.vehicleCategoryMedia:
//                    sortStr = "ID_M1,ID_M2,ID_M3";
//                    break;
//                default:
//                    throw new DynamicTableRulesException("Le format de détail " + webSession.PreformatedMediaDetail.ToString() + " n'est pas un cas valide.");
//            }
//            DataRow[] medias = dsData.Tables[0].Select("", sortStr);
//            #endregion

//            #region Initialisation des constantes media
//            Hashtable MEDIA_RESULT_INDEXES = new Hashtable();

//            //Index de la premiere colonne contennant un media
//            FIRST_MEDIA_RESULT_INDEX = FIRST_PRODUCT_RESULT_INDEX+PRODUCT_DATA_INDEXES.GetLength(0);

//            //recupération index de la première colone media "id_m"
//            FIRST_MEDIA_DATA_INDEX = dtData.Columns["ID_M1"].Ordinal;

//            //outil dynamique pour cibler les colonnes contenants les données qualitatives media
//            MEDIA_DATA_INDEXES = new int[(FIRST_DATA_INDEX-FIRST_MEDIA_DATA_INDEX)/2, 3];
//            //initialisation
//            for(i=0; i<MEDIA_DATA_INDEXES.GetLength(0); i++){
//                //index colonne
//                MEDIA_DATA_INDEXES[i,0]=FIRST_MEDIA_DATA_INDEX + 2*i;
//                //old value
//                MEDIA_DATA_INDEXES[i,1]=-1;
//                //indexe dans le tableau de resultat
//                MEDIA_DATA_INDEXES[i,2]=0;
//            }
//            #endregion

//            #region Construction de la liste des colonnes de la nomenclatures media et de la liste des entête du tableau de resultat
//            int numColumn;
//            //Pluri ou non? Si oui pluri total a faire sinon, non
//            //Colonne total : plurimedia ou alors champ "m1"
//            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID != DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
//                numColumn = FIRST_MEDIA_RESULT_INDEX;
//            }
//            else{
//                headers.Append(TOTAL);
//                numColumn = FIRST_MEDIA_RESULT_INDEX + 1;
//                MEDIA_RESULT_INDEXES.Add(0, FIRST_MEDIA_RESULT_INDEX);
//            }
//            int[] tmpIndexes = null;
//            foreach(DataRow currentRow in medias){
//                //Créations de la liste des entêtes et de l indexage des colonnes media
//                for(i=0; i<MEDIA_DATA_INDEXES.GetLength(0); i++){
//                    if (int.Parse(currentRow[MEDIA_DATA_INDEXES[i,0]].ToString())!= MEDIA_DATA_INDEXES[i,1]){
//                        //sauvegarde du niveau courant
//                        MEDIA_DATA_INDEXES[i,1] = int.Parse(currentRow[MEDIA_DATA_INDEXES[i,0]].ToString());
//                        MEDIA_DATA_INDEXES[i,2] = numColumn;
//                        if (i==MEDIA_DATA_INDEXES.GetUpperBound(0)){
//                            //on ne doit ajouter que le niveau le pus bas dans la liste des colonnes resultats:
//                            //pour un detail pluri/media ==> ajout du niveau media
//                            //pour un detail pluri / media / category ==> ajout du niveau categorie

//                            //Créations de la liste des colonnes a affecter pour le niveau media courant
//                            tmpIndexes = new int[i+1];
//                            for(j=0; j<=i; j++){
//                                tmpIndexes[j] = MEDIA_DATA_INDEXES[j,2];
//                            }
//                            MEDIA_RESULT_INDEXES.Add(Int64.Parse(currentRow[MEDIA_DATA_INDEXES[MEDIA_DATA_INDEXES.GetUpperBound(0),0]].ToString()), tmpIndexes);
//                            //ajout de l'entete de la colonne
//                            //ajout de l'entete de la colonne
//                            headers.Append( ((headers.Length<=0)?"":",") + currentRow[MEDIA_DATA_INDEXES[i,0]+1].ToString());
//                        }
//                        else{
//                            //ajout de l'entete de la colonne
//                            headers.Append((headers.Length>0?"|":"") + currentRow[MEDIA_DATA_INDEXES[i,0]+1].ToString() + (headers.Length>0?("," + GestionWeb.GetWebWord(1102,webSession.SiteLanguage)):""));
//                        }
//                        numColumn++;
//                    }
//                }
//            }
//            //A ce stade, on dispose de:
//            //la liste des entetes séparées par des virgules si le detail ne va pas jusqu au support
//            //ou comme il suit MEDIA|CAT1,SUP1,SUP2|CAT3,SUP3,SUP5,SUP4,SUP8
//            //pour chaque plus bas niveau media, de l'indexe des colonnes qu'il affecte (la sienne plus les totaux)
//            #endregion

//            #endregion

//            #region Initialisation du tableau de resultat : création et entetes de tableau
//            //instanciation du tableau de resultat.
//                //taille : (nb de ligne de données + nb de lignes d'entêtes(2 au max)) et (nb de colonnes de produits + nb de colonnes de media + nb de totaux medias)
//                //nb de totaux media = 0 si on est en plurimedia ou en mono détaillé par média. sinon, on peut se baser sur le nombre d'entetes principale)
//            mainHeaders = headers.ToString().Split('|');
//            int offset = (webSession.PreformatedMediaDetail==FormatCst.PreformatedMediaDetails.vehicle)?0:mainHeaders.Length;
//            data = new object[NB_LINE + Math.Min(mainHeaders.Length,2),PRODUCT_DATA_INDEXES.GetLength(0)+MEDIA_RESULT_INDEXES.Count+offset+PERSO_COLUMN];
//            //Remplissage des entêtes
//            if (mainHeaders.Length<2){
//                //une seule ligne d'entete <== pas de detail support
//                currentLine = 0;
//            }
//            else{
//                //deux lignes d'entetes <== detail support
//                currentLine = 1;
//            }
//            //Premiere entete au dessus de la premiere colonne de données
//            i=FIRST_MEDIA_RESULT_INDEX;
//            foreach(string str in mainHeaders){
//                string[] lowerHeaders = str.Split(',');
//                data[0,i] = lowerHeaders[0];
//                //si on ne dispose que d'une entete dans la chaine principale ou dans la sous chaine, 
//                //c qu on est dans le cas d'une chaine TOTO|TITI|TATA donc on a une seule ligne d'entete et 
//                //on avance dans la cellule voisine
//                if (lowerHeaders.Length<2||mainHeaders.Length<2)i++;
//                for(j=1; j<lowerHeaders.Length; j++){
//                    data[currentLine,i] = lowerHeaders[j];
//                    i++;
//                }
//            }
//            #endregion

//            #region Alimentation du tableau sans calculs d'évlution, pdm ou pdv

//            #region Réinitialisation des "oldLevelId" dans le PRODUCT_DATA_INDEXES
//            for(i=0; i<=PRODUCT_DATA_INDEXES.GetUpperBound(0); i++){
//                PRODUCT_DATA_INDEXES[i,2]=-1;
//            }
//            #endregion

//            #region Alimentation du tableau en données
//            yearN= FctUtilities.Dates.getPeriodLabel(webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.currentYear);
//            yearN1=FctUtilities.Dates.getPeriodLabel(webSession,TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.previousYear);
//            //	2 l'etude ne porte que sur 1 année
//            //	3 l'etude porte sur 2 année
//            if (!webSession.ComparativeStudy)nbYearData=2;
//            else nbYearData = 3;

//            //Ligne de total
//            currentLine++;
//            FIRST_RESULT_LINE_INDEX = currentLine;
//            for (j=currentLine; j< currentLine+ nbYearData+NB_OPTION ; j++){
//                for(i=0; i < FIRST_MEDIA_RESULT_INDEX; i++)data[j, i] = null;
//                for(i=FIRST_MEDIA_RESULT_INDEX; i< data.GetLength(1)-PERSO_COLUMN; i++)data[j, i]=0.0;
//                if (PERSO_COLUMN>0){
//                    data[j,data.GetUpperBound(1)] = WebCst.AdvertiserPersonalisation.Type.none;
//                }
//            }
//            data[FIRST_RESULT_LINE_INDEX,0] = "TOTAL";
//            data[FIRST_RESULT_LINE_INDEX+1,0] = yearN;
//            k=2;
//            if(nbYearData>2){
//                data[FIRST_RESULT_LINE_INDEX+2,0]=yearN1;
//                k++;
//                if (webSession.Evolution){
//                    data[FIRST_RESULT_LINE_INDEX+3, 0] = GestionWeb.GetWebWord(1168, webSession.SiteLanguage);
//                    k++;
//                }
//            }

//            if(webSession.PDM){
//                data[FIRST_RESULT_LINE_INDEX+k, 0] = GestionWeb.GetWebWord(806, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN;
//                k++;
//                if (nbYearData>2) {
//                    data[FIRST_RESULT_LINE_INDEX+k, 0] = GestionWeb.GetWebWord(806, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN1;
//                    k++;
//                }
//            }
			
//            if(webSession.PDV){
//                data[FIRST_RESULT_LINE_INDEX+k, 0] = GestionWeb.GetWebWord(1166, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN;
//                k++;
//                if (nbYearData>2) {
//                    data[FIRST_RESULT_LINE_INDEX+k, 0] = GestionWeb.GetWebWord(1166, webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN1;
//                }
//            }

			

//            foreach(DataRow currentRow in dtData.Rows){
				
//                #region Données qualitatives
//                for(i=0; i<=PRODUCT_DATA_INDEXES.GetUpperBound(0); i++){
					
//                    //Pour chaque niveau de nomenclature (nomenclature produit uniquement)
//                    if(PRODUCT_DATA_INDEXES[i,2]!=int.Parse(currentRow[PRODUCT_DATA_INDEXES[i,0]].ToString())){
						
//                        //incrémentation de la ligne courante
//                        currentLine += nbYearData+NB_OPTION;

//                        //initialisation des cellules du tableau
//                        for(k = 0; k<nbYearData+NB_OPTION; k++){
//                            //Pour chaque ligne du bloc courant
//                            //Mettre a null toutes les colonnes correspondant aux niveaux supérieurs dans chacune des lignes nécessaires
//                            for(j=0; j<i; j++){data[currentLine+k, j]=null;}
//                            //Mettre a null les niveaux inférieurs et a 0 les données;
//                            for(j=i+1; j<FIRST_MEDIA_RESULT_INDEX; j++) data[currentLine+k, j]=null;
//                            for(j=FIRST_MEDIA_RESULT_INDEX; j<data.GetLength(1)-PERSO_COLUMN; j++) data[currentLine+k, j]=0.0;
//                            if (PERSO_COLUMN>0) data[currentLine+k, data.GetUpperBound(1)] = WebCst.AdvertiserPersonalisation.Type.none;
//                        }

//                        //Renseigner la ligne courante
//                        data[currentLine, i] = currentRow[PRODUCT_DATA_INDEXES[i,0]+1].ToString();
//                        data[currentLine+1, i] = yearN;
//                        //Annonceur normal, reference ou concurrent
//                        //si on visualise les elmt perso et
//                        //		si on considère un niveau de détail avec advertiser en niveau pere
//                        //		ou si on est en detail gp/Annon ou Gp/Ref et que c le niveau le plus bas
//                        //			si l'annonceur considéré est reference ou concurrents...
//                        if(PERSO_COLUMN>0 && 
//                            ( webSession.PreformatedProductDetail.ToString().StartsWith(FormatCst.PreformatedProductDetails.advertiser.ToString())
//                            || (( webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupAdvertiser
//                            ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupProduct
//                            ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.brand
//                            ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentAdvertiser
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentProduct
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.product
//                            )
//                            && i == PRODUCT_DATA_INDEXES.GetUpperBound(0)))
//                            ){
//                            if (Contains(referenceSelection,currentRow["id_advertiser"].ToString())){
//                                data[currentLine,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.reference;
//                                data[currentLine+1,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.reference;
//                            }
//                            else if (Contains(concurrentSelection,currentRow["id_advertiser"].ToString())){
//                                data[currentLine,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                                data[currentLine+1,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                            }

//                        }

//                        //Année N-2?
//                        k=2;
//                        if (nbYearData>2){ 
//                            data[currentLine+2, i] = yearN1;
//                            data[currentLine+2, data.GetLength(1)-1] = data[currentLine,data.GetLength(1)-1];
//                            k++;
//                            if (webSession.Evolution){
//                                data[currentLine+3, i] = GestionWeb.GetWebWord(1168, webSession.SiteLanguage);
//                                k++;
//                            }
//                        }
//                        if(webSession.PDV){
//                            data[currentLine+k, i] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN;
//                            k++;
//                            if (nbYearData>2)
//                            {
//                                data[currentLine+k, i] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN1;
//                                k++;
//                            }
//                        }
//                        if(webSession.PDM){
//                            data[currentLine+k, i] = GestionWeb.GetWebWord( 806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN;
//                            k++;
//                            if (nbYearData>2)
//                            {
//                                data[currentLine+k, i] = GestionWeb.GetWebWord(806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187, webSession.SiteLanguage) + yearN1;
//                                k++;
//                            }
//                        }

//                        //Sauvegarde du niveau courant
//                        PRODUCT_DATA_INDEXES[i,2]=int.Parse(currentRow[PRODUCT_DATA_INDEXES[i,0]].ToString());
//                        PRODUCT_DATA_INDEXES[i,1]=currentLine;
//                        //Réinitialisation des niveaux inférieurs
//                        for(j=i+1; j<=PRODUCT_DATA_INDEXES.GetUpperBound(0); j++){
//                            PRODUCT_DATA_INDEXES[j,2]=-1;
//                        }
//                    }
//                }
//                #endregion

//                #region année N
//                for(i=0; i<PRODUCT_DATA_INDEXES.GetLength(0);i++){
//                    for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
//                        //année N
//                        data[PRODUCT_DATA_INDEXES[i,1]+1, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                            double.Parse(data[PRODUCT_DATA_INDEXES[i,1]+1, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString()));

//                        //total multi année
//                        data[PRODUCT_DATA_INDEXES[i,1], ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                            double.Parse(data[PRODUCT_DATA_INDEXES[i,1], ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString()));
//                    }
//                    //Plurimedia? oui ==> calcul des totaux de la colonne pluri 
//                    if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
//                        //total année
//                        data[PRODUCT_DATA_INDEXES[i,1]+1, FIRST_MEDIA_RESULT_INDEX] = 
//                            double.Parse(data[PRODUCT_DATA_INDEXES[i,1]+1, FIRST_MEDIA_RESULT_INDEX].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString()));

//                        //total multi année
//                        data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX] = 
//                            double.Parse(data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString()));
//                    }
//                }
//                //Total général
//                for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
//                    //sur l annee
//                    data[FIRST_RESULT_LINE_INDEX+1, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                        double.Parse(data[FIRST_RESULT_LINE_INDEX+1, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                        + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString()));

//                    //multi annee
//                    data[FIRST_RESULT_LINE_INDEX, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                        double.Parse(data[FIRST_RESULT_LINE_INDEX, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                        + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString())); 

//                }
//                //Plurimedia? oui ==> calcul des totaux de la colonne pluri 
//                if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
//                    //total année
//                    data[FIRST_RESULT_LINE_INDEX+1, FIRST_MEDIA_RESULT_INDEX] = 
//                        double.Parse(data[FIRST_RESULT_LINE_INDEX+1, FIRST_MEDIA_RESULT_INDEX].ToString())
//                        + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString()));

//                    data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX] = 
//                        double.Parse(data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX].ToString())
//                        + (double.Parse(currentRow[FIRST_DATA_INDEX].ToString()));
//                }
//                #endregion

//                #region Année N-1 si étude comparative
//                if (nbYearData>2){
//                    //année N : renseigner la cellule (produit X media) et les totaux horizontaux
//                    for(i=0; i<PRODUCT_DATA_INDEXES.GetLength(0);i++){
//                        for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
//                            //année N
//                            data[PRODUCT_DATA_INDEXES[i,1]+2, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                                double.Parse(data[PRODUCT_DATA_INDEXES[i,1]+2, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));

//                            //total multi année
//                            data[PRODUCT_DATA_INDEXES[i,1], ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                                double.Parse(data[PRODUCT_DATA_INDEXES[i,1], ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));

//                        }
//                        //Plurimedia? oui ==> calcul des totaux de la colonne pluri 
//                        if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
//                            //total année
//                            data[PRODUCT_DATA_INDEXES[i,1]+2, FIRST_MEDIA_RESULT_INDEX] = 
//                                double.Parse(data[PRODUCT_DATA_INDEXES[i,1]+2, FIRST_MEDIA_RESULT_INDEX].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));

//                            //total multi année
//                            data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX] = 
//                                double.Parse(data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));
//                        }
//                    }
//                    //Totaux général
//                    //Plurimedia? oui ==> calcul des totaux de la colonne pluri 
//                    if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
//                        data[FIRST_RESULT_LINE_INDEX+2, FIRST_MEDIA_RESULT_INDEX] = 
//                            double.Parse(data[FIRST_RESULT_LINE_INDEX+2, FIRST_MEDIA_RESULT_INDEX].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));

//                        data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX] = 
//                            double.Parse(data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));

//                    }

//                    //colonnes des medias
//                    for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
//                        //sur l annee
//                        data[FIRST_RESULT_LINE_INDEX+2, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                            double.Parse(data[FIRST_RESULT_LINE_INDEX+2, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));

//                        //multi annee
//                        data[FIRST_RESULT_LINE_INDEX, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]] = 
//                            double.Parse(data[FIRST_RESULT_LINE_INDEX, ((int[])MEDIA_RESULT_INDEXES[long.Parse(currentRow[FIRST_DATA_INDEX-2].ToString())])[j]].ToString())
//                            + (double.Parse(currentRow[FIRST_DATA_INDEX+1].ToString()));
//                    }

//                }
//                #endregion

//            }
//            #endregion

//            #endregion

//            #region Calculs d'évolutions, de PDM, de PDV

//            #region Initialisation des "totaux" d'es niveaus intermédiaires (cad =/ du pus bas bniveau qui ne sera pas une reference total)
//            //Product_Index contiendra sur une ligne i les totaux du niveau de nomenclature supérieur au niveau i
//            //ligne 0 : totaux généraux pour le calcul des PDM du niveau 0
//            double[,] TOTAL_INDEXES_N = new double[FIRST_MEDIA_RESULT_INDEX,data.GetLength(1)-FIRST_MEDIA_RESULT_INDEX-PERSO_COLUMN];
//            double[,] TOTAL_INDEXES_N_1 = new double[FIRST_MEDIA_RESULT_INDEX,data.GetLength(1)-FIRST_MEDIA_RESULT_INDEX-PERSO_COLUMN];

//            for (i=0; i < TOTAL_INDEXES_N.GetLength(0); i++)
//            {
//                for(j=0; j < TOTAL_INDEXES_N.GetLength(1); j++){
//                    TOTAL_INDEXES_N[i,j] = 0.0;
//                    if (webSession.ComparativeStudy)
//                        TOTAL_INDEXES_N_1[i,j] = 0.0;
//                }
//            }
//            #endregion

//            #region Calculs
//            currentLine = FIRST_RESULT_LINE_INDEX;

//            for(i=FIRST_RESULT_LINE_INDEX; i<=data.GetUpperBound(0); i++){

//                //Extraction de l'information niveau
//                for(j=0; j<FIRST_MEDIA_RESULT_INDEX; j++){
//                    if (data[i,j]!=null) break;
//                }

//                //Edition des informations evol, PDM, PDV de chaque colonne
//                for(k=FIRST_MEDIA_RESULT_INDEX; k < data.GetLength(1)-PERSO_COLUMN; k++){

//                    //si ligne total, affectation dans j cad 0
//                    //sinon, si ligne different du total et niveau différent du plus bas, affectation dans j+1 cad niveau+1
//                    if(i==FIRST_RESULT_LINE_INDEX)
//                    {
//                        TOTAL_INDEXES_N[j,k-FIRST_MEDIA_RESULT_INDEX] = double.Parse(data[i+1,k].ToString());
//                        if (webSession.ComparativeStudy)
//                            TOTAL_INDEXES_N_1[j,k-FIRST_MEDIA_RESULT_INDEX] = double.Parse(data[i+2,k].ToString());
//                    }
//                    else if(j<FIRST_MEDIA_RESULT_INDEX-1)
//                    {
//                        TOTAL_INDEXES_N[j+1,k-FIRST_MEDIA_RESULT_INDEX] = double.Parse(data[i+1,k].ToString());
//                        if (webSession.ComparativeStudy)
//                            TOTAL_INDEXES_N_1[j+1,k-FIRST_MEDIA_RESULT_INDEX] = double.Parse(data[i+2,k].ToString());
//                    }

//                    currentLine = i + nbYearData;

//                    //calcul evolution
//                    if (webSession.ComparativeStudy && webSession.Evolution){
//                        if (PERSO_COLUMN>0) data[currentLine, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
//                        data[currentLine,k] = 100 * ( double.Parse(data[i+1,k].ToString())
//                            - double.Parse(data[i+2,k].ToString()))
//                            / double.Parse(data[i+2,k].ToString());
//                        currentLine++;
//                    }

//                    //calcul PDV
//                    if (webSession.PDV){
//                        if (PERSO_COLUMN>0) {
//                            data[currentLine, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
//                            if(webSession.ComparativeStudy)
//                                data[currentLine+1, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
//                        }
//                        if(i!=FIRST_RESULT_LINE_INDEX){
//                            data[currentLine,k] = 100 * double.Parse(data[i+1,k].ToString()) 
//                                / TOTAL_INDEXES_N[j,k-FIRST_MEDIA_RESULT_INDEX];
//                            if (webSession.ComparativeStudy)
//                                data[currentLine+1,k] = 100 * double.Parse(data[i+2,k].ToString()) 
//                                    / TOTAL_INDEXES_N_1[j,k-FIRST_MEDIA_RESULT_INDEX];
//                        }
//                        else{
//                            data[currentLine,k] = 100;
//                            if (webSession.ComparativeStudy)
//                                data[currentLine+1,k] = 100;
//                        }
//                        currentLine+=(webSession.ComparativeStudy)?2:1;
//                    }

//                    //calcul PDM
//                    if (webSession.PDM){
//                        if (PERSO_COLUMN>0) {
//                            data[currentLine, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
//                            if (webSession.ComparativeStudy)
//                                data[currentLine+1, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
//                        }
//                        if(k != FIRST_MEDIA_RESULT_INDEX) {
//                            if (FIRST_RESULT_LINE_INDEX<2) {
//                                //PDV par rapport au total général
//                                if (double.Parse(data[i+1,FIRST_MEDIA_RESULT_INDEX].ToString()) != 0) {
//                                    data[currentLine,k] = 100 * double.Parse(data[i+1,k].ToString())
//                                        / double.Parse(data[i+1,FIRST_MEDIA_RESULT_INDEX].ToString());
//                                }
//                                else {
//                                    data[currentLine,k] = null;
//                                }
//                                if (webSession.ComparativeStudy) {
//                                    if (double.Parse(data[i+2,FIRST_MEDIA_RESULT_INDEX].ToString()) != 0) {
//                                        data[currentLine+1,k] = 100 * double.Parse(data[i+2,k].ToString())
//                                            / double.Parse(data[i+2,FIRST_MEDIA_RESULT_INDEX].ToString());
//                                    }
//                                    else {
//                                        data[currentLine+1,k] = null;
//                                    }
//                                }

//                            }
//                            else {
//                                //pdv par rapport a un sous total a rechercher. 
//                                //trouver total si colonne courante = sous total ou sinon trouver "sous total"=GestionWeb.GetWebWord(1102,webSession.SiteLanguage))
//                                if (data[FIRST_RESULT_LINE_INDEX-1,k].ToString() == GestionWeb.GetWebWord(1102,webSession.SiteLanguage)) {
//                                    l = FIRST_MEDIA_RESULT_INDEX;
//                                }
//                                else {
//                                    for(l=k-1; l>=FIRST_MEDIA_RESULT_INDEX; l--) {
//                                        if (! (data[FIRST_RESULT_LINE_INDEX-1, l].ToString() != GestionWeb.GetWebWord(1102,webSession.SiteLanguage))) {
//                                            break;
//                                        }
//                                    }
//                                }
//                                if(double.Parse(data[i+1,l].ToString())!=0)
//                                    data[currentLine,k] = 100 * double.Parse(data[i+1,k].ToString())
//                                        / double.Parse(data[i+1,l].ToString());
//                                else
//                                    data[currentLine,k] = null;
//                                if (webSession.ComparativeStudy) {
//                                    if(double.Parse(data[i+2,l].ToString())!=0)
//                                        data[currentLine+1,k] = 100 * double.Parse(data[i+2,k].ToString())
//                                            / double.Parse(data[i+2,l].ToString());
//                                    else
//                                        data[currentLine+1,k] = null;
//                                }

//                            }
//                        }
//                        else {
//                            data[currentLine,k] = 100;
//                            if (webSession.ComparativeStudy)
//                                data[currentLine+1,k] = 100;
//                        }
//                    }
//                }

//                i += nbYearData + NB_OPTION - 1;

//            }

//            #endregion

//            #endregion

//            return data;

//        }
//        #endregion

//        #region Tableau de type 6 7 8 9 
//        private static object[,] BuildDataTableType_6_7_8_9(DataSet dsData, WebSession webSession){

//            DataTable dtData = dsData.Tables[0];

//            if (dtData.Rows.Count<=0) return null;


//            #region Variables
//            object[,] data;

//            int i,j,k;

//            int FIRST_DATA_INDEX;
//            int[,] CLASSIF_INDEXES;
//            int NB_LINE;
//            int NB_OPTION = 0;
//            int NB_YEAR = 2;
//            string YEAR_N = "";
//            string YEAR_N1 = "";
//            int currentLine;
//            bool media = false;
//            bool mediaN1 = false;
//            bool product = false;
//            bool productN1 = false;
//            bool evolution = false;

//            #endregion

//            #region Constantes

//            //Notion de personnalisation? dernière colonne = advertiser ==> perso_column=dernière colonne
//            int PERSO_COLUMN = 0;
//            string[] referenceSelection = null;
//            string[] concurrentSelection = null;
//            if(dtData.Columns.Contains("id_advertiser")){
//                PERSO_COLUMN=1;
//                //referenceSelection = webSession.GetSelection(webSession.ReferenceUniversAdvertiser, RightCst.type.advertiserAccess).Split(',');
//                //concurrentSelection= (webSession.CompetitorUniversAdvertiser[0]!=null)?webSession.GetSelection( ((TreeNode)webSession.CompetitorUniversAdvertiser[0]), RightCst.type.advertiserAccess).Split(','):null;
//                string tempString = "";				
//                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) referenceSelection = tempString.Split(',');
//                }
//                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1) && webSession.SecondaryProductUniverses[1].Contains(0)) {
//                    tempString = webSession.SecondaryProductUniverses[1].GetGroup(0).GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempString != null && tempString.Length > 0) concurrentSelection = tempString.Split(',');
//                }
//            }

//            //Etude comparatif ou monoannuelle?
//            if (!webSession.ComparativeStudy){
//                NB_YEAR--;
//            }
//            else if (webSession.Evolution){
//                evolution = true;
//                NB_OPTION++;
//            }

//            //nombre d'options PDM, PDV, Evol
//            if (webSession.PDM && webSession.PreformatedTable.ToString().StartsWith("media")){
//                media = true;
//                if (NB_YEAR>1)
//                    mediaN1 = true;
//                NB_OPTION += NB_YEAR;
//            }

//            if (webSession.PDV && webSession.PreformatedTable.ToString().StartsWith("product")){
//                product = true;
//                if (NB_YEAR>1)
//                    productN1 = true;
//                NB_OPTION += NB_YEAR;
//            }


//            //Calcul de l'index de la première colonne a contenir des données quantitatives
//            for(i=0; i<dtData.Columns.Count; i=i+2){
//                if (dtData.Columns[i].ColumnName.IndexOf("ID_M")<0 && dtData.Columns[i].ColumnName.IndexOf("ID_P")<0){
//                    break;
//                }
//            }
//            FIRST_DATA_INDEX=i;
//            //Supprime les lignes vides
//            dtData=DynamicDataTable(dsData,FIRST_DATA_INDEX,PERSO_COLUMN);

//            //Edition de la liste des colonnes descriptives (media, produit, groupe, category...) et initialisation
//            //de la partie "nbLevel" et "oldLevelID"
//            CLASSIF_INDEXES = new int[FIRST_DATA_INDEX/2, 3];
//            for(i = 0; i<(FIRST_DATA_INDEX); i+=2){
//                CLASSIF_INDEXES[i/2, 0]=i;
//                CLASSIF_INDEXES[i/2, 1]=0;
//                CLASSIF_INDEXES[i/2, 2]=-1;
//            }
//            #endregion

//            #region Calcul du nombre de lignes au maximum et initialisation de data

//            if (webSession.PreformatedTable == FormatCst.PreformatedTables.productYear_X_Cumul
//                || webSession.PreformatedTable == FormatCst.PreformatedTables.productYear_X_Mensual
//                || ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode())
//            {	
//                NB_LINE = 1+NB_YEAR+NB_OPTION;
//            }
//            else{
//                NB_LINE = 0;
//            }

//            foreach(DataRow currentRow in dtData.Rows){
//                for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
//                    if(CLASSIF_INDEXES[i,2]!=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString())){
//                        NB_LINE+= 1+NB_YEAR+NB_OPTION;
//                        CLASSIF_INDEXES[i,2]=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString());
//                        for(j=i+1; j<=CLASSIF_INDEXES.GetUpperBound(0); j++){
//                            CLASSIF_INDEXES[j,2]=-1;
//                        }
//                    }
//                }
//            }
//            //Initialisation de data:
//                //taille : nb de ligne...    nb de colonne = nombre de colonne dans la table/nb d'année + le nb de niveaux de classif
//            data = new object[NB_LINE,(dtData.Columns.Count - PERSO_COLUMN - 2*CLASSIF_INDEXES.GetLength(0)) / NB_YEAR + CLASSIF_INDEXES.GetLength(0) + PERSO_COLUMN];
//            #endregion

//            #region Construction du tableau
//            //Réinitialisation des "oldIdLevel"
//            for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
//                CLASSIF_INDEXES[i,2]=-1;
//            }

//            YEAR_N = webSession.PeriodBeginningDate.Substring(0,4);
//            if (NB_YEAR>1)YEAR_N1 = (int.Parse(YEAR_N)-1).ToString();
			

//            currentLine = -(1+NB_YEAR+NB_OPTION);

//            //Ligne de total
//            if (webSession.PreformatedTable == FormatCst.PreformatedTables.productYear_X_Cumul
//                || webSession.PreformatedTable == FormatCst.PreformatedTables.productYear_X_Mensual
//                || ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode())
//            {
//                for(j=0; j<=NB_YEAR+NB_OPTION; j++){
//                    for(i=0; i < CLASSIF_INDEXES.GetLength(0); i++)data[j, i] = null;
//                    for(i=CLASSIF_INDEXES.GetLength(0); i < data.GetLength(1) - PERSO_COLUMN; i++)data[j, i] = 0.0;
//                    if (PERSO_COLUMN>0) data[j, data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.none;
//                }
//                data[0,0] = "TOTAL";
//                data[1,0] = YEAR_N;
//                if(NB_YEAR>1)data[2,0] = YEAR_N1;
//                if(evolution)data[1+NB_YEAR,0] = GestionWeb.GetWebWord(1168,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                if(media)
//                {
//                    if (mediaN1)
//                    {
//                        data[NB_YEAR+NB_OPTION-1,0] = GestionWeb.GetWebWord(806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                        data[NB_YEAR+NB_OPTION,0] = GestionWeb.GetWebWord(806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N1;
//                    }
//                    else
//                    {
//                        data[NB_YEAR+NB_OPTION,0] = GestionWeb.GetWebWord(806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                    }
//                }
//                if(product)
//                {
//                    if (productN1)
//                    {
//                        data[NB_YEAR+NB_OPTION-1,0] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                        data[NB_YEAR+NB_OPTION,0] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N1;
//                    }
//                    else
//                    {
//                        data[NB_YEAR+NB_OPTION,0] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                    }
//                }
//                currentLine = 0;
//            }

			
//            foreach(DataRow currentRow in dtData.Rows){

//                for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
//                    //Pour chaque niveau de nomenclature (produit ou media indifféremment)
//                    if(CLASSIF_INDEXES[i,2]!=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString())){

//                        //niveau différent du précédent ==> nouvelle ligne dans le tableau
//                        currentLine += 1+NB_YEAR+NB_OPTION;
						
//                        //Pour chaque année, chaque option et la ligne mmulti années
//                        for(k=0; k <= NB_YEAR+NB_OPTION; k++){
//                            //Mettre a null toutes les colonnes correspondant aux niveaux supérieurs
//                            for(j=0; j<i; j++) data[currentLine, j]=null;
//                            //Initialisation des cellules de données
//                            for(j=CLASSIF_INDEXES.GetLength(0); j < data.GetLength(1)-PERSO_COLUMN; j++)data[currentLine+k, j] = 0.0;
//                            //Mettre a null les niveaux inférieurs;
//                            for(j=i+1; j<=(CLASSIF_INDEXES.GetUpperBound(0)-2); j++) data[currentLine+k, j]=null;
//                            if (PERSO_COLUMN>0) data[currentLine+k, data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.none;
//                        }
//                        //Renseigner la ligne courante
//                        data[currentLine, i] = currentRow[CLASSIF_INDEXES[i,0]+1].ToString();
//                        data[currentLine+1, i] = YEAR_N;
//                        if (YEAR_N1!="")data[currentLine+2, i] = YEAR_N1;
//                        if (webSession.Evolution)data[currentLine+NB_YEAR+1, i] = GestionWeb.GetWebWord(1168,webSession.SiteLanguage);
//                        if (media)
//                        {
//                            if (!mediaN1)
//                            {
//                                data[currentLine+NB_YEAR+NB_OPTION, i] = GestionWeb.GetWebWord(806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                            }
//                            else
//                            {
//                                data[currentLine+NB_YEAR+NB_OPTION-1, i] = GestionWeb.GetWebWord(806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                                data[currentLine+NB_YEAR+NB_OPTION, i] = GestionWeb.GetWebWord(806,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N1;
//                            }
//                        }
//                        if (product)
//                        {
//                            if (!productN1)
//                            {
//                                data[currentLine+NB_YEAR+NB_OPTION, i] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                            }
//                            else
//                            {
//                                data[currentLine+NB_YEAR+NB_OPTION-1, i] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N;
//                                data[currentLine+NB_YEAR+NB_OPTION, i] = GestionWeb.GetWebWord(1166,webSession.SiteLanguage) + GestionWeb.GetWebWord(1187,webSession.SiteLanguage) + YEAR_N1;
//                            }
//                        }
//                        //Annonceur
//                        if(PERSO_COLUMN>0 && 
//                            ( webSession.PreformatedProductDetail.ToString().StartsWith(FormatCst.PreformatedProductDetails.advertiser.ToString())
//                            || (( webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupAdvertiser
//                            ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupProduct
//                            ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.brand
//                            ||webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.groupBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentAdvertiser
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentBrand
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.segmentProduct
//                            || webSession.PreformatedProductDetail == FormatCst.PreformatedProductDetails.product
//                            )
//                            && i == CLASSIF_INDEXES.GetUpperBound(0)))
//                            ){
//                            if (Contains(referenceSelection,currentRow["id_advertiser"].ToString())){
//                                data[currentLine,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.reference;
//                                data[currentLine+1,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.reference;
//                                if(NB_YEAR>1) data[currentLine+2,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.reference;
//                            }
//                            else if (Contains(concurrentSelection,currentRow["id_advertiser"].ToString())){
//                                data[currentLine,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                                data[currentLine+1,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                                if(NB_YEAR>1) data[currentLine+2,data.GetLength(1)-1] = WebCst.AdvertiserPersonalisation.Type.competitor;
//                            }
//                        }

//                        //Sauvegarde du niveau courant
//                        CLASSIF_INDEXES[i,2]=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString());
//                        CLASSIF_INDEXES[i,1]=currentLine;
						
//                        //Réinitialisation des niveaux inférieurs
//                        for(j=i+1; j<=CLASSIF_INDEXES.GetUpperBound(0); j++)CLASSIF_INDEXES[j,2]=-1;
//                    }
//                }

//                //Données quantitatives de la ligne courante

//                for(i=1; i <= NB_YEAR; i++){
//                //Pour chaque année + le total multi années
//                    //Pour chaque colonne de données
//                    for(k = 0; k < ( dtData.Columns.Count - FIRST_DATA_INDEX - PERSO_COLUMN ); k+=NB_YEAR){


//                        for(j=0; j<CLASSIF_INDEXES.GetLength(0);j++){
//                        //Pour chaque niveau de nomenclature
//                            //ligne par année
//                            data[CLASSIF_INDEXES[j,1]+i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] = 
//                                double.Parse(data[CLASSIF_INDEXES[j,1]+i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)].ToString()));

//                            //ligne toutes années confondues
//                            data[CLASSIF_INDEXES[j,1], CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] = 
//                                double.Parse(data[CLASSIF_INDEXES[j,1], CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)].ToString()));
//                        }

//                        //Total général si nécessaire : plurimedia ou nomenclature produit
//                        if (webSession.PreformatedTable == FormatCst.PreformatedTables.productYear_X_Cumul
//                            || webSession.PreformatedTable == FormatCst.PreformatedTables.productYear_X_Mensual
//                            || ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == DBClassificationCst.Vehicles.names.plurimedia.GetHashCode())
//                        {
//                            //ligne par année
//                            data[i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] = 
//                                double.Parse(data[i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)].ToString()));

//                            //ligne toutes années confondues
//                            data[0, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] = 
//                                double.Parse(data[0, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR].ToString())
//                                + (double.Parse(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)].ToString()));
//                        }
//                    }
//                }
//            }
//            #endregion

//            #region Calculs

//            #region Initialisation des "totaux" d'es niveaus intermédiaires (cad =/ du pus bas bniveau qui ne sera pas une reference total)
//            int FIRST_RESULT_COLUMN = CLASSIF_INDEXES.GetLength(0);
//            //Product_Index contiendra sur une ligne i les totaux du niveau de nomenclature supérieur au niveau i
//            //ligne 0 : totaux généraux pour le calcul des PDM/PDV du niveau 0
//            double[,] TOTAL_INDEXES_N = new double[CLASSIF_INDEXES.GetLength(0),data.GetLength(1)-FIRST_RESULT_COLUMN-PERSO_COLUMN];
//            double[,] TOTAL_INDEXES_N_1 = new double[CLASSIF_INDEXES.GetLength(0),data.GetLength(1)-FIRST_RESULT_COLUMN-PERSO_COLUMN];
//            for (i=0; i < TOTAL_INDEXES_N.GetLength(0); i++)
//            {
//                for(j=0; j < TOTAL_INDEXES_N.GetLength(1); j++){
//                    TOTAL_INDEXES_N[i,j] = 0.0;
//                    if (productN1||mediaN1) TOTAL_INDEXES_N_1[i,j] = 0.0;
//                }
//            }
//            #endregion

//            #region Parcours du tableau
//            currentLine = 0;
//            bool monoMedia = false;
//            //pluri et présentation nomenclauture media
//            if ((webSession.PreformatedTable == FormatCst.PreformatedTables.mediaYear_X_Cumul
//                ||webSession.PreformatedTable == FormatCst.PreformatedTables.mediaYear_X_Mensual)
//                &&((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID != DBClassificationCst.Vehicles.names.plurimedia.GetHashCode()){
//                monoMedia = true;
//            }

//            for(i=0; i<=data.GetUpperBound(0); i++){

//                //Extraction de l'information niveau
//                for(j=0; j<FIRST_RESULT_COLUMN; j++){
//                    if (data[i,j]!=null) break;
//                }

//                if (PERSO_COLUMN>0){
//                    currentLine = i +  1 + NB_YEAR;
//                    //calcul evolution
//                    if (evolution){
//                        data[currentLine,data.GetLength(1)-1] = data[i,data.GetLength(1)-1];
//                        currentLine++;
//                    }

//                    //personnalisation PDM || PDV
//                    if (media || product){
//                        data[currentLine,data.GetLength(1)-1] = data[i,data.GetLength(1)-1];
//                        if (mediaN1||productN1)data[currentLine+1,data.GetLength(1)-1] = data[i,data.GetLength(1)-1];
//                    }
//                }


//                //Edition des informations evol, PDM, PDV de chaque colonne
//                for(k=FIRST_RESULT_COLUMN; k < data.GetLength(1)-PERSO_COLUMN; k++){
 
//                    //si ligne courante = total, affectation dans j cad 0
//                    //sinon, si ligne different du total et niveau différent du plus bas, affectation dans j+1 cad niveau+1
//                    if(i==0 && !monoMedia)
//                    {
//                        TOTAL_INDEXES_N[j,k-FIRST_RESULT_COLUMN] = double.Parse(data[i+1,k].ToString());
//                        if(mediaN1||productN1) 
//                            TOTAL_INDEXES_N_1[j,k-FIRST_RESULT_COLUMN] = double.Parse(data[i+2,k].ToString());
//                    }
//                    else if(j<FIRST_RESULT_COLUMN-1)
//                    {
//                        TOTAL_INDEXES_N[j+1,k-FIRST_RESULT_COLUMN] = double.Parse(data[i+1,k].ToString());
//                        if(mediaN1||productN1)
//                            TOTAL_INDEXES_N_1[j+1,k-FIRST_RESULT_COLUMN] = double.Parse(data[i+2,k].ToString());
//                    }

//                    currentLine = i +  1 + NB_YEAR;

//                    //calcul evolution
//                    if (evolution){
//                        data[currentLine,k] = 100 * ( double.Parse(data[i+1,k].ToString())
//                            - double.Parse(data[i+2,k].ToString()))
//                            / double.Parse(data[i+2,k].ToString());
//                        currentLine++;
//                    }

//                    //calcul PDM || PDV
//                    if (media || product){
//                        if(i!=0){
//                            if(double.Parse(TOTAL_INDEXES_N[j,k-FIRST_RESULT_COLUMN].ToString())!=0)
//                                data[currentLine,k] = 100 * double.Parse(data[i+1,k].ToString()) 
//                                    / TOTAL_INDEXES_N[j,k-FIRST_RESULT_COLUMN];
//                            else
//                                data[currentLine,k] = null;
//                        }
//                        else{
//                            data[currentLine,k] = 100;
//                        }
//                        if (mediaN1||productN1)
//                        {
//                            if(i!=0)
//                            {
//                                if(double.Parse(TOTAL_INDEXES_N_1[j,k-FIRST_RESULT_COLUMN].ToString())!=0)
//                                    data[currentLine+1,k] = 100 * double.Parse(data[i+2,k].ToString()) 
//                                        / TOTAL_INDEXES_N_1[j,k-FIRST_RESULT_COLUMN];
//                                else
//                                    data[currentLine+1,k] = null;
//                            }
//                            else
//                            {
//                                data[currentLine+1,k] = 100;
//                            }
//                        }

//                    }

//                }

//                i += NB_YEAR + NB_OPTION ;

//            }

//            #endregion

//            #endregion

//            return data;

//        }
//        #endregion

//        #region Méthodes internes
//        /// <summary>
//        /// Détermine si la valeur est contenue dans le tableau de string
//        /// </summary>
//        /// <param name="table">tableau de string</param>
//        /// <param name="valueToFind">valeur recherchée</param>
//        /// <returns>true si la chaine est contenue dans le tableau</returns>
//        protected static bool Contains(string[] table, string valueToFind){
//            if (table != null && table.Length > 0){
//                foreach(string str in table){
//                    if (str == valueToFind) return true;
//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// Methode pour supprimer les lignes vides
//        /// </summary>
//        /// <param name="dsData">Dataset provenant de la requête</param>
//        /// <param name="firstData">Première ligne avec les données</param>
//        /// <returns>DataTable sans les lignes sans données</returns>
//        /// <param name="advertiser">Existance de l'idAdvertiser dans la table 1 si il existe 0 sinon</param>
//        protected static DataTable DynamicDataTable(DataSet dsData,int firstData,int advertiser){
//            DataTable dtData = dsData.Tables[0];
//            bool existData=false;
//            int line=0;
//            int i=0;
//            int lengthTable;

			
//            ArrayList listLineToDelete=new ArrayList();
//            foreach(DataRow currentRow in dtData.Rows){
//                lengthTable=currentRow.ItemArray.Length-advertiser;
//                for(i=firstData;i<lengthTable;i++){
//                    if(double.Parse(currentRow[i].ToString())!=0){
//                        existData=true;
//                    }
//                }
//                if(!existData){
//                    listLineToDelete.Add(line);
//                }
//                existData=false;
//                line++;				
//            }		
//            line=listLineToDelete.Count-1;
//            for(i=0;i<listLineToDelete.Count;i++){
//                dtData.Rows.RemoveAt((int)listLineToDelete[line]);
//                line--;
//            }
			
//            return dtData;
//        }

//        #endregion
//    }
//}
