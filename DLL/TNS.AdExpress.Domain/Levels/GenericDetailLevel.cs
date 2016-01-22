#region Informations
// Auteur: G. Facon
// Cr�ation: 27/03/2006
// Modification:
#endregion

using System;
using System.Collections;
using System.Data;
using TNS.AdExpress.Constantes.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Domain.Level{
	/// <summary>
	/// Description d'un niveau de d�tail g�n�rique
	/// </summary>
	public class GenericDetailLevel{

		#region Variables
		/// <summary>
		/// Niveaux de d�tails
		/// </summary>
		/// <remarks>Contient des DetailLevelItemInformation</remarks>
		protected ArrayList _levels;
		///<summary>
		/// D�finit le type d'emplacement d'o� c'est la s�lection du niveau de d�tail
		/// </summary>
		///  <label>_selectedFrom</label>
		protected WebConstantes.GenericDetailLevel.SelectedFrom _selectedFrom;
		///<summary>
		/// Type de la liste
		/// </summary>
		///  <label>_type</label>
		protected WebConstantes.GenericDetailLevel.Type _type;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Contructeur avec une liste de niveaux de d�tail
		/// </summary>
		/// <param name="levelIds">Liste des identifiant des �l�ments duniveau de d�tail</param>
		/// <remarks>levelIds doit contenir des int</remarks>
		/// <exception cref="System.ArgumentNullException">Si la liste de niveaux de d�tail est null</exception>
		public GenericDetailLevel(ArrayList levelIds){
			_selectedFrom=WebConstantes.GenericDetailLevel.SelectedFrom.unknown;
			_type=WebConstantes.GenericDetailLevel.Type.unknown;
			if(levelIds==null)throw(new ArgumentNullException("levelsId list is null"));
			_levels=new ArrayList();
			foreach(int currentId in levelIds){
				_levels.Add(DetailLevelItemsInformation.Get(currentId));
			}
		}

		/// <summary>
		/// Contructeur avec une liste de niveaux de d�tail
		/// </summary>
		/// <param name="levelIds">Liste des identifiant des �l�ments duniveau de d�tail</param>
		/// <param name="selectedFrom">Niveau S�lectionn� � partir de</param>
		/// <remarks>levelIds doit contenir des int</remarks>
		/// <exception cref="System.ArgumentNullException">Si la liste de niveaux de d�tail est null</exception>
		public GenericDetailLevel(ArrayList levelIds,WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom):this(levelIds){
			_selectedFrom=selectedFrom;
		}
		/// <summary>
		/// Contructeur avec une liste de niveaux de d�tail
		/// </summary>
		/// <param name="levelIds">Liste des identifiant des �l�ments duniveau de d�tail</param>
		/// <param name="selectedFrom">Niveau S�lectionn� � partir de</param>
		/// <param name="type">Type de la liste</param>
		/// <remarks>levelIds doit contenir des int</remarks>
		/// <exception cref="System.ArgumentNullException">Si la liste de niveaux de d�tail est null</exception>
		public GenericDetailLevel(ArrayList levelIds,WebConstantes.GenericDetailLevel.SelectedFrom selectedFrom,WebConstantes.GenericDetailLevel.Type type):this(levelIds,selectedFrom){
			_type=type;
		}


		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou d�finit le type de la liste
		/// </summary>
		public WebConstantes.GenericDetailLevel.Type Type{
			get{return(_type);}
			set{_type=value;}
		}
		/// <summary>
		/// Obtient ou d�finit le type d'emplacement d'o� c'est la s�lection du niveau de d�tail
		/// </summary>
		public WebConstantes.GenericDetailLevel.SelectedFrom FromControlItem{
			get{return(_selectedFrom);}
			set{_selectedFrom=value;}
		}
		/// <summary>
		/// Obtient la liste contenant les niveaux de d�tails
		/// </summary>
		public ArrayList Levels{
			get{return(_levels);}
		}
		/// <summary>
		/// Obtient la liste des identifiants des �l�ments d'un niveaux de d�tails
		/// </summary>
		public ArrayList LevelIds{
			get{
				ArrayList levelIds=new ArrayList();
				foreach(DetailLevelItemInformation currentLevel in _levels){
					levelIds.Add(currentLevel.Id);
				}
				return(levelIds);}
		}

		/// <summary>
		/// Obtient le nombre de niveau
		/// </summary>
		public int GetNbLevels{
			get{return(_levels.Count);}
		}

        /// <summary>
        /// Obtient le niveau de d�tail
        /// </summary>
        /// <param name="rank">1 -> NbLevels</param>
        /// <returns>Niveau de d�tail</returns>
        public DetailLevelItemInformation this[int rank] {
            get {
                return ((DetailLevelItemInformation)_levels[rank-1]);
            }
        }
        
		#endregion

		#region M�thode publiques

		#region SQLGenerator
		/// <summary>
		/// Obtient le code SQL des champs correspondant aux �l�ments du niveau de d�tail
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFields(){
			string sql="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				sql+=currentLevel.GetSqlFieldId()+","+currentLevel.GetSqlField()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL des champs correspondant aux �l�ments du niveau de d�tail pour les requ�tes plurimedia.
		/// Cette fonction est utilis�e pour obtenir les champs logique pour une requ�te avec union 
		/// (les champs sont sans sp�cification de la table)
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldsWithoutTablePrefix(){
			string sql="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				sql+=currentLevel.GetSqlFieldIdWithoutTablePrefix()+","+currentLevel.GetSqlFieldWithoutTablePrefix()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}
		
		/// <summary>
		/// Obtient le code SQL de la clause order correspondant aux �l�ments du niveau de d�tail
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlOrderFields(){
			string sql="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				sql+=currentLevel.GetSqlFieldForOrder()+","+currentLevel.GetSqlIdFieldForOrder()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL de la clause order correspondant aux �l�ments du niveau de d�tail
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlOrderFieldsWithoutTablePrefix(){
			string sql="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				sql+=currentLevel.GetSqlFieldForOrderWithoutTablePrefix()+","+currentLevel.GetSqlIdFieldForOrderWithoutTablePrefix()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL de la clause group by correspondant aux �l�ments du niveau de d�tail
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlGroupByFields(){
			string sql="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				sql+=currentLevel.GetSqlIdFieldForGroupBy()+","+currentLevel.GetSqlFieldForGroupBy()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL de la clause group by correspondant aux �l�ments du niveau de d�tail
		/// </summary>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlGroupByFieldsWithoutTablePrefix(){
			string sql="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				sql+=currentLevel.GetSqlIdFieldForGroupByWithoutTablePrefix()+","+currentLevel.GetSqlFieldForGroupByWithoutTablePrefix()+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		
		/// <summary>
		/// Obtient le code SQL des tables correspondant aux �l�ments du niveau de d�tail
		/// </summary>
		/// <param name="dbSchemaName">Schema de la base de donn�es � utiliser</param>
		/// <remarks>Ne termine pas par une virgule</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlTables(string dbSchemaName){
			if(dbSchemaName==null || dbSchemaName.Length==0)throw(new ArgumentNullException("Parameter dbSchemaName is invalid"));
			string sql="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				if(currentLevel.DataBaseTableName!=null) sql+=dbSchemaName+"."+currentLevel.DataBaseTableName+" "+currentLevel.DataBaseTableNamePrefix+",";
			}
			if(sql.Length>0)sql=sql.Substring(0,sql.Length-1);
			return(sql);
		}

		/// <summary>
		/// Obtient le code SQL des jointures correspondant aux �l�ments du niveau de d�tail
		/// </summary>
		/// <remarks>D�but par And</remarks>
		/// <param name="languageId">Langue � afficher</param>
		/// <param name="dataTablePrefix">Pr�fixe de la table de donn�es sur laquelle on fait la jointure</param>
		/// <returns>Code SQL</returns>
		public string GetSqlJoins(int languageId,string dataTablePrefix){
			if(dataTablePrefix==null || dataTablePrefix.Length==0)throw(new ArgumentNullException("Parameter dataTablePrefix is invalid"));
			string sql="";
			string externalJoinSymbol="";
			foreach(DetailLevelItemInformation currentLevel in _levels){
				if(currentLevel.DataBaseTableName!=null){
					if(currentLevel.DataBaseExternalJoin)externalJoinSymbol="(+)";
					#region ancienne version
//					sql+=" and "+dataTablePrefix+"."+currentLevel.DataBaseIdField+"="+currentLevel.DataBaseTableNamePrefix+"."+currentLevel.DataBaseIdField;
//					sql+=" and "+currentLevel.DataBaseTableNamePrefix+".id_language="+languageId.ToString();
//					sql+=" and "+currentLevel.DataBaseTableNamePrefix+".activation<"+ActivationValues.UNACTIVATED;
					#endregion
					//TODO : faire valider par G.F
					sql+=" and "+currentLevel.DataBaseTableNamePrefix+"."+currentLevel.DataBaseIdField+externalJoinSymbol+"="+dataTablePrefix+"."+currentLevel.DataBaseIdField;
					sql+=" and "+currentLevel.DataBaseTableNamePrefix+".id_language"+externalJoinSymbol+"="+languageId.ToString();
					sql+=" and "+currentLevel.DataBaseTableNamePrefix+".activation"+externalJoinSymbol+"<"+ActivationValues.UNACTIVATED;
					externalJoinSymbol=""; 
				}
			}
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL des jointures correspondant aux �l�ments du niveau de d�tail entre eux
		/// </summary>
		/// <remarks>D�but par And</remarks>
		/// <param name="languageId">Langue � afficher</param>
		/// <returns>Code SQL</returns>
		public string GetSqlJoinsBetweenLevels(int languageId){
			string sql="";
			string externalJoinSymbol="";
			DetailLevelItemInformation current=null;
			DetailLevelItemInformation parent=null;
			foreach(DetailLevelItemInformation currentLevel in _levels){
				if(currentLevel.DataBaseTableName!=null){
					if(currentLevel.DataBaseExternalJoin)externalJoinSymbol="(+)";
					//TODO : faire valider par G.F
					sql+=" and "+currentLevel.DataBaseTableNamePrefix+".id_language"+externalJoinSymbol+"="+languageId.ToString();
					sql+=" and "+currentLevel.DataBaseTableNamePrefix+".activation"+externalJoinSymbol+"<"+ActivationValues.UNACTIVATED;
					externalJoinSymbol=""; 
				}
			}
			for(int i=(_levels.Count-1);i>=1;i--){
				current =(DetailLevelItemInformation)_levels[i];
				parent =(DetailLevelItemInformation)_levels[i-1];
				sql+=" and "+current.DataBaseTableNamePrefix+"."+parent.DataBaseIdField+"="+parent.DataBaseTableNamePrefix+"."+parent.DataBaseIdField;
			}

			return(sql);

		}
		#endregion

		#region Rules
		/// <summary>
		/// Obtient la valeur de l'identifiant du niveau level pour la ligne dr
		/// </summary>
		/// <param name="dr">Ligne de donn�es</param>
		/// <param name="level">Niveau de d�tail</param>
		/// <returns>Valeur de l'identifiant</returns>
		public Int64 GetIdValue(DataRow dr,int level){
			if(level>_levels.Count)return(-1);
			return(Int64.Parse(dr[this.GetColumnNameLevelId(level)].ToString()));
		}
		/// <summary>
		/// Obtient la valeur du libell� du niveau level pour la ligne dr
		/// </summary>
		/// <param name="dr">Ligne de donn�es</param>
		/// <param name="level">Niveau de d�tail</param>
		/// <returns>Valeur du libell�</returns>
		public string GetLabelValue(DataRow dr,int level){
			if(level>_levels.Count)return("");
			return(dr[this.GetColumnNameLevelLabel(level)].ToString());
		}

		/// <summary>
		/// Obtient le nom de la column contenant l'identifiant d'un �l�ment du niveau de d�tail
		/// </summary>
		/// <remarks>Les niveaux d�butent � 1</remarks>
		/// <param name="level">Niveau demand�</param>
		/// <returns>Nom de la colonne</returns>
		public string GetColumnNameLevelId(int level){
			if(level<1)throw(new ArgumentException("invalid parameter level"));
			return(((DetailLevelItemInformation)_levels[level-1]).GetIdFieldForRules());
		}
		/// <summary>
		/// Obtient le nom de la column contenant l'identifiant d'un �l�ment du niveau de d�tail
		/// </summary>
		/// <remarks>Les niveaux d�butent � 1</remarks>
		/// <param name="level">Niveau demand�</param>
		/// <returns>Nom de la colonne</returns>
		public string GetColumnNameLevelLabel(int level){
			if(level<1)throw(new ArgumentException("invalid parameter level")); 
			return(((DetailLevelItemInformation)_levels[level-1]).GetFieldForRules());
			//			if(level<1)throw(new ArgumentException("invalid parameter level"));
			//			DetailLevelItemInformation detailLevel=(DetailLevelItemInformation)_levels[level-1];
			//			if(detailLevel.DataBaseAliasField==null)return(detailLevel.DataBaseAliasField);
			//			return(detailLevel.DataBaseField);
		}
		#endregion

		#region UI
		/// <summary>
		/// Obtient le nom traduit d'un �l�ment du niveau de d�tail
		/// </summary>
		/// <param name="level">Niveau</param>
		/// <param name="languageId">Langue</param>
		/// <returns>Nom traduit</returns>
		public string GetLevelText(int level,int languageId){
			if(level<1)throw(new ArgumentException("invalid parameter level"));
			return(GestionWeb.GetWebWord(((DetailLevelItemInformation)_levels[level-1]).WebTextId,languageId));
		}

		/// <summary>
		/// Obtient le libell� du niveau de d�tail
		/// </summary>
		/// <param name="languageId">Langue</param>
		/// <returns>Libell� du niveau de d�tail</returns>
		public string GetLabel(int languageId){
			string ui="";
			for(int level=1;level<=_levels.Count;level++){
				ui+=GetLevelText(level,languageId)+@"\";
			}
			if(ui.Length>1)ui=ui.Substring(0,ui.Length-1);
			return(ui);
		}
		#endregion

		#region Tracking
		/// <summary>
		/// Iniduqe si au moins un niveau doit �tre tracker
		/// </summary>
		/// <param name="levelId">Identifiant de l'�l�ment du niveau de d�tail</param>
		/// <returns>True si oui, sinon false</returns>
		public bool ContainLevelToTrack(DetailLevelItemInformation.Levels levelId){
			foreach(DetailLevelItemInformation currentLevel in _levels){
				if(currentLevel.ToTrack && currentLevel.Id==levelId)return(true);
			}
			return(false);
		}
		#endregion

		/// <summary>
		/// Ajoute un niveau de d�tail
		/// </summary>
		/// <param name="levelId">Identifiant du niveau de d�tail</param>
		public void AddLevel(int levelId){
			_levels.Add(DetailLevelItemsInformation.Get(levelId));
		}

		/// <summary>
		/// Indique si un niveau est contenu dans le niveau de detaille
		/// </summary>
		/// <param name="levelId">Identifiant de l'�l�ment du niveau de d�tail</param>
		/// <returns>True si oui, sinon false</returns>
		public bool ContainDetailLevelItem(DetailLevelItemInformation.Levels levelId){
			foreach(DetailLevelItemInformation currentLevel in _levels){
				if(currentLevel.Id==levelId)return(true);
			}
			return(false);
		}
		
		/// <summary>
		/// Donne le niveau de l'�l�ment du niveau de d�tail 
		/// </summary>
		/// <remarks>Si le niveau n'est pas contenu la fonction retourne -1</remarks>
		/// <param name="levelId">Identifiant de l'�l�ment du niveau de d�tail</param>
		/// <returns>Niveau de l'�l�ment du niveau de d�tail </returns>
		public int DetailLevelItemLevelIndex(DetailLevelItemInformation.Levels levelId){
			int index=1;
			foreach(DetailLevelItemInformation currentLevel in _levels){
				if(currentLevel.Id==levelId)return(index);
				index++;
			}
			return(-1);
		}

		/// <summary>
		/// Obtient le niveau de d�tail
		/// </summary>
		/// <param name="rank">Rang</param>
		/// <returns>Niveau de d�tail</returns>
		public DetailLevelItemInformation.Levels GetDetailLevelItemInformation(int rank){
			return(((DetailLevelItemInformation)_levels[rank-1]).Id);
		}
		
		/// <summary>
		/// Obtient le niveau d'un �l�ment
		/// si le niveaux n'existe pas la fonction retourne 0
		/// </summary>
		/// <param name="levelId">Identifiant de l'�l�ment du niveau de d�tail</param>
		/// <returns>Niveau de l'�l�ment</returns>
		public int GetLevelRankDetailLevelItem(DetailLevelItemInformation.Levels levelId){
			int rank=0;
			foreach(DetailLevelItemInformation currentLevel in _levels){
				rank++;
				if(currentLevel.Id==levelId)return(rank);
			}
			return(0);
		}

		/// <summary>
		/// Indique si la liste en param�tre � les m�mes niveaux de d�tail
		/// </summary>
		/// <param name="toTest">Liste � tester</param>
		/// <returns>True si elle contient les m�mes niveaux, false sinon</returns>
		public bool EqualLevelItems(GenericDetailLevel toTest){
			if(this.GetNbLevels!=toTest.GetNbLevels)return(false);
			for(int i=0; i<toTest.GetNbLevels;i++){
				if(((DetailLevelItemInformation.Levels)this.LevelIds[i])!=((DetailLevelItemInformation.Levels)toTest.LevelIds[i]))return(false);
			}
			return(true);

		}

		/// <summary>
		/// Surcharge de la m�thode ToString d'Object.
		/// Cette m�thode est utilis�e pour le debugage
		/// </summary>
		/// <remarks>La langue est le fran�ais</remarks>
		/// <returns>La cha�ne repr�sentant le niveau de d�tail</returns>
		public override string ToString() {
			return(GetLabel(TNS.AdExpress.Constantes.DB.Language.FRENCH));
		}

		#endregion

	}
}
