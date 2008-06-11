//#region Information
//// Auteur: Youssef R'kaina
//// Cr�� le: 28/12/2007
//// Modifi�e le:
//#endregion

//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace TNS.AdExpress.Web.Common.Results {
//    /// <summary>
//    /// Repr�sente un sous groupe de s�lection d'univers
//    /// </summary>
//    public class SelectionSubGroup : SelectionGroup {

//        #region Variables
//        /// <summary>
//        /// Identifiant du groupe p�re
//        /// </summary>
//        protected int _parentId;
//        /// <summary>
//        /// Identifiant du groupe dans la base de donn�es
//        /// </summary>
//        protected int _dataBaseId;
//        #endregion

//        #region Constructeurs
//        /// <summary>
//        /// Constructeurs par d�faut
//        /// </summary>
//        /// <param name="id">Identifiant du sous groupe</param>
//        public SelectionSubGroup(int id):base(id){
//        }

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        /// <param name="id">Identifiant du sous groupe</param>
//        /// <param name="itemsNumber">Nombre d'�l�ments dans le groupe</param>
//        public SelectionSubGroup(int id,int itemsNumber):base(id, itemsNumber){
//        }

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        /// <param name="id">Identifiant du sous groupe</param>
//        /// <param name="itemsNumber">Nombre d'�l�ments dans le groupe</param>
//        /// <param name="indexInResultTable">Indexe du sous total dans le tableau de r�sultat</param>
//        public SelectionSubGroup(int id, int itemsNumber, int indexInResultTable):base(id, itemsNumber, indexInResultTable) {
//        }

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        /// <param name="id">Identifiant du sous groupe</param>
//        /// <param name="itemsNumber">Nombre d'�l�ments dans le groupe</param>
//        /// <param name="indexInResultTable">Indexe du sous total dans le tableau de r�sultat</param>
//        /// <param name="parentId">Identifiant du groupe p�re</param>
//        public SelectionSubGroup(int id, int itemsNumber, int indexInResultTable, int parentId)
//            : base(id, itemsNumber, indexInResultTable) {
//            _parentId = parentId;
//        }

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        /// <param name="id">Identifiant du sous groupe</param>
//        /// <param name="itemsNumber">Nombre d'�l�ments dans le groupe</param>
//        /// <param name="indexInResultTable">Indexe du sous total dans le tableau de r�sultat</param>
//        /// <param name="parentId">Identifiant du groupe p�re</param>
//        /// <param name="dataBaseId">Identifiant du groupe dans la base de donn�es</param>
//        public SelectionSubGroup(int id, int itemsNumber, int indexInResultTable, int parentId, int dataBaseId)
//            : base(id, itemsNumber, indexInResultTable) {
//            _parentId = parentId;
//            _dataBaseId = dataBaseId;
//        }
//        #endregion

//        #region Accesseurs
//        /// <summary>
//        /// Obtient ou d�finit l'identifiant du group p�re
//        /// </summary>
//        public int ParentId {
//            get { return (_parentId); }
//            set { _parentId = value; }
//        }
//        /// <summary>
//        /// Obtient ou d�finit l'identifiant du group dans la base de donn�es
//        /// </summary>
//        public int DataBaseId {
//            get { return (_dataBaseId); }
//            set { _dataBaseId = value; }
//        }
//        #endregion

//    }
//}
