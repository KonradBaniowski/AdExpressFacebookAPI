#region Informations
// Auteur: G. Facon
// Création: 27/03/2006
// Modification:
#endregion

using System;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.Level{


	///<summary>
	/// Description d'un niveau de détail d'un tableau
	/// </summary>
	///  <author>G.Facon</author>
	public class DetailLevelItemInformation {

		#region Enum
		/// <summary>
		/// Eléments compsant les Niveaux de détailles
		/// </summary>
		public enum Levels{
			/// <summary>
			/// Media
			/// </summary>
			vehicle=1,
			/// <summary>
			/// Catégorie
			/// </summary>
			category=2,
			/// <summary>
			/// Support
			/// </summary>
			media=3,
			/// <summary>
			/// Centre d'interet
			/// </summary>
			interestCenter=4,
			/// <summary>
			/// Régie
			/// </summary>
			mediaSeller=5,
			/// <summary>
			/// Version
			/// </summary>
			slogan=6,
			/// <summary>
			/// Groupe de société
			/// </summary>
			holdingCompany=7,
			/// <summary>
			/// Annonceur
			/// </summary>
			advertiser=8,
			/// <summary>
			/// Marque
			/// </summary>
			brand=9,
			/// <summary>
			/// Produit
			/// </summary>
			product=10,
			/// <summary>
			/// Famille
			/// </summary>
			sector=11,
			/// <summary>
			/// Classe
			/// </summary>
			subSector=12,
			/// <summary>
			/// Groupe
			/// </summary>
			group=13,
			/// <summary>
			/// Variété
			/// </summary>
			segment=14,
			/// <summary>
			/// Groupe d'agences
			/// </summary>
			groupMediaAgency=15,
			/// <summary>
			/// Agence
			/// </summary>
			agency=16,
			/// <summary>
			/// Titre
			/// </summary>
			title=17,
			/// <summary>
			/// Date
			/// </summary>
			date=18,
			/// <summary>
			/// Durée
			/// </summary>
			duration=19,
			/// <summary>
			/// Format
			/// </summary>
			format=20,
			/// <summary>
			/// Code écran
			/// </summary>
			commecialBreak=21,
			/// <summary>
			/// Format affiche
			/// </summary>
			typeBoard=22,
			/// <summary>
			/// Genre d'émission
			/// </summary>
			programType=23,
			/// <summary>
			/// Emission
			/// </summary>
			program=24,
			/// <summary>
			/// Forme de parrainage
			/// </summary>
			sponsorshipForm=25  
		}
		#endregion

		#region Variables
		/// <summary>
		/// Identitifant du niveau de détail
		/// </summary>
		private DetailLevelItemInformation.Levels _id;
		/// <summary>
		/// Nom du niveau de détail
		/// </summary>
		private string _name;
		/// <summary>
		/// Identifiant du texte du niveau de détail
		/// </summary>
		private Int64 _webTextId;
		/// <summary>
		/// Champ identifiant pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseIdField;
		/// <summary>
		/// Champ libellé logique pour l'identifiant de niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseAliasIdField=null;
		/// <summary>
		/// Champ libellé pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseField;
		/// <summary>
		/// Champ libellé logique pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseAliasField=null;
		/// <summary>
		/// Nom de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseTableName=null;
		/// <summary>
		/// Préfixe de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseTableNamePrefix=null;
		/// <summary>
		/// Le niveau doit être utilisé dans le tracking
		/// </summary>
		private bool _toTrack=false;
		/// <summary>
		/// Doit convertir la valeur null d'un identifiant en 0
		/// </summary>
		private bool _convertNullDbId=false;
		/// <summary>
		/// Doit convertir la valeur null d'un champ en 0
		/// </summary>
		private bool _convertNullDbField=false;
		/// <summary>
		/// Relation de données de type jointure externe
		/// </summary>
		private bool _dataBaseExternalJoin=false;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">Identitifant du niveau de détail</param>
		/// <param name="name">Nom du niveau de détail</param>
		/// <param name="webTextId">Identifiant du texte du niveau de détail</param>
		/// <param name="dataBaseIdField">Champ identifiant pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseField">Champ libellé pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseAliasField">Champ libellé logique pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseTableName">Nom de la table pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseTableNamePrefix">Préfixe de la table pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="toTrack">Le niveau doit être utilisé dans le tracking</param>
		/// <param name="convertNullDbId">Doit convertir la valeur null d'un identifiant en 0</param>
		/// <param name="convertNullDbField">Doit convertir la valeur null d'un champ en 0</param>
		/// <param name="dataBaseAliasIdField">Champ libellé logique pour l'identifiant de niveau de détail de la base de données AdExpress 3</param>
		///<param name="dataBaseExternalJoin">Doit être relié à un autre champ de données par une jointure externe</param>
		public DetailLevelItemInformation(int id,string name,Int64 webTextId,string dataBaseIdField,string dataBaseAliasIdField,bool convertNullDbId,string dataBaseField,string dataBaseAliasField,bool convertNullDbField,string dataBaseTableName,string dataBaseTableNamePrefix,bool toTrack,bool dataBaseExternalJoin){
			if(id<0)throw(new ArgumentException("Invalid argument id"));
			if(name==null ||name.Length<1)throw(new ArgumentException("Invalid argument name"));
			if(webTextId<0)throw(new ArgumentException("Invalid argument webTextId"));
			if(dataBaseIdField==null ||dataBaseIdField.Length<1)throw(new ArgumentException("Invalid argument dataBaseIdField"));
			if(dataBaseField==null ||dataBaseField.Length<1)throw(new ArgumentException("Invalid argument dataBaseField"));  
			try{
				_id=(DetailLevelItemInformation.Levels)id;
			}
			catch(System.Exception err){
				throw(new ArgumentException("Incorrect level Id","id",err));
			}
			_name=name;
			_webTextId=webTextId;
			_dataBaseIdField=dataBaseIdField;
			_dataBaseField=dataBaseField;
			_toTrack=toTrack;
			_convertNullDbId=convertNullDbId;
			_convertNullDbField=convertNullDbField;
			_dataBaseExternalJoin=dataBaseExternalJoin;
			if(dataBaseAliasField!=null && dataBaseAliasField.Length>0)_dataBaseAliasField=dataBaseAliasField;
			if(dataBaseAliasIdField!=null && dataBaseAliasIdField.Length>0)_dataBaseAliasIdField=dataBaseAliasIdField;
			if(dataBaseTableName!=null && dataBaseTableName.Length>0)_dataBaseTableName=dataBaseTableName;
			if(dataBaseTableNamePrefix!=null && dataBaseTableNamePrefix.Length>0)_dataBaseTableNamePrefix=dataBaseTableNamePrefix;
			if(_convertNullDbId && dataBaseAliasIdField==null)throw(new GenericDetailLevelException("Id alias have to be defined if convertNullDbId is true"));
			if(_convertNullDbField && dataBaseAliasField==null)throw(new GenericDetailLevelException("Field alias have to be defined if convertNullDbId is true"));
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient l'identitifant du niveau de détail
		/// </summary>
		public DetailLevelItemInformation.Levels Id{
			get{return(_id);}
		}
		/// <summary>
		/// Obtient le nom du niveau de détail
		/// </summary>
		public string Name{
			get{return(_name);}
		}
		/// <summary>
		/// Obtient l'identifiant du texte du niveau de détail
		/// </summary>
		public Int64 WebTextId{
			get{return(_webTextId);}
		}
		/// <summary>
		/// Obtient le champ identifiant pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseIdField{
			get{return(_dataBaseIdField);}
		}
		/// <summary>
		/// Obtient le champ libellé pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseField{
			get{return(_dataBaseField);}
		}
		/// <summary>
		/// Obtient le champ libellé logique pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseAliasField{
			get{return(_dataBaseAliasField);}
		}
		/// <summary>
		/// Obtient le champ libellé logique pour l'identifiant du niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseAliasIdField{
			get{return(_dataBaseAliasIdField);}
		}
		/// <summary>
		/// Obtient le nom de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseTableName{
			get{return(_dataBaseTableName);}
		}
		/// <summary>
		/// Obtient le préfixe de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseTableNamePrefix{
			get{return(_dataBaseTableNamePrefix);}
		}
		/// <summary>
		/// Indique si le niveau doit être utilisé dans le tracking
		/// </summary>
		public bool ToTrack{
			get{return(_toTrack);}
		}
		/// <summary>
		/// Indique si on doit convertir la valeur null d'un identifiant en 0
		/// </summary>
		public bool ConvertNullDbId{
			get{return(_convertNullDbId);}
		}
		/// <summary>
		/// Indique si on doit convertir la valeur null d'un champ en 0
		/// </summary>
		public bool ConvertNullDbField{
			get{return(_convertNullDbField);}
		}

		/// <summary>
		///Relation de données de type jointure externe
		/// </summary>
		public bool DataBaseExternalJoin{
			get{return(_dataBaseExternalJoin);}
		}
		#endregion

		#region Méthode publiques

		#region SQL
		/// <summary>
		/// Obtient le code SQL pour le champ
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlField(){
			string sql="";
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			if(_convertNullDbField)sql+="nvl("+prefix+_dataBaseField+",0)";
			else sql+=prefix+_dataBaseField;
			if(_dataBaseAliasField!=null)sql+=" as "+_dataBaseAliasField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL pour le champ sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldWithoutTablePrefix(){
			string sql="";
			if(_convertNullDbField)sql+="nvl("+_dataBaseField+",0)";
			else sql+=_dataBaseField;
			if(_dataBaseAliasField!=null)sql+=" as "+_dataBaseAliasField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL pour l'identifiant du champ
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldId(){
			string sql="";
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			if(_convertNullDbId)sql+="nvl("+prefix+_dataBaseIdField+",0)";
			else sql+=prefix+_dataBaseIdField;
			if(_dataBaseAliasIdField!=null)sql+=" as "+_dataBaseAliasIdField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL pour l'identifiant du champ sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldIdWithoutTablePrefix(){
			string sql="";
			if(_convertNullDbField)sql+="nvl("+_dataBaseIdField+",0)";
			else sql+=_dataBaseIdField;
			if(_dataBaseAliasIdField!=null)sql+=" as "+_dataBaseAliasIdField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForOrder(){
			if(_dataBaseAliasField!=null)return(_dataBaseAliasField);
			return(_dataBaseTableNamePrefix+"."+_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande order sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForOrderWithoutTablePrefix(){
			if(_dataBaseAliasField!=null)return(_dataBaseAliasField);
			return(_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForOrder(){
			if(_dataBaseAliasIdField!=null)return(_dataBaseAliasIdField);
			return(_dataBaseTableNamePrefix+"."+_dataBaseIdField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande order sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForOrderWithoutTablePrefix(){
			if(_dataBaseAliasIdField!=null)return(_dataBaseAliasIdField);
			return(_dataBaseIdField);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande Group By sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForGroupBy(){
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			return(prefix+_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande Group By
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForGroupByWithoutTablePrefix(){
			return(_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande Group By
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForGroupBy(){
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			return(prefix+_dataBaseIdField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande Group By sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForGroupByWithoutTablePrefix(){
			return(_dataBaseIdField);
		}

		#endregion

		#region Rules
		/// <summary>
		/// Obtient le libélle du le champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetFieldForRules(){
			if(_dataBaseAliasField!=null)return(_dataBaseAliasField);
			return(_dataBaseField);
		}
		/// <summary>
		/// Obtient le libélle de l'identifiant du le champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetIdFieldForRules(){
			if(_dataBaseAliasIdField!=null)return(_dataBaseAliasIdField);
			return(_dataBaseIdField);
		}
		#endregion

		/// <summary>
		/// Obtient le nom du champ avec le nom logique (s'il existe)
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Nom du champ avec le nom logique</returns>
		public string GetTableNameWithPrefix(){
			string tmp="";
			if(_dataBaseTableName==null)return(null);
			tmp+=_dataBaseTableName+" ";
			if(_dataBaseTableNamePrefix!=null)tmp+=_dataBaseTableNamePrefix+" ";
			return(tmp);
		}

		#endregion

	}
}
