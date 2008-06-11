//#region Information
//// Auteur: Guillaume Facon
//// Créé le: 17/09/2004
//// Modifiée le:17/09/2004
////	G. Facon	11/08/2005	Nom des variables
//#endregion

//using System;

//namespace TNS.AdExpress.Web.Common.Results{
//    /// <summary>
//    /// Représente un groupe de sélection d'univers
//    /// </summary>
//    public class SelectionGroup{

//        #region Variables
//        /// <summary>
//        /// Identifiant du groupe
//        /// </summary>
//        protected int _id;
//        /// <summary>
//        /// Nombre d'éléments dans le groupe
//        /// </summary>
//        protected int _itemsNumber=0;
//        /// <summary>
//        /// Indexe du sous total dans le tableau de résultat
//        /// </summary>
//        protected  int _indexInResultTable=0;
//        #endregion

//        #region Constructeurs
//        /// <summary>
//        /// Constructeurs par défaut
//        /// </summary>
//        /// <param name="id">Identifiant du groupe</param>
//        public SelectionGroup(int id){
//            _id=id;
//        }

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        /// <param name="id">Identifiant du groupe</param>
//        /// <param name="itemsNumber">Nombre d'éléments dans le groupe</param>
//        public SelectionGroup(int id,int itemsNumber){
//            _id=id;
//            _itemsNumber=itemsNumber; 
//        }

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        /// <param name="id">Identifiant du groupe</param>
//        /// <param name="itemsNumber">Nombre d'éléments dans le groupe</param>
//        /// <param name="indexInResultTable">Indexe du sous total dans le tableau de résultat</param>
//        public SelectionGroup(int id,int itemsNumber,int indexInResultTable){
//            _id=id;
//            _itemsNumber=itemsNumber;
//            _indexInResultTable=indexInResultTable;
//        }
//        #endregion

//        #region Accesseurs
		
//        /// <summary>
//        /// Obtient l'identifiant du group
//        /// </summary>
//        public int Id{
//            get{return(_id);}
//        }

//        /// <summary>
//        /// Obtient le nombre d'éléments du group
//        /// </summary>
//        public int Count{
//            get{return(_itemsNumber);}
//        }

//        /// <summary>
//        /// Dédinit le nombre d'éléments du group
//        /// </summary>
//        public int SetItemsNumber{
//            set{_itemsNumber=value;}
//        }

//        /// <summary>
//        /// Obtient ou définit l'indexe du groupe dans un tableau de résultats
//        /// </summary>
//        public int IndexInResultTable{
//            get{return(_indexInResultTable);}
//            set{_indexInResultTable=value;}
//        }
//        #endregion
//    }
//}
